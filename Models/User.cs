using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CourseNotesSharing.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NativeLanguage { get; set; } = string.Empty;
        public string LearningMotto { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; } = DateTime.Now;

        public virtual ICollection<LanguageTrack> LanguageTracks { get; set; } = new List<LanguageTrack>();
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public virtual ICollection<PracticeLog> PracticeLogs { get; set; } = new List<PracticeLog>();
    }
} 
