using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TheSite.EvalAnalysis
{

	public partial class DeclareEngine
	{

		public abstract class DeclareEvalUnit : DeclareEvalUnitBase
		{

         protected abstract void AnalysisResult(FormCollection fc, EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items);
            
         static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
         static APDBDef.EvalDeclareResultTableDef er = APDBDef.EvalDeclareResult;
         static APDBDef.EvalDeclareResultItemTableDef eri = APDBDef.EvalDeclareResultItem;
         static APDBDef.ExpGroupTableDef g = APDBDef.ExpGroup;
         static APDBDef.DeclareReviewTableDef dr = APDBDef.DeclareReview;

         public override double FullScroe => 100;


         public override double Proportion => 1;


         public override string RuleView
         	=> ViewPath + "/DeclareRuleView" + TargetId;


         public override string EvalView
         	=> ViewPath + "/DeclareEvalView" + TargetId;


         public override string ResultView
         	=> ViewPath + "/DeclareResultView" + TargetId;


         //public override string SubmitResultView
         //	=> ViewPath + "/QualityEvalSubmitView";


         //public override EvalQualitySubmitResult GetSubmitResult(APDBDef db, QualityEvalParam param)
         //{
         //	var t = APDBDef.EvalQualitySubmitResult;

         //	return APQuery.select(t.Asterisk, g.Name, u.RealName)
         //		.from(t,
         //			u.JoinInner(t.Accesser == u.UserId),
         //			g.JoinInner(t.GroupId == g.GroupId))
         //		.where(t.PeriodId == param.PeriodId & t.TeacherId == param.TeacherId)
         //		.query(db, r =>
         //		{
         //			EvalQualitySubmitResult data = new EvalQualitySubmitResult();
         //			t.Fullup(r, data, false);
         //			data.AccesserName = u.RealName.GetValue(r);
         //			data.GroupName = g.Name.GetValue(r);
         //			return data;
         //		}).FirstOrDefault();
         //}


         public override List<EvalDeclareResult> GetResults(APDBDef db, DeclareEvalParam param)
         {
            var query = APQuery.select(er.Asterisk, u.RealName)
               .from(er, u.JoinInner(er.Accesser == u.UserId))
               .where(er.PeriodId == param.PeriodId & er.TeacherId == param.TeacherId)
               .query(db, r =>
               {
                  EvalDeclareResult data = new EvalDeclareResult();
                  er.Fullup(r, data, false);
                  data.AccesserName = u.RealName.GetValue(r);
                  return data;
               }).ToList();


            return query;
         }


         public override EvalDeclareResult GetResult(APDBDef db, DeclareEvalParam param)
         {
            var t = APDBDef.EvalDeclareResult;

            return APQuery.select(t.Asterisk, u.RealName)
               .from(t, u.JoinInner(t.Accesser == u.UserId))
               .where(t.ResultId == param.ResultId)
               .query(db, r =>
               {
                  EvalDeclareResult data = new EvalDeclareResult();
                  t.Fullup(r, data, false);
                  data.AccesserName = u.RealName.GetValue(r);
                  return data;
               }).FirstOrDefault();
         }


         public override Dictionary<string, EvalDeclareResultItem> GetResultItem(APDBDef db, DeclareEvalParam param)
            => APQuery.select(eri.EvalItemKey, eri.ChooseValue, eri.ResultValue)
               .from(eri)
               .where(eri.ResultId == param.ResultId)
               .query(db, r =>
               {
                  return new EvalDeclareResultItem
                  {
                     EvalItemKey = eri.EvalItemKey.GetValue(r),
                     ChooseValue = eri.ChooseValue.GetValue(r),
                     ResultValue = eri.ResultValue.GetValue(r)
                  };
               }).ToDictionary(m => m.EvalItemKey);


         public override long Eval(APDBDef db, DeclareEvalParam param, FormCollection fc)
         {
            var eval = db.EvalDeclareResultDal.ConditionQuery(er.PeriodId == param.PeriodId & er.TeacherId == param.TeacherId
            & er.Accesser == param.AccesserId, null, null, null).FirstOrDefault();

            var declareReview = db.DeclareReviewDal.ConditionQuery(dr.TeacherId==param.TeacherId & dr.StatusKey==DeclareKeys.ReviewSuccess,null,null,null).FirstOrDefault();

            if (declareReview == null) throw new ApplicationException("该老师的申报请求还未通过校审核！");

            var result = new EvalDeclareResult()
            {
               PeriodId = param.PeriodId,
               TeacherId = param.TeacherId,
               GroupId = param.GroupId,
               Accesser = param.AccesserId,
               AccessDate = DateTime.Now,
               DeclareTargetPKID = param.TargetId,
               DeclareCompanyId = declareReview.CompanyId,
               DeclareSubjectPKID = declareReview.DeclareSubjectPKID,
               FullScore = FullScroe,
            };
            var items = new Dictionary<string, EvalDeclareResultItem>();

            AnalysisResult(fc, result, items);

            if (eval != null)
            {
               db.EvalDeclareResultDal.PrimaryDelete(eval.ResultId);
               db.EvalDeclareResultItemDal.ConditionDelete(eri.ResultId == eval.ResultId);
            }

            db.EvalDeclareResultDal.Insert(result);

            foreach (var item in items.Values)
            {
               item.ResultId = result.ResultId;
               db.EvalDeclareResultItemDal.Insert(item);
            }

            return result.ResultId;
         }


         public override Dictionary<string, string> ChooseEvalResultItems(Dictionary<string, EvalDeclareResultItem> items)
         {
            return null;
         }

         protected virtual IEnumerable<EvalQualityResultItem> Choose(int takeCount, params EvalQualityResultItem[] items)
           => items.OrderByDescending(item => Convert.ToDouble(item.ResultValue.Replace("分", string.Empty))).Take(takeCount);

      }

	}

}