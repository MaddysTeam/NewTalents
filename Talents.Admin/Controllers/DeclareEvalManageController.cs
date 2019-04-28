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


		public ActionResult EvalSchoolMemberExport()
		{
			var pdfRender = new HtmlRender();
			var htmlText = pdfRender.RenderViewToString(this, "EvalSchoolMemberExport", null);
			byte[] pdfFile = FormatConverter.ConvertHtmlTextToPDF(htmlText);
			string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 10000);
			return new BinaryContentResult($"{fileName}.pdf", "application/pdf", pdfFile);
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