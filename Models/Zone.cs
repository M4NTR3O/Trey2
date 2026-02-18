using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class Zone: LowModel
    {
        /// <summary>
        /// Caption zone - string with length = 15\
        /// Описание зоны
        /// </summary>
        [MaxLength(15)]
        required public string caption_zone { get; set; }

        /// <summary>
        /// Unit - string with length = 10\
        /// Единица измерения
        /// </summary>
        [MaxLength(10)]
        required public string unit { get; set; }
    }
}
