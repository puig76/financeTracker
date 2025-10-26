using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Web.Models;

    public class LoginModel
    {
        [Required]
        public required string UserName { get; set; }

        [Required]
        public required string Password { get; set; }
    }
