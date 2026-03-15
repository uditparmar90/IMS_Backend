using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace IMS_Backend.Models
{
    public class Transactions
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int UserId { get; set; }
        public int prodId { get; set; }
        [AllowNull]
        public string name { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal price { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal quantity { get; set; }
        [AllowNull]
        [Column(TypeName="decimal(18,2)")]
        public decimal Original_Cost { get; set; }
        public DateTime tran_Date { get; set; } 
    }
}
