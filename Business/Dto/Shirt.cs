using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SUShirts.Business.Dto
{
    public class ShirtDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ColorDto PrimaryColor { get; set; }
        public ColorDto SecondaryColor { get; set; }
        public bool Hidden { get; set; }
        public string ImageUrl { get; set; }
        public List<ShirtVariantDto> Variants { get; set; }
    }
}
