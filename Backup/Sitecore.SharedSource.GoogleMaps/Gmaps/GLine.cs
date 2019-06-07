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
    /// Represents a line on a map, consists of a list of GLatLng points and additional settings
    /// </summary>
    [DataContract]
    public class GLine
    {
        /// <summary>
        /// List of points that make up the line on the map when connected
        /// </summary>
        [DataMember]
        public List<GLatLng> Points { get; set; }

        /// <summary>
        /// Colour used to display the outline - defined in HTML format e.g. "#FF0000"
        /// </summary>
        [DataMember]
        public string StrokeColor { get; set; }

        /// <summary>
        /// Opacity of the outline - between 0.0 and 1.0
        /// </summary>
        [DataMember]
        public double StrokeOpacity { get; set; }

        /// <summary>
        /// Weight of the outline in pixel
        /// </summary>
        [DataMember]
        public int StrokeWeight { get; set; }

    }
}