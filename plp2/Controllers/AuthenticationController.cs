//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Authentication;

//namespace plp2.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AuthenticationController : ControllerBase
//    {
//        [HttpGet]
//        public IActionResult Get()
//        {
//            if (User.Identity.IsAuthenticated)
//            {
//                return Ok();
//            }
//            else
//            {
//                return Unauthorized();
//            }
//        }
//    }
//}
