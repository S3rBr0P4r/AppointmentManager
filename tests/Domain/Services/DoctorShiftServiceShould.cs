using AppointmentManager.Domain.Formatters;
using AppointmentManager.Domain.Services;
using Moq.Protected;
using Moq;
using System.Net;

namespace AppointmentManager.Domain.Tests.Services
{
    public class DoctorShiftServiceShould
    {
        [Fact]
        public async Task ReturnSlotDurationMinutes()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var responseContent = """
                                  { "SlotDurationMinutes": 60 }
                                  """;
            var handler = SetupHttpMessageHandlerResponse(responseContent, HttpStatusCode.OK);
            var client = SetupHttpClient(handler);
            var dateRequestFormatter = new DateRequestFormatter();
            var sut = new DoctorShiftService(dateRequestFormatter, client);

            // Act
            var slotsInformation = await sut.GetSlotsInformationAsync(dateOnly, CancellationToken.None);

            // Assert
            Assert.Equal(60, slotsInformation.SlotDurationMinutes);
        }

        [Fact]
        public async Task ReturnSlotsInformationForMonday()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var responseContent = """
                                  {
                                      "SlotDurationMinutes": 60,
                                      "Monday": {
                                        "WorkPeriod": { "StartHour": 10, "LunchStartHour": 13, "LunchEndHour": 17, "EndHour": 19 },
                                  		"BusySlots": [{ "Start": "2025-11-20T10:00:00", "End": "2025-11-20T11:00:00" }]
                                      }
                                  }
                                  """;
            var handler = SetupHttpMessageHandlerResponse(responseContent, HttpStatusCode.OK);
            var client = SetupHttpClient(handler);
            var dateRequestFormatter = new DateRequestFormatter();
            var sut = new DoctorShiftService(dateRequestFormatter, client);

            // Act
            var slotsInformation = await sut.GetSlotsInformationAsync(dateOnly, CancellationToken.None);

