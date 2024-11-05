using GroupService.Models.Group;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Data;

public class GroupServiceContext : DbContext
{
    public DbSet<Group> Groups { get; set; }

    public GroupServiceContext(DbContextOptions<GroupServiceContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {       
        // If I used a seperate reference model for ChargeStations, this is where
        // I would onfigure one-to-many relationship between Group and ChargeStation Ref model        
    }
}
