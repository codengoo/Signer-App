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
        private Process? proc86 = null;
        private Process? proc64 = null;

        private static async Task CheckWorkerHealthAsync(Process proc, string url)
        {
            var timeoutAt = DateTime.UtcNow.AddSeconds(10);

            while (DateTime.UtcNow < timeoutAt)
            {
                if (proc.HasExited)
                    throw new Exception($"Worker exited early: {url}");

                try
                {
                    var resp = await _http.GetAsync(url);
                    if (resp.IsSuccessStatusCode)
                        return;
                }
                catch
                {
                    // ignore & retry
                }

                await Task.Delay(500);
            }

            throw new Exception($"Health check timeout: {url}");
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

        public async Task<bool> StartCoreServiceAsync()
        {
            var filePath86 = @"D:\WORK\aaaa\out\x86\SignerCore.exe";
            var filePath64 = @"D:\WORK\aaaa\out\x86\SignerCore.exe";

            try
            {
                proc86 = StartProcess(filePath86, PORT_86);
                proc64 = StartProcess(filePath64, PORT_64);

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
            using var channel = GrpcChannel.ForAddress(arch == Arch.X86 ? $"https://localhost:{PORT_86}" : $"https://localhost:{PORT_86}");
            var client = new Worker.WorkerClient(channel);
            return await client.DoWorkAsync(request);
        }

        public async Task<bool> CheckHealth()
        {

            if (proc86 == null || proc64 == null)
                throw new Exception("Failed to start one or both processes.");

            await Task.WhenAll(
               CheckWorkerHealthAsync(proc86, $"http://localhost:{PORT_86}/health"),
               CheckWorkerHealthAsync(proc64, $"http://localhost:{PORT_64}/health")
           );

            return true;
        }
    }
}
