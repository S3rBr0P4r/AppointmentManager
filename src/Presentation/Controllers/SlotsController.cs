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

        public SlotsController(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet("AvailableSlots")]
        [ProducesResponseType<IEnumerable<Slot>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] DateOnly date, CancellationToken cancellationToken)
        {
            var query = new GetAvailableSlotsQuery(date);
            var slots = await _queryDispatcher.Dispatch<GetAvailableSlotsQuery, IEnumerable<Slot>>(query, cancellationToken);
            return Ok(slots);
        }
    }
}
