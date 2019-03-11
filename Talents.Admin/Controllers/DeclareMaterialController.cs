using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
      private static APDBDef.DeclareFormTableDef df = APDBDef.DeclareForm;

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
      public ActionResult DeclareAchievement(long id, bool isDeclare,long declareTargetId)
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
         if (key == DeclareKeys.XuekDaitr_Shenb || key == DeclareKeys.XuekDaitr_ZhicPog)
         {
            return PartialView("MaterialView5005", key);
         }

         return View();
      }


      // Get: DeclareMaterial/Submit
      // POST-Ajax: DeclareMaterial/Submit

      public ActionResult FormIndexEdit(string key)
      {
         var decalreForm = db.DeclareFormDal.ConditionQuery(df.PeriodId == Period.PeriodId & df.TeacherId == UserProfile.UserId, null, null, null).FirstOrDefault();
         decalreForm = decalreForm ?? new DeclareForm();
         return PartialView("_index", decalreForm);
      }

      [HttpPost]
      public ActionResult FormIndexEdit(DeclareForm model)
      {
         model.PeriodId = Period.PeriodId;
         model.TeacherId = UserProfile.UserId;
         if (model.DeclareFormId == 0)
         {
            db.DeclareFormDal.Insert(model);
         }
         else
         {
            db.DeclareFormDal.UpdatePartial(model.DeclareFormId, new
            {
               model.CompanyId,
               model.DeclareTargetPKID,
               model.DeclareSubjectPKID,
               model.IsBrokenRoles,
               model.Reason,
               model.AllowFlowToDowngrade,
               model.AllowFlowToSchool,
               model.AllowFitResearcher
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "操作成功！"
         });
      }


      // Get: DeclareMaterial/BasicMaterialEdit
      // POST-Ajax: DeclareMaterial/BasicMaterialEdit

      public ActionResult BasicProfileEdit(string key)
      {
         var profile = db.BzUserProfileDal.PrimaryGet(UserProfile.UserId);
         return PartialView("_profile5005", profile);
      }

      [HttpPost]
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
            model.EduSubjectPKID,
            model.SkillTitlePKID,
            model.RankTitlePKID,
            model.Hiredate,
            model.Phonemobile,
            model.Phone,
            model.Email,
            model.Dynamic1,
            model.Dynamic2,
            model.Dynamic3,
            model.Dynamic4,
            model.Dynamic5
         });

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "操作成功！"
         });
      }


      // Get: DeclareMaterial/DeclareActiveList   TODO:declareTargetId表示当前申报的称号

      public ActionResult DeclareActiveList(string itemKey, long declareTargetId)
      {
         var results = db.DeclareActiveDal.ConditionQuery(da.TeacherId == UserProfile.UserId & da.IsDeclare == false & da.ActiveKey == itemKey, null, null, null);

         return PartialView("_declare_active_list", results);
      }


      // Get: DeclareMaterial/Items   TODO:declareTargetId表示当前申报的称号

      public ActionResult Items(long declareTargetId)
      {
         var teacherId = UserProfile.UserId;
         var actives = APQuery.select(dm.MaterialId, dm.DeclareTargetPKID, da.Asterisk)
            .from(dm, da.JoinInner(dm.ItemId == da.DeclareActiveId))
            .where(dm.PeriodId == Period.PeriodId & dm.TeacherId == teacherId & da.Creator == teacherId & dm.DeclareTargetPKID == declareTargetId)
            .query(db, r =>
            {
               var active = new DeclareActive();
               da.Fullup(r, active, false);

               return active;
            }).ToList();

         var achievements = APQuery.select(dm.MaterialId, dm.DeclareTargetPKID, dac.Asterisk)
            .from(dm, dac.JoinInner(dm.ItemId == dac.DeclareAchievementId))
            .where(dm.PeriodId == Period.PeriodId & dm.TeacherId == teacherId & dac.Creator == teacherId & dm.DeclareTargetPKID == declareTargetId)
            .query(db, r =>
            {
               var achievement = new DeclareAchievement();
               dac.Fullup(r, achievement, false);

               return achievement;
            }).ToList();


         ViewBag.DeclareActives = actives;

         ViewBag.DecalreAchievements = achievements;

         return PartialView("_items5005");
      }


      // Get: DeclareMaterial/Items

      public ActionResult Preview()
      {
         return View();
      }


      // POST-Ajax: DeclareMaterial/Submit

      [HttpPost]
      public ActionResult Submit(long formId)
      {
         db.DeclareFormDal.UpdatePartial(formId, new {
            StatusKey = DeclareKeys.ReviewProcess
         });

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "操作成功！"
         });
      }


      #region [ Helper ]

      private string SubString(string str)
    => str.Length > 50 ? str.Substring(0, 50) + "..." : str;

      #endregion

   }

}