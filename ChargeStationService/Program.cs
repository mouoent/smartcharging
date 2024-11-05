using ChargeStationService.Data;
using ChargeStationService.Interfaces;
using ChargeStationService.Models.ChargeStation;
using ChargeStationService.Models.ChargeStation.Mapping;
using ChargeStationService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Infrastructure;
using Shared.Data;
using Shared.Interfaces;
using Shared.Utilities;
using ChargeStationService.EventListeners.Group;
using ChargeStationService.EventListeners;
using ChargeStationService.EventListeners.ChargeStation;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddControllers();

// Adding services
builder.Services.AddScoped<IChargeStationService, ChargeStationService.Services.ChargeStationService>();

// Adding infrastructure services 
builder.Services.AddInfrastructureServices(builder.Configuration);

// Creating a test InMemory database
builder.Services.AddDbContext<ChargeStationServiceContext>(options =>
    options.UseInMemoryDatabase("TestDatabase"));

builder.Services.AddScoped<IChargeStationRepository, ChargeStationRepository>();

// Add event listeners
builder.Services.AddHostedService<GroupDeletedEventListener>();
builder.Services.AddHostedService<ConnectorCreatedEventListener>();
builder.Services.AddHostedService<ConnectorUpdatedEventListener>();
builder.Services.AddHostedService<ConnectorDeletedEventListener>();

// Add service from infrastructure
builder.Services.AddScoped<IEventPublisher, EventPublisher>();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ChargeStation Service API",
        Version = "v1",
        Description = "API documentation for ChargeStation Service"
    });
});

// Configure AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Seed InMemory db with data from csv
var seedActions = new List<Action<ChargeStationServiceContext>>
{
    context =>
    {
        var chargeStations = CsvLoader
                                .LoadCsvData<ChargeStation>(Path.Combine("Data", "ChargeStations.csv"), new ChargeStationCsvMap());
        context.Set<ChargeStation>().AddRange(chargeStations);
    }
};
DatabaseSeeder.SeedDatabase(app.Services, seedActions);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChargeStation Service API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.MapControllers();

app.Run();

