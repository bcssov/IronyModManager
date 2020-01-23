// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="LocalizationInterceptor.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using IronyModManager.Localization.Attributes;
using IronyModManager.Localization.Attributes.Handlers;
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
        private static readonly string actualTypeMethodName = $"{GetMethod}{nameof(ILocalizableViewModel.ActualType)}";

        /// <summary>
        /// The locale changed
        /// </summary>
        private static readonly string localeChanged = nameof(ILocalizableViewModel.OnLocaleChanged);

        /// <summary>
        /// The attribute handlers
        /// </summary>
        private readonly IEnumerable<ILocalizationAttributeHandler> attributeHandlers;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationInterceptor{T}" /> class.
        /// </summary>
        /// <param name="attributeHandlers">The attribute handlers.</param>
        public LocalizationInterceptor(IEnumerable<ILocalizationAttributeHandler> attributeHandlers)
        {
            this.attributeHandlers = attributeHandlers;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public override void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.Equals(actualTypeMethodName) && invocation.InvocationTarget is ILocalizableViewModel)
            {
                // Needed for view\view model resolution... It's a bit hacky... If you've got a better solution I'm all ears.
                invocation.ReturnValue = typeof(T);
                return;
            }
            else if (invocation.Method.Name.StartsWith(localeChanged) && invocation.InvocationTarget is ILocalizableViewModel)
            {
                ProcessLocaleChanged(invocation);
                return;
            }
            // Handle on change event... Decided to ditch Fody, it has a dubious MIT license (wasn't aware of it at first)
            else if (invocation.Method.Name.StartsWith(SetMethod) && invocation.InvocationTarget is ILocalizableModel)
            {
                base.Intercept(invocation);
                return;
            }
            else if (invocation.Method.Name.StartsWith(GetMethod) && invocation.InvocationTarget is ILocalizableModel)
            {
                ProcessValueRequested(invocation);
                return;
            }
            invocation.Proceed();
        }

        /// <summary>
        /// Fires the event.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="prop">The property.</param>
        protected override void FireEvent(IInvocation invocation, PropertyInfo prop)
        {
            ((ILocalizableModel)invocation.Proxy).OnPropertyChanging(prop.Name);
            invocation.Proceed();
            ((ILocalizableModel)invocation.Proxy).OnPropertyChanged(prop.Name);
        }

        /// <summary>
        /// Processes the locale changed.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        private void ProcessLocaleChanged(IInvocation invocation)
        {
            var localizationProperties = invocation.TargetType.GetProperties().Where(p => Attribute.IsDefined(p, typeof(LocalizationAttributeBase)));
            if (localizationProperties.Count() > 0)
            {
                foreach (var prop in localizationProperties)
                {
                    ((ILocalizableModel)invocation.Proxy).OnPropertyChanging(prop.Name);
                    ((ILocalizableModel)invocation.Proxy).OnPropertyChanged(prop.Name);
                }
            }
            invocation.Proceed();
        }

        /// <summary>
        /// Processes the value requested.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        private void ProcessValueRequested(IInvocation invocation)
        {
            var methodName = invocation.Method.Name.Replace(GetMethod, string.Empty);
            var prop = invocation.TargetType.GetProperty(methodName);
            var attr = Attribute.GetCustomAttribute(prop, typeof(LocalizationAttributeBase), true);
            if (attr != null)
            {
                var locAttr = (LocalizationAttributeBase)attr;
                if (attributeHandlers.Any(p => p.CanProcess(locAttr, prop, invocation.InvocationTarget as ILocalizableModel)))
                {
                    var handler = attributeHandlers.FirstOrDefault(p => p.CanProcess(locAttr, prop, invocation.InvocationTarget as ILocalizableModel));
                    if (handler.HasData(locAttr, prop, invocation.InvocationTarget as ILocalizableModel))
                    {
                        var data = handler.GetData(locAttr, prop, invocation.InvocationTarget as ILocalizableModel);
                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            invocation.ReturnValue = handler.GetData(locAttr, prop, invocation.InvocationTarget as ILocalizableModel);
                            return;
                        }
                    }
                }
            }
            invocation.Proceed();
        }

        #endregion Methods
    }
}
