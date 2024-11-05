using ConnectorService.Models.Connector;
using Microsoft.EntityFrameworkCore;

namespace ConnectorService.Data;

public class ConnectorServiceContext : DbContext
{
    public DbSet<Connector> Connectors { get; set; }    

    public ConnectorServiceContext(DbContextOptions<ConnectorServiceContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // If I used a seperate reference model for ChargeStations, this is where
        // I would onfigure many-to-one relationship between Connector and ChargeStation Ref model        
    }
}
