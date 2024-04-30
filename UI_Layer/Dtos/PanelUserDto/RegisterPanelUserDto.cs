using System.ComponentModel.DataAnnotations;

namespace UI_Layer.Dtos.PanelUserDto
{
    public class RegisterPanelUserDto
    {
        [Required(ErrorMessage = "Lütfen isim giriniz!")]
        [MinLength(3, ErrorMessage = "İsim en az 3 karakter olmalıdır.")]
        [MaxLength(25, ErrorMessage = "İsim en fazla 25 karakter olabilir.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Lütfen soyisim giriniz!")]
        [MinLength(2, ErrorMessage = "Soyisim en az 2 karakter olmalıdır.")]
        [MaxLength(25, ErrorMessage = "Soyisim en fazla 25 karakter olabilir.")]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "Lütfen kullanıcı adını giriniz!")]
        [MinLength(5, ErrorMessage = "Kullanıcı adı en az 5 karakter olmalıdır.")]
        [MaxLength(8, ErrorMessage = "Kullanıcı adı en fazla 8 karakter olabilir.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Lütfen mail giriniz!")]
        [MinLength(10, ErrorMessage = "Mail en az 10 karakter olmalıdır.")]
        [MaxLength(25, ErrorMessage = "Mail en fazla 8 karakter olabilir.")]
        public string? Mail { get; set; }

        [Required(ErrorMessage = "Lütfen şifre giriniz!")]
        [MinLength(5, ErrorMessage = "Şifre en az 5 karakter olmalıdır.")]
        [MaxLength(15, ErrorMessage = "Şifre en fazla 15 karakter olabilir.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Lütfen şifreyi tekrar giriniz!")]
        [Compare("Password", ErrorMessage = "Şifreler aynı değil!!!")]
        public string? ConfirmPassword { get; set; }

        public DateTime? RegisterDate { get; set; }

        [Required(ErrorMessage = "Lütfen departmant seçiniz!")]
        public int DepartmentID { get; set; }
    }
}
