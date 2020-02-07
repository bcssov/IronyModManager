// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Microsoft
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-17-2020
// ***********************************************************************
// <copyright file="StrongNamePublicKeyBlob.cs" company="Microsoft">
//     Microsoft
// </copyright>
// <summary>Src: https://github.com/microsoft/referencesource/blob/master/mscorlib/system/security/permissions/strongnamepublickeyblob.cs</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.DI.Assemblies
{
    /// <summary>
    /// Class StrongNamePublicKeyBlob. This class cannot be inherited.
    /// </summary>
    internal sealed class StrongNamePublicKeyBlob
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StrongNamePublicKeyBlob" /> class.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        public StrongNamePublicKeyBlob(byte[] publicKey)
        {
            PublicKey = publicKey;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <value>The public key.</value>
        public byte[] PublicKey { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is StrongNamePublicKeyBlob))
            {
                return false;
            }

            return this.Equals((StrongNamePublicKeyBlob)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return GetByteArrayHashCode(PublicKey);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
        {
            return Convert.ToBase64String(PublicKey);
        }

        /// <summary>
        /// Equalses the specified BLOB.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool Equals(StrongNamePublicKeyBlob blob)
        {
            if (blob == null)
                return false;
            else
                return CompareArrays(this.PublicKey, blob.PublicKey);
        }

        /// <summary>
        /// Compares the arrays.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool CompareArrays(byte[] first, byte[] second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }

            int count = first.Length;
            for (int i = 0; i < count; ++i)
            {
                if (first[i] != second[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the byte array hash code.
        /// </summary>
        /// <param name="baData">The ba data.</param>
        /// <returns>System.Int32.</returns>
        static private int GetByteArrayHashCode(byte[] baData)
        {
            if (baData == null)
                return 0;

            int accumulator = 0;

            for (int i = 0; i < baData.Length; ++i)
            {
                accumulator = (accumulator << 8) ^ (int)baData[i] ^ (accumulator >> 24);
            }

            return accumulator;
        }

        #endregion Methods
    }
}
