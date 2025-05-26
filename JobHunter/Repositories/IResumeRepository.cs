using JobHunter.DTOs;
using JobHunter.Models;

namespace JobHunter.Repositories
{
    public interface IResumeRepository
    {
        public bool CreateResume(DTOs.ResumeCreateEditDTO resumeCreateDTO, User user);
        public bool UpdateResume(DTOs.ResumeCreateEditDTO resumeCreateDTO);
        public Resume MapResumeDTOToResumeModel(DTOs.ResumeCreateEditDTO resumeCreateDTO, User user);
        public List<ResumeIndexDTO> GetAllResumesByUserId(string userId);

        public ResumeCreateEditDTO GetResumeById(Guid resumeId);
    }
}
