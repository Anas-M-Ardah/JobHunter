using JobHunter.Models;

namespace JobHunter.Repositories
{
    public interface IAdminRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<int> GetTotalResumesAsync();
        Task<int> GetTotalPortfoliosAsync();

    }
}
