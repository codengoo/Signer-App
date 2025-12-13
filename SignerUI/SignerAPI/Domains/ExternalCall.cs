using Grpc.Net.Client;
using WorkerProto;

namespace SignerAPI.Domains
{
    public class ExternalCall
    {
        static public async Task Call()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:8888");

            var client = new Worker.WorkerClient(channel);

            var reply = await client.DoWorkAsync(new WorkRequest
            {
            });

            Console.WriteLine("Response from worker:");
            Console.WriteLine(reply.Output);
        }
    }
}
