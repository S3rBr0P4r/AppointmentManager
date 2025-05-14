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
        public async Task ReturnShiftForMonday()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var responseContent = """
                                  {
                                      "Facility": {
                                          "FacilityId": "794fad3e-6734-4773-b221-e744e11bbb5a",
                                          "Name": "Las Palmeras",
                                          "Address": "Plaza de la independencia 36, 38006 Santa Cruz de Tenerife"
                                      },
                                      "SlotDurationMinutes": 60,
                                      "Monday": {
                                          "WorkPeriod": {
                                              "StartHour": 10,
                                              "EndHour": 13,
                                              "LunchStartHour": 17,
                                              "LunchEndHour": 19
                                          },
                                  		"BusySlots": [
                                  			{
                                  				"Start": "2025-11-20T10:00:00",
                                  				"End": "2025-11-20T11:00:00"
                                  			}
                                  		]
                                      }
                                  }
                                  """;
            var handler = new Mock<HttpMessageHandler>();
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                })
                .Verifiable();

            var client = new HttpClient(handler.Object);
            client.BaseAddress = new Uri("https://draliatest.azurewebsites.net/api/availability/GetWeeklyAvailability/");
            var dateRequestFormatter = new DateRequestFormatter();
            var sut = new DoctorShiftService(dateRequestFormatter, client);

            // Act
            var workDays = (await sut.GetWorkDaysShiftAsync(dateOnly)).ToList();

            // Assert
            Assert.Single(workDays);
            var workDay = workDays.First();
            Assert.Equal(10, workDay.WorkPeriod.StartHour);
            Assert.Equal(13, workDay.WorkPeriod.EndHour);
            Assert.Equal(17, workDay.WorkPeriod.LunchStartHour);
            Assert.Equal(19, workDay.WorkPeriod.LunchEndHour);
            Assert.Single(workDay.BusySlots);
            var busySlot = workDay.BusySlots.First();
            Assert.Equal(new DateTime(2025, 11, 20, 10,0,0), busySlot.Start);
            Assert.Equal(new DateTime(2025, 11, 20, 11,0,0), busySlot.End);
        }
    }
}
