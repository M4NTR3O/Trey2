using Trey2.Models;

namespace Trey2.DTO.User
{
    public class UserCookie: BaseModel
    {
        public string nickname { get; set; }
        public string role { get; set; }
    }
}
