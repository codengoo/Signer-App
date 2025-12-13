using Signer.Models;
using WorkerProto;

namespace SignerAPI.Domains.WorkerCall
{
    public interface IWorkerCall
    {
        public Task<bool> StartCoreServiceAsync();
        public Task<WorkReply?> Call(WorkRequest request, Arch arch);
        public Task<bool> CheckHealth();

    }
}
