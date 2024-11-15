using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Authentication.ViewModels
{
    public class ResetPasswordModel
    {
        
        [Required(ErrorMessage = "Mật khẩu mới là trường bắt buộc", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string ConfirmPassword { get; set; }
        [Key]
        [Required]
        public string ResetCode { get; set; }
    }
}