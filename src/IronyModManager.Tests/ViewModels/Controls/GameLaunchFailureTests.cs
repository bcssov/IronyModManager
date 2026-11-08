using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AwesomeAssertions;
using IronyModManager.Common;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Localization;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using Moq;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    public class GameLaunchFailureTests
    {
        [Fact]
        public async Task Existing_native_executable_should_still_start()
        {
            var path = OperatingSystem.IsWindows() ? Path.Combine(Environment.SystemDirectory, "cmd.exe") : "/bin/true";
            var logger = new Mock<ILogger>();
            var action = new AppAction(logger.Object);
            (await action.RunAsync(path, OperatingSystem.IsWindows() ? "/c exit 0" : "")).Should().BeTrue();
            logger.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Missing_game_should_return_false_log_and_report_the_execution_failure()
        {
            var directory = Path.Combine(Path.GetTempPath(), "irony-launch-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                var logger = new Mock<ILogger>();
                var action = new AppAction(logger.Object);
                var missing = 0;
                (await action.RunGameAsync(false, Path.Combine(directory, "missing.exe"), directory, "", 394360, null, "", () => missing++)).Should().BeFalse();
                (await action.RunAsync(Path.Combine(directory, "missing-native"))).Should().BeFalse();
                missing.Should().Be(1);
                logger.Verify(p => p.Error(It.IsAny<FileNotFoundException>()), Times.Exactly(2));
            }
            finally { Directory.Delete(directory, true); }
        }

        [Fact]
        public async Task Missing_executable_should_show_one_localized_warning_for_ten_seconds()
        {
            var localization = new Mock<ILocalizationManager>();
            var notifications = new Mock<INotificationAction>();
            localization.Setup(p => p.GetResource(LocalizationResources.Mod_Actions.LaunchGame.ExecutableNotFound.Title)).Returns("localized title");
            localization.Setup(p => p.GetResource(LocalizationResources.Mod_Actions.LaunchGame.ExecutableNotFound.Message)).Returns("localized message");
            var interaction = new ModControlInteraction(Mock.Of<IIDGenerator>(), localization.Object, notifications.Object, new AppAction(Mock.Of<ILogger>()));
            var missing = false;
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "missing.exe");
            (await interaction.RunGameAsync(false, path, "client", "", 394360, null, "", () => missing = true)).Should().BeFalse();
            missing.Should().BeTrue();

            notifications.Verify(p => p.ShowNotification("localized title", "localized message", NotificationType.Warning, 10, null), Times.Once);
            notifications.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Launch_without_a_reported_missing_game_should_not_show_a_missing_warning(bool launched)
        {
            var notifications = new Mock<INotificationAction>();
            var action = new Mock<IAppAction>();
            action.Setup(p => p.RunGameAsync(false, "game", "client", "", 394360, null, "", It.IsAny<Action>())).ReturnsAsync(launched);
            var interaction = new ModControlInteraction(Mock.Of<IIDGenerator>(), Mock.Of<ILocalizationManager>(), notifications.Object, action.Object);
            var missing = false;
            (await interaction.RunGameAsync(false, "game", "client", "", 394360, null, "", () => missing = true)).Should().Be(launched);
            missing.Should().BeFalse();
            notifications.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Launch_forwarding_should_preserve_URI_route_and_defer_library_enumeration()
        {
            var action = new Mock<IAppAction>();
            action.Setup(p => p.OpenAsync("steam://run/394360")).ReturnsAsync(true);
            IEnumerable<string> Libraries()
            {
                throw new InvalidOperationException("Forwarding must not enumerate libraries.");
#pragma warning disable CS0162
                yield break;
#pragma warning restore CS0162
            }
            var roots = Libraries();
            action.Setup(p => p.RunGameAsync(false, "hoi4", "client", "", 394360, roots, "", It.IsAny<Action>())).ReturnsAsync(true);
            var interaction = new ModControlInteraction(Mock.Of<IIDGenerator>(), Mock.Of<ILocalizationManager>(), Mock.Of<INotificationAction>(), action.Object);
            (await interaction.OpenAsync("steam://run/394360")).Should().BeTrue();
            (await interaction.RunGameAsync(false, "hoi4", "client", "", 394360, roots, "", null)).Should().BeTrue();
            action.Verify(p => p.OpenAsync("steam://run/394360"), Times.Once);
            action.Verify(p => p.RunGameAsync(false, "hoi4", "client", "", 394360, roots, "", It.IsAny<Action>()), Times.Once);
        }
    }
}
