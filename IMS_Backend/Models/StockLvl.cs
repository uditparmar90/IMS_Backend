using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS_Backend.Models
{
    public class StockLvl
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int StockId { get; set; }
        public int Product_id { get; set; }
        public int Warehouse_id { get; set; }
        public int Quantity_on_hand { get; set; }
        public DateTime Last_updated { get; set; }
    }
}
