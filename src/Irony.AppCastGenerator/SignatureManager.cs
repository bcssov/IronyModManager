// ***********************************************************************
// Assembly         : Irony.AppCastGenerator
// Author           : NetSparkle
// Created          : 09-14-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2025
// ***********************************************************************
// <copyright file="SignatureManager.cs" company="NetSparkle">
//     NetSparkle
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;

namespace Irony.AppCastGenerator
{
    /// <summary>
    /// Class SignatureManager.
    /// </summary>
    public class SignatureManager
    {
        #region Fields

        /// <summary>
        /// The private key environment variable
        /// </summary>
        public const string PrivateKeyEnvironmentVariable = "SPARKLE_PRIVATE_KEY";

        /// <summary>
        /// The public key environment variable
        /// </summary>
        public const string PublicKeyEnvironmentVariable = "SPARKLE_PUBLIC_KEY";

        /// <summary>
        /// The private key file path
        /// </summary>
        private string privateKeyFilePath;

        /// <summary>
        /// The public key file path
        /// </summary>
        private string publicKeyFilePath;

        /// <summary>
        /// The storage path
        /// </summary>
        private string storagePath;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureManager" /> class.
        /// </summary>
        public SignatureManager()
        {
            SetStorageDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "netsparkle"));
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Generates the specified force.
        /// </summary>
        /// <param name="force">if set to <c>true</c> [force].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Generate(bool force = false)
        {
            if (KeysExist() && !force)
            {
                Console.WriteLine("Keys already exist, use --force to force regeneration");
                return false;
            }

            // start key generation
            Console.WriteLine("Generating key pair...");

            var Random = new SecureRandom();

            Ed25519KeyPairGenerator kpg = new Ed25519KeyPairGenerator();
            kpg.Init(new Ed25519KeyGenerationParameters(Random));

            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();
            Ed25519PrivateKeyParameters privateKey = (Ed25519PrivateKeyParameters)kp.Private;
            Ed25519PublicKeyParameters publicKey = (Ed25519PublicKeyParameters)kp.Public;

            var privateKeyBase64 = Convert.ToBase64String(privateKey.GetEncoded());
            var pubKeyBase64 = Convert.ToBase64String(publicKey.GetEncoded());

            File.WriteAllText(privateKeyFilePath, privateKeyBase64);
            File.WriteAllText(publicKeyFilePath, pubKeyBase64);

            Console.WriteLine("Storing public/private keys to " + storagePath);
            return true;
        }

        /// <summary>
        /// Gets the private key.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        public byte[] GetPrivateKey()
        {
            return ResolveKeyLocation(PrivateKeyEnvironmentVariable, privateKeyFilePath);
        }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        public byte[] GetPublicKey()
        {
            return ResolveKeyLocation(PublicKeyEnvironmentVariable, publicKeyFilePath);
        }

        /// <summary>
        /// Gets the signature for file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public string GetSignatureForFile(string filePath)
        {
            return GetSignatureForFile(new FileInfo(filePath));
        }

        /// <summary>
        /// Gets the signature for file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.String.</returns>
        public string GetSignatureForFile(FileInfo file)
        {
            if (!KeysExist())
            {
                Console.WriteLine("Keys do not exist");
                return null;
            }

            if (!file.Exists)
            {
                Console.Error.WriteLine("Target binary " + file.FullName + " does not exists");
                return null;
            }

            var data = File.ReadAllBytes(file.FullName);

            var signer = new Ed25519Signer();

            signer.Init(true, new Ed25519PrivateKeyParameters(GetPrivateKey(), 0));
            signer.BlockUpdate(data, 0, data.Length);

            return Convert.ToBase64String(signer.GenerateSignature());
        }

        /// <summary>
        /// Checks if key exists.
        /// </summary>
        /// <returns><c>true</c> if key exists, <c>false</c> otherwise.</returns>
        public bool KeysExist()
        {
            if (GetPublicKey() != null && GetPrivateKey() != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the storage directory.
        /// </summary>
        /// <param name="path">The path.</param>
        public void SetStorageDirectory(string path)
        {
            storagePath = path;

            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath!);
            }

            // ReSharper disable once StringLiteralTypo
            privateKeyFilePath = Path.Combine(storagePath, "NetSparkle_Ed25519.priv");
            publicKeyFilePath = Path.Combine(storagePath, "NetSparkle_Ed25519.pub");
        }

        /// <summary>
        /// Verifies the signature.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool VerifySignature(string filePath, string signature)
        {
            return VerifySignature(new FileInfo(filePath), signature);
        }

        /// <summary>
        /// Verifies the signature.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool VerifySignature(FileInfo file, string signature)
        {
            if (!KeysExist())
            {
                Console.WriteLine("Keys do not exist");
                return false;
            }

            if (signature == null)
            {
                Console.WriteLine("Signature at path {0} is null", file.FullName);
                return false;
            }

            var data = File.ReadAllBytes(file.FullName);

            var validator = new Ed25519Signer();
            validator.Init(false, new Ed25519PublicKeyParameters(GetPublicKey(), 0));
            validator.BlockUpdate(data, 0, data.Length);

            return validator.VerifySignature(Convert.FromBase64String(signature));
        }

        /// <summary>
        /// Resolves the key location.
        /// </summary>
        /// <param name="environmentVariableName">Name of the environment variable.</param>
        /// <param name="fileLocation">The file location.</param>
        /// <returns>System.Byte[].</returns>
        private byte[] ResolveKeyLocation(string environmentVariableName, string fileLocation)
        {
            var key = Environment.GetEnvironmentVariable(environmentVariableName);

            if (key != null)
            {
                return Convert.FromBase64String(key);
            }

            if (!File.Exists(fileLocation))
            {
                return null;
            }

            return Convert.FromBase64String(File.ReadAllText(fileLocation));
        }

        #endregion Methods
    }
}
