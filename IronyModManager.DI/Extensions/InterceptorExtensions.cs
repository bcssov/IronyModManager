// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-19-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
// ***********************************************************************
// <copyright file="InterceptorExtensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using SimpleInjector;

namespace IronyModManager.DI.Extensions
{
    /// <summary>
    /// Class InterceptorExtensions.
    /// </summary>
    public static class InterceptorExtensions
    {
        #region Fields

        /// <summary>
        /// The create class proxy
        /// </summary>
        private static readonly Func<Type, object[], IInterceptor, object> createClassProxy =
            (type, args, interceptor) => proxyGenerator.CreateClassProxy(type, args, interceptor);

        /// <summary>
        /// The create class proxy target
        /// </summary>
        private static readonly Func<Type, object, object[], IInterceptor, object> createClassProxyTarget =
            (type, obj, args, interceptor) => proxyGenerator.CreateClassProxyWithTarget(type, obj, args, interceptor);

        /// <summary>
        /// The create proxy interface
        /// </summary>
        private static readonly Func<Type, IInterceptor, object> createProxyInterface =
            (type, interceptor) => proxyGenerator.CreateInterfaceProxyWithoutTarget(type, interceptor);

        /// <summary>
        /// The create proxy interface target
        /// </summary>
        private static readonly Func<Type, object, IInterceptor, object> createProxyInterfaceTarget =
            (type, obj, interceptor) => proxyGenerator.CreateInterfaceProxyWithTarget(type, obj, interceptor);

        /// <summary>
        /// The proxy generator
        /// </summary>
        private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Intercepts the with.
        /// </summary>
        /// <typeparam name="TInterceptor">The type of the t interceptor.</typeparam>
        /// <param name="container">The container.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="asTarget">if set to <c>true</c> [as target].</param>
        public static void InterceptWith<TInterceptor>(this Container container, Predicate<Type> predicate, bool asTarget = true) where TInterceptor : class, IInterceptor
        {
            container.ExpressionBuilt += (s, e) =>
            {
                if (predicate(e.RegisteredServiceType))
                {
                    var interceptorExpression = container.GetRegistration(typeof(TInterceptor), true).BuildExpression();

                    if (e.RegisteredServiceType.IsInterface)
                    {
                        if (asTarget)
                        {
                            e.Expression = Expression.Convert(
                                Expression.Invoke(Expression.Constant(createProxyInterfaceTarget),
                                    Expression.Constant(e.RegisteredServiceType, typeof(Type)),
                                    e.Expression,
                                    interceptorExpression),
                                e.RegisteredServiceType);
                        }
                        else
                        {
                            e.Expression = Expression.Convert(
                                Expression.Invoke(Expression.Constant(createProxyInterface),
                                    Expression.Constant(e.RegisteredServiceType, typeof(Type)),
                                    interceptorExpression),
                                e.RegisteredServiceType);
                        }
                    }
                    else
                    {
                        var args = new List<object>();
                        var ctors = e.RegisteredServiceType.GetConstructors();
                        foreach (var arg in ctors.First().GetParameters())
                        {
                            args.Add(container.GetInstance(arg.ParameterType));
                        }

                        if (asTarget)
                        {
                            e.Expression = Expression.Convert(
                                Expression.Invoke(Expression.Constant(createClassProxyTarget),
                                    Expression.Constant(e.RegisteredServiceType, typeof(Type)),
                                    e.Expression,
                                    Expression.Constant(args.ToArray(), typeof(object[])),
                                    interceptorExpression),
                                e.RegisteredServiceType);
                        }
                        else
                        {
                            e.Expression = Expression.Convert(
                                Expression.Invoke(Expression.Constant(createClassProxy),
                                    Expression.Constant(e.RegisteredServiceType, typeof(Type)),
                                    Expression.Constant(args.ToArray(), typeof(object[])),
                                    interceptorExpression),
                                e.RegisteredServiceType);
                        }
                    }
                }
            };
        }

        #endregion Methods
    }
}
