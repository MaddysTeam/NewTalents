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


		// GET: DeclareEval/EvalSchoolMemberExport

		public ActionResult EvalSchoolMemberExport()
		{
			var companyId = UserProfile.CompanyId;
			var results = GetDeclareSchoolEvalResultViewModels(companyId);

			var company = db.CompanyDal.PrimaryGet(UserProfile.CompanyId);
			var viewModel = new ExpertDeclareSchoolViewModel { CompanyName = company.CompanyName, Results = results };

			var pdfRender = new HtmlRender();
			var htmlText = pdfRender.RenderViewToString(this, "EvalSchoolMemberExport", viewModel);
			byte[] pdfFile = FormatConverter.ConvertHtmlTextToPDF(htmlText);
			string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 10000);
			return new BinaryContentResult($"{fileName}.pdf", "application/pdf", pdfFile);
		}


		// GET: DeclareEval/EvalSchoolMemberExcelExport
		// GET: DeclareEval/EvllExpertMemberExcelExport

		public ActionResult EvalSchoolMemberExcelExport()
		{
			var companyId = UserProfile.CompanyId;
			var results = GetDeclareSchoolEvalResultViewModels(companyId).ToDictionary(x=>x.Id,y=>y);
			var book=NPOIHelper.CreateBook(results);

			// 写入到客户端 
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			book.Write(ms);
			ms.Seek(0, SeekOrigin.Begin);
			DateTime dt = DateTime.Now;
			string dateTime = dt.ToString("yyyyMMdd");
			string fileName = "单位评分汇总表" + dateTime + ".xls";
			return File(ms, "application/vnd.ms-excel", fileName);
		}

		public ActionResult EvalExpertMemberExcelExport()
		{
			var results = GetExpertEvalResults().ToDictionary(x => x.id, y => y);
			var book = NPOIHelper.CreateBook(results);

			// 写入到客户端 
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			book.Write(ms);
			ms.Seek(0, SeekOrigin.Begin);
			DateTime dt = DateTime.Now;
			string dateTime = dt.ToString("yyyyMMdd");
			string fileName = "单位评分汇总表" + dateTime + ".xls";
			return File(ms, "application/vnd.ms-excel", fileName);
		}


		// GET: DeclareEval/ExpertEvalOverview

		public ActionResult ExpertEvalOverview()
		{
			var periodId = Period.PeriodId;
			var query = APQuery.select(eg.GroupId, eg.Name, er.TeacherId, u.RealName)
							 .from(eg,
								 egt.JoinLeft(eg.GroupId == egt.GroupId),
								 egm.JoinLeft(eg.GroupId== egm.GroupId),
								 er.JoinLeft(er.TeacherId == egt.MemberId & er.Accesser==egm.ExpectID & er.GroupId > 0),
								 u.JoinLeft(egm.ExpectID == u.UserId)
								 )
							 .group_by(eg.GroupId, eg.Name, egt.MemberId, er.TeacherId, u.RealName);

			var grp = query.query(db, r => new
			{
				groupId = eg.GroupId.GetValue(r),
				groupName = eg.Name.GetValue(r),
				assessor= u.RealName.GetValue(r),
				teacherId = er.TeacherId.GetValue(r)
			}).ToList();

			var results = grp.GroupBy(x => new { x.groupId, x.groupName,x.assessor }).Select(y =>
			{
				var memberCount = y.Count();
				var evalMemberCount = y.Count(z => z.teacherId > 0);
				return new ExpertEvalOverviewModels
				{
					PeriodId = periodId,
					GroupId = y.Key.groupId,
					GroupName = y.Key.groupName,
					Accessor = y.Key.assessor,
					GroupTargetMemberCount = y.Count(),
					EvalTargetMemberCount = y.Count(z => z.teacherId > 0),
					NotEvalTargetMemberCount = memberCount - evalMemberCount,
					EvalStatus = memberCount == evalMemberCount && memberCount > 0 ? EvalStatus.Success
								   : memberCount > evalMemberCount && evalMemberCount > 0 ? EvalStatus.Pending
								   : EvalStatus.NotStart
				};
			}).ToList();


			return View(results);
		}


		// GET:  DeclareEvalManage/EvalExpertMemberList
		// POST-Ajax: DeclareEvalManage/EvalExpertMemberList

		public ActionResult EvalExpertMemberList()
		{
			EnsureIsMaster();

			return View();
		}

		[HttpPost]
		public ActionResult EvalExpertMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long companyId)
		{
			EnsureIsMaster();

			ThrowNotAjax();

			var c = APDBDef.Company;
			var periodId = Period.PeriodId;

			var query = APQuery.select(er.TeacherId, dr.TeacherName,
				  dr.DeclareTargetPKID, dr.DeclareSubjectPKID, dr.CompanyId, eg.Name.As("GroupName"),
				  er.Score.Avg().As("EvalScore"), er.FullScore, c.CompanyName)
			   .from(er,
					  dr.JoinInner(er.TeacherId == dr.TeacherId),
					  c.JoinInner(dr.CompanyId == c.CompanyId),
					  egt.JoinInner(egt.MemberId == dr.TeacherId),
					  eg.JoinInner(eg.GroupId == egt.GroupId)
					 )
			   .where(dr.StatusKey == DeclareKeys.ReviewSuccess & er.GroupId > 0 & er.PeriodId == periodId)
			   .group_by(er.TeacherId, dr.TeacherName, dr.DeclareTargetPKID,
					   dr.DeclareSubjectPKID, dr.CompanyId, eg.Name, er.FullScore, c.CompanyName);

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
					group = eg.Name.GetValue(r, "GroupName")
				};
			}).ToList();

			var total = result.Count;
			if (total > 0)
			{
				result = result.Skip((current - 1) * rowCount).Take(rowCount).ToList();
			}

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
			EnsureIsMaster();

			return View();
		}

		[HttpPost]
		public ActionResult NotEvalExpertMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long companyId)
		{
			EnsureIsMaster();

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
					  c.JoinInner(dr.CompanyId == c.CompanyId),
					  eg.JoinInner(eg.GroupId == egt.GroupId)
					 )
			   .where(dr.StatusKey == DeclareKeys.ReviewSuccess & egt.MemberId.NotIn(subQuery))
			   .primary(dr.DeclareReviewId)
			   .skip((current - 1) * rowCount)
			   .take(rowCount);

			if (groupId > 0)
				query = query.where_and(egt.GroupId == groupId);

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
					company = c.CompanyName.GetValue(rd),
					group = eg.Name.GetValue(rd, "GroupName")
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


		public ActionResult EvalExpertMemberDetails(long teacherId)
		{
			EnsureIsMaster();

			var result = APQuery.select(er.ResultId, er.Score, u.UserName, er.TeacherId, er.PeriodId, er.DeclareTargetPKID)
			   .from(er, u.JoinInner(er.Accesser == u.UserId))
			   .where(er.TeacherId == teacherId & er.GroupId > 0)
			   .query(db, r => new EvalDeclareResult
			   {
				   ResultId = er.ResultId.GetValue(r),
				   AccesserName = u.UserName.GetValue(r),
				   Score = er.Score.GetValue(r),
				   TeacherId = er.TeacherId.GetValue(r),
				   PeriodId = er.PeriodId.GetValue(r),
				   DeclareTargetPKID = er.DeclareTargetPKID.GetValue(r)
			   }).ToList();

			return View(result);
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


		private List<InsepctionDeclareSchoolEvalResult> GetDeclareSchoolEvalResultViewModels(long? companyId)
		{
			APSqlSelectCommand query = APQuery.select(dr.TeacherId, dr.TeacherName, c.CompanyName,
				  dr.DeclareTargetPKID, dr.DeclareSubjectPKID,
				  er.Score, er.FullScore, er.ResultId, er.GroupId, er.Comment)
			   .from(dr,
					  er.JoinLeft(er.TeacherId == dr.TeacherId & er.GroupId == 0),
					  c.JoinInner(dr.CompanyId == c.CompanyId)
					 )
			   .where(dr.PeriodId == Period.PeriodId
				   & dr.StatusKey == DeclareKeys.ReviewSuccess
               & dr.DeclareTargetPKID.In(new long[] { DeclareTargetIds.JiaoxNengs, DeclareTargetIds.JiaoxXinx}))
             //& dr.DeclareTargetPKID.In(new long[] { DeclareTargetIds.GongzsZhucr, DeclareTargetIds.XuekDaitr, DeclareTargetIds.GugJiaos })) TODO:
            .order_by(dr.DeclareTargetPKID.Asc)
				.order_by_add(er.Score.Desc);

			if (UserProfile.IsSchoolAdmin)
				query.where_and(dr.CompanyId == UserProfile.CompanyId);
			else if (UserProfile.IsSystemAdmin && companyId != null && companyId > 0)
				query.where_and(dr.CompanyId == companyId.Value);

			var engine = EngineManager.Engines[Period.AnalysisType].DeclareEvals;

			var results = query.query(db, r =>
			{
				var resultId = er.ResultId.GetValue(r, "ResultId");
				var targetId = dr.DeclareTargetPKID.GetValue(r);
				var shid = er.Comment.GetValue(r);
				return new InsepctionDeclareSchoolEvalResult
				{
					Id = dr.TeacherId.GetValue(r),
					TeacherName = dr.TeacherName.GetValue(r),
					DeclareCompany = c.CompanyName.GetValue(r),
					DeclareTarget = DeclareBaseHelper.DeclareTarget.GetName(targetId, "", false),
					DeclareSubject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(r)),
					Score = shid == "不合格" ? "0" : er.Score.GetValue(r).ToString(),
					FullScore = engine[targetId].CompanyFullScore,
					Status = resultId == 0 ? "未评审" : "已评审",
					Shid = er.Comment.GetValue(r)
				};
			}).ToList();

			return results;
		}


		private List<DeclareExperEvalManageViewModel> GetExpertEvalResults()
		{
			var sql = @"select 
						accessor,
						id,
						teacherId,
						targetId,
						subjectId,
						companyId,
						teacher,
						target,
						subject,
						company,
						avg(totalScore) totalScore,
						isnull(avg([教育教学.公开课]),0) as gkk,
						isnull(avg([教育教学.评比.区级以上]),0) as jxpb,
						isnull(avg([教育教学.评比.其他]),0) as qt,
						isnull(avg([教研工作.中小学命题]),0) as mt,
						isnull(avg([教研工作.担任评委]),0) as pingw,
						isnull(avg([教研工作.德育]),0) as dey,
						isnull(avg([教研工作.担任评委2]),0) as drpw2,
						isnull(avg([教育科研.立项课题或项目研究]),0) as xmyj,
						isnull(avg([教育科研.发表论文]),0) as fblw,
						isnull(avg([教师培训.培训课程]),0) as jspx,
						isnull(avg([教师培训.专题讲座]),0) as ztjz,
						isnull(avg([个人特色.专著]),0) as zz,
						isnull(avg([个人特色.其他身份]),0) as qtsf,
						isnull(avg([个人特色.学员成长]),0) as xycz
						from (
						select 
							e.ResultId as 'id',
							dr.teacherid as 'teacherId',
							dr.DeclareTargetPKID as 'targetId',
							dr.DeclareSubjectPKID as 'subjectId',
							dr.CompanyId as 'companyId',
							dr.TeacherName as 'teacher',
							p1.Name as 'target',
							p2.Name as 'subject',
							c.CompanyName as 'company',
							e.Score as 'totalScore',
							i.EvalItemKey,case i.ResultValue
								when '合格' then 1 
								when '不合格'then 0
								else cast(i.ResultValue as float) end score 
							,u.username as 'accessor'	
						from EvalDeclareResult e
						join EvalDeclareResultItem i 
						on e.ResultId=i.ResultId
						join DeclareReview dr 
						on dr.TeacherId=e.teacherId
						join PicklistItem p1 
						on p1.PicklistItemId =dr.DeclareTargetPKID
						join PicklistItem p2 
						on p2.PicklistItemId=dr.DeclareSubjectPKID
						join Company c
						on c.companyId=dr.CompanyId
						join BzUserProfile u
						on e.Accesser=u.UserId
						where  e.periodid=5005 and dr.statusKey='审核通过' and e.GroupId>0
						) c
						pivot(sum(c.score) for c.EvalItemKey in (
						[教育教学.公开课],
						[教育教学.评比.区级以上],
						[教育教学.评比.其他],
						[教研工作.中小学命题],
						[教研工作.担任评委],
						[教研工作.德育],
						[教研工作.担任评委2],
						[教育科研.立项课题或项目研究],
						[教育科研.发表论文],
						[教师培训.专题讲座],
						[教师培训.培训课程],
						[个人特色.专著],
						[个人特色.其他身份],
						[个人特色.学员成长]
						)) AS T
						group by t.teacherId,T.teacher,T.target,T.subject,T.company,T.targetId,T.companyId,T.subjectId,t.accessor,t.id
						";

			var results= DapperHelper.QueryBySQL<DeclareExperEvalManageViewModel>(sql);
			foreach(var item in results)
			{
				var score = new double[] { item.zz, item.qtsf, item.xycz }.Max()
					+ new double[] { item.jxpb,item.qt}.Max()
					+ new double[] { item.gkk,item.mt,item.pingw,item.dey,item.drpw2,item.xmyj,item.fblw,item.jspx,item.ztjz }.Sum();

				item.totalScore = Math.Round(score,2);
			}

			return results;
		}


		private void EnsureIsMaster()
		{
			if (!UserProfile.IsSystemAdmin) throw new ApplicationException("没有权限查看");
		}

		#endregion

	}

}