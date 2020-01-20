// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
// ***********************************************************************
// <copyright file="PropertyChangedInterceptor.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Class PropertyChangedInterceptor.
    /// Implements the <see cref="IronyModManager.Models.Common.PropertyChangedInterceptorBase" />
    /// Implements the <see cref="Castle.DynamicProxy.IInterceptor" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IronyModManager.Models.Common.PropertyChangedInterceptorBase" />
    /// <seealso cref="Castle.DynamicProxy.IInterceptor" />
    public class PropertyChangedInterceptor<T> : PropertyChangedInterceptorBase, IInterceptor where T : IModel
    {
        #region Methods

        /// <summary>
        /// Fires the event.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="prop">The property.</param>
        public override void FireEvent(IInvocation invocation, PropertyInfo prop)
        {
            ((IModel)invocation.Proxy).OnPropertyChanging(prop.Name);
            invocation.Proceed();
            ((IModel)invocation.Proxy).OnPropertyChanged(prop.Name);
        }

        #endregion Methods
    }
}
