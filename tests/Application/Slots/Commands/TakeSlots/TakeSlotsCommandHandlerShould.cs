using System.Net;
using AppointmentManager.Application.Slots.Commands.TakeSlots;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Services;
using Moq;
using Xunit;

namespace AppointmentManager.Application.Tests.Slots.Commands.TakeSlots
{
    public class TakeSlotsCommandHandlerShould
    {
        [Fact]
        public async Task CallSlotsManagementService()
        {
            // Arrange
            var facilityId = Guid.NewGuid().ToString();
            var command = new TakeSlotsCommand
            {
                FacilityId = facilityId
            };
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new TakeSlotsCommandHandler(slotsManagementService.Object);

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
            var command = new TakeSlotsCommand
            {
                FacilityId = facilityId
            };
            var slotsManagementService = new Mock<ISlotsManagementService>();
            slotsManagementService.Setup(sms => sms.TakeSlotAsync(
                It.Is<Appointment>(a => a.FacilityId.Equals(facilityId)),
                CancellationToken.None)).ReturnsAsync(HttpStatusCode.OK);
            var sut = new TakeSlotsCommandHandler(slotsManagementService.Object);

            // Act
            var statusCode = await sut.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, statusCode);
        }
    }
}
