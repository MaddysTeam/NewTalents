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

		public class SchoolEvalUnit : SchoolEvalUnitBase
		{
			static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
			static APDBDef.EvalSchoolResultTableDef er = APDBDef.EvalSchoolResult;
			static APDBDef.EvalSchoolResultItemTableDef eri = APDBDef.EvalSchoolResultItem;


			public override double FullScroe => 100;

			public override double Proportion => 0.3;

			public static double ProportionValue => 0.3;

			//TODO: for eval 2020
			public static Dictionary<long, double> PropertionValues => new Dictionary<long, double> {
				{ Business.Helper.DeclareTargetIds.TezXuey, 0.8 },
				{ Business.Helper.DeclareTargetIds.JiaoxXinx, 0.8 },
				{ Business.Helper.DeclareTargetIds.JiaoxNengs, 0.8 },
				{ Business.Helper.DeclareTargetIds.GugJiaos, 0.3 },
				{ Business.Helper.DeclareTargetIds.XuekDaitr, 0.3 },
				{ Business.Helper.DeclareTargetIds.GongzsZhucr, 0.8 },
				{ Business.Helper.DeclareTargetIds.JidZhucr, 0.8 },
				{ Business.Helper.DeclareTargetIds.GaodLisz, 0.8 },
			};


			public override EvalSchoolResult GetResult(APDBDef db, SchoolEvalParam param)
			{
				return APQuery.select(er.Asterisk, u.RealName)
					 .from(er, u.JoinInner(er.Accesser == u.UserId))
					 .where(er.PeriodId == param.PeriodId & er.TeacherId == param.TeacherId)
					 .query(db, r =>
					 {
						 EvalSchoolResult data = new EvalSchoolResult();
						 er.Fullup(r, data, false);
						 data.AccesserName = u.RealName.GetValue(r);
						 return data;
					 }).FirstOrDefault();
			}


			public override Dictionary<string, EvalSchoolResultItem> GetResultItem(APDBDef db, SchoolEvalParam param)
				 => APQuery.select(eri.EvalItemKey, eri.ChooseValue, eri.ResultValue)
					  .from(er, eri.JoinInner(er.ResultId == eri.ResultId))
					  .where(er.PeriodId == param.PeriodId & er.TeacherId == param.TeacherId)
					  .query(db, r =>
					  {
						  return new EvalSchoolResultItem
						  {
							  EvalItemKey = eri.EvalItemKey.GetValue(r),
							  ChooseValue = eri.ChooseValue.GetValue(r),
							  ResultValue = eri.ResultValue.GetValue(r)
						  };
					  }).ToDictionary(m => m.EvalItemKey);


			public override Dictionary<string, string> ChooseEvalResultItems(Dictionary<string, EvalSchoolResultItem> items)
			{
				throw new NotImplementedException();
			}


			public override long Eval(APDBDef db, SchoolEvalParam param, FormCollection fc)
			{
				var eval = db.EvalSchoolResultDal.ConditionQuery(er.PeriodId == param.PeriodId & er.TeacherId == param.TeacherId
						  & er.CompanyId == param.SchoolId, null, null, null).FirstOrDefault();

				var result = new EvalSchoolResult()
				{
					PeriodId = param.PeriodId,
					TeacherId = param.TeacherId,
					Accesser = param.AccesserId,
					AccessDate = DateTime.Now,
					CompanyId = param.SchoolId,
					FullScore = FullScroe
				};
				var items = new Dictionary<string, EvalSchoolResultItem>();

				AnalysisResult(fc, result, items);


				if (eval != null)
				{
					db.EvalSchoolResultDal.PrimaryDelete(eval.ResultId);
					db.EvalSchoolResultItemDal.ConditionDelete(eri.ResultId == eval.ResultId);
				}

				db.EvalSchoolResultDal.Insert(result);

				foreach (var item in items.Values)
				{
					item.ResultId = result.ResultId;
					db.EvalSchoolResultItemDal.Insert(item);
				}


				return result.ResultId;
			}


			#region [ 分析 ]


			private void AnalysisResult(FormCollection fc, EvalSchoolResult result, Dictionary<string, EvalSchoolResultItem> items)
			{
				//AnalysisGongzl(result, items, fc[EvalSchoolRuleKeys.XiaonLvz_Gongzl]);
				AnalysisGongzZhil(result, items, fc[EvalSchoolRuleKeys.XiaonLvz_GongzZhil], fc[EvalSchoolRuleKeys.XiaonLvz_GongzZhil_Jeig]);
				AnalysisShid(result, items, fc[EvalSchoolRuleKeys.Shid]);
			}

			private void AnalysisGongzZhil(EvalSchoolResult result, Dictionary<string, EvalSchoolResultItem> items, string choose, string score)
			{
				var item = new EvalSchoolResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalSchoolRuleKeys.XiaonLvz_GongzZhil,
				};
				items.Add(item.EvalItemKey, item);


				// 客户端数据不可相信，还得验证

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 100);
				item.ResultValue = result.Score.ToString();
			}


			private void AnalysisShid(EvalSchoolResult result, Dictionary<string, EvalSchoolResultItem> items, string choose)
			{
				var item = new EvalSchoolResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalSchoolRuleKeys.Shid,
				};
				items.Add(item.EvalItemKey, item);

				switch (choose)
				{
					case "A":
						result.Morality = "合格";
						item.ResultValue = "合格";
						break;
					case "B":
						result.Morality = "基本合格";
						item.ResultValue = "基本合格";
						break;
					case "C":
						result.Score = 0;
						result.Morality = "不合格";
						item.ResultValue = "不合格";
						break;
				}
			}


			#endregion

		}

	}

}