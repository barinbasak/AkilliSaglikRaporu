using Microsoft.AspNetCore.Http;

namespace AkilliSaglikRaporu.Services
{
    public interface IIpfsService
    {
        Task<string> UploadFileToIpfsAsync(IFormFile file);
        Task<byte[]> GetFileFromIpfsAsync(string ipfsHash);
    }
} 