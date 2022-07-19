// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 07-11-2022
// ***********************************************************************
// <copyright file="ParadoxLauncherHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Platforms;
using IronyModManager.Shared;

namespace IronyModManager.IO.Platforms
{
    /// <summary>
    /// Class ParadoxLauncherHandler.
    /// Implements the <see cref="IParadoxLauncher" />
    /// </summary>
    /// <seealso cref="IParadoxLauncher" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ParadoxLauncherHandler : IParadoxLauncher
    {
        #region Methods

        /// <summary>
        /// Determines whether [is running asynchronous].
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> IsRunningAsync()
        {
            var processes = Process.GetProcesses();
            return Task.FromResult(processes.Any(p => p.ProcessName.Equals("Paradox Launcher", StringComparison.OrdinalIgnoreCase)));
        }

        #endregion Methods
    }
}
