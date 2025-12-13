using Grpc.Core;
using Signer.Models;
using SignerCore.Domains;
using WorkerProto;

namespace SignerCore.Services
{
    public class WorkerService : Worker.WorkerBase
    {
        public override Task<WorkReply> DoWork(WorkRequest request, ServerCallContext context)
        {
            Console.WriteLine(request);

            WorkReply reply = request.Task switch
            {
                TaskType.SignHash => HandleSignHash(request.Context, request.SignHash),
                TaskType.SignPdf => HandleSignPdf(request.Context, request.SignPdf),
                TaskType.ListCertsUnspecified => HandleListCert(request.Context, request.ListCert),
                _ => throw new RpcException(new Status(StatusCode.InvalidArgument, "Unknown task"))
            };

            return Task.FromResult(reply);
        }

        private static WorkReply ExecuteTask(Func<WorkReply> taskFunc)
        {
            try
            {
                return taskFunc();
            }
            catch (Exception ex)
            {
                return new WorkReply
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }


        private static WorkReply HandleListCert(SignerContext context, ListCertRequest req)
        {
            return ExecuteTask(() =>
            {
                using var pkcs = new PKCSSigner(context.Pin, context.DllPath);
                var certs = pkcs.ListCerts();

                return new WorkReply
                {
                    ListCert = new ListCertReply { Pin = "123" }
                    // TODO: map certs to ListCertReply fields
                };
            });
        }

        private static WorkReply HandleSignHash(SignerContext context, SignHashRequest req)
        {
            return ExecuteTask(() =>
            {
                using var pkcs = new PKCSSigner(context.Pin, context.DllPath);
                var cert = pkcs.GetCertByThumprint(req.Thumprint)
                            ?? throw new Exception("Cert not found");
                var privateKey = pkcs.GetPrivateKey(cert.KeyId)
                            ?? throw new Exception("Private key not found");

                return new WorkReply
                {
                    ListCert = new ListCertReply { Pin = "123" }
                    // TODO: map certs to ListCertReply fields
                };
            });
        }

        private static WorkReply HandleSignPdf(SignerContext context, SignPdfRequest req)
        {
            return ExecuteTask(() =>
            {
                using var pkcs = new PKCSSigner(context.Pin, context.DllPath);
                var certs = pkcs.ListCerts();

                return new WorkReply
                {
                    ListCert = new ListCertReply { Pin = "123" }
                    // TODO: map certs to ListCertReply fields
                };
            });
        }

    }
}