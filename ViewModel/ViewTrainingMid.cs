using System.ComponentModel.DataAnnotations;
using Trey2.Models;

namespace Trey2.ViewModel
{
    public class ViewTrainingMid: BaseModel
    {
        /// <summary>
        /// Sportsman ID - unsigned long (bigint)\
        /// ID Спортсмена
        /// </summary>
        required public ulong sportsman_id { get; set; }

        /// <summary>
        /// Sport name - string\
        /// Наименование вида спорта
        /// </summary>
        required public string sport { get; set; }

        /// <summary>
        /// Type Training name - string\
        /// Наименование типа тренировки
        /// </summary>
        required public string type_training { get; set; }

        /// <summary>
        /// Training name - string with length = 100 (may be null)\
        /// Наименование тренировки (Может быть null)
        /// </summary>
        public string? name_training { get; set; }

        /// <summary>
        /// Time when training is start (format UTC) - datetime\
        /// Время начала тренировки (Формат UTC)
        /// </summary>
        required public DateTime time_start { get; set; }

        /// <summary>
        /// Time when training is start (Local time) (OR time, when training is end - будет стандартизироваться) - datetime\
        /// Время начала тренировки (Локальное время) (Или время, когда заканчивается тренировка (UTC формат) - будет стандартизироваться
        /// </summary>
        required public DateTime local_timestamp { get; set; }

        /// <summary>
        /// Training duration  - float\
        /// Продолжительность тренировки
        /// </summary>
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

        public float? avg_speed { get; set; }


        /// <summary>
        /// Total Ascent (m) - unsigned short (smallint) (may be null)\
        /// Общий подъем (м) (может быть null)
        /// </summary>
        public ushort? total_ascent { get; set; }

        /// <summary>
        /// Start position latitude (dd.dddddd) - float (may be null)\
        /// Стартовая позиция широты (формат dd.dddddd) (может быть null)
        /// </summary>
        public float? start_position_lat { get; set; }

        /// <summary>
        /// Start position longtude (dd.dddddd) - float (may be null)\
        /// Стартовая позиция долготы (формат dd.dddddd) (может быть null)
        /// </summary>
        public float? start_position_long { get; set; }

        /// <summary>
        /// Max altitude during trainig - short (smallint) (may be null)\
        /// Максимальная высота в течение тренировки (может быть null)
        /// </summary>
        public short? max_altitude { get; set; }

        /// <summary>
        /// Calories - unsigned short(smallint) (may be null)\
        /// Калории (может быть null)
        /// </summary>
        public ushort? calories { get; set; }

        /// <summary>
        /// flag answered has any gps-tag - bool (tinyint(1))\
        /// Флаг, показывающий есть ли хоть одна gps-метка
        /// </summary>
        required public bool is_gps { get; set; }

        public ulong? coach_creator { get; set; }

        /// <summary>
        /// Comment coach about training - string with length = 1000 (may be null)\
        /// Комментарий тренера о тренировке (может быть null)
        /// </summary>
        public string? status_training { get; set; }

        /// <summary>
        /// List of Laps (may be null)\
        /// Список кругов
        /// </summary>
        public List<TrainingLap>? laps { get; set; }

        /// <summary>
        ///  List of Trackpoint (may be null)\
        /// Список трэкпоинтов (формат Timestamp, latitude, longitude)
        /// </summary>
        public List<TrackpointView>? trackpoints { get; set; }
        
        public string? description { get; set; }
    }
}
