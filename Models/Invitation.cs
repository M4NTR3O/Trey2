namespace Trey2.Models
{
    public class Invitation: BaseModel
    {
        public ulong sportsman_id { get; set; }
        public ulong coach_id { get; set; }
        /// <summary>
        /// Status invite:
        /// 0 - Pendind
        /// 1 - Accepted
        /// 2 - Rejected
        /// 3 - Canceled
        /// </summary>
        public byte status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? responded_at { get; set; }
    }
}
