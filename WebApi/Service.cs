using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using WebService;
using WebService.Util;

namespace WebApi
{
    public class Service
    {

        public string GetUserInfo(string djlsh)
        {
            LogInfo l = new LogInfo("返回用户信息");
            Result r = new Result();
            string res = @"{
""status"":{0},
""name"": ""{1}"",
""sex"": ""{2}"",
""age"": ""{3}"",
""height"": ""{4}"",
""weight"": ""{5}"",
""identify"": ""{6}""
}";
            try
            {
                l.Write("记录入参:"+ djlsh);
                Data data = new Data();
                var row = data.GetGrxx("", 0, djlsh).IfNull("用户信息").Rows[0];

                string name = row["XM"].ToStr();
                string sex = row["sex"].ToStr();
                string age = row["NL"].ToStr();
                string height = "0";//row["NL"].ToStr();
                string weight = "0";// row["NL"].ToStr();
                string identify = row["SFZH"].ToStr();

                return string.Format(res,"0", name, sex, age, height, weight, identify);
            }
            catch (Exception e)
            {
                l.Write("查询用户信息接口发生异常："+e.ExFormat());
                return string.Format(res, "-1", "", "", "", "", "", "");
            }
        }

        public Result GetFGNResult(string jsonData)
        {
            LogInfo l = new LogInfo("获取肺功能结果");
            Result r = new Result();
            string djlsh = string.Empty;
            string tplj = string.Empty;
            string jg = string.Empty;
            string base64Str = string.Empty;
            string date = string.Empty;
            string shr=string.Empty;    
            try
            {
                #region 获取检测数据
                JObject job;
                try
                {
                    job = JObject.Parse(jsonData);
                    djlsh = job["scanId"].ToStr();
                    jg = job["judge"].ToStr();
                    base64Str = job["reportImg"].ToStr();
                    date = job["date"].ToTime();
                }
                catch (Exception e)
                {
                    return r.SetLog(500, "传入的非Json字符串,无法解析："+e.ExFormat(), l);
                }

                string dicOfMCAndZHXMBH_Str = ConfigurationManager.AppSettings["MCAndZHXMBH"];
                string valKey= ConfigurationManager.AppSettings["valKey"];
                shr = ConfigurationManager.AppSettings["shr"];
                if (string.IsNullOrEmpty(dicOfMCAndZHXMBH_Str))
                    return r.SetLog(500, "配置文件节点【MCAndZHXMBH】未设置，无法进一步操作，退出", l);

                Dictionary<string, string> dicOfMCAndZHXMBH = new Dictionary<string, string>();
                try
                {
                    dicOfMCAndZHXMBH = JsonConvert.DeserializeObject<Dictionary<string, string>>(dicOfMCAndZHXMBH_Str);
                }
                catch (Exception e)
                {
                    return r.SetLog(500, "解析配置文件节点【MCAndZHXMBH】失败，请检查格式:" + e.ExFormat(), l);
                }

                //具体明细项的名称和取值
                Dictionary<string, string> mcAndVal = new Dictionary<string, string>();
                foreach (var item in dicOfMCAndZHXMBH)
                {
                    string val = job[item.Key][valKey].Value<string>();
                    if (string.IsNullOrWhiteSpace(val))
                        l.Write("项目：" + item.Key + "的值为空");
                    mcAndVal.Add(item.Key, val);
                }
                l.Write("mcAndVal.Count:" + mcAndVal.Count);

                //保存图片
                tplj = SaveTp(djlsh, base64Str, l);
                if (string.IsNullOrEmpty(tplj))
                    return r.SetLog(500, "保存图片失败，此次保存结果失败", l);
                #endregion

                #region 上传数据库
                Data data = new Data();
                int sucCount = 0;
                List<string> failStr = new List<string>();
                List<Data.LisReport> reports = new List<Data.LisReport>();
                foreach (var item in mcAndVal)
                {
                    string xmmc = item.Key;
                    string xmbh = dicOfMCAndZHXMBH[xmmc];
                    string val = item.Value;
                    string shrqStr =date;
                    DateTime shrq;
                    if (!DateTime.TryParse(shrqStr, out shrq))
                    {
                        l.Write("检查时间格式转换失败" + shrqStr + ",默认采用当前日期");
                        shrq = DateTime.Now;
                    }

                    Data.LisReport lis = new Data.LisReport();
                    lis.djlsh = djlsh;
                    lis.shr = shr;
                    lis.shrq = shrq;
                    lis.czy = ConfigurationManager.AppSettings["czy"];
                    lis.zhxmbh = ConfigurationManager.AppSettings["zhxmbh"];
                    lis.zhxmmc = ConfigurationManager.AppSettings["zhxmmc"];
                    lis.xmbh = xmbh;
                    lis.xmmc = xmmc;
                    lis.jg = val;

                    r = data.UploadOneLisReprot(lis, l);
                    if (r.Code == 200)
                        sucCount++;
                    else
                        failStr.Add(lis.xmmc + ":" + val);
                }

                if (sucCount == mcAndVal.Count)
                    return r.SetLog(200, "全部获取成功", l);
                else
                    return r.SetLog(500, "成功数：" + sucCount + "失败数："
                        + failStr.Count + "失败项目：" + string.Join(",", failStr), l);
                #endregion
            }
            catch (Exception e)
            {
                return r.SetLog(500,e.ExFormat(), l);
            }
        }
        private string SaveTp(string djlsh,string base64Str,LogInfo l)
        {
            try
            {
                string baseSavePath = ConfigurationManager.AppSettings["baseSavePath"];
                if (!Directory.Exists(baseSavePath))
                    throw new Exception("配置的文件夹不存在，请确认地址是否正确或文件夹是否创建");
                string path=Path.Combine(baseSavePath,DateTime.Now.ToString("yyyy-MM-dd"),djlsh+".JPG");
                while (File.Exists(path))
                    path.Replace(".JPG", "(1).JPG");
                byte[] bytes = Convert.FromBase64String(base64Str);
                File.WriteAllBytes(path, bytes);
                return path;
            }
            catch (Exception e)
            {
                l.Write("保存图片的时候出现异常："+e.ExFormat());
                return "";
            }
        }

    }
}