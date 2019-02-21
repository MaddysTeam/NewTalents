namespace System.Web.Mvc
{

	public class NoCacheAttribute : ActionFilterAttribute
	{
		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			filterContext.HttpContext.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
			filterContext.HttpContext.Response.Cache.SetNoStore();
		}
	}

}