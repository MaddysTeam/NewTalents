using Business;
using Business.XOrg;
using System.Web.Mvc;
using System.Web.Routing;

namespace System.Web.Mvc
{

	public static class ControllerExtensions
	{

		public static XOrgData GetXorg(this Controller controller)
			=> controller.RouteData.GetXorg();


		public static BzUserProfile GetUserProfile(this Controller controller)
			=> controller.HttpContext.GetUserProfile();


		public static bool IsRole(this Controller controller, string roleName)
			=> controller.HttpContext.IsRole(roleName);


		public static bool IsInRole(this Controller controller, params string[] roleNames)
			=> controller.HttpContext.IsInRole(roleNames);


		public static bool IsRoleInScope(this Controller controller, string scopeType, long scopeId, string roleName)
			=> controller.HttpContext.IsRoleInScope(scopeType, scopeId, roleName);


		public static bool IsRoleInScope(this Controller controller, string scopeType, long scopeId, params string[] roleNames)
			=> controller.HttpContext.IsRoleInScope(scopeType, scopeId, roleNames);

	}

}