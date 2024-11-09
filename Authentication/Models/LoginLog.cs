namespace Authentication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LoginLog")]
    public partial class LoginLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int DeviceId { get; set; }

        public DateTime LoginTime { get; set; }

        [Required]
        [StringLength(50)]
        public string IpAddress { get; set; }

        public bool IsNewDevice { get; set; }

        public bool Status { get; set; }

        public virtual UserDevice UserDevice { get; set; }

        public virtual Users Users { get; set; }
    }
}
