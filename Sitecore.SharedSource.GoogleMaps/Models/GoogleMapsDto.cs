using System.Web.Helpers;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.SharedSource.GoogleMaps.Gmaps;

namespace Sitecore.SharedSource.GoogleMaps.Models
{
	public class GoogleMapsDto
	{
		public Rendering Rendering { get; set; }
		public Item Item { get; set; }
		public Item PageItem { get; set; }
		public string PageId { get; set; }
		public string DatasourceID { get; set; }
		public GMap CurrentMap { get; set; }
		public string ApiKey { get; set; }
		public string JsonModel { get; set; }
		public void Initialize(Rendering rendering)
		{
			Rendering = rendering;
			Item = rendering.Item;
			PageItem = PageContext.Current.Item;
		}
	}
}