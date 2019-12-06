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

		public abstract class QualityEvalUnit : QualityEvalUnitBase
		{
			static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
			static APDBDef.EvalQualityResultTableDef er = APDBDef.EvalQualityResult;
			static APDBDef.EvalQualityResultItemTableDef eri = APDBDef.EvalQualityResultItem;
			static APDBDef.ExpGroupTableDef g = APDBDef.ExpGroup;

			// for 2016
			//public override double FullScroe
			//	=> 100;


			//public override double Proportion
			// => 0.5;

			// for 2019  由于时间关系，2019年有三张考核表，但从逻辑上还是认为一张表，总分300分，然后通过逻辑拆分
			public override double FullScroe
				=> 300;


			public override double Proportion
			 => 1;

			public static double ProportionValue => 0.5;


			public override string RuleView
				=> ViewPath + "/QualityRuleView" + TargetId;


			public override string EvalView
				=> ViewPath + "/QualityEvalView" + TargetId;


			public override string ResultView
				=> ViewPath + "/QualityResultView" + TargetId;


			public override string SubmitResultView
				=> ViewPath + "/QualityEvalSubmitView";


			public override EvalQualitySubmitResult GetSubmitResult(APDBDef db, QualityEvalParam param)
			{
				var t = APDBDef.EvalQualitySubmitResult;

				return APQuery.select(t.Asterisk, g.Name, u.RealName)
					.from(t,
						u.JoinInner(t.Accesser == u.UserId),
						g.JoinInner(t.GroupId == g.GroupId))
					.where(t.PeriodId == param.PeriodId & t.TeacherId == param.TeacherId)
					.query(db, r =>
					{
						EvalQualitySubmitResult data = new EvalQualitySubmitResult();
						t.Fullup(r, data, false);
						data.AccesserName = u.RealName.GetValue(r);
						data.GroupName = g.Name.GetValue(r);
						return data;
					}).FirstOrDefault();
			}


			public override List<EvalQualityResult> GetResults(APDBDef db, QualityEvalParam param)
			{
				var query = APQuery.select(er.Asterisk, u.RealName)
					.from(er, u.JoinInner(er.Accesser == u.UserId))
					.where(er.PeriodId == param.PeriodId & er.TeacherId == param.TeacherId)
					.query(db, r =>
					{
						EvalQualityResult data = new EvalQualityResult();
						er.Fullup(r, data, false);
						data.AccesserName = u.RealName.GetValue(r);
						return data;
					}).ToList();


				return query;
			}


			public override EvalQualityResult GetResult(APDBDef db, QualityEvalParam param)
			{
				var t = APDBDef.EvalQualityResult;

				return APQuery.select(t.Asterisk, u.RealName)
					.from(t, u.JoinInner(t.Accesser == u.UserId))
					.where(t.ResultId == param.ResultId)
					.query(db, r =>
					{
						EvalQualityResult data = new EvalQualityResult();
						t.Fullup(r, data, false);
						data.AccesserName = u.RealName.GetValue(r);
						return data;
					}).FirstOrDefault();
			}


			public override Dictionary<string, EvalQualityResultItem> GetResultItem(APDBDef db, QualityEvalParam param)
				=> APQuery.select(eri.EvalItemKey, eri.ChooseValue, eri.ResultValue)
					.from(eri)
					.where(eri.ResultId == param.ResultId)
					.query(db, r =>
					{
						return new EvalQualityResultItem
						{
							EvalItemKey = eri.EvalItemKey.GetValue(r),
							ChooseValue = eri.ChooseValue.GetValue(r),
							ResultValue = eri.ResultValue.GetValue(r)
						};
					}).ToDictionary(m => m.EvalItemKey);


			public override long Eval(APDBDef db, QualityEvalParam param, FormCollection fc)
			{
				var eval = db.EvalQualityResultDal.ConditionQuery(er.PeriodId == param.PeriodId & er.TeacherId == param.TeacherId
				& er.Accesser == param.AccesserId, null, null, null).FirstOrDefault();

				var result = new EvalQualityResult()
				{
					PeriodId = param.PeriodId,
					TeacherId = param.TeacherId,
					GroupId = param.GroupId,
					Accesser = param.AccesserId,
					AccessDate = DateTime.Now,
					DeclareTargetPKID = param.TargetId,
					FullScore = FullScroe,
					DynamicComment1 = fc["DynamicComment1"],
					DynamicComment2 = fc["DynamicComment2"],
					DynamicComment3 = fc["DynamicComment3"],
					Comment = fc["Comment"]
				};
				var items = new Dictionary<string, EvalQualityResultItem>();

				AnalysisResult(fc, result, items);


				if (eval != null)
				{
					db.EvalQualityResultDal.PrimaryDelete(eval.ResultId);
					db.EvalQualityResultItemDal.ConditionDelete(eri.ResultId == eval.ResultId);
				}

				db.EvalQualityResultDal.Insert(result);

				foreach (var item in items.Values)
				{
					item.ResultId = result.ResultId;
					db.EvalQualityResultItemDal.Insert(item);
				}

				return result.ResultId;
			}


			public override Dictionary<string, string> ChooseEvalResultItems(Dictionary<string, EvalQualityResultItem> items)
			{
				Dictionary<string, string> keys = new Dictionary<string, string>();

				keys.Add(EvalQualityRuleKeys.ZisFaz_DusHuod, "读书活动");
				keys.Add(EvalQualityRuleKeys.DaijJiaos_XueyShul, "学员数量");
				keys.Add(EvalQualityRuleKeys.DaijJiaos_DaijJih, "带教计划（方案）、小结");
				keys.Add(EvalQualityRuleKeys.DaijJiaos_XueyChengzFenx, "学员的成长分析");
				keys.Add(EvalQualityRuleKeys.DaijJiaos_DaijZhid, "带教指导");
				keys.Add(EvalQualityRuleKeys.Tes, "特色");

				Choose(1,
					items[EvalQualityRuleKeys.ZisFaz_KaizShik],
					items[EvalQualityRuleKeys.ZisFaz_DanrPingwGongz],
					items[EvalQualityRuleKeys.ZisFaz_PingbHuoj])
					.ToList()
					.ForEach(m => keys.Add(m.EvalItemKey, m.EvalItemKey));

				Choose(1,
					items[EvalQualityRuleKeys.ZisFaz_FabLunw],
					items[EvalQualityRuleKeys.ZisFaz_LixKet],
					items[EvalQualityRuleKeys.ZisFaz_XiangmYanj])
					.ToList()
					.ForEach(m => keys.Add(m.EvalItemKey, m.EvalItemKey));

				Choose(1,
					items[EvalQualityRuleKeys.PeixKec_JiangzBaog],
					items[EvalQualityRuleKeys.PeixKec_KaisJiaosPeixKec],
					items[EvalQualityRuleKeys.PeixKec_KecZiyKaif])
					.ToList()
					.ForEach(m => keys.Add(m.EvalItemKey, m.EvalItemKey));

				Choose(2,
					items[EvalQualityRuleKeys.DaijJiaos_KaizShik],
					items[EvalQualityRuleKeys.DaijJiaos_FablunwHuocYukTiyJiu],
					items[EvalQualityRuleKeys.DaijJiaos_JiaoyJiaoxPingb])
					.ToList()
					.ForEach(m => keys.Add(m.EvalItemKey, m.EvalItemKey));


				return keys;
			}


			protected virtual IEnumerable<EvalQualityResultItem> Choose(int takeCount, params EvalQualityResultItem[] items)
			  => items.OrderByDescending(item => Convert.ToDouble(item.ResultValue.Replace("分", string.Empty))).Take(takeCount);


			protected abstract void AnalysisResult(FormCollection fc, EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items);

		}

	}

}