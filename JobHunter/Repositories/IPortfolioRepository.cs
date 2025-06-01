using JobHunter.DTOs;
using JobHunter.Models;

namespace JobHunter.Repositories
{
    public interface IPortfolioRepository
    {
        public Task<bool> CreatePortfolioAsync(PortfolioCreateEditDTO portfolioCreateDTO, User user);
        public Task<List<PortfolioIndexDTO>> GetAllPortfoliosByUserId(User user);
        public Task<PortfolioCreateEditDTO> GetPortfolioById(Guid portfolioId);
        public Task<bool> UpdatePortfolioAsync(PortfolioCreateEditDTO portfolioCreateDTO, User user);
        public byte[] ConvertFileToBytes(IFormFile file);
    }
}
