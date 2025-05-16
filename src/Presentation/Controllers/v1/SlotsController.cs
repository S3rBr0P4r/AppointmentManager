using System.Net;
using System.Net.Mime;
using AppointmentManager.Application.Slots.Commands.TakeSlot;
using AppointmentManager.Application.Slots.Queries.GetAvailableSlots;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Infrastructure.Dispatchers;
using AppointmentManager.Presentation.Configuration.SwaggerExamples;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Filters;

namespace AppointmentManager.Presentation.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class SlotsController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public SlotsController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
        }

        /// <summary>
        /// Gets available slots for a week, excluding the existent busy slots
        /// </summary>
        /// <param name="date">
        ///     The date to get available slots, <b>always expecting Monday</b>
        ///     <br />
        ///     Expected format: YYYY-MM-DD
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">OK with a list of available slots for the week</response>
        /// <response code="400">BadRequest because of an invalid date used</response>
        [HttpGet("Available")]
        [ProducesResponseType<IEnumerable<Slot>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SlotExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemDetailsExample))]
        public async Task<IActionResult> GetAvailableSlots([FromQuery, BindRequired] DateOnly date, CancellationToken cancellationToken)
        {
            var query = new GetAvailableSlotsQuery(date);
            var slots = await _queryDispatcher.Dispatch<GetAvailableSlotsQuery, IEnumerable<Slot>>(query, cancellationToken);
            return Ok(slots);
        }

        /// <summary>
        /// Takes an available spot in doctor's calendar and save an appointment for the patient
        /// </summary>
        /// <param name="command">
        ///     TakeSlot command including all the information about the slot
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <response code="202">Accepted indicating that the response was successfully processed by Slot service</response>
        /// <response code="400">BadRequest because of an issue coming from Slot service</response>
        [HttpPost("Take")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [SwaggerRequestExample(typeof(TakeSlotCommand), typeof(TakeSlotCommandExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemDetailsExample))]
        public async Task<IActionResult> PostSlot([FromBody, BindRequired] TakeSlotCommand command, CancellationToken cancellationToken)
        {
            var statusCode = await _commandDispatcher.Dispatch<TakeSlotCommand, HttpStatusCode>(command, cancellationToken);
            if (statusCode == HttpStatusCode.OK)
            {
                return Accepted();
            }

            return BadRequest();
        }
    }
}
