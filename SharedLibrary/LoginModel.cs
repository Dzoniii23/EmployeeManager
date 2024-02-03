using System.ComponentModel.DataAnnotations;

namespace SharedLibrary
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required")] //data annotation attributes
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")] //data annotation attributes
        public string Password { get; set; } = string.Empty;
    }
}
