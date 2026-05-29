using Trey2.Models;

namespace Trey2.DTO.Coach
{
    public class SportsmanElement: BaseModel
    {
        public string fullname { get; set; }
        public string nickname { get; set; }
        public ulong id_relation { get; set; }
    }
}
