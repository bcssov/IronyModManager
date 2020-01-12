// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-12-2020
// ***********************************************************************
// <copyright file="StrongName.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class StrongName. This class cannot be inherited.
    /// </summary>
    internal sealed class StrongName
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StrongName" /> class.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="ArgumentNullException">name
        /// or
        /// blob
        /// or
        /// version</exception>
        public StrongName(StrongNamePublicKeyBlob blob, string name, Version version)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            if (blob == null)
            {
                throw new ArgumentNullException("blob");
            }
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }
            PublicKey = blob;
            Name = name;
            Version = version;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <value>The public key.</value>
        public StrongNamePublicKeyBlob PublicKey { get; private set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public Version Version { get; private set; }

        #endregion Properties
    }
}
