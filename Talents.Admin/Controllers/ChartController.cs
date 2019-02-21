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

	public class ChartController : BaseController
	{


		static APDBDef.TeamMemberTableDef tm = APDBDef.TeamMember;
		static APDBDef.TeamActiveTableDef ta = APDBDef.TeamActive;
		static APDBDef.TeamActiveResultTableDef tar = APDBDef.TeamActiveResult;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;

		//	GET:	Chart/Team

		public ActionResult TeamInfo()
		{
			var list = APQuery.select(d.DeclareTargetPKID, d.DeclareTargetPKID.Count().As("TotalCount"))
				.from(d)
				.group_by(d.DeclareTargetPKID)
				.order_by(d.DeclareTargetPKID.Count().Desc)
				.query(db, r =>
				{
					return new
					{
						label = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(r)),
						data = r["TotalCount"],
					};
				}).ToArray();


			return Json(new
			{
				result = AjaxResults.Success,
				list
			} , JsonRequestBehavior.AllowGet);
		}


		//	GET:	Chart/SchoolEval

		public ActionResult SchoolEval()
		{
			var t = APDBDef.EvalSchoolResult;

			var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null).FirstOrDefault();

			if (period != null)
			{
				var total = db.CompanyDeclareDal.ConditionQueryCount(null);
				var evalTotal = db.EvalSchoolResultDal.ConditionQueryCount(t.PeriodId == period.PeriodId);
				var notEvalTotal = total - evalTotal;

				var list = MakeData(evalTotal, notEvalTotal);


				return Json(new
				{
					result = AjaxResults.Success,
					data = list.ToArray()
				}, JsonRequestBehavior.AllowGet);
			}


			return Json(new
			{
				result = AjaxResults.Error,
				msg = "请设置一个考核周期!"
			}, JsonRequestBehavior.AllowGet);
		}


		//	GET:	Chart/SchoolAdminEval

		public ActionResult SchoolAdminEval()
		{
			var t = APDBDef.EvalSchoolResult;
			var cd = APDBDef.CompanyDeclare;
			var ca = APDBDef.CompanyAccesser;

			var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null).FirstOrDefault();

			if (period != null)
			{
				var total = APQuery.select(cd.TeacherId)
					.from(ca, cd.JoinLeft(ca.CompanyId == cd.CompanyId))
					.where(ca.UserId == UserProfile.UserId)
					.count(db);
				var evalTotal = db.EvalSchoolResultDal.ConditionQueryCount(t.PeriodId == period.PeriodId & t.Accesser == UserProfile.UserId);
				var notEvalTotal = total - evalTotal;

				var list = MakeData(evalTotal, notEvalTotal);


				return Json(new
				{
					result = AjaxResults.Success,
					data = list.ToArray()
				}, JsonRequestBehavior.AllowGet);
			}

			
			return Json(new
			{
				result = AjaxResults.Error,
				msg = "当前时间不在考核周期之内!"
			}, JsonRequestBehavior.AllowGet);
		}

		//	GET:	Chart/VolumnEval

		public ActionResult VolumnEval()
		{
			var t = APDBDef.EvalVolumnResult;

			var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null).FirstOrDefault();

			if (period != null)
			{
				var total = db.DeclareBaseDal.ConditionQueryCount(d.DeclareTargetPKID < DeclareTargetIds.JiaoxNengs &
						 d.DeclareTargetPKID > DeclareTargetIds.WaipDaos);
				var evalTotal = db.EvalVolumnResultDal.ConditionQueryCount(t.PeriodId == period.PeriodId);
				var notEvalTotal = total - evalTotal;

				var list = MakeData(evalTotal, notEvalTotal);


				return Json(new
				{
					result = AjaxResults.Success,
					data = list.ToArray()
				}, JsonRequestBehavior.AllowGet);
			}


			return Json(new
			{
				result = AjaxResults.Error,
				msg = "请设置一个考核周期!"
			}, JsonRequestBehavior.AllowGet);
		}


		//	GET:	Chart/Quality


		public ActionResult QualityEval()
		{
			var t = APDBDef.EvalQualitySubmitResult;

			var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null).FirstOrDefault();

			if (period != null)
			{
				var total = db.ExpGroupTargetDal.ConditionQueryCount(null);
				var evalTotal = db.EvalQualitySubmitResultDal.ConditionQueryCount(t.PeriodId == period.PeriodId);
				var notEvalTotal = total - evalTotal;

				var list = MakeData(evalTotal, notEvalTotal);


				return Json(new
				{
					result = AjaxResults.Success,
					data = list.ToArray()
				}, JsonRequestBehavior.AllowGet);
			}


			return Json(new
			{
				result = AjaxResults.Error,
				msg = "请设置一个考核周期!"
			}, JsonRequestBehavior.AllowGet);
		}


		#region [ helper]

		public List<ChartViewModel> MakeData(int evalTotal, int notEvalTotal)
		{
			List<ChartViewModel> list = new List<ChartViewModel>();
			list.Add(new ChartViewModel() { label = "未评学员", data = notEvalTotal });
			list.Add(new ChartViewModel() { label = "已评学员", data = evalTotal });

			return list;
		}

		#endregion

	}

}