using CasUtility;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.Mvc;

namespace TheSite.Mvc
{

	public class CasAuthorizeAttribute : AuthorizeAttribute
	{

		/// <summary>
		/// 检查 Cas 是否被单点登出，且用户的访问还持有 ticket
		/// </summary>
		/// <param name="filterContext"></param>
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!httpContext.User.Identity.IsAuthenticated)
				return false;

			if (httpContext.Request.IsCasSingleLogouted())
			{
				// 客户端登出： 处理自己想要进行的用户登出，看自己想用的方式
				// 
				//		1. 通过 Microsoft.Identity 的做法
				//			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
				//		2. 通过 Session 的做法
				//			Session.Remove("user");
				//
				//		其他的方法根据自己情况类推

				httpContext.Request.CasRevokeTicket();
				httpContext.GetOwinContext().Authentication
					.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

				return false;
			}

			return base.AuthorizeCore(httpContext);
		}

	}

}