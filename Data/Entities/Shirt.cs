using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SUShirts.Data.Entities
{
    public class Shirt
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public int PrimaryColorId { get; set; }
        public Color PrimaryColor { get; set; }

        [Required] public int SecondaryColorId { get; set; }
        public Color SecondaryColor { get; set; }

        [Required] public bool Hidden { get; set; } = false;
        
        public string ImageUrl { get; set; }

        public ICollection<ShirtVariant> Variants { get; set; }
    }
}
