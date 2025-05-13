using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentManager.Domain.Models
{
    public class WorkDay
    {
        public string Name { get; set; }
        public WorkPeriod WorkPeriod { get; set; } = new WorkPeriod();

        public List<BusySlot> BusySlots { get; set; } = new List<BusySlot>();
    }
}
