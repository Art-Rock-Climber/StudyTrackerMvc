using System.ComponentModel.DataAnnotations;

namespace stTrackerMVC.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsStudent { get; set; }
    }
}
