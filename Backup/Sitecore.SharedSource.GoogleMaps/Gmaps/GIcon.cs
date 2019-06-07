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
    /// Represents a point on an image with its X and Y coordinates.
    /// </summary>
    [DataContract]
    public class Point
    {
        [DataMember]
        public int X { get; set; }
        
        [DataMember]
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Represents image dimensions by with and height.
    /// </summary>
    [DataContract]
    public class Dimensions
    {
        [DataMember]
        public int Width { get; set; }

        [DataMember] 
        public int Height { get; set; }

        public Dimensions(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }    

    /// <summary>
    /// Represents a custom icon for a marker. 
    /// </summary>
    [DataContract]
    public class GIcon
    {
        /// <summary>
        /// URL to the icon. PNG format preferred. Required. 
        /// </summary>
        [DataMember]
        public string ImageURL { get; set; }

        /// <summary>
        /// Dimensions of the icon image. 
        /// </summary>
        [DataMember]
        public Dimensions ImageDimensions { get; set; }

        /// <summary>
        /// Point of the image that is used to pinpoint the latitude/longitude position on the map.
        /// </summary>
        [DataMember]
        public Point Anchor { get; set; }
        
        /// <summary>
        /// An optional shadow for the icon on the map.
        /// </summary>
        [DataMember]
        public string ShadowURL { get; set; }

        /// <summary>
        /// Dimensions of the shadow image. 
        /// </summary>
        [DataMember]
        public Dimensions ShadowDimensions { get; set; }

        /// <summary>
        /// Point of the shadow to pinpoint on the map. 
        /// </summary>
        [DataMember]
        public Point ShadowAnchor { get; set; }

        /// <summary>
        /// A list of X and Y coordinates to define a polygon that makes up the clickable area of the icon. 
        /// The whole icon is clickable if this is not set. 
        /// </summary>
        [DataMember]
        public string ClickablePolygon { get; set; }
    }
}