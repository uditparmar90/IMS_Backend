using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS_Backend.Models
{
    public class Inventory_Transactions
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Transaction_id { get; set; } //PK
        public int Product_id { get; set; } //FK
        public int Transaction_type { get; set; }//(e.g., 'Inbound', 'Outbound', 'Adjustment', 'Return')

        public int Quantity { get; set; }//(Positive for additions, negative for subtractions)
        public DateTime Created_at { get; set; }
        public int? UserId { get; set; }
    }
}
