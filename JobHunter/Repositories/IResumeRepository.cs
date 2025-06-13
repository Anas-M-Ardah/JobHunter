using JobHunter.DTOs;
using JobHunter.Models;

namespace JobHunter.Repositories
{
    public interface IResumeRepository
    {
        public Task<bool> CreateResume(DTOs.ResumeCreateEditDTO resumeCreateDTO, User user);
        public Task<bool> UpdateResume(DTOs.ResumeCreateEditDTO resumeCreateDTO);
        public Task<bool> UpdateResume(Resume resume);
        public Task<Resume> MapResumeDTOToResumeModel(DTOs.ResumeCreateEditDTO resumeCreateDTO, User user);
        public List<ResumeIndexDTO> GetAllResumesByUserId(string userId);
        public ResumeCreateEditDTO GetResumeById(Guid resumeId);
        public Task<Resume> GetResumeByIdAsync(Guid resumeId);    
        public Task<bool> DeleteResumeAsync(Guid resumeId);
    }
}
