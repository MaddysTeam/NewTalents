using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheSite.Models;

namespace Business
{
	public static class AttachmentsExtensions
	{

		public static APDBDef.AttachmentsTableDef at = APDBDef.Attachments;


      public static List<Attachments> GetAttachmentList(APDBDef db, long joinId)
         => db.AttachmentsDal.ConditionQuery(at.JoinId == joinId, null, null, null);


      public static List<Attachments> GetAttachmentList(APDBDef db, long joinId, string type)
			=> db.AttachmentsDal.ConditionQuery(at.Type == type & at.JoinId == joinId, null, null, null);


		public static List<Attachments> GetAttachmentList(APDBDef db, long joinId, long userId, string type)
			=> db.AttachmentsDal.ConditionQuery(at.Type == type & at.JoinId == joinId & at.UserId == userId, null, null, null);

		public static AttachmentsViewModel GetAttachment(List<Attachments> list)
		{
			var model = new AttachmentsViewModel();

			if (list != null)
			{
				var tempName = "";
				var tempUrl = "";
				foreach (var item in list)
				{
					tempName += item.AttachmentName + "|";
					tempUrl += item.AttachmentUrl + "|";
				}

				model.Name = tempName.Length > 0 ? tempName.Substring(0, tempName.LastIndexOf('|')) : tempName;
				model.Url = tempUrl.Length > 0 ? tempUrl.Substring(0, tempUrl.LastIndexOf('|')) : tempUrl;
			}


			return model;
		}


      public static bool HasAttachment(APDBDef db, long joinId, long userId)
         => db.AttachmentsDal.ConditionQueryCount(at.JoinId==joinId & at.UserId==userId) > 0;


		public static void InsertAtta(APDBDef db, AttachmentsDataModel model)
		{
			var nameArray = model.Name.Split('|');
			var urlArray = model.Url.Split('|');

			for (int i = 0; i < nameArray.Length; i++)
			{
				var data = new Attachments()
				{
					AttachmentName = nameArray[i],
					AttachmentUrl = urlArray[i],
					JoinId = model.JoinId,
					Type = model.Type,
					UploadDate = DateTime.Now,
					UserId = model.UserId
				};
				db.AttachmentsDal.Insert(data);
			}
		}

      public static void InsertAttas(APDBDef db, AttachmentsDataModel[] models)
      {
         foreach(var item in models)
         {
            InsertAtta(db, item);
         }
      }



      public static void DeleteAtta(APDBDef db, long joinId, string type)
			=> db.AttachmentsDal.ConditionDelete(at.Type == type & at.JoinId == joinId);

		public static void DeleteAtta(APDBDef db, long joinId, long userId, string type)
			=> db.AttachmentsDal.ConditionDelete(at.Type == type & at.JoinId == joinId & at.UserId == userId);

      public static void DeleteAttas(APDBDef db, long joinId, string[] types)
         => db.AttachmentsDal.ConditionDelete(at.Type.In(types) & at.JoinId == joinId);

   }
}
