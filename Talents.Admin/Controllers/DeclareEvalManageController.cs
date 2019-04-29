using Business;
using Business.Helper;
using Business.Utilities;
using NPOI.HSSF.UserModel;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using TheSite.EvalAnalysis;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class DeclareEvalManageController : BaseController
	{

		static APDBDef.DeclarePeriodTableDef dp = APDBDef.DeclarePeriod;
		static APDBDef.DeclareReviewTableDef dr = APDBDef.DeclareReview;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.EvalDeclareResultTableDef er = APDBDef.EvalDeclareResult;
		static APDBDef.ExpGroupMemberTableDef egm = APDBDef.ExpGroupMember;
		static APDBDef.ExpGroupTargetTableDef egt = APDBDef.ExpGroupTarget;
		static APDBDef.ExpGroupTableDef eg = APDBDef.ExpGroup;
		static APDBDef.CompanyTableDef c = APDBDef.Company;


		// GET: DeclareEval/SchoolEvalExport

		public ActionResult SchoolEvalExport(long? companyId)
		{
			APSqlSelectCommand query = APQuery.select(dr.TeacherId, dr.TeacherName, c.CompanyName,
					dr.DeclareTargetPKID, dr.DeclareSubjectPKID,
					er.Score, er.FullScore, er.ResultId, er.GroupId)
				.from(dr,
						 er.JoinLeft(er.TeacherId == dr.TeacherId & er.GroupId == 0),
						 c.JoinInner(dr.CompanyId == c.CompanyId)
						)
				.where(dr.PeriodId == Period.PeriodId
					 // & dr.CompanyId == UserProfile.CompanyId
					 & dr.StatusKey == DeclareKeys.ReviewSuccess
					 & dr.DeclareTargetPKID.In(new long[] { DeclareTargetIds.GongzsZhucr, DeclareTargetIds.XuekDaitr, DeclareTargetIds.GugJiaos }));

			if (UserProfile.IsSchoolAdmin)
				query.where_and(dr.CompanyId == UserProfile.CompanyId);
			else if (UserProfile.IsSystemAdmin && companyId != null && companyId > 0)
				query.where_and(dr.CompanyId == companyId.Value);

			var dic = query.query(db, r =>
			{
				var resultId = er.ResultId.GetValue(r, "ResultId");

				return new InsepectionDeclareSchoolEvalResult
				{
					Id = dr.TeacherId.GetValue(r),
					TeacherName = dr.TeacherName.GetValue(r),
					DeclareCompany = c.CompanyName.GetValue(r),
					DeclareTarget = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(r), "", false),
					Score = er.Score.GetValue(r).ToString(),
					FullScore = "100",
					Status = resultId == 0 ? "未评审" : "已评审",
				};
			}).ToDictionary(x => x.Id);

			//创建Excel文件的对象
			var book = CreateBook(dic);

			// 写入到客户端 
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			book.Write(ms);
			ms.Seek(0, SeekOrigin.Begin);
			DateTime dt = DateTime.Now;
			string dateTime = dt.ToString("yyyyMMdd");
			string fileName = $"{UserProfile.CompanyName}教师申报评审表" + dateTime + ".xls";
			return File(ms, "application/vnd.ms-excel", fileName);
		}


		// GET: DeclareEval/EvalSchoolMemberExport

		public ActionResult EvalSchoolMemberExport()
		{
			var pdfRender = new HtmlRender();
			var htmlText = pdfRender.RenderViewToString(this, "EvalSchoolMemberExport", null);
			byte[] pdfFile = FormatConverter.ConvertHtmlTextToPDF(htmlText);
			string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 10000);
			return new BinaryContentResult($"{fileName}.pdf", "application/pdf", pdfFile);
		}


		// GET: DeclareEval/ExpertEvalOverview

		public ActionResult ExpertEvalOverview()
		{
			var periodId = Period.PeriodId;
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

				return new ExpertEvalOverviewModels
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


		// GET:  DeclareEvalManage/EvalExpertMemberList
		// POST-Ajax: DeclareEvalManage/EvalExpertMemberList

		public ActionResult EvalExpertMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult EvalExpertMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long companyId)
		{
			ThrowNotAjax();

			var c = APDBDef.Company;
			var periodId = Period.PeriodId;

			var query = APQuery.select(er.TeacherId, dr.TeacherName,
					dr.DeclareTargetPKID, dr.DeclareSubjectPKID, dr.CompanyId,eg.Name.As("GroupName"),
					er.Score.Sum().As("EvalScore"), er.FullScore, c.CompanyName)
				.from(er,
						 dr.JoinInner(er.TeacherId == dr.TeacherId),
						 c.JoinInner(dr.CompanyId == c.CompanyId),
						 egt.JoinInner(egt.MemberId==dr.TeacherId),
						 eg.JoinInner(eg.GroupId==egt.GroupId)
						)
				.where(dr.StatusKey == DeclareKeys.ReviewSuccess & er.GroupId > 0, er.PeriodId == periodId)
				.group_by(er.TeacherId,dr.TeacherName, dr.DeclareTargetPKID, 
				          dr.DeclareSubjectPKID, dr.CompanyId, eg.Name, er.FullScore, c.CompanyName)
				.primary(er.ResultId)
				.skip((current - 1) * rowCount)
				.take(rowCount);

			if (groupId > 0)
				query.where_and(er.GroupId == groupId);

			if (companyId > 0)
				query.where_and(dr.CompanyId == companyId);

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
				var score = er.Score.GetValue(r, "EvalScore");
				var targetId = dr.DeclareTargetPKID.GetValue(r);
				var engine = EngineManager.Engines[Period.AnalysisType].DeclareEvals;
				var fullScore = engine[targetId].ExpertFullScore;

				return new
				{
					id = er.TeacherId.GetValue(r),
					realName = dr.TeacherName.GetValue(r),
					targetId = dr.DeclareTargetPKID.GetValue(r),
					target = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(r)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(r)),
					company = c.CompanyName.GetValue(r),
					score = string.Format("{0} / {1}", score, fullScore),
					group= eg.Name.GetValue(r, "GroupName")
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


		public ActionResult EvalExpertDetails()
		{
			return null;
		}

		// GET: DeclareEval/NotEvalExpertMemberList
		// POST-Ajax: DeclareEval/NotEvalExpertMemberList

		public ActionResult NotEvalExpertMemberList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult NotEvalExpertMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long companyId)
		{
			ThrowNotAjax();

			var periodId = Period.PeriodId;

			var subQuery = APQuery.select(er.TeacherId).from(er).where(er.PeriodId == periodId);
			if (groupId > 0)
				subQuery.where_and(er.GroupId == groupId);

			var query = APQuery.select(egt.MemberId, dr.TeacherName, dr.DeclareTargetPKID,
									   dr.DeclareSubjectPKID, c.CompanyName,
									   eg.Name.As("GroupName")
									   )
				.from(egt,
						 dr.JoinInner(dr.TeacherId == egt.MemberId),
						 c.JoinInner(dr.CompanyId==c.CompanyId),
						 eg.JoinInner(eg.GroupId==egt.GroupId)
						)
				.where( dr.StatusKey == DeclareKeys.ReviewSuccess & egt.MemberId.NotIn(subQuery))
				.primary(egt.MemberId)
				.skip((current - 1) * rowCount)
				.take(rowCount);

			if (groupId > 0)
				query = query.where_and(egt.GroupId==groupId);

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
					realName = dr.TeacherName.GetValue(rd),
					target = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(rd)),
					subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(rd)),
					targetId = dr.DeclareTargetPKID.GetValue(rd),
					company=c.CompanyName.GetValue(rd),
					group=eg.Name.GetValue(rd, "GroupName")
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


		#region [ Helper ]

		private HSSFWorkbook CreateBook<T>(Dictionary<long, T> dic) where T : class
		{
			//创建Excel文件的对象
			NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
			//添加一个sheet
			NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

			#region [头部设计]

			var i = 0;
			//给sheet1添加第一行的头部标题
			NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
			foreach (var item in typeof(T).GetProperties())
			{
				if (item.PropertyType == typeof(string))
				{
					var display = item.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
					row1.CreateCell(i).SetCellValue(display.Name);
					i++;
				}
			}

			#endregion

			i = 0;
			foreach (var item in dic.Values)
			{
				i++;
				NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);
				var properties = item.GetType().GetProperties();
				var j = 0;
				foreach (var subItem in properties)
				{
					if (subItem.PropertyType == typeof(string))
					{
						rowtemp.CreateCell(j).SetCellValue(subItem.GetValue(item, null).ToString());
						j++;
					}
				}
			}

			return book;
		}

		#endregion
	}

}