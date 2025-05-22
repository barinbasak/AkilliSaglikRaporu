using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace AkilliSaglikRaporu.Services
{
    public class IpfsService : IIpfsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IpfsService> _logger;
        private readonly string _ipfsApiUrl;

        public IpfsService(HttpClient httpClient, ILogger<IpfsService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _ipfsApiUrl = configuration["IpfsSettings:ApiUrl"] ?? "https://ipfs.infura.io:5001";
        }

        public async Task<string> UploadFileToIpfsAsync(IFormFile file)
        {
            try
            {
                // Bu implementasyon basit bir örnek - gerçek uygulamada IPFS node'una uygun API çağrıları yapılmalı
                // Normalde Infura, Pinata gibi bir IPFS gateway kullanılabilir
                
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(memoryStream);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                content.Add(fileContent, "file", file.FileName);

                var response = await _httpClient.PostAsync($"{_ipfsApiUrl}/api/v0/add", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                // Gerçek implementasyonda bu JSON parse edilmeli ve hash değeri alınmalı
                return $"QmSimulatedHash{Guid.NewGuid().ToString().Substring(0, 8)}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IPFS'e dosya yüklenirken hata oluştu");
                throw;
            }
        }

        public async Task<byte[]> GetFileFromIpfsAsync(string ipfsHash)
        {
            try
            {
                // Gerçek uygulamada IPFS node'una uygun API çağrıları yapılmalı
                var response = await _httpClient.GetAsync($"{_ipfsApiUrl}/api/v0/cat?arg={ipfsHash}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"IPFS'ten dosya alınırken hata oluştu. Hash: {ipfsHash}");
                throw;
            }
        }
    }
} 