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

		public abstract class VolumnEvalUnit : VolumnEvalUnitBase
		{
			static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
			static APDBDef.EvalVolumnResultTableDef er = APDBDef.EvalVolumnResult;
			static APDBDef.EvalVolumnResultItemTableDef eri = APDBDef.EvalVolumnResultItem;

			public override double FullScroe
				=> 100;

			public override double Proportion
			 => 0.2;

			public static double ProportionValue => 0.2;



			public override string RuleView
				=> ViewPath + "/VolumnRuleView" + TargetId;


			public override string EvalView
			{
				get
				{
					throw new NotSupportedException();
				}
			}


			public override string ResultView
				=> ViewPath + "/VolumnResultView" + TargetId;


			public override EvalVolumnResult GetResult(APDBDef db, VolumnEvalParam param)
			{
				return APQuery.select(er.Asterisk, u.RealName)
					.from(er, u.JoinInner(er.Accesser == u.UserId))
					.where(er.PeriodId == param.PeriodId & er.TeacherId == param.TeacherId)
					.query(db, r =>
					{
						EvalVolumnResult data = new EvalVolumnResult();
						er.Fullup(r, data, false);
						data.AccesserName = u.RealName.GetValue(r);
						return data;
					}).FirstOrDefault();
			}


			public override Dictionary<string, EvalVolumnResultItem> GetResultItem(APDBDef db, VolumnEvalParam param)
				=> APQuery.select(eri.EvalItemKey, eri.ChooseValue, eri.ResultValue)
					.from(er, eri.JoinInner(er.ResultId == eri.ResultId))
					.where(er.PeriodId == param.PeriodId & er.TeacherId == param.TeacherId)
					.query(db, r =>
					{
						return new EvalVolumnResultItem
						{
							EvalItemKey = eri.EvalItemKey.GetValue(r),
							ChooseValue = eri.ChooseValue.GetValue(r),
							ResultValue = eri.ResultValue.GetValue(r)
						};
					}).ToDictionary(m => m.EvalItemKey);

			public override Dictionary<string, string> ChooseEvalResultItems(Dictionary<string, EvalVolumnResultItem> items)
			{
				throw new NotImplementedException();
			}

		}

	}

}