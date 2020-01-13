// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-12-2020
// ***********************************************************************
// <copyright file="ViewLocator.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using IronyModManager.DI;
using IronyModManager.Log;
using IronyModManager.ViewModels;

namespace IronyModManager
{
    /// <summary>
    /// Class ViewLocator.
    /// Implements the <see cref="Avalonia.Controls.Templates.IDataTemplate" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Templates.IDataTemplate" />
    public class ViewLocator : IDataTemplate
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether [supports recycling].
        /// </summary>
        /// <value><c>true</c> if [supports recycling]; otherwise, <c>false</c>.</value>
        public bool SupportsRecycling => false;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Builds the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>IControl.</returns>
        public IControl Build(object data)
        {
            var resolver = DIResolver.Get<IViewResolver>();
            var name = resolver.FormatUserControlName(data);
            if (resolver.IsControl(name))
            {
                try
                {
                    var control = resolver.ResolveUserControl(data);
                    return control;
                }
                catch (Exception e)
                {
                    var message = $"Not Found: {name}";

                    var logger = DIResolver.Get<ILogger>();
                    logger.Error(e);

                    return new TextBlock { Text = message };
                }
            }
            else
            {
                var message = $"Not Supported: {name}";

                var logger = DIResolver.Get<ILogger>();
                logger.Info(message);

                return new TextBlock { Text = message };
            }
        }

        /// <summary>
        /// Matches the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Match(object data)
        {
            return data is ViewModelBase;
        }

        #endregion Methods
    }
}
