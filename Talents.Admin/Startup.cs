using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheSite.Mvc.Startup))]
namespace TheSite.Mvc
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}
