using Business.XOrg;

namespace System.Web.Routing
{

	public static class RouteDataExtensions
	{

		public static XOrgData GetXorg(this RouteData routeData)
			=> routeData.Values[XorgControllerFactory.DataKey] as XOrgData;

	}

}