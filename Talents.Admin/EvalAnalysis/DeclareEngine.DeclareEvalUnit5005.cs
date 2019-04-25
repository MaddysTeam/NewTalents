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

		public class DeclareEvalUnit5005 : DeclareEvalUnit
		{

			public override long TargetId => 5005;


			protected override void AnalysisResult(FormCollection fc, EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items)
			{
				double score = 0, score1 = 0, score2 = 0, score3 = 0, score4 = 0;

				if (fc[EvalDeclareRuleKeys.XiaonLvz] != null)
					score = Analysis_JiaoyJiaox_XiaonLvz(result, items, fc[EvalDeclareRuleKeys.XiaonLvz], fc[EvalDeclareRuleKeys.XiaonLvz_Def]);

				if (fc[EvalDeclareRuleKeys.JiaoyJiaox_Gongkk] != null)
					score += Analysis_JiaoyJiaox_Gongkk(result, items, fc[EvalDeclareRuleKeys.JiaoyJiaox_Gongkk], fc[EvalDeclareRuleKeys.JiaoyJiaox_Gongkk_Def]);

				if (fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Quj] != null)
					score1 = Analysis_JiaoyJiaox_Pb_Quj(result, items, fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Quj], fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Quj_Def]);

				if (fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Qt] != null)
					score2 = Analysis_JiaoyJiaox_Pb_Qit(result, items, fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Qt], fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Qt_Def]);

				score += score1 > score2 ? score1 : score2;

				if (fc[EvalDeclareRuleKeys.JiaoyGongz_ZhongxXueMingt] != null)
					score3 = Analysis_JiaoyJiaox_ZhongxXueMingt(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_ZhongxXueMingt], fc[EvalDeclareRuleKeys.JiaoyGongz_ZhongxXueMingt_Def]);

				if (fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw] != null)
					score3 += Analysis_JiaoyJiaox_DanrPingw(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw], fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw_Def]);

				if (fc[EvalDeclareRuleKeys.JiaoyGongz_Dey] != null)
					score4 = Analysis_JiaoyJiaox_Dey(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_Dey], fc[EvalDeclareRuleKeys.JiaoyGongz_Dey_Def]);

				if (fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw2] != null)
					score4 += Analysis_JiaoyJiaox_DanrPingw2(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw2], fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw2_Def]);

				score += score3 > score4 ? score3 : score4;

				result.Score = score;
			}


			#region [ 校内履职 ]

			private double Analysis_JiaoyJiaox_XiaonLvz(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.XiaonLvz,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 13.0, 15.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 11.0, 12.9);
						break;
					case "C":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 6.0, 10.9);
						break;
					case "D":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 3.0, 5.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;

			}

			#endregion


			#region [ 公开课 ]

			private double Analysis_JiaoyJiaox_Gongkk(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.JiaoyJiaox_Gongkk,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 13.0, 15.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 11.0, 12.9);
						break;
					case "C":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 6.0, 10.9);
						break;
					case "D":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 3.0, 5.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;

			}

			#endregion


			#region [ 教育教学评比.区级以上]

			private double Analysis_JiaoyJiaox_Pb_Quj(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.JiaoyJiaox_Pb_Quj,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 10.0, 10.0);
						break;
					case "B1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 9.9);
						break;
					case "C1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.5, 8.5);
						break;

					case "A2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.5, 9.5);
						break;
					case "B2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 9.0);
						break;
					case "C2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.5, 8.5);
						break;
					case "D2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.0, 8.0);
						break;

					case "A3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 9.0);
						break;
					case "B3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.5, 8.5);
						break;
					case "C3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.0, 8.0);
						break;
					case "D3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 7.0, 7.0);
						break;

				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


			#region [ 教育教学评比.其他]

			private double Analysis_JiaoyJiaox_Pb_Qit(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.JiaoyJiaox_Pb_Qt,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 10.0, 10.0);
						break;
					case "B1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 9.9);
						break;
					case "C1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.5, 8.5);
						break;

					case "A2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.5, 9.5);
						break;
					case "B2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 9.0);
						break;
					case "C2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.5, 8.5);
						break;
					case "D2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.0, 8.0);
						break;

					case "A3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 9.0);
						break;
					case "B3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.5, 8.5);
						break;
					case "C3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.0, 8.0);
						break;
					case "D3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 7.0, 7.0);
						break;

				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


			#region [教研工作.中小学命题]

			private double Analysis_JiaoyJiaox_ZhongxXueMingt(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.JiaoyGongz_ZhongxXueMingt,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 10.0, 10.0);
						break;
					case "B1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.0, 8.0);
						break;
					case "C1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 7.0, 7.0);
						break;
					case "A2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 7.0, 7.0);
						break;
					case "B2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


			#region [教研工作.评委相关工作1]

			private double Analysis_JiaoyJiaox_DanrPingw(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.JiaoyGongz_DanrPingw,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 5.0, 5.0);
						break;
					case "B1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						break;
					case "A2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


			#region [教研工作.德育]

			private double Analysis_JiaoyJiaox_Dey(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.JiaoyGongz_Dey,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 7.0, 10.0);
						break;
					case "B1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 6.9);
						break;
						//case "A2":
						//	result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 5.0, 5.0);
						//	break;
						//case "B2":
						//	result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						//	break;
						//case "C2":
						//	result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						//	break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


			#region [教研工作.评委相关工作2]

			private double Analysis_JiaoyJiaox_DanrPingw2(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.JiaoyGongz_DanrPingw2,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 5.0, 5.0);
						break;
					case "B1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						break;
					case "A2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion

		}

	}

}