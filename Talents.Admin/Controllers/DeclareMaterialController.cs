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
         if (key == DeclareKeys.XuekDaitr_Shenb || key == DeclareKeys.XuekDaitr_ZhicPog)
         {
            return PartialView("MaterialView5005", key);
         }
         else if (key == DeclareKeys.XuekDaitr_CaiLPog)
         {
            return PartialView("MaterialView9999", key);
         }
         else
         {
            var form = db.DeclareFormDal.PrimaryGet(Convert.ToInt64(key));
            if (form == null) return Content("正在开发中");
            var isMaterialBreakRole = form.TypeKey.IndexOf("材料破格") >= 0;
            var declareTargetId = isMaterialBreakRole ? 9999 : form.DeclareTargetPKID;
            return Preview(new DeclarePreviewParam { IsExport = false, DeclareTargetId = form.DeclareTargetPKID, TypeKey = form.TypeKey, View = $"MaterialPreview{declareTargetId}" });
         }
      }


      // Get: DeclareMaterial/Submit
      // POST-Ajax: DeclareMaterial/Submit
      //[HttpPost]
      public ActionResult FormIndexEdit(DeclareParam param)
      {
         var typeKey = param.TypeKey;
         var declareTargetId = param.DeclareTargetId;
         var poge = ".职称破格";
         var isZcPoge = typeKey.IndexOf(poge) > 0;
         var decalreForm = db.DeclareFormDal.ConditionQuery(df.PeriodId == Period.PeriodId
                         & df.TeacherId == UserProfile.UserId
                         & df.TypeKey == (isZcPoge ? typeKey.Replace(poge, ".申报") : typeKey)
                         & df.DeclareTargetPKID == declareTargetId
                         , null, null, null).FirstOrDefault();

         decalreForm = decalreForm ?? new DeclareForm
         {
            DeclareTargetPKID = declareTargetId,
            TypeKey = HttpUtility.UrlEncode(typeKey),
            AllowFitResearcher = true,
            AllowFlowToDowngrade = true,
            AllowFlowToSchool = true
         };
         return PartialView(param.View, decalreForm);
      }

      [HttpPost]
      public ActionResult FormIndexEdit(DeclareForm model)
      {
         // 职称破格和申报公用一张表
         var poge = ".职称破格";
         if (model.TypeKey.IndexOf(poge) > 0)
            model.TypeKey = model.TypeKey.Replace(poge, ".申报");

         model.PeriodId = Period.PeriodId;
         model.TeacherId = UserProfile.UserId;

         if (model.DeclareFormId == 0)
         {
            //var isExists = db.DeclareFormDal.ConditionQueryCount(df.TeacherId == model.TeacherId & df.PeriodId == Period.PeriodId & df.DeclareTargetPKID == model.DeclareTargetPKID) > 0;
            //if (!isExists)
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
         return PartialView(param.View, profile);
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


         ViewBag.DeclareActives = actives;

         ViewBag.DecalreAchievements = achievements;

         return PartialView(param.View);
      }


      // Get: DeclareMaterial/Items

      public ActionResult Preview(DeclarePreviewParam param)
      {
         var u = APDBDef.BzUserProfile;
         var c = APDBDef.Company;
         var pdfRender = new HtmlRender();
         var model = new DeclarePreviewViewModel();
         var profile = db.BzUserProfileDal.PrimaryGet(UserProfile.UserId);
         var myCompnay = db.CompanyDal.PrimaryGet(profile.CompanyId);
         var form = db.DeclareFormDal.ConditionQuery(
            df.TeacherId == profile.UserId &
            df.PeriodId == Period.PeriodId &
            df.DeclareTargetPKID == 5005 &
            df.TypeKey == param.TypeKey, null, null, null).FirstOrDefault();
         form = form ?? new DeclareForm();

         var declareCompany = db.CompanyDal.PrimaryGet(form.CompanyId);
         var declareActives = GetDeclareActives(param.DeclareTargetId, profile.UserId);
         var declareAchievement = GetDeclareAchievements(param.DeclareTargetId, profile.UserId);
         var poge = ".职称破格";

         model.DeclareTargetId = 5005;
         model.Decalre = DeclareBaseHelper.DeclareTarget.GetName(param.DeclareTargetId);
         model.DeclareSubject = BzUserProfileHelper.EduSubject.GetName(form.DeclareSubjectPKID);
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
         model.Is1000 = profile.Dynamic4 == DeclareKeys.GonggJihChengy;
         model.Is2000 = profile.Dynamic4 == DeclareKeys.ZhongzJihLingxReng;
         model.Is5004 = profile.Dynamic4 == DeclareKeys.GongzsZhucr;
         model.Is5005 = profile.Dynamic4 == DeclareKeys.XuekDaitr;
         model.Is5006 = profile.Dynamic4 == DeclareKeys.GugJiaos;
         model.Is5007 = profile.Dynamic4 == DeclareKeys.JiaoxNengs;
         model.Is6000 = profile.Dynamic4 == DeclareKeys.GaodJiaoSYanxBanXuey;
         model.Comment1 = profile.Dynamic5;
         model.IsAllowDownGrade = form.AllowFlowToDowngrade;
         model.IsAllowdFlow = form.AllowFlowToSchool;
         model.DeclareActies = declareActives;
         model.DeclareAchievements = declareAchievement;
         model.Reason = form.Reason;
         // 职称破格和申报公用一张表 所以要用form IsBrokenRoles 和 param.TypeKey 一起判断
         model.IsBrokRoles = param.TypeKey.IndexOf(poge) > 0;

         if (param.IsExport != null && param.IsExport.Value)
         {
            var htmlText = pdfRender.RenderViewToString(this, param.View, model);
            byte[] pdfFile = FormatConverter.ConvertHtmlTextToPDF(htmlText);
            return new BinaryContentResult($"temp.pdf", "application/pdf", pdfFile);
         }

         return PartialView(param.View, model);
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

      #endregion

   }

}