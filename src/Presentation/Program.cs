using System.Text;
using AppointmentManager.Domain.Services;
using AppointmentManager.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCoreDependencies();
builder.Services.AddHttpClient<ISlotsManagementService, SlotsManagementService>(client =>
{
    var slotServiceEndpoint = Environment.GetEnvironmentVariable("SLOT_SERVICE_ENDPOINT");
    var slotServiceEndpointUserName = Environment.GetEnvironmentVariable("SLOT_SERVICE_ENDPOINT_USERNAME");
    var slotServiceEndpointPassword = Environment.GetEnvironmentVariable("SLOT_SERVICE_ENDPOINT_PASSWORD");
    client.BaseAddress = new Uri(slotServiceEndpoint);
    client.DefaultRequestHeaders.Authorization
        = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{slotServiceEndpointUserName}:{slotServiceEndpointPassword}")));
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
