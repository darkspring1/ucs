using System;
using System.Linq;
using System.Linq.Expressions;

namespace Rkl.DAL.Abstractions
{
    public interface ISpecification<TState>
    {
        [Obsolete("Для фильтрации используйте ToExpression")]
        IQueryable<TState> Build(IQueryable<TState> source);

        /// <summary>
        /// True - не использовать Build
        /// Для удобства выпиливания Build
        /// </summary>
        bool DontUseBuild { get; }

        bool IsSatisfiedBy(TState obj);

        Expression<Func<TState, bool>> ToExpression();
        
    }
}
