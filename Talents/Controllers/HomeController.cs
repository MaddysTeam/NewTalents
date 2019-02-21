using Business;
using Business.Config;
using Symber.Web.Data;
using System.Linq;
using System.Web.Mvc;
using TheSite.Models;
using Webdiyer.WebControls.Mvc;

namespace Talents.Controllers
{

   public class HomeController : BaseController
   {

      //	首页
      // GET:  /Home/Index

      public ActionResult Index()
      {
         //获取新闻列表

         var t = APDBDef.News;

         ViewBag.NewsList = APQuery.select(t.Asterisk, t.Title)
            .from(t)
            .take(10)
            .primary(t.NewsId)
            .order_by(t.CreatedTime.Desc)
            .query(db, t.TolerantMap).ToList();


         // 获取图片信息

         var t1 = APDBDef.HomePageImage;
         ViewBag.ImgList = db.HomePageImageDal.ConditionQuery(t1.ImgType == true, t1.UploadDate.Desc, null, null);

         return View();
      }


      //	新闻列表
      // GET:  /Home/News

      public ActionResult News(int pageIndex = 1)
      {

         var t = APDBDef.News;
         var t1 = APDBDef.BzUserProfile;

         var result = APQuery.select(t.NewsId, t.Title, t.CreatedTime, t1.RealName, t.ThumbUrl, t.Content)
            .from(t, t1.JoinInner(t.Creator == t1.UserId))
            .primary(t.NewsId)
            .order_by(t.CreatedTime.Desc)
            .query(db, r =>
            {
               var data = new NewsModel();
               t.Fullup(r, data, false);
               data.RealName = t1.RealName.GetValue(r);

               return data;
            }).ToList();


         return View(result.ToPagedList(pageIndex, ThisApp.PageSize));
      }


      //	新闻详细
      // GET:  /Home/NewsDetail

      public ActionResult NewsDetail(long id = 5001)
      {

         var t = APDBDef.News;
         var t1 = APDBDef.BzUserProfile;

         var result = APQuery.select(t.Title, t.Content, t.ThumbUrl, t.CreatedTime, t1.RealName)
            .from(t, t1.JoinInner(t.Creator == t1.UserId))
            .where(t.NewsId == id)
            .query(db, r =>
            {
               var data = new NewsModel();
               t.Fullup(r, data, false);
               data.RealName = t1.RealName.GetValue(r);

               return data;
            }).ToList().First();


         return View(result);
      }


      // GET:  /Home/Corps

      public ActionResult Corps(long? target, long? corp, int pageIndex = 1)
      {
         var t = APDBDef.TeamActive;
         var d = APDBDef.DeclareBase;

         var query =
           APQuery.select(d.TeamName, t.TeamId, t.TeamActiveId, t.Title, t.Date, t.ContentValue, t.IsShare)
           .from(t,
                 d.JoinInner(t.TeamId == d.TeacherId & d.HasTeam == true))
                 .where(t.IsShow == true)
                 .order_by(t.Date.Desc)
              .primary(t.TeamActiveId)
              .take(ThisApp.PageSize)
              .skip((pageIndex - 1) * 100);

         if (target != null)
            query.where_and(d.DeclareTargetPKID == target);
         else if (corp != null)
            query.where_and(t.TeamId == corp);

         var model = query.query(db, r =>
         {
            var m = new HomeTeamActiveModel();
            t.Fullup(r, m, false);
            m.TeamName = d.TeamName.GetValue(r);
            return m;
         }).ToList();


         return View(model.ToPagedList(pageIndex, ThisApp.PageSize));
      }


