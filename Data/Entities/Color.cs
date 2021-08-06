using System.ComponentModel.DataAnnotations;

namespace SUShirts.Data.Entities
{
    public class Color
    {
        [Key] public int Id { get; set; }

        [Required] [StringLength(64)] public string Name { get; set; }

        [Required] [StringLength(6)] public string Hex { get; set; }
    }
}
