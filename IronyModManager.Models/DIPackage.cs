using System;
using System.Collections.Generic;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace IronyModManager.Models
{
    public class DIPackage : IPackage
    {
        #region Methods

        /// <summary>
        /// Registers the set of services in the specified <paramref name="container" />.
        /// </summary>
        /// <param name="container">The container the set of services is registered into.</param>
        public void RegisterServices(Container container)
        {
            container.Register<IPreferences, Preferences>();
            container.Register<ITheme, Theme>();
        }

        #endregion Methods
    }
}
