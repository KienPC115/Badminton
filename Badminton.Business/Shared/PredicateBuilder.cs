using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Badminton.Business.Shared {
    public static class PredicateBuilder {
        // Returns a predicate that is always true.
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        // Returns a predicate that is always false.
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        // Combines two predicates with a logical OR.
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                          Expression<Func<T, bool>> expr2) {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        // Combines two predicates with a logical AND.
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                           Expression<Func<T, bool>> expr2) {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}
