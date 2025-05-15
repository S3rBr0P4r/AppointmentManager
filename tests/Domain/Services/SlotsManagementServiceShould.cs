using AppointmentManager.Domain.Models;
using AppointmentManager.Domain.Services;
using Moq;

namespace AppointmentManager.Domain.Tests.Services
{
    public class SlotsManagementServiceShould
    {
        [Theory]
        [InlineData(60, 5, 9)]
        [InlineData(30, 10, 18)]
        [InlineData(15, 20, 36)]
        public async Task ReturnAvailableSlots(int slotDurationMinutes, int expectedAvailableSlotsForFirstWorkDay, int expectedAvailableSlotsForSecondWorkDay)
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var doctorShiftService = new Mock<IDoctorShiftService>();
            doctorShiftService
                .Setup(dss => dss.GetSlotsInformationAsync(dateOnly, CancellationToken.None))
                .ReturnsAsync(() => new SlotsInformation
                {
                    SlotDurationMinutes = slotDurationMinutes,
                    WorkDays = new List<WorkDay>
                    {
                        new()
                        {
                            Day = dateOnly,
                            WorkPeriod = new WorkPeriod { StartHour = 10, LunchStartHour = 13, LunchEndHour = 17, EndHour = 19 }
                        },
                        new()
                        {
                            Day = dateOnly.AddDays(1),
                            WorkPeriod = new WorkPeriod { StartHour = 9, LunchStartHour = 14, LunchEndHour = 16, EndHour = 20 }
                        }
                    }
                });
            
            var sut = new SlotsManagementService(doctorShiftService.Object);

            // Act
            var slots = (await sut.GetAvailableSlotsAsync(dateOnly, CancellationToken.None)).ToList();

            // Assert
            Assert.Equal(expectedAvailableSlotsForFirstWorkDay + expectedAvailableSlotsForSecondWorkDay, slots.Count);
            var availableSlotsForFirstWorkDay = slots.Count(s => DateOnly.FromDateTime(s.Start).Equals(dateOnly));
            Assert.Equal(expectedAvailableSlotsForFirstWorkDay, availableSlotsForFirstWorkDay);
            var availableSlotsForSecondWorkDay = slots.Count(s => DateOnly.FromDateTime(s.Start).Equals(dateOnly.AddDays(1)));
            Assert.Equal(expectedAvailableSlotsForSecondWorkDay, availableSlotsForSecondWorkDay);
        }

        [Fact]
        public async Task ReturnSlotsTakingCareBusySlots()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var slotDurationMinutes = 60;
            var expectedAvailableSlots = 4;
            var doctorShiftService = new Mock<IDoctorShiftService>();
            doctorShiftService
                .Setup(dss => dss.GetSlotsInformationAsync(dateOnly, CancellationToken.None))
                .ReturnsAsync(() => new SlotsInformation
                {
                    SlotDurationMinutes = slotDurationMinutes,
                    WorkDays = new List<WorkDay>
                    {
                        new WorkDay
                        {
                            Day = dateOnly,
                            WorkPeriod = new WorkPeriod { StartHour = 10, LunchStartHour = 13, LunchEndHour = 17, EndHour = 19 },
                            BusySlots =
                            [ new BusySlot { Start = new DateTime(2025, 11, 20, 10, 0, 0), End = new DateTime(2025, 11, 20, 11, 0, 0) } ]
                        }
                    }
                });
            var sut = new SlotsManagementService(doctorShiftService.Object);

            // Act
            var slots = await sut.GetAvailableSlotsAsync(dateOnly, CancellationToken.None);

            // Assert
            Assert.Equal(expectedAvailableSlots, slots.Count());
        }
    }
}