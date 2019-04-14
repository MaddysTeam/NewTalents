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
      //static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
      static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
      static APDBDef.EvalDeclareResultTableDef er = APDBDef.EvalDeclareResult;
      //static APDBDef.EvalQualityResultTableDef er = APDBDef.EvalQualityResult;
      //static APDBDef.EvalQualityResultItemTableDef eri = APDBDef.EvalQualityResultItem;
      //static APDBDef.EvalQualitySubmitResultTableDef esr = APDBDef.EvalQualitySubmitResult;
      static APDBDef.ExpGroupMemberTableDef egm = APDBDef.ExpGroupMember;
      static APDBDef.ExpGroupTargetTableDef egt = APDBDef.ExpGroupTarget;
      static APDBDef.ExpGroupTableDef eg = APDBDef.ExpGroup;


      //// GET: DeclareEval/Index

      //public ActionResult Index()
      //{
      //   var period = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == true, null, null, null)
      //       .FirstOrDefault();

      //   if (period == null || !period.InAccessDateRegion(DateTime.Now))
      //   {
      //      return View("../EvalPeriod/NotInAccessRegion");
      //   }

      //   return RedirectToAction("List", new { period.PeriodId });
      //}


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
         var evalCount = db.EvalQualityResultDal.ConditionQueryCount(
                 er.GroupId == groupId & er.PeriodId == periodId & er.Accesser == UserProfile.UserId);

         return Json(new
         {
            leaderName,
            memberNames = String.Join(", ", memberNames),
            evalCount,
            notEvalCount = totalCount - evalCount
         });
      }


      //// GET: DeclareEval/EvalMemberList
      //// POST-Ajax: DeclareEval/EvalMemberList

      //public ActionResult EvalMemberList()
      //{
      //   return View();
      //}

      //[HttpPost]
      //public ActionResult EvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long periodId)
      //{
      //   ThrowNotAjax();


      //   var query = APQuery.select(er.TeacherId, u.RealName,
      //           d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID,
      //           er.Score, er.FullScore, er.DynamicScore1, er.DynamicScore2, er.DynamicScore3, er.Characteristic, esr.ResultId)
      //       .from(er,
      //                d.JoinInner(er.TeacherId == d.TeacherId),
      //                u.JoinInner(er.TeacherId == u.UserId),
      //                esr.JoinLeft(esr.TeacherId == er.TeacherId & esr.PeriodId == periodId)
      //               )
      //       .where(er.GroupId == groupId, er.PeriodId == periodId & er.Accesser == UserProfile.UserId)
      //       .primary(er.ResultId)
      //       .skip((current - 1) * rowCount)
      //       .take(rowCount);


      //   //过滤条件
      //   //模糊搜索姓名

      //   searchPhrase = searchPhrase.Trim();
      //   if (searchPhrase != "")
      //   {
      //      query.where_and(u.RealName.Match(searchPhrase));
      //   }


      //   //排序条件表达式

      //   if (sort != null)
      //   {
      //      switch (sort.ID)
      //      {
      //         case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
      //         //case "target": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
      //         //case "subject": query.order_by(sort.OrderBy(d.DeclareSubjectPKID)); break;
      //         //case "stage": query.order_by(sort.OrderBy(d.DeclareStagePKID)); break;
      //         case "score": query.order_by(sort.OrderBy(er.Score)); break;
      //         case "dynamicScore1": query.order_by(sort.OrderBy(er.DynamicScore1)); break;
      //         case "dynamicScore2": query.order_by(sort.OrderBy(er.DynamicScore2)); break;
      //         case "dynamicScore3": query.order_by(sort.OrderBy(er.DynamicScore3)); break;
      //         case "characteristic": query.order_by(sort.OrderBy(er.Characteristic)); break;
      //         case "submitStatus": query.order_by(sort.OrderBy(esr.ResultId)); break;
      //      }
      //   }
      //   else
      //   {
      //      query.order_by(er.DeclareTargetPKID.Asc).order_by_add(er.Score.Desc);
      //   }

      //   var total = db.ExecuteSizeOfSelect(query);

      //   var result = query.query(db, r =>
      //   {
      //      var score = er.Score.GetValue(r);
      //      var fullScore = er.FullScore.GetValue(r);
      //      var status = esr.ResultId.GetValue(r);

      //      return new
      //      {
      //         id = er.TeacherId.GetValue(r),
      //         realName = u.RealName.GetValue(r),
      //         target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(r)),
      //         subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(r)),
      //         stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(r)),
      //         score = string.Format("{0} / {1}", score, fullScore),
      //         submitStatus = status == 0 ? "未提交" : "已提交",
      //         targetId = d.DeclareTargetPKID.GetValue(r),
      //         dynamicScore1 = er.DynamicScore1.GetValue(r),
      //         dynamicScore2 = er.DynamicScore2.GetValue(r),
      //         dynamicScore3 = er.DynamicScore3.GetValue(r),
      //         characteristic = er.Characteristic.GetValue(r)
      //      };
      //   }).ToList();


      //   return Json(new
      //   {
      //      rows = result,
      //      current,
      //      rowCount,
      //      total
      //   });
      //}


      //// GET: DeclareEval/NotEvalMemberList
      //// POST-Ajax: DeclareEval/NotEvalMemberList

      //public ActionResult NotEvalMemberList()
      //{
      //   return View();
      //}

      //[HttpPost]
      //public ActionResult NotEvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, long groupId, long periodId)
      //{
      //   ThrowNotAjax();


      //   var subQuery = APQuery.select(er.TeacherId)
      //       .from(er)
      //       .where(er.Accesser == UserProfile.UserId & er.GroupId == groupId & er.PeriodId == periodId);

      //   var query = APQuery.select(egt.MemberId, u.RealName, d.DeclareTargetPKID, d.DeclareSubjectPKID, d.DeclareStagePKID)
      //       .from(egt,
      //                u.JoinInner(u.UserId == egt.MemberId),
      //                d.JoinInner(d.TeacherId == egt.MemberId)
      //               )
      //       .where(egt.GroupId == groupId & egt.MemberId.NotIn(subQuery))
      //       .primary(egt.MemberId)
      //       .skip((current - 1) * rowCount)
      //       .take(rowCount);


      //   //过滤条件
      //   //模糊搜索姓名

      //   searchPhrase = searchPhrase.Trim();
      //   if (searchPhrase != "")
      //   {
      //      query.where_and(u.RealName.Match(searchPhrase));
      //   }


      //   //排序条件表达式

      //   if (sort != null)
      //   {
      //      switch (sort.ID)
      //      {
      //         case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
      //         case "target": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
      //         case "subject": query.order_by(sort.OrderBy(d.DeclareSubjectPKID)); break;
      //         case "stage": query.order_by(sort.OrderBy(d.DeclareStagePKID)); break;
      //      }
      //   }

      //   var total = db.ExecuteSizeOfSelect(query);

      //   var result = query.query(db, rd =>
      //   {
      //      return new
      //      {
      //         id = egt.MemberId.GetValue(rd),
      //         realName = u.RealName.GetValue(rd),
      //         target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
      //         subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
      //         stage = DeclareBaseHelper.DeclareStage.GetName(d.DeclareStagePKID.GetValue(rd)),
      //         targetId = d.DeclareTargetPKID.GetValue(rd),
      //      };
      //   }).ToList();


      //   return Json(new
      //   {
      //      rows = result,
      //      current,
      //      rowCount,
      //      total
      //   });
      //}


      ////	GET: DeclareEval/Eval
      ////	POST: DecalreEval/Eval

      [NoCache]
      public ActionResult Eval(DeclareEvalParam param)
      {
         var IsLeader = db.ExpGroupMemberDal.ConditionQueryCount(
                    egm.ExpectID == UserProfile.UserId &
                    egm.GroupId == param.GroupId &
                    egm.IsLeader == true) > 0;

         param.AccesserId = UserProfile.UserId;

         //var isEvalSubmit = db.EvalQualitySubmitResultDal
         //                                  .ConditionQueryCount(esr.TeacherId == param.TeacherId & esr.PeriodId == param.PeriodId) > 0;
         var isEvalSubmit = false;
         var period = db.EvalPeriodDal.PrimaryGet(param.PeriodId);
         var engines = EngineManager.Engines[period.AnalysisType].DeclareEvals;
         if (isEvalSubmit || !engines.ContainsKey(param.TargetId))
         {
            throw new ApplicationException("当前不支持该学员考评");
         }

         var engine = engines[param.TargetId];

         var result = db.EvalQualityResultDal.ConditionQuery(
             er.PeriodId == param.PeriodId &
             er.TeacherId == param.TeacherId &
             er.Accesser == this.UserProfile.UserId, null, null, null)
             .FirstOrDefault();

         param.ResultId = result == null ? 0 : result.ResultId;

         ViewBag.Result = engine.GetResult(db, param);

         ViewBag.ResultItems = engine.GetResultItem(db, param);

         ViewBag.Declare = param.GetDeclareInfo(db);

         return View(engine.EvalView, param);
      }

      [HttpPost]
      public ActionResult Eval(DeclareEvalParam param, FormCollection fc)
      {
         var period = db.EvalPeriodDal.PrimaryGet(param.PeriodId);

         var engine = EngineManager.Engines[period.AnalysisType].DeclareEvals;


         db.BeginTrans();

         try
         {
            var resultId = engine[param.TargetId].Eval(db, param, fc);

            db.Commit();

            return RedirectToAction("SubmitResultView", new
            {
               teacherId = param.TeacherId,
               periodId = param.PeriodId,
               groupId = param.GroupId
            });
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