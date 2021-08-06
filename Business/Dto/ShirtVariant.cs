using System.ComponentModel.DataAnnotations;
using SUShirts.Data.Enums;

namespace SUShirts.Business.Dto
{
    public class ShirtVariantDto
    {
        public int Id { get; set; }
        public SexVariant Sex { get; set; }
        public SizeVariant Size { get; set; }
        public int ItemsLeft { get; set; }
        public int Price { get; set; }
    }
}
