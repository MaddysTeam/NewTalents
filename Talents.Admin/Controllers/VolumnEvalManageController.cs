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

	public class VolumnEvalManageController : BaseController
	{

		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;
		static APDBDef.EvalVolumnResultTableDef er = APDBDef.EvalVolumnResult;
		static APDBDef.EvalVolumnResultItemTableDef eri = APDBDef.EvalVolumnResultItem;
		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.CompanyDeclareTableDef cd = APDBDef.CompanyDeclare;
		static APDBDef.CompanyTableDef c = APDBDef.Company;


		// GET: VolumnEvalManage/Overview

		public ActionResult Overview(long periodId = 0)
		{
			EvalPeriod period = null;

			if (periodId == 0)
			{
				period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null)
					.FirstOrDefault();

				if (period == null)
				{
					return View("../EvalPeriod/NotInAccessRegion");
				}
				else
				{
					return RedirectToAction("Overview", new { periodId = period.PeriodId });
				}
			}


			period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null)
				.FirstOrDefault();
			ViewBag.IsCurrent = period.IsCurrent;


			var query = APQuery.select(d.DeclareTargetPKID, d.DeclareTargetPKID.Count().As("TotalCount"),
												er.TeacherId.Count().As("EvalCount"))
				.from(d, er.JoinLeft(d.TeacherId == er.TeacherId & er.PeriodId == periodId))
				.where(d.DeclareTargetPKID < DeclareTargetIds.JiaoxNengs & 
						 d.DeclareTargetPKID > DeclareTargetIds.WaipDaos)
				.group_by(d.DeclareTargetPKID);


			var result = query.query(db, rd =>
			{
				var totalCount = rd.GetInt32(rd.GetOrdinal("TotalCount"));
				var evalCount = rd.GetInt32(rd.GetOrdinal("EvalCount"));

				return new VolumnEvalOverviewModels
				{
					TargetId = d.DeclareTargetPKID.GetValue(rd),
					TargetName = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					TotalCount = totalCount,
					EvalCount = evalCount,
					Status = totalCount == evalCount && totalCount > 0 ? EvalStatus.Success
										: totalCount > evalCount && evalCount > 0 ? EvalStatus.Pending
										: EvalStatus.NotStart
				};
			}).ToList();


			return View(result);
		}


		//	GET:	VolumnEvalManage/EvalMemberList
		//	POST-Ajax:	VolumnEvalManage/EvalMemberList

		public ActionResult EvalMemberList(long periodId)
		{
			var period = db.EvalPeriodDal.ConditionQuery(ep.PeriodId == periodId, null, null, null).First();

			ViewBag.PeriodId = period.PeriodId;
			ViewBag.IsCurrent = period.IsCurrent;

			return View();
		}

		[HttpPost]
		public ActionResult EvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long periodId, long Target)
		{
			ThrowNotAjax();


			var up = APDBDef.BzUserProfile.As("up");

			var query = APQuery.select(d.DeclareTargetPKID, d.TeacherId, u.RealName, up.RealName.As("Accesser"), er.AccessDate, 
												er.ResultId, er.PeriodId, er.Score, er.FullScore)
				.from(d,
						er.JoinInner(d.TeacherId == er.TeacherId),
						u.JoinInner(d.TeacherId == u.UserId),
						up.JoinInner(er.TeacherId == up.UserId))
				.where(er.PeriodId == periodId)
				.primary(d.TeacherId)
				.skip((current - 1) * rowCount)
				.take(rowCount);


			//过滤条件
			//模糊搜索姓名，评审人姓名

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(u.RealName.Match(searchPhrase) | up.RealName.Match(searchPhrase));
			}

			if (Target > 0)
			{
				query.where_and(d.DeclareTargetPKID == Target);
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "targetName": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
					case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
					case "accesser": query.order_by(sort.OrderBy(er.Accesser)); break;
					case "accessDate": query.order_by(sort.OrderBy(er.AccessDate)); break;
					case "score": query.order_by(sort.OrderBy(er.Score)); break;
				}
			}


			var total = db.ExecuteSizeOfSelect(query);


			var result = query.query(db, rd =>
			{
				var score = er.Score.GetValue(rd);
				var fullScore = er.FullScore.GetValue(rd);
				return new
				{
					id = er.ResultId.GetValue(rd),
					periodId = er.PeriodId.GetValue(rd),
					teacherId = er.TeacherId.GetValue(rd),
					targetId = d.DeclareTargetPKID.GetValue(rd),
					targetName = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					realName = u.RealName.GetValue(rd),
					accesser = up.RealName.GetValue(rd, "Accesser"),
					accessDate = er.AccessDate.GetValue(rd),
					score = string.Format("{0} / {1}", score, fullScore)
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


		//	GET:	VolumnEvalManage/NotEvalMemberList
		//	POST-Ajax:	VolumnEvalManage/NotEvalMemberList

		public ActionResult NotEvalMemberList(long periodId)
		{
			var period = db.EvalPeriodDal.ConditionQuery(ep.PeriodId == periodId, null, null, null).First();

			ViewBag.PeriodId = period.PeriodId;
			ViewBag.IsCurrent = period.IsCurrent;

			return View();
		}

		[HttpPost]
		public ActionResult NotEvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long periodId, long Target)
		{
			ThrowNotAjax();


			var subquery = APQuery.select(er.TeacherId)
				.from(er)
				.where(er.PeriodId == periodId);


			var query = APQuery.select(d.DeclareTargetPKID, d.TeacherId, u.RealName)
				.from(d,
						u.JoinInner(d.TeacherId == u.UserId))
				.where(d.TeacherId.NotIn(subquery) & 
						 d.DeclareTargetPKID < DeclareTargetIds.JiaoxNengs &
						 d.DeclareTargetPKID > DeclareTargetIds.WaipDaos)
				.primary(d.TeacherId)
				.skip((current - 1) * rowCount)
				.take(rowCount);


			//过滤条件
			//模糊搜索姓名

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(u.RealName.Match(searchPhrase));
			}

			if (Target > 0)
			{
				query.where_and(d.DeclareTargetPKID == Target);
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
					case "targetName": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
				}
			}

			var total = db.ExecuteSizeOfSelect(query);


			var result = query.query(db, rd =>
			{
				return new
				{
					teacherId = d.TeacherId.GetValue(rd),
					targetId = d.DeclareTargetPKID.GetValue(rd),
					targetName = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					realName = u.RealName.GetValue(rd),
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


		// GET: VolumnEvalManage/List

		public ActionResult List()
		{
			var list = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == false, null, null, null);

			return View(list);
		}


		//	POST-Ajax:	VolumnEvalManage/Eval

		[HttpPost]
		public ActionResult Eval(long periodId, long teacherId, long targetId)
		{
			ThrowNotAjax();


			var period = db.EvalPeriodDal.PrimaryGet(periodId);

			var engine = EngineManager.Engines[period.AnalysisType].VolumnEvals[targetId];

			db.BeginTrans();

			try
			{
				engine.Eval(db, new VolumnEvalParam { PeriodId = periodId, AccesserId = UserProfile.UserId }, teacherId);
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
				msg = "量评已完成!"
			});
		}


		//	GET: VolumnEvalManage/Evals

		public ActionResult Evals(long periodId, long targetId)
		{
			var teacherIds = APQuery.select(d.TeacherId)
				.from(d)
				.where(d.DeclareTargetPKID == targetId)
				.query(db, r =>
				{
					return d.TeacherId.GetValue(r);
				}).ToArray();

			var period = db.EvalPeriodDal.PrimaryGet(periodId);

			var engine = EngineManager.Engines[period.AnalysisType].VolumnEvals[targetId];

			db.BeginTrans();
			try
			{
				engine.Eval(db, new VolumnEvalParam { PeriodId = periodId, AccesserId = UserProfile.UserId }, teacherIds);
				db.Commit();
			}
			catch (Exception ex)
			{
				db.Rollback();
				throw ex;
			}
			

			return RedirectToAction("Overview");
		}


		//	GET:	VolumnEvalManage/ResultView

		public ActionResult ResultView(VolumnEvalParam param)
		{
			VolumnEvalPeriodModel model = new VolumnEvalPeriodModel(param);

			model.Period = db.EvalPeriodDal.PrimaryGet(model.PeriodId);
			model.Declare = model.GetDeclareInfo(db);

			if (!DeclareTargetIds.HasTeam(model.Declare.TargetId))
			{
				model.Message = "申报（担当）称号未纳入当期量评考评。";
			}
			else
			{
				var engine = EngineManager.Engines[model.Period.AnalysisType].VolumnEvals[model.Declare.TargetId];

				model.AnalysisUnit = engine;
				model.Result = engine.GetResult(db, param);

				if (model.Result == null)
				{
					model.Message = "当期量评还未执行!";
				}
				else
				{
					model.ResultItems = engine.GetResultItem(db, param);
				}
			}


			return View("../EvalUtilities/ResultView", model);
		}


		//	GET: VolumnEvalManage/Report

		public ActionResult Report(long teacherId, long periodId)
		{
			var t = APDBDef.EvalVolumnResult;

			var model = db.EvalVolumnResultDal.ConditionQuery(t.TeacherId == teacherId & t.PeriodId == periodId, 
				null, null, null).FirstOrDefault();

			if (model == null)
			{
				model = new EvalVolumnResult();
			}


			return PartialView("Report", model);
		}
	}

}