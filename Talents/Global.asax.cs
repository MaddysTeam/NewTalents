using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TheSite.ModelBinder;

namespace Talents
{
   public class MvcApplication : System.Web.HttpApplication
   {
      protected void Application_Start()
      {
         //Symber.Web.Compilation.APGenManager.SyncAndInitData();


         AreaRegistration.RegisterAllAreas();
         FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
         RouteConfig.RegisterRoutes(RouteTable.Routes);
         BundleConfig.RegisterBundles(BundleTable.Bundles);
         BundleTable.EnableOptimizations = false;
         ModelBinders.Binders.DefaultBinder = new EmptyStringModelBinder();

      }
   }
}
