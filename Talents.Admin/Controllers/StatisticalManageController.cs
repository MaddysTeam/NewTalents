using Business;
using Business.Config;
using Business.Helper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheSite.Models;
using Webdiyer.WebControls.Mvc;

namespace TheSite.Controllers
{

	public class StatisticalManageController : BaseController
	{

		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.EvalSchoolResultTableDef esr = APDBDef.EvalSchoolResult;
		static APDBDef.EvalSchoolResultItemTableDef esri = APDBDef.EvalSchoolResultItem;
		static APDBDef.EvalVolumnResultTableDef evr = APDBDef.EvalVolumnResult;
		static APDBDef.EvalVolumnResultItemTableDef evri = APDBDef.EvalVolumnResultItem;
		static APDBDef.EvalQualitySubmitResultTableDef eqsr = APDBDef.EvalQualitySubmitResult;
		static APDBDef.CompanyDeclareTableDef cd = APDBDef.CompanyDeclare;
		static APDBDef.ExpGroupTargetTableDef egt = APDBDef.ExpGroupTarget;
		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;
		static APDBDef.EvalQualityResultTableDef eqr = APDBDef.EvalQualityResult;
		static APDBDef.EvalQualityResultItemTableDef eqri = APDBDef.EvalQualityResultItem;
		static APDBDef.CompanyTableDef c = APDBDef.Company;

		//	GET:	StatisticalManage/Fragment

		public ActionResult Fragment(string key, long? periodId)
		{
			switch (key)
			{
				case StatisticalKeys.Student_Target:
					return Student_Target();
				case StatisticalKeys.Student_Subject:
					return Student_Subject();
				case StatisticalKeys.Student_Stage:
					return Student_Stage();
				case StatisticalKeys.SchoolEval_Overview:
					return SchoolEval_Overview(periodId.Value);
				case StatisticalKeys.SchoolEval_Target:
					return SchoolEval_Target(periodId.Value);
				case StatisticalKeys.SchoolEval_Subject:
					return SchoolEval_Subject(periodId.Value);
				case StatisticalKeys.SchoolEval_Stage:
					return SchoolEval_Stage(periodId.Value);
				case StatisticalKeys.VolumnEval_Overview:
					return VolumnEval_Overview(periodId.Value);
				case StatisticalKeys.VolumnEval_Target:
					return VolumnEval_Target(periodId.Value);
				case StatisticalKeys.VolumnEval_Subject:
					return VolumnEval_Subject(periodId.Value);
				case StatisticalKeys.VolumnEval_Stage:
					return VolumnEval_Stage(periodId.Value);
				case StatisticalKeys.QualityEval_Overview:
					return QualityEval_Overview(periodId.Value);
				case StatisticalKeys.QualityEval_Target:
					return QualityEval_Target(periodId.Value);
				case StatisticalKeys.QualityEval_Subject:
					return QualityEval_Subject(periodId.Value);
				case StatisticalKeys.QualityEval_Stage:
					return QualityEval_Stage(periodId.Value);
			}


			return Content("");
		}


		//	GET:	StatisticalManage/Student 

		public ActionResult Student()
		{
			return View();
		}


		//	GET:	StatisticalManage/Student_Target 

		public ActionResult Student_Target()
		{
			var list = APQuery.select(d.DeclareTargetPKID, d.DeclareTargetPKID.Count().As("Count"))
				.from(d)
				.order_by(d.DeclareTargetPKID.Count().Desc)
				.group_by(d.DeclareTargetPKID)
				.query(db, r =>
				{
					return new TempListViewModel
					{
						Name = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(r)),
						Count = d.DeclareTargetPKID.GetValue(r, "Count")
					};
				}).ToList();


			return PartialView("TempList", list);
		}


		//	GET:	StatisticalManage/Student_Subject 

		public ActionResult Student_Subject()
		{
			var list = APQuery.select(d.DeclareSubjectPKID, d.DeclareSubjectPKID.Count().As("Count"))
				.from(d)
				.order_by(d.DeclareSubjectPKID.Count().Desc)
				.group_by(d.DeclareSubjectPKID)
				.query(db, r =>
				{
					return new TempListViewModel
					{
						Name = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(r)),
						Count = d.DeclareSubjectPKID.GetValue(r, "Count")
					};
				}).ToList();


