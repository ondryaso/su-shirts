using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SUShirts.Data.Entities
{
    public class Reservation
    {
        [Key] public int Id { get; set; }

        [Required] [StringLength(128)] public string Name { get; set; }

        [Required] [StringLength(128)] public string Email { get; set; }

        [StringLength(64)] public string PhoneOrDiscordTag { get; set; }

        public ICollection<ReservationItem> Items { get; set; }

        [Required] public bool Handled { get; set; } = false;
        [StringLength(128)] public string HandledBy { get; set; }

        [StringLength(512)] public string Note { get; set; }
    }
}
