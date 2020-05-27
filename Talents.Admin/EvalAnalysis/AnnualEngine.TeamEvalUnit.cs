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

		public class TeamEvalUnit : TeamEvalUnitBase
		{
			static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
			static APDBDef.TeamEvalResultTableDef er = APDBDef.TeamEvalResult;
			static APDBDef.TeamEvalResultItemTableDef eri = APDBDef.TeamEvalResultItem;


			public override double FullScroe
			=> 100;

			public override double Proportion
			=> 1;

			// this field for eval 2020 only to distinguish which target id is special , then speical eval target is team leader and eval by specal expert account
			public override long TargetId => 0;

			public static double ProportionValue => 0.2;


			public override TeamEvalResult GetResult(APDBDef db, TeamEvalParam param)
			{
				return APQuery.select(er.Asterisk, u.RealName)
					 .from(er, u.JoinInner(er.Accesser == u.UserId))
					 .where(er.PeriodId == param.PeriodId & er.MemberId == param.TeacherId)
					 .query(db, r =>
					 {
						 TeamEvalResult data = new TeamEvalResult();
						 er.Fullup(r, data, false);
						 // data.AccesserName = u.RealName.GetValue(r);
						 return data;
					 }).FirstOrDefault();
			}


			public override Dictionary<string, TeamEvalResultItem> GetResultItem(APDBDef db, TeamEvalParam param)
				 =>
				APQuery.select(eri.EvalItemKey, eri.ChooseValue, eri.ResultValue)
					  .from(er, eri.JoinInner(er.ResultId == eri.ResultId))
					  .where(er.PeriodId == param.PeriodId & er.MemberId == param.TeacherId)
					  .query(db, r =>
					  {
						  return new TeamEvalResultItem
						  {
							  EvalItemKey = eri.EvalItemKey.GetValue(r),
							  ChooseValue = eri.ChooseValue.GetValue(r),
							  ResultValue = eri.ResultValue.GetValue(r)
						  };
					  }).ToDictionary(m => m.EvalItemKey);


			public override long Eval(APDBDef db, TeamEvalParam param, FormCollection fc)
			{
				var eval = db.TeamEvalResultDal.ConditionQuery(er.PeriodId == param.PeriodId & er.MemberId == param.TeacherId
						  & er.TeamId == param.TeamId, null, null, null).FirstOrDefault();

				var result = new TeamEvalResult()
				{
					PeriodId = param.PeriodId,
					MemberId = param.TeacherId,
					Accesser = param.AccesserId,
					AccessDate = DateTime.Now,
					TeamId = param.TeamId,
					FullScore = FullScroe,
					Comment = fc["Comment"]
				};
				var items = new Dictionary<string, TeamEvalResultItem>();

				AnalysisResult(fc, result, items);

				if (eval != null)
				{
					db.TeamEvalResultDal.PrimaryDelete(eval.ResultId);
					db.TeamEvalResultItemDal.ConditionDelete(eri.ResultId == eval.ResultId);
				}

				db.TeamEvalResultDal.Insert(result);

				foreach (var item in items.Values)
				{
					item.ResultId = result.ResultId;
					db.TeamEvalResultItemDal.Insert(item);
				}

				return result.ResultId;
			}


			#region [ 分析 ]

			string _zeroScore = "0";

			protected virtual void AnalysisResult(FormCollection fc, TeamEvalResult result, Dictionary<string, TeamEvalResultItem> items)
			{
				AnalysisResult(result, items, null, fc[EvalQualityRuleKeys.TuandKaoh]);
			}

			private void AnalysisResult(TeamEvalResult result, Dictionary<string, TeamEvalResultItem> items, string choose, string score)
			{
				var item = new TeamEvalResultItem
				{
					EvalItemKey = EvalQualityRuleKeys.TuandKaoh,
				};
				items.Add(item.EvalItemKey, item);

				score = string.IsNullOrEmpty(score) || string.IsNullOrWhiteSpace(score) ? _zeroScore : score.Trim();
				var scoreValue = Convert.ToDouble(score);
				result.Score += scoreValue.EnsureInRange(0, 100);

				item.ResultValue = scoreValue.ToString();
			}

			#endregion

		}

	}

}