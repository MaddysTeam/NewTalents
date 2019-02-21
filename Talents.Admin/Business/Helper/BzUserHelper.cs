using System.Collections.Generic;
using System.Web.Mvc;

namespace Business.Helper
{

	public static class BzUserHelper
	{

		public static IEnumerable<SelectListItem> GetRoleSelectList(string noneLabel = null)
		{
			if (noneLabel != null)
				yield return new SelectListItem() { Value = "", Text = noneLabel };

			foreach (var item in BzRoleCache.Cached)
			{
				yield return new SelectListItem()
				{
					Value = item.Value.Name,
					Text = item.Value.NormalizedName
				};
			}
		}

	}

}