using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SmartDomainDrivenDesign.Domain.Shared
{
    public static class ExpressionBuilder
    {
        /// <summary>
        /// https://lostechies.com/jimmybogard/2011/02/15/prototyping-with-anonymous-classes/
        /// Workaround to create Expressions for Anonymous types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="_">Fake parameter to the workaround</param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression<Func<T, TResult>> CreateExpression<T, TResult>(this T _, Expression<Func<T, TResult>> expr) => expr;

        /// <summary>
        /// Obtiene el nombre de la propiedad a partir de la expresión de selección de la propiedad
        /// </summary>
        /// <typeparam name="T">Clase que contiene la propiedad</typeparam>
        /// <param name="propertyExpression">Expresión de selección de la propiedad</param>
        /// <returns>Cadena con el nombre de la propiedad</returns>
        public static string GetPropertyName<T>(Expression<Func<T, object>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression body)
                return body.Member.Name;

            UnaryExpression ubody = (UnaryExpression)propertyExpression.Body;
            body = ubody.Operand as MemberExpression;

            return body?.Member.Name;
        }

        /// <summary>
        /// Contains
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="propertyValue"></param>
        /// <param name="parameterExp"></param>
        /// <returns></returns>
        public static MethodCallExpression Contains<T, B>(Expression<Func<T, object>> propertyExpression, string propertyValue, ParameterExpression parameterExp)
        {
            string propertyName = GetPropertyName<T>(propertyExpression);
            return Contains<B>(propertyName, propertyValue, parameterExp);
        }

        /// <summary>
        /// Contains
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="parameterExp"></param>
        /// <returns></returns>
        public static MethodCallExpression Contains<B>(string propertyName, string propertyValue, ParameterExpression parameterExp)
        {
            MemberExpression propertyExp = Expression.Property(parameterExp, propertyName);
            MethodInfo method = typeof(B).GetMethod("Contains", new[] { typeof(B) });
            ConstantExpression someValue = Expression.Constant(propertyValue, typeof(string));
            return Expression.Call(propertyExp, method, someValue);
        }

        /// <summary>
        /// A general utility method to compose lambda expressions without using invoke
        /// (I’ll call it Compose), and leverage it to implement EF-friendly <c>And</c> and <c>Or</c> builder method.
        /// </summary>
        /// <see cref="http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx"/>
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            Dictionary<ParameterExpression, ParameterExpression> map = first.Parameters.Select((f, i) => new
            {
                f,
                s = second.Parameters[i]
            }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            Expression secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// And
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) => first.Compose(second, Expression.And);

        /// <summary>
        /// Or
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) => first.Compose(second, Expression.Or);

        /// <summary>
        /// Execute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool Execute<T>(this Expression<Func<T, bool>> first, T value) => first.Compile()(value);
    }

    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp) => new ParameterRebinder(map).Visit(exp);

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (this.map.TryGetValue(p, out ParameterExpression replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }
}
