using Business;
using Business.Config;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

	public class UserController : BaseController
	{

		static APDBDef.BzUserTableDef u = APDBDef.BzUser;
		static APDBDef.BzUserProfileTableDef up = APDBDef.BzUserProfile;


		// GET: User/Search
		// POST-Ajax: User/Search

        //[Permisson(Admin.UserVisit)]
		public ActionResult Search()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Search(int current, int rowCount, AjaxOrder sort, string searchPhrase, string userType)
		{
			ThrowNotAjax();

			var query = APQuery.select(up.UserId, up.UserName, up.RealName, up.UserType, up.CompanyName)
				.from(up)
				.primary(up.UserId)
				.skip((current - 1) * rowCount)
				.take(rowCount);


			//过滤条件
			//模糊搜索用户名、实名进行

			searchPhrase = searchPhrase.Trim();
			if (searchPhrase != "")
			{
				query.where_and(up.UserName.Match(searchPhrase) | up.RealName.Match(searchPhrase));
			}

			userType = userType.Trim();
			if (userType != "全部")
			{
				query.where_and(up.UserType == userType);
			}

			//排序条件表达式

			if (sort != null)
			{
				switch (sort.ID)
				{
					case "userName": query.order_by(sort.OrderBy(up.UserName)); break;
					case "realName": query.order_by(sort.OrderBy(up.RealName)); break;
					case "userType": query.order_by(sort.OrderBy(up.UserType)); break;
					case "company": query.order_by(sort.OrderBy(up.CompanyName)); break;
				}
			}


			//获得查询的总数量

			var total = db.ExecuteSizeOfSelect(query);


			//查询结果集

			var result = query.query(db, rd =>
			{
				return new
				{
					id = up.UserId.GetValue(rd),
					userName = up.UserName.GetValue(rd),
					realName = up.RealName.GetValue(rd),
					userType = up.UserType.GetValue(rd),
					company = up.CompanyName.GetValue(rd)
				};
			});

			return Json(new
			{
				rows = result,
				current,
				rowCount,
				total
			});
		}


        //	GET: User/Add

        [Permisson(Admin.UserAdd)]
        public ActionResult Add(long? id)
        {
            ThrowNotAjax();

            return RedirectToAction("Edit",new { id=id});
        }

        //	GET: User/Edit
        //	POST-Ajax: User/Edit

        public ActionResult Edit(long? id)
		{
			ThrowNotAjax();

			var model = id != null ? db.BzUserProfileDal.PrimaryGet(id.Value) : new BzUserProfile();


			return PartialView("Edit", model);
		}

		[HttpPost]
		public async Task<ActionResult> Edit(BzUserProfile model)
		{
			ThrowNotAjax();


			if (model.UserId == 0)
			{
				if (db.BzUserDal.ConditionQueryCount(u.UserName == model.UserName) > 0)
				{
					return Json(new
					{
						result = AjaxResults.Error,
						msg = "用户名已存在"
					});
				}

				db.BeginTrans();
				try
				{
					var user = new BzUser()
					{
						UserName = model.UserName,
						Email = model.UserName + ThisApp.DefaultEmailSuffix,
						Actived = true,
					};

					var result = await UserManager.CreateAsync(user, ThisApp.DefaultPassword);
					if (result.Succeeded)
					{
						model.UserId = user.Id;
						db.BzUserProfileDal.Insert(model);
					}
					else
					{
						throw new Exception(result.Errors.First());
					}

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
			}
			else
			{
				db.BzUserProfileDal.UpdatePartial(model.UserId, new
				{
					model.UserType,
					model.RealName,
					model.IDCard,
					model.GenderPKID,
					model.Birthday,
					model.CompanyName
				});
			}


			return Json(new
			{
				result = AjaxResults.Success,
				msg = "用户编辑成功"
			});
		}
		
	}

}