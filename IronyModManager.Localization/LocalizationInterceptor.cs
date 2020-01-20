// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
// ***********************************************************************
// <copyright file="LocalizationInterceptor.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Linq;
using IronyModManager.Common.ViewModels;
using IronyModManager.Shared;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Class LocalizationInterceptor.
    /// Implements the <see cref="Castle.DynamicProxy.IInterceptor" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Castle.DynamicProxy.IInterceptor" />
    public class LocalizationInterceptor<T> : Castle.DynamicProxy.IInterceptor where T : IViewModel
    {
        #region Fields

        /// <summary>
        /// The get method
        /// </summary>
        private const string GetMethod = "get_";

        /// <summary>
        /// The set method
        /// </summary>
        private const string SetMethod = "set_";

        /// <summary>
        /// The actual type
        /// </summary>
        private static readonly string actualTypeMethodName = $"{GetMethod}{nameof(IViewModel.ActualType)}";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(Castle.DynamicProxy.IInvocation invocation)
        {
            if (invocation.Method.Name.Equals(actualTypeMethodName))
            {
                // Needed for view\view model resolution... It's a bit hacky... If you've got a better solution I'm all ears.
                invocation.ReturnValue = typeof(T);
                return;
            }
            // Handle on change event... Decided to ditch Fody, it has a dubious MIT license (wasn't aware of it at first)
            else if (invocation.Method.Name.StartsWith(SetMethod))
            {
                var methodName = invocation.Method.Name.Replace(SetMethod, string.Empty);
                var prop = invocation.TargetType.GetProperty(methodName);
                var newVal = invocation.Arguments?[0];
                var oldVal = prop.GetValue(invocation.InvocationTarget, null);
                if (newVal != oldVal)
                {
                    var attr = prop.GetCustomAttributes(typeof(DoNotNotifyAttribute), true);
                    if (attr?.Count() > 0)
                    {
                        invocation.Proceed();
                    }
                    else
                    {
                        ((IViewModel)invocation.Proxy).OnPropertyChanging(methodName);
                        invocation.Proceed();
                        ((IViewModel)invocation.Proxy).OnPropertyChanged(methodName);
                    }
                }
                return;
            }
            else if (invocation.Method.Name.StartsWith(GetMethod))
            {
                var methodName = invocation.Method.Name.Replace(GetMethod, string.Empty);
                var prop = invocation.TargetType.GetProperty(methodName);
                var attr = prop.GetCustomAttributes(typeof(LocalizationAttribute), true);
                if (attr?.Count() > 0)
                {
                    var locAttr = ((LocalizationAttribute)attr.First());
                    invocation.ReturnValue = locAttr.ResourceKey;
                    return;
                }
            }
            invocation.Proceed();
        }

        #endregion Methods
    }
}
