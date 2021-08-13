using SUShirts.Data.Enums;

namespace SUShirts.Business.Dto
{
    public class ShirtVariantRequestDto
    {
        public int Id { get; set; }
        public SexVariant Sex { get; set; }
        public SizeVariant Size { get; set; }
        public int ItemsLeft { get; set; }
        public int Price { get; set; }

        public int ShirtId { get; set; }
        public string ShirtName { get; set; }

        public int RequestedCount { get; set; }
        public bool WillBeRequested { get; set; }
    }
}