using Business;
using Business.BasicExtinsions;
using Business.Helper;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace TheSite.EvalAnalysis
{

	public partial class AnnualEngine
	{

		public class QualityEvalUnit2019 : QualityEvalUnit
		{

			private string _zeroScore = "0";

			protected override void AnalysisResult(FormCollection fc, EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items)
			{
				//三年规划部分

				Analysis_SannGuih_ZhidSix(result, items, fc[EvalQualityRuleKeys.SannGuih_ZhidSix], fc[EvalQualityRuleKeys.SannGuih_ZhidSix_Def]);
				Analysis_SannGuih_XianzFenx(result, items, fc[EvalQualityRuleKeys.GerJiH_ZiwFengx], fc[EvalQualityRuleKeys.GerJiH_ZiwFengx_Def]);
				Analysis_SanGuih_FazMub(result, items, fc[EvalQualityRuleKeys.SanGuih_FazMub], fc[EvalQualityRuleKeys.SanGuih_FazMub_Def]);
				Analysis_SanGuih_JutShisCuos(result, items, fc[EvalQualityRuleKeys.SanGuih_JutShisCuos], fc[EvalQualityRuleKeys.SanGuih_JutShisCuo_Def]);
				Analysis_SanGuih_GuanlBaoz(result, items, fc[EvalQualityRuleKeys.SanGuih_GuanlBaoz], fc[EvalQualityRuleKeys.SanGuih_GuanlBaoz_Def]);

				//团队计划部分

				Analysis_TuandJih_ZhidSix(result, items, fc[EvalQualityRuleKeys.TuandJih_ZhidSix], fc[EvalQualityRuleKeys.TuandJih_ZhidSix_Def]);
				Analysis_TuandJih_XueqMub(result, items, fc[EvalQualityRuleKeys.TuandJih_XueqMub], fc[EvalQualityRuleKeys.TuandJih_XueqMub_Def]);
				Analysis_TuandJih_JutAnp(result, items, fc[EvalQualityRuleKeys.TuandJih_JutAnp], fc[EvalQualityRuleKeys.TuandJih_JutAnp_Def]);
				Analysis_TuandJih_KaohPingj(result, items, fc[EvalQualityRuleKeys.TuandJih_KaohPingj], fc[EvalQualityRuleKeys.TuandJih_KaohPingj_Def]);

				//个人分析部分

				Analysis_GerJiH_ZiwFengx(result, items, fc[EvalQualityRuleKeys.GerJiH_ZiwFengx], fc[EvalQualityRuleKeys.GerJiH_ZiwFengx_Def]);
				Analysis_GerJiH_FazMub(result, items, fc[EvalQualityRuleKeys.GerJiH_FazMub], fc[EvalQualityRuleKeys.GerJiH_FazMub_Def]);
				Analysis_GerJiH_JutShis(result, items, fc[EvalQualityRuleKeys.GerJiH_JutShis], fc[EvalQualityRuleKeys.GerJiH_JutShis_Def]);

			}


			#region [ 三年规划分析 ]


			private void Analysis_SannGuih_ZhidSix(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.SannGuih_ZhidSix,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 10);
				result.DynamicScore1 += scoreValue.EnsureInRange(0, 10);

				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_SannGuih_XianzFenx(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.SannGuih_XianzFenx,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 20);
				result.DynamicScore1 += scoreValue.EnsureInRange(0, 20);

				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_SanGuih_FazMub(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.SanGuih_FazMub,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 25);
				result.DynamicScore1 += scoreValue.EnsureInRange(0, 25);

				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_SanGuih_JutShisCuos(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.SanGuih_JutShisCuos,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 30);
				result.DynamicScore1 += scoreValue.EnsureInRange(0, 30);

				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_SanGuih_GuanlBaoz(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.SanGuih_GuanlBaoz,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 15);
				result.DynamicScore1 += scoreValue.EnsureInRange(0, 15);

				item.ResultValue = scoreValue.ToString();
			}

			#endregion


			#region [ 团队计划分析 ]


			private void Analysis_TuandJih_ZhidSix(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.TuandJih_ZhidSix,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 20);
				result.DynamicScore2 += scoreValue.EnsureInRange(0, 20);

				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_TuandJih_XueqMub(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.TuandJih_XueqMub,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 20);
				result.DynamicScore2 += scoreValue.EnsureInRange(0, 20);

				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_TuandJih_JutAnp(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.TuandJih_JutAnp,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 40);
				result.DynamicScore2 += scoreValue.EnsureInRange(0, 40);

				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_TuandJih_KaohPingj(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.TuandJih_KaohPingj,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 20);
				result.DynamicScore2 += scoreValue.EnsureInRange(0, 20);

				item.ResultValue = scoreValue.ToString();
			}


			#endregion


			#region [ 个人计划分析 ]


			private void Analysis_GerJiH_ZiwFengx(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.GerJiH_ZiwFengx,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 20);
				result.DynamicScore3 += scoreValue.EnsureInRange(0, 20);

				item.ResultValue = scoreValue.ToString();
			}

			private void Analysis_GerJiH_FazMub(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.GerJiH_FazMub,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 30);
				result.DynamicScore3 += scoreValue.EnsureInRange(0, 30);

				item.ResultValue = scoreValue.ToString();
			}

			private void Analysis_GerJiH_JutShis(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.GerJiH_JutShis,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 50);
				result.DynamicScore3 += scoreValue.EnsureInRange(0, 50);

				item.ResultValue = scoreValue.ToString();
			}


			#endregion

		}

	}

}