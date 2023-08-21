using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASPProject.Controllers
{
    [Route("api/back")]
    [ApiController]
    public class BackController : ControllerBase
    {
        [HttpGet]
        public object Get([FromQuery] int x, [FromQuery] int y)
        {
            if(Request.Query.ContainsKey("y"))
            {
                return new
                {
                    message = "hello from get method",
                    x,
                    y,
                    my = Request.Headers["My-Header"]
                };
            }
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return "\"Parameter 'y' required\"";
           
        }
        [HttpPost]
        public object Post(dynamic body) 
        {

            return new
            {
                message = "hello from post method",
                body
            };
        }


        [HttpPut]
        public object Put(dynamic x)
        {
            return new
            {
                message = "hello from put method",
                x
            };
        }
        [HttpDelete]
        public object Delete([FromQuery] int x)
        {
            return new
            {
                message = "hello from delete method",
                x
            };
        }
    }
    public class PostBody
    {
        public String Data { get; set; }
    }
}


