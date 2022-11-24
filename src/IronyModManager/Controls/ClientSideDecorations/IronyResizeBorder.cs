// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-23-2022
//
// Last Modified By : Mario
// Last Modified On : 11-23-2022
// ***********************************************************************
// <copyright file="IronyResizeBorder.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace IronyModManager.Controls.ClientSideDecorations
{
    /// <summary>
    /// Class IronyResizeeBorder.
    /// </summary>
    public class IronyResizeBorder : TemplatedControl, IStyleable
    {
        #region Properties

        /// <summary>
        /// Gets the type by which the control is styled.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(IronyResizeBorder);

        #endregion Properties
    }
}
