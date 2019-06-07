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
    /// Represents a polygon area on the map. The points making up the polygon do not have to be closed - the last 
    /// line between the first and last point is added by the Google Maps API.
    /// </summary>
    [DataContract]
    public class GPolygon : GLine
    {
        /// <summary>
        /// Color used to fill the polygon - entered as HTML like "#00FF00"
        /// </summary>
        [DataMember]
        public string FillColor { get; set; }

        /// <summary>
        /// Opacity of the fill - between 0.0 and 1.0
        /// </summary>
        [DataMember]
        public double FillOpacity { get; set; }

        /// <summary>
        /// Sets the polygon as clickable.
        /// </summary>
        [DataMember]
        public bool Clickable { get; set; }

        /// <summary>
        /// Content of the info window to show when the polygon is clickable. Can contain HTML. 
        /// </summary>
        [DataMember]
        public string InfoWindow { get; set; }
    }
}