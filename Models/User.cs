using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class User: BaseModel
    {
        [Required]
        required public string user_surname { get; set; }
        [Required]
        required public string user_name { get; set; }
        public string? user_fathername { get; set; }
        
        [Required]
        [EmailAddress]
        required public string user_email { get; set; }
        [Required]
        required public string user_nickname { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        required public DateOnly user_birthday { get; set; }
        [Required]
        required public bool user_gender { get; set; }
        [Required]
        required public float user_height { get; set; }
        [Required]
        required public float user_weight { get; set; }
        [Required]
        required public bool user_only_coach { get; set; }
        [Required]
        required public byte user_hr_rest { get; set; }
        [Required]
        required public byte user_hr_max { get; set; }
        public float? user_pace_max { get; set; }

    }
}
