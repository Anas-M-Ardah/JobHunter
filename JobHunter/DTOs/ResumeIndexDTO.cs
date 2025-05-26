using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobHunter.DTOs
{
    public class ResumeIndexDTO
    {
        public Guid ResumeId { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Professional Title")]
        public string Title { get; set; }

        [DisplayName("Last Updated")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime UpdatedAt { get; set; }
    }
}