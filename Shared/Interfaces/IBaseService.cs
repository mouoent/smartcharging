using Shared.Models;

namespace Shared.Interfaces
{
    public interface IBaseService<TModel, TDto, CreateTDto, UpdateTDto> where TModel : BaseEntity
    {
        Task<TModel> AddAsync(CreateTDto dto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TModel> GetByIdAsync(Guid id);
        Task<TModel> UpdateAsync(Guid id, UpdateTDto dto);
    }
}