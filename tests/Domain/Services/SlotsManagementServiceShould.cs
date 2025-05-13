using System.Net;
using AppointmentManager.Domain.Services;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Moq.Protected;

namespace AppointmentManager.Domain.Tests.Services
{
    public class SlotsManagementServiceShould
    {
        [Theory]
        [InlineData(60, 7)]
        [InlineData(30, 14)]
        [InlineData(15, 28)]
        public async Task ReturnSlotsForMondayNotHavingBusySlots(int slotDurationMinutes, int expectedAvailableSlots)
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var responseContent = $@"
                                  {{
                                      ""Facility"": {{
                                          ""FacilityId"": ""794fad3e-6734-4773-b221-e744e11bbb5a"",
                                          ""Name"": ""Las Palmeras"",
                                          ""Address"": ""Plaza de la independencia 36, 38006 Santa Cruz de Tenerife""
                                      }},
                                      ""SlotDurationMinutes"": {slotDurationMinutes},
                                      ""Monday"": {{
                                          ""WorkPeriod"": {{
                                              ""StartHour"": 9,
                                              ""EndHour"": 17,
                                              ""LunchStartHour"": 13,
                                              ""LunchEndHour"": 14
                                          }}
                                      }}
                                  }}
                                  ";
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
            var sut = new SlotsManagementService(client);

            // Act
            var slots = await sut.GetAvailableSlotsAsync(dateOnly);

            // Assert
            Assert.Equal(expectedAvailableSlots, slots.Count());
        }

        [Fact]
        public async Task ReturnSlotsForMondayHavingBusySlots()
        {
            // Arrange
            var dateOnly = new DateOnly(2025, 11, 20);
            var slotDurationMinutes = 60;
            var expectedAvailableSlots = 6;
            var responseContent = $@"
                                  {{
                                      ""Facility"": {{
                                          ""FacilityId"": ""794fad3e-6734-4773-b221-e744e11bbb5a"",
                                          ""Name"": ""Las Palmeras"",
                                          ""Address"": ""Plaza de la independencia 36, 38006 Santa Cruz de Tenerife""
                                      }},
                                      ""SlotDurationMinutes"": {slotDurationMinutes},
                                      ""Monday"": {{
                                          ""WorkPeriod"": {{
                                              ""StartHour"": 9,
                                              ""EndHour"": 17,
                                              ""LunchStartHour"": 13,
                                              ""LunchEndHour"": 14
                                          }},
                                          ""BusySlots"": [
			                                {{
				                                ""Start"": ""2025-11-20T10:00:00"",
				                                ""End"": ""2025-11-20T11:00:00""
			                                }}
		                                  ]
                                      }}
                                  }}
                                  ";
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
            var sut = new SlotsManagementService(client);

            // Act
            var slots = await sut.GetAvailableSlotsAsync(dateOnly);

            // Assert
            Assert.Equal(expectedAvailableSlots, slots.Count());
        }
    }
}