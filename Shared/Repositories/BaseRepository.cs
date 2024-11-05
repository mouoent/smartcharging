using Microsoft.EntityFrameworkCore;
using Shared.Interfaces;

namespace Shared.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>(); 
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var item = await _dbSet.FindAsync(id);
        if (item != null)
        {
            _dbSet.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
}
