using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Trey2.ViewModel
{
    public class TrainingCreate
    {
        /// <summary>
        /// Sportsman ID - unsigned long (bigint)\
        /// ID Спортсмена
        /// </summary>
        [Required]
        required public ulong sportsman_id { get; set; }

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
        /// Coach ID who planned training - unsigned long (bigint) (may be null)\
        /// ID тренера,кто запланировал тренировку (может быть null)
        /// </summary>
        public ulong? coach_creator { get; set; }

        /// <summary>
        /// Comment coach about training - string with length = 1000 (may be null)\
        /// Комментарий тренера о тренировке (может быть null)
        /// </summary>
        [MaxLength(1000)]
        public string? comment_coach { get; set; }

        /// <summary>
        /// ID Status training - byte (may be null)\
        /// ID статуса тренировки - может быть null
        /// </summary>
        public byte? status_training { get; set; }

        public string? description { get; set; }

        [SetsRequiredMembers]
        public TrainingCreate(TrainingCreatePlan training)
        {
            try
            {
                sportsman_id = (ulong)training.sportsman_id;
                sport_id = training.sport_id;
                type_training_id = training.type_training_id;
                name_training = training.name_training;
                local_timestamp = training.local_timestamp.LocalDateTime;
                time_start = training.local_timestamp.UtcDateTime;
                duration = training.duration;
                distance = training.distance;
                avg_heart_rate = training.avg_heart_rate;
                avg_speed = training.avg_speed;
                coach_creator = training.coach_creator;
                status_training = training.status_training;
                description = training.description;
            }
            catch
            {
                throw new Exception("ID спортсмена не задан");
            }
        }
    }
}
