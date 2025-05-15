using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Services;
using AppointmentManager.Infrastructure.Handlers;

namespace AppointmentManager.Application.Slots.Queries.GetAvailableSlots
{
    public class GetAvailableSlotsQueryHandler : IQueryHandler<GetAvailableSlotsQuery, IEnumerable<Slot>>
    {
        private readonly ISlotsManagementService _slotsManagementService;
        public GetAvailableSlotsQueryHandler(ISlotsManagementService slotsManagementService)
        {
            _slotsManagementService = slotsManagementService;
        }

        public async Task<IEnumerable<Slot>> Handle(GetAvailableSlotsQuery query, CancellationToken cancellationToken)
        {
            return await _slotsManagementService.GetAvailableSlotsAsync(query.Date, cancellationToken);
        }
    }
}
