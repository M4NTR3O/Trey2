namespace Trey2.Models
{
    public class LowTraining: BaseModel
    {
        public required string? name_training { get; set; }
        public required float duration { get; set; }
        public required float? distance { get; set; }
    }
}
