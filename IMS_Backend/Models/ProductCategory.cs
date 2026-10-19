using System.ComponentModel.DataAnnotations.Schema;

namespace IMS_Backend.Models
{
    public class ProductCategory
    {
        public int? UserId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
