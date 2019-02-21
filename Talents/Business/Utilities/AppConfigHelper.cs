using System;
using System.Configuration;

namespace Business
{

   public static class AppConfigHelper
   {

      public static string LoginUrl
         => "http://rctd.hkedu.sh.cn:8000/Account/CasLogin"; //GetAppSetting("LoginUrl", "");


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