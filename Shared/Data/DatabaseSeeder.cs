using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Data;

public static class DatabaseSeeder
{
    public static void SeedDatabase<TContext>(
        IServiceProvider serviceProvider,
        IEnumerable<Action<TContext>> seedActions) where TContext : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        // Create in memory database
        foreach (var seedAction in seedActions)
        {
            seedAction(context);
        }

        context.SaveChanges(); // Save all seeded data to the database
        
        /*if (context.Database.EnsureCreated()) 
        {                
        
        }*/
    }
}
