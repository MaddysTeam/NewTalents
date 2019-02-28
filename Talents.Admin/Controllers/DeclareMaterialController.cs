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

         var period = Period;
         var resume = db.DeclareResumeDal.PrimaryGet(id);
         if (resume != null)
         {
            resume.IsDeclare = isDeclare;

            db.BeginTrans();

            try
            {
               db.DeclareResumeDal.UpdatePartial(id, new { isDeclare = resume.IsDeclare });

               DeclareMaterialHelper.AddDeclareMaterial(resume, period, db);

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



      // POST-Ajax: DeclareMaterial/DeclareOrgConst  申报档案建设

      [HttpPost]
      [DecalrePeriod]
      public ActionResult DeclareOrgConst(long id, bool isDeclare)
      {
         ThrowNotAjax();

         var period = Period;
         var org = db.DeclareOrgConstDal.PrimaryGet(id);
         org.IsDeclare = isDeclare;

         if (org != null)
         {
            db.BeginTrans();

            try
            {
               db.DeclareOrgConstDal.UpdatePartial(id, new { isDeclare = org.IsDeclare });

               DeclareMaterialHelper.AddDeclareMaterial(org, period, db);

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


      // POST-Ajax: DeclareMaterial/DeclareTeamActive 申报团队活动

      [HttpPost]
      [DecalrePeriod]
      public ActionResult DeclareTeamActive(long id, bool isDeclare)
      {
         ThrowNotAjax();

         var period = Period;

         var teamActive = db.TeamActiveDal.PrimaryGet(id);
         teamActive.IsDeclare = isDeclare;

         if (teamActive != null)
         {
            db.BeginTrans();

            try
            {
               db.TeamActiveDal.UpdatePartial(id, new { IsDeclare = teamActive.IsDeclare });

               DeclareMaterialHelper.AddDeclareMaterial(teamActive, period, db);

               //if (teamActive.IsDeclare)
               //   db.DeclareMaterialDal.Insert(new DeclareMaterial
               //   {
               //      ItemId = id,
               //      ParentType = "DeclareTeamActive",
               //      CreateDate = DateTime.Now,
               //      PubishDate = DateTime.Now,
               //      Title = teamActive.Title,
               //      Type = PicklistHelper.TeamActiveType.GetName(teamActive.ActiveType),
               //      TeacherId = UserProfile.UserId,
               //      PeriodId = period.PeriodId
               //   });
               //else
               //   db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);


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


      // POST-Ajax: DeclareMaterial/DeclareTeamActive 申报定向性课程实施

      [HttpPost]
      [DecalrePeriod]
      public ActionResult DeclareTeamSpecialCourse(long id, bool isDeclare)
      {
         ThrowNotAjax();

         var period = Period;

         var specialCourse = db.TeamSpecialCourseDal.PrimaryGet(id);
         specialCourse.IsDeclare = isDeclare;

         if (specialCourse != null)
         {
            db.BeginTrans();

            try
            {
               db.TeamSpecialCourseDal.UpdatePartial(id, new { IsDeclare = specialCourse.IsDeclare });

               DeclareMaterialHelper.AddDeclareMaterial(specialCourse,period,db);

               //if (specialCourse.IsDeclare)
               //   db.DeclareMaterialDal.Insert(new DeclareMaterial
               //   {
               //      ItemId = id,
               //      ParentType = "DeclareTeamSpecialCourse",
               //      CreateDate = DateTime.Now,
               //      PubishDate = DateTime.Now,
               //      Title = specialCourse.Title,
               //      Type = TeamKeys.KecShis_Chak,
               //      TeacherId = UserProfile.UserId,
               //      PeriodId = period.PeriodId
               //   });
               //else
               //   db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);


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

         db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == Period.PeriodId);

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

         ViewBag.DeclareAchievement = QueryDeclareAchievementList(id, currentPeriod);

         ViewBag.DeclareOrgConst = db.DeclareOrgConstDal.ConditionQuery(d.TeacherId == id, null, null, null);

         ViewBag.DeclareBase = GetDeclareBase(id);

         ViewBag.TeamContent = db.TeamContentDal.ConditionQuery(tc.TeamId == id & tc.IsDeclare == true, null, null, null);

         ViewBag.TeamMember = db.GetMemberListById(id);

         ViewBag.TeamSpecialCourse = GetTeamSpecialCourseList(id);

         ViewBag.TeamActive = GetTeamActiveList(id, currentPeriod);

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


      private List<DeclareAchievement> QueryDeclareAchievementList(long teacherId, DeclarePeriod period)
       => db.DeclareAchievementDal.ConditionQuery(
          dac.TeacherId == teacherId & dac.CreateDate >= period.BeginDate
          & dac.CreateDate <= period.EndDate
          & dac.IsDeclare==true,
          null, null, null);


      private Dictionary<long, TeamActiveViewModel> GetTeamActiveList(long teamId, DeclarePeriod current)
      {
         var p = APDBDef.PicklistItem;


         var list = new Dictionary<long, TeamActiveViewModel>();

         if (current == null) throw new ApplicationException("当前没有考核周期");

         list = APQuery.select(t.Asterisk, p.Name)
            .from(t, p.JoinInner(t.ActiveType == p.PicklistItemId))
            .where(t.TeamId == teamId
            & t.CreateDate >= current.BeginDate & t.CreateDate <= current.EndDate
            & t.Date >= current.BeginDate & t.Date <= current.EndDate
            & t.IsDeclare == true)
            .query(db, r =>
            {
               return new TeamActiveViewModel()
               {
                  Title = t.Title.GetValue(r),
                  ContentValue = t.ContentValue.GetValue(r),
                  Date = t.Date.GetValue(r),
                  Location = t.Location.GetValue(r),
                  TeamActiveId = t.TeamActiveId.GetValue(r),
                  TypeName = p.Name.GetValue(r)
               };
            }).ToDictionary(m => m.TeamActiveId);

         GetTeamActiveResult(list, teamId);

         return list;
      }


      private void GetTeamActiveResult(Dictionary<long, TeamActiveViewModel> list, long teamId)
      {
         APQuery.select(t.TeamActiveId, tar.ActiveResult, u.RealName, tar.ResultId)
            .from(t,
                  tar.JoinInner(t.TeamActiveId == tar.ActiveId),
                  u.JoinInner(tar.MemberId == u.UserId))
            .where(t.TeamId == teamId)
            .query(db, r =>
            {
               var ActiveId = t.TeamActiveId.GetValue(r);
               var Name = u.RealName.GetValue(r);
               var Content = tar.ActiveResult.GetValue(r);
               var ResultId = tar.ResultId.GetValue(r);
               if (list.ContainsKey(ActiveId))
               {
                  list[ActiveId].Result.Add(new TeamActiveResultViewModel
                  {
                     ResultId = ResultId,
                     MemberName = Name,
                     ActiveResult = Content
                  });
               }

               return ActiveId;
            }).ToList();
      }

      private Dictionary<long, TeamSpecialCourseModel> GetTeamSpecialCourseList(long teamId)
      {
         var list = new Dictionary<long, TeamSpecialCourseModel>();

         list = APQuery.select(tsc.Asterisk)
            .from(tsc)
            .where(tsc.TeamId == teamId & tsc.IsDeclare == true)
            .query(db, r =>
            {
               return new TeamSpecialCourseModel()
               {
                  CourseId = tsc.CourseId.GetValue(r),
                  TeamId = tsc.TeamId.GetValue(r),
                  Title = tsc.Title.GetValue(r),
                  StartDate = tsc.StartDate.GetValue(r),
                  EndDate = tsc.EndDate.GetValue(r),
                  CourseTarget = tsc.CourseTarget.GetValue(r),
                  CoursePlan = tsc.CoursePlan.GetValue(r),
                  CourseRecords = tsc.CourseRecords.GetValue(r),
                  CourseResults = tsc.CourseResults.GetValue(r),
                  CourseSummary = tsc.CourseSummary.GetValue(r),
                  Remark = tsc.Remark.GetValue(r),
                  TotalCount = tsc.TotalCount.GetValue(r),
                  MemberCount = tsc.MemberCount.GetValue(r),
                  MemberRecord = tsc.MemberRecord.GetValue(r)
               };
            }).ToDictionary(m => m.CourseId);

         GetTeamSpecialCourseItemList(list, teamId);


         return list;
      }


      private void GetTeamSpecialCourseItemList(Dictionary<long, TeamSpecialCourseModel> list, long teamId)
      {
         var tsci = APDBDef.TeamSpecialCourseItem;

         APQuery.select(tsci.Asterisk)
            .from(tsc, tsci.JoinInner(tsc.CourseId == tsci.CourseId))
            .where(tsc.TeamId == teamId)
            .query(db, r =>
            {
               var CourseId = tsci.CourseId.GetValue(r);
               var ItemDate = tsci.ItemDate.GetValue(r);
               var Location = tsci.Location.GetValue(r);
               var Title = tsci.Title.GetValue(r);
               var Content = tsci.Content.GetValue(r);
               var ActivityType = tsci.ActivityType.GetValue(r);
               var Speaker = tsci.Speaker.GetValue(r);
               var Remark = tsci.Remark.GetValue(r);

               if (list.ContainsKey(CourseId))
               {
                  list[CourseId].Item.Add(new TeamSpecialCourseItem
                  {
                     ItemDate = ItemDate,
                     Location = Location,
                     Title = Title,
                     Content = Content,
                     ActivityType = ActivityType,
                     Speaker = Speaker,
                     Remark = Remark
                  });
               }
               return CourseId;
            }).ToList();
      }

      private string SubString(string str)
    => str.Length > 50 ? str.Substring(0, 50) + "..." : str;

      #endregion

   }
}