			return PartialView("TempList", list);
		}


		//	GET:	StatisticalManage/Student_Stage

		public ActionResult Student_Stage()
		{
			var list = APQuery.select(d.DeclareStagePKID, d.DeclareStagePKID.Count().As("Count"))
				.from(d)
				.order_by(d.DeclareTargetPKID.Count().Desc)
				.group_by(d.DeclareStagePKID)
				.query(db, r =>
				{
					return new TempListViewModel
					{
						Name = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(r)),
						Count = d.DeclareStagePKID.GetValue(r, "Count")
					};
				}).ToList();


			return PartialView("TempList", list);
		}


		//	GET:	StatisticalManage/SchoolEval 

		public ActionResult SchoolEval()
		{
			ViewBag.periods = GetPeriodSelect();


			return View();
		}


		//	GET:	StatisticalManage/SchoolOverview 

		public ActionResult SchoolEval_Overview(long periodId = 0)
		{
			var result = SchoolCompany_Overview(periodId);
			result.AddRange(SchoolMember_Overview(periodId));

			return PartialView("TempList", result);
		}


		//	GET:	StatisticalManage/VolumnEval_Target

		public ActionResult SchoolEval_Target(long periodId)
		{
			var list = APQuery.select(d.DeclareTargetPKID, cd.TeacherId.Count().As("TotalCount"),
												esr.TeacherId.Count().As("EvalCount"))
				.from(d,
						cd.JoinInner(d.TeacherId == cd.TeacherId),
						esr.JoinLeft(d.TeacherId == esr.TeacherId & esr.PeriodId == periodId)
						)
				.order_by(d.DeclareTargetPKID.Count().Desc)
				.group_by(d.DeclareTargetPKID)
				.query(db, r =>
				{
					return new MulTempListViewModel
					{
						Name = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(r)),
						Count = d.DeclareTargetPKID.GetValue(r, "TotalCount"),
						EvalCount = d.DeclareTargetPKID.GetValue(r, "EvalCount")
					};
				}).ToList();


			return PartialView("MulTempList", list);
		}


		//	GET:	StatisticalManage/VolumnEval_Subject

		public ActionResult SchoolEval_Subject(long periodId)
		{
			var list = APQuery.select(d.DeclareSubjectPKID, cd.TeacherId.Count().As("TotalCount"),
												esr.TeacherId.Count().As("EvalCount"))
				.from(d,
						cd.JoinInner(d.TeacherId == cd.TeacherId),
						esr.JoinLeft(d.TeacherId == esr.TeacherId & esr.PeriodId == periodId)
						)
				.order_by(d.DeclareSubjectPKID.Count().Desc)
				.group_by(d.DeclareSubjectPKID)
				.query(db, r =>
				{
					return new MulTempListViewModel
					{
						Name = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(r)),
						Count = d.DeclareTargetPKID.GetValue(r, "TotalCount"),
						EvalCount = d.DeclareSubjectPKID.GetValue(r, "EvalCount")
					};
				}).ToList();


			return PartialView("MulTempList", list);
		}


		//	GET:	StatisticalManage/VolumnEval_Stage

		public ActionResult SchoolEval_Stage(long periodId)
		{
			var list = APQuery.select(d.DeclareStagePKID, cd.TeacherId.Count().As("TotalCount"),
												esr.TeacherId.Count().As("EvalCount"))
				.from(d,
						cd.JoinInner(d.TeacherId == cd.TeacherId),
						esr.JoinLeft(d.TeacherId == esr.TeacherId & esr.PeriodId == periodId)
						)
				.order_by(d.DeclareStagePKID.Count().Desc)
				.group_by(d.DeclareStagePKID)
				.query(db, r =>
				{
					return new MulTempListViewModel
					{
						Name = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(r)),
						Count = d.DeclareTargetPKID.GetValue(r, "TotalCount"),
						EvalCount = d.DeclareStagePKID.GetValue(r, "EvalCount")
					};
				}).ToList();


			return PartialView("MulTempList", list);
		}


		//	GET:	StatisticalManage/VolumnEval 

		public ActionResult VolumnEval()
		{
			ViewBag.periods = GetPeriodSelect();


			return View();
		}


		//	GET:	StatisticalManage/VolumnEval_Overview		

		public ActionResult VolumnEval_Overview(long periodId)
		{
			var totalCount = db.DeclareBaseDal.ConditionQueryCount(d.DeclareTargetPKID < DeclareTargetIds.JiaoxNengs &
						 d.DeclareTargetPKID > DeclareTargetIds.WaipDaos);

			var evalCount = db.EvalVolumnResultDal.ConditionQueryCount(evr.PeriodId == periodId);

			var notEvalCount = totalCount - evalCount;

			var list = new List<TempListViewModel>
			{
				 new TempListViewModel { Name = StatisticalKeys.MemberCount, Count = totalCount},
				 new TempListViewModel { Name = StatisticalKeys.EvalMemberCount, Count = evalCount},
				 new TempListViewModel { Name = StatisticalKeys.NotEvalMemberCount, Count = notEvalCount}
			};


			return PartialView("TempList", list);
		}


		//	GET:	StatisticalManage/VolumnEval_Target

		public ActionResult VolumnEval_Target(long periodId)
		{
			var list = APQuery.select(d.DeclareTargetPKID, d.DeclareTargetPKID.Count().As("TotalCount"),
												evr.TeacherId.Count().As("EvalCount"))
				.from(d, evr.JoinLeft(d.TeacherId == evr.TeacherId & evr.PeriodId == periodId))
				.where(d.DeclareTargetPKID < DeclareTargetIds.JiaoxNengs &
						 d.DeclareTargetPKID > DeclareTargetIds.WaipDaos)
				.order_by(d.DeclareTargetPKID.Count().Desc)
				.group_by(d.DeclareTargetPKID)
				.query(db, r =>
				{
					return new MulTempListViewModel
					{
						Name = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(r)),
						Count = d.DeclareTargetPKID.GetValue(r, "TotalCount"),
						EvalCount = d.DeclareTargetPKID.GetValue(r, "EvalCount")
					};
				}).ToList();


			return PartialView("MulTempList", list);
		}


		//	GET:	StatisticalManage/VolumnEval_Subject

		public ActionResult VolumnEval_Subject(long periodId)
		{
			var list = APQuery.select(d.DeclareSubjectPKID, d.DeclareSubjectPKID.Count().As("TotalCount"),
												evr.TeacherId.Count().As("EvalCount"))
				.from(d, evr.JoinLeft(d.TeacherId == evr.TeacherId & evr.PeriodId == periodId))
				.where(d.DeclareTargetPKID < DeclareTargetIds.JiaoxNengs &
						 d.DeclareTargetPKID > DeclareTargetIds.WaipDaos)
				.order_by(d.DeclareSubjectPKID.Count().Desc)
				.group_by(d.DeclareSubjectPKID)
				.query(db, r =>
				{
					return new MulTempListViewModel
					{
						Name = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(r)),
						Count = d.DeclareTargetPKID.GetValue(r, "TotalCount"),
						EvalCount = d.DeclareSubjectPKID.GetValue(r, "EvalCount")
					};
				}).ToList();


			return PartialView("MulTempList", list);
		}


		//	GET:	StatisticalManage/VolumnEval_Stage

		public ActionResult VolumnEval_Stage(long periodId)
		{
			var list = APQuery.select(d.DeclareStagePKID, d.DeclareStagePKID.Count().As("TotalCount"),
												evr.TeacherId.Count().As("EvalCount"))
				.from(d, evr.JoinLeft(d.TeacherId == evr.TeacherId & evr.PeriodId == periodId))
				.where(d.DeclareTargetPKID < DeclareTargetIds.JiaoxNengs &
						 d.DeclareTargetPKID > DeclareTargetIds.WaipDaos)
				.order_by(d.DeclareStagePKID.Count().Desc)
				.group_by(d.DeclareStagePKID)
				.query(db, r =>
				{
					return new MulTempListViewModel
					{
						Name = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(r)),
						Count = d.DeclareTargetPKID.GetValue(r, "TotalCount"),
						EvalCount = d.DeclareStagePKID.GetValue(r, "EvalCount")
					};
				}).ToList();


			return PartialView("MulTempList", list);
		}


		//	GET:	StatisticalManage/QualityEval 

		public ActionResult QualityEval()
		{
			ViewBag.periods = GetPeriodSelect();


			return View();
		}


		//	GET:	StatisticalManage/QualityEval_Overview

		public ActionResult QualityEval_Overview(long periodId)
		{
			var egt = APDBDef.ExpGroupTarget;


			var data = APQuery.select(egt.MemberId.Count().As("TotalCount"), eqsr.TeacherId.Count().As("EvalCount"))
									.from(egt,
											d.JoinInner(egt.MemberId == d.TeacherId),
											eqsr.JoinLeft(egt.MemberId == eqsr.TeacherId)
											)
									.where(d.DeclareTargetPKID < DeclareTargetIds.JiaoxNengs &
											 d.DeclareTargetPKID > DeclareTargetIds.WaipDaos &
                                 eqsr.PeriodId==periodId
                                  )
									.query(db, r =>
									 {
										 return new
										 {
											 MemberCount = r.GetInt32(r.GetOrdinal("TotalCount")),
											 EvalMemberCount = r.GetInt32(r.GetOrdinal("EvalCount")),
										 };
									 }).FirstOrDefault();

			var result = new List<TempListViewModel>
			{
				 new TempListViewModel { Name=StatisticalKeys.MemberCount,Count= data.MemberCount},
				 new TempListViewModel { Name=StatisticalKeys.EvalMemberCount,Count= data.EvalMemberCount},
				 new TempListViewModel { Name=StatisticalKeys.NotEvalMemberCount,Count= data.MemberCount-data.EvalMemberCount}
			};


			return PartialView("TempList", result);
		}


		//	GET:	StatisticalManage/QualityEval_Target

		public ActionResult QualityEval_Target(long periodId)
		{
			var result = APQuery.select(d.DeclareTargetPKID, egt.MemberId.Count().As("TotalCount"),
															eqsr.TeacherId.Count().As("EvalCount"))
				 .from(d,
						 egt.JoinInner(d.TeacherId == egt.MemberId),
						 eqsr.JoinLeft(d.TeacherId == eqsr.TeacherId & eqsr.PeriodId == periodId))
				 .where(d.DeclareTargetPKID < DeclareTargetIds.JiaoxNengs &
								 d.DeclareTargetPKID > DeclareTargetIds.WaipDaos)
				 .group_by(d.DeclareTargetPKID)
				 .query(db, r =>
				 {
					 return new MulTempListViewModel
					 {
						 Name = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(r)),
						 Count = d.DeclareTargetPKID.GetValue(r, "TotalCount"),
						 EvalCount = d.DeclareTargetPKID.GetValue(r, "EvalCount")
					 };
				 }).ToList();


			return PartialView("MulTempList", result);
		}


		//	GET:	StatisticalManage/QualityEval_Subject

		public ActionResult QualityEval_Subject(long periodId)
		{
			var result = APQuery.select(d.DeclareSubjectPKID, egt.MemberId.Count().As("TotalCount"),
															eqsr.TeacherId.Count().As("EvalCount"))
				 .from(d,
						 egt.JoinInner(d.TeacherId == egt.MemberId),
						 eqsr.JoinLeft(d.TeacherId == eqsr.TeacherId & eqsr.PeriodId == periodId)
						 )
				 .where(d.DeclareTargetPKID < DeclareTargetIds.JiaoxNengs &
							 d.DeclareTargetPKID > DeclareTargetIds.WaipDaos)
				 .group_by(d.DeclareSubjectPKID)
				 .query(db, r =>
				 {
					 return new MulTempListViewModel
					 {
						 Name = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(r)),
						 Count = d.DeclareSubjectPKID.GetValue(r, "TotalCount"),
						 EvalCount = d.DeclareSubjectPKID.GetValue(r, "EvalCount")
					 };
				 }).ToList();


			return PartialView("MulTempList", result);
		}


		//	GET:	StatisticalManage/QualityEval_Stage

		public ActionResult QualityEval_Stage(long periodId)
		{
			var result = APQuery.select(d.DeclareStagePKID, egt.MemberId.Count().As("TotalCount"),
															eqsr.TeacherId.Count().As("EvalCount"))
				 .from(d,
						egt.JoinInner(d.TeacherId == egt.MemberId),
						eqsr.JoinLeft(d.TeacherId == eqsr.TeacherId & eqsr.PeriodId == periodId)
				 )
				 .where(d.DeclareTargetPKID < DeclareTargetIds.JiaoxNengs &
							 d.DeclareTargetPKID > DeclareTargetIds.WaipDaos)
				 .group_by(d.DeclareStagePKID)
				 .query(db, r =>
				 {
					 return new MulTempListViewModel
					 {
						 Name = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(r)),
						 Count = d.DeclareStagePKID.GetValue(r, "TotalCount"),
						 EvalCount = d.DeclareStagePKID.GetValue(r, "EvalCount")
					 };
				 }).ToList();


			return PartialView("MulTempList", result);
		}


		//	考核成绩统计
		//	GET:	StatisticalManage/Inspection 
		//	POST-AJAX:	StatisticalManage/Inspection

		public ActionResult Inspection()
		{
			PagedList<InspectionViewModel> list = new PagedList<InspectionViewModel>(new List<InspectionViewModel>(), 1 , 10);

			return View(list);
		}

		[HttpPost]
		public ActionResult Inspection(FormCollection fc, int current = 1, int rowCount = 10)
		{
			ThrowNotAjax();


			var dic = new Dictionary<long, InspectionViewModel>();
			var t = APDBDef.BzUserProfile;


         var periodId = Convert.ToInt32(fc["Period"]);

         var period = db.EvalPeriodDal.PrimaryGet(periodId);
			if (period == null)
			{
				return View(dic);
			}

			var query = APQuery.select(d.TeacherId, d.DeclareTargetPKID, d.DeclareSubjectPKID, t.RealName, c.CompanyName)
				.from(d, 
						t.JoinInner(d.TeacherId == t.UserId),
						cd.JoinLeft(d.TeacherId == cd.TeacherId),
						c.JoinLeft(cd.CompanyId == c.CompanyId))
				.primary(d.TeacherId)
				.where(d.HasTeam == true)
				.skip((current - 1) * rowCount)
				.take(rowCount);


			//过滤条件取条件
			// 取过滤条件
			foreach (string cond in fc.Keys)
			{
				switch (cond)
				{
               case "Target":
						if (Int64.Parse(fc[cond]) > 0)
							query.where_and(d.DeclareTargetPKID == Int64.Parse(fc[cond])); break;
					case "Subject":
						if (Int64.Parse(fc[cond]) > 0)
							query.where_and(d.DeclareSubjectPKID == Int64.Parse(fc[cond])); break;
					case "CompanyId":
						if(!string.IsNullOrEmpty(fc[cond]))
							query.where_and(c.CompanyId== Int64.Parse(fc[cond])); break;
					case "Name":
						if (!string.IsNullOrEmpty(fc[cond]))
							query.where_and(t.RealName.Match(fc[cond]) | t.UserName.Match(fc[cond])); break;
				}
			}


			//获得查询的总数量

			var total = db.ExecuteSizeOfSelect(query);

			dic = query
				.query(db, r =>
				{
					return new InspectionViewModel
					{
						TeacherId = d.TeacherId.GetValue(r),
						TeacherName = t.RealName.GetValue(r),
						TargetId = d.DeclareTargetPKID.GetValue(r),
						TargetName = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(r)),
						SubjectName = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(r)),
						CompanyName = c.CompanyName.GetValue(r)
					};
				}).ToDictionary(m => m.TeacherId);

			GetSchoolEval(dic, period.PeriodId);
			GetVolumnEval(dic, period.PeriodId);
			GetQualityEval(dic, period.PeriodId);

			PagedList<InspectionViewModel> list = new PagedList<InspectionViewModel>(dic.Values.ToList(), current, rowCount, total);

			return PartialView("_AjaxSearchPost", list);
		}


		public FileResult ExportStu(long periodId,long Target, long Subject, long CompanyId, string Name)
		{
			var dic = new Dictionary<long, InspectionViewModel>();
			var t = APDBDef.BzUserProfile;

			var period = db.EvalPeriodDal.PrimaryGet(periodId);
         if (period == null)
			{
				throw new Exception() { };
			}

			var query = APQuery.select(d.TeacherId, d.DeclareTargetPKID, d.DeclareSubjectPKID, t.RealName, c.CompanyName)
				.from(d, t.JoinInner(d.TeacherId == t.UserId), cd.JoinLeft(d.TeacherId == cd.TeacherId), c.JoinLeft(cd.CompanyId == c.CompanyId))
				.primary(d.TeacherId)
				.where(d.HasTeam == true);



			//过滤条件取条件
			if (Target > 0)
				query.where_and(d.DeclareTargetPKID == Target);
			if (Subject > 0)
				query.where_and(d.DeclareSubjectPKID == Subject);
			if (CompanyId>0)
				query.where_and(c.CompanyId == CompanyId);
			if (!string.IsNullOrEmpty(Name))
				query.where_and(t.RealName.Match(Name) | t.UserName.Match(Name));


			dic = query
				.query(db, r =>
				{
					return new InspectionViewModel
					{
						TeacherId = d.TeacherId.GetValue(r),
						TeacherName = t.RealName.GetValue(r),
						TargetId = d.DeclareTargetPKID.GetValue(r),
						TargetName = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(r)),
						SubjectName = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(r)),
						CompanyName = c.CompanyName.GetValue(r)
					};
				}).ToDictionary(m => m.TeacherId);

			GetSchoolEval(dic, period.PeriodId);
			GetVolumnEval(dic, period.PeriodId);
			GetQualityEval(dic, period.PeriodId);


			//创建Excel文件的对象
			var book = NPOIHelper(dic);
			
			// 写入到客户端 
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			book.Write(ms);
			ms.Seek(0, SeekOrigin.Begin);
			DateTime dt = DateTime.Now;
			string dateTime = dt.ToString("yyyyMMdd");
			string fileName = "考核成绩统计" + dateTime + ".xls";
			return File(ms, "application/vnd.ms-excel", fileName);
		}


		//	GET:	StatisticalManage/MemberList

		public ActionResult MemberList(long id)
		{
			var tm = APDBDef.TeamMember;
			var u = APDBDef.BzUserProfile;

			var list = APQuery.select(u.RealName, u.CompanyName, d.TeacherId,
												d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID)
				.from(tm,
						u.JoinInner(tm.MemberId == u.UserId),
						d.JoinInner(tm.MemberId == d.TeacherId))
				.where(tm.TeamId == id)
				.query(db, rd =>
				{
					return new TeamMemberModel()
					{
						TeacherId = d.TeacherId.GetValue(rd),
						RealName = u.RealName.GetValue(rd),
						CompanyName = u.CompanyName.GetValue(rd),
						Target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
						Subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
						Stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd))
					};
				}).ToList();


			var info = db.BzUserProfileDal.PrimaryGet(id);
			var dcl = db.DeclareBaseDal.PrimaryGet(id);

			list.Insert(0, new TeamMemberModel
			{
				RealName = info.RealName,
				CompanyName = info.CompanyName,
				TeacherId = info.UserId,
				Target = DeclareBaseHelper.DeclareTarget.GetName(dcl.DeclareTargetPKID),
				Subject = DeclareBaseHelper.DeclareSubject.GetName(dcl.DeclareSubjectPKID),
				Stage = DeclareBaseHelper.DeclareStage.GetName(dcl.DeclareStagePKID)
			});


			return PartialView("MemberList", list);
		}


		#region [ Helper ]


		private List<TempListViewModel> SchoolCompany_Overview(long periodId = 0)
		{
			var c = APDBDef.Company;
			var cd = APDBDef.CompanyDeclare;


			var totalCount = db.CompanyDal.ConditionQueryCount(null);

			var evalCount = APQuery.select(cd.CompanyId.Count().As("TotalCount"), esr.CompanyId.Count().As("EvalCount"))
									.from(cd,
										 esr.JoinLeft(esr.TeacherId == cd.TeacherId & esr.PeriodId == periodId)
										)
									.group_by(cd.CompanyId)
									.query(db, r => r.GetInt32(r.GetOrdinal("TotalCount")) == r.GetInt32(r.GetOrdinal("EvalCount"))
									).Count(r => r);

			var notEvalCount = db.CompanyDal.ConditionQueryCount(
				  c.CompanyId.NotIn(
						APQuery.select(esr.CompanyId)
								 .from(esr)
								 .where(esr.PeriodId == periodId)
					  )
				);

			var evalingCount = totalCount - notEvalCount - evalCount;


			return new List<TempListViewModel>
			{
				 new TempListViewModel { Name=StatisticalKeys.SchoolEval_SchoolCount,Count= totalCount},
				 new TempListViewModel { Name=StatisticalKeys.SchoolEval_EvalSchoolCount,Count= evalCount},
				 new TempListViewModel { Name=StatisticalKeys.SchoolEval_NotEvalSchoolCount,Count= notEvalCount},
				 new TempListViewModel { Name=StatisticalKeys.SchoolEval_EvalingSchoolCount,Count= evalingCount}
			};
		}


		private List<TempListViewModel> SchoolMember_Overview(long periodId = 0)
		{
			var cd = APDBDef.CompanyDeclare;


			var totalCount = db.CompanyDeclareDal.ConditionQueryCount(null);

			var evalCount = db.EvalSchoolResultDal.ConditionQueryCount(esr.PeriodId == periodId);

			var notEvalCount = totalCount - evalCount;


			return new List<TempListViewModel>
			{
				 new TempListViewModel { Name=StatisticalKeys.MemberCount,Count= totalCount},
				 new TempListViewModel { Name=StatisticalKeys.EvalMemberCount,Count= evalCount},
				 new TempListViewModel { Name=StatisticalKeys.NotEvalMemberCount,Count= notEvalCount}
			};

		}


		private IEnumerable<SelectListItem> GetPeriodSelect()
		{
			var ep = APDBDef.EvalPeriod;


			var result = APQuery.select(ep.AnalysisType, ep.Name, ep.IsCurrent, ep.PeriodId)
				 .from(ep)
				 .query(db, r =>
				 {
					 var text = ep.Name.GetValue(r);
					 var isCurrent = ep.IsCurrent.GetValue(r);

					 return new SelectListItem()
					 {
						 Value = ep.PeriodId.GetValue(r).ToString(),
						 Text = isCurrent ? text + "（当期）" : text
					 };
				 }).ToList();


			return result;
		}


		private void GetSchoolEval(Dictionary<long, InspectionViewModel> dic, long periodId)
			=> APQuery.select(esr.TeacherId, esr.Morality, esri.EvalItemKey, esri.ResultValue)
				.from(esr, esri.JoinInner(esr.ResultId == esri.ResultId))
				.where(esr.PeriodId == periodId)
				.query(db, r =>
				{
					var id = esr.TeacherId.GetValue(r);
					var key = esri.EvalItemKey.GetValue(r);
					var morality = esr.Morality.GetValue(r);
					var value = esri.ResultValue.GetValue(r);
					if (dic.ContainsKey(id))
					{
						dic[id].School.Shid = morality;

						switch (key)
						{
							case EvalSchoolRuleKeys.XiaonLvz_Gongzl:
								dic[id].School.VolumnScore = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.SchoolEvalUnit.ProportionValue;
								break;
							case EvalSchoolRuleKeys.XiaonLvz_GongzZhil:
								dic[id].School.QualityScore = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.SchoolEvalUnit.ProportionValue;
								break;
						}
					}

					return id;
				}).ToList();


		private void GetVolumnEval(Dictionary<long, InspectionViewModel> dic, long periodId)
		{
			APQuery.select(evr.TeacherId, evr.DeclareTargetPKID, evri.EvalItemKey, evri.ResultValue)
				.from(evr, evri.JoinInner(evr.ResultId == evri.ResultId))
				.where(evr.PeriodId == periodId)
				.query(db, r =>
				{
					var id = evr.TeacherId.GetValue(r);
					var targetId = evr.DeclareTargetPKID.GetValue(r);
					var key = evri.EvalItemKey.GetValue(r);
					var value =  evri.ResultValue.GetValue(r);
					if (dic.ContainsKey(id))
					{
						dic[id].Volumn.TargetId = targetId;
						switch (key)
						{
							case EvalVolumnRuleKeys.DusHuod:
								dic[id].Volumn.ZisFaz.DusHuod_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.KaiZhansk:
								dic[id].Volumn.ZisFaz.Kaik_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.DanrPingwGongz:
								dic[id].Volumn.ZisFaz.Pingw_Score = Convert.ToDouble(value.Replace("分", ""))* EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.PingbHuoj:
								dic[id].Volumn.ZisFaz.Pingb_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.FabLunw:
								dic[id].Volumn.ZisFaz.FabLunw_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.LixKet:
								dic[id].Volumn.ZisFaz.LixKet_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.XiangmYanj:
								dic[id].Volumn.ZisFaz.XiangmYanj_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.JiaosPeixKec:
								dic[id].Volumn.PeixKec.KaisKec_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.ZhuantJiangz:
								dic[id].Volumn.PeixKec.JiangzBaog_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.KecZiyKaif:
								dic[id].Volumn.PeixKec.KecZiy_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.RicGongxlZhid:
								dic[id].Volumn.DaijJiaos.RicGongxlZhid_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.TingkZhid:
								dic[id].Volumn.DaijJiaos.TingkZhid_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.JiaoalXiugZhid:
								dic[id].Volumn.DaijJiaos.JiaoalXiugZhid_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.LunwHuoKetXiugZhid:
								dic[id].Volumn.DaijJiaos.LunwHuoKetXiugZhid_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.HuodAnpb:
								dic[id].Volumn.DaijJiaos.HuodAnp = value;
								break;
							case EvalVolumnRuleKeys.JingfGuanlBanf:
								dic[id].Volumn.DaijJiaos.JiangfZhid = value;
								break;
							case EvalVolumnRuleKeys.XueyShul:
								dic[id].Volumn.DaijJiaos.XueyShul_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.DaijXiey:
								dic[id].Volumn.DaijJiaos.DaijXiey_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.JutJih:
								dic[id].Volumn.DaijJiaos.DaijFanga_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.DaijXiaoj:
								dic[id].Volumn.DaijJiaos.DaijXiaoj_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.XueydChengzFangx:
								dic[id].Volumn.DaijJiaos.XueyChengzFenx_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.DaijZhidKaiZhansk:
								dic[id].Volumn.DaijJiaos.KaisZhansk_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.JiaoyXuekKeyChengg:
								dic[id].Volumn.DaijJiaos.FabLunwHuoCanyKetYanj_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
							case EvalVolumnRuleKeys.JiaoyJiaoxPingb:
								dic[id].Volumn.DaijJiaos.JiaoyJiaoxPingbi_Score = Convert.ToDouble(value.Replace("分", "")) * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue;
								break;
						}
					}

					return id;
				}).ToList();
		}


		private void SetList(List<InspectionQualityDataModel> list, List<InspectionQualityDataModel> tempList, long resultId, string Newkey, InspectionQualityType ScoreKey, params string[] OldKey)
		{
			if (!list.Exists(m => m.EvalItemKey == Newkey & m.ResultId == resultId))
			{
				double totalScore = 0.0;

				foreach (var item in OldKey)
				{
					var temp = tempList.Find(m => m.EvalItemKey == item);
					if (temp != null)
					{
						totalScore += temp.ResultValue;
					}
				}

				var data = tempList.Find(m => m.EvalItemKey == OldKey[0]);

				var MaxScore = InspectionQualityHelper.GetMaxScore(data.DeclareTargetPKID, ScoreKey, 2);

				list.Add(new InspectionQualityDataModel
				{
					TeacherId = data.TeacherId,
					PeriodId = data.PeriodId,
					ResultId = data.ResultId,
					DeclareTargetPKID = data.DeclareTargetPKID,
					GroupId = data.GroupId,
					Characteristic = data.Characteristic,
					EvalItemKey = Newkey,
					ResultValue = EvalHelper.GetScore(MaxScore, totalScore),
					AdjustScore = data.AdjustScore
				});
			}
		}

		private void GetQualityEval(Dictionary<long, InspectionViewModel> dic, long periodId)
		{
			var templist = APQuery.select(eqr.TeacherId, eqr.PeriodId, eqr.ResultId, eqr.DeclareTargetPKID, eqr.GroupId, eqr.Characteristic,
												eqri.EvalItemKey, eqri.ResultValue, eqsr.AdjustScore)
				.from(eqr, eqri.JoinLeft(eqr.ResultId == eqri.ResultId), 
						eqsr.JoinLeft( eqr.PeriodId == eqsr.PeriodId & eqr.TeacherId == eqsr.TeacherId))
				.where(eqr.PeriodId == periodId)
				.query(db, r =>
				{
					return new InspectionQualityDataModel
					{
						TeacherId = eqr.TeacherId.GetValue(r),
						PeriodId = eqr.PeriodId.GetValue(r),
						ResultId = eqr.ResultId.GetValue(r),
						DeclareTargetPKID = eqr.DeclareTargetPKID.GetValue(r),
						GroupId = eqr.GroupId.GetValue(r),
						Characteristic = eqr.Characteristic.GetValue(r),
						EvalItemKey = eqri.EvalItemKey.GetValue(r),
						ResultValue = Convert.ToDouble(eqri.ResultValue.GetValue(r)),
						AdjustScore = eqsr.AdjustScore.GetValue(r)
					};
				}).ToList();


			var setList = templist;

			foreach (var item in templist.GroupBy(m => m.ResultId))
			{
				var temp = templist.FindAll(m => m.ResultId == item.Key);
				SetList(setList, temp, item.Key,
					EvalQualityRuleKeys.ZisFaz_KaikPingwPingb, InspectionQualityType.KaikPingwPingb, EvalQualityRuleKeys.ZisFaz_KaizShik,
					EvalQualityRuleKeys.ZisFaz_DanrPingwGongz, EvalQualityRuleKeys.ZisFaz_PingbHuoj);

				SetList(setList, temp, item.Key, EvalQualityRuleKeys.ZisFaz_Key, InspectionQualityType.Key,
					EvalQualityRuleKeys.ZisFaz_FabLunw, EvalQualityRuleKeys.ZisFaz_LixKet, EvalQualityRuleKeys.ZisFaz_XiangmYanj);

				SetList(setList, temp, item.Key, EvalQualityRuleKeys.PeixKec_Total, InspectionQualityType.PeixKec,
					EvalQualityRuleKeys.PeixKec_KaisJiaosPeixKec, EvalQualityRuleKeys.PeixKec_JiangzBaog, EvalQualityRuleKeys.PeixKec_KecZiyKaif);

				SetList(setList, temp, item.Key, EvalQualityRuleKeys.DaijJiaos_DaijChengg, InspectionQualityType.DaijChengg,
					EvalQualityRuleKeys.DaijJiaos_KaizShik, EvalQualityRuleKeys.DaijJiaos_FablunwHuocYukTiyJiu, EvalQualityRuleKeys.DaijJiaos_JiaoyJiaoxPingb);
			}

			var list = (from query in templist
						  group query by new { query.TeacherId, query.PeriodId, query.DeclareTargetPKID, query.GroupId, query.EvalItemKey }
						  into g
						  select new InspectionQualityDataModel
						  {
							  TeacherId = g.Key.TeacherId,
							  PeriodId = g.Key.PeriodId,
							  DeclareTargetPKID = g.Key.DeclareTargetPKID,
							  GroupId = g.Key.GroupId,
							  EvalItemKey = g.Key.EvalItemKey,
							  Characteristic = g.Average(x => x.Characteristic),
							  ResultValue = g.Average(x => x.ResultValue),
							  AdjustScore = g.Average(x => x.AdjustScore)
						  }).ToList();


			foreach (var item in list)
			{
				var id = item.TeacherId;
				var value = item.ResultValue * EvalAnalysis.AnnualEngine.QualityEvalUnit.ProportionValue;
				var characteristic = item.Characteristic;
				var adjustScore = item.AdjustScore * EvalAnalysis.AnnualEngine.QualityEvalUnit.ProportionValue;

				if (dic.ContainsKey(id))
				{
					dic[id].Quality.TargetId = item.DeclareTargetPKID;
					dic[id].Quality.TesGongz_Score = characteristic;
					dic[id].Quality.Adjust_Score = adjustScore;

					switch (item.EvalItemKey)
					{
						case EvalQualityRuleKeys.ZisFaz_DusHuod:
							dic[id].Quality.ZisFaz.DusHuod_Score = value;
							break;
						case EvalQualityRuleKeys.ZisFaz_KaizShik:
							dic[id].Quality.ZisFaz.Kaik_Score = value;
							break;
						case EvalQualityRuleKeys.ZisFaz_DanrPingwGongz:
							dic[id].Quality.ZisFaz.Pingw_Score = value;
							break;
						case EvalQualityRuleKeys.ZisFaz_PingbHuoj:
							dic[id].Quality.ZisFaz.Pingb_Score = value;
							break;
						case EvalQualityRuleKeys.ZisFaz_KaikPingwPingb:
							dic[id].Quality.KaikPingwPingb_Score = value;
							break;
						case EvalQualityRuleKeys.ZisFaz_FabLunw:
							dic[id].Quality.ZisFaz.FabLunw_Score = value;
							break;
						case EvalQualityRuleKeys.ZisFaz_LixKet:
							dic[id].Quality.ZisFaz.LixKet_Score = value;
							break;
						case EvalQualityRuleKeys.ZisFaz_XiangmYanj:
							dic[id].Quality.ZisFaz.XiangmYanj_Score = value;
							break;
						case EvalQualityRuleKeys.ZisFaz_Key:
							dic[id].Quality.Key_Score = value;
							break;
						case EvalQualityRuleKeys.PeixKec_KaisJiaosPeixKec:
							dic[id].Quality.PeixKec.KaisKec_Score = value;
							break;
						case EvalQualityRuleKeys.PeixKec_JiangzBaog:
							dic[id].Quality.PeixKec.JiangzBaog_Score = value;
							break;
						case EvalQualityRuleKeys.PeixKec_KecZiyKaif:
							dic[id].Quality.PeixKec.KecZiy_Score = value;
							break;
						case EvalQualityRuleKeys.PeixKec_Total:
							dic[id].Quality.PeixKec_Score = value;
							break;
						case EvalQualityRuleKeys.DaijJiaos_XueyShul:
							dic[id].Quality.DaijJiaos.XueyShul_Score = value;
							break;
						case EvalQualityRuleKeys.DaijJiaos_DaijJih:
							dic[id].Quality.DaijJiaos.DaijFanga_Score = value;
							break;
						case EvalQualityRuleKeys.DaijJiaos_XueyChengzFenx:
							dic[id].Quality.DaijJiaos.XueyChengzFenx_Score = value;
							break;
						case EvalQualityRuleKeys.DaijJiaos_DaijZhid:
							dic[id].Quality.DaijZhid_Score = value;
							break;
						case EvalQualityRuleKeys.DaijJiaos_KaizShik:
							dic[id].Quality.DaijJiaos.KaisZhansk_Score = value;
							break;
						case EvalQualityRuleKeys.DaijJiaos_FablunwHuocYukTiyJiu:
							dic[id].Quality.DaijJiaos.FabLunwHuoCanyKetYanj_Score = value;
							break;
						case EvalQualityRuleKeys.DaijJiaos_JiaoyJiaoxPingb:
							dic[id].Quality.DaijJiaos.JiaoyJiaoxPingbi_Score = value;
							break;
						case EvalQualityRuleKeys.DaijJiaos_DaijChengg:
							dic[id].Quality.DaijChengg_Score = value;
							break;
					}
				}
			}
			
		}


		private HSSFWorkbook NPOIHelper(Dictionary<long, InspectionViewModel> dic)
		{
			//创建Excel文件的对象
			NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
			//添加一个sheet
			NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

			#region [头部设计]
			//给sheet1添加第一行的头部标题
			NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
			row1.CreateCell(0).SetCellValue("序号");
			row1.CreateCell(1).SetCellValue("带教团队，称号");
			row1.CreateCell(3).SetCellValue("学科");
			row1.CreateCell(4).SetCellValue("姓名");
			row1.CreateCell(5).SetCellValue("单位");
			row1.CreateCell(6).SetCellValue("总得分/分值");
			row1.CreateCell(7).SetCellValue("校评得分/分值");
			row1.CreateCell(8).SetCellValue("量评得分/分值");
			row1.CreateCell(9).SetCellValue("质评得分/分值");
			row1.CreateCell(10).SetCellValue("特色分值/分值");
			row1.CreateCell(11).SetCellValue("师德");

			row1.CreateCell(12).SetCellValue("在校履职");
			row1.CreateCell(14).SetCellValue("自身发展");
			row1.CreateCell(22).SetCellValue("培训课程");
			row1.CreateCell(30).SetCellValue("带教教师");

			//给sheet1添加第二行的头部标题
			NPOI.SS.UserModel.IRow row2 = sheet1.CreateRow(1);
			row2.CreateCell(12).SetCellValue("量评得分/分值");
			row2.CreateCell(13).SetCellValue("质评得分/分值");
			row2.CreateCell(14).SetCellValue("读书活动");
			row2.CreateCell(16).SetCellValue("开课、评委、评比");
			row2.CreateCell(18).SetCellValue("科研");
			row2.CreateCell(20).SetCellValue("量评得分/分值");
			row2.CreateCell(21).SetCellValue("质评得分/分值");

			row2.CreateCell(22).SetCellValue("开设课程");
			row2.CreateCell(24).SetCellValue("讲座、报告");
			row2.CreateCell(26).SetCellValue("课程资源");
			row2.CreateCell(28).SetCellValue("量评得分/分值");
			row2.CreateCell(29).SetCellValue("质评得分/分值");

			row2.CreateCell(30).SetCellValue("带教指导");
			row2.CreateCell(32).SetCellValue("活动与经费制度");
			row2.CreateCell(34).SetCellValue("其他带教工作");
			row2.CreateCell(36).SetCellValue("带教成果");
			row2.CreateCell(38).SetCellValue("量评得分/分值");
			row2.CreateCell(39).SetCellValue("质评得分/分值");

			//给sheet1添加第三行的头部标题
			NPOI.SS.UserModel.IRow row3 = sheet1.CreateRow(2);
			row3.CreateCell(14).SetCellValue("量评得分/分值");
			row3.CreateCell(15).SetCellValue("质评得分/分值");
			row3.CreateCell(16).SetCellValue("量评得分/分值");
			row3.CreateCell(17).SetCellValue("质评得分/分值");
			row3.CreateCell(18).SetCellValue("量评得分/分值");
			row3.CreateCell(19).SetCellValue("质评得分/分值");

			row3.CreateCell(22).SetCellValue("量评得分/分值");
			row3.CreateCell(23).SetCellValue("质评得分/分值");
			row3.CreateCell(24).SetCellValue("量评得分/分值");
			row3.CreateCell(25).SetCellValue("质评得分/分值");
			row3.CreateCell(26).SetCellValue("量评得分/分值");
			row3.CreateCell(27).SetCellValue("质评得分/分值");

			row3.CreateCell(30).SetCellValue("量评得分/分值");
			row3.CreateCell(31).SetCellValue("质评得分/分值");
			row3.CreateCell(32).SetCellValue("量评得分/分值");
			row3.CreateCell(33).SetCellValue("质评得分/分值");
			row3.CreateCell(34).SetCellValue("量评得分/分值");
			row3.CreateCell(35).SetCellValue("质评得分/分值");
			row3.CreateCell(36).SetCellValue("量评得分/分值");
			row3.CreateCell(37).SetCellValue("质评得分/分值");

			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 0, 0)); //	序号
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 1, 2));	//	带教团队，称号
			//sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 2, 2));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 3, 3));	//	学科
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 4, 4));	// 姓名
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 5, 5));	//	单位
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 6, 6));  //总得分
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 7, 7));  //	校评得分
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 8, 8));  //	量评得分
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 9, 9));  //	质评得分
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 10, 10));  //	特色得分
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 11, 11));  //	师德

			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 12, 13));  //	在校履职
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 15, 22));  //	自身发展
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 23, 30));  //	培训课程
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 31, 40));  //	带教教师


			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 2, 12, 12));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 2, 13, 13));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 14, 15));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 16, 17));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 18, 19));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 2, 20, 20));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 2, 21, 21));

			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 22, 23));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 24, 25));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 26, 27));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 2, 28, 28));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 2, 29, 29));

			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 30, 31));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 32, 33));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 34, 35));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 36, 37));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 2, 38, 38));
			sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 2, 39, 39));

			#endregion

			var i = 0;
			foreach (var item in dic.Values)
			{
				i++;
				NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 2);
				sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i + 2, i + 2, 1, 2));
				rowtemp.CreateCell(0).SetCellValue(i);
				rowtemp.CreateCell(1).SetCellValue(item.TargetName);
				rowtemp.CreateCell(3).SetCellValue(item.SubjectName);
				rowtemp.CreateCell(4).SetCellValue(item.TeacherName);
				rowtemp.CreateCell(5).SetCellValue(item.CompanyName);
				rowtemp.CreateCell(6).SetCellValue(item.TotalStr);
				rowtemp.CreateCell(7).SetCellValue(item.SchoolStr);
				rowtemp.CreateCell(8).SetCellValue(item.VolumnStr);
				rowtemp.CreateCell(9).SetCellValue(item.QualityStr);
				rowtemp.CreateCell(10).SetCellValue(item.Quality.TesGongz_Str);
				rowtemp.CreateCell(11).SetCellValue(item.School.Shid);
				rowtemp.CreateCell(12).SetCellValue(item.School.Volumn_Str);
				rowtemp.CreateCell(13).SetCellValue(item.School.Quality_Str);
				rowtemp.CreateCell(14).SetCellValue(item.Volumn.DusHuod_Str);
				rowtemp.CreateCell(15).SetCellValue(item.Quality.DusHuod_Str);
				rowtemp.CreateCell(16).SetCellValue(item.Volumn.KaikPingwPingb_Str);
				rowtemp.CreateCell(17).SetCellValue(item.Quality.KaikPingwPingb_Str);
				rowtemp.CreateCell(18).SetCellValue(item.Volumn.Key_Str);
				rowtemp.CreateCell(19).SetCellValue(item.Quality.Key_Str);
				rowtemp.CreateCell(20).SetCellValue(item.Volumn.ZisFaz_Str);
				rowtemp.CreateCell(21).SetCellValue(item.Quality.ZisFaz_Str);
				rowtemp.CreateCell(22).SetCellValue(item.Volumn.KaisKec_Str);
				rowtemp.CreateCell(23).SetCellValue(item.Quality.KaisKec_Str);
				rowtemp.CreateCell(24).SetCellValue(item.Volumn.JiangzBaog_Str);
				rowtemp.CreateCell(25).SetCellValue(item.Quality.JiangzBaog_Str);
				rowtemp.CreateCell(26).SetCellValue(item.Volumn.KecZiy_Str);
				rowtemp.CreateCell(27).SetCellValue(item.Quality.KecZiy_Str);
				rowtemp.CreateCell(28).SetCellValue(item.Volumn.PeixKec_Str);
				rowtemp.CreateCell(29).SetCellValue(item.Quality.PeixKec_Str);
				rowtemp.CreateCell(30).SetCellValue(item.Volumn.DaijZhid_Str);
				rowtemp.CreateCell(31).SetCellValue(item.Quality.DaijZhid_Str);
				rowtemp.CreateCell(32).SetCellValue(item.Volumn.HuodJingfZhid);
				rowtemp.CreateCell(33).SetCellValue("无");
				rowtemp.CreateCell(34).SetCellValue(item.Volumn.QitDaijGongz_Str);
				rowtemp.CreateCell(35).SetCellValue(item.Quality.QitDaijGongz_Str);
				rowtemp.CreateCell(36).SetCellValue(item.Volumn.DaijChengg_Str);
				rowtemp.CreateCell(37).SetCellValue(item.Quality.DaijChengg_Str);
				rowtemp.CreateCell(38).SetCellValue(item.Volumn.DaijJiaos_Str);
				rowtemp.CreateCell(39).SetCellValue(item.Quality.DaijJiaos_Str);
			}

			return book;
		}

		#endregion

	}

}