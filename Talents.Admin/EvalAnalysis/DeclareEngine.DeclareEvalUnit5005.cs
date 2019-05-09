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

         public override double CompanyFullScore => 20;

         public override double ExpertFullScore => 70;

         public override double SpecialFullScore => 10;


         protected override void AnalysisResult(FormCollection fc, EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items)
         {
            double score = 0, score1 = 0, score2 = 0, score3 = 0, score4 = 0, score5 = 0, score6 = 0, score7 = 0;

            if (fc[EvalDeclareRuleKeys.XiaonLvz] != null)
               score = Analysis_JiaoyJiaox_XiaonLvz(result, items, fc[EvalDeclareRuleKeys.XiaonLvz], fc[EvalDeclareRuleKeys.XiaonLvz_Def]);

            if (fc[EvalDeclareRuleKeys.Shid] != null)
            {
               AnalysisShid(result, items, fc[EvalDeclareRuleKeys.Shid]);
               score = result.Score;
            }

            if (fc[EvalDeclareRuleKeys.JiaoyJiaox_Gongkk] != null)
               score += Analysis_JiaoyJiaox_Gongkk(result, items, fc[EvalDeclareRuleKeys.JiaoyJiaox_Gongkk], fc[EvalDeclareRuleKeys.JiaoyJiaox_Gongkk_Def]);

            if (fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Quj] != null)
               score1 = Analysis_JiaoyJiaox_Pb_Quj(result, items, fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Quj], fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Quj_Def]);

            if (fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Qt] != null)
               score2 = Analysis_JiaoyJiaox_Pb_Qit(result, items, fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Qt], fc[EvalDeclareRuleKeys.JiaoyJiaox_Pb_Qt_Def]);

            score += score1 > score2 ? score1 : score2;

            if (fc[EvalDeclareRuleKeys.JiaoyGongz_ZhongxXueMingt] != null)
               score3 += Analysis_JiaoyJiaox_ZhongxXueMingt(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_ZhongxXueMingt], fc[EvalDeclareRuleKeys.JiaoyGongz_ZhongxXueMingt_Def]);

            if (fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw] != null)
               score3 += Analysis_JiaoyJiaox_DanrPingw(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw], fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw_Def]);

            if (fc[EvalDeclareRuleKeys.JiaoyGongz_Dey] != null)
               score4 += Analysis_JiaoyJiaox_Dey(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_Dey], fc[EvalDeclareRuleKeys.JiaoyGongz_Dey_Def]);

            if (fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw2] != null)
               score4 += Analysis_JiaoyJiaox_DanrPingw2(result, items, fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw2], fc[EvalDeclareRuleKeys.JiaoyGongz_DanrPingw2_Def]);

            score += score3 > score4 ? score3 : score4;

            if (fc[EvalDeclareRuleKeys.JiaoyKey_XiangmYanj] != null)
               score += Analysis_JiaoyKey_XiangmYanj(result, items, fc[EvalDeclareRuleKeys.JiaoyKey_XiangmYanj], fc[EvalDeclareRuleKeys.JiaoyKey_XiangmYanj_Def]);

            if (fc[EvalDeclareRuleKeys.JiaoyKey_FabLunw] != null)
               score += Analysis_JiaoyKey_FabLunw(result, items, fc[EvalDeclareRuleKeys.JiaoyKey_FabLunw], fc[EvalDeclareRuleKeys.JiaoyKey_FabLunw_Def]);


            if (fc[EvalDeclareRuleKeys.JiaosPeix_PeixKec] != null)
               score += Analysis_JiaosPeix_PeixKec(result, items, fc[EvalDeclareRuleKeys.JiaosPeix_PeixKec], fc[EvalDeclareRuleKeys.JiaosPeix_PeixKec_Def]);

            if (fc[EvalDeclareRuleKeys.JiaosPeix_ZhuantJiangz] != null)
               score += Analysis_JiaosPeix_ZhuantJiangz(result, items, fc[EvalDeclareRuleKeys.JiaosPeix_ZhuantJiangz], fc[EvalDeclareRuleKeys.JiaosPeix_ZhuantJiangz_Def]);

            if (fc[EvalDeclareRuleKeys.GerTes_Zhuanz] != null)
               score5 += Analysis_GerTes_Zhuanz(result, items, fc[EvalDeclareRuleKeys.GerTes_Zhuanz], fc[EvalDeclareRuleKeys.GerTes_Zhuanz_Def]);

            if (fc[EvalDeclareRuleKeys.GerTes_QitShenf] != null)
               score6 = Analysis_GerTes_QitShenf(result, items, fc[EvalDeclareRuleKeys.GerTes_QitShenf], fc[EvalDeclareRuleKeys.GerTes_QitShenf_Def]);

            if (fc[EvalDeclareRuleKeys.GerTes_XueyChengz] != null)
               score7 += Analysis_GerTes_XueyChengz(result, items, fc[EvalDeclareRuleKeys.GerTes_XueyChengz], fc[EvalDeclareRuleKeys.GerTes_XueyChengz_Def]);

            score += new double[] { score5, score6, score7 }.Max();

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
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 18.0, 20.0);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 14.0, 17.9);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;

			}

			#endregion


			#region [ 师德 ]

			private void AnalysisShid(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalSchoolRuleKeys.Shid,
				};
				items.Add(item.EvalItemKey, item);

				switch (choose)
				{
					case "A":
						result.Comment = "合格"; //TODO 不想加字段了，用comment 替代
						item.ResultValue = "合格";
						break;
					case "B":
						result.Score = 0;
						result.Comment = "不合格"; //TODO 不想加字段了，用comment 替代
						item.ResultValue = "不合格";
						break;
				}
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
					case "A1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 13.0, 15.0);
						break;
					case "A2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 11.0, 12.9);
						break;
					case "A3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 6.0, 10.9);
						break;
					case "A4":
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
					case "A2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 6.9);
						break;
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


			#region [教育科研.项目研究]

			private double Analysis_JiaoyKey_XiangmYanj(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.JiaoyKey_XiangmYanj,
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
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.5, 4.5);
						break;
					case "C1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						break;

					case "A2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.5, 4.5);
						break;
					case "B2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						break;
					case "C2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 3.5, 3.5);
						break;

					case "A3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						break;
					case "B3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 3.5, 3.5);
						break;
					case "C3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 3.0, 3.0);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


			#region [教育科研.发表论文]

			private double Analysis_JiaoyKey_FabLunw(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.JiaoyKey_FabLunw,
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
					case "B2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 3.0, 3.0);
						break;
					case "A3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 2.0, 2.0);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


			#region [教师培训.培训课程]

			private double Analysis_JiaosPeix_PeixKec(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.JiaosPeix_PeixKec,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 10.0, 10.0);
						break;
					case "A2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 7.0, 7.0);
						break;
					case "A3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


			#region [教师培训.专题讲座]

			private double Analysis_JiaosPeix_ZhuantJiangz(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.JiaosPeix_ZhuantJiangz,
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
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 9.0);
						break;
					case "A2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 9.0);
						break;
					case "B2":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 8.0, 8.0);
						break;
					case "A3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 7.0, 7.0);
						break;
					case "B3":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 6.0, 6.0);
						break;
					case "A4":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 5.0, 5.0);
						break;
					case "B4":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.0, 4.0);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


			#region [个人特色.专著]

			private double Analysis_GerTes_Zhuanz(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.GerTes_Zhuanz,
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
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


			#region [个人特色.其他身份]

			private double Analysis_GerTes_QitShenf(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.GerTes_QitShenf,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 9.0, 9.0);
						break;
					case "B1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 7.0, 7.0);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


			#region [个人特色.学员成长]

			private double Analysis_GerTes_XueyChengz(EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items, string choose, string score)
			{
				var item = new EvalDeclareResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalDeclareRuleKeys.GerTes_XueyChengz,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "A1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 7.0, 7.0);
						break;
					case "B1":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 7.0, 7.0);
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}

			#endregion


		}

	}

}