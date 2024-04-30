using System.ComponentModel.DataAnnotations;

namespace UI_Layer.Dtos.EmployeeDto
{
    public class LoginEmployeeDto
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50)]
        [MinLength(5)]
        public string Password { get; set; }
    }
}
