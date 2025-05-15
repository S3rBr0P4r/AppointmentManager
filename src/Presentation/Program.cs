using System.Text;
using AppointmentManager.Domain.Services;
using AppointmentManager.Presentation;
using AppointmentManager.Presentation.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDomainDependencies();
builder.Services.AddApplicationDependencies();
builder.Services.AddInfrastructureDependencies();
builder.Services.AddHttpClient<IDoctorShiftService, DoctorShiftService>(client =>
{
    var slotServiceUri = Environment.GetEnvironmentVariable("SLOT_SERVICE_URI");
    var slotServiceUserName = Environment.GetEnvironmentVariable("SLOT_SERVICE_USERNAME");
    var slotServicePassword = Environment.GetEnvironmentVariable("SLOT_SERVICE_PASSWORD");
    client.BaseAddress = new Uri(slotServiceUri);
    client.DefaultRequestHeaders.Authorization
        = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{slotServiceUserName}:{slotServicePassword}")));
});

builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<InternalServerErrorExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
