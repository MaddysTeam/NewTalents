using Business;
using Business.Config;
using Business.Helper;
using CasUtility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Symber.Web.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

   public class DeclareReviewController : BaseController
   {

      static APDBDef.DeclareReviewTableDef dr = APDBDef.DeclareReview;

      // GET: DeclareMaterial/Review
      // POST-Ajax: DeclareMaterial/Review

      public ActionResult Edit(long? id)
      {
         DeclareReview review = null;
         if (id != null)
         {
            review = db.DeclareReviewDal.PrimaryGet(id.Value);
         }

         return PartialView(review ?? new DeclareReview());
      }

      [HttpPost]
      [DecalrePeriod]
      public ActionResult Edit(DeclareReview review)
      {
         var cd = APDBDef.CompanyDeclare;

         if (!Period.IsInReviewPeriod)
         {
            return Json(new
            {
               result = AjaxResults.Error,
               msg = "未在评审期,请联系管理员!"
            });
         }

         db.BeginTrans();

         try
         {
            db.DeclareReviewDal.UpdatePartial(review.DeclareReviewId, new { StatusKey = review.StatusKey, ReviewComment = review.ReviewComment });

            db.CompanyDeclareDal.ConditionDelete(cd.TeacherId == review.TeacherId);
            db.CompanyDeclareDal.Insert(new CompanyDeclare { CompanyId = review.CompanyId, TeacherId = review.TeacherId });
            

            db.Commit();
         }
         catch
         {
            db.Rollback();
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "操作成功!"
         });
      }

      [DecalrePeriod]
      public ActionResult List(long companyId)
      {
         return View();
      }

      [HttpPost]
      public ActionResult List(long companyId, int current, int rowCount, AjaxOrder sort, string searchPhrase)
      {
         var u = APDBDef.BzUserProfile;
         var u2 = APDBDef.BzUserProfile.As("reviewer");
         var c = APDBDef.Company;
         var currentPeriod = Period ?? new DeclarePeriod();
         var query = APQuery.select(dr.DeclareReviewId, dr.ReviewComment, dr.StatusKey, dr.TeacherId, dr.ReviewerId, dr.DeclareTargetPKID, dr.DeclareSubjectPKID,
                             u.RealName, u2.RealName.As("reviewer"), c.CompanyName)
                          .from(dr,
                                u.JoinInner(dr.TeacherId == u.UserId),
                                c.JoinInner(c.CompanyId == dr.CompanyId),
                                u2.JoinLeft(dr.ReviewerId == u2.UserId)
                                )
                          .where(dr.PeriodId == currentPeriod.PeriodId & dr.StatusKey != string.Empty);

         if (companyId > 0)
            query = query.where_and(c.CompanyId == companyId);


         //过滤条件
         //模糊搜索姓名,标题

         searchPhrase = searchPhrase.Trim();
         if (searchPhrase != "")
         {
            query.where_and(u.RealName.Match(searchPhrase));
         }


         //排序条件表达式

         if (sort != null)
         {
            switch (sort.ID)
            {
               case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
            }
         }


         var total = db.ExecuteSizeOfSelect(query);


         var result = query.query(db, r => new
         {
            id = dr.DeclareReviewId.GetValue(r),
            comment = dr.ReviewComment.GetValue(r),
            status = dr.StatusKey.GetValue(r),
            teacherId = dr.TeacherId.GetValue(r),
            realName = u.RealName.GetValue(r),
            reviewer = u2.RealName.GetValue(r, "reviewer"),
            company = c.CompanyName.GetValue(r),
            declare = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(r)),
            // subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(r))
         }).ToList();

         return Json(new
         {
            rows = result,
            current,
            rowCount,
            total
         });
      }

   }

}