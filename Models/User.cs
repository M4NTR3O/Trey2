using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class User: BaseModel
    {
        /// <summary>
        /// Surname user - string with length = 50
        /// </summary>
        [Required]
        [MaxLength(50)]
        required public string user_surname { get; set; }

        /// <summary>
        /// Name user - string with length = 50
        /// </summary>
        [Required]
        [MaxLength(50)]
        required public string user_name { get; set; }

        /// <summary>
        /// Fatherrname user - string with length = 50 (may be null)
        /// </summary>
        [MaxLength(50)]
        public string? user_fathername { get; set; }

        /// <summary>
        /// Email user - string with length = 50
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(50)]
        required public string user_email { get; set; }

        /// <summary>
        /// Nickname user - string with length = 50
        /// </summary>
        [Required]
        [MaxLength(50)]
        required public string user_nickname { get; set; }

        /// <summary>
        /// Password user - string
        /// </summary>
        [Required]
        public string? password { get; set; }

        /// <summary>
        /// Burtday user - Dateonly ("2026-01-31")
        /// </summary>
        [Required]
        required public DateOnly user_birthday { get; set; }

        /// <summary>
        /// Gender user - bool (tinyint(1))  \
        /// Men - true (1)  \
        /// Women - False (0)  
        /// </summary>
        [Required]
        required public bool user_gender { get; set; }

        /// <summary>
        /// Height user - float
        /// </summary>
        [Required]
        required public float user_height { get; set; }

        /// <summary>
        /// Weight user - float
        /// </summary>
        [Required]
        required public float user_weight { get; set; }

        /// <summary>
        /// Flag Only Coach accaunt - bool (tinyint(1))  \
        /// Only coach - true (1)  \
        /// Sportsman, Coach and sportsmen - false (0)
        /// </summary>
        [Required]
        required public bool user_only_coach { get; set; }

        /// <summary>
        /// User rest heart rate - byte (default = 60)
        /// </summary>
        [Required]
        required public byte user_hr_rest { get; set; }

        /// <summary>
        /// User Max heart rate - byte (default = 200)
        /// </summary>
        [Required]
        required public byte user_hr_max { get; set; }

        /// <summary>
        /// User Max pace - float (may be null)
        /// </summary>
        public float? user_pace_max { get; set; }

        /// <summary>
        /// flag "like coach" - boolean (default = false)
        /// </summary>
        public bool like_coach { get; set; }

    }
}
