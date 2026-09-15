// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************

using AwesomeAssertions;
using IronyModManager.ViewModels.Controls;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    /// <summary>
    /// Tests missing-mod warning ownership and de-duplication.
    /// </summary>
    public class MissingModNotificationPolicyTests
    {
        [Fact]
        public void Import_warning_should_suppress_the_ordinary_warning_for_the_same_missing_entries()
        {
            var policy = new MissingModNotificationPolicy();
            policy.BeginImport("Imported");

            policy.ShouldShowOrdinaryWarning("game", "game", "Imported", ["missing.mod"]).Should().BeFalse();
            policy.ShouldShowOrdinaryWarning("game", "game", "Imported", ["missing.mod"]).Should().BeFalse();
        }

        [Fact]
        public void Existing_collection_warning_should_show_once_until_the_missing_state_clears()
        {
            var policy = new MissingModNotificationPolicy();

            policy.ShouldShowOrdinaryWarning("game", "game", "Existing", ["first.mod"]).Should().BeTrue();
            policy.ShouldShowOrdinaryWarning("game", "game", "Existing", ["first.mod"]).Should().BeFalse();
            policy.ShouldShowOrdinaryWarning("game", "game", "Existing", []).Should().BeFalse();
            policy.ShouldShowOrdinaryWarning("game", "game", "Existing", ["first.mod"]).Should().BeTrue();
        }

        [Fact]
        public void Changed_missing_member_set_should_allow_a_later_warning()
        {
            var policy = new MissingModNotificationPolicy();

            policy.ShouldShowOrdinaryWarning("game", "game", "Existing", ["first.mod"]).Should().BeTrue();
            policy.ShouldShowOrdinaryWarning("game", "game", "Existing", ["first.mod", "second.mod"]).Should().BeTrue();
            policy.ShouldShowOrdinaryWarning("game", "game", "Existing", ["second.mod", "first.mod"]).Should().BeFalse();
        }
    }
}
