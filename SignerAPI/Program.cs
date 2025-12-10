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
            _ = (lifetime?.ApplicationStarted.Register(() =>
            {
                // Lấy danh sách URL từ IServerAddressesFeature
                var addresses = server.Features.Get<IServerAddressesFeature>();
                var runningUrls = addresses.Addresses;

                // Gọi Callback nếu được cung cấp
                onStartedCallback?.Invoke(runningUrls);
            }));

            app.MapGet("/", () => "Hello World!");

            return app;
        }

        public static void Main()
        {
            var app = Create();
            app.Run();
        }
    }
}
