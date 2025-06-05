using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobHunter.Models
{
    public class Resume : PersonalInfo
    {
        [Key]
        public Guid ResumeId { get; set; } = Guid.NewGuid();

        [DataType(DataType.DateTime)]
        [DisplayName("Created Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [DisplayName("Last Modified")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public List<Education> Educations { get; set; }

        public List<Experience> Experiences { get; set; }

        public List<Skill> Skills { get; set; }

        public List<Language> Languages { get; set; }

        public List<Certificate> Certificates { get; set; }

        [Required(ErrorMessage = "User is required")]

        [ForeignKey("EndUserId")]
        public EndUser EndUser { get; set; }

        //add is deleted flag
        public bool IsDeleted { get; set; } = false;

        public string UserInputSkills { get; set; }
        public string UserInputLanguages { get; set; }
        public string UserInputExperiences { get; set; }
        public string UserInputCertificates { get; set; }
        public string UserInputEducation { get; set; }
        public string UserInputBio { get; set; } 
    }
}