// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************

using System.Linq;
using AwesomeAssertions;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.ViewModels.Controls;
using Moq;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    /// <summary>
    /// Tests collection membership independently from runtime enablement.
    /// </summary>
    public class CollectionModMembershipTests
    {
        [Fact]
        public void Collection_projection_should_remain_persisted_independently_from_runtime_selection()
        {
            var enabled = new Mod { DescriptorFile = "enabled", IsSelected = true };
            var disabled = new Mod { DescriptorFile = "disabled", IsSelected = false };
            var virtualMod = new Mod { DescriptorFile = "missing", IsSelected = false, IsVirtual = true };

            new CollectionModMembership(Mock.Of<IModService>()).GetPersistedMembers([enabled, disabled, virtualMod])
                .Should().Equal(enabled, disabled, virtualMod);
        }

        [Fact]
        public void Persisted_membership_should_not_be_inferred_from_runtime_selection()
        {
            var recoveredMember = new Mod { DescriptorFile = "recovered", IsSelected = false };

            new CollectionModMembership(Mock.Of<IModService>()).GetPersistedMembers([recoveredMember])
                .Should().Equal(recoveredMember);
        }

        [Fact]
        public void Authoritative_resolution_should_restore_only_persisted_collection_selection()
        {
            var collectionMember = new Mod { DescriptorFile = "member", IsSelected = false };
            var installedNonMember = new Mod { DescriptorFile = "non-member", IsSelected = true };
            var virtualMember = new Mod { DescriptorFile = "missing", IsSelected = false, IsVirtual = true };
            var membership = new CollectionModMembership(Mock.Of<IModService>());

            membership.RestoreSelection([collectionMember, installedNonMember], [collectionMember, virtualMember]);

            collectionMember.IsSelected.Should().BeTrue();
            installedNonMember.IsSelected.Should().BeFalse();
            virtualMember.IsSelected.Should().BeTrue();
            membership.GetPersistedMembers([collectionMember, virtualMember])
                .Should().Equal(collectionMember, virtualMember);
        }

        [Fact]
        public void Healthy_locked_recovery_shutdown_restart_should_preserve_collection_member()
        {
            var membership = new CollectionModMembership(Mock.Of<IModService>());
            var original = new Mod { DescriptorFile = "member", IsSelected = true };
            var persistedDescriptors = membership.GetPersistedMembers([original]).Select(p => p.DescriptorFile).ToList();

            var game = new Game { Type = "game" };
            var safety = new Mock<IGameStateSafetyService>();
            safety.Setup(p => p.IsLocked(game)).Returns(true);
            var reconciliation = new ModStateReconciliationCoordinator(safety.Object);
            reconciliation.ReconcileModsPublication(game, false, true, persistedDescriptors.Clear).Should().BeFalse();
            reconciliation.ReconcileRefreshPublication(game, false, true, persistedDescriptors.Clear).Should().BeFalse();

            // The locked cold start cannot reach the destructive collection-save callback.
            persistedDescriptors.Should().Equal("member");

            var recovered = new Mod { DescriptorFile = persistedDescriptors[0], IsSelected = false };
            membership.RestoreSelection([recovered], [recovered]);
            persistedDescriptors = membership.GetPersistedMembers([recovered]).Select(p => p.DescriptorFile).ToList();

            var nextStart = new Mod { DescriptorFile = persistedDescriptors[0], IsSelected = false };
            membership.RestoreSelection([nextStart], [nextStart]);

            persistedDescriptors.Should().Equal("member");
            recovered.IsSelected.Should().BeTrue();
            nextStart.IsSelected.Should().BeTrue();
            membership.GetPersistedMembers([nextStart]).Should().Equal(nextStart);
        }

        [Fact]
        public void Explicit_remove_should_remove_only_the_requested_virtual_member()
        {
            var real = new Mod { DescriptorFile = "real", IsSelected = true };
            var virtualMod = new Mod { DescriptorFile = "missing", IsVirtual = true };
            var reconstructedVirtualMod = new Mod { DescriptorFile = "missing", IsVirtual = true };
            var modService = new Mock<IModService>();
            modService.Setup(p => p.AreModDefinitionsEquivalent(It.IsAny<IMod>(), It.IsAny<IMod>()))
                .Returns((IMod candidate, IMod requested) => candidate.DescriptorFile == requested.DescriptorFile);

            var membership = new CollectionModMembership(modService.Object);

            membership.RemoveVirtual([real, virtualMod], virtualMod).Should().Equal(real);
            membership.RemoveVirtual([real, virtualMod], real).Should().Equal(real, virtualMod);
            membership.RemoveVirtual([real, virtualMod], reconstructedVirtualMod).Should().Equal(real);
        }

        [Fact]
        public void Disable_all_should_retain_virtual_membership()
        {
            var real = new Mod { DescriptorFile = "real", IsSelected = true };
            var virtualMod = new Mod { DescriptorFile = "missing", IsSelected = true, IsVirtual = true };

            new CollectionModMembership(Mock.Of<IModService>()).DisableAllRealMods([real, virtualMod]).Should().Equal(virtualMod);
            real.IsSelected.Should().BeFalse();
            virtualMod.IsSelected.Should().BeTrue();
        }
    }
}
