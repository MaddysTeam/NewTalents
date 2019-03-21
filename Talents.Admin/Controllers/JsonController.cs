using Business;
using Business.Helper;
using Symber.Web.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TheSite.Controllers
{

   public class JsonController : BaseController
   {

      static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
      static APDBDef.TeamMemberTableDef tm = APDBDef.TeamMember;


      public ActionResult GetViewMenu(long userId)
      {
         ThrowNotAjax();


         var model = db.DeclareBaseDal.PrimaryGet(userId);
         var targetId = model.DeclareTargetPKID;


         List<json_treenode> list = new List<json_treenode>();


         #region [ 自身发展 ]


         list.Add(new json_treenode { id = DeclareKeys.ZisFaz, text = "自身发展", type = json_treenode_types.database, children = new List<json_treenode>() });

         var zisFaz = list.Find(m => m.id == DeclareKeys.ZisFaz);

         if (zisFaz != null)
         {
            zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_GerChengj, text = "个人成就", type = json_treenode_types.content });
            zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_GerJianl, text = "个人简历", type = json_treenode_types.content });

            if (targetId != DeclareTargetIds.GaodLisz && targetId != DeclareTargetIds.JidZhucr)
            {
               zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_GerSWOT, text = "个人SWOT分析", type = json_treenode_types.content });
               zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_ZiwFazJih, text = "自我发展计划", type = json_treenode_types.content });
            }

            zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_ZiwYanx, text = "自我研修(读书活动或其他)", type = json_treenode_types.active });
            zisFaz.children.Add(new json_treenode()
            {
               id = DeclareKeys.ZisFaz_JiaoxHuod,
               text = "教学活动",
               type = json_treenode_types.database,
               children = new List<json_treenode>()
               {
                  new json_treenode()
                  {
                     id = DeclareKeys.ZisFaz_ZhansKec,
                     text = "开展示课",
                     type = json_treenode_types.database,
                     children = new List<json_treenode>()
                     {
                        new json_treenode() { id = DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk, text = "开设教学公开课", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.ZisFaz_JiaoxHuod_Yantk, text = "开设研讨课", type = json_treenode_types.active },
                     }
                  },
                  new json_treenode()
                  {
                     id = DeclareKeys.ZisFaz_PingwGongzHuoPbiHuoj,
                     text = "学术活动（评委工作或评比获奖）",
                     type = json_treenode_types.database,
                     children = new List<json_treenode>()
                     {
                       new json_treenode() { id = DeclareKeys.ZisFaz_JiaoxHuod_JiaoxPingb, text = "参加教育教学评比", type = json_treenode_types.active },
                       new json_treenode() { id = DeclareKeys.ZisFaz_XuesHuod, text = DeclareTargetIds.AllowXuesHuod(targetId)? "学术活动、特色" :"学术活动", type = json_treenode_types.active },
                     }
                  }
               }
            });
            zisFaz.children.Add(new json_treenode()
            {
               id = DeclareKeys.ZisFaz_KeyChengg,
               text = "教育教学科研成果",
               type = json_treenode_types.database,
               children = new List<json_treenode>()
               {
                  new json_treenode() { id = DeclareKeys.ZisFaz_KeyChengg_FabLunw, text="论文发表", type = json_treenode_types.active },
                  new json_treenode() { id = DeclareKeys.ZisFaz_KeyChengg_KetYanj, text="开展课题(项目)研究工作", type = json_treenode_types.active },
                  new json_treenode() { id = DeclareKeys.ZisFaz_KeyChengg_LunzQingk, text="论著情况", type = json_treenode_types.active }
               }
            });
            zisFaz.children.Add(new json_treenode()
            {
               id = DeclareKeys.ZisFaz_PeixJiangz,
               text = "培训与讲座",
               type = json_treenode_types.database,
               children = new List<json_treenode>()
               {
                  new json_treenode() { id = DeclareKeys.ZisFaz_PeixJiangz_JiaosPeixKec, text="开设教师培训课程", type = json_treenode_types.active },
                  new json_treenode() { id = DeclareKeys.ZisFaz_PeixJiangz_ZhuantJiangz, text="开设学科类专题讲座", type = json_treenode_types.active }
               }
            });
            if (DeclareTargetIds.AllowKecShis(targetId))
            {
               var ZisFaz_PeixJiangz = zisFaz.children.Find(m => m.id == DeclareKeys.ZisFaz_PeixJiangz);
               ZisFaz_PeixJiangz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_PeixJiangz_DingxxKec, text = "开设定向性课程", type = json_treenode_types.active });
            }

            if (DeclareTargetIds.AllowKecZiy(targetId))
            {
               var ZisFaz_PeixJiangz = zisFaz.children.Find(m => m.id == DeclareKeys.ZisFaz_PeixJiangz);
               ZisFaz_PeixJiangz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_PeixJiangz_KecZiyKaif, text = "课程资源开发", type = json_treenode_types.active });
            }

            zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_ShiqjHuod, text = "市、区级大活动", type = json_treenode_types.active });
         }


         #endregion


         #region [ 制度建设 ]


         if (targetId == DeclareTargetIds.GaodLisz || targetId == DeclareTargetIds.JidZhucr || targetId == DeclareTargetIds.GongzsZhucr || targetId == DeclareTargetIds.PutLaos)
         {
            list.Add(new json_treenode { id = DeclareKeys.ZhidJians, text = "制度建设", type = json_treenode_types.database, children = new List<json_treenode>() });
         }

         var ZhidJians = list.Find(m => m.id == DeclareKeys.ZhidJians);

         if (ZhidJians != null)
         {
            if (targetId != DeclareTargetIds.GongzsZhucr)
            {
               ZhidJians.children.Add(new json_treenode() { id = DeclareKeys.ZhidJians_YingxlDeGongz, text = "有影响力的工作", type = json_treenode_types.active });
            }

            ZhidJians.children.Add(new json_treenode() { id = DeclareKeys.ZhidJians_TesHuodKaiz, text = "特色活动开展", type = json_treenode_types.active });
            ZhidJians.children.Add(new json_treenode() { id = DeclareKeys.ZhidJians_DangaJians, text = "档案建设", type = json_treenode_types.active });
         }


         #endregion


         #region [ 区内流动 ]


         if (model.AllowFlowToSchool)
         {
            list.Add(new json_treenode { id = DeclareKeys.QunLiud, text = "区内流动", type = json_treenode_types.database, children = new List<json_treenode>() });
         }

         var QunLiud = list.Find(m => m.id == DeclareKeys.QunLiud);

         if (QunLiud != null)
         {
            if (targetId == DeclareTargetIds.XuekDaitr)
            {
               QunLiud.children.Add(new json_treenode()
               {
                  id = DeclareKeys.QunLiud_XuekDaitr,
                  text = "学科带头人",
                  type = json_treenode_types.database,
                  children = new List<json_treenode>
                  {
                     new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_LiurXuex, text="流入学校", type = json_treenode_types.content },
                     new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_KetJiaox, text="课堂教学", type = json_treenode_types.database, children = new List<json_treenode>
                     {
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_Gongkk, text="上公开课", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_GongkHuibk, text="公开汇报课", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_Suitk, text="接受教师听随堂课", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_TingKZhid, text="开展听课指导", type = json_treenode_types.active },
                     }},
                     new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey, text="教育科研", type = json_treenode_types.database, children = new List<json_treenode>
                     {
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz, text="开设专题讲座", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod, text="主持教研活动", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_CanyHuod, text="参与教研组、备课组活动", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_JiedxZongj, text="阶段性总结", type = json_treenode_types.content },
                     }},
                     new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_DaijPeix, text="带教培训", type = json_treenode_types.database, children = new List<json_treenode>
                     {
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijDuix, text="带教对象", type = json_treenode_types.content },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijZhidJil, text="带教指导记录", type = json_treenode_types.active }
                     }}
                  }
               });
            }


            if (targetId == DeclareTargetIds.GugJiaos)
            {
               QunLiud.children.Add(new json_treenode()
               {
                  id = DeclareKeys.QunLiud_GugJiaos,
                  text = "骨干教师",
                  type = json_treenode_types.database,
                  children = new List<json_treenode>
                  {
                     new json_treenode() { id = DeclareKeys.QunLiud_GugJiaos_LiurXuex, text="流入学校", type = json_treenode_types.content },
                     new json_treenode() { id = DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj, text="任教年级班级", type = json_treenode_types.content },
                     new json_treenode() { id = DeclareKeys.QunLiud_GugJiaos_Gongkk, text="开设公开课", type = json_treenode_types.active },
                     new json_treenode() { id = DeclareKeys.QunLiud_GugJiaos_TingkZhid, text="开展听课指导", type = json_treenode_types.active },
                     new json_treenode() { id = DeclareKeys.QunLiud_GugJiaos_BeikzHuod, text="主持备课组活动", type = json_treenode_types.active }
                  }
               });
            }
         }


         #endregion


         #region [ 配合教研员工作 ]


         if (model.AllowFitResearcher || targetId == DeclareTargetIds.PutLaos)
         {
            list.Add(new json_treenode { id = DeclareKeys.PeihJiaoyyGongz, text = "配合教研员工作", type = json_treenode_types.database, children = new List<json_treenode>() });
         }

         var PeihJiaoyyGongz = list.Find(m => m.id == DeclareKeys.PeihJiaoyyGongz);

         if (PeihJiaoyyGongz != null)
         {
            PeihJiaoyyGongz.children.Add(new json_treenode() { id = DeclareKeys.PeihJiaoyyGongz_JiaoyXinx, text = "教研信息", type = json_treenode_types.content });
            PeihJiaoyyGongz.children.Add(new json_treenode() { id = DeclareKeys.PeihJiaoyyGongz_XuekJiaoy, text = "学科教研", type = json_treenode_types.active });
            PeihJiaoyyGongz.children.Add(new json_treenode() { id = DeclareKeys.PeihJiaoyyGongz_XuekMingt, text = "学科命题", type = json_treenode_types.active });
            PeihJiaoyyGongz.children.Add(new json_treenode() { id = DeclareKeys.PeihJiaoyyGongz_JicXuexTiaoy, text = "基层学校调研", type = json_treenode_types.active });
         }


         #endregion


         #region [ 年底总结 ]


         list.Add(new json_treenode { id = DeclareKeys.NiandZongj, text = "年底总结", type = json_treenode_types.database, children = new List<json_treenode>() });

         var NiandZongj = list.Find(m => m.id == DeclareKeys.NiandZongj);

         if (NiandZongj != null)
         {
            NiandZongj.children.Add(new json_treenode() { id = DeclareKeys.NiandZongj_Diyn, text = "第一年", type = json_treenode_types.content });
            NiandZongj.children.Add(new json_treenode() { id = DeclareKeys.NiandZongj_Dien, text = "第二年", type = json_treenode_types.content });
            NiandZongj.children.Add(new json_treenode() { id = DeclareKeys.NiandZongj_Disn, text = "第三年", type = json_treenode_types.content });
         }


         #endregion


         #region [ 梯队信息 ]


         if (DeclareTargetIds.HasTeam(targetId))
         {
            list.Add(new json_treenode { id = TeamKeys.FuddTid, text = "辅导的梯队", type = json_treenode_types.database, children = new List<json_treenode>() });

            var Tid = list.Find(m => m.id == TeamKeys.FuddTid);

            if (Tid != null)
            {
               Tid.children.Add(new json_treenode { id = TeamKeys.TidXinx, text = "梯队信息", type = json_treenode_types.content });
               Tid.children.Add(new json_treenode { id = TeamKeys.DaijJih, text = "带教计划", type = json_treenode_types.content });
               Tid.children.Add(new json_treenode { id = TeamKeys.TidChengy, text = "梯队成员", type = json_treenode_types.active });
               Tid.children.Add(new json_treenode { id = TeamKeys.DaijHuod, text = "带教活动", type = json_treenode_types.active });
               if (DeclareTargetIds.AllowKecShis(targetId))
               {
                  Tid.children.Add(new json_treenode { id = TeamKeys.KecShis, text = "定向性课程实施", type = json_treenode_types.active });
               }
               Tid.children.Add(new json_treenode { id = TeamKeys.DaijChengg, text = "带教成果", type = json_treenode_types.active });
            }
         }


         #endregion



         return Json(list, JsonRequestBehavior.AllowGet);
      }


      public ActionResult GetDeclareViewMenu(long userId, long targetId, string typeKey)
      {
         ThrowNotAjax();

         List<json_treenode> list = new List<json_treenode>();

         if (targetId == DeclareTargetIds.GaodLisz || targetId == DeclareTargetIds.JidZhucr)
         {
            list.Add(new json_treenode { id = DeclareKeys.ShenbQingk, text = "申报情况", type = json_treenode_types.database, children = new List<json_treenode>() });
         }
         else if (targetId == DeclareTargetIds.GongzsZhucr || targetId == DeclareTargetIds.XuekDaitr || targetId == DeclareTargetIds.GugJiaos || targetId == DeclareTargetIds.JiaoxNengs || targetId == DeclareTargetIds.JiaoxXinx)
         {
            list.Add(new json_treenode { id = DeclareKeys.ShenbQingk, text = "申报情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ShenbLiy, text = "申报理由", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.GerJibQingk, text = "个人基本情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.SannKaohQingk, text = "校内履职-近三年考核情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ShangyRenTidQingk, text = "市“双名工程”上一轮人才梯队任职情况", type = json_treenode_types.database, children = new List<json_treenode>() });
         }

         if (targetId == DeclareTargetIds.GugJiaos || typeKey == DeclareKeys.GugJiaos_CaiLPog || typeKey == DeclareKeys.XuekDaitr_CaiLPog || typeKey == DeclareKeys.JiaoxNengs_CaiLPog)
         {
            list.Add(new json_treenode { id = DeclareKeys.ZhuanyWeiyHuiQingk, text = "目前在区级及以上专业委员会任职情况（名称、职务）", type = json_treenode_types.database, children = new List<json_treenode>() });
         }
         else if (targetId == DeclareTargetIds.XuekDaitr || targetId == DeclareTargetIds.GongzsZhucr)
         {
            list.Add(new json_treenode { id = DeclareKeys.ZhuanyWeiyHuiQingk, text = "目前在市级及以上专业委员会任职情况（名称、职务）", type = json_treenode_types.database, children = new List<json_treenode>() });
         }

         if (typeKey.IndexOf("材料破格") > 0)
            return Json(list, JsonRequestBehavior.AllowGet);

         if (targetId == DeclareTargetIds.XuekDaitr || targetId == DeclareTargetIds.GongzsZhucr)
         {
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk, text = "近三年 “公开课”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_JiaoxHuod_JiaoyJiaoxPingb, text = "近三年区级及以上“课堂教学评比或其他教育评比”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.PeihJiaoyyGongz_XuekMingt, text = "近三年区级及以上“命题工作”（除幼教、特教外）情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_PingwGongzHuoPbiHuoj, text = "教育教学-评委工作或评比获奖", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.JiaoyJiaox_JiaoyHuod_FahZuoy, text = "近三年在市、区“教研活动发挥作用”（幼教、特教填写）情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_KeyChengg_KetYanj, text = "近三年区级及以上“课题、项目”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_KeyChengg_FabLunw, text = "近三年“发表论文”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_PeixJiangz_JiaosPeixKec, text = "近三年开设校级及以上“教师培训课程”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_PeixJiangz_ZhuantJiangz, text = "近三年校级及以上“学术讲座”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_KeyChengg_LunzQingk, text = "近三年正式出版“论著”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.GerTes_QitShenf, text = "近三年“其他身份”（与申报学科相关）情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.GerTes_XueyChengz, text = "近三年所带“学员成长”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
         }
         else if (targetId == DeclareTargetIds.GugJiaos)
         {
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk, text = "近三年区级及以上“公开课”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_JiaoxHuod_JiaoyJiaoxPingb, text = "近三年校级及以上“课堂教学评比或其他教育评比”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.PeihJiaoyyGongz_XuekMingt, text = "近三年校级及以上“命题工作”（除幼教、特教外）情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_PingwGongzHuoPbiHuoj, text = "近三年校级及以上“评委工作”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.JiaoyJiaox_JiaoyHuod_FahZuoy, text = "近三年在校级及以上“教研活动发挥作用”（幼教、特教填写）情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_KeyChengg_KetYanj, text = "近三年校级及以上“课题、项目”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_KeyChengg_FabLunw, text = "近三年在校级及以上刊物“发表（交流）论文”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_PeixJiangz_JiaosPeixKec, text = "近三年开设或参与校级及以上“教师培训微课程”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_PeixJiangz_ZhuantJiangz, text = "近三年开设校级及以上“专题讲座”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
         }
         else if (targetId == DeclareTargetIds.JiaoxNengs)
         {
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_JiaoxHuod_JiaoyJiaoxPingb, text = "近三年校级及以上“教研、科研、德研比赛获奖”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.Qit_JibGongZshiHuoj, text = "入职3年青年教师“三个一”基本功展示获奖情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.Qit_ZonghxingRongy, text = "近三年校级及以上“其它综合性荣誉”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
         }
         else if (targetId == DeclareTargetIds.JiaoxXinx)
         {
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk, text = "近三年 “公开课”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.ZisFaz_JiaoxHuod_JiaoyJiaoxPingb, text = "近三年校级及以上“教育教学、基本功评比获奖”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.Qit_JianxJiaosPingx, text = "“见习教师规范化培训优秀学员评选”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
            list.Add(new json_treenode { id = DeclareKeys.Qit_JianxJiaosDasHuoj, text = "市、区“见习教师大奖赛获奖”情况", type = json_treenode_types.database, children = new List<json_treenode>() });
         }

         return Json(list, JsonRequestBehavior.AllowGet);
      }


      //	GET: Json/GetDeclare

      public ActionResult GetDeclare(long? userId)
      {
         ThrowNotAjax();


         userId = userId == null ? UserProfile.UserId : userId;

         var model = db.DeclareBaseDal.PrimaryGet(userId.Value);
         var targetId = model.DeclareTargetPKID;


         List<json_treenode> list = new List<json_treenode>();


         #region [ 自身发展 ]


         list.Add(new json_treenode { id = DeclareKeys.ZisFaz, text = "自身发展", type = json_treenode_types.database, children = new List<json_treenode>() });

         var zisFaz = list.Find(m => m.id == DeclareKeys.ZisFaz);

         if (zisFaz != null)
         {
            zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_GerChengj, text = "个人成就", type = json_treenode_types.content });
            zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_GerJianl, text = "个人简历", type = json_treenode_types.content });

            if (targetId != DeclareTargetIds.GaodLisz && targetId != DeclareTargetIds.JidZhucr)
            {
               zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_GerSWOT, text = "个人SWOT分析", type = json_treenode_types.content });
               zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_ZiwFazJih, text = "自我发展计划", type = json_treenode_types.content });
            }

            zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_ZiwYanx, text = "自我研修", type = json_treenode_types.active });
            zisFaz.children.Add(new json_treenode()
            {
               id = DeclareKeys.ZisFaz_JiaoxHuod,
               text = "教学活动",
               type = json_treenode_types.database,
               children = new List<json_treenode>()
               {
                  new json_treenode() { id = DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk, text = "开设教学公开课", type = json_treenode_types.active },
                  new json_treenode() { id = DeclareKeys.ZisFaz_JiaoxHuod_Yantk, text = "开设研讨课", type = json_treenode_types.active },
                  new json_treenode() { id = DeclareKeys.ZisFaz_JiaoxHuod_JiaoxPingb, text = "参加教育教学评比", type = json_treenode_types.active }
               }
            });
            zisFaz.children.Add(new json_treenode()
            {
               id = DeclareKeys.ZisFaz_PeixJiangz,
               text = "培训与讲座",
               type = json_treenode_types.database,
               children = new List<json_treenode>()
               {
                  new json_treenode() { id = DeclareKeys.ZisFaz_PeixJiangz_JiaosPeixKec, text="开设教师培训课程", type = json_treenode_types.active },
                  new json_treenode() { id = DeclareKeys.ZisFaz_PeixJiangz_ZhuantJiangz, text="开设学科类专题讲座", type = json_treenode_types.active }
               }
            });
            if (DeclareTargetIds.AllowKecShis(targetId))
            {
               var ZisFaz_PeixJiangz = zisFaz.children.Find(m => m.id == DeclareKeys.ZisFaz_PeixJiangz);
               ZisFaz_PeixJiangz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_PeixJiangz_DingxxKec, text = "开设定向性课程", type = json_treenode_types.active });
            }
            if (DeclareTargetIds.AllowKecZiy(targetId))
            {
               var ZisFaz_PeixJiangz = zisFaz.children.Find(m => m.id == DeclareKeys.ZisFaz_PeixJiangz);
               ZisFaz_PeixJiangz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_PeixJiangz_KecZiyKaif, text = "课程资源开发", type = json_treenode_types.active });
            }
            if (DeclareTargetIds.AllowXuesHuod(targetId))
            {
               zisFaz.children.Add(new json_treenode()
               {
                  id = DeclareKeys.ZisFaz_XuesHuod,
                  text = "学术活动、特色",
                  type = json_treenode_types.active
               });
            }
            else
            {
               zisFaz.children.Add(new json_treenode()
               {
                  id = DeclareKeys.ZisFaz_XuesHuod,
                  text = "学术活动",
                  type = json_treenode_types.active
               });
            }
            zisFaz.children.Add(new json_treenode()
            {
               id = DeclareKeys.ZisFaz_KeyChengg,
               text = "教育教学科研成果",
               type = json_treenode_types.database,
               children = new List<json_treenode>()
               {
                  new json_treenode() { id = DeclareKeys.ZisFaz_KeyChengg_KetYanj, text="开展课题(项目)研究工作", type = json_treenode_types.active },
                  new json_treenode() { id = DeclareKeys.ZisFaz_KeyChengg_FabLunw, text="论文发表", type = json_treenode_types.active },
                  new json_treenode() { id = DeclareKeys.ZisFaz_KeyChengg_LunzQingk, text="论著情况", type = json_treenode_types.active }
               }
            });
            zisFaz.children.Add(new json_treenode() { id = DeclareKeys.ZisFaz_ShiqjHuod, text = "市、区级大活动", type = json_treenode_types.active });
         }


         #endregion


         #region [ 制度建设 ]


         if (targetId == DeclareTargetIds.GaodLisz || targetId == DeclareTargetIds.JidZhucr || targetId == DeclareTargetIds.GongzsZhucr)
         {
            list.Add(new json_treenode { id = DeclareKeys.ZhidJians, text = "制度建设", type = json_treenode_types.database, children = new List<json_treenode>() });
         }

         var ZhidJians = list.Find(m => m.id == DeclareKeys.ZhidJians);

         if (ZhidJians != null)
         {
            if (targetId != DeclareTargetIds.GongzsZhucr)
            {
               ZhidJians.children.Add(new json_treenode() { id = DeclareKeys.ZhidJians_YingxlDeGongz, text = "有影响力的工作", type = json_treenode_types.active });
            }

            ZhidJians.children.Add(new json_treenode() { id = DeclareKeys.ZhidJians_TesHuodKaiz, text = "特色活动开展", type = json_treenode_types.active });
            ZhidJians.children.Add(new json_treenode() { id = DeclareKeys.ZhidJians_DangaJians, text = "档案建设", type = json_treenode_types.active });
         }


         #endregion


         #region [ 区内流动 ]


         if (model.AllowFlowToSchool)
         {
            list.Add(new json_treenode { id = DeclareKeys.QunLiud, text = "区内流动", type = json_treenode_types.database, children = new List<json_treenode>() });
         }

         var QunLiud = list.Find(m => m.id == DeclareKeys.QunLiud);

         if (QunLiud != null)
         {
            if (targetId == DeclareTargetIds.XuekDaitr)
            {
               QunLiud.children.Add(new json_treenode()
               {
                  id = DeclareKeys.QunLiud_XuekDaitr,
                  text = "学科带头人",
                  type = json_treenode_types.database,
                  children = new List<json_treenode>
                  {
                     new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_LiurXuex, text="流入学校", type = json_treenode_types.content },
                     new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_KetJiaox, text="课堂教学", type = json_treenode_types.database, children = new List<json_treenode>
                     {
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_Gongkk, text="上公开课", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_GongkHuibk, text="公开汇报课", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_Suitk, text="接受教师听随堂课", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_TingKZhid, text="开展听课指导", type = json_treenode_types.active },
                     }},
                     new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey, text="教育科研", type = json_treenode_types.database, children = new List<json_treenode>
                     {
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz, text="开设专题讲座", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod, text="主持教研活动", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_CanyHuod, text="参与教研组、备课组活动", type = json_treenode_types.active },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_JiedxZongj, text="阶段性总结", type = json_treenode_types.content },
                     }},
                     new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_DaijPeix, text="带教培训", type = json_treenode_types.database, children = new List<json_treenode>
                     {
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijDuix, text="带教对象", type = json_treenode_types.content },
                        new json_treenode() { id = DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijZhidJil, text="带教指导记录", type = json_treenode_types.active }
                     }}
                  }
               });
            }


            if (targetId == DeclareTargetIds.GugJiaos)
            {
               QunLiud.children.Add(new json_treenode()
               {
                  id = DeclareKeys.QunLiud_GugJiaos,
                  text = "骨干教师",
                  type = json_treenode_types.database,
                  children = new List<json_treenode>
                  {
                     new json_treenode() { id = DeclareKeys.QunLiud_GugJiaos_LiurXuex, text="流入学校", type = json_treenode_types.content },
                     new json_treenode() { id = DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj, text="任教年级班级", type = json_treenode_types.content },
                     new json_treenode() { id = DeclareKeys.QunLiud_GugJiaos_Gongkk, text="开设公开课", type = json_treenode_types.active },
                     new json_treenode() { id = DeclareKeys.QunLiud_GugJiaos_TingkZhid, text="开展听课指导", type = json_treenode_types.active },
                     new json_treenode() { id = DeclareKeys.QunLiud_GugJiaos_BeikzHuod, text="主持备课组活动", type = json_treenode_types.active }
                  }
               });
            }
         }


         #endregion


         #region [ 配合教研员工作 ]


         if (model.AllowFitResearcher)
         {
            list.Add(new json_treenode { id = DeclareKeys.PeihJiaoyyGongz, text = "配合教研员工作", type = json_treenode_types.database, children = new List<json_treenode>() });
         }

         var PeihJiaoyyGongz = list.Find(m => m.id == DeclareKeys.PeihJiaoyyGongz);

         if (PeihJiaoyyGongz != null)
         {
            PeihJiaoyyGongz.children.Add(new json_treenode() { id = DeclareKeys.PeihJiaoyyGongz_JiaoyXinx, text = "教研信息", type = json_treenode_types.content });
            PeihJiaoyyGongz.children.Add(new json_treenode() { id = DeclareKeys.PeihJiaoyyGongz_XuekJiaoy, text = "学科教研", type = json_treenode_types.active });
            PeihJiaoyyGongz.children.Add(new json_treenode() { id = DeclareKeys.PeihJiaoyyGongz_XuekMingt, text = "学科命题", type = json_treenode_types.active });
            PeihJiaoyyGongz.children.Add(new json_treenode() { id = DeclareKeys.PeihJiaoyyGongz_JicXuexTiaoy, text = "基层学校调研", type = json_treenode_types.active });
         }


         #endregion


         #region [ 年底总结 ]


         list.Add(new json_treenode { id = DeclareKeys.NiandZongj, text = "年底总结", type = json_treenode_types.database, children = new List<json_treenode>() });

         var NiandZongj = list.Find(m => m.id == DeclareKeys.NiandZongj);

         if (NiandZongj != null)
         {
            NiandZongj.children.Add(new json_treenode() { id = DeclareKeys.NiandZongj_Diyn, text = "第一年", type = json_treenode_types.content });
            NiandZongj.children.Add(new json_treenode() { id = DeclareKeys.NiandZongj_Dien, text = "第二年", type = json_treenode_types.content });
            NiandZongj.children.Add(new json_treenode() { id = DeclareKeys.NiandZongj_Disn, text = "第三年", type = json_treenode_types.content });
         }


         #endregion


         return Json(list, JsonRequestBehavior.AllowGet);
      }


      //	GET: Json/GetDecalreMaterial

      public ActionResult GetDecalreMaterial(long? userId)
      {
         ThrowNotAjax();

         var dr = APDBDef.DeclareReview;

         List<json_treenode> list = new List<json_treenode>();

         var gaodLisZhang = new json_treenode { id = DeclareKeys.GaodLisz_ZijBiao, text = "学科高地理事长", type = json_treenode_types.database, children = new List<json_treenode>() };
         var jiDZhucReng = new json_treenode { id = DeclareKeys.JidZhucr_ZijBiao, text = "学科培训基地主持人", type = json_treenode_types.database, children = new List<json_treenode>() };
         var gongzShiZhucReng = new json_treenode { id = DeclareKeys.GongzsZhucr_Shenb, text = "学科培训工作室主持人", type = json_treenode_types.database, children = new List<json_treenode>() };
         var xuekDaitReng = new json_treenode { id = DeclareKeys.XuekDaitr, text = "学科带头人", type = json_treenode_types.database, children = new List<json_treenode>() };
         xuekDaitReng.children.Add(new json_treenode { id = DeclareKeys.XuekDaitr_Shenb, text = "申报" });
         xuekDaitReng.children.Add(new json_treenode { id = DeclareKeys.XuekDaitr_ZhicPog, text = "职称破格" });
         xuekDaitReng.children.Add(new json_treenode { id = string.Format("{0}-{1}", DeclareKeys.XuekDaitr_CaiLPog, DeclareTargetIds.XuekDaitr), text = "材料破格" });
         var gugJiaos = new json_treenode { id = DeclareKeys.GugJiaos, text = "骨干教师", type = json_treenode_types.database, children = new List<json_treenode>() };
         gugJiaos.children.Add(new json_treenode { id = DeclareKeys.GugJiaos_Shenb, text = "申报" });
         gugJiaos.children.Add(new json_treenode { id = DeclareKeys.GugJiaos_ZhicPog, text = "职称破格" });
         gugJiaos.children.Add(new json_treenode { id = string.Format("{0}-{1}", DeclareKeys.GugJiaos_CaiLPog, DeclareTargetIds.GugJiaos), text = "材料破格" });
         var jiaoxNengs = new json_treenode { id = DeclareKeys.JiaoxNengs, text = "教学能手", type = json_treenode_types.database, children = new List<json_treenode>() };
         jiaoxNengs.children.Add(new json_treenode { id = DeclareKeys.JiaoxNengs_Shenb, text = "申报" });
         jiaoxNengs.children.Add(new json_treenode { id = DeclareKeys.JiaoxNengs_ZhicPog, text = "职称破格" });
         jiaoxNengs.children.Add(new json_treenode { id = string.Format("{0}-{1}", DeclareKeys.JiaoxNengs_CaiLPog, DeclareTargetIds.JiaoxNengs), text = "材料破格" });
         var jiaoxXinx = new json_treenode { id = DeclareKeys.JiaoxXinx, text = "教学新秀", type = json_treenode_types.database, children = new List<json_treenode>() };


         string[] submitStatus = { DeclareKeys.ReviewProcess, DeclareKeys.ReviewSuccess, DeclareKeys.ReviewFailure };
         var declareForm = db.DeclareReviewDal
            .ConditionQuery(dr.TeacherId == userId & dr.PeriodId == Period.PeriodId & dr.StatusKey.In(submitStatus), null, null, null)
            .FirstOrDefault();

         if (declareForm != null)
         {
            list.Add(new json_treenode { id = declareForm.DeclareReviewId.ToString(), text = "我的申报", type = json_treenode_types.database });
         }
         else
         {
            list.Add(gaodLisZhang);
            list.Add(jiDZhucReng);
            list.Add(gongzShiZhucReng);
            list.Add(xuekDaitReng);
            list.Add(gugJiaos);
            list.Add(jiaoxNengs);
            list.Add(jiaoxXinx);
         }

         return Json(list, JsonRequestBehavior.AllowGet);
      }


      //	GET: Json/GetTeamMaster

      public ActionResult GetTeamMaster()
      {
         ThrowNotAjax();

         var model = db.DeclareBaseDal.PrimaryGet(UserProfile.UserId);
         var targetId = model.DeclareTargetPKID;

         List<json_treenode> list = new List<json_treenode>();

         if (DeclareTargetIds.HasTeam(targetId))
         {
            list.Add(new json_treenode { id = TeamKeys.TidXinx, text = "梯队信息", type = json_treenode_types.content });
            list.Add(new json_treenode { id = TeamKeys.DaijJih, text = "带教计划", type = json_treenode_types.content });
            list.Add(new json_treenode { id = TeamKeys.DaijHuod, text = "带教活动", type = json_treenode_types.active });
            if (DeclareTargetIds.AllowKecShis(targetId))
            {
               list.Add(new json_treenode { id = TeamKeys.KecShis, text = "定向性课程实施", type = json_treenode_types.active });
            }
         }


         return Json(list, JsonRequestBehavior.AllowGet);
      }


      //	GET: Json/GetTeamMember

      public ActionResult GetTeamMember()
      {
         ThrowNotAjax();


         var targetId = APQuery.select(d.DeclareTargetPKID)
            .from(tm, d.JoinInner(tm.TeamId == d.TeacherId))
            .where(tm.MemberId == UserProfile.UserId)
            .query(db, r => d.DeclareTargetPKID.GetValue(r))
            .First();


         List<json_treenode> list = new List<json_treenode>();

         if (targetId > 0)
         {
            list.Add(new json_treenode { id = TeamKeys.TidXinx, text = "梯队信息", type = json_treenode_types.content });
            list.Add(new json_treenode { id = TeamKeys.DaijJih, text = "带教计划", type = json_treenode_types.content });
            list.Add(new json_treenode { id = TeamKeys.DaijHuod, text = "带教活动", type = json_treenode_types.active });
            if (DeclareTargetIds.AllowKecShis(targetId))
            {
               list.Add(new json_treenode { id = TeamKeys.KecShis, text = "定向性课程实施", type = json_treenode_types.active });
            }
         }

         return Json(list, JsonRequestBehavior.AllowGet);
      }


      // GET: Json/GetExpert

      public ActionResult GetExpert()
      {
         ThrowNotAjax();


         var list = new List<json_treenode>
         {
            new json_treenode { id=ExpertKeys.SuosZhuanjzXinx,text="所属专家组信息", type=json_treenode_types.content },
				//new json_treenode { },
				//new json_treenode { }
			};

         return Json(list, JsonRequestBehavior.AllowGet);
      }

   }

}