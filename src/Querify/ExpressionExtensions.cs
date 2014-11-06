using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Querify
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            // need to detect whether they use the same
            // parameter instance; if not, they need fixing
            ParameterExpression param = expr1.Parameters[0];
            if (ReferenceEquals(param, expr2.Parameters[0]))
            {
                // simple version
                return Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(expr1.Body, expr2.Body), param);
            }
            // otherwise, keep expr1 "as is" and invoke expr2
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    expr1.Body,
                    Expression.Invoke(expr2, param)), param);
        }

        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            // need to detect whether they use the same
            // parameter instance; if not, they need fixing
            ParameterExpression param = expr1.Parameters[0];
            if (ReferenceEquals(param, expr2.Parameters[0]))
            {
                // simple version
                return Expression.Lambda<Func<T, bool>>(
                    Expression.OrElse(expr1.Body, expr2.Body), param);
            }
            // otherwise, keep expr1 "as is" and invoke expr2
            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(
                    expr1.Body,
                    Expression.Invoke(expr2, param)), param);
        }
    }

    public static class Expressions
    {
        public static Expression<Func<T, bool>> Apply<T>(Func<bool> condition, Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (!condition())
            {
                return left;
            }

            if (left == null)
            {
                return right;
            }
            else
            {
                return left.AndAlso(right);
            }
        }

        public static IQueryable<TSource> AppendContains<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            IEnumerable<TResult> options)
        {
            if (options == null || !options.Any())
                throw new ArgumentException("At least one options must be specified.", "options");
            Expression orExpression = null;
            Expression selectorExpression = (MemberExpression)selector.Body;
            foreach (var item in options)
            { Expression equalExpression = Expression.Equal(selectorExpression, Expression.Constant(item)); orExpression = orExpression == null ? equalExpression : Expression.OrElse(orExpression, equalExpression); }
            var funcType = Expression.GetFuncType(typeof(TSource), typeof(bool));
            var sourceParam = Expression.Parameter(typeof(TSource), "source");
            var whereClause = (Expression<Func<TSource, bool>>)Expression.Lambda(funcType, orExpression, sourceParam);
            return source.Where(whereClause);
        }
    }
}