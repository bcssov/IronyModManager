// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-12-2020
// ***********************************************************************
// <copyright file="ViewResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using Avalonia.Controls;
using IronyModManager.DI;
using IronyModManager.ViewModels;

namespace IronyModManager
{
    /// <summary>
    /// Class ViewResolver.
    /// Implements the <see cref="IronyModManager.IViewResolver" />
    /// </summary>
    /// <seealso cref="IronyModManager.IViewResolver" />
    public class ViewResolver : IViewResolver
    {
        #region Methods

        /// <summary>
        /// Formats the name of the user control.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.String.</returns>
        public string FormatUserControlName(object obj)
        {
            return obj.GetType().FullName.Replace("ViewModel", "View");
        }

        /// <summary>
        /// Formats the name of the view model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.String.</returns>
        public string FormatViewModelName<T>()
        {
            return $"{typeof(T).FullName.Replace(".Views.", ".ViewModels.")}ViewModel";
        }

        /// <summary>
        /// Resolves the user control.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>UserControl.</returns>
        public UserControl ResolveUserControl(object obj)
        {
            var name = FormatUserControlName(obj);
            var type = Type.GetType(name);
            return (UserControl)DIResolver.Get(type);
        }

        /// <summary>
        /// Resolves the view model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ViewModelBase.</returns>
        public ViewModelBase ResolveViewModel<T>() where T : Window
        {
            var name = FormatViewModelName<T>();
            var type = Type.GetType(name);
            return (ViewModelBase)DIResolver.Get(type);
        }

        #endregion Methods
    }
}
