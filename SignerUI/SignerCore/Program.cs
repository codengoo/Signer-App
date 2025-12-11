var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<WorkerService>();

Console.WriteLine("ChildWorker gRPC server running on http://localhost:50051");
app.Run();
