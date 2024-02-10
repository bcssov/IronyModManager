
// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-10-2024
//
// Last Modified By : Mario
// Last Modified On : 02-10-2024
// ***********************************************************************
// <copyright file="Args.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronyModManager.Implementation.SingleInstance
{

    /// <summary>
    /// Class Args.
    /// Implements the <see cref="EventArgs" />
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class Args
    {
        #region Properties

        /// <summary>
        /// Gets or sets the command line arguments.
        /// </summary>
        /// <value>The command line arguments.</value>
        public string[] CommandLineArgs { get; set; }

        #endregion Properties
    }
}
