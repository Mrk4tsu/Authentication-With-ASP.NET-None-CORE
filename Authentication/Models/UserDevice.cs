namespace Authentication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserDevice")]
    public partial class UserDevice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserDevice()
        {
            LoginLog = new HashSet<LoginLog>();
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string DeviceName { get; set; }

        [Required]
        [StringLength(50)]
        public string IpAddress { get; set; }

        public DateTime FistLogin { get; set; }

        public DateTime LastLogin { get; set; }

        public bool IsTrusted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LoginLog> LoginLog { get; set; }

        public virtual Users Users { get; set; }
    }
}
