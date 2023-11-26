using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService;
using WebService.Util;

namespace WebApi.Controllers
{
    /// <summary>
    /// add by zy  肺功能接口
    /// </summary>
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scanId"></param>
        /// <returns></returns>
        public string GetUserInfo(string scanId)
        {
            return new Service().GetUserInfo(scanId);   
        }
        
         public string FGNResultBack(string json)
        {
            return new Service().GetFGNResult(json).ToJson();   
        }
    }
}
