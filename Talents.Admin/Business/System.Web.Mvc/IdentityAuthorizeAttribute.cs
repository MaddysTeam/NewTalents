namespace System.Web.Mvc
{

	public class IdentityAuthorizeAttribute : AuthorizeAttribute
	{

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!httpContext.User.Identity.IsAuthenticated)
				return false;

			string[] StrRoles = Roles.Split(',');

			if (string.IsNullOrWhiteSpace(Roles))
			{
				return true;
			}

			if (httpContext.IsInRole(StrRoles))
			{
				return true;
			}
			else
			{
				throw new Exception("无访问权限！");
			}

			//return true;
		}
	}

}