// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 02-14-2021
// ***********************************************************************
// <copyright file="DLCService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.DLC;
using IronyModManager.Services.Common;
using IronyModManager.Shared.Cache;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class DLCService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IDLCService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IDLCService" />
    public class DLCService : BaseService, IDLCService
    {
        #region Fields

        /// <summary>
        /// The cache prefix
        /// </summary>
        private const string CachePrefix = "DLC";

        /// <summary>
        /// The DLC folder
        /// </summary>
        private const string DLCFolder = "dlc";

        /// <summary>
        /// The cache
        /// </summary>
        private readonly ICache cache;

        /// <summary>
        /// The DLC parser
        /// </summary>
        private readonly IDLCParser dlcParser;

        /// <summary>
        /// The reader
        /// </summary>
        private readonly IReader reader;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DLCService" /> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="dlcParser">The DLC parser.</param>
        /// <param name="storage">The storage.</param>
        /// <param name="mapper">The mapper.</param>
        public DLCService(ICache cache, IReader reader, IDLCParser dlcParser, IStorageProvider storage, IMapper mapper) : base(storage, mapper)
        {
            this.reader = reader;
            this.dlcParser = dlcParser;
            this.cache = cache;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;IDLC&gt;.</returns>
        public Task<IReadOnlyCollection<IDLC>> GetAsync(IGame game)
        {
            if (game != null)
            {
                var cached = cache.Get<List<IDLC>>(CachePrefix, game.Type);
                if (cached != null)
                {
                    return Task.FromResult((IReadOnlyCollection<IDLC>)cached);
                }
                var result = new List<IDLC>();
                if (!string.IsNullOrWhiteSpace(game.ExecutableLocation))
                {
                    string directory = string.Empty;
                    var cleanedExePath = System.IO.Path.GetDirectoryName(game.ExecutableLocation);
                    while (!string.IsNullOrWhiteSpace(cleanedExePath))
                    {
                        directory = System.IO.Path.Combine(cleanedExePath, DLCFolder);
                        if (System.IO.Directory.Exists(directory))
                        {
                            break;
                        }
                        cleanedExePath = System.IO.Path.GetDirectoryName(cleanedExePath);
                    }
                    if (System.IO.Directory.Exists(directory))
                    {
                        var infos = reader.Read(directory);
                        if (infos != null && infos.Any())
                        {
                            foreach (var item in infos)
                            {
                                var dlcObject = dlcParser.Parse(System.IO.Path.Combine(DLCFolder, item.FileName), item.Content);
                                result.Add(Mapper.Map<IDLC>(dlcObject));
                            }
                        }
                        cache.Set(CachePrefix, game.Type, result);
                    }
                }
                return Task.FromResult((IReadOnlyCollection<IDLC>)result);
            }
            return Task.FromResult((IReadOnlyCollection<IDLC>)new List<IDLC>());
        }

        #endregion Methods
    }
}
