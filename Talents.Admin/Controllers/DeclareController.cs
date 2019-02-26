using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

   // 用户填写申报信息页面

   public class DeclareController : BaseController
   {

      private static APDBDef.DeclareContentTableDef tc = APDBDef.DeclareContent;
      private static APDBDef.DeclareActiveTableDef da = APDBDef.DeclareActive;
      static APDBDef.DeclareAchievementTableDef dac = APDBDef.DeclareAchievement;
      static APDBDef.DeclareBaseTableDef dcl = APDBDef.DeclareBase;
      private static APDBDef.DeclareAchievementTableDef tat = APDBDef.DeclareAchievement;
      private static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
      private static APDBDef.TeamActiveTableDef t = APDBDef.TeamActive;
      private static APDBDef.TeamActiveItemTableDef tai = APDBDef.TeamActiveItem;
      private static APDBDef.TeamActiveResultTableDef tar = APDBDef.TeamActiveResult;
      private static APDBDef.TeamSpecialCourseTableDef tsc = APDBDef.TeamSpecialCourse;
      private static APDBDef.TeamMemberTableDef tm = APDBDef.TeamMember;
      private static APDBDef.DeclareMaterialTableDef dm = APDBDef.DeclareMaterial;

      // GET: Declare/Index

      public ActionResult Index()
      {
         return View();
      }


      // GET: Declare/Fragment

      public ActionResult Fragment(string key)
      {
         switch (key)
         {
            case DeclareKeys.ZisFaz_GerChengj:
               return ZisFaz_GerChengj();
            case DeclareKeys.ZisFaz_GerJianl:
               return ZisFaz_GerJianl();
            case DeclareKeys.ZisFaz_ZiwYanx:
               return ZisFaz_ZiwYanx();
            case DeclareKeys.ZisFaz_GerSWOT:
               return ZisFaz_GerSWTOFenx();
            case DeclareKeys.ZisFaz_ZiwFazJih:
               return ZisFaz_ZiwFazJih();
            case DeclareKeys.ZisFaz_ShiqjHuod:
               return ZisFaz_ShiqjHuod();
            case DeclareKeys.ZisFaz_JiaoxHuod:
               return ZisFaz_JiaoxHuod();
            case DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk:
               return ZisFaz_JiaoxHuod_JiaoxGongkk();
            case DeclareKeys.ZisFaz_JiaoxHuod_Yantk:
               return ZisFaz_JiaoxHuod_Yantk();
            case DeclareKeys.ZisFaz_JiaoxHuod_JiaoxPingb:
               return ZisFaz_JiaoxHuod_JiaoxPingb();
            case DeclareKeys.ZisFaz_PeixJiangz:
               return ZisFaz_PeixJiangz();
            case DeclareKeys.ZisFaz_PeixJiangz_JiaosPeixKec:
               return ZisFaz_PeixJiangz_JiaosPeixKec();
            case DeclareKeys.ZisFaz_PeixJiangz_ZhuantJiangz:
               return ZisFaz_PeixJiangz_ZhuantJiangz();
            case DeclareKeys.ZisFaz_PeixJiangz_DingxxKec:
               return ZisFaz_PeixJiangz_DingxxKec();
            case DeclareKeys.ZisFaz_PeixJiangz_KecZiyKaif:
               return ZisFaz_PeixJiangz_KecZiyKaif();
            case DeclareKeys.ZisFaz_XuesHuod:
               return ZisFaz_XuesHuod();
            case DeclareKeys.ZisFaz_KeyChengg:
               return ZisFaz_KeyChengg();
            case DeclareKeys.ZisFaz_KeyChengg_KetYanj:
               return ZisFaz_KeyChengg_KetYanj();
            case DeclareKeys.ZisFaz_KeyChengg_FabLunw:
               return ZisFaz_KeyChengg_FabLunw();
            case DeclareKeys.ZisFaz_KeyChengg_LunzQingk:
               return ZisFaz_KeyChengg_LunzQingk();
            case DeclareKeys.ZhidJians_YingxlDeGongz:
               return ZhidJians_YingxlDeGongz();
            case DeclareKeys.ZhidJians_TesHuodKaiz:
               return ZhidJians_TesHuodKaiz();
            case DeclareKeys.ZhidJians_DangaJians:
               return ZhidJians_DangaJians();
            case DeclareKeys.QunLiud_XuekDaitr_LiurXuex:
               return QunLiud_XuekDaitr_LiurXuex();
            case DeclareKeys.QunLiud_XuekDaitr_KetJiaox_Gongkk:
               return QunLiud_XuekDaitr_KetJiaox_Gongkk();
            case DeclareKeys.QunLiud_XuekDaitr_KetJiaox_GongkHuibk:
               return QunLiud_XuekDaitr_KetJiaox_GongkHuibk();
            case DeclareKeys.QunLiud_XuekDaitr_KetJiaox_Suitk:
               return QunLiud_XuekDaitr_KetJiaox_Suitk();
            case DeclareKeys.QunLiud_XuekDaitr_KetJiaox_TingKZhid:
               return QunLiud_XuekDaitr_KetJiaox_TingKZhid();
            case DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz:
               return QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz();
            case DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod:
               return QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod();
            case DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_CanyHuod:
               return QunLiud_XuekDaitr_JiaoyKey_CanyHuod();
            case DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_JiedxZongj:
               return QunLiud_XuekDaitr_JiaoyKey_JiedxZongj();
            case DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijDuix:
               return QunLiud_XuekDaitr_DaijPeix_DaijDuix();
            case DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijZhidJil:
               return QunLiud_XuekDaitr_DaijPeix_DaijZhidJil();
            case DeclareKeys.QunLiud_GugJiaos_LiurXuex:
               return QunLiud_GugJiaos_LiurXuex();
            case DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj:
               return QunLiud_GugJiaos_RenjNianjBanj();
            case DeclareKeys.QunLiud_GugJiaos_Gongkk:
               return QunLiud_GugJiaos_Gongkk();
            case DeclareKeys.QunLiud_GugJiaos_TingkZhid:
               return QunLiud_GugJiaos_TingkZhid();
            case DeclareKeys.QunLiud_GugJiaos_BeikzHuod:
               return QunLiud_GugJiaos_BeikzHuod();
            case DeclareKeys.PeihJiaoyyGongz_JiaoyXinx:
               return PeihJiaoyyGongz_JiaoyXinx();
            case DeclareKeys.PeihJiaoyyGongz_XuekJiaoy:
               return PeihJiaoyyGongz_XuekJiaoy();
            case DeclareKeys.PeihJiaoyyGongz_XuekMingt:
               return PeihJiaoyyGongz_XuekMingt();
            case DeclareKeys.PeihJiaoyyGongz_JicXuexTiaoy:
               return PeihJiaoyyGongz_JicXuexTiaoy();
            case DeclareKeys.NiandZongj_Diyn:
            case DeclareKeys.NiandZongj_Disn:
            case DeclareKeys.NiandZongj_Dien:
               return NiandZongj(key);
         }

         return PartialView("PromptInfo");
      }


      // POST-Ajax: Declare/RemoveActive

      [HttpPost]
      public ActionResult RemoveActive(long id, string type)
      {
         ThrowNotAjax();

         var deletedActive = db.DeclareActiveDal.PrimaryGet(id);
         var period = db.GetCurrentDeclarePeriod();

         db.BeginTrans();

         try
         {
            db.DeclareActiveDal.PrimaryDelete(id);
            AttachmentsExtensions.DeleteAtta(db, id, type);

            if (period != null && deletedActive.IsDeclare)
               db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);

            db.Commit();


            LogFactory.Create().Log(new LogModel
            {
               UserID = UserProfile.UserId,
               OperationDate = DateTime.Now,
               Where = deletedActive.ActiveKey,
               DoSomthing = string.Format("删除了ID为{0} 的decalre active 项", id)
            });
         }
         catch (Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }


         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已删除！"
         });
      }


      // POST-Ajax: Declare/RemoveAchievement

      [HttpPost]
      public ActionResult RemoveAchievement(long id, string type)
      {
         ThrowNotAjax();

         var period = db.GetCurrentDeclarePeriod();

         db.BeginTrans();

         try
         {
            db.DeclareAchievementDal.PrimaryDelete(id);

            AttachmentsExtensions.DeleteAtta(db, id, type);

            if (period != null)
               db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);

            db.Commit();
         }
         catch (Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }


         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已删除！"
         });
      }


      // POST-Ajax: Declare/RemoveOrgConst

      [HttpPost]
      public ActionResult RemoveOrgConst(long id)
      {
         ThrowNotAjax();

         var period = db.GetCurrentDeclarePeriod();

         db.DeclareOrgConstDal.PrimaryDelete(id);

         if (period != null)
            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已删除！"
         });
      }


      // POST-Ajax: Declare/ShareActive

      [HttpPost]
      public ActionResult ShareActive(long id, bool isShare)
      {
         ThrowNotAjax();

         var s = APDBDef.Share;

         var active = db.DeclareActiveDal.PrimaryGet(id);
         active.IsShare = isShare;

         if (active != null)
         {
            var hasAttachment = AttachmentsExtensions.HasAttachment(db, id, UserProfile.UserId);
            if (!hasAttachment && isShare)
            {
               return Json(new
               {
                  result = AjaxResults.Success,
                  msg = "必须上传附件！"
               });
            }


            db.BeginTrans();

            try
            {
               db.DeclareActiveDal.UpdatePartial(id, new { IsShare = active.IsShare });

               if (active.IsShare)
                  db.ShareDal.Insert(new Share
                  {
                     ItemId = id,
                     ParentType = "ShareActive",
                     CreateDate = DateTime.Now,
                     PubishDate = DateTime.Now,
                     Title = active.ContentValue,
                     Type = active.ActiveKey,
                     UserId = UserProfile.UserId
                  });
               else
                  db.ShareDal.ConditionDelete(s.ItemId == id);


               db.Commit();
            }
            catch (Exception e)
            {
               db.Rollback();

               return Json(new
               {
                  result = AjaxResults.Error,
                  msg = "共享失败！"
               });
            }

            return Json(new
            {
               result = AjaxResults.Success,
               msg = "操作成功！"
            });
         }


         return Json(new
         {
            result = AjaxResults.Success,
            msg = "没有可以共享的活动！"
         });

      }


      // POST-Ajax: Declare/ShareAchievement

      [HttpPost]
      public ActionResult ShareAchievement(long id, bool isShare)
      {
         ThrowNotAjax();

         var s = APDBDef.Share;

         var achievement = db.DeclareAchievementDal.PrimaryGet(id);
         achievement.IsShare = isShare;

         if (achievement != null)
         {
            var hasAttachment = AttachmentsExtensions.HasAttachment(db, id, UserProfile.UserId);
            if (!hasAttachment && isShare)
            {
               return Json(new
               {
                  result = AjaxResults.Success,
                  msg = "必须上传附件！"
               });
            }


            db.BeginTrans();

            try
            {
               db.DeclareAchievementDal.UpdatePartial(id, new { IsShare = achievement.IsShare });

               if (achievement.IsShare)
                  db.ShareDal.Insert(new Share
                  {
                     ItemId = id,
                     ParentType = "ShareAchievement",
                     CreateDate = DateTime.Now,
                     PubishDate = DateTime.Now,
                     Title = achievement.NameOrTitle,
                     Type = achievement.AchievementKey,
                     UserId = UserProfile.UserId
                  });
               else
                  db.ShareDal.ConditionDelete(s.ItemId == id);


               db.Commit();
            }
            catch
            {
               db.Rollback();

               return Json(new
               {
                  result = AjaxResults.Error,
                  msg = "操作失败！"
               });
            }

            return Json(new
            {
               result = AjaxResults.Success,
               msg = "操作成功！"
            });
         }


         return Json(new
         {
            result = AjaxResults.Success,
            msg = "没有可以共享的成果！"
         });
      }


      #region [ 自身发展.个人成就 ]

      //	GET-Ajax:			Declare/ZisFaz_GerChengj
      //	POST-Ajax:			Declare/ZisFaz_GerChengj

      private ActionResult ZisFaz_GerChengj()
      {
         var list = QueryDeclareContent(DeclareKeys.ZisFaz_GerChengj);
         ZisFaz_GerChengjModel model = new ZisFaz_GerChengjModel();

         list.ForEach(m =>
         {
            if (m.ContentKey == DeclareKeys.ZisFaz_GerChengj_Memo1)
            {
               model.Memo1 = m.ContentValue;
               model.IsDeclare1 = m.IsDeclare;
            }
            else if (m.ContentKey == DeclareKeys.ZisFaz_GerChengj_Memo2)
            {
               model.Memo2 = m.ContentValue;
               model.IsDeclare2 = m.IsDeclare;
            }
            else if (m.ContentKey == DeclareKeys.ZisFaz_GerChengj_Memo3)
            {
               model.Memo3 = m.ContentValue;
               model.IsDeclare3 = m.IsDeclare;
            }
         });

         return PartialView("ZisFaz_GerChengj", model);
      }

      [HttpPost]
      public ActionResult ZisFaz_GerChengj(ZisFaz_GerChengjModel model)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            SetDeclareContent(DeclareKeys.ZisFaz_GerChengj_Memo1, model.Memo1, model.IsDeclare1);
            SetDeclareContent(DeclareKeys.ZisFaz_GerChengj_Memo2, model.Memo2, model.IsDeclare2);
            SetDeclareContent(DeclareKeys.ZisFaz_GerChengj_Memo3, model.Memo3, model.IsDeclare3);

            db.Commit();
         }
         catch (System.Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 自身发展.个人简历 ]


      //	GET-Ajax:			Declare/ZisFaz_GerChengj
      //	删除
      //	POST-Ajax:			Declare/RemoveResume

      public ActionResult ZisFaz_GerJianl()
      {
         var t = APDBDef.DeclareResume;
         var model = db.DeclareResumeDal.ConditionQuery(t.TeacherId == UserProfile.UserId, null, null, null);

         return PartialView("ZisFaz_GerJianl", model);
      }

      [HttpPost]
      public ActionResult RemoveResume(long id)
      {
         ThrowNotAjax();

         db.DeclareResumeDal.PrimaryDelete(id);

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已删除！"
         });
      }


      #endregion


      #region [ 自身发展.个人SWOT分析 ]


      //	GET-Ajax:			Declare/ZisFaz_GerSWTOFenx
      //	POST-Ajax:			Declare/ZisFaz_GerSWTOFenx

      private ActionResult ZisFaz_GerSWTOFenx()
      {
         var list = QueryDeclareContent(DeclareKeys.ZisFaz_GerSWOT);
         ZisFaz_GerSWOTModel model = new ZisFaz_GerSWOTModel();

         list.ForEach(m =>
         {
            switch (m.ContentKey)
            {
               case DeclareKeys.ZisFaz_GerSWOT_Goodness1:
                  model.Goodness1 = m.ContentValue;
                  model.IsDeclare1 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Goodness2:
                  model.Goodness2 = m.ContentValue;
                  model.IsDeclare2 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Goodness3:
                  model.Goodness3 = m.ContentValue;
                  model.IsDeclare3 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Goodness4:
                  model.Goodness4 = m.ContentValue;
                  model.IsDeclare4 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Weakness1:
                  model.Weakness1 = m.ContentValue;
                  model.IsDeclare5 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Weakness2:
                  model.Weakness2 = m.ContentValue;
                  model.IsDeclare6 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Weakness3:
                  model.Weakness3 = m.ContentValue;
                  model.IsDeclare7 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Weakness4:
                  model.Weakness4 = m.ContentValue;
                  model.IsDeclare8 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Opportunity1:
                  model.Opportunity1 = m.ContentValue;
                  model.IsDeclare9 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Opportunity2:
                  model.Opportunity2 = m.ContentValue;
                  model.IsDeclare10 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Opportunity3:
                  model.Opportunity3 = m.ContentValue;
                  model.IsDeclare11 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Challenge1:
                  model.Challenge1 = m.ContentValue;
                  model.IsDeclare12 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Challenge2:
                  model.Challenge2 = m.ContentValue;
                  model.IsDeclare13 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_Challenge3:
                  model.Challenge3 = m.ContentValue;
                  model.IsDeclare14 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_GerSWOT_GaijCuos:
                  model.GaijCuos = m.ContentValue;
                  model.IsDeclare15 = m.IsDeclare;
                  break;
            }
         });

         return PartialView("ZisFaz_GerSWOT", model);
      }

      [HttpPost]
      public ActionResult ZisFaz_GerSWOT(ZisFaz_GerSWOTModel model)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Goodness1, model.Goodness1, model.IsDeclare1);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Goodness2, model.Goodness2, model.IsDeclare2);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Goodness3, model.Goodness3, model.IsDeclare3);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Goodness4, model.Goodness4, model.IsDeclare4);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Weakness1, model.Weakness1, model.IsDeclare5);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Weakness2, model.Weakness2, model.IsDeclare6);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Weakness3, model.Weakness3, model.IsDeclare7);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Weakness4, model.Weakness4, model.IsDeclare8);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Opportunity1, model.Opportunity1, model.IsDeclare9);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Opportunity2, model.Opportunity2, model.IsDeclare10);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Opportunity3, model.Opportunity3, model.IsDeclare11);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Challenge1, model.Challenge1, model.IsDeclare12);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Challenge2, model.Challenge2, model.IsDeclare13);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_Challenge3, model.Challenge3, model.IsDeclare14);
            SetDeclareContent(DeclareKeys.ZisFaz_GerSWOT_GaijCuos, model.GaijCuos, model.IsDeclare15);

            db.Commit();
         }
         catch (System.Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 自身发展.自我发展计划 ]


      //	GET-Ajax:			Declare/ZisFaz_ZiwFazJih
      //	POST-Ajax:			Declare/ZisFaz_ZiwFazJih

      private ActionResult ZisFaz_ZiwFazJih()
      {
         var list = QueryDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih);
         ZisFaz_ZiwFazJihModel model = new ZisFaz_ZiwFazJihModel();

         list.ForEach(m =>
         {
            switch (m.ContentKey)
            {
               case DeclareKeys.ZisFaz_ZiwFazJih_ZhuanyeFazMub_Memo1:
                  model.ZhuanyeFazMub_Memo1 = m.ContentValue;
                  model.IsDeclare1 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_ZiwFazJih_JiedMub_Memo1:
                  model.JiedMub_Memo1 = m.ContentValue;
                  model.IsDeclare2 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_ZiwFazJih_JiedMub_Memo2:
                  model.JiedMub_Memo2 = m.ContentValue;
                  model.IsDeclare3 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_ZiwFazJih_JiedMub_Memo3:
                  model.JiedMub_Memo3 = m.ContentValue;
                  model.IsDeclare4 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_ZiwFazJih_JutJih_ZhuanyNenglFangm_Memo1:
                  model.ZhuanyNenglFangm_Memo1 = m.ContentValue;
                  model.IsDeclare5 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_ZiwFazJih_JutJih_ZhuanyNenglFangm_Memo2:
                  model.ZhuanyNenglFangm_Memo2 = m.ContentValue;
                  model.IsDeclare6 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_ZiwFazJih_JutJih_ZhuanyNenglFangm_Memo3:
                  model.ZhuanyNenglFangm_Memo3 = m.ContentValue;
                  model.IsDeclare7 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_ZiwFazJih_JutJih_ZhuanyNenglFangm_Memo4:
                  model.ZhuanyNenglFangm_Memo4 = m.ContentValue;
                  model.IsDeclare8 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_ZiwFazJih_JutJih_XuyFangm_Memo1:
                  model.XuyFangm_Memo1 = m.ContentValue;
                  model.IsDeclare9 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_ZiwFazJih_JutJih_XuyFangm_Memo2:
                  model.XuyFangm_Memo2 = m.ContentValue;
                  model.IsDeclare10 = m.IsDeclare;
                  break;
               case DeclareKeys.ZisFaz_ZiwFazJih_JutJih_XuyFangm_Memo3:
                  model.XuyFangm_Memo3 = m.ContentValue;
                  model.IsDeclare11 = m.IsDeclare;
                  break;
            }
         });

         return PartialView("ZisFaz_ZiwFazJih", model);
      }

      [HttpPost]
      public ActionResult ZisFaz_ZiwFazJih(ZisFaz_ZiwFazJihModel model)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            SetDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih_ZhuanyeFazMub_Memo1, model.ZhuanyeFazMub_Memo1, model.IsDeclare1);
            SetDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih_JiedMub_Memo1, model.JiedMub_Memo1, model.IsDeclare2);
            SetDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih_JiedMub_Memo2, model.JiedMub_Memo2, model.IsDeclare3);
            SetDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih_JiedMub_Memo3, model.JiedMub_Memo3, model.IsDeclare4);
            SetDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih_JutJih_ZhuanyNenglFangm_Memo1, model.ZhuanyNenglFangm_Memo1, model.IsDeclare5);
            SetDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih_JutJih_ZhuanyNenglFangm_Memo2, model.ZhuanyNenglFangm_Memo2, model.IsDeclare6);
            SetDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih_JutJih_ZhuanyNenglFangm_Memo3, model.ZhuanyNenglFangm_Memo3, model.IsDeclare7);
            SetDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih_JutJih_ZhuanyNenglFangm_Memo4, model.ZhuanyNenglFangm_Memo4, model.IsDeclare8);
            SetDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih_JutJih_XuyFangm_Memo1, model.XuyFangm_Memo1, model.IsDeclare9);
            SetDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih_JutJih_XuyFangm_Memo2, model.XuyFangm_Memo2, model.IsDeclare10);
            SetDeclareContent(DeclareKeys.ZisFaz_ZiwFazJih_JutJih_XuyFangm_Memo3, model.XuyFangm_Memo3, model.IsDeclare11);

            db.Commit();
         }
         catch (System.Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 自身发展.自我研修 ]


      //	GET-Ajax:			Declare/ZisFaz_ZiwYanx

      private ActionResult ZisFaz_ZiwYanx()
      {
         var list = QueryDeclareActive(DeclareKeys.ZisFaz_ZiwYanx);
         List<ZisFaz_ZiwYanxModel> model = new List<ZisFaz_ZiwYanxModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_ZiwYanxModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZisFaz_ZiwYanx", model);
      }


      #endregion


      #region [ 自身发展.教学活动 ]

      //	GET-Ajax: Declare/ZisFaz_JiaoxHuod

      public ActionResult ZisFaz_JiaoxHuod()
      {
         var list = QueryDeclareActiveList(DeclareKeys.ZisFaz_JiaoxHuod);

         return PartialView("ZisFaz_JiaoxHuod", list);
      }

      #endregion


      #region [ 自身发展.教学活动.开设教学公开课 ]


      //	GET-Ajax: Declare/ZisFaz_JiaoxHuod_JiaoxGongkk

      private ActionResult ZisFaz_JiaoxHuod_JiaoxGongkk()
      {
         var list = QueryDeclareActive(DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk);
         List<ZisFaz_JiaoxHuod_JiaoxGongkkModel> model = new List<ZisFaz_JiaoxHuod_JiaoxGongkkModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_JiaoxHuod_JiaoxGongkkModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Level = m.Level,
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2,
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });


         return PartialView("ZisFaz_JiaoxHuod_JiaoxGongkk", model);
      }


      #endregion


      #region [ 自身发展.教学活动.开设研讨课 ]


      //	GET-Ajax:			Declare/ZisFaz_JiaoxHuod_Yantk

      private ActionResult ZisFaz_JiaoxHuod_Yantk()
      {
         var list = QueryDeclareActive(DeclareKeys.ZisFaz_JiaoxHuod_Yantk);
         List<ZisFaz_JiaoxHuod_YantkModel> model = new List<ZisFaz_JiaoxHuod_YantkModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_JiaoxHuod_YantkModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Level = m.Level,
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2,
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZisFaz_JiaoxHuod_Yantk", model);
      }


      #endregion


      #region [ 自身发展.教学活动.参加教学评比 ]


      //	GET-Ajax:			Declare/ZisFaz_JiaoxHuod_JiaoxPingb

      private ActionResult ZisFaz_JiaoxHuod_JiaoxPingb()
      {
         var list = QueryDeclareActive(DeclareKeys.ZisFaz_JiaoxHuod_JiaoxPingb);
         List<ZisFaz_JiaoxHuod_JiaoxPingbModel> model = new List<ZisFaz_JiaoxHuod_JiaoxPingbModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_JiaoxHuod_JiaoxPingbModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic2 = m.Dynamic2,
               Dynamic1 = m.Dynamic1,
               IsShare = m.IsShare,
               Level = m.Level,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZisFaz_JiaoxHuod_JiaoxPingb", model);
      }


      #endregion


      #region [ 自身发展.培训与讲座 ]


      public ActionResult ZisFaz_PeixJiangz()
      {
         var list = QueryDeclareActiveList(DeclareKeys.ZisFaz_PeixJiangz);

         ViewBag.Approve = db.DeclareBaseDal.PrimaryGet(UserProfile.UserId).DeclareTargetPKID > DeclareTargetIds.GongzsZhucr;

         ViewBag.Approve1 = DeclareTargetIds.AllowKecZiy(db.DeclareBaseDal.PrimaryGet(UserProfile.UserId).DeclareTargetPKID);

         return PartialView("ZisFaz_PeixJiangz", list);
      }


      #endregion


      #region [ 自身发展.培训与讲座.开设教师培训课程 ]


      //	GET-Ajax:			Declare/ZisFaz_PeixJiangz_JiaosPeixKec

      private ActionResult ZisFaz_PeixJiangz_JiaosPeixKec()
      {
         var list = QueryDeclareActive(DeclareKeys.ZisFaz_PeixJiangz_JiaosPeixKec);
         List<ZisFaz_PeixJiangzModel> model = new List<ZisFaz_PeixJiangzModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_PeixJiangzModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               ContentValue = m.ContentValue,
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2,
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZisFaz_PeixJiangz_JiaosPeixKec", model);
      }


      #endregion


      #region [ 自身发展.培训与讲座.开设学科类专题讲座 ]


      //	GET-Ajax:			Declare/ZisFaz_PeixJiangz_ZhuantJiangz

      private ActionResult ZisFaz_PeixJiangz_ZhuantJiangz()
      {
         var list = QueryDeclareActive(DeclareKeys.ZisFaz_PeixJiangz_ZhuantJiangz);
         List<ZisFaz_PeixJiangz_ZhuantJiangzModel> model = new List<ZisFaz_PeixJiangz_ZhuantJiangzModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_PeixJiangz_ZhuantJiangzModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Level = m.Level,
               Dynamic1 = m.Dynamic1,
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });


         return PartialView("ZisFaz_PeixJiangz_ZhuantJiangz", model);
      }


      #endregion


      #region [ 自身发展.培训与讲座.开设定向性课程 ]


      //	GET-Ajax:			Declare/ZisFaz_PeixJiangz_DingxxKec

      private ActionResult ZisFaz_PeixJiangz_DingxxKec()
      {
         var list = QueryDeclareActive(DeclareKeys.ZisFaz_PeixJiangz_DingxxKec);
         List<ZisFaz_PeixJiangz_DingxxKecModel> model = new List<ZisFaz_PeixJiangz_DingxxKecModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_PeixJiangz_DingxxKecModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2,
               IsShare = m.IsShare,
               Level = m.Level,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZisFaz_PeixJiangz_DingxxKec", model);
      }


      #endregion


      #region [ 制度建设.课程资源开发 ]


      //	GET-Ajax:			Declare/ZisFaz_PeixJiangz_KecZiyKaif

      private ActionResult ZisFaz_PeixJiangz_KecZiyKaif()
      {
         var list = QueryDeclareActive(DeclareKeys.ZisFaz_PeixJiangz_KecZiyKaif);
         List<ZhidJians_TesHuodKaizModel> model = new List<ZhidJians_TesHuodKaizModel>();    // 课程资源开发与特色活动模型通用一个。

         list.ForEach(m =>
         {
            model.Add(new ZhidJians_TesHuodKaizModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2,
               Level = m.Level,
               Dynamic9 = m.Dynamic9,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZisFaz_PeixJiangz_KecZiyKaif", model);
      }


      #endregion


      #region [ 自身发展.学术活动 ]


      public ActionResult ZisFaz_XuesHuod()
      {

         var list = QueryDeclareActive(DeclareKeys.ZisFaz_XuesHuod);
         List<ZisFaz_XuesHuodModel> model = new List<ZisFaz_XuesHuodModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_XuesHuodModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2,
               Level = m.Level,
               Dynamic9 = m.Dynamic9,
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });


         return PartialView("ZisFaz_XuesHuod", model);
      }


      #endregion


      #region [ 自身发展.教育教学科研成果  ]


      public ActionResult ZisFaz_KeyChengg()
      {
         var list = QueryDeclareAchievementList(DeclareKeys.ZisFaz_KeyChengg);

         return PartialView("ZisFaz_KeyChengg", list);
      }


      #endregion


      #region [ 自身发展.教育教学科研成果.开展课题(项目)研究工作 ]


      //	GET-Ajax:			Declare/ZisFaz_KeyChengg_KetYanj

      private ActionResult ZisFaz_KeyChengg_KetYanj()
      {
         var list = QueryDeclareAchievement(DeclareKeys.ZisFaz_KeyChengg_KetYanj);
         List<ZisFaz_KeyChengg_KetYanjModel> model = new List<ZisFaz_KeyChengg_KetYanjModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_KeyChengg_KetYanjModel
            {
               DeclareAchievementId = m.DeclareAchievementId,
               DateRegion = m.DateRegion,
               Dynamic1 = bool.Parse(m.Dynamic1),
               Level = m.Level,
               Dynamic2 = m.Dynamic2,
               NameOrTitle = m.NameOrTitle,
               Dynamic6 = m.Dynamic6,
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZisFaz_KeyChengg_KetYanj", model);
      }


      #endregion


      #region [ 自身发展.教育教学科研成果.在区级及以上刊物发表论文 ]


      //	GET-Ajax:			Declare/ZisFaz_KeyChengg_FabLunw

      private ActionResult ZisFaz_KeyChengg_FabLunw()
      {
         var list = QueryDeclareAchievement(DeclareKeys.ZisFaz_KeyChengg_FabLunw);
         List<ZisFaz_KeyChengg_FabLunwModel> model = new List<ZisFaz_KeyChengg_FabLunwModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_KeyChengg_FabLunwModel
            {
               DeclareAchievementId = m.DeclareAchievementId,
               Date = DateTime.Parse(m.Date),
               Dynamic1 = m.Dynamic1,
               Level = m.Level,
               Dynamic2 = m.Dynamic2,
               NameOrTitle = m.NameOrTitle,
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZisFaz_KeyChengg_FabLunw", model);
      }


      #endregion


      #region [ 自身发展.教育教学科研成果.论著情况 ]


      //	GET-Ajax:			Declare/ZisFaz_KeyChengg_LunzQingk

      private ActionResult ZisFaz_KeyChengg_LunzQingk()
      {
         var list = QueryDeclareAchievement(DeclareKeys.ZisFaz_KeyChengg_LunzQingk);
         List<ZisFaz_KeyChengg_LunzQingkModel> model = new List<ZisFaz_KeyChengg_LunzQingkModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_KeyChengg_LunzQingkModel
            {
               DeclareAchievementId = m.DeclareAchievementId,
               Date = DateTime.Parse(m.Date),
               Dynamic1 = m.Dynamic1,
               Dynamic2 = bool.Parse(m.Dynamic2),
               NameOrTitle = m.NameOrTitle,
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZisFaz_KeyChengg_LunzQingk", model);
      }


      #endregion


      #region [ 自身发展.市、区级大活动 ]


      //	GET-Ajax:			Declare/ZisFaz_ShiqjHuod

      private ActionResult ZisFaz_ShiqjHuod()
      {
         var list = QueryDeclareActive(DeclareKeys.ZisFaz_ShiqjHuod);
         List<ZisFaz_ShiqjHuodModel> model = new List<ZisFaz_ShiqjHuodModel>();

         list.ForEach(m =>
         {
            model.Add(new ZisFaz_ShiqjHuodModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2,
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZisFaz_ShiqjHuod", model);
      }


      #endregion


      #region [ 制度建设.有影响力的工作 ]


      //	GET-Ajax:			Declare/ZhidJians_YingxlDeGongz

      private ActionResult ZhidJians_YingxlDeGongz()
      {
         var list = QueryDeclareActive(DeclareKeys.ZhidJians_YingxlDeGongz);
         List<ZhidJians_YingxlDeGongzModel> model = new List<ZhidJians_YingxlDeGongzModel>();

         list.ForEach(m =>
         {
            model.Add(new ZhidJians_YingxlDeGongzModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZhidJians_YingxlDeGongz", model);
      }


      #endregion


      #region [ 制度建设.特色活动开展 ]


      //	GET-Ajax:			Declare/ZhidJians_TesHuodKaiz

      private ActionResult ZhidJians_TesHuodKaiz()
      {
         var list = QueryDeclareActive(DeclareKeys.ZhidJians_TesHuodKaiz);
         List<ZhidJians_TesHuodKaizModel> model = new List<ZhidJians_TesHuodKaizModel>();

         list.ForEach(m =>
         {
            model.Add(new ZhidJians_TesHuodKaizModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2,
               Level = m.Level,
               Dynamic9 = m.Dynamic9,
               IsShare = m.IsShare,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("ZhidJians_TesHuodKaiz", model);
      }


      #endregion


      #region [ 制度建设.档案建设 ]


      //	GET-Ajax:			Declare/ZhidJians_DangaJians

      private ActionResult ZhidJians_DangaJians()
      {
         var t = APDBDef.DeclareOrgConst;

         var model = APQuery.select(t.Asterisk)
            .from(t)
            .where(t.TeacherId == UserProfile.UserId)
            .query(db, r =>
            {
               return new DeclareOrgConst()
               {
                  DeclareOrgConstId = t.DeclareOrgConstId.GetValue(r),
                  Content = SubString(t.Content.GetValue(r)),
                  Remark = SubString(t.Remark.GetValue(r)),
                  Work = SubString(t.Work.GetValue(r)),
                  IsDeclare = t.IsDeclare.GetValue(r)
               };
            }).ToList();

         return PartialView("ZhidJians_DangaJians", model);
      }


      #endregion


      #region [ 区内流动.学科带头人.流入学校 ]


      private ActionResult QunLiud_XuekDaitr_LiurXuex()
      {
         var list = QueryDeclareContent(DeclareKeys.QunLiud_XuekDaitr_LiurXuex);
         QunLiud_LiurXuexModel model = new QunLiud_LiurXuexModel();

         list.ForEach(m =>
         {
            if (m.ContentKey == DeclareKeys.QunLiud_XuekDaitr_LiurXuex_XuexMingc)
            {
               model.XuexMingc = m.ContentValue;
            }
            else if (m.ContentKey == DeclareKeys.QunLiud_XuekDaitr_LiurXuex_JiaoyZuc)
            {
               model.JiaoyZuc = m.ContentValue;
            }
         });

         return PartialView("QunLiud_XuekDaitr_LiurXuex", model);
      }

      [HttpPost]
      public ActionResult QunLiud_XuekDaitr_LiurXuex(QunLiud_LiurXuexModel model)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            SetDeclareContent(DeclareKeys.QunLiud_XuekDaitr_LiurXuex_XuexMingc, model.XuexMingc, false);
            SetDeclareContent(DeclareKeys.QunLiud_XuekDaitr_LiurXuex_JiaoyZuc, model.JiaoyZuc, false);

            db.Commit();
         }
         catch (System.Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 区内流动.学科带头人.课堂教学.上公开课 ]


      //	GET-Ajax:			Declare/QunLiud_XuekDaitr_KetJiaox_Gongkk

      private ActionResult QunLiud_XuekDaitr_KetJiaox_Gongkk()
      {
         var list = QueryDeclareActive(DeclareKeys.QunLiud_XuekDaitr_KetJiaox_Gongkk);
         List<QunLiud_XuekDaitr_KetJiaox_GongkkModel> model = new List<QunLiud_XuekDaitr_KetJiaox_GongkkModel>();

         list.ForEach(m =>
         {
            model.Add(new QunLiud_XuekDaitr_KetJiaox_GongkkModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1
            });
         });

         return PartialView("QunLiud_XuekDaitr_KetJiaox_Gongkk", model);
      }


      #endregion


      #region [ 区内流动.学科带头人.课堂教学.公开汇报课 ]


      //	GET-Ajax:			Declare/QunLiud_XuekDaitr_KetJiaox_GongkHuibk

      private ActionResult QunLiud_XuekDaitr_KetJiaox_GongkHuibk()
      {
         var list = QueryDeclareActive(DeclareKeys.QunLiud_XuekDaitr_KetJiaox_GongkHuibk);
         List<QunLiud_XuekDaitr_KetJiaox_GongkHuibkModel> model = new List<QunLiud_XuekDaitr_KetJiaox_GongkHuibkModel>();

         list.ForEach(m =>
         {
            model.Add(new QunLiud_XuekDaitr_KetJiaox_GongkHuibkModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1
            });
         });

         return PartialView("QunLiud_XuekDaitr_KetJiaox_GongkHuibk", model);
      }


      #endregion


      #region [ 区内流动.学科带头人.课堂教学.接受教师听随堂课 ]


      //	GET-Ajax:			Declare/QunLiud_XuekDaitr_KetJiaox_Suitk

      private ActionResult QunLiud_XuekDaitr_KetJiaox_Suitk()
      {
         var list = QueryDeclareActive(DeclareKeys.QunLiud_XuekDaitr_KetJiaox_Suitk);
         List<QunLiud_XuekDaitr_KetJiaox_SuitkModel> model = new List<QunLiud_XuekDaitr_KetJiaox_SuitkModel>();

         list.ForEach(m =>
         {
            model.Add(new QunLiud_XuekDaitr_KetJiaox_SuitkModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1
            });
         });

         return PartialView("QunLiud_XuekDaitr_KetJiaox_Suitk", model);
      }


      #endregion


      #region [ 区内流动.学科带头人.课堂教学.开展听课指导 ]


      //	GET-Ajax:			Declare/QunLiud_XuekDaitr_KetJiaox_TingKZhid

      private ActionResult QunLiud_XuekDaitr_KetJiaox_TingKZhid()
      {
         var list = QueryDeclareActive(DeclareKeys.QunLiud_XuekDaitr_KetJiaox_TingKZhid);
         List<QunLiud_XuekDaitr_KetJiaox_TingKZhidModel> model = new List<QunLiud_XuekDaitr_KetJiaox_TingKZhidModel>();

         list.ForEach(m =>
         {
            model.Add(new QunLiud_XuekDaitr_KetJiaox_TingKZhidModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2
            });
         });

         return PartialView("QunLiud_XuekDaitr_KetJiaox_TingKZhid", model);
      }


      #endregion


      #region [ 区内流动.学科带头人.教育科研.开设专题讲座 ]


      //	GET-Ajax:			Declare/QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz

      private ActionResult QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz()
      {
         var list = QueryDeclareActive(DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz);
         List<QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangzModel> model = new List<QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangzModel>();

         list.ForEach(m =>
         {
            model.Add(new QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangzModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1
            });
         });

         return PartialView("QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz", model);
      }


      #endregion


      #region [ 区内流动.学科带头人.教育科研.主持教研活动 ]


      //	GET-Ajax:			Declare/QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod

      private ActionResult QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod()
      {
         var list = QueryDeclareActive(DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod);
         List<QunLiud_XuekDaitr_JiaoyKey_JiaoyHuodModel> model = new List<QunLiud_XuekDaitr_JiaoyKey_JiaoyHuodModel>();

         list.ForEach(m =>
         {
            model.Add(new QunLiud_XuekDaitr_JiaoyKey_JiaoyHuodModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2
            });
         });

         return PartialView("QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod", model);
      }


      #endregion


      #region [ 区内流动.学科带头人.教育科研.参与教研组、备课组活动 ]


      //	GET-Ajax:			Declare/QunLiud_XuekDaitr_JiaoyKey_CanyHuod

      private ActionResult QunLiud_XuekDaitr_JiaoyKey_CanyHuod()
      {
         var list = QueryDeclareActive(DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_CanyHuod);
         List<QunLiud_XuekDaitr_JiaoyKey_CanyHuodModel> model = new List<QunLiud_XuekDaitr_JiaoyKey_CanyHuodModel>();

         list.ForEach(m =>
         {
            model.Add(new QunLiud_XuekDaitr_JiaoyKey_CanyHuodModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1
            });
         });

         return PartialView("QunLiud_XuekDaitr_JiaoyKey_CanyHuod", model);
      }


      #endregion


      #region [ 区内流动.学科带头人.教育科研.阶段性总结 ]

      //	GET-Ajax:			Declare/QunLiud_XuekDaitr_JiaoyKey_JiedxZongj
      //	POST-Ajax:			Declare/QunLiud_XuekDaitr_JiaoyKey_JiedxZongj

      private ActionResult QunLiud_XuekDaitr_JiaoyKey_JiedxZongj()
      {
         var list = QueryDeclareContent(DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_JiedxZongj);
         QunLiud_XuekDaitr_JiaoyKey_JiedxZongjModel model = new QunLiud_XuekDaitr_JiaoyKey_JiedxZongjModel();

         list.ForEach(m =>
         {
            if (m.ContentKey == DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_JiedxZongj)
            {
               model.Summary = m.ContentValue;
            }

         });

         return PartialView("QunLiud_XuekDaitr_JiaoyKey_JiedxZongj", model);
      }

      [HttpPost]
      public ActionResult QunLiud_XuekDaitr_JiaoyKey_JiedxZongj(QunLiud_XuekDaitr_JiaoyKey_JiedxZongjModel model)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            SetDeclareContent(DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_JiedxZongj, model.Summary, false);

            db.Commit();
         }
         catch (System.Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 区内流动.学科带头人.带教培训.带教对象 ]

      //	GET-Ajax:			Declare/QunLiud_XuekDaitr_DaijPeix_DaijDuix
      //	POST-Ajax:			Declare/QunLiud_XuekDaitr_DaijPeix_DaijDuix

      private ActionResult QunLiud_XuekDaitr_DaijPeix_DaijDuix()
      {
         var list = QueryDeclareContent(DeclareKeys.QunLiud_XuekDaitr_DaijPeix);
         QunLiud_XuekDaitr_DaijPeix_DaijDuixModel model = new QunLiud_XuekDaitr_DaijPeix_DaijDuixModel();

         list.ForEach(m =>
         {
            if (m.ContentKey == DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijDuix)
            {
               model.Member = m.ContentValue;
            }
            else if (m.ContentKey == DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijXuek)
            {
               model.Subject = m.ContentValue;
            }
            else if (m.ContentKey == DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijDuixFazFenx)
            {
               model.Analysis = m.ContentValue;
            }
            else if (m.ContentKey == DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijFangaGaiy)
            {
               model.Summary = m.ContentValue;
            }

         });

         return PartialView("QunLiud_XuekDaitr_DaijPeix_DaijDuix", model);
      }

      [HttpPost]
      public ActionResult QunLiud_XuekDaitr_DaijPeix_DaijDuix(QunLiud_XuekDaitr_DaijPeix_DaijDuixModel model)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            SetDeclareContent(DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijDuix, model.Member, false);
            SetDeclareContent(DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijXuek, model.Subject, false);
            SetDeclareContent(DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijDuixFazFenx, model.Analysis, false);
            SetDeclareContent(DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijFangaGaiy, model.Summary, false);

            db.Commit();
         }
         catch (System.Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 区内流动.学科带头人.带教培训.带教指导记录 ]


      //	GET-Ajax:			Declare/QunLiud_XuekDaitr_DaijPeix_DaijZhidJil

      private ActionResult QunLiud_XuekDaitr_DaijPeix_DaijZhidJil()
      {
         var list = QueryDeclareActive(DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijZhidJil);
         List<QunLiud_XuekDaitr_DaijPeix_DaijZhidJilModel> model = new List<QunLiud_XuekDaitr_DaijPeix_DaijZhidJilModel>();

         list.ForEach(m =>
         {
            model.Add(new QunLiud_XuekDaitr_DaijPeix_DaijZhidJilModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue)
            });
         });

         return PartialView("QunLiud_XuekDaitr_DaijPeix_DaijZhidJil", model);
      }


      #endregion


      #region [ 区内流动.骨干教师.流入学校 ]


      private ActionResult QunLiud_GugJiaos_LiurXuex()
      {
         var list = QueryDeclareContent(DeclareKeys.QunLiud_GugJiaos_LiurXuex);
         QunLiud_LiurXuexModel model = new QunLiud_LiurXuexModel();

         list.ForEach(m =>
         {
            if (m.ContentKey == DeclareKeys.QunLiud_GugJiaos_LiurXuex_XuexMingc)
            {
               model.XuexMingc = m.ContentValue;
            }
            else if (m.ContentKey == DeclareKeys.QunLiud_GugJiaos_LiurXuex_JiaoyZuc)
            {
               model.JiaoyZuc = m.ContentValue;
            }
         });

         return PartialView("QunLiud_GugJiaos_LiurXuex", model);
      }

      [HttpPost]
      public ActionResult QunLiud_GugJiaos_LiurXuex(QunLiud_LiurXuexModel model)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            SetDeclareContent(DeclareKeys.QunLiud_GugJiaos_LiurXuex_XuexMingc, model.XuexMingc, false);
            SetDeclareContent(DeclareKeys.QunLiud_GugJiaos_LiurXuex_JiaoyZuc, model.JiaoyZuc, false);

            db.Commit();
         }
         catch (System.Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 区内流动.骨干教师.开设公开课 ]


      //	GET-Ajax:			Declare/QunLiud_GugJiaos_Gongkk

      private ActionResult QunLiud_GugJiaos_Gongkk()
      {
         var list = QueryDeclareActive(DeclareKeys.QunLiud_GugJiaos_Gongkk);
         List<QunLiud_GugJiaos_GongkkModel> model = new List<QunLiud_GugJiaos_GongkkModel>();

         list.ForEach(m =>
         {
            model.Add(new QunLiud_GugJiaos_GongkkModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1
            });
         });

         return PartialView("QunLiud_GugJiaos_Gongkk", model);
      }


      #endregion


      #region [ 区内流动.骨干教师.开展听课指导 ]


      //	GET-Ajax:			Declare/QunLiud_GugJiaos_TingkZhid

      private ActionResult QunLiud_GugJiaos_TingkZhid()
      {
         var list = QueryDeclareActive(DeclareKeys.QunLiud_GugJiaos_TingkZhid);
         List<QunLiud_GugJiaos_TingkZhidModel> model = new List<QunLiud_GugJiaos_TingkZhidModel>();

         list.ForEach(m =>
         {
            model.Add(new QunLiud_GugJiaos_TingkZhidModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2
            });
         });

         return PartialView("QunLiud_GugJiaos_TingkZhid", model);
      }


      #endregion


      #region [ 区内流动.骨干教师.主持备课组活动 ]


      //	GET-Ajax:			Declare/QunLiud_GugJiaos_BeikzHuod

      private ActionResult QunLiud_GugJiaos_BeikzHuod()
      {
         var list = QueryDeclareActive(DeclareKeys.QunLiud_GugJiaos_BeikzHuod);
         List<QunLiud_GugJiaos_BeikzHuodModel> model = new List<QunLiud_GugJiaos_BeikzHuodModel>();

         list.ForEach(m =>
         {
            model.Add(new QunLiud_GugJiaos_BeikzHuodModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2
            });
         });

         return PartialView("QunLiud_GugJiaos_BeikzHuod", model);
      }


      #endregion


      #region [ 区内流动.骨干教师.任教年级班级 ]


      private ActionResult QunLiud_GugJiaos_RenjNianjBanj()
      {
         var list = QueryDeclareContent(DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj);
         QunLiud_GugJiaos_RenjNianjBanjModel model = new QunLiud_GugJiaos_RenjNianjBanjModel();

         list.ForEach(m =>
         {
            if (m.ContentKey == DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj_Memo1)
            {
               model.Memo1 = m.ContentValue;
            }
            else if (m.ContentKey == DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj_Memo2)
            {
               model.Memo2 = m.ContentValue;
            }
            else if (m.ContentKey == DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj_Memo3)
            {
               model.Memo3 = m.ContentValue;
            }
            else if (m.ContentKey == DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj_Memo4)
            {
               model.Memo4 = m.ContentValue;
            }
         });

         return PartialView("QunLiud_GugJiaos_RenjNianjBanj", model);
      }

      [HttpPost]
      public ActionResult QunLiud_GugJiaos_RenjNianjBanj(QunLiud_GugJiaos_RenjNianjBanjModel model)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            SetDeclareContent(DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj_Memo1, model.Memo1, false);
            SetDeclareContent(DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj_Memo2, model.Memo2, false);
            SetDeclareContent(DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj_Memo3, model.Memo3, false);
            SetDeclareContent(DeclareKeys.QunLiud_GugJiaos_RenjNianjBanj_Memo4, model.Memo4, false);

            db.Commit();
         }
         catch (System.Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 配合教研员.教研信息 ]


      private ActionResult PeihJiaoyyGongz_JiaoyXinx()
      {
         var list = QueryDeclareContent(DeclareKeys.PeihJiaoyyGongz_JiaoyXinx);
         PeihJiaoyyGongz_JiaoyXinxModel model = new PeihJiaoyyGongz_JiaoyXinxModel();

         model.Memo3 = DateTime.Now;

         list.ForEach(m =>
         {
            if (m.ContentKey == DeclareKeys.PeihJiaoyyGongz_JiaoyXinx_Memo1)
            {
               model.Memo1 = m.ContentValue;
               model.IsDeclare1 = m.IsDeclare;
            }
            else if (m.ContentKey == DeclareKeys.PeihJiaoyyGongz_JiaoyXinx_Memo2)
            {
               model.Memo2 = m.ContentValue;
               model.IsDeclare2 = m.IsDeclare;
            }
            else if (m.ContentKey == DeclareKeys.PeihJiaoyyGongz_JiaoyXinx_Memo3)
            {
               model.Memo3 = System.Convert.ToDateTime(m.ContentValue);
               model.IsDeclare3 = m.IsDeclare;
            }
         });

         return PartialView("PeihJiaoyyGongz_JiaoyXinx", model);
      }

      [HttpPost]
      public ActionResult PeihJiaoyyGongz_JiaoyXinx(PeihJiaoyyGongz_JiaoyXinxModel model)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            SetDeclareContent(DeclareKeys.PeihJiaoyyGongz_JiaoyXinx_Memo1, model.Memo1, model.IsDeclare1);
            SetDeclareContent(DeclareKeys.PeihJiaoyyGongz_JiaoyXinx_Memo2, model.Memo2, model.IsDeclare2);
            SetDeclareContent(DeclareKeys.PeihJiaoyyGongz_JiaoyXinx_Memo3, model.Memo3.ToString(), model.IsDeclare3, DataTypeKey.DateTime);

            db.Commit();
         }
         catch (System.Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 配合教研员工作.学科教研 ]


      //	GET-Ajax:			Declare/PeihJiaoyyGongz_XuekJiaoy

      private ActionResult PeihJiaoyyGongz_XuekJiaoy()
      {
         var list = QueryDeclareActive(DeclareKeys.PeihJiaoyyGongz_XuekJiaoy);
         List<PeihJiaoyyGongz_XuekJiaoyModel> model = new List<PeihJiaoyyGongz_XuekJiaoyModel>();

         list.ForEach(m =>
         {
            model.Add(new PeihJiaoyyGongz_XuekJiaoyModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("PeihJiaoyyGongz_XuekJiaoy", model);
      }


      #endregion


      #region [ 配合教研员工作.学科命题 ]


      //	GET-Ajax:			Declare/PeihJiaoyyGongz_XuekMingt

      private ActionResult PeihJiaoyyGongz_XuekMingt()
      {
         var list = QueryDeclareActive(DeclareKeys.PeihJiaoyyGongz_XuekMingt);
         List<PeihJiaoyyGongz_XuekMingtModel> model = new List<PeihJiaoyyGongz_XuekMingtModel>();

         list.ForEach(m =>
         {
            model.Add(new PeihJiaoyyGongz_XuekMingtModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               Dynamic2 = m.Dynamic2,
               Dynamic3 = m.Dynamic3,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("PeihJiaoyyGongz_XuekMingt", model);
      }


      #endregion


      #region [ 配合教研员工作.基层学校调研 ]


      //	GET-Ajax:			Declare/PeihJiaoyyGongz_JicXuexTiaoy

      private ActionResult PeihJiaoyyGongz_JicXuexTiaoy()
      {
         var list = QueryDeclareActive(DeclareKeys.PeihJiaoyyGongz_JicXuexTiaoy);
         List<PeihJiaoyyGongz_JicXuexTiaoyModel> model = new List<PeihJiaoyyGongz_JicXuexTiaoyModel>();

         list.ForEach(m =>
         {
            model.Add(new PeihJiaoyyGongz_JicXuexTiaoyModel
            {
               DeclareActiveId = m.DeclareActiveId,
               Date = m.Date,
               Location = m.Location,
               ContentValue = SubString(m.ContentValue),
               Dynamic1 = m.Dynamic1,
               IsDeclare = m.IsDeclare
            });
         });

         return PartialView("PeihJiaoyyGongz_JicXuexTiaoy", model);
      }


      #endregion


      #region [ 年底总结 ]


      private ActionResult NiandZongj(string Key)
      {
         var list = QueryDeclareContent(Key);
         NiandZongjModel model = new NiandZongjModel();

         model.Key = Key;

         list.ForEach(m =>
         {
            if (m.ContentKey == Key)
            {
               model.Summary = m.ContentValue;
               model.IsDeclare = m.IsDeclare;
            }
         });

         return PartialView("NiandZongj", model);
      }

      [HttpPost]
      [ValidateInput(false)]
      public ActionResult NiandZongj(NiandZongjModel model)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            SetDeclareContent(model.Key, model.Key, model.IsDeclare);

            db.Commit();
         }
         catch (System.Exception ex)
         {
            db.Rollback();

            return Json(new
            {
               result = AjaxResults.Error,
               msg = ex.Message
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 填报信息 ]

      public ActionResult Overview(long id)
      {
         var ep = APDBDef.EvalPeriod;
         var currentPeriod = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null).FirstOrDefault();
         if (currentPeriod == null) throw new ApplicationException("当前不再任何考核期！");

         ViewBag.DeclareContent = QueryDeclareContent(id, currentPeriod);

         var t = APDBDef.DeclareResume;
         ViewBag.DeclareResume = db.DeclareResumeDal.ConditionQuery(t.TeacherId == id, null, null, null);

         ViewBag.DeclareActive = QueryDeclareActiveList(id, currentPeriod);

         ViewBag.DeclareAchievement = QueryDeclareAchievementList(id, currentPeriod);

         var d = APDBDef.DeclareOrgConst;
         ViewBag.DeclareOrgConst = db.DeclareOrgConstDal.ConditionQuery(d.TeacherId == id, null, null, null);


         ViewBag.DeclareBase = GetDeclareBase(id);

         var tc = APDBDef.TeamContent;
         ViewBag.TeamContent = db.TeamContentDal.ConditionQuery(tc.TeamId == id, null, null, null);

         ViewBag.TeamMember = db.GetMemberListById(id);


         ViewBag.TeamSpecialCourse = GetTeamSpecialCourseList(id);

         ViewBag.TeamActive = GetTeamActiveList(id, currentPeriod);

         var subQuery = APQuery.select(tm.MemberId).from(tm).where(tm.TeamId == id);
         var memberDaList = APQuery.select(da.Asterisk, dcl.DeclareTargetPKID)
            .from(da, dcl.JoinInner(da.TeacherId == dcl.TeacherId))
            .where(
            da.CreateDate >= currentPeriod.BeginDate & da.CreateDate <= currentPeriod.EndDate &
            da.Date >= currentPeriod.BeginDate & da.Date <= currentPeriod.EndDate &
            da.TeacherId.In(subQuery) & (da.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk |
                                                da.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_Yantk |
                                                da.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_JiaoxPingb))
            .query(db, r =>
            {
               var data = new DeclareActiveDataModel();
               da.Fullup(r, data, false);
               data.TargetId = dcl.DeclareTargetPKID.GetValue(r);
               return data;
            }).ToList();

         var memberDacList = APQuery.select(dac.Asterisk, dcl.DeclareTargetPKID)
            .from(dac, dcl.JoinInner(dac.TeacherId == dcl.TeacherId))
            .where(
            dac.CreateDate >= currentPeriod.BeginDate & dac.CreateDate <= currentPeriod.EndDate &
            dac.TeacherId.In(subQuery) & (dac.AchievementKey == DeclareKeys.ZisFaz_KeyChengg_KetYanj | dac.AchievementKey == DeclareKeys.ZisFaz_KeyChengg_FabLunw))
            .query(db, r =>
            {
               var data = new DeclareAchievementDataModel();
               dac.Fullup(r, data, false);
               data.TargetId = dcl.DeclareTargetPKID.GetValue(r);
               return data;
            }).ToList();

         ViewBag.MemberDaList = memberDaList;
         ViewBag.MemberDacList = memberDacList;

         return View();
      }

      #endregion


      #region [ Helper ]


      private List<DeclareContent> QueryDeclareContent(long teacherId, EvalPeriod period)
         => db.DeclareContentDal.ConditionQuery(
            tc.TeacherId == teacherId,
            null, null, null);


      private List<DeclareContent> QueryDeclareContent(string startWith)
         => db.DeclareContentDal.ConditionQuery(
            tc.TeacherId == UserProfile.UserId & tc.ContentKey.StartWith(startWith),
            null, null, null);


      private void SetDeclareContent(string key, string value, bool isDeclare, string type = "String")
      {
         value = value.Trim();

         var period = db.GetCurrentDeclarePeriod();
         //var maybeId = APQuery.select(tc.DeclareContentId)
         //   .from(tc)
         //   .where(tc.TeacherId == UserProfile.UserId & tc.ContentKey == key)
         //   .executeScale(db);

         var content = db.DeclareContentDal.ConditionQuery(tc.ContentKey == key & tc.TeacherId == UserProfile.UserId, null, null, null).FirstOrDefault();
         if (content == null)
         {
            content = new DeclareContent
            {
               TeacherId = UserProfile.UserId,
               ContentKey = key,
               ContentValue = value,
               ContentDataType = type,
               Creator = UserProfile.UserId,
               CreateDate = DateTime.Now,
               IsDeclare = isDeclare
            };

            db.DeclareContentDal.Insert(content);
         }
         else
         {
            APQuery.update(tc)
               .set(tc.ContentValue.SetValue(value))
               .set(tc.Modifier.SetValue(UserProfile.UserId))
               .set(tc.ModifyDate.SetValue(DateTime.Now))
               .set(tc.IsDeclare.SetValue(isDeclare))
               .where(tc.DeclareContentId == content.DeclareContentId)
               .execute(db);
         }

         content.IsDeclare = isDeclare;
         AddDeclareMaterial(content, period);
      }


      private void AddDeclareMaterial(DeclareContent content, DeclarePeriod period)
      {
         if (content != null && period != null && content.IsDeclare)
         {
            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == content.DeclareContentId & dm.PeriodId == period.PeriodId);
            if (content.IsDeclare)
               db.DeclareMaterialDal.Insert(new DeclareMaterial
               {
                  ItemId = content.DeclareContentId,
                  ParentType = "DeclareContent",
                  CreateDate = DateTime.Now,
                  PubishDate = DateTime.Now,
                  Title = content.ContentValue,
                  Type = content.ContentKey,
                  TeacherId = UserProfile.UserId,
                  PeriodId = period.PeriodId
               });
         }
      }


      private List<DeclareActive> QueryDeclareActiveList(long teacherId, EvalPeriod period)
         => db.DeclareActiveDal.ConditionQuery(
            da.TeacherId == teacherId
            & da.CreateDate >= period.BeginDate
            & da.CreateDate <= period.EndDate
            & da.Date >= period.BeginDate
            & da.Date <= period.EndDate,
            null, null, null);


      private List<DeclareActive> QueryDeclareActiveList(string condition)
         => db.DeclareActiveDal.ConditionQuery(
            da.TeacherId == UserProfile.UserId & da.ActiveKey.Match(condition),
            null, null, null);


      private List<DeclareActive> QueryDeclareActive(string condition)
         => db.DeclareActiveDal.ConditionQuery(
            da.TeacherId == UserProfile.UserId & da.ActiveKey == condition,
            null, null, null);


      private List<DeclareAchievement> QueryDeclareAchievementList(long teacherId, EvalPeriod period)
         => db.DeclareAchievementDal.ConditionQuery(
            tat.TeacherId == teacherId & tat.CreateDate >= period.BeginDate & tat.CreateDate <= period.EndDate,
            null, null, null);


      private List<DeclareAchievement> QueryDeclareAchievementList(string condition)
         => db.DeclareAchievementDal.ConditionQuery(
            tat.TeacherId == UserProfile.UserId & tat.AchievementKey.Match(condition),
            null, null, null);


      private List<DeclareAchievement> QueryDeclareAchievement(string condition)
         => db.DeclareAchievementDal.ConditionQuery(
            tat.TeacherId == UserProfile.UserId & tat.AchievementKey == condition,
            null, null, null);


      private Dictionary<long, TeamActiveViewModel> GetTeamActiveList(long teamId, EvalPeriod current)
      {
         var p = APDBDef.PicklistItem;

         var list = new Dictionary<long, TeamActiveViewModel>();

         if (current == null) throw new ApplicationException("当前没有考核周期");

         list = APQuery.select(t.Asterisk, p.Name)
            .from(t, p.JoinInner(t.ActiveType == p.PicklistItemId))
            .where(t.TeamId == teamId
            & t.CreateDate >= current.BeginDate & t.CreateDate <= current.EndDate
            & t.Date >= current.BeginDate & t.Date <= current.EndDate)
            .query(db, r =>
            {
               return new TeamActiveViewModel()
               {
                  Title = t.Title.GetValue(r),
                  ContentValue = t.ContentValue.GetValue(r),
                  Date = t.Date.GetValue(r),
                  Location = t.Location.GetValue(r),
                  TeamActiveId = t.TeamActiveId.GetValue(r),
                  TypeName = p.Name.GetValue(r)
               };
            }).ToDictionary(m => m.TeamActiveId);

         GetTeamActiveResult(list, teamId);


         return list;
      }


      private void GetTeamActiveItem(Dictionary<long, TeamActiveViewModel> list, long teamId)
      {
         APQuery.select(t.TeamActiveId, tai.SendDate, tai.ItemContent, u.RealName)
            .from(t,
                  tai.JoinInner(t.TeamActiveId == tai.ActiveId),
                  u.JoinInner(tai.MemberId == u.UserId))
            .where(t.TeamId == teamId)
            .query(db, r =>
            {
               var ActiveId = t.TeamActiveId.GetValue(r);
               var Name = u.RealName.GetValue(r);
               var ItemContent = tai.ItemContent.GetValue(r);
               var SendDate = tai.SendDate.GetValue(r);
               if (list.ContainsKey(ActiveId))
               {
                  list[ActiveId].Item.Add(new TeamActiveItemViewModel
                  {
                     SendDate = SendDate,
                     ItemContent = ItemContent,
                     MemberName = Name
                  });
               }

               return ActiveId;

            }).ToList();
      }


      private void GetTeamActiveResult(Dictionary<long, TeamActiveViewModel> list, long teamId)
      {
         APQuery.select(t.TeamActiveId, tar.ActiveResult, u.RealName, tar.ResultId)
            .from(t,
                  tar.JoinInner(t.TeamActiveId == tar.ActiveId),
                  u.JoinInner(tar.MemberId == u.UserId))
            .where(t.TeamId == teamId)
            .query(db, r =>
            {
               var ActiveId = t.TeamActiveId.GetValue(r);
               var Name = u.RealName.GetValue(r);
               var Content = tar.ActiveResult.GetValue(r);
               var ResultId = tar.ResultId.GetValue(r);
               if (list.ContainsKey(ActiveId))
               {
                  list[ActiveId].Result.Add(new TeamActiveResultViewModel
                  {
                     ResultId = ResultId,
                     MemberName = Name,
                     ActiveResult = Content
                  });
               }

               return ActiveId;
            }).ToList();
      }


      private Dictionary<long, TeamSpecialCourseModel> GetTeamSpecialCourseList(long teamId)
      {
         var list = new Dictionary<long, TeamSpecialCourseModel>();

         list = APQuery.select(tsc.Asterisk)
            .from(tsc)
            .where(tsc.TeamId == teamId)
            .query(db, r =>
            {
               return new TeamSpecialCourseModel()
               {
                  CourseId = tsc.CourseId.GetValue(r),
                  TeamId = tsc.TeamId.GetValue(r),
                  Title = tsc.Title.GetValue(r),
                  StartDate = tsc.StartDate.GetValue(r),
                  EndDate = tsc.EndDate.GetValue(r),
                  CourseTarget = tsc.CourseTarget.GetValue(r),
                  CoursePlan = tsc.CoursePlan.GetValue(r),
                  CourseRecords = tsc.CourseRecords.GetValue(r),
                  CourseResults = tsc.CourseResults.GetValue(r),
                  CourseSummary = tsc.CourseSummary.GetValue(r),
                  Remark = tsc.Remark.GetValue(r),
                  TotalCount = tsc.TotalCount.GetValue(r),
                  MemberCount = tsc.MemberCount.GetValue(r),
                  MemberRecord = tsc.MemberRecord.GetValue(r)
               };
            }).ToDictionary(m => m.CourseId);

         GetTeamSpecialCourseItemList(list, teamId);


         return list;
      }


      private void GetTeamSpecialCourseItemList(Dictionary<long, TeamSpecialCourseModel> list, long teamId)
      {
         var tsci = APDBDef.TeamSpecialCourseItem;

         APQuery.select(tsci.Asterisk)
            .from(tsc, tsci.JoinInner(tsc.CourseId == tsci.CourseId))
            .where(tsc.TeamId == teamId)
            .query(db, r =>
            {
               var CourseId = tsci.CourseId.GetValue(r);
               var ItemDate = tsci.ItemDate.GetValue(r);
               var Location = tsci.Location.GetValue(r);
               var Title = tsci.Title.GetValue(r);
               var Content = tsci.Content.GetValue(r);
               var ActivityType = tsci.ActivityType.GetValue(r);
               var Speaker = tsci.Speaker.GetValue(r);
               var Remark = tsci.Remark.GetValue(r);

               if (list.ContainsKey(CourseId))
               {
                  list[CourseId].Item.Add(new TeamSpecialCourseItem
                  {
                     ItemDate = ItemDate,
                     Location = Location,
                     Title = Title,
                     Content = Content,
                     ActivityType = ActivityType,
                     Speaker = Speaker,
                     Remark = Remark
                  });
               }
               return CourseId;
            }).ToList();
      }


      private DeclareBase GetDeclareBase(long teacherId)
      {
         var t = APDBDef.DeclareBase;

         var declare = APQuery.select(t.Asterisk, u.RealName)
            .from(t, u.JoinInner(t.TeacherId == u.UserId))
            .where(t.TeacherId == teacherId)
            .query(db, rd =>
            {
               return new DeclareBase
               {
                  TeacherId = t.TeacherId.GetValue(rd),
                  DeclareTargetPKID = t.DeclareTargetPKID.GetValue(rd),
                  DeclareSubjectPKID = t.DeclareSubjectPKID.GetValue(rd),
                  DeclareStagePKID = t.DeclareStagePKID.GetValue(rd),
                  AllowFlowToSchool = t.AllowFlowToSchool.GetValue(rd),
                  AllowFitResearcher = t.AllowFitResearcher.GetValue(rd),
                  HasTeam = t.HasTeam.GetValue(rd),
                  TeamName = t.TeamName.GetValue(rd),
                  MemberCount = t.MemberCount.GetValue(rd),
                  ActiveCount = t.ActiveCount.GetValue(rd),
                  RealName = u.RealName.GetValue(rd)
               };
            }).ToList().First();


         return declare;
      }


      private string SubString(string str, int length)
         => str.Length > length ? str.Substring(0, length) + "..." : str;



      private string SubString(string str)
         => str.Length > 50 ? str.Substring(0, 50) + "..." : str;


      #endregion

   }

}