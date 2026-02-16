using System.ComponentModel.DataAnnotations;
using Trey2.Models;

namespace Trey2.ViewModel
{
    public class LowTraining: BaseModel
    {
        /// <summary>
        /// Name Training - - string with length = 50
        /// </summary>
        public required string? name_training { get; set; }
        /// <summary>
        /// Duration training - float
        /// </summary>
        public required float duration { get; set; }
        /// <summary>
        /// Distance training - float (May be null)
        /// </summary>
        public required float? distance { get; set; }

        /// <summary>
        /// Local time start training - DateTime
        /// </summary>
        public required DateTime local_timestamp { get; set; }
    }
}
