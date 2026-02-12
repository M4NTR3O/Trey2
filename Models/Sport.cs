using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class Sport: LowModel
    {
        [Required]
        required public string sport_name { get; set; }
    }
}
