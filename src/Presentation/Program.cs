using AppointmentManager.Presentation.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.EnableSerilog();

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddPresentationDependencies();
builder.Services.AddDomainDependencies();
builder.Services.AddApplicationDependencies();
builder.Services.AddInfrastructureDependencies();

builder.Services.ConfigureHttpClients();
builder.Services.ConfigureVersioning();

builder.Services.AddExceptionsHandlers();
builder.Services.AddProblemDetails();

builder.Services.AddBasicAuthorization();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.EnableSwagger();
}

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.EnableSerilogRequestLogging();

app.Run();
