using Business;
using Business.Config;
using CasUtility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Symber.Web.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheSite.Models;

namespace TheSite.Controllers
{

   public class MyCasUserInfo : CasUserInfo
   {

      public string UUID { get; set; }
      public string XXDM { get; set; }
      public string XM { get; set; }

   }

   public class AccountController : BaseController
   {

      // GET: Account/Login
      // POST: Account/Login

      [AllowAnonymous]
      public ActionResult Login()
      {
         //SignInManager.SignIn(db.BzUserDal.PrimaryGet(ThisApp.AppUser_Admin_Id), false, false);

         return View();
      }

      [HttpPost]
      [AllowAnonymous]
      [ValidateAntiForgeryToken]
      public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
      {
         if (!ModelState.IsValid)
         {
            return View(model);
         }

         //if (model.Username.ToLower().IndexOf("admin") < 0 
         //   && model.Username.IndexOf("专家") < 0 
         //   && model.Username.IndexOf("tdkh") < 0)
         //{
         //   ModelState.AddModelError("", "非管理员用户，请使用统一认证登录方式。");
         //   return View(model);
         //}

         // 这不会计入到为执行帐户锁定而统计的登录失败次数中
         // 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
         var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);

         if (Request.IsAjaxRequest())
         {
            switch (result)
            {
               case SignInStatus.Success:
                  return Json(new { result = AjaxResults.Success, returnUrl });
               case SignInStatus.LockedOut:
                  return Json(new { result = AjaxResults.Error, msg = "账号锁定，请稍后重试" });
               case SignInStatus.RequiresVerification:
                  return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
               case SignInStatus.Failure:
               default:
                  return Json(new { result = AjaxResults.Error, msg = "用户名或密码不正确" });
            }
         }
         else
         {
            switch (result)
            {
               case SignInStatus.Success:
                  return RedirectToLocal(returnUrl);
               case SignInStatus.LockedOut:
                  return View("Lockout");
               case SignInStatus.RequiresVerification:
                  return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
               case SignInStatus.Failure:
               default:
                  ModelState.AddModelError("", "用户名或密码不正确。");
                  return View(model);
            }
         }
      }


      // GET: Account/CasLogin
      // POST: Account/CasLogin

      [AllowAnonymous]
      public ActionResult CasLogin(string ticket, string returnUrl)
      {
         if (ticket != null)
         {
            // 登录步骤 2： 来着客户端携带 Cas ticket 的再次访问
            // 登录步骤 3： 服务端发起 Cas 验证，并获取用户的登录信息

            var info = Request.CasLoginInfo<MyCasUserInfo>(ticket);

            // 登录步骤 4： 处理自己想要进行的用户登录，看自己想用的方式
            // 
            //		1. 通过 Microsoft.Identity 的做法
            //			var user = TUser.find(info.user);
            //			SignInManager.SignIn(user, false, false);
            //		2. 通过 Session 的做法
            //			Session.Add("user", info);
            //
            // 其他的方法根据自己情况类推

            var t = APDBDef.BzUser;
            var p = APDBDef.BzUserProfile;
            var userId = (long)APQuery.select(p.UserId)
               .from(p)
               .where(p.UUID == info.UUID)
               .executeScale(db);
            var user = db.BzUserDal.PrimaryGet(userId);

            if (user != null)
            {
               user.IsExtLogined = true;
               SignInManager.SignIn(user, false, false);
               return RedirectToAction("Profiles", "Studio");
            }
         }
         else
         {
            // 登录步骤 1： 来着客户端的首次请求，重定向到 Cas 登录服务地址

            return Redirect(Request.CasLoginUrl(returnUrl));
         }

         return RedirectToAction("Login");
      }

      [AllowAnonymous]
      [HttpPost]
      [ValidateInput(false)]
      public ActionResult CasLogin(string logoutRequest)
      {
         Request.CasSingleLogout(logoutRequest);

         // 测试用
         // CasManager.RevokeTick();

         return Content("OK");
      }


      // POST: Account/LogOff

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult LogOff()
      {
         // 客户端登出： 处理自己想要进行的用户登出，看自己想用的方式
         // 
         //		1. 通过 Microsoft.Identity 的做法
         //			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
         //		2. 通过 Session 的做法
         //			Session.Remove("user");
         //
         //		其他的方法根据自己情况类推

         AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
         Session[ThisApp.Approve] = null;

         if (UserProfile.IsExtLogined)
         {
            Request.CasRevokeTicket();
            return Redirect(Request.CasLogoutUrl());
         }
         else {
            return RedirectToAction("Login", "Account");
         }
      }


      //	修改密码
      //	GET:						/Account/ChgPwd			
      //	POST-AJAX:				/Account/ChgPwd	

      public ActionResult ChgPwd()
      {
         return View();
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<ActionResult> ChgPwd(ChgPwdViewModel model)
      {
         ThrowNotAjax();

         var result = await UserManager.ChangePasswordAsync(UserProfile.UserId, model.OldPassword, model.NewPassword);

         return Json(new
         {
            result = result.Succeeded ? AjaxResults.Success : AjaxResults.Error,
            msg = result.Succeeded ? "密码修改成功" : result.Errors.FirstOrDefault()
         });
      }


      //	用户重置密码
      //	POST-AJAX:			/Account/Reset

      [HttpPost]
      public async Task<ActionResult> Reset(long id)
      {
         ThrowNotAjax();

         db.BeginTrans();

         try
         {
            var Token = await UserManager.GeneratePasswordResetTokenAsync(id);
            var result = await UserManager.ResetPasswordAsync(id, Token, ThisApp.DefaultPassword);

            if (!result.Succeeded)
               throw new Exception(result.Errors.First());

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
            msg = "密码已重置为：" + ThisApp.DefaultPassword
         });
      }


      #region [ Private ]


      private ActionResult RedirectToLocal(string returnUrl)
      {
         if (Url.IsLocalUrl(returnUrl))
         {
            return Redirect(returnUrl);
         }

         return RedirectToAction("Index", "Home");
      }


      #endregion

   }

}