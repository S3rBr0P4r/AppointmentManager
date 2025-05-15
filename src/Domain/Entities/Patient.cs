namespace AppointmentManager.Domain.Entities
{
    public class Patient(string name, string secondName, string email, string phone)
    {
        public string Name { get; set; } = name;
        public string SecondName { get; set; } = secondName;
        public string Email { get; set; } = email;
        public string Phone { get; set; } = phone;
    }
}
