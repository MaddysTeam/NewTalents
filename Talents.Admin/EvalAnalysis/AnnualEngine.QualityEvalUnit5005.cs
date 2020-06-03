using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TheSite.EvalAnalysis
{

	public partial class AnnualEngine
	{

		public class QualityEvalUnit5005 : QualityEvalUnit
		{

			public override long TargetId
				=> 5005;

			public override string EvalView => ViewPath + "/QualityEvalView5005";
			public override string ResultView => ViewPath + "/QualityResultView5005";

			double score = 0, score1 = 0, score2 = 0;

			protected override void AnalysisResult(FormCollection fc, EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items)
			{
				score1 = Analysis_KetJiaox_Gongkk(result, items, fc[EvalQualityRuleKeys.KetJiaox_Gongkk], fc[EvalQualityRuleKeys.KetJiaox_Gongkk_Def]);
				score2 = Analysis_KetJiaox_Zhidk(result, items, fc[EvalQualityRuleKeys.KetJiaox_Zhidk], fc[EvalQualityRuleKeys.KetJiaox_Zhidk_Def]);

				result.Score = new double[] { score1, score2 }.Max();

				Analysis_KaisJiangz(result, items, fc[EvalQualityRuleKeys.KaisJiangz], fc[EvalQualityRuleKeys.KaisJiangz_Def]);
				Analysis_DaijJiaos(result, items, fc[EvalQualityRuleKeys.DaijJiaos], fc[EvalQualityRuleKeys.DaijJiaos_Def]);
				Analysis_XiangmYanj(result, items, fc[EvalQualityRuleKeys.XiangmYanj], fc[EvalQualityRuleKeys.XiangmYanj_Def]);
				Analysis_Lunw(result, items, fc[EvalQualityRuleKeys.Lunw], fc[EvalQualityRuleKeys.Lunw_Def]);
				Analysis_ShijPeixKec(result, items, fc[EvalQualityRuleKeys.ShijPeixKec], fc[EvalQualityRuleKeys.ShijPeixKec_Def]);
				Analysis_Tes(result, items, fc[EvalQualityRuleKeys.Tes], fc[EvalQualityRuleKeys.Tes_Def]);
			}


			#region [ 教育教学 ]


			private double Analysis_KetJiaox_Gongkk(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.KetJiaox_Gongkk,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 18.0, 20.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 12.0, 17.9);
						break;
					case "C":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 0, 11.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			private double Analysis_KetJiaox_Zhidk(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.KetJiaox_Zhidk,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 18.0, 20.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 12.0, 17.9);
						break;
					case "C":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 0, 11.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			private void Analysis_KaisJiangz(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.KaisJiangz,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 10.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 6.0, 8.9);
						break;
					case "C":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 0, 5.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_DaijJiaos(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.DaijJiaos,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 10.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 6.0, 8.9);
						break;
					case "C":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 0, 5.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();
			}


			#endregion


			#region [ 教育科研 ]


			private void Analysis_XiangmYanj(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.XiangmYanj,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 10.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 6.0, 8.9);
						break;
					case "C":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 0, 5.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_Lunw(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.Lunw,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 10.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 6.0, 8.9);
						break;
					case "C":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 0, 5.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();
			}


			#endregion


			#region [ 市级培训课程 ]

			private void Analysis_ShijPeixKec(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.ShijPeixKec,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 27.0, 30.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 18.0, 26.9);
						break;
					case "C":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 0, 17.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();
			}

			#endregion


			#region [ 特色 ]

			private void Analysis_Tes(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.Tes,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 10.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 6.0, 8.9);
						break;
					case "C":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 0, 5.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();
			}

			#endregion


		}

	}

}