using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASPProject.Controllers
{
    [Route("api/rate")]
    [ApiController]
    public class RateController : ControllerBase
    {
        [HttpGet]
        public object DoGet()
        {
            return new { message = "Hello from GET" };
        }
        [HttpPost]
        public IEnumerable<String> DoPost()
        {
            return new String[]
            {
                "Hello",
                "from",
                "POST"
            };
        }
        [HttpPut]
        public object DoPut([FromBody] dynamic body)
        {
            return body;
        }
        [HttpDelete]
        public object DoDelete([FromQuery] String id)
        {
            return new { id };
        }
        public object DoDefault()
        {
            return this.Request.Method switch
            {
                "LINK" => DoLink(),
                _ => new { method = this.Request.Method }
            };
               
        }
        private object DoLink()
        {
            return new { link = true };
        }
    }
}
