using Business.Helper;
using System.Linq;

namespace System.Web.Mvc
{
    public class PermissonAttribute : AuthorizeAttribute
    {

        string _permission;

        public PermissonAttribute(string permission)
        {
            this._permission = permission;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            if (httpContext.HasPermission(_permission))
            {
                return true;
            }
            else
            {
                throw new Exception("无访问权限！");
            }
        }
    }

}