// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 02-07-2020
// ***********************************************************************
// <copyright file="Storage.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IronyModManager.Models.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class Storage.
    /// Implements the <see cref="IronyModManager.Storage.Common.IStorageProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.Storage.Common.IStorageProvider" />
    public class Storage : IStorageProvider
    {
        #region Fields

        /// <summary>
        /// The database lock
        /// </summary>
        private static readonly object dbLock = new { };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage" /> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="mapper">The mapper.</param>
        public Storage(IDatabase database, IMapper mapper)
        {
            Database = database;
            Mapper = mapper;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        protected IDatabase Database { get; private set; }

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <value>The mapper.</value>
        protected IMapper Mapper { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the preferences.
        /// </summary>
        /// <returns>IPreferences.</returns>
        public virtual IPreferences GetPreferences()
        {
            lock (dbLock)
            {
                var result = Mapper.Map<IPreferences, IPreferences>(Database.Preferences);
                return result;
            }
        }

        /// <summary>
        /// Gets the themes.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, IEnumerable&lt;System.String&gt;&gt;.</returns>
        public virtual IEnumerable<IThemeType> GetThemes()
        {
            lock (dbLock)
            {
                return Database.Themes;
            }
        }

        /// <summary>
        /// Gets the state of the window.
        /// </summary>
        /// <returns>IWindowState.</returns>
        public virtual IWindowState GetWindowState()
        {
            lock (dbLock)
            {
                var result = Mapper.Map<IWindowState, IWindowState>(Database.WindowState);
                return result;
            }
        }

        /// <summary>
        /// Registers the theme.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="styles">The styles.</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">There is already a default theme registered.</exception>
        public virtual bool RegisterTheme(string name, IEnumerable<string> styles, bool isDefault = false)
        {
            lock (dbLock)
            {
                if (isDefault && Database.Themes.Any(s => s.IsDefault))
                {
                    throw new InvalidOperationException("There is already a default theme registered.");
                }
                Database.Themes.Add(new ThemeType()
                {
                    IsDefault = isDefault,
                    Name = name,
                    Styles = styles
                });
                return true;
            }
        }

        /// <summary>
        /// Sets the preferences.
        /// </summary>
        /// <param name="preferences">The preferences.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool SetPreferences(IPreferences preferences)
        {
            lock (dbLock)
            {
                Database.Preferences = preferences;
                return true;
            }
        }

        /// <summary>
        /// Sets the state of the window.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool SetWindowState(IWindowState state)
        {
            lock (dbLock)
            {
                Database.WindowState = state;
                return true;
            }
        }

        #endregion Methods
    }
}
