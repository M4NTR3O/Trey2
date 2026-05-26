using Trey2.Models;

namespace Trey2.ViewModel
{
    public class Coach: BaseModel
    {
        public ulong relation_id { get; set; }
        public string fullname { get; set; }
        public string nickname { get; set; }
    }
}
