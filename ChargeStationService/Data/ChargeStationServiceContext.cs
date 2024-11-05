using ChargeStationService.Models.ChargeStation;
using Microsoft.EntityFrameworkCore;

namespace ChargeStationService.Data;

public class ChargeStationServiceContext : DbContext
{
    public DbSet<ChargeStation> ChargeStations { get; set; }

    public ChargeStationServiceContext(DbContextOptions<ChargeStationServiceContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Same as other DbContexts; here I would define the relationship
        // between ChargeStation and its Ref objects
    }
}
