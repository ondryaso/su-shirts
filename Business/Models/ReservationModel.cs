using System.ComponentModel.DataAnnotations;

namespace SUShirts.Business.Models
{
    public class ReservationModel
    {
        [Required(ErrorMessage = "Vyplň jméno.")]
        [StringLength(128, MinimumLength = 3, ErrorMessage = "Jméno musí mít minimálně 3 znaky, maximálně 128 znaků.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vyplň e-mail.")]
        [EmailAddress(ErrorMessage = "Tohle není e-mail.")]
        public string Email { get; set; }

        [StringLength(64, ErrorMessage = "To je nějak moc dlouhý telefon nebo Discord tag.")]
        public string PhoneOrDiscordTag { get; set; }

        [StringLength(450, ErrorMessage = "Poznámka může mít maximálně 450 znaků.")]
        public string Note { get; set; }

        [Required] public bool IsClubMember { get; set; } = false;
    }
}
