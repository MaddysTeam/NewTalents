using Business;
using Symber.Web.Data;
using System;
using System.Web.Mvc;
using TheSite.EvalAnalysis;

namespace TheSite.Controllers
{

	public class DeclarePeriodController : BaseController
	{

		static APDBDef.DeclarePeriodTableDef dp = APDBDef.DeclarePeriod;


		// GET: EvalPeriod/List

		public ActionResult List()
		{
			var list = db.DeclarePeriodDal.ConditionQuery(null, null, null, null);

			return View(list);
		}


		//	GET: EvalPeriod/Edit
		//	POST-Ajax: EvalPeriod/Edit

		public ActionResult Edit(long? id)
		{
         var model = new DeclarePeriod()
         {
            DeclareStartDate = DateTime.Today,
            DeclareEndDate = DateTime.Today.AddYears(3),
            ReveiwStartDate = DateTime.Today.AddDays(30),
            ReveiwEndDate=DateTime.Today.AddDays(45),
			};

			if (id != null)
			{
				model = db.DeclarePeriodDal.PrimaryGet(id.Value);
			}

			return PartialView("Edit", model);
		}

		[HttpPost]
		public ActionResult Edit(DeclarePeriod model)
		{
			ThrowNotAjax();

			if (model.PeriodId > 0 )
			{
				db.DeclarePeriodDal.UpdatePartial(model.PeriodId, new
				{
					model.Name,
					model.BeginDate,
					model.EndDate,
					model.DeclareEndDate,
					model.DeclareStartDate,
               model.ReveiwStartDate,
               model.ReveiwEndDate,
				});
			}
			else
			{
				db.DeclarePeriodDal.Insert(model);
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

			db.DeclarePeriodDal.PrimaryDelete(id);

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
				APQuery.update(dp)
				.set(dp.IsCurrent.SetValue(false))
				.execute(db);

				APQuery.update(dp)
					.set(dp.IsCurrent.SetValue(true))
					.where(dp.PeriodId == id)
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