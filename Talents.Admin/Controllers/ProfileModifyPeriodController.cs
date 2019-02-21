using Business;
using Symber.Web.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace TheSite.Controllers
{

   public class ProfileModifyPeriodController : BaseController
   {

      static APDBDef.ProfileModifyPeriodTableDef pmp = APDBDef.ProfileModifyPeriod;


      // GET: EvalPeriod/List

      public ActionResult List()
      {
         var list = db.ProfileModifyPeriodDal.ConditionQuery(null, null, null, null);

         return View(list);
      }


      //	GET: UserProfileModifyPeriod/Edit
      //	POST-Ajax: UserProfileModifyPeriod/Edit

      public ActionResult Edit(long? id)
      {
         var model = new ProfileModifyPeriod
         {
            BeginDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1),
         };

         if (id != null)
         {
            model = db.ProfileModifyPeriodDal.PrimaryGet(id.Value);
         }

         return PartialView("Edit", model);
      }

      [HttpPost]
      public ActionResult Edit(ProfileModifyPeriod model)
      {
         ThrowNotAjax();


         if (model.PeriodId > 0)
         {
            db.ProfileModifyPeriodDal.UpdatePartial(model.PeriodId, new
            {
               model.Name,
               model.BeginDate,
               model.EndDate,
            });
         }
         else
         {
            db.ProfileModifyPeriodDal.Insert(model);
         }


         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存!"
         });
      }


      //	POST-Ajax: UserProfileModifyPeriod/Remove		

      [HttpPost]
      public ActionResult Remove(long id)
      {
         ThrowNotAjax();

         db.ProfileModifyPeriodDal.PrimaryDelete(id);

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已删除"
         });
      }


      //	POST-Ajax: UserProfileModifyPeriod/SetCurrent	

      [HttpPost]
      public ActionResult SetCurrent(long id)
      {
         ThrowNotAjax();

         var buh = APDBDef.BzUserProfileHistory;
         var u = APDBDef.BzUserProfile;
         var d = APDBDef.DeclareBase;
         var dh = APDBDef.DeclareBaseHistory;


         var users = db.BzUserProfileDal.ConditionQuery(null, null, null, null);


         db.BeginTrans();

         try
         {
            db.BzUserProfileHistoryDal.ConditionDelete(buh.PeriodId == id);

            db.DeclareBaseHistoryDal.ConditionDelete(dh.PeriodId == id);

            db.CreateSqlCommand(string.Format(@"insert into [BzUserProfileHistory]
                                 (
                                        [UserId]
                                       ,[CompanyId]
                                       ,[UserName]
                                       ,[UserType]
                                       ,[RealName]
                                       ,[IDCard]
                                       ,[TrainNo]
                                       ,[GenderPKID]
                                       ,[Birthday]
                                       ,[PoliticalStatusPKID]
                                       ,[NationalityPKID]
                                       ,[EduSubjectPKID]
                                       ,[EduStagePKID]
                                       ,[JobDate]
                                       ,[SkillTitlePKID]
                                       ,[SkillDate]
                                       ,[CompanyName]
                                       ,[CompanyNameOuter]
                                       ,[Companyaddress]
                                       ,[RankTitlePKID]
                                       ,[EduBgPKID]
                                       ,[EduDegreePKID]
                                       ,[GraduateSchool]
                                       ,[GraduateDate]
                                       ,[Phonemobile]
                                       ,[Email]
                                       ,[PeriodId]
                                 )
                                 SELECT 
                                        [UserId]
                                       ,[CompanyId]
                                       ,[UserName]
                                       ,[UserType]
                                       ,[RealName]
                                       ,[IDCard]
                                       ,[TrainNo]
                                       ,[GenderPKID]
                                       ,[Birthday]
                                       ,[PoliticalStatusPKID]
                                       ,[NationalityPKID]
                                       ,[EduSubjectPKID]
                                       ,[EduStagePKID]
                                       ,[JobDate]
                                       ,[SkillTitlePKID]
                                       ,[SkillDate]
                                       ,[CompanyName]
                                       ,[CompanyNameOuter]
                                       ,[Companyaddress]
                                       ,[RankTitlePKID]
                                       ,[EduBgPKID]
                                       ,[EduDegreePKID]
                                       ,[GraduateSchool]
                                       ,[GraduateDate]
                                       ,[Phonemobile]
                                       ,[Email]
                                       ,[PeriodId]
                                   FROM [{0}].[dbo].[BzUserProfile]", Business.Config.ThisApp.DBName)).ExecuteNonQuery();

            db.CreateSqlCommand(string.Format(@"INSERT INTO [dbo].[DeclareBaseHistory]
           ([TeacherId]
           ,[DeclareTargetPKID]
           ,[DeclareSubjectPKID]
           ,[DeclareStagePKID]
           ,[AllowFlowToSchool]
           ,[AllowFitResearcher]
           ,[HasTeam]
           ,[TeamName]
           ,[MemberCount]
           ,[ActiveCount]
           ,[PeriodId])
            select 
            [TeacherId]
           ,[DeclareTargetPKID]
           ,[DeclareSubjectPKID]
           ,[DeclareStagePKID]
           ,[AllowFlowToSchool]
           ,[AllowFitResearcher]
           ,[HasTeam]
           ,[TeamName]
           ,[MemberCount]
           ,[ActiveCount]
           ,[PeriodId]
            from [{0}].[dbo].DeclareBase", Business.Config.ThisApp.DBName)).ExecuteNonQuery();

            APQuery.update(u)
            .set(u.PeriodId.SetValue(id))
            .execute(db);

            APQuery.update(d)
            .set(d.PeriodId.SetValue(id))
            .execute(db);

            APQuery.update(pmp)
            .set(pmp.IsCurrent.SetValue(false))
            .execute(db);

            APQuery.update(pmp)
               .set(pmp.IsCurrent.SetValue(true))
               .where(pmp.PeriodId == id)
               .execute(db);


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
            msg = "设置已成功！"
         });
      }

   }

}