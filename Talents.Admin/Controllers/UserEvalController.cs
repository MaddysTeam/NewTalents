using Business;
using Business.Config;
using Symber.Web.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheSite.EvalAnalysis;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class UserEvalController : BaseController
	{

		//	GET:	UserEval/Overview

		public ActionResult Overview(long userId)
		{
			var e = APDBDef.EvalPeriod;
			var es = APDBDef.EvalSchoolResult;
			var ev = APDBDef.EvalVolumnResult;
			var eq = APDBDef.EvalQualitySubmitResult;
            var currentPeriod = db.GetCurrentEvalPeriod();

			var list = APQuery.select(e.PeriodId, e.Name, es.Score.As("schoolScore"), es.FullScore.As("schoolFullScore"),
								ev.Score.As("volumnScore"), ev.FullScore.As("volumnFullScore"),
								eq.Score.As("qualityScore"), eq.FullScore.As("qualityFullScore"),
								eq.Characteristic.As("characteristicScore"))
				.from(e,
						es.JoinLeft(e.PeriodId == es.PeriodId & es.TeacherId == userId),
						ev.JoinLeft(e.PeriodId == ev.PeriodId & ev.TeacherId == userId),
						eq.JoinLeft(e.PeriodId == eq.PeriodId & eq.TeacherId == userId))
                .where(e.PeriodId != 5016) //TODO:  需要在检查下为什么会读取“考评周期”
                .group_by(e.PeriodId, e.Name, es.Score, es.FullScore, ev.Score, ev.FullScore, eq.Score, eq.FullScore, eq.Characteristic)
				.query(db, r =>
				{
					return new UserEvalViewModel()
					{
						PeriodId = e.PeriodId.GetValue(r),
						PeriodName = e.Name.GetValue(r),
						SchoolScore = ev.Score.GetValue(r, "schoolScore") * EvalAnalysis.AnnualEngine.SchoolEvalUnit.ProportionValue,
						SchoolFullScore = ev.Score.GetValue(r, "schoolFullScore") * EvalAnalysis.AnnualEngine.SchoolEvalUnit.ProportionValue,
						VolumnScore = ev.Score.GetValue(r, "volumnScore") * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue,
						VolumnFullScore = ev.Score.GetValue(r, "volumnFullScore") * EvalAnalysis.AnnualEngine.VolumnEvalUnit.ProportionValue,
						QualityScore = ev.Score.GetValue(r, "qualityScore") * EvalAnalysis.AnnualEngine.QualityEvalUnit.ProportionValue,
						QualityFullScore = ev.Score.GetValue(r, "qualityFullScore") * EvalAnalysis.AnnualEngine.QualityEvalUnit.ProportionValue,
						CharacteristicScore = ev.Score.GetValue(r, "characteristicScore") , // 特色分
                        FullScore = EngineManager.Engines[currentPeriod.AnalysisType].FullScore
                    };
				}).ToList();


            ViewBag.UserName = db.BzUserProfileDal.PrimaryGet(userId).RealName;


            return View(list);
		}

	}

}