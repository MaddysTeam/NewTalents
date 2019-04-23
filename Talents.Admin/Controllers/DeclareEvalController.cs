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

   public class DeclareEvalController : BaseController
   {
      static APDBDef.DeclarePeriodTableDef dp = APDBDef.DeclarePeriod;
      static APDBDef.DeclareReviewTableDef dr = APDBDef.DeclareReview;
      static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
      static APDBDef.EvalDeclareResultTableDef er = APDBDef.EvalDeclareResult;
      static APDBDef.ExpGroupMemberTableDef egm = APDBDef.ExpGroupMember;
      static APDBDef.ExpGroupTargetTableDef egt = APDBDef.ExpGroupTarget;
      static APDBDef.ExpGroupTableDef eg = APDBDef.ExpGroup;


      //// GET: DeclareEval/Index

      public ActionResult Index()
      {
         if (Period == null)
         {
            return View("../EvalPeriod/NotInAccessRegion");
         }

         return RedirectToAction("List", new { Period.PeriodId });
      }


      //// GET: DeclareEval/BlockList
      //// POST-Ajax: DeclareEval/BlockList

      public ActionResult List()
      {
         var groups = APQuery.select(eg.GroupId, eg.Name)
             .from(egm, eg.JoinInner(egm.GroupId == eg.GroupId),
                     u.JoinInner(u.UserId == egm.ExpectID))
                     .group_by(eg.GroupId, eg.Name)
                     .where(egm.ExpectID == UserProfile.UserId)
             .query(db, (rd) =>
             {
                return new ExpGroup
                {
                   GroupId = eg.GroupId.GetValue(rd),
                   Name = eg.Name.GetValue(rd),
                };
             }).ToList();

         var result = new ExpGroupList
         {
            Groups = groups
         };

         return View(result);
      }

      [HttpPost]
      public JsonResult GetGroupInfo(long groupId, long periodId)
      {
         ThrowNotAjax();


         string leaderName = null;

         var memberNames = APQuery.select(egm.IsLeader, u.RealName)
              .from(egm, u.JoinInner(u.UserId == egm.ExpectID))
              .where(egm.GroupId == groupId)
              .query(db, r =>
              {
                 var name = u.RealName.GetValue(r);
                 if (egm.IsLeader.GetValue(r))
                 {
                    leaderName = name;
                 }
                 return name;
              }).ToArray();

         var totalCount = db.ExpGroupTargetDal.ConditionQueryCount(egt.GroupId == groupId);
         var evalCount = db.EvalDeclareResultDal.ConditionQueryCount(
              er.GroupId == groupId & er.PeriodId == periodId & er.Accesser == UserProfile.UserId);


         return Json(new
         {
            leaderName,
            memberNames = String.Join(", ", memberNames),
            evalCount,
            notEvalCount = totalCount - evalCount
         });
      }


      // GET: DeclareEval/EvalExpertMemberList
      // POST-Ajax: DeclareEval/EvalExpertMemberList

      public ActionResult EvalExpertMemberList()
      {
         return View();
      }

      [HttpPost]
      public ActionResult EvalExpertMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long periodId)
      {
         ThrowNotAjax();

         var c = APDBDef.Company;

         var query = APQuery.select(er.TeacherId, u.RealName,
                 dr.DeclareTargetPKID, dr.DeclareSubjectPKID, dr.CompanyId,
                 er.Score, er.FullScore, c.CompanyName)
             .from(er,
                      dr.JoinInner(er.TeacherId == dr.TeacherId),
                      u.JoinInner(er.TeacherId == u.UserId),
                      c.JoinInner(dr.CompanyId == c.CompanyId)
                     )
             .where(dr.StatusKey==DeclareKeys.ReviewSuccess & er.GroupId == groupId, er.PeriodId == periodId & er.Accesser == UserProfile.UserId)
             .primary(er.ResultId)
             .skip((current - 1) * rowCount)
             .take(rowCount);


         //过滤条件
         //模糊搜索姓名

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
               case "score": query.order_by(sort.OrderBy(er.Score)); break;
            }
         }
         else
         {
            query.order_by(er.DeclareTargetPKID.Asc).order_by_add(er.Score.Desc);
         }

         var total = db.ExecuteSizeOfSelect(query);

         var result = query.query(db, r =>
         {
            var score = er.Score.GetValue(r);
            var fullScore = er.FullScore.GetValue(r);

            return new
            {
               id = er.TeacherId.GetValue(r),
               realName = u.RealName.GetValue(r),
               targetId = dr.DeclareTargetPKID.GetValue(r),
               target = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(r)),
               subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(r)),
               company =c.CompanyName.GetValue(r),
               score = string.Format("{0} / {1}", score, fullScore),
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


      // GET: DeclareEval/NotEvalExpertMemberList
      // POST-Ajax: DeclareEval/NotEvalExpertMemberList

      public ActionResult NotEvalExpertMemberList()
      {
         return View();
      }

      [HttpPost]
      public ActionResult NotEvalExpertMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long periodId)
      {
         ThrowNotAjax();


         var subQuery = APQuery.select(er.TeacherId)
             .from(er)
             .where(er.Accesser == UserProfile.UserId & er.GroupId == groupId & er.PeriodId == periodId);

         var query = APQuery.select(egt.MemberId, u.RealName, dr.DeclareTargetPKID, dr.DeclareSubjectPKID)
             .from(egt,
                      u.JoinInner(u.UserId == egt.MemberId),
                      dr.JoinInner(dr.TeacherId == egt.MemberId)
                     )
             .where(egt.GroupId == groupId & dr.StatusKey != "" & dr.StatusKey != DeclareKeys.ReviewBack & egt.MemberId.NotIn(subQuery))
             .primary(egt.MemberId)
             .skip((current - 1) * rowCount)
             .take(rowCount);


         //过滤条件
         //模糊搜索姓名

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
               case "target": query.order_by(sort.OrderBy(dr.DeclareTargetPKID)); break;
               case "subject": query.order_by(sort.OrderBy(dr.DeclareSubjectPKID)); break;
                  // case "stage": query.order_by(sort.OrderBy(d.DeclareStagePKID)); break;
            }
         }

         var total = db.ExecuteSizeOfSelect(query);

         var result = query.query(db, rd =>
         {
            return new
            {
               id = egt.MemberId.GetValue(rd),
               realName = u.RealName.GetValue(rd),
               target = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(rd)),
               subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(rd)),
               //stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
               targetId = dr.DeclareTargetPKID.GetValue(rd),
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


      // GET: DeclareEval/EvalSchoolMemberList
      // POST-Ajax: DeclareEval/EvalSchoolMemberList

      public ActionResult EvalSchoolMemberList()
      {
         return View();
      }

      [HttpPost]
      public ActionResult EvalSchoolMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase)
      {
         ThrowNotAjax();

         var query = APQuery.select(dr.TeacherId, u.RealName,
                 dr.DeclareTargetPKID, dr.DeclareSubjectPKID,
                 er.Score, er.FullScore, er.ResultId)
             .from(dr,
                      er.JoinLeft(er.TeacherId == dr.TeacherId),
                      u.JoinInner(dr.TeacherId == u.UserId)
                     //er.JoinLeft(er.Accesser == UserProfile.UserId)
                     )
             .where(dr.PeriodId == Period.PeriodId & dr.CompanyId == UserProfile.CompanyId & dr.StatusKey.NotIn(new string[] { string.Empty, DeclareKeys.ReviewBack }))
             .primary(er.ResultId)
             .skip((current - 1) * rowCount)
             .take(rowCount);


         //过滤条件
         //模糊搜索姓名

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
               case "score": query.order_by(sort.OrderBy(er.Score)); break;
            }
         }
         else
         {
            query.order_by(er.DeclareTargetPKID.Asc).order_by_add(er.Score.Desc);
         }

         var total = db.ExecuteSizeOfSelect(query);

         var result = query.query(db, r =>
         {
            var score = er.Score.GetValue(r);
            var fullScore = er.FullScore.GetValue(r);
            var status = er.ResultId.GetValue(r);

            return new
            {
               id = dr.TeacherId.GetValue(r),
               periodId = Period.PeriodId,
               realName = u.RealName.GetValue(r),
               target = DeclareBaseHelper.DeclareTarget.GetName(dr.DeclareTargetPKID.GetValue(r)),
               subject = DeclareBaseHelper.DeclareSubject.GetName(dr.DeclareSubjectPKID.GetValue(r)),
               score = string.Format("{0} / {1}", score, fullScore),
               //submitStatus = status == 0 ? "未提交" : "已提交",
               submitStatus = status == 0 ? "未评审" : "已评审",
               targetId = dr.DeclareTargetPKID.GetValue(r)
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

      ////	GET: DeclareEval/Eval
      ////	POST: DecalreEval/Eval

      [NoCache]
      public ActionResult Eval(DeclareEvalParam param)
      {
         param.AccesserId = UserProfile.UserId;

         var isEvalSubmit = false;

         var engines = EngineManager.Engines[Period.AnalysisType].DeclareEvals;
         if (isEvalSubmit || !engines.ContainsKey(param.TargetId))
         {
            throw new ApplicationException("当前不支持该学员考评");
         }

         var engine = engines[param.TargetId];

         var result = db.EvalDeclareResultDal.ConditionQuery(
             er.PeriodId == param.PeriodId &
             er.TeacherId == param.TeacherId &
             er.Accesser == this.UserProfile.UserId, null, null, null)
             .FirstOrDefault();

         param.ResultId = result == null ? 0 : result.ResultId;

         ViewBag.Result = engine.GetResult(db, param);

         ViewBag.ResultItems = engine.GetResultItem(db, param);

         ViewBag.Declare = param.GetDeclareReviewInfo(db);

         return View(engine.EvalView, param);
      }

      [HttpPost]
      public ActionResult Eval(DeclareEvalParam param, FormCollection fc)
      {
         var engine = EngineManager.Engines[Period.AnalysisType].DeclareEvals;

         db.BeginTrans();

         try
         {
            var resultId = engine[param.TargetId].Eval(db, param, fc);

            db.Commit();

            if (UserProfile.IsSchoolAdmin)
            {
               return RedirectToAction("EvalSchoolMemberList");
            }
            else if (UserProfile.IsExpert)
            {
               return RedirectToAction("NotEvalExpertMemberList", new
               {
                  teacherId = param.TeacherId,
                  periodId = param.PeriodId,
                  groupId = param.GroupId
               });
            }
            else
            {
               return RedirectToAction("SubmitResultView", new
               {
                  teacherId = param.TeacherId,
                  periodId = param.PeriodId,
                  groupId = param.GroupId
               });
            }

         }
         catch
         {
            db.Rollback();
            throw;
         }
      }


      ////	POST: DeclareEval/SubmitEvalResult

      //[HttpPost]
      //public ActionResult SubmitEvalResult(EvalQualitySubmitResult model)
      //{
      //   model.Score += model.AdjustScore;
      //   model.AccessDate = DateTime.Now;

      //   var exists = db.EvalQualitySubmitResultDal.ConditionQueryCount(esr.TeacherId == model.TeacherId & esr.PeriodId == model.PeriodId) > 0;
      //   if (!exists)
      //   {
      //      db.EvalQualitySubmitResultDal.Insert(model);
      //   }

      //   return RedirectToAction("SubmitResultView", new
      //   {
      //      model.TeacherId,
      //      model.PeriodId,
      //      model.GroupId
      //   });
      //}


      //// GET: DeclareEval/SubmitResultView

      //public ActionResult SubmitResultView(DeclareEvalParam param)
      //{
      //   DeclareEvalSubmitPeriodModel model = new DeclareEvalSubmitPeriodModel(param);

      //   model.Period = db.EvalPeriodDal.PrimaryGet(model.PeriodId);
      //   model.Declare = model.GetDeclareInfo(db);

      //   if (!DeclareTargetIds.HasTeam(model.Declare.TargetId))
      //   {
      //      model.Message = "申报（担当）称号未纳入当期质评考评。";
      //   }
      //   else
      //   {
      //      var engine = EngineManager.Engines[model.Period.AnalysisType].DeclareEvals[model.Declare.TargetId];

      //      model.AnalysisUnit = engine;
      //      model.Result = engine.GetSubmitResult(db, param);

      //      if (model.Result == null && param.GroupId == 0)
      //      {
      //         model.Message = "当期质评还未执行!";
      //      }
      //      else
      //      {
      //         model.EvalResults = engine.GetResults(db, param);

      //         if (param.GroupId != 0)
      //         {
      //            model.NotEvalExperts =
      //                APQuery.select(u.RealName)
      //                .from(egm,
      //                    u.JoinInner(egm.ExpectID == u.UserId))
      //                .where(egm.GroupId == param.GroupId & egm.IsLeader == false & egm.ExpectID.NotIn(
      //                    APQuery.select(er.Accesser)
      //                        .from(er)
      //                        .where(er.GroupId == param.GroupId
      //                             & er.PeriodId == param.PeriodId
      //                             & er.TeacherId == param.TeacherId
      //                         )
      //                    ))
      //                .query(db, r => r.GetString(0)).ToList();


      //            //  是否是组长

      //            model.IsLeader = db.ExpGroupMemberDal.ConditionQueryCount(
      //                egm.ExpectID == UserProfile.UserId &
      //                egm.GroupId == param.GroupId &
      //                egm.IsLeader == true) > 0;
      //         }
      //      }

      //      if (model.CanSubmit)
      //      {
      //         model.DoSubmit = new EvalQualitySubmitResult
      //         {
      //            PeriodId = param.PeriodId,
      //            DeclareTargetPKID = model.Declare.TargetId,
      //            TeacherId = param.TeacherId,
      //            FullScore = engine.FullScroe,
      //            Score = model.EvalResults.Count <= 0 ? 0 : model.EvalResults.Average(m => m.Score),
      //            Characteristic = model.EvalResults.Count <= 0 ? 0 : model.EvalResults.Average(m => m.Characteristic),
      //            AccesserCount = model.EvalResults.Count <= 0 ? 0 : model.EvalResults.Count,
      //            GroupId = param.GroupId,
      //            Accesser = UserProfile.UserId
      //         };
      //      }
      //   }

      //   return View("../EvalUtilities/ResultView", model);
      //}


      ////	POST-Ajax: DeclareEval/ResultView

      //[HttpPost]
      //public ActionResult ResultView(DeclareEvalParam param)
      //{
      //   ThrowNotAjax();

      //   DeclareEvalPeriodModel model = new DeclareEvalPeriodModel(param);

      //   model.Period = db.EvalPeriodDal.PrimaryGet(model.PeriodId);
      //   model.Declare = model.GetDeclareInfo(db);

      //   var engine = EngineManager.Engines[model.Period.AnalysisType].DeclareEvals[model.Declare.TargetId];

      //   model.AnalysisUnit = engine;
      //   model.Result = engine.GetResult(db, param);
      //   model.ResultItems = engine.GetResultItem(db, param);


      //   return PartialView(engine.ResultView, model);
      //}

   }

}