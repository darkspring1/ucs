using System;
using System.Linq;
using System.Linq.Expressions;

namespace Rkl.DAL.Abstractions
{
    public abstract class Specification<T> : ISpecification<T>
    {
        /// <summary>
        /// True - не использовать Build
        /// Для удобства выпиливания Build
        /// </summary>
        public abstract bool DontUseBuild { get; }

        [Obsolete("Для фильтрации используйте ToExpression")]
        public abstract IQueryable<T> Build(IQueryable<T> source);

        public bool IsSatisfiedBy(T entity)
        {
            return ToExpression().Compile().Invoke(entity);
        }

        public abstract Expression<Func<T, bool>> ToExpression();

        public Specification<T> And(Specification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }
    }
}
