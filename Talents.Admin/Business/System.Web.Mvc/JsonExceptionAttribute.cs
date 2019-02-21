namespace System.Web.Mvc
{

	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class JsonExceptionAttribute : HandleErrorAttribute
	{

		public override void OnException(ExceptionContext filterContext)
		{
			if (!filterContext.ExceptionHandled)
			{
				//返回异常JSON
				filterContext.Result = new JsonResult
				{
					Data = new
					{
						result = AjaxResults.Error,
						msg = filterContext.Exception.Message
					},
					JsonRequestBehavior = JsonRequestBehavior.AllowGet
				};
			}
		}

	}

}