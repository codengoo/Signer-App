using Grpc.Core;
using WorkerProto;

public class WorkerService : Worker.WorkerBase
{
    public override Task<WorkReply> DoWork(WorkRequest request, ServerCallContext context)
    {
        return Task.FromResult(new WorkReply
        {
            Output = $"Child process received: {request.Input}"
        });
    }
}
