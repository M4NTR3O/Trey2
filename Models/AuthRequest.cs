using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    public class AuthRequest
    {
        [Required]
        required public string Login { get; set; }
        [Required]
        required public string Password { get; set; }
    }
}
