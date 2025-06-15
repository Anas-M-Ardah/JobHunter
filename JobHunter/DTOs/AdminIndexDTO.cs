namespace JobHunter.Models.DTOs
{
    public class AdminIndexDTO
    {
        public List<User> AllUsers { get; set; } = new List<User>();
        public int TotalUsers { get; set; }
        public int TotalResumes { get; set; }
        public int TotalPortfolios { get; set; }
    }
}