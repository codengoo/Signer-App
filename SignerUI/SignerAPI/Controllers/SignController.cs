using Microsoft.AspNetCore.Mvc;
using Signer.Services.FileUpload;
using SignerAPI.Dto;
using SignerAPI.Models;
using SignerAPI.Services;

namespace SignerAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class SignController(ISignService signService, IScanService scanService, IFileUpload fileUpload, IHostEnvironment env) : ControllerBase
    {
        private List<DllInfo> dllList = [];

        [HttpGet("")]
        public async Task<IActionResult> GetHealth()
        {
            var isOke = await signService.CheckHealth();
            return Ok(isOke);
        }

        [HttpGet("start")]
        public async Task<IActionResult> StartService()
        {
            var isOke = await signService.StartService();
            return Ok(isOke);
        }

        [HttpGet("certs")]
        public async Task<IActionResult> ListCert([FromQuery] CertQuery query)
        {
            var data = await signService.ListCerts(query.Pin);
            return Ok(data);
        }

        [HttpGet("dll")]
        public async Task<IActionResult> ListAllCert([FromQuery] CertQuery query)
        {
            var dll = await signService.FindDll(query.Pin);
            return Ok(dll);
        }

        [HttpPost("sign")]
        public async Task<IActionResult> SignHash([FromBody] SignBody body)
        {
            var data = await signService.SignHash(body.Pin, body.Thumbprint, body.HashToSignBase64);
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

            await signService.SignPdfFile(
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
