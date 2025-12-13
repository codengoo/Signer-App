using Microsoft.AspNetCore.Server.Kestrel.Core;
using SignerCore.Services;

int port = (args.Length > 0 && int.TryParse(args[0], out var p)) ? p : 8888;
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(port, listen =>
    {
        listen.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

builder.Services.AddGrpc();
var app = builder.Build();

app.MapGrpcService<WorkerService>();
app.MapGet("/health", () => "Ok");
app.Run();
