using System.Net;
using AppointmentManager.Application.Slots.Commands.TakeSlot;
using AppointmentManager.Application.Slots.Queries.GetAvailableSlots;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Infrastructure.Dispatchers;
using AppointmentManager.Presentation.Controllers.v1;
using AppointmentManager.TestUtilities.Builders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AppointmentManager.Presentation.Tests.Controllers.v1
{
    public class SlotControllerShould
    {
        [Fact]
        public async Task ReturnOkWithSlotsWhenGetAvailableSlotsEndpointIsCalled()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 06, 12);
            var queryDispatcher = new Mock<IQueryDispatcher>();
            queryDispatcher.Setup(qd =>
                    qd.Dispatch<GetAvailableSlotsQuery, IEnumerable<Slot>>(
                        It.Is<GetAvailableSlotsQuery>(gasq => gasq.Date.Equals(dateOnly)), CancellationToken.None))
                .ReturnsAsync(new List<Slot> { new() });
            var commandDispatcher = new Mock<ICommandDispatcher>();
            var sut = new SlotsController(queryDispatcher.Object, commandDispatcher.Object);

            // Act
            var actionResult = await sut.GetAvailableSlotsAsync(dateOnly, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(actionResult);
            var response = actionResult as OkObjectResult;
            Assert.IsType<List<Slot>>(response.Value);
            var slots = response.Value as List<Slot>;
            Assert.Equal(1, slots.Count);
        }

        [Fact]
        public async Task ReturnAcceptedWhenSlotIsSuccessfullyStoredInSlotService()
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().Build();
            var queryDispatcher = new Mock<IQueryDispatcher>();
            var commandDispatcher = new Mock<ICommandDispatcher>();
            commandDispatcher
                .Setup(cd => cd.Dispatch<TakeSlotCommand, HttpStatusCode>(It.Is<TakeSlotCommand>(tsc => tsc.Equals(command)), CancellationToken.None))
                .ReturnsAsync(HttpStatusCode.OK);
            var sut = new SlotsController(queryDispatcher.Object, commandDispatcher.Object);

            // Act
            var actionResult = await sut.TakeSlotAsync(command, CancellationToken.None);

            // Assert
            Assert.IsType<AcceptedResult>(actionResult);
        }

        [Fact]
        public async Task ReturnBadRequestWhenSlotIsNotSuccessfullyStoredInSlotService()
        {
            // Arrange
            var command = new TakeSlotCommandBuilder().Build();
            var queryDispatcher = new Mock<IQueryDispatcher>();
            var commandDispatcher = new Mock<ICommandDispatcher>();
            commandDispatcher
                .Setup(cd => cd.Dispatch<TakeSlotCommand, HttpStatusCode>(It.Is<TakeSlotCommand>(tsc => tsc.Equals(command)), CancellationToken.None))
                .ReturnsAsync(HttpStatusCode.BadRequest);
            var sut = new SlotsController(queryDispatcher.Object, commandDispatcher.Object);

            // Act
            var actionResult = await sut.TakeSlotAsync(command, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestResult>(actionResult);
        }
    }
}