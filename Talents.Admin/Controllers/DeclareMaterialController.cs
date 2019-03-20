using Business;
using Business.Helper;
using Business.Utilities;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheSite.EvalAnalysis;
using TheSite.Models;

namespace TheSite.Controllers
{

   public class DeclareMaterialController : BaseController
   {

      private static APDBDef.DeclareMaterialTableDef dm = APDBDef.DeclareMaterial;
      private static APDBDef.DeclareActiveTableDef da = APDBDef.DeclareActive;
      private static APDBDef.DeclareAchievementTableDef dac = APDBDef.DeclareAchievement;
      private static APDBDef.DeclareContentTableDef dc = APDBDef.DeclareContent;
      private static APDBDef.DeclarePeriodTableDef p = APDBDef.DeclarePeriod;
      private static APDBDef.DeclareReviewTableDef df = APDBDef.DeclareReview;
      private static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
      private static APDBDef.CompanyTableDef c = APDBDef.Company;

      public ActionResult Index()
      {
         return View();
      }

      // POST-Ajax: DeclareMaterial/DeclareActive 申报活动 TODO: declareTargetId 表示当前正在申报的称号

      [HttpPost]
      [DecalrePeriod]
      public ActionResult DeclareActive(long id, bool isDeclare, long declareTargetId)
      {
         ThrowNotAjax();

         var period = Period;
         var active = db.DeclareActiveDal.PrimaryGet(id);
         active.IsDeclare = isDeclare;

         if (active != null)
         {
            db.BeginTrans();

            try
            {
               db.DeclareActiveDal.UpdatePartial(id, new { isDeclare = active.IsDeclare });

               DeclareMaterialHelper.AddDeclareMaterial(active, period, db, declareTargetId);

               db.Commit();
            }
            catch (Exception e)
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
            msg = "没有可以申报的活动！"
         });

      }

      // POST-Ajax: DeclareMaterial/DeclareAchievement 申报成果材料 TODO: declareTargetId 表示当前正在申报的称号

      [HttpPost]
      [DecalrePeriod]
      public ActionResult DeclareAchievement(long id, bool isDeclare, long declareTargetId)
      {
         ThrowNotAjax();

         var period = Period;
         var achievement = db.DeclareAchievementDal.PrimaryGet(id);
         achievement.IsDeclare = isDeclare;

         if (achievement != null)
         {
            db.BeginTrans();

            try
            {
               db.DeclareAchievementDal.UpdatePartial(id, new { IsDeclare = achievement.IsDeclare });

               DeclareMaterialHelper.AddDeclareMaterial(achievement, period, db, declareTargetId);

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
            msg = "没有可以申报的成果！"
         });
      }


      // POST-Ajax: DeclareMaterial/DeclareContent  申报内容材料

