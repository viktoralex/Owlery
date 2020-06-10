using Microsoft.AspNetCore.Mvc;

namespace Owlery.Example.Controllers
{
    [ApiController]
    [Route("Greeting")]
    public class GreetingRestController : ControllerBase
    {
        [HttpGet("Hello")]
        public GreetingResult SayHello([FromBody] GreetingRequest greetingRequest)
        {
            return new GreetingResult() {
                Greeting = $"Hello, {greetingRequest.Name}!"
            };
        }
        [HttpGet("Hi")]
        public GreetingResult SayHi([FromBody] GreetingRequest greetingRequest)
        {
            return new GreetingResult() {
                Greeting = $"Hi, {greetingRequest.Name}!"
            };
        }
    }
}