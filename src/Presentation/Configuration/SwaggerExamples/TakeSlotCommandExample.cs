using AppointmentManager.Application.Slots.Commands.TakeSlot;
using AppointmentManager.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace AppointmentManager.Presentation.Configuration.SwaggerExamples
{
    public class TakeSlotCommandExample : IExamplesProvider<TakeSlotCommand>
    {
        public TakeSlotCommand GetExamples()
        {
            return SwaggerExample.Create(
                "Example",
                new TakeSlotCommand
                {
                    FacilityId = Guid.NewGuid().ToString(),
                    Start = new DateTime(2023, 6, 12, 9, 0, 0),
                    End = new DateTime(2023, 6, 12, 9, 10, 0),
                    Comments = "Pain on my left arm",
                    Patient = new Patient("Sergio", "Brotons", "email@email.com", "555 99 88 55")
                }
            ).Value;
        }
    }
}
