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

	public class TeamController : BaseController
	{

		static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;
		static APDBDef.TeamMemberTableDef tm = APDBDef.TeamMember;
		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		static APDBDef.TeamContentTableDef tc = APDBDef.TeamContent;
		static APDBDef.TeamActiveTableDef ta = APDBDef.TeamActive;
		static APDBDef.TeamActiveItemTableDef tai = APDBDef.TeamActiveItem;
		static APDBDef.TeamActiveResultTableDef tar = APDBDef.TeamActiveResult;
		static APDBDef.TeamSpecialCourseTableDef tsc = APDBDef.TeamSpecialCourse;
		static APDBDef.TeamSpecialCourseItemTableDef tsci = APDBDef.TeamSpecialCourseItem;
		static APDBDef.DeclareMaterialTableDef dm = APDBDef.DeclareMaterial;

		#region [ 领衔人访问自己的团队 ]


		// GET: Team/Master

		public ActionResult Master(long? id)
		{
			if (id == null)
			{
				var model = db.DeclareBaseDal.PrimaryGet(UserProfile.UserId);

				if (model == null || !model.HasTeam)
				{
					throw new Exception("你还未拥有梯队导师身份，请向管理人员资讯！");
				}

				return RedirectToAction("Master", "Team", new { id = model.TeacherId });
			}
			else
			{
				return View();
			}
		}


		#endregion


		#region [ 学员访问自己的团队 ]


		// GET: Team/Member

		public ActionResult Member(long? id)
		{
			if (id == null)
			{
				var model = db.TeamMemberDal.ConditionQuery(tm.MemberId == UserProfile.UserId, null, null, null).FirstOrDefault();

				if (model == null)
				{
					throw new Exception("你还未拥有梯队学生身份，请向管理人员资讯！");
				}

				return RedirectToAction("Member", "Team", new { id = model.TeamId });
			}
			else
			{
				return View();
			}
		}


		#endregion


		// GET: Team/Fragment

		public ActionResult Fragment(string key, long teamId, string visiter, long? courseId)
		{
			switch (key)
			{
				//case TeamKeys.TidXinx:
				//   return TidXinx(teamId, visiter);
				//case TeamKeys.DaijJih:
				//   return DaijJih(teamId, visiter);
				//case TeamKeys.DaijHuod:
				//   return DaijHuod(teamId, visiter);
				//case TeamKeys.DaijHuod_Edit:
				//   return DaijHuod_Edit(visiter, Int64.Parse(Request["activeId"]));
				//case TeamKeys.DaijHuod_Timeline:
				//   return DaijHuod_Timeline(visiter, Int64.Parse(Request["activeId"]));
				//case TeamKeys.KecShis:
				//   return KecShis(teamId, visiter);
				//case TeamKeys.KecShis_Bianj:
				//   return KecShis_Bianj(teamId, visiter, courseId);
				//case TeamKeys.KecShis_Anp:
				//   return KecShis_Anp(courseId.Value, visiter);
				//case TeamKeys.KecShis_Chak:
				//   return KecShis_Detail(courseId.Value, visiter);

				case TeamKeys.TuanDXinx:
					return TuanDXinx(teamId, visiter);
				case TeamKeys.YanxHuod:
					return YanxHuod(teamId, visiter);
				case TeamKeys.YanxHuod_Edit:
					return YanxHuod_Edit(visiter, Int64.Parse(Request["activeId"]));
				case TeamKeys.TuanDZhidJians:
					return TuanDZhidJians(teamId, visiter);
			}

			return Content("该项目无效");
		}


		public ActionResult ShareTeamActive(long id, bool isShare)
		{
			ThrowNotAjax();

			var s = APDBDef.Share;

			var teamActive = db.TeamActiveDal.PrimaryGet(id);
			teamActive.IsShare = isShare;

			if (teamActive != null)
			{
				var hasAttachment = AttachmentsExtensions.HasAttachment(db, id, UserProfile.UserId);
				if (!hasAttachment && isShare)
				{
					return Json(new
					{
						result = AjaxResults.Success,
						msg = "该活动没有附件！"
					});
				}


				db.BeginTrans();

				try
				{
					db.TeamActiveDal.UpdatePartial(id, new { IsShare = teamActive.IsShare });

					if (teamActive.IsShare)
						db.ShareDal.Insert(new Share
						{
							ItemId = id,
							ParentType = "ShareTeamActive",
							CreateDate = DateTime.Now,
							PubishDate = DateTime.Now,
							Title = teamActive.Title,
							Type = PicklistHelper.TeamActiveType.GetName(teamActive.ActiveType),
							UserId = UserProfile.UserId
						});
					else
						db.ShareDal.ConditionDelete(s.ItemId == id);


					db.Commit();
				}
				catch
				{
					db.Rollback();

					return Json(new
					{
						result = AjaxResults.Error,
						msg = "操作失败！"
					});
				}

				return Json(new
				{
					result = AjaxResults.Success,
					msg = "操作成功！"
				});
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "没有可以共享的活动！"
			});
		}


		#region [ 团队信息 ]


		// GET: Team/TidXinx

		public ActionResult TuanDXinx(long teamId, string visiter)
		{
			var model = APQuery.select(d.Asterisk, u.RealName)
			   .from(d, u.JoinInner(d.TeacherId == u.UserId))
			   .where(d.TeacherId == teamId)
			   .query(db, r =>
			   {
				   var data = new DeclareBase();
				   d.Fullup(r, data, false);
				   data.RealName = u.RealName.GetValue(r);

				   return data;
			   }).ToList().First();


			ViewBag.MemberList = db.GetMemberListById(teamId);


			if (visiter.ToLower() != "master")
			{
				return PartialView("TuanDXinxView", model);
			}
			else
			{
				return PartialView("TuanDXinx", model);
			}
		}


		#endregion


		#region [ 研修活动 ]


		// GET: Team/YanxHuod

		public ActionResult YanxHuod(long teamId, string visiter)
		{
			var memberCount = (int)APQuery.select(tm.MemberId.Count())
			   .from(tm)
			   .where(tm.TeamId == teamId)
			   .executeScale(db);

			var model = APQuery.select(ta.TeamActiveId, ta.ActiveType, ta.Date, ta.Title, ta.IsShare, tar.ActiveId.Count().As("ActiveCount"), ta.IsDeclare)
			   .from(ta, tar.JoinLeft(ta.TeamActiveId == tar.ActiveId))
			   .group_by(ta.TeamActiveId, ta.ActiveType, ta.Date, ta.Title, ta.IsShare, ta.IsDeclare)
			   .where(ta.TeamId == teamId)
			   .query(db, rd =>
			   {
				   return new TeamActiveModel()
				   {
					   TeamActiveId = ta.TeamActiveId.GetValue(rd),
					   Date = ta.Date.GetValue(rd),
					   Title = ta.Title.GetValue(rd),
					   Count = rd.GetInt32(rd.GetOrdinal("ActiveCount")),
					   MemberCount = memberCount,
					   ActiveType = ta.ActiveType.GetValue(rd),
					   IsShare = ta.IsShare.GetValue(rd),
					   IsDeclare = ta.IsDeclare.GetValue(rd)
				   };
			   }).ToList();


			if (visiter.ToLower() != "master")
			{
				return PartialView("YanxHuodView", model);
			}
			else
			{
				return PartialView("YanxHuod", model);
			}
		}


		//	POST-Ajax: Team/RemoveYanxHuod

		[HttpPost]

		public ActionResult RemoveYanxHuod(long id)
		{
			ThrowNotAjax();

			var period = Period;
			var teamId = (long)APQuery.select(ta.TeamId)
			   .from(ta)
			   .where(ta.TeamActiveId == id)
			   .executeScale(db);

			db.BeginTrans();

			try
			{
				db.TeamActiveDal.PrimaryDelete(id);
				db.TeamActiveResultDal.ConditionDelete(tar.ActiveId == id);
				db.TeamActiveItemDal.ConditionDelete(tai.ActiveId == id);
				AttachmentsExtensions.DeleteAtta(db, id, AttachmentsKeys.YanXHuod_Edit);

				APQuery.update(d)
				   .set(d.ActiveCount.SetValue(APSqlRawExpr.Expr("ActiveCount - 1")))
				   .where(d.TeacherId == teamId)
				   .execute(db);

				db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == period.PeriodId);

				db.Commit();


				//记录日志
				Log(TeamKeys.DaijHuod_Delete, "删除:" + id);
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
				msg = "信息已删除！"
			});
		}


		//	POST-Ajax: Team/RemoveXueyChengg

		[HttpPost]

		public ActionResult RemoveXueyChengg(long id)
		{
			ThrowNotAjax();

			db.BeginTrans();

			try
			{
				db.TeamActiveResultDal.PrimaryDelete(id);

				AttachmentsExtensions.DeleteAtta(db, id, AttachmentsKeys.YanXHuod_XueyChengg);

				db.Commit();
			}
			catch
			{
				db.Rollback();
			}

			//记录日志
			Log(TeamKeys.DaijHuod_XueyChengg, "删除:" + id);


			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已删除！"
			});
		}


		public ActionResult YanxHuod_Edit(string visiter, long activeId)
		{
			var model = new TeamActiveDataModel() { TeamId = UserProfile.UserId, Date = DateTime.Today, AttachmentName = "" };

			if (activeId != 0)
			{
				model = APQuery.select(ta.Asterisk)
				   .from(ta)
				   .where(ta.TeamActiveId == activeId)
				   .query(db, r =>
				   {
					   return new TeamActiveDataModel()
					   {
						   ActiveType = ta.ActiveType.GetValue(r),
						   ContentValue = ta.ContentValue.GetValue(r),
						   Date = ta.Date.GetValue(r),
						   IsShow = ta.IsShow.GetValue(r),
						   Location = ta.Location.GetValue(r),
						   TeamActiveId = ta.TeamActiveId.GetValue(r),
						   TeamId = ta.TeamId.GetValue(r),
						   Title = ta.Title.GetValue(r),
						   IsDeclare = ta.IsDeclare.GetValue(r)
					   };
				   }).First();

				var atta = AttachmentsExtensions.GetAttachment(
				   AttachmentsExtensions.GetAttachmentList(db, activeId, AttachmentsKeys.YanXHuod_Edit));
				model.AttachmentName = atta.Name;
				model.AttachmentUrl = atta.Url;
			};


			return PartialView("YanxHuod_Edit", model);
		}

		[HttpPost]
		[ValidateInput(false)]

		public ActionResult YanxHuod_Edit(long? id, TeamActiveDataModel model)
		{
			ThrowNotAjax();

			var atta = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.YanXHuod_Edit,
				Name = model.AttachmentName,
				Url = model.AttachmentUrl,
				UserId = UserProfile.UserId
			};

			TeamActive data = null;

			if (id == null)
			{
				db.BeginTrans();

				try
				{
					data = new TeamActive()
					{
						ActiveType = model.ActiveType,
						ContentValue = model.ContentValue,
						Date = model.Date,
						IsShow = model.IsShow,
						Location = model.Location,
						TeamId = model.TeamId,
						Title = model.Title,
						//IsShare = model.IsShare,
						IsDeclare = model.IsDeclare,
						Creator = UserProfile.UserId,
						CreateDate = DateTime.Now
					};

					db.TeamActiveDal.Insert(data);

					APQuery.update(d)
					   .set(d.ActiveCount.SetValue(APSqlRawExpr.Expr("ActiveCount + 1")))
					   .where(d.TeacherId == model.TeamId)
					   .execute(db);

					atta.JoinId = data.TeamActiveId;
					AttachmentsExtensions.InsertAtta(db, atta);

					db.Commit();
				}
				catch
				{
					db.Rollback();
				}
			}
			else
			{
				db.BeginTrans();

				try
				{
					db.TeamActiveDal.UpdatePartial(id.Value, new
					{
						model.Title,
						model.Location,
						model.IsShow,
						model.ContentValue,
						model.ActiveType,
						model.Date,
						model.IsShare,
						Modifier = UserProfile.UserId,
						ModifyDate = DateTime.Now,
						model.IsDeclare
					});

					AttachmentsExtensions.DeleteAtta(db, id.Value, AttachmentsKeys.YanXHuod_Edit);
					atta.JoinId = id.Value;
					AttachmentsExtensions.InsertAtta(db, atta);

					db.Commit();
				}

				catch
				{
					db.Rollback();
				}

				data = db.TeamActiveDal.PrimaryGet(id.Value);

			}

			DeclareMaterialHelper.AddDeclareMaterial(data, Period, db);


			//记录日志
			var doSomthing = id == null ? "新增:" + id : "修改:" + id;
			if (!string.IsNullOrEmpty(atta.Name))
				doSomthing += string.Format(" 并且上传了附件:{0}", atta.Name);

			Log(TeamKeys.DaijHuod_Edit, doSomthing);


			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已保存！"
			});
		}


		#endregion


		#region [ 研修活动内容 ]


		// GET: Team/Timeline
		//	POST-Ajax: Team/RemoveDaijHuod_Item

		public ActionResult YanxHuod_Timeline(string visiter, long activeId)
		{
			var model = APQuery.select(u.RealName, tai.SendDate, tai.ItemContent, tai.ItemId, tai.MemberId, ta.TeamId)
			   .from(tai,
					 u.JoinInner(tai.MemberId == u.UserId),
					 ta.JoinInner(tai.ActiveId == ta.TeamActiveId))
			   .where(tai.ActiveId == activeId)
			   .order_by(tai.SendDate.Asc)
			   .query(db, rd =>
			   {
				   return new TeamActiveItemViewModel()
				   {
					   MemberName = u.RealName.GetValue(rd),
					   SendDate = tai.SendDate.GetValue(rd),
					   ItemContent = tai.ItemContent.GetValue(rd),
					   ItemId = tai.ItemId.GetValue(rd),
					   MemberId = tai.MemberId.GetValue(rd),
					   TeamId = ta.TeamId.GetValue(rd)
				   };
			   }).ToList();


			ViewBag.activeId = activeId;

			return PartialView("YanxHuod_Timeline", model);
		}

		[HttpPost]
		public ActionResult RemoveDaijHuod_Item(long id)
		{
			ThrowNotAjax();

			db.TeamActiveItemDal.PrimaryDelete(id);


			//记录日志
			Log(TeamKeys.DaijHuod_HuodNeir, "删除:" + id);


			return Json(new
			{
				result = AjaxResults.Success,
				msg = "信息已删除!"
			});
		}


		#endregion


		#region [ 制度建设和规划 ]

		public ActionResult TuanDZhidJians(long id, string visiter)
		{
			var allAts = AttachmentsExtensions.GetAttachmentList(db, id);
			var ats1 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_ZhidJians);
			var ats2 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_SannGuih);
			var at1 = AttachmentsExtensions.GetAttachment(ats1);
			var at2 = AttachmentsExtensions.GetAttachment(ats2);

			return PartialView("TuanDZhidJians", new ZhidJiansViewModel()
			{
				AttachmentName1 = at1.Name,
				AttachmentName2 = at2.Name,
				AttachmentUrl1 = at1.Url,
				AttachmentUrl2 = at2.Url
			});
		}

		[HttpPost]
		public ActionResult TuanDZhidJians(ZhidJiansViewModel model)
		{
			ThrowNotAjax();

			var atta1 = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.Tuand_ZhidJians,
				Name = model.AttachmentName1,
				Url = model.AttachmentUrl1,
				UserId = UserProfile.UserId,
				JoinId = UserProfile.UserId,
			};

			var atta2 = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.Tuand_SannGuih,
				Name = model.AttachmentName2,
				Url = model.AttachmentUrl2,
				UserId = UserProfile.UserId,
				JoinId = UserProfile.UserId,
			};

			db.BeginTrans();

			try
			{
				AttachmentsExtensions.DeleteAttas(db, UserProfile.UserId, new string[] { AttachmentsKeys.ZhidJians_DangaJians });

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta1, atta2 });

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
				msg = "信息已保存!"
			});
		}


		#endregion


		#region [ 团队项目 ]

		public ActionResult TuanDXinagm(long id, string visiter)
		{
			var allAts = AttachmentsExtensions.GetAttachmentList(db, id);
			var ats1 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_ZhidJians);
			var ats2 = allAts.FindAll(a => a.Type == AttachmentsKeys.Tuand_SannGuih);
			var at1 = AttachmentsExtensions.GetAttachment(ats1);
			var at2 = AttachmentsExtensions.GetAttachment(ats2);

			//return PartialView("TuanDZhidJians", new ZhidJiansViewModel()
			//{
			//	AttachmentName1 = at1.Name,
			//	AttachmentName2 = at2.Name,
			//	AttachmentUrl1 = at1.Url,
			//	AttachmentUrl2 = at2.Url
			//});
			var model = new TuandXiangmViewModel();

			var data = db.TeamProjectDal.ConditionQuery(
				tc.TeamId == id & tc.ContentKey == TeamKeys.TuanDXiangm,
				null, null, null).FirstOrDefault();

			if (data != null)
			{
				model.Name = data.DeclareUser;
				model.Company = data.DeclareCompany;
				model.Date = data.FillDate;

				//var atta = AttachmentsExtensions.GetAttachment(
				//	AttachmentsExtensions.GetAttachmentList(db, UserProfile.UserId, AttachmentsKeys.DaijJih_Memo2));
				//model.AttachmentName = atta.Name;
				//model.AttachmentUrl = atta.Url;
				//model.IsDeclare = data.IsDeclare;
			}

			return View(data);
		}

		[HttpPost]
		public ActionResult TuanDXinagm(long? id, TuandXiangmViewModel model)
		{
			ThrowNotAjax();

			var atta1 = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.Tuand_Xiangm_Kait,
				Name = model.AttachmentName1,
				Url = model.AttachmentUrl1,
				UserId = UserProfile.UserId,
				JoinId = id.Value
			};

			var atta2 = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.Tuand_Xiangm_zhongq,
				Name = model.AttachmentName2,
				Url = model.AttachmentUrl2,
				UserId = UserProfile.UserId,
				JoinId = id.Value
			};

			var atta3 = new AttachmentsDataModel()
			{
				Type = AttachmentsKeys.Tuand_Xiangm_zhongq,
				Name = model.AttachmentName3,
				Url = model.AttachmentUrl3,
				UserId = UserProfile.UserId,
				JoinId = id.Value
			};

			TeamProject data = null;

			db.BeginTrans();

			try
			{
				if (null == id)
				{
					data = new TeamProject()
					{
						ContentKey = TeamKeys.TuanDXiangm,
						DeclareUser = model.UserName,
						DeclareCompany = model.Company,
						FillDate = model.Date,
						TeacherId = UserProfile.UserId,
						TeamId = UserProfile.UserId,
						CreateDate = DateTime.Now,
						Creator = UserProfile.UserId,
					};

					db.TeamProjectDal.Insert(data);
				}
				else
				{
					db.TeamProjectDal.UpdatePartial(id.Value, new
					{
						DeclareUser = model.UserName,
						DeclareCompany = model.Company,
						FillDate = model.Date,
						Modifier = UserProfile.UserId,
						ModifyDate = DateTime.Now,
					});
				}

				AttachmentsExtensions.DeleteAttas(db, id.Value, new string[] {
					AttachmentsKeys.Tuand_Xiangm_Kait,
					AttachmentsKeys.Tuand_Xiangm_zhongq,
					AttachmentsKeys.Tuand_Xiangm_jiet });

				AttachmentsExtensions.InsertAttas(db, new AttachmentsDataModel[] { atta1, atta2, atta3 });

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
				msg = "信息已保存!"
			});
		}


		#endregion


		#region [Helper]


		private List<TeamContent> QueryTeamContent(long teamId, string startWith)
		   => db.TeamContentDal.ConditionQuery(
			  tc.TeamId == teamId & tc.ContentKey.StartWith(startWith),
			  null, null, null);


		private void SetTeamContent(string key, string value, bool isDeclare, string type = "String")
		{
			value = value.Trim();

			var maybeId = APQuery.select(tc.TeamContentId)
			   .from(tc)
			   .where(tc.TeamId == UserProfile.UserId & tc.ContentKey == key)
			   .executeScale(db);

			if (maybeId == null)
			{
				db.TeamContentDal.Insert(new TeamContent
				{
					TeamId = UserProfile.UserId,
					ContentKey = key,
					ContentValue = value,
					ContentDataType = type,
					Creator = UserProfile.UserId,
					CreateDate = DateTime.Now,
					IsDeclare = isDeclare,
				});
			}
			else
			{
				APQuery.update(tc)
				   .set(tc.ContentValue.SetValue(value))
				   .set(tc.Modifier.SetValue(UserProfile.UserId))
				   .set(tc.ModifyDate.SetValue(DateTime.Now))
				   .set(tc.IsDeclare.SetValue(isDeclare))
				   .where(tc.TeamContentId == (long)maybeId)
				   .execute(db);
			}
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