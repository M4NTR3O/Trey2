namespace Trey2.Models
{
    public class Message: BaseModel
    {
        public ulong relation_id { get; set; }
        public ulong sender_id { get; set; }
        public string content { get; set; }
        public DateTime created_time { get; set; }
        public bool is_deleted { get; set; }
    }
}
