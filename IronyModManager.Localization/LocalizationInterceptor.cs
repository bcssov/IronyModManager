// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
// ***********************************************************************
// <copyright file="LocalizationInterceptor.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using IronyModManager.Models.Common;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Class LocalizationInterceptor.
    /// Implements the <see cref="IronyModManager.Models.Common.PropertyChangedInterceptorBase" />
    /// Implements the <see cref="Castle.DynamicProxy.IInterceptor" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IronyModManager.Models.Common.PropertyChangedInterceptorBase" />
    /// <seealso cref="Castle.DynamicProxy.IInterceptor" />
    public class LocalizationInterceptor<T> : PropertyChangedInterceptorBase, IInterceptor where T : ILocalizableModel
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
        private static readonly string actualTypeMethodName = $"{GetMethod}{nameof(ILocalizableModel.ActualType)}";

        /// <summary>
        /// The locale changed
        /// </summary>
        private static readonly string LocaleChanged = nameof(ILocalizableModel.OnLocaleChanged);

        #endregion Fields

        #region Methods

        /// <summary>
        /// Fires the event.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="prop">The property.</param>
        public override void FireEvent(IInvocation invocation, PropertyInfo prop)
        {
            ((ILocalizableModel)invocation.Proxy).OnPropertyChanging(prop.Name);
            invocation.Proceed();
            ((ILocalizableModel)invocation.Proxy).OnPropertyChanged(prop.Name);
        }

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public override void Intercept(IInvocation invocation)
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
                base.Intercept(invocation);
                return;
            }
            else if (invocation.Method.Name.StartsWith(LocaleChanged))
            {
                var localizationProperties = invocation.TargetType.GetProperties().Where(p => Attribute.IsDefined(p, typeof(LocalizationAttribute)));
                if (localizationProperties.Count() > 0)
                {
                    foreach (var prop in localizationProperties)
                    {
                        ((ILocalizableModel)invocation.Proxy).OnPropertyChanging(prop.Name);
                        ((ILocalizableModel)invocation.Proxy).OnPropertyChanged(prop.Name);
                    }
                }
                invocation.Proceed();
                return;
            }
            else if (invocation.Method.Name.StartsWith(GetMethod))
            {
                var methodName = invocation.Method.Name.Replace(GetMethod, string.Empty);
                var prop = invocation.TargetType.GetProperty(methodName);
                var attr = Attribute.GetCustomAttribute(prop, typeof(LocalizationAttribute), true);
                if (attr != null)
                {
                    var locAttr = ((LocalizationAttribute)attr);
                    invocation.ReturnValue = locAttr.ResourceKey;
                    return;
                }
            }
            invocation.Proceed();
        }

        #endregion Methods
    }
}
