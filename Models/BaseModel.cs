using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public abstract class BaseModel
    {
        /// <summary>
        /// ID Model - unsigned long
        /// </summary>
        [Required]
        required public ulong id { get; set; }
    }
}