      [HttpPost]
      [DecalrePeriod]
      public ActionResult DeclareContent(long id, bool isDeclare)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            db.BeginTrans();

            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == Period.PeriodId);

            APQuery.update(dc).set(dc.IsDeclare.SetValue(false)).where(dc.DeclareContentId == id).execute(db);


            db.Commit();
         }
         catch
         {
            db.Rollback();
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "操作成功！"
         });

      }


      // GET: DeclareMaterial/Fragment

      public ActionResult Fragment(string key)
      {
         if (key == DeclareKeys.GaodLisz_ZijBiao)
         {
            return PartialView("MaterialView5002", key);
         }
         else if (key == DeclareKeys.JidZhucr_ZijBiao)
         {
            return PartialView("MaterialView5003", key);
         }
         else if (key == DeclareKeys.GongzsZhucr || key == DeclareKeys.GongzsZhucr_Shenb)
         {
            key = key == DeclareKeys.GongzsZhucr ? DeclareKeys.GongzsZhucr_Shenb : key;
            return PartialView("MaterialView5004", key);
         }
         else if (key == DeclareKeys.XuekDaitr || key == DeclareKeys.XuekDaitr_Shenb || key == DeclareKeys.XuekDaitr_ZhicPog)
         {
            key = key == DeclareKeys.XuekDaitr ? DeclareKeys.XuekDaitr_Shenb : key;
            return PartialView("MaterialView5005", key);
         }
         else if (key == DeclareKeys.GugJiaos || key == DeclareKeys.GugJiaos_Shenb || key == DeclareKeys.GugJiaos_ZhicPog)
         {
            key = key == DeclareKeys.GugJiaos ? DeclareKeys.GugJiaos_Shenb : key;
            return PartialView("MaterialView5006", key);
         }
         else if (key == DeclareKeys.JiaoxNengs || key == DeclareKeys.JiaoxNengs_Shenb || key == DeclareKeys.JiaoxNengs_ZhicPog)
         {
            key = key == DeclareKeys.JiaoxNengs ? DeclareKeys.JiaoxNengs_Shenb : key;
            return PartialView("MaterialView5007", key);
         }
         if (key == DeclareKeys.JiaoxXinx || key == DeclareKeys.JiaoxXinx_Shenb)
         {
            key = key == DeclareKeys.JiaoxXinx ? DeclareKeys.JiaoxXinx_Shenb : key;
            return PartialView("MaterialView5008", key);
         }
         else if (key.IndexOf("-") > 0)
         {
            var typeKey = key.Substring(0, key.IndexOf("-"));

            ViewBag.DeclareTargetId = key.Substring(key.IndexOf("-") + 1);

            return PartialView("MaterialView9999", typeKey);
         }
         else
         {
            return Content("正在开发中");
         }
      }

      // Get: DeclareMaterial/ReviewEdit
      // POST-Ajax: DeclareMaterial/ReviewEdit

      public ActionResult ReviewEdit(DeclareParam param)
      {
         var typeKey = param.TypeKey;
         var declareTargetId = param.DeclareTargetId;
         var poge = ".职称破格";
         var isZcPoge = typeKey.IndexOf(poge) > 0;
         var decalreReview = db.DeclareReviewDal.ConditionQuery(df.PeriodId == Period.PeriodId
                         & df.TeacherId == UserProfile.UserId
                         & df.TypeKey == (isZcPoge ? typeKey.Replace(poge, ".申报") : typeKey)
                         & df.DeclareTargetPKID == declareTargetId
                         , null, null, null).FirstOrDefault();

         decalreReview = decalreReview ?? new DeclareReview
         {
            DeclareTargetPKID = declareTargetId,
            AllowFitResearcher = true,
            AllowFlowToDowngrade = true,
            AllowFlowToSchool = true
         };
         decalreReview.TypeKey = param.TypeKey;

         return PartialView(param.View, decalreReview);
      }

      [HttpPost]
      [DecalrePeriod]
      public ActionResult ReviewEdit(DeclareReview model)
      {
         // 职称破格和申报公用一张表
         var poge = ".职称破格";
         if (model.TypeKey.IndexOf(poge) > 0)
            model.TypeKey = model.TypeKey.Replace(poge, ".申报");

         model.PeriodId = Period.PeriodId;
         model.TeacherId = UserProfile.UserId;

         if (model.CompanyId == 0)
         {
            return Json(new
            {
               result = AjaxResults.Error,
               msg = "必须选择申报单位！",
            });
         }

         if (model.DeclareSubjectPKID == 0)
         {
            return Json(new
            {
               result = AjaxResults.Error,
               msg = "必须选择申报学科！",
            });
         }

         if (model.DeclareReviewId == 0)
         {
            //var isExists = db.DeclareReviewDal.ConditionQueryCount(df.TeacherId == model.TeacherId & df.PeriodId == Period.PeriodId & df.DeclareTargetPKID == model.DeclareTargetPKID) > 0;
            //if (!isExists)
            db.DeclareReviewDal.Insert(model);
         }
         else
         {
            db.DeclareReviewDal.UpdatePartial(model.DeclareReviewId, new
            {
               model.CompanyId,
               model.DeclareTargetPKID,
               model.DeclareSubjectPKID,
               model.IsBrokenRoles,
               model.Reason,
               model.AllowFlowToDowngrade,
               model.AllowFlowToSchool,
               model.AllowFitResearcher,
               model.StatusKey,
               model.TypeKey,
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "操作成功！",
            data = model
         });
      }


      // Get: DeclareMaterial/BasicMaterialEdit
      // POST-Ajax: DeclareMaterial/BasicMaterialEdit

      public ActionResult BasicProfileEdit(DeclareParam param)
      {
         var profile = db.BzUserProfileDal.PrimaryGet(UserProfile.UserId);

         //TODO: 高地理事长和基地支持人的自荐表特殊处理
         if (param.DeclareTargetId == DeclareTargetIds.GaodLisz || param.DeclareTargetId == DeclareTargetIds.JidZhucr)
         {
            profile = profile ?? new BzUserProfile();
            profile.TargetId = param.DeclareTargetId;
         }

         return PartialView(param.View, profile);
      }

      [HttpPost]
      [DecalrePeriod]
      public ActionResult BasicProfileEdit(BzUserProfile model)
      {
         db.BzUserProfileDal.UpdatePartial(UserProfile.UserId, new
         {
            model.RealName,
            model.GenderPKID,
            model.Birthday,
            model.CompanyId,
            model.TrainNo,
            model.PoliticalStatusPKID,
            model.CourseCountPerWeek,
            model.NationalityPKID,
            model.SkillTitlePKID,
            model.RankTitlePKID,
            model.Hiredate,
            model.Phonemobile,
            model.Phone,
            model.Email,
            model.EduBgPKID,
            model.EduDegreePKID,
            model.EduStagePKID,
            model.EduSubjectPKID,
            model.Dynamic1,
            model.Dynamic2,
            model.Dynamic3,
            model.Dynamic4,
            model.Dynamic5
         });

         //TODO: 高地理事长和基地支持人的自荐表特殊处理
         if (model.TargetId > 0 && Period != null)
         {
            var decalreTargetId = model.TargetId;
            var typeKey = decalreTargetId == DeclareTargetIds.GaodLisz ? DeclareKeys.GaodLisz_ZijBiao : DeclareKeys.JidZhucr_ZijBiao;
            var review = db.DeclareReviewDal.ConditionQuery(df.DeclareTargetPKID == decalreTargetId & df.TeacherId == UserProfile.UserId & df.PeriodId == Period.PeriodId, null, null, null).FirstOrDefault();
            review = review ?? new DeclareReview
            {
               DeclareTargetPKID = decalreTargetId,
               DeclareSubjectPKID = model.EduSubjectPKID,
               CompanyId = model.CompanyId,
               TypeKey = typeKey,
            };
            review.StatusKey = model.StatusKey;

            ReviewEdit(review);
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "操作成功！"
         });
      }


      // Get: DeclareMaterial/DeclareActiveList   TODO: 历史库 declareTargetId表示当前申报的称号

      public ActionResult DeclareActiveList(string itemKey, long declareTargetId)
      {
         var results = db.DeclareActiveDal.ConditionQuery(da.TeacherId == UserProfile.UserId & da.IsDeclare == false & da.ActiveKey == itemKey, null, null, null);

         return PartialView("_declare_active_list", results);
      }


      // Get: DeclareMaterial/DeclareActiveList   TODO: 历史库 declareTargetId表示当前申报的称号

      public ActionResult DeclareAchievementList(string itemKey, long declareTargetId)
      {
         var results = db.DeclareAchievementDal.ConditionQuery(dac.TeacherId == UserProfile.UserId & dac.IsDeclare == false & dac.AchievementKey == itemKey, null, null, null);

         return PartialView("_declare_achievement_list", results);
      }


      // Get: DeclareMaterial/Items   TODO: declareTargetId表示当前申报的称号

      public ActionResult Items(DeclareParam param)
      {
         var teacherId = UserProfile.UserId;
         var actives = APQuery.select(dm.MaterialId, dm.DeclareTargetPKID, da.Asterisk)
            .from(dm, da.JoinInner(dm.ItemId == da.DeclareActiveId))
            .where(dm.PeriodId == Period.PeriodId & dm.TeacherId == teacherId & da.Creator == teacherId & dm.DeclareTargetPKID == param.DeclareTargetId)
            .query(db, r =>
            {
               var active = new DeclareActive();
               da.Fullup(r, active, false);

               return active;
            }).ToList();

         var achievements = APQuery.select(dm.MaterialId, dm.DeclareTargetPKID, dac.Asterisk)
            .from(dm, dac.JoinInner(dm.ItemId == dac.DeclareAchievementId))
            .where(dm.PeriodId == Period.PeriodId & dm.TeacherId == teacherId & dac.Creator == teacherId & dm.DeclareTargetPKID == param.DeclareTargetId)
            .query(db, r =>
            {
               var achievement = new DeclareAchievement();
               dac.Fullup(r, achievement, false);

               return achievement;
            }).ToList();


         return PartialView(param.View, new DeclareItemsViewModel
         {
            DeclareTargetId = param.DeclareTargetId,
            View = param.View,
            DeclareAchievements = achievements,
            DeclareActives = actives
         });
      }


      // Get: DeclareMaterial/Preview

      public ActionResult Preview(DeclarePreviewParam param)
      {
         var pdfRender = new HtmlRender();
         var model=GetPreviewViewModel(param);

         if (param.IsExport != null && param.IsExport.Value)
         {
            var htmlText = pdfRender.RenderViewToString(this, param.View, model);
            byte[] pdfFile = FormatConverter.ConvertHtmlTextToPDF(htmlText);
            return new BinaryContentResult($"temp.pdf", "application/pdf", pdfFile);
         }

         return PartialView(param.View, model);
      }


      // Get: DeclareMaterial/Overview

      public ActionResult Overview(DeclarePreviewParam param)
      {
         var model = GetPreviewViewModel(param);

         return View(param.View, model);
      }


      #region [ Helper ]

      private string SubString(string str)
    => str.Length > 50 ? str.Substring(0, 50) + "..." : str;

      private List<DeclareActive> GetDeclareActives(long declareTargetId, long teacherId) =>
         APQuery.select(dm.MaterialId, dm.DeclareTargetPKID, da.Asterisk)
          .from(dm, da.JoinInner(dm.ItemId == da.DeclareActiveId))
          .where(dm.PeriodId == Period.PeriodId & dm.TeacherId == teacherId & da.Creator == teacherId & dm.DeclareTargetPKID == declareTargetId)
          .query(db, r =>
          {
             var active = new DeclareActive();
             da.Fullup(r, active, false);

             return active;
          }).ToList();

      private List<DeclareAchievement> GetDeclareAchievements(long declareTargetId, long teacherId) =>
         APQuery.select(dm.MaterialId, dm.DeclareTargetPKID, dac.Asterisk)
            .from(dm, dac.JoinInner(dm.ItemId == dac.DeclareAchievementId))
            .where(dm.PeriodId == Period.PeriodId & dm.TeacherId == teacherId & dac.Creator == teacherId & dm.DeclareTargetPKID == declareTargetId)
            .query(db, r =>
            {
               var achievement = new DeclareAchievement();
               dac.Fullup(r, achievement, false);

               return achievement;
            }).ToList();


      private DeclarePreviewViewModel GetPreviewViewModel(DeclarePreviewParam param)
      {
         var isSchoolAdmin = UserProfile.IsSchoolAdmin;
         var userid = isSchoolAdmin ? param.TeacherId : UserProfile.UserId;
         var model = new DeclarePreviewViewModel();
         var profile = db.BzUserProfileDal.PrimaryGet(userid);//不从缓存里获取，从数据库获取
         var myCompnay = db.CompanyDal.PrimaryGet(profile.CompanyId);
         var review = db.DeclareReviewDal.ConditionQuery(
            df.TeacherId == userid &
            df.PeriodId == Period.PeriodId &
            df.DeclareTargetPKID == param.DeclareTargetId &
            df.TypeKey == param.TypeKey, null, null, null).FirstOrDefault();
         review = review ?? new DeclareReview();

         var declareCompany = db.CompanyDal.PrimaryGet(review.CompanyId);
         var declareActives = GetDeclareActives(param.DeclareTargetId, userid);
         var declareAchievement = GetDeclareAchievements(param.DeclareTargetId, userid);
         var poge = ".职称破格";

         model.DeclareTargetId = param.DeclareTargetId;
         model.TypeKey = param.TypeKey;
         model.Decalre = DeclareBaseHelper.DeclareTarget.GetName(param.DeclareTargetId);
         model.DeclareSubject = BzUserProfileHelper.EduSubject.GetName(review.DeclareSubjectPKID);
         model.DeclareCompany = declareCompany == null ? string.Empty : declareCompany.CompanyName;
         model.RealName = profile.RealName;
         model.RankTitle = profile.RankTitle;
         model.SkillTitle = profile.SkillTitle;
         model.TrainNo = profile.TrainNo;
         model.Plitics = profile.PoliticalStatus;
         model.EduBg = profile.EduBg;
         model.CourseCount = profile.CourseCountPerWeek;
         model.Company = myCompnay == null ? string.Empty : myCompnay.CompanyName;
         model.Mobile = profile.Phonemobile;
         model.Phone = profile.Phone;
         model.Email = profile.Email;
         model.Hiredate = profile.Hiredate.ToString("yyyy-MM-dd");
         model.Birthday = profile.Birthday.ToString("yyyy-MM-dd");
         model.Gender = profile.Gender;
         model.Nation = profile.Nationality;
         model.Subject = profile.EduStage + profile.EduSubject;
         model.EduBg = profile.EduBg + profile.EduDegree;
         model.FirstYearScore = profile.Dynamic1;
         model.SecondYearScore = profile.Dynamic2;
         model.ThirdYearScore = profile.Dynamic3;
         model.Is500= profile.Dynamic4 == DeclareKeys.ZhongzJihChengy;
         model.Is1000 = profile.Dynamic4 == DeclareKeys.GonggJihChengy;
         model.Is2000 = profile.Dynamic4 == DeclareKeys.ZhongzJihLingxReng;
         model.Is3000 = profile.Dynamic4 == DeclareKeys.GaofJihZhucRen;
         model.Is4000 = profile.Dynamic4 == DeclareKeys.GonggJihZhucRen;
         model.Is5002 = profile.Dynamic4 == DeclareKeys.GaodLisz;
         model.Is5003 = profile.Dynamic4 == DeclareKeys.JidZhucr;
         model.Is5004 = profile.Dynamic4 == DeclareKeys.GongzsZhucr;
         model.Is5005 = profile.Dynamic4 == DeclareKeys.XuekDaitr;
         model.Is5006 = profile.Dynamic4 == DeclareKeys.GugJiaos;
         model.Is5007 = profile.Dynamic4 == DeclareKeys.JiaoxNengs;
         model.Is5008 = profile.Dynamic4 == DeclareKeys.JiaoxXinx;
         model.Is6000 = profile.Dynamic4 == DeclareKeys.GaodJiaoSYanxBanXuey;
         model.Comment1 = profile.Dynamic5;
         model.IsAllowDownGrade = review.AllowFlowToDowngrade;
         model.IsAllowdFlow = review.AllowFlowToSchool;
         model.DeclareActies = declareActives;
         model.DeclareAchievements = declareAchievement;
         model.Reason = review.Reason;
         // 职称破格和申报公用一张表 所以要用form IsBrokenRoles 和 param.TypeKey 一起判断
         model.IsBrokRoles = param.TypeKey.IndexOf(poge) > 0;

         return model;
      }

      #endregion

   }

}