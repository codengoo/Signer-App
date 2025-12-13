using SignerAPI.Domains.ScanDll;
using SignerAPI.Models;

namespace SignerAPI.Services
{
    public class ScanService(IDllScaner dllScaner) : IScanService
    {
        public List<DllInfo> ListDll()
        {
            var results = dllScaner.Scan();
            return results;
        }
    }
}
