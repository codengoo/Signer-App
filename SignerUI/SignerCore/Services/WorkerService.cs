using Grpc.Core;
using WorkerProto;

namespace SignerCore.Services
{
    public class WorkerService : Worker.WorkerBase
    {
        public override Task<WorkReply> DoWork(WorkRequest request, ServerCallContext context)
        {
            Console.WriteLine(request);
            return Task.FromResult(new WorkReply
            {
               ListCert = new ListCertReply() { Pin = "abc"}
            });
        }
    }
}