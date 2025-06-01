namespace JobHunter.Models
{
    public class EndUser : User
    {
        public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
        public List<Resume> Resumes { get; set; }
    }
}
