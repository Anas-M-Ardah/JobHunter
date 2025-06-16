namespace JobHunter.Models.JSONResponse
{
    public class ResumeAIResponse
    {
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; } = string.Empty;
        public ResumeData? ResumeData { get; set; }
    }
}
