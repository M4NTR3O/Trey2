using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class TrainingLap
    {

        /// <summary>
        /// Lap number - unsigned int\
        /// Номер круга
        /// </summary>
        [Required]
        required public int lap_number { get; set; }

        /// <summary>
        /// Lap caption - string with length = 40 (may be null)\
        /// Описание круга (Может быть null)
        /// </summary>
        [MaxLength(40)]
        public string? lap_caption { get; set; }

        /// <summary>
        /// Lap distance - float (may be null)\
        /// Дистанция круга (Может быть null)
        /// </summary>
        public double? distance { get; set; }

        /// <summary>
        /// Lap duration  - float\
        /// Продолжительность круга
        /// </summary>
        [Required]
        required public double duration { get; set; }

        /// <summary>
        /// Time when lap is start (Local time) (OR time, when lap is end - будет стандартизироваться) - datetime\
        /// Время начала круга (Локальное время) (Или время, когда заканчивается круг (UTC формат) - будет стандартизироваться
        /// </summary>
        [Required]
        required public DateTime timestamp { get; set; }

        /// <summary>
        /// Time when lap is start (format UTC) - datetime\
        /// Время начала круга (Формат UTC)
        /// </summary>
        [Required]
        required public DateTime start_time { get; set; }

        /// <summary>
        /// Average heart rate - byte (tinyint) (may be null)\
        /// Средняя Частота сердечных сокращений (Может быть null)
        /// </summary>
        public byte? avg_heart_rate { get; set; }

        /// <summary>
        /// Mininmum heart rate - byte (tinyint) (may be null)\
        /// Минимальная Частота сердечных сокращений (Может быть null)
        /// </summary>
        public byte? min_heart_rate { get; set; }

        /// <summary>
        /// Maximum heart rate - byte (tinyint) (may be null)\
        /// Максимальная Частота сердечных сокращений (Может быть null)
        /// </summary>
        public byte? max_heart_rate { get; set; }

        /// <summary>
        /// Average Speed (m/s) - float (may be null)\
        /// Средняя скорость (м/с) (может быть null)
        /// </summary>
        public double? avg_speed { get; set; }

        /// <summary>
        /// Maximum Speed (m/s) - float (may be null)\
        /// Максимальная скорость (м/с) (может быть null)
        /// </summary>
        public double? max_speed { get; set; }

        /// <summary>
        /// Average running cadence (step pair rate/min) - unsigned short (smallint) (may be null)\
        /// Средний беговой каденс (частота пар шагов/мин) (может быть null)
        /// </summary>
        public ushort? avg_running_cadence { get; set; }

        /// <summary>
        /// Maximum running cadence (step pair rate/min) - unsigned short (smallint) (may be null)\
        /// Максимальный беговой каденс (частота пар шагов/мин) (может быть null)
        /// </summary>
        public ushort? max_running_cadence { get; set; }

        /// <summary>
        /// Average power (Wt) - unsigned short (smallint) (may be null)\
        /// Средняя мощность (Вт) (может быть null)
        /// </summary>
        public ushort? avg_power { get; set; }

        /// <summary>
        /// Maximum power (Wt) - unsigned short (smallint) (may be null)\
        /// Максимальная мощность (Вт) (может быть null)
        /// </summary>
        public ushort? max_power { get; set; }

        /// <summary>
        /// Total Ascent (m) - unsigned short (smallint) (may be null)\
        /// Общий подъем (м) (может быть null)
        /// </summary>
        public ushort? total_descent { get; set; }

        /// <summary>
        /// Start position latitude (dd.dddddd) - float (may be null)\
        /// Стартовая позиция широты (формат dd.dddddd) (может быть null)
        /// </summary>
        public ushort? total_ascent { get; set; }


    }
}
