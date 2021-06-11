using Business;
using Business.Helper;
using Business.BasicExtinsions;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Business.Config;

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
			=> 1;

			// this field for eval 2020 only to distinguish which target id is special , then speical eval target is team leader and eval by specal expert account
			public override long TargetId => 1;

			public static double ProportionValue => 1;

			public override string EvalView => ViewPath + "/TeamEvalView";


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
					FullScore = FullScroe,
					Comment = fc["Comment"]
				};
				var items = new Dictionary<string, TeamEvalResultItem>();

				//AnalysisResult(fc, result, items);

				//Analysis_GerJiH_ZiwFengx(result, items, fc[TeamEvalRuleKeys.GerJiH_ZiwFengx], fc[TeamEvalRuleKeys.GerJiH_ZiwFengx_Def]);
				//Analysis_GerJiH_FazMub(result, items, fc[TeamEvalRuleKeys.GerJiH_FazMub], fc[TeamEvalRuleKeys.GerJiH_FazMub_Def]);
				//Analysis_GerJiH_JutShis(result, items, fc[TeamEvalRuleKeys.GerJiH_JutShis], fc[TeamEvalRuleKeys.GerJiH_JutShis_Def]);

				Analysis_ChuqLv(result, items, fc[EvalQualityRuleKeys.TuandKaoh], fc[EvalQualityRuleKeys.TuandKaoh_Def]);

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


			string _zeroScore = "0";

			#region [20200910 remove]
			//protected virtual void AnalysisResult(FormCollection fc, TeamEvalResult result, Dictionary<string, TeamEvalResultItem> items)
			//{
			//	AnalysisResult(result, items, null, fc[EvalQualityRuleKeys.TuandKaoh_Def]);
			//}

			//private void AnalysisResult(TeamEvalResult result, Dictionary<string, TeamEvalResultItem> items, string choose, string score)
			//{
			//	var item = new TeamEvalResultItem
			//	{
			//		EvalItemKey = EvalQualityRuleKeys.TuandKaoh,
			//	};
			//	items.Add(item.EvalItemKey, item);

			//	score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
			//	var scoreValue = Convert.ToDouble(score);
			//	result.Score += scoreValue.EnsureInRange(0, 100);

			//	item.ResultValue = scoreValue.ToString();
			//}
			#endregion

			#region [ 个人计划分析 ]

			private void Analysis_GerJiH_ZiwFengx(TeamEvalResult result, Dictionary<string, TeamEvalResultItem> items, string choose, string score)
			{
				var item = new TeamEvalResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.GerJiH_ZiwFengx,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 20);
				//result.DynamicScore3 += scoreValue.EnsureInRange(0, 20);

				item.ResultValue = scoreValue.ToString();
			}

			private void Analysis_GerJiH_FazMub(TeamEvalResult result, Dictionary<string, TeamEvalResultItem> items, string choose, string score)
			{
				var item = new TeamEvalResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.GerJiH_FazMub,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 30);
				//result.DynamicScore3 += scoreValue.EnsureInRange(0, 30);

				item.ResultValue = scoreValue.ToString();
			}

			private void Analysis_GerJiH_JutShis(TeamEvalResult result, Dictionary<string, TeamEvalResultItem> items, string choose, string score)
			{
				var item = new TeamEvalResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.GerJiH_JutShis,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 50);
				//result.DynamicScore3 += scoreValue.EnsureInRange(0, 50);

				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_ChuqLv(TeamEvalResult result, Dictionary<string, TeamEvalResultItem> items, string choose, string score)
			{
				var item = new TeamEvalResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.TuandKaoh,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 100);
				//result.DynamicScore3 += scoreValue.EnsureInRange(0, 50);

				item.ResultValue = scoreValue.ToString();
			}



			#endregion

		}

	}

}