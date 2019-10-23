using Business;
using Business.Helper;
using Business.BasicExtinsions;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TheSite.EvalAnalysis
{

	public partial class AnnualEngine
	{

		public class TeamEvalUnit : TeamEvalUnitBase
		{
			static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
			static APDBDef.TeamEvalResultTableDef er = APDBDef.TeamEvalResult;
			static APDBDef.TeamEvalResultItemTableDef eri = APDBDef.TeamEvalResultItem;


			public override double FullScroe
			=> 100;

			public override double Proportion
			=> 0.3;

			public static double ProportionValue => 0.3;


			public override TeamEvalResult GetResult(APDBDef db, TeamEvalParam param)
			{
				return APQuery.select(er.Asterisk, u.RealName)
					 .from(er, u.JoinInner(er.Accesser == u.UserId))
					 .where(er.PeriodId == param.PeriodId & er.MemberId == param.TeacherId)
					 .query(db, r =>
					 {
						 TeamEvalResult data = new TeamEvalResult();
						 er.Fullup(r, data, false);
						 // data.AccesserName = u.RealName.GetValue(r);
						 return data;
					 }).FirstOrDefault();
			}


			public override Dictionary<string, TeamEvalResultItem> GetResultItem(APDBDef db, TeamEvalParam param)
				 =>
				APQuery.select(eri.EvalItemKey, eri.ChooseValue, eri.ResultValue)
					  .from(er, eri.JoinInner(er.ResultId == eri.ResultId))
					  .where(er.PeriodId == param.PeriodId & er.MemberId == param.TeacherId)
					  .query(db, r =>
					  {
						  return new TeamEvalResultItem
						  {
							  EvalItemKey = eri.EvalItemKey.GetValue(r),
							  ChooseValue = eri.ChooseValue.GetValue(r),
							  ResultValue = eri.ResultValue.GetValue(r)
						  };
					  }).ToDictionary(m => m.EvalItemKey);


			public override long Eval(APDBDef db, TeamEvalParam param, FormCollection fc)
			{
				var eval = db.TeamEvalResultDal.ConditionQuery(er.PeriodId == param.PeriodId & er.MemberId == param.TeacherId
						  & er.TeamId == param.TeamId, null, null, null).FirstOrDefault();

				var result = new TeamEvalResult()
				{
					PeriodId = param.PeriodId,
					MemberId = param.TeacherId,
					Accesser = param.AccesserId,
					AccessDate = DateTime.Now,
					TeamId = param.TeamId,
					FullScore = FullScroe
				};
				var items = new Dictionary<string, TeamEvalResultItem>();

				AnalysisResult(fc, result, items);

				if (eval != null)
				{
					db.TeamEvalResultDal.PrimaryDelete(eval.ResultId);
					db.TeamEvalResultItemDal.ConditionDelete(eri.ResultId == eval.ResultId);
				}

				db.TeamEvalResultDal.Insert(result);

				foreach (var item in items.Values)
				{
					item.ResultId = result.ResultId;
					db.TeamEvalResultItemDal.Insert(item);
				}

				return result.ResultId;
			}


			#region [ 分析 ]


			private void AnalysisResult(FormCollection fc, TeamEvalResult result, Dictionary<string, TeamEvalResultItem> items)
			{
				//AnalysisGongzl(result, items, fc[EvalSchoolRuleKeys.XiaonLvz_Gongzl]);
			}


			//private void AnalysisGongzl(EvalSchoolResult result, Dictionary<string, EvalSchoolResultItem> items, string choose)
			//{
			//	var item = new EvalSchoolResultItem
			//	{
			//		ChooseValue = choose,
			//		EvalItemKey = EvalSchoolRuleKeys.XiaonLvz_Gongzl,
			//	};
			//	items.Add(item.EvalItemKey, item);

			//	switch (choose)
			//	{
			//		case "A":
			//			result.Score += 50;
			//			item.ResultValue = "50分";
			//			break;
			//		case "B":
			//			result.Score += 40;
			//			item.ResultValue = "40分";
			//			break;
			//		case "C":
			//			result.Score += 25;
			//			item.ResultValue = "25分";
			//			break;
			//		case "D":
			//			result.Score += 15;
			//			item.ResultValue = "15分";
			//			break;
			//	}
			//}





			#endregion

		}

	}

}