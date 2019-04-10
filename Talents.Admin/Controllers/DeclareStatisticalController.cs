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
      static APDBDef.PicklistItemTableDef pi = APDBDef.PicklistItem;

      //	GET:	DeclareStatistical/DeclareProfileList

      public ActionResult DeclareProfileList()
      {
         return View();
      }

      [HttpPost]
      public ActionResult DeclareProfileList(long companyId, long eduBgId, long degreeId,
         long skillId, long politicalId, long nationId, long subjectId, long stageId, long rankId,
         int current, int rowCount, AjaxOrder sort, string searchPhrase)
      {
         var query = APQuery
                 .select(dp.Asterisk, c.CompanyName.As("Company"), pi.Name.As("targetName"))
                 .from(dp,
                 p.JoinInner(p.PeriodId == dp.PeriodId),
                 pi.JoinInner(dp.DeclareTargetPKID == pi.PicklistItemId),
                 df.JoinInner(df.PeriodId == dp.PeriodId & df.TeacherId == dp.UserId & df.DeclareTargetPKID == dp.DeclareTargetPKID),
                 c.JoinLeft(c.CompanyId == dp.CompanyId)
                 )
         .primary(dp.UserId)
         .where(dp.PeriodId == Period.PeriodId & df.StatusKey != string.Empty);
         //.skip((current - 1) * rowCount)
         //.take(rowCount);

         if (UserProfile.IsSchoolAdmin)
         {
            query.where_and(dp.CompanyId == UserProfile.CompanyId | df.CompanyId == UserProfile.CompanyId);
         }
         else if (UserProfile.IsSystemAdmin && companyId > 0)
         {
            query.where_and(dp.CompanyId == companyId | df.CompanyId == companyId);
         }
         if (eduBgId > 0)
         {
            query.where_and(dp.EduBgPKID == eduBgId);
         }
         if (degreeId > 0)
         {
            query.where_and(dp.EduDegreePKID == degreeId);
         }
         if (skillId > 0)
         {
            query.where_and(dp.SkillTitlePKID == skillId);
         }
         if (politicalId > 0)
         {
            query.where_and(dp.PoliticalStatusPKID == politicalId);
         }
         if (nationId > 0)
         {
            query.where_and(dp.NationalityPKID == nationId);
         }
         if (subjectId > 0)
         {
            query.where_and(dp.EduSubjectPKID == subjectId);
         }
         if (stageId > 0)
         {
            query.where_and(dp.EduStagePKID == stageId);
         }
         if (rankId > 0)
         {
            query.where_and(dp.RankTitlePKID == rankId);
         }

         //过滤条件
         //模糊搜索姓名,标题

         searchPhrase = searchPhrase.Trim();
         if (searchPhrase != "")
         {
            query.where_and(dp.RealName.Match(searchPhrase));
         }


         //排序条件表达式

         if (sort != null)
         {
            switch (sort.ID)
            {
               case "RealName": query.order_by(sort.OrderBy(dp.RealName)); break;
               case "Gender": query.order_by(sort.OrderBy(dp.GenderPKID)); break;
               case "CompanyName": query.order_by(sort.OrderBy(c.CompanyName)); break;
               case "Birthday": query.order_by(sort.OrderBy(dp.Birthday)); break;
               case "Nationality": query.order_by(sort.OrderBy(dp.NationalityPKID)); break;
               case "Hiredate": query.order_by(sort.OrderBy(dp.Hiredate)); break;
               case "TrainNo": query.order_by(sort.OrderBy(dp.TrainNo)); break;
               case "RankTitle": query.order_by(sort.OrderBy(dp.RankTitlePKID)); break;
               case "EduBg": query.order_by(sort.OrderBy(dp.EduBgPKID)); break;
               case "EduDegree": query.order_by(sort.OrderBy(dp.EduDegreePKID)); break;
               case "DeclareTargetName": query.order_by(sort.OrderBy(dp.DeclareTargetPKID)); break;
               case "SkillTitle": query.order_by(sort.OrderBy(dp.SkillTitlePKID)); break;
               case "EduStage": query.order_by(sort.OrderBy(dp.EduStagePKID)); break;
               case "EduSubject": query.order_by(sort.OrderBy(dp.EduSubjectPKID)); break;
               case "CourseCountPerWeek": query.order_by(sort.OrderBy(dp.CourseCountPerWeek)); break;
            }
         }


         var total = db.ExecuteSizeOfSelect(query);

         var result = query.query(db, rd =>
         {
            DeclareProfile profile = new DeclareProfile();
            dp.Fullup(rd, profile, false);
            profile.CompanyName = c.CompanyName.GetValue(rd, "Company");
            profile.DeclareTargetName = pi.Name.GetValue(rd, "TargetName");

            return profile;
         }).ToList();

         if (result.Count > 0)
         {
            result = result.Skip((current <= 1 ? 0 : current - 1) * rowCount).Take(rowCount).ToList();
         }


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
      public ActionResult DeclareSummaryList(long targetId, long declareCompanyId, long decalreSubjectId, long subjectId,
         int current, int rowCount, AjaxOrder sort, string searchPhrase)
      {
         // 申报汇总表了包含：学校名称、申报称号、申报学科、姓名、性别、出生年月、任教学科、联系方式（手机）、年度考核优秀（年份）、是否破格（普通申报、职称破格和材料破格）

         var query = APQuery
               .select(df.Asterisk, c.CompanyName, pi.Name.As("DeclareTarget"),
                       dp.EduSubjectPKID, dp.GenderPKID, dp.Dynamic1, dp.Dynamic2, dp.Dynamic3, dp.Phonemobile)
               .from(df,
               p.JoinInner(p.PeriodId == df.PeriodId),
               pi.JoinInner(df.DeclareTargetPKID == pi.PicklistItemId),
               dp.JoinInner(dp.UserId == df.TeacherId & dp.PeriodId == df.PeriodId & df.DeclareTargetPKID == dp.DeclareTargetPKID),
               c.JoinLeft(c.CompanyId == df.CompanyId)
               )
               .primary(df.DeclareReviewId)
       .where(df.StatusKey != string.Empty)
       .skip((current - 1) * rowCount)
       .take(rowCount);

         if (UserProfile.IsSchoolAdmin)
         {
            query.where_and(df.CompanyId == UserProfile.CompanyId);
         }
         else if (UserProfile.IsSystemAdmin && declareCompanyId > 0)
         {
            query.where_and(df.CompanyId == declareCompanyId);
         }

         if (targetId > 0)
         {
            query.where_and(df.DeclareTargetPKID == targetId);
         }
         if (decalreSubjectId > 0)
         {
            query.where_and(df.DeclareSubjectPKID == decalreSubjectId);
         }
         if (subjectId > 0)
         {
            query.where_and(dp.EduSubjectPKID == subjectId);
         }


         //过滤条件
         //模糊搜索姓名,标题

         searchPhrase = searchPhrase.Trim();
         if (searchPhrase != "")
         {
            query.where_and(df.TeacherName.Match(searchPhrase));
         }


         //排序条件表达式

         if (sort != null)
         {
            switch (sort.ID)
            {
               case "TeacherName": query.order_by(sort.OrderBy(df.TeacherName)); break;
               case "Gender": query.order_by(sort.OrderBy(dp.GenderPKID)); break;
               case "DeclareTargetName": query.order_by(sort.OrderBy(df.DeclareTargetPKID)); break;
               case "DeclareCompnay": query.order_by(sort.OrderBy(df.CompanyId)); break;
               case "Subject": query.order_by(sort.OrderBy(dp.EduSubjectPKID)); break;
               case "IsBroke": query.order_by(sort.OrderBy(df.IsBrokenRoles)); break;
            }
         }


         var total = db.ExecuteSizeOfSelect(query);

         var result = query.query(db, rd =>
         {
            var goodYear1 = dp.Dynamic1.GetValue(rd) == "优秀" ? "2016" : string.Empty;
            var goodYear2 = dp.Dynamic1.GetValue(rd) == "优秀" ? "2017" : string.Empty;
            var goodYear3 = dp.Dynamic1.GetValue(rd) == "优秀" ? "2018" : string.Empty;
            string[] goodYears = { goodYear1, goodYear2, goodYear3 };
            string years = string.Join(",", goodYears);

            DeclareReview review = new DeclareReview();
            df.Fullup(rd, review, false);
            review.DeclareCompnay = c.CompanyName.GetValue(rd);
            review.DeclareTargetName = pi.Name.GetValue(rd, "DeclareTarget");
            review.DeclareSubject = DeclareBaseHelper.DeclareSubject.GetName(df.DeclareSubjectPKID.GetValue(rd), "", false);
            review.Subject = BzUserProfileHelper.EduSubject.GetName(dp.EduSubjectPKID.GetValue(rd), "", false);
            review.Gender = BzUserProfileHelper.Gender.GetName(dp.GenderPKID.GetValue(rd), "", false);
            review.GoodYear = string.IsNullOrEmpty(years.Replace(",", "")) ? "-" : years;
            review.IsDeclareBroke = review.IsBrokenRoles ? "是" : "否";
            review.IsMaterialBroke = review.TypeKey.IndexOf("材料破格") > 0 ? "是" : "否";
            review.PhoneMobile = dp.Phonemobile.GetValue(rd);

            return review;
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