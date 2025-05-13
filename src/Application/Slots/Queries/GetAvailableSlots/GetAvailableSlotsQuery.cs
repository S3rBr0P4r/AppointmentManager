using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
