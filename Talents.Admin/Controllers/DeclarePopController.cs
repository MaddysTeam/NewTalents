using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TheSite.Models;


namespace TheSite.Controllers
{

	// 用户填写申报信息页面 对话框

	public class DeclarePopController : BaseController
	{

		private static APDBDef.DeclareActiveTableDef t = APDBDef.DeclareActive;
		private static APDBDef.DeclareAchievementTableDef ta = APDBDef.DeclareAchievement;


		#region [ 自身发展.个人简历 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_GerJianl
		//	POST-Ajax:			DeclarePop/ZisFaz_GerJianl

		public ActionResult ZisFaz_GerJianl(long? id)
		{
			if (id == null)
				return PartialView("ZisFaz_GerJianl");

			var model = db.DeclareResumeDal.PrimaryGet(id.Value);

			return PartialView("ZisFaz_GerJianl", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_GerJianl(long? id, DeclareResume model)
		{
			ThrowNotAjax();

			db.BeginTrans();

			try
			{

				if (id == null)
				{
					model.TeacherId = UserProfile.UserId;
					model.Creator = UserProfile.UserId;
					model.CreateDate = DateTime.Now;
					db.DeclareResumeDal.Insert(model);
				}
				else
				{
					model.Modifier = UserProfile.UserId;
					model.ModifyDate = DateTime.Now;
					db.DeclareResumeDal.UpdatePartial(id.Value, new
					{
						model.DateRegion,
						model.Company,
						model.Title,
						model.IsDeclare
					});

					model = db.DeclareResumeDal.PrimaryGet(id.Value);
				}

				DeclareMaterialHelper.AddDeclareMaterial(model, Period, db);

				db.Commit();
			}
			catch
			{
				db.Rollback();
			}

			var doSomthing = id == null ? "新增:" + id : "修改:" + id;
			Log(DeclareKeys.ZisFaz_GerJianl, doSomthing);


			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.自我研修 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_ZiwYanx
		//	POST-Ajax:			DeclarePop/ZisFaz_ZiwYanx

		public ActionResult ZisFaz_ZiwYanx(long? id)
		{
			var model = new ZisFaz_ZiwYanxModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_ZiwYanxModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare
					};

					var at = AttachmentsExtensions.GetAttachment(AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.ZisFaz_ZiwYanx));

					model.AttachmentName = at.Name;
					model.AttachmentUrl = at.Url;
				}
			}


			return PartialView("ZisFaz_ZiwYanx", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_ZiwYanx(long? id, ZisFaz_ZiwYanxModel model)
		{
			ThrowNotAjax();

			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.ZisFaz_ZiwYanx,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZisFaz_ZiwYanx,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						IsShare = model.IsShare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now,
						IsDeclare = model.IsDeclare
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					.set(t.Date.SetValue(model.Date))
					.set(t.Location.SetValue(model.Location))
					.set(t.ContentValue.SetValue(model.ContentValue))
					//.set(t.IsShare.SetValue(model.IsShare))
					.set(t.Modifier.SetValue(UserProfile.UserId))
					.set(t.ModifyDate.SetValue(DateTime.Now))
					.set(t.IsDeclare.SetValue(model.IsDeclare))
					.where(t.DeclareActiveId == id.Value)
					.execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.ZisFaz_ZiwYanx);
					atta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);

				}

				AttachmentsExtensions.InsertAtta(db, atta);

				//TODO: AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db);

				db.Commit();


				var doSomthing = id == null ? "新增:" + id : "修改:" + id;
				if (!string.IsNullOrEmpty(atta.Name))
					doSomthing += string.Format(" 并且上传了附件:{0}", atta.Name);

				Log(DeclareKeys.ZisFaz_ZiwYanx, doSomthing);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.教学活动.开设教学公开课 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_JiaoxHuod_JiaoxGongkk
		//	POST-Ajax:			DeclarePop/ZisFaz_JiaoxHuod_JiaoxGongkk

		public ActionResult ZisFaz_JiaoxHuod_JiaoxGongkk(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new ZisFaz_JiaoxHuod_JiaoxGongkkModel() { Date = System.DateTime.Now, AttachmentName = "", IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_JiaoxHuod_JiaoxGongkkModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Level = list.Level,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_JiaoxHuod_JiaoxGongkk);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_JiaoxHuod_JiaoxGongkk + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("ZisFaz_JiaoxHuod_JiaoxGongkk", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_JiaoxHuod_JiaoxGongkk(long? id, ZisFaz_JiaoxHuod_JiaoxGongkkModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.ZisFaz_JiaoxHuod_JiaoxGongkk;
			var vertifyTypeKey = AttachmentsKeys.ZisFaz_JiaoxHuod_JiaoxGongkk + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZisFaz_JiaoxHuod_JiaoxGongkk,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Level = model.Level,
						IsShare = model.IsShare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now,
						IsDeclare = model.IsDeclare

					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Level.SetValue(model.Level))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });

					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				// AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.教学活动.开设研讨课 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_JiaoxHuod_Yantk
		//	POST-Ajax:			DeclarePop/ZisFaz_JiaoxHuod_Yantk

