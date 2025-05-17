using System;
using System.Net;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Models;
using AppointmentManager.Domain.Services;
using Microsoft.Extensions.Logging;
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
            var logger = new Mock<ILogger<SlotsManagementService>>();
            var sut = new SlotsManagementService(logger.Object, doctorShiftService.Object);

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
            var logger = new Mock<ILogger<SlotsManagementService>>();
            var sut = new SlotsManagementService(logger.Object, doctorShiftService.Object);

            // Act
            var slots = await sut.GetAvailableSlotsAsync(dateOnly, CancellationToken.None);

            // Assert
            Assert.Equal(expectedAvailableSlots, slots.Count());
        }

        [Fact]
        public async Task LogInformationAboutSlots()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var slotDurationMinutes = 60;
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
            var logger = new Mock<ILogger<SlotsManagementService>>();
            var sut = new SlotsManagementService(logger.Object, doctorShiftService.Object);

            // Act
            await sut.GetAvailableSlotsAsync(dateOnly, CancellationToken.None);

            // Assert
            logger.Verify(l =>
                l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((value, _) => 
                        value.ToString()!.Equals("Slot from 11/20/2025 10:00:00 till 11/20/2025 11:00:00 already booked")),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Once
            );
            logger.Verify(l =>
                l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((value, _) => 
                        value.ToString()!.Equals("4 available slot(s) found with a duration of 60 minutes each of them")),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Once
            );
        }

        [Fact]
        public async Task ReturnOkWhenSlotIsSuccessfullyBookedByPatient()
        {
            // Arrange
            var facilityId = Guid.NewGuid().ToString();
            var appointment = new Appointment(facilityId, "Pain in left arm",
                new Patient("Sergio", "Brotons", "s3rbr0p4r@email.com", "555 66 77 88"))
            {
                Start = new DateTime(2025, 11, 20, 10, 0, 0),
                End = new DateTime(2025, 11, 20, 11, 0, 0),
            };
            var doctorShiftService = new Mock<IDoctorShiftService>();
            doctorShiftService
                .Setup(dss => dss.AddSlotToShiftAsync(
                    It.Is<Appointment>(a => a.FacilityId.Equals(facilityId)),
                    CancellationToken.None))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("")
                });
            var logger = new Mock<ILogger<SlotsManagementService>>();
            var sut = new SlotsManagementService(logger.Object, doctorShiftService.Object);

            // Act
            var statusCode = await sut.TakeSlotAsync(appointment, CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task LogRegistryWhenSlotIsSuccessfullyBookedByPatient()
        {
            // Arrange
            var facilityId = Guid.NewGuid().ToString();
            var appointment = new Appointment(facilityId, "Pain in left arm",
                new Patient("Sergio", "Brotons", "s3rbr0p4r@email.com", "555 66 77 88"))
            {
                Start = new DateTime(2025, 11, 20, 10, 0, 0),
                End = new DateTime(2025, 11, 20, 11, 0, 0),
            };
            var doctorShiftService = new Mock<IDoctorShiftService>();
            doctorShiftService
                .Setup(dss => dss.AddSlotToShiftAsync(
                    It.Is<Appointment>(a => a.FacilityId.Equals(facilityId)),
                    CancellationToken.None))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("")
                });
            var logger = new Mock<ILogger<SlotsManagementService>>();
            var sut = new SlotsManagementService(logger.Object, doctorShiftService.Object);

            // Act
            await sut.TakeSlotAsync(appointment, CancellationToken.None);

            // Assert
            logger.Verify(l =>
                l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((value, _) =>
                        value.ToString()!.Equals("Slot from 11/20/2025 10:00:00 till 11/20/2025 11:00:00 booked to patient Sergio Brotons")),
                    It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Once
            );
        }

        [Fact]
        public void ThrowsBadRequestExceptionWhenSlotIsNotSuccessfullyBookedByPatient()
        {
            // Arrange
            var facilityId = Guid.NewGuid().ToString();
            var appointment = new Appointment(facilityId, "Pain in left arm",
                new Patient("Sergio", "Brotons", "s3rbr0p4r@email.com", "555 66 77 88"))
            {
                Start = new DateTime(2025, 11, 20, 10, 0, 0),
                End = new DateTime(2025, 11, 20, 11, 0, 0),
            };
            var doctorShiftService = new Mock<IDoctorShiftService>();
            doctorShiftService
                .Setup(dss => dss.AddSlotToShiftAsync(
                    It.Is<Appointment>(a => a.FacilityId.Equals(facilityId)),
                    CancellationToken.None))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Valid slot required")
                });
            var logger = new Mock<ILogger<SlotsManagementService>>();
            var sut = new SlotsManagementService(logger.Object, doctorShiftService.Object);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.TakeSlotAsync(appointment, CancellationToken.None));

            // Assert
            Assert.Contains("Valid slot required", exception.Result.Message);
        }
    }
}