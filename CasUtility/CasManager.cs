using CasUtility.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Xml;

namespace CasUtility
{

	public static class CasManager
	{

		private static XmlReaderSettings xmlReaderSettings;
		private static NameTable xmlNameTable;
		private static XmlNamespaceManager xmlNamespaceManager;
		private const string XML_SESSION_INDEX_ELEMENT_NAME = "samlp:SessionIndex";
		private const string XML_USER_ELEMENT_NAME = "cas:user";
		private const string XML_USER_ATTRIBUTES_NAME = "cas:attributes";

		private const string REQUEST_SESSION_TICKET = "CasTicket::RequestSessionTicket";
		private const string CACHE_TICKET_KEY_PREFIX = "CasTicket::";

		private static TimeSpan timeoutMinutes;


		static CasManager()
		{
			SessionStateSection sessionStateSection = WebConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;
			timeoutMinutes = sessionStateSection.Timeout;
		}


		/// <summary>
		/// 从指定 Uri 中获取应用的服务地址
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static string GetServiceUrl(Uri uri)
			=> uri.OriginalString.Substring(0, uri.OriginalString.Length - uri.Query.Length);


		/// <summary>
		/// 获取 Cas 服务的登录地址
		/// </summary>
		/// <param name="serviceUrl"></param>
		/// <param name="returnUrl"></param>
		/// <returns></returns>
		public static string GetCasLoginUrl(string serviceUrl, string returnUrl)
		{
			if (!String.IsNullOrEmpty(returnUrl))
			{
				serviceUrl += ((serviceUrl.IndexOf('?') == -1) ? "?" : ":") + "returnUrl=" + returnUrl;
			}

			var loginUrl = String.Format("{0}?service={1}",
							AppConfigHelper.CasLoginUrl,
							HttpUtility.UrlEncode(serviceUrl));

			return loginUrl;
		}


		/// <summary>
		/// 获取 Cas 服务的登出地址
		/// </summary>
		/// <returns></returns>
		public static string GetCasLogoutUrl() => AppConfigHelper.CasLogoutUrl;


		/// <summary>
		/// 获取 Cas 服务的验证地址
		/// </summary>
		/// <param name="serviceUrl"></param>
		/// <param name="ticket"></param>
		/// <returns></returns>
		public static string GetCasValidateUrl(string serviceUrl, string ticket)
			=> String.Format("{0}?service={1}&ticket={2}",
				AppConfigHelper.CasValidateUrl,
				serviceUrl,
				ticket);


		/// <summary>
		/// 获取 Cas 验证后的用户信息，Xml字符串
		/// </summary>
		/// <param name="validateUrl"></param>
		/// <returns></returns>
		public static string GetCasValidateResult(string validateUrl)
			=> HttpHelper.PerformHttpGet(validateUrl, true);


		/// <summary>
		/// 解析 Cas 验证后的用户登录信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="validateResult"></param>
		/// <returns></returns>
		public static T ParseLoginInfo<T>(string validateResult) where T : CasUserInfo
		{
			Type dataType = typeof(T);
			var t = Activator.CreateInstance(dataType) as T;

			CheckXmlEnv();

			XmlParserContext xmlParserContext = new XmlParserContext(null, xmlNamespaceManager, null, XmlSpace.None);

			using (TextReader textReader = new StringReader(validateResult))
			{
				XmlReader reader = XmlReader.Create(textReader, xmlReaderSettings, xmlParserContext);
				bool hasUser = false, overAttr = false;

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.Name == XML_USER_ELEMENT_NAME)
						{
							t.User = reader.ReadString();
							hasUser = true;
						}
						else if (reader.Name == XML_USER_ATTRIBUTES_NAME)
						{
							while (reader.Read())
							{
								if (reader.NodeType == XmlNodeType.EndElement && reader.Name == XML_USER_ATTRIBUTES_NAME)
								{
									overAttr = true;
									break;
								}
								if (reader.NodeType == XmlNodeType.Element)
								{
									var name = reader.Name.Replace("cas:", "");
									var text = reader.ReadString();

									var piProp = dataType.GetProperty(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
									if (piProp != null)
									{
										piProp.SetValue(t, Convert.ChangeType(text, piProp.PropertyType));
									}
								}
							}
						}
					}

					if (hasUser && overAttr)
					{
						break;
					}
				}


