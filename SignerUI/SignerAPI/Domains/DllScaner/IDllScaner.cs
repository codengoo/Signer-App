using SignerAPI.Models;

namespace SignerAPI.Domains.ScanDll
{
    public interface IDllScaner
    {
        public DllInfo? Dll { get; set; }
        public List<DllInfo> Scan();
    }
}