      public ActionResult SharedCorps(long? target, long? corp, int pageIndex = 1)
      {
         var t = APDBDef.TeamActive;
         var d = APDBDef.DeclareBase;
         var a = APDBDef.Attachments;

         var query =
           APQuery.select(d.TeamName, t.TeamId, t.TeamActiveId, t.Title, t.Date, t.ContentValue, t.IsShare, a.ID.Count().As("AttachmentCount"))
           .from(t,
                 d.JoinInner(t.TeamId == d.TeacherId & d.HasTeam == true),
                 a.JoinInner(t.TeamActiveId == a.JoinId))
               .where(t.IsShare == true & a.Type == "带教活动.编辑")
               .group_by(d.TeamName, t.TeamId, t.TeamActiveId, t.Title, t.Date, t.ContentValue, t.IsShare);


         query.order_by(t.Date.Desc)
                 .primary(t.TeamActiveId)
                 .take(ThisApp.PageSize)
                 .skip((pageIndex - 1) * 100);

         if (target != null)
            query.where_and(d.DeclareTargetPKID == target);
         else if (corp != null)
            query.where_and(t.TeamId == corp);

         var model = query.query(db, r =>
         {
            var m = new HomeTeamActiveModel();
            t.Fullup(r, m, false);
            m.ContentValue = SubString(m.ContentValue);
            m.TeamName = d.TeamName.GetValue(r);
            m.AttachmentCount = (int)r.GetValue(r.GetOrdinal("AttachmentCount"));
            return m;
         }).ToList();


         return View("Corps", model.ToPagedList(pageIndex, ThisApp.PageSize));
      }


      // GET:  /Home/CorpDetail

      public ActionResult CorpDetail(long id)
      {
         var t = APDBDef.TeamActive;
         var a = APDBDef.Attachments;

         var model = db.TeamActiveDal.PrimaryGet(id);

         if (model != null && model.IsShare)
            ViewBag.Attachments = db.AttachmentsDal.ConditionQuery(a.JoinId == model.TeamActiveId & a.Type == "带教活动.编辑", null, null, null);

         return View(model);
      }


      // GET:  /Home/CorpList

      public ActionResult CorpList(long? target)
      {
         var t = APDBDef.TeamActive;
         var d = APDBDef.DeclareBase;
         var p = APDBDef.PicklistItem;
         var p1 = APDBDef.PicklistItem.As("p1");

         var query =
            APQuery.select(d.TeacherId, d.TeamName, d.MemberCount, t.TeamActiveId.Count().As("taskcount"), p.Name, p1.Name.As("targetName"))
            .from(d,
                  t.JoinLeft(d.TeacherId == t.TeamId),
                  p.JoinInner(d.DeclareSubjectPKID == p.PicklistItemId),
                  p1.JoinInner(d.DeclareTargetPKID == p1.PicklistItemId))
            .where(t.IsShow == true & d.HasTeam == true)
            .group_by(d.TeacherId, d.TeamName, d.DeclareTargetPKID, p.Name, p1.Name, d.MemberCount);

         if (target != null)
            query.where(d.DeclareTargetPKID == target);

         var model = query.query(db, r =>
         {
            var m = new HomeCorpInfoModel();
            d.Fullup(r, m, false);
            m.SubjectName = p.Name.GetValue(r);
            m.TargetName = r.GetValue(r.GetOrdinal("targetName")).ToString();
            var cnt = r.GetValue(r.GetOrdinal("taskcount"));
            if (cnt != System.DBNull.Value)
               m.TaskCount = (int)cnt;
            return m;
         }).ToList();


         return View(model);
      }


      // GET:  /Home/Share

      public ActionResult Share(long? target, int pageIndex = 1)
      {
         var s = APDBDef.Share;
         var u = APDBDef.BzUserProfile;
         var a = APDBDef.Attachments;
         var d = APDBDef.DeclareBase;

         var query = APQuery.select(s.ShareId, s.ItemId, s.Title, u.RealName, s.Type, s.PubishDate, s.CreateDate, a.ID.Count().As("AttachmentCount"))
                .from(s,
                      d.JoinInner(d.TeacherId==s.UserId),
                      u.JoinInner(s.UserId == u.UserId),
                      a.JoinInner(s.ItemId == a.JoinId))
                .group_by(s.ShareId, s.ItemId, s.Title, u.RealName, s.Type, s.PubishDate, s.CreateDate)
                .where(s.ParentType == "ShareActive" | s.ParentType == "ShareAchievement");

         if (target > 0)
            query.where_and(d.DeclareTargetPKID == target);

          var result=query.query(db, r =>
                {
                   return new ShareModel
                   {
                      ShareId = s.ShareId.GetValue(r),
                      RealName = u.RealName.GetValue(r),
                      Type = s.Type.GetValue(r),
                      Title = SubString(s.Title.GetValue(r)),
                      ItemId = s.ItemId.GetValue(r),
                      CreateDate = s.CreateDate.GetValue(r),
                      PubishDate = s.PubishDate.GetValue(r),
                      AttachmentCount = (int)r.GetValue(r.GetOrdinal("AttachmentCount"))
                   };
                }).ToList();


         return View(result.ToPagedList(pageIndex, ThisApp.PageSize));
      }


