using System.Net;
using AppointmentManager.Application.Slots.Commands.TakeSlots;
using AppointmentManager.Application.Slots.Queries.GetAvailableSlots;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Infrastructure.Dispatchers;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public SlotsController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
        }

        [HttpGet("Available")]
        [ProducesResponseType<IEnumerable<Slot>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] DateOnly date, CancellationToken cancellationToken)
        {
            var query = new GetAvailableSlotsQuery(date);
            var slots = await _queryDispatcher.Dispatch<GetAvailableSlotsQuery, IEnumerable<Slot>>(query, cancellationToken);
            return Ok(slots);
        }

        [HttpPost("Take")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSlot([FromBody] TakeSlotsCommand command, CancellationToken cancellationToken)
        {
            var statusCode = await _commandDispatcher.Dispatch<TakeSlotsCommand, HttpStatusCode>(command, cancellationToken);
            if (statusCode == HttpStatusCode.Accepted)
            {
                return Accepted();
            }

            return BadRequest();
        }
    }
}
