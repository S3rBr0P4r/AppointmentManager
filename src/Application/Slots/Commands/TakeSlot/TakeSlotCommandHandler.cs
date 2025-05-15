using System.Net;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Handlers;
using AppointmentManager.Domain.Services;

namespace AppointmentManager.Application.Slots.Commands.TakeSlot
{
    public class TakeSlotCommandHandler : ICommandHandler<TakeSlotCommand, HttpStatusCode>
    {
        private readonly ISlotsManagementService _slotsManagementService;

        public TakeSlotCommandHandler(ISlotsManagementService slotsManagementService)
        {
            _slotsManagementService = slotsManagementService;
        }

        public async Task<HttpStatusCode> Handle(TakeSlotCommand command, CancellationToken cancellationToken)
        {
            var appointment = new Appointment(command.FacilityId, command.Comments, command.Patient)
            {
                Start = command.Start,
                End = command.End
            };
            var statusCode = await _slotsManagementService.TakeSlotAsync(appointment, cancellationToken);
            return statusCode;
        }
    }
}
