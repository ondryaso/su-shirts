using System.Threading.Tasks;
using SUShirts.Data.Entities;

namespace SUShirts.Business.Services
{
    public interface IEmailService
    {
        Task SendReservationEmailToUser(Reservation reservation);
        Task SendReservationEmailToManager(string reservationUrl);
    }
}
