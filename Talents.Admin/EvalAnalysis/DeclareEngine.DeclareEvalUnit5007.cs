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

		public class DeclareEvalUnit5007 : DeclareEvalUnit
		{

			public override long TargetId => 5007;

			public override double FullScroe => 60;

			public override double CompanyFullScore => 60;

			public override double ExpertFullScore => 0;

			public override double SpecialFullScore => 0;

			protected override void AnalysisResult(FormCollection fc, EvalDeclareResult result, Dictionary<string, EvalDeclareResultItem> items)
			{
				double score = 0;

				if (fc[EvalDeclareRuleKeys.XiaonLvz] != null)
					score = Analysis_JiaoyJiaox_XiaonLvz(result, items, fc[EvalDeclareRuleKeys.XiaonLvz], fc[EvalDeclareRuleKeys.XiaonLvz_Def]);

				if (fc[EvalDeclareRuleKeys.Shid] != null)
				{
					AnalysisShid(result, items, fc[EvalDeclareRuleKeys.Shid]);
					score = result.Score;
				}

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
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 54, 60);
						break;
					case "B":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 45, 53.9);
						break;
					case "C":
						result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 36, 44.9);
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

		}

	}

}