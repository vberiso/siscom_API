using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("View_Profile")]
    public class ViewProfile
    {
        [Column("id_profile")]
        public int ViewId { get; set; }
        public View View { get; set; }

        [Column("id_view")]
        public string RoleId { get; set; }
        public ApplicationRol ApplicationRol { get; set; }
    }
}
