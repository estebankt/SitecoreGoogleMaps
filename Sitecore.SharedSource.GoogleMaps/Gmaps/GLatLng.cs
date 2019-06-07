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
    /// Represents a position on a map given a latitude and longitude.
    /// </summary>
    [DataContract]
    public class GLatLng
    {
        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        public GLatLng()
        { 
        }

        public GLatLng(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public GLatLng(string latitude, string longitude)
        {
            Latitude = Utilities.EnsureLongLat(latitude);
            Longitude = Utilities.EnsureLongLat(longitude);
        }

    }
}