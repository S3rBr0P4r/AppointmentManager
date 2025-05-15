using System.Net;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Handlers;
using AppointmentManager.Domain.Services;

namespace AppointmentManager.Application.Slots.Commands.TakeSlots
{
    public class TakeSlotsCommandHandler : ICommandHandler<TakeSlotsCommand, HttpStatusCode>
    {
        private readonly ISlotsManagementService _slotsManagementService;

        public TakeSlotsCommandHandler(ISlotsManagementService slotsManagementService)
        {
            _slotsManagementService = slotsManagementService;
        }

        public async Task<HttpStatusCode> Handle(TakeSlotsCommand command, CancellationToken cancellationToken)
        {
            var appointment = new Appointment(command.FacilityId, command.Comments, command.Patient)
            {
                Start = command.Start,
                End = command.End
            };
            await _slotsManagementService.TakeSlotAsync(appointment, cancellationToken);
            return HttpStatusCode.Accepted;
        }
    }
}
