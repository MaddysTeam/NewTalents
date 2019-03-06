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
      private static APDBDef.AttachmentsTableDef att = APDBDef.Attachments;
      private static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
      private static APDBDef.DeclareActiveTableDef da = APDBDef.DeclareActive;
      private static APDBDef.DeclareAchievementTableDef dac = APDBDef.DeclareAchievement;
      private static APDBDef.DeclareContentTableDef dc = APDBDef.DeclareContent;
      private static APDBDef.DeclarePeriodTableDef p = APDBDef.DeclarePeriod;
      private static APDBDef.TeamActiveTableDef t = APDBDef.TeamActive;
      private static APDBDef.TeamActiveResultTableDef tar = APDBDef.TeamActiveResult;
      private static APDBDef.TeamSpecialCourseTableDef tsc = APDBDef.TeamSpecialCourse;


      public ActionResult Index()
      {
         return View();
      }

      // GET: DeclareMaterial/List
      // POST: DeclareMaterial/List

      public ActionResult List(long? teacherId)
      {
         return View();
      }

      [HttpPost]
      public ActionResult List(long? teacherId, int current, int rowCount, AjaxOrder sort, string searchPhrase)
      {
         var period = db.DeclarePeriodDal.ConditionQuery(p.IsCurrent == true, null, null, null).FirstOrDefault();
         if (period == null)
            throw new ApplicationException("请设置申报周期");

         var query = APQuery
             .select(dm.MaterialId, dm.ItemId, dm.TeacherId, dm.Title, dm.Type, dm.ParentType,
                     u.RealName)
             .from(dm, u.JoinInner(u.UserId == dm.TeacherId))
             .primary(dm.MaterialId)
             .skip((current - 1) * rowCount)
             .take(rowCount)
             .where(dm.TeacherId == teacherId & dm.PeriodId == period.PeriodId);

         if (teacherId > 0)
            query = query.where_and(dm.TeacherId == teacherId);

         var total = db.ExecuteSizeOfSelect(query);

         var result = query.query(db, r =>
         {
            return new
            {
               id = dm.MaterialId.GetValue(r),
               itemId = dm.ItemId.GetValue(r),
               userId = dm.TeacherId.GetValue(r),
               realName = u.RealName.GetValue(r),
               type = dm.Type.GetValue(r),
               parentType = dm.ParentType.GetValue(r),
               title = SubString(dm.Title.GetValue(r))
            };
         }).ToList();

         return Json(new
         {
            rows = result,
            current,
            rowCount,
            total
         });
      }


      // POST-Ajax: DeclareMaterial/DeclareActive  申报活动材料

      [HttpPost]
      [DecalrePeriod]
      public ActionResult DeclareActive(long id, bool isDeclare)
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

               DeclareMaterialHelper.AddDeclareMaterial(active, period, db);

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

      // POST-Ajax: DeclareMaterial/DeclareAchievement 申报成果材料

      [HttpPost]
      [DecalrePeriod]
      public ActionResult DeclareAchievement(long id, bool isDeclare)
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

               DeclareMaterialHelper.AddDeclareMaterial(achievement, period, db);

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
         if (key == DeclareKeys.GaodLisz)
         {
            return PartialView("MaterialView5002");
         }

         return View();
      }


      #region [ Helper ]

      private string SubString(string str)
    => str.Length > 50 ? str.Substring(0, 50) + "..." : str;

      #endregion

   }
}