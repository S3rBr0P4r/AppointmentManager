using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace AppointmentManager.Presentation.Configuration.SwaggerExamples
{
    public class BadRequestProblemDetailsExample : IExamplesProvider<ProblemDetails>
    {
        public ProblemDetails GetExamples()
        {
            return SwaggerExample.Create(
                "Example",
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad Request",
                    Detail = "Exception message"
                }).Value;
        }
    }
}
