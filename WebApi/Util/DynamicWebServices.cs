using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;

namespace WebService.Util
{
    /// <summary>
    /// 调用第三方JAVA或PHP编写的WebService
    /// 作者:刘鹏
    /// 日期:2022-5-11
    /// </summary>
    public class DynamicWebServices
    {
        /// <summary>
        /// 调用JAVAWebService
        /// </summary>
        /// <param name="InputData"></param>
        /// <returns></returns>
        public static string Invoke(string InputData)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["Url"].ToStr();
                //构造soap请求信息 需进入SOAPUI获取请求信息
                var soap = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:zls=""http://www.zlsoft.com/"">
                  <soapenv:Header/>
                                   <soapenv:Body>
                                        <zls:hl7Msg>
                                         <![CDATA[{InputData}]]>
                                        </zls:hl7Msg>
                                     </soapenv:Body>
                                  </soapenv:Envelope>";
                //发起请求
                var uri = new Uri(url);  //webServices 地址
                var request = WebRequest.Create(uri) as HttpWebRequest;
                request.ContentType = "text/xml; charset=UTF-8";
                request.UserAgent = "Apache-HttpClient/4.5.5 (java/12.0.1)";
                request.Accept = "gzip,deflate";
                request.Method = "POST";
                //SOAPAction 必填 进SOAPUI进行获取
                request.Headers.Add("SOAPAction", "http://www.zlsoft.com/hl7Msg");
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] paramBytes = Encoding.UTF8.GetBytes(soap.ToString());
                    requestStream.Write(paramBytes, 0, paramBytes.Length);
                }
                //响应
                var webResponse = request.GetResponse();
                using (var myStreamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                {
                    return myStreamReader.ReadToEnd().ToStr();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}