using System.ComponentModel.DataAnnotations;
using Trey2.Models;

namespace Trey2.ViewModel
{
    public class UpdatePlanTraining: BaseModel
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
    }
}
