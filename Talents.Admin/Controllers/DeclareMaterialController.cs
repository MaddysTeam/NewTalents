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


      // POST-Ajax: DeclareMaterial/DeclareContent  申报简历材料

      [HttpPost]
      public ActionResult DeclareResume(long id, bool isDeclare)
      {
         ThrowNotAjax();

         var period = db.GetCurrentDeclarePeriod();
         var resume = db.DeclareResumeDal.PrimaryGet(id);
         resume.IsDeclare = isDeclare;

         if (resume != null)
         {
            db.BeginTrans();

            try
            {
               db.DeclareResumeDal.UpdatePartial(id, new { isDeclare = resume.IsDeclare });

               if (resume.IsDeclare)
                  db.DeclareMaterialDal.Insert(new DeclareMaterial
                  {
                     ItemId = id,
                     ParentType = "DeclareResume",
                     CreateDate = DateTime.Now,
                     PubishDate = DateTime.Now,
                     Title = resume.Title,
                     Type = DeclareKeys.ZisFaz_GerJianl,
                     TeacherId = UserProfile.UserId,
                     PeriodId = period.PeriodId
                  });
               else
                  db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);


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
            msg = "没有可以申报的活动！"
         });

      }

     
      // POST-Ajax: DeclareMaterial/DeclareActive  申报活动材料

      [HttpPost]
      public ActionResult DeclareActive(long id, bool isDeclare)
      {
         ThrowNotAjax();

         var period = db.GetCurrentDeclarePeriod();
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
                     TeacherId = UserProfile.UserId,
                     PeriodId = period.PeriodId
                  });
               else
                  db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);


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



      // POST-Ajax: DeclareMaterial/DeclareOrgConst  申报档案建设

      [HttpPost]
      public ActionResult DeclareOrgConst(long id, bool isDeclare)
      {
         ThrowNotAjax();

         var period = db.GetCurrentDeclarePeriod();
         var org = db.DeclareOrgConstDal.PrimaryGet(id);
         org.IsDeclare = isDeclare;

         if (org != null)
         {
            db.BeginTrans();

            try
            {
               db.DeclareOrgConstDal.UpdatePartial(id, new { isDeclare = org.IsDeclare });

               if (org.IsDeclare)
                  db.DeclareMaterialDal.Insert(new DeclareMaterial
                  {
                     ItemId = id,
                     ParentType = "DeclareOrgConst",
                     CreateDate = DateTime.Now,
                     PubishDate = DateTime.Now,
                     Title = org.Content,
                     Type = DeclareKeys.ZhidJians_DangaJians,
                     TeacherId = UserProfile.UserId,
                     PeriodId = period.PeriodId
                  });
               else
                  db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);


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

         var period = db.GetCurrentDeclarePeriod();
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
                     TeacherId = UserProfile.UserId,
                     PeriodId = period.PeriodId
                  });
               else
                  db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);


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


      // POST-Ajax: DeclareMaterial/DeclareTeamActive 申报团队活动

      [HttpPost]
      public ActionResult DeclareTeamActive(long id, bool isDeclare)
      {
         ThrowNotAjax();

         var period = db.DeclarePeriodDal.ConditionQuery(p.IsCurrent == true, null, null, null).FirstOrDefault();
         if (period == null)
            throw new ApplicationException("请设置申报周期");

         var teamActive = db.TeamActiveDal.PrimaryGet(id);
         teamActive.IsDeclare = isDeclare;

         if (teamActive != null)
         {
            db.BeginTrans();

            try
            {
               db.TeamActiveDal.UpdatePartial(id, new { IsDeclare = teamActive.IsDeclare });

               if (teamActive.IsDeclare)
                  db.DeclareMaterialDal.Insert(new DeclareMaterial
                  {
                     ItemId = id,
                     ParentType = "DeclareTeamActive",
                     CreateDate = DateTime.Now,
                     PubishDate = DateTime.Now,
                     Title = teamActive.Title,
                     Type = PicklistHelper.TeamActiveType.GetName(teamActive.ActiveType),
                     TeacherId = UserProfile.UserId,
                     PeriodId = period.PeriodId
                  });
               else
                  db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);


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
      public ActionResult DeclareContent(long id, bool isDeclare)
      {
         ThrowNotAjax();

         var period = db.GetCurrentDeclarePeriod();

         db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);

         APQuery.update(dc).set(dc.IsDeclare.SetValue(false)).where(dc.DeclareContentId == id).execute(db);

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "操作成功！"
         });

      }


      // GET: DeclareMaterial/Overview

      public ActionResult Overview(long id)
      {
         var dr = APDBDef.DeclareResume;
         var dp = APDBDef.DeclarePeriod;
         var t = APDBDef.TeamActive;
         var tai = APDBDef.TeamActiveItem;
         var tar = APDBDef.TeamActiveResult;
         var tsc = APDBDef.TeamSpecialCourse;
         var tm = APDBDef.TeamMember;
         var d = APDBDef.DeclareOrgConst;
         var tc = APDBDef.TeamContent;
         var dcl = APDBDef.DeclareBase;

         var currentPeriod = db.DeclarePeriodDal.ConditionQuery(dp.IsCurrent == true, null, null, null).FirstOrDefault();
         if (currentPeriod == null) throw new ApplicationException("当前不再任何申报周期！");

         ViewBag.DeclareContent = QueryDeclareContent(id, currentPeriod);

         ViewBag.DeclareResume = db.DeclareResumeDal.ConditionQuery(dr.TeacherId == id & dr.IsDeclare == true, null, null, null);

         ViewBag.DeclareActive = QueryDeclareActiveList(id, currentPeriod);

         ViewBag.DeclareAchievement = new List<DeclareAchievement>(); // QueryDeclareAchievementList(id, currentPeriod);

         ViewBag.DeclareOrgConst = db.DeclareOrgConstDal.ConditionQuery(d.TeacherId == id, null, null, null);

         ViewBag.DeclareBase = GetDeclareBase(id);

         ViewBag.TeamContent = db.TeamContentDal.ConditionQuery(tc.TeamId == id & tc.IsDeclare == true, null, null, null);

         ViewBag.TeamMember = db.GetMemberListById(id);

         ViewBag.TeamSpecialCourse = new Dictionary<long, TeamSpecialCourseModel>(); // GetTeamSpecialCourseList(id);

         ViewBag.TeamActive = new Dictionary<long, TeamActiveViewModel>(); // GetTeamActiveList(id, currentPeriod);

         var subQuery = APQuery.select(tm.MemberId).from(tm).where(tm.TeamId == id);
         var memberDaList = APQuery.select(da.Asterisk, dcl.DeclareTargetPKID)
            .from(da, dcl.JoinInner(da.TeacherId == dcl.TeacherId))
            .where(
            da.CreateDate >= currentPeriod.BeginDate & da.CreateDate <= currentPeriod.EndDate &
            da.Date >= currentPeriod.BeginDate & da.Date <= currentPeriod.EndDate &
            da.TeacherId.In(subQuery) & (da.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk |
                                                da.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_Yantk |
                                                da.ActiveKey == DeclareKeys.ZisFaz_JiaoxHuod_JiaoxPingb))
            .query(db, r =>
            {
               var data = new DeclareActiveDataModel();
               da.Fullup(r, data, false);
               data.TargetId = dcl.DeclareTargetPKID.GetValue(r);
               return data;
            }).ToList();

         var memberDacList = APQuery.select(dac.Asterisk, dcl.DeclareTargetPKID)
            .from(dac, dcl.JoinInner(dac.TeacherId == dcl.TeacherId))
            .where(
            dac.CreateDate >= currentPeriod.BeginDate & dac.CreateDate <= currentPeriod.EndDate &
            dac.TeacherId.In(subQuery) & (dac.AchievementKey == DeclareKeys.ZisFaz_KeyChengg_KetYanj | dac.AchievementKey == DeclareKeys.ZisFaz_KeyChengg_FabLunw))
            .query(db, r =>
            {
               var data = new DeclareAchievementDataModel();
               dac.Fullup(r, data, false);
               data.TargetId = dcl.DeclareTargetPKID.GetValue(r);
               return data;
            }).ToList();

         ViewBag.MemberDaList = memberDaList;
         ViewBag.MemberDacList = memberDacList;

         return View();

      }


      #region [ Helper ]

      private List<DeclareContent> QueryDeclareContent(long teacherId, DeclarePeriod period)
      => db.DeclareContentDal.ConditionQuery(
         dc.TeacherId == teacherId
         & dc.CreateDate >= period.BeginDate
         & dc.CreateDate <= period.EndDate
         & dc.ModifyDate >= period.BeginDate
         & dc.ModifyDate <= period.EndDate
         & dc.IsDeclare == true,
         null, null, null);


      private DeclareBase GetDeclareBase(long teacherId)
      {
         var t = APDBDef.DeclareBase;

         var declare = APQuery.select(t.Asterisk, u.RealName)
            .from(t, u.JoinInner(t.TeacherId == u.UserId))
            .where(t.TeacherId == teacherId)
            .query(db, rd =>
            {
               return new DeclareBase
               {
                  TeacherId = t.TeacherId.GetValue(rd),
                  DeclareTargetPKID = t.DeclareTargetPKID.GetValue(rd),
                  DeclareSubjectPKID = t.DeclareSubjectPKID.GetValue(rd),
                  DeclareStagePKID = t.DeclareStagePKID.GetValue(rd),
                  AllowFlowToSchool = t.AllowFlowToSchool.GetValue(rd),
                  AllowFitResearcher = t.AllowFitResearcher.GetValue(rd),
                  HasTeam = t.HasTeam.GetValue(rd),
                  TeamName = t.TeamName.GetValue(rd),
                  MemberCount = t.MemberCount.GetValue(rd),
                  ActiveCount = t.ActiveCount.GetValue(rd),
                  RealName = u.RealName.GetValue(rd)
               };
            }).ToList().First();


         return declare;
      }


      private List<DeclareActive> QueryDeclareActiveList(long teacherId, DeclarePeriod period)
        => db.DeclareActiveDal.ConditionQuery(
           da.TeacherId == teacherId
           & da.CreateDate >= period.BeginDate
           & da.CreateDate <= period.EndDate
           & da.Date >= period.BeginDate
           & da.Date <= period.EndDate
           & da.IsDeclare == true,
           null, null, null);

      private string SubString(string str)
    => str.Length > 50 ? str.Substring(0, 50) + "..." : str;

      #endregion

   }
}