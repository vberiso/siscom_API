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
        [Column("asenta"), StringLength(150)]
        public string Asenta { get; set; }
        [Column("tipo_asenta"), StringLength(150)]
        public string TipoAsenta { get; set; }
        [Column("municipio"), StringLength(150)]
        public string Municipio { get; set; }
        [Column("estado"), StringLength(150)]
        public string Estado { get; set; }
        [Column("ciudad"), StringLength(150)]
        public string Ciudad { get; set; }
        [Column("cp"), StringLength(6)]
        public int Cp { get; set; }
        [Column("cod_estado"), StringLength(2)]
        public int CEstado { get; set; }
        [Column("oficina"), StringLength(5)]
        public int Oficina { get; set; }
        [Column("cod_cp"), StringLength(2)]
        public int CCp { get; set; }
        [Column("cod_tipo_asenta"), StringLength(2)]
        public int CTipoAsenta { get; set; }
        [Column("cod_municipio"), StringLength(3)]
        public int CMunicipio { get; set; }
        [Column("id_asenta_cpcons"), StringLength(4)]
        public int IdAsentaCpcons { get; set; }
        [Column("zona"), StringLength(20)]
        public string Zona { get; set; }
        [Column("cve_ciudad"), StringLength(2)]
        public int CveCiudad { get; set; }
    }
}
