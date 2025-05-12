using AppointmentManager.Core.Slots;
using AppointmentManager.Domain.Slots;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly ISlotsManagementService _slotsManagementService;

        public SlotsController(ISlotsManagementService slotsManagementService)
        {
            _slotsManagementService = slotsManagementService;
        }

        [HttpGet("AvailableSlots")]
        [ProducesResponseType<Slot>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] string date)
        {
            var slots = _slotsManagementService.GetAvailableSlots();
            return Ok(slots);
        }
    }
}
