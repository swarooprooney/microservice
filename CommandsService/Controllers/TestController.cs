using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpPost]
        [Route("TestConnection")]
        public ActionResult TestConnection()
        {
            Console.WriteLine("Inbound connection succeeded");
            return Ok("Inbound connection succeded");
        }
    }
}