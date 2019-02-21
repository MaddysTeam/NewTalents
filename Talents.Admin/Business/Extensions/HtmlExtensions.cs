using Business;
using Business.XOrg;
using System.Web.Mvc;
using System.Web.Routing;

namespace System.Web.Mvc
{

	public static class HtmlExtensions
	{

		public static XOrgData GetXorg(this HtmlHelper helper)
			=> helper.ViewContext.RouteData.GetXorg();


		public static BzUserProfile GetUserProfile(this HtmlHelper helper)
			=> helper.ViewContext.HttpContext.GetUserProfile();


		public static bool IsRole(this HtmlHelper helper, string roleName)
			=> helper.ViewContext.HttpContext.IsRole(roleName);


		public static bool IsInRole(this HtmlHelper helper, params string[] roleNames)
			=> helper.ViewContext.HttpContext.IsInRole(roleNames);


		public static bool IsRoleInScope(this HtmlHelper helper, string scopeType, long scopeId, string roleName)
			=> helper.ViewContext.HttpContext.IsRoleInScope(scopeType, scopeId, roleName);


		public static bool IsRoleInScope(this HtmlHelper helper, string scopeType, long scopeId, params string[] roleNames)
			=> helper.ViewContext.HttpContext.IsRoleInScope(scopeType, scopeId, roleNames);


        public static bool HasPermission(this HtmlHelper helper, string permisson)
          => helper.ViewContext.HttpContext.HasPermission(permisson);

	}

}