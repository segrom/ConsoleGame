using Domain.Common;

namespace DataAccess.Common;

public interface ICrudSet<T> where T: IModel
{
    Task<bool> CreateAsync(T model);
    Task<bool> UpdateAsync(T model);
    Task<T?> GetAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<T>> AllAsync();
}