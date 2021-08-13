using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SUShirts.Data.Enums;

namespace SUShirts.Data.Entities
{
    public class ShirtVariant
    {
        [Key] public int Id { get; set; }

        [Required] public int ShirtId { get; set; }
        public Shirt Shirt { get; set; }

        [Required] public SexVariant Sex { get; set; }
        [Required] public SizeVariant Size { get; set; }
        [Required] [ConcurrencyCheck] public int ItemsLeft { get; set; }
        [Required] public int Price { get; set; }
    }
}
