namespace Signer.Services.FileUpload
{
    public interface IFileUpload
    {
        Task<string> SaveFileAsync(IFormFile file, string subFolder = "");
    }
}
