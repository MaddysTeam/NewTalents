﻿namespace System.Web.Mvc
{

	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public class LogExceptionAttribute : HandleErrorAttribute
	{

		public override void OnException(ExceptionContext filterContext)
		{
			if (!filterContext.ExceptionHandled)
			{
				string controllerName = (string)filterContext.RouteData.Values["controller"];
				string actionName = (string)filterContext.RouteData.Values["action"];
				string msgTemplate = "在执行 controller[{0}] 的 action[{1}] 时产生异常";
				// LogManager.GetLogger("LogExceptionAttribute").Error(string.Format(msgTemplate, controllerName, actionName), filterContext.Exception);
			}

			if (filterContext.Result is JsonResult)
			{
				// 当结果为json时，设置异常已处理

				filterContext.ExceptionHandled = true;
			}
			else
			{
				// 否则调用原始设置

				base.OnException(filterContext);
			}
		}

	}

}