// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-23-2024
//
// Last Modified By : Mario
// Last Modified On : 02-23-2024
// ***********************************************************************
// <copyright file="GC.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IronyModManager.Shared
{
    /// <summary>
    /// The gc runner.
    /// </summary>
    public class GCRunner
    {
        #region Methods

        /// <summary>
        /// Run gc.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="blocking">If true, blocking.</param>
        /// <returns></returns>
        public static void RunGC(GCCollectionMode mode, bool blocking = false)
        {
            if (blocking)
            {
                Task.Run(() =>
                {
                    GC.Collect(GC.MaxGeneration, mode, blocking, blocking);
                    GC.WaitForPendingFinalizers();
                    GC.Collect(GC.MaxGeneration, mode, blocking, blocking);
                });
            }
            else
            {
                Task.Run(() =>
                {
                    GC.Collect(GC.MaxGeneration, mode);
                    GC.WaitForPendingFinalizers();
                    GC.Collect(GC.MaxGeneration, mode);
                });
            }
        }

        #endregion Methods
    }
}
