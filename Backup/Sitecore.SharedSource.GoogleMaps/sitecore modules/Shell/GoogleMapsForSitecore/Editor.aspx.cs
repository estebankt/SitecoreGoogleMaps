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
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Data.Fields;
using Sitecore.SharedSource.GoogleMaps.Gmaps;
using Sitecore.Data;

namespace Sitecore.SharedSource.GoogleMaps
{
    public partial class Editor : System.Web.UI.Page
    {
        /// <summary>
        /// Zoom level of the map
        /// </summary>
        public int CurrentZoom { get; set; }

        /// <summary>
        /// Position of all markers on the map
        /// </summary>
        public List<GLatLng> Coordinates { get; set; }

        /// <summary>
        /// Initial map? no marker position set yet
        /// </summary>
        public bool IsDefaultMap { get { return (Coordinates == null || Coordinates.Count() == 0); } }

        /// <summary>
        /// Are we editing a line or polygon?
        /// </summary>
        public bool EditMultiplePoints
        {
            get
            {
                return (HttpContext.Current.Request.QueryString["multiplePoints"] != null && HttpContext.Current.Request.QueryString["multiplePoints"] == "true");
            }
        }

        /// <summary>
        /// Initial position of the map
        /// </summary>
        public GLatLng StartPoint
        {
            get
            {
                if (IsDefaultMap)
                {
                    return new GLatLng(Utilities.EnsureLongLat(SettingsItem.Fields["Latitude"].Value), Utilities.EnsureLongLat(SettingsItem.Fields["Longitude"].Value));
                }
                else
                {
                    return Coordinates.First();
                }
            }
        }

        /// <summary>
        /// Sitecore Master Database
        /// </summary>
        private Database _masterDB;
        private Database MasterDB
        {
            get
            {
                return _masterDB != null ? _masterDB : _masterDB = Sitecore.Configuration.Factory.GetDatabase("master");
            }
        }

        /// <summary>
        /// The item in Sitecore containing the marker, line or polygon and currently being edited
        /// </summary>
        private Item _currentItem;
        private Item CurrentItem
        {
            get
            {
                return _currentItem != null ? _currentItem : _currentItem = MasterDB.GetItem(HttpContext.Current.Request.QueryString["id"], Language.Parse(HttpContext.Current.Request.QueryString["language"]), new Sitecore.Data.Version(HttpContext.Current.Request.QueryString["v"]));
            }
        }

        /// <summary>
        /// The Sitecore item containing all basic settings for the Google Maps module. The Sitecore path is defined in the configuration file.
        /// </summary>
        private Item _settingsItem;
        private Item SettingsItem
        {
            get
            {
                return _settingsItem != null ? _settingsItem : _settingsItem = MasterDB.GetItem(Settings.SitecoreSettingsPath, Language.Parse("en"));
            }
        }

        /// <summary>
        /// Initialize the editor
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            if (!Page.IsPostBack)
            {
                PopulateCoordinates();
                Utilities.EnsureField(SettingsItem, "Initial Zoom Level", true);
                CurrentZoom = Utilities.EnsureZoomLevel(SettingsItem.Fields["Initial Zoom Level"].Value);
                txtZoom.Text = CurrentZoom.ToString();
            }
            else
            {
                CurrentZoom = Utilities.EnsureZoomLevel(txtZoom.Text);
            }

        }

        /// <summary>
        /// Save marker and zoom level back into the current Sitecore item
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            CurrentZoom = Utilities.EnsureZoomLevel(txtZoom.Text);

            if (CurrentItem == null)
                return;

            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                CurrentItem.Editing.BeginEdit();

                if (CurrentItem.Fields["Initial Zoom Level"] != null)
                {
                    CurrentItem.Fields["Initial Zoom Level"].Value = CurrentZoom.ToString();
                }

                if (EditMultiplePoints && CurrentItem.Fields["Points"] != null)
                {
                    CurrentItem.Fields["Points"].Value = txtMultiple.Text;
                }

                if (!EditMultiplePoints && !string.IsNullOrEmpty(txtLat.Text) && !string.IsNullOrEmpty(txtLong.Text)
                    && CurrentItem.Fields["Latitude"] != null && CurrentItem.Fields["Longitude"] != null)
                {
                    GLatLng latLng = new GLatLng(Utilities.EnsureLongLat(txtLat.Text), Utilities.EnsureLongLat(txtLong.Text));
                    CurrentItem.Fields["Latitude"].Value = latLng.Latitude.ToString();
                    CurrentItem.Fields["Longitude"].Value = latLng.Longitude.ToString();
                }

                CurrentItem.Editing.EndEdit();
            }

            PopulateCoordinates();
        }

        /// <summary>
        /// Parse content of the current Sitecore item to display on the map
        /// </summary>
        private void PopulateCoordinates()
        {
            Coordinates = new List<GLatLng>();
            if (EditMultiplePoints)
            {
                if (CurrentItem.Fields["Points"] != null && !string.IsNullOrEmpty(CurrentItem.Fields["Points"].Value))
                {
                    var points = ((MultilistField)CurrentItem.Fields["Points"]).Items;
                    foreach (string point in points)
                    {
                        var parts = point.Split(',');
                        if (parts.Length == 2)
                        {
                            Coordinates.Add(new GLatLng(parts[0], parts[1]));
                        }
                    }
                    if (Coordinates.Count() > 0)
                    {
                        txtLat.Text = Coordinates.First().Latitude.ToString();
                        txtLong.Text = Coordinates.First().Longitude.ToString();
                        txtMultiple.Text = CurrentItem.Fields["Points"].Value;
                    }
                }
            }
            else
            {
                if (CurrentItem.Fields["Longitude"] != null && CurrentItem.Fields["Latitude"] != null && !string.IsNullOrEmpty(CurrentItem.Fields["Longitude"].Value) && !string.IsNullOrEmpty(CurrentItem.Fields["Latitude"].Value))
                {
                    GLatLng latLng = new GLatLng(Utilities.EnsureLongLat(CurrentItem.Fields["Latitude"].Value), Utilities.EnsureLongLat(CurrentItem.Fields["Longitude"].Value));
                    Coordinates.Add(latLng);
                    txtLat.Text = Coordinates.First().Latitude.ToString();
                    txtLong.Text = Coordinates.First().Longitude.ToString();
                }
            }
        }

    }
}