using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

   public class ShareManageController : BaseController
   {
      private static APDBDef.ShareTableDef s = APDBDef.Share;
      private static APDBDef.AttachmentsTableDef att = APDBDef.Attachments;
      private static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;

      // GET: ShareManage/DeclareList
      // POST: ShareManage/DeclareList

      public ActionResult DeclareList(long? teacherId)
      {
         return View();
      }

      [HttpPost]
      public ActionResult DeclareList(long? teacherId, int current, int rowCount, AjaxOrder sort, string searchPhrase)
      {
         var query = APQuery
             .select(s.ShareId, s.ItemId, s.UserId, s.Type, s.Title, s.ParentType,
                     u.RealName)
             .from(s, u.JoinInner(u.UserId == s.UserId))
             .primary(s.ShareId)
             .skip((current - 1) * rowCount)
             .take(rowCount)
             .where(s.ParentType == ShareKeys.ActiveShare | s.ParentType == ShareKeys.AchievementShare);

         if (teacherId > 0)
            query = query.where_and(s.UserId == teacherId);

         var total = db.ExecuteSizeOfSelect(query);

         var result = query.query(db, r => {
            return new
            {
               id = s.ShareId.GetValue(r),
               itemId = s.ItemId.GetValue(r),
               userId = s.UserId.GetValue(r),
               realName = u.RealName.GetValue(r),
               title = SubString(s.Title.GetValue(r)),
               parentType = s.ParentType.GetValue(r),
               type = s.Type.GetValue(r),
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


      // GET: ShareManage/TeamList
      // POST: ShareManage/TeamList

      public ActionResult TeamList(long? teacherId)
      {
         return View();
      }

      [HttpPost]
      public ActionResult TeamList(long? teacherId, int current, int rowCount, AjaxOrder sort, string searchPhrase)
      {
         var query = APQuery
             .select(s.ShareId, s.ItemId, s.UserId, s.Type, s.Title, s.ParentType,
                     u.RealName)
             .from(s, u.JoinInner(u.UserId == s.UserId))
             .primary(s.ShareId)
             .skip((current - 1) * rowCount)
             .take(rowCount)
             .where(s.ParentType == ShareKeys.TeamActiveShare);

         if (teacherId > 0)
            query = query.where_and(s.UserId == teacherId);


         var total = db.ExecuteSizeOfSelect(query);

         var result = query.query(db, r => {
            return new
            {
               id = s.ShareId.GetValue(r),
               itemId = s.ItemId.GetValue(r),
               userId = s.UserId.GetValue(r),
               realName = u.RealName.GetValue(r),
               title = SubString(s.Title.GetValue(r)),
               parentType = s.ParentType.GetValue(r),
               type = AttachmentsKeys.YanXHuod_Edit//s.Type.GetValue(r),
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


      public ActionResult TeamActiveList(long teacherId)
      {
         return null;
      }



      private string SubString(string str)
        => str.Length > 50 ? str.Substring(0, 50) + "..." : str;

   }

}