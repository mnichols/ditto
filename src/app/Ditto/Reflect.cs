using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ditto
{
	/// <summary>
	/// Helper for simplifying lamba access to members
	/// <see cref="http://blog.andreloker.de/post/2008/06/Getting-rid-of-strings-(2)-use-lambda-expressions.aspx"/>
	/// </summary>
	public static class Reflect
	{
		/// <summary>
		/// Gets the MethodInfo for the method that is called in the provided <paramref name="expression"/>
		/// </summary>
		/// <example>
		/// <code>
		/// public class HomeController : SmartDispatcherController {
		/// public void Index() {
		/// // RedirectToAction("Login");
		/// this.RedirectToAction(c => c.Login());
		/// }
		/// public void Login(){ }
		/// }
		/// </code>
		/// </example>
		/// <typeparam name="TClass">The type of the class.</typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns>Method info</returns>
		/// <exception cref="ArgumentException">The provided expression is not a method call</exception>
		public static MethodInfo GetMethod<TClass>(Expression<Action<TClass>> expression)
		{
			var methodCall = expression.Body as MethodCallExpression;
			if (methodCall == null)
			{
				throw new ArgumentException("Expected method call");
			}
			return methodCall.Method;
		}
		/// <summary>
		/// Gets the property for the call in the provided <paramref name="expression"/>.
		/// </summary>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// var dayProperty = Reflect.GetProperty<DateTime>(dt => dt.Day);
		/// //OR
		/// var dayProperty = Reflect.GetProperty( (DateTime dt) => dt.Day);
		/// ]]>
		/// </code>
		/// </example> 
		/// <typeparam name="TClass">The type of the class.</typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns></returns>
		public static PropertyInfo GetProperty<TClass>(Expression<Func<TClass, object>> expression)
		{
			MemberExpression memberExpression;
			// if the return value had to be cast to object, the body will be an UnaryExpression
			var unary = expression.Body as UnaryExpression;
			if (unary != null)
			{
				// the operand is the "real" property access
				memberExpression = unary.Operand as MemberExpression;
			}
			else
			{
				// in case the property is of type object the body itself is the correct expression
				memberExpression = expression.Body as MemberExpression;
			}
			// as before:
			if (memberExpression == null || !(memberExpression.Member is PropertyInfo))
			{
				throw new ArgumentException("Expected property expression");
			}
			return (PropertyInfo)memberExpression.Member;
		}

        /// <summary>
        /// Gets the property for the call in the provided <paramref name="expression"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var dayProperty = Reflect.GetProperty<DateTime>(dt => dt.Day);
        /// //OR
        /// var dayProperty = Reflect.GetProperty( (DateTime dt) => dt.Day);
        /// ]]>
        /// </code>
        /// </example> 
        /// <typeparam name="TClass">The type of the class.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<TClass,TProperty>(Expression<Func<TClass, TProperty>> expression)
        {
            MemberExpression memberExpression;
            // if the return value had to be cast to object, the body will be an UnaryExpression
            var unary = expression.Body as UnaryExpression;
            if (unary != null)
            {
                // the operand is the "real" property access
                memberExpression = unary.Operand as MemberExpression;
            }
            else
            {
                // in case the property is of type object the body itself is the correct expression
                memberExpression = expression.Body as MemberExpression;
            }
            // as before:
            if (memberExpression == null || !(memberExpression.Member is PropertyInfo))
            {
                throw new ArgumentException("Expected property expression");
            }
            return (PropertyInfo)memberExpression.Member;
        }

		/// <summary>
		/// Gets the field accessor.
		/// http://rogeralsing.com/2008/02/26/linq-expressions-access-private-fields/
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="R"></typeparam>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns></returns>
		public static Func<T, R> GetFieldAccessor<T, R>(string fieldName)
		{
			ParameterExpression param =
				Expression.Parameter(typeof(T), "arg");

			MemberExpression member =
				Expression.Field(param, fieldName);

			LambdaExpression lambda =
				Expression.Lambda(typeof(Func<T, R>), member, param);

			Func<T, R> compiled = (Func<T, R>)lambda.Compile();
			return compiled;
		}

	}
}