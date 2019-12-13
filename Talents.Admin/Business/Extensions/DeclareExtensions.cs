using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TheSite.Models;

namespace Business
{
	public static class DeclareExtensions
	{

		public static string GetDeclareContent(List<DeclareContent> list, string key)
		   => list == null ? "" : list.Find(m => m.ContentKey == key) == null ? "" : list.Find(m => m.ContentKey == key).ContentValue;

		public static DeclarePeriod GetCurrentDeclarePeriod(this APDBDef db)
		{
			var dp = APDBDef.DeclarePeriod;

			var period = db.DeclarePeriodDal.ConditionQuery(dp.IsCurrent == true, null, null, null)
			   .FirstOrDefault();


			return period;
		}

		public static EvalPeriod GetCurrentPeriod(this APDBDef db)
		{
			var dp = APDBDef.EvalPeriod;

			var period = db.EvalPeriodDal.ConditionQuery(dp.IsCurrent == true, null, null, null)
			   .FirstOrDefault();


			return period;
		}

	}
}
