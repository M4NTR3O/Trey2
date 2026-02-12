using System.ComponentModel.DataAnnotations;

namespace Trey2.Models
{
    /// <summary>
    /// Model authorization
    /// </summary>
    public class AuthRequest
    {
        /// <summary>
        /// User's Email - string with length = 50
        /// </summary>
        [Required]
        required public string Login { get; set; }
        /// <summary>
        /// User's Password
        /// </summary>
        [Required]
        required public string Password { get; set; }
    }
}