            // Assert
            Assert.Single(slotsInformation.WorkDays);
            var workDay = slotsInformation.WorkDays.First();
            Assert.Equal(10, workDay.WorkPeriod.StartHour);
            Assert.Equal(13, workDay.WorkPeriod.LunchStartHour);
            Assert.Equal(17, workDay.WorkPeriod.LunchEndHour);
            Assert.Equal(19, workDay.WorkPeriod.EndHour);
            Assert.Single(workDay.BusySlots);
            var busySlot = workDay.BusySlots.First();
            Assert.Equal(new DateTime(2025, 11, 20, 10,0,0), busySlot.Start);
            Assert.Equal(new DateTime(2025, 11, 20, 11,0,0), busySlot.End);
        }

        [Fact]
        public async Task ReturnSlotsInformationForTuesday()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var responseContent = """
                                  {
                                      "SlotDurationMinutes": 60,
                                      "Tuesday": {
                                        "WorkPeriod": { "StartHour": 9, "LunchStartHour": 12, "LunchEndHour": 16, "EndHour": 18 },
                                  		"BusySlots": [{ "Start": "2025-11-20T11:00:00", "End": "2025-11-20T12:00:00" }]
                                      }
                                  }
                                  """;
            var handler = SetupHttpMessageHandlerResponse(responseContent, HttpStatusCode.OK);
            var client = SetupHttpClient(handler);
            var dateRequestFormatter = new DateRequestFormatter();
            var sut = new DoctorShiftService(dateRequestFormatter, client);

            // Act
            var slotsInformation = await sut.GetSlotsInformationAsync(dateOnly, CancellationToken.None);

            // Assert
            Assert.Single(slotsInformation.WorkDays);
            var workDay = slotsInformation.WorkDays.First();
            Assert.Equal(9, workDay.WorkPeriod.StartHour);
            Assert.Equal(12, workDay.WorkPeriod.LunchStartHour);
            Assert.Equal(16, workDay.WorkPeriod.LunchEndHour);
            Assert.Equal(18, workDay.WorkPeriod.EndHour);
            Assert.Single(workDay.BusySlots);
            var busySlot = workDay.BusySlots.First();
            Assert.Equal(new DateTime(2025, 11, 20, 11, 0, 0), busySlot.Start);
            Assert.Equal(new DateTime(2025, 11, 20, 12, 0, 0), busySlot.End);
        }

        [Fact]
        public async Task ReturnSlotsInformationForWednesday()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var responseContent = """
                                  {
                                      "SlotDurationMinutes": 60,
                                      "Wednesday": {
                                        "WorkPeriod": { "StartHour": 8, "LunchStartHour": 11, "LunchEndHour": 15, "EndHour": 19 },
                                  		"BusySlots": [{ "Start": "2025-11-20T15:00:00", "End": "2025-11-20T16:00:00" }]
                                      }
                                  }
                                  """;
            var handler = SetupHttpMessageHandlerResponse(responseContent, HttpStatusCode.OK);
            var client = SetupHttpClient(handler);
            var dateRequestFormatter = new DateRequestFormatter();
            var sut = new DoctorShiftService(dateRequestFormatter, client);

            // Act
            var slotsInformation = await sut.GetSlotsInformationAsync(dateOnly, CancellationToken.None);

            // Assert
            Assert.Single(slotsInformation.WorkDays);    
            var workDay = slotsInformation.WorkDays.First();
            Assert.Equal(8, workDay.WorkPeriod.StartHour);
            Assert.Equal(11, workDay.WorkPeriod.LunchStartHour);
            Assert.Equal(15, workDay.WorkPeriod.LunchEndHour);
            Assert.Equal(19, workDay.WorkPeriod.EndHour);
            Assert.Single(workDay.BusySlots);
            var busySlot = workDay.BusySlots.First();
            Assert.Equal(new DateTime(2025, 11, 20, 15, 0, 0), busySlot.Start);
            Assert.Equal(new DateTime(2025, 11, 20, 16, 0, 0), busySlot.End);
        }

        [Fact]
        public async Task ReturnSlotsInformationForThursday()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var responseContent = """
                                  {
                                      "SlotDurationMinutes": 60,
                                      "Thursday": {
                                        "WorkPeriod": { "StartHour": 8, "LunchStartHour": 13, "LunchEndHour": 16, "EndHour": 20 },
                                  		"BusySlots": [{ "Start": "2025-11-20T17:00:00", "End": "2025-11-20T18:00:00" }]
                                      }
                                  }
                                  """;
            var handler = SetupHttpMessageHandlerResponse(responseContent, HttpStatusCode.OK);
            var client = SetupHttpClient(handler);
            var dateRequestFormatter = new DateRequestFormatter();
            var sut = new DoctorShiftService(dateRequestFormatter, client);

            // Act
            var slotsInformation = await sut.GetSlotsInformationAsync(dateOnly, CancellationToken.None);

            // Assert
            Assert.Single(slotsInformation.WorkDays);
            var workDay = slotsInformation.WorkDays.First();
            Assert.Equal(8, workDay.WorkPeriod.StartHour);
            Assert.Equal(13, workDay.WorkPeriod.LunchStartHour);
            Assert.Equal(16, workDay.WorkPeriod.LunchEndHour);
            Assert.Equal(20, workDay.WorkPeriod.EndHour);
            Assert.Single(workDay.BusySlots);
            var busySlot = workDay.BusySlots.First();
            Assert.Equal(new DateTime(2025, 11, 20, 17, 0, 0), busySlot.Start);
            Assert.Equal(new DateTime(2025, 11, 20, 18, 0, 0), busySlot.End);
        }

        [Fact]
        public async Task ReturnSlotsInformationForFriday()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var responseContent = """
                                  {
                                      "SlotDurationMinutes": 60,
                                      "Friday": {
                                        "WorkPeriod": { "StartHour": 8, "LunchStartHour": 14, "LunchEndHour": 16, "EndHour": 19 },
                                  		"BusySlots": [{ "Start": "2025-11-20T18:00:00", "End": "2025-11-20T19:00:00" }]
                                      }
                                  }
                                  """;
            var handler = SetupHttpMessageHandlerResponse(responseContent, HttpStatusCode.OK);
            var client = SetupHttpClient(handler);
            var dateRequestFormatter = new DateRequestFormatter();
            var sut = new DoctorShiftService(dateRequestFormatter, client);

            // Act
            var slotsInformation = await sut.GetSlotsInformationAsync(dateOnly, CancellationToken.None);

            // Assert
            Assert.Single(slotsInformation.WorkDays);
            var workDay = slotsInformation.WorkDays.First();
            Assert.Equal(8, workDay.WorkPeriod.StartHour);
            Assert.Equal(14, workDay.WorkPeriod.LunchStartHour);
            Assert.Equal(16, workDay.WorkPeriod.LunchEndHour);
            Assert.Equal(19, workDay.WorkPeriod.EndHour);
            Assert.Single(workDay.BusySlots);
            var busySlot = workDay.BusySlots.First();
            Assert.Equal(new DateTime(2025, 11, 20, 18, 0, 0), busySlot.Start);
            Assert.Equal(new DateTime(2025, 11, 20, 19, 0, 0), busySlot.End);
        }

        [Fact]
        public async Task ReturnRightDateForEachWorkDay()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var responseContent = """
                                  {
                                      "SlotDurationMinutes": 60,
                                      "Thursday": {
                                        "WorkPeriod": { "StartHour": 8, "LunchStartHour": 14, "LunchEndHour": 16, "EndHour": 19 },
                                    	"BusySlots": [{ "Start": "2025-11-23T18:00:00", "End": "2025-11-23T19:00:00" }]
                                      },
                                      "Friday": {
                                        "WorkPeriod": { "StartHour": 8, "LunchStartHour": 14, "LunchEndHour": 16, "EndHour": 19 },
                                  		"BusySlots": [{ "Start": "2025-11-24T18:00:00", "End": "2025-11-24T19:00:00" }]
                                      }
                                  }
                                  """;
            var handler = SetupHttpMessageHandlerResponse(responseContent, HttpStatusCode.OK);
            var client = SetupHttpClient(handler);
            var dateRequestFormatter = new DateRequestFormatter();
            var sut = new DoctorShiftService(dateRequestFormatter, client);

            // Act
            var slotsInformation = await sut.GetSlotsInformationAsync(dateOnly, CancellationToken.None);

            // Assert
            Assert.Equal(2, slotsInformation.WorkDays.Count());
            var thursday = slotsInformation.WorkDays.First();
            Assert.Equal(new DateOnly(2025, 11, 23), thursday.Day);
            var friday = slotsInformation.WorkDays.Last();
            Assert.Equal(new DateOnly(2025, 11, 24), friday.Day);
        }

        [Fact]
        public void ReturnArgumentExceptionWhenDateRequestIsNotMonday()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var responseContent = "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">datetime must be a Monday</string>";
            var handler = SetupHttpMessageHandlerResponse(responseContent, HttpStatusCode.BadRequest);
            var client = SetupHttpClient(handler);
            var dateRequestFormatter = new DateRequestFormatter();
            var sut = new DoctorShiftService(dateRequestFormatter, client);

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await sut.GetSlotsInformationAsync(dateOnly, CancellationToken.None));

            // Assert
            Assert.Contains($"DateTime '{dateOnly}' must be Monday", exception.Result.Message);
        }

        [Fact]
        public void ReturnArgumentExceptionWhenSlotServiceIsDown()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var responseContent = "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">Service is down</string>";
            var handler = SetupHttpMessageHandlerResponse(responseContent, HttpStatusCode.InternalServerError);
            var client = SetupHttpClient(handler);
            var dateRequestFormatter = new DateRequestFormatter();
            var sut = new DoctorShiftService(dateRequestFormatter, client);

            // Act
            var exception = Assert.ThrowsAsync<TimeoutException>(async () => await sut.GetSlotsInformationAsync(dateOnly, CancellationToken.None));

            // Assert
            Assert.Contains("Slot service is down and work days shift cannot be retrieved", exception.Result.Message);
        }

        private static HttpClient SetupHttpClient(Mock<HttpMessageHandler> handler)
        {
            var client = new HttpClient(handler.Object);
            client.BaseAddress = new Uri("https://draliatest.azurewebsites.net/api/availability/GetWeeklyAvailability/");
            return client;
        }

        private static Mock<HttpMessageHandler> SetupHttpMessageHandlerResponse(string responseContent, HttpStatusCode statusCode)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseContent)
                })
                .Verifiable();
            return handler;
        }
    }
}
