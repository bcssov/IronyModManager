
// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-24-2023
//
// Last Modified By : Mario
// Last Modified On : 06-25-2023
// ***********************************************************************
// <copyright file="StoreOptions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using MessagePack.Resolvers;

namespace IronyModManager.Shared.KeyValueStore
{

    /// <summary>
    /// Class StoreOptions.
    /// Implements the <see cref="MessagePackSerializerOptions" />
    /// </summary>
    /// <seealso cref="MessagePackSerializerOptions" />
    internal class StoreOptions : MessagePackSerializerOptions
    {
        #region Fields

        /// <summary>
        /// The resolver
        /// </summary>
        private readonly Func<string, Type> resolver;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreOptions" /> class.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        protected internal StoreOptions(Func<string, Type> resolver) : base(Standard.WithResolver(TypelessObjectResolver.Instance).WithCompression(MessagePackCompression.Lz4BlockArray))
        {
            this.resolver = resolver;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets a type given a string representation of the type.
        /// </summary>
        /// <param name="typeName">The name of the type to load. This is typically the <see cref="P:System.Type.AssemblyQualifiedName" /> but may use the assembly's simple name.</param>
        /// <returns>The loaded type or <c>null</c> if no matching type could be found.</returns>
        public override Type LoadType(string typeName)
        {
            if (resolver != null)
            {
                var result = resolver(typeName);
                if (result != null)
                {
                    return result;
                }
            }
            return base.LoadType(typeName);
        }

        #endregion Methods
    }
}
