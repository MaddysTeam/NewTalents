using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Identity
{

	// 配置要在此应用程序中使用的应用程序登录管理器。
	public class ApplicationSignInManager : SignInManager<BzUser, long>
	{

		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
			 : base(userManager, authenticationManager)
		{
		}


		public override Task<ClaimsIdentity> CreateUserIdentityAsync(BzUser user)
		{
			return null;
		}


		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
		}

	}

}
