using Eshop.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.ViewModel
{
    public class RegisterViewModel
    {
        public int Id { get; set; }

        [DisplayName("Tên đăng nhập")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "{0} từ 6-20 kí tự")]
        [Remote(action: "ValidateUserName", controller: "Accounts")]
        public string Username { get; set; }

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "{0} từ 6-50 kí tự")]
        public string Password { get; set; }

        [NotMapped]
        [Display(Name = "Nhập lại mật khẩu")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "{0} từ 6-50 kí tự")]
        [Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "{0} không hợp lệ")]
        [Remote(action: "ValidateEmail", controller: "Accounts")]
        public string Email { get; set; }

        [DisplayName("SĐT")]
        [RegularExpression(@"0\d{9}", ErrorMessage = "SĐT không hợp lệ")]
        [Remote(action: "ValidatePhone", controller: "Accounts")]
        public string Phone { get; set; }

        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        [DisplayName("Họ tên")]
        public string FullName { get; set; }

        [DisplayName("Là admin")]
        public bool IsAdmin { get; set; }

        [DisplayName("Ảnh đại diện")]
        public string Avatar { get; set; }

        [DisplayName("Còn hoạt động")]
        public bool Status { get; set; }

        public string RandomKey { get; set; }
    }
}
