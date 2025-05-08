using System.ComponentModel.DataAnnotations;

namespace User.Domain
{
    public class UserModel
    {
        public string Name { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
