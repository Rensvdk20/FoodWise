using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Student
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }
        [Required, MaxLength(254)]
        public string Email { get; set; }
        [Required]
        public DateTime Birthday
        {
            get
            {
                return this.Birthday;
            }
            set
            {
                DateTime tempDate = value;
                if ((DateTime.Now - tempDate.AddYears(16)).TotalDays < 0)
                {
                    throw new ArgumentException("Student must be 16 years or older");
                }

                this.Birthday = value;
            }
        }
        [Required, MaxLength(12)]
        public int StudentNumber { get; set; }
        [Required]
        public City StudyCity { get; set; }
        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; }
    }
}