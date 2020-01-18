// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-17-2020
// ***********************************************************************
// <copyright file="ViewResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using Avalonia.Controls;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;

namespace IronyModManager
{
    /// <summary>
    /// Class ViewResolver.
    /// Implements the <see cref="IronyModManager.Common.IViewResolver" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.IViewResolver" />
    public class ViewResolver : IViewResolver
    {
        #region Fields

        /// <summary>
        /// The control pattern
        /// </summary>
        private const string ControlPattern = "Control";

        #endregion Fields

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
            return $"{typeof(T).FullName.Replace(".Views.", ".ViewModels.I")}ViewModel";
        }

        /// <summary>
        /// Determines whether the specified name is control.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified name is control; otherwise, <c>false</c>.</returns>
        public bool IsControl(string name)
        {
            return name.Contains(ControlPattern);
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
        public IViewModel ResolveViewModel<T>() where T : Window
        {
            var name = FormatViewModelName<T>();
            var type = Type.GetType(name);
            return (IViewModel)DIResolver.Get(type);
        }

        #endregion Methods
    }
}
