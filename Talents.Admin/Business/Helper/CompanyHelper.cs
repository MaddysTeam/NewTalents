using System.Collections.Generic;
using System.Web.Mvc;

namespace Business.Helper
{

	public static class CompanyHelper
   {

		public static IEnumerable<SelectListItem> GetCompanySelectList(string noneLabel = null)
		{
			if (noneLabel != null)
				yield return new SelectListItem() { Value = "", Text = noneLabel };

         var db = new APDBDef();
         var companys = db.CompanyDal
            .ConditionQuery(null, null, null, null);

         companys.Insert(0, new Company { CompanyName = SelectNames.All, CompanyId = 0 });

         foreach (var item in companys)
			{
				yield return new SelectListItem()
				{
					Value = item.CompanyId.ToString(),
					Text = item.CompanyName
				};
			}
		}

	}

}