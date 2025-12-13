using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using SignerAPI.Domains.WorkerCall;
using SignerAPI.Middlewares;
using SignerAPI.Services;

namespace SignerAPI
{
    public static class ApiHost
    {
        public static WebApplication Create(Action<ICollection<string>>? onStartedCallback = null)
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddControllers();
            builder.Services.AddScoped<IWorkerCall, WorkerCall>();
            builder.Services.AddScoped<ISignService, SignService>();

            var app = builder.Build();

            var lifetime = app.Services.GetService<IHostApplicationLifetime>();
            var server = app.Services.GetService<IServer>();

            // Đăng ký Callback
            lifetime?.ApplicationStarted.Register(() =>
            {
                var addresses = server.Features.Get<IServerAddressesFeature>();
                var runningUrls = addresses.Addresses;

                onStartedCallback?.Invoke(runningUrls);
            });

            //app.MapGet("/", () => "Hello World!");
            //app.MapGet("/test", () => WorkerCall.Call());
            //app.MapGet("/start-core-service", () => WorkerCall.StartCoreServiceAsync());

            app.UseMiddleware<ApiExceptionMiddleware>();
            app.MapControllers();

            return app;
        }

        public static bool StopApp(WebApplication app, Task? runTask)
        {
            var lifetime = app.Services.GetService<IHostApplicationLifetime>();
            lifetime?.StopApplication();

            return runTask == null || runTask.Wait(TimeSpan.FromSeconds(5));
        }

        public static void Main()
        {
            var app = Create();
            app.Run();
        }
    }
}
