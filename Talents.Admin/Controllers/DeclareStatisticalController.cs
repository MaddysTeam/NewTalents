using Business;
using Business.Helper;
using Business.Utilities;
using Dapper;
using NPOI.HSSF.UserModel;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class DeclareStatisticalController : BaseController
	{

		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.DeclareMaterialTableDef dm = APDBDef.DeclareMaterial;
		static APDBDef.DeclarePeriodTableDef p = APDBDef.DeclarePeriod;
		static APDBDef.DeclareReviewTableDef df = APDBDef.DeclareReview;
		static APDBDef.DeclareProfileTableDef dp = APDBDef.DeclareProfile;
		static APDBDef.CompanyTableDef c = APDBDef.Company;
		static APDBDef.PicklistItemTableDef pi = APDBDef.PicklistItem;


		//	GET:	DeclareStatistical/DeclareProfileList
		// POST-Ajax: DeclareStatistical/DeclareProfileList

		public ActionResult DeclareProfileList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult DeclareProfileList( long eduBgId, long degreeId,
		   long skillId, long politicalId, long nationId, long subjectId, long stageId, long rankId, long allowFlowToSchool, long allowFlowToDowngrade,
		   int current, int rowCount, AjaxOrder sort, string searchPhrase, long companyId = 0)
		{
			var query = APQuery
					.select(dp.Asterisk, c.CompanyName.As("Company"), pi.Name.As("targetName"), df.AllowFlowToDowngrade, df.AllowFlowToSchool)
					.from(dp,
					p.JoinInner(p.PeriodId == dp.PeriodId),
					pi.JoinInner(dp.DeclareTargetPKID == pi.PicklistItemId),
					df.JoinInner(df.PeriodId == dp.PeriodId & df.TeacherId == dp.UserId & df.DeclareTargetPKID == dp.DeclareTargetPKID),
					c.JoinLeft(c.CompanyId == dp.CompanyId)
					)
			.primary(dp.UserId)
			.where(dp.PeriodId == Period.PeriodId & df.StatusKey != string.Empty & df.StatusKey != DeclareKeys.ReviewBack & df.StatusKey != DeclareKeys.ReviewFailure);
			//.skip((current - 1) * rowCount)
			//.take(rowCount);

			if (UserProfile.IsSchoolAdmin)
			{
				query.where_and(dp.CompanyId == UserProfile.CompanyId | df.CompanyId == UserProfile.CompanyId);
			}
			else if (UserProfile.IsSystemAdmin && companyId > 0)
			{
				query.where_and(dp.CompanyId == companyId | df.CompanyId == companyId);
			}
			if (eduBgId > 0)
			{
				query.where_and(dp.EduBgPKID == eduBgId);
			}
			if (degreeId > 0)
			{
				query.where_and(dp.EduDegreePKID == degreeId);
			}
			if (skillId > 0)
			{
				query.where_and(dp.SkillTitlePKID == skillId);
			}
			if (politicalId > 0)
			{
				query.where_and(dp.PoliticalStatusPKID == politicalId);
			}
			if (nationId > 0)
			{
				query.where_and(dp.NationalityPKID == nationId);
			}
			if (subjectId > 0)
			{
				query.where_and(dp.EduSubjectPKID == subjectId);
			}
			if (stageId > 0)
			{
				query.where_and(dp.EduStagePKID == stageId);
			}
			if (rankId > 0)
			{
				query.where_and(dp.RankTitlePKID == rankId);
			}
			if (allowFlowToSchool > 0)
			{
				bool rs = allowFlowToSchool == 1;
				query.where_and(df.AllowFlowToSchool == rs);
			}
			if (allowFlowToDowngrade > 0)
			{
				bool rs = allowFlowToDowngrade == 1;
				query.where_and(df.AllowFlowToDowngrade == rs);
			}

			//过滤条件
			//模糊搜索姓名,标题

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(dp.RealName.Match(searchPhrase));
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "RealName": query.order_by(sort.OrderBy(dp.RealName)); break;
					case "Gender": query.order_by(sort.OrderBy(dp.GenderPKID)); break;
					case "CompanyName": query.order_by(sort.OrderBy(c.CompanyName)); break;
					case "Birthday": query.order_by(sort.OrderBy(dp.Birthday)); break;
					case "Nationality": query.order_by(sort.OrderBy(dp.NationalityPKID)); break;
					case "Hiredate": query.order_by(sort.OrderBy(dp.Hiredate)); break;
					case "TrainNo": query.order_by(sort.OrderBy(dp.TrainNo)); break;
					case "RankTitle": query.order_by(sort.OrderBy(dp.RankTitlePKID)); break;
					case "EduBg": query.order_by(sort.OrderBy(dp.EduBgPKID)); break;
					case "EduDegree": query.order_by(sort.OrderBy(dp.EduDegreePKID)); break;
					case "DeclareTargetName": query.order_by(sort.OrderBy(dp.DeclareTargetPKID)); break;
					case "SkillTitle": query.order_by(sort.OrderBy(dp.SkillTitlePKID)); break;
					case "EduStage": query.order_by(sort.OrderBy(dp.EduStagePKID)); break;
					case "EduSubject": query.order_by(sort.OrderBy(dp.EduSubjectPKID)); break;
					case "CourseCountPerWeek": query.order_by(sort.OrderBy(dp.CourseCountPerWeek)); break;
				}
			}


			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				DeclareProfile profile = new DeclareProfile();
				dp.Fullup(rd, profile, false);
				profile.CompanyName = c.CompanyName.GetValue(rd, "Company");
				profile.DeclareTargetName = pi.Name.GetValue(rd, "TargetName");
				profile.AllowFlowToSchool = df.AllowFlowToSchool.GetValue(rd) ? "是" : "否";
				profile.AllowFlowToDowngrade = df.AllowFlowToDowngrade.GetValue(rd) ? "是" : "否";

				return profile;
			}).ToList();

			if (result.Count > 0)
			{
				result = result.Skip((current <= 1 ? 0 : current - 1) * rowCount).Take(rowCount).ToList();
			}


			return Json(new
			{
				rows = result,
				current,
				rowCount,
				total
			});
		}


		//	GET:	DeclareStatistical/DeclareSummaryList
		// POST-Ajax: DeclareStatistical/DeclareSummaryList

		public ActionResult DeclareSummaryList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult DeclareSummaryList(long targetId, long declareCompanyId, long decalreSubjectId, long subjectId,
		   long skillId, long allowFlowToSchool, long allowFlowToDowngrade, long isMaterialBroke, long isDeclareBroke,
		   int current, int rowCount, AjaxOrder sort, string searchPhrase)
		{
			// 申报汇总表了包含：学校名称、申报称号、申报学科、姓名、性别、出生年月、任教学科、联系方式（手机）、年度考核优秀（年份）、是否破格（普通申报、职称破格和材料破格）

			var query = APQuery
				  .select(df.Asterisk, c.CompanyName, pi.Name.As("DeclareTarget"),
						  dp.EduSubjectPKID, dp.GenderPKID, dp.Dynamic1, dp.Dynamic2, dp.Dynamic3, dp.Phonemobile, dp.SkillTitlePKID, dp.Birthday)
				  .from(df,
				  p.JoinInner(p.PeriodId == df.PeriodId),
				  pi.JoinInner(df.DeclareTargetPKID == pi.PicklistItemId),
				  dp.JoinInner(dp.UserId == df.TeacherId & dp.PeriodId == df.PeriodId & df.DeclareTargetPKID == dp.DeclareTargetPKID),
				  c.JoinLeft(c.CompanyId == df.CompanyId)
				  )
				  .primary(df.DeclareReviewId)
		  .where(df.StatusKey != string.Empty & df.StatusKey != DeclareKeys.ReviewBack & df.StatusKey != DeclareKeys.ReviewFailure)
		  .skip((current - 1) * rowCount)
		  .take(rowCount);

			if (UserProfile.IsSchoolAdmin)
			{
				query.where_and(df.CompanyId == UserProfile.CompanyId);
			}
			else if (UserProfile.IsSystemAdmin && declareCompanyId > 0)
			{
				query.where_and(df.CompanyId == declareCompanyId);
			}

			if (targetId > 0)
			{
				query.where_and(df.DeclareTargetPKID == targetId);
			}
			if (decalreSubjectId > 0)
			{
				query.where_and(df.DeclareSubjectPKID == decalreSubjectId);
			}
			if (subjectId > 0)
			{
				query.where_and(dp.EduSubjectPKID == subjectId);
			}
			if (skillId > 0)
			{
				query.where_and(dp.SkillTitlePKID == skillId);
			}
			if (allowFlowToSchool > 0)
			{
				bool rs = allowFlowToSchool == 1;
				query.where_and(df.AllowFlowToSchool == rs);
			}
			if (allowFlowToDowngrade > 0)
			{
				bool rs = allowFlowToDowngrade == 1;
				query.where_and(df.AllowFlowToDowngrade == rs);
			}
			if (isDeclareBroke > 0)
			{
				bool rs = isDeclareBroke == 1;
				query.where_and(df.IsBrokenRoles == rs);
			}
			if (isMaterialBroke > 0)
			{
				bool rs = isMaterialBroke == 1;
				query.where_and(df.TypeKey.Match("材料破格"));
			}

			//过滤条件
			//模糊搜索姓名,标题

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(df.TeacherName.Match(searchPhrase));
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "TeacherName": query.order_by(sort.OrderBy(df.TeacherName)); break;
					case "Gender": query.order_by(sort.OrderBy(dp.GenderPKID)); break;
					case "DeclareTargetName": query.order_by(sort.OrderBy(df.DeclareTargetPKID)); break;
					case "DeclareCompnay": query.order_by(sort.OrderBy(df.CompanyId)); break;
					case "Subject": query.order_by(sort.OrderBy(dp.EduSubjectPKID)); break;
					case "IsDeclareBroke": query.order_by(sort.OrderBy(df.IsBrokenRoles)); break;
				}
			}


			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				var goodYear1 = dp.Dynamic1.GetValue(rd) == "优秀" ? "2016" : string.Empty;
				var goodYear2 = dp.Dynamic2.GetValue(rd) == "优秀" ? "2017" : string.Empty;
				var goodYear3 = dp.Dynamic3.GetValue(rd) == "优秀" ? "2018" : string.Empty;
				string[] goodYears = { goodYear1, goodYear2, goodYear3 };
				string years = string.Join(",", goodYears);

				DeclareReview review = new DeclareReview();
				df.Fullup(rd, review, false);
				review.DeclareCompnay = c.CompanyName.GetValue(rd);
				review.DeclareTargetName = pi.Name.GetValue(rd, "DeclareTarget");
				review.DeclareSubject = DeclareBaseHelper.DeclareSubject.GetName(df.DeclareSubjectPKID.GetValue(rd), "", false);
				review.Subject = BzUserProfileHelper.EduSubject.GetName(dp.EduSubjectPKID.GetValue(rd), "", false);
				review.Gender = BzUserProfileHelper.Gender.GetName(dp.GenderPKID.GetValue(rd), "", false);
				review.GoodYear = string.IsNullOrEmpty(years.Replace(",", "")) ? "-" : years;
				review.IsDeclareBroke = review.IsBrokenRoles ? "是" : "否";
				review.IsMaterialBroke = review.TypeKey.IndexOf("材料破格") > 0 ? "是" : "否";
				review.PhoneMobile = dp.Phonemobile.GetValue(rd);
				review.SkillTitle = BzUserProfileHelper.SkillTitle.GetName(dp.SkillTitlePKID.GetValue(rd));
				review.Birthday = dp.Birthday.GetValue(rd).ToString("yyyy-MM-dd");
				review.IsAllowDowngrade = df.AllowFlowToDowngrade.GetValue(rd) ? "是" : "否";
				review.IsAllowFlowToSchool = df.AllowFlowToSchool.GetValue(rd) ? "是" : "否";

				return review;
			}).ToList();


			return Json(new
			{
				rows = result,
				current,
				rowCount,
				total
			});
		}


		// GET: DeclareStatistical/ExportProfile

		public ActionResult ExportProfile()
		{
			var query = APQuery
				   .select(dp.Asterisk, c.CompanyName.As("Company"), pi.Name.As("targetName"), df.AllowFlowToDowngrade, df.AllowFlowToSchool)
				   .from(dp,
				   p.JoinInner(p.PeriodId == dp.PeriodId),
				   pi.JoinInner(dp.DeclareTargetPKID == pi.PicklistItemId),
				   df.JoinInner(df.PeriodId == dp.PeriodId & df.TeacherId == dp.UserId & df.DeclareTargetPKID == dp.DeclareTargetPKID),
				   c.JoinLeft(c.CompanyId == dp.CompanyId)
				   )
		   .where(dp.PeriodId == Period.PeriodId & df.StatusKey != string.Empty & df.StatusKey != DeclareKeys.ReviewBack);

			if (UserProfile.IsSchoolAdmin)
			{
				query.where_and(dp.CompanyId == UserProfile.CompanyId | df.CompanyId == UserProfile.CompanyId);
			}

			var dic = query.query(db, r =>
			  new InsepctionDeclareProfile
			  {
				  Id = dp.DeclareProfileId.GetValue(r),
				  RealName = dp.RealName.GetValue(r),
				  Gender = BzUserProfileHelper.Gender.GetName(dp.GenderPKID.GetValue(r)),
				  Company = c.CompanyName.GetValue(r, "Company"),
				  Birthday = dp.Birthday.GetValue(r).ToString("yyyy-MM-dd"),
				  Nation = BzUserProfileHelper.Nationality.GetName(dp.NationalityPKID.GetValue(r)),
				  HireDate = dp.Hiredate.GetValue(r).ToString("yyyy-MM-dd"),
				  TrainNo = dp.TrainNo.GetValue(r),
				  RankTitle = BzUserProfileHelper.RankTitle.GetName(dp.RankTitlePKID.GetValue(r)),
				  EduBg = BzUserProfileHelper.EduBg.GetName(dp.EduBgPKID.GetValue(r)),
				  EduDegree = BzUserProfileHelper.EduDegree.GetName(dp.EduDegreePKID.GetValue(r)),
				  SkillTitle = BzUserProfileHelper.SkillTitle.GetName(dp.SkillTitlePKID.GetValue(r)),
				  EduStage = BzUserProfileHelper.EduStage.GetName(dp.EduStagePKID.GetValue(r)),
				  EduSubject = BzUserProfileHelper.EduSubject.GetName(dp.EduSubjectPKID.GetValue(r)),
				  WeekCount = dp.CourseCountPerWeek.GetValue(r),
				  Mobile = dp.Phonemobile.GetValue(r),
				  Phone = dp.Phone.GetValue(r),
				  EMail = dp.Email.GetValue(r),
				  IsAllowDowngrade = df.AllowFlowToDowngrade.GetValue(r) ? "是" : "否",
				  IsAllowFlowToSchool = df.AllowFlowToSchool.GetValue(r) ? "是" : "否"

			  }).ToDictionary(x => x.Id);

			//创建Excel文件的对象
			var book = NPOIHelper.CreateBook(dic);

			// 写入到客户端 
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			book.Write(ms);
			ms.Seek(0, SeekOrigin.Begin);
			DateTime dt = DateTime.Now;
			string dateTime = dt.ToString("yyyyMMdd");
			string fileName = "申报基本情况汇总表" + dateTime + ".xls";
			return File(ms, "application/vnd.ms-excel", fileName);
		}


		// GET: DeclareStatistical/ExportProfile

		public ActionResult ExportSummaryList()
		{
			var query = APQuery
				 .select(df.Asterisk, c.CompanyName, pi.Name.As("DeclareTarget"),
						 dp.EduSubjectPKID, dp.GenderPKID, dp.Dynamic1, dp.Dynamic2, dp.Dynamic3, dp.Phonemobile, dp.Birthday, dp.SkillTitlePKID)
				 .from(df,
				 p.JoinInner(p.PeriodId == df.PeriodId),
				 pi.JoinInner(df.DeclareTargetPKID == pi.PicklistItemId),
				 dp.JoinInner(dp.UserId == df.TeacherId & dp.PeriodId == df.PeriodId & df.DeclareTargetPKID == dp.DeclareTargetPKID),
				 c.JoinLeft(c.CompanyId == df.CompanyId)
				 )
				.where(df.StatusKey != string.Empty & df.StatusKey != DeclareKeys.ReviewBack);

			var dic = query.query(db, rd =>
			{
				var goodYear1 = dp.Dynamic1.GetValue(rd) == "优秀" ? "2016" : string.Empty;
				var goodYear2 = dp.Dynamic1.GetValue(rd) == "优秀" ? "2017" : string.Empty;
				var goodYear3 = dp.Dynamic1.GetValue(rd) == "优秀" ? "2018" : string.Empty;
				string[] goodYears = { goodYear1, goodYear2, goodYear3 };
				string years = string.Join(",", goodYears);

				return new InsepctionDeclareReview
				{
					Id = df.DeclareReviewId.GetValue(rd),
					RealName = df.TeacherName.GetValue(rd),
					Gender = BzUserProfileHelper.Gender.GetName(dp.GenderPKID.GetValue(rd), "", false),
					Birthday = dp.Birthday.GetValue(rd).ToString("yyyy-MM-dd"),
					SkillTitle = BzUserProfileHelper.SkillTitle.GetName(dp.SkillTitlePKID.GetValue(rd), "", false),
					DeclareCompany = c.CompanyName.GetValue(rd),
					DeclareTarget = pi.Name.GetValue(rd, "DeclareTarget"),
					DeclareSubject = DeclareBaseHelper.DeclareSubject.GetName(df.DeclareSubjectPKID.GetValue(rd), "", false),
					EduSubject = BzUserProfileHelper.EduSubject.GetName(dp.EduSubjectPKID.GetValue(rd), "", false),
					GoodYear = string.IsNullOrEmpty(years.Replace(",", "")) ? "-" : years,
					IsDeclareBroke = df.IsBrokenRoles.GetValue(rd) ? "是" : "否",
					IsMaterialBroke = df.TypeKey.GetValue(rd).IndexOf("材料破格") > 0 ? "是" : "否",
					Mobile = dp.Phonemobile.GetValue(rd),
					IsAllowDowngrade = df.AllowFlowToDowngrade.GetValue(rd) ? "是" : "否",
					IsAllowFlowToSchool = df.AllowFlowToSchool.GetValue(rd) ? "是" : "否",
				};
			}).ToDictionary(x => x.Id);

			//创建Excel文件的对象
			var book = NPOIHelper.CreateBook(dic);

			// 写入到客户端 
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			book.Write(ms);
			ms.Seek(0, SeekOrigin.Begin);
			DateTime dt = DateTime.Now;
			string dateTime = dt.ToString("yyyyMMdd");
			string fileName = "申报汇总表" + dateTime + ".xls";
			return File(ms, "application/vnd.ms-excel", fileName);

		}


		// GET: DeclareStatistical/DecalreEvalSummaryList
		// POST-Ajax: DeclareStatistical/DecalreEvalSummaryList

		public ActionResult DecalreEvalSummaryList()
		{
			return View();
		}

		[HttpPost]
		public ActionResult DecalreEvalSummaryList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long targetId, long subjectId, long companyId)
		{
			var results = GetEvalStatisticalResults();

			if (!string.IsNullOrEmpty(searchPhrase))
				results = results.FindAll(x=>x.teacher.Contains(searchPhrase));

			if (targetId > 0)
				results = results.FindAll(x => x.targetId == targetId);

			if (subjectId > 0)
				results = results.FindAll(x => x.subjectId == subjectId);

			if (companyId > 0)
				results = results.FindAll(x => x.companyId == companyId);

			if (sort != null)
			{
				if (sort.According == APSqlOrderAccording.Asc)
				{
					switch (sort.ID)
					{
						case "teacher": results = results.OrderBy(x => x.teacher).ToList(); break;
						case "target": results = results.OrderBy(x => x.targetId).ToList(); break;
						case "subject": results = results.OrderBy(x => x.subjectId).ToList(); break;
						case "company": results = results.OrderBy(x => x.companyId).ToList(); break;
						case "totalScore": results = results.OrderBy(x => x.totalScore).ToList(); break;
					}
				}
				else
				{
					switch (sort.ID)
					{
						case "teacher": results = results.OrderByDescending(x => x.teacher).ToList(); break;
						case "target": results = results.OrderByDescending(x => x.targetId).ToList(); break;
						case "subject": results = results.OrderByDescending(x => x.subjectId).ToList(); break;
						case "company": results = results.OrderByDescending(x => x.companyId).ToList(); break;
						case "totalScore": results = results.OrderByDescending(x => x.totalScore).ToList(); break;
					}
				}
			}

			var total = results?.Count;

			if (total > 0)
				results = results.Skip((current - 1) * rowCount).Take(rowCount).ToList();

			return Json(new
			{
				rows = results,
				current,
				rowCount,
				total
			});
		}


		// GET: DeclareStatistical/DecalreEvalSummaryList

		public ActionResult ExportEvalSummary()
		{
			var results = GetEvalStatisticalResults().ToDictionary(x=>x.teacherId);

			//创建Excel文件的对象
			var book = NPOIHelper.CreateBook(results);

			// 写入到客户端 
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			book.Write(ms);
			ms.Seek(0, SeekOrigin.Begin);
			DateTime dt = DateTime.Now;
			string dateTime = dt.ToString("yyyyMMdd");
			string fileName = "申报评分汇总表" + dateTime + ".xls";
			return File(ms, "application/vnd.ms-excel", fileName);
		}

		#region [ Helper ]

		private List<DeclareEvalStaisticalViewModel> GetEvalStatisticalResults()
		{
			var sql = @"select 
						teacherId,
						targetId,
						subjectId,
						companyId,
						teacher,
						target,
						subject,
						company,
						avg(totalScore) totalScore,
						isnull(avg([校内履职]),0) as xnlz,
						isnull(case avg([师德]) when 0 then '不合格' when 1 then '合格' end,'未评') as shd,
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
						where  e.periodid=5005 and dr.statusKey='审核通过'
						) c
						pivot(avg(c.score) for c.EvalItemKey in (
						[校内履职],
						[师德],
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
						group by t.teacherId,T.teacher,T.target,T.subject,T.company,T.targetId,T.companyId,T.subjectId
						";

			var results= DapperHelper.QueryBySQL<DeclareEvalStaisticalViewModel>(sql);
			foreach(var item in results)
			{
				var score = new double[] { item.zz, item.qtsf, item.xycz }.Max()
					+ new double[] { item.jxpb,item.qt}.Max()
					+ new double[] { item.gkk,item.mt,item.pingw,item.dey,item.drpw2,item.xmyj,item.fblw,item.jspx,item.ztjz,item.xnlz }.Sum();

				item.totalScore = Math.Round(score,2);
			}

			return results;
		}

		//private List<T> QueryBySQL<T>(string sql, object paras = null)
		//{
		//	if (string.IsNullOrEmpty(sql)) return null;

		//	var _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
		//	var result = _conn.Query<T>(sql, paras);

		//	return result == null ? null : result.ToList();
		//}

		#endregion
	}

}