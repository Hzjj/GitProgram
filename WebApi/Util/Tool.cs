using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace WebService.Util
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Tool
    {
        public static string GetFileStr(string path)
        {
            string Meesage = "";
            string Jsonto = "";
            try
            {
                path = System.AppDomain.CurrentDomain.BaseDirectory + path;
                StreamReader sr = new StreamReader(path, Encoding.UTF8);
                while ((Jsonto = sr.ReadLine()) != null)
                {
                    Meesage += Jsonto;
                }
                sr.Close();
                return Meesage;
            }
            catch (Exception ex)
            {
                return Meesage;
            }
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        /// <summary>
        /// 32位MD5加密（小写）
        /// </summary>
        /// <param name="input">输入字段</param>
        /// <returns></returns>
        public static string Encrypt(string str)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));//转化为小写的16进制
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 32位MD5加密（大写）
        /// </summary>
        /// <param name="input">输入字段</param>
        /// <returns></returns>
        public string Encrypts(string str)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));//转化为大写的16进制
            }
            return sBuilder.ToString();
        }
            /// <summary>
            /// 将实体类转化为Xml
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static string XmlSerialize<T>(T obj)
        {
            using (System.IO.StringWriter sw = new StringWriter())
            {
                Type t = obj.GetType();
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
                sw.Close();
                return sw.ToString();
            }
        }

        /// <summary>
        /// 身份证号码验证（判断是否是正确的身份证号码）
        /// </summary>
        /// <param name="number">15或18位身份证</param>
        /// <returns>bool true or false</returns>
        public static bool CheckIDCard(string number)
        {
            try
            {
                if (number.Length != 15 && number.Length != 18)
                {
                    return false;
                }
                number = number.ToUpper();
                if (number.Length == 18)
                {
                    bool check = CheckIDCard18(number);
                    return check;
                }
                else
                {
                    bool check = CheckIDCard15(number);
                    return check;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否是15的身份证号
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证

            }

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";

            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }

            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();

            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }

            return true;//符合15位身份证标准

        }

        /// <summary>
        /// 判断是否是18位的身份证号
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";

            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准

        }

        //发送http请求
        public static string PostJson(string postUrl, string paramDat, string actions)
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
                DateTime businessTime = new DateTime();
                DateTime operationTime = new DateTime();
                request.Method = "Post";
                request.ContentType = "text/xml";
                //头信息
                //SetHeaderValue(request.Headers, "Content-Type", "application/fhir+json");
                //SetHeaderValue(request.Headers, "rootId", "402881f96463c15d016463c368670000");
                //SetHeaderValue(request.Headers, "token", "application/fhir+json");
                //SetHeaderValue(request.Headers, "businessTime", businessTime.ToString());
                //SetHeaderValue(request.Headers, "domain", "TFDTJ");
                //SetHeaderValue(request.Headers, "key", "7a162e35-105b-4ef7-b82f-9c1949be0483");
                //SetHeaderValue(request.Headers, "operationTime", operationTime.ToString());

                request.Timeout = 1000 * 3600;
                string requests = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:urn=\"urn:hl7-org:v3\">";
                requests += @" <soap:Header/>
                <soap:Body>
                <urn:HIPMessageServer>
                <!--Optional:-->
                <urn:action>" + actions + @"</urn:action>
                <!--Optional:--> ";
                requests += " <urn:message><![CDATA[" + paramDat + "]]></urn:message>";
                requests += @"</urn:HIPMessageServer>
                </soap:Body>
                </soap:Envelope>";
                byte[] data = Encoding.UTF8.GetBytes(requests);

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
                //LogHelper.Info(string.Format("【调用TopskyAPI接口异常，api名称{0}】{1}{2}", apiName, ex.Message, ex.StackTrace));
            }
            return responseString;
        }
        public static string PostJsons(string postUrl, string paramDat, string actions)
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
                request.ContentType = "text/json";
                request.Timeout = 1000 * 3600;
                SetHeaderValue(request.Headers, "Content-Type", "application/fhir+json;UTF-8");
                string requests = paramDat;
                byte[] data = Encoding.UTF8.GetBytes(requests);
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
                //LogHelper.Info(string.Format("【调用TopskyAPI接口异常，api名称{0}】{1}{2}", apiName, ex.Message, ex.StackTrace));
            }
            return responseString;
        }

        public static string GetJsons(string postUrl, string paramDat, string actions)
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
                request.Method = "Get";
                request.ContentType = "text/json";
                request.Timeout = 1000 * 3600;
                SetHeaderValue(request.Headers, "Content-Type", "application/fhir+json;UTF-8");
                //string requests = "";
                //byte[] data = Encoding.UTF8.GetBytes(requests);
                //request.ContentLength = data.Length;
                Stream reqStream = request.GetRequestStream();
                //reqStream.Write(data, 0, data.Length);
                reqStream.Close();
                using (StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.UTF8))
                {
                    responseString = sr.ReadToEnd();
                    //result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseString);
                }
            }
            catch (Exception ex)
            {
                //LogHelper.Info(string.Format("【调用TopskyAPI接口异常，api名称{0}】{1}{2}", apiName, ex.Message, ex.StackTrace));
            }
            return responseString;
        }
        public static string GetJson(string postUrl)
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
                //?appId=saca_9usU6a8WUN&appSecret=hXiBAGGbHnPq5vq0YCREQSTCrRd3PnfM
                request.Method = "Get";
                request.ContentType = "application/json";
                //SetHeaderValue(request.Headers, "appId", "saca_9usU6a8WUN");
                // SetHeaderValue(request.Headers, "appSecret", "hXiBAGGbHnPq5vq0YCREQSTCrRd3PnfM");
                //头信息
                request.Timeout = 1000 * 60;
                //string requests =paramDat;
                //string requests = "";
                //byte[] data = Encoding.UTF8.GetBytes(requests);

                //request.ContentLength = data.Length;
                //Stream reqStream = request.GetRequestStream();
                //reqStream.Write(data, 0, data.Length);
                //reqStream.Close();
                using (StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.UTF8))
                {
                    responseString = sr.ReadToEnd();
                    //result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseString);
                }
            }
            catch (Exception ex)
            {
                //LogHelper.Info(string.Format("【调用TopskyAPI接口异常，api名称{0}】{1}{2}", apiName, ex.Message, ex.StackTrace));
            }
            return responseString;
        }
        //string->base64
        public static string DecodeBase64(Encoding encode, string source)
        {
            string result = "";
            byte[] bytes = Convert.FromBase64String(source);
            try
            {
                result = encode.GetString(bytes);
            }
            catch
            {
                result = source;
            }
            return result;
        }
        public static string toBase64Str(string str)
        {
            string strBase64 = "";
            byte[] bt = new byte[str.Length];
            bt = System.Text.Encoding.UTF8.GetBytes(str);
            strBase64 = Convert.ToBase64String(bt);
            return strBase64;
        }
        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   // 总是接受  
            return true;
        }
        public static string CheckLen(string str, int maxlen, int minlex, string name, int linenum)
        {
            if (str != "")
            {
                int a = str.Length;
                if (a >= minlex && a <= maxlen)
                {
                    return str;
                }
                else
                {
                    throw new Exception(name + "，长度不符合" + minlex + "-" + maxlen + "位的验证！" + "第" + linenum + "行");
                }
            }
            else
            {
                throw new Exception(name + "，为空！" + "第" + linenum + "行");
            }
        }
        public static int GetCodeLineNum(int skipFrames)
        {
            StackTrace st = new StackTrace(skipFrames, true);
            StackFrame fram = st.GetFrame(0);
            int lineNum = fram.GetFileLineNumber();
            return lineNum;
        }
       //样板
        public Result QueryUnFee(string pid,string startTime,string endTime)
        {
            Result r = new Result();
            LogInfo l = new LogInfo("用户未缴费列表查询");
            try
            {
                l.Write($"记录入参：pid:{pid},startTime:{startTime},endTime:{endTime}");
                return r;
            }
            catch (Exception ex)
            {
                return r.SetLog(500, "程序出现异常：" + ex.ExFormat(),l);
            }
        }
//去除命名空间

       private XmlDocument RemoveNS(XmlDocument doc)
        {
            var xml = doc.OuterXml;
            var newxml = Regex.Replace(xml, @"xmlns[:xsi|:xsd]*="".*?""", "");
            var newdoc = new XmlDocument();
            newdoc.LoadXml(newxml);
            return newdoc;
        }

    }
}