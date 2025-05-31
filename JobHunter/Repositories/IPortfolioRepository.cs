using JobHunter.DTOs;
using JobHunter.Models;

namespace JobHunter.Repositories
{
    public interface IPortfolioRepository
    {
        public bool CreatePortfolio(PortfolioCreateEditDTO portfolioCreateDTO, User user);
        public byte[] ConvertFileToBytes(IFormFile file);
    }
}
