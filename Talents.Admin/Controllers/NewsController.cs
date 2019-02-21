using Business;
using Symber.Web.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace TheSite.Controllers
{

	public class NewsController : BaseController
	{

		static APDBDef.NewsTableDef n = APDBDef.News;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;


		//	GET: News/List
		//	POST-Ajax:	News/List

		public ActionResult List()
		{
			return View();
		}

		[HttpPost]
		public ActionResult List(int current, int rowCount, AjaxOrder sort, string searchPhrase)
		{
			ThrowNotAjax();


			var query = APQuery.select(n.NewsId, n.Title, u.RealName, n.CreatedTime)
				.from(n, u.JoinInner(n.Creator == u.UserId))
				.primary(n.NewsId)
				.skip((current - 1) * rowCount)
				.take(rowCount);


			//过滤条件
			//模糊搜索姓名,标题

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(u.RealName.Match(searchPhrase) | n.Title.Match(searchPhrase));
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
					case "title": query.order_by(sort.OrderBy(n.Title)); break;
					case "createdTime": query.order_by(sort.OrderBy(n.CreatedTime)); break;
				}
			}


			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				return new
				{
					id = n.NewsId.GetValue(rd),
					title = n.Title.GetValue(rd),
					realName = u.RealName.GetValue(rd),
					createdTime = n.CreatedTime.GetValue(rd)
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


		//	GET:	News/Edit
		//	POST-Ajax:	News/Edti

		public ActionResult Edit(long? id)
		{
			var model = id == null ? new News() :
				db.NewsDal.PrimaryGet(id.Value);
			
			return View(model);
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Edit(News model)
		{
			if (model.NewsId == 0)
			{
				model.Creator = UserProfile.UserId;
				model.CreatedTime = DateTime.Now;
				db.NewsDal.Insert(model);
			}
			else
			{
				db.NewsDal.UpdatePartial(model.NewsId, new
				{
					model.ThumbUrl,
					model.Title,
					model.Content
				});
			}
			

			return RedirectToAction("Details", new { id = model.NewsId });
		}


		//	POST-Ajax：	News/Remove

		[HttpPost]
		public ActionResult Remove(long id)
		{
			ThrowNotAjax();

			db.NewsDal.PrimaryDelete(id);

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已删除！"
			});
		}


		//	GET:	News/Details

		public ActionResult Details(long id)
		{
			var model = db.NewsDal.PrimaryGet(id);

			return View(model);
		}

	}

}