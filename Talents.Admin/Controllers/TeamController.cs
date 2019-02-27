using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

   public class TeamController : BaseController
   {

      static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
      static APDBDef.TeamMemberTableDef tm = APDBDef.TeamMember;
      static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
      static APDBDef.TeamContentTableDef tc = APDBDef.TeamContent;
      static APDBDef.TeamActiveTableDef ta = APDBDef.TeamActive;
      static APDBDef.TeamActiveItemTableDef tai = APDBDef.TeamActiveItem;
      static APDBDef.TeamActiveResultTableDef tar = APDBDef.TeamActiveResult;
      static APDBDef.TeamSpecialCourseTableDef tsc = APDBDef.TeamSpecialCourse;
      static APDBDef.TeamSpecialCourseItemTableDef tsci = APDBDef.TeamSpecialCourseItem;
      static APDBDef.DeclareMaterialTableDef dm = APDBDef.DeclareMaterial;

      #region [ 导师访问自己的梯队 ]


      // GET: Team/Master

      public ActionResult Master(long? id)
      {
         if (id == null)
         {
            var model = db.DeclareBaseDal.PrimaryGet(UserProfile.UserId);

            if (model == null || !model.HasTeam)
            {
               throw new Exception("你还未拥有梯队导师身份，请向管理人员资讯！");
            }

            return RedirectToAction("Master", "Team", new { id = model.TeacherId });
         }
         else
         {
            return View();
         }
      }


      #endregion


      #region [ 学员访问自己的梯队 ]


      // GET: Team/Member

      public ActionResult Member(long? id)
      {
         if (id == null)
         {
            var model = db.TeamMemberDal.ConditionQuery(tm.MemberId == UserProfile.UserId, null, null, null).FirstOrDefault();

            if (model == null)
            {
               throw new Exception("你还未拥有梯队学生身份，请向管理人员资讯！");
            }

            return RedirectToAction("Member", "Team", new { id = model.TeamId });
         }
         else
         {
            return View();
         }
      }


      #endregion


      // GET: Team/Fragment

      public ActionResult Fragment(string key, long teamId, string visiter, long? courseId)
      {
         switch (key)
         {
            case TeamKeys.TidXinx:
               return TidXinx(teamId, visiter);
            case TeamKeys.DaijJih:
               return DaijJih(teamId, visiter);
            case TeamKeys.DaijHuod:
               return DaijHuod(teamId, visiter);
            case TeamKeys.DaijHuod_Edit:
               return DaijHuod_Edit(visiter, Int64.Parse(Request["activeId"]));
            case TeamKeys.DaijHuod_Timeline:
               return DaijHuod_Timeline(visiter, Int64.Parse(Request["activeId"]));
            case TeamKeys.KecShis:
               return KecShis(teamId, visiter);
            case TeamKeys.KecShis_Bianj:
               return KecShis_Bianj(teamId, visiter, courseId);
            case TeamKeys.KecShis_Anp:
               return KecShis_Anp(courseId.Value, visiter);
            case TeamKeys.KecShis_Chak:
               return KecShis_Detail(courseId.Value, visiter);
         }

         return Content("该项目无效");
      }


      public ActionResult ShareTeamActive(long id, bool isShare)
      {
         ThrowNotAjax();

         var s = APDBDef.Share;

         var teamActive = db.TeamActiveDal.PrimaryGet(id);
         teamActive.IsShare = isShare;

         if (teamActive != null)
         {
            var hasAttachment = AttachmentsExtensions.HasAttachment(db, id, UserProfile.UserId);
            if (!hasAttachment && isShare)
            {
               return Json(new
               {
                  result = AjaxResults.Success,
                  msg = "该活动没有附件！"
               });
            }


            db.BeginTrans();

            try
            {
               db.TeamActiveDal.UpdatePartial(id, new { IsShare = teamActive.IsShare });

               if (teamActive.IsShare)
                  db.ShareDal.Insert(new Share
                  {
                     ItemId = id,
                     ParentType = "ShareTeamActive",
                     CreateDate = DateTime.Now,
                     PubishDate = DateTime.Now,
                     Title = teamActive.Title,
                     Type = PicklistHelper.TeamActiveType.GetName(teamActive.ActiveType),
                     UserId = UserProfile.UserId
                  });
               else
                  db.ShareDal.ConditionDelete(s.ItemId == id);


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
            msg = "没有可以共享的活动！"
         });
      }


      #region [ 梯队信息 ]


      // GET: Team/TidXinx

      public ActionResult TidXinx(long teamId, string visiter)
      {
         var model = APQuery.select(d.Asterisk, u.RealName)
            .from(d, u.JoinInner(d.TeacherId == u.UserId))
            .where(d.TeacherId == teamId)
            .query(db, r =>
            {
               var data = new DeclareBase();
               d.Fullup(r, data, false);
               data.RealName = u.RealName.GetValue(r);

               return data;
            }).ToList().First();


         ViewBag.MemberList = db.GetMemberListById(teamId);


         if (visiter.ToLower() != "master")
         {
            return PartialView("TidXinxView", model);
         }
         else
         {
            return PartialView("TidXinx", model);
         }
      }


      #endregion


      #region [ 带教计划 ]


      // GET: Team/DaijJih
      //	POST-ajax: Team/DaijJih

      public ActionResult DaijJih(long teamId, string visiter)
      {
         var list = QueryTeamContent(teamId, TeamKeys.DaijJih);

         DaijJihModel model = new DaijJihModel();

         list.ForEach(m =>
         {
            if (m.ContentKey == TeamKeys.DaijJih_Memo1)
            {
               model.Memo1 = m.ContentValue;
               model.IsDeclare1 = m.IsDeclare;
            }
            else if (m.ContentKey == TeamKeys.DaijJih_Memo2)
            {
               model.Memo2 = m.ContentValue;
               model.IsDeclare2 = m.IsDeclare;
            }
            else if (m.ContentKey == TeamKeys.DaijJih_Memo3)
            {
               model.Memo3 = m.ContentValue;
               model.IsDeclare3 = m.IsDeclare;
            }
         });

         ViewBag.ID = teamId;

         if (visiter.ToLower() != "master")
         {
            return PartialView("DaijJihView", model);
         }
         else
         {
            return PartialView("DaijJih", model);
         }
      }

      [HttpPost]
      [ValidateInput(false)]
      public ActionResult DaijJih(DaijJihModel model)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            SetTeamContent(TeamKeys.DaijJih_Memo1, model.Memo1, model.IsDeclare1);
            SetTeamContent(TeamKeys.DaijJih_Memo2, model.Memo2, model.IsDeclare2);
            SetTeamContent(TeamKeys.DaijJih_Memo3, model.Memo3, model.IsDeclare3);

            db.Commit();
         }
         catch (System.Exception ex)
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
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 带教活动 ]


      // GET: Team/DaijHuod

      public ActionResult DaijHuod(long teamId, string visiter)
      {
         var memberCount = (int)APQuery.select(tm.MemberId.Count())
            .from(tm)
            .where(tm.TeamId == teamId)
            .executeScale(db);

         var model = APQuery.select(ta.TeamActiveId, ta.ActiveType, ta.Date, ta.Title, ta.IsShare, tar.ActiveId.Count().As("ActiveCount"),ta.IsDeclare)
            .from(ta, tar.JoinLeft(ta.TeamActiveId == tar.ActiveId))
            .group_by(ta.TeamActiveId, ta.ActiveType, ta.Date, ta.Title, ta.IsShare, ta.IsDeclare)
            .where(ta.TeamId == teamId)
            .query(db, rd =>
            {
               return new TeamActiveModel()
               {
                  TeamActiveId = ta.TeamActiveId.GetValue(rd),
                  Date = ta.Date.GetValue(rd),
                  Title = ta.Title.GetValue(rd),
                  Count = rd.GetInt32(rd.GetOrdinal("ActiveCount")),
                  MemberCount = memberCount,
                  ActiveType = ta.ActiveType.GetValue(rd),
                  IsShare = ta.IsShare.GetValue(rd),
                  IsDeclare=ta.IsDeclare.GetValue(rd)
               };
            }).ToList();


         if (visiter.ToLower() != "master")
         {
            return PartialView("DaijHuodView", model);
         }
         else
         {
            return PartialView("DaijHuod", model);
         }
      }


      //	POST-Ajax: Team/RemoveDaijHuod

      [HttpPost]
      public ActionResult RemoveDaijHuod(long id)
      {
         ThrowNotAjax();

         var period = Period;
         var teamId = (long)APQuery.select(ta.TeamId)
            .from(ta)
            .where(ta.TeamActiveId == id)
            .executeScale(db);

         db.BeginTrans();

         try
         {
            db.TeamActiveDal.PrimaryDelete(id);
            db.TeamActiveResultDal.ConditionDelete(tar.ActiveId == id);
            db.TeamActiveItemDal.ConditionDelete(tai.ActiveId == id);
            AttachmentsExtensions.DeleteAtta(db, id, AttachmentsKeys.DaijHuod_Edit);

            APQuery.update(d)
               .set(d.ActiveCount.SetValue(APSqlRawExpr.Expr("ActiveCount - 1")))
               .where(d.TeacherId == teamId)
               .execute(db);

            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);

            db.Commit();


            //记录日志
            Log(TeamKeys.DaijHuod_Delete, "删除:" + id);
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
            msg = "信息已删除！"
         });
      }


      //	POST-Ajax: Team/RemoveXueyChengg

      [HttpPost]
      public ActionResult RemoveXueyChengg(long id)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            db.TeamActiveResultDal.PrimaryDelete(id);

            AttachmentsExtensions.DeleteAtta(db, id, AttachmentsKeys.DaijHuod_XueyChengg);

            db.Commit();
         }
         catch
         {
            db.Rollback();
         }

         //记录日志
         Log(TeamKeys.DaijHuod_XueyChengg, "删除:" + id);


         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已删除！"
         });
      }


      public ActionResult DaijHuod_Edit(string visiter, long activeId)
      {
         var model = new TeamActiveDataModel() { TeamId = UserProfile.UserId, Date = DateTime.Today, AttachmentName = "" };

         if (activeId != 0)
         {
            model = APQuery.select(ta.Asterisk)
               .from(ta)
               .where(ta.TeamActiveId == activeId)
               .query(db, r =>
               {
                  return new TeamActiveDataModel()
                  {
                     ActiveType = ta.ActiveType.GetValue(r),
                     ContentValue = ta.ContentValue.GetValue(r),
                     Date = ta.Date.GetValue(r),
                     IsShow = ta.IsShow.GetValue(r),
                     Location = ta.Location.GetValue(r),
                     TeamActiveId = ta.TeamActiveId.GetValue(r),
                     TeamId = ta.TeamId.GetValue(r),
                     Title = ta.Title.GetValue(r)
                  };
               }).First();

            var atta = AttachmentsExtensions.GetAttachment(
               AttachmentsExtensions.GetAttachmentList(db, activeId, AttachmentsKeys.DaijHuod_Edit));
            model.AttachmentName = atta.Name;
            model.AttachmentUrl = atta.Url;
         };


         return PartialView("DaijHuod_Edit", model);
      }

      [HttpPost]
      [ValidateInput(false)]
      public ActionResult DaijHuod_Edit(long? id, TeamActiveDataModel model)
      {
         ThrowNotAjax();

         var atta = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.DaijHuod_Edit,
            Name = model.AttachmentName,
            Url = model.AttachmentUrl,
            UserId = UserProfile.UserId
         };

         if (id == null)
         {
            db.BeginTrans();

            try
            {
               var data = new TeamActive()
               {
                  ActiveType = model.ActiveType,
                  ContentValue = model.ContentValue,
                  Date = model.Date,
                  IsShow = model.IsShow,
                  Location = model.Location,
                  TeamId = model.TeamId,
                  Title = model.Title,
                  IsShare = model.IsShare,
                  Creator = UserProfile.UserId,
                  CreateDate = DateTime.Now
               };

               db.TeamActiveDal.Insert(data);

               APQuery.update(d)
                  .set(d.ActiveCount.SetValue(APSqlRawExpr.Expr("ActiveCount + 1")))
                  .where(d.TeacherId == model.TeamId)
                  .execute(db);

               atta.JoinId = data.TeamActiveId;
               AttachmentsExtensions.InsertAtta(db, atta);

               db.Commit();
            }
            catch
            {
               db.Rollback();
            }
         }
         else
         {
            db.BeginTrans();

            try
            {
               db.TeamActiveDal.UpdatePartial(id.Value, new
               {
                  model.Title,
                  model.Location,
                  model.IsShow,
                  model.ContentValue,
                  model.ActiveType,
                  model.Date,
                  model.IsShare,
                  Modifier = UserProfile.UserId,
                  ModifyDate = DateTime.Now
               });

               AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.DaijHuod_Edit);
               atta.JoinId = id.Value;
               AttachmentsExtensions.InsertAtta(db, atta);

               db.Commit();
            }

            catch
            {
               db.Rollback();
            }

         }


         //记录日志
         var doSomthing = id == null ? "新增:" + id : "修改:" + id;
         if (!string.IsNullOrEmpty(atta.Name))
            doSomthing += string.Format(" 并且上传了附件:{0}", atta.Name);

         Log(TeamKeys.DaijHuod_Edit, doSomthing);


         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存！"
         });
      }


      #endregion


      #region [ 带教活动内容 ]


      // GET: Team/Timeline
      //	POST-Ajax: Team/RemoveDaijHuod_Item

      public ActionResult DaijHuod_Timeline(string visiter, long activeId)
      {
         var model = APQuery.select(u.RealName, tai.SendDate, tai.ItemContent, tai.ItemId, tai.MemberId, ta.TeamId)
            .from(tai,
                  u.JoinInner(tai.MemberId == u.UserId),
                  ta.JoinInner(tai.ActiveId == ta.TeamActiveId))
            .where(tai.ActiveId == activeId)
            .order_by(tai.SendDate.Asc)
            .query(db, rd =>
            {
               return new TeamActiveItemViewModel()
               {
                  MemberName = u.RealName.GetValue(rd),
                  SendDate = tai.SendDate.GetValue(rd),
                  ItemContent = tai.ItemContent.GetValue(rd),
                  ItemId = tai.ItemId.GetValue(rd),
                  MemberId = tai.MemberId.GetValue(rd),
                  TeamId = ta.TeamId.GetValue(rd)
               };
            }).ToList();


         ViewBag.activeId = activeId;

         return PartialView("DaijHuod_Timeline", model);
      }

      [HttpPost]
      public ActionResult RemoveDaijHuod_Item(long id)
      {
         ThrowNotAjax();

         db.TeamActiveItemDal.PrimaryDelete(id);


         //记录日志
         Log(TeamKeys.DaijHuod_HuodNeir, "删除:" + id);


         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已删除!"
         });
      }


      #endregion


      #region [ 定向性课程实施 ]


      // GET: Team/KecShis

      public ActionResult KecShis(long teamId, string visiter)
      {
         var t = APDBDef.TeamSpecialCourse;

         var model = APQuery.select(tsc.CourseId, tsc.StartDate, tsc.EndDate, tsc.Title, tsc.IsDeclare,
            tsci.CourseId.Count().As("ItemCount"))
            .from(tsc, tsci.JoinLeft(tsc.CourseId == tsci.CourseId))
            .group_by(tsc.CourseId, tsc.StartDate, tsc.EndDate, tsc.Title, tsc.IsDeclare)
            .where(tsc.TeamId == teamId)
            .query(db, rd =>
            {
               TeamSpecialCourseViewModel data = new TeamSpecialCourseViewModel();
               t.Fullup(rd, data, false);
               data.ItemCount = Convert.ToInt32(rd["ItemCount"]);

               return data;
            }).ToList();


         if (visiter.ToLower() != "master")
         {
            return PartialView("KecShisView", model);
         }
         else
         {
            return PartialView("KecShis", model);
         }
      }


      //	POST_Ajax: Team/RemoveKecShis

      [HttpPost]
      public ActionResult RemoveKecShis(long id)
      {
         ThrowNotAjax();

         var period = Period;

         db.BeginTrans();

         try
         {
            db.TeamSpecialCourseDal.PrimaryDelete(id);
            db.TeamSpecialCourseItemDal.ConditionDelete(tsci.CourseId == id);
            db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);

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

         //记录日志
         Log(TeamKeys.KecShis_Delete, "删除:" + id);

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已删除！"
         });
      }


      //	GET: Team/KecShis_Bianj
      //	POST-Ajax: Team/KecShis_Bianj

      public ActionResult KecShis_Bianj(long teamId, string visiter, long? courseId)
      {
         var model = courseId == null ? new TeamSpecialCourse()
         {
            TeamId = teamId,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(7)
         } :
                                        db.TeamSpecialCourseDal.PrimaryGet(courseId.Value);


         return PartialView("kecShis_Bianj", model);
      }

      [HttpPost]
      [ValidateInput(false)]
      public ActionResult KecShis_Bianj(long? id, TeamSpecialCourse model)
      {
         ThrowNotAjax();


         if (id == null)
         {
            model.Creator = UserProfile.UserId;
            model.CreateDate = DateTime.Now;
            db.TeamSpecialCourseDal.Insert(model);
         }
         else
         {
            model.ModifyDate = DateTime.Now;
            model.Modifier = UserProfile.UserId;
            db.TeamSpecialCourseDal.Update(model);
         }

         //记录日志
         var doSomthing = id == null ? "新增:" + id : "修改:" + id;
         Log(TeamKeys.KecShis_Bianj, doSomthing);


         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存"
         });
      }


      // GET: Team/KecShis_Anp

      public ActionResult KecShis_Anp(long courseId, string visiter)
      {
         var model = db.TeamSpecialCourseItemDal.ConditionQuery(tsci.CourseId == courseId,
            null, null, null).ToList();


         return PartialView("KecShis_Anp", model);
      }


      // POST-Ajax: Team/KecShis_Anp

      [HttpPost]
      public ActionResult RemoveKecShis_Anp(long id)
      {
         ThrowNotAjax();

         db.TeamSpecialCourseItemDal.PrimaryDelete(id);


         //记录日志
         Log(TeamKeys.KecShis_Anp, "删除:" + id);


         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已删除！"
         });
      }


      //	GET:　Team/KecShis_Detail

      public ActionResult KecShis_Detail(long id, string visiter)
      {
         var model = db.TeamSpecialCourseDal.PrimaryGet(id);

         return PartialView("KecShis_Detail", model);
      }


      #endregion


      #region [Helper]


      private List<TeamContent> QueryTeamContent(long teamId, string startWith)
         => db.TeamContentDal.ConditionQuery(
            tc.TeamId == teamId & tc.ContentKey.StartWith(startWith),
            null, null, null);


      private void SetTeamContent(string key, string value, bool isDeclare, string type = "String")
      {
         value = value.Trim();

         var maybeId = APQuery.select(tc.TeamContentId)
            .from(tc)
            .where(tc.TeamId == UserProfile.UserId & tc.ContentKey == key)
            .executeScale(db);

         if (maybeId == null)
         {
            db.TeamContentDal.Insert(new TeamContent
            {
               TeamId = UserProfile.UserId,
               ContentKey = key,
               ContentValue = value,
               ContentDataType = type,
               Creator = UserProfile.UserId,
               CreateDate = DateTime.Now,
               IsDeclare = isDeclare,
            });
         }
         else
         {
            APQuery.update(tc)
               .set(tc.ContentValue.SetValue(value))
               .set(tc.Modifier.SetValue(UserProfile.UserId))
               .set(tc.ModifyDate.SetValue(DateTime.Now))
               .set(tc.IsDeclare.SetValue(isDeclare))
               .where(tc.TeamContentId == (long)maybeId)
               .execute(db);
         }
      }


      private void Log(string where, string doSomthing)
      {
         LogFactory.Create().Log(new LogModel
         {
            UserID = UserProfile.UserId,
            OperationDate = DateTime.Now,
            Where = where,
            DoSomthing = doSomthing
         });
      }


      #endregion

   }

}