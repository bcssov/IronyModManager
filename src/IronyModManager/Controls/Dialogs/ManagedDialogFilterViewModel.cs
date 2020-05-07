// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 05-07-2020
// ***********************************************************************
// <copyright file="ManagedDialogFilterViewModel.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>
// Based on Avalonia ManagedFileChooserFilterViewModel. Why of why would
// the Avalonia guys expose some of this stuff?
// </summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using IronyModManager.Shared;

namespace IronyModManager.Controls.Dialogs
{
    /// <summary>
    /// Class ManagedDialogFilterViewModel.
    /// Implements the <see cref="IronyModManager.Controls.Dialogs.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Controls.Dialogs.BaseViewModel" />
    [ExcludeFromCoverage("External logic.")]
    public class ManagedDialogFilterViewModel : BaseViewModel
    {
        #region Constructors

        /// <summary>
        /// The extensions
        /// </summary>
        /// <param name="filter">The filter.</param>
        public ManagedDialogFilterViewModel(FileDialogFilter filter)
        {
            Name = filter.Name;

            if (filter.Extensions.Contains("*"))
            {
                return;
            }

            Extensions = filter.Extensions?.Select(e => "." + e.ToLowerInvariant()).ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedDialogFilterViewModel" /> class.
        /// </summary>
        public ManagedDialogFilterViewModel()
        {
            Name = "All files";
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        /// <value>The extensions.</value>
        public string[] Extensions { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Matches the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Match(string filename)
        {
            if (Extensions == null)
            {
                return true;
            }

            foreach (var ext in Extensions)
            {
                if (filename.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => Name;

        #endregion Methods
    }
}
