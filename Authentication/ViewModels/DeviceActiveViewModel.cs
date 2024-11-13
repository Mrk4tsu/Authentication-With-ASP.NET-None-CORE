using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Authentication.ViewModels
{
    public class DeviceActiveViewModel
    {
        [Key]
        public string Username { get; set; }
        public string OTP { get; set; }
    }
}