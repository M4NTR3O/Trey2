namespace Trey2.Models
{
    public class UserZones: BaseModel
    {
        /// <summary>
        /// User id - unsigned long (bigint)
        /// </summary>
        required public ulong user_id { get; set; }

        /// <summary>
        /// Nuber zone - byte
        /// </summary>
        required public byte zone_number { get; set; }

        /// <summary>
        /// Low limit of zone - double\
        /// Нижняя граница зоны
        /// </summary>
        required public double low_limit { get; set; }

        /// <summary>
        /// High limit of zone - double\
        /// Верхняя граница зоны
        /// </summary>
        required public double high_limit { get; set; }

        /// <summary>
        /// Type zone - byte\
        /// Тип зоны
        /// </summary>
        required public byte type_zone { get; set; }

    }
}
