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

		public  class DeclareEvalUnit5006 : DeclareEvalUnit
		{

			public override long TargetId => 5006;

         public override double CompanyFullScore => 30;

         protected override void AnalysisResult(FormCollection fc, EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items)
			{
				double score = 0, score1 = 0, score2 = 0, score3 = 0, score4 = 0;

				if (fc[EvalDeclareRuleKeys.XiaonLvz] != null)
					score = Analysis_JiaoyJiaox_XiaonLvz(result, items, fc[EvalDeclareRuleKeys.XiaonLvz], fc[EvalDeclareRuleKeys.XiaonLvz_Def]);

				//if (fc[EvalDeclareRuleKeys.JiaoyJiaox_Gongkk] != null)
				//	score += Analysis_JiaoyJiaox_Gongkk(result, items, fc[EvalDeclareRuleKeys.JiaoyJiaox_Gongkk], fc[EvalDeclareRuleKeys.JiaoyJiaox_Gongkk_Def]);

				//if (fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Quj] != null)
				//	score1 = Analysis_JiaoyJiaox_Pb_Quj(result, items, fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Quj], fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Quj_Def]);

				//if (fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Qt] != null)
				//	score2 = Analysis_JiaoyJiaox_Pb_Qit(result, items, fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Qt], fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Qt_Def]);

				//score += score1 > score2 ? score1 : score2;

				//if (fc[EvalDeclareRuleKeys.JiaoyGongz_ZhongxXueMingt] != null)
				//	score3 = Analysis_JiaoyJiaox_ZhongxXueMingt(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_ZhongxXueMingt], fc[EvalDeclareRuleKeys.JiaoyGongz_ZhongxXueMingt_Def]);

				//if (fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw] != null)
				//	score3 += Analysis_JiaoyJiaox_DanrPingw(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw], fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw_Def]);

				//if (fc[EvalDeclareRuleKeys.JiaoyGongz_Dey] != null)
				//	score4 = Analysis_JiaoyJiaox_Dey(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_Dey], fc[EvalDeclareRuleKeys.JiaoyGongz_Dey_Def]);

				//if (fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw2] != null)
				//	score4 += Analysis_JiaoyJiaox_DanrPingw2(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw2], fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw2_Def]);

				//score += score3 > score4 ? score3 : score4;

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
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 27.0, 30.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 21.0, 26.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;

			}

			#endregion

		}

	}

}