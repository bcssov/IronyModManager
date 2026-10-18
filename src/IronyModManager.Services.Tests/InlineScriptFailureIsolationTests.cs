using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.IO.Common.Game;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Definitions;
using IronyModManager.Parser.Models;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Shared.Models;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Services.Tests
{
    public class InlineScriptFailureIsolationTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Conflict_solver_excludes_the_whole_source_file_and_preserves_other_mods(bool secondaryRejection)
        {
            var fixture = new Fixture(secondaryRejection: secondaryRejection);
            var definitions = new[]
            {
                fixture.Source("A", "shared.txt", "first"),
                fixture.Source("A", "shared.txt", "fail"),
                fixture.Source("A", "shared.txt", "last"),
                fixture.Source("A", "shared.txt", "plain", inline: false),
                fixture.Source("B", "shared.txt", "other_mod"),
                fixture.Source("A", "independent.txt", "independent")
            };
            var result = await fixture.Analyze(definitions);
            var invalid = Assert.Single(result, d => d.ModName == "A" && d.File == definitions[0].File);
            Assert.Equal(ValueType.Invalid, invalid.ValueType);
            Assert.Equal(17, invalid.ErrorLine);
            Assert.Equal(9, invalid.ErrorColumn);
            Assert.Contains("limit: 100000", invalid.ErrorMessage);
            Assert.Contains("fixture.txt", invalid.ErrorMessage);
            Assert.Contains("A", invalid.ErrorMessage);
            Assert.Contains("shared.txt", invalid.ErrorMessage);
            Assert.True(invalid.ErrorMessage.Length < 1500);
            Assert.Contains(result, d => d.ModName == "B" && d.ValueType != ValueType.Invalid);
            Assert.Contains(result, d => d.File.EndsWith("independent.txt") && d.ValueType != ValueType.Invalid);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task Independent_callers_of_one_rejected_target_each_have_diagnostics()
        {
            var fixture = new Fixture();
            var result = await fixture.Analyze([
                fixture.Source("A", "one.txt", "fail"),
                fixture.Source("A", "two.txt", "fail"),
                fixture.Source("B", "one.txt", "fail")
            ]);
            Assert.Equal(3, result.Count);
            Assert.All(result, d => Assert.Equal(ValueType.Invalid, d.ValueType));
            Assert.Equal(3, result.Select(d => (d.ModName, d.FileCI)).Distinct().Count());
        }

        [Fact]
        public async Task Conflict_solver_handles_rejection_during_the_final_validation_pass()
        {
            var fixture = new Fixture(finalValidationRejection: true);
            var result = await fixture.Analyze([fixture.Source("A", "final.txt", "fail")]);
            Assert.Equal(ValueType.Invalid, Assert.Single(result).ValueType);
            Assert.Contains("limit: 100000", result[0].ErrorMessage);
        }

        [Fact]
        public async Task Error_presentation_is_bounded_and_retains_original_source_instead_of_expanded_content()
        {
            var fixture = new Fixture(rejectionReason: "syntax rejection: " + new string('x', 2000000));
            var source = fixture.Source("A", "bounded.txt", "fail");
            var invalid = Assert.Single(await fixture.Analyze([source]));
            Assert.Equal(ValueType.Invalid, invalid.ValueType);
            Assert.True(invalid.ErrorMessage.Length < 1500);
            Assert.Contains("syntax rejection:", invalid.ErrorMessage);
            Assert.Equal(source.OriginalCode, invalid.Code);
        }

        [Fact]
        public async Task Rejected_source_does_not_repeat_its_expansion_and_failures_are_local_to_one_analysis()
        {
            var fixture = new Fixture();
            var definitions = Enumerable.Range(0, 12).Select(i => fixture.Source("A", "failed.txt", "fail", "item_" + i)).ToList();
            var first = await fixture.Analyze(definitions);
            Assert.Equal(ValueType.Invalid, Assert.Single(first).ValueType);
            Assert.Equal(1, fixture.Expansions["fail"]);
            await fixture.Analyze([fixture.Source("A", "failed.txt", "fail")]);
            Assert.Equal(2, fixture.Expansions["fail"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Provider_capability_controls_conflict_solver_inline_processing(bool enabled)
        {
            var fixture = new Fixture(supportsInline: enabled);
            var source = fixture.Source("A", "capability.txt", "fail");
            var result = await fixture.Analyze([source]);
            Assert.Equal(enabled ? ValueType.Invalid : ValueType.Object, Assert.Single(result).ValueType);
            Assert.Equal(enabled ? 1 : 0, fixture.Expansions.Count);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Game_indexing_rejects_the_whole_file_and_keeps_independent_files(bool secondaryRejection)
        {
            var fixture = new Fixture(secondaryRejection: secondaryRejection);
            var first = fixture.Source("game", "shared.txt", "first");
            var failure = fixture.Source("game", "shared.txt", "fail");
            var last = fixture.Source("game", "shared.txt", "last");
            var plain = fixture.Source("game", "shared.txt", "plain", inline: false);
            var good = fixture.Source("game", "independent.txt", "good");
            var result = await fixture.Index([first, failure, last, plain, good]);
            var invalid = Assert.Single(result, d => d.File == first.File);
            Assert.Equal(ValueType.Invalid, invalid.ValueType);
            Assert.Contains("limit: 100000", invalid.ErrorMessage);
            Assert.Contains(result, d => d.File == good.File && d.ValueType != ValueType.Invalid);
            Assert.False(fixture.Expansions.ContainsKey("last"));
            Assert.Equal(2, result.Count);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Provider_capability_controls_game_indexing_inline_processing(bool enabled)
        {
            var fixture = new Fixture(supportsInline: enabled);
            var source = fixture.Source("game", "capability.txt", "fail");
            var result = await fixture.Index([source]);
            Assert.Equal(enabled ? ValueType.Invalid : ValueType.Object, Assert.Single(result).ValueType);
            Assert.Equal(enabled ? 1 : 0, fixture.Expansions.Count);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Original_game_parser_rejection_cannot_leave_siblings_for_an_inline_capable_provider(bool enabled)
        {
            var fixture = new Fixture(supportsInline: enabled);
            var invalid = fixture.Source("game", "original.txt", "invalid", inline: false);
            invalid.ValueType = ValueType.Invalid;
            invalid.ErrorMessage = "original source rejected";
            var sibling = fixture.Source("game", "original.txt", "sibling", inline: false);
            var result = await fixture.Index([sibling, invalid]);
            if (enabled)
                Assert.Empty(result);
            else
                Assert.Equal(sibling, Assert.Single(result));
        }

        [Fact]
        public async Task Unexpected_expansion_exception_is_not_an_invalid_file()
        {
            var fixture = new Fixture(unexpectedFailure: true);
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => fixture.Analyze([fixture.Source("A", "defect.txt", "fail")]));
            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        private delegate string Expand(string code, string parameters, out bool processed, bool force);

        private sealed class Fixture
        {
            private readonly Mock<IDefinitionInfoProvider> provider = new();
            private readonly Mock<IParametrizedParser> parametrized = new();
            private readonly Mock<IParserManager> manager = new();
            private readonly Mock<IGameService> gameService = new();
            private readonly Mock<IMessageBus> messageBus = new();
            private readonly IGame game = new Game { Name = "game", Type = "CapabilityTestGame" };
            private readonly string inlineFolder = Path.Combine("common", "inline_scripts");
            private readonly string sourceFolder = Path.Combine("common", "button_effects");
            private readonly bool secondaryRejection;
            private IReadOnlyList<IDefinition> gameSources;

            public ConcurrentDictionary<string, int> Expansions { get; } = new();

            public Fixture(bool supportsInline = true, bool secondaryRejection = false, bool unexpectedFailure = false,
                bool finalValidationRejection = false, string rejectionReason = "File exceeded allowed number of lines. Lines: 171314; limit: 100000.")
            {
                DISetup.SetupContainer();
                this.secondaryRejection = secondaryRejection || finalValidationRejection;
                provider.Setup(p => p.CanProcess("CapabilityTestGame")).Returns(true);
                provider.SetupGet(p => p.SupportsInlineScripts).Returns(supportsInline);
                provider.SetupGet(p => p.InlineScriptsPath).Returns(inlineFolder);
                provider.SetupGet(p => p.GlobalVariablesPath).Returns(Path.Combine("common", "scripted_variables"));
                gameService.Setup(p => p.GetSelected()).Returns(game);
                parametrized.Setup(p => p.GetOptimizedScriptPath(It.IsAny<string>())).Returns("fixture");
                parametrized.Setup(p => p.GetFirstOptimizedScriptPath(It.IsAny<string>())).Returns("fixture");
                var processed = true;
                parametrized.Setup(p => p.ProcessOptimized(It.IsAny<string>(), It.IsAny<string>(), out processed, false))
                    .Returns(new Expand((string code, string parameters, out bool logicProcessed, bool force) =>
                    {
                        logicProcessed = true;
                        parameters = parameters.Trim();
                        Expansions.AddOrUpdate(parameters, 1, (_, count) => count + 1);
                        if (parameters == "fail" && !this.secondaryRejection)
                        {
                            if (unexpectedFailure) throw new InvalidOperationException("test defect");
                            throw new ScriptParseException(new ScriptError { Message = rejectionReason, Line = 17, Column = 9 });
                        }

                        return parameters;
                    }));
                manager.Setup(p => p.Parse(It.IsAny<ParserManagerArgs>())).Returns((ParserManagerArgs args) =>
                {
                    if (args.ContentSHA == "original-game-file")
                        return gameSources.Where(d => d.File == args.File).ToList();
                    var text = string.Join(Environment.NewLine, args.Lines).Trim();
                    var result = Source(args.ModName, Path.GetFileName(args.File), "expanded_" + text, inline: false);
                    if (text == "fail" && this.secondaryRejection && (!finalValidationRejection || args.ValidationType != ValidationType.SkipAll))
                    {
                        result.ValueType = ValueType.Invalid;
                        result.ErrorMessage = rejectionReason;
                        result.ErrorLine = 17;
                        result.ErrorColumn = 9;
                    }

                    return [result];
                });
            }

            public Definition Source(string mod, string file, string code, string id = null, bool inline = true)
            {
                return new Definition
                {
                    ModName = mod, File = Path.Combine(sourceFolder, file), Type = sourceFolder,
                    Id = id ?? code, Code = code, OriginalCode = code, ValueType = ValueType.Object,
                    ContainsInlineIdentifier = inline
                };
            }

            private Definition Target()
            {
                return new Definition
                {
                    File = Path.Combine(inlineFolder, "fixture.txt"), Type = inlineFolder, Id = "fixture.txt",
                    ModName = "scripts", Code = "fixture", OriginalCode = "fixture", ValueType = ValueType.WholeTextFile
                };
            }

            public async Task<List<IDefinition>> Analyze(IEnumerable<IDefinition> sources)
            {
                var indexed = new IndexedDefinitions();
                await indexed.InitMapAsync(sources.Append(Target()));
                var service = new ModPatchCollectionService(new Cache(), messageBus.Object, manager.Object, [provider.Object],
                    Mock.Of<IModPatchExporter>(), Mock.Of<IReader>(), Mock.Of<IModWriter>(), Mock.Of<IModParser>(),
                    gameService.Object, Mock.Of<IStorageProvider>(), Mock.Of<IMapper>(), Mock.Of<IDefinitionShallowComparer>(),
                    Mock.Of<IValidateParser>(), parametrized.Object, Mock.Of<IParserMerger>(), Mock.Of<IGameStateSafetyService>());
                var result = await service.FindConflictsAsync(indexed, ["scripts", "A", "B"], PatchStateMode.Advanced, null);
                var definitions = (await result.AllConflicts.GetAllAsync()).Where(d => !d.File.StartsWith(inlineFolder)).ToList();
                return definitions;
            }

            public async Task<List<IDefinition>> Index(IReadOnlyList<IDefinition> sources)
            {
                gameSources = sources;
                var inlineDefinitions = new IndexedDefinitions();
                await inlineDefinitions.InitMapAsync([Target()]);
                var variables = new IndexedDefinitions();
                await variables.InitMapAsync([]);
                var files = sources.Select(s => s.File).Distinct().Select(f => (IFileInfo)new IronyModManager.IO.FileInfo
                {
                    FileName = Path.GetFileName(f), Content = ["original"], ContentSHA = "original-game-file"
                });
                var service = new IndexProbe(parametrized.Object, messageBus.Object, manager.Object, provider.Object);
                return (await service.Parse(game, files, sourceFolder, inlineDefinitions, variables)).ToList();
            }
        }

        private sealed class IndexProbe(IParametrizedParser parser, IMessageBus bus, IParserManager manager, IDefinitionInfoProvider provider)
            : GameIndexService(parser, bus, manager, Mock.Of<IGameIndexer>(), new Cache(), [provider], Mock.Of<IReader>(),
                Mock.Of<IModWriter>(), Mock.Of<IModParser>(), Mock.Of<IGameService>(), Mock.Of<IStorageProvider>(), Mock.Of<IMapper>())
        {
            public Task<IEnumerable<IDefinition>> Parse(IGame game, IEnumerable<IFileInfo> files, string folder, IIndexedDefinitions inlines, IIndexedDefinitions variables)
            {
                return ParseGameFiles(game, files, folder, inlines, variables);
            }
        }
    }
}
