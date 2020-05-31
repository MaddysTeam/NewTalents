using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class SchoolEvalManageController : BaseController
	{

		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;
		static APDBDef.CompanyTableDef c = APDBDef.Company;
		static APDBDef.CompanyAccesserTableDef ca = APDBDef.CompanyAccesser;
		static APDBDef.CompanyDeclareTableDef cd = APDBDef.CompanyDeclare;
		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.EvalSchoolResultTableDef er = APDBDef.EvalSchoolResult;
		static APDBDef.EvalSchoolResultItemTableDef eri = APDBDef.EvalSchoolResultItem;


		// GET: SchoolEvalManage/Overview

		public ActionResult Overview(long periodId = 0)
		{
			if (periodId == 0)
			{
				var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null)
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


			var query = APQuery.select(c.CompanyId, c.CompanyName, cd.TeacherId.Count().As("TotalCount"), er.TeacherId.Count().As("EvalCount"))
							.from(c,
								cd.JoinLeft(c.CompanyId == cd.CompanyId),
								er.JoinLeft(c.CompanyId == er.CompanyId & er.TeacherId == cd.TeacherId & er.PeriodId == periodId))
							.group_by(c.CompanyId, c.CompanyName);

			var result = query.query(db, rd =>
			{
				var totalCount = rd.GetInt32(rd.GetOrdinal("TotalCount"));
				var evalCount = rd.GetInt32(rd.GetOrdinal("EvalCount"));

				return new SchoolEvalOverviewModel
				{
					CompanyId = c.CompanyId.GetValue(rd),
					PeriodId = periodId,
					CompanyName = c.CompanyName.GetValue(rd),
					TotalMemberCount = totalCount,
					EvalMemberCount = evalCount,
					EvalStatus = totalCount == evalCount && totalCount > 0
					? EvalStatus.Success : totalCount > evalCount && evalCount > 0
					? EvalStatus.Pending : EvalStatus.NotStart
				};
			}).ToList();


			return View(result);
		}


		// GET: SchoolEvalManage/EvalMemberList
		// POST: SchoolEvalManage/EvalMemberList

		public ActionResult EvalMemberList(int periodId)
		{
			return View();
		}

		[HttpPost]
		public ActionResult EvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, int periodId, long companyId)
		{
			ThrowNotAjax();

			var ualias = APDBDef.BzUserProfile.As("ualias");

			var query = APQuery.select(er.ResultId, er.PeriodId, er.TeacherId, c.CompanyName, u.RealName,
				er.AccessDate, er.Score, er.FullScore, ualias.RealName.As("Accesser"))
				.from(er,
						cd.JoinInner(er.TeacherId == cd.TeacherId),
						c.JoinInner(cd.CompanyId == c.CompanyId),
						u.JoinInner(er.TeacherId == u.UserId),
						ualias.JoinInner(er.Accesser == ualias.UserId)
						)
				.where(er.PeriodId == periodId)
				.primary(er.TeacherId)
			.skip((current - 1) * rowCount)
				.take(rowCount);


			if (companyId > 0)
				query.where_and(c.CompanyId == companyId);


			//过滤条件
			//模糊搜索姓名

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(u.RealName.Match(searchPhrase));
			}

			query.order_by(c.CompanyName.Asc);

			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
					case "score": query.order_by(sort.OrderBy(er.Score)); break;
					case "company": query.order_by(sort.OrderBy(c.CompanyName)); break;
					case "accesser": query.order_by(sort.OrderBy(er.Accesser)); break;
					case "accessDate": query.order_by(sort.OrderBy(er.AccessDate)); break;
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
					company = c.CompanyName.GetValue(rd),
					realName = u.RealName.GetValue(rd),
					accessDate = er.AccessDate.GetValue(rd),
					score = string.Format("{0} / {1}", score, fullScore),
					accesser = ualias.RealName.GetValue(rd, "Accesser")
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


		// GET: SchoolEvalManage/NotEvalMemberList
		// POST: SchoolEvalManage/NotEvalMemberList

		public ActionResult NotEvalMemberList(int periodId)
		{
			ViewBag.PeriodId = periodId;

			return View();
		}

		[HttpPost]
		public ActionResult NotEvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, int periodId, string companyName)
		{
			ThrowNotAjax();

			var subquery = APQuery.select(er.TeacherId)
				.from(er)
				.where(er.PeriodId == periodId);

			var query = APQuery.select(d.TeacherId, u.RealName, c.CompanyName)
			   .from(d,
					 u.JoinInner(d.TeacherId == u.UserId),
					 cd.JoinInner(cd.TeacherId == u.UserId),
					 c.JoinInner(c.CompanyId == cd.CompanyId))
			   .where(d.TeacherId.NotIn(subquery))
			   .primary(d.TeacherId)
			   .order_by(c.CompanyName.Asc)
			   .skip((current - 1) * rowCount)
			   .take(rowCount);


			if (!string.IsNullOrEmpty(companyName) && companyName != SelectNames.All)
				query.where_and(u.CompanyName.Match(companyName) | u.CompanyNameOuter.Match(companyName));

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
					case "company": query.order_by(sort.OrderBy(c.CompanyName)); break;
				}
			}

			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				return new
				{
					periodId = periodId,
					company = c.CompanyName.GetValue(rd),
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


		// GET: SchoolEvalManage/List

		public ActionResult List()
		{
			var list = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == false, null, null, null);

			return View(list);
		}


		// GET: DeclareEval/EvalSchoolMemberExport

		public ActionResult EvalSchoolMemberExport(int periodId, bool? isLowDeclareLevel)
		{
			var companyId = UserProfile.CompanyId;
			var results = GetDeclareSchoolEvalResultViewModels(companyId, periodId, isLowDeclareLevel);
			var company = db.CompanyDal.PrimaryGet(UserProfile.CompanyId);
			string companyName = company.CompanyName;

			if (isLowDeclareLevel != null)
			{
				string subTitle = isLowDeclareLevel.Value ? "2019学年度教学能手，教学新秀，特招学员" : "2019学年度骨干及以上层级";
				companyName = string.Format("{0}{1}", companyName, subTitle);
			}
			var viewModel = new ExpertDeclareSchoolViewModel { CompanyName = companyName, Results = results };
			var pdfRender = new HtmlRender();
			var htmlText = pdfRender.RenderViewToString(this, "EvalSchoolMemberExport", viewModel);
			byte[] pdfFile = Business.Utilities.FormatConverter.ConvertHtmlTextToPDF(htmlText);
			string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 10000);
			return new BinaryContentResult($"{fileName}.pdf", "application/pdf", pdfFile);
		}


		private List<InsepctionDeclareSchoolEvalResult> GetDeclareSchoolEvalResultViewModels(long? companyId, int periodId, bool? isLowDeclareLevel)
		{
			APSqlSelectCommand query = APQuery.select(d.TeacherId, u.RealName, c.CompanyName,
				  d.DeclareTargetPKID, d.DeclareSubjectPKID,
				  er.Score, er.FullScore, er.ResultId, er.Morality)
			   .from(d,
					  u.JoinLeft(u.UserId == d.TeacherId),
					  er.JoinLeft(er.TeacherId == d.TeacherId & er.PeriodId==periodId),
					  c.JoinLeft(er.CompanyId == c.CompanyId)
					 )
				//.where(er.PeriodId == periodId)
				.order_by(d.DeclareTargetPKID.Asc)
				.order_by_add(er.Score.Desc);

			if (isLowDeclareLevel != null)
			{
				query = isLowDeclareLevel.Value ?
					query.where_and(d.DeclareTargetPKID > DeclareTargetIds.GugJiaos) :
					query.where_and(d.DeclareTargetPKID <= DeclareTargetIds.GugJiaos);
			}

			if (UserProfile.IsSchoolAdmin)
				query.where_and(u.CompanyId == UserProfile.CompanyId);
			else if (UserProfile.IsSystemAdmin && companyId != null && companyId > 0)
				query.where_and(u.CompanyId == companyId.Value);

			var results = query.query(db, r =>
			{
				var resultId = er.ResultId.GetValue(r, "ResultId");
				var targetId = d.DeclareTargetPKID.GetValue(r);
				var shid = er.Morality.GetValue(r);
				return new InsepctionDeclareSchoolEvalResult
				{
					Id = er.TeacherId.GetValue(r),
					TeacherName = u.RealName.GetValue(r),
					DeclareCompany = c.CompanyName.GetValue(r),
					DeclareTarget = DeclareBaseHelper.DeclareTarget.GetName(targetId, "", false),
					DeclareSubject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(r)),
					Score = shid == "不合格" ? "0" : er.Score.GetValue(r).ToString(),
					FullScore = 100,//TODO: engine[targetId].CompanyFullScore,
					Status = resultId == 0 ? "未评审" : "已评审",
					Shid = shid
				};
			}).ToList();

			return results;
		}

	}


}