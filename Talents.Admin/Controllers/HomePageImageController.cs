using Business;
using Symber.Web.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace TheSite.Controllers
{

	public class HomePageImageController : BaseController
	{

		static APDBDef.HomePageImageTableDef h = APDBDef.HomePageImage;


		//	GET: HomePageImage/List
		//	POST-Ajax:	HomePageImage/List

		public ActionResult List()
		{
			return View();
		}

		[HttpPost]
		public ActionResult List(int current, int rowCount, AjaxOrder sort, string searchPhrase)
		{
			ThrowNotAjax();


			var query = APQuery.select(h.ImgName, h.UploadDate, h.UseDate, h.ImgType, h.ImgUrl, h.ImgId)
				.from(h)
				.primary(h.ImgId)
				.skip((current - 1) * rowCount)
				.take(rowCount);


			//过滤条件
			//模糊搜索姓名,标题

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(h.ImgName.Match(searchPhrase));
			}


			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "name": query.order_by(sort.OrderBy(h.ImgName)); break;
					case "updateDate": query.order_by(sort.OrderBy(h.UploadDate)); break;
					case "useDate": query.order_by(sort.OrderBy(h.UseDate)); break;
					case "imgType": query.order_by(sort.OrderBy(h.ImgType)); break;
				}
			}


			var total = db.ExecuteSizeOfSelect(query);

			var result = query.query(db, rd =>
			{
				return new
				{
					id = h.ImgId.GetValue(rd),
					name = h.ImgName.GetValue(rd),
					updateDate = h.UploadDate.GetValue(rd),
					useDate = h.UseDate.GetValue(rd),
					imgType = h.ImgType.GetValue(rd),
					url = h.ImgUrl.GetValue(rd)
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


		//	GET:	HomePageImage/Edit
		//	POST-Ajax:	HomePageImage/Edit

		public ActionResult Edit()
		{
			var model = new HomePageImage { ImgUrl = "/assets/img/News404.jpg" };

			return PartialView("Edit", model);
		}

		[HttpPost]
		public ActionResult Edit(HomePageImage model)
		{
			ThrowNotAjax();


			string[] urlname = model.ImgUrl.Split('/');
			model.ImgName = urlname[urlname.Length - 1];
			model.UploadDate = DateTime.Now;
			model.UseDate = DateTime.Now;
			db.HomePageImageDal.Insert(model);


			return Json(new
			{
				result = AjaxResults.Success,
				msg = "图片已保存！",
			});
		}


		//	GET:	HomePageImage/Details	

		public ActionResult Details(string url)
		{
			return PartialView("Details", url);
		}


		//	POST-Ajax:	HomePageImage/Approve

		[HttpPost]
		public ActionResult Approve(long id, bool imgType)
		{
			ThrowNotAjax();


			db.HomePageImageDal.UpdatePartial(id, new
			{
				ImgType = !imgType
			});
			

			return Json(new
			{
				result = AjaxResults.Success,
				msg = string.Format("{0}架已成功！", imgType ? "下" : "上")
			});
		}


		//	POST-Ajax：	HomePageImage/Remove

		[HttpPost]
		public ActionResult Remove(long id)
		{
			ThrowNotAjax();

			db.HomePageImageDal.PrimaryDelete(id);

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "图片已删除！"
			});
		}

	}

}