				reader.Close();
			}

			return t;
		}


		/// <summary>
		/// 解析 Cas 的单点登出信息
		/// </summary>
		/// <param name="logoutRequest"></param>
		/// <returns></returns>
		public static string ParseSingleLogoutInfo(string logoutRequest)
		{
			CheckXmlEnv();

			XmlParserContext xmlParserContext = new XmlParserContext(null, xmlNamespaceManager, null, XmlSpace.None);
			string elementText = null;

			using (TextReader textReader = new StringReader(logoutRequest))
			{
				XmlReader reader = XmlReader.Create(textReader, xmlReaderSettings, xmlParserContext);
				bool foundElement = reader.ReadToFollowing(XML_SESSION_INDEX_ELEMENT_NAME);
				if (foundElement)
				{
					elementText = reader.ReadElementString();
				}

				reader.Close();
			}

			return elementText;
		}


		/// <summary>
		/// 记录当前用户登录成功时的 ticket
		/// </summary>
		/// <param name="ticket"></param>
		public static void InsertTicket(string ticket)
		{
			HttpContext.Current.Session[REQUEST_SESSION_TICKET] = ticket;
			HttpContext.Current.Cache.Insert(CACHE_TICKET_KEY_PREFIX + ticket, ticket, null, DateTime.Now.Add(timeoutMinutes), Cache.NoSlidingExpiration);
		}


		/// <summary>
		/// 清除当前登录用户的 ticket，几乎只会在用户登出系统前调用
		/// </summary>
		public static void RevokeTick()
		{
			var ticket = HttpContext.Current.Session[REQUEST_SESSION_TICKET] as string;
			if (ticket != null)
			{
				HttpContext.Current.Session.Remove(REQUEST_SESSION_TICKET);
				HttpContext.Current.Cache.Remove(CACHE_TICKET_KEY_PREFIX + ticket);
			}
		}


		/// <summary>
		/// 来着 Cas 单点登出时的调用，销毁记录的 ticket，之后会在登录用户的访问时，检查
		/// Session 中的 ticket 是否有效
		/// </summary>
		/// <param name="ticket"></param>
		public static void SingleLogoutRevokeTicket(string ticket)
		{
			HttpContext.Current.Cache.Remove(CACHE_TICKET_KEY_PREFIX + ticket);
		}


		/// <summary>
		/// 检查 Cas 是否被单点登出，且用户的访问还持有 ticket
		/// </summary>
		/// <returns></returns>
		public static bool IsCasSingleLogouted()
		{
			var ticket = HttpContext.Current.Session[REQUEST_SESSION_TICKET] as string;
			if (ticket != null && HttpContext.Current.Cache[CACHE_TICKET_KEY_PREFIX + ticket] == null)
				return true;
			return false;
		}


		private static void CheckXmlEnv()
		{
			if (xmlNamespaceManager == null)
			{
				xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
				xmlReaderSettings.IgnoreWhitespace = true;

				xmlNameTable = new NameTable();

				xmlNamespaceManager = new XmlNamespaceManager(xmlNameTable);
				xmlNamespaceManager.AddNamespace("cas", "http://www.yale.edu/tp/cas");
				xmlNamespaceManager.AddNamespace("saml", "urn: oasis:names:tc:SAML:1.0:assertion");
				xmlNamespaceManager.AddNamespace("saml2", "urn: oasis:names:tc:SAML:1.0:assertion");
				xmlNamespaceManager.AddNamespace("samlp", "urn: oasis:names:tc:SAML:1.0:protocol");
			}
		}

	}

}
