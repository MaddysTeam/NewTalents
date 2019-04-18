using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TheSite.EvalAnalysis;
using TheSite.Models;

namespace Business
{
	public static class EvalPeriodExtensions
	{

		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.ExpGroupTableDef g = APDBDef.ExpGroup;


        public static List<EvalPeriod> GetAllPeriods(this APDBDef db)
        {
            return db.EvalPeriodDal.ConditionQuery(null, null, null, null);
        }

	
		public static EvalPeriod GetCurrentEvalPeriod(this APDBDef db)
		{
			APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;

			var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent==true,null,null,null)
				.FirstOrDefault();


			return period;
		}


        public static bool InAccessDateRegion(this EvalPeriod period, DateTime time)
        => period.AccessBeginDate < time && period.AccessEndDate > time;


        public static DeclareModel GetDeclareInfo(this PeriodModel period, APDBDef db)
			=> GetDeclareInfo(db, period.TeacherId);


		public static DeclareModel GetDeclareInfo(this EvalParam param, APDBDef db)
			=> GetDeclareInfo(db, param.TeacherId);


      public static DeclareReviewModel GetDeclareReviewInfo(this DeclareEvalParam param, APDBDef db) 
         => GetDeclareReviewInfo(db, param.TeacherId);



      private static DeclareModel GetDeclareInfo(APDBDef db, long teacherId)
		{
			APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
			APDBDef.CompanyDeclareTableDef cd = APDBDef.CompanyDeclare;
			APDBDef.CompanyTableDef c = APDBDef.Company;

			var model = APQuery.select(u.RealName, d.TeacherId, d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID, c.CompanyName)
				.from(u,
						d.JoinInner(u.UserId == d.TeacherId),
						cd.JoinLeft(u.UserId == cd.TeacherId),
						c.JoinLeft(c.CompanyId == cd.CompanyId))
				.where(u.UserId == teacherId)
				.query(db, rd =>
				{
					return new DeclareModel
					{
						TeacherId = d.TeacherId.GetValue(rd),
						RealName = u.RealName.GetValue(rd),
						CompanyName = c.CompanyName.GetValue(rd),
						TargetId = d.DeclareTargetPKID.GetValue(rd),
						Target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
						Subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
						Stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
					};
				}).FirstOrDefault();


			return model;
		}

      private static DeclareReviewModel GetDeclareReviewInfo(APDBDef db, long teacherId)
      {
         APDBDef.DeclareReviewTableDef dr = APDBDef.DeclareReview;
         APDBDef.CompanyDeclareTableDef cd = APDBDef.CompanyDeclare;
         APDBDef.CompanyTableDef c = APDBDef.Company;

         var model = APQuery.select(u.RealName, dr.TeacherId, dr.DeclareTargetPKID, dr.DeclareSubjectPKID, dr.TypeKey, c.CompanyName)
            .from(u,
                  dr.JoinInner(u.UserId == dr.TeacherId),
                  c.JoinLeft(c.CompanyId == dr.CompanyId))
            .where(u.UserId == teacherId)
            .query(db, rd =>
            {
               return new DeclareReviewModel
               {
                  TeacherId = dr.TeacherId.GetValue(rd),
                  RealName = u.RealName.GetValue(rd),
                  CompanyName = c.CompanyName.GetValue(rd),
                  TargetId = dr.DeclareTargetPKID.GetValue(rd),
                  Target = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(rd)),
                  Subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(rd)),
                  TypeKey=dr.TypeKey.GetValue(rd)
               };
            }).FirstOrDefault();


         return model;
      }

   }

}
