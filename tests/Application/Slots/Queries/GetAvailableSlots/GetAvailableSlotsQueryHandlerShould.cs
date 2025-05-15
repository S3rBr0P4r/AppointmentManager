using AppointmentManager.Application.Slots.Queries.GetAvailableSlots;
using AppointmentManager.Domain.Services;
using Moq;
using Xunit;

namespace AppointmentManager.Application.Tests.Slots.Queries.GetAvailableSlots
{
    public class GetAvailableSlotsQueryHandlerShould
    {
        [Fact]
        public async Task CallSlotsManagementService()
        {
            // Arrange
            var query = new GetAvailableSlotsQuery(DateOnly.MaxValue);
            var slotsManagementService = new Mock<ISlotsManagementService>();
            var sut = new GetAvailableSlotsQueryHandler(slotsManagementService.Object);

            // Act
            await sut.Handle(query, CancellationToken.None);

            // Assert
            slotsManagementService.Verify(sms => sms.GetAvailableSlotsAsync(query.Date, CancellationToken.None), Times.Once);
        }
    }
}
