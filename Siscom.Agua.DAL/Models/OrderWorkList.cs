using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Siscom.Agua.DAL.Models
{
    [Table("order_work_list")]
    public class OrderWorkList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("agreement_id"), Required]
        public int AgreementId { get; set; }
        [Column("status"), StringLength(6)]
        public string Status { get; set; }
        [Column("folio_order_result"), StringLength(30)]
        public string FolioOrderResult { get; set; }
        [Column("type_order_result"), StringLength(5)]
        public string TypeOrderResult { get; set; }
        [Column("order_work_id_result"), StringLength(5)]
        public string OrderWorkIdResult { get; set; }
        [Column("latitude"), StringLength(25)]
        public string Latitude { get; set; }
        [Column("longitude"), StringLength(25)]
        public string Longitude { get; set; }

        [Column("year"), StringLength(10)]
        public string Year { get; set; }
        [Column("account"), StringLength(10)]
        public string Account { get; set; }
        [Column("tipo_toma"), StringLength(25)]
        public string TipoToma { get; set; }
        [Column("tipo_servicio"), StringLength(25)]
        public string TipoServicio { get; set; }
        [Column("nombre"), StringLength(100)]
        public string Nombre { get; set; }
        [Column("domicilio"), StringLength(150)]
        public string Domicilio { get; set; }
        [Column("email"), StringLength(50)]
        public string Email { get; set; }
        [Column("tel"), StringLength(15)]
        public string Tel { get; set; }
        [Column("ruta"), StringLength(10)]
        public string Ruta { get; set; }
        [Column("status_account"), StringLength(20)]
        public string StatusAccount { get; set; }
        [Column("descuento_vulnerable"), StringLength(10)]
        public string DescuentoVulnerable { get; set; }
        [Column("have_convenio"), StringLength(10)]
        public string HaveConvenio { get; set; }
        [Column("adeudo")]
        public double Adeudo { get; set; }
        [Column("periodos")]
        public int Periodos { get; set; }
        

        public int OrderWorkId { get; set; }
        public OrderWork OrderWork { get; set; }
    }
}
