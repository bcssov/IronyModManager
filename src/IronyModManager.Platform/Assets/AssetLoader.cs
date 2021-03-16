// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 10-02-2020
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="AssetLoader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Platform;
using IronyModManager.Shared;

namespace IronyModManager.Platform.Assets
{
    /// <summary>
    /// Class AssetLoader.
    /// Implements the <see cref="Avalonia.Platform.IAssetLoader" />
    /// </summary>
    /// <seealso cref="Avalonia.Platform.IAssetLoader" />
    internal class AssetLoader : IAssetLoader
    {
        #region Fields

        /// <summary>
        /// The font scheme
        /// </summary>
        private const string FontScheme = "font";

        /// <summary>
        /// The asset loader
        /// </summary>
        private readonly Avalonia.Shared.PlatformSupport.AssetLoader assetLoader;

        /// <summary>
        /// The font assets
        /// </summary>
        private readonly ConcurrentDictionary<string, Stream> fontAssets;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetLoader" /> class.
        /// </summary>
        public AssetLoader()
        {
            assetLoader = new Avalonia.Shared.PlatformSupport.AssetLoader(Assembly.GetEntryAssembly());
            fontAssets = new ConcurrentDictionary<string, Stream>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Registers the resource URI parsers.
        /// </summary>
        public static void RegisterResUriParsers()
        {
            if (!UriParser.IsKnownScheme(FontScheme))
                UriParser.Register(new GenericUriParser(
                    GenericUriParserOptions.GenericAuthority |
                    GenericUriParserOptions.NoUserInfo |
                    GenericUriParserOptions.NoPort |
                    GenericUriParserOptions.NoQuery |
                    GenericUriParserOptions.NoFragment), FontScheme, -1);
        }

        /// <summary>
        /// Existses the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Exists(Uri uri, Uri baseUri = null)
        {
            return GetAsset(uri) != null || assetLoader.Exists(uri, baseUri);
        }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <returns>Assembly.</returns>
        public Assembly GetAssembly(Uri uri, Uri baseUri = null)
        {
            if (IsExternalResource(uri))
            {
                return Assembly.GetEntryAssembly();
            }
            return assetLoader.GetAssembly(uri, baseUri);
        }

        /// <summary>
        /// Gets the assets.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <returns>IEnumerable&lt;Uri&gt;.</returns>
        public IEnumerable<Uri> GetAssets(Uri uri, Uri baseUri)
        {
            if (uri.IsAbsoluteUri)
            {
                if (uri.Scheme == FontScheme)
                {
                    var result = Directory.EnumerateFiles(Path.Combine(uri.Authority, Uri.UnescapeDataString(uri.AbsolutePath).StandardizeDirectorySeparator().Trim(Path.DirectorySeparatorChar)), "*.ttf", SearchOption.TopDirectoryOnly).
                        Union(Directory.EnumerateFiles(Path.Combine(uri.Authority, Uri.UnescapeDataString(uri.AbsolutePath).StandardizeDirectorySeparator().Trim(Path.DirectorySeparatorChar)), "*.otf", SearchOption.TopDirectoryOnly)).
                        Select(p => new Uri($"{uri.Scheme}://{p.Replace(AppDomain.CurrentDomain.BaseDirectory, string.Empty).Trim(Path.DirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/')}")).ToList();
                    return result;
                }
            }
            return assetLoader.GetAssets(uri, baseUri);
        }

        /// <summary>
        /// Opens the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <returns>Stream.</returns>
        public Stream Open(Uri uri, Uri baseUri = null)
        {
            return OpenAndGetAssembly(uri, baseUri).stream;
        }

        /// <summary>
        /// Opens the and get assembly.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <returns>System.ValueTuple&lt;Stream, Assembly&gt;.</returns>
        public (Stream stream, Assembly assembly) OpenAndGetAssembly(Uri uri, Uri baseUri = null)
        {
            var asset = GetAsset(uri);
            if (asset != null)
            {
                return (asset, Assembly.GetExecutingAssembly());
            }
            return assetLoader.OpenAndGetAssembly(uri, baseUri);
        }

        /// <summary>
        /// Sets the default assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void SetDefaultAssembly(Assembly assembly)
        {
            assetLoader.SetDefaultAssembly(assembly);
        }

        /// <summary>
        /// Gets the asset.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Stream.</returns>
        private Stream GetAsset(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                if (uri.Scheme == FontScheme)
                {
                    if (!fontAssets.ContainsKey(uri.ToString()))
                    {
                        var fs = File.OpenRead(Path.Combine(uri.Authority, Uri.UnescapeDataString(uri.AbsolutePath).StandardizeDirectorySeparator().Trim(Path.DirectorySeparatorChar)));
                        var ms = new MemoryStream();
                        fs.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        fontAssets.TryAdd(uri.ToString(), ms);
                        return ms;
                    }
                    return fontAssets[uri.ToString()];
                }
            }
            return null;
        }

        /// <summary>
        /// Determines whether [is external resource] [the specified URI].
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns><c>true</c> if [is external resource] [the specified URI]; otherwise, <c>false</c>.</returns>
        private bool IsExternalResource(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                if (uri.Scheme == FontScheme)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion Methods
    }
}
