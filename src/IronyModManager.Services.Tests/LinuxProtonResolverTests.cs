using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AwesomeAssertions;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Services.Registrations;
using IronyModManager.Shared;
using IronyModManager.Storage;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    public class LinuxProtonResolverTests : IDisposable
    {
        private readonly string directory = Path.Combine(Path.GetTempPath(), "irony-proton-" + Guid.NewGuid().ToString("N"));
        private readonly Dictionary<long, string> originalMappings;
        private readonly FieldInfo rootField = typeof(SteamDirectory).GetField("baseSteamLocation", BindingFlags.Static | BindingFlags.NonPublic);
        private readonly string originalRoot;
        private readonly Dictionary<long, string> mappings = (Dictionary<long, string>)typeof(SteamDirectory).GetField("protonCache", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        private readonly Dictionary<string, List<string>> cachedPaths = (Dictionary<string, List<string>>)typeof(SteamDirectory).GetField("cachedPaths", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        private readonly Dictionary<string, List<string>> originalPaths;

        public LinuxProtonResolverTests()
        {
            Directory.CreateDirectory(directory);
            originalMappings = new Dictionary<long, string>(mappings);
            originalRoot = (string)rootField!.GetValue(null);
            originalPaths = new Dictionary<string, List<string>>(cachedPaths);
        }

        private string Root(string name)
        {
            var path = Path.Combine(directory, name);
            Directory.CreateDirectory(path);
            return path;
        }

        private string FileAt(string root, string relative, string text = "")
        {
            var path = Path.Combine(root, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, text);
            return path;
        }

        private string Proton(string root, string name) => FileAt(root, Path.Combine("steamapps", "common", name, "proton"));

        [Fact]
        public void Client_candidates_should_preserve_precedence_without_enumerating_libraries()
        {
            var client = Root("client");
            var expected = Proton(client, "Proton 9.0");
            IEnumerable<string> Libraries()
            {
                throw new InvalidOperationException("Client candidates must avoid library IO.");
#pragma warning disable CS0162
                yield break;
#pragma warning restore CS0162
            }
            LinuxProtonResolver.ResolveProtonPath(client, "proton_10", Libraries()).Should().Be(expected);
            LinuxProtonResolver.ResolveProtonPath(client, "proton_10").Should().Be(expected);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Should_find_Proton_in_secondary_or_third_registered_library(int index)
        {
            var client = Root("client");
            var libraries = new[] { client, Root("secondary"), Root("third") };
            var expected = Proton(libraries[index], "Proton 10.0");
            LinuxProtonResolver.ResolveProtonPath(client, "proton_10", libraries).Should().Be(expected);
        }

        [Fact]
        public void Should_select_requested_version_from_combined_additional_pool()
        {
            var client = Root("client");
            var second = Root("second");
            var third = Root("third");
            Proton(second, "Proton - Experimental");
            Proton(second, "Proton 9.0");
            var expected = Proton(third, "Proton 10.0");
            LinuxProtonResolver.ResolveProtonPath(client, "proton_10", new[] { second, third }).Should().Be(expected);
        }

        [Theory]
        [InlineData("proton_10", "Proton 10.0")]
        [InlineData("proton_103", "Proton 10.3")]
        [InlineData("proton_104", "Proton 10.3")]
        [InlineData("", "Proton - Experimental")]
        [InlineData("proton_hotfix", "Proton - Experimental")]
        [InlineData("GE-Proton10-1", "Proton - Experimental")]
        public void Should_preserve_existing_matching_and_fallback_order(string requested, string selected)
        {
            var client = Root("client");
            var second = Root("second");
            var third = Root("third");
            foreach (var name in new[] { "Proton 10.0", "Proton - Experimental", "Proton Hotfix" }) Proton(second, name);
            foreach (var name in new[] { "Proton 11.0", "Proton 10.3" }) Proton(third, name);
            var actual = LinuxProtonResolver.ResolveProtonPath(client, requested, new[] { second, third });
            Path.GetFileName(Path.GetDirectoryName(actual)).Should().Be(selected);
        }

        [Fact]
        public void Numeric_and_hotfix_fallbacks_should_remain_ordered()
        {
            var client = Root("client");
            var second = Root("second");
            var hotfix = Proton(second, "Proton Hotfix");
            LinuxProtonResolver.ResolveProtonPath(client, "", new[] { second }).Should().Be(hotfix);
            var numeric = Proton(second, "Proton 11.0");
            LinuxProtonResolver.ResolveProtonPath(client, "", new[] { second }).Should().Be(numeric);
        }

        [Fact]
        public void Should_skip_invalid_duplicate_and_non_directory_additional_roots()
        {
            var client = Root("client");
            var second = Root("second");
            var third = Root("third");
            var file = FileAt(directory, "not-a-directory");
            var expected = Proton(third, "Proton 10.0");
            LinuxProtonResolver.ResolveProtonPath(client, "", new[] { null, "", "\0", file, Root("missing-parent") + "/missing", second, second + "/.", third }).Should().Be(expected);
        }

        [Fact]
        public void Missing_scripts_and_custom_tools_should_not_be_accepted()
        {
            var client = Root("client");
            var second = Root("second");
            Directory.CreateDirectory(Path.Combine(second, "steamapps/common/Proton 10.0"));
            FileAt(client, "compatibilitytools.d/GE-Proton10-1/proton");
            LinuxProtonResolver.ResolveProtonPath(client, "", new[] { second }).Should().BeNull();
            LinuxProtonResolver.ResolveProtonPath(Path.Combine(directory, "absent"), "").Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Empty_client_root_should_remain_an_argument_failure(string root)
        {
            Action resolve = () => LinuxProtonResolver.ResolveProtonPath(root, "", Array.Empty<string>());
            resolve.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Registered_library_snapshot_should_reach_selected_game_and_resolver()
        {
            DISetup.SetupContainer();
            var client = Root("client");
            var second = Root("second");
            var third = Root("third");
            string VdfPath(string value) => value.Replace("\\", "\\\\");
            var vdf = FileAt(client, "config/libraryfolders.vdf", $"\"libraryfolders\" {{ \"1\" {{ \"path\" \"{VdfPath(second)}\" }} \"2\" {{ \"path\" \"{VdfPath(second + "/.")}\" }} \"3\" {{ \"path\" \"{VdfPath(third)}\" }} \"4\" {{ \"path\" \"{VdfPath(Path.Combine(directory, "missing"))}\" }} }}");
            FileAt(second, "steamapps/appmanifest_394360.acf", "\"AppState\" { \"appid\" \"394360\" \"installdir\" \"HOI4\" }");
            Directory.CreateDirectory(Path.Combine(second, "steamapps/common/HOI4"));
            var expected = Proton(third, "Proton 10.0");
            rootField.SetValue(null, client);
            mappings.Remove(394360);

            var registration = new GameRegistration();
            var getHoi4 = typeof(GameRegistration).GetMethod("GetHOI4", BindingFlags.NonPublic | BindingFlags.Instance);
            var transferred = (IGameType)getHoi4!.Invoke(registration, new object[] { directory });
            transferred.SteamLibraryRoots.Should().Equal(client, second, third);
            var database = new Database();
            var storage = new IronyModManager.Storage.Storage(database, DIResolver.Get<IMapper>());
            storage.RegisterGame(transferred).Should().BeTrue();
            database.Games.Single().SteamLibraryRoots.Should().Equal(client, second, third);
            storage.GetGames().Single().SteamLibraryRoots.Should().Equal(client, second, third);
            var preferences = new Mock<IPreferencesService>();
            preferences.Setup(p => p.Get()).Returns(new Preferences { Game = transferred.Name });
            var service = new GameService(null, null, Mock.Of<IReader>(), storage, preferences.Object, DIResolver.Get<IMapper>());
            var game = service.GetSelected();
            game.SteamRoot.Should().Be(client);
            game.SteamLibraryRoots.Should().Equal(client, second, third);

            // Runtime uses the registered snapshot even when the metadata file is no longer available.
            File.Delete(vdf);
            LinuxProtonResolver.ResolveProtonPath(game.SteamRoot, game.LinuxProtonVersion, game.SteamLibraryRoots).Should().Be(expected);
        }

        [Fact]
        public void Global_mapping_should_not_become_a_per_game_preference()
        {
            var client = Root("client");
            FileAt(client, "config/config.vdf", "\"InstallConfigStore\" { \"Software\" { \"Valve\" { \"Steam\" { \"CompatToolMapping\" { \"0\" { \"name\" \"proton_experimental\" } } } } } }");
            rootField.SetValue(null, client);
            mappings.Remove(394360);
            SteamDirectory.GetProtonVersion(394360).Should().BeEmpty();
            var second = Root("second");
            var expected = Proton(second, "Proton - Experimental");
            Proton(second, "Proton 11.0");
            LinuxProtonResolver.ResolveProtonPath(client, SteamDirectory.GetProtonVersion(394360), new[] { second }).Should().Be(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Proton_start_info_should_keep_client_game_and_tool_libraries_separate(bool primary)
        {
            var client = Root("client");
            var gameLibrary = Root("game-library");
            var toolLibrary = Root("tool-library");
            var game = FileAt(gameLibrary, "steamapps/common/HOI4/hoi4.exe");
            var expected = Proton(primary ? client : toolLibrary, "Proton 10.0");
            var builder = typeof(ProcessRunner).GetMethod("CreateProtonStartInfo", BindingFlags.NonPublic | BindingFlags.Static);
            var psi = (ProcessStartInfo)builder!.Invoke(null, new object[] { game, client, "proton_10", 394360L, new[] { gameLibrary, toolLibrary }, "-debug" });
            psi.FileName.Should().Be(expected);
            psi.Environment["STEAM_COMPAT_CLIENT_INSTALL_PATH"].Should().Be(client);
            psi.Environment["STEAM_COMPAT_DATA_PATH"].Should().Be(Path.Combine(gameLibrary, "steamapps", "compatdata", "394360"));
            psi.WorkingDirectory.Should().Be(Path.GetDirectoryName(game));
            psi.UseShellExecute.Should().BeFalse();
            psi.ArgumentList.Should().Equal("run", game, "-debug");
        }

        [Fact]
        public void Missing_metadata_and_non_Steam_registration_should_keep_empty_roots()
        {
            DISetup.SetupContainer();
            SteamDirectory.GetSteamLibraryRoots(null).Should().BeEmpty();
            SteamDirectory.GetSteamLibraryRoots(Path.Combine(directory, "absent")).Should().BeEmpty();
            var client = Root("client");
            SteamDirectory.GetSteamLibraryRoots(client).Should().Equal(client);
            var database = new Database();
            var storage = new IronyModManager.Storage.Storage(database, DIResolver.Get<IMapper>());
            storage.RegisterGame(new GameType
            {
                Name = "non-Steam", SteamLibraryRoots = null,
                SupportedOperatingSystems = SupportedOperatingSystems.Windows | SupportedOperatingSystems.Linux | SupportedOperatingSystems.OSX
            }).Should().BeTrue();
            storage.GetGames().Single().SteamLibraryRoots.Should().BeEmpty();
            var preferences = new Mock<IPreferencesService>();
            preferences.Setup(p => p.Get()).Returns(new Preferences { Game = "non-Steam" });
            var service = new GameService(null, null, Mock.Of<IReader>(), storage, preferences.Object, DIResolver.Get<IMapper>());
            service.GetSelected().SteamLibraryRoots.Should().BeEmpty();
        }

        [Fact]
        public void Unresolved_Proton_and_missing_direct_executable_should_throw_before_start()
        {
            var client = Root("client");
            var game = FileAt(client, "steamapps/common/HOI4/hoi4.exe");
            Action launchProton = () => ProcessRunner.RunProtonProcess(game, client, "", 394360, Array.Empty<string>(), "");
            launchProton.Should().Throw<InvalidOperationException>().WithMessage("No Proton executable*");
            Action launchDirect = () => ProcessRunner.RunExternalProcess(Path.Combine(directory, "missing.exe"));
            launchDirect.Should().Throw<FileNotFoundException>();
        }

        [Fact]
        public void Candidate_glob_should_preserve_host_filesystem_casing()
        {
            var client = Root("client");
            var second = Root("second");
            var lower = Proton(second, "proton 10.0");
            var actual = LinuxProtonResolver.ResolveProtonPath(client, "", new[] { second });
            if (OperatingSystem.IsLinux()) actual.Should().BeNull();
            else if (OperatingSystem.IsWindows()) actual.Should().Be(lower);
        }

        public void Dispose()
        {
            rootField.SetValue(null, originalRoot);
            mappings.Clear();
            foreach (var pair in originalMappings) mappings.Add(pair.Key, pair.Value);
            cachedPaths.Clear();
            foreach (var pair in originalPaths) cachedPaths.Add(pair.Key, pair.Value);
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }
}
