﻿using System.Web;
using System.Web.Mvc;

namespace Sitecore.SharedSource.GoogleMapsMVC
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
