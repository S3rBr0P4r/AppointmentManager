using AppointmentManager.Presentation.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddPresentationDependencies();
builder.Services.AddDomainDependencies();
builder.Services.AddApplicationDependencies();
builder.Services.AddInfrastructureDependencies();

builder.Services.ConfigureHttpClients();
builder.Services.ConfigureVersioning();

builder.Services.AddExceptionsHandlers();
builder.Services.AddProblemDetails();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
