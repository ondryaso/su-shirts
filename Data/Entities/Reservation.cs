using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;
using SUShirts.Data.Enums;

namespace SUShirts.Data.Entities
{
    public class Reservation
    {
        [Key] public int Id { get; set; }

        [Required] [StringLength(128)] public string Name { get; set; }

        [Required] [StringLength(128)] public string Email { get; set; }

        [StringLength(64)] public string PhoneOrDiscordTag { get; set; }

        public ICollection<ReservationItem> Items { get; set; }

        [Required] [Column("Handled")] public ReservationState State { get; set; } = ReservationState.New;
        [StringLength(128)] public string HandledBy { get; set; }
        [StringLength(128)] public string AssignedTo { get; set; }
        [StringLength(512)] public string Note { get; set; }
        [StringLength(512)] public string InternalNote { get; set; }
        [Required] public DateTime MadeOn { get; set; }
        public DateTime? HandledOn { get; set; }
    }
}
