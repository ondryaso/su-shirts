using SUShirts.Data.Enums;

namespace SUShirts.Business.Dto
{
    public class ReservationItemDto
    {
        public int ShirtVariantId { get; set; }
        public int ShirtId { get; set; }
        public string ShirtName { get; set; }
        public SizeVariant ShirtSize { get; set; }
        public SexVariant ShirtSex { get; set; }
        public int Count { get; set; }
        public int InStock { get; set; }
        public int LeftForReserving { get; set; }
        public int Price { get; set; }
    }
}
