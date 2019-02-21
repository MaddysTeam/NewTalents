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

    public class QualityEvalManageController : BaseController
    {
        static APDBDef.EvalPeriodTableDef ep = APDBDef.EvalPeriod;
        static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
        static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
        static APDBDef.EvalQualityResultTableDef er = APDBDef.EvalQualityResult;
        static APDBDef.EvalQualityResultItemTableDef eri = APDBDef.EvalQualityResultItem;
        static APDBDef.EvalQualitySubmitResultTableDef esr = APDBDef.EvalQualitySubmitResult;
        static APDBDef.ExpGroupMemberTableDef egm = APDBDef.ExpGroupMember;
        static APDBDef.ExpGroupTargetTableDef egt = APDBDef.ExpGroupTarget;
        static APDBDef.ExpGroupTableDef eg = APDBDef.ExpGroup;


        // GET: QualityEvalManage/Overview

        public ActionResult Overview(long periodId = 0)
        {
            if (periodId == 0)
            {
                var period = db.GetCurrentEvalPeriod();

                if (period == null)
                {
                    return View("../EvalPeriod/NotInAccessRegion");
                }
                else
                {
                    return RedirectToAction("Overview", new { periodId = period.PeriodId });
                }
            }

            var query = APQuery.select(eg.GroupId, eg.Name,
                                                egt.MemberId.Count().As("TotalCount"),
                                                esr.ResultId.Count().As("EvalCount"))
                                  .from(eg,
                                        egt.JoinLeft(eg.GroupId == egt.GroupId),
                                        esr.JoinLeft(esr.TeacherId == egt.MemberId & esr.PeriodId == periodId)
                                        )
                                  .group_by(eg.GroupId, eg.Name);

            var result = query.query(db, rd =>
            {
                var memberCount = rd.GetInt32(rd.GetOrdinal("TotalCount"));
                var evalMemberCount = rd.GetInt32(rd.GetOrdinal("EvalCount"));

                return new QualityEvalOverviewModels
                {
                    PeriodId = periodId,
                    GroupId = eg.GroupId.GetValue(rd),
                    GroupName = eg.Name.GetValue(rd),
                    GroupTargetMemberCount = memberCount,
                    EvalTargetMemberCount = evalMemberCount,
                    EvalStatus = memberCount == evalMemberCount && memberCount > 0 ? EvalStatus.Success
                                        : memberCount > evalMemberCount && evalMemberCount > 0 ? EvalStatus.Pending
                                        : EvalStatus.NotStart
                };
            }).ToList();


            return View(result);
        }


        // GET: QualityEvalManage/EvalMemberList
        // POST: QualityEvalManage/EvalMemberList

        public ActionResult EvalMemberList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, int periodId)
        {
            ThrowNotAjax();


            var query = APQuery.select(u.RealName, d.DeclareTargetPKID, esr.TeacherId, esr.AccesserCount, esr.AdjustScore, esr.Score, esr.FullScore, esr.AccessDate)
                .from(egt,
                        esr.JoinInner(egt.MemberId == esr.TeacherId),
                        u.JoinInner(egt.MemberId == u.UserId),
                        d.JoinInner(d.TeacherId == u.UserId)
                      )
                .where(esr.PeriodId == periodId)
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
                    case "targetName": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
                    case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
                    case "score": query.order_by(sort.OrderBy(esr.Score)); break;
                    case "adjustScore": query.order_by(sort.OrderBy(esr.AdjustScore)); break;
                    case "accessCount": query.order_by(sort.OrderBy(esr.AccesserCount)); break;
                    case "accessDate": query.order_by(sort.OrderBy(esr.AccessDate)); break;
                }
            }

            var total = db.ExecuteSizeOfSelect(query);

            var result = query.query(db, rd =>
            {
                var score = er.Score.GetValue(rd);
                var fullScore = er.FullScore.GetValue(rd);
                return new
                {
                    teacherId = esr.TeacherId.GetValue(rd),
                    targetName = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
                    realName = u.RealName.GetValue(rd),
                    accessCount = esr.AccesserCount.GetValue(rd),
                    accessDate = esr.AccessDate.GetValue(rd),
                    adjustScore = esr.AdjustScore.GetValue(rd),
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


        // GET: QualityEvalManage/NotEvalMemberList
        // POST: QualityEvalManage/NotEvalMemberList

        public ActionResult NotEvalMemberList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NotEvalMemberList(int current, int rowCount, AjaxOrder sort, string searchPhrase, int periodId)
        {
            ThrowNotAjax();


            var subQuery = APQuery.select(esr.TeacherId)
            .from(esr)
           .where(esr.PeriodId == periodId);

            var query = APQuery.select(egt.MemberId,eg.Name, u.RealName)
            .from(egt,
                  eg.JoinInner(egt.GroupId == eg.GroupId),
                  u.JoinInner(egt.MemberId == u.UserId)
                 )
            .where(egt.MemberId.NotIn(subQuery))
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
                    case "groupName": query.order_by(sort.OrderBy(eg.Name)); break;
                    case "teacherName": query.order_by(sort.OrderBy(u.RealName)); break;
                }
            }

            var total = db.ExecuteSizeOfSelect(query);

            var result = query.query(db, rd =>
            {
                return new
                {
                    id= egt.MemberId.GetValue(rd),
                    groupName = eg.Name.GetValue(rd),
                    teacherName = u.RealName.GetValue(rd)
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


        // GET: QualityEvalManage/List

        public ActionResult List()
        {
            var list = db.EvalPeriodDal.ConditionQuery(ep.IsCurrent == false, null, null, null);

            return View(list);
        }

    }


}