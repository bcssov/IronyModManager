// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-12-2020
// ***********************************************************************
// <copyright file="SampleControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Class SampleControlViewModel.
    /// Implements the <see cref="IronyModManager.ViewModels.ViewModelBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.ViewModels.ViewModelBase" />
    public class SampleControlViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public virtual string Text => "Yay!";

        #endregion Properties
    }
}
