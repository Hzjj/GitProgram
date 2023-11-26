using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.Util
{
    /// <summary>
    /// HL7解析类
    /// </summary>
    public class HL7
    {
        /// <summary>
        /// 设置HL7的值
        /// </summary>
        /// <param name="strInput">HL7段落,例子:OBR|0|0</param>
        /// <param name="key">标头,例子:OBR</param>
        /// <param name="position">需要设置的位数</param>
        /// <param name="replceStr">设置的字符</param>
        /// <returns></returns>
        public string SetValue(string strInput, string key, int position, string replceStr)
        {

            var strInputList = strInput.Split('|');
            var index = 0;
            var resultStr = "";
            int k = 0;
            foreach (var i in strInputList)
            {
                if (i.Contains(key))
                {
                    int o = strInput.Length;
                    k = index + position;

                }
                if (index == k && k != 0)
                {
                    resultStr += replceStr + "|";
                }
                else
                {
                    resultStr += i + "|";
                }
                index++;
            }
            if (resultStr.Length > 1)
            {
                resultStr = resultStr.Substring(0, resultStr.Length - 1);
            }
            return resultStr;
        }

        /// <summary>
        /// 根据提供的KEY和位置，来解析HL7数据，获取相关的值
        /// </summary>
        /// <param name="strInput">HL7数据</param>
        /// <param name="key">相关的值名称</param>
        /// <param name="postion">相关的值的位置</param>
        /// <returns></returns>
        public string GetValue(string strInput, string key, int postion)
        {
            string val = string.Empty;
            string kls = strInput;
            int a = kls.IndexOf(key);
            kls = kls.Remove(0, a);
            string[] ss = kls.Split(new char[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (ss.Count() > 0)
            {
                foreach (var item in ss)
                {
                    if (item.Trim().StartsWith(key + "|"))
                    {
                        string[] djlshArr = item.Split('|');
                        if (djlshArr.Count() > postion)
                        {
                            val = djlshArr[postion];
                            break;
                        }
                    }
                }
            }
            else
            {
                if (strInput.StartsWith(key + "|"))
                {
                    string[] djlshArr = strInput.Split('|');
                    if (djlshArr.Count() > postion)
                    {
                        val = djlshArr[postion];
                    }
                }

            }

            return val;
        }

        /// <summary>
        /// 将重复的标头,将其分隔成泛型数组 比如有多个OBR医嘱结点可以通过这个来获取结果
        /// </summary>
        /// <param name="strInput">HL7段落</param>
        /// <param name="key">标头</param>
        /// <returns></returns>
        public List<string> HQXH(string strInput, string key)
        {
            List<string> vs = new List<string>();
            string val = string.Empty;
            string[] ss = strInput.Split(new char[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in ss)
            {
                if (item.Trim().StartsWith(key + "|"))
                {
                    vs.Add(item);
                }
            }
            return vs;
        }
    }
}