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

namespace Sitecore.SharedSource.GoogleMaps
{
    /// <summary>
    /// Custom exception to handle a missing or empty field on a Sitecore item
    /// </summary>
    public class FieldValidationException : Exception, ISerializable
    {
        public FieldValidationException()
            : base()
        {
        }

        public FieldValidationException(string message)
            : base(message)
        {
        }

        public FieldValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public FieldValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}