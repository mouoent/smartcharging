using GroupService.Data;
using Infrastructure;
using GroupService.Interfaces;
using GroupService.Models.Group;
using GroupService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared.Data;
using Shared.Utilities;
using GroupService.EventListeners.Connector;
using GroupService.EventListeners.ChargeStation;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddControllers();

// Adding services
builder.Services.AddScoped<IGroupService, GroupService.Services.GroupService>();

// Adding infrastructure services 
builder.Services.AddInfrastructureServices(builder.Configuration);

// Creating a test InMemory database
builder.Services.AddDbContext<GroupServiceContext>(options =>
    options.UseInMemoryDatabase("TestDatabase"));

builder.Services.AddScoped<IGroupRepository, GroupRepository>();

// Add event listener
builder.Services.AddHostedService<ConnectorCreatedEventListener>();
builder.Services.AddHostedService<ConnectorUpdatedEventListener>();
builder.Services.AddHostedService<ConnectorDeletedEventListener>();
builder.Services.AddHostedService<ChargeStationCreatedEventListener>();
builder.Services.AddHostedService<ChargeStationDeletedEventListener>();
builder.Services.AddHostedService<ChargeStationUpdatedEventListener>();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Group Service API",
        Version = "v1",
        Description = "API documentation for Group Service"
    });
});

// Configure AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Seed InMemory db with data from csv
var seedActions = new List<Action<GroupServiceContext>>
{
    context =>
    {
        var groups = CsvLoader.LoadCsvData<Group>(Path.Combine("Data", "Groups.csv"), new GroupCsvMap());
        context.Set<Group>().AddRange(groups);
    }
};
DatabaseSeeder.SeedDatabase(app.Services, seedActions);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Group Service API v1");
        c.RoutePrefix = string.Empty; // Sets Swagger UI as the default page
    });
}

app.MapControllers();

app.Run();
