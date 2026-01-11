using System.ComponentModel.DataAnnotations.Schema;

namespace IMS_Backend.Models
{
    public class Products
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Column(TypeName ="decimal(18,2)")]
        public decimal Price { get; set; }
        public int Category_id { get; set; }
        public int SubCategory_id { get; set; }
        //..Stock kipping Unit
        public required string SKU { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Original_Cost { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Reorder_lavel { get; set; }


    }
}
