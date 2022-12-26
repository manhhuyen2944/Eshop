using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Eshop.ViewModel
{
    public class LoginViewModel
    {
        public int Id { get; set; }

        [DisplayName("Tên đăng nhập")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "{0} từ 6-20 kí tự")]
        public string Username { get; set; }

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "{0} từ 6-50 kí tự")]
        public string Password { get; set; }
    }
}
