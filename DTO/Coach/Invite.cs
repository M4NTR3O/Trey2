using Trey2.Models;

namespace Trey2.DTO.Coach
{
    public class Invite: BaseModel
    {
        public string sportsman_fullname { get; set; }
        public string sportsman_nickname { get; set; }
        public ulong sportsman_id { get; set; }
        public string coach_fullname { get; set; }
        public string coach_nickname { get; set; }
        public ulong coach_id { get; set; }
        public byte status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? responded_at { get; set; }
    }
}
