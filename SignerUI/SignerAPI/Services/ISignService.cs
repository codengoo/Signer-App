using Signer.Models;

namespace SignerAPI.Services
{
    public interface ISignService
    {
        public Task<bool> CheckHealth();
        public Task<bool> StartService();
        public Task<List<ListCertData>> ListCerts(string userPin);
        public Task<SignHashReply> SignHash(string userPin, string thumbprint, string hashToSignBase64);
        public Task<SignPdfReply> SignPdfFile(string userPin, string thumbprint, string inputPdfPath, string outputPdfPath, string signatureImage, PositionData position);
    }
}
