using IronyModManager.Shared;
using IronyModManager.Storage.Common;

namespace IronyModManager.Storage
{
    /// <inheritdoc />
    public class ModAlias : PropertyChangedModelBase, IModAlias
    {
        public virtual string DescriptorFile { get; set; }

        public virtual string Game { get; set; }

        public virtual string NameOverride { get; set; }
    }
}
