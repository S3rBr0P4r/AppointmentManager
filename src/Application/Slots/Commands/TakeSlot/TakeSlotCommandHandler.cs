using System.Net;
using System.Text;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Handlers;
using AppointmentManager.Domain.Services;
using FluentValidation.Results;

namespace AppointmentManager.Application.Slots.Commands.TakeSlot
{
    public class TakeSlotCommandHandler : ICommandHandler<TakeSlotCommand, HttpStatusCode>
    {
        private readonly ISlotsManagementService _slotsManagementService;
        private readonly TakeSlotCommandValidator _takeSlotCommandValidator;

        public TakeSlotCommandHandler(ISlotsManagementService slotsManagementService)
        {
            _slotsManagementService = slotsManagementService;
            _takeSlotCommandValidator = new TakeSlotCommandValidator();
        }

        public async Task<HttpStatusCode> Handle(TakeSlotCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _takeSlotCommandValidator.ValidateAsync(command, cancellationToken);
            EnsureCommandIsValid(validationResult);

            var appointment = new Appointment(command.FacilityId, command.Comments, command.Patient)
            {
                Start = command.Start,
                End = command.End
            };
            var statusCode = await _slotsManagementService.TakeSlotAsync(appointment, cancellationToken);
            return statusCode;
        }

        private static void EnsureCommandIsValid(ValidationResult commandValidationResult)
        {
            if (commandValidationResult.Errors.Any())
            {
                var errorMessage = new StringBuilder();
                foreach (var error in commandValidationResult.Errors)
                {
                    errorMessage.AppendLine(error.ErrorMessage);
                }
                throw new ArgumentException(errorMessage.ToString().TrimEnd('\r', '\n'));
            }
        }
    }
}
