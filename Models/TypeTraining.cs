using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class TypeTraining: LowModel
    {
        /// <summary>
        /// Name type training - string wuth length = 50
        /// </summary>
        [Required]
        [MaxLength(50)]
        required public string type_training_name { get; set; }
    }
}
