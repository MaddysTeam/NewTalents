using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.EvalAnalysis
{

	public partial class AnnualEngine
	{

		public class VolumnEvalUnit5002 : VolumnEvalUnit
		{

			static APDBDef.DeclareContentTableDef dc = APDBDef.DeclareContent;
			static APDBDef.DeclareActiveTableDef da = APDBDef.DeclareActive;
			static APDBDef.DeclareAchievementTableDef dac = APDBDef.DeclareAchievement;
			static APDBDef.TeamActiveTableDef ta = APDBDef.TeamActive;
			static APDBDef.TeamMemberTableDef tm = APDBDef.TeamMember;
			static APDBDef.TeamContentTableDef tc = APDBDef.TeamContent;
			static APDBDef.EvalVolumnResultTableDef e = APDBDef.EvalVolumnResult;
			static APDBDef.EvalVolumnResultItemTableDef er = APDBDef.EvalVolumnResultItem;
			static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;


			public override long TargetId
				=> 5002;


			public override EvalVolumnResult AnalysisContent(APDBDef db, VolumnEvalParam param, long teacherId)
			{
				var result = new EvalVolumnResult
				{
					TeacherId = param.TeacherId,
					PeriodId = param.PeriodId,
					AccessDate = DateTime.Now,
					Accesser = param.AccesserId,
					FullScore = FullScroe,
					DeclareTargetPKID = TargetId
				};

				var items = new List<EvalVolumnResultItem>();

				//	items 在这里是无效的， 不想在修改分析的代码所以加上
				AnalysisResult(param.TeacherId, db, result, items);

				return result;
			}


			public override void Eval(APDBDef db, VolumnEvalParam param, params long[] teacherIds)
			{
				foreach (var item in teacherIds)
				{
					param.TeacherId = item;
					Eval(db, param);
				}
			}


			public void Eval(APDBDef db, VolumnEvalParam param)
			{
				var eval = db.EvalVolumnResultDal.ConditionQuery(e.PeriodId == param.PeriodId & 
					e.TeacherId == param.TeacherId, null, null, null).FirstOrDefault();


				var result = new EvalVolumnResult
				{
					TeacherId = param.TeacherId,
					PeriodId = param.PeriodId,
					AccessDate = DateTime.Now,
					Accesser = param.AccesserId,
					FullScore = FullScroe,
					DeclareTargetPKID = TargetId
				};

				var items = new List<EvalVolumnResultItem>();

				AnalysisResult(param.TeacherId, db, result, items);


				if (eval != null)
				{
					db.EvalVolumnResultDal.PrimaryDelete(eval.ResultId);
					db.EvalVolumnResultItemDal.ConditionDelete(er.ResultId == eval.ResultId);
				}

				db.EvalVolumnResultDal.Insert(result);

				foreach (var item in items)
				{
					item.ResultId = result.ResultId;
					db.EvalVolumnResultItemDal.Insert(item);
				}
			}


			

			#region [ 分析 ]


			private void AnalysisResult(long teacherId, APDBDef db, EvalVolumnResult result, List<EvalVolumnResultItem> items)
			{
				var t = APDBDef.EvalPeriod;
				var eval = db.EvalPeriodDal.ConditionQuery(t.IsCurrent == true, null, null, null).FirstOrDefault();

				if (eval == null)
				{
					return;
				}

				var dcList = db.DeclareContentDal.ConditionQuery(dc.TeacherId == teacherId, null, null, null);
				var daList = db.DeclareActiveDal.ConditionQuery(da.TeacherId == teacherId & da.CreateDate.Between(eval.BeginDate, eval.EndDate), null, null, null);
				var dacList = db.DeclareAchievementDal.ConditionQuery(dac.TeacherId == teacherId & dac.CreateDate.Between(eval.BeginDate, eval.EndDate), null, null, null);
				// 学员数量不用做时间验证
				var memberList = db.TeamMemberDal.ConditionQuery(tm.TeamId == teacherId, null, null, null);
				var taList = db.TeamActiveDal.ConditionQuery(ta.TeamId == teacherId & ta.CreateDate.Between(eval.BeginDate, eval.EndDate), null, null, null);
            var tcList = db.TeamContentDal.ConditionQuery(tc.TeamId == teacherId, null, null, null);


            ZisFaz_ZiwYanx(items, result, daList.FindAll(m => m.ActiveKey == DeclareKeys.ZisFaz_ZiwYanx));

				var analysis = "";
				var ZhanskScroe = ZisFaz_JiaoxHuod_KaiZhansk(items, daList.FindAll(m => m.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk || m.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_Yantk), ref analysis);
				var PingwGongzScore = ZisFaz_XuesHuod_DanrPingwGongz(items, daList.FindAll(m => m.ActiveKey == DeclareKeys.ZisFaz_XuesHuod & m.Dynamic9 == PicklistHelper.XuesHuodType.GetItemId(XuesHuodKeys.DanrPingwGongz)), ref analysis);
				var PingbHuojScore = ZisFaz_XuesHuod_PingbHuoj(items, daList.FindAll(m => m.ActiveKey == DeclareKeys.ZisFaz_XuesHuod & m.Dynamic9 == PicklistHelper.XuesHuodType.GetItemId(XuesHuodKeys.PingbHuoj)), ref analysis);

				result.Score += EvalHelper.GetScore(10, ZhanskScroe, PingwGongzScore, PingbHuojScore);
				result.AnalysisContent += (ZhanskScroe + PingwGongzScore + PingbHuojScore) >= 10 ? "" : analysis;

				analysis = "";
				var FabLunwScore = ZisFaz_KeyChengg_FabLunw(items, dacList.FindAll(m => m.AchievementKey == DeclareKeys.ZisFaz_KeyChengg_FabLunw), ref analysis);
				var LixKetScore = ZisFaz_KeyChengg_LixKet(items, dacList.FindAll(m => m.AchievementKey == DeclareKeys.ZisFaz_KeyChengg_KetYanj & m.Dynamic6 == PicklistHelper.Dynamic6.GetItemId(KetYanjKeys.Ket)), ref analysis);
				var XiangmYanjScore = ZisFaz_KeyChengg_XiangmYanj(items, dacList.FindAll(m => m.AchievementKey == DeclareKeys.ZisFaz_KeyChengg_KetYanj & m.Dynamic6 == PicklistHelper.Dynamic6.GetItemId(KetYanjKeys.Xiangm)), ref analysis);
				result.Score += EvalHelper.GetScore(10, FabLunwScore, LixKetScore, XiangmYanjScore);
				result.AnalysisContent += (FabLunwScore + LixKetScore + XiangmYanjScore) >= 10 ? "" : analysis;

				analysis = "";
				var JiaosPeixKecScore = ZisFaz_PeixJiangz_JiaosPeixKec(items, daList.FindAll(m => m.ActiveKey == DeclareKeys.ZisFaz_PeixJiangz_JiaosPeixKec || m.ActiveKey == DeclareKeys.ZisFaz_PeixJiangz_DingxxKec), ref analysis);
				var ZhuantJiangzScore = ZisFaz_PeixJiangz_ZhuantJiangz(items, daList.FindAll(m => m.ActiveKey == DeclareKeys.ZisFaz_PeixJiangz_ZhuantJiangz), ref analysis);
				var KecZiyKafScore = ZhidJians_TesHuodKaiz_KecZiyKaif(items, daList.FindAll(m => m.ActiveKey == DeclareKeys.ZhidJians_TesHuodKaiz && m.Dynamic9 == PicklistHelper.TesHuodKaizType.GetItemId(TesHuodKaizKeys.KecZiyl)), ref analysis);
				result.Score += EvalHelper.GetScore(25, JiaosPeixKecScore, ZhuantJiangzScore, KecZiyKafScore);
				result.AnalysisContent += ZhuantJiangzScore == 0 ? "\"培训课程.讲座、报告\"分析： 您需要参加区级以上的\"讲座、报告\"， 可以获得此项满分。" : (JiaosPeixKecScore + ZhuantJiangzScore + KecZiyKafScore) >= 25 ? "" : analysis;
				
				XueyShul(memberList.Count, result, items);
				if (memberList.Count > 0)
				{
					var subQuery = APQuery.select(tm.MemberId).from(tm).where(tm.TeamId == teacherId);
					var memberDaList = APQuery.select(da.Asterisk, d.DeclareTargetPKID)
						.from(da, d.JoinInner(da.TeacherId == d.TeacherId))
						.where(da.TeacherId.In(subQuery) & (da.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk |
																		da.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_Yantk |
																		da.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_JiaoxPingb) & da.CreateDate.Between(eval.BeginDate, eval.EndDate))
						.query(db, r =>
						{
							var data = new DeclareActiveDataModel();
							da.Fullup(r, data, false);
							data.TargetId = d.DeclareTargetPKID.GetValue(r);
							return data;
						}).ToList();

					var memberDacList = APQuery.select(dac.Asterisk, d.DeclareTargetPKID)
						.from(dac, d.JoinInner(dac.TeacherId == d.TeacherId))
						.where(dac.TeacherId.In(subQuery) & (dac.AchievementKey == DeclareKeys.ZisFaz_KeyChengg_KetYanj | dac.AchievementKey == DeclareKeys.ZisFaz_KeyChengg_FabLunw) &
						dac.CreateDate.Between(eval.BeginDate, eval.EndDate))
						.query(db, r =>
						{
							var data = new DeclareAchievementDataModel();
							dac.Fullup(r, data, false);
							data.TargetId = d.DeclareTargetPKID.GetValue(r);
							return data;
						}).ToList();

					var ZhuanyeFazMubCount = db.DeclareContentDal.ConditionQueryCount(dc.TeacherId.In(subQuery) & dc.ContentKey == DeclareKeys.ZisFaz_ZiwFazJih_ZhuanyeFazMub_Memo1);

					DaijXiex(result, items);
					DaijJih(result, items, tcList);
					DaijXiaoj(result, items, tcList);
					XueyChengzFenx(result, items, memberList);
					DaijZhid(result, items, taList, memberList.Count);
					DaijChengg(result, items, memberDaList, memberDacList, memberList.Count);
					YanxHuodGuiz(items);
					JingfGuanlBanf(items);
					HuodAnpb(items);
					XueyDusShum(items);
					YanxHuodKaoq(items);
					XueyGerFazGuih(items, ZhuanyeFazMubCount, memberList.Count);
				}
			}


			// 读书活动

			private void ZisFaz_ZiwYanx(List<EvalVolumnResultItem> items, EvalVolumnResult result,
				List<DeclareActive> source)
			{
				var item = new EvalVolumnResultItem
				{
					EvalItemKey = EvalVolumnRuleKeys.DusHuod
				};
				items.Add(item);

				var cnt = source.Count;

				switch (cnt)
				{
					case 0:
						item.ChooseValue = "D";
						item.ResultValue = "0分";
						break;
					case 1:
						result.Score += 0.5;
						item.ChooseValue = "C";
						item.ResultValue = "0.5分";
						break;
					case 2:
					case 3:
						result.Score += 2.5;
						item.ChooseValue = "B";
						item.ResultValue = "2.5分";
						break;
					default:
						result.Score += 5;
						item.ChooseValue = "A";
						item.ResultValue = "5分";
						break;
				}

				if(cnt < 4)
				{ 
					result.AnalysisContent += string.Format("\"自身发展.读书活动\"分析： 您还需要填写{0}篇\"自身发展.读书活动\"， 可以获得此项满分。", 4 - cnt);
				}
			}


			//	开展示课(“教学公开课”+“开设研讨课”)

			private double ZisFaz_JiaoxHuod_KaiZhansk(List<EvalVolumnResultItem> items,
				List<DeclareActive> source, ref string analysis)
			{
				var item = new EvalVolumnResultItem
				{
					EvalItemKey = EvalVolumnRuleKeys.KaiZhansk
				};
				items.Add(item);

				double tempScore = 0;
				var tempStr = "\"自身发展.开展示课\"分析： 您还需要参加校级以上的\"教学公开课\"或\"研讨课\"， 可以获得此项满分。";

				if (source.Exists(m => m.Level == LevelNames.Guojj || m.Level == LevelNames.Shij || m.Level == LevelNames.Quj))
				{
					tempScore = 10;
					item.ChooseValue = "A";
					item.ResultValue = "10分";
					tempStr = "";
				}
				else if (source.Exists(m => m.Level == LevelNames.Xiaoj))
				{
					tempScore = 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else
				{
					item.ChooseValue = "C";
					item.ResultValue = "0分";
				}
				analysis += tempStr;

				return tempScore;
			}


			//	担任评委工作

			private double ZisFaz_XuesHuod_DanrPingwGongz(List<EvalVolumnResultItem> items, 
				List<DeclareActive> source, ref string analysis)
			{
				var item = new EvalVolumnResultItem
				{
					EvalItemKey = EvalVolumnRuleKeys.DanrPingwGongz
				};
				items.Add(item);
				var tempStr = "\"自身发展.担任评委工作\"分析: 您还需要参加市级以上的\"担任评委工作\", 可以获得此项满分。";

				double tempScore = 0;
				if (source.Exists(m => m.Level == LevelNames.Guojj || m.Level == LevelNames.Shij))
				{
					tempScore = 10;
					item.ChooseValue = "A";
					item.ResultValue = "10分";
					tempStr = "";
				}
				else if (source.Exists(m => m.Level == LevelNames.Quj))
				{
					tempScore = 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else if (source.Exists(m => m.Level == LevelNames.Xiaoj))
				{
					item.ChooseValue = "C";
					item.ResultValue = "0分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}
				analysis += tempStr;


				return tempScore;
			}


			//	评比获奖

			private double ZisFaz_XuesHuod_PingbHuoj(List<EvalVolumnResultItem> items, 
				List<DeclareActive> source, ref string analysis)
			{
				var item = new EvalVolumnResultItem
				{
					EvalItemKey = EvalVolumnRuleKeys.PingbHuoj
				};
				items.Add(item);
				var tempStr = "\"自身发展.评比获奖\"分析： 您还需要参加市级以上的\"评比获奖\"， 可以获得此项满分。";
				

				double tempScore = 0;
				if (source.Exists(m => m.Level == LevelNames.Guojj || m.Level == LevelNames.Shij))
				{
					tempScore = 10;
					item.ChooseValue = "A";
					item.ResultValue = "10分";
					tempStr = "";
				}
				else if (source.Exists(m => m.Level == LevelNames.Quj))
				{
					tempScore = 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else if (source.Exists(m => m.Level == LevelNames.Xiaoj))
				{
					item.ChooseValue = "C";
					item.ResultValue = "0分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}
				analysis += tempStr;

				return tempScore;
			}


			//	发表论文

			private double ZisFaz_KeyChengg_FabLunw(List<EvalVolumnResultItem> items, 
				List<DeclareAchievement> source, ref string analysis)
			{
				var item = new EvalVolumnResultItem
				{
					EvalItemKey = EvalVolumnRuleKeys.FabLunw
				};
				items.Add(item);
				var tempStr = "\"自身发展.发表论文\"分析： 您还需要参加市级以上的\"发表论文\"， 可以获得此项满分。";

				double tempScore = 0;
				if (source.Exists(m => m.Level == LevelNames.Guojj || m.Level == LevelNames.Shij))
				{
					tempScore = 10;
					item.ChooseValue = "A";
					item.ResultValue = "10分";
					tempStr = "";
				}
				else if (source.Exists(m => m.Level == LevelNames.Quj))
				{
					tempScore = 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else if (source.Exists(m => m.Level == LevelNames.Xiaoj))
				{
					item.ChooseValue = "C";
					item.ResultValue = "0分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}
				analysis += tempStr;


				return tempScore;
			}


			//	立项课题

			private double ZisFaz_KeyChengg_LixKet(List<EvalVolumnResultItem> items, 
				List<DeclareAchievement> source, ref string analysis)
			{
				var item = new EvalVolumnResultItem
				{
					EvalItemKey = EvalVolumnRuleKeys.LixKet
				};
				items.Add(item);

				var tempStr = "\"自身发展.立项课题\"分析： 您还需要参加区级以上的\"立项课题\"， 可以获得此项满分。";

				double tempScore = 0;
				if (source.Exists(m => m.Level == LevelNames.Guojj || m.Level == LevelNames.Shij))
				{
					tempScore = 10;
					item.ChooseValue = "A";
					item.ResultValue = "10分";
					tempStr = "";
				}
				else if (source.Exists(m => m.Level == LevelNames.Quj))
				{
					tempScore = 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else if (source.Exists(m => m.Level == LevelNames.Xiaoj))
				{
					item.ChooseValue = "C";
					item.ResultValue = "0分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}
				analysis += tempStr;

				return tempScore;
			}


			//	项目研究

			private double ZisFaz_KeyChengg_XiangmYanj(List<EvalVolumnResultItem> items, 
				List<DeclareAchievement> source, ref string analysis)
			{
				var item = new EvalVolumnResultItem
				{
					EvalItemKey = EvalVolumnRuleKeys.XiangmYanj
				};
				items.Add(item);

				var tempStr = "\"自身发展.项目研究\"分析： 您还需要参加市级以上的\"项目研究\"， 可以获得此项满分。";

				double tempScore = 0;
				if (source.Exists(m => m.Level == LevelNames.Guojj || m.Level == LevelNames.Shij))
				{
					tempScore = 10;
					item.ChooseValue = "A";
					item.ResultValue = "10分";
					tempStr = "";
				}
				else if (source.Exists(m => m.Level == LevelNames.Quj))
				{
					tempScore = 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else if (source.Exists(m => m.Level == LevelNames.Xiaoj))
				{
					item.ChooseValue = "C";
					item.ResultValue = "0分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}
				analysis += tempStr;

				return tempScore;
			}


			//	开设教师培训课程

			private double ZisFaz_PeixJiangz_JiaosPeixKec(List<EvalVolumnResultItem> items, 
				List<DeclareActive> source, ref string analysis)
			{
				var item = new EvalVolumnResultItem
				{
					EvalItemKey = EvalVolumnRuleKeys.JiaosPeixKec
				};
				items.Add(item);

				var tempStr = "\"培训课程.开设教师培训课程\"分析： 您还需要参加一门\"定向性课程\"和一门\"区级研训一体课程\"或\"市级课程\"， 可以获得此项满分。";

				double tempScore = 0;
				if (source.Exists(m => m.ActiveKey == DeclareKeys.ZisFaz_PeixJiangz_DingxxKec) && 
					 source.Exists(m => m.ActiveKey == DeclareKeys.ZisFaz_PeixJiangz_JiaosPeixKec && 
									  (m.Dynamic2 == LevelNames.PeixJiangz_Shij || m.Dynamic2 == LevelNames.PeixJiangz_YanxYit)))
				{
					tempScore = 25;
					item.ChooseValue = "A";
					item.ResultValue = "25分";
					tempStr = "";
				}
				else if (source.Exists(m => m.ActiveKey == DeclareKeys.ZisFaz_PeixJiangz_DingxxKec))
				{
					tempScore = 15;
					item.ChooseValue = "B";
					item.ResultValue = "15分";
				}
				else if (source.Exists(m => m.ActiveKey == DeclareKeys.ZisFaz_PeixJiangz_JiaosPeixKec &&
									       (m.Dynamic2 == LevelNames.PeixJiangz_Shij || m.Dynamic2 == LevelNames.PeixJiangz_YanxYit)))
				{
					tempScore = 15;
					item.ChooseValue = "C";
					item.ResultValue = "15分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}
				analysis += tempStr;


				return tempScore;
			}


			//	讲座、报告

			private double ZisFaz_PeixJiangz_ZhuantJiangz(List<EvalVolumnResultItem> items, 
				List<DeclareActive> source, ref string analysis)
			{
				var item = new EvalVolumnResultItem
				{
					EvalItemKey = EvalVolumnRuleKeys.ZhuantJiangz
				};
				items.Add(item);

				var tempStr = "\"培训课程.讲座、报告\"分析： 您需要参加区级以上的\"讲座、报告\"， 可以获得此项满分。";

				double tempScore = 0;
				if (source.Exists(m => m.Level == LevelNames.Guojj || m.Level == LevelNames.Shij))
				{
					tempScore = 25;
					item.ChooseValue = "A";
					item.ResultValue = "25分";
					tempStr = "";
				}
				else if (source.Exists(m => m.Level == LevelNames.Quj))
				{
					tempScore = 12.5;
					item.ChooseValue = "B";
					item.ResultValue = "12.5分";
				}
				else if (source.Exists(m => m.Level == LevelNames.Xiaoj))
				{
					tempScore = 2.5;
					item.ChooseValue = "C";
					item.ResultValue = "2.5分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}
				analysis += tempStr;


				return tempScore;
			}


			//	课程资源开发

			private double ZhidJians_TesHuodKaiz_KecZiyKaif(List<EvalVolumnResultItem> items, 
				List<DeclareActive> source, ref string analysis)
			{
				var item = new EvalVolumnResultItem
				{
					EvalItemKey = EvalVolumnRuleKeys.KecZiyKaif
				};
				items.Add(item);

				var tempStr = "\"培训课程.课程资源开发\"分析： 您还需要参加市级以上的\"课程资源开发\"， 可以获得此项满分。";

				double tempScore = 0;
				if (source.Exists(m => m.Level == LevelNames.Guojj))
				{
					tempScore = 25;
					item.ChooseValue = "A";
					item.ResultValue = "25分";
					tempStr = "";
				}
				else if (source.Exists(m => m.Level == LevelNames.Shij))
				{
					tempScore = 25;
					item.ChooseValue = "B";
					item.ResultValue = "25分";
					tempStr = "";
				}
				else if (source.Exists(m => m.Level == LevelNames.Quj))
				{
					tempScore = 12.5;
					item.ChooseValue = "C";
					item.ResultValue = "12.5分";
				}
				else if (source.Exists(m => m.Level == LevelNames.Xiaoj))
				{
					tempScore = 2.5;
					item.ChooseValue = "D";
					item.ResultValue = "2.5分";
				}
				else
				{
					item.ChooseValue = "E";
					item.ResultValue = "0分";
				}
				analysis += tempStr;

				return tempScore;
			}


			//	学员数量

			private void XueyShul(int MemberCount, EvalVolumnResult result, List<EvalVolumnResultItem> items)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.XueyShul
				};
				items.Add(item);


				if (MemberCount >= 7)
				{
					result.Score += 5;
					item.ChooseValue = "A";
					item.ResultValue = "5分";
				}
				else if (MemberCount > 0 & MemberCount < 7)
				{
					result.Score += 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else
				{
					item.ChooseValue = "C";
					item.ResultValue = "0分";
				}

				if(MemberCount < 7)
				{ 
					result.AnalysisContent += string.Format("\"带教教师.学员数量\"分析： 你还需带{0}学员， 可以获得此项满分。", 7 - MemberCount);
				}
			}


			//	带教协议

			private void DaijXiex(EvalVolumnResult result, List<EvalVolumnResultItem> items)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.DaijXiey,
					ChooseValue = "A",
					ResultValue = "2.5分"
				};
				items.Add(item);

				result.Score += 2.5;
			}


			//	带教计划

			private void DaijJih(EvalVolumnResult result, List<EvalVolumnResultItem> items, 
				List<TeamContent> source)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.JutJih
				};
				items.Add(item);

				var exists = source.Find(m => m.ContentKey == TeamKeys.DaijJih_Memo2);


				if (exists != null)
				{
					result.Score += 7.5;
					item.ChooseValue = "A";
					item.ResultValue = "7.5分";
				}
				else
				{
					item.ChooseValue = "B";
					item.ResultValue = "0分";
					result.AnalysisContent += string.Format("\"带教教师.带教计划\"分析： 您需要填写\"带教计划\"，可以获得此项满分。");
				}
			}


			// 带教小结

			private void DaijXiaoj(EvalVolumnResult result, List<EvalVolumnResultItem> items, 
				List<TeamContent> source)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.DaijXiaoj
				};
				items.Add(item);

				var exists = source.Find(m => m.ContentKey == TeamKeys.DaijJih_Memo3);


				if (exists != null)
				{
					result.Score += 5;
					item.ChooseValue = "A";
					item.ResultValue = "5分";
				}
				else
				{
					item.ChooseValue = "B";
					item.ResultValue = "0分";
					result.AnalysisContent += string.Format("\"带教教师.带教小结\"分析： 您需要填写\"带教小结\"， 可以获得此项满分。");
				}
			}


			//	学员成长分析

			private void XueyChengzFenx(EvalVolumnResult result, List<EvalVolumnResultItem> items,
				List<TeamMember> source)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.XueydChengzFangx
				};
				items.Add(item);

				var exists = source.FindAll(m => m.ContentValue.Trim() != "");
				var analysis = "\"带教教师.学员成长分析\"分析： 您需要填写每个学员的\"学员成长分析\"， 可以获得此项满分。";

				if (exists == null || exists.Count == 0)
				{
					item.ChooseValue = "C";
					item.ResultValue = "0分";
				}
				else if (exists.Count < source.Count)
				{
					result.Score += 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else
				{
					result.Score += 5;
					item.ChooseValue = "A";
					item.ResultValue = "5分";
					analysis = "";
				}

				result.AnalysisContent += analysis;
			}


			//	带教指导

			private void DaijZhid(EvalVolumnResult result, List<EvalVolumnResultItem> items, List<TeamActive> source, int MemberCount)
			{
				var analysis = "";
				var RicGongxlZhidScore = RicGongxlZhid(items, source.FindAll(m => m.ActiveType == 
				PicklistHelper.TeamActiveType.GetItemId(TeamActiveKeys.RicGongxlZhid)), ref analysis);

				var TingkZhidScore = TingkZhid(items, source.FindAll(m => m.ActiveType ==
				PicklistHelper.TeamActiveType.GetItemId(TeamActiveKeys.TingkZhid)), MemberCount, ref analysis);

				var JiaoalXiugZhidScore = JiaoalXiugZhid(items, source.FindAll(m => m.ActiveType ==
				PicklistHelper.TeamActiveType.GetItemId(TeamActiveKeys.JiaoalXiugZhid)), MemberCount, ref analysis);

				var LunwHuoKetXiugZhidScore = LunwHuoKetXiugZhid(items, source.FindAll(m => m.ActiveType ==
				PicklistHelper.TeamActiveType.GetItemId(TeamActiveKeys.LunwHuoKetXiugZhid)), MemberCount, ref analysis);

				result.Score += EvalHelper.GetScore(10, RicGongxlZhidScore, TingkZhidScore, JiaoalXiugZhidScore, LunwHuoKetXiugZhidScore);
				result.AnalysisContent += analysis;
			}

			//	日常共性类指导

			private double RicGongxlZhid(List<EvalVolumnResultItem> items, List<TeamActive> source, ref string analysis)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.RicGongxlZhid
				};
				items.Add(item);

				var count = source.Count();
				var tempScore = 0.0;
				analysis += "";


				if (count >= 8)
				{
					tempScore = 5;
					item.ChooseValue = "A";
					item.ResultValue = "5分";
				}
				else if (count >= 5 && count <= 7)
				{
					tempScore = 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else if (count >= 1 && count <= 4)
				{
					tempScore = 1.3;
					item.ChooseValue = "C";
					item.ResultValue = "1.3分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}

				if(count < 8)
				{
					analysis += string.Format("\"带教指导.日常共性类指导\"分析： 您还需要填写{0}份\"日常共性类指导\"， 可以获得此项满分。", 8 - count);
				}
				

				return tempScore;
			}


			//	听课指导

			private double TingkZhid(List<EvalVolumnResultItem> items, List<TeamActive> source, int memberCount, ref string analysis)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.TingkZhid
				};
				items.Add(item);

				
				var count = source.Count();
				var tempScore = 0.0;
				analysis += "";

				if (count >= 4*memberCount)
				{
					tempScore = 5;
					item.ChooseValue = "A";
					item.ResultValue = "5分";
				}
				else if (count >= memberCount && count <= 4*memberCount)
				{
					tempScore = 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else if (count < memberCount && count > 0)
				{
					tempScore = 1.3;
					item.ChooseValue = "C";
					item.ResultValue = "1.3分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}

				if (count < 4 * memberCount)
				{
					analysis += string.Format("\"带教指导.听课指导\"分析： 您还需要填写{0}份\"听课指导\"， 可以获得此项满分。", 4 * memberCount - count);
				}

				return tempScore;
			}


			//	教案类修改指导

			private double JiaoalXiugZhid(List<EvalVolumnResultItem> items, List<TeamActive> source, int memberCount, ref string analysis)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.JiaoalXiugZhid
				};
				items.Add(item);

				var count = source.Count();
				var tempScore = 0.0;
				analysis += "";

				if (count >= 2 * memberCount)
				{
					tempScore = 5;
					item.ChooseValue = "A";
					item.ResultValue = "5分";
				}
				else if (count >= memberCount && count <= 2 * memberCount)
				{
					tempScore = 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else if (count < memberCount && count > 0)
				{
					tempScore = 1.3;
					item.ChooseValue = "C";
					item.ResultValue = "1.3分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}

				if (count < 2 * memberCount)
				{
					analysis += string.Format("\"带教指导.教案类修改指导\"分析： 您还需要填写{0}份\"教案类修改指导\", 可以获得此项满分。", 2 * memberCount - count);
				}

				return tempScore;
			}


			//	论文或课题修改指导

			private double LunwHuoKetXiugZhid(List<EvalVolumnResultItem> items, List<TeamActive> source, int memberCount, ref string analysis)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.LunwHuoKetXiugZhid
				};
				items.Add(item);

				var count = source.Count();
				var tempScore = 0.0;
				analysis += "";

				if (count >= 2 * memberCount)
				{
					tempScore = 5;
					item.ChooseValue = "A";
					item.ResultValue = "5分";
				}
				else if (count >= memberCount && count <= 2 * memberCount)
				{
					tempScore = 2.5;
					item.ChooseValue = "B";
					item.ResultValue = "2.5分";
				}
				else if (count < memberCount && count > 0)
				{
					tempScore = 1.3;
					item.ChooseValue = "C";
					item.ResultValue = "1.3分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}

				if (count < 2 * memberCount)
				{
					analysis = string.Format("\"带教指导.论文或课题修改指导\"分析： 您还需要填写{0}份\"论文或课题修改指导\"， 可以获得此项满分。", 2 * memberCount - count);
				}

				return tempScore;
			}


			//	带教指导

			private void DaijChengg(EvalVolumnResult result, List<EvalVolumnResultItem> items, List<DeclareActiveDataModel> source, List<DeclareAchievementDataModel> source1, int memberCount)
			{
				var analysis = "";
				var KaisZhanskScore = KaisZhansk(items, source.FindAll( m => (m.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk |
																								  m.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_Yantk) & 
																								  (m.Level != LevelNames.Xiaoj)),
															memberCount, ref analysis);

				var JiaoyXuekKeyChenggScore = JiaoyXuekKeyChengg(items, source1.FindAll(m => m.Level != LevelNames.Xiaoj), memberCount, ref analysis);

				var JiaoyJiaoxPingbScore = JiaoyJiaoxPingb(items, source.FindAll(m => (m.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_JiaoxPingb) &
																												(m.Level != LevelNames.Xiaoj)), memberCount, ref analysis);

				result.Score += EvalHelper.GetScore(15, KaisZhanskScore, JiaoyXuekKeyChenggScore, JiaoyJiaoxPingbScore);

				result.AnalysisContent += (KaisZhanskScore + JiaoyXuekKeyChenggScore + JiaoyJiaoxPingbScore) >= 15 ? "" : analysis;
			}


			//	带教指导.开展示课

			private double KaisZhansk(List<EvalVolumnResultItem> items, List<DeclareActiveDataModel> source, int memberCount, ref string analysis)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.DaijZhidKaiZhansk
				};
				items.Add(item);

				var sourceCount = source.GroupBy(m => m.TeacherId).Count();
				var maxScore =  DajChenggHelper(item, sourceCount, memberCount);
				analysis += "";

				if (maxScore != 7.5)
				{
					analysis += string.Format("\"带教指导.开展示课\"分析： 您还需要{0}个学员获得\"开展示课\"满分， 可以获得此项满分。", memberCount - sourceCount);
				}

				return maxScore;
			}


			//	带教指导.教育学科科研成果

			private double JiaoyXuekKeyChengg(List<EvalVolumnResultItem> items, List<DeclareAchievementDataModel> source, int memberCount, ref string analysis)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.JiaoyXuekKeyChengg
				};
				items.Add(item);

				var sourceCount = source.GroupBy(m => m.TeacherId).Count();
				var maxScore = DajChenggHelper(item, sourceCount, memberCount);
				analysis += "";

				if (maxScore != 7.5)
				{
					analysis += string.Format("\"带教指导.教育学科科研成果\"分析： 您还需要{0}个学员获得\"教育学科科研成果\"满分， 可以获得此项满分。", memberCount - sourceCount);
				}

				return maxScore;
			}


			//	带教指导.教育教学评比

			private double JiaoyJiaoxPingb(List<EvalVolumnResultItem> items, List<DeclareActiveDataModel> source, int memberCount, ref string analysis)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.JiaoyJiaoxPingb
				};
				items.Add(item);


				var sourceCount = source.GroupBy(m => m.TeacherId).Count();
				var maxScore = DajChenggHelper(item, sourceCount, memberCount);
				analysis += "";

				if (maxScore != 7.5)
				{
					analysis += string.Format("\"带教指导.教育教学评比\"分析： 您还需要{0}个学员获得\"教育教学评比\"满分， 可以获得此项满分。", memberCount - sourceCount);
				}

				return maxScore;
			}


			private double DajChenggHelper(EvalVolumnResultItem item, int count, int memberCount)
			{
				var tempScore = 0.0;
				if (count == memberCount)
				{
					tempScore = 7.5;
					item.ChooseValue = "A";
					item.ResultValue = "7.5分";
				}
				else if (count < memberCount & count >= memberCount * 0.6)
				{
					tempScore = 3.8;
					item.ChooseValue = "B";
					item.ResultValue = "3.8分";
				}
				else if (count < memberCount * 0.6 & count >= memberCount * 0.3)
				{
					tempScore = 2.3;
					item.ChooseValue = "C";
					item.ResultValue = "2.3分";
				}
				else
				{
					item.ChooseValue = "D";
					item.ResultValue = "0分";
				}


				return tempScore;
			}


			//	研修活动规则

			private void YanxHuodGuiz(List<EvalVolumnResultItem> items)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.YanxHuodGuiz,
					ChooseValue = "A",
					ResultValue = "达标"
				};

				items.Add(item);
			}


			// 经费管理办法

			private void JingfGuanlBanf(List<EvalVolumnResultItem> items)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.JingfGuanlBanf,
					ChooseValue = "A",
					ResultValue = "达标"
				};

				items.Add(item);
			}


			// 活动安排表

			private void HuodAnpb(List<EvalVolumnResultItem> items)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.HuodAnpb,
					ChooseValue = "A",
					ResultValue = "达标"
				};

				items.Add(item);
			}


			// 学员读书书目

			private void XueyDusShum(List<EvalVolumnResultItem> items)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.XueyDusShum,
					ChooseValue = "A",
					ResultValue = "达标"
				};

				items.Add(item);
			}


			// 研修活动考勤

			private void YanxHuodKaoq(List<EvalVolumnResultItem> items)
			{
				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.YanxHuodKaoq,
					ChooseValue = "A",
					ResultValue = "达标"
				};

				items.Add(item);
			}


			//	学员个人发展规划

			private void XueyGerFazGuih(List<EvalVolumnResultItem> items, int count, int memberCount)
			{

				var item = new EvalVolumnResultItem()
				{
					EvalItemKey = EvalVolumnRuleKeys.XueyGerFazGuih,
				};
				items.Add(item);

				if (count == memberCount)
				{
					item.ChooseValue = "A";
					item.ResultValue = "达标";
				}
				else if (count != memberCount && count > 0)
				{
					item.ChooseValue = "B";
					item.ResultValue = "未达标";
				}
				else
				{
					item.ChooseValue = "C";
					item.ResultValue = "无";
				}
			}


			#endregion

		}

	}

}