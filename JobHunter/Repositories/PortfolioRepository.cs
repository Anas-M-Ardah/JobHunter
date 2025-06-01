using JobHunter.Data;
using JobHunter.DTOs;
using JobHunter.Models;
using Microsoft.EntityFrameworkCore;

namespace JobHunter.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDbContext _context;
        public PortfolioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreatePortfolioAsync(PortfolioCreateEditDTO portfolioCreateDTO, User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create portfolio first
                var portfolio = new Portfolio
                {
                    Address = portfolioCreateDTO.Address,
                    PhoneNumber = portfolioCreateDTO.PhoneNumber,
                    Email = portfolioCreateDTO.Email,
                    FirstName = portfolioCreateDTO.FirstName,
                    SecondName = portfolioCreateDTO.SecondName,
                    ThirdName = portfolioCreateDTO.ThirdName,
                    LastName = portfolioCreateDTO.LastName,
                    DateOfBirth = portfolioCreateDTO.DateOfBirth,
                    Title = portfolioCreateDTO.Title,
                    Major = portfolioCreateDTO.Major,
                    Bio = portfolioCreateDTO.Bio,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    PersonalImage = portfolioCreateDTO.PersonalImage,
                    EndUserId = user.Id,
                };

                _context.Portfolios.Add(portfolio);
                await _context.SaveChangesAsync();

                // Now add services with the correct portfolio ID
                var services = portfolioCreateDTO.Services.Select(s => new Service
                {
                    ServiceName = s.ServiceName,
                    ServiceDescription = s.ServiceDescription,
                    ServiceImage = s.ServiceImage,
                    PortfolioId = portfolio.PortfolioId,
                }).ToList();

                _context.Services.AddRange(services);
                await _context.SaveChangesAsync();

                // Add projects with correct portfolio ID and service references
                var projects = portfolioCreateDTO.Projects.Select(p => new Project
                {
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ProjectLink = p.ProjectLink,
                    PortfolioId = portfolio.PortfolioId,

                    // Only set ServiceId if it references an existing service
                    ServiceId = p.ServiceId,

                    // File handling
                    ProjectAttachments = ConvertFileToBytes(p.ProjectAttachments),
                    ProjectAttachmentsName = p.ProjectAttachments.FileName,
                    ProjectAttachmentsContentType = p.ProjectAttachments.ContentType,
                }).ToList();

                _context.Projects.AddRange(projects);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        

        public byte[] ConvertFileToBytes(IFormFile file)
        {
            if(file.Length > 0)
            {
                Stream st = file.OpenReadStream();
                var fileBytes = new byte[file.Length];
                using (BinaryReader br = new BinaryReader(st))
                {
                    fileBytes = br.ReadBytes((int)file.Length);
                }
                return fileBytes;
            }
            return null;
        }

        public async Task<List<PortfolioIndexDTO>> GetAllPortfoliosByUserId(User user)
        {
            try
            {
                var portfolios = await _context.Portfolios
                    .Where(p => p.EndUserId == user.Id)
                    .Select(p => new PortfolioIndexDTO
                    {
                        PortfolioId = p.PortfolioId,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Title = p.Title,
                        ModifiedDate = p.ModifiedDate

                    })
                    .OrderByDescending(p => p.ModifiedDate)
                    .ToListAsync();

                return portfolios;
            }
            catch (Exception ex)
            {
                // Log the exception
                // You might want to use ILogger here
                throw new ApplicationException($"Error retrieving portfolios for user {user.Id}", ex);
            }
        }

        public async Task<PortfolioCreateEditDTO> GetPortfolioById(Guid portfolioId)
        {
            var result = await _context.Portfolios
                .Where(p => p.PortfolioId == portfolioId)
                .Select(p => new PortfolioCreateEditDTO
                {
                    PortfolioId = p.PortfolioId,
                    Address = p.Address,
                    PhoneNumber = p.PhoneNumber,
                    Email = p.Email,
                    FirstName = p.FirstName,
                    SecondName = p.SecondName,
                    ThirdName = p.ThirdName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    Title = p.Title,
                    Major = p.Major,
                    Bio = p.Bio,
                    PersonalImage = p.PersonalImage,
                    Services = _context.Services
                        .Where(s => s.PortfolioId == p.PortfolioId)
                        .Select(s => new ServiceDTO
                        {
                            ServiceId = s.ServiceId,
                            ServiceName = s.ServiceName,
                            ServiceDescription = s.ServiceDescription,
                            ServiceImage = s.ServiceImage
                        }).ToList(),
                    Projects = _context.Projects
                        .Where(pr => pr.PortfolioId == p.PortfolioId)
                        .Select(pr => new ProjectDTO
                        {
                            ProjectId = pr.ProjectId,
                            ProjectName = pr.ProjectName,
                            ProjectDescription = pr.ProjectDescription,
                            StartDate = pr.StartDate,
                            EndDate = pr.EndDate,
                            ProjectLink = pr.ProjectLink,
                            ServiceId = pr.ServiceId,
                        }).ToList()
                })
                .FirstOrDefaultAsync();
            return result ?? throw new KeyNotFoundException($"Portfolio with ID {portfolioId} not found.");
        }

        public async Task<bool> UpdatePortfolioAsync(PortfolioCreateEditDTO portfolioCreateDTO, User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Find existing portfolio
                var existingPortfolio = await _context.Portfolios
                    .Include(p => p.Services)
                    .Include(p => p.Projects)
                    .FirstOrDefaultAsync(p => p.PortfolioId == portfolioCreateDTO.PortfolioId && p.EndUserId == user.Id);

                if (existingPortfolio == null)
                {
                    return false; // Portfolio not found or user doesn't own it
                }

                // Update portfolio properties
                existingPortfolio.Address = portfolioCreateDTO.Address;
                existingPortfolio.PhoneNumber = portfolioCreateDTO.PhoneNumber;
                existingPortfolio.Email = portfolioCreateDTO.Email;
                existingPortfolio.FirstName = portfolioCreateDTO.FirstName;
                existingPortfolio.SecondName = portfolioCreateDTO.SecondName;
                existingPortfolio.ThirdName = portfolioCreateDTO.ThirdName;
                existingPortfolio.LastName = portfolioCreateDTO.LastName;
                existingPortfolio.DateOfBirth = portfolioCreateDTO.DateOfBirth;
                existingPortfolio.Title = portfolioCreateDTO.Title;
                existingPortfolio.Major = portfolioCreateDTO.Major;
                existingPortfolio.ModifiedDate = DateTime.Now;
                existingPortfolio.Bio = portfolioCreateDTO.Bio;

                // Update personal image only if a new one is provided
                if (portfolioCreateDTO.PersonalImage != null)
                {
                    existingPortfolio.PersonalImage = portfolioCreateDTO.PersonalImage;
                }

                _context.Portfolios.Update(existingPortfolio);
                await _context.SaveChangesAsync();

                // Handle Services - Remove existing and add new ones
                if (existingPortfolio.Services?.Any() == true)
                {
                    _context.Services.RemoveRange(existingPortfolio.Services);
                }

                if (portfolioCreateDTO.Services?.Any() == true)
                {
                    var services = portfolioCreateDTO.Services.Select(s => new Service
                    {
                        ServiceName = s.ServiceName,
                        ServiceDescription = s.ServiceDescription,
                        ServiceImage = s.ServiceImage,
                        PortfolioId = existingPortfolio.PortfolioId,
                    }).ToList();

                    _context.Services.AddRange(services);
                }

                await _context.SaveChangesAsync();

                // Handle Projects - Remove existing and add new ones
                if (existingPortfolio.Projects?.Any() == true)
                {
                    _context.Projects.RemoveRange(existingPortfolio.Projects);
                }

                if (portfolioCreateDTO.Projects?.Any() == true)
                {
                    var projects = portfolioCreateDTO.Projects.Select(p => new Project
                    {
                        ProjectName = p.ProjectName,
                        ProjectDescription = p.ProjectDescription,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        ProjectLink = p.ProjectLink,
                        PortfolioId = existingPortfolio.PortfolioId,
                        ServiceId = p.ServiceId,
                        // File handling - only update if new file is provided
                        ProjectAttachments = p.ProjectAttachments != null ? ConvertFileToBytes(p.ProjectAttachments) : null,
                        ProjectAttachmentsName = p.ProjectAttachments?.FileName,
                        ProjectAttachmentsContentType = p.ProjectAttachments?.ContentType,
                    }).ToList();

                    _context.Projects.AddRange(projects);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Consider logging the exception here
                return false;
            }
        }
    }
}
