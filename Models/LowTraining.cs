using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class LowTraining: BaseModel
    {
        /// <summary>
        /// Name Training - - string with length = 50
        /// </summary>
        [Required]
        public required string? name_training { get; set; }
        /// <summary>
        /// Duration training - float
        /// </summary>
        [Required]
        public required float duration { get; set; }
        /// <summary>
        /// Distance training - float?
        /// </summary>
        [Required]
        public required float? distance { get; set; }
    }
}
