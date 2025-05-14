using AppointmentManager.Domain.Formatters;

namespace AppointmentManager.Domain.Tests.Formatters
{
    public class DateRequestFormatterShould
    {
        [Theory]
        [MemberData(nameof(Dates))]
        public void ReturnValidDateForSlotServiceEndpoint(DateOnly date, string expectedDateForEndpoint)
        {
            // Arrange
            var sut = new DateRequestFormatter();

            // Act
            var compatibleDate = sut.GetCompatibleDateWithSlotService(date);

            // Assert
            Assert.Equal(expectedDateForEndpoint, compatibleDate);
        }

        public static IEnumerable<object[]> Dates =>
        [
            [new DateOnly(2025, 1, 2), "20250102"],
            [new DateOnly(2025, 10, 2), "20251002"],
            [new DateOnly(2025, 8, 20), "20250820"],
            [new DateOnly(2025, 12, 12), "20251212"]
        ];
    }
}
