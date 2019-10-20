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

      #region [ 领衔人访问自己的团队 ]


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


      #region [ 学员访问自己的团队 ]


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
            //case TeamKeys.DaijHuod_Timeline:
            //   return DaijHuod_Timeline(visiter, Int64.Parse(Request["activeId"]));
            //case TeamKeys.KecShis:
            //   return KecShis(teamId, visiter);
            //case TeamKeys.KecShis_Bianj:
            //   return KecShis_Bianj(teamId, visiter, courseId);
            //case TeamKeys.KecShis_Anp:
            //   return KecShis_Anp(courseId.Value, visiter);
            //case TeamKeys.KecShis_Chak:
            //   return KecShis_Detail(courseId.Value, visiter);

            case TeamKeys.TuanDXinx:
               return TuanDXinx(teamId, visiter);
            case TeamKeys.YanxHuod:
               return YanxHuod(teamId, visiter);
            case TeamKeys.YanxHuod_Edit:
               return YanxHuod_Edit(visiter, Int64.Parse(Request["activeId"]));
            case TeamKeys.TuanDZhidJians:
               return TuanDZhidJians(teamId, visiter);
            case TeamKeys.TuanDXiangm:
               return TuanDXinagm(teamId);
            case TeamKeys.TuanDGerXinx:
               return TuanDGerXinx();
            case TeamKeys.TuanDGerJh:
               return TuanDGerJih();
            case TeamKeys.TuanDZiXiangm:
               return TuanDZiXiangm();
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


      #region [ 团队信息 ]


      // GET: Team/TidXinx

      public ActionResult TuanDXinx(long teamId, string visiter)
      {
         var model = APQuery.select(d.Asterisk, u.RealName)
            .from(d, u.JoinInner(d.TeacherId == u.UserId))
            .where(d.TeacherId == teamId)
            .query(db, r =>
            {
               var data = new DeclareBase();
               d.Fullup(r, data, false);
               data.RealName = u.RealName.GetValue(r);
               data.TeamName = $"{data.DeclareStage}{data.DeclareSubject}{data.RealName }";

               return data;
            }).ToList().First();


         ViewBag.MemberList = db.GetMemberListById(teamId);


         if (visiter.ToLower() != "master")
         {
            return PartialView("TuanDXinxView", model);
         }
         else
         {
            return PartialView("TuanDXinx", model);
         }
      }


      #endregion


      #region [ 研修活动 ]


      // GET: Team/YanxHuod

      public ActionResult YanxHuod(long teamId, string visiter)
      {
         var memberCount = (int)APQuery.select(tm.MemberId.Count())
            .from(tm)
            .where(tm.TeamId == teamId)
            .executeScale(db);

         var model = APQuery.select(ta.TeamActiveId, ta.ActiveType, ta.Date, ta.Title, ta.IsShare, tar.ActiveId.Count().As("ActiveCount"), ta.IsDeclare)
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
                  IsDeclare = ta.IsDeclare.GetValue(rd)
               };
            }).ToList();


         if (visiter.ToLower() != "master")
         {
            return PartialView("YanxHuodView", model);
         }
         else
         {
            return PartialView("YanxHuod", model);
         }
      }


      //	POST-Ajax: Team/RemoveYanxHuod

      [HttpPost]

      public ActionResult RemoveYanxHuod(long id)
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
            AttachmentsExtensions.DeleteAtta(db, id, AttachmentsKeys.YanXHuod_Edit);

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

            AttachmentsExtensions.DeleteAtta(db, id, AttachmentsKeys.YanXHuod_XueyChengg);

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


      public ActionResult YanxHuod_Edit(string visiter, long activeId)
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
                     Title = ta.Title.GetValue(r),
                     IsDeclare = ta.IsDeclare.GetValue(r)
                  };
               }).First();

            var atta = AttachmentsExtensions.GetAttachment(
               AttachmentsExtensions.GetAttachmentList(db, activeId, AttachmentsKeys.YanXHuod_Edit));
            model.AttachmentName = atta.Name;
            model.AttachmentUrl = atta.Url;
         };


         return PartialView("YanxHuod_Edit", model);
      }

      [HttpPost]
      [ValidateInput(false)]

      public ActionResult YanxHuod_Edit(long? id, TeamActiveDataModel model)
      {
         ThrowNotAjax();

         var atta = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.YanXHuod_Edit,
            Name = model.AttachmentName,
            Url = model.AttachmentUrl,
            UserId = UserProfile.UserId
         };

         TeamActive data = null;

         if (id == null)
         {
            db.BeginTrans();

            try
            {
               data = new TeamActive()
               {
                  ActiveType = model.ActiveType,
                  ContentValue = model.ContentValue,
                  Date = model.Date,
                  IsShow = model.IsShow,
                  Location = model.Location,
                  TeamId = model.TeamId,
                  Title = model.Title,
                  //IsShare = model.IsShare,
                  IsDeclare = model.IsDeclare,
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
                  ModifyDate = DateTime.Now,
                  model.IsDeclare
               });

               AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.YanXHuod_Edit);
               atta.JoinId = id.Value;
               AttachmentsExtensions.InsertAtta(db, atta);

               db.Commit();
            }

            catch
            {
               db.Rollback();
            }

            data = db.TeamActiveDal.PrimaryGet(id.Value);

         }

         DeclareMaterialHelper.AddDeclareMaterial(data, Period, db);


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


      #region [ 研修活动内容 ]


      // GET: Team/Timeline
      //	POST-Ajax: Team/RemoveDaijHuod_Item

      public ActionResult YanxHuod_Timeline(string visiter, long activeId)
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

         return PartialView("YanxHuod_Timeline", model);
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


      #region [ 制度建设和规划 ]

      public ActionResult TuanDZhidJians(long id, string visiter)
      {
         var allAts = AttachmentsExtensions.GetAttachmentList(db, id);
         var ats1 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_ZhidJians);
         var ats2 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_SannGuih);
         var ats3 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqJih);
         var ats4 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqJih2);
         var ats5 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqJih3);
         var ats6 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqJih4);
         var ats7 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqJih5);
         var ats8 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqJih6);

         var ats9 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqHuodAnp);
         var ats10 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqHuodAnp2);
         var ats11 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqHuodAnp3);
         var ats12 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqHuodAnp4);
         var ats13 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqHuodAnp5);
         var ats14 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_XueqHuodAnp6);


         var at1 = AttachmentsExtensions.GetAttachment(ats1);
         var at2 = AttachmentsExtensions.GetAttachment(ats2);
         var at3 = AttachmentsExtensions.GetAttachment(ats3);
         var at4 = AttachmentsExtensions.GetAttachment(ats4);
         var at5 = AttachmentsExtensions.GetAttachment(ats5);
         var at6 = AttachmentsExtensions.GetAttachment(ats6);
         var at7 = AttachmentsExtensions.GetAttachment(ats7);
         var at8 = AttachmentsExtensions.GetAttachment(ats8);

         var at9 = AttachmentsExtensions.GetAttachment(ats9);
         var at10 = AttachmentsExtensions.GetAttachment(ats10);
         var at11 = AttachmentsExtensions.GetAttachment(ats11);
         var at12 = AttachmentsExtensions.GetAttachment(ats12);
         var at13 = AttachmentsExtensions.GetAttachment(ats13);
         var at14 = AttachmentsExtensions.GetAttachment(ats14);

         return PartialView("TuanDZhidJians", new ZhidJiansViewModel()
         {
            AttachmentName1 = at1.Name,
            AttachmentName2 = at2.Name,
            AttachmentUrl1 = at1.Url,
            AttachmentUrl2 = at2.Url,
            SemesterAttachmentName1 = at3.Name,
            SemesterAttachmentUrl1 = at3.Url,
            SemesterAttachmentName2 = at4.Name,
            SemesterAttachmentUrl2 = at4.Url,
            SemesterAttachmentName3 = at5.Name,
            SemesterAttachmentUrl3 = at5.Url,
            SemesterAttachmentName4 = at6.Name,
            SemesterAttachmentUrl4 = at6.Url,
            SemesterAttachmentName5 = at7.Name,
            SemesterAttachmentUrl5 = at7.Url,
            SemesterAttachmentName6 = at8.Name,
            SemesterAttachmentUrl6 = at8.Url,

            SemesterActiveAttachmentName1 = at9.Name,
            SemesterActiveAttachmentUrl1 = at9.Url,
            SemesterActiveAttachmentName2 = at10.Name,
            SemesterActiveAttachmentUrl2 = at10.Url,
            SemesterActiveAttachmentName3 = at11.Name,
            SemesterActiveAttachmentUrl3 = at11.Url,
            SemesterActiveAttachmentName4 = at12.Name,
            SemesterActiveAttachmentUrl4 = at12.Url,
            SemesterActiveAttachmentName5 = at13.Name,
            SemesterActiveAttachmentUrl5 = at13.Url,
            SemesterActiveAttachmentName6 = at14.Name,
            SemesterActiveAttachmentUrl6 = at14.Url,
         });
      }

      [HttpPost]
      public ActionResult TuanDZhidJians(ZhidJiansViewModel model)
      {
         ThrowNotAjax();

         #region [ attachment models ]

         var atta1 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_ZhidJians,
            Name = model.AttachmentName1,
            Url = model.AttachmentUrl1,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta2 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_SannGuih,
            Name = model.AttachmentName2,
            Url = model.AttachmentUrl2,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta3 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqJih,
            Name = model.SemesterAttachmentName1,
            Url = model.SemesterAttachmentUrl1,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta4 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqJih2,
            Name = model.SemesterAttachmentName2,
            Url = model.SemesterAttachmentUrl2,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta5 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqJih3,
            Name = model.SemesterAttachmentName3,
            Url = model.SemesterAttachmentUrl3,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta6 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqJih4,
            Name = model.SemesterAttachmentName4,
            Url = model.SemesterAttachmentUrl4,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta7 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqJih5,
            Name = model.SemesterAttachmentName5,
            Url = model.SemesterAttachmentUrl5,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta8 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqJih6,
            Name = model.SemesterAttachmentName6,
            Url = model.SemesterAttachmentUrl6,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };


         var atta9 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqHuodAnp,
            Name = model.SemesterActiveAttachmentName1,
            Url = model.SemesterActiveAttachmentUrl1,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta10 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqHuodAnp2,
            Name = model.SemesterActiveAttachmentName2,
            Url = model.SemesterActiveAttachmentUrl2,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta11 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqHuodAnp3,
            Name = model.SemesterActiveAttachmentName3,
            Url = model.SemesterActiveAttachmentUrl3,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta12 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqHuodAnp4,
            Name = model.SemesterActiveAttachmentName4,
            Url = model.SemesterActiveAttachmentUrl4,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta13 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqHuodAnp5,
            Name = model.SemesterActiveAttachmentName5,
            Url = model.SemesterActiveAttachmentUrl5,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         var atta14 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_XueqHuodAnp6,
            Name = model.SemesterActiveAttachmentName6,
            Url = model.SemesterActiveAttachmentUrl6,
            UserId = UserProfile.UserId,
            JoinId = UserProfile.UserId,
         };

         #endregion


         db.BeginTrans();

         try
         {
            AttachmentsExtensions.DeleteAttas(db, UserProfile.UserId, new string[] {
            AttachmentsKeys.Tuand_ZhidJians,
            AttachmentsKeys.Tuand_SannGuih,
            AttachmentsKeys.Tuand_XueqJih,
            AttachmentsKeys.Tuand_XueqJih2,
            AttachmentsKeys.Tuand_XueqJih3,
            AttachmentsKeys.Tuand_XueqJih4,
            AttachmentsKeys.Tuand_XueqJih5,
            AttachmentsKeys.Tuand_XueqJih6,

            AttachmentsKeys.Tuand_XueqHuodAnp,
            AttachmentsKeys.Tuand_XueqHuodAnp2,
            AttachmentsKeys.Tuand_XueqHuodAnp3,
            AttachmentsKeys.Tuand_XueqHuodAnp4,
            AttachmentsKeys.Tuand_XueqHuodAnp5,
            AttachmentsKeys.Tuand_XueqHuodAnp6,
         });

            AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta1, atta2, atta3, atta4, atta5, atta6, atta7, atta8, atta9, atta10, atta11, atta12, atta13, atta14 });

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
            msg = "信息已保存!"
         });
      }


      #endregion


      #region [ 团队项目 ]

      public ActionResult TuanDXinagm(long teamId)
      {
         var tp = APDBDef.TeamProject;
         var model = new TuandXiangmViewModel();

         var data = db.TeamProjectDal.ConditionQuery(tp.TeamId == teamId & tp.TeacherId == UserProfile.UserId, null, null, null).FirstOrDefault();

         if (data != null)
         {
            var allAts = AttachmentsExtensions.GetAttachmentList(db, data.TeamPojectId);
            var ats1 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_Xiangm_Kait);
            var ats2 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_Xiangm_zhongq);
            var ats3 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_Xiangm_jiet);
            var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_Xiangm + AttachmentsKeys.Vertify);
            var vt = AttachmentsExtensions.GetAttachment(vts);
            var at1 = AttachmentsExtensions.GetAttachment(ats1);
            var at2 = AttachmentsExtensions.GetAttachment(ats2);
            var at3 = AttachmentsExtensions.GetAttachment(ats3);

            model.Id = data.TeamPojectId;
            model.TeamId = teamId;
            model.Name = data.ProjectName;
            model.UserName = data.DeclareUser;
            model.Company = data.DeclareCompany;
            model.Date = data.FillDate;
            model.AttachmentName1 = at1.Name;
            model.AttachmentUrl1 = at1.Url;
            model.AttachmentName2 = at2.Name;
            model.AttachmentUrl2 = at2.Url;
            model.AttachmentName3 = at3.Name;
            model.AttachmentUrl3 = at3.Url;
            model.VertificationName = vt.Name;
            model.VertificationUrl = vt.Url;

         }

         return PartialView("TuanDXiangm", model);
      }

      [HttpPost]
      public ActionResult TuanDXinagm(long? id, TuandXiangmViewModel model)
      {
         ThrowNotAjax();

         var atta1 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_Xiangm_Kait,
            Name = model.AttachmentName1,
            Url = model.AttachmentUrl1,
            UserId = UserProfile.UserId,
            JoinId = id ?? 0,
         };

         var atta2 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_Xiangm_zhongq,
            Name = model.AttachmentName2,
            Url = model.AttachmentUrl2,
            UserId = UserProfile.UserId,
            JoinId = id ?? 0,
         };

         var atta3 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_Xiangm_jiet,
            Name = model.AttachmentName3,
            Url = model.AttachmentUrl3,
            UserId = UserProfile.UserId,
            JoinId = id ?? 0,
         };

         var vertAtta = new AttachmentsDataModel
         {
            Type = AttachmentsKeys.Tuand_Xiangm + AttachmentsKeys.Vertify,
            Name = model.VertificationName,
            Url = model.VertificationUrl,
            UserId = UserProfile.UserId,
            JoinId = id ?? 0,
         };


         TeamProject data = null;

         db.BeginTrans();

         try
         {
            if (null == id)
            {
               data = new TeamProject()
               {
                  ContentKey = TeamKeys.TuanDXiangm,
                  ProjectName = model.Name,
                  DeclareUser = model.UserName,
                  DeclareCompany = model.Company,
                  FillDate = model.Date,
                  TeacherId = UserProfile.UserId,
                  TeamId = model.TeamId,
                  CreateDate = DateTime.Now,
                  Creator = UserProfile.UserId,
               };

               db.TeamProjectDal.Insert(data);
            }
            else
            {
               db.TeamProjectDal.UpdatePartial(id.Value, new
               {
                  DeclareUser = model.UserName,
                  DeclareCompany = model.Company,
                  FillDate = model.Date,
                  Modifier = UserProfile.UserId,
                  ModifyDate = DateTime.Now,
               });

               AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] {
                  AttachmentsKeys.Tuand_Xiangm_Kait,
                  AttachmentsKeys.Tuand_Xiangm_zhongq,
                  AttachmentsKeys.Tuand_Xiangm_jiet,
                  AttachmentsKeys.Tuand_Xiangm + AttachmentsKeys.Vertify });
            }

            AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta1, atta2, atta3, vertAtta });

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
            msg = "信息已保存!"
         });
      }


      #endregion


      #region [ 团队个人信息 ]


      // GET: Team/TidXinx

      public ActionResult TuanDGerXinx()
      {
         var m = APDBDef.TeamMember;
         var d2 = APDBDef.DeclareBase.As("Team");
         var userId = UserProfile.UserId;
         var subQuery = APQuery.select(m.TeamId).from(m).where(m.MemberId == userId);
         var team = db.DeclareBaseDal.ConditionQuery(d.TeacherId.In(subQuery), null, null, null).FirstOrDefault();
         var currentDeclare = db.DeclareBaseDal.ConditionQuery(d.TeacherId == userId, null, null, null).FirstOrDefault();
         currentDeclare.RealName = UserProfile.RealName;
         currentDeclare.TeamName = $"{currentDeclare.DeclareStage}{currentDeclare.DeclareSubject}{currentDeclare.RealName }";

         return PartialView("TuandGerXinx", currentDeclare);
      }

      #endregion


      #region [ 团队个人计划 ]

      public ActionResult TuanDGerJih()
      {
         var userId = UserProfile.UserId;
         var allAts = AttachmentsExtensions.GetAttachmentList(db, userId);
         var atta1 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_GrJih1 && a.UserId == userId);
         var atta2 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_GrJih2 && a.UserId == userId);
         var atta3 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_GrJih3 && a.UserId == userId);
         var at1 = AttachmentsExtensions.GetAttachment(atta1);
         var at2 = AttachmentsExtensions.GetAttachment(atta2);
         var at3 = AttachmentsExtensions.GetAttachment(atta3);

         return PartialView("TuanDGerJih", new TeamGerJihViewModel
         {
            AttachmentName1 = at1.Name,
            AttachmentName2 = at2.Name,
            AttachmentName3 = at3.Name,
            AttachmentUrl1 = at1.Url,
            AttachmentUrl2 = at2.Url,
            AttachmentUrl3 = at3.Url
         });
      }


      [HttpPost]

      public ActionResult TuanDGerJih(TeamGerJihViewModel model)
      {
         var userId = UserProfile.UserId;
         var atta1 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_GrJih1,
            Name = model.AttachmentName1,
            Url = model.AttachmentUrl1,
            UserId = userId,
            JoinId = userId
         };

         var atta2 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_GrJih2,
            Name = model.AttachmentName2,
            Url = model.AttachmentUrl2,
            UserId = userId,
            JoinId = userId,
         };

         var atta3 = new AttachmentsDataModel()
         {
            Type = AttachmentsKeys.Tuand_GrJih3,
            Name = model.AttachmentName3,
            Url = model.AttachmentName3,
            UserId = userId,
            JoinId = userId,
         };

         AttachmentsExtensions.DeleteAttas(db, UserProfile.UserId, new string[] {
            AttachmentsKeys.Tuand_GrJih1,
            AttachmentsKeys.Tuand_GrJih2,
            AttachmentsKeys.Tuand_GrJih3
         });

         AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta1, atta2, atta3 });

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "信息已保存!"
         });
      }

      #endregion
 

      #region [ 团队子项目 ]

      public ActionResult TuanDZiXiangm()
      {
         var m = APDBDef.TeamMember;
         var team = db.TeamMemberDal.ConditionQuery(m.MemberId == UserProfile.UserId, null, null, null).FirstOrDefault();

         return RedirectToAction("TuanDXinagm", new { teamId = team.TeamId });
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