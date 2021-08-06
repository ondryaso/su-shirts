using System.ComponentModel.DataAnnotations;

namespace SUShirts.Data.Entities
{
    public class ReservationItem
    {
        public int ReservationId { get; set; }
        public int ShirtVariantId { get; set; }

        public Reservation Reservation { get; set; }
        public ShirtVariant ShirtVariant { get; set; }

        [Required] public int Count { get; set; } = 1;
        [Required] public int Price { get; set; }
    }
}