		public ActionResult ZisFaz_JiaoxHuod_Yantk(long? id)
		{
			var model = new ZisFaz_JiaoxHuod_YantkModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_JiaoxHuod_YantkModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Level = list.Level,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_JiaoxHuod_Yantk);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_JiaoxHuod_Yantk + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}


			return PartialView("ZisFaz_JiaoxHuod_Yantk", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_JiaoxHuod_Yantk(long? id, ZisFaz_JiaoxHuod_YantkModel model)
		{
			ThrowNotAjax();


			var attachmentTypeKey = AttachmentsKeys.ZisFaz_JiaoxHuod_Yantk;
			var vertifyTypeKey = AttachmentsKeys.ZisFaz_JiaoxHuod_Yantk + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};


			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZisFaz_JiaoxHuod_Yantk,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Level = model.Level,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now,
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Level.SetValue(model.Level))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.ZisFaz_JiaoxHuod_Yantk);
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				//TODO: AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db);

				db.Commit();

				var doSomthing = id == null ? "新增:" + id : "修改:" + id;
				if (!string.IsNullOrEmpty(atta.Name))
					doSomthing += string.Format(" 并且上传了附件:{0}", atta.Name);

				Log(DeclareKeys.ZisFaz_JiaoxHuod_Yantk, doSomthing);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.教学活动.参加教学评比 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_JiaoxHuod_JiaoxPingb
		//	POST-Ajax:			DeclarePop/ZisFaz_JiaoxHuod_JiaoxPingb

		public ActionResult ZisFaz_JiaoxHuod_JiaoxPingb(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new ZisFaz_JiaoxHuod_JiaoxPingbModel() { Date = System.DateTime.Now, AttachmentName = "", IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_JiaoxHuod_JiaoxPingbModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Level = list.Level,
						Dynamic2 = list.Dynamic2,
						Dynamic1 = list.Dynamic1,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_JiaoxHuod_JiaoxPingb);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_JiaoxHuod_JiaoxPingb + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}

			}

			return PartialView("ZisFaz_JiaoxHuod_JiaoxPingb", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_JiaoxHuod_JiaoxPingb(long? id, ZisFaz_JiaoxHuod_JiaoxPingbModel model)
		{
			ThrowNotAjax();


			var attachmentTypeKey = AttachmentsKeys.ZisFaz_JiaoxHuod_JiaoxPingb;
			var vertifyTypeKey = AttachmentsKeys.ZisFaz_JiaoxHuod_JiaoxPingb + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZisFaz_JiaoxHuod_JiaoxPingb,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Level = model.Level,
						//IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Level.SetValue(model.Level))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				//TODO:AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.培训与讲座.开设教师培训课程 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_PeixJiangz_JiaosPeixKec
		//	POST-Ajax:			DeclarePop/ZisFaz_PeixJiangz_JiaosPeixKec

		public ActionResult ZisFaz_PeixJiangz_JiaosPeixKec(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new ZisFaz_PeixJiangzModel() { Date = System.DateTime.Now, AttachmentName = "", IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_PeixJiangzModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
                  var ats1 = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_PeixJiangz_JiaosPeixKec + AttachmentsKeys.StartStage);
                  var ats2 = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_PeixJiangz_JiaosPeixKec + AttachmentsKeys.ProcessStage);
                  var ats3 = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_PeixJiangz_JiaosPeixKec + AttachmentsKeys.EndStage);
                  var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_PeixJiangz_JiaosPeixKec + AttachmentsKeys.Vertify);
                  var at1 = AttachmentsExtensions.GetAttachment(ats1);
                  var at2 = AttachmentsExtensions.GetAttachment(ats2);
                  var at3 = AttachmentsExtensions.GetAttachment(ats3);
                  var vt = AttachmentsExtensions.GetAttachment(vts);
                  model.AttachmentName1 = at1.Name;
                  model.AttachmentUrl1 = at1.Url;
                  model.AttachmentName2 = at2.Name;
                  model.AttachmentUrl2 = at2.Url;
                  model.AttachmentName3 = at3.Name;
                  model.AttachmentUrl3 = at3.Url;
                  model.VertificationName = vt.Name;
                  model.VertificationUrl = vt.Url;
               }
				}
			}


			return PartialView("ZisFaz_PeixJiangz_JiaosPeixKec", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_PeixJiangz_JiaosPeixKec(long? id, ZisFaz_PeixJiangzModel model)
		{
			ThrowNotAjax();

         string attachmentTypeKey = AttachmentsKeys.ZisFaz_PeixJiangz_JiaosPeixKec,
         vertifyTypeKey = AttachmentsKeys.ZisFaz_PeixJiangz_JiaosPeixKec + AttachmentsKeys.Vertify,
         typeKey1 = attachmentTypeKey + AttachmentsKeys.StartStage,
         typeKey2 = attachmentTypeKey + AttachmentsKeys.ProcessStage,
         typeKey3 = attachmentTypeKey + AttachmentsKeys.EndStage;

         var atta1 = new AttachmentsDataModel
         {
            Type = typeKey1,
            Name = model.AttachmentName1,
            Url = model.AttachmentUrl1,
            UserId = UserProfile.UserId
         };

         var atta2 = new AttachmentsDataModel
         {
            Type = typeKey2,
            Name = model.AttachmentName2,
            Url = model.AttachmentUrl2,
            UserId = UserProfile.UserId
         };

         var atta3 = new AttachmentsDataModel
         {
            Type = typeKey3,
            Name = model.AttachmentName3,
            Url = model.AttachmentUrl3,
            UserId = UserProfile.UserId
         };

         var vertAtta = new AttachmentsDataModel
         {
            Type = vertifyTypeKey,
            Name = model.VertificationName,
            Url = model.VertificationUrl,
            UserId = UserProfile.UserId
         };

         DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZisFaz_PeixJiangz_JiaosPeixKec,
						Date = model.Date,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now,
					};

					db.DeclareActiveDal.Insert(data);
               atta1.JoinId = data.DeclareActiveId;
               atta2.JoinId = data.DeclareActiveId;
               atta3.JoinId = data.DeclareActiveId;
               vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   // .set(t.IsShare.SetValue(model.IsShare))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { typeKey1, typeKey2, typeKey3, vertifyTypeKey });
               atta1.JoinId = id.Value;
               atta2.JoinId = id.Value;
               atta3.JoinId = id.Value;
               vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta1, atta2, atta3, vertAtta });

				//TODO: AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.培训与讲座.开设学科类专题讲座 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_PeixJiangz_ZhuantJiangz
		//	POST-Ajax:			DeclarePop/ZisFaz_PeixJiangz_ZhuantJiangz

		public ActionResult ZisFaz_PeixJiangz_ZhuantJiangz(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new ZisFaz_PeixJiangz_ZhuantJiangzModel() { Date = System.DateTime.Now, AttachmentName = "", IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_PeixJiangz_ZhuantJiangzModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Level = list.Level,
						Dynamic1 = list.Dynamic1,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_PeixJiangz_ZhuantJiangz);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_PeixJiangz_ZhuantJiangz + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("ZisFaz_PeixJiangz_ZhuantJiangz", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_PeixJiangz_ZhuantJiangz(long? id, ZisFaz_PeixJiangz_ZhuantJiangzModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.ZisFaz_PeixJiangz_ZhuantJiangz;
			var vertifyTypeKey = AttachmentsKeys.ZisFaz_PeixJiangz_ZhuantJiangz + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZisFaz_PeixJiangz_ZhuantJiangz,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Level = model.Level,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Level.SetValue(model.Level))
					   .set(t.Location.SetValue(model.Location))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				//TODO:AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.培训与讲座.定向性课程实施 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_PeixJiangz_DingxxKec
		//	POST-Ajax:			DeclarePop/ZisFaz_PeixJiangz_DingxxKec

		public ActionResult ZisFaz_PeixJiangz_DingxxKec(long? id)
		{
			var model = new ZisFaz_PeixJiangz_DingxxKecModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_PeixJiangz_DingxxKecModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						Level = list.Level
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_PeixJiangz_DingxxKec);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_PeixJiangz_DingxxKec + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}

			}


			return PartialView("ZisFaz_PeixJiangz_DingxxKec", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_PeixJiangz_DingxxKec(long? id, ZisFaz_PeixJiangz_DingxxKecModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.ZisFaz_PeixJiangz_DingxxKec;
			var vertifyTypeKey = AttachmentsKeys.ZisFaz_PeixJiangz_DingxxKec + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZisFaz_PeixJiangz_DingxxKec,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now,
						Level = model.Level
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Level.SetValue(model.Level))
					   .set(t.Location.SetValue(model.Location))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				//TODO:AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.培训与讲座.课程资源开发 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_PeixJiangz_KecZiyKaif
		//	POST-Ajax:			DeclarePop/ZisFaz_PeixJiangz_KecZiyKaif

		public ActionResult ZisFaz_PeixJiangz_KecZiyKaif(long? id)
		{
			var model = new ZhidJians_TesHuodKaizModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZhidJians_TesHuodKaizModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						Level = list.Level,
						Dynamic9 = list.Dynamic9,
						IsDeclare = list.IsDeclare
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_PeixJiangz_KecZiyKaif);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_PeixJiangz_KecZiyKaif + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}

			}

			return PartialView("ZisFaz_PeixJiangz_KecZiyKaif", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_PeixJiangz_KecZiyKaif(long? id, ZhidJians_TesHuodKaizModel model)
		{
			ThrowNotAjax();


			var attachmentTypeKey = AttachmentsKeys.ZisFaz_PeixJiangz_KecZiyKaif;
			var vertifyTypeKey = AttachmentsKeys.ZisFaz_PeixJiangz_KecZiyKaif + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZisFaz_PeixJiangz_KecZiyKaif,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Level = model.Level,
						Dynamic9 = model.Dynamic9,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now,
						IsDeclare = model.IsDeclare
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Level.SetValue(model.Level))
					   .set(t.Dynamic9.SetValue(model.Dynamic9))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.学术活动 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_XuesHuod
		//	POST-Ajax:			DeclarePop/ZisFaz_XuesHuod

		public ActionResult ZisFaz_XuesHuod(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new ZisFaz_XuesHuodModel() { Date = System.DateTime.Now, AttachmentName = "", IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_XuesHuodModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						Dynamic9 = list.Dynamic9,
						Level = list.Level,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_XuesHuod);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_XuesHuod + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("ZisFaz_XuesHuod", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_XuesHuod(long? id, ZisFaz_XuesHuodModel model)
		{
			ThrowNotAjax();


			var attachmentTypeKey = AttachmentsKeys.ZisFaz_XuesHuod;
			var vertifyTypeKey = AttachmentsKeys.ZisFaz_XuesHuod + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZisFaz_XuesHuod,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Dynamic9 = model.Dynamic9,
						Level = model.Level,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Dynamic9.SetValue(model.Dynamic9))
					   .set(t.Level.SetValue(model.Level))
					   // .set(t.IsShare.SetValue(model.IsShare))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				//AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.教育教学科研成果.开展课题(项目)研究工作 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_KeyChengg_KetYanj
		//	POST-Ajax:			DeclarePop/ZisFaz_KeyChengg_KetYanj

		public ActionResult ZisFaz_KeyChengg_KetYanj(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new ZisFaz_KeyChengg_KetYanjModel() { IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareAchievementDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_KeyChengg_KetYanjModel()
					{
						DeclareAchievementId = list.DeclareAchievementId,
						DateRegion = list.DateRegion,
						//Dynamic1 = bool.Parse(list.Dynamic1),
						Dynamic3 = list.Dynamic3,
						Dynamic2 = list.Dynamic2,
						Level = list.Level,
						NameOrTitle = list.NameOrTitle,
						Dynamic6 = list.Dynamic6,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats1 = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_KeyChengg_KetYanj + AttachmentsKeys.StartStage);
						var ats2 = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_KeyChengg_KetYanj + AttachmentsKeys.ProcessStage);
						var ats3 = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_KeyChengg_KetYanj + AttachmentsKeys.EndStage);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_KeyChengg_KetYanj + AttachmentsKeys.Vertify);
						var at1 = AttachmentsExtensions.GetAttachment(ats1);
						var at2 = AttachmentsExtensions.GetAttachment(ats2);
						var at3 = AttachmentsExtensions.GetAttachment(ats3);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName1 = at1.Name;
						model.AttachmentUrl1 = at1.Url;
						model.AttachmentName2 = at2.Name;
						model.AttachmentUrl2 = at2.Url;
						model.AttachmentName3 = at3.Name;
						model.AttachmentUrl3 = at3.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("ZisFaz_KeyChengg_KetYanj", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_KeyChengg_KetYanj(long? id, ZisFaz_KeyChengg_KetYanjModel model)
		{
			ThrowNotAjax();

			string attachmentTypeKey = AttachmentsKeys.ZisFaz_KeyChengg_KetYanj,
				vertifyTypeKey = AttachmentsKeys.ZisFaz_KeyChengg_KetYanj + AttachmentsKeys.Vertify,
				typeKey1 = attachmentTypeKey + AttachmentsKeys.StartStage,
				typeKey2 = attachmentTypeKey + AttachmentsKeys.ProcessStage,
				typeKey3 = attachmentTypeKey + AttachmentsKeys.EndStage;

			var atta1 = new AttachmentsDataModel
			{
				Type = typeKey1,
				Name = model.AttachmentName1,
				Url = model.AttachmentUrl1,
				UserId = UserProfile.UserId
			};

			var atta2 = new AttachmentsDataModel
			{
				Type = typeKey2,
				Name = model.AttachmentName2,
				Url = model.AttachmentUrl2,
				UserId = UserProfile.UserId
			};

			var atta3 = new AttachmentsDataModel
			{
				Type = typeKey3,
				Name = model.AttachmentName3,
				Url = model.AttachmentUrl3,
				UserId = UserProfile.UserId
			};

			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareAchievement data;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareAchievement()
					{
						TeacherId = UserProfile.UserId,
						AchievementKey = DeclareKeys.ZisFaz_KeyChengg_KetYanj,
						DateRegion = model.DateRegion,
						//Dynamic1 = model.Dynamic1.ToString(),
						Dynamic3 = model.Dynamic3.ToString(),
						Dynamic2 = model.Dynamic2,
						Level = model.Level,
						NameOrTitle = model.NameOrTitle,
						Dynamic6 = model.Dynamic6,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now,
					};

					db.DeclareAchievementDal.Insert(data);
					atta1.JoinId = data.DeclareAchievementId;
					atta2.JoinId = data.DeclareAchievementId;
					atta3.JoinId = data.DeclareAchievementId;
					vertAtta.JoinId = data.DeclareAchievementId;
				}
				else
				{
					APQuery.update(ta)
					   .set(ta.DateRegion.SetValue(model.DateRegion))
					   //.set(ta.Dynamic1.SetValue(model.Dynamic1.ToString()))
					   .set(ta.Dynamic3.SetValue(model.Dynamic3.ToString()))
					   .set(ta.Dynamic2.SetValue(model.Dynamic2))
					   .set(ta.Level.SetValue(model.Level))
					   .set(ta.NameOrTitle.SetValue(model.NameOrTitle))
					   .set(ta.Dynamic6.SetValue(model.Dynamic6))
					   //.set(ta.IsShare.SetValue(model.IsShare))
					   .set(ta.IsDeclare.SetValue(model.IsDeclare))
					   .set(ta.Modifier.SetValue(UserProfile.UserId))
					   .set(ta.ModifyDate.SetValue(DateTime.Now))
					   .where(ta.DeclareAchievementId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { typeKey1, typeKey2, typeKey3, vertifyTypeKey });
					atta1.JoinId = id.Value;
					atta2.JoinId = id.Value;
					atta3.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareAchievementDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta1, atta2, atta3, vertAtta });

				//TODO:AddOrDelShare(atta.JoinId, model.NameOrTitle, ShareKeys.AchievementShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.教育教学科研成果.在区级及以上刊物发表论文 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_KeyChengg_FabLunw
		//	POST-Ajax:			DeclarePop/ZisFaz_KeyChengg_FabLunw

		public ActionResult ZisFaz_KeyChengg_FabLunw(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new ZisFaz_KeyChengg_FabLunwModel() { Date = System.DateTime.Now, AttachmentName = "", IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareAchievementDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_KeyChengg_FabLunwModel()
					{
						DeclareAchievementId = list.DeclareAchievementId,
						Date = DateTime.Parse(list.Date),
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						Level = list.Level,
						NameOrTitle = list.NameOrTitle,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_KeyChengg_FabLunw);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_KeyChengg_FabLunw + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("ZisFaz_KeyChengg_FabLunw", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_KeyChengg_FabLunw(long? id, ZisFaz_KeyChengg_FabLunwModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.ZisFaz_KeyChengg_FabLunw;
			var vertifyTypeKey = AttachmentsKeys.ZisFaz_KeyChengg_FabLunw + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareAchievement data;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareAchievement()
					{
						TeacherId = UserProfile.UserId,
						AchievementKey = DeclareKeys.ZisFaz_KeyChengg_FabLunw,
						Date = model.Date.ToString("yyyy-MM-dd"),
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Level = model.Level,
						NameOrTitle = model.NameOrTitle,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareAchievementDal.Insert(data);
					atta.JoinId = data.DeclareAchievementId;
					vertAtta.JoinId = data.DeclareAchievementId;
				}
				else
				{
					APQuery.update(ta)
					   .set(ta.Date.SetValue(model.Date.ToString("yyyy-MM-dd")))
					   .set(ta.Dynamic1.SetValue(model.Dynamic1))
					   .set(ta.Dynamic2.SetValue(model.Dynamic2))
					   .set(ta.Level.SetValue(model.Level))
					   .set(ta.NameOrTitle.SetValue(model.NameOrTitle))
					   //.set(ta.IsShare.SetValue(model.IsShare))
					   .set(ta.IsDeclare.SetValue(model.IsDeclare))
					   .set(ta.Modifier.SetValue(UserProfile.UserId))
					   .set(ta.ModifyDate.SetValue(DateTime.Now))
					   .where(ta.DeclareAchievementId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareAchievementDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				//TODO: AddOrDelShare(atta.JoinId, model.NameOrTitle, ShareKeys.AchievementShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.教育教学科研成果.论著情况 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_KeyChengg_LunzQingk
		//	POST-Ajax:			DeclarePop/ZisFaz_KeyChengg_LunzQingk

		public ActionResult ZisFaz_KeyChengg_LunzQingk(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new ZisFaz_KeyChengg_LunzQingkModel() { Date = System.DateTime.Now, AttachmentName = "", IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareAchievementDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_KeyChengg_LunzQingkModel()
					{
						DeclareAchievementId = list.DeclareAchievementId,
						Date = DateTime.Parse(list.Date),
						Dynamic1 = list.Dynamic1,
						Dynamic2 = bool.Parse(list.Dynamic2),
						NameOrTitle = list.NameOrTitle,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_KeyChengg_LunzQingk);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_KeyChengg_LunzQingk + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}

			}

			return PartialView("ZisFaz_KeyChengg_LunzQingk", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_KeyChengg_LunzQingk(long? id, ZisFaz_KeyChengg_LunzQingkModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.ZisFaz_KeyChengg_LunzQingk;
			var vertifyTypeKey = AttachmentsKeys.ZisFaz_KeyChengg_LunzQingk + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareAchievement data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareAchievement()
					{
						TeacherId = UserProfile.UserId,
						AchievementKey = DeclareKeys.ZisFaz_KeyChengg_LunzQingk,
						Date = model.Date.ToString("yyyy-MM-dd"),
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2.ToString(),
						NameOrTitle = model.NameOrTitle,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						CreateDate = DateTime.Now,
						Creator = UserProfile.UserId
					};

					db.DeclareAchievementDal.Insert(data);
					atta.JoinId = data.DeclareAchievementId;
					vertAtta.JoinId = data.DeclareAchievementId;
				}
				else
				{
					APQuery.update(ta)
					   .set(ta.Date.SetValue(model.Date.ToString("yyyy-MM-dd")))
					   .set(ta.Dynamic1.SetValue(model.Dynamic1))
					   .set(ta.Dynamic2.SetValue(model.Dynamic2.ToString()))
					   .set(ta.NameOrTitle.SetValue(model.NameOrTitle))
					   //.set(ta.IsShare.SetValue(model.IsShare))
					   .set(ta.IsDeclare.SetValue(model.IsDeclare))
					   .set(ta.ModifyDate.SetValue(DateTime.Now))
					   .set(ta.Modifier.SetValue(UserProfile.UserId))
					   .where(ta.DeclareAchievementId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareAchievementDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				//TODO: AddOrDelShare(atta.JoinId, model.NameOrTitle, ShareKeys.AchievementShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 自身发展.市区级大活动 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_ShiqjHuod
		//	POST-Ajax:			DeclarePop/ZisFaz_ShiqjHuod

		public ActionResult ZisFaz_ShiqjHuod(long? id)
		{
			var model = new ZisFaz_ShiqjHuodModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZisFaz_ShiqjHuodModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_ShiqjHuod);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZisFaz_ShiqjHuod + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("ZisFaz_ShiqjHuod", model);
		}

		[HttpPost]

		public ActionResult ZisFaz_ShiqjHuod(long? id, ZisFaz_ShiqjHuodModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.ZisFaz_ShiqjHuod;
			var vertifyTypeKey = AttachmentsKeys.ZisFaz_ShiqjHuod + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZisFaz_ShiqjHuod,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						CreateDate = DateTime.Now,
						Creator = UserProfile.UserId
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				//TODO:AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 制度建设.有影响力的工作 ]


		//	GET-Ajax:			DeclarePop/ZhidJians_YingxlDeGongz
		//	POST-Ajax:			DeclarePop/ZhidJians_YingxlDeGongz

		public ActionResult ZhidJians_YingxlDeGongz(long? id)
		{
			var model = new ZhidJians_YingxlDeGongzModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZhidJians_YingxlDeGongzModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZhidJians_YingxlDeGongz);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZhidJians_YingxlDeGongz + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}

			}

			return PartialView("ZhidJians_YingxlDeGongz", model);
		}

		[HttpPost]

		public ActionResult ZhidJians_YingxlDeGongz(long? id, ZhidJians_YingxlDeGongzModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.ZhidJians_YingxlDeGongz;
			var vertifyTypeKey = AttachmentsKeys.ZhidJians_YingxlDeGongz + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZhidJians_YingxlDeGongz,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				//TODO:AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 制度建设.特色活动开展 ]


		//	GET-Ajax:			DeclarePop/ZhidJians_TesHuodKaiz
		//	POST-Ajax:			DeclarePop/ZhidJians_TesHuodKaiz

		public ActionResult ZhidJians_TesHuodKaiz(long? id)
		{
			var model = new ZhidJians_TesHuodKaizModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new ZhidJians_TesHuodKaizModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						Level = list.Level,
						Dynamic9 = list.Dynamic9,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.ZhidJians_TesHuodKaiz);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.ZhidJians_TesHuodKaiz + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}

			}

			return PartialView("ZhidJians_TesHuodKaiz", model);
		}

		[HttpPost]

		public ActionResult ZhidJians_TesHuodKaiz(long? id, ZhidJians_TesHuodKaizModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.ZhidJians_TesHuodKaiz;
			var vertifyTypeKey = AttachmentsKeys.ZhidJians_TesHuodKaiz + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.ZhidJians_TesHuodKaiz,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Level = model.Level,
						Dynamic9 = model.Dynamic9,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Level.SetValue(model.Level))
					   .set(t.Dynamic9.SetValue(model.Dynamic9))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.ZisFaz_JiaoxHuod_Yantk);
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				//TODO:AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 制度建设.档案建设 ]


		//	GET-Ajax:			DeclarePop/ZhidJians_DangaJians
		//	POST-Ajax:			DeclarePop/ZhidJians_DangaJians

		public ActionResult ZhidJians_DangaJians(long? id)
		{
			if (id == null)
				return PartialView("ZhidJians_DangaJians");

			var model = db.DeclareOrgConstDal.PrimaryGet(id.Value);
			return PartialView("ZhidJians_DangaJians", model);
		}

		[HttpPost]

		public ActionResult ZhidJians_DangaJians(long? id, DeclareOrgConst model)
		{
			ThrowNotAjax();

			DeclareOrgConst data = null;

			db.BeginTrans();

			try
			{

				if (id == null)
				{

					data = new DeclareOrgConst()
					{
						TeacherId = UserProfile.UserId,
						Content = model.Content,
						Remark = model.Remark,
						Work = model.Work,
						CreateDate = DateTime.Now,
						Creator = UserProfile.UserId,
						IsDeclare = model.IsDeclare
					};

					db.DeclareOrgConstDal.Insert(data);
				}
				else
				{
					var t = APDBDef.DeclareOrgConst;

					APQuery.update(t)
					   .set(t.Content.SetValue(model.Content))
					   .set(t.Work.SetValue(model.Work))
					   .set(t.Remark.SetValue(model.Remark))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .where(t.DeclareOrgConstId == id.Value)
					   .execute(db);

					data = db.DeclareOrgConstDal.PrimaryGet(id.Value);
				}

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db);

				db.Commit();
			}
			catch
			{
				db.Rollback();
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 区内流动.学科带头人.课堂教学.上公开课 ]


		//	GET-Ajax:			DeclarePop/QunLiud_XuekDaitr_KetJiaox_Gongkk
		//	POST-Ajax:			DeclarePop/QunLiud_XuekDaitr_KetJiaox_Gongkk

		public ActionResult QunLiud_XuekDaitr_KetJiaox_Gongkk(long? id)
		{
			var model = new QunLiud_XuekDaitr_KetJiaox_GongkkModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new QunLiud_XuekDaitr_KetJiaox_GongkkModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_Gongkk));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}

			}

			return PartialView("QunLiud_XuekDaitr_KetJiaox_Gongkk", model);
		}

		[HttpPost]
		public ActionResult QunLiud_XuekDaitr_KetJiaox_Gongkk(long? id, QunLiud_XuekDaitr_KetJiaox_GongkkModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_Gongkk,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{
				if (id == null)
				{
					var data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_Gongkk,
						Date = model.Date,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_Gongkk);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 区内流动.学科带头人.课堂教学.公开汇报课 ]


		//	GET-Ajax:			DeclarePop/QunLiud_XuekDaitr_KetJiaox_GongkHuibk
		//	POST-Ajax:			DeclarePop/QunLiud_XuekDaitr_KetJiaox_GongkHuibk

		public ActionResult QunLiud_XuekDaitr_KetJiaox_GongkHuibk(long? id)
		{
			var model = new QunLiud_XuekDaitr_KetJiaox_GongkHuibkModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new QunLiud_XuekDaitr_KetJiaox_GongkHuibkModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_GongkHuibk));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}

			}

			return PartialView("QunLiud_XuekDaitr_KetJiaox_GongkHuibk", model);
		}

		[HttpPost]
		public ActionResult QunLiud_XuekDaitr_KetJiaox_GongkHuibk(long? id, QunLiud_XuekDaitr_KetJiaox_GongkHuibkModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_GongkHuibk,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{
				if (id == null)
				{
					var data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_GongkHuibk,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_GongkHuibk);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 区内流动.学科带头人.课堂教学.接受教师听随堂课 ]


		//	GET-Ajax:			DeclarePop/QunLiud_XuekDaitr_KetJiaox_Suitk
		//	POST-Ajax:			DeclarePop/QunLiud_XuekDaitr_KetJiaox_Suitk

		public ActionResult QunLiud_XuekDaitr_KetJiaox_Suitk(long? id)
		{
			var model = new QunLiud_XuekDaitr_KetJiaox_SuitkModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new QunLiud_XuekDaitr_KetJiaox_SuitkModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_Suitk));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}
			}

			return PartialView("QunLiud_XuekDaitr_KetJiaox_Suitk", model);
		}

		[HttpPost]
		public ActionResult QunLiud_XuekDaitr_KetJiaox_Suitk(long? id, QunLiud_XuekDaitr_KetJiaox_SuitkModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_Suitk,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{
				if (id == null)
				{
					var data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_Suitk,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						CreateDate = DateTime.Now,
						Creator = UserProfile.UserId
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_Suitk);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 区内流动.学科带头人.课堂教学.开展听课指导 ]


		//	GET-Ajax:			DeclarePop/QunLiud_XuekDaitr_KetJiaox_TingKZhid
		//	POST-Ajax:			DeclarePop/QunLiud_XuekDaitr_KetJiaox_TingKZhid

		public ActionResult QunLiud_XuekDaitr_KetJiaox_TingKZhid(long? id)
		{
			var model = new QunLiud_XuekDaitr_KetJiaox_TingKZhidModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new QunLiud_XuekDaitr_KetJiaox_TingKZhidModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_TingKZhid));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}

			}

			return PartialView("QunLiud_XuekDaitr_KetJiaox_TingKZhid", model);
		}

		[HttpPost]
		public ActionResult QunLiud_XuekDaitr_KetJiaox_TingKZhid(long? id, QunLiud_XuekDaitr_KetJiaox_TingKZhidModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_TingKZhid,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{
				if (id == null)
				{
					var data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.QunLiud_XuekDaitr_KetJiaox_TingKZhid,
						Date = model.Date,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_KetJiaox_TingKZhid);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 区内流动.学科带头人.教育科研.开设专题讲座 ]


		//	GET-Ajax:			DeclarePop/QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz
		//	POST-Ajax:			DeclarePop/QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz

		public ActionResult QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz(long? id)
		{
			var model = new QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangzModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangzModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}

			}

			return PartialView("QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz", model);
		}

		[HttpPost]
		public ActionResult QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz(long? id, QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangzModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{
				if (id == null)
				{
					var data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_JiaoyKey_ZhuantJiangz);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 区内流动.学科带头人.教育科研.主持教研活动 ]


		//	GET-Ajax:			DeclarePop/QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod
		//	POST-Ajax:			DeclarePop/QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod

		public ActionResult QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod(long? id)
		{
			var model = new QunLiud_XuekDaitr_JiaoyKey_JiaoyHuodModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new QunLiud_XuekDaitr_JiaoyKey_JiaoyHuodModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}

			}

			return PartialView("QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod", model);
		}

		[HttpPost]
		public ActionResult QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod(long? id, QunLiud_XuekDaitr_JiaoyKey_JiaoyHuodModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{
				if (id == null)
				{
					var data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_JiaoyKey_JiaoyHuod);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 区内流动.学科带头人.教育科研.参与教研组、备课组活动 ]


		//	GET-Ajax:			DeclarePop/QunLiud_XuekDaitr_JiaoyKey_CanyHuod
		//	POST-Ajax:			DeclarePop/QunLiud_XuekDaitr_JiaoyKey_CanyHuod

		public ActionResult QunLiud_XuekDaitr_JiaoyKey_CanyHuod(long? id)
		{
			var model = new QunLiud_XuekDaitr_JiaoyKey_CanyHuodModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new QunLiud_XuekDaitr_JiaoyKey_CanyHuodModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_JiaoyKey_CanyHuod));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}

			}

			return PartialView("QunLiud_XuekDaitr_JiaoyKey_CanyHuod", model);
		}

		[HttpPost]
		public ActionResult QunLiud_XuekDaitr_JiaoyKey_CanyHuod(long? id, QunLiud_XuekDaitr_JiaoyKey_CanyHuodModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.QunLiud_XuekDaitr_JiaoyKey_CanyHuod,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{
				if (id == null)
				{
					var data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.QunLiud_XuekDaitr_JiaoyKey_CanyHuod,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_JiaoyKey_CanyHuod);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 区内流动.学科带头人.带教培训.带教指导记录 ]


		//	GET-Ajax:			DeclarePop/QunLiud_XuekDaitr_DaijPeix_DaijZhidJil
		//	POST-Ajax:			DeclarePop/QunLiud_XuekDaitr_DaijPeix_DaijZhidJil

		public ActionResult QunLiud_XuekDaitr_DaijPeix_DaijZhidJil(long? id)
		{
			var model = new QunLiud_XuekDaitr_DaijPeix_DaijZhidJilModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new QunLiud_XuekDaitr_DaijPeix_DaijZhidJilModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_DaijPeix_DaijZhidJil));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}

			}

			return PartialView("QunLiud_XuekDaitr_DaijPeix_DaijZhidJil", model);
		}

		[HttpPost]
		public ActionResult QunLiud_XuekDaitr_DaijPeix_DaijZhidJil(long? id, QunLiud_XuekDaitr_DaijPeix_DaijZhidJilModel model)
		{
			ThrowNotAjax();

			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.QunLiud_XuekDaitr_DaijPeix_DaijZhidJil,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{
				if (id == null)
				{
					var data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.QunLiud_XuekDaitr_DaijPeix_DaijZhidJil,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						CreateDate = DateTime.Now,
						Creator = UserProfile.UserId
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.QunLiud_XuekDaitr_DaijPeix_DaijZhidJil);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 区内流动.骨干教师.开设公开课 ]


		//	GET-Ajax:			DeclarePop/QunLiud_GugJiaos_Gongkk
		//	POST-Ajax:			DeclarePop/QunLiud_GugJiaos_Gongkk

		public ActionResult QunLiud_GugJiaos_Gongkk(long? id)
		{
			var model = new QunLiud_GugJiaos_GongkkModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new QunLiud_GugJiaos_GongkkModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.QunLiud_GugJiaos_Gongkk));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}

			}

			return PartialView("QunLiud_GugJiaos_Gongkk", model);
		}

		[HttpPost]
		public ActionResult QunLiud_GugJiaos_Gongkk(long? id, QunLiud_GugJiaos_GongkkModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.QunLiud_GugJiaos_Gongkk,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};

			try
			{
				if (id == null)
				{
					var data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.QunLiud_GugJiaos_Gongkk,
						Date = model.Date,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.QunLiud_GugJiaos_Gongkk);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 区内流动.骨干教师.开展听课指导 ]


		//	GET-Ajax:			DeclarePop/QunLiud_GugJiaos_TingkZhid
		//	POST-Ajax:			DeclarePop/QunLiud_GugJiaos_TingkZhid

		public ActionResult QunLiud_GugJiaos_TingkZhid(long? id)
		{
			var model = new QunLiud_GugJiaos_TingkZhidModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new QunLiud_GugJiaos_TingkZhidModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.QunLiud_GugJiaos_TingkZhid));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}
			}

			return PartialView("QunLiud_GugJiaos_TingkZhid", model);
		}

		[HttpPost]
		public ActionResult QunLiud_GugJiaos_TingkZhid(long? id, QunLiud_GugJiaos_TingkZhidModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.QunLiud_GugJiaos_TingkZhid,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{
				if (id == null)
				{
					var data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.QunLiud_GugJiaos_TingkZhid,
						Date = model.Date,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						CreateDate = DateTime.Now,
						Creator = UserProfile.UserId
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.QunLiud_GugJiaos_TingkZhid);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 区内流动.骨干教师.主持备课组活动 ]


		//	GET-Ajax:			DeclarePop/QunLiud_GugJiaos_BeikzHuod
		//	POST-Ajax:			DeclarePop/QunLiud_GugJiaos_BeikzHuod

		public ActionResult QunLiud_GugJiaos_BeikzHuod(long? id)
		{
			var model = new QunLiud_GugJiaos_BeikzHuodModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new QunLiud_GugJiaos_BeikzHuodModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.QunLiud_GugJiaos_BeikzHuod));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}
			}

			return PartialView("QunLiud_GugJiaos_BeikzHuod", model);
		}

		[HttpPost]
		public ActionResult QunLiud_GugJiaos_BeikzHuod(long? id, QunLiud_GugJiaos_BeikzHuodModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.QunLiud_GugJiaos_BeikzHuod,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{
				if (id == null)
				{
					var data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.QunLiud_GugJiaos_BeikzHuod,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.QunLiud_GugJiaos_BeikzHuod);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 配合教研员工作.学科教研 ]


		//	GET-Ajax:			DeclarePop/PeihJiaoyyGongz_XuekJiaoy
		//	POST-Ajax:			DeclarePop/PeihJiaoyyGongz_XuekJiaoy

		public ActionResult PeihJiaoyyGongz_XuekJiaoy(long? id)
		{
			var model = new PeihJiaoyyGongz_XuekJiaoyModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new PeihJiaoyyGongz_XuekJiaoyModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						IsDeclare = list.IsDeclare
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.PeihJiaoyyGongz_XuekJiaoy));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}
			}

			return PartialView("PeihJiaoyyGongz_XuekJiaoy", model);
		}

		[HttpPost]

		public ActionResult PeihJiaoyyGongz_XuekJiaoy(long? id, PeihJiaoyyGongz_XuekJiaoyModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.PeihJiaoyyGongz_XuekJiaoy,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.PeihJiaoyyGongz_XuekJiaoy,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now,
						IsDeclare = model.IsDeclare
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.PeihJiaoyyGongz_XuekJiaoy);
					atta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAtta(db, atta);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 配合教研员工作.学科命题 ]


		//	GET-Ajax:			DeclarePop/PeihJiaoyyGongz_XuekMingt
		//	POST-Ajax:			DeclarePop/PeihJiaoyyGongz_XuekMingt

		public ActionResult PeihJiaoyyGongz_XuekMingt(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new PeihJiaoyyGongz_XuekMingtModel() { Date = System.DateTime.Now, AttachmentName = "", IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new PeihJiaoyyGongz_XuekMingtModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						Dynamic3 = list.Dynamic3,
						Dynamic4 = list.Dynamic4,
						Level = list.Level,
						IsDeclare = model.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.PeihJiaoyyGongz_XuekMingt);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.PeihJiaoyyGongz_XuekMingt + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("PeihJiaoyyGongz_XuekMingt", model);
		}

		[HttpPost]

		public ActionResult PeihJiaoyyGongz_XuekMingt(long? id, PeihJiaoyyGongz_XuekMingtModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.PeihJiaoyyGongz_XuekMingt;
			var vertifyTypeKey = AttachmentsKeys.PeihJiaoyyGongz_XuekMingt + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel()
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.PeihJiaoyyGongz_XuekMingt,
						Date = model.Date,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Dynamic3 = model.Dynamic3,
						Dynamic4 = model.Dynamic4,
						Level = model.Level,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now,
						IsDeclare = model.IsDeclare
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Level.SetValue(model.Level))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Dynamic3.SetValue(model.Dynamic3))
					   .set(t.Dynamic4.SetValue(model.Dynamic4))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });
					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 配合教研员工作.基层学校调研 ]


		//	GET-Ajax:			DeclarePop/PeihJiaoyyGongz_JicXuexTiaoy
		//	POST-Ajax:			DeclarePop/PeihJiaoyyGongz_JicXuexTiaoy

		public ActionResult PeihJiaoyyGongz_JicXuexTiaoy(long? id)
		{
			var model = new PeihJiaoyyGongz_JicXuexTiaoyModel() { Date = System.DateTime.Now, AttachmentName = "" };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new PeihJiaoyyGongz_JicXuexTiaoyModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Dynamic1 = list.Dynamic1,
						IsDeclare = list.IsDeclare
					};

					var atta = AttachmentsExtensions.GetAttachment(
					   AttachmentsExtensions.GetAttachmentList(db, id.Value, AttachmentsKeys.PeihJiaoyyGongz_JicXuexTiaoy));
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}
			}

			return PartialView("PeihJiaoyyGongz_JicXuexTiaoy", model);
		}

		[HttpPost]

		public ActionResult PeihJiaoyyGongz_JicXuexTiaoy(long? id, PeihJiaoyyGongz_JicXuexTiaoyModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.PeihJiaoyyGongz_JicXuexTiaoy,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.PeihJiaoyyGongz_JicXuexTiaoy,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now,
						IsDeclare = model.IsDeclare
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.PeihJiaoyyGongz_JicXuexTiaoy);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);

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
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 教育教学.教研活动.发挥作用 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_JiaoxHuod_JiaoxGongkk
		//	POST-Ajax:			DeclarePop/ZisFaz_JiaoxHuod_JiaoxGongkk

		public ActionResult Shenb_JiaoyHuod_FahZuoy(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new JiaoyJiaox_JiaoyHuod_FahZuoyModel() { Date = System.DateTime.Now, AttachmentName = "", IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new JiaoyJiaox_JiaoyHuod_FahZuoyModel()
					{
						DeclareActiveId = list.DeclareActiveId,
						Date = list.Date,
						Location = list.Location,
						ContentValue = list.ContentValue,
						Level = list.Level,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.Shenb_JiaoyHuod_FahZuoy);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.Shenb_JiaoyHuod_FahZuoy + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("JiaoyJiaox_JiaoyHuod_FahZuoy", model);
		}

		[HttpPost]

		public ActionResult Shenb_JiaoyHuod_FahZuoy(long? id, JiaoyJiaox_JiaoyHuod_FahZuoyModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.Shenb_JiaoyHuod_FahZuoy;
			var vertifyTypeKey = AttachmentsKeys.Shenb_JiaoyHuod_FahZuoy + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						ActiveKey = DeclareKeys.JiaoyJiaox_JiaoyHuod_FahZuoy,
						Date = model.Date,
						Location = model.Location,
						ContentValue = model.ContentValue,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Level = model.Level,
						IsShare = model.IsShare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now,
						IsDeclare = model.IsDeclare
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Location.SetValue(model.Location))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.Level.SetValue(model.Level))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });

					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				// AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}

		#endregion


		#region [ 个人特色.其他身份 ]


		//	GET-Ajax:			DeclarePop/ZisFaz_JiaoxHuod_JiaoxGongkk
		//	POST-Ajax:			DeclarePop/ZisFaz_JiaoxHuod_JiaoxGongkk

		public ActionResult GerTes_QitShenf(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new GerTes_QitShenfModel() { AttachmentName = "", IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareAchievementDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new GerTes_QitShenfModel()
					{
						DeclareAchievementId = list.DeclareAchievementId,
						NameOrTitle = list.NameOrTitle,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						Dynamic3 = list.Dynamic3,
						IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.GerTes_QitShenf);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.GerTes_QitShenf + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("GerTes_QitShenf", model);
		}

		[HttpPost]

		public ActionResult GerTes_QitShenf(long? id, GerTes_QitShenfModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.GerTes_QitShenf;
			var vertifyTypeKey = AttachmentsKeys.GerTes_QitShenf + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareAchievement data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareAchievement()
					{
						TeacherId = UserProfile.UserId,
						AchievementKey = DeclareKeys.GerTes_QitShenf,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						Dynamic3 = model.Dynamic3,
						NameOrTitle = model.NameOrTitle,
						IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareAchievementDal.Insert(data);
					atta.JoinId = data.DeclareAchievementId;
					vertAtta.JoinId = data.DeclareAchievementId;
				}
				else
				{
					APQuery.update(ta)
					   .set(ta.Dynamic1.SetValue(model.Dynamic1))
					   .set(ta.Dynamic2.SetValue(model.Dynamic2))
					   .set(ta.Dynamic3.SetValue(model.Dynamic3))
					   .set(ta.NameOrTitle.SetValue(model.NameOrTitle))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(ta.IsDeclare.SetValue(model.IsDeclare))
					   .set(ta.Modifier.SetValue(UserProfile.UserId))
					   .set(ta.ModifyDate.SetValue(DateTime.Now))
					   .where(ta.DeclareAchievementId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });

					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareAchievementDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				// AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}

		#endregion


		#region [ 个人特色 学员成果 ]

		public ActionResult GerTes_XueyChengz(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new GerTes_XueyChengzModel() { AttachmentName = "", IsDeclare = isInDeclare };

			if (id != null)
			{
				var list = db.DeclareAchievementDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new GerTes_XueyChengzModel()
					{
						DeclareAchievementId = list.DeclareAchievementId,
						NameOrTitle = list.NameOrTitle,
						Date = list.Date,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						//IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.GerTes_XueyChengz);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.GerTes_XueyChengz + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("GerTes_XueyChengz", model);
		}

		[HttpPost]

		public ActionResult GerTes_XueyChengz(long? id, GerTes_XueyChengzModel model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.GerTes_XueyChengz;
			var vertifyTypeKey = AttachmentsKeys.GerTes_XueyChengz + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareAchievement data = null;

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					data = new DeclareAchievement()
					{
						TeacherId = UserProfile.UserId,
						Date = model.Date,
						AchievementKey = DeclareKeys.GerTes_XueyChengz,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						NameOrTitle = model.NameOrTitle,
						//IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareAchievementDal.Insert(data);
					atta.JoinId = data.DeclareAchievementId;
					vertAtta.JoinId = data.DeclareAchievementId;
				}
				else
				{
					APQuery.update(ta)
					   .set(ta.Dynamic1.SetValue(model.Dynamic1))
					   .set(ta.Dynamic2.SetValue(model.Dynamic2))
					   .set(ta.NameOrTitle.SetValue(model.NameOrTitle))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(ta.IsDeclare.SetValue(model.IsDeclare))
					   .set(ta.Modifier.SetValue(UserProfile.UserId))
					   .set(ta.ModifyDate.SetValue(DateTime.Now))
					   .where(ta.DeclareAchievementId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });

					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareAchievementDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				// AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

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
				msg = "信息已保存！"
			});
		}

		#endregion


		#region [ 其他.基本功展示获奖 ]

		public ActionResult Qit_JibGongZshiHuoj(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new Qit_JibGongZshiHuoj() { AttachmentName = "", IsDeclare = isInDeclare, Date = DateTime.Now };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new Qit_JibGongZshiHuoj()
					{
						DeclareActiveId = list.DeclareActiveId,
						ContentValue = list.ContentValue,
						Date = list.Date,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						//IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.Qit_JibGongZshiHuoj);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.Qit_JibGongZshiHuoj + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("Qit_JibGongZshiHuoj", model);
		}

		[HttpPost]

		public ActionResult Qit_JibGongZshiHuoj(long? id, Qit_JibGongZshiHuoj model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.Qit_JibGongZshiHuoj;
			var vertifyTypeKey = AttachmentsKeys.Qit_JibGongZshiHuoj + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						Date = model.Date,
						ActiveKey = DeclareKeys.Qit_JibGongZshiHuoj,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						ContentValue = model.ContentValue,
						//IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   //.set(t.IsShare.SetValue(model.IsShare))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });

					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				// AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

				db.Commit();
			}
			catch
			{
				db.Rollback();
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已保存！"
			});

		}

		#endregion


		#region [ 其他 综合性荣誉 ]

		public ActionResult Qit_ZonghxingRongy(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new Qit_ZonghxingRongy() { AttachmentName = "", IsDeclare = isInDeclare, Date = DateTime.Now };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new Qit_ZonghxingRongy()
					{
						DeclareActiveId = list.DeclareActiveId,
						ContentValue = list.ContentValue,
						Date = list.Date,
						Level = list.Level,
						Dynamic1 = list.Dynamic1,
						//IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.Qit_ZonghxingRongy);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.Qit_ZonghxingRongy + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("Qit_ZonghxingRongy", model);
		}

		[HttpPost]
		public ActionResult Qit_ZonghxingRongy(long? id, Qit_ZonghxingRongy model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.Qit_ZonghxingRongy;
			var vertifyTypeKey = AttachmentsKeys.Qit_ZonghxingRongy + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						Date = model.Date,
						ActiveKey = DeclareKeys.Qit_ZonghxingRongy,
						Level = model.Level,
						Dynamic1 = model.Dynamic1,
						ContentValue = model.ContentValue,
						//IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Level.SetValue(model.Level))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });

					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				// AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

				db.Commit();
			}
			catch
			{
				db.Rollback();
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已保存！"
			});
		}

		#endregion


		#region [其他.见习教师评选]


		public ActionResult Qit_JianxJiaosPingx(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new Qit_JianxJiaosPingx() { AttachmentName = "", IsDeclare = isInDeclare, Date = DateTime.Now };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new Qit_JianxJiaosPingx()
					{
						DeclareActiveId = list.DeclareActiveId,
						ContentValue = list.ContentValue,
						Date = list.Date,
						Level = list.Level,
						Dynamic1 = list.Dynamic1,
						//IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.Qit_JianxJiaosPingx);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.Qit_JianxJiaosPingx + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("Qit_JianxJiaosPingx", model);
		}

		[HttpPost]
		public ActionResult Qit_JianxJiaosPingx(long? id, Qit_JianxJiaosPingx model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.Qit_JianxJiaosPingx;
			var vertifyTypeKey = AttachmentsKeys.Qit_JianxJiaosPingx + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						Date = model.Date,
						ActiveKey = DeclareKeys.Qit_JianxJiaosPingx,
						Level = model.Level,
						Dynamic1 = model.Dynamic1,
						ContentValue = model.ContentValue,
						//IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Level.SetValue(model.Level))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });

					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				// AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

				db.Commit();
			}
			catch
			{
				db.Rollback();
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [其他.见习教师大奖赛获奖]

		public ActionResult Qit_JianxJiaosDasHuoj(long? id, long? declareTargetId)
		{
			var isInDeclare = Period != null && Period.IsInDeclarePeriod;
			var model = new Qit_JianxJiaosDasHuoj() { AttachmentName = "", IsDeclare = isInDeclare, Date = DateTime.Now };

			if (id != null)
			{
				var list = db.DeclareActiveDal.PrimaryGet(id.Value);

				if (list != null)
				{
					model = new Qit_JianxJiaosDasHuoj()
					{
						DeclareActiveId = list.DeclareActiveId,
						ContentValue = list.ContentValue,
						Date = list.Date,
						Level = list.Level,
						Dynamic1 = list.Dynamic1,
						Dynamic2 = list.Dynamic2,
						//IsShare = list.IsShare,
						IsDeclare = list.IsDeclare,
						DeclareTargetId = declareTargetId ?? 0
					};

					var allAts = AttachmentsExtensions.GetAttachmentList(db, id.Value);
					if (allAts.Count > 0)
					{
						var ats = allAts.FindAll(a => a.Type == AttachmentsKeys.Qit_JianxJiaosDasHuoj);
						var vts = allAts.FindAll(a => a.Type == AttachmentsKeys.Qit_JianxJiaosDasHuoj + AttachmentsKeys.Vertify);
						var at = AttachmentsExtensions.GetAttachment(ats);
						var vt = AttachmentsExtensions.GetAttachment(vts);
						model.AttachmentName = at.Name;
						model.AttachmentUrl = at.Url;
						model.VertificationName = vt.Name;
						model.VertificationUrl = vt.Url;
					}
				}
			}

			return PartialView("Qit_JianxJiaosDasHuoj", model);
		}

		[HttpPost]
		public ActionResult Qit_JianxJiaosDasHuoj(long? id, Qit_JianxJiaosDasHuoj model)
		{
			ThrowNotAjax();

			var attachmentTypeKey = AttachmentsKeys.Qit_JianxJiaosDasHuoj;
			var vertifyTypeKey = AttachmentsKeys.Qit_JianxJiaosDasHuoj + AttachmentsKeys.Vertify;
			var atta = new AttachmentsDataModel
			{
				Type = attachmentTypeKey,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};
			var vertAtta = new AttachmentsDataModel
			{
				Type = vertifyTypeKey,
				Name = model.VertificationName,
				Url = model.VertificationUrl,
				UserId = UserProfile.UserId
			};

			DeclareActive data = null;

			try
			{
				if (id == null)
				{
					data = new DeclareActive()
					{
						TeacherId = UserProfile.UserId,
						Date = model.Date,
						ActiveKey = DeclareKeys.Qit_JianxJiaosDasHuoj,
						Level = model.Level,
						Dynamic1 = model.Dynamic1,
						Dynamic2 = model.Dynamic2,
						ContentValue = model.ContentValue,
						//IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.DeclareActiveDal.Insert(data);
					atta.JoinId = data.DeclareActiveId;
					vertAtta.JoinId = data.DeclareActiveId;
				}
				else
				{
					APQuery.update(t)
					   .set(t.Date.SetValue(model.Date))
					   .set(t.Dynamic1.SetValue(model.Dynamic1))
					   .set(t.Level.SetValue(model.Level))
					   .set(t.Dynamic2.SetValue(model.Dynamic2))
					   .set(t.ContentValue.SetValue(model.ContentValue))
					   .set(t.IsDeclare.SetValue(model.IsDeclare))
					   .set(t.Modifier.SetValue(UserProfile.UserId))
					   .set(t.ModifyDate.SetValue(DateTime.Now))
					   .where(t.DeclareActiveId == id.Value)
					   .execute(db);

					AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] { attachmentTypeKey, vertifyTypeKey });

					atta.JoinId = id.Value;
					vertAtta.JoinId = id.Value;

					data = db.DeclareActiveDal.PrimaryGet(id.Value);
				}

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta, vertAtta });

				// AddOrDelShare(atta.JoinId, model.ContentValue, ShareKeys.ActiveShare, model.IsShare);

				DeclareMaterialHelper.AddDeclareMaterial(data, Period, db, model.DeclareTargetId);

				db.Commit();
			}
			catch
			{
				db.Rollback();
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已保存！"
			});
		}

		#endregion


		#region [ 查看附件 ]

		public ActionResult AttachmentsView(long id, string type)
		{
			var list = AttachmentsExtensions.GetAttachmentList(db, id, type);

			return PartialView(list);
		}

		#endregion


		#region [Helper]


		private void AddOrDelShare(long itemId, string content, string parentType, bool isShare)
		{
			//var hasAttachment = AttachmentsExtensions.HasAttachment(db, itemId, UserProfile.UserId);
			//if (!hasAttachment)
			//{
			//   return false;
			//}

			var s = APDBDef.Share;

			var type = string.Empty;

			if (parentType == ShareKeys.ActiveShare)
				type = db.DeclareActiveDal.PrimaryGet(itemId).ActiveKey;
			else if (parentType == ShareKeys.AchievementShare)
				type = db.DeclareAchievementDal.PrimaryGet(itemId).AchievementKey;

			db.ShareDal.ConditionDelete(s.ItemId == itemId);

			if (isShare)
				db.ShareDal.Insert(new Share
				{
					ItemId = itemId,
					ParentType = parentType,
					CreateDate = DateTime.Now,
					PubishDate = DateTime.Now,
					Title = content,
					Type = type,
					UserId = UserProfile.UserId
				});

			// return true;
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