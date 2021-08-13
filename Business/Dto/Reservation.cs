using System;
using System.Collections.Generic;

namespace SUShirts.Business.Dto
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneOrDiscordTag { get; set; }
        public string Note { get; set; }
        public bool Handled { get; set; }
        public string HandledBy { get; set; }
        public DateTime MadeOn { get; set; }
        public DateTime? HandledOn { get; set; }
        public List<ReservationItemDto> Items { get; set; }
    }
}
