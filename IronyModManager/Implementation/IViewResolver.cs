// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-12-2020
// ***********************************************************************
// <copyright file="IViewResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using Avalonia.Controls;
using IronyModManager.ViewModels;

namespace IronyModManager
{
    /// <summary>
    /// Interface IViewResolver
    /// </summary>
    public interface IViewResolver
    {
        #region Methods

        /// <summary>
        /// Formats the name of the user control.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.String.</returns>
        string FormatUserControlName(object obj);

        /// <summary>
        /// Formats the name of the view model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.String.</returns>
        string FormatViewModelName<T>();

        /// <summary>
        /// Determines whether the specified name is control.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified name is control; otherwise, <c>false</c>.</returns>
        bool IsControl(string name);

        /// <summary>
        /// Resolves the user control.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>UserControl.</returns>
        UserControl ResolveUserControl(object obj);

        /// <summary>
        /// Resolves the view model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ViewModelBase.</returns>
        BaseViewModel ResolveViewModel<T>() where T : Window;

        #endregion Methods
    }
}
