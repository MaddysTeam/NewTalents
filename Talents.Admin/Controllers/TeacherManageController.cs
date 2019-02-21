using Business;
using Business.Config;
using Business.Helper;
using NPOI.HSSF.UserModel;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

   public class TeacherManageController : BaseController
   {

      static APDBDef.BzUserProfileTableDef up = APDBDef.BzUserProfile;
      static APDBDef.BzUserProfileHistoryTableDef uph = APDBDef.BzUserProfileHistory;
      static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
      static APDBDef.DeclareBaseHistoryTableDef dh = APDBDef.DeclareBaseHistory;
      static APDBDef.CompanyDeclareTableDef cd = APDBDef.CompanyDeclare;

      // GET: TeacherManage/Search
      // POST-Ajax: TeacherManage/Search

      //[Permisson(Admin.UserVisit)]
      public ActionResult Search()
      {
         ViewBag.Periods = db.ProfileModifyPeriodDal.ConditionQuery(null, null, null, null);

         return View();
      }

      [HttpPost]
      public ActionResult Search(int current, int rowCount, AjaxOrder sort, string searchPhrase, long periodId, long target, long subject, long stage, long companyId)
      {
         ThrowNotAjax();

         var period = db.ProfileModifyPeriodDal.PrimaryGet(periodId);

         if (periodId == 0 || period.IsCurrent)
            return SearchCurrent(current, rowCount, sort, searchPhrase, target, subject, stage, companyId);
         else
            return SearchHistory(current, rowCount, sort, searchPhrase, periodId, target, subject, stage, companyId);
      }


      private ActionResult SearchHistory(int current, int rowCount, AjaxOrder sort, string searchPhrase, long periodId, long target, long subject, long stage, long companyId)
      {
         var p1 = APDBDef.PicklistItem.As("p1");
         var p2 = APDBDef.PicklistItem.As("p2");
         var p3 = APDBDef.PicklistItem.As("p3");
         var p4 = APDBDef.PicklistItem.As("p4");
         var p5 = APDBDef.PicklistItem.As("p5");
         var p6 = APDBDef.PicklistItem.As("p6");
         var c = APDBDef.Company;

         var query = APQuery.select(uph.UserId, uph.UserName, uph.RealName, dh.TeacherId,
            uph.UserType, uph.CompanyName, uph.CompanyNameOuter, uph.Phonemobile,
            p1.Name.As("PoliticalStatus"), p2.Name.As("DeclareSubject"), p3.Name.As("DeclareTarget"),
            p4.Name.As("DeclareStage"), p5.Name.As("RankTitle"), p6.Name.As("SkillTitle"),
            c.CompanyName.As("CompanyName2"))
            .from(uph,
                  dh.JoinLeft(uph.UserId == dh.TeacherId),
                  p1.JoinLeft(uph.PoliticalStatusPKID == p1.PicklistItemId),
                  p2.JoinLeft(dh.DeclareSubjectPKID == p2.PicklistItemId),
                  p3.JoinLeft(dh.DeclareTargetPKID == p3.PicklistItemId),
                  p4.JoinLeft(dh.DeclareStagePKID == p4.PicklistItemId),
                  p5.JoinLeft(uph.RankTitlePKID == p5.PicklistItemId),
                  p6.JoinLeft(uph.SkillTitlePKID == p6.PicklistItemId),
                  c.JoinLeft(c.CompanyId == uph.CompanyId)
                  )
            .where(uph.PeriodId == periodId & dh.PeriodId == periodId & dh.TeacherId > 0 & uph.UserType == "teacher");

         if (target > 0)
            query.where_and(dh.DeclareTargetPKID == target);
         else if (target == -1)
            query.where_and(dh.DeclareTargetPKID == null);
         if (subject > 0)
            query.where_and(dh.DeclareSubjectPKID == subject);
         if (stage > 0)
            query.where_and(dh.DeclareStagePKID == stage);
         if (companyId > 0)
            query.where_and(c.CompanyId == companyId);


         query.primary(uph.UserId)
            .skip((current - 1) * rowCount)
            .take(rowCount);


         //过滤条件
         //模糊搜索用户名、实名进行

         searchPhrase = searchPhrase.Trim();
         if (searchPhrase != "")
         {
            query.where_and(uph.UserName.Match(searchPhrase) | uph.RealName.Match(searchPhrase));
         }

         //userType = userType.Trim();
         //if (userType != "全部")
         //{
         //   query.where_and(uph.UserType == userType);
         //}

         //排序条件表达式

         //if (sort != null)
         //{
         //   switch (sort.ID)
         //   {
         //      case "userName": query.order_by(sort.OrderBy(uph.UserName)); break;
         //      case "realName": query.order_by(sort.OrderBy(uph.RealName)); break;
         //      case "userType": query.order_by(sort.OrderBy(uph.UserType)); break;
         //      case "company": query.order_by(sort.OrderBy(uph.CompanyName)); break;
         //   }
         //}


         //获得查询的总数量

         var total = db.ExecuteSizeOfSelect(query);


         //查询结果集

         var result = query.query(db, rd =>
         {
            //var companyName = uph.CompanyName.GetValue(rd);
            //companyName = companyName ?? c.CompanyName.GetValue(rd, "CompanyName2");
            //companyName = companyName ?? uph.CompanyNameOuter.GetValue(rd);

            return new
            {
               id = uph.UserId.GetValue(rd),
               userName = uph.UserName.GetValue(rd),
               realName = uph.RealName.GetValue(rd),
               company = c.CompanyName.GetValue(rd, "CompanyName2"),
               politicalStatus = p1.Name.GetValue(rd, "PoliticalStatus"),
               subject = p2.Name.GetValue(rd, "DeclareSubject"),
               target = p3.Name.GetValue(rd, "DeclareTarget"),
               stage = p4.Name.GetValue(rd, "DeclareStage"),
               rankTitle = p5.Name.GetValue(rd, "RankTitle"),
               skillTitle = p6.Name.GetValue(rd, "SkillTitle"),
               mobile = uph.Phonemobile.GetValue(rd)
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


      private ActionResult SearchCurrent(int current, int rowCount, AjaxOrder sort, string searchPhrase, long target, long subject, long stage, long companyId)
      {
         var p1 = APDBDef.PicklistItem.As("p1");
         var p2 = APDBDef.PicklistItem.As("p2");
         var p3 = APDBDef.PicklistItem.As("p3");
         var p4 = APDBDef.PicklistItem.As("p4");
         var p5 = APDBDef.PicklistItem.As("p5");
         var p6 = APDBDef.PicklistItem.As("p6");
         var c = APDBDef.Company;

         var query = APQuery.select(up.UserId, up.UserName, up.RealName, d.TeacherId,
            up.UserType, up.CompanyName, up.CompanyNameOuter, up.Phonemobile,
            p1.Name.As("PoliticalStatus"), p2.Name.As("DeclareSubject"), p3.Name.As("DeclareTarget"),
            p4.Name.As("DeclareStage"), p5.Name.As("RankTitle"), p6.Name.As("SkillTitle"),
            c.CompanyName.As("CompanyName2"))
            .from(up,
                  d.JoinLeft(up.UserId == d.TeacherId),
                  p1.JoinLeft(up.PoliticalStatusPKID == p1.PicklistItemId),
                  p2.JoinLeft(d.DeclareSubjectPKID == p2.PicklistItemId),
                  p3.JoinLeft(d.DeclareTargetPKID == p3.PicklistItemId),
                  p4.JoinLeft(d.DeclareStagePKID == p4.PicklistItemId),
                  p5.JoinLeft(up.RankTitlePKID == p5.PicklistItemId),
                  p6.JoinLeft(up.SkillTitlePKID == p6.PicklistItemId),
                  cd.JoinLeft(cd.TeacherId == up.UserId),
                  c.JoinInner(c.CompanyId == cd.CompanyId)
                  )
            .where(d.TeacherId >= 0 & up.UserType == "teacher");

         if (target > 0)
            query.where_and(d.DeclareTargetPKID == target);
         else if (target == -1)
            query.where_and(d.DeclareTargetPKID == null);
         if (subject > 0)
            query.where_and(d.DeclareSubjectPKID == subject);
         if (stage > 0)
            query.where_and(d.DeclareStagePKID == stage);
         if (companyId > 0)
            query.where_and(c.CompanyId == companyId);

         query.primary(up.UserId)
            .skip((current - 1) * rowCount)
            .take(rowCount);


         //过滤条件
         //模糊搜索用户名、实名进行

         searchPhrase = searchPhrase.Trim();
         if (searchPhrase != "")
         {
            query.where_and(up.UserName.Match(searchPhrase) | up.RealName.Match(searchPhrase));
         }

         //userType = userType.Trim();
         //if (userType != "全部")
         //{
         //   query.where_and(up.UserType == userType);
         //}

         //排序条件表达式

         //if (sort != null)
         //{
         //   switch (sort.ID)
         //   {
         //      case "userName": query.order_by(sort.OrderBy(up.UserName)); break;
         //      case "realName": query.order_by(sort.OrderBy(up.RealName)); break;
         //      case "userType": query.order_by(sort.OrderBy(up.UserType)); break;
         //      case "company": query.order_by(sort.OrderBy(up.CompanyName)); break;
         //   }
         //}


         //获得查询的总数量

         var total = db.ExecuteSizeOfSelect(query);


         //查询结果集

         var result = query.query(db, rd =>
         {
            //var companyName = up.CompanyName.GetValue(rd);
            //companyName = string.IsNullOrEmpty(companyName) ? c.CompanyName.GetValue(rd, "CompanyName2"):companyName;
            //companyName = string.IsNullOrEmpty(companyName) ? up.CompanyNameOuter.GetValue(rd) : companyName;

            return new
            {
               id = up.UserId.GetValue(rd),
               userName = up.UserName.GetValue(rd),
               realName = up.RealName.GetValue(rd),
               company = c.CompanyName.GetValue(rd, "CompanyName2"),
               politicalStatus = p1.Name.GetValue(rd, "PoliticalStatus"),
               subject = p2.Name.GetValue(rd, "DeclareSubject"),
               target = p3.Name.GetValue(rd, "DeclareTarget"),
               stage = p4.Name.GetValue(rd, "DeclareStage"),
               rankTitle = p5.Name.GetValue(rd, "RankTitle"),
               skillTitle = p6.Name.GetValue(rd, "SkillTitle"),
               mobile = up.Phonemobile.GetValue(rd)
            };
         });

         return Json(new
         {
            rows = result,
            current,
            rowCount,
            total
         });
      }


      //	GET: TeacherManage/Statistical
      //	POST-Ajax: TeacherManage/Statistical

      public ActionResult Statistical()
      {
         return View();
      }

      [HttpPost]
      public ActionResult Statistical(FormCollection fc, int current, int rowCount, AjaxOrder sort)
      {
         ThrowNotAjax();

         var t = APDBDef.EvalPeriod;
         var s = APDBDef.EvalSchoolResult;
         var v = APDBDef.EvalVolumnResult;
         var q = APDBDef.EvalQualitySubmitResult;
         var evalPeriod = db.EvalPeriodDal.ConditionQuery(t.IsCurrent == true, null, null, null).FirstOrDefault();
         long periodId = 0;
         if (evalPeriod != null)
         {
            periodId = evalPeriod.PeriodId;
         }

         var c = APDBDef.Company;

         var query = APQuery.select(up.UserId, up.CompanyName, up.CompanyNameOuter, c.CompanyName.As("CName"), c.CompanyId, up.RealName, up.GenderPKID, up.Birthday, up.EduBgPKID,
            d.DeclareTargetPKID, up.SkillTitlePKID, d.DeclareSubjectPKID, up.EduSubjectPKID, up.EduStagePKID,
            s.Score.As("s_score"), v.Score.As("v_score"), q.Score.As("q_score"), q.Characteristic)
            .from(up, cd.JoinLeft(up.UserId == cd.TeacherId),
                  c.JoinLeft(cd.CompanyId == c.CompanyId),
                  d.JoinInner(up.UserId == d.TeacherId),//TODO：由于其他老师的单位数据有问题，先显示有称号的老师
                  s.JoinLeft(up.UserId == s.TeacherId & s.PeriodId == periodId),
                  v.JoinLeft(up.UserId == v.TeacherId & v.PeriodId == periodId),
                  q.JoinLeft(up.UserId == q.TeacherId & q.PeriodId == periodId));

         //过滤条件
         //模糊搜索用户名、实名进行

         foreach (string cond in fc.Keys)
         {
            switch (cond)
            {
               case "Target":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(d.DeclareTargetPKID == Int64.Parse(fc[cond])); break;
               case "Company":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(cd.CompanyId == Int64.Parse(fc[cond])); break;
               case "Name":
                  if (!string.IsNullOrEmpty(fc[cond]))
                     query.where_and(up.RealName.Match(fc[cond])); break;
               case "Subject":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(d.DeclareSubjectPKID == Int64.Parse(fc[cond])); break;
               case "Gender":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(up.GenderPKID == Int64.Parse(fc[cond])); break;
               case "Date":
                  if (!string.IsNullOrEmpty(fc[cond]))
                     query.where_and(up.Birthday == DateTime.Parse(fc[cond])); break;
               case "EduBg":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(up.EduBgPKID == Int64.Parse(fc[cond])); break;
               case "EduDegree":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(up.EduDegreePKID == Int64.Parse(fc[cond])); break;
               case "SkillTitle":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(up.SkillTitlePKID == Int64.Parse(fc[cond])); break;
               case "PoliticalStatus":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(up.PoliticalStatusPKID == Int64.Parse(fc[cond])); break;
               case "Nationality":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(up.NationalityPKID == Int64.Parse(fc[cond])); break;
               case "EduSubject":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(up.EduSubjectPKID == Int64.Parse(fc[cond])); break;
               case "EduStage":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(up.EduStagePKID == Int64.Parse(fc[cond])); break;
               case "RankTitle":
                  if (Int64.Parse(fc[cond]) > 0)
                     query.where_and(up.RankTitlePKID == Int64.Parse(fc[cond])); break;
               case "qualityScore_1":
                  if (!string.IsNullOrEmpty(fc[cond]))
                     query.where_and(q.Score > (Int64.Parse(fc[cond]) * 2));
                  break;
               case "qualityScore_2":
                  if (!string.IsNullOrEmpty(fc[cond]))
                     query.where_and(q.Score < (Int64.Parse(fc[cond]) * 2));
                  break;
               case "volumnScore_1":
                  if (!string.IsNullOrEmpty(fc[cond]))
                     query.where_and(v.Score > (Int64.Parse(fc[cond]) * 5));
                  break;
               case "volumnScore_2":
                  if (!string.IsNullOrEmpty(fc[cond]))
                     query.where_and(v.Score < (Int64.Parse(fc[cond]) * 5));
                  break;
               case "schoolScore_1":
                  if (!string.IsNullOrEmpty(fc[cond]))
                     query.where_and(s.Score > (Int64.Parse(fc[cond]) * 3));
                  break;
               case "schoolScore_2":
                  if (!string.IsNullOrEmpty(fc[cond]))
                     query.where_and(s.Score < (Int64.Parse(fc[cond]) * 3));
                  break;
               case "characteristicScore_1":
                  if (!string.IsNullOrEmpty(fc[cond]))
                     query.where_and(q.Characteristic > Int64.Parse(fc[cond]));
                  break;
               case "characteristicScore_2":
                  if (!string.IsNullOrEmpty(fc[cond]))
                     query.where_and(q.Characteristic < Int64.Parse(fc[cond]));
                  break;
                  //case "totalScore_1":
                  //	if (!string.IsNullOrEmpty(fc[cond])) {
                  //		query.where_and(APSqlRawExpr.Expr("s.Score + v.Score + q.Score + q.Characteristic") > Int64.Parse(fc[cond]));
                  //	}
                  //	break;
                  //case "totalScore_2":
                  //	if (!string.IsNullOrEmpty(fc[cond])) {
                  //		query.where_and(APSqlRawExpr.Expr("s.Score + v.Score + q.Score + q.Characteristic") < Int64.Parse(fc[cond]));
                  //	}
                  //	break;
            }
         }

         query.primary(up.UserId)
         .skip((current - 1) * rowCount)
         .take(rowCount);

         //排序条件表达式

         //if (sort != null)
         //{
         //	switch (sort.ID)
         //	{
         //		case "userName": query.order_by(sort.OrderBy(up.UserName)); break;
         //		case "realName": query.order_by(sort.OrderBy(up.RealName)); break;
         //		case "userType": query.order_by(sort.OrderBy(up.UserType)); break;
         //		case "company": query.order_by(sort.OrderBy(up.CompanyName)); break;
         //	}
         //}


         //获得查询的总数量

         var total = db.ExecuteSizeOfSelect(query);


         //查询结果集

         var result = query.query(db, rd =>
         {
            return new
            {
               id = up.UserId.GetValue(rd),
               company = c.CompanyName.GetValue(rd,"CName"),
               realName = up.RealName.GetValue(rd),
               gender = BzUserProfileHelper.Gender.GetName(up.GenderPKID.GetValue(rd)),
               birthday = up.Birthday.GetValue(rd),
               eduBg = BzUserProfileHelper.EduBg.GetName(up.EduBgPKID.GetValue(rd)),
               target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(rd)),
               skillTitle = BzUserProfileHelper.SkillTitle.GetName(up.SkillTitlePKID.GetValue(rd)),
               subject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(rd)),
               eduSubject = BzUserProfileHelper.EduSubject.GetName(up.EduSubjectPKID.GetValue(rd)),
               eduStage = BzUserProfileHelper.EduStage.GetName(up.EduStagePKID.GetValue(rd)),
               s_score = s.Score.GetValue(rd, "s_score"),
               v_score = v.Score.GetValue(rd, "v_score")
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

      //	GET: /TeacherManage/UserInfo

      public ActionResult UserInfo(long id)
      {
         ThrowNotAjax();

         var t = APDBDef.EvalPeriod;
         var s = APDBDef.EvalSchoolResult;
         var v = APDBDef.EvalVolumnResult;
         var q = APDBDef.EvalQualitySubmitResult;
         var dc = APDBDef.DeclareContent;
         long periodId = 0;
         var evalPeriod = db.EvalPeriodDal.ConditionQuery(t.IsCurrent == true, null, null, null).FirstOrDefault();
         if (evalPeriod != null)
         {
            periodId = evalPeriod.PeriodId;
         }

         var list = APQuery.select(up.Asterisk, d.MemberCount, s.Score.As("s_score"),
            v.Score.As("v_score"), q.Score.As("q_score"),
            q.Characteristic, dc.ContentValue)
            .from(up,
                  d.JoinLeft(up.UserId == d.TeacherId),
                  s.JoinLeft(up.UserId == s.TeacherId & s.PeriodId == periodId),
                  v.JoinLeft(up.UserId == v.TeacherId & v.PeriodId == periodId),
                  q.JoinLeft(up.UserId == q.TeacherId & q.PeriodId == periodId),
                  dc.JoinLeft(up.UserId == dc.TeacherId & dc.ContentKey == DeclareKeys.ZisFaz_GerChengj_Memo2))
            .where(up.UserId == id)
            .query(db, r =>
            {
               return new UserInfoModel
               {
                  Nationality = BzUserProfileHelper.Nationality.GetName(up.NationalityPKID.GetValue(r)),
                  PoliticalStatus = BzUserProfileHelper.PoliticalStatus.GetName(up.PoliticalStatusPKID.GetValue(r)),
                  SkillDate = up.SkillDate.GetValue(r),
                  GraduateSchool = up.GraduateSchool.GetValue(r),
                  RankTitle = BzUserProfileHelper.RankTitle.GetName(up.RankTitlePKID.GetValue(r)),
                  TrainNo = up.TrainNo.GetValue(r),
                  Phonemobile = up.Phonemobile.GetValue(r),
                  Email = up.Email.GetValue(r),
                  MemberCount = d.MemberCount.GetValue(r),
                  SchoolScore = s.Score.GetValue(r, "s_score") * 0.3,
                  VolumnScore = v.Score.GetValue(r, "v_score") * 0.2,
                  QualityScore = q.Score.GetValue(r, "q_score") * 0.5,
                  ChaScore = q.Characteristic.GetValue(r),
                  Honor = dc.ContentValue.GetValue(r)
               };
            }).FirstOrDefault();

         return PartialView("UserInfo", list == null ? new UserInfoModel() : list);
      }

      //	GET: /TeacherManage/Export

      public FileResult Export(FormCollection fc)
      {

         var t = APDBDef.EvalPeriod;
         var s = APDBDef.EvalSchoolResult;
         var v = APDBDef.EvalVolumnResult;
         var q = APDBDef.EvalQualitySubmitResult;
         var dc = APDBDef.DeclareContent;
         var evalPeriod = db.EvalPeriodDal.ConditionQuery(t.IsCurrent == true, null, null, null).FirstOrDefault();
         long periodId = 0;
         if (evalPeriod != null)
         {
            periodId = evalPeriod.PeriodId;
         }

         var c = APDBDef.Company;

         var query = APQuery.select(up.Asterisk, c.CompanyName.As("CName"), d.DeclareTargetPKID, d.DeclareSubjectPKID, d.MemberCount,
            s.Score.As("s_score"), v.Score.As("v_score"), q.Score.As("q_score"), q.Characteristic, dc.ContentValue)
            .from(up, cd.JoinLeft(up.UserId == cd.TeacherId),
                  c.JoinLeft(cd.CompanyId == c.CompanyId),
                  d.JoinInner(up.UserId == d.TeacherId),//TODO：由于其他老师的单位数据有问题，先显示有称号的老师
                  s.JoinLeft(up.UserId == s.TeacherId & s.PeriodId == periodId),
                  v.JoinLeft(up.UserId == v.TeacherId & v.PeriodId == periodId),
                  q.JoinLeft(up.UserId == q.TeacherId & q.PeriodId == periodId),
                  dc.JoinLeft(up.UserId == dc.TeacherId & dc.ContentKey == DeclareKeys.ZisFaz_GerChengj_Memo2));

         //过滤条件
         //模糊搜索用户名、实名进行

         foreach (string cond in Request.Params)
         {
            switch (cond)
            {
               case "Target":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(d.DeclareTargetPKID == Int64.Parse(Request.Params[cond])); break;
               case "Company":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(cd.CompanyId == Int64.Parse(Request.Params[cond])); break;
               case "Name":
                  if (!string.IsNullOrEmpty(Request.Params[cond]))
                     query.where_and(up.RealName.Match(Request.Params[cond])); break;
               case "Subject":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(d.DeclareSubjectPKID == Int64.Parse(Request.Params[cond])); break;
               case "Gender":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(up.GenderPKID == Int64.Parse(Request.Params[cond])); break;
               case "Date":
                  if (!string.IsNullOrEmpty(Request.Params[cond]))
                     query.where_and(up.Birthday == DateTime.Parse(Request.Params[cond])); break;
               case "EduBg":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(up.EduBgPKID == Int64.Parse(Request.Params[cond])); break;
               case "EduDegree":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(up.EduDegreePKID == Int64.Parse(Request.Params[cond])); break;
               case "SkillTitle":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(up.SkillTitlePKID == Int64.Parse(Request.Params[cond])); break;
               case "PoliticalStatus":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(up.PoliticalStatusPKID == Int64.Parse(Request.Params[cond])); break;
               case "Nationality":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(up.NationalityPKID == Int64.Parse(Request.Params[cond])); break;
               case "EduSubject":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(up.EduSubjectPKID == Int64.Parse(Request.Params[cond])); break;
               case "EduStage":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(up.EduStagePKID == Int64.Parse(Request.Params[cond])); break;
               case "RankTitle":
                  if (Int64.Parse(Request.Params[cond]) > 0)
                     query.where_and(up.RankTitlePKID == Int64.Parse(Request.Params[cond])); break;
               case "qualityScore_1":
                  if (!string.IsNullOrEmpty(Request.Params[cond]))
                     query.where_and(q.Score > (Int64.Parse(Request.Params[cond]) * 2));
                  break;
               case "qualityScore_2":
                  if (!string.IsNullOrEmpty(Request.Params[cond]))
                     query.where_and(q.Score < (Int64.Parse(Request.Params[cond]) * 2));
                  break;
               case "volumnScore_1":
                  if (!string.IsNullOrEmpty(Request.Params[cond]))
                     query.where_and(v.Score > (Int64.Parse(Request.Params[cond]) * 5));
                  break;
               case "volumnScore_2":
                  if (!string.IsNullOrEmpty(Request.Params[cond]))
                     query.where_and(v.Score < (Int64.Parse(Request.Params[cond]) * 5));
                  break;
               case "schoolScore_1":
                  if (!string.IsNullOrEmpty(Request.Params[cond]))
                     query.where_and(s.Score > (Int64.Parse(Request.Params[cond]) * 3));
                  break;
               case "schoolScore_2":
                  if (!string.IsNullOrEmpty(Request.Params[cond]))
                     query.where_and(s.Score < (Int64.Parse(Request.Params[cond]) * 3));
                  break;
               case "characteristicScore_1":
                  if (!string.IsNullOrEmpty(Request.Params[cond]))
                     query.where_and(q.Characteristic > Int64.Parse(Request.Params[cond]));
                  break;
               case "characteristicScore_2":
                  if (!string.IsNullOrEmpty(Request.Params[cond]))
                     query.where_and(q.Characteristic < Int64.Parse(Request.Params[cond]));
                  break;
            }
         }

         query.primary(up.UserId);

         var result = query.query(db, r =>
         {
            return new TeacherEvalInfo()
            {
               CompanyName = c.CompanyName.GetValue(r,"CName"),
               RealName = up.RealName.GetValue(r),
               Gender = BzUserProfileHelper.Gender.GetName(up.GenderPKID.GetValue(r)),
               Birthday = up.Birthday.GetValue(r),
               EduBg = BzUserProfileHelper.EduBg.GetName(up.EduBgPKID.GetValue(r)),
               Target = DeclareBaseHelper.DeclareTarget.GetName(d.DeclareTargetPKID.GetValue(r)),
               SkillTitle = BzUserProfileHelper.SkillTitle.GetName(up.SkillTitlePKID.GetValue(r)),
               DeclareSubject = DeclareBaseHelper.DeclareSubject.GetName(d.DeclareSubjectPKID.GetValue(r)),
               EduSubject = BzUserProfileHelper.EduSubject.GetName(up.EduSubjectPKID.GetValue(r)),
               EduStage = BzUserProfileHelper.EduStage.GetName(up.EduStagePKID.GetValue(r)),
               EduDegree = BzUserProfileHelper.EduDegree.GetName(up.EduDegreePKID.GetValue(r)),
               SkillDate = up.SkillDate.GetValue(r),
               PoliticalStatus = BzUserProfileHelper.PoliticalStatus.GetName(up.PoliticalStatusPKID.GetValue(r)),
               Nationality = BzUserProfileHelper.Nationality.GetName(up.NationalityPKID.GetValue(r)),
               RankTitle = BzUserProfileHelper.RankTitle.GetName(up.RankTitlePKID.GetValue(r)),
               GraduateSchool = up.GraduateSchool.GetValue(r),
               TrainNo = up.TrainNo.GetValue(r),
               Phonemobile = up.Phonemobile.GetValue(r),
               Email = up.Email.GetValue(r),
               MemberCount = d.MemberCount.GetValue(r),
               QualityScore = q.Score.GetValue(r, "q_score") * 0.5,
               VolumnScore = v.Score.GetValue(r, "v_score") * 0.2,
               SchoolScore = s.Score.GetValue(r, "s_score") * 0.3,
               CharacteristicScore = q.Characteristic.GetValue(r),
               Honor = dc.ContentValue.GetValue(r)
            };
         }).ToList();

         //创建Excel文件的对象
         var book = NPOIHelper(result);

         // 写入到客户端 
         System.IO.MemoryStream ms = new System.IO.MemoryStream();
         book.Write(ms);
         ms.Seek(0, SeekOrigin.Begin);
         DateTime dt = DateTime.Now;
         string dateTime = dt.ToString("yyyyMMdd");
         string fileName = "考核成绩统计" + dateTime + ".xls";
         return File(ms, "application/vnd.ms-excel", fileName);
      }

      private HSSFWorkbook NPOIHelper(List<TeacherEvalInfo> source)
      {
         //创建Excel文件的对象
         NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
         //添加一个sheet
         NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

         #region [头部设计]
         //给sheet1添加第一行的头部标题
         int i = 0;
         NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
         row1.CreateCell(i++).SetCellValue("所在单位");
         row1.CreateCell(i++).SetCellValue("姓名");
         row1.CreateCell(i++).SetCellValue("性别");
         row1.CreateCell(i++).SetCellValue("出生年月");
         row1.CreateCell(i++).SetCellValue("学历");
         row1.CreateCell(i++).SetCellValue("称号");
         row1.CreateCell(i++).SetCellValue("现任专技职称");
         row1.CreateCell(i++).SetCellValue("申报学科");
         row1.CreateCell(i++).SetCellValue("任教学科学段");
         row1.CreateCell(i++).SetCellValue("学位");
         row1.CreateCell(i++).SetCellValue("职称通过年月");
         row1.CreateCell(i++).SetCellValue("政治面貌");
         row1.CreateCell(i++).SetCellValue("民族");
         row1.CreateCell(i++).SetCellValue("行政职务");
         row1.CreateCell(i++).SetCellValue("毕业院校(专业)");
         row1.CreateCell(i++).SetCellValue("师训编号");
         row1.CreateCell(i++).SetCellValue("手机");
         row1.CreateCell(i++).SetCellValue("E-mail");
         row1.CreateCell(i++).SetCellValue("学员数量");
         row1.CreateCell(i++).SetCellValue("总得分/分值");
         row1.CreateCell(i++).SetCellValue("质评得分/分值");
         row1.CreateCell(i++).SetCellValue("量评得分/分值");
         row1.CreateCell(i++).SetCellValue("校评得分/分值");
         row1.CreateCell(i++).SetCellValue("特色得分/分值");
         row1.CreateCell(i++).SetCellValue("荣誉");

         #endregion

         for (var j = 0; j < source.Count; j++)
         {
            var k = 0;
            NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(j + 1);
            rowtemp.CreateCell(k++).SetCellValue(source[j].CompanyName);
            rowtemp.CreateCell(k++).SetCellValue(source[j].RealName);
            rowtemp.CreateCell(k++).SetCellValue(source[j].Gender);
            rowtemp.CreateCell(k++).SetCellValue(source[j].Birthday.ToString());
            rowtemp.CreateCell(k++).SetCellValue(source[j].EduBg);
            rowtemp.CreateCell(k++).SetCellValue(source[j].Target);
            rowtemp.CreateCell(k++).SetCellValue(source[j].SkillTitle);
            rowtemp.CreateCell(k++).SetCellValue(source[j].DeclareSubject);
            rowtemp.CreateCell(k++).SetCellValue(source[j].EduSubject + source[j].EduStage);
            rowtemp.CreateCell(k++).SetCellValue(source[j].EduDegree);
            rowtemp.CreateCell(k++).SetCellValue(source[j].SkillDate.ToString());
            rowtemp.CreateCell(k++).SetCellValue(source[j].PoliticalStatus);
            rowtemp.CreateCell(k++).SetCellValue(source[j].Nationality);
            rowtemp.CreateCell(k++).SetCellValue(source[j].RankTitle);
            rowtemp.CreateCell(k++).SetCellValue(source[j].GraduateSchool);
            rowtemp.CreateCell(k++).SetCellValue(source[j].TrainNo);
            rowtemp.CreateCell(k++).SetCellValue(source[j].Phonemobile);
            rowtemp.CreateCell(k++).SetCellValue(source[j].Email);
            rowtemp.CreateCell(k++).SetCellValue(source[j].MemberCount);
            rowtemp.CreateCell(k++).SetCellValue(source[j].TotalScore);
            rowtemp.CreateCell(k++).SetCellValue(source[j].QualityScore);
            rowtemp.CreateCell(k++).SetCellValue(source[j].VolumnScore);
            rowtemp.CreateCell(k++).SetCellValue(source[j].SchoolScore);
            rowtemp.CreateCell(k++).SetCellValue(source[j].CharacteristicScore);
            rowtemp.CreateCell(k++).SetCellValue(source[j].Honor);
         }

         //var i = 0;
         //foreach (var item in dic.Values)
         //{
         //	i++;
         //	NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 2);
         //	sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i + 2, i + 2, 1, 2));
         //	rowtemp.CreateCell(0).SetCellValue(i);
         //	rowtemp.CreateCell(1).SetCellValue(item.TargetName);
         //	rowtemp.CreateCell(3).SetCellValue(item.SubjectName);
         //	rowtemp.CreateCell(4).SetCellValue(item.TeacherName);
         //	rowtemp.CreateCell(5).SetCellValue(item.CompanyName);
         //	rowtemp.CreateCell(6).SetCellValue(item.TotalStr);
         //	rowtemp.CreateCell(7).SetCellValue(item.SchoolStr);
         //	rowtemp.CreateCell(8).SetCellValue(item.VolumnStr);
         //	rowtemp.CreateCell(9).SetCellValue(item.QualityStr);
         //	rowtemp.CreateCell(10).SetCellValue(item.Quality.TesGongz_Str);
         //	rowtemp.CreateCell(11).SetCellValue(item.School.Shid);
         //	rowtemp.CreateCell(12).SetCellValue(item.School.Volumn_Str);
         //	rowtemp.CreateCell(13).SetCellValue(item.School.Quality_Str);
         //	rowtemp.CreateCell(14).SetCellValue(item.Volumn.DusHuod_Str);
         //	rowtemp.CreateCell(15).SetCellValue(item.Quality.DusHuod_Str);
         //	rowtemp.CreateCell(16).SetCellValue(item.Volumn.KaikPingwPingb_Str);
         //	rowtemp.CreateCell(17).SetCellValue(item.Quality.KaikPingwPingb_Str);
         //	rowtemp.CreateCell(18).SetCellValue(item.Volumn.Key_Str);
         //	rowtemp.CreateCell(19).SetCellValue(item.Quality.Key_Str);
         //	rowtemp.CreateCell(20).SetCellValue(item.Volumn.ZisFaz_Str);
         //	rowtemp.CreateCell(21).SetCellValue(item.Quality.ZisFaz_Str);
         //	rowtemp.CreateCell(22).SetCellValue(item.Volumn.KaisKec_Str);
         //	rowtemp.CreateCell(23).SetCellValue(item.Quality.KaisKec_Str);
         //	rowtemp.CreateCell(24).SetCellValue(item.Volumn.JiangzBaog_Str);
         //	rowtemp.CreateCell(25).SetCellValue(item.Quality.JiangzBaog_Str);
         //	rowtemp.CreateCell(26).SetCellValue(item.Volumn.KecZiy_Str);
         //	rowtemp.CreateCell(27).SetCellValue(item.Quality.KecZiy_Str);
         //	rowtemp.CreateCell(28).SetCellValue(item.Volumn.PeixKec_Str);
         //	rowtemp.CreateCell(29).SetCellValue(item.Quality.PeixKec_Str);
         //	rowtemp.CreateCell(30).SetCellValue(item.Volumn.DaijZhid_Str);
         //	rowtemp.CreateCell(31).SetCellValue(item.Quality.DaijZhid_Str);
         //	rowtemp.CreateCell(32).SetCellValue(item.Volumn.HuodJingfZhid);
         //	rowtemp.CreateCell(33).SetCellValue("无");
         //	rowtemp.CreateCell(34).SetCellValue(item.Volumn.QitDaijGongz_Str);
         //	rowtemp.CreateCell(35).SetCellValue(item.Quality.QitDaijGongz_Str);
         //	rowtemp.CreateCell(36).SetCellValue(item.Volumn.DaijChengg_Str);
         //	rowtemp.CreateCell(37).SetCellValue(item.Quality.DaijChengg_Str);
         //	rowtemp.CreateCell(38).SetCellValue(item.Volumn.DaijJiaos_Str);
         //	rowtemp.CreateCell(39).SetCellValue(item.Quality.DaijJiaos_Str);
         //}

         return book;
      }

   }

}