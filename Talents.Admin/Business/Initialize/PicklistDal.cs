using Business.Config;
using Business.Helper;
using System;
using System.Collections.Generic;

namespace Business
{

	public partial class APDalDef
	{

		public partial class PicklistDal
		{

			public override void InitData(APDBDef db)
			{
				long key, lessthen;

				#region [ 1000 < 1010 : Gender 性别 ]
				{
					key = 1000; lessthen = 1010;

					var pk = new Picklist(key, PicklistKeys.Gender, "性别", false, false, "对性别进行选择的字典项。");

					var items = FromArray(
						0,
						itemNames: new string[] { "男", "女" },
						codes: new string[] { "1", "2" }
						);
					items.Add(new PicklistItem());
					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 1010 < 1030 : PoliticalStatus 政治面貌 ]
				{
					key = 1010; lessthen = 1030;

					var pk = new Picklist(key, PicklistKeys.PoliticalStatus, "政治面貌", false, false, "对政治面貌进行选择的字典项。");

					var items = FromArray(
						0,
						itemNames: new string[] {
							"中国共产党党员", "中国共产党预备党员", "中国共产主义青年团团员", "中国国民党革命委员会会员", "中国民主同盟盟员",
							"中国民主建国会会员", "中国民主促进会会员", "中国农工民主党党员", "中国致公党党员", "九三学社社员",
							"台湾民主自治同盟盟员", "无党派民主人士", "群众", "其他"
							}
						);

					items[items.Count - 1].PicklistItemId = --lessthen;

					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 1030 < 1130 : Nationality 民族]
				{
					key = 1030; lessthen = 1130;

					var pk = new Picklist(key, PicklistKeys.Nationality, "民族", false, false, "对民族进行选择的字典项。");

					var items = FromArray(
						0,
						itemNames: new string[]
						{
							"汉族", "蒙古族", "回族", "藏族", "维吾尔族", "苗族", "彝族", "壮族", "布依族", "朝鲜族",
							"满族", "侗族", "瑶族", "白族", "土家族", "哈尼族", "哈萨克族", "傣族", "黎族", "傈僳族",
							"佤族", "畲族", "高山族", "拉祜族", "水族", "东乡族", "纳西族", "景颇族", "柯尔克孜族", "土族",
							"达斡尔族", "仫佬族", "羌族", "布朗族", "撒拉族", "毛难族", "仡佬族", "锡伯族", "阿昌族", "普米族",
							"塔吉克族", "怒族", "乌孜别克族", "俄罗斯族", "鄂温克族", "德昂族", "保安族", "裕固族", "京族", "塔塔尔族",
							"独龙族", "鄂伦春族", "赫哲族", "门巴族", "珞巴族", "基诺族", "其他"
						});

					items[items.Count - 1].PicklistItemId = --lessthen;

					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 1130 < 1230 : EduBg 学历 ]
				{
					key = 1130; lessthen = 1230;

					var pk = new Picklist(key, PicklistKeys.EduBg, "学历", false, false, "对学历进行选择的字典项。");

					var items = FromArray(
						0,
						itemNames: new string[] {
							"研究生教育", "博士研究生毕业", "博士研究生结业", "博士研究生肄业", "硕士研究生毕业",
							"硕士研究生结业", "硕士研究生肄业", "研究生班毕业", "研究生班结业", "研究生班肄业",
							"大学本科教育", "大学本科毕业", "大学本科结业", "大学本科肄业", "大学普通班毕业",
							"大学专科教育", "大学专科毕业", "大学专科结业", "大学专科肄业",
							"中等职业教育", "中等专科毕业", "中等专科结业", "中等专科肄业", "职业高中毕业",
							"职业高中结业", "职业高中肄业", "技工学校毕业", "技工学校结业", "技工学校肄业",
							"普通高级中学教育", "普通高中毕业", "普通高中结业", "普通高中肄业",
							"初级中学教育", "初中毕业", "初中肄业",
							"小学教育", "小学毕业", "小学肄业", "其他",
							},
						codes: new string[] {
							"10", "11", "12", "13", "14",
							"15", "16", "17", "18", "19",
							"20", "21", "22", "23", "28",
							"30", "31", "32", "33",
							"40", "41", "42", "43", "44",
							"45", "46", "47", "48", "49",
							"60", "61", "62", "63",
							"70", "71", "73",
							"80", "81", "83", "90",
							}
						);
					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 1230 < 1330 : EduDegree 学位 ]
				{
					key = 1230; lessthen = 1330;

					var pk = new Picklist(key, PicklistKeys.EduDegree, "学位", false, false, "对学位进行选择的字典项。");

					var items = FromArray(
						0,
						itemNames: new string[] {
							"名誉博士", "博士", "哲学博士学位", "经济学博士学位", "法学博士学位",
							"教育学博士学位", "文学博士学位", "历史学博士学位", "理学博士学位", "工学博士学位",
							"农学博士学位", "医学博士学位", "军事学博士学位", "管理学博士学位", "临床医学博士专业学位",
							"兽医博士专业学位", "口腔医学博士专业学位",
							"硕士", "哲学硕士学位", "经济学硕士学位", "法学硕士学位", "教育学硕士学位",
							"文学硕士学位", "历史学硕士学位", "理学硕士学位", "工学硕士学位", "农学硕士学位",
							"医学硕士学位", "军事学硕士学位", "管理学硕士学位", "法律硕士专业学位", "教育硕士专业学位",
							"工程硕士专业学位", "建筑学硕士专业学位", "临床学硕士专业学位", "工商管理硕士专业学位", "农业推广硕士专业学位",
							"兽医硕士专业学位", "公共管理硕士专业学位", "口腔医学硕士专业学位", "公共卫生硕士专业学位", "军事硕士专业学位",
							"学士", "哲学学士学位", "经济学学士学位", "法学学士学位", "教育学学士学位",
							"文学学士学位", "历史学学士学位", "理学学士学位", "工学学士学位", "农学学士学位",
							"医学学士学位", "军事学学士学位"
							},
						codes: new string[] {
							"1", "2", "201", "202", "203",
							"204", "205", "206", "207", "208",
							"209", "210", "211", "212", "245",
							"248", "250",
							"3", "301", "302", "303", "304",
							"305", "306", "307", "308", "309",
							"310", "311", "312", "341", "342",
							"343", "344", "345", "346", "347",
							"348", "349", "350", "351", "352",
							"4", "401", "402", "403", "404",
							"405", "406", "407", "408", "409",
							"410", "411"
							}
						);

					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 1380 < 1410 : SkillTitle 职称 ]
				{
					key = 1380; lessthen = 1410;

					var pk = new Picklist(key, PicklistKeys.SkillTitle, "职称", false, false, "对职称进行选择的字典项。");

					var items = FromArray(
						0,
						itemNames: new string[] {
							"正高级教师(正高级)", "高级教师(副高级)", "一级教师(中级)", "二级教师(助理级)", "三级教师(员级)"
							},
						codes: new string[] {
							"1", "2", "3", "4", "5"
							}
						);

					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 1410 < 1450 : RankTitle 职务 ]
				{
					key = 1410; lessthen = 1450;

					var pk = new Picklist(key, PicklistKeys.RankTitle, "职务", false, false, "对职务进行选择的字典项。");

					var items = FromArray(
						0,
						itemNames: new string[] {
							"教师", "教师兼行政", "教研室主任（组长）", "年级主任（组长）", "班主任",
							"辅导员、教练员", "共青团工作负责人", "工会工作负责人", "妇女工作负责人", "其他工作负责人",
							"行政", "校领导", "行政处室负责人", "行政处室工作", "行政兼教学工作",
							"教辅", "实习实验工作与管理", "教学仪器设备维护与管理", "体育设备设施维护与管理"
							},
						codes: new string[] {
							"10", "20", "21", "22", "23",
							"24", "25", "26", "27", "28",
							"30", "31", "32", "33", "34",
							"40", "41", "42", "43"
							}
						);

					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 1500 < 1600 : EduSubject 任教学科 ]
				{
					key = 1500; lessthen = 1600;

					var pk = new Picklist(key, PicklistKeys.EduSubject, "任教学科", false, false, "对任教学科进行选择的字典项。");

					var items = FromArray(
						0,
						itemNames: new string[] {
							"品德与社会", "思想品德", "语文", "数学", "科学",
							"物理", "化学", "地理", "历史", "体育与健身",
							"艺术", "音乐", "美术", "思想政治", "社会",
							"生命科学", "科学与技术", "自然", "外语",
							"英语", "日语", "俄语",
							"其他外国语", "综合实践活动", "信息科技", "劳动技术", "教育心理",
							"拓展、探究", "职业生涯教育"
							},
						codes: new string[] {
							"11", "12", "13", "14", "15",
							"16", "17", "20","21", "22",
							"23", "24", "25",
							"28", "29", "30", "31", "32",
							"40", "41", "42", "43", "49",
							"60", "61", "62", "63", "64",
							"80"
							}
						);

					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 1600 < 1620 : EduStage 任教学段 ]
				{
					key = 1600; lessthen = 1620;

					var pk = new Picklist(key, PicklistKeys.EduStage, "任教学段", false, false, "对任教学段进行选择的字典项");

					var items = FromArray(
						0,
						itemNames: new string[] {
							"学前教育", "小学", "普通初中", "普通高中", "职业初中",
							"职业高中", "成人中等专业学校", "成人中学", "特殊教育", "其他",
						},
						codes: new string[] {
							"1", "2", "3", "4", "5",
							"6", "7", "8", "A", "Z",
							}
						);
					items[items.Count - 1].PicklistItemId = --lessthen;

					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 5000 < 5020 : DeclareTarget 申报（担任）称号 ]
				{
					key = 5000; lessthen = 5020;

					var pk = new Picklist(key, PicklistKeys.DeclareTarget, "申报（担任）称号", false, false, "对申报资格进行选择的字典项");

					var items = FromArray(
						new string[] { "外聘导师", "学科高地理事长", "学科培训基地主持人", "学科培训工作室主持人", "学科带头人", "骨干教师", "教学能手", "教学新秀", "特招学员" }
						);

					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 5220 < 5240 : DeclareStage 申报学段 ]
				{
					key = 5220; lessthen = 5240;

					var pk = new Picklist(key, PicklistKeys.DeclareStage, "申报学段", false, false, "对申报学段进行选择的字典项");

					var items = FromArray(
						0,
						itemNames: new string[] {
							"学前教育", "小学", "普通初中", "普通高中", "职业初中",
							"职业高中", "成人中等专业学校", "成人中学", "特殊教育", "其他",
						},
						codes: new string[] {
							"1", "2", "3", "4", "5",
							"6", "7", "8", "A", "Z",
							}
						);
					items[items.Count - 1].PicklistItemId = --lessthen;

					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 5240 < 5340 : DeclareSubject 申报学科 ]
				{
					key = 5240; lessthen = 5340;

					var pk = new Picklist(key, PicklistKeys.DeclareSubject, "申报学科", false, false, "对申报学科进行选择的字典项");

					var items = FromArray(1,
						new string[]
						{
							"语文", "数学", "英语", "政治（思想与品德）",
							"历史", "地理", "物理", "化学",
							"德育", "教育和心理", "音乐", "体育",
							"美术", "信息科技", "跨学科教育", "劳动技术教育"
						});
					var items2 = FromArray(0,
						new string[]
						{
							"幼教", "特殊教育", "职业教育", "校外教育", "社区教育",
							"生命科学", "科学与技术"
						}, baseItemId: 5300);
					items.AddRange(items2);

					SyncInitData(db, pk, items);
				}
				#endregion

				#region [ 5440 < 5540 : XuesHuodType 自身发展.学术活动.类型 ]
				{
					key = 5440; lessthen = 5540;

					var pk = new Picklist(key, PicklistKeys.XuesHuodType, "自身发展.学术活动.类型", false, false, "对自身发展.学术活动.类型进行选择的字典项");

					var items = FromArray(
						new string[] { XuesHuodKeys.DusShal, XuesHuodKeys.WaicKaoc, XuesHuodKeys.DanrPingwGongz, XuesHuodKeys.PingbHuoj, XuesHuodKeys.Qit }
						);

					SyncInitData(db, pk, items);
				}
				#endregion


				#region [ 5640 < 5740 : KetYanjType 自身发展.教育教学科研成果.开展课题(项目)研究工作 ]
				{
					key = 5640; lessthen = 5740;

					var pk = new Picklist(key, PicklistKeys.KetYanjType, "自身发展.教育教学科研成果.开展课题(项目)研究工作.类型", false, false, "对自身发展.教育教学科研成果.开展课题(项目)研究工作.类型进行选择的字典项");

					var items = FromArray(
						new string[] { KetYanjKeys.Ket, KetYanjKeys.Xiangm }
						);

					SyncInitData(db, pk, items);
				}
				#endregion


				#region [ 5840 < 5940 : KetYanjType 自身发展.教育教学科研成果.开展课题(项目)研究工作 ]
				{
					key = 5840; lessthen = 5940;

					var pk = new Picklist(key, PicklistKeys.TesHuodKaizType, "自身发展.教育教学科研成果.开展课题(项目)研究工作.类型", false, false, "对自身发展.教育教学科研成果.开展课题(项目)研究工作.类型进行选择的字典项");

					var items = FromArray(
						new string[] { TesHuodKaizKeys.KecZiyl, TesHuodKaizKeys.HuodZhansl, TesHuodKaizKeys.Qit }
						);

					SyncInitData(db, pk, items);
				}
				#endregion


				#region [ 5940 < 6040 : TeamActiveType 带教活动 ]
				{
					key = 5940; lessthen = 6040;

					var pk = new Picklist(key, PicklistKeys.TeamActiveType, "带教活动", false, false, "对带教活动进行选择的字典项");

					var items = FromArray(
						new string[] { TeamActiveKeys.RicGongxlZhid, TeamActiveKeys.TingkZhid, TeamActiveKeys.LunwHuoKetXiugZhid, TeamActiveKeys.JiaoalXiugZhid }
						);

					SyncInitData(db, pk, items);
				}
				#endregion
			}


