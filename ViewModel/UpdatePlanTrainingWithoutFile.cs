using System.ComponentModel.DataAnnotations;
using Trey2.Models;

namespace Trey2.ViewModel
{
    public class UpdatePlanTrainingWithoutFile: BaseModel
    {
        /// <summary>
        /// Stress score - byte (may be null) (1..10)\
        /// Оценка напряжения (может быть null)
        /// </summary>
        [Range(1, 10)]
        public byte? stress_score { get; set; }

        /// <summary>
        /// Feeling score - byte (may be null) (1..5)\
        /// Оценка самочувствия (может быть null)
        /// </summary>
        [Range(1, 5)]
        public byte? feeling_score { get; set; }

        /// <summary>
        /// Comment sportsman about training - string with length = 1000 (may be null)\
        /// Комментарий спортсмена о тренировке (может быть null)
        /// </summary>
        [MaxLength(1000)]
        public string? comment_sportsman { get; set; }
        /// <summary>
        /// Time when training is start (format UTC) - datetime\
        /// Время начала тренировки (Формат UTC)
        /// </summary>
        [Required]
        required public DateTime time_start { get; set; }

        /// <summary>
        /// Time when training is start (Local time) (OR time, when training is end - будет стандартизироваться) - datetime\
        /// Время начала тренировки (Локальное время) (Или время, когда заканчивается тренировка (UTC формат) - будет стандартизироваться
        /// </summary>
        [Required]
        required public DateTime local_timestamp { get; set; }

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

        public ushort? calories { get; set; }
    }
}
