using System.ComponentModel.DataAnnotations;
using Trey2.Models;

namespace Trey2.DTO.Coach
{
    public class CoachElement: BaseModel
    {
        public string email {get; set;}
        public string nickname { get; set; }
        public string fullname { get; set; }
    }
}
