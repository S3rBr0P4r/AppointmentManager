using System.Net;
using AppointmentManager.Application.Slots.Commands.TakeSlot;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Services;
using Moq;
using Xunit;

namespace AppointmentManager.Application.Tests.Slots.Commands.TakeSlots
{
    public class TakeSlotCommandHandlerShould
    {
        [Fact]
        public void ThrowArgumentExceptionWhenFacilityIdIsNotValid()
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().WithFacilityId("Invalid GUID").Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("Facility Id must be a valid GUID", exception.Result.Message);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenStartIsMinValue()
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().WithStart(DateTime.MinValue).Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("Start must be a valid date", exception.Result.Message);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenEndIsMaxValue()
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().WithEnd(DateTime.MaxValue).Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("End must be a valid date", exception.Result.Message);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenEndIsBeforeStart()
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().WithEnd(DateTime.Now.AddDays(-1)).Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("Start must be after End", exception.Result.Message);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenStartIsAfterEnd()
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().WithStart(DateTime.Now.AddDays(2)).Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("Start must be after End", exception.Result.Message);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenCommentsIsEmpty()
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().WithComments(string.Empty).Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("Comments must not be empty", exception.Result.Message);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenPatientNameIsEmpty()
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().WithPatientName(string.Empty).Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("Patient Name must not be empty", exception.Result.Message);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenPatientSecondNameIsEmpty()
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().WithPatientSecondName(string.Empty).Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("Patient Second Name must not be empty", exception.Result.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("email")]
        [InlineData("email@")]
        [InlineData("email @   email.com")]
        public void ThrowArgumentExceptionWhenPatientEmailIsNotValid(string email)
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().WithPatientEmail(email).Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("Patient Email must be valid", exception.Result.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("555")]
        [InlineData("555 44")]
        [InlineData("555 44 33")]
        [InlineData("555 44 33 2")]
        [InlineData("555 aa ee 2")]
        public void ThrowArgumentExceptionWhenPatientPhoneIsNotValid(string phone)
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().WithPatientPhone(phone).Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            Assert.Equal("Patient Phone must be valid", exception.Result.Message);
        }

        [Fact]
        public async Task CallSlotsManagementService()
        {
            // Arrange
            var facilityId = Guid.NewGuid().ToString();
            var command = new TakeSlotCommandBuilder().WithFacilityId(facilityId).Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            slotsManagementService.Verify(
                sms => sms.TakeSlotAsync(It.Is<Appointment>(a => a.FacilityId.Equals(facilityId)),
                    CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task ReturnOkWhenSlotIsSuccessfullyBooked()
        {
            // Arrange
            var facilityId = Guid.NewGuid().ToString();
            var command = new TakeSlotCommandBuilder().WithFacilityId(facilityId).Build();
            var slotsManagementService = new Mock<ISlotsManagementService>();
            slotsManagementService.Setup(sms => sms.TakeSlotAsync(
                It.Is<Appointment>(a => a.FacilityId.Equals(facilityId)),
                CancellationToken.None)).ReturnsAsync(HttpStatusCode.OK);
            var sut = new TakeSlotCommandHandler(slotsManagementService.Object);

            // Act
            var statusCode = await sut.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, statusCode);
        }
    }
}
