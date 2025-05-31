using JobHunter.Data;
using JobHunter.DTOs;
using JobHunter.Models;

namespace JobHunter.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDbContext _context;
        public PortfolioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CreatePortfolio(PortfolioCreateEditDTO portfolioCreateDTO, User user)
        {
            try
            {
                Portfolio portfolio = new Portfolio();
                //personal information
                portfolio.Address = portfolioCreateDTO.Address;
                portfolio.PhoneNumber = portfolioCreateDTO.PhoneNumber;
                portfolio.Email = portfolioCreateDTO.Email;
                portfolio.FirstName = portfolioCreateDTO.FirstName;
                portfolio.SecondName = portfolioCreateDTO.SecondName;
                portfolio.ThirdName = portfolioCreateDTO.ThirdName;
                portfolio.LastName = portfolioCreateDTO.LastName;
                portfolio.DateOfBirth = portfolioCreateDTO.DateOfBirth;
                portfolio.Major = portfolioCreateDTO.Major;

                //portfolio information
                portfolio.CreatedDate = DateTime.Now;
                portfolio.ModifiedDate = DateTime.Now;
                portfolio.PersonalImage = portfolioCreateDTO.PersonalImage;
                portfolio.EndUser.Id = user.Id;

                //Services and Projects
                portfolio.Services = new List<Service>(); //replace this with a function
                portfolio.Projects = new List<Project>(); //same here

                //File handling
                
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
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
    }
}
