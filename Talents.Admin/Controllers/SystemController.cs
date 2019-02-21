using Business;
using Business.Config;
using Business.Helper;
using Business.Identity;
using Microsoft.AspNet.Identity.Owin;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class SystemController : BaseController
	{

		//	GET:					/Admin/System/SyncDBC
		//	POST-Ajax:			/Admin/Studio/BeginSync

		public ActionResult SyncDBC()
		{
			return View();
		}


		[HttpPost]
		public async Task<ActionResult> BeginSync()
		{
			var staffList = db.Staff_InfoDal.ConditionQuery(null, null, null, null);

			var userDict = db.BzUserProfileDal.ConditionQuery(null, null, null, null)
				.FindAll(m => m.IDCard != "")
				.ToDictionary(m => m.IDCard);

			foreach(var staff in staffList)
			{
				staff.sfzjh = staff.sfzjh.Trim();
				if (userDict.ContainsKey(staff.sfzjh))
				{
					string xm = staff.xm;
					long xb = staff.xb == "1" ? 1001 : 1002;
					DateTime csrq = DateTime.ParseExact(staff.csrq, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
					string dw = staff.dw ?? "";

					var old = userDict[staff.sfzjh];
					if (old.RealName != xm || old.GenderPKID != xb || old.Birthday != csrq || old.CompanyNameOuter != dw)
					{
						var t = APDBDef.BzUserProfile;
						APQuery.update(t)
							.set(t.RealName, xm)
							.set(t.GenderPKID, xb)
							.set(t.Birthday, csrq)
							.set(t.CompanyNameOuter, dw)
							.where(t.UUID == staff.id)
							.execute(db);
					}
				}
				else
				{

					try
					{
						// 新增用户信息

						await _initUserAdd(
							new BzUser
							{
								UserName = staff.sfzjh,
								Email = staff.sfzjh + "@hk_talents.com",
								Actived = true,
							},
							ThisApp.DefaultPassword,
							new BzUserProfile
							{
								UUID = staff.id,
								UserName = staff.sfzjh,
								UserType = ThisApp.Teacher,
								RealName = staff.xm,
								IDCard = staff.sfzjh,
								TrainNo = "",
								GenderPKID = staff.xb == "1" ? 1001 : 1002,
								Birthday = DateTime.ParseExact(staff.csrq, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
								CompanyName = staff.dw ?? "",
								CompanyNameOuter = staff.dw ?? "",
							});
					}
					catch (Exception ex)
					{
						return Json(new
						{
							result = "error",
							msg = ex.Message
						});
					}

				}
			}

			return Json(new
			{
				result = "success",
				msg = "同步成功"
			});
		}

		private async Task _initUserAdd(BzUser user, string password, BzUserProfile profile)
		{
			var result = await UserManager.CreateAsync(user, password);

			if (result.Succeeded)
			{
				profile.UserId = user.Id;
				db.BzUserProfileDal.Insert(profile);
			}
			else
			{
				throw new Exception(result.Errors.First());
			}
		}

	}
}