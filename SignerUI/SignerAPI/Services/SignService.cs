using Signer.Models;
using SignerAPI.Domains.WorkerCall;
using System.Threading.Tasks;
using WorkerProto;

namespace SignerAPI.Services
{
    public class SignService(IWorkerCall workerCall) : ISignService
    {
        private readonly IWorkerCall _workerCall = workerCall;

        public async Task<bool> CheckHealth()
        {
            var isOke = await _workerCall.CheckHealth();
            return isOke;
        }

        public async Task<List<ListCertData>> ListCerts(string userPin)
        {
            var result = await _workerCall.Call(
                new WorkRequest()
                {
                    Task = TaskType.ListCertsUnspecified,
                    Context = new SignerContext() { DllPath = "", Pin = userPin }
                },
                Arch.X86);

            if (result == null || !result.Success) throw new Exception(result?.ErrorMessage ?? "unknown error");
            return result.ListCert.Certs?.ToList() ?? [];
        }

        public async Task<SignHashReply> SignHash(string userPin, string thumbprint, string hashToSignBase64)
        {
            var result = await _workerCall.Call(
               new WorkRequest()
               {
                   Task = TaskType.SignHash,
                   Context = new SignerContext() { DllPath = "", Pin = userPin },
                   SignHash = new SignHashRequest() { HashData = hashToSignBase64, Thumprint = thumbprint }
               },
               Arch.X86);

            if (result == null || !result.Success) throw new Exception(result?.ErrorMessage ?? "unknown error");
            return result.SignHash;
        }

        public async Task<SignPdfReply> SignPdfFile(string userPin, string thumbprint, string inputPdfPath, string outputPdfPath, string signatureImage, PositionData position)
        {
            var result = await _workerCall.Call(
              new WorkRequest()
              {
                  Task = TaskType.SignPdf,
                  Context = new SignerContext() { DllPath = "", Pin = userPin },
                  SignPdf = new SignPdfRequest()
                  {
                      ImagePath = signatureImage,
                      InputPath = inputPdfPath,
                      OutpuPath = outputPdfPath,
                      Position = position
                  }
              },
              Arch.X86);

            if (result == null || !result.Success) throw new Exception(result?.ErrorMessage ?? "unknown error");
            return result.SignPdf;
        }
    }
}
