using System.ComponentModel.DataAnnotations.Schema;

namespace IMS_Backend.Models
{
    public class Products
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Category_id { get; set; }
        public int SubCategory_id { get; set; }
        //..Stock kipping Unit
        public string SKU { get; set; }
        public Decimal Original_Cost { get; set; }
        public Decimal Reorder_lavel { get; set; }


    }
}
