using Business;
using Business.Helper;
using Business.Utilities;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class DeclareMaterialController : BaseController
	{

		private static APDBDef.DeclareMaterialTableDef dm = APDBDef.DeclareMaterial;
		private static APDBDef.DeclareActiveTableDef da = APDBDef.DeclareActive;
		private static APDBDef.DeclareAchievementTableDef dac = APDBDef.DeclareAchievement;
		private static APDBDef.DeclareContentTableDef dc = APDBDef.DeclareContent;
		private static APDBDef.DeclarePeriodTableDef p = APDBDef.DeclarePeriod;
		private static APDBDef.DeclareReviewTableDef df = APDBDef.DeclareReview;
		private static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;
		private static APDBDef.DeclareProfileTableDef dp = APDBDef.DeclareProfile;
		private static APDBDef.CompanyTableDef c = APDBDef.Company;
		private static APDBDef.AttachmentsTableDef at = APDBDef.Attachments;

		public ActionResult Index()
		{
			return View();
		}

		// POST-Ajax: DeclareMaterial/DeclareActive 申报活动 TODO: declareTargetId 表示当前正在申报的称号

		[HttpPost]
		// 
		public ActionResult DeclareActive(long id, bool isDeclare, long declareTargetId)
		{
			ThrowNotAjax();

			var period = Period;
			var active = db.DeclareActiveDal.PrimaryGet(id);
			active.IsDeclare = isDeclare;

			if (active != null)
			{
				db.BeginTrans();

				try
				{
					db.DeclareActiveDal.UpdatePartial(id, new { isDeclare = active.IsDeclare });

					DeclareMaterialHelper.AddDeclareMaterial(active, period, db, declareTargetId);

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
				msg = "没有可以申报的活动！"
			});

		}

		// POST-Ajax: DeclareMaterial/DeclareAchievement 申报成果材料 TODO: declareTargetId 表示当前正在申报的称号

		[HttpPost]
		//  
		public ActionResult DeclareAchievement(long id, bool isDeclare, long declareTargetId)
		{
			ThrowNotAjax();

			var period = Period;
			var achievement = db.DeclareAchievementDal.PrimaryGet(id);
			achievement.IsDeclare = isDeclare;

			if (achievement != null)
			{
				db.BeginTrans();

				try
				{
					db.DeclareAchievementDal.UpdatePartial(id, new { IsDeclare = achievement.IsDeclare });

					DeclareMaterialHelper.AddDeclareMaterial(achievement, period, db, declareTargetId);

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
				msg = "没有可以申报的成果！"
			});
		}


		// POST-Ajax: DeclareMaterial/DeclareContent  申报内容材料

		[HttpPost]

		public ActionResult DeclareContent(long id, bool isDeclare)
		{
			ThrowNotAjax();

			db.BeginTrans();

			try
			{
				db.BeginTrans();

				db.DeclareMaterialDal.ConditionDelete(dm.ItemId == id & dm.PeriodId == Period.PeriodId);

				APQuery.update(dc).set(dc.IsDeclare.SetValue(false)).where(dc.DeclareContentId == id).execute(db);


				db.Commit();
			}
			catch
			{
				db.Rollback();
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "操作成功！"
			});

		}


		// GET: DeclareMaterial/Fragment

		public ActionResult Fragment(string key)
		{
			if (key == DeclareKeys.GaodLisz_ZijBiao)
			{
				return PartialView("MaterialView5002", key);
			}
			else if (key == DeclareKeys.JidZhucr_ZijBiao)
			{
				return PartialView("MaterialView5003", key);
			}
			else if (key == DeclareKeys.GongzsZhucr || key == DeclareKeys.GongzsZhucr_Shenb)
			{
				key = key == DeclareKeys.GongzsZhucr ? DeclareKeys.GongzsZhucr_Shenb : key;
				return PartialView("MaterialView5004", key);
			}
			else if (key == DeclareKeys.XuekDaitr || key == DeclareKeys.XuekDaitr_Shenb || key == DeclareKeys.XuekDaitr_ZhicPog)
			{
				key = key == DeclareKeys.XuekDaitr ? DeclareKeys.XuekDaitr_Shenb : key;
				return PartialView("MaterialView5005", key);
			}
			else if (key == DeclareKeys.GugJiaos || key == DeclareKeys.GugJiaos_Shenb || key == DeclareKeys.GugJiaos_ZhicPog)
			{
				key = key == DeclareKeys.GugJiaos ? DeclareKeys.GugJiaos_Shenb : key;
				return PartialView("MaterialView5006", key);
			}
			else if (key == DeclareKeys.JiaoxNengs || key == DeclareKeys.JiaoxNengs_Shenb || key == DeclareKeys.JiaoxNengs_ZhicPog)
			{
				key = key == DeclareKeys.JiaoxNengs ? DeclareKeys.JiaoxNengs_Shenb : key;
				return PartialView("MaterialView5007", key);
			}
			if (key == DeclareKeys.JiaoxXinx || key == DeclareKeys.JiaoxXinx_Shenb)
			{
				key = key == DeclareKeys.JiaoxXinx ? DeclareKeys.JiaoxXinx_Shenb : key;
				return PartialView("MaterialView5008", key);
			}
			else if (key.IndexOf("-") > 0)
			{
				var typeKey = key.Substring(0, key.IndexOf("-"));

				ViewBag.DeclareTargetId = key.Substring(key.IndexOf("-") + 1);

				return PartialView("MaterialView9999", typeKey);
			}
			else
			{
				var review = db.DeclareReviewDal.PrimaryGet(Convert.ToInt64(key));
				var view = review.TypeKey.IndexOf(DeclareKeys.CaiLPog) >= 0 ? "Overview9999" : "Overview" + review.DeclareTargetPKID;
				return RedirectToAction("Overview", new DeclarePreviewParam { TeacherId = review.TeacherId, DeclareTargetId = review.DeclareTargetPKID, View = view, IsPartialView = true, TypeKey = review.TypeKey });
			}
		}

		// Get: DeclareMaterial/ReviewEdit
		// POST-Ajax: DeclareMaterial/ReviewEdit

		public ActionResult ReviewEdit(DeclareParam param)
		{
			var typeKey = param.TypeKey;
			var declareTargetId = param.DeclareTargetId;
			var poge = ".职称破格";
			var isZcPoge = typeKey.IndexOf(poge) > 0;
			var decalreReview = db.DeclareReviewDal.ConditionQuery(df.PeriodId == Period.PeriodId
							& df.TeacherId == UserProfile.UserId
							& df.TypeKey == (isZcPoge ? typeKey.Replace(poge, ".申报") : typeKey)
							& df.DeclareTargetPKID == declareTargetId
							, null, null, null).FirstOrDefault();

			decalreReview = decalreReview ?? new DeclareReview
			{
				DeclareTargetPKID = declareTargetId,
				AllowFitResearcher = true,
				AllowFlowToDowngrade = true,
				AllowFlowToSchool = true,
				TeacherName = UserProfile.RealName
			};
			decalreReview.TypeKey = param.TypeKey;

			return PartialView(param.View, decalreReview);
		}

		[HttpPost]

		public ActionResult ReviewEdit(DeclareReview model)
		{
			var userId = UserProfile.IsDeclare ? UserProfile.UserId : model.TeacherId;

			if (userId == 0 || BzUserProfile.PrimaryGet(userId) == null)
			{
				return Json(new
				{
					result = AjaxResults.Error,
					msg = "该用户不存在，请联系管理员！",
				});
			}

			var existReviews = db.DeclareReviewDal.ConditionQuery(df.TeacherId == userId & df.PeriodId == Period.PeriodId, null, null, null);
			if (existReviews.Exists(x => x.IsSubmit))
			{
				return Json(new
				{
					result = AjaxResults.Error,
					msg = "您已提交过表单！",
				});
			}

			if (string.IsNullOrEmpty(model.TypeKey))
			{
				return Json(new
				{
					result = AjaxResults.Error,
					msg = "必须选择申报类型！",
				});
			}

			if (model.DeclareTargetPKID == 0)
			{
				return Json(new
				{
					result = AjaxResults.Error,
					msg = "申报称号异常！请联系管理员",
				});
			}

			if (model.CompanyId == 0)
			{
				return Json(new
				{
					result = AjaxResults.Error,
					msg = "必须选择申报单位！",
				});
			}

			if (model.DeclareSubjectPKID == 0)
			{
				return Json(new
				{
					result = AjaxResults.Error,
					msg = "必须选择申报学科！",
				});
			}

			if (model.IsSubmit &&
			   GetProfile(userId, model.PeriodId, model.DeclareTargetPKID) == null)
			{
				return Json(new
				{
					result = AjaxResults.Error,
					msg = "提交前必须先完善和保存基本情况和校内履职！",
				});
			}

			string errorMessage = string.Empty;
			if (model.IsSubmit && !MaterialHasVertify(model.DeclareTargetPKID, out errorMessage))
			{
				return Json(new
				{
					result = AjaxResults.Error,
					msg = errorMessage,
				});
			}

			// 职称破格和申报公用一张表
			var poge = ".职称破格";
			if (model.TypeKey.IndexOf(poge) > 0)
				model.TypeKey = model.TypeKey.Replace(poge, ".申报");

			model.PeriodId = Period.PeriodId;
			model.TeacherId = userId;

			if (model.DeclareReviewId == 0)
			{
				if (!existReviews.Exists(x => x.TypeKey == model.TypeKey))
				{
					model.CreateDate = DateTime.Now;
					db.DeclareReviewDal.Insert(model);
				}
			}
			else
			{
				model.ModifyDate = DateTime.Now;
				db.DeclareReviewDal.UpdatePartial(model.DeclareReviewId, new
				{
					model.CompanyId,
					model.DeclareTargetPKID,
					model.DeclareSubjectPKID,
					model.IsBrokenRoles,
					model.Reason,
					model.AllowFlowToDowngrade,
					model.AllowFlowToSchool,
					model.AllowFitResearcher,
					model.StatusKey,
					model.TypeKey,
					model.TeacherName,
					model.ModifyDate
				});
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "操作成功！",
				data = model
			});
		}


		// Get: DeclareMaterial/BasicMaterialEdit
		// POST-Ajax: DeclareMaterial/BasicMaterialEdit

		public ActionResult BasicProfileEdit(DeclareParam param)
		{
			var profile = GetProfile(UserProfile.UserId, Period.PeriodId, param.DeclareTargetId);
			profile = profile ?? MappingProfile();
			if (profile.DeclareTargetPKID == 0)
			{
				profile.DeclareTargetPKID = param.DeclareTargetId;
			}

			// 绑定上一轮人才梯队任职情况，为多选
			if (!string.IsNullOrEmpty(profile.Dynamic4))
			{
				var targetIds = profile.Dynamic4.Split(',');
				profile.PrevioursDeclareTargets = new long[targetIds.Length];
				for (int i = 0; i < targetIds.Length; i++)
				{
					profile.PrevioursDeclareTargets[i] = Convert.ToInt64(targetIds[i]);
				}
			}

			// 如果是高地或者是基地主持人则 statuskey 从 review表获取
			if (param.DeclareTargetId == DeclareTargetIds.GaodLisz || param.DeclareTargetId == DeclareTargetIds.JidZhucr)
			{
				var review = db.DeclareReviewDal.ConditionQuery(df.DeclareTargetPKID == param.DeclareTargetId & df.TeacherId == UserProfile.UserId & df.PeriodId == Period.PeriodId, null, null, null).FirstOrDefault();
				if (review != null)
				{
					profile.StatusKey = review.StatusKey;
				}
			}

			return PartialView(param.View, profile);
		}

		[HttpPost]

		public ActionResult BasicProfileEdit(DeclareProfile model)
		{
			if (DeclareMaterialHelper.IsDeclareSubmit(Period.PeriodId, UserProfile.UserId, db))
				return Json(new
				{
					result = AjaxResults.Error,
					msg = "该老师的申报表单已经提交！"
				});

			if (model.PrevioursDeclareTargets != null && model.PrevioursDeclareTargets.Length > 0)
			{
				model.Dynamic4 = string.Join(",", model.PrevioursDeclareTargets);
			}

			if (model.DeclareProfileId == 0)
			{
				model.PeriodId = Period.PeriodId;
				model.UserId = UserProfile.UserId;

				db.DeclareProfileDal.Insert(model);
			}
			else
			{
				db.DeclareProfileDal.UpdatePartial(model.DeclareProfileId, UserProfile.UserId, new
				{
					model.RealName,
					model.GenderPKID,
					model.Birthday,
					model.CompanyId,
					model.TrainNo,
					model.PoliticalStatusPKID,
					model.CourseCountPerWeek,
					model.NationalityPKID,
					model.SkillTitlePKID,
					model.RankTitlePKID,
					model.Hiredate,
					model.Phonemobile,
					model.Phone,
					model.Email,
					model.EduBgPKID,
					model.EduDegreePKID,
					model.EduStagePKID,
					model.EduSubjectPKID,
					model.Dynamic1,
					model.Dynamic2,
					model.Dynamic3,
					model.Dynamic4,
					model.Dynamic5,
					model.StatusKey
				});
			}

			//TODO: 高地理事长和基地支持人的自荐表特殊处理
			if (model.DeclareTargetPKID == DeclareTargetIds.GaodLisz || model.DeclareTargetPKID == DeclareTargetIds.JidZhucr)
			{
				var decalreTargetId = model.DeclareTargetPKID;
				var typeKey = decalreTargetId == DeclareTargetIds.GaodLisz ? DeclareKeys.GaodLisz_ZijBiao : DeclareKeys.JidZhucr_ZijBiao;
				var review = db.DeclareReviewDal.ConditionQuery(df.DeclareTargetPKID == decalreTargetId & df.TeacherId == UserProfile.UserId & df.PeriodId == Period.PeriodId, null, null, null).FirstOrDefault();
				review = review ?? new DeclareReview
				{
					DeclareTargetPKID = decalreTargetId,
					DeclareSubjectPKID = model.EduSubjectPKID,
					CompanyId = model.CompanyId,
					TypeKey = typeKey,
					TeacherName = UserProfile.RealName,
				};
				review.StatusKey = model.StatusKey;
				review.TeacherName = string.IsNullOrEmpty(review.TeacherName) ? UserProfile.RealName : review.TeacherName;
				review.CompanyId = model.CompanyId;

				return ReviewEdit(review);
			}

			return Json(new
			{
				result = AjaxResults.Success,
				msg = "操作成功！"
			});
		}


		// Get: DeclareMaterial/DeclareActiveList   TODO: 历史库 declareTargetId表示当前申报的称号

		public ActionResult DeclareActiveList(string itemKey, long declareTargetId)
		{
			var subquery = APQuery.select(at.JoinId).from(at).where(at.UserId == UserProfile.UserId & at.Type == itemKey & at.AttachmentUrl.Match("cdn.sser.shdjg.net"));
			var results = db.DeclareActiveDal.ConditionQuery(da.TeacherId == UserProfile.UserId & da.ActiveKey == itemKey & da.DeclareActiveId.In(subquery), null, null, null);

			return PartialView("_declare_active_list", results);
		}


		// Get: DeclareMaterial/DeclareActiveList   TODO: 历史库 declareTargetId表示当前申报的称号

		public ActionResult DeclareAchievementList(string itemKey, long declareTargetId)
		{
			var subquery = APQuery.select(at.JoinId).from(at).where(at.UserId == UserProfile.UserId & at.Type == itemKey & at.AttachmentUrl.Match("cdn.sser.shdjg.net"));
			var results = db.DeclareAchievementDal.ConditionQuery(dac.TeacherId == UserProfile.UserId & dac.AchievementKey == itemKey & dac.DeclareAchievementId.In(subquery), null, null, null);

			return PartialView("_declare_achievement_list", results);
		}


		// Get: DeclareMaterial/Items   TODO: declareTargetId表示当前申报的称号

		public ActionResult Items(DeclareParam param)
		{
			var teacherId = UserProfile.UserId;
			var actives = APQuery.select(dm.MaterialId, dm.DeclareTargetPKID, da.Asterisk)
			   .from(dm, da.JoinInner(dm.ItemId == da.DeclareActiveId))
			   .where(dm.PeriodId == Period.PeriodId & dm.TeacherId == teacherId & da.Creator == teacherId & dm.DeclareTargetPKID == param.DeclareTargetId)
			   .query(db, r =>
			   {
				   var active = new DeclareActive();
				   da.Fullup(r, active, false);

				   return active;
			   }).ToList();

			var achievements = APQuery.select(dm.MaterialId, dm.DeclareTargetPKID, dac.Asterisk)
			   .from(dm, dac.JoinInner(dm.ItemId == dac.DeclareAchievementId))
			   .where(dm.PeriodId == Period.PeriodId & dm.TeacherId == teacherId & dac.Creator == teacherId & dm.DeclareTargetPKID == param.DeclareTargetId)
			   .query(db, r =>
			   {
				   var achievement = new DeclareAchievement();
				   dac.Fullup(r, achievement, false);

				   return achievement;
			   }).ToList();


			return PartialView(param.View, new DeclareItemsViewModel
			{
				DeclareTargetId = param.DeclareTargetId,
				View = param.View,
				DeclareAchievements = achievements,
				DeclareActives = actives
			});
		}


		// Get: DeclareMaterial/Preview

		public ActionResult Preview(DeclarePreviewParam param)
		{
			var pdfRender = new HtmlRender();
			var model = GetPreviewViewModel(param);

			if (param.IsExport != null && param.IsExport.Value)
			{
				var htmlText = pdfRender.RenderViewToString(this, param.View, model);
				byte[] pdfFile = FormatConverter.ConvertHtmlTextToPDF(htmlText);
				string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 10000);
				return new BinaryContentResult($"{fileName}.pdf", "application/pdf", pdfFile);
			}

			return PartialView(param.View, model);
		}


		// Get: DeclareMaterial/Overview

		public ActionResult Overview(DeclarePreviewParam param)
		{
			var model = GetPreviewViewModel(param);

			if (param.IsPartialView)
			{
				return PartialView(param.View, model);
			}

			return View(param.View, model);
		}


		#region [ Helper ]

		private string SubString(string str)
	  => str.Length > 50 ? str.Substring(0, 50) + "..." : str;

		private List<DeclareActive> GetDeclareActives(long declareTargetId, long teacherId) =>
		   APQuery.select(dm.MaterialId, dm.DeclareTargetPKID, da.Asterisk)
			.from(dm, da.JoinInner(dm.ItemId == da.DeclareActiveId))
			.where(dm.PeriodId == Period.PeriodId & dm.TeacherId == teacherId & da.Creator == teacherId & dm.DeclareTargetPKID == declareTargetId)
			.query(db, r =>
			{
				var active = new DeclareActive();
				da.Fullup(r, active, false);

				return active;
			}).ToList();

		private List<DeclareAchievement> GetDeclareAchievements(long declareTargetId, long teacherId) =>
		   APQuery.select(dm.MaterialId, dm.DeclareTargetPKID, dac.Asterisk)
			  .from(dm, dac.JoinInner(dm.ItemId == dac.DeclareAchievementId))
			  .where(dm.PeriodId == Period.PeriodId & dm.TeacherId == teacherId & dac.Creator == teacherId & dm.DeclareTargetPKID == declareTargetId)
			  .query(db, r =>
			  {
				  var achievement = new DeclareAchievement();
				  dac.Fullup(r, achievement, false);

				  return achievement;
			  }).ToList();

		private DeclareProfile MappingProfile(BzUserProfile profile = null)
		{
			profile = profile ?? UserProfile;
			return new DeclareProfile
			{
				RealName = profile.RealName,
				Birthday = profile.Birthday,
				CompanyId = profile.CompanyId,
				CompanyName = profile.CompanyName,
				CourseCountPerWeek = profile.CourseCountPerWeek,
				EduBgPKID = profile.EduBgPKID,
				EduDegreePKID = profile.EduDegreePKID,
				EduStagePKID = profile.EduStagePKID,
				EduSubjectPKID = profile.EduSubjectPKID,
				Email = profile.Email,
				GenderPKID = profile.GenderPKID,
				NationalityPKID = profile.NationalityPKID,
				TrainNo = profile.TrainNo,
				PoliticalStatusPKID = profile.PoliticalStatusPKID,
				SkillTitlePKID = profile.SkillTitlePKID,
				RankTitlePKID = profile.RankTitlePKID,
				Phonemobile = profile.Phonemobile,
				Phone = profile.Phone,
				Hiredate = DateTime.Now
			};
		}

		private DeclarePreviewViewModel GetPreviewViewModel(DeclarePreviewParam param)
		{
			var poge = ".职称破格";
			var isAdmin = UserProfile.IsSchoolAdmin || UserProfile.IsSystemAdmin || UserProfile.IsExpert || UserProfile.IsSpecialExpert || UserProfile.UserType.ToLower()=="admin";
			var userid = isAdmin ? param.TeacherId : UserProfile.UserId;
			var user = db.BzUserProfileDal.PrimaryGet(userid);
			var model = new DeclarePreviewViewModel();
			var profile = db.DeclareProfileDal.ConditionQuery
			  (dp.UserId == userid & dp.PeriodId == Period.PeriodId & dp.DeclareTargetPKID == param.DeclareTargetId,
			  null, null, null).FirstOrDefault();
			profile = profile ?? MappingProfile(user);

			var myCompnay = db.CompanyDal.PrimaryGet(profile.CompanyId);

			var review = db.DeclareReviewDal.ConditionQuery(
			   df.TeacherId == userid &
			   df.PeriodId == Period.PeriodId &
			   df.DeclareTargetPKID == param.DeclareTargetId &
			   df.TypeKey == param.TypeKey.Replace(poge, ".申报"), null, null, null).FirstOrDefault();

			// 如果review 表数据为空（没点保存按钮）则：职称破格和申报公用一张表 所以要用form IsBrokenRoles 和 param.TypeKey 一起判断，否则绑定IsBrokRoles字段
			model.IsBrokRoles = review == null ? string.IsNullOrEmpty(param.TypeKey) ? false : param.TypeKey.IndexOf(poge) > 0 : review.IsBrokenRoles;

			review = review ?? new DeclareReview();

			var declareCompany = db.CompanyDal.PrimaryGet(review.CompanyId);
			var declareActives = GetDeclareActives(param.DeclareTargetId, userid);
			var declareAchievement = GetDeclareAchievements(param.DeclareTargetId, userid);

			model.DeclareTargetId = param.DeclareTargetId;
			model.TypeKey = param.TypeKey;
			model.Decalre = DeclareBaseHelper.DeclareTarget.GetName(param.DeclareTargetId, string.Empty, false);
			model.DeclareSubject = BzUserProfileHelper.DeclareSubject.GetName(review.DeclareSubjectPKID, string.Empty, false);
			model.DeclareCompany = declareCompany == null ? string.Empty : declareCompany.CompanyName;
			model.ReviewTeacherName = review.TeacherName; //string.IsNullOrEmpty(user.RealName)? review.TeacherName:user.RealName;
			model.ProfileTeacherName = profile.RealName;
			model.RankTitle = profile.RankTitle;
			model.SkillTitle = profile.SkillTitle;
			model.TrainNo = profile.TrainNo;
			model.Plitics = profile.PoliticalStatus;
			//model.EduBg = profile.EduBg;
			model.CourseCount = profile.CourseCountPerWeek;
			model.Company = myCompnay == null ? string.Empty : myCompnay.CompanyName;
			model.Mobile = profile.Phonemobile;
			model.Phone = profile.Phone;
			model.Email = profile.Email;
			model.Hiredate = profile.Hiredate.ToString("yyyy-MM-dd");
			model.Birthday = profile.Birthday.ToString("yyyy-MM-dd");
			model.Gender = profile.Gender;
			model.Nation = profile.Nationality;
			model.Subject = profile.EduStage + profile.EduSubject;
			model.EduBg = profile.EduBg + profile.EduDegree.Replace("无", string.Empty);
			model.FirstYearScore = profile.Dynamic1;
			model.SecondYearScore = profile.Dynamic2;
			model.ThirdYearScore = profile.Dynamic3;
			profile.Dynamic4 = profile.Dynamic4 ?? string.Empty;
			model.Is500 = profile.Dynamic4.IndexOf(DeclareTargetIds.ZhongzJihChengy.ToString()) >= 0;
			model.Is1000 = profile.Dynamic4.IndexOf(DeclareTargetIds.GonggJihChengy.ToString()) >= 0;
			model.Is2000 = profile.Dynamic4.IndexOf(DeclareTargetIds.ZhongzJihLingxReng.ToString()) >= 0;
			model.Is3000 = profile.Dynamic4.IndexOf(DeclareTargetIds.GaofJihZhucRen.ToString()) >= 0;
			model.Is4000 = profile.Dynamic4.IndexOf(DeclareTargetIds.GonggJihZhucRen.ToString()) >= 0;
			model.Is5002 = profile.Dynamic4.IndexOf(DeclareTargetIds.GaodLisz.ToString()) >= 0;
			model.Is5003 = profile.Dynamic4.IndexOf(DeclareTargetIds.JidZhucr.ToString()) >= 0;
			model.Is5004 = profile.Dynamic4.IndexOf(DeclareTargetIds.GongzsZhucr.ToString()) >= 0;
			model.Is5005 = profile.Dynamic4.IndexOf(DeclareTargetIds.XuekDaitr.ToString()) >= 0;
			model.Is5006 = profile.Dynamic4.IndexOf(DeclareTargetIds.GugJiaos.ToString()) >= 0;
			model.Is5007 = profile.Dynamic4.IndexOf(DeclareTargetIds.JiaoxNengs.ToString()) >= 0;
			model.Is5008 = profile.Dynamic4.IndexOf(DeclareTargetIds.JiaoxXinx.ToString()) >= 0;
			model.Is6000 = profile.Dynamic4.IndexOf(DeclareTargetIds.GaodJiaoSYanxBanXuey.ToString()) >= 0;
			model.Comment1 = profile.Dynamic5;
			model.IsAllowDownGrade = review.AllowFlowToDowngrade;
			model.IsAllowdFlow = review.AllowFlowToSchool;
			model.DeclareActies = declareActives;
			model.DeclareAchievements = declareAchievement;
			model.Reason = review.Reason;
			model.IsPartialView = param.IsPartialView;


			return model;
		}

		private DeclareProfile GetProfile(long teacherId, long periodId, long targetId) =>
			db.DeclareProfileDal.ConditionQuery
			  (dp.UserId == UserProfile.UserId & dp.PeriodId == Period.PeriodId & dp.DeclareTargetPKID == targetId,
			  null, null, null).FirstOrDefault();


		private bool MaterialHasVertify(long declareTargetId, out string error)
		{
			bool result = true;
			error = "需编辑和完善历史库中选取的申报材料（例如上传证明文件）";
			var a = APDBDef.Attachments;
			var userid = UserProfile.UserId;
			var results = APQuery
			   .select(dm.Type, dm.Title, a.ID, dac.Dynamic1, dac.Dynamic2)
			   .from(dm,
					 dac.JoinLeft(dac.DeclareAchievementId == dm.ItemId & dm.ParentType == "DeclareAchievement"),
					 a.JoinLeft(dm.ItemId == a.JoinId & a.UserId == userid & a.Type.Match("证明"))
					 )
			   .where(
			   dm.TeacherId == userid &
			   dm.PeriodId == Period.PeriodId &
			   dm.DeclareTargetPKID == declareTargetId
			   ).query(db, r => new
			   {
				   type = dm.Type.GetValue(r),
				   title = dm.Title.GetValue(r),
				   hasVertify = a.ID.GetValue(r) > 0,
				   hasEror = string.IsNullOrEmpty(dac.Dynamic1.GetValue(r)) || string.IsNullOrEmpty(dac.Dynamic2.GetValue(r))
			   }).ToList();

			foreach (var item in results)
			{
				if (!item.hasVertify)
				{
					error += $"</br>材料名称：{item.title}";
					result = false;
				}
				if (item.type == DeclareKeys.ZisFaz_KeyChengg_FabLunw && item.hasEror)
				{
					error += $"</br>材料名称:{item.title}";
					result = false;
				}
				else if (item.type == DeclareKeys.ZisFaz_KeyChengg_LunzQingk && item.hasEror)
				{
					error += $"</br>材料名称:{item.title}";
					result = false;
				}
			}

			return result;
		}
		#endregion

	}

}