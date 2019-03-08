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

		public class QualityEvalUnit5010 : QualityEvalUnit
		{

			public override long TargetId
				 => 5010;


			protected override void AnalysisResult(FormCollection fc, EvalQualityResult result, Dictionary<string, EvalQualityResultItem> items)
			{
				//Analysis_ZisFaz_DusHuod(result, items, fc[EvalQualityRuleKeys.ZisFaz_DusHuod], fc[EvalQualityRuleKeys.ZisFaz_DusHuod_Def]);

				//var score1 = Analysis_ZisFaz_KaizShik(result, items, fc[EvalQualityRuleKeys.ZisFaz_KaizShik], fc[EvalQualityRuleKeys.ZisFaz_KaizShik_Def]);
				//var score2 = Analysis_ZisFaz_DanrPingwGongz(result, items, fc[EvalQualityRuleKeys.ZisFaz_DanrPingwGongz], fc[EvalQualityRuleKeys.ZisFaz_DanrPingwGongz_Def]);
				//var score3 = Analysis_ZisFaz_PingbHuoj(result, items, fc[EvalQualityRuleKeys.ZisFaz_PingbHuoj], fc[EvalQualityRuleKeys.ZisFaz_PingbHuoj_Def]);
				//result.Score += EvalHelper.GetScore(30, score1, score2, score3);

				//var score4 = Analysis_ZisFaz_FabLunw(result, items, fc[EvalQualityRuleKeys.ZisFaz_FabLunw], fc[EvalQualityRuleKeys.ZisFaz_FabLunw_Def]);
				//var score5 = Analysis_ZisFaz_LixKet(result, items, fc[EvalQualityRuleKeys.ZisFaz_LixKet], fc[EvalQualityRuleKeys.ZisFaz_LixKet_Def]);
				//var score6 = Analysis_ZisFaz_XiangmYanj(result, items, fc[EvalQualityRuleKeys.ZisFaz_XiangmYanj], fc[EvalQualityRuleKeys.ZisFaz_XiangmYanj_Def]);
				//result.Score += EvalHelper.GetScore(24, score4, score5, score6);

    //            //自身发展模块总得分
    //            result.DynamicScore1 = result.Score;
    //            result.Score = 0;

    //            var score7 = Analysis_PeixKec_KaisJiaosPeixKec(result, items, fc[EvalQualityRuleKeys.PeixKec_KaisJiaosPeixKec], fc[EvalQualityRuleKeys.PeixKec_KaisJiaosPeixKec_Def]);
				//var score8 = Analysis_PeixKec_JiangzBaog(result, items, fc[EvalQualityRuleKeys.PeixKec_JiangzBaog], fc[EvalQualityRuleKeys.PeixKec_JiangzBaog_Def]);
				//var score9 = Analysis_PeixKec_KecZiyKaif(result, items, fc[EvalQualityRuleKeys.PeixKec_KecZiyKaif], fc[EvalQualityRuleKeys.PeixKec_KecZiyKaif_Def]);
				//result.Score += EvalHelper.GetScore(30, score7, score8, score9);

    //            //培训课程模块得分
    //            result.DynamicScore2 = result.Score;
    //            result.Score = 0;

    //            //TODO：这里先暂时放一下，以后改成5001-5005类似的格式
    //            var score10 = Analysis_DaijJiaos_XueyShul(result, items, fc[EvalQualityRuleKeys.DaijJiaos_XueyShul], fc[EvalQualityRuleKeys.DaijJiaos_XueyShul_Def]);
				//var score11 = Analysis_DaijJiaos_DaijJih(result, items, fc[EvalQualityRuleKeys.DaijJiaos_DaijJih], fc[EvalQualityRuleKeys.DaijJiaos_DaijJih_Def]);
				//var score12 = Analysis_DaijJiaos_XueyChengzFenx(result, items, fc[EvalQualityRuleKeys.DaijJiaos_XueyChengzFenx], fc[EvalQualityRuleKeys.DaijJiaos_XueyChengzFenx_Def]);
				//var score13 = Analysis_DaijJiaos_DaijZhid(result, items, fc[EvalQualityRuleKeys.DaijJiaos_DaijZhid], fc[EvalQualityRuleKeys.DaijJiaos_DaijZhid_Def]);
				//result.Score += new double[] { score10, score11, score12, score13 }.Sum();

				//var score14 = Analysis_DaijJiaos_KaizShik(result, items, fc[EvalQualityRuleKeys.DaijJiaos_KaizShik], fc[EvalQualityRuleKeys.DaijJiaos_KaizShik_Def]);
				//var score15 = Analysis_DaijJiaos_FablunwHuocYukTiyJiu(result, items, fc[EvalQualityRuleKeys.DaijJiaos_FablunwHuocYukTiyJiu], fc[EvalQualityRuleKeys.DaijJiaos_FablunwHuocYukTiyJiu_Def]);
				//var score16 = Analysis_DaijJiaos_JiaoyJiaoxPingb(result, items, fc[EvalQualityRuleKeys.DaijJiaos_JiaoyJiaoxPingb], fc[EvalQualityRuleKeys.DaijJiaos_JiaoyJiaoxPingb_Def]);
			 //  result.Score += EvalHelper.GetScore(3, score14, score15, score16);

    //            //带教教师模块得分
    //            result.DynamicScore3 = result.Score;

    //            //总得分
    //            result.Score = result.DynamicScore1 + result.DynamicScore2 + result.DynamicScore3;

    //            Analysis_Tes(result, items, fc[EvalQualityRuleKeys.Tes], fc[EvalQualityRuleKeys.Tes_Def]);
			}

		}

	}

}