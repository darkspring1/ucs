using Rkl.DAL.Abstractions;
using System.Threading.Tasks;

namespace Rkl.DAL.EF
{
    public interface IIncludableRepository<T> : IRepository<T>
    where T : class
    {
        Task<PagingResult<T>> GetPagesAsync(IPaging paging, IInclude<T> include, bool asNoTracking = false);

        Task<PagingResult<T>> GetPagesAsync(ISpecification<T> specification, IInclude<T> include, IPaging paging, bool asNoTracking = false);

        Task<T[]> GetEntitiesAsync(ISpecification<T> specification, IInclude<T> include, bool asNoTracking = false);

        Task<T[]> GetEntitiesAsync(IInclude<T> include);

        Task<T> FirstAsync(IInclude<T> include, bool asNoTracking = false);

        Task<T> FirstAsync(ISpecification<T> specification, IInclude<T> include, bool asNoTracking = false);

        Task<T> FirstOrDefaultAsync(ISpecification<T> specification, IInclude<T> include, bool asNoTracking = false);

        Task<T> FirstOrDefaultAsync(IInclude<T> include, bool asNoTracking = false);

        Task<T> SingleAsync(ISpecification<T> specification, IInclude<T> include, bool asNoTracking = false);

        Task<T> SingleOrDefaultAsync(ISpecification<T> specification, IInclude<T> include, bool asNoTracking = false);

        Task<T> SingleAsync(IInclude<T> include, bool asNoTracking = false);
    }
}
