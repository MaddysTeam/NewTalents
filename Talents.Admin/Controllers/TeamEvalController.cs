using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Linq;
using System.Web.Mvc;
using TheSite.EvalAnalysis;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class TeamEvalController : BaseController
	{

		static APDBDef.TeamEvalResultTableDef er = APDBDef.TeamEvalResult;
		static APDBDef.TeamEvalResultItemTableDef eri = APDBDef.TeamEvalResultItem;
		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.TeamMemberTableDef t = APDBDef.TeamMember;
		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;


		public ActionResult CurrentList(long periodId=0)
		{
			if (periodId == 0)
			{
				var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null)
				.FirstOrDefault();

				if (null == period || !period.InAccessDateRegion(DateTime.Now))
				{
					return View("../EvalPeriod/NotInAccessRegion");
				}
				else
				{
					return RedirectToAction("CurrentList", period.PeriodId);
				}
			}

			return View("EvalList");
		}


		// GET: TeamEval/EvalList
		// POST-Ajax: TeamEval/EvalList

		public ActionResult EvalList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult EvalList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long teamId, long periodId)
		{
			ThrowNotAjax();


			var query = APQuery.select(er.MemberId, er.TeamId, er.ResultId.As("ResultId"),
					d.DeclareTargetPKID, d.DeclareSubjectPKID,
					er.Score, er.FullScore)
				.from(er,
						 d.JoinInner(er.MemberId == d.TeacherId),
						 u.JoinInner(er.MemberId == u.UserId),
						 t.JoinInner(t.TeamId == er.TeamId)
						)
				.where(er.TeamId == teamId, er.PeriodId == periodId & er.Accesser == UserProfile.UserId)
				.primary(er.ResultId)
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
					case "score": query.order_by(sort.OrderBy(er.Score)); break;
				}
			}
			else
			{
				query.order_by(d.DeclareTargetPKID.Asc).order_by_add(er.Score.Desc);
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, r =>
			{
				var score = er.Score.GetValue(r);
				var fullScore = er.FullScore.GetValue(r);

				return new
				{
					id = er.MemberId.GetValue(r),
					resultId = er.ResultId.GetValue(r, "ResultId"),
					realName = u.RealName.GetValue(r),
					targetId = d.DeclareTargetPKID.GetValue(r),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(r)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(r)),
					score = string.Format("{0} / {1}", score, fullScore),
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


		// GET: TeamEval/NotEvalList
		// POST-Ajax: TeamEval/NotEvalList

		public ActionResult NotEvalList(long periodId)
		{
			return View();
		}

		[HttpPost]
		public JsonResult NotEvalList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long periodId)
		{
			ThrowNotAjax();

			var subquery = APQuery.select(er.MemberId)
				   .from(er)
				   .where(er.PeriodId == periodId & er.Accesser == UserProfile.UserId);

			var query = APQuery.select(u.RealName, d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID,
												t.TeamId, u.UserId)
				.from(t,
						d.JoinInner(d.TeacherId == t.MemberId),
						u.JoinInner(u.UserId == t.MemberId))
				.where(t.TeamId == UserProfile.UserId & d.TeacherId.NotIn(subquery))
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
				}
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				return new
				{
					teacherId = u.UserId.GetValue(rd),
					teamId = t.TeamId.GetValue(rd),
					realName = u.RealName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
					stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd))
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


		// GET: TeamEval/Eval
		//	POST: TeamEval/Eval

		[NoCache]
		public ActionResult Eval(TeamEvalParam param)
		{
			var period = db.EvalPeriodDal.PrimaryGet(param.PeriodId);

			param.AccesserId = UserProfile.UserId;
			var engine = EngineManager.Engines[period.AnalysisType].TeamEvals;
			ViewBag.ResultItems = engine.GetResultItem(db, param);
			ViewBag.Declare = param.GetDeclareInfo(db);

			return View(engine.EvalView, param);
		}

		[HttpPost]
		public ActionResult Eval(TeamEvalParam param, FormCollection fc)
		{
			var period = db.EvalPeriodDal.PrimaryGet(param.PeriodId);

			var engine = EngineManager.Engines[period.AnalysisType].TeamEvals;

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


		//	GET: TeamEval/ResultView

		public ActionResult ResultView(TeamEvalParam param)
		{
			TeamEvalPeiodModel model = new TeamEvalPeiodModel(param);

			model.Period = db.EvalPeriodDal.PrimaryGet(model.PeriodId);
			model.Declare = model.GetDeclareInfo(db);

			var engine = EngineManager.Engines[model.Period.AnalysisType].TeamEvals;

			model.AnalysisUnit = engine;
			model.Result = engine.GetResult(db, param);

			if (model.Result == null)
			{
				model.Message = "当期团队考核还未执行!";
			}
			else
			{
				model.ResultItems = engine.GetResultItem(db, param);
			}


			return View("../EvalUtilities/ResultView", model);
		}

	}

}