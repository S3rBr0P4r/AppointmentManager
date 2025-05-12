using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentManager.Core.Slots;

namespace AppointmentManager.Domain.Slots
{
    public interface ISlotsManagementService
    {
        public IEnumerable<Slot> GetAvailableSlots();
    }
}
