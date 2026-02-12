using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class TypeTraining: LowModel
    {
        [Required]
        required public string type_training_name { get; set; }
    }
}
