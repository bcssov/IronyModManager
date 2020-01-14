// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-11-2020
// ***********************************************************************
// <copyright file="Storage.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using AutoMapper;
using IronyModManager.Models;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class Storage.
    /// Implements the <see cref="IronyModManager.Storage.IStorageProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.Storage.IStorageProvider" />
    public class Storage : IStorageProvider
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage" /> class.
        /// </summary>
        /// <param name="database">The database.</param>
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
            var result = Mapper.Map<IPreferences, IPreferences>(Database.Preferences);
            return result;
        }

        /// <summary>
        /// Sets the preferences.
        /// </summary>
        /// <param name="preferences">The preferences.</param>
        public virtual void SetPreferences(IPreferences preferences)
        {
            Database.Preferences = preferences;
        }

        #endregion Methods
    }
}
