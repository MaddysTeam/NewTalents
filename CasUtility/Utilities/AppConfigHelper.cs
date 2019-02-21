using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasUtility.Utilities
{

	internal static class AppConfigHelper
	{

		public static string CasLoginUrl
			=> GetAppSetting("CasLoginUrl", "");


		public static string CasLogoutUrl
			=> GetAppSetting("CasLogoutUrl", "");


		public static string CasValidateUrl
			=> GetAppSetting("CasValidateUrl", "");


		public static string CasSessId
			=> GetAppSetting("CasSessId", "");


		public static T GetAppSetting<T>(string key, T defaultValue)
		{
			if (!string.IsNullOrEmpty(key))
			{
				string value = ConfigurationManager.AppSettings[key];
				try
				{
					if (value != null)
					{
						var theType = typeof(T);
						if (theType.IsEnum)
							return (T)Enum.Parse(theType, value.ToString(), true);

						return (T)Convert.ChangeType(value, theType);
					}

					return default(T);
				}
				catch { }
			}

			return defaultValue;
		}

	}

}
