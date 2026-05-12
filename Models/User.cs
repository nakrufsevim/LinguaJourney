using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinguaJourney.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(60)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(60)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(60)]
        public string NativeLanguage { get; set; } = string.Empty;

        [StringLength(160)]
        public string LearningMotto { get; set; } = string.Empty;

        [StringLength(260)]
        public string ProfilePicture { get; set; } = string.Empty;

        public DateTime JoinDate { get; set; } = DateTime.Now;

        public virtual ICollection<LanguageTrack> LanguageTracks { get; set; } = new List<LanguageTrack>();
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public virtual ICollection<PracticeLog> PracticeLogs { get; set; } = new List<PracticeLog>();
    }
} 
