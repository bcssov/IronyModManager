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
using IronyModManager.Shared;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Class LocalizationInterceptor.
    /// Implements the <see cref="IronyModManager.Shared.PropertyChangedInterceptorBase" />
    /// Implements the <see cref="Castle.DynamicProxy.IInterceptor" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IronyModManager.Shared.PropertyChangedInterceptorBase" />
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

        /// <summary>
        /// The refresh handlers
        /// </summary>
        private readonly IEnumerable<ILocalizationRefreshHandler> refreshHandlers;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationInterceptor{T}" /> class.
        /// </summary>
        /// <param name="attributeHandlers">The attribute handlers.</param>
        /// <param name="refreshHandlers">The refresh handlers.</param>
        public LocalizationInterceptor(IEnumerable<ILocalizationAttributeHandler> attributeHandlers, IEnumerable<ILocalizationRefreshHandler> refreshHandlers)
        {
            this.attributeHandlers = attributeHandlers;
            this.refreshHandlers = refreshHandlers;
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
            }
            else if (invocation.Method.Name.StartsWith(localeChanged) && invocation.InvocationTarget is ILocalizableViewModel)
            {
                ProcessLocaleChanged(invocation);
                invocation.Proceed();
            }
            // Handle on change event... Decided to ditch Fody, it has a dubious MIT license (wasn't aware of it at first)
            else if (invocation.Method.Name.StartsWith(SetMethod) && invocation.InvocationTarget is ILocalizableModel)
            {
                base.Intercept(invocation);
            }
            else if (invocation.Method.Name.StartsWith(GetMethod) && invocation.InvocationTarget is ILocalizableModel)
            {
                invocation.Proceed();
                ProcessLocalizationAttributes(invocation);
            }
            else
            {
                invocation.Proceed();
            }
        }

        /// <summary>
        /// Fires the event.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="prop">The property.</param>
        protected override void FireEvent(IInvocation invocation, PropertyInfo prop)
        {
            var instance = ((ILocalizableModel)invocation.Proxy);
            instance.OnPropertyChanging(prop.Name);
            invocation.Proceed();
            instance.OnPropertyChanged(prop.Name);
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
                    var args = new LocalizationRefreshArgs(invocation, prop);
                    var refreshHandler = refreshHandlers.FirstOrDefault(p => p.CanRefresh(args));
                    if (refreshHandler != null)
                    {
                        refreshHandler.Refresh(args);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the localization attributes.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        private void ProcessLocalizationAttributes(IInvocation invocation)
        {
            var methodName = invocation.Method.Name.Replace(GetMethod, string.Empty);
            var prop = invocation.TargetType.GetProperty(methodName);
            var attr = Attribute.GetCustomAttribute(prop, typeof(LocalizationAttributeBase), true);
            if (attr != null)
            {
                var locAttr = (LocalizationAttributeBase)attr;
                var args = new AttributeHandlersArgs(locAttr, invocation, prop, invocation.ReturnValue);
                if (attributeHandlers.Any(p => p.CanProcess(args)))
                {
                    var handler = attributeHandlers.FirstOrDefault(p => p.CanProcess(args));
                    if (handler.HasData(args))
                    {
                        var data = handler.GetData(args);
                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            invocation.ReturnValue = data;
                            return;
                        }
                    }
                }
            }
        }

        #endregion Methods
    }
}
