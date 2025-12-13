using Signer.Models;
using SignerAPI.Domains.ScanDll;
using SignerAPI.Domains.WorkerCall;
using SignerAPI.Models;
using WorkerProto;

namespace SignerAPI.Services
{
    public class SignService(IWorkerCall workerCall, IDllScaner scanDll) : ISignService
    {
        private async Task SetupDll(string userPin)
        {
            var dllList = scanDll.Scan();

            foreach (var dll in dllList)
            {
                var result = await workerCall.Call(
                    new WorkRequest()
                    {
                        Task = TaskType.ListCerts,
                        Context = new SignerContext() { DllPath = dll.DllPath, Pin = userPin }
                    },
                    dll.Arch);

                if (result != null && result.Success)
                {
                    scanDll.Dll = dll;
                    return;
                }
            }

            throw new Exception("Provider not found with this pin");
        }

        public async Task<bool> CheckHealth()
        {
            var isOke = await workerCall.CheckHealth();
            return isOke;
        }

        public async Task<bool> StartService()
        {
            var isOke = await workerCall.StartCoreServiceAsync();
            return isOke;
        }

        public async Task<List<ListCertData>> ListCerts(string userPin)
        {
            if (scanDll.Dll == null) await SetupDll(userPin);

            var result = await workerCall.Call(
                new WorkRequest()
                {
                    Task = TaskType.ListCerts,
                    Context = new SignerContext() { DllPath = scanDll.Dll!.DllPath, Pin = userPin }
                },
                scanDll.Dll.Arch);

            if (result == null || !result.Success) throw new Exception(result?.ErrorMessage ?? "unknown error");
            return result.ListCert.Certs?.ToList() ?? [];
        }

        public async Task<SignHashReply> SignHash(string userPin, string thumbprint, string hashToSignBase64)
        {
            if (scanDll.Dll == null) await SetupDll(userPin);

            var result = await workerCall.Call(
               new WorkRequest()
               {
                   Task = TaskType.SignHash,
                   Context = new SignerContext() { DllPath = scanDll.Dll!.DllPath, Pin = userPin },
                   SignHash = new SignHashRequest() { HashData = hashToSignBase64, Thumprint = thumbprint }
               },
               scanDll.Dll.Arch);

            if (result == null || !result.Success) throw new Exception(result?.ErrorMessage ?? "unknown error");
            return result.SignHash;
        }

        public async Task<SignPdfReply> SignPdfFile(string userPin, string thumbprint, string inputPdfPath, string outputPdfPath, string signatureImage, PositionData position)
        {
            if (scanDll.Dll == null) await SetupDll(userPin);

            var result = await workerCall.Call(
              new WorkRequest()
              {
                  Task = TaskType.SignPdf,
                  Context = new SignerContext() { DllPath = scanDll.Dll!.DllPath, Pin = userPin },
                  SignPdf = new SignPdfRequest()
                  {
                      ImagePath = signatureImage,
                      InputPath = inputPdfPath,
                      OutpuPath = outputPdfPath,
                      Position = position
                  }
              },
              scanDll.Dll.Arch);

            if (result == null || !result.Success) throw new Exception(result?.ErrorMessage ?? "unknown error");
            return result.SignPdf;
        }
    }
}
