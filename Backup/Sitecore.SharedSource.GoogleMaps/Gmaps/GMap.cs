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
using System.Runtime.Serialization;

namespace Sitecore.SharedSource.GoogleMaps.Gmaps

{    
    /// <summary>
    /// Represents a map. Contains all settings and the markers, lines and polygons to display on the map.
    /// </summary>
    [DataContract]
    public class GMap
    {
        /// <summary>
        /// HTML ID of the div canvas that holds the map. Dynamic to support multiple maps on the same page. Required. 
        /// </summary>
        [DataMember]
        public string CanvasID { get; set; }

        /// <summary>
        /// Initial zoom level. Required.
        /// </summary>
        [DataMember]
        public int Zoom { get; set; }

        /// <summary>
        /// Initial position of the map. Required. 
        /// </summary>
        [DataMember]
        public GLatLng Center { get; set; }

        /// <summary>
        /// Allowed map display types, supplied as JavaScript enum used by the Google Maps API.
        /// The first map type will be the initial map type of the map. 
        /// </summary>
        [DataMember]
        public List<string> MapTypes { get; set; }

        /// <summary>
        /// Width of the map canvas. Required. 
        /// </summary>
        [DataMember]
        public int Width { get; set; }

        /// <summary>
        /// Height of the map canvas. Required. 
        /// </summary>
        [DataMember]
        public int Height { get; set; }

        /// <summary>
        /// Optional list of markers to display on the map. 
        /// </summary>
        [DataMember]
        public List<GMarker> Markers { get; set; }

        /// <summary>
        /// Optional list of lines to display on the map. 
        /// </summary>
        [DataMember]
        public List<GLine> Lines { get; set; }

        /// <summary>
        /// Optional list of polygons to display on the map. 
        /// </summary>
        [DataMember]
        public List<GPolygon> Polygons { get; set; }
        
        /// <summary>
        /// Optional custom cursor to use on a draggable item.
        /// </summary>
        [DataMember]
        public string DraggableCursor { get; set; }

        /// <summary>
        /// Optional custom cursor to use on a dragging item.
        /// </summary>
        [DataMember]
        public string DraggingCursor { get; set; }

        /// <summary>
        /// Optional maximum zoom level.
        /// </summary>
        [DataMember]
        public int? MaxZoomLevel { get; set; }
        
        /// <summary>
        /// Optional minimum zoom level.
        /// </summary>
        [DataMember]
        public int? MinZoomLevel { get; set; }

        /// <summary>
        /// Enable double click to zoom feature. 
        /// </summary>
        [DataMember]
        public bool EnableDoubleClickZoom { get; set; }

        /// <summary>
        /// Enable dragging of the map (map position is fixed if this is set to false). 
        /// </summary>
        [DataMember]
        public bool EnableDragging { get; set; }        

        /// <summary>
        /// Enable zooming via mouse scroll wheel.
        /// </summary>
        [DataMember]
        public bool EnableScrollWheelZoom { get; set; }

        /// <summary>
        /// Enable keyboard functionality (zoom, pan)
        /// </summary>
        [DataMember]
        public bool EnableKeyboardFunctionality { get; set; }

        /// <summary>
        /// Disable all UI controls on the map (can be turned on individually). 
        /// </summary>
        [DataMember]
        public bool DisableAllDefaultUIElements { get; set; }

        /// <summary>
        /// Enable the map overview (to show the map position in a lower zoom level)
        /// </summary>
        [DataMember]
        public bool EnableOverview { get; set; }

        /// <summary>
        /// Enable the pan control to move the map. 
        /// </summary>
        [DataMember]
        public bool EnablePanControl { get; set; }

        /// <summary>
        /// Enable the scale control to see the map scaling. 
        /// </summary>
        [DataMember]
        public bool EnableScaleControl { get; set; }

        /// <summary>
        /// Enable Google street view on the map.
        /// </summary>
        [DataMember]
        public bool EnableStreetViewControl { get; set; }

        /// <summary>
        /// Enable the zoom control UI on the map. 
        /// </summary>
        [DataMember]
        public bool EnableZoomControl { get; set; }        

    }
}