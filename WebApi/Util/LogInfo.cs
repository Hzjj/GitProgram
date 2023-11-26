using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebService.Util
{
    /// <summary>
    /// 创建人刘鹏
    /// 创建日期:2021-5-7
    /// </summary>

    public class LogInfo
    {
        public string type = "Error";
        public LogInfo(string Types)
        {
            type = Types;
        }

        private static Dictionary<long, long> lockDic = new Dictionary<long, long>();


        private bool IsDebug()
        {
            try
            {
                string isdebug = ConfigurationManager.AppSettings["IsDebug"];
                 if(isdebug.ToLower()=="true")
                    return true;
            }
            catch
            {            }
            return false;
        }

        /// <summary>  
        /// 写入文本  
        /// </summary>  
        /// <param name="content">文本内容</param>  
        public void Write(string content)
        {
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + type + "\\";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + type + "\\[" + DateTime.Now.ToString("yyyy-MM-dd HH") + "].txt";
                content = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:fff") + "]:" + content;
                content += "\r\n";
                using (System.IO.FileStream fs = new System.IO.FileStream(logPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, 8, System.IO.FileOptions.Asynchronous))
                {
                    //Byte[] dataArray = System.Text.Encoding.ASCII.GetBytes(System.DateTime.Now.ToString() + content + "/r/n");  
                    Byte[] dataArray = System.Text.Encoding.Default.GetBytes(content);
                    bool flag = true;
                    long slen = dataArray.Length;
                    long len = 0;
                    while (flag)
                    {
                        try
                        {
                            if (len >= fs.Length)
                            {
                                fs.Lock(len, slen);
                                lockDic[len] = slen;
                                flag = false;
                            }
                            else
                            {
                                len = fs.Length;
                            }
                        }
                        catch (Exception ex)
                        {
                            while (!lockDic.ContainsKey(len))
                            {
                                len += lockDic[len];
                            }
                        }
                    }
                    fs.Seek(len, System.IO.SeekOrigin.Begin);
                    fs.Write(dataArray, 0, dataArray.Length);
                    fs.Close();
                }
        DirectoryInfo dir = new DirectoryInfo(filePath);
                FileInfo[] files = dir.GetFiles();
foreach (var file in files)//自动删除七天数据
            {
                DateTime time = DateTime.Now.AddDays(-7);
                if (file.LastWriteTime < time)
                {
                    file.Delete();
                }
            }
            }
            catch (Exception ex) { }
        }
        /// <summary>  
        /// 写入文本  
        /// </summary>  
        /// <param name="content">文本内容</param>  
        public void WriteDebug(string content)
        {
            try
            {
                if (!IsDebug())
                    return;
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + type + "\\";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + type + "\\[" + DateTime.Now.ToString("yyyy-MM-dd HH") + "].txt";
                content = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:fff") + "]:" + content;
                content += "\r\n";
                using (System.IO.FileStream fs = new System.IO.FileStream(logPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, 8, System.IO.FileOptions.Asynchronous))
                {
                    //Byte[] dataArray = System.Text.Encoding.ASCII.GetBytes(System.DateTime.Now.ToString() + content + "/r/n");  
                    Byte[] dataArray = System.Text.Encoding.Default.GetBytes(content);
                    bool flag = true;
                    long slen = dataArray.Length;
                    long len = 0;
                    while (flag)
                    {
                        try
                        {
                            if (len >= fs.Length)
                            {
                                fs.Lock(len, slen);
                                lockDic[len] = slen;
                                flag = false;
                            }
                            else
                            {
                                len = fs.Length;
                            }
                        }
                        catch (Exception ex)
                        {
                            while (!lockDic.ContainsKey(len))
                            {
                                len += lockDic[len];
                            }
                        }
                    }
                    fs.Seek(len, System.IO.SeekOrigin.Begin);
                    fs.Write(dataArray, 0, dataArray.Length);
                    fs.Close();
                }
                DirectoryInfo dir = new DirectoryInfo(filePath);
                FileInfo[] files = dir.GetFiles();
                foreach (var file in files)//自动删除七天数据
                {
                    DateTime time = DateTime.Now.AddDays(-7);
                    if (file.LastWriteTime < time)
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ex) { }
        }

        public void WriteError(string content)
        {
            Write(content);
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log\\Error\\";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\Error\\[" + DateTime.Now.ToString("yyyy-MM-dd HH") + "].txt";
                content = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:fff") + "]:" + content;
                content += "\r\n";
                using (System.IO.FileStream fs = new System.IO.FileStream(logPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, 8, System.IO.FileOptions.Asynchronous))
                {
                    //Byte[] dataArray = System.Text.Encoding.ASCII.GetBytes(System.DateTime.Now.ToString() + content + "/r/n");  
                    Byte[] dataArray = System.Text.Encoding.Default.GetBytes(content);
                    bool flag = true;
                    long slen = dataArray.Length;
                    long len = 0;
                    while (flag)
                    {
                        try
                        {
                            if (len >= fs.Length)
                            {
                                fs.Lock(len, slen);
                                lockDic[len] = slen;
                                flag = false;
                            }
                            else
                            {
                                len = fs.Length;
                            }
                        }
                        catch (Exception ex)
                        {
                            while (!lockDic.ContainsKey(len))
                            {
                                len += lockDic[len];
                            }
                        }
                    }
                    fs.Seek(len, System.IO.SeekOrigin.Begin);
                    fs.Write(dataArray, 0, dataArray.Length);
                    fs.Close();
                }
                DirectoryInfo dir = new DirectoryInfo(filePath);
                FileInfo[] files = dir.GetFiles();
                foreach (var file in files)//自动删除七天数据
                {
                    DateTime time = DateTime.Now.AddDays(-7);
                    if (file.LastWriteTime < time)
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ex) { }
        }
    }

}