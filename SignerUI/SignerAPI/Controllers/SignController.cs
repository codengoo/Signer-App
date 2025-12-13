using Microsoft.AspNetCore.Mvc;
using SignerAPI.Dto;
using SignerAPI.Services;

namespace SignerAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class SignController(ISignService signService, IFileUpload fileUpload, IHostEnvironment env) : ControllerBase
    {
        private readonly ISignService _signService = signService;

        [HttpGet("")]
        public IActionResult GetHealth()
        {
            var isOke = _signService.CheckHealth();
            return Ok(isOke);
        }

        [HttpGet("/health")]
        public IActionResult GetHealth2()
        {
            var isOke = _signService.CheckHealth();
            return Ok(isOke);
        }

        [HttpGet("certs")]
        public IActionResult ListCert([FromQuery] CertQuery query)
        {
            var data = _signService.ListCerts(query.Pin);
            return Ok(data);
        }

        [HttpPost("sign")]
        public IActionResult SignHash([FromBody] SignBody body)
        {
            var data = _signService.SignHash(body.Pin, body.Thumbprint, body.HashToSignBase64);
            return Ok(data);
        }

        [HttpPost("sign-pdf-file")]
        public async Task<IActionResult> SignFile([FromForm] SignFileForm form)
        {
            var outputRoot = Path.Combine(env.ContentRootPath, "outputs");
            Directory.CreateDirectory(outputRoot);

            var inputPdfPath = await fileUpload.SaveFileAsync(form.File, "doc");
            var inputImagePath = await fileUpload.SaveFileAsync(form.Image, "image");
            var outputPdfPath = Path.Combine(outputRoot, Guid.NewGuid() + "_signed.pdf").Replace("\\", "/");

            _signService.SignPdfFile(
                form.Pin, form.Thumbprint, inputPdfPath, outputPdfPath, inputImagePath,
                new PositionData()
                {
                    Height = form.Height,
                    Width = form.Width,
                    PosX = form.PosX,
                    PosY = form.PosY,
                    Page = form.Page,
                }
            );

            return File(
                new FileStream(outputPdfPath, FileMode.Open, FileAccess.Read),
                "application/pdf",
                Path.GetFileName(outputPdfPath),
                enableRangeProcessing: true
            );
        }
    }
}
