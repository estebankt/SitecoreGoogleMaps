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
    /// Represents a marker on a map.
    /// </summary>
    [DataContract]
    public class GMarker
    {
        /// <summary>
        /// Latitude and longitude position of the marker on the map. Required.
        /// </summary>
        [DataMember]
        public GLatLng Position { get; set; }

        /// <summary>
        /// Title, displayed as tooltip on the marker. 
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Optional info window that is displayed when the marker is clicked. Can contain HTML. 
        /// </summary>
        [DataMember]
        public string InfoWindow { get; set; }

        /// <summary>
        /// Optional custom icon that is used on the map. A red default icon is used if this is not set. 
        /// </summary>
        [DataMember]
        public GIcon CustomIcon { get; set; }
    }
}