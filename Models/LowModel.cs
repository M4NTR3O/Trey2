using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class LowModel
    {
        /// <summary>
        /// ID Model - byte
        /// </summary>
        [Required]
        required public byte id { get; set; }
    }
}
