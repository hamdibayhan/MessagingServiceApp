using System.ComponentModel.DataAnnotations;

namespace MessagingServiceApp.Dto.ApiParameter
{
    public class LoginParams
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
