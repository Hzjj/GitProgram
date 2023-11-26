using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebService.Util
{
    public class ReturnData
    {
        public string Data { get; set; }
        public int Code { get; set; } = 400;
        public string Message { get; set; }
        public string returnResult(string message, LogInfo log, int code = 400, string data = "")
        {
            Data = data; Code = code; Data = data;
            string str = JsonConvert.SerializeObject(this);
            log.Write(str);
            return str;
        }
    }

}