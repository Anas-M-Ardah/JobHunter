namespace JobHunter.Models
{
    public class EndUser : User
    {
        List<Portfolio> Portfolios { get; set; }
        List<Resume> Resumes { get; set; }
    }
}
