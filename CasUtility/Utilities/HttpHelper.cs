using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CasUtility.Utilities
{

	internal static class HttpHelper
	{

		public static string PerformHttpGet(string url, bool requireHttp200)
		{
			string responseBody = null;

			ServicePointManager.ServerCertificateValidationCallback
				= new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				if (!requireHttp200 || response.StatusCode == HttpStatusCode.OK)
				{
					using (Stream responseStream = response.GetResponseStream())
					{
						if (responseStream != null)
						{
							using (StreamReader responseReader = new StreamReader(responseStream))
							{
								responseBody = responseReader.ReadToEnd();
							}
						}
					}
				}
			}

			return responseBody;
		}


		internal static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{
			return true;
		}


		public static string PerformHttpPost(string url, string postData, bool requireHttp200)
		{
			string responseBody = null;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = Encoding.UTF8.GetByteCount(postData);

			using (StreamWriter requestWriter = new StreamWriter(request.GetRequestStream()))
			{
				requestWriter.Write(postData);
			}

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					if (responseStream != null)
					{
						using (StreamReader responseReader = new StreamReader(responseStream))
						{
							responseBody = responseReader.ReadToEnd();
						}
					}
				}
			}

			return responseBody;
		}

	}

}
