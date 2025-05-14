using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentManager.Domain.Formatters
{
    public interface IDateRequestFormatter
    {
        public string GetCompatibleDateWithSlotService(DateOnly date);
    }
}
