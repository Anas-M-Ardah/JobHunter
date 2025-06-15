using JobHunter.Data;
using JobHunter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobHunter.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var allUsers = _userManager.Users.ToList();
                var endUsers = new List<User>();

                foreach (var user in allUsers)
                {
                    if (await _userManager.IsInRoleAsync(user, "EndUser"))
                    {
                        endUsers.Add(user);
                    }
                }

                return endUsers;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while retrieving users.", ex);
            }
        }

        public Task<int> GetTotalPortfoliosAsync()
        {
            try
            {
                return _context.Portfolios.CountAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while retrieving total portfolios.", ex);
            }
        }

        public Task<int> GetTotalResumesAsync()
        {
            try
            {
                return _context.Resumes.CountAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while retrieving total resumes.", ex);
            }
        }
    }
}
