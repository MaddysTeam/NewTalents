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

      // GET: DeclareMaterial/List
      // POST: DeclareMaterial/List

      public ActionResult List(long? teacherId)
      {
         return View();
      }

      [HttpPost]
      public ActionResult List(long? teacherId, int current, int rowCount, AjaxOrder sort, string searchPhrase)
      {
         var query = APQuery
             .select(dm.MaterialId, dm.ItemId, dm.UserId, dm.Title, dm.Type,
                     u.RealName)
             .from(dm, u.JoinInner(u.UserId == dm.UserId))
             .primary(dm.MaterialId)
             .skip((current - 1) * rowCount)
             .take(rowCount)
             .where(dm.UserId == teacherId);

         if (teacherId > 0)
            query = query.where_and(dm.UserId == teacherId);

         var total = db.ExecuteSizeOfSelect(query);

         var result = query.query(db, r =>
         {
            return new
            {
               id = dm.MaterialId.GetValue(r),
               itemId = dm.ItemId.GetValue(r),
               userId = dm.UserId.GetValue(r),
               realName = u.RealName.GetValue(r),
               type = dm.Type.GetValue(r),
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
      public ActionResult DeclareActive(long id, bool isDeclare)
      {
         ThrowNotAjax();

         var active = db.DeclareActiveDal.PrimaryGet(id);
         active.IsDeclare = isDeclare;

         if (active != null)
         {
            db.BeginTrans();

            try
            {
               db.DeclareActiveDal.UpdatePartial(id, new { isDeclare = active.IsDeclare });

               if (active.IsDeclare)
                  db.DeclareMaterialDal.Insert(new DeclareMaterial
                  {
                     ItemId = id,
                     ParentType = "DeclareActive",
                     CreateDate = DateTime.Now,
                     PubishDate = DateTime.Now,
                     Title = active.ContentValue,
                     Type = active.ActiveKey,
                     UserId = UserProfile.UserId
                  });
               else
                  db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id);


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
      public ActionResult DeclareAchievement(long id, bool isDeclare)
      {
         ThrowNotAjax();

         var achievement = db.DeclareAchievementDal.PrimaryGet(id);
         achievement.IsDeclare = isDeclare;

         if (achievement != null)
         {
            db.BeginTrans();

            try
            {
               db.DeclareAchievementDal.UpdatePartial(id, new { IsDeclare = achievement.IsDeclare });

               if (achievement.IsDeclare)
                  db.DeclareMaterialDal.Insert(new DeclareMaterial
                  {
                     ItemId = id,
                     ParentType = "DeclareAchievement",
                     CreateDate = DateTime.Now,
                     PubishDate = DateTime.Now,
                     Title = achievement.NameOrTitle,
                     Type = achievement.AchievementKey,
                     UserId = UserProfile.UserId
                  });
               else
                  db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id);


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


      private string SubString(string str)
    => str.Length > 50 ? str.Substring(0, 50) + "..." : str;

   }
}