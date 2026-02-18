using System.ComponentModel.DataAnnotations;
using Trey2.Models;

namespace Trey2.ViewModel
{
    public class LowTraining: BaseModel
    {
        /// <summary>
        /// Sport ID - byte (tinyint)\
        /// ID Спорта // Ожидается Bitmap в дальнейшем
        /// </summary>
        [Required]
        required public byte sport_id { get; set; }
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

        /// <summary>
        /// Name Status training - byte (may be null)\
        /// Наименование статуса тренировки - может быть null
        /// </summary>
        public string? status_training { get; set; }
    }
}
