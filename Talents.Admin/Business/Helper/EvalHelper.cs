using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Business.Helper
{

	public static class EvalHelper
	{

		public static double GetScore(double maxScore, params double[] scoreList)
		{
			var tempScore = 0.0;
			foreach (var score in scoreList)
			{
				tempScore += score;
			}

			return maxScore > tempScore ? tempScore : maxScore;
		}


		public static string GetScore(double maxScore, string trimKey, params string[] strList)
		{
			var tempScore = 0.0;
			foreach (var item in strList)
			{
				var str = string.IsNullOrEmpty(trimKey) ? item : item.Replace(trimKey, "");
				tempScore += str.Trim() == "" ? 0 : Convert.ToDouble(str.Trim());
			}

			tempScore = maxScore > tempScore ? tempScore : maxScore;

			return tempScore.ToString() + trimKey;
		}


		public static double EnsureScoreInRange(double score, double min, double max)
		=> score > max ? max :
			score < min ? min :
			score;


      public static IEnumerable<SelectListItem> GetEvalPeriodSelectList(string noneLabel = null)
      {
         if (noneLabel != null)
            yield return new SelectListItem() { Value = "", Text = noneLabel };

         var ep = APDBDef.EvalPeriod;

         var db = new APDBDef();
         var periods = db.EvalPeriodDal
            .ConditionQuery(null, ep.PeriodId.Desc, null, null);

         foreach (var item in periods)
         {
            yield return new SelectListItem()
            {
               Value = item.PeriodId.ToString(),
               Text = item.Name
            };
         }
      }

   }

}