using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace AppointmentManager.Presentation.Configuration.SwaggerExamples
{
    public class UnauthorizedProblemDetailsExample : IExamplesProvider<ProblemDetails>
    {
        public ProblemDetails GetExamples()
        {
            return SwaggerExample.Create(
                "Example",
                new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Detail = "Exception message"
                }).Value;
        }
    }
}
