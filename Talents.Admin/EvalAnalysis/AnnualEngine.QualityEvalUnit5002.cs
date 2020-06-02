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

      public class QualityEvalUnit5002 : QualityEvalUnit
      {

         public override long TargetId
            => 5002;

         public override string EvalView => ViewPath + "/QualityEvalView5002";
         public override string ResultView => ViewPath + "/QualityResultView5002";

         double score = 0, score1 = 0, score2 = 0, score3 = 0, score4 = 0, score5 = 0, score6 = 0, score7 = 0;

         protected override void AnalysisResult(FormCollection fc, EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items)
         {
            score1 = Analysis_KetJiaox_Gongkk(result, items, fc[EvalQualityRuleKeys.KetJiaox_Gongkk], fc[EvalQualityRuleKeys.KetJiaox_Gongkk_Def]);
            score2 = Analysis_KetJiaox_Zhidk(result, items, fc[EvalQualityRuleKeys.KetJiaox_Zhidk], fc[EvalQualityRuleKeys.KetJiaox_Zhidk_Def]);
            score3 = Analysis_KaisJiangz(result, items, fc[EvalQualityRuleKeys.KaisJiangz], fc[EvalQualityRuleKeys.KaisJiangz_Def]);
            score4 = Analysis_Lunw(result, items, fc[EvalQualityRuleKeys.Lunw], fc[EvalQualityRuleKeys.Lunw_Def]);

            score = new double[] { score1, score2, score3, score4 }.Max();

            score5 = Analysis_XiangmYanj(result, items, fc[EvalQualityRuleKeys.XiangmYanj], fc[EvalQualityRuleKeys.XiangmYanj_Def]);
            score6 = Analysis_ShijPeixKec(result, items, fc[EvalQualityRuleKeys.ShijPeixKec], fc[EvalQualityRuleKeys.ShijPeixKec_Def]);
            score7 = Analysis_Tes(result, items, fc[EvalQualityRuleKeys.Tes], fc[EvalQualityRuleKeys.Tes_Def]);

            score += new double[] { score5, score6, score7 }.Sum();

            result.Score = score;
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


         private double Analysis_KaisJiangz(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
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


         #endregion


         #region [ 教育科研 ]


         private double Analysis_XiangmYanj(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
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

            return scoreValue;
         }


         private double Analysis_Lunw(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
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
                  result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 4.5, 5.0);
                  break;
               case "B":
                  result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 3.0, 4.4);
                  break;
               case "C":
                  result.Score += EvalHelper.EnsureScoreInRange(scoreValue, 0, 2.9);
                  break;
            }

            item.ResultValue = scoreValue.ToString();

            return scoreValue;
         }


         #endregion


         #region [ 市级培训课程 ]

         private double Analysis_ShijPeixKec(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
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

            return scoreValue;
         }

         #endregion


         #region [ 特色 ]

         private double Analysis_Tes(EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items, string choose, string score)
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

         #endregion

      }

   }

}