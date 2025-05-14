namespace AppointmentManager.Domain.Formatters
{
    public class DateRequestFormatter : IDateRequestFormatter
    {
        private const int October = 10;

        public string GetCompatibleDateWithSlotService(DateOnly date)
        {
            var compatibleDate = date.Year.ToString();
            compatibleDate += GetMonth(date.Month);
            compatibleDate += GetDay(date.Day);

            return compatibleDate;
        }

        private static string GetMonth(int monthNumber)
        {
            return monthNumber < October ? $"0{monthNumber}" : monthNumber.ToString();
        }

        private static string GetDay(int dayNumber)
        {
            return dayNumber < 10 ? $"0{dayNumber}" : dayNumber.ToString();
        }
    }
}
