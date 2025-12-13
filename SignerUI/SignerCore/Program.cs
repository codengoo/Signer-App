using Microsoft.AspNetCore.Server.Kestrel.Core;
using SignerCore.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(8888, listen =>
    {
        listen.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddGrpc();
var app = builder.Build();

app.MapGrpcService<WorkerService>();
app.Run();
