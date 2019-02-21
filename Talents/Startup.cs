using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Talents.Startup))]
namespace Talents
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