      // GET:  /Home/ShareDetails

      public ActionResult ShareDetails(long id)
      {
         var s = APDBDef.Share;
         var u = APDBDef.BzUserProfile;
         var a = APDBDef.Attachments;

         var result = APQuery.select(s.ShareId, s.ItemId, s.Title, s.Type, s.PubishDate, s.CreateDate, u.RealName)
            .from(s,
                  u.JoinInner(s.UserId == u.UserId))
            .where(s.ShareId == id)
            .query(db, r =>
            {
               return new ShareModel
               {
                  ShareId = s.ShareId.GetValue(r),
                  RealName = u.RealName.GetValue(r),
                  Type = s.Type.GetValue(r),
                  Title = s.Title.GetValue(r),
                  ItemId = s.ItemId.GetValue(r),
                  CreateDate = s.CreateDate.GetValue(r),
                  PubishDate = s.PubishDate.GetValue(r),
               };
            }).FirstOrDefault();

         if (result != null)
            ViewBag.Attachments = db.AttachmentsDal.ConditionQuery(a.JoinId == result.ItemId, null, null, null);


         return View(result);
      }


      // GET:  /Home/NoitceList

      public ActionResult NoticeList(int pageIndex = 1)
      {
         var n = APDBDef.Notice;
         var u = APDBDef.BzUserProfile;
         var a = APDBDef.Attachments;

         var result = APQuery.select(n.NoticeId, n.Title, u.RealName, n.IsSend, n.CreatedTime, n.Content, a.ID.Count().As("AttachmentCount"))
            .from(n,
                  u.JoinInner(n.Creator == u.UserId),
                  a.JoinLeft(a.JoinId == n.NoticeId & a.Type == "通知"))
            .group_by(n.NoticeId, n.Title, u.RealName, n.IsSend, n.CreatedTime, n.Content)
            .query(db, r =>
            {
               return new NoticeModel
               {
                  NoticeId = n.NoticeId.GetValue(r),
                  Title = n.Title.GetValue(r),
                  Content = n.Content.GetValue(r),
                  CreatedTime = n.CreatedTime.GetValue(r),
                  Publisher = u.RealName.GetValue(r),
                  AttachmentCount = (int)r.GetValue(r.GetOrdinal("AttachmentCount"))
               };
            }).ToList();


         return View(result.ToPagedList(pageIndex, ThisApp.PageSize));
      }


      // GET:  /Home/NoticeDetails

      public ActionResult NoticeDetails(long id)
      {
         var n = APDBDef.Notice;
         var u = APDBDef.BzUserProfile;
         var a = APDBDef.Attachments;

         var result = APQuery.select(n.NoticeId, n.Title, u.RealName, n.IsSend, n.CreatedTime, n.Content)
            .from(n, u.JoinInner(n.Creator == u.UserId))
            .where(n.NoticeId == id)
            .query(db, r =>
            {
               return new NoticeModel
               {
                  NoticeId = n.NoticeId.GetValue(r),
                  Title = n.Title.GetValue(r),
                  Content = n.Content.GetValue(r),
                  CreatedTime = n.CreatedTime.GetValue(r),
                  Publisher = u.RealName.GetValue(r)
               };
            }).FirstOrDefault();


         if (result != null)
            ViewBag.Attachments = db.AttachmentsDal.ConditionQuery(a.JoinId == result.NoticeId & a.Type == "通知", null, null, null);

         return View(result);
      }

      //	GET:		/Home/Task

      public ActionResult Task(long id = 0)
      {
         return View();
      }


      //	GET:		/Home/TaskDetail

      public ActionResult TaskDetail(long id = 0)
      {
         return View();
      }


      // GET:  /Home/Contact

      public ActionResult Contact()
      {
         return View();
      }


      // GET:  /Home/About

      public ActionResult About()
      {
         return View();
      }


      // GET:  /Home/FAQ

      public ActionResult FAQ()
      {
         return View();
      }


      private string SubString(string str)
        => str.Length > 50 ? str.Substring(0, 50) + "..." : str;
   }

}