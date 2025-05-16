using AppointmentManager.Application.Slots.Commands.TakeSlot;
using AppointmentManager.Domain.Entities;

namespace AppointmentManager.TestUtilities.Builders
{
    public class TakeSlotCommandBuilder
    {
        private readonly TakeSlotCommand _command;

        public TakeSlotCommandBuilder()
        {
            _command = new TakeSlotCommand
            {
                FacilityId = Guid.NewGuid().ToString(),
                Start = DateTime.Now,
                End = DateTime.Now.AddMinutes(10),
                Comments = "Something is wrong",
                Patient = new Patient("Name", "SecondName", "email@email.com", "111 22 33 44")
            };
        }

        public TakeSlotCommandBuilder WithFacilityId(string facilityId)
        {
            _command.FacilityId = facilityId;
            return this;
        }

        public TakeSlotCommandBuilder WithStart(DateTime start)
        {
            _command.Start = start;
            return this;
        }

        public TakeSlotCommandBuilder WithEnd(DateTime end)
        {
            _command.End = end;
            return this;
        }

        public TakeSlotCommandBuilder WithComments(string comments)
        {
            _command.Comments = comments;
            return this;
        }

        public TakeSlotCommandBuilder WithPatientName(string patientName)
        {
            _command.Patient.Name = patientName;
            return this;
        }

        public TakeSlotCommandBuilder WithPatientSecondName(string patientSecondName)
        {
            _command.Patient.SecondName = patientSecondName;
            return this;
        }

        public TakeSlotCommandBuilder WithPatientEmail(string patientEmail)
        {
            _command.Patient.Email = patientEmail;
            return this;
        }

        public TakeSlotCommandBuilder WithPatientPhone(string patientPhone)
        {
            _command.Patient.Phone = patientPhone;
            return this;
        }

        public TakeSlotCommand Build()
        {
            return _command;
        }
    }
}
