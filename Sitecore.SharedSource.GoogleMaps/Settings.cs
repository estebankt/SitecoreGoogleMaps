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

namespace Sitecore.SharedSource.GoogleMaps
{
    /// <summary>
    /// Static class to provide access to the settings of the module
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Sitecore path of the settings item for the module
        /// </summary>
        public static string SitecoreSettingsPath = Sitecore.Configuration.Settings.GetSetting("GoogleMapForSitecore.SitecoreSettingsPath", "/sitecore/system/Modules/Google Maps Settings/Settings");

        /// <summary>
        /// Name of the template for a map
        /// </summary>
        public static string GoogleMapTemplateName = Sitecore.Configuration.Settings.GetSetting("GoogleMapForSitecore.GoogleMapTemplateName", "Google Map");

        /// <summary>
        /// Name of the template for a location marker
        /// </summary>
        public static string LocationTemplateName = Sitecore.Configuration.Settings.GetSetting("GoogleMapForSitecore.LocationTemplateName", "Location");

        /// <summary>
        /// Name of the template for a line
        /// </summary>
        public static string LineTemplateName = Sitecore.Configuration.Settings.GetSetting("GoogleMapForSitecore.LineTemplateName", "Line");

        /// <summary>
        /// Name of the template for a polygon
        /// </summary>
        public static string PolygonTemplateName = Sitecore.Configuration.Settings.GetSetting("GoogleMapForSitecore.PolygonTemplateName", "Polygon");

        /// <summary>
        /// Name of owner of log entries
        /// </summary>
        public static string LoggingOwner = Sitecore.Configuration.Settings.GetSetting("GoogleMapForSitecore.LoggingOwner", "Google Maps Control");

        /// <summary>
        /// relative URL to the map element editor page
        /// </summary>
        public static string EditorURL = Sitecore.Configuration.Settings.GetSetting("GoogleMapForSitecore.EditorURL", "/sitecore modules/Shell/GoogleMapsForSitecore/Editor.aspx");

		/// <summary>
		/// Name of the template for a line
		/// </summary>
		public static string ApiKey = Sitecore.Configuration.Settings.GetSetting("GoogleMapForSitecore.ApiKey", "");

	}
}