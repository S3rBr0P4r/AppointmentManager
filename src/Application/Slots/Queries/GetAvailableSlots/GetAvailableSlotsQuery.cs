namespace AppointmentManager.Application.Slots.Queries.GetAvailableSlots
{
    public class GetAvailableSlotsQuery
    {
        public GetAvailableSlotsQuery(DateOnly date)
        {
            Date = date;
        }

        public DateOnly Date { get; private set; }
    }
}
