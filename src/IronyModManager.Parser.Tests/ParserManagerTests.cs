// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="ParserManagerTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Definitions;
using IronyModManager.Shared.Models;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class ParserManagerTests.
    /// </summary>
    public class ParserManagerTests
    {

        /// <summary>
        /// Defines the test method Should_invoke_game_parser.
        /// </summary>
        [Fact]
        public void Should_invoke_game_parser()
        {
            var defaultParser = new List<IDefaultParser>() { new DefaultParser() };
            var genericParser = new List<IGenericParser>() { new GenericParser() };
            var gameParser = new List<IGameParser>() { new GameParser() };

            var manager = new ParserManager(gameParser, genericParser, defaultParser);
            var result = manager.Parse(new ParserManagerArgs()
            {
                File = "fake-game",
                GameType = "game"
            });
            result.First().File.Should().Be("game_parser");
            result.First().UsedParser.Should().Be("GameParser");
        }

        /// <summary>
        /// Defines the test method Should_invoke_generic_parser.
        /// </summary>
        [Fact]
        public void Should_invoke_generic_parser()
        {
            var defaultParser = new List<IDefaultParser>() { new DefaultParser() };
            var genericParser = new List<IGenericParser>() { new GenericParser() };
            var gameParser = new List<IGameParser>() { new GameParser() };

            var manager = new ParserManager(gameParser, genericParser, defaultParser);
            var result = manager.Parse(new ParserManagerArgs()
            {
                File = "fake-generic",
                GameType = "game"
            });
            result.First().File.Should().Be("generic_parser");
            result.First().UsedParser.Should().Be("GenericParser");
        }

        /// <summary>
        /// Defines the test method Should_invoke_default_parser.
        /// </summary>
        [Fact]
        public void Should_invoke_default_parser()
        {
            var defaultParser = new List<IDefaultParser>() { new DefaultParser() };
            var genericParser = new List<IGenericParser>() { new GenericParser() };
            var gameParser = new List<IGameParser>() { new GameParser() };

            var manager = new ParserManager(gameParser, genericParser, defaultParser);
            var result = manager.Parse(new ParserManagerArgs()
            {
                File = "fake",
                GameType = "game"
            });
            result.First().File.Should().Be("default_parser");
            result.First().UsedParser.Should().Be("DefaultParser");
        }

        /// <summary>
        /// Defines the test method Should_mark_all_definitions_as_placholders.
        /// </summary>
        [Fact]
        public void Should_mark_all_definitions_as_placholders()
        {
            var defaultParser = new List<IDefaultParser>() { new DefaultParser() };
            var genericParser = new List<IGenericParser>() { new GenericParser() };
            var gameParser = new List<IGameParser>() { new PlaceholderFileParser() };

            var manager = new ParserManager(gameParser, genericParser, defaultParser);
            var result = manager.Parse(new ParserManagerArgs()
            {
                File = "fake-game",
                GameType = "game",
                Lines = new List<string>()
                {
                    Parser.Common.Constants.Scripts.PlaceholderFileComment
                }
            });
            var testResult = result.ToList().TrueForAll(p => p.IsPlaceholder);
            testResult.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_mark_single_definition_as_placholders.
        /// </summary>
        [Fact]
        public void Should_mark_single_definition_as_placholders()
        {
            var defaultParser = new List<IDefaultParser>() { new DefaultParser() };
            var genericParser = new List<IGenericParser>() { new GenericParser() };
            var gameParser = new List<IGameParser>() { new PlaceholderFileParser() };

            var manager = new ParserManager(gameParser, genericParser, defaultParser);
            var result = manager.Parse(new ParserManagerArgs()
            {
                File = "fake-game",
                GameType = "game",
                Lines = new List<string>()
                {
                    Parser.Common.Constants.Scripts.PlaceholderObjectsComment + "id2"
                }
            });
            result.FirstOrDefault(p => p.Id.Equals("id2")).IsPlaceholder.Should().BeTrue();
            var testResult = result.Where(p => p.Id != "id2").ToList().TrueForAll(p => !p.IsPlaceholder);
            testResult.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_mark_any_definition_as_placholders.
        /// </summary>
        [Fact]
        public void Should_not_mark_any_definition_as_placholders()
        {
            var defaultParser = new List<IDefaultParser>() { new DefaultParser() };
            var genericParser = new List<IGenericParser>() { new GenericParser() };
            var gameParser = new List<IGameParser>() { new PlaceholderFileParser() };

            var manager = new ParserManager(gameParser, genericParser, defaultParser);
            var result = manager.Parse(new ParserManagerArgs()
            {
                File = "fake-game",
                GameType = "game",
                Lines = new List<string>()
                {
                    Parser.Common.Constants.Scripts.PlaceholderObjectsComment
                }
            });
            var testResult = result.ToList().TrueForAll(p => !p.IsPlaceholder);
            testResult.Should().BeTrue();
        }

        /// <summary>
        /// Class DefaultParser.
        /// Implements the <see cref="IronyModManager.Parser.Default.IDefaultParser" />
        /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IDefaultParser" />
        /// </summary>
        /// <seealso cref="IronyModManager.Parser.Common.Parsers.IDefaultParser" />
        /// <seealso cref="IronyModManager.Parser.Default.IDefaultParser" />
        class DefaultParser : IDefaultParser
        {
            /// <summary>
            /// Gets the name of the parser.
            /// </summary>
            /// <value>The name of the parser.</value>
            public string ParserName => nameof(DefaultParser);

            /// <summary>
            /// Determines whether this instance can parse the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
            public bool CanParse(CanParseArgs args)
            {
                return true;
            }

            /// <summary>
            /// Parses the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
            public IEnumerable<IDefinition> Parse(ParserArgs args)
            {
                return new List<IDefinition>() {
                    new Definition() { File = "default_parser" }
                };
            }
        }

        /// <summary>
        /// Class GenericParser.
        /// Implements the <see cref="IronyModManager.Parser.Generic.IGenericParser" />
        /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
        /// </summary>
        /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
        /// <seealso cref="IronyModManager.Parser.Generic.IGenericParser" />
        class GenericParser : IGenericParser
        {
            /// <summary>
            /// Gets the name of the parser.
            /// </summary>
            /// <value>The name of the parser.</value>
            public string ParserName => nameof(GenericParser);

            /// <summary>
            /// Gets the priority.
            /// </summary>
            /// <value>The priority.</value>
            public int Priority => 10;

            /// <summary>
            /// Determines whether this instance can parse the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
            public bool CanParse(CanParseArgs args)
            {
                return args.File == "fake-generic";
            }

            /// <summary>
            /// Parses the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
            public IEnumerable<IDefinition> Parse(ParserArgs args)
            {
                return new List<IDefinition>() {
                    new Definition() { File = "generic_parser" }
                };
            }
        }

        /// <summary>
        /// Class GameParser.
        /// Implements the <see cref="IronyModManager.Parser.Games.IGameParser" />
        /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
        /// </summary>
        /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
        /// <seealso cref="IronyModManager.Parser.Games.IGameParser" />
        class GameParser : IGameParser
        {
            /// <summary>
            /// Gets the name of the parser.
            /// </summary>
            /// <value>The name of the parser.</value>
            public string ParserName => nameof(GameParser);

            /// <summary>
            /// Gets the priority.
            /// </summary>
            /// <value>The priority.</value>
            public int Priority => 10;

            /// <summary>
            /// Determines whether this instance can parse the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
            public bool CanParse(CanParseArgs args)
            {
                return args.File == "fake-game";
            }

            /// <summary>
            /// Parses the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
            public IEnumerable<IDefinition> Parse(ParserArgs args)
            {
                return new List<IDefinition>() {
                    new Definition() { File = "game_parser" }
                };
            }
        }

        class PlaceholderFileParser : IGameParser
        {
            /// <summary>
            /// Gets the name of the parser.
            /// </summary>
            /// <value>The name of the parser.</value>
            public string ParserName => nameof(GameParser);

            /// <summary>
            /// Gets the priority.
            /// </summary>
            /// <value>The priority.</value>
            public int Priority => 10;

            /// <summary>
            /// Determines whether this instance can parse the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
            public bool CanParse(CanParseArgs args)
            {
                return args.File == "fake-game";
            }

            /// <summary>
            /// Parses the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
            public IEnumerable<IDefinition> Parse(ParserArgs args)
            {
                return new List<IDefinition>() {
                    new Definition() { File = "game_parser", Id = "id1" },
                    new Definition() { File = "game_parser2", Id = "id2" }
                };
            }
        }

        class PlaceholderObjectParser : IGameParser
        {
            /// <summary>
            /// Gets the name of the parser.
            /// </summary>
            /// <value>The name of the parser.</value>
            public string ParserName => nameof(GameParser);

            /// <summary>
            /// Gets the priority.
            /// </summary>
            /// <value>The priority.</value>
            public int Priority => 10;

            /// <summary>
            /// Determines whether this instance can parse the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
            public bool CanParse(CanParseArgs args)
            {
                return args.File == "fake-game";
            }

            /// <summary>
            /// Parses the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
            public IEnumerable<IDefinition> Parse(ParserArgs args)
            {
                return new List<IDefinition>() {
                    new Definition() { File = "game_parser", Id = "id1" },
                    new Definition() { File = "game_parser2", Id = "id2" }
                };
            }
        }
    }
}
