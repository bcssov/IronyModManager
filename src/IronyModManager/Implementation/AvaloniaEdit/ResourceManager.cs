// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 07-23-2022
//
// Last Modified By : Mario
// Last Modified On : 07-23-2022
// ***********************************************************************
// <copyright file="ResourceManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Shared;
using AE = AvaloniaEdit;

namespace IronyModManager.Implementation.AvaloniaEdit
{
    /// <summary>
    /// Class ResourceManager.
    /// Implements the <see cref="System.Resources.ResourceManager" />
    /// </summary>
    /// <seealso cref="System.Resources.ResourceManager" />
    public class ResourceManager : System.Resources.ResourceManager
    {
        #region Fields

        /// <summary>
        /// The localization manager
        /// </summary>
        private ILocalizationManager localizationManager;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Init()
        {
            // Hackjob, strings are hardcoded more or less and I don't need another resource provider we want to plug in our own
            var field = typeof(AE.SR).GetField("resourceMan", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(null, new ResourceManager());
        }

        /// <summary>
        /// Returns the value of the specified string resource.
        /// </summary>
        /// <param name="name">The name of the resource to retrieve.</param>
        /// <returns>The value of the resource localized for the caller's current UI culture, or <see langword="null" /> if <paramref name="name" /> cannot be found in a resource set.</returns>
        public override string GetString(string name)
        {
            if (localizationManager == null)
            {
                localizationManager = DIResolver.Get<ILocalizationManager>();
            }
            return localizationManager.GetResource($"{LocalizationResources.Conflict_Solver.EditorResources.Prefix}{name}");
        }

        /// <summary>
        /// Returns the value of the string resource localized for the specified culture.
        /// </summary>
        /// <param name="name">The name of the resource to retrieve.</param>
        /// <param name="culture">An object that represents the culture for which the resource is localized.</param>
        /// <returns>The value of the resource localized for the specified culture, or <see langword="null" /> if <paramref name="name" /> cannot be found in a resource set.</returns>
        public override string GetString(string name, CultureInfo culture)
        {
            return this.GetString(name);
        }

        #endregion Methods
    }
}
