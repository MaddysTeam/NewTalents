using Business;
using System;
using System.Web.Mvc;

namespace TheSite.Controllers
{

	public class StudioController : BaseController
	{

		static APDBDef.BzUserProfileTableDef u = APDBDef.BzUserProfile;

		// GET: Studio/Profiles
		// POST-Ajax: Studio/Profiles

		public ActionResult Profiles()
		{
			var model = db.BzUserProfileDal.PrimaryGet(UserProfile.UserId);

         var current= db
            .ProfileModifyPeriodDal
            .ConditionQuery(null, null, null, null).FindLast(pmp=>pmp.IsCurrent);

         if (current != null)
         {
            ViewBag.CurrentPeriod = current;

            ViewBag.CanEdit = current.BeginDate <= TodayEnd(DateTime.Now) && current.EndDate >= TodayStart(DateTime.Now);
         }

         return PartialView(model);
		}

		[HttpPost]
		public ActionResult Profiles(BzUserProfile model)
		{
			ThrowNotAjax();

			db.BzUserProfileDal.UpdatePartial(UserProfile.UserId, new
			{
				model.TrainNo,
				model.PoliticalStatusPKID,
				model.NationalityPKID,
            model.GenderPKID,

				model.EduSubjectPKID,
				model.EduStagePKID,
				model.JobDate,
				model.SkillTitlePKID,
				model.SkillDate,
				model.Companyaddress,
				model.RankTitlePKID,

				model.EduBgPKID,
				model.EduDegreePKID,
				model.GraduateSchool,
				model.GraduateDate,

				model.Email,
				model.Phonemobile,
            model.PeriodId
			});

			if (Request.IsAjaxRequest())
			{

				return Json(new
				{
					result = AjaxResults.Success,
					msg = "个人简档已保存！"
				});
			}
			else
			{
				return View();
			}
		}


      #region [ Helper ]


      private DateTime TodayStart( DateTime date)
      {
         return DateTime.Parse(date.ToString("yyyy-MM-dd") + "  00:00:00");
      }

      private  DateTime TodayEnd( DateTime date)
      {
         return DateTime.Parse(date.AddDays(1).ToString("yyyy-MM-dd") + "  00:00:00");
      }


      #endregion

   }

}