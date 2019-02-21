using Business;
using Symber.Web.Data;
using System;
using System.Web.Mvc;
using TheSite.EvalAnalysis;

namespace TheSite.Controllers
{

	public class EvalPeriodController : BaseController
	{

		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;


		// GET: EvalPeriod/List

		public ActionResult List()
		{
			var list = db.EvalPeriodDal.ConditionQuery(null, null, null, null);

			return View(list);
		}


		//	GET: EvalPeriod/Edit
		//	POST-Ajax: EvalPeriod/Edit

		public ActionResult Edit(long? id)
		{
			var model = new EvalPeriod()
			{
				BeginDate = DateTime.Today,
				EndDate = DateTime.Today.AddYears(3),
				AccessBeginDate = DateTime.Today,
				AccessEndDate = DateTime.Today.AddYears(1)
			};

			if (id != null)
			{
				model = db.EvalPeriodDal.PrimaryGet(id.Value);
			}

			return PartialView("Edit", model);
		}

		[HttpPost]
		public ActionResult Edit(EvalPeriod model)
		{
			ThrowNotAjax();


			model.AnalysisName = EngineManager.Engines[model.AnalysisType].AnalysisName;

			if (model.PeriodId > 0 )
			{
				model.Creator = UserProfile.UserId;
				model.CreateDate = DateTime.Now;
				db.EvalPeriodDal.UpdatePartial(model.PeriodId, new
				{
					model.Name,
					model.BeginDate,
					model.EndDate,
					model.AccessBeginDate,
					model.AccessEndDate,
					model.AnalysisName,
					model.AnalysisType
				});
			}
			else
			{
				model.ModifyDate = DateTime.Now;
				model.Modifier = UserProfile.UserId;
				db.EvalPeriodDal.Insert(model);
			}


			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已保存!"
			});
		}


		//	POST-Ajax: EvalPeriod/Remove		

		[HttpPost]
		public ActionResult Remove(long id)
		{
			ThrowNotAjax();

			db.EvalPeriodDal.PrimaryDelete(id);

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已删除"
			});
		}


		//	POST-Ajax: EvalPeriod/SetCurrent	

		[HttpPost]
		public ActionResult SetCurrent(long id)
		{
			ThrowNotAjax();


			db.BeginTrans();

			try
			{
				APQuery.update(ep)
				.set(ep.IsCurrent.SetValue(false))
				.execute(db);

				APQuery.update(ep)
					.set(ep.IsCurrent.SetValue(true))
					.where(ep.PeriodId == id)
					.execute(db);

				db.Commit();
			}
			catch (Exception ex)
			{
				db.Rollback();

				return Json(new
				{
					result = AjaxResults.Error,
					msg = ex.Message
				});
			}
			

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "设置已成功！"
			});
		}

	}

}