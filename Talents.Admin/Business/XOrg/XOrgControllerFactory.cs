using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Business.XOrg
{

	public class XorgControllerFactory : DefaultControllerFactory
	{
		internal readonly static string DataKey = "xorgcache";

		protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
		{
			// 需要在 Global.asax.cs 中注册本工厂
			// ControllerBuilder.Current.SetControllerFactory(new XorgControllerFactory());

			//var xorg = requestContext.RouteData.Values[dataKey];
			//if (xorg != null)
			//{
			//	Dictionary<string, XOrg> dict = ThisAppCache.GetCache<Dictionary<string, XOrg>>();

			//	if (dict == null)
			//	{
			//		// TODO: 获取对应XOrg数据 
			//		// dict = APBplDef.MoocOrgBpl.GetAll().ToDictionary((t) => t.Subdomain);
			//		ThisAppCache.SetCache(dict);
			//	}

			//	requestContext.RouteData.Values[dataKey] = dict[(string)xorg];
			//}

			requestContext.RouteData.Values[DataKey] = new XOrgData
			{
				OrgId = 1,
				OrgName = "xxx"
			};

			return base.GetControllerInstance(requestContext, controllerType);
		}

	}

}