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
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Text;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System.Collections.Specialized;
using Sitecore;

namespace Sitecore.SharedSource.GoogleMaps.SitecoreCommands
{

    /// <summary>
    /// Command to open the map editor for a single latitude/longitude point. 
    /// </summary>
    [Serializable]
    public class ShowMap : Command
    {
        /// <summary>
        /// URl to the editor
        /// </summary>
        public UrlString editorUrl = new UrlString(Settings.EditorURL);

        // Methods
        public override void Execute(CommandContext context)
        {
            Error.AssertObject(context, "context");
            if (context.Items.Length != 1) return;
            var item = context.Items[0];
            var parameters = new NameValueCollection();
            parameters["id"] = item.ID.ToString();
            parameters["language"] = item.Language.ToString();
            parameters["version"] = item.Version.ToString();
            Context.ClientPage.Start(this, "Run", parameters);
        }

        public override CommandState QueryState(CommandContext context)
        {
            Error.AssertObject(context, "context");
            if (context.Items.Length != 1)
            {
                return CommandState.Disabled;
            }
            var item = context.Items[0];
            if (item.Appearance.ReadOnly)
            {
                return CommandState.Disabled;
            }
            if (!item.Access.CanWrite())
            {
                return CommandState.Disabled;
            }
            if (Command.IsLockedByOther(item))
            {
                return CommandState.Disabled;
            }
            return base.QueryState(context);
        }

        protected void Run(ClientPipelineArgs args)
        {
            var argId = args.Parameters["id"];
            var argLanguage = args.Parameters["language"];
            var argVersion = args.Parameters["version"];
            var item = Context.ContentDatabase.GetItem(argId, Language.Parse(argLanguage), Sitecore.Data.Version.Parse(argVersion));

            Error.AssertItemFound(item);
            if (!SheerResponse.CheckModified()) return;
            if (args.IsPostBack)
            {
                Context.ClientPage.SendMessage(this, "item:load(id=" + argId + ",language=" + argLanguage + ",version=" + argVersion + ")");
            }
            else
            {
                editorUrl.Add("id", argId);
                editorUrl.Add("language", argLanguage);
                editorUrl.Add("version", argVersion);
                SheerResponse.ShowModalDialog(editorUrl.ToString(), "700", "500", "Please click on the map to select a coordinate. Use the search functionality to search for and then select a point on the map.", true);
                args.WaitForPostBack();
            }
        }
    }
}


