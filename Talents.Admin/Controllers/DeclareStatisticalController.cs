using Business;
using Business.Config;
using Business.Helper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheSite.Models;
using Webdiyer.WebControls.Mvc;

namespace TheSite.Controllers
{

	public class DeclareStatisticalController : BaseController
	{

      static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
      static APDBDef.DeclareMaterialTableDef dm = APDBDef.DeclareMaterial;
      static APDBDef.DeclarePeriodTableDef p = APDBDef.DeclarePeriod;
      static APDBDef.DeclareReviewTableDef df = APDBDef.DeclareReview;
      static APDBDef.DeclareProfileTableDef dp = APDBDef.DeclareProfile;
      static APDBDef.CompanyTableDef c = APDBDef.Company;

      //	GET:	DeclareStatistical/DeclareProfileList

      public ActionResult DeclareProfileList(long companyId)
      {
         return View();
      }

      [HttpPost]
      public ActionResult DeclareProfileList(long companyId, int current, int rowCount, AjaxOrder sort, string searchPhrase)
      {
         var query = APQuery
                 .select(dp.Asterisk)
                 .from(dp,
                 p.JoinInner(p.PeriodId == dp.PeriodId),
                 c.JoinLeft(c.CompanyId == dp.CompanyId))
                 .primary(dp.DeclareProfileId);
                 //.where(dp.StatusKey!=string.Empty);
                 //.skip(current*(rowCount-1))
                 //.take(rowCount);

         if (companyId > 0)
         {
            query.where(dp.CompanyId == companyId);
         }


         //过滤条件
         //模糊搜索姓名,标题

         searchPhrase = searchPhrase.Trim();
         if (searchPhrase != "")
         {
            query.where_and(dp.RealName.Match(searchPhrase));
         }


         //排序条件表达式

         //if (sort != null)
         //{
         //   switch (sort.ID)
         //   {
         //      case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
         //      case "title": query.order_by(sort.OrderBy(n.Title)); break;
         //      case "createdTime": query.order_by(sort.OrderBy(n.CreatedTime)); break;
         //   }
         //}


         var total = db.ExecuteSizeOfSelect(query);

         var result = query.query(db, rd =>
         {
            DeclareProfile profile = new DeclareProfile();
            dp.Fullup(rd, profile, false);

            return profile;
         }).ToList();


         return Json(new
         {
            rows = result,
            current,
            rowCount,
            total
         });
      }


      public ActionResult DeclareSummaryList()
      {
         return View();
      }

      [HttpPost]
      public ActionResult DeclareSummaryList(long compnayId, int current, int rowCount, AjaxOrder sort, string searchPhrase)
      {
         return Json(new { });
      }

   }

}