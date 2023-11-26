using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.Util
{
    public class Result
    {
        public int Code { get; set; } = 200;

        public string Msg { get; set; } = "成功";
    }
}