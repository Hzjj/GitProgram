using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
namespace WebService.Util
{
    public class Http
    {
        public string PostJson(string postUrl, string paramDat, string Token = "")
        {
            string responseString = string.Empty;
            try
            {
                string url = postUrl;
                HttpWebRequest request = null;
                //如果是发送HTTPS请求  
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }
                request.Method = "Post";
                request.ContentType = "application/json;charset=UTF-8";

                if (!string.IsNullOrEmpty(Token))
                {
                    request.Headers.Set("Authorization", Token.ToStr());
                }

                request.Timeout = 1000 * 3600;

                byte[] data = Encoding.UTF8.GetBytes(paramDat);

                request.ContentLength = data.Length;

                Stream reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();


                using (StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.UTF8))
                {
                    responseString = sr.ReadToEnd();
                    //result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseString);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseString;
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   // 总是接受  
            return true;
        }




    }
}