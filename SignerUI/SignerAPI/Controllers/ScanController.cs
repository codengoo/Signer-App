using Microsoft.AspNetCore.Mvc;
using SignerAPI.Services;

namespace SignerAPI.Controllers
{
    [Route("api/scan")]
    [ApiController]
    public class ScanController(IScanService scanService) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetDll()
        {
            var results = await scanService.Scan();
            return Ok(results);
        }
    }
}
