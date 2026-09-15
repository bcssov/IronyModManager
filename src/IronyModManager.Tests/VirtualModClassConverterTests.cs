// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************
using AwesomeAssertions;
using IronyModManager.Converters;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Tests virtual-mod presentation class selection.
    /// </summary>
    public class VirtualModClassConverterTests
    {
        [Theory]
        [InlineData(true, "VirtualMod")]
        [InlineData(false, "")]
        [InlineData(null, "")]
        public void Only_virtual_mods_should_receive_the_virtual_style_class(bool? isVirtual, string expected)
        {
            var converter = new VirtualModClassConverter();

            converter.Convert(isVirtual, null, null, null).Should().Be(expected);
        }

        [Fact]
        public void Resolved_real_mod_should_remove_the_virtual_style_class()
        {
            var converter = new VirtualModClassConverter();

            converter.Convert(true, null, null, null).Should().Be("VirtualMod");
            converter.Convert(false, null, null, null).Should().Be(string.Empty);
        }
    }
}
