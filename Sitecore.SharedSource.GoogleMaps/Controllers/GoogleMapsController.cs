using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.SharedSource.GoogleMaps.Gmaps;
using Sitecore.SharedSource.GoogleMaps.Models;
using Newtonsoft.Json;

namespace Sitecore.SharedSource.GoogleMaps.Controllers
{
	public class GoogleMapsController : Controller
	{

		/// <summary>
		/// The Api Key. 
		/// </summary>
		
		public ActionResult GoogleMapsResult()
		{
			var model = new GoogleMapsDto();
			model.Initialize(RenderingContext.Current.Rendering);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

			using (new Sitecore.Diagnostics.ProfileSection("Google Maps Initializer"))
			{
				InitializeMapData(model);
			}

			model.JsonModel = Utilities.JSON(model.CurrentMap);
			var result = JsonConvert.SerializeObject(new { root = model.JsonModel });

			

			return View("/Views/GoogleMaps.cshtml", model);
		}

		public GoogleMapsDto InitializeMapData(GoogleMapsDto model)
		{

			var datasource = model.Item ?? model.PageItem;

			// populate map data
			var db = Sitecore.Context.Database;

			if (datasource == null || datasource.TemplateName != Settings.GoogleMapTemplateName)
			{
				Log.Error("Google Maps Module - could not find the data source to display the map", Settings.LoggingOwner);
				return model;
			}

			try
			{
				//get api key
				model.ApiKey = Settings.ApiKey;

				// get map settings
				model.CurrentMap = new GMap();
				model.CurrentMap.CanvasID = "map_canvas" + Guid.NewGuid();

				// EnsureField checks whether a field is present on an item and logs errors
				Utilities.EnsureField(datasource, "Latitude", true);
				Utilities.EnsureField(datasource, "Longitude", true);
				model.CurrentMap.Center = new GLatLng(datasource.Fields["Latitude"].Value, datasource.Fields["Longitude"].Value);

				Utilities.EnsureField(datasource, "Initial Zoom Level", false);
				model.CurrentMap.Zoom = int.Parse(datasource.Fields["Initial Zoom Level"].Value);

				Utilities.EnsureField(datasource, "Width", false);
				Utilities.EnsureField(datasource, "Height", false);
				model.CurrentMap.Width = int.Parse(datasource.Fields["Width"].Value);
				model.CurrentMap.Height = int.Parse(datasource.Fields["Height"].Value);

				Utilities.EnsureField(datasource, "Map Types", false);
				model.CurrentMap.MapTypes = new List<string>();
				MultilistField mapTypes = ((MultilistField)datasource.Fields["Map Types"]);
				foreach (ID mapTypeId in mapTypes.TargetIDs)
				{
					Item mapTypeItem = db.GetItem(mapTypeId);
					Utilities.EnsureField(mapTypeItem, "Type", true);
					model.CurrentMap.MapTypes.Add(mapTypeItem.Fields["Type"].Value);
				}

				Utilities.EnsureField(datasource, "Draggable Cursor", false);
				model.CurrentMap.DraggableCursor = datasource.Fields["Draggable Cursor"].Value;

				Utilities.EnsureField(datasource, "Dragging Cursor", false);
				model.CurrentMap.DraggingCursor = datasource.Fields["Dragging Cursor"].Value;

				int tmpval;
				Utilities.EnsureField(datasource, "Min Zoom Level", false);
				if (int.TryParse(datasource.Fields["Min Zoom Level"].Value, out tmpval))
				{
					model.CurrentMap.MinZoomLevel = tmpval;
				}
				else
				{
					model.CurrentMap.MinZoomLevel = null;
				}

				Utilities.EnsureField(datasource, "Max Zoom Level", false);
				if (int.TryParse(datasource.Fields["Max Zoom Level"].Value, out tmpval))
				{
					model.CurrentMap.MaxZoomLevel = tmpval;
				}
				else
				{
					model.CurrentMap.MaxZoomLevel = null;
				}

				Utilities.EnsureField(datasource, "Enable Double Click Zoom", false);
				model.CurrentMap.EnableDoubleClickZoom = ((CheckboxField)datasource.Fields["Enable Double Click Zoom"]).Checked;

				Utilities.EnsureField(datasource, "Enable Dragging", false);
				model.CurrentMap.EnableDragging = ((CheckboxField)datasource.Fields["Enable Dragging"]).Checked;

				Utilities.EnsureField(datasource, "Enable Scroll Wheel Zoom", false);
				model.CurrentMap.EnableScrollWheelZoom = ((CheckboxField)datasource.Fields["Enable Scroll Wheel Zoom"]).Checked;

				Utilities.EnsureField(datasource, "Enable Keyboard Functionality", false);
				model.CurrentMap.EnableKeyboardFunctionality = ((CheckboxField)datasource.Fields["Enable Keyboard Functionality"]).Checked;

				Utilities.EnsureField(datasource, "Disable All Default UI Elements", false);
				model.CurrentMap.DisableAllDefaultUIElements = ((CheckboxField)datasource.Fields["Disable All Default UI Elements"]).Checked;

				Utilities.EnsureField(datasource, "Enable Overview", false);
				model.CurrentMap.EnableOverview = ((CheckboxField)datasource.Fields["Enable Overview"]).Checked;

				Utilities.EnsureField(datasource, "Enable Pan Control", false);
				model.CurrentMap.EnablePanControl = ((CheckboxField)datasource.Fields["Enable Pan Control"]).Checked;

				Utilities.EnsureField(datasource, "Enable Scale Control", false);
				model.CurrentMap.EnableScaleControl = ((CheckboxField)datasource.Fields["Enable Scale Control"]).Checked;

				Utilities.EnsureField(datasource, "Enable Street View Control", false);
				model.CurrentMap.EnableStreetViewControl = ((CheckboxField)datasource.Fields["Enable Street View Control"]).Checked;

				Utilities.EnsureField(datasource, "Enable Zoom Control", false);
				model.CurrentMap.EnableZoomControl = ((CheckboxField)datasource.Fields["Enable Zoom Control"]).Checked;

				// get all markers
				model.CurrentMap.Markers = new List<GMarker>();
				model.CurrentMap.Lines = new List<GLine>();
				model.CurrentMap.Polygons = new List<GPolygon>();

				if (datasource.HasChildren)
				{
					foreach (Item childElement in datasource.Children)
					{
						if (childElement.TemplateName == Settings.LocationTemplateName)
						{
							GMarker marker = new GMarker();

							Utilities.EnsureField(childElement, "Latitude", true);
							Utilities.EnsureField(childElement, "Longitude", true);
							marker.Position = new GLatLng(childElement.Fields["Latitude"].Value, childElement.Fields["Longitude"].Value);
							marker.Title = childElement.Name;
							Utilities.EnsureField(childElement, "Text", false);
							marker.InfoWindow = childElement.Fields["Text"].Value;

							// check if marker has a custom icon                            
							if (childElement.Fields["Custom Icon"] != null && childElement.Fields["Custom Icon"].HasValue && ((ReferenceField)childElement.Fields["Custom Icon"]).TargetItem != null)
							{
								Item customIcon = ((ReferenceField)childElement.Fields["Custom Icon"]).TargetItem;
								GIcon icon = new GIcon();
								Utilities.EnsureField(customIcon, "Image", true);
								icon.ImageURL = Sitecore.StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(((Sitecore.Data.Fields.ImageField)customIcon.Fields["Image"]).MediaItem));
								icon.ImageDimensions = Utilities.GetMediaItemSize((Sitecore.Data.Fields.ImageField)customIcon.Fields["Image"]);

								Utilities.EnsureField(customIcon, "Anchor", false);
								icon.Anchor = Utilities.StringToPoint(customIcon.Fields["Anchor"].Value);

								Utilities.EnsureField(customIcon, "Shadow Image", false);
								if (customIcon.Fields["Shadow Image"].HasValue && ((Sitecore.Data.Fields.ImageField)customIcon.Fields["Shadow Image"]).MediaItem != null)
								{
									icon.ShadowURL = Sitecore.StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(((Sitecore.Data.Fields.ImageField)customIcon.Fields["Shadow Image"]).MediaItem));
									icon.ShadowDimensions = Utilities.GetMediaItemSize((Sitecore.Data.Fields.ImageField)customIcon.Fields["Shadow Image"]);
									Utilities.EnsureField(customIcon, "Shadow Anchor", false);
									icon.ShadowAnchor = Utilities.StringToPoint(customIcon.Fields["Shadow Anchor"].Value);
								}

								Utilities.EnsureField(customIcon, "Clickable Polygon", false);
								if (customIcon.Fields["Clickable Polygon"].HasValue && !string.IsNullOrEmpty(customIcon.Fields["Clickable Polygon"].Value))
								{
									icon.ClickablePolygon = customIcon.Fields["Clickable Polygon"].Value;
								}

								marker.CustomIcon = icon;
							}

							model.CurrentMap.Markers.Add(marker);
						}
						else if (childElement.TemplateName == Settings.LineTemplateName)
						{
							GLine line = new GLine();
							Utilities.EnsureField(childElement, "Stroke Color", false);
							line.StrokeColor = childElement.Fields["Stroke Color"].Value;

							Utilities.EnsureField(childElement, "Stroke Opacity", false);
							line.StrokeOpacity = Utilities.EnsureOpacity(childElement.Fields["Stroke Opacity"].Value);

							Utilities.EnsureField(childElement, "Stroke Weight", false);
							line.StrokeWeight = Utilities.EnsureWeight(childElement.Fields["Stroke Weight"].Value);

							Utilities.EnsureField(childElement, "Points", false);
							var points = ((MultilistField)childElement.Fields["Points"]).Items;
							line.Points = Utilities.ArrayToLatLngList(points);

							model.CurrentMap.Lines.Add(line);
						}
						else if (childElement.TemplateName == Settings.PolygonTemplateName)
						{
							GPolygon poly = new GPolygon();
							Utilities.EnsureField(childElement, "Stroke Color", false);
							poly.StrokeColor = childElement.Fields["Stroke Color"].Value;

							Utilities.EnsureField(childElement, "Stroke Opacity", false);
							poly.StrokeOpacity = Utilities.EnsureOpacity(childElement.Fields["Stroke Opacity"].Value);

							Utilities.EnsureField(childElement, "Stroke Weight", false);
							poly.StrokeWeight = Utilities.EnsureWeight(childElement.Fields["Stroke Weight"].Value);

							Utilities.EnsureField(childElement, "Points", false);
							var points = ((MultilistField)childElement.Fields["Points"]).Items;
							poly.Points = Utilities.ArrayToLatLngList(points);

							Utilities.EnsureField(childElement, "Fill Color", false);
							poly.FillColor = childElement.Fields["Fill Color"].Value;

							Utilities.EnsureField(childElement, "Fill Opacity", false);
							poly.FillOpacity = Utilities.EnsureOpacity(childElement.Fields["Fill Opacity"].Value);

							Utilities.EnsureField(childElement, "Is Clickable", false);
							poly.Clickable = ((CheckboxField)childElement.Fields["Is Clickable"]).Checked;

							Utilities.EnsureField(childElement, "Text", false);
							poly.InfoWindow = childElement.Fields["Text"].Value;

							model.CurrentMap.Polygons.Add(poly);
						}
					}
				}
			}
			catch (FieldValidationException)
			{
				// this has been logged already - nothing to do here. 
			}
			catch (Exception ex)
			{
				Log.Error("Google Maps Module - could not initialize the map", ex, Settings.LoggingOwner);
			}

			return model;
		}

	}
}