using System;
using System.Linq;
using System.Linq.Expressions;

namespace Rkl.DAL.Abstractions
{
    public class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public AndSpecification(Specification<T> left, Specification<T> right)
        {
            _right = right;
            _left = left;
        }


        /// <summary>
        /// true
        /// </summary>
        public override bool DontUseBuild => true;

        public override IQueryable<T> Build(IQueryable<T> source)
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> expressionOne = _left.ToExpression();
            Expression<Func<T, bool>> expressionTwo = _right.ToExpression();
            var invokedSecond = Expression.Invoke(expressionTwo, expressionOne.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.And(expressionOne.Body, invokedSecond), expressionOne.Parameters);
        }

    }
}
