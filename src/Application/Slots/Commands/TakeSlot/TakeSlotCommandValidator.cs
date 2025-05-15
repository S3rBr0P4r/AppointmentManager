using FluentValidation;

namespace AppointmentManager.Application.Slots.Commands.TakeSlot
{
    public class TakeSlotCommandValidator : AbstractValidator<TakeSlotCommand>
    {
        private const string GuidRegexPattern =
            @"[a-fA-F\d]{8}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{4}-[a-fA-F\d]{12}";

        private const string PhoneRegexPattern = @"\d{3}\ \d{2}\ \d{2}\ \d{2}";

        public TakeSlotCommandValidator()
        {
            RuleFor(c => c.FacilityId).Matches(GuidRegexPattern).WithMessage("{PropertyName} must be a valid GUID");
            RuleFor(c => c.Start).Must(s => s != DateTime.MinValue && s != DateTime.MaxValue).WithMessage("{PropertyName} must be a valid date");
            RuleFor(c => c.End).Must(s => s != DateTime.MinValue && s != DateTime.MaxValue).WithMessage("{PropertyName} must be a valid date");
            RuleFor(c => c.End).GreaterThan(s => s.Start).WithMessage("Start must be after End");
            RuleFor(c => c.Comments).NotEmpty().WithMessage("{PropertyName} must not be empty");
            RuleFor(c => c.Patient.Name).NotEmpty().WithMessage("{PropertyName} must not be empty");
            RuleFor(c => c.Patient.SecondName).NotEmpty().WithMessage("{PropertyName} must not be empty");
            RuleFor(c => c.Patient.Email).Must(e => !e.Any(char.IsWhiteSpace)).WithMessage("{PropertyName} must be valid");
            RuleFor(c => c.Patient.Email).EmailAddress().WithMessage("{PropertyName} must be valid");
            RuleFor(c => c.Patient.Phone).Matches(PhoneRegexPattern).WithMessage("{PropertyName} must be valid");
        }
    }
}
