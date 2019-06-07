/********************************************************************************************** 
* Product: Google Maps Module
* Author : IE Agency (http://www.ie.com.au) - Heiko Franz
* Purpose: Google Maps to be controlled from within Sitecore
* Status : Published
*
* This is a Sitecore shared source module with Sitecore and IE both not liable for the use
* of this code, please refer to the license information:
* http://sdn.sitecore.net/Resources/Shared%20Source/Shared%20Source%20License.aspx
**********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Globalization;
using Sitecore.SharedSource.GoogleMaps.Gmaps;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using Sitecore.Globalization;
using Sitecore.Resources.Media;

namespace Sitecore.SharedSource.GoogleMaps.Sublayouts
{
    /// <summary>
    /// Displays a Google Map in a page using a Sitecore item as data source.
    /// </summary>
    public partial class GoogleMap : BaseSublayout
    {

        /// <summary>
        /// The map to display. Contains all settings, markers, lines and polygons. 
        /// </summary>
        public GMap CurrentMap { get; set; }

		/// <summary>
		/// The Api Key. 
		/// </summary>
		public string ApiKey { get; set; }

		protected void Page_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            using (new Sitecore.Diagnostics.ProfileSection("Google Maps Initializer"))
            {
                InitializeMapData();
            }
        }

        /// <summary>
        /// Initialize the map
        /// </summary>
        public void InitializeMapData()
        {
            // populate map data
            var db = Sitecore.Context.Database;

            if (DataSource == null || DataSource.TemplateName != Settings.GoogleMapTemplateName)
            {
                Log.Error("Google Maps Module - could not find the data source to display the map", Settings.LoggingOwner);
                return;
            }

            try
            {
				//get api key
				ApiKey =  Settings.ApiKey;

				// get map settings
				CurrentMap = new GMap();
                CurrentMap.CanvasID = "map_canvas" + ClientID;

                // EnsureField checks whether a field is present on an item and logs errors
                Utilities.EnsureField(DataSource, "Latitude", true);
                Utilities.EnsureField(DataSource, "Longitude", true);
                CurrentMap.Center = new GLatLng(DataSource.Fields["Latitude"].Value, DataSource.Fields["Longitude"].Value);

                Utilities.EnsureField(DataSource, "Initial Zoom Level", false);
                CurrentMap.Zoom = int.Parse(DataSource.Fields["Initial Zoom Level"].Value);

                Utilities.EnsureField(DataSource, "Width", false);
                Utilities.EnsureField(DataSource, "Height", false);
                CurrentMap.Width = int.Parse(DataSource.Fields["Width"].Value);
                CurrentMap.Height = int.Parse(DataSource.Fields["Height"].Value);

                Utilities.EnsureField(DataSource, "Map Types", false);
                CurrentMap.MapTypes = new List<string>();
                MultilistField mapTypes = ((MultilistField)DataSource.Fields["Map Types"]);
                foreach (ID mapTypeId in mapTypes.TargetIDs)
                {
                    Item mapTypeItem = db.GetItem(mapTypeId);
                    Utilities.EnsureField(mapTypeItem, "Type", true);
                    CurrentMap.MapTypes.Add(mapTypeItem.Fields["Type"].Value);
                }

                Utilities.EnsureField(DataSource, "Draggable Cursor", false);
                CurrentMap.DraggableCursor = DataSource.Fields["Draggable Cursor"].Value;

                Utilities.EnsureField(DataSource, "Dragging Cursor", false);
                CurrentMap.DraggingCursor = DataSource.Fields["Dragging Cursor"].Value;

                int tmpval;
                Utilities.EnsureField(DataSource, "Min Zoom Level", false);
                if (int.TryParse(DataSource.Fields["Min Zoom Level"].Value, out tmpval))
                {
                    CurrentMap.MinZoomLevel = tmpval;
                }
                else
                {
                    CurrentMap.MinZoomLevel = null;
                }

                Utilities.EnsureField(DataSource, "Max Zoom Level", false);
                if (int.TryParse(DataSource.Fields["Max Zoom Level"].Value, out tmpval))
                {
                    CurrentMap.MaxZoomLevel = tmpval;
                }
                else
                {
                    CurrentMap.MaxZoomLevel = null;
                }

                Utilities.EnsureField(DataSource, "Enable Double Click Zoom", false);
                CurrentMap.EnableDoubleClickZoom = ((CheckboxField)DataSource.Fields["Enable Double Click Zoom"]).Checked;

                Utilities.EnsureField(DataSource, "Enable Dragging", false);
                CurrentMap.EnableDragging = ((CheckboxField)DataSource.Fields["Enable Dragging"]).Checked;

                Utilities.EnsureField(DataSource, "Enable Scroll Wheel Zoom", false);
                CurrentMap.EnableScrollWheelZoom = ((CheckboxField)DataSource.Fields["Enable Scroll Wheel Zoom"]).Checked;

                Utilities.EnsureField(DataSource, "Enable Keyboard Functionality", false);
                CurrentMap.EnableKeyboardFunctionality = ((CheckboxField)DataSource.Fields["Enable Keyboard Functionality"]).Checked;

                Utilities.EnsureField(DataSource, "Disable All Default UI Elements", false);
                CurrentMap.DisableAllDefaultUIElements = ((CheckboxField)DataSource.Fields["Disable All Default UI Elements"]).Checked;

                Utilities.EnsureField(DataSource, "Enable Overview", false);
                CurrentMap.EnableOverview = ((CheckboxField)DataSource.Fields["Enable Overview"]).Checked;

                Utilities.EnsureField(DataSource, "Enable Pan Control", false);
                CurrentMap.EnablePanControl = ((CheckboxField)DataSource.Fields["Enable Pan Control"]).Checked;

                Utilities.EnsureField(DataSource, "Enable Scale Control", false);
                CurrentMap.EnableScaleControl = ((CheckboxField)DataSource.Fields["Enable Scale Control"]).Checked;

                Utilities.EnsureField(DataSource, "Enable Street View Control", false);
                CurrentMap.EnableStreetViewControl = ((CheckboxField)DataSource.Fields["Enable Street View Control"]).Checked;

                Utilities.EnsureField(DataSource, "Enable Zoom Control", false);
                CurrentMap.EnableZoomControl = ((CheckboxField)DataSource.Fields["Enable Zoom Control"]).Checked;

                // get all markers
                CurrentMap.Markers = new List<GMarker>();
                CurrentMap.Lines = new List<GLine>();
                CurrentMap.Polygons = new List<GPolygon>();

                if (DataSource.HasChildren)
                {
                    foreach (Item childElement in DataSource.Children)
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

                            CurrentMap.Markers.Add(marker);
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

                            CurrentMap.Lines.Add(line);
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

                            CurrentMap.Polygons.Add(poly);
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
        }

    }
}