// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************

using System.Linq;
using AwesomeAssertions;
using IronyModManager.DI;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Tests.Common;
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
            DISetup.SetupContainer();
            var collectionMember = CreateMod("member", "Same name");
            var installedCollectionMember = CreateMod("member", "Same name");
            var installedNonMember = CreateMod("non-member", isSelected: true);
            var superficiallySimilarNonMember = CreateMod("different", "Same name", true);
            var virtualMember = CreateMod("missing", isVirtual: true);
            var modService = new Mock<IModService>();
            modService.Setup(p => p.AreModIdentitiesEquivalent(It.IsAny<IMod>(), It.IsAny<IMod>()))
                .Returns((IMod candidate, IMod requested) => candidate.DescriptorFile == requested.DescriptorFile);
            var membership = new CollectionModMembership(modService.Object);

            membership.RestoreSelection([installedCollectionMember, installedNonMember, superficiallySimilarNonMember],
                [collectionMember, virtualMember]);

            collectionMember.IsSelected.Should().BeTrue();
            installedCollectionMember.IsSelected.Should().BeTrue();
            installedNonMember.IsSelected.Should().BeFalse();
            superficiallySimilarNonMember.IsSelected.Should().BeFalse();
            virtualMember.IsSelected.Should().BeTrue();
            collectionMember.GetType().Should().NotBe(typeof(Mod));
            installedCollectionMember.GetType().Should().NotBe(typeof(Mod));
            collectionMember.Should().NotBeSameAs(installedCollectionMember);
            membership.GetPersistedMembers([collectionMember, virtualMember])
                .Should().Equal(collectionMember, virtualMember);
            modService.Verify(p => p.AreModIdentitiesEquivalent(installedCollectionMember, collectionMember), Times.Once);
        }

        [Fact]
        public void Ordered_projection_comparison_should_use_domain_equivalence()
        {
            DISetup.SetupContainer();
            var first = CreateMod("first", "Same");
            var second = CreateMod("second", "Second");
            var equivalentFirst = CreateMod("first", "Same");
            var equivalentSecond = CreateMod("second", "Second");
            var similarButDifferent = CreateMod("different", "Same");
            var modService = new Mock<IModService>();
            modService.Setup(p => p.AreModIdentitiesEquivalent(It.IsAny<IMod>(), It.IsAny<IMod>()))
                .Returns((IMod candidate, IMod requested) => candidate.DescriptorFile == requested.DescriptorFile);
            var membership = new CollectionModMembership(modService.Object);

            membership.AreEquivalentInOrder([first, second], [equivalentFirst, equivalentSecond]).Should().BeTrue();
            membership.AreEquivalentInOrder([first, second], [similarButDifferent, equivalentSecond]).Should().BeFalse();
            membership.AreEquivalentInOrder([first, second], [equivalentSecond, equivalentFirst]).Should().BeFalse();
        }

        private static IMod CreateMod(string descriptor, string name = null, bool isSelected = false, bool isVirtual = false)
        {
            var mod = DIResolver.Get<IMod>();
            mod.DescriptorFile = descriptor;
            mod.Name = name ?? descriptor;
            mod.IsSelected = isSelected;
            mod.IsVirtual = isVirtual;
            return mod;
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
            modService.Setup(p => p.AreModIdentitiesEquivalent(It.IsAny<IMod>(), It.IsAny<IMod>()))
                .Returns((IMod candidate, IMod requested) => candidate.DescriptorFile == requested.DescriptorFile);

            var membership = new CollectionModMembership(modService.Object);

            membership.RemoveVirtual([real, virtualMod], virtualMod).Should().Equal(real);
            membership.RemoveVirtual([real, virtualMod], real).Should().Equal(real, virtualMod);
            membership.RemoveVirtual([real, virtualMod], reconstructedVirtualMod).Should().Equal(real);
            modService.Verify(p => p.AreModIdentitiesEquivalent(It.IsAny<IMod>(), reconstructedVirtualMod), Times.Exactly(2));
            real.IsSelected.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Explicit_remove_should_preserve_remaining_order_for_every_position(int removedIndex)
        {
            var mods = new[]
            {
                new Mod { DescriptorFile = "first", IsVirtual = true },
                new Mod { DescriptorFile = "middle", IsVirtual = true },
                new Mod { DescriptorFile = "last", IsVirtual = true }
            };
            var contextMod = new Mod { DescriptorFile = mods[removedIndex].DescriptorFile, IsVirtual = true };
            var modService = new Mock<IModService>();
            modService.Setup(p => p.AreModIdentitiesEquivalent(It.IsAny<IMod>(), It.IsAny<IMod>()))
                .Returns((IMod candidate, IMod requested) => candidate.DescriptorFile == requested.DescriptorFile);

            var result = new CollectionModMembership(modService.Object).RemoveVirtual(mods, contextMod);

            result.Select(p => p.DescriptorFile).Should().Equal(
                mods.Where((_, index) => index != removedIndex).Select(p => p.DescriptorFile));
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
