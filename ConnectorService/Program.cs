using ConnectorService.Data;
using ConnectorService.EventListeners;
using ConnectorService.Interfaces;
using ConnectorService.Models.Connector;
using ConnectorService.Models.Connector.Mapping;
using ConnectorService.Repositories;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared.Data;
using Shared.Utilities;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddControllers();

// Adding services
builder.Services.AddScoped<IConnectorService, ConnectorService.Services.ConnectorService>();

// Adding infrastructure services 
builder.Services.AddInfrastructureServices(builder.Configuration);

// Creating a test InMemory database
builder.Services.AddDbContext<ConnectorServiceContext>(options =>
    options.UseInMemoryDatabase("TestDatabase"));

builder.Services.AddScoped<IConnectorRepository, ConnectorRepository>();

// Add event listeners
builder.Services.AddHostedService<ChargeStationDeletedEventListener>();
builder.Services.AddHostedService<ChargeStationCreatedEventListener>();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Connector Service API",
        Version = "v1",
        Description = "API documentation for Connector Service"
    });
});

// Configure AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Seed InMemory db with data from csv
var seedActions = new List<Action<ConnectorServiceContext>>
{
    context =>
    {
        var connectors = CsvLoader.LoadCsvData<Connector>(Path.Combine("Data", "Connectors.csv"), new ConnectorCsvMap());
        context.Set<Connector>().AddRange(connectors);
    }
};
DatabaseSeeder.SeedDatabase(app.Services, seedActions);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Group Service API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.MapControllers();

app.Run();
