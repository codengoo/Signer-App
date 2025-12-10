using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace SignerAPI
{
    public static class ApiHost
    {
        public static WebApplication Create(Action<ICollection<string>>? onStartedCallback = null)
        {
            var builder = WebApplication.CreateBuilder();
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



            app.MapGet("/", () => "Hello World!");

            return app;
        }

        public static bool StopApp(WebApplication app, Task runTask)
        {
            try
            {
                var lifetime = app.Services.GetService<IHostApplicationLifetime>();
                lifetime?.StopApplication();

                return runTask.Wait(TimeSpan.FromSeconds(5));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void Main()
        {
            var app = Create();
            app.Run();
        }
    }
}
