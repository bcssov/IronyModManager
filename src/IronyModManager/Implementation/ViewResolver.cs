// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 02-08-2020
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
using IronyModManager.DI.Assemblies;

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
        public virtual string FormatUserControlName(object obj)
        {
            Type type;
            if (obj.GetType().FullName.Contains(Shared.Constants.ProxyNamespace))
            {
                type = ((IViewModel)obj).ActualType;
            }
            else
            {
                type = obj.GetType();
            }
            return type.FullName.Replace("ViewModel", "View");
        }

        /// <summary>
        /// Formats the name of the view model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.String.</returns>
        public virtual string FormatViewModelName<T>()
        {
            var result = typeof(T).FullName.Replace("View", "ViewModel");
            if (!result.EndsWith("ViewModel"))
            {
                return $"{result}ViewModel";
            }
            return result;
        }

        /// <summary>
        /// Determines whether the specified name is control.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified name is control; otherwise, <c>false</c>.</returns>
        public virtual bool IsControl(string name)
        {
            return name.Contains(ControlPattern);
        }

        /// <summary>
        /// Resolves the user control.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>UserControl.</returns>
        public virtual UserControl ResolveUserControl(object obj)
        {
            var name = FormatUserControlName(obj);
            var type = AssemblyManager.FindType(name);
            return (UserControl)DIResolver.Get(type);
        }

        /// <summary>
        /// Resolves the view model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ViewModelBase.</returns>
        public virtual IViewModel ResolveViewModel<T>() where T : Window
        {
            var name = FormatViewModelName<T>();
            var type = AssemblyManager.FindType(name);
            return (IViewModel)DIResolver.Get(type);
        }

        #endregion Methods
    }
}
