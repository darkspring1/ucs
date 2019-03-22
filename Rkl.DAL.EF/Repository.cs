using Microsoft.EntityFrameworkCore;
using Rkl.DAL.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rkl.DAL.EF
{

    public class Repository<T> : IIncludableRepository<T>
      where T : class
    {

        protected virtual async Task<T> GetOneAsync(Func<IQueryable<T>, Task<T>> getOneFunc, ISpecification<T> specification, IInclude<T> include, bool asNoTracking)
        {
            var states = await GetStatesAsync(specification, include);
            if (asNoTracking)
            {
                states = states.AsNoTracking();
            }
            var state = await getOneFunc(states);

            if (state == null)
            {
                return null;
            }

            return state;
        }

        protected DbContext DbContext { get; }
        protected DbSet<T> CurrentSet { get; }

        protected IQueryable<T> ApplySpecificationAndInclude(IQueryable<T> states, ISpecification<T> specification, IInclude<T> include)
        {
            if (specification == null)
            {
                if (include == null)
                {
                    return states;
                }

                return include.Include(states);
            }

            if (specification.DontUseBuild)
            {
                var query = states.Where(specification.ToExpression());
                if (include == null)
                {
                    return query;
                }
                return include.Include(query);
            }

            //для совместимости у устаревшими спецификациями
            return specification.Build(states);
        }

        protected virtual async Task<PagingResult<T>> CreatePagingResultAsync(IQueryable<T> states, IPaging paging, bool asNoTracking)
        {
            var totalCount = await states.CountAsync();

            if (asNoTracking)
            {
                states = states.AsNoTracking();
            }

            var items = await states
               .Skip(paging.Skip)
               .Take(paging.Take)
               .ToArrayAsync();

            return new PagingResult<T>(items, totalCount);
        }

        protected virtual Task<IQueryable<T>> GetStatesAsync(ISpecification<T> specification, IInclude<T> include)
        {
            return Task.FromResult(ApplySpecificationAndInclude(CurrentSet, specification, include));
        }

        public Repository(DbContext context)
        {
            CurrentSet = context.Set<T>();
            DbContext = context;
        }

        public void Add(T entity)
        {
            DbContext.Add(entity);
        }

        public virtual void Delete(params T[] entities)
        {
            DbContext.RemoveRange(entities);
        }

        public void Attach(params T[] entities)
        {
            DbContext.AttachRange(entities);
        }

        public Task<T> FirstAsync(ISpecification<T> specification, bool asNoTracking = false)
        {
            return GetOneAsync(states => states.FirstAsync(), specification, null, asNoTracking);
        }

        public Task<T> FirstAsync(bool asNoTracking = false)
        {
            return GetOneAsync(states => states.FirstAsync(), null, null, asNoTracking);
        }

        public Task<T> FirstOrDefaultAsync(ISpecification<T> specification, bool asNoTracking = false)
        {
            return GetOneAsync(states => states.FirstOrDefaultAsync(), specification, null, asNoTracking);
        }

        public Task<T> FirstOrDefaultAsync(bool asNoTracking = false)
        {
            return GetOneAsync(states => states.FirstOrDefaultAsync(), null, null, asNoTracking);
        }

        public Task<T> SingleAsync(ISpecification<T> specification, bool asNoTracking = false)
        {
            return GetOneAsync(states => states.SingleAsync(), specification, null, asNoTracking);
        }

        public Task<T> SingleOrDefaultAsync(ISpecification<T> specification, bool asNoTracking = false)
        {
            return GetOneAsync(states => states.SingleOrDefaultAsync(), specification, null, asNoTracking);
        }

        public Task<T> SingleAsync(bool asNoTracking = false)
        {
            return GetOneAsync(states => states.SingleAsync(), null, null, asNoTracking);
        }

        public virtual Task<T[]> GetEntitiesAsync(ISpecification<T> specification, bool asNoTracking = false)
        {
            return GetEntitiesAsync(specification, null, asNoTracking);
        }

        public Task<T[]> GetEntitiesAsync()
        {
            return GetEntitiesAsync(null);
        }

        public async Task<PagingResult<T>> GetPagesAsync(IPaging paging, bool asNoTracking = false)
        {
            var states = await GetStatesAsync(null, null);
            return await CreatePagingResultAsync(states, paging, asNoTracking);
        }

        public async Task<PagingResult<T>> GetPagesAsync(ISpecification<T> specification, IPaging paging, bool asNoTracking = false)
        {
            var states = await GetStatesAsync(specification, null);
            return await CreatePagingResultAsync(states, paging, asNoTracking);
        }

        public void Update(T entity)
        {
            DbContext.Update(entity);
        }

        public async Task<bool> AnyAsync(ISpecification<T> specification)
        {
            var states = await GetStatesAsync(specification, null);
            return await states.AnyAsync();
        }

        #region IIncludableRepository<T>
       
        public async Task<PagingResult<T>> GetPagesAsync(IPaging paging, IInclude<T> include, bool asNoTracking = false)
        {
            var states = await GetStatesAsync(null, include);
            return await CreatePagingResultAsync(states, paging, asNoTracking);
        }

        public async Task<PagingResult<T>> GetPagesAsync(ISpecification<T> specification, IInclude<T> include, IPaging paging, bool asNoTracking = false)
        {
            var states = await GetStatesAsync(specification, include);
            return await CreatePagingResultAsync(states, paging, asNoTracking);
        }

        public async Task<T[]> GetEntitiesAsync(ISpecification<T> specification, IInclude<T> include, bool asNoTracking = false)
        {
            var states = await GetStatesAsync(specification, include);
            if (asNoTracking)
            {
                states = states.AsNoTracking();
            }
            var result = states.ToArray();

            return result;
        }

        public Task<T[]> GetEntitiesAsync(IInclude<T> include)
        {
            return GetEntitiesAsync(null, include, false);
        }

        public Task<T> FirstAsync(IInclude<T> include, bool asNoTracking = false)
        {
            return GetOneAsync(states => states.FirstAsync(), null, include, asNoTracking);
        }

        public Task<T> FirstAsync(ISpecification<T> specification, IInclude<T> include, bool asNoTracking = false)
        {
            return GetOneAsync(states => states.FirstAsync(), specification, include, asNoTracking);
        }

        public Task<T> FirstOrDefaultAsync(ISpecification<T> specification, IInclude<T> include, bool asNoTracking = false)
        {
            return GetOneAsync(states => states.FirstOrDefaultAsync(), specification, include, asNoTracking);
        }

        public Task<T> FirstOrDefaultAsync(IInclude<T> include, bool asNoTracking = false)
        {
            return GetOneAsync(states => states.FirstOrDefaultAsync(), null, include, asNoTracking);
        }

        public Task<T> SingleAsync(ISpecification<T> specification, IInclude<T> include, bool asNoTracking = false)
        {
            return GetOneAsync(states => states.SingleAsync(), specification, include, asNoTracking);
        }

        public Task<T> SingleOrDefaultAsync(ISpecification<T> specification, IInclude<T> include, bool asNoTracking = false)
        {
            return GetOneAsync(states => states.SingleOrDefaultAsync(), specification, include, asNoTracking);
        }

        public Task<T> SingleAsync(IInclude<T> include, bool asNoTracking = false)
        {
            return GetOneAsync(states => states.SingleAsync(), null, include, asNoTracking);
        }
        #endregion
    }
}
