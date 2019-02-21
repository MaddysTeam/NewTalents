﻿using Business;
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

		public class QualityEvalUnit5002 : QualityEvalUnit
		{

			public override long TargetId
				=> 5002;


			protected override void AnalysisResult(FormCollection fc, EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items)
			{
				Analysis_ZisFaz_DusHuod(result, items, fc[EvalQualityRuleKeys.ZisFaz_DusHuod], fc[EvalQualityRuleKeys.ZisFaz_DusHuod_Def]);
                
                var score1 = Analysis_ZisFaz_KaizShik(result, items, fc[EvalQualityRuleKeys.ZisFaz_KaizShik], fc[EvalQualityRuleKeys.ZisFaz_KaizShik_Def]);
				var score2 = Analysis_ZisFaz_DanrPingwGongz(result, items, fc[EvalQualityRuleKeys.ZisFaz_DanrPingwGongz], fc[EvalQualityRuleKeys.ZisFaz_DanrPingwGongz_Def]);
				var score3 = Analysis_ZisFaz_PingbHuoj(result, items, fc[EvalQualityRuleKeys.ZisFaz_PingbHuoj], fc[EvalQualityRuleKeys.ZisFaz_PingbHuoj_Def]);
				result.Score += EvalHelper.GetScore(8, score1, score2, score3);

				var score4 = Analysis_ZisFaz_FabLunw(result, items, fc[EvalQualityRuleKeys.ZisFaz_FabLunw], fc[EvalQualityRuleKeys.ZisFaz_FabLunw_Def]);
				var score5 = Analysis_ZisFaz_LixKet(result, items, fc[EvalQualityRuleKeys.ZisFaz_LixKet], fc[EvalQualityRuleKeys.ZisFaz_LixKet_Def]);
				var score6 = Analysis_ZisFaz_XiangmYanj(result, items, fc[EvalQualityRuleKeys.ZisFaz_XiangmYanj], fc[EvalQualityRuleKeys.ZisFaz_XiangmYanj_Def]);
				result.Score += EvalHelper.GetScore(8, score4, score5, score6);

                //自身发展模块得分
                result.DynamicScore1 = result.Score;
                result.Score = 0;

                var score7 = Analysis_PeixKec_KaisJiaosPeixKec(result, items, fc[EvalQualityRuleKeys.PeixKec_KaisJiaosPeixKec], fc[EvalQualityRuleKeys.PeixKec_KaisJiaosPeixKec_Def]);
				var score8 = Analysis_PeixKec_JiangzBaog(result, items, fc[EvalQualityRuleKeys.PeixKec_JiangzBaog], fc[EvalQualityRuleKeys.PeixKec_JiangzBaog_Def]);
				var score9 = Analysis_PeixKec_KecZiyKaif(result, items, fc[EvalQualityRuleKeys.PeixKec_KecZiyKaif], fc[EvalQualityRuleKeys.PeixKec_KecZiyKaif_Def]);
				result.Score += EvalHelper.GetScore(40, score7, score8, score9);

                //培训课程模块得分
                result.DynamicScore2 = result.Score;
                result.Score = 0;

                Analysis_DaijJiaos_XueyShul(result, items, fc[EvalQualityRuleKeys.DaijJiaos_XueyShul], fc[EvalQualityRuleKeys.DaijJiaos_XueyShul_Def]);
				Analysis_DaijJiaos_DaijJih(result, items, fc[EvalQualityRuleKeys.DaijJiaos_DaijJih], fc[EvalQualityRuleKeys.DaijJiaos_DaijJih_Def]);
				Analysis_DaijJiaos_XueyChengzFenx(result, items, fc[EvalQualityRuleKeys.DaijJiaos_XueyChengzFenx], fc[EvalQualityRuleKeys.DaijJiaos_XueyChengzFenx_Def]);
				Analysis_DaijJiaos_DaijZhid(result, items, fc[EvalQualityRuleKeys.DaijJiaos_DaijZhid], fc[EvalQualityRuleKeys.DaijJiaos_DaijZhid_Def]);
				var score10 = Analysis_DaijJiaos_KaizShik(result, items, fc[EvalQualityRuleKeys.DaijJiaos_KaizShik], fc[EvalQualityRuleKeys.DaijJiaos_KaizShik_Def]);
				var score11 = Analysis_DaijJiaos_FablunwHuocYukTiyJiu(result, items, fc[EvalQualityRuleKeys.DaijJiaos_FablunwHuocYukTiyJiu], fc[EvalQualityRuleKeys.DaijJiaos_FablunwHuocYukTiyJiu_Def]);
				var score12 = Analysis_DaijJiaos_JiaoyJiaoxPingb(result, items, fc[EvalQualityRuleKeys.DaijJiaos_JiaoyJiaoxPingb], fc[EvalQualityRuleKeys.DaijJiaos_JiaoyJiaoxPingb_Def]);
				result.Score += EvalHelper.GetScore(15, score10, score11, score12);

                //带教教师模块得分
                result.DynamicScore3 = result.Score;

                //总得分
                result.Score = result.DynamicScore1 + result.DynamicScore2 + result.DynamicScore3;

                Analysis_Tes(result, items, fc[EvalQualityRuleKeys.Tes], fc[EvalQualityRuleKeys.Tes_Def]);
			}


			#region [ 自身发展分析 ]


			private void Analysis_ZisFaz_DusHuod(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.ZisFaz_DusHuod,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
						result.Score += scoreValue.EnsureInRange(3.6, 4);
						break;
					case "1B":
						result.Score += scoreValue.EnsureInRange(3.2, 3.6);
						break;
					case "1C":
						result.Score += scoreValue.EnsureInRange(2.8, 3.2);
						break;
					case "1D":
						result.Score += scoreValue.EnsureInRange(2.4, 2.8);
						break;
					case "2A":
						result.Score += scoreValue.EnsureInRange(2.4, 2.8);
						break;
					case "2B":
						result.Score += scoreValue.EnsureInRange(1.2, 1.6);
						break;
					case "2C":
						result.Score += 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();
			}


			private double Analysis_ZisFaz_KaizShik(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.ZisFaz_KaizShik,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
                        scoreValue = scoreValue.EnsureInRange(7.2, 8.0);
						break;
					case "1B":
                        scoreValue = scoreValue.EnsureInRange(6.4, 7.2);
						break;
					case "1C":
                        scoreValue = scoreValue.EnsureInRange(5.6, 6.4);
						break;
					case "1D":
                        scoreValue = scoreValue.EnsureInRange(4.8, 5.6);
						break;
					case "2A":
                        scoreValue = scoreValue.EnsureInRange(2.4, 3.2);
						break;
					case "2B":
                        scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			private double Analysis_ZisFaz_DanrPingwGongz(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.ZisFaz_DanrPingwGongz,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
                        scoreValue = scoreValue.EnsureInRange(7.2, 8.0);
						break;
					case "1B":
                        scoreValue = scoreValue.EnsureInRange(6.4, 7.2);
						break;
					case "1C":
                        scoreValue = scoreValue.EnsureInRange(5.6, 6.4);
						break;
					case "1D":
                        scoreValue = scoreValue.EnsureInRange(4.8, 5.6);
						break;
					case "2A":
                        scoreValue = scoreValue.EnsureInRange(2.4, 3.2);
						break;
					case "2B":
                        scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			private double Analysis_ZisFaz_PingbHuoj(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.ZisFaz_PingbHuoj,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
                        scoreValue = scoreValue.EnsureInRange(7.2, 8.0);
						break;
					case "1B":
                        scoreValue = scoreValue.EnsureInRange(6.4, 7.2);
						break;
					case "1C":
                        scoreValue = scoreValue.EnsureInRange(5.6, 6.4);
						break;
					case "1D":
                        scoreValue = scoreValue.EnsureInRange(4.8, 5.6);
						break;
					case "2A":
                        scoreValue = scoreValue.EnsureInRange(2.4, 3.2);
						break;
					case "2B":
                        scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			private double Analysis_ZisFaz_FabLunw(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.ZisFaz_FabLunw,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
                        scoreValue = scoreValue.EnsureInRange(7.2, 8.0);
						break;
					case "1B":
                        scoreValue = scoreValue.EnsureInRange(6.4, 7.2);
						break;
					case "1C":
                        scoreValue = scoreValue.EnsureInRange(5.6, 6.4);
						break;
					case "1D":
                        scoreValue = scoreValue.EnsureInRange(4.8, 5.6);
						break;
					case "2A":
                        scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			private double Analysis_ZisFaz_LixKet(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.ZisFaz_LixKet,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
                        scoreValue = scoreValue.EnsureInRange(7.2, 8.0);
						break;
					case "1B":
                        scoreValue = scoreValue.EnsureInRange(6.4, 7.2);
						break;
					case "1C":
                        scoreValue = scoreValue.EnsureInRange(5.6, 6.4);
						break;
					case "1D":
                        scoreValue = scoreValue.EnsureInRange(4.8, 5.6);
						break;
					case "2A":
                        scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			private double Analysis_ZisFaz_XiangmYanj(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.ZisFaz_XiangmYanj,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
                        scoreValue = scoreValue.EnsureInRange(7.2, 8.0);
						break;
					case "1B":
                        scoreValue = scoreValue.EnsureInRange(6.4, 7.2);
						break;
					case "1C":
                        scoreValue = scoreValue.EnsureInRange(5.6, 6.4);
						break;
					case "1D":
                        scoreValue = scoreValue.EnsureInRange(4.8, 5.6);
						break;
					case "2A":
                        scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			#endregion


			#region [ 培训课程分析 ]

			private double Analysis_PeixKec_KaisJiaosPeixKec(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.PeixKec_KaisJiaosPeixKec,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
                        scoreValue = scoreValue.EnsureInRange(36.0, 40);
						break;
					case "1B":
                        scoreValue = scoreValue.EnsureInRange(32.0, 36.0);
						break;
					case "1C":
                        scoreValue = scoreValue.EnsureInRange(28.0, 32.0);
						break;
					case "1D":
                        scoreValue = scoreValue.EnsureInRange(24.0, 28.0);
						break;
					case "2A":
                        scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			private double Analysis_PeixKec_JiangzBaog(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.PeixKec_JiangzBaog,
				};
				items.Add(item.EvalItemKey, item);


				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
                        scoreValue = scoreValue.EnsureInRange(36.0, 40);
						break;
					case "1B":
                        scoreValue = scoreValue.EnsureInRange(32.0, 36.0);
						break;
					case "1C":
                        scoreValue = scoreValue.EnsureInRange(28.0, 32.0);
						break;
					case "1D":
                        scoreValue = scoreValue.EnsureInRange(24.0, 28.0);
						break;
					case "2A":
                        scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			private double Analysis_PeixKec_KecZiyKaif(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.PeixKec_KecZiyKaif,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
                        scoreValue = scoreValue.EnsureInRange(36.0, 40);
						break;
					case "1B":
                        scoreValue = scoreValue.EnsureInRange(32.0, 36.0);
						break;
					case "1C":
                        scoreValue = scoreValue.EnsureInRange(28.0, 32.0);
						break;
					case "1D":
                        scoreValue = scoreValue.EnsureInRange(24.0, 28.0);
						break;
					case "2A":
                        scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			#endregion


			#region [ 带教教师分析 ]


			private void Analysis_DaijJiaos_XueyShul(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.DaijJiaos_XueyShul,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
						result.Score += scoreValue.EnsureInRange(0.9, 1.0);
						break;
					case "1B":
						result.Score += scoreValue.EnsureInRange(0.8, 0.9);
						break;
					case "2A":
						result.Score += scoreValue.EnsureInRange(0.3, 0.4);
						break;
					case "2B":
						result.Score += scoreValue.EnsureInRange(0.1, 0.2);
						break;
					case "2C":
						result.Score += 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_DaijJiaos_DaijJih(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.DaijJiaos_DaijJih,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				switch (choose)
				{
					case "1A":
						scoreValue = scoreValue.EnsureInRange(3.6, 4.0);
						break;
					case "1B":
						scoreValue = scoreValue.EnsureInRange(3.2, 3.6);
						break;
					case "1C":
						scoreValue = scoreValue.EnsureInRange(2.8, 3.2);
						break;
					case "1D":
						scoreValue = scoreValue.EnsureInRange(2.4, 2.8);
						break;
					case "2A":
						scoreValue = scoreValue.EnsureInRange(1.2, 1.6);
						break;
					case "2B":
						scoreValue = 0;
						break;
				}

				result.Score += scoreValue;
				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_DaijJiaos_XueyChengzFenx(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.DaijJiaos_XueyChengzFenx,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);

				switch (choose)
				{
					case "1A":
						scoreValue = scoreValue.EnsureInRange(3.6, 4.0);
						break;
					case "1B":
						scoreValue = scoreValue.EnsureInRange(3.2, 3.6);
						break;
					case "1C":
						scoreValue = scoreValue.EnsureInRange(2.8, 3.2);
						break;
					case "1D":
						scoreValue = scoreValue.EnsureInRange(2.4, 2.8);
						break;
					case "2A":
						scoreValue = scoreValue.EnsureInRange(1.2, 1.6);
						break;
					case "2B":
						scoreValue = scoreValue.EnsureInRange(0.4, 0.8);
						break;
					case "2C":
						scoreValue = 0;
						break;
				}

				result.Score += scoreValue;
				item.ResultValue = scoreValue.ToString();
			}


			private void Analysis_DaijJiaos_DaijZhid(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose,string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.DaijJiaos_DaijZhid,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);

				switch (choose)
				{
					case "1A":
						scoreValue = scoreValue.EnsureInRange(14.4,16.0);
						break;
					case "1B":
						scoreValue = scoreValue.EnsureInRange(12.8,14.4);
						break;
					case "1C":
						scoreValue = scoreValue.EnsureInRange(11.2,12.8);
						break;
					case "1D":
						scoreValue = scoreValue.EnsureInRange(9.6,11.2);
						break;
					case "2A":
						scoreValue = scoreValue.EnsureInRange(4.8,6.4);
						break;
					case "2B":
						scoreValue = scoreValue.EnsureInRange(1.6,3.2);
						break;
					case "2C":
						scoreValue = 0;
						break;
				}

				result.Score += scoreValue;

				item.ResultValue = scoreValue.ToString();

			}


			private double Analysis_DaijJiaos_KaizShik(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose,string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.DaijJiaos_KaizShik,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);

				switch (choose)
				{
					case "1A":
						scoreValue = scoreValue.EnsureInRange(6.7,7.5);
						break;
					case "1B":
						scoreValue = scoreValue.EnsureInRange(6.0,6.7);
						break;
					case "1C":
						scoreValue = scoreValue.EnsureInRange(5.3,6.0);
						break;
					case "1D":
						scoreValue = scoreValue.EnsureInRange(4.5,5.3);
						break;
					case "2A":
						scoreValue = scoreValue.EnsureInRange(2.2,3.0);
						break;
					case "2B":
						scoreValue = scoreValue.EnsureInRange(0.7,1.5);
						break;
					case "2C":
						scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			private double Analysis_DaijJiaos_FablunwHuocYukTiyJiu(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose,string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.DaijJiaos_FablunwHuocYukTiyJiu,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);

				switch (choose)
				{
					case "1A":
						scoreValue = scoreValue.EnsureInRange(6.7, 7.5);
						break;
					case "1B":
						scoreValue = scoreValue.EnsureInRange(6.0, 6.7);
						break;
					case "1C":
						scoreValue = scoreValue.EnsureInRange(5.3, 6.0);
						break;
					case "1D":
						scoreValue = scoreValue.EnsureInRange(4.5, 5.3);
						break;
					case "2A":
						scoreValue = scoreValue.EnsureInRange(2.2, 3.0);
						break;
					case "2B":
						scoreValue = scoreValue.EnsureInRange(0.7, 1.5);
						break;
					case "2C":
						scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			private double Analysis_DaijJiaos_JiaoyJiaoxPingb(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose,string score)
			{
				var item = new EvalQualityResultItem
				{
					ChooseValue = choose,
					EvalItemKey = EvalQualityRuleKeys.DaijJiaos_JiaoyJiaoxPingb,
				};

				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? string.Empty : score.Trim();
				var scoreValue = Convert.ToDouble(score);

				switch (choose)
				{
					case "1A":
						scoreValue = scoreValue.EnsureInRange(6.7, 7.5);
						break;
					case "1B":
						scoreValue = scoreValue.EnsureInRange(6.0, 6.7);
						break;
					case "1C":
						scoreValue = scoreValue.EnsureInRange(5.3, 6.0);
						break;
					case "1D":
						scoreValue = scoreValue.EnsureInRange(4.5, 5.3);
						break;
					case "2A":
						scoreValue = scoreValue.EnsureInRange(2.2, 3.0);
						break;
					case "2B":
						scoreValue = scoreValue.EnsureInRange(0.7, 1.5);
						break;
					case "2C":
						scoreValue = 0;
						break;
				}

				item.ResultValue = scoreValue.ToString();

				return scoreValue;
			}


			#endregion


			#region [ 特色 ]


			private void Analysis_Tes(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose,string score)
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
					case "1A":
                        scoreValue = EvalHelper.EnsureScoreInRange(scoreValue, 4,5);
                        break;
					case "1B":
                        scoreValue = EvalHelper.EnsureScoreInRange(scoreValue, 4,5);
                        break;
					case "1C":
                        scoreValue = EvalHelper.EnsureScoreInRange(scoreValue, 2,3);
                        break;
					case "1D":
                        scoreValue = EvalHelper.EnsureScoreInRange(scoreValue, 4,5);
                        break;
					case "1E":
                        scoreValue = EvalHelper.EnsureScoreInRange(scoreValue, 4.8,5);
                        break;
					case "2A":
                        scoreValue = EvalHelper.EnsureScoreInRange(scoreValue, 0.5,0.75);
                        break;
					case "2B":
                        scoreValue = 0;
                        break;
				}

                item.ResultValue = scoreValue.ToString();

                result.Characteristic = scoreValue;
            }


            #endregion

        }

	}

}