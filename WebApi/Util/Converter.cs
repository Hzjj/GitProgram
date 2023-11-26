using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using WebService.Util;


namespace WebService
{
    /// <summary>
    /// 作者刘鹏
    /// 说明:拓展类
    /// </summary>
    public static class Converter
    {

        public static string Validate<T>(this T t)
        {
            Type type = t.GetType();
            //获取所有属性
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<string> errorList = new List<string>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.IsDefined(typeof(BaseAttribute),false))//如果属性上有定义该属性,此步没有构造出实例
                {
                    foreach (BaseAttribute attribute in propertyInfo.GetCustomAttributes(typeof(BaseAttribute),false))
                    {
                        if (!attribute.Validate(propertyInfo.GetValue(t, null)))
                        {
                            errorList.Add($"[{propertyInfo.Name}]" + attribute.error);
                        }
                    }
                }
            }
            return string.Join(",", errorList);
        }


        /// <summary>
        /// 设置返回结果值
        /// </summary>
        /// <param name="res"></param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static DataTable IfNull(this DataTable res, string Msg = "")
        {
            if (res.Rows.Count <= 0)
            {
                throw new Exception("[" + Msg + "]数据源获取失败,0条数据。");
            }
            return res;
        }

        /// <summary>
        /// 设置返回结果值
        /// </summary>
        /// <param name="res"></param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result Set(this Result res, int code, string msg)
        {
            try
            {
                res.Code = code;
                res.Msg = msg;
            }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Msg = ex.Message;
            }
            return res;
        }

        /// <summary>
        /// 设置返回结果值并写入日志
        /// </summary>
        /// <param name="res"></param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result SetLog(this Result res, int code, string msg, LogInfo log)
        {
            try
            {
                res.Code = code;
                res.Msg = msg;
                log.Write(msg);
            }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Msg = ex.Message;
            }
            return res;
        }

        /// <summary>
        /// 设置返回结果值并写入日志
        /// </summary>
        /// <param name="res"></param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result SetLog(this Result res, int code, Exception msg, LogInfo log)
        {
            try
            {
                res.Code = code;
                res.Msg = msg.Message.ToStr()+msg.Source+msg.StackTrace;
                log.Write(msg.ToStr());
            }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Msg = ex.Message;
            }
            return res;
        }

        /// <summary>
        /// MD5 32位小写加密
        /// </summary>
        /// <param name="ConvertString"></param>
        /// <returns></returns>
        public static string Md5(this string ConvertString) //32位大写
        {

            string cl = ConvertString;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X2");
            }
            return pwd;
        }

        /// <summary>
        /// 转化为yyyy-MM-dd时间格式
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ToTime8(this object obj, string defaultValue = "")
        {
            try
            {
                if (obj == null)
                {
                    DateTime time2 = DateTime.Now;
                    string retime3 = time2.ToString("yyyy-MM-dd").Trim();
                    return retime3;
                }
                DateTime time = Convert.ToDateTime(obj);
                string retime = time.ToString("yyyy-MM-dd").Trim();
                return retime;
            }
            catch (Exception ex)
            {
                DateTime time = DateTime.Now;
                string retime = time.ToString("yyyy-MM-dd").Trim();
                return retime;
            }
        }

        /// <summary>
        /// 转化为yyyy-MM-dd HH:mm:ss时间格式
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ToTime(this object obj, string defaultValue = "")
        {
            try
            {
                if (obj == null)
                {
                    DateTime time2 = DateTime.Now;
                    return time2.ToString("yyyy-MM-dd HH:mm:ss").Trim();
                }
                DateTime time = Convert.ToDateTime(obj);
                string retime = time.ToString("yyyy-MM-dd HH:mm:ss").Trim();
                return retime;
            }
            catch (Exception ex)
            {
                DateTime time = DateTime.Now;
                string retime = time.ToString("yyyy-MM-dd HH:mm:ss").Trim();
                return retime;
            }
        }

        /// <summary>
        /// 转化为yyyyMMddHHmmss时间格式
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ToTime16(this object obj, string defaultValue = "")
        {
            try
            {
                if (obj == null)
                {
                    DateTime time2 = DateTime.Now;
                    return time2.ToString("yyyyMMddHHmmss").Trim();
                }
                DateTime time = Convert.ToDateTime(obj);
                string retime = time.ToString("yyyyMMddHHmmss").Trim();
                return retime;
            }
            catch (Exception ex)
            {
                DateTime time = DateTime.Now;
                string retime = time.ToString("yyyyMMddHHmmss").Trim();
                return retime;
            }
        }

        /// <summary>
        /// 转化为字符串默认值为""
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ToStr(this object obj, string defaultValue = "")
        {
            try
            {
                if (obj == null)
                {
                    return defaultValue;
                }
                return obj.ToString().Trim();

            }
            catch (Exception ex)
            {
                return "";
            }

        }

        //效验是否为空
        public static string IfNull(this object obj, string defaultValue = "", int maxlen = 0)
        {
            string sm = defaultValue + ",不能为空!!!";
            try
            {
                if (obj == null)
                {
                    return sm;
                }
                if (obj.ToStr().Length > maxlen && maxlen!=0)
                {
                    throw new Exception(defaultValue + "超出了最大长度(" + maxlen + ")的限制！！！");
                }
                return obj.ToStr() == "" ? throw new Exception(sm) : obj.ToStr();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 转化为Base64字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ToBs64(this object obj, string defaultValue = "")
        {
            try
            {
                Encoding en = Encoding.Default;
                if (obj == null)
                {

                    string re = string.Empty;
                    byte[] bt = en.GetBytes("");
                    re = Convert.ToBase64String(bt);
                    return re;
                }

                string res = string.Empty;
                byte[] bts = en.GetBytes(obj.ToString());
                res = Convert.ToBase64String(bts);
                return res;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 转化为int
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this object obj, int defaultValue = 0)
        {
            if (obj == null)
            {
                return defaultValue;
            }

            int value;
            if (int.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 转化为Double
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble(this object obj, double defaultValue = 0)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            double value;
            if (double.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 转化为Bool
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ToBool(this object obj, bool defaultValue = false)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            if (string.IsNullOrEmpty(obj.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }

            bool value;
            if (bool.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 转化为Float
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float ToFloat(this object obj, float defaultValue = 0)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            float value;
            if (float.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 转换为decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Decimal ToDecimal(this object obj, Decimal defaultValue = 0)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            Decimal value;
            if (Decimal.TryParse(obj.ToString(), out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }



        /// <summary>
        /// 获取XmlNode的子元素的值
        /// </summary>
        /// <param name="xno">无需填写,获取自身</param>
        /// <param name="str">需要获取子元素的路径</param>
        /// <returns></returns>
        public static string GetValue(this XmlNode xno, string str)
        {
            string result = "";
            try
            {
                result = xno.SelectSingleNode(str).InnerText.ToStr();

                return result;
            }
            catch (Exception ex)
            {
                string msg = "在" + xno.Name + "节点下未找到" + str + "节点";
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 获取XmlNode的子元素的值重载
        /// </summary>
        /// <param name="xno">无需填写,获取自身</param>
        /// <param name="str">需要获取子元素的路径</param>
        /// <returns></returns>
        public static string GetValue(this XmlElement xno, string str)
        {
            string result = "";
            try
            {
                result = xno.SelectSingleNode(str).InnerText.ToStr();

                return result;
            }
            catch (Exception ex)
            {
                string msg = "在" + xno.Name + "节点下未找到" + str + "节点";
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 获取XmlNode的子元素的值重载
        /// </summary>
        /// <param name="xno">无需填写,获取自身</param>
        /// <param name="str">需要获取子元素的路径</param>
        /// <returns></returns>
        public static string GetValue(this XmlDocument xno, string str)
        {
            string result = "";
            try
            {
                result = xno.SelectSingleNode(str).InnerText.ToStr();

                return result;

            }
            catch (Exception ex)
            {
                string msg = "在" + xno.Name + "节点下未找到" + str + "节点";
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 获取Xml文档中的XmlNode节点
        /// </summary>
        /// <param name="xno">无需填写,自身</param>
        /// <param name="str">需要获取元素的路径</param>
        /// <returns></returns>
        public static XmlNode GetXno(this XmlDocument xno, string str)
        {
            XmlNode result = null;
            try
            {
                result = xno.SelectSingleNode(str);
                if (result == null)
                {
                    return null;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                string msg = "在" + xno.Name + "节点下未找到" + str + "节点";
                throw new Exception(msg);
            }
        }



        /// <summary>
        /// 获取Xml文档中的XmlNode节点
        /// </summary>
        /// <param name="xno">无需填写,自身</param>
        /// <param name="str">需要获取元素的路径</param>
        /// <returns></returns>
        public static XmlNode GetXno(this XmlNode xno, string str)
        {
            XmlNode result = null;
            try
            {
                result = xno.SelectSingleNode(str.ToStr());
                if (result == null)
                {
                    return null;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                string msg = "在" + xno.Name + "节点下未找到" + str + "节点";
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 获取Xml文档中的XmlNode节点
        /// </summary>
        /// <param name="xno">无需填写,自身</param>
        /// <param name="str">需要获取元素的路径</param>
        /// <returns></returns>
        public static XmlNode GetXno(this XmlElement xno, string str)
        {
            XmlNode result = null;
            try
            {
                result = xno.SelectSingleNode(str.ToStr());
                if (result == null)
                {
                    return null;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                string msg = "在" + xno.Name + "节点下未找到" + str + "节点";
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 设置Xml的路径下的子元素的值
        /// </summary>
        /// <param name="xno">自身</param>
        /// <param name="xelname">子元素路径</param>
        /// <param name="str">值</param>
        public static void SetValue(this XmlNode xno, string xelname, string str)
        {
            try
            {
                xno.SelectSingleNode(xelname.ToStr()).InnerText = str.ToStr();
            }
            catch (Exception ex)
            {
                string msg = "在" + xno.Name + "节点下未找到" + xelname + "节点";
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 设置Xml的路径下的子元素的值
        /// </summary>
        /// <param name="xno">自身</param>
        /// <param name="xelname">子元素路径</param>
        /// <param name="str">值</param>
        public static void SetValue(this XmlDocument xno, string xelname, string str)
        {
            try
            {
                xno.SelectSingleNode(xelname.ToStr()).InnerText = str.ToStr();
            }
            catch (Exception ex)
            {
                string msg = "在" + xno.Name + "节点下未找到" + xelname + "节点";
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 设置Xml的路径下的子元素的值
        /// </summary>
        /// <param name="xno">自身</param>
        /// <param name="xelname">子元素路径</param>
        /// <param name="str">值</param>
        public static void SetValue(this XmlElement xno, string xelname, string str)
        {
            try
            {
                xno.SelectSingleNode(xelname.ToStr()).InnerText = str.ToStr();
            }
            catch (Exception ex)
            {
                string msg = "在" + xno.Name + "节点下未找到" + xelname + "节点";
                throw new Exception(msg);
            }
        }
       public static string ToJson(this Result result)
        {
            try
            {
                return JsonConvert.SerializeObject(result);
            }
            catch
            {
                return $@"{{""Code"":""{result.Code}"",""Msg"":""{result.Msg}""}}";
            }
        }
        public static Result SyncToDbByObj(JToken obj,string tableName,List<string> specialCol,LogInfo l)
        {
    
            Result r = new Result();
            try
            {
                if (obj == null)
                    return r.SetLog(500, "传入对象为空", l);


                var jpropetys = obj.Children<JProperty>();

                StringBuilder whereStr = new StringBuilder(" where ");
                //exists和 update的条件
                foreach (var item in specialCol)
                {
                    if (whereStr.Length != 7)
                        whereStr.Append("  and  ");
                    whereStr.Append(item+"=@"+item);
                }
                if (specialCol.Count == 0)
                    whereStr = new StringBuilder();
                //StringBuilder existStr = new StringBuilder($@"If(Exists(select count(*) from {tableName} where {whereStr.ToString()} ))");

                StringBuilder upStr = new StringBuilder($"update {tableName} set ");
                StringBuilder insStr = new StringBuilder($"insert into {tableName} (");
                StringBuilder insStr_valStr = new StringBuilder();
                SqlParameter[] sqlParameters = new SqlParameter[jpropetys.Count()];
                int index_para = 0;
                foreach (var pi in jpropetys) {
                    string name = pi.Name;
                    string val = pi.Value.Value<string>();

                    insStr.Append(" "+ name + ",");
                    insStr_valStr.Append(" @"+ name + ",");
                    upStr.Append(" "+ name +"=@"+name +",");

                    sqlParameters[index_para++] =new SqlParameter(name,val);
                    
                }
                upStr = upStr.Remove(upStr.Length-1,1);
                upStr.Append(" "+whereStr.ToString());
                insStr = insStr.Remove(insStr.Length - 1, 1);
                insStr_valStr = insStr_valStr.Remove(insStr_valStr.Length - 1, 1);
                insStr.Append(")values( "+ insStr_valStr+")");
                string sql = $@"If(Exists(select count(*) from {tableName} where {whereStr.ToString()} ))
                                    begin
                                        {upStr}
                                    end
                                else
                                    begin
                                        {insStr}
                                    end
";

                l.Write("sql:"+sql);

                if (SqlHepler.ExecuteSql(sql, sqlParameters) > 0)
                    return r.SetLog(200, "成功上传" + JsonConvert.SerializeObject(sqlParameters), l);
                else
                    return r.SetLog(500, "上传失败:" + JsonConvert.SerializeObject(sqlParameters), l) ;
            }
            catch (Exception e)
            {
                return r.SetLog(500, "上传失败出现异常:"+e.ToString()+JsonConvert.SerializeObject(obj), l);
            }
        }

        public static string ParameterFormate(this string str)
        {
            return "'" + str.Replace(" ", "").Replace("，", ",").Replace(",", "','") + "'";
        }

        public static string ExFormat(this Exception ex)
        {
            string innerEx = ex.InnerException != null ? ex.InnerException.Message : "";
            return "程序内部出现异常：" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Source + "\r\n" + innerEx;
        }


    }
}
