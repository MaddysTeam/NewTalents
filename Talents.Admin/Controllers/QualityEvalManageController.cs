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

	public class QualityEvalManageController : BaseController
	{
		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;
		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.EvalQualityResultTableDef er = APDBDef.EvalQualityResult;
		static APDBDef.EvalQualityResultItemTableDef eri = APDBDef.EvalQualityResultItem;
		//static APDBDef.EvalQualitySubmitResultTableDef esr = APDBDef.EvalQualitySubmitResult;
		static APDBDef.ExpGroupMemberTableDef egm = APDBDef.ExpGroupMember;
		static APDBDef.ExpGroupTargetTableDef egt = APDBDef.ExpGroupTarget;
		static APDBDef.ExpGroupTableDef eg = APDBDef.ExpGroup;


		// GET: QualityEvalManage/Overview

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

			var query = APQuery.select(eg.GroupId, eg.Name,
												egt.MemberId.Count().As("TotalCount"),
												er.ResultId.Count().As("EvalCount"))
								  .from(eg,
										egt.JoinLeft(eg.GroupId == egt.GroupId),
										er.JoinLeft(er.TeacherId == egt.MemberId & er.PeriodId == periodId)
										)
								  .group_by(eg.GroupId, eg.Name);

			var result = query.query(db, rd =>
			{
				var memberCount = rd.GetInt32(rd.GetOrdinal("TotalCount"));
				var evalMemberCount = rd.GetInt32(rd.GetOrdinal("EvalCount"));

				return new QualityEvalOverviewModels
				{
					PeriodId = periodId,
					GroupId = eg.GroupId.GetValue(rd),
					GroupName = eg.Name.GetValue(rd),
					GroupTargetMemberCount = memberCount,
					EvalTargetMemberCount = evalMemberCount,
					EvalStatus = memberCount == evalMemberCount && memberCount > 0 ? EvalStatus.Success
										: memberCount > evalMemberCount && evalMemberCount > 0 ? EvalStatus.Pending
										: EvalStatus.NotStart
				};
			}).ToList();


			return View(result);
		}


		// GET: QualityEvalManage/EvalMemberList
		// POST: QualityEvalManage/EvalMemberList

		public ActionResult EvalMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult EvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, int periodId)
		{
			ThrowNotAjax();


			var query = APQuery.select(u.RealName, d.DeclareTargetPKID,d.DeclareSubjectPKID,d.DeclareStagePKID, er.Accesser, er.ResultId, er.TeacherId, er.Score, er.FullScore, er.GroupId, er.PeriodId,
									   er.DynamicScore1, er.DynamicScore2, er.DynamicScore3, er.AccessDate)
				.from(egt,
						er.JoinInner(er.TeacherId == egt.MemberId),
						u.JoinInner(egt.MemberId == u.UserId),
						d.JoinInner(d.TeacherId == u.UserId)
					  )
				.where(er.PeriodId == periodId)
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
					case "targetName": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
					case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
					//case "score": query.order_by(sort.OrderBy(er.Score)); break;
					case "dynamicScore1": query.order_by(sort.OrderBy(er.DynamicScore1)); break;
					case "dynamicScore2": query.order_by(sort.OrderBy(er.DynamicScore2)); break;
					case "dynamicScore3": query.order_by(sort.OrderBy(er.DynamicScore3)); break;

					case "accessDate": query.order_by(sort.OrderBy(er.AccessDate)); break;
				}
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				var dynamicScore1 = er.DynamicScore1.GetValue(rd);
				var dynamicScore2 = er.DynamicScore2.GetValue(rd);
				var dynamicScore3 = er.DynamicScore3.GetValue(rd);
				var leaderSubject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd));
				var leaderStage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd));

				var fullScore = er.FullScore.GetValue(rd);
				return new
				{
					resultId = er.ResultId.GetValue(rd),
					teacherId = er.TeacherId.GetValue(rd),
					periodId = er.PeriodId.GetValue(rd),
					groupId = er.GroupId.GetValue(rd),
					accesserId = er.Accesser.GetValue(rd),
					TeamName = $"{leaderStage}{leaderSubject}{u.RealName.GetValue(rd)}",
					targetName = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					realName = u.RealName.GetValue(rd),
					accessDate = er.AccessDate.GetValue(rd),
					dynamicScore1 = string.Format("{0} / {1}", dynamicScore1, fullScore),
					dynamicScore2 = string.Format("{0} / {1}", dynamicScore2, fullScore),
					dynamicScore3 = string.Format("{0} / {1}", dynamicScore3, fullScore),
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


		// GET: QualityEvalManage/NotEvalMemberList
		// POST: QualityEvalManage/NotEvalMemberList

		public ActionResult NotEvalMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult NotEvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, int periodId)
		{
			ThrowNotAjax();


			var subQuery = APQuery.select(er.TeacherId)
			.from(er)
		   .where(er.PeriodId == periodId);

			var query = APQuery.select(egt.MemberId, eg.Name, u.RealName)
			.from(egt,
				  eg.JoinInner(egt.GroupId == eg.GroupId),
				  u.JoinInner(egt.MemberId == u.UserId)
				 )
			.where(egt.MemberId.NotIn(subQuery))
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
					case "groupName": query.order_by(sort.OrderBy(eg.Name)); break;
					case "teacherName": query.order_by(sort.OrderBy(u.RealName)); break;
				}
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				return new
				{
					id = egt.MemberId.GetValue(rd),
					groupName = eg.Name.GetValue(rd),
					teacherName = u.RealName.GetValue(rd)
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


		// GET: QualityEvalManage/Export

		public ActionResult Export(long periodId)
		{
			var t = APDBDef.TeamMember;
			var a = APDBDef.BzUserProfile.As("Accessor");

			var query = APQuery.select(u.RealName, d.DeclareTargetPKID, er.Accesser, er.ResultId, er.TeacherId, er.Score, er.FullScore, er.GroupId, er.PeriodId,
									   er.DynamicScore1, er.DynamicScore2, er.DynamicScore3, er.AccessDate, er.DynamicComment1, er.DynamicComment2, er.DynamicComment3,
									   d.DeclareSubjectPKID, d.DeclareStagePKID, a.RealName.As("AccessorName")
									   )
				.from(egt,
						er.JoinInner(er.TeacherId == egt.MemberId),
						u.JoinInner(egt.MemberId == u.UserId),
						d.JoinInner(d.TeacherId == u.UserId),
						t.JoinInner(t.MemberId==u.UserId),
						a.JoinInner(a.UserId==er.Accesser)
					  )
				.where(er.PeriodId == periodId)
				.primary(egt.MemberId);

			var results = query.query(db, rd =>
			{
				var dynamicScore1 = er.DynamicScore1.GetValue(rd);
				var dynamicScore2 = er.DynamicScore2.GetValue(rd);
				var dynamicScore3 = er.DynamicScore3.GetValue(rd);
				var leaderSubject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd));
				var leaderStage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd));

				var fullScore = er.FullScore.GetValue(rd);
				return new QualityEvalResultExportModels
				{
					ResultId = er.ResultId.GetValue(rd),
					TargetName = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
					TeamName = $"{leaderStage}{leaderSubject}{u.RealName.GetValue(rd)}",
					TeacherName = u.RealName.GetValue(rd),
					Accessor=a.RealName.GetValue(rd,"AccessorName"),
					AccessDate = er.AccessDate.GetValue(rd).ToString("yyyy-MM-dd"),
					EvalScore1 = string.Format("{0} / {1}", dynamicScore1, fullScore),
					EvalScore2 = string.Format("{0} / {1}", dynamicScore2, fullScore),
					EvalScore3 = string.Format("{0} / {1}", dynamicScore3, fullScore),
					EvalComment1 = er.DynamicComment1.GetValue(rd),
					EvalComment2 = er.DynamicComment2.GetValue(rd),
					EvalComment3 = er.DynamicComment3.GetValue(rd),
				};
			}).ToDictionary(x => x.ResultId);


			var book = NPOIHelper.CreateBook(results);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			book.Write(ms);
			ms.Seek(0, System.IO.SeekOrigin.Begin);
			string dateTime = DateTime.Now.ToString("yyyyMMdd");
			string fileName = "专家考核汇总表" + dateTime + ".xls";
			return File(ms, "application/vnd.ms-excel", fileName);


		}


		// GET: QualityEvalManage/List

		public ActionResult List()
		{
			var list = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == false, null, null, null);

			return View(list);
		}

	}


}