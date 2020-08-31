// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 08-31-2020
// ***********************************************************************
// <copyright file="DIPackage.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.DI.Extensions;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Parser.Default;
using IronyModManager.Parser.Definitions;
using IronyModManager.Parser.Games.Stellaris;
using IronyModManager.Parser.Generic;
using IronyModManager.Parser.Mod;
using IronyModManager.Parser.Models;
using IronyModManager.Shared;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class DIPackage.
    /// Implements the <see cref="SimpleInjector.Packaging.IPackage" />
    /// </summary>
    /// <seealso cref="SimpleInjector.Packaging.IPackage" />
    [ExcludeFromCoverage("Should not test external DI.")]
    public class DIPackage : IPackage
    {
        #region Methods

        /// <summary>
        /// Registers the set of services in the specified <paramref name="container" />.
        /// </summary>
        /// <param name="container">The container the set of services is registered into.</param>
        public void RegisterServices(Container container)
        {
            container.Register<IDefinition, Definition>();
            container.RegisterWithoutTransientWarning<IIndexedDefinitions, IndexedDefinitions>();
            container.Collection.Register(typeof(IDefaultParser), new List<Type>()
            {
                typeof(DefaultParser)
            });
            container.Collection.Register(typeof(IGenericParser), new List<Type>()
            {
                 typeof(BinaryParser), typeof(DefinesParser), typeof(GraphicsParser),
                 typeof(KeyParser), typeof(LocalizationParser), typeof(Generic.WholeTextParser)
            });
            container.Collection.Register(typeof(IGameParser), new List<Type>
            {
                typeof(FlagsParser), typeof(SolarSystemInitializersParser), typeof(Games.Stellaris.WholeTextParser),
                typeof(OverwrittenParser), typeof(ScriptedVariablesParser)
            });
            container.Register<IParserManager, ParserManager>();
            container.Register<IModObject, ModObject>();
            container.Register<IModParser, ModParser>();
            container.Register<ICodeParser, CodeParser>();
            container.Register<IHierarchicalDefinitions, HierarchicalDefinitions>();
            container.Register<IParserMap, ParserMap>();
            container.Register<IScriptElement, ScriptElement>();
            container.Register<IScriptError, ScriptError>();
            container.Register<IParseResponse, ParseResponse>();
        }

        #endregion Methods
    }
}
