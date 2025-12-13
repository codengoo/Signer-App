using Grpc.Net.Client;
using System.Diagnostics;
using WorkerProto;

namespace SignerAPI.Domains.WorkerCall
{
    public class WorkerCall : IWorkerCall
    {
        private static readonly HttpClient _http = new();
        private static readonly int PORT_86 = 8686;
        private static readonly int PORT_64 = 6464;
        private readonly GrpcChannel _channel86;
        private readonly GrpcChannel _channel64;

        public WorkerCall()
        {
            var handler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true
            };

            _channel86 = GrpcChannel.ForAddress(
                $"http://localhost:{PORT_86}",
                new GrpcChannelOptions { HttpHandler = handler }
            );

            _channel64 = GrpcChannel.ForAddress(
                $"http://localhost:{PORT_64}",
                new GrpcChannelOptions { HttpHandler = handler }
            );
        }

        private static void KillProcess(Process? proc)
        {
            try
            {
                if (proc is { HasExited: false })
                    proc.Kill(entireProcessTree: true);
            }
            catch { }
        }

        private static Process StartProcess(string path, int port)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = path,
                Arguments = port.ToString(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }) ?? throw new Exception($"Cannot start process: {path}");

            return process;
        }

        private async Task CheckWorkerHealthAsync(Arch arch)
        {
            var timeoutAt = DateTime.UtcNow.AddSeconds(10);

            while (DateTime.UtcNow < timeoutAt)
            {
                try
                {
                    var result = await Call(new WorkRequest() { Task = TaskType.PingUnspecified }, arch);
                    if (result == null || !result.Success) throw new Exception(result?.ErrorMessage ?? "unknown error");
                    return;
                }
                catch
                {
                    // ignore & retry
                }

                await Task.Delay(500);
            }

            throw new Exception($"Health check timeout: " + arch.ToString());
        }

        public async Task<bool> StartCoreServiceAsync()
        {
            var filePath86 = @"D:\WORK\aaaa\out\x86\SignerCore.exe";
            var filePath64 = @"D:\WORK\aaaa\out\x64\SignerCore.exe";
            Process? proc86 = null;
            Process? proc64 = null;

            try
            {
                // nhóm process để khi main kill thì các process cũng kill
                var job = ProcessHelper.CreateJob();
                proc86 = StartProcess(filePath86, PORT_86);
                proc64 = StartProcess(filePath64, PORT_64);

                ProcessHelper.Assign(proc86, job);
                ProcessHelper.Assign(proc64, job);

                await CheckHealth();
                return true;
            }
            catch
            {
                KillProcess(proc86);
                KillProcess(proc64);
                throw;
            }
        }

        public async Task<WorkReply?> Call(WorkRequest request, Arch arch)
        {
            var channel = arch == Arch.X86 ? _channel86 : _channel64;
            var client = new Worker.WorkerClient(channel);
            return await client.DoWorkAsync(request);
        }

        public async Task<bool> CheckHealth()
        {
            await Task.WhenAll(
               CheckWorkerHealthAsync(Arch.X86),
               CheckWorkerHealthAsync(Arch.X64)
           );

            return true;
        }
    }
}
