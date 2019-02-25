using Business;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Linq;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class TeamPopController : BaseController
	{

		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.TeamActiveResultTableDef tar = APDBDef.TeamActiveResult;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.TeamActiveTableDef ta = APDBDef.TeamActive;
		static APDBDef.TeamContentTableDef tc = APDBDef.TeamContent;
		static APDBDef.AttachmentsTableDef at = APDBDef.Attachments;
      private static APDBDef.DeclareMaterialTableDef dm = APDBDef.DeclareMaterial;

      #region [ 学员的成长方向 ]

      public ActionResult DaijHuod_Advice(long MemberId)
		{
			var model = new TeamMemberDataModel()
			{
				MemberId = MemberId,
				TeamId = UserProfile.UserId
			};

			var data = db.TeamMemberDal.PrimaryGet(model.TeamId, model.MemberId);

			if (data != null)
			{
				model.ContentValue = data.ContentValue;

				var atta = AttachmentsExtensions.GetAttachment(
					AttachmentsExtensions.GetAttachmentList(db, MemberId, UserProfile.UserId, AttachmentsKeys.XueyChengzJihFenx));
				model.AttachmentName = atta.Name;
				model.AttachmentUrl = atta.Url;
			}

			return PartialView("DaijHuod_Advice", model);
		}

		[HttpPost]
		public ActionResult DaijHuod_Advice(TeamMemberDataModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.XueyChengzJihFenx,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId,
				JoinId = model.MemberId
			};

			var t = APDBDef.TeamMember;


			db.BeginTrans();

			try
			{
				APQuery.update(t)
					.set(t.ContentValue.SetValue(model.ContentValue))
					.where(t.TeamId == model.TeamId & t.MemberId == model.MemberId)
					.execute(db);

				AttachmentsExtensions.DeleteAtta(db, model.MemberId, UserProfile.UserId, AttachmentsKeys.XueyChengzJihFenx);
				if (model.AttachmentName != "" && model.AttachmentName.Length > 0)
				{
					AttachmentsExtensions.InsertAtta(db, atta);
				}
				
				db.Commit();


            //记录日志
            var doSomthing = "修改";
            if (!string.IsNullOrEmpty(atta.Name))
               doSomthing += string.Format(" 并且上传了附件:{0}", atta.Name);

            Log(AttachmentsKeys.XueyChengzJihFenx, doSomthing);
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
				msg = "信息已保存!"
			});
		}


		#endregion


		#region [ 学员成果 ]

		//	查看学员成果
		//	GET: TeamPop/MemberResult

		public ActionResult MemberResult(long id)
		{
			var model = APQuery.select(tar.ResultId, tar.MemberId, tar.ActiveResult, u.RealName, ta.TeamId, 
												at.AttachmentName, at.AttachmentUrl)
				.from(tar, 
						u.JoinInner(tar.MemberId == u.UserId),
						ta.JoinInner(tar.ActiveId == ta.TeamActiveId),
						at.JoinLeft(tar.ResultId == at.JoinId & at.Type == AttachmentsKeys.DaijHuod_XueyChengg))
				.where(tar.ActiveId == id)
				.query(db, rd =>
				{
					MemberResultViewModel data = new MemberResultViewModel();
					tar.Fullup(rd, data, false);
					data.MemberName = u.RealName.GetValue(rd);
					data.TeamId = ta.TeamId.GetValue(rd);
					data.AttachmentName = at.AttachmentName.GetValue(rd);
					data.AttachmentUrl = at.AttachmentUrl.GetValue(rd);
					return data;
				}).ToList();

			
			return PartialView(model);
		}


		//	学员填写成果
		//	GET: TeamPop/DaijHuod_XueyChengg
		//	POST-Ajax: TeamPop/DaijHuod_XueyChengg

		public ActionResult DaijHuod_XueyChengg(long activeId)
		{
			var model = db.TeamActiveResultDal.ConditionQuery(tar.ActiveId == activeId & 
				tar.MemberId == UserProfile.UserId, null, null, null).FirstOrDefault();


			if (model == null)
			{
				model = new TeamActiveResult() { ActiveId = activeId };
			}
			else
			{
				var atta = AttachmentsExtensions.GetAttachment(
					AttachmentsExtensions.GetAttachmentList(db, model.ResultId, AttachmentsKeys.DaijHuod_XueyChengg));

				if (atta != null && atta.Name.Length > 0)
				{
					model.AttachmentName = atta.Name;
					model.AttachmentUrl = atta.Url;
				}
			}


			return PartialView("DaijHuod_XueyChengg", model);
		}

		[HttpPost]
		public ActionResult DaijHuod_XueyChengg(long? id, TeamActiveResult model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.DaijHuod_XueyChengg,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};

			db.BeginTrans();

			try
			{
				if (id == null)
				{
					var data = new TeamActiveResult
					{
						ActiveResult = model.ActiveResult,
						MemberId = UserProfile.UserId,
						ActiveId = model.ActiveId,
                  CreateDate=DateTime.Now,
                  Creator= UserProfile.UserId
               };

					db.TeamActiveResultDal.Insert(data);
					atta.JoinId = data.ResultId;
				}
				else
				{
               model.Modifier = UserProfile.UserId;
               model.ModifyDate = DateTime.Now;

					db.TeamActiveResultDal.UpdatePartial(id.Value, new
					{
						model.ActiveResult,
                  model.Modifier,
                  model.ModifyDate
					});

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.DaijHuod_XueyChengg);
					atta.JoinId = id.Value;
				}

				AttachmentsExtensions.InsertAtta(db, atta);
				db.Commit();


            //记录日志
            var doSomthing = id == null ? "新增:" + id : "修改:" + id;
            if (!string.IsNullOrEmpty(atta.Name))
               doSomthing += string.Format(" 并且上传了附件:{0}", atta.Name);

            Log(AttachmentsKeys.DaijHuod_XueyChengg, doSomthing);
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


		//	首页学员填写成果
		//	GET:	TeamPop/Home_XueyChengg
		//	POST-Ajax: TeamPop/Home_XueyChengg

		public ActionResult Home_XueyChengg(long activeId)
		{
			return PartialView("Home_XueyChengg");
		}

		[HttpPost]
		public ActionResult Home_XueyChengg(TeamActiveResult model)
		{
			ThrowNotAjax();

			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.DaijHuod_XueyChengg,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{
				var data = new TeamActiveResult
				{
					ActiveResult = model.ActiveResult,
					MemberId = UserProfile.UserId,
					ActiveId = model.ActiveId,
               CreateDate=DateTime.Now,
               Creator= UserProfile.UserId
            };

				db.TeamActiveResultDal.Insert(data);
				atta.JoinId = data.ResultId;
				AttachmentsExtensions.InsertAtta(db, atta);

				db.Commit();

            //记录日志
            var doSomthing = "新增";
            if (!string.IsNullOrEmpty(atta.Name))
               doSomthing += string.Format(" 并且上传了附件:{0}", atta.Name);

            Log(AttachmentsKeys.DaijHuod_XueyChengg, doSomthing);
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


		#region [ 活动内容 ]


		//	活动内容
		//	GET: TeamPop/DaijHuod_Item
		//	POST-Ajax: TeamPop/DaijHuod_Item

		public ActionResult DaijHuod_Item(long activeId, long? itemId)
		{
			var model = new TeamActiveItem()
			{
				ActiveId = activeId,
				SendDate = DateTime.Today
			};

			if (itemId != null)
				model = db.TeamActiveItemDal.PrimaryGet(itemId.Value);


			return PartialView("DaijHuod_Item", model);
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult DaijHuod_Item(long? id, TeamActiveItem model)
		{
			ThrowNotAjax();

			if (id == null)
			{
				model.MemberId = UserProfile.UserId;
				model.Creator = UserProfile.UserId;
				model.CreateDate = DateTime.Now;
				db.TeamActiveItemDal.Insert(model);
			}
			else
			{
				model.Modifier = UserProfile.UserId;
				model.ModifyDate = DateTime.Now;
				db.TeamActiveItemDal.UpdatePartial(id.Value, new
				{
					model.ItemContent,
					model.SendDate,
					model.Modifier,
					model.ModifyDate
				});
			}


         //记录日志
         var doSomthing = id == null ? "新增:" + id : "修改:" + id;
         Log(AttachmentsKeys.DaijHuod_HuodNeir, doSomthing);


         return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 定向性课程实施.带教安排 ]


		//	GET: TeamPop/KecShis_Anp
		//	POST-Ajax: TeamPop/KecShis_Anp

		public ActionResult KecShis_Anp(long? itemId, long? courseId)
		{
			if (courseId != null || itemId != null)
			{
				var model = new TeamSpecialCourseItem()
				{
					TeamId = UserProfile.UserId,
					ItemDate = DateTime.Today
				};

				if (courseId != null)
					model.CourseId = courseId.Value;

				if (itemId != null)
					model = db.TeamSpecialCourseItemDal.PrimaryGet(itemId.Value);


				return PartialView("KecShis_Anp", model);
			}
			else
			{
				return Json(new
				{
					result = AjaxResults.Error,
					msg = "非法操作！"
				});
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult KecShis_Anp(long? id, TeamSpecialCourseItem model)
		{
			ThrowNotAjax();

			if (id == null)
			{
				model.Creator = UserProfile.UserId;
				model.CreateDate = DateTime.Now;
				db.TeamSpecialCourseItemDal.Insert(model);
			}
			else
			{
				model.Modifier = UserProfile.UserId;
				model.ModifyDate = DateTime.Now;
				db.TeamSpecialCourseItemDal.UpdatePartial(id.Value, new
				{
					model.ItemDate,
					model.Location,
					model.Speaker,
					model.Title,
					model.Content,
					model.ActivityType,
					model.Modifier,
					model.ModifyDate
				});
			}


         //记录日志
         var doSomthing = id == null ? "新增:" + id : "修改:" + id;
         Log("定向性课程实施.带教安排", doSomthing);


         return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 培养目标 ]


		public ActionResult Memo1()
		{

			var model = db.TeamContentDal.ConditionQuery(
				tc.TeamId == UserProfile.UserId & tc.ContentKey == TeamKeys.DaijJih_Memo1,
				null, null, null).FirstOrDefault();
			 
			return PartialView("Memo1", model);
		}

		[HttpPost]
		public ActionResult Memo1(long? id, TeamContent model)
		{
			ThrowNotAjax();

			if (id == null)
			{
				model.ContentKey = TeamKeys.DaijJih_Memo1;
				model.TeamId = UserProfile.UserId;
				model.Creator = UserProfile.UserId;
				model.CreateDate = DateTime.Now;

				db.TeamContentDal.Insert(model);
			}
			else
			{
				model.Modifier = UserProfile.UserId;
				model.ModifyDate = DateTime.Now;

				db.TeamContentDal.UpdatePartial(id.Value, new
				{
					model.ContentValue,
					model.Modifier,
					model.ModifyDate,
               model.IsDeclare
				});
			}

         AddDeclareMaterial(model, db.GetCurrentDeclarePeriod());

         //记录日志
         var doSomthing = id == null ? "新增:" + id : "修改:" + id;
         Log("培养目标", doSomthing);


         return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已保存!"
			});
		}


		#endregion


		#region [ 具体计划 ]


		public ActionResult Memo2()
		{
			var model = new TeamJutJihModel() { AttachmentName = "" };

			var data = db.TeamContentDal.ConditionQuery(
				tc.TeamId == UserProfile.UserId & tc.ContentKey == TeamKeys.DaijJih_Memo2,
				null, null, null).FirstOrDefault();

			if (data != null)
			{
				model.ContentValue = data.ContentValue;
				model.TeamContentId = data.TeamContentId;
				var atta = AttachmentsExtensions.GetAttachment(
					AttachmentsExtensions.GetAttachmentList(db, UserProfile.UserId, AttachmentsKeys.DaijJih_Memo2));
				model.AttachmentName = atta.Name;
				model.AttachmentUrl = atta.Url;
			}
		
			return PartialView("Memo2", model);
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Memo2(long? id, TeamJutJihModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.DaijJih_Memo2,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId,
				JoinId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{

				if (id == null)
				{
					var data = new TeamContent()
					{
						ContentKey = TeamKeys.DaijJih_Memo2,
						ContentValue = model.ContentValue,
						TeamId = UserProfile.UserId,
						CreateDate = DateTime.Now,
						Creator = UserProfile.UserId
					};

					db.TeamContentDal.Insert(data);
				}
				else
				{
					db.TeamContentDal.UpdatePartial(id.Value, new
					{
						ContentValue = model.ContentValue,
						Modifier = UserProfile.UserId,
						ModifyDate = DateTime.Now
					});

					AttachmentsExtensions.DeleteAtta(db, UserProfile.UserId, AttachmentsKeys.DaijJih_Memo2);
				}

				AttachmentsExtensions.InsertAtta(db, atta);
				db.Commit();


            //记录日志
            var doSomthing = id == null ? "新增:" + id : "修改:" + id;
            if (!string.IsNullOrEmpty(atta.Name))
               doSomthing += string.Format(" 并且上传了附件:{0}", atta.Name);

            Log(AttachmentsKeys.DaijJih_Memo2, doSomthing);
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
				msg = "信息已保存!"
			});
		}


		#endregion


		#region [ 带教小结 ]


		public ActionResult Memo3()
		{
			var model = new TeamDaijXiaojModel() { AttachmentName = "" };

			var data = db.TeamContentDal.ConditionQuery(
				tc.TeamId == UserProfile.UserId & tc.ContentKey == TeamKeys.DaijJih_Memo3,
				null, null, null).FirstOrDefault();

			if (data != null)
			{
				model.ContentValue = data.ContentValue;
				model.TeamContentId = data.TeamContentId;
				var atta = AttachmentsExtensions.GetAttachment(
					AttachmentsExtensions.GetAttachmentList(db, UserProfile.UserId, AttachmentsKeys.DaijJih_Memo3));
				model.AttachmentName = atta.Name;
				model.AttachmentUrl = atta.Url;
			}

			return PartialView("Memo3", model);
		}

		[HttpPost]
		public ActionResult Memo3(long? id, TeamDaijXiaojModel model)
		{
			ThrowNotAjax();


			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.DaijJih_Memo3,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId,
				JoinId = UserProfile.UserId
			};


			db.BeginTrans();

			try
			{

				if (id == null)
				{
					var data = new TeamContent()
					{
						ContentKey = TeamKeys.DaijJih_Memo3,
						ContentValue = model.ContentValue,
						TeamId = UserProfile.UserId,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.TeamContentDal.Insert(data);
				}
				else
				{
					db.TeamContentDal.UpdatePartial(id.Value, new
					{
						ContentValue = model.ContentValue,
						ModifyDate = DateTime.Now,
						Modifier = UserProfile.UserId
					});

					AttachmentsExtensions.DeleteAtta(db, UserProfile.UserId, AttachmentsKeys.DaijJih_Memo3);
				}

				AttachmentsExtensions.InsertAtta(db, atta);
				db.Commit();


            //记录日志
            var doSomthing = id == null ? "新增:" + id : "修改:" + id;
            if (!string.IsNullOrEmpty(atta.Name))
               doSomthing += string.Format(" 并且上传了附件:{0}", atta.Name);

            Log(AttachmentsKeys.DaijJih_Memo3, doSomthing);
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
				msg = "信息已保存!"
			});
		}


      #endregion


      #region [Helper]


      private void AddDeclareMaterial(TeamContent content, DeclarePeriod period)
      {
         db.DeclareMaterialDal.ConditionDelete(dm.ItemId == content.TeamContentId & dm.PeriodId == period.PeriodId);
         if (content.IsDeclare)
            db.DeclareMaterialDal.Insert(new DeclareMaterial
            {
               ItemId = content.TeamContentId,
               ParentType = "TeamContent",
               CreateDate = DateTime.Now,
               PubishDate = DateTime.Now,
               Title = content.ContentValue,
               Type = content.ContentKey,
               TeacherId = UserProfile.UserId,
               PeriodId = period.PeriodId
            });
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