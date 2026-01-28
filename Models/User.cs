using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class User: BaseModel
    {
        [Required]
        public string user_surname { get; set; }
        [Required]
        public string user_name { get; set; }
        public string? user_fathername { get; set; }
        [Required]
        [EmailAddress]
        public string user_email { get; set; }
        [Required]
        public string user_nickname { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public DateOnly user_birthday { get; set; }
        [Required]
        public bool user_gender { get; set; }
        [Required]
        public float user_height { get; set; }
        [Required]
        public float user_weight { get; set; }
        [Required]
        public bool user_only_coach { get; set; }
        [Required]
        public byte user_hr_rest { get; set; }
        [Required]
        public byte user_hr_max { get; set; }
        public float? user_pace_max { get; set; }

    }
}
