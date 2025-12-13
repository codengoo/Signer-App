using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using iText.Forms.Form.Element;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.Signatures;
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
                TaskType.ListCerts => HandleListCert(request.Context, request.ListCert),
                TaskType.PingUnspecified => HandlePing(),
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

                var certDataList = certs.Select(c => new ListCertData
                {
                    Label = c.Label,
                    KeyId = ByteString.CopyFrom(c.KeyId),
                    Subject = c.Subject,
                    Thumbprint = c.Thumbprint,
                    NotBefore = Timestamp.FromDateTime(c.NotBefore.ToUniversalTime()),
                    NotAfter = Timestamp.FromDateTime(c.NotAfter.ToUniversalTime()),
                    CertBase64 = c.CertBase64
                }).ToList();

                return new WorkReply
                {
                    Success = true,
                    ListCert = new ListCertReply
                    {
                        Certs = { certDataList }
                    }
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
                var signature = pkcs.SignHash(req.HashData, privateKey);

                return new WorkReply
                {
                    Success = true,
                    SignHash = new SignHashReply
                    {
                        SignatureBase64 = signature,
                        CertificateBase64 = cert.CertBase64
                    }
                };
            });
        }

        private static WorkReply HandleSignPdf(SignerContext context, SignPdfRequest req)
        {
            return ExecuteTask(() =>
            {
                using var pkcs = new PKCSSigner(context.Pin, context.DllPath);
                var cert = pkcs.GetCertByThumprint(req.Thumprint)
                           ?? throw new Exception("Cert not found");

                var position = req.Position;
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("signature-field");
                appearance.SetWidth(position.Width);
                appearance.SetHeight(position.Height);
                appearance.SetBorder(new SolidBorder(ColorConstants.DARK_GRAY, 2));
                appearance.SetContent(ImageDataFactory.Create(req.ImagePath));

                SignerProperties signerProps = new SignerProperties()
                    .SetFieldName("signature-field")
                    .SetPageRect(new Rectangle(position.PosX, position.PosY, position.Width, position.Height))
                    .SetPageNumber(position.Page)
                    .SetReason("Tôi đồng ý với nội dung tài liệu")
                    .SetLocation("Việt Nam")
                    .SetSignatureAppearance(appearance);

                pkcs.SignPdfFile(cert, req.InputPath, req.OutpuPath, signerProps);

                return new WorkReply
                {
                    Success = true,
                    SignPdf = new SignPdfReply
                    {
                        InputPath = req.InputPath,
                        OutputPath = req.OutpuPath,
                    }
                };
            });
        }

        private static WorkReply HandlePing()
        {
            return ExecuteTask(() =>
            {
                return new WorkReply
                {
                    Success = true,
                };
            });
        }
    }
}