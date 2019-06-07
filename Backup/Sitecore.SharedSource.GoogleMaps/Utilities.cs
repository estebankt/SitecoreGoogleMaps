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
using System.IO;
using System.Web.Script.Serialization;
using Sitecore.SharedSource.GoogleMaps.Gmaps;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;

namespace Sitecore.SharedSource.GoogleMaps
{
    /// <summary>
    /// Static class for all common utilities and helper of the module
    /// </summary>
    public static class Utilities
    {

        /// <summary>
        /// Ensures a field is present on a Sitecore item. Additionally, checks if the field has a 
        /// value if mustHaveValue is set to true. Throws a FieldValidationException on failure. 
        /// </summary>
        /// <param name="item">The Sitecore item to check for a field</param>
        /// <param name="fieldName">The field name to check</param>
        /// <param name="mustHaveValue">Additionaly ensure the field has a value</param>
        /// <exception cref="FieldValidationException">Field is not present or empty (if checked for value)</exception>
        public static void EnsureField(Item item, string fieldName, bool mustHaveValue = false)
        {
            if (item == null)
            {
                Log.Error("Google Maps Module - item not found", Settings.LoggingOwner);
                throw new FieldValidationException("Item is null");
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                // nothing to check, stop here. 
                return;
            }
            if (item.Fields[fieldName] == null)
            {
                Log.Error(string.Format("Google Maps Module - Field {0} is not present on {1}.", fieldName, item.Name), Settings.LoggingOwner);
                throw new FieldValidationException("Field is not present");
            }
            if (mustHaveValue && !item.Fields[fieldName].ContainsStandardValue && string.IsNullOrEmpty(item.Fields[fieldName].Value))
            {
                Log.Error(string.Format("Google Maps Module - Field {0} is empty on {1}.", fieldName, item.Name), Settings.LoggingOwner);
                throw new FieldValidationException("Field is empty");
            }
            // all passed
        }


        /// <summary>
        /// Ensures a string is converted into a double value that works as long or lat value
        /// </summary>
        public static double EnsureLongLat(string value)
        {
            double retval = 0.0;
            try
            {
                retval = double.Parse(value.Replace(',', '.'));
            }
            catch (Exception) { }
            return retval;
        }

        /// <summary>
        /// Ensures a string is a valid zoom level for a Google Map
        /// </summary>
        public static int EnsureZoomLevel(string value)
        {
            int retval = 1;
            try
            {
                retval = int.Parse(value);
                if (retval <= 0)
                    retval = 1;
                if (retval > 100)
                    retval = 100;
            }
            catch (Exception) { }
            return retval;
        }

        /// <summary>
        /// Ensures a string is converted into a valid opacity value
        /// </summary>
        public static double EnsureOpacity(string value)
        {
            double retval = 1.0;
            try
            {
                retval = double.Parse(value);
                if (retval < 0.0 || retval > 1.0)
                    retval = 1.0;
            }
            catch (Exception) { }
            return retval;
        }

        /// <summary>
        /// Ensures a string is converted into a valid weight value
        /// </summary>        
        public static int EnsureWeight(string value)
        {
            int retval = 1;
            try
            {
                retval = int.Parse(value);
                if (retval <= 0)
                    retval = 1;
            }
            catch (Exception) { }
            return retval;
        }

        /// <summary>
        /// Converts a string in the format xx,yy into a point
        /// </summary>
        public static Point StringToPoint(string input)
        {
            int x = 0;
            int y = 0;

            var parts = input.Split(',');
            if (parts.Length >= 2)
            {
                try
                {
                    x = int.Parse(parts[0].Trim());
                    y = int.Parse(parts[1].Trim());
                }
                catch (Exception) { }
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Converts an array of string latitude and longitude into a list of objects
        /// </summary>
        /// <param name="input">array of lat/lng in the format xxx,yyy</param>
        public static List<GLatLng> ArrayToLatLngList(string[] input)
        {
            var retval = new List<GLatLng>();
            foreach (string latLangStr in input)
            {
                var parts = latLangStr.Split(',');
                if (parts.Length == 2)
                {
                    retval.Add(new GLatLng(parts[0], parts[1]));
                }
            }
            return retval;
        }

        /// <summary>
        /// Returns the dimension of a media item given as an Sitecore ImageField
        /// </summary>
        public static Dimensions GetMediaItemSize(ImageField field)
        {
            int width = 0;
            int height = 0;

            try
            {
                width = int.Parse(field.Width);
                height = int.Parse(field.Height);
            }
            catch (Exception) { }

            return new Dimensions(width, height);
        }

        /// <summary>
        /// Serializes an object to a JSON string
        /// </summary>        
        public static string JSON(object thing)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(thing);
        }

    }
}