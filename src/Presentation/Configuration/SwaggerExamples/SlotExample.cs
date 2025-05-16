using AppointmentManager.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace AppointmentManager.Presentation.Configuration.SwaggerExamples
{
    public class SlotExample : IExamplesProvider<Slot>
    {
        public Slot GetExamples()
        {
            return SwaggerExample.Create(
                "Example",
                new Slot
                {
                    Start = new DateTime(2023, 06, 12, 9, 0 ,0),
                    End = new DateTime(2023, 06, 12, 9, 10, 0)
                }).Value;
        }
    }
}
