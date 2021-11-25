using SUShirts.Data.Enums;

namespace SUShirts.Data.Extensions
{
    public static class ReservationStateExtensions
    {
        public static bool IsClosed(this ReservationState state)
        {
            return state == ReservationState.Finished || state == ReservationState.Cancelled;
        }
    }
}
