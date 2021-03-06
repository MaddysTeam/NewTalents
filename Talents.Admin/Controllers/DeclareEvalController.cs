﻿using Business;
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

	public class DeclareEvalController : BaseController
	{
		static APDBDef.DeclarePeriodTableDef dp = APDBDef.DeclarePeriod;
		static APDBDef.DeclareReviewTableDef dr = APDBDef.DeclareReview;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.EvalDeclareResultTableDef er = APDBDef.EvalDeclareResult;
		static APDBDef.ExpGroupMemberTableDef egm = APDBDef.ExpGroupMember;
		static APDBDef.ExpGroupTargetTableDef egt = APDBDef.ExpGroupTarget;
		static APDBDef.ExpGroupTableDef eg = APDBDef.ExpGroup;
		static APDBDef.CompanyTableDef c = APDBDef.Company;

		//// GET: DeclareEval/Index

		public ActionResult Index()
		{
			if (Period == null)
			{
				return View("../EvalPeriod/NotInAccessRegion");
			}

			return RedirectToAction("List", new { Period.PeriodId });
		}


		//// GET: DeclareEval/BlockList
		//// POST-Ajax: DeclareEval/BlockList

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

			var subQuery = APQuery.select(dr.TeacherId).from(dr).where(dr.StatusKey == DeclareKeys.ReviewSuccess);
			var totalCount = db.ExpGroupTargetDal.ConditionQueryCount(egt.GroupId == groupId & egt.MemberId.In(subQuery));
			var evalCount = db.EvalDeclareResultDal.ConditionQueryCount(
				 er.GroupId == groupId & er.PeriodId == periodId & er.Accesser == UserProfile.UserId);


			return Json(new
			{
				leaderName,
				memberNames = String.Join(", ", memberNames),
				evalCount,
				notEvalCount = totalCount - evalCount
			});
		}


		// GET: DeclareEval/EvalExpertMemberList
		// POST-Ajax: DeclareEval/EvalExpertMemberList

		public ActionResult EvalExpertMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult EvalExpertMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long periodId)
		{
			ThrowNotAjax();

			var c = APDBDef.Company;

			var query = APQuery.select(er.TeacherId, er.ResultId.As("ResultId"), dr.TeacherName,
					dr.DeclareTargetPKID, dr.DeclareSubjectPKID, dr.CompanyId,
					er.Score, er.FullScore, c.CompanyName)
				.from(er,
						 dr.JoinInner(er.TeacherId == dr.TeacherId),
						 u.JoinInner(er.TeacherId == u.UserId),
						 c.JoinInner(dr.CompanyId == c.CompanyId)
						)
				.where(dr.StatusKey == DeclareKeys.ReviewSuccess & er.GroupId == groupId, er.PeriodId == periodId & er.Accesser == UserProfile.UserId)
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
				query.order_by(er.DeclareTargetPKID.Asc).order_by_add(er.Score.Desc);
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, r =>
			{
				var score = er.Score.GetValue(r);
				var fullScore = er.FullScore.GetValue(r);

				return new
				{
					id = er.TeacherId.GetValue(r),
					resultId = er.ResultId.GetValue(r, "ResultId"),
					realName = dr.TeacherName.GetValue(r),
					targetId = dr.DeclareTargetPKID.GetValue(r),
					target = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(r)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(r)),
					company = c.CompanyName.GetValue(r),
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


		// GET: DeclareEval/NotEvalExpertMemberList
		// POST-Ajax: DeclareEval/NotEvalExpertMemberList

		public ActionResult NotEvalExpertMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult NotEvalExpertMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long periodId)
		{
			ThrowNotAjax();


			var subQuery = APQuery.select(er.TeacherId)
				.from(er)
				.where(er.Accesser == UserProfile.UserId & er.GroupId == groupId & er.PeriodId == periodId);

			var query = APQuery.select(egt.MemberId, dr.TeacherName, dr.DeclareTargetPKID, dr.DeclareSubjectPKID, c.CompanyName)
				.from(egt,
						 dr.JoinInner(dr.TeacherId == egt.MemberId),
						 c.JoinInner(c.CompanyId == dr.CompanyId)
						)
				.where(egt.GroupId == groupId & dr.StatusKey == DeclareKeys.ReviewSuccess & egt.MemberId.NotIn(subQuery))
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
					case "target": query.order_by(sort.OrderBy(dr.DeclareTargetPKID)); break;
					case "subject": query.order_by(sort.OrderBy(dr.DeclareSubjectPKID)); break;
				}
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				return new
				{
					id = egt.MemberId.GetValue(rd),
					company = c.CompanyName.GetValue(rd),
					realName = dr.TeacherName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(rd)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(rd)),
					targetId = dr.DeclareTargetPKID.GetValue(rd),
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


		// GET: DeclareEval/EvalSchoolMemberList
		// POST-Ajax: DeclareEval/EvalSchoolMemberList

		public ActionResult EvalSchoolMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult EvalSchoolMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long companyId, long statusId)
		{
			ThrowNotAjax();

			var query = APQuery.select(dr.TeacherId, dr.TeacherName, c.CompanyName,
				  dr.DeclareTargetPKID, dr.DeclareSubjectPKID,
				  er.ResultId.As("ResultId"), er.Score, er.FullScore, er.ResultId, er.GroupId, er.Comment)
			   .from(dr,
					  er.JoinLeft(er.TeacherId == dr.TeacherId & er.GroupId == 0),
					  c.JoinInner(dr.CompanyId == c.CompanyId)
					 )
			   .where(dr.PeriodId == Period.PeriodId
				   & dr.StatusKey == DeclareKeys.ReviewSuccess
				   & (dr.DeclareTargetPKID.In(new long[] {
                 // DeclareTargetIds.GongzsZhucr, DeclareTargetIds.XuekDaitr, DeclareTargetIds.GugJiaos
                 DeclareTargetIds.JiaoxNengs,DeclareTargetIds.JiaoxXinx
               } ) | dr.IsDowngrade == true
				   ))
			   .primary(dr.TeacherId)
			   .skip((current - 1) * rowCount)
			   .take(rowCount);

			if (UserProfile.IsSchoolAdmin)
				query.where_and(dr.CompanyId == UserProfile.CompanyId);
			else if (UserProfile.IsSystemAdmin && companyId > 0)
				query.where_and(dr.CompanyId == companyId);

			if (statusId == 1)
			{
				query.where_and(er.ResultId > 0);
			}
			else if (statusId == 0)
			{
				query.where_and(er.ResultId == null);
			}

			//过滤条件
			//模糊搜索姓名

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(dr.TeacherName.Match(searchPhrase));
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "realName": query.order_by(sort.OrderBy(dr.TeacherName)); break;
					case "score": query.order_by(sort.OrderBy(er.Score)); break;
				}
			}
			else
			{
				query.order_by(er.DeclareTargetPKID.Asc).order_by_add(er.Score.Desc);
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, r =>
			{
				var score = er.Score.GetValue(r);
				var fullScore = er.FullScore.GetValue(r);
				var status = er.ResultId.GetValue(r);

				return new
				{
					id = dr.TeacherId.GetValue(r),
					periodId = Period.PeriodId,
					realName = dr.TeacherName.GetValue(r),
					target = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(r)),
					company = c.CompanyName.GetValue(r),
					subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(r)),
					score = string.Format("{0} / {1}", score, fullScore),
					submitStatus = status == 0 ? "未评审" : "已评审",
					targetId = dr.DeclareTargetPKID.GetValue(r),
					resultId = er.ResultId.GetValue(r, "ResultId"),
					shid = er.Comment.GetValue(r)
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


		// GET: DeclareEval/EvalSpecialExpertMemberList  这里special expert 用于培训处的考核
		// POST-Ajax: DeclareEval/EvalSpecialExpertMemberList

		public ActionResult NotEvalSpecialExpertMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult NotEvalSpecialExpertMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long companyId, long targetId, long subjectId)
		{
			ThrowNotAjax();


			APSqlSelectCommand subQuery = null;

			if (UserProfile.IsSpecialExpert)
				subQuery = APQuery.select(er.TeacherId).from(er).where(er.Accesser == UserProfile.UserId & er.PeriodId == Period.PeriodId);
			else if (UserProfile.IsSystemAdmin)
				subQuery = APQuery.select(er.TeacherId).from(er).where(er.GroupId == -1 & er.PeriodId == Period.PeriodId);

			var query = APQuery.select(dr.TeacherId, dr.TeacherName, c.CompanyName,
									   dr.DeclareTargetPKID, dr.DeclareSubjectPKID)
			   .from(dr,
					  c.JoinInner(dr.CompanyId == c.CompanyId)
					 )
			   .where(dr.PeriodId == Period.PeriodId
				   & dr.StatusKey == DeclareKeys.ReviewSuccess
				   & dr.DeclareTargetPKID.In(new long[] { DeclareTargetIds.GongzsZhucr, DeclareTargetIds.XuekDaitr, DeclareTargetIds.GugJiaos })
				   & dr.TeacherId.NotIn(subQuery)
				   )
			   .primary(dr.DeclareReviewId)
			   .skip((current - 1) * rowCount)
			   .take(rowCount);

			if (companyId > 0)
				query.where_and(dr.CompanyId == companyId);

			if (targetId > 0)
				query.where_and(dr.DeclareTargetPKID == targetId);

			if (subjectId > 0)
				query.where_and(dr.DeclareSubjectPKID == subjectId);

			//过滤条件
			//模糊搜索姓名

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(dr.TeacherName.Match(searchPhrase));
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "realName": query.order_by(sort.OrderBy(dr.TeacherName)); break;
					case "score": query.order_by(sort.OrderBy(er.Score)); break;
				}
			}


			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, r =>
			{
				return new
				{
					id = dr.TeacherId.GetValue(r),
					periodId = Period.PeriodId,
					realName = dr.TeacherName.GetValue(r),
					company = c.CompanyName.GetValue(r),
					target = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(r)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(r)),
					submitStatus = "未评审",
					targetId = dr.DeclareTargetPKID.GetValue(r),
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

		public ActionResult EvalSpecialExpertMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult EvalSpecialExpertMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long companyId, long targetId, long subjectId)
		{
			ThrowNotAjax();

			var query = APQuery.select(dr.TeacherId, dr.TeacherName, c.CompanyName,
				  dr.DeclareTargetPKID, dr.DeclareSubjectPKID,
				  er.ResultId.As("ResultId"), er.Score, er.FullScore, er.ResultId, er.GroupId)
			   .from(dr,
					  er.JoinLeft(er.TeacherId == dr.TeacherId),
					  c.JoinInner(dr.CompanyId == c.CompanyId)
					 )
			   .where(dr.PeriodId == Period.PeriodId
				   & dr.StatusKey == DeclareKeys.ReviewSuccess
				   & er.GroupId == -1
				   & dr.DeclareTargetPKID.In(new long[] { DeclareTargetIds.GongzsZhucr, DeclareTargetIds.XuekDaitr, DeclareTargetIds.GugJiaos }))
			   .primary(dr.DeclareReviewId)
			   .skip((current - 1) * rowCount)
			   .take(rowCount);

			if (UserProfile.IsSpecialExpert)
			{
				query.where(er.Accesser == UserProfile.UserId);
			}

			if (companyId > 0)
				query.where_and(dr.CompanyId == companyId);

			if (targetId > 0)
				query.where_and(dr.DeclareTargetPKID == targetId);

			if (subjectId > 0)
				query.where_and(dr.DeclareSubjectPKID == subjectId);



			//过滤条件
			//模糊搜索姓名

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(dr.TeacherName.Match(searchPhrase));
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "realName": query.order_by(sort.OrderBy(dr.TeacherName)); break;
					case "score": query.order_by(sort.OrderBy(er.Score)); break;
				}
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, r =>
			{
				var score = er.Score.GetValue(r);
				var currentTargetId = dr.DeclareTargetPKID.GetValue(r);
				var engine = EngineManager.Engines[Period.AnalysisType].DeclareEvals;
				var fullScore = engine[currentTargetId].SpecialFullScore;

				return new
				{
					id = dr.TeacherId.GetValue(r),
					periodId = Period.PeriodId,
					realName = dr.TeacherName.GetValue(r),
					company = c.CompanyName.GetValue(r),
					target = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(r)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(r)),
					score = string.Format("{0} / {1}", score, fullScore),
					submitStatus = "已评审",
					targetId = currentTargetId,
					resultId = er.ResultId.GetValue(r, "ResultId")
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


		//	GET: DeclareEval/Eval
		//	POST: DecalreEval/Eval

		[NoCache]
		public ActionResult Eval(DeclareEvalParam param)
		{
			param.AccesserId = UserProfile.UserId;
			param.PeriodId = Period.PeriodId;

			var isEvalSubmit = false;

			var engines = EngineManager.Engines[Period.AnalysisType].DeclareEvals;
			if (isEvalSubmit || !engines.ContainsKey(param.TargetId))
			{
				throw new ApplicationException("当前不支持该学员考评");
			}

			var engine = engines[param.TargetId];

			var result = db.EvalDeclareResultDal.ConditionQuery(
				er.PeriodId == param.PeriodId &
				er.TeacherId == param.TeacherId &
				er.Accesser == param.AccesserId, null, null, null)
				.FirstOrDefault();

			param.ResultId = result == null ? 0 : result.ResultId;

			ViewBag.Result = engine.GetResult(db, param);

			ViewBag.ResultItems = engine.GetResultItem(db, param);

			ViewBag.Declare = param.GetDeclareReviewInfo(db);

			return View(engine.EvalView, param);
		}

		[HttpPost]
		public ActionResult Eval(DeclareEvalParam param, FormCollection fc)
		{
			var engine = EngineManager.Engines[Period.AnalysisType].DeclareEvals;

			db.BeginTrans();

			try
			{
				param.PeriodId = Period.PeriodId;

				var resultId = engine[param.TargetId].Eval(db, param, fc);

				db.Commit();

				if (UserProfile.IsSchoolAdmin)
				{
					return RedirectToAction("EvalSchoolMemberList");
				}
				else if (UserProfile.IsExpert)
				{
					return RedirectToAction("NotEvalExpertMemberList", new
					{
						teacherId = param.TeacherId,
						periodId = param.PeriodId,
						groupId = param.GroupId
					});
				}
				else if (UserProfile.IsSpecialExpert)
				{
					return RedirectToAction("EvalSpecialExpertMemberList");
				}

				return null;

			}
			catch
			{
				db.Rollback();
				throw;
			}
		}


		//	POST-Ajax: DeclareEval/ResultView

		public ActionResult ResultView(DeclareEvalParam param)
		{
			ThrowNotAjax();

			var model = new EvalDeclareResultModel(param);
			var engine = EngineManager.Engines[Period.AnalysisType].DeclareEvals[model.TargetId];

			//model.AnalysisUnit = engine;
			model.Result = engine.GetResult(db, param);
			model.ResultItems = engine.GetResultItem(db, param);

			return PartialView(engine.ResultView, model);
		}


	}

}