using Signer.Models;

namespace SignerAPI.Models
{
    public record DllInfo(
        string DllPath,
        Arch Arch,
        string Company,
        string Product,
        string Description
    );
}
