namespace AkilliSaglikRaporu.Services
{
    public interface ISmartContractService
    {
        Task<bool> StoreHealthReportAsync(string userAddress, string ipfsHash, string doctorAddress, DateTime expiryDate);
        Task<string> GetHealthReportAsync(string userAddress, string doctorAddress);
        Task<bool> IsValidAccessAsync(string doctorAddress, string ipfsHash);
        Task<string> GetUserAddressFromMetaMaskAsync();
    }
} 