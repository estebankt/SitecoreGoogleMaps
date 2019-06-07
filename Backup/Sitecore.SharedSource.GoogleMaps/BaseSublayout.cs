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
using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.SharedSource.GoogleMaps
{
    /// <summary>
    /// base class for a sublayout to ease access to its datasource. 
    /// ref http://firebreaksice.com/using-the-datasource-field-with-sitecore-sublayouts/
    /// </summary>
    public class BaseSublayout : System.Web.UI.UserControl
    {

        private Item _dataSource = null;
        public Item DataSource
        {
            get
            {
                if (_dataSource == null)
                    if (Parent is Sublayout)
                        _dataSource = Sitecore.Context.Database.GetItem(((Sublayout)Parent).DataSource);

                return _dataSource;
            }
        }

        public BaseSublayout() : base() { }

    }
}
