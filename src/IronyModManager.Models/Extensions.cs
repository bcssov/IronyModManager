using System;
using System.Collections.Generic;
using IronyModManager.DI.Extensions;
using IronyModManager.Models.Common;
using SimpleInjector;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Registers the model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="container">The container.</param>
        public static void RegisterModel<T, T2>(this Container container) where T : class, IModel where T2 : BaseModel, T
        {
            container.Register<T, T2>();
            container.InterceptWith<PropertyChangedInterceptor<T>>(x => x == typeof(T), true);
        }

        #endregion Methods
    }
}
