using Business;
using Business.Helper;
using Business.Utilities;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class TeamEvalManageController : BaseController
	{

		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;
		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.TeamEvalResultTableDef er = APDBDef.TeamEvalResult;
		static APDBDef.TeamEvalResultItemTableDef eri = APDBDef.TeamEvalResultItem;
		static APDBDef.TeamMemberTableDef t = APDBDef.TeamMember;

		// GET: TeamEvalManage/Overview

		public ActionResult Overview(long periodId = 0)
		{
			if (periodId == 0)
			{
				var period = db.GetCurrentEvalPeriod();

				if (period == null)
				{
					return View("../EvalPeriod/NotInAccessRegion");
				}
				else
				{
					return RedirectToAction("Overview", new { periodId = period.PeriodId });
				}
			}

			var query = APQuery.select(t.TeamId, u.RealName,
												t.MemberId.Count().As("TotalCount"),
												er.ResultId.Count().As("EvalCount"))
								  .from(t,
										u.JoinLeft(u.UserId == t.TeamId),
										er.JoinLeft(er.MemberId == t.MemberId & er.PeriodId == periodId)
										)
								  .group_by(t.TeamId, u.RealName);

			var result = query.query(db, rd =>
			{
				var memberCount = rd.GetInt32(rd.GetOrdinal("TotalCount"));
				var evalMemberCount = rd.GetInt32(rd.GetOrdinal("EvalCount"));

				return new TeamEvalOverviewModels
				{
					PeriodId = periodId,
					TeamId = t.TeamId.GetValue(rd),
					TeamName = u.RealName.GetValue(rd) + "团队",
					TeamMemberCount = memberCount,
					TeamEvalTargetMemberCount = evalMemberCount,
					EvalStatus = memberCount == evalMemberCount && memberCount > 0 ? EvalStatus.Success
										: memberCount > evalMemberCount && evalMemberCount > 0 ? EvalStatus.Pending
										: EvalStatus.NotStart
				};
			}).ToList();


			return View(result);
		}


		// GET: TeamEvalManage/EvalMemberList
		// POST: TeamEvalManage/EvalMemberList

		public ActionResult EvalMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult EvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, int periodId)
		{
			ThrowNotAjax();

			var l = APDBDef.BzUserProfile.As("leader");
			var dl = APDBDef.DeclareBase.As("declareLeader");

			var query = APQuery.select(u.RealName, d.DeclareTargetPKID, dl.DeclareSubjectPKID, dl.DeclareStagePKID, l.RealName.As("leaderRealName"),
				                      er.MemberId, er.Score, er.FullScore, er.Score, er.AccessDate,er.TeamId)
				.from(t,
						er.JoinInner(er.MemberId == t.MemberId),
						u.JoinInner(er.MemberId == u.UserId),
						l.JoinInner(er.Accesser == l.UserId),
						d.JoinInner(d.TeacherId == u.UserId),
						dl.JoinInner(dl.TeacherId==l.UserId)
					  )
				.where(er.PeriodId == periodId)
				.primary(t.MemberId)
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
					case "targetName": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
					case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
					case "score": query.order_by(sort.OrderBy(er.Score)); break;
					case "accessDate": query.order_by(sort.OrderBy(er.AccessDate)); break;
				}
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				var fullScore = er.FullScore.GetValue(rd);
				var score = er.Score.GetValue(rd);
				var accessorSubject = DeclareBaseHelper.DeclareSubject.GetName(dl.DeclareSubjectPKID.GetValue(rd));
				var accessorStage = DeclareBaseHelper.DeclareStage.GetName(dl.DeclareStagePKID.GetValue(rd));

				return new
				{
					teacherId = er.MemberId.GetValue(rd),
					teamId= er.TeamId.GetValue(rd),
					teamName = $"{accessorStage}{accessorSubject}{l.RealName.GetValue(rd, "leaderRealName")}",
					targetName = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					realName = u.RealName.GetValue(rd),
					accessDate = er.AccessDate.GetValue(rd),
					score = string.Format("{0} / {1}", er.Score.GetValue(rd), fullScore),
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


		// GET:  TeamEvalManage/NotEvalMemberList
		// POST: TeamEvalManage/NotEvalMemberList

		public ActionResult NotEvalMemberList(long periodId)
		{
			return View();
		}

		[HttpPost]
		public ActionResult NotEvalMemberList(long teamId, long periodId, int current, int rowCount, AjaxOrder sort, string searchPhrase)
		{
			ThrowNotAjax();

			var l = APDBDef.BzUserProfile.As("teamLeader");

			var subquery = APQuery.select(er.MemberId)
					   .from(er)
					   .where(er.PeriodId == periodId);

			var query = APQuery.select(u.RealName, d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID,
												t.TeamId, u.UserId, l.RealName.As("leader"))
				.from(t,
						d.JoinInner(d.TeacherId == t.TeamId),
						u.JoinInner(u.UserId == t.MemberId),
						l.JoinInner(l.UserId == t.TeamId)
						)
				.where(d.TeacherId.NotIn(subquery))
					.primary(u.UserId);


			if (teamId > 0)
			{
				query = query.where(t.TeamId == teamId);
			}

			query = query.skip((current - 1) * rowCount).take(rowCount);


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
				var target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd));
				var subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd));
				var stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd));

				return new
				{
					teacherId = u.UserId.GetValue(rd),
					teamId = t.TeamId.GetValue(rd),
					teamName = $"{stage}{subject}{l.RealName.GetValue(rd, "leader")}",
					realName = u.RealName.GetValue(rd),
					target,
					subject,
					stage
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


		// Get: TeamEvalManage/Export

		public ActionResult Export(long periodId)
		{
			var l = APDBDef.BzUserProfile.As("leader");
			var dl = APDBDef.DeclareBase.As("declareLeader");

			var query = APQuery.select(u.RealName, d.DeclareTargetPKID, dl.DeclareSubjectPKID, dl.DeclareStagePKID, l.RealName.As("leaderRealName"),
									   er.Score, er.FullScore, er.Score, er.AccessDate,er.Comment,er.ResultId)
				.from(t,
						er.JoinInner(er.MemberId == t.MemberId),
						u.JoinInner(er.MemberId == u.UserId),
						l.JoinInner(er.Accesser == l.UserId),
						d.JoinInner(d.TeacherId == u.UserId),
						dl.JoinInner(dl.TeacherId == l.UserId)
					  )
				.where(er.PeriodId == periodId)
				.primary(t.MemberId);


			var total = db.ExecuteSizeOfSelect(query);

			var results = query.query(db, rd =>
			{
				var fullScore = er.FullScore.GetValue(rd);
				var score = er.Score.GetValue(rd);
				var accessorSubject = DeclareBaseHelper.DeclareSubject.GetName(dl.DeclareSubjectPKID.GetValue(rd));
				var accessorStage = DeclareBaseHelper.DeclareStage.GetName(dl.DeclareStagePKID.GetValue(rd));

				return new TeamEvalResultExportModels
				{
					ResultId = er.ResultId.GetValue(rd),
					EvalTitle = Business.Helper.EvalQualityRuleKeys.GerJiHx,
					TeamName = $"{accessorStage}{accessorSubject}{l.RealName.GetValue(rd, "leaderRealName")}",
					TargetName = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					TeacherName = u.RealName.GetValue(rd),
					AccessDate = er.AccessDate.GetValue(rd).ToString("yyyy-MM-dd"),
					Score = string.Format("{0} / {1}", er.Score.GetValue(rd), fullScore),
					EvalComment=er.Comment.GetValue(rd)
				};
			}).ToDictionary(x=>x.ResultId);

			var book = NPOIHelper.CreateBook(results);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			book.Write(ms);
			ms.Seek(0, System.IO.SeekOrigin.Begin);
			string dateTime = DateTime.Now.ToString("yyyyMMdd");
			string fileName = "团队考核汇总表" + dateTime + ".xls";
			return File(ms, "application/vnd.ms-excel", fileName);
		}


		// GET: TeamEvalManage/List

		public ActionResult List()
		{
			var list = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == false, null, null, null);

			return View(list);
		}

	}


}