using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Linq;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class NoticeController : BaseController
	{

		static APDBDef.NoticeTableDef n = APDBDef.Notice;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;


		//	通知列表
		//	GET:	/Notice/List
		//	POST-AJAX:	/Notice/List

		public ActionResult List()
		{
			return View();
		}

		[HttpPost]
		public ActionResult List(int current, int rowCount, AjaxOrder sort, string searchPhrase)
		{
			ThrowNotAjax();


			var query = APQuery.select(n.NoticeId, n.Title, u.RealName, n.IsSend, n.CreatedTime)
				.from(n, u.JoinInner(n.Creator == u.UserId))
				.primary(n.NoticeId)
				.where(n.IsSend == true)
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
					id = n.NoticeId.GetValue(rd),
					title = n.Title.GetValue(rd),
					realName = u.RealName.GetValue(rd),
					createdTime = n.CreatedTime.GetValue(rd),
					isSend = n.IsSend.GetValue(rd)
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


		//	通知编辑
		//	GET:	/Notice/Edit
		//	POST-AJAX:	/Notice/Edit

		public ActionResult Edit(long? id)
		{
			var model = new NoticeModel() { AttachmentName = "" };

			if (id != null)
			{
				var data = db.NoticeDal.PrimaryGet(id.Value);

				model.NoticeId = data.NoticeId;
				model.Title = data.Title;
				model.IsSend = data.IsSend;
				model.Content = data.Content;

				var atta = AttachmentsExtensions.GetAttachment(
					AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.Notice));
				if (atta != null)
				{
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}
			}


			return View(model);
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Edit(long? id, NoticeModel model)
		{
			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.Notice,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{

				if (id == null)
				{
					var data = new Notice
					{
						Title = model.Title,
						IsSend = model.IsSend,
						Content = model.Content,
						CreatedTime = DateTime.Now,
						Creator = UserProfile.UserId,
					};

					db.NoticeDal.Insert(data);
					atta.JoinId = data.NoticeId;
				}
				else
				{
					db.NoticeDal.UpdatePartial(id.Value, new
					{
						model.Title,
						model.IsSend,
						model.Content
					});

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.Notice);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
				db.Commit();
			}
			catch (Exception)
			{
				db.Rollback();
			}


			return RedirectToAction("Details", new { id = atta.JoinId });
		}


		//	通知明细
		//	GET:	/Notice/Details

		public ActionResult Details(long id)
		{
			var model = db.NoticeDal.PrimaryGet(id);

			ViewBag.AttachmentList = AttachmentsExtensions.GetAttachmentList(db, id, AttachmentsKeys.Notice);


			var t = APDBDef.ReadNotice;

			if (db.ReadNoticeDal.ConditionQueryCount(t.NoticeId == id & t.UserId == UserProfile.UserId) == 0)
			{
				db.ReadNoticeDal.Insert(new ReadNotice
				{
					NoticeId = id,
					UserId = UserProfile.UserId,
					ReadTime = DateTime.Now
				});
			}


			return View(model);
		}
		
	}
}