# Appointment Manager
Doctors offer slots to patients. A slot is a period of time which the patient could ask
for a visit. The doctor defines a slot duration (for example, 20 minutes) and
determines the work period (from 8 am to 1 pm, for example). The doctor expects
that the patient will be able to see available slots and book an appointment.
The user should be able to see slots by week, select one and fill in the required data
to book it.

## Steps to run Appointment Manager

1. Build the solution

2. Set as startup project `AppointmentManager.Presentation` and run the application using the profile `Development`. Swagger must be visible on `http://localhost:5292`


## Project configuration

| Scope  | Resources(s)  |
|---|---|
| Backend | .NET 8 LTS  |
| Frontend  | Swagger |
| Testing  | xUnit |
| Mock frameworks  | Moq |
| Software architecture patterns | RESTful API design
| Design patterns | CQRS
| | SOLID principles
| Development tools | Visual Studio Professional 2022, Version 17.13.6
| | Visual Studio Code 1.100.1

## Observations

In this project, clean architecture has been applied as better as possible. Any feedback about it is more than welcome.

## Next steps

* Improve logging using Serilog instead of ILogger
* Add authorization in API write operations
