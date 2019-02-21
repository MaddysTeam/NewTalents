using CasUtility.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Xml;

namespace CasUtility
{

	public static class HttpRequestBaseExtensions
	{

		/// <summary>
		/// 获取 Cas 服务的登录地址
		/// </summary>
		/// <param name="request"></param>
		/// <param name="returnUrl"></param>
		/// <returns></returns>
		public static string CasLoginUrl(this HttpRequestBase request, string returnUrl)
			=> CasManager.GetCasLoginUrl(
				CasManager.GetServiceUrl(request.Url),
				returnUrl);


		/// <summary>
		/// 获取 Cas 服务的登录地址
		/// </summary>
		/// <param name="request"></param>
		/// <param name="serviceUrl"></param>
		/// <param name="returnUrl"></param>
		/// <returns></returns>
		public static string CasLoginUrl(this HttpRequestBase request, string serviceUrl, string returnUrl)
			=> CasManager.GetCasLoginUrl(
				serviceUrl,
				returnUrl);


		/// <summary>
		/// 获取 Cas 服务的登出地址
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static string CasLogoutUrl(this HttpRequestBase request)
			=> CasManager.GetCasLogoutUrl();


		/// <summary>
		/// 获取用户登录信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <param name="ticket"></param>
		/// <returns></returns>
		public static T CasLoginInfo<T>(this HttpRequestBase request, string ticket)
			where T : CasUserInfo
		{
			var validateUrl = CasManager.GetCasValidateUrl(CasManager.GetServiceUrl(request.Url), ticket);
			var validateResult = CasManager.GetCasValidateResult(validateUrl);
			var t = CasManager.ParseLoginInfo<T>(validateResult);

			CasManager.InsertTicket(ticket);

			return t;
		}


		/// <summary>
		/// 来自 Cas 的单点登出
		/// </summary>
		/// <param name="request"></param>
		/// <param name="logoutRequest"></param>
		public static void CasSingleLogout(this HttpRequestBase request, string logoutRequest)
		{
			var ticket = CasManager.ParseSingleLogoutInfo(logoutRequest);
			CasManager.SingleLogoutRevokeTicket(ticket);
		}


		/// <summary>
		/// 清除当前用户的 ticket
		/// </summary>
		/// <param name="request"></param>
		public static void CasRevokeTicket(this HttpRequestBase request)
			=> CasManager.RevokeTick();


		/// <summary>
		/// 检查 Cas 是否被单点登出，且用户的访问还持有 ticket
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static bool IsCasSingleLogouted(this HttpRequestBase request)
			=> CasManager.IsCasSingleLogouted();

	}

}
