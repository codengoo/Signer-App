using WorkerProto;

namespace SignerAPI.Domains.WorkerCall
{
    public enum Arch
    {
        X86,
        X64
    }

    public interface IWorkerCall
    {
        public Task<bool> StartCoreServiceAsync();
        public Task<WorkReply?> Call(WorkRequest request, Arch arch);
        public Task<bool> CheckHealth();

    }
}
