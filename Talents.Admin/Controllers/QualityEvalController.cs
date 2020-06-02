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

	public class QualityEvalController : BaseController
	{
		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;
		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.EvalQualityResultTableDef er = APDBDef.EvalQualityResult;
		static APDBDef.EvalQualityResultItemTableDef eri = APDBDef.EvalQualityResultItem;
		static APDBDef.EvalQualitySubmitResultTableDef esr = APDBDef.EvalQualitySubmitResult;
		static APDBDef.ExpGroupMemberTableDef egm = APDBDef.ExpGroupMember;
		static APDBDef.ExpGroupTargetTableDef egt = APDBDef.ExpGroupTarget;
		static APDBDef.ExpGroupTableDef eg = APDBDef.ExpGroup;


		// GET: QualityEval/Index

		public ActionResult Index()
		{
			var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null)
				.FirstOrDefault();

			if (period == null || !period.InAccessDateRegion(DateTime.Now))
			{
				return View("../EvalPeriod/NotInAccessRegion");
			}

			return RedirectToAction("List", new { period.PeriodId });
		}


		// GET: QualityEval/BlockList
		// POST-Ajax: QualityEval/BlockList

		public ActionResult List()
		{
			var groups = APQuery.select(eg.GroupId, eg.Name)
				.from(egm, eg.JoinInner(egm.GroupId == eg.GroupId),
						u.JoinInner(u.UserId == egm.ExpectID))
						.group_by(eg.GroupId, eg.Name)
						.where(egm.ExpectID == UserProfile.UserId)
				.query(db, (rd) =>
				 {
					 return new ExpGroup
					 {
						 GroupId = eg.GroupId.GetValue(rd),
						 Name = eg.Name.GetValue(rd),
					 };
				 }).ToList();

			var result = new ExpGroupList
			{
				Groups = groups
			};

			return View(result);
		}

		[HttpPost]
		public JsonResult GetGroupInfo(long groupId, long periodId)
		{
			ThrowNotAjax();


			string leaderName = null;

			var memberNames = APQuery.select(egm.IsLeader, u.RealName)
				 .from(egm, u.JoinInner(u.UserId == egm.ExpectID))
				 .where(egm.GroupId == groupId)
				 .query(db, r =>
				 {
					 var name = u.RealName.GetValue(r);
					 if (egm.IsLeader.GetValue(r))
					 {
						 leaderName = name;
					 }
					 return name;
				 }).ToArray();

			var totalCount = db.ExpGroupTargetDal.ConditionQueryCount(egt.GroupId == groupId);
			var evalCount = db.EvalQualityResultDal.ConditionQueryCount(
					er.GroupId == groupId & er.PeriodId == periodId & er.Accesser == UserProfile.UserId);

			return Json(new
			{
				leaderName,
				memberNames = String.Join(", ", memberNames),
				evalCount,
				notEvalCount = totalCount - evalCount
			});
		}


		// GET: QualityEval/EvalMemberList        TODO: for 2019 version only
		// POST-Ajax: QualityEval/EvalMemberList  TODO: for 2019 version only

		public ActionResult EvalMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult EvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long periodId)
		{
			ThrowNotAjax();


			var query = APQuery.select(er.ResultId, er.TeacherId,
									   er.GroupId, er.FullScore, er.DynamicScore1, 
									   er.DynamicScore2, er.DynamicScore3,u.RealName,
									   d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID
					)
				.from(er,
						 d.JoinInner(er.TeacherId == d.TeacherId),
						 u.JoinInner(er.TeacherId == u.UserId)
						)
				.where(er.GroupId == groupId, er.PeriodId == periodId & er.Accesser == UserProfile.UserId)
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
					case "dynamicScore1": query.order_by(sort.OrderBy(er.DynamicScore1)); break;
					case "dynamicScore2": query.order_by(sort.OrderBy(er.DynamicScore2)); break;
					case "dynamicScore3": query.order_by(sort.OrderBy(er.DynamicScore3)); break;
				}
			}
			else
			{
				query.order_by(er.DeclareTargetPKID.Asc).order_by_add(er.Score.Desc);
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, r =>
			{
				var fullScore = er.FullScore.GetValue(r);
				var score1 = er.DynamicScore1.GetValue(r);
				var score2 = er.DynamicScore2.GetValue(r);
				var score3 = er.DynamicScore3.GetValue(r);

				return new
				{
					id = er.TeacherId.GetValue(r),
					resultId = er.ResultId.GetValue(r),
					realName = u.RealName.GetValue(r),
					groupId = er.GroupId.GetValue(r),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(r)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(r)),
					stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(r)),
					targetId = d.DeclareTargetPKID.GetValue(r),
					dynamicScore1 = string.Format("{0} / {1}", score1, fullScore),
					dynamicScore2 = string.Format("{0} / {1}", score2, fullScore),
					dynamicScore3 = string.Format("{0} / {1}", score3, fullScore),
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


		// GET: QualityEval/NotEvalMemberList
		// POST-Ajax: QualityEval/NotEvalMemberList

		public ActionResult NotEvalMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult NotEvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long periodId)
		{
			ThrowNotAjax();


			var subQuery = APQuery.select(er.TeacherId)
				.from(er)
				.where(er.Accesser == UserProfile.UserId & er.GroupId == groupId & er.PeriodId == periodId);

			var query = APQuery.select(egt.MemberId, u.RealName, d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID)
				.from(egt,
						 u.JoinInner(u.UserId == egt.MemberId),
						 d.JoinInner(d.TeacherId == egt.MemberId)
						)
				.where(egt.GroupId == groupId & egt.MemberId.NotIn(subQuery))
				.primary(egt.MemberId)
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
					case "target": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
					case "subject": query.order_by(sort.OrderBy(d.DeclareSubjectPKID)); break;
					case "stage": query.order_by(sort.OrderBy(d.DeclareStagePKID)); break;
				}
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				return new
				{
					id = egt.MemberId.GetValue(rd),
					realName = u.RealName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
					stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
					targetId = d.DeclareTargetPKID.GetValue(rd),
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


		//	GET: QualityEval/Eval
		//	POST: QualityEval/Eval

		[NoCache]
		public ActionResult Eval(QualityEvalParam param)
		{
			param.AccesserId = UserProfile.UserId;

			var isEvalSubmit = db.EvalQualitySubmitResultDal
											  .ConditionQueryCount(esr.TeacherId == param.TeacherId & esr.PeriodId == param.PeriodId) > 0;

			var period = db.EvalPeriodDal.PrimaryGet(param.PeriodId);
			var engines = EngineManager.Engines[period.AnalysisType].QualityEvals;
			var engine = engines[param.TargetId];

			var result = db.EvalQualityResultDal.ConditionQuery(
				er.PeriodId == param.PeriodId &
				er.TeacherId == param.TeacherId &
				er.Accesser == this.UserProfile.UserId, null, null, null)
				.FirstOrDefault();

			param.ResultId = result == null ? 0 : result.ResultId;

			ViewBag.Result = engine.GetResult(db, param);

			ViewBag.ResultItems = engine.GetResultItem(db, param);

			ViewBag.Declare = param.GetDeclareInfo(db);

			return View(engine.EvalView, param);
		}

		[HttpPost]
		public ActionResult Eval(QualityEvalParam param, FormCollection fc)
		{
			var period = db.EvalPeriodDal.PrimaryGet(param.PeriodId);

			var engine = EngineManager.Engines[period.AnalysisType].QualityEvals;


			db.BeginTrans();

			try
			{
				var resultId = engine[param.TargetId].Eval(db, param, fc);

				db.Commit();

				return RedirectToAction("NotEvalMemberList", new
				{
					teacherId = param.TeacherId,
					periodId = param.PeriodId,
					groupId = param.GroupId
				});
			}
			catch
			{
				db.Rollback();
				throw;
			}
		}


		// POST: QualityEval/SubmitEvalResult

		[HttpPost]
		public ActionResult SubmitEvalResult(EvalQualitySubmitResult model)
		{
			model.Score += model.AdjustScore;
			model.AccessDate = DateTime.Now;

			var exists = db.EvalQualitySubmitResultDal.ConditionQueryCount(esr.TeacherId == model.TeacherId & esr.PeriodId == model.PeriodId) > 0;
			if (!exists)
			{
				db.EvalQualitySubmitResultDal.Insert(model);
			}

			return RedirectToAction("SubmitResultView", new
			{
				model.TeacherId,
				model.PeriodId,
				model.GroupId
			});
		}


		// GET: QualityEval/SubmitResultView

		public ActionResult SubmitResultView(QualityEvalParam param)
		{
			QualityEvalSubmitPeriodModel model = new QualityEvalSubmitPeriodModel(param);

			model.Period = db.EvalPeriodDal.PrimaryGet(model.PeriodId);
			model.Declare = model.GetDeclareInfo(db);

			if (!DeclareTargetIds.HasTeam(model.Declare.TargetId))
			{
				model.Message = "申报（担当）称号未纳入当期质评考评。";
			}
			else
			{
				var engine = EngineManager.Engines[model.Period.AnalysisType].QualityEvals[model.Declare.TargetId];

				model.AnalysisUnit = engine;
				model.Result = engine.GetSubmitResult(db, param);

				if (model.Result == null && param.GroupId == 0)
				{
					model.Message = "当期质评还未执行!";
				}
				else
				{
					model.EvalResults = engine.GetResults(db, param);

					if (param.GroupId != 0)
					{
						model.NotEvalExperts =
							APQuery.select(u.RealName)
							.from(egm,
								u.JoinInner(egm.ExpectID == u.UserId))
							.where(egm.GroupId == param.GroupId & egm.IsLeader == false & egm.ExpectID.NotIn(
								APQuery.select(er.Accesser)
									.from(er)
									.where(er.GroupId == param.GroupId
										 & er.PeriodId == param.PeriodId
										 & er.TeacherId == param.TeacherId
									 )
								))
							.query(db, r => r.GetString(0)).ToList();


						//  是否是组长

						model.IsLeader = db.ExpGroupMemberDal.ConditionQueryCount(
							egm.ExpectID == UserProfile.UserId &
							egm.GroupId == param.GroupId &
							egm.IsLeader == true) > 0;
					}
				}

				if (model.CanSubmit)
				{
					model.DoSubmit = new EvalQualitySubmitResult
					{
						PeriodId = param.PeriodId,
						DeclareTargetPKID = model.Declare.TargetId,
						TeacherId = param.TeacherId,
						FullScore = engine.FullScroe,
						Score = model.EvalResults.Count <= 0 ? 0 : model.EvalResults.Average(m => m.Score),
						Characteristic = model.EvalResults.Count <= 0 ? 0 : model.EvalResults.Average(m => m.Characteristic),
						AccesserCount = model.EvalResults.Count <= 0 ? 0 : model.EvalResults.Count,
						GroupId = param.GroupId,
						Accesser = UserProfile.UserId
					};
				}
			}

			return View("../EvalUtilities/ResultView", model);
		}


		//	POST-Ajax: QualityEval/ResultView

		//[HttpPost]
		public ActionResult ResultView(QualityEvalParam param)
		{
			//ThrowNotAjax();

			QualityEvalPeriodModel model = new QualityEvalPeriodModel(param);

			model.Period = db.EvalPeriodDal.PrimaryGet(model.PeriodId);
			model.Declare = model.GetDeclareInfo(db);

			var engine = EngineManager.Engines[model.Period.AnalysisType].QualityEvals[param.TargetId];

			model.AnalysisUnit = engine;
			model.Result = engine.GetResult(db, param);
			model.ResultItems = engine.GetResultItem(db, param);

			return View(engine.ResultView, model);
		}

	}

}