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

		public class QualityEvalUnit2019 : QualityEvalUnit
		{

			protected override void AnalysisResult(FormCollection fc, EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items)
			{
				//三年计划部分

				Analysis_SannGuih_ZhidSix(result, items, fc[EvalQualityRuleKeys.SannGuih_ZhidSix], fc[EvalQualityRuleKeys.SannGuih_ZhidSix_Def]);
			}


			#region [ 三年计划分析 ]


			private void Analysis_SannGuih_ZhidSix(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.SannGuih_ZhidSix,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
						result.Score += scoreValue.EnsureInRange(0, 10);
						result.DynamicScore1 += scoreValue.EnsureInRange(0, 10);
						break;
				}

				item.ResultValue = scoreValue.ToString();
			}


			#endregion

		}

	}

}