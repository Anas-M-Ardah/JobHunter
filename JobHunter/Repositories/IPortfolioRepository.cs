using JobHunter.DTOs;
using JobHunter.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobHunter.Repositories
{
    public interface IPortfolioRepository
    {
        public Task<bool> CreatePortfolioAsync(PortfolioCreateEditDTO portfolioCreateDTO, User user);
        public Task<List<PortfolioIndexDTO>> GetAllPortfoliosByUserId(User user);
        public Task<PortfolioCreateEditDTO> GetPortfolioByIdForEdit(Guid portfolioId);
        public Task<Portfolio> GetPortfolioById(Guid portfolioId);
        public Task<bool> UpdatePortfolioAsync(PortfolioCreateEditDTO portfolioCreateDTO, User user);
        public byte[] ConvertFileToBytes(IFormFile file);
        public Task<FileStreamResult> GetFileFromDatabaseAsync(Guid projectId);

        public Task<bool> DeletePortfolioAsync(Guid portfolioId);
    }
}
