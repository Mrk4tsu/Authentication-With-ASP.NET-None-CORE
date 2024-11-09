using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Authentication.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Biệt danh")]
        public string FullName { get; set; }
        [Required]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Vui lòng nhập lại đúng mật khẩu đã đặt")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }
    }
}