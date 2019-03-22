using System.Linq;

namespace Rkl.DAL.EF
{
    public interface IInclude<TState>
    {
        IQueryable<TState> Include(IQueryable<TState> source);
    }
}
