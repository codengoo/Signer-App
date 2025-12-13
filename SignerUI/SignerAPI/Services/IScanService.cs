using SignerAPI.Models;

namespace SignerAPI.Services
{
    public interface IScanService
    {
        public Task<List<DllInfo>> Scan();
    }
}
