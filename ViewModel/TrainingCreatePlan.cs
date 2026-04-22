using System.ComponentModel.DataAnnotations;

namespace Trey2.ViewModel
{
    public class TrainingCreatePlan
    {
        /// <summary>
        /// Sportsman ID - unsigned long (bigint)\
        /// ID Спортсмена
        /// </summary>
        public ulong? sportsman_id { get; set; }

        /// <summary>
        /// Sport ID - byte (tinyint)\
        /// ID Спорта
        /// </summary>
        [Required]
        required public byte sport_id { get; set; }

        /// <summary>
        /// Type Training ID - byte (tinyint)\
        /// ID Типа тренировки
        /// </summary>
        [Required]
        required public byte type_training_id { get; set; }

        /// <summary>
        /// Training name - string with length = 100 (may be null)\
        /// Наименование тренировки (Может быть null)
        /// </summary>
        [MaxLength(100)]
        public string? name_training { get; set; }

        [Required]
        required public DateTimeOffset local_timestamp { get; set; }

        /// <summary>
        /// Training duration  - float\
        /// Продолжительность тренировки
        /// </summary>
        [Required]
        required public float duration { get; set; }

        /// <summary>
        /// Training distance - float (may be null)\
        /// Дистанция тренировки (Может быть null)
        /// </summary>
        public float? distance { get; set; }

        /// <summary>
        /// Average heart rate - byte (tinyint) (may be null)\
        /// Средняя Частота сердечных сокращений (Может быть null)
        /// </summary>
        public byte? avg_heart_rate { get; set; }

        /// <summary>
        /// Average Speed (m/s) - float (may be null)\
        /// Средняя скорость (м/с) (может быть null)
        /// </summary>
        public float? avg_speed { get; set; }

        /// <summary>
        /// Coach ID who planned training - unsigned long (bigint) (may be null)\
        /// ID тренера,кто запланировал тренировку (может быть null)
        /// </summary>
        public ulong? coach_creator { get; set; }

        /// <summary>
        /// ID Status training - byte (may be null)\
        /// ID статуса тренировки - может быть null
        /// </summary>
        public byte? status_training { get; set; }

        public string? description { get; set; }
    }
}
