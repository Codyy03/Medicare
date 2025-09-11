using Microsoft.AspNetCore.Mvc;
using MediCare.Server.Helpers;

namespace MediCare.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToolsController : ControllerBase
    {
        [HttpGet("hash/{password}")]
        public IActionResult GetHash(string password)
        {
            var hash = CreatePasswordHash.CreateHash(password);
            return Ok(hash);
        }
    }
}