			#region [ Helper methods ]


			private void SyncInitData(APDBDef db, Picklist pk, List<PicklistItem> items, int baseInc = 0)
			{
				var t = APDBDef.Picklist;
				if (db.PicklistDal.ConditionQueryCount(t.InnerKey == pk.InnerKey) == 0)
				{
					if (pk.PicklistId == 0)
						throw new Exception("Has not special PicklistId! This is a Obvious Mistake.");
					db.PicklistDal.Insert(pk);

					long baseId = pk.PicklistId + 1 + baseInc;
					foreach (PicklistItem item in items)
					{
						if (item.PicklistItemId == 0)
							item.PicklistItemId = baseId++;
						item.PicklistId = pk.PicklistId;
						db.PicklistItemDal.Insert(item);
					}
				}
			}


			private List<PicklistItem> FromArray(string[] itemNames, long[] strengthenValues = null, string[] codes = null, string defaultItem = null, long baseItemId = 0)
			{
				List<PicklistItem> items = new List<PicklistItem>();
				for (int i = 0, len = itemNames.Length; i < len; i++)
				{
					PicklistItem item = new PicklistItem { Name = itemNames[i] };
					if (strengthenValues != null)
						item.StrengthenValue = strengthenValues[i];
					if (codes != null)
						item.Code = codes[i];
					if (item.Name == defaultItem)
						item.IsDefault = true;
					if (baseItemId != 0)
						item.PicklistItemId = baseItemId++;
					items.Add(item);
				}

				return items;
			}


			private List<PicklistItem> FromArray(long strengthenValue, string[] itemNames, string[] codes = null, string defaultItem = null, long baseItemId = 0)
			{
				List<PicklistItem> items = new List<PicklistItem>();
				for (int i = 0, len = itemNames.Length; i < len; i++)
				{
					PicklistItem item = new PicklistItem { Name = itemNames[i], StrengthenValue = strengthenValue };
					if (codes != null)
						item.Code = codes[i];
					if (item.Name == defaultItem)
						item.IsDefault = true;
					if (baseItemId != 0)
						item.PicklistItemId = baseItemId++;
					items.Add(item);
				}

				return items;
			}


			#endregion


		}

	}

}