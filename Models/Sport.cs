using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class Sport: LowModel
    {
        /// <summary>
        /// Name sport - string wuth length = 50
        /// </summary>
        [Required]
        [MaxLength(50)]
        required public string sport_name { get; set; }
    }
}
