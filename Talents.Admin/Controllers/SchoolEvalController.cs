using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TheSite.EvalAnalysis;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class SchoolEvalController : BaseController
	{

		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;
		static APDBDef.CompanyTableDef c = APDBDef.Company;
		static APDBDef.CompanyAccesserTableDef ca = APDBDef.CompanyAccesser;
		static APDBDef.CompanyDeclareTableDef cd = APDBDef.CompanyDeclare;
		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.EvalSchoolResultTableDef er = APDBDef.EvalSchoolResult;
		static APDBDef.EvalSchoolResultItemTableDef eri = APDBDef.EvalSchoolResultItem;

		// GET: SchoolEval/CurrentList
		// POST-Ajax: SchoolEval/CurrentList
		// TODO: variable isLowDeclareLevel only for eval 2020   

		public ActionResult CurrentList(long periodId = 0, bool isLowDeclareLevel = false)
		{
			if (periodId == 0)
			{
				var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null)
					.FirstOrDefault();

				if (period == null || !period.InAccessDateRegion(DateTime.Now))
				{
					return View("../EvalPeriod/NotInAccessRegion");
				}
				else
				{
					return RedirectToAction("CurrentList", "SchoolEval", new { periodId = period.PeriodId, isLowDeclareLevel });
				}
			}

			return View("List");
		}

		[HttpPost]
		public JsonResult CurrentList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long periodId, bool isLowDeclareLevel)
		{
			ThrowNotAjax();


			var query = APQuery.select(u.RealName, d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID,
												   cd.CompanyId, u.UserId, er.Score, er.FullScore, er.ResultId)
				   .from(ca,
						   cd.JoinLeft(ca.CompanyId == cd.CompanyId),
						   d.JoinLeft(cd.TeacherId == d.TeacherId),
						   u.JoinLeft(cd.TeacherId == u.UserId),
						   er.JoinLeft(cd.TeacherId == er.TeacherId & er.PeriodId == periodId));

			query = isLowDeclareLevel ?
					query.where(d.DeclareTargetPKID > DeclareTargetIds.GugJiaos & d.DeclareTargetPKID < DeclareTargetIds.PutLaos) :
					query.where(d.DeclareTargetPKID <= DeclareTargetIds.GugJiaos);

			query = query.where_and(ca.UserId == UserProfile.UserId);


			//过滤条件
			//模糊搜索姓名

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(u.RealName.Match(searchPhrase));
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
					case "target": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
					case "subject": query.order_by(sort.OrderBy(d.DeclareSubjectPKID)); break;
					case "stage": query.order_by(sort.OrderBy(d.DeclareStagePKID)); break;
					case "score": query.order_by(sort.OrderBy(er.Score)); break;
				}
			}

			//var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				var score = er.Score.GetValue(rd);
				var fullScore = er.FullScore.GetValue(rd);
				return new
				{
					id = er.ResultId.GetValue(rd),
					teacherId = u.UserId.GetValue(rd),
					companyId = cd.CompanyId.GetValue(rd),
					realName = u.RealName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
					stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
					score = fullScore > 0 ? string.Format("{0} / {1}", score, fullScore) : "",
				};
			}).ToList();


			var total = result.Count;
			var results = result.Skip(rowCount * (current - 1)).Take(rowCount).ToList();


			return Json(new
			{
				rows = results,
				current,
				rowCount,
				total
			});
		}


		// GET: SchoolEval/CurrentList
		//	POST-Ajax: SchoolEval/CurrentList

		public ActionResult NotEvalList(long periodId)
		{
			return View();
		}

		[HttpPost]
		public JsonResult NotEvalList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long periodId)
		{
			ThrowNotAjax();

			var v = APDBDef.EvalVolumnResult;

			var subquery = APQuery.select(er.TeacherId)
				   .from(er)
				   .where(er.PeriodId == periodId & er.Accesser == UserProfile.UserId);


			var query = APQuery.select(u.RealName, d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID, v.Score,
												cd.CompanyId, u.UserId)
				.from(ca,
						cd.JoinLeft(ca.CompanyId == cd.CompanyId),
						d.JoinInner(cd.TeacherId == d.TeacherId),
						u.JoinInner(cd.TeacherId == u.UserId),
				  v.JoinLeft(u.UserId == v.TeacherId & v.PeriodId == periodId))
			.where(ca.UserId == UserProfile.UserId & d.TeacherId.NotIn(subquery) & d.DeclareTargetPKID < DeclareTargetIds.PutLaos)
				.primary(u.UserId)
				.skip((current - 1) * rowCount)
				.take(rowCount);


			//过滤条件
			//模糊搜索姓名

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(u.RealName.Match(searchPhrase));
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
						//case "target": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
						//case "subject": query.order_by(sort.OrderBy(d.DeclareSubjectPKID)); break;
						//case "stage": query.order_by(sort.OrderBy(d.DeclareStagePKID)); break;
						//case "score": query.order_by(sort.OrderBy(er.Score)); break;
				}
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				return new
				{
					teacherId = u.UserId.GetValue(rd),
					companyId = cd.CompanyId.GetValue(rd),
					realName = u.RealName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
					stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
					vscore = string.Format("{0}/{1}", v.Score.GetValue(rd), 100)
				};
			}).ToList();


			return Json(new
			{
				rows = result,
				current,
				rowCount,
				total
			});
		}


		// GET: SchoolEval/Eval
		//	POST: SchoolEval/Eval

		[NoCache]
		public ActionResult Eval(SchoolEvalParam param)
		{
			var period = db.EvalPeriodDal.PrimaryGet(param.PeriodId);

			param.AccesserId = UserProfile.UserId;
			var engine = EngineManager.Engines[period.AnalysisType].SchoolEval;
			ViewBag.ResultItems = engine.GetResultItem(db, param);

			ViewBag.Declare = param.GetDeclareInfo(db);


			return View(engine.EvalView, param);
		}

		[HttpPost]
		public ActionResult Eval(SchoolEvalParam param, FormCollection fc)
		{
			var period = db.EvalPeriodDal.PrimaryGet(param.PeriodId);

			var engine = EngineManager.Engines[period.AnalysisType].SchoolEval;

			db.BeginTrans();

			try
			{
				engine.Eval(db, param, fc);

				db.Commit();
			}
			catch (Exception ex)
			{
				db.Rollback();
				throw ex;
			}



			return RedirectToAction("ResultView", new { param.TeacherId, param.PeriodId });
		}


		//	GET: SchoolEval/ResultView

		public ActionResult ResultView(SchoolEvalParam param)
		{
			SchoolEvalPeriodModel model = new SchoolEvalPeriodModel(param);

			model.Period = db.EvalPeriodDal.PrimaryGet(model.PeriodId);
			model.Declare = model.GetDeclareInfo(db);

			var engine = EngineManager.Engines[model.Period.AnalysisType].SchoolEval;

			model.AnalysisUnit = engine;
			model.Result = engine.GetResult(db, param);

			if (model.Result == null)
			{
				model.Message = "当期校评还未执行!";
			}
			else
			{
				model.ResultItems = engine.GetResultItem(db, param);
			}


			return View("../EvalUtilities/ResultView", model);
		}



	}

}