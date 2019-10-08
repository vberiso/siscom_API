using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("Postal_Mx")]
    public class PostalMx
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("IdPostalmx")]
        public int Id { get; set; }
        [Column("codigo"), StringLength(5)]
        public string Codigo { get; set; }
        [Column("asenta"), StringLength(5)]
        public string Asenta { get; set; }
        public string DTipoAsenta { get; set; }
        public string DMunicipio { get; set; }
        public string DEstado { get; set; }
        public string DCiudad { get; set; }
        public int DCp { get; set; }
        public int CEstado { get; set; }
        public int COficina { get; set; }
        public int CCp { get; set; }
        public int CTipoAsenta { get; set; }
        public int CMnpio { get; set; }
        public int IdAsentaCpcons { get; set; }
        public string DZona { get; set; }
        public int CCveCiudad { get; set; }
    }
}
