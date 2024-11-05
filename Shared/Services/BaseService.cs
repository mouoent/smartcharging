using AutoMapper;
using Shared.Interfaces;
using Shared.Models;

namespace Shared.Services;

public abstract class BaseService<TModel, TDto, CreateTDto, UpdateTDto> 
    : IBaseService<TModel, TDto, CreateTDto, UpdateTDto> 
    where TModel : BaseEntity
{
    protected readonly IRepository<TModel> _repository;
    protected readonly IMapper _mapper;

    protected BaseService(IRepository<TModel> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        var dtoList = _mapper.Map<IEnumerable<TDto>>(entities);

        return dtoList;
    }
    public virtual Task<TModel> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);
    public virtual async Task<TModel> AddAsync(CreateTDto dto)
    {
        var entity = _mapper.Map<TModel>(dto);
        await _repository.AddAsync(entity);

        return entity;
    }
    public virtual async Task<TModel> UpdateAsync(Guid id, UpdateTDto dto)
    {
        var existingEntity = await _repository.GetByIdAsync(id);
        if (existingEntity == null) throw new Exception($"Entity not found");

        _mapper.Map(dto, existingEntity);
        await _repository.UpdateAsync(existingEntity);

        return existingEntity;
    }
    public virtual async Task DeleteAsync(Guid id)
    {
        var existingEntity = await _repository.GetByIdAsync(id);
        if (existingEntity == null) throw new Exception($"Entity not found");
        await _repository.DeleteAsync(id);
    }
}
