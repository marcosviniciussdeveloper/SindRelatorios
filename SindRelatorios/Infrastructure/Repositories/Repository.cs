using Microsoft.EntityFrameworkCore;
using SindRelatorios.Application.Interfaces;
using SindRelatorios.Infrastructure.Data;


namespace SindRelatorios.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly SindDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public Repository(SindDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    
    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity; 
    }

  
    public async Task<T> UpdateAsync(T entity)
    {

        var idProperty = typeof(T).GetProperty("Id");
        
        if (idProperty != null)
        {
            var entityId = (Guid)idProperty.GetValue(entity);

     
            var existingEntity = _dbSet.Local.FirstOrDefault(e => 
                (Guid)e.GetType().GetProperty("Id").GetValue(e) == entityId);

            
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }
        }

     
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
        {
            return false; 
        }

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true; 
    }
}