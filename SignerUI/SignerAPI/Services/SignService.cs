using Signer.Models;
using SignerAPI.Domains.WorkerCall;
using SignerAPI.Models;
using WorkerProto;

namespace SignerAPI.Services
{
    public class SignService(IWorkerCall workerCall, IScanService scanDll) : ISignService
    {
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
            var result = await workerCall.Call(
                new WorkRequest()
                {
                    Task = TaskType.ListCerts,
                    Context = new SignerContext() { DllPath = "", Pin = userPin }
                },
                Arch.X86);

            if (result == null || !result.Success) throw new Exception(result?.ErrorMessage ?? "unknown error");
            return result.ListCert.Certs?.ToList() ?? [];
        }

        public async Task<DllInfo?> FindDll(string userPin)
        {
            var dllList = await scanDll.Scan();

            foreach (var dll in dllList)
            {
                var result1 = await workerCall.Call(
                    new WorkRequest()
                    {
                        Task = TaskType.ListCerts,
                        Context = new SignerContext() { DllPath = dll.DllPath, Pin = userPin }
                    },
                    dll.Arch);

                if (result1 != null && result1.Success) return dll;
            }

            return null;
        }

        public async Task<SignHashReply> SignHash(string userPin, string thumbprint, string hashToSignBase64)
        {
            var result = await workerCall.Call(
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
            var result = await workerCall.Call(
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
