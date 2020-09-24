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

	public class HomeController : BaseController
	{

		static APDBDef.TeamMemberTableDef tm = APDBDef.TeamMember;
		static APDBDef.TeamActiveTableDef ta = APDBDef.TeamActive;
		static APDBDef.TeamActiveResultTableDef tar = APDBDef.TeamActiveResult;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;

		//	GET:	 Home/Index

		public ActionResult Index()
		{
			switch (UserProfile.UserType)
			{
				case BzRoleNames.Admin:
					return AdminIndex();
				case BzRoleNames.SchoolAdmin:
					return SchoolIndex();
				case BzRoleNames.Teacher:
					return TeacherIndex();
			}


			return View();
		}


		//	GET:	 Home/EvalPeriodAlert

		public ActionResult EvalPeriodAlert()
		{
			var t = APDBDef.EvalPeriod;

			var model = db.EvalPeriodDal.ConditionQuery(t.IsCurrent == true, null, null, null).FirstOrDefault();

			if (model == null)
				return Content("");


			return PartialView(model);
		}


		#region [ amdin ]


		//	GET: Home/AdminIndex

		public ActionResult AdminIndex()
		{
			return View("AdminIndex");
		}


		//	GET:	 Home/OverView

		public ActionResult OverView()
		{
			AdminOverViewModel model = new AdminOverViewModel();

			model.TeacherCount = db.BzUserProfileDal.ConditionQueryCount(u.UserType == BzRoleNames.Teacher);
			model.MemberCount = db.DeclareBaseDal.ConditionQueryCount(null);
			model.TeamCount = db.DeclareBaseDal.ConditionQueryCount(d.HasTeam == true);
			model.ExpertsGroupCount = db.ExpGroupDal.ConditionQueryCount(null);
			model.ExpertsCount = db.ExpGroupMemberDal.ConditionQueryCount(null);
			model.SchollCount = db.CompanyDal.ConditionQueryCount(null);


			return PartialView(model);
		}


		#endregion


		#region [ school]


		public ActionResult SchoolIndex()
		{
			return View("SchoolIndex");
		}


		#endregion


		#region [ teacher]


		public ActionResult Report()
		{

			var t = APDBDef.EvalVolumnResult;
			var t1 = APDBDef.EvalPeriod;

			var period = db.EvalPeriodDal.ConditionQuery(t1.IsCurrent == true, null, null, null).FirstOrDefault();

			if (EngineManager.Engines[period.AnalysisType].VolumnEvals.ContainsKey(UserProfile.TargetId))
			{
				var engine = EngineManager.Engines[period.AnalysisType].VolumnEvals[UserProfile.TargetId];

				var model = engine.AnalysisContent(db, new VolumnEvalParam { PeriodId = period.PeriodId, TeacherId = UserProfile.UserId, AccesserId = UserProfile.UserId }, UserProfile.UserId);

				return PartialView("Report", model ?? new EvalVolumnResult());
			}
			else
				throw new NotSupportedException("当前您的称号不支持该功能");
		}



		public ActionResult TeacherIndex()
		{
			var dr = APDBDef.DeclareReview;
			if (null != Period)
				ViewBag.DeclareReview = db.DeclareReviewDal.ConditionQuery(dr.TeacherId == UserProfile.UserId
				   & dr.StatusKey.In(new string[] { DeclareKeys.ReviewSuccess, DeclareKeys.ReviewFailure, DeclareKeys.ReviewProcess })
				   & dr.PeriodId == Period.PeriodId, null, null, null).FirstOrDefault();

			return View("TeacherIndex");
		}


		//	GET:	 Home/ActiveTotal

		public ActionResult DeclareActiveTotal()
		{
			var t = APDBDef.DeclareActive;


			var list = APQuery.select(t.Date.DateGroup(APSqlDateGroupMode.Year).As("Year"), t.DeclareActiveId.Count().As("activeCount"))
				.from(t)
				.where(t.TeacherId == UserProfile.UserId)
				.group_by(t.Date.DateGroup(APSqlDateGroupMode.Year))
				.take(5)
				.query(db, r =>
				{
					return new DeclareActiveTotalViewModel
					{
						Year = t.Date.GetValue(r, "Year"),
						ActiveCount = t.DeclareActiveId.GetValue(r, "activeCount")
					};
				}).ToList();


			return PartialView(list);
		}


		//	GET: Home/JoinTeam

		public ActionResult JoinTeam()
		{
			var model = APQuery.select(ta.TeamActiveId, ta.Title, tar.ResultId)
				.from(tm,
						ta.JoinInner(tm.TeamId == ta.TeamId),
						tar.JoinLeft(ta.TeamActiveId == tar.ActiveId & tar.MemberId == UserProfile.UserId))
				.where(tm.MemberId == UserProfile.UserId)
				.order_by(tar.ResultId.Asc, ta.Date.Desc)
				.take(5)
				.query(db, r =>
				{
					return new JoinTeamViewModel
					{
						ActiveId = ta.TeamActiveId.GetValue(r),
						Title = ta.Title.GetValue(r),
						IsComplete = tar.ResultId.GetValue(r) > 0
					};
				}).ToList();


			return PartialView(model);
		}


		//	GET: Home/LeaderTeam

		public ActionResult LeaderTeam()
		{
			var memberCount = db.TeamMemberDal.ConditionQueryCount(tm.TeamId == UserProfile.UserId);

			var list = APQuery.select(ta.TeamActiveId, ta.Title, tar.ActiveId.Count().As("resultCount"))
				.from(ta, tar.JoinLeft(ta.TeamActiveId == tar.ActiveId & tar.MemberId.In(APQuery.select(tm.MemberId).from(tm))))
				.where(ta.TeamId == UserProfile.UserId)
				//.order_by(ta.Date.Desc)
				.group_by(ta.TeamActiveId, ta.Title)
				.take(5)
				.query(db, r =>
				{
					return new LeaderTeamViewModel()
					{
						Title = ta.Title.GetValue(r),
						ResultCount = tar.ActiveId.GetValue(r, "resultCount"),
						MemberCount = memberCount
					};
				}).ToList();


			return PartialView(list);
		}


		//	GET: 	Home/EvalPeriodScores

		public ActionResult EvalPeriodScores()
		{
			var e = APDBDef.EvalPeriod;
			var es = APDBDef.EvalSchoolResult;
			var ev = APDBDef.EvalVolumnResult;
			var eq = APDBDef.EvalQualitySubmitResult;
			var userId = UserProfile.UserId;
			var currentPeriod = db.GetCurrentEvalPeriod();

			var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null)
			 .FirstOrDefault();

			var currentPeriodId = period == null ? 5106 : period.PeriodId;
			var list = APQuery.select(e.PeriodId, e.Name, es.Score.As("schoolScore"), es.FullScore.As("schoolFullScore"),
								ev.Score.As("volumnScore"), ev.FullScore.As("volumnFullScore"),
								eq.Score.As("qualityScore"), eq.FullScore.As("qualityFullScore"),
							   eq.Characteristic.As("characteristicScore"))
				.from(e,
						es.JoinLeft(e.PeriodId == es.PeriodId & es.TeacherId == userId),
						ev.JoinLeft(e.PeriodId == ev.PeriodId & ev.TeacherId == userId),
						eq.JoinLeft(e.PeriodId == eq.PeriodId & eq.TeacherId == userId))
				.group_by(e.PeriodId, e.Name, es.Score, es.FullScore, ev.Score, ev.FullScore, eq.Score, eq.FullScore, eq.Characteristic)
				.where(e.PeriodId == currentPeriodId) //TODO:  需要在检查下为什么会读取“考评周期”
				.take(5)
				.query(db, r =>
				{
					return new UserEvalViewModel()
					{
						PeriodId = e.PeriodId.GetValue(r),
						PeriodName = e.Name.GetValue(r),
						SchoolScore = ev.Score.GetValue(r, "schoolScore") * EvalAnalysis.AnnualEngine.SchoolEvalUnit.ProportionValue,
						SchoolFullScore = ev.Score.GetValue(r, "schoolFullScore") * EvalAnalysis.AnnualEngine.SchoolEvalUnit.ProportionValue,
						VolumnScore = ev.Score.GetValue(r, "volumnScore") * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue,
						VolumnFullScore = ev.Score.GetValue(r, "volumnFullScore") * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue,
						QualityScore = ev.Score.GetValue(r, "qualityScore") * EvalAnalysis.AnnualEngine.QualityEvalUnit.ProportionValue,
						QualityFullScore = ev.Score.GetValue(r, "qualityFullScore") * EvalAnalysis.AnnualEngine.QualityEvalUnit.ProportionValue,
						CharacteristicScore = ev.Score.GetValue(r, "characteristicScore"), // 特色分
						FullScore = EngineManager.Engines[currentPeriod.AnalysisType].FullScore
					};
				}).ToList();


			return PartialView(list);
		}


		//	GET: 	Home/EvalCommnets

		public ActionResult EvalComments()
		{
			var eqr = APDBDef.EvalQualityResult;
			var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null)
			   .FirstOrDefault();

			var ls = new List<EvalCommentAndScoreViewModel>();
			if (period != null)
			{
				var results = APQuery.select(eqr.Comment, u.UserName, eqr.ResultId)
				   .from(eqr, u.JoinInner(eqr.Accesser == u.UserId))
				   .where(eqr.TeacherId == UserProfile.UserId)
				   .query(db, r => new
				   {
					   resultId = eqr.ResultId.GetValue(r),
					   accessor = u.UserName.GetValue(r),
					   comment = string.IsNullOrEmpty(eqr.Comment.GetValue(r)) ? "暂无评价" : eqr.Comment.GetValue(r),
				   });
				foreach (var item in results)
				{
					ls.Add(new EvalCommentAndScoreViewModel
					{
						EvalResultId = item.resultId,
						EvalComment = item.comment,
						ExpertName = item.accessor
					});
				}
			}
			return PartialView(ls);
		}


		// TODO: for eval 2019

		public ActionResult TeamEvalResult()
		{
			ThrowNotAjax();

			var ter = APDBDef.TeamEvalResult;

			var result = db.TeamEvalResultDal.ConditionQuery(ter.MemberId == UserProfile.UserId & ter.PeriodId == EvalPeriod.PeriodId, null, null, null)?.FirstOrDefault();

			var list = new List<EvalCommentAndScoreViewModel>();
			if (null != result)
			{
				list.Add(new EvalCommentAndScoreViewModel
				{
					EvalResultId = result.ResultId,
					EvalComment = result.Comment,
					EvalTitle = EvalQualityRuleKeys.GerJiHx,
					Score = result.Score,
					FullScore = result.FullScore
				});
			}

			return PartialView("EvalResult", list);
		}


		// TODO: for eval 2019

		public ActionResult QualityEvalResult()
		{

			ThrowNotAjax();

			var list = new List<EvalCommentAndScoreViewModel>();

			var er = APDBDef.EvalQualityResult;

			if(null == EvalPeriod)
				return PartialView("EvalResult", list);

			var result =  db.EvalQualityResultDal.ConditionQuery(er.TeacherId == UserProfile.UserId & er.PeriodId == EvalPeriod.PeriodId, null, null, null).FirstOrDefault();

			list = new List<EvalCommentAndScoreViewModel>();

			for (int i = 0; i < 3 & result != null; i++)
			{
				list.Add(new EvalCommentAndScoreViewModel
				{
					EvalResultId = result.ResultId,
					Score = i == 0 ? result.DynamicScore1 : i == 1 ? result.DynamicScore2 : result.DynamicScore3,
					EvalTitle = i == 0 ? EvalQualityRuleKeys.SannGuih : i == 1 ? EvalQualityRuleKeys.TuandJih : EvalQualityRuleKeys.GerJiHx,
					EvalComment = i == 0 ? result.DynamicComment1 : i == 1 ? result.DynamicComment2 : result.DynamicComment3,
					FullScore = result.FullScore
				});
			}

			return PartialView("EvalResult", list);
		}



		//	GET:	 Home/EvalPeriodScores

		public ActionResult ExpGroups()
		{
			var egm = APDBDef.ExpGroupMember;
			var u = APDBDef.BzUserProfile;
			var eg = APDBDef.ExpGroup;

			//TODO: 是否需要判断下专家组不可能为空的逻辑

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


			ViewBag.periodId = db.GetCurrentEvalPeriod().PeriodId;


			return PartialView(result);
		}


		//	POST-Ajax:	Home/ExpGroupInfo

		[HttpPost]
		public ActionResult ExpGroupInfo(long groupId, long periodId)
		{
			ThrowNotAjax();


			var egm = APDBDef.ExpGroupMember;
			var u = APDBDef.BzUserProfile;
			var egt = APDBDef.ExpGroupTarget;
			var er = APDBDef.EvalQualityResult;
			var edr = APDBDef.EvalDeclareResult;

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
			var evalCount = 0;
			if (Period != null & Period.IsInDeclarePeriod)
			{
				evalCount = db.EvalDeclareResultDal.ConditionQueryCount(
				  edr.GroupId == groupId & edr.PeriodId == Period.PeriodId & edr.Accesser == UserProfile.UserId);
			}
			else
			{
				evalCount = db.EvalQualityResultDal.ConditionQueryCount(
				  er.GroupId == groupId & er.PeriodId == periodId & er.Accesser == UserProfile.UserId);
			}

			return Json(new
			{
				leaderName,
				memberNames = String.Join(", ", memberNames),
				evalCount,
				notEvalCount = totalCount - evalCount
			});
		}




		[HttpPost]
		public ActionResult GetTempScore()
		{
			var sql = @"select userId,isnull(totalScore,0) as totalScore from tempScore";

			TempScore score = new TempScore();
			var results = DapperHelper.QueryBySQL<TempScore>(sql);
			if (results.Count > 0)
			{
				score = results.Find(x => x.userId == UserProfile.UserId);
			}

			return Json(new {
				data=score
			});
		}


		#endregion

	}

	public class TempScore
	{
		public long userId { get; set; }
		public long totalScore { get; set; }
	}


}