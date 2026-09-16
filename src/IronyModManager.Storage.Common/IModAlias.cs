using IronyModManager.Shared;

namespace IronyModManager.Storage.Common
{
    /// <summary>
    /// An Irony-local mod display-name preference.
    /// </summary>
    public interface IModAlias : IPropertyChangedModel
    {
        string DescriptorFile { get; set; }

        string Game { get; set; }

        string NameOverride { get; set; }
    }
}
