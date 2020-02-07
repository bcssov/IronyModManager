// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 02-07-2020
//
// Last Modified By : Mario
// Last Modified On : 02-07-2020
// ***********************************************************************
// <copyright file="PostStartupFinder.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IronyModManager.DI.PostStartup
{
    /// <summary>
    /// Class PostStartupFinder.
    /// </summary>
    internal static class PostStartupFinder
    {
        #region Methods

        /// <summary>
        /// Finds the and execute.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public static void FindAndExecute(List<Assembly> assemblies)
        {
            var postStartups = assemblies.Select(p => p.GetTypes().Where(x => typeof(Shared.PostStartup).IsAssignableFrom(x) && !x.IsAbstract));
            foreach (var assemblyStartups in postStartups)
            {
                foreach (var assemblyStartup in assemblyStartups)
                {
                    var postStartup = Activator.CreateInstance(assemblyStartup) as Shared.PostStartup;
                    postStartup.OnPostStartup();
                }
            }
        }

        #endregion Methods
    }
}
