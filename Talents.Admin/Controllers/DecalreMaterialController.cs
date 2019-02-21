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

   public class DecalreMaterialController : BaseController
   {

      private static APDBDef.DeclareMaterialTableDef dm = APDBDef.DeclareMaterial;
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
             .select(dm.MaterialId, dm.ItemId, dm.UserId, dm.Title,
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

      private string SubString(string str)
     => str.Length > 50 ? str.Substring(0, 50) + "..." : str;

   }

}