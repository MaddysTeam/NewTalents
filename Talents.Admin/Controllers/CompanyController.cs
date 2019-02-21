using Business;
using Business.Config;
using Business.Helper;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheSite.Controllers;

namespace Talents.Admin.Controllers
{

   public class CompanyController : BaseController
   {

      static APDBDef.CompanyTableDef c = APDBDef.Company;
      static APDBDef.BzUserProfileTableDef p = APDBDef.BzUserProfile;
      static APDBDef.DeclareBaseTableDef d = APDBDef.DeclareBase;


      // GET: Company/Search
      // POST-Ajax: Company/Search

      public ActionResult Search()
      {
         return View();
      }

      [HttpPost]
      public JsonResult Search(int current, int rowCount, AjaxOrder sort, string searchPhrase, long companyId)
      {
         ThrowNotAjax();

         var query = APQuery.select(c.CompanyId, c.CompanyName)
             .from(c)
             .primary(c.CompanyId)
             .skip((current - 1) * rowCount)
             .take(rowCount);


         if (companyId > 0)
            query.where_and(c.CompanyId == companyId);

         //过滤条件
         //模糊搜索用户名、实名进行

         searchPhrase = searchPhrase.Trim();
         if (searchPhrase != "")
         {
            query.where_and(c.CompanyName.Match(searchPhrase));
         }

         //排序条件表达式

         if (sort != null)
         {
            switch (sort.ID)
            {
               case "name": query.order_by(sort.OrderBy(c.CompanyName)); break;
            }
         }


         //获得查询的总数量

         var total = db.ExecuteSizeOfSelect(query);


         //查询结果集

         var result = query.query(db, rd =>
         {
            return new
            {
               id = c.CompanyId.GetValue(rd),
               name = c.CompanyName.GetValue(rd),
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


      // GET: Company/Edit
      // POST-Ajax: Company/Edit

      public ActionResult Edit()
      {
         return PartialView();
      }

      [HttpPost]
      public ActionResult Edit(Company model)
      {
         if (model.CompanyId == 0)
         {
            db.CompanyDal.Insert(model);
         }
         else
         {
            db.NewsDal.UpdatePartial(model.CompanyId, new
            {
               model.CompanyName,
            });
         }

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "编辑成功！"
         });
      }


      // POST-Ajax: Company/AutoGenUser

      [HttpPost]
      public async Task<ActionResult> AutoGenUser()
      {
         ThrowNotAjax();

         var companies = db.CompanyDal.ConditionQuery(null, null, null, null);

         var existUsernames = APQuery.select(p.UserName)
            .from(p)
            .query(db, r => p.UserName.GetValue(r))
            .ToDictionary(m => m);


         //开始事务

         db.BeginTrans();

         try
         {
            foreach (var item in companies)
            {
               var user = new BzUser
               {
                  UserName = string.Format("schooladmin_{0:000}", item.CompanyId),
                  Actived = true,
               };
               var bzUserProfile = new BzUserProfile
               {
                  RealName = string.Format("{0}管理员", item.CompanyName),
                  Birthday = DateTime.Now,
                  UserType = BzRoleNames.SchoolAdmin,
               };

               if (existUsernames.ContainsKey(user.UserName))
               {
                  continue;
               }

               await _initUser(user,
                        ThisApp.DefaultPassword,
                        bzUserProfile,
                        new List<UserScope> {
                            new UserScope() { RoleId=BzRoleIds.SchoolAdmin, ScopeType = "", ScopeId = 0 },
                           },
                        item.CompanyId);
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

         return Json(new
         {
            result = AjaxResults.Success,
            msg = "用户创建成功"
         });
      }


      private async Task _initUser(BzUser user, string password, BzUserProfile profile, List<UserScope> roles, long companyId)
      {
         if (db.BzUserDal.PrimaryGet(user.Id) == null)
         {
            user.Email
               = profile.Email
               = user.UserName + ThisApp.DefaultEmailSuffix;

            var result = await UserManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
               throw new Exception("Create User Error!");
            }

            profile.UserId = user.Id;
            profile.UserName = user.UserName;
            db.BzUserProfileDal.Insert(profile);

            if (roles.Any())
            {
               foreach (var role in roles)
               {
                  result = await UserManager.AddToRoleAsync(user.Id, role.RoleId, role.ScopeType, role.ScopeId);
               }
            }

            db.CompanyAccesserDal.Insert(new CompanyAccesser
            {
               UserId = user.Id,
               CompanyId = companyId
            });
         }
      }


      private class UserScope
      {
         public long RoleId { get; set; }
         public string ScopeType { get; set; }
         public long ScopeId { get; set; }
      }

   }

}