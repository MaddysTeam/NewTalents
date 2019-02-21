using Business;
using Symber.Web.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TheSite.EvalAnalysis;

namespace TheSite.Controllers
{

	public class EvalManageController : BaseController
	{

		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;


		#region [ 校评量表 ]


		// GET: EvalManage/SchoolRule
		// POST-Ajax: EvalManage/SchoolRule

		public ActionResult SchoolRule()
		{
			ViewBag.Periods = GetPeriodSelect();

			return View();
		}

		[HttpPost]
		public ActionResult SchoolRule(long periodId)
		{
			ThrowNotAjax();
			

			var period = db.EvalPeriodDal.PrimaryGet(periodId);
			var engine = EngineManager.Engines[period.AnalysisType].SchoolEval;


			return PartialView(engine.RuleView, period);
		}


		#endregion


		#region [ 量评量表  ]


		// GET: EvalManage/VolumnRule
		// POST-Ajax: EvalManage/VolumnRule

		public ActionResult VolumnRule()
		{
			ViewBag.Periods = GetPeriodSelect();

			return View();
		}

		[HttpPost]
		public ActionResult VolumnRule(long periodId, long targetId)
		{
			ThrowNotAjax();


			var period = db.EvalPeriodDal.PrimaryGet(periodId);
			var engine = EngineManager.Engines[period.AnalysisType].VolumnEvals[targetId];


			return PartialView(engine.RuleView);
		}


		#endregion


		#region [ 质评量表 ]


		// GET: EvalManage/QualityRule
		// POST-Ajax: EvalManage/QualityRule

		public ActionResult QualityRule()
		{
			ViewBag.Periods = GetPeriodSelect();

			return View();
		}

		[HttpPost]
		public ActionResult QualityRule(long periodId, long targetId)
		{
			ThrowNotAjax();

			var period = db.EvalPeriodDal.PrimaryGet(periodId);
			var engine = EngineManager.Engines[period.AnalysisType].QualityEvals[targetId];


			return PartialView(engine.RuleView);
		}


		#endregion


		#region [ Helper ]


		private IList<SelectListItem> GetPeriodSelect()
			=> APQuery.select(ep.AnalysisType, ep.Name, ep.IsCurrent, ep.PeriodId)
				.from(ep)
				.query(db, r =>
				{
					var text = ep.Name.GetValue(r);
					var isCurrent = ep.IsCurrent.GetValue(r);

					return new SelectListItem()
					{
						Value = ep.PeriodId.GetValue(r).ToString(),
						Text = isCurrent ? text + "（当期）" : text
					};
				}).ToList();


		#endregion

	}

}