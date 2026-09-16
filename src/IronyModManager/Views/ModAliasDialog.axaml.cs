using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IronyModManager.Views
{
    /// <summary>
    /// Collects an Irony-local presentation alias without editing mod metadata.
    /// </summary>
    public partial class ModAliasDialog : Window
    {
        /// <summary>
        /// Initializes the dialog for Avalonia XAML construction.
        /// </summary>
        public ModAliasDialog()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public ModAliasDialog(string title, string originalNameLabel, string aliasLabel, string saveLabel, string cancelLabel,
            string canonicalName, string alias)
        {
            Title = title;
            OriginalNameLabel = originalNameLabel;
            AliasLabel = aliasLabel;
            SaveLabel = saveLabel;
            CancelLabel = cancelLabel;
            CanonicalName = canonicalName;
            Alias = alias;
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
        }

        public string Alias { get; set; }

        public string AliasLabel { get; private set; }

        public string CancelLabel { get; private set; }

        public string CanonicalName { get; private set; }

        public string OriginalNameLabel { get; private set; }

        public string SaveLabel { get; private set; }

        private void Cancel(object sender, Avalonia.Interactivity.RoutedEventArgs e) => Close(false);

        private void Save(object sender, Avalonia.Interactivity.RoutedEventArgs e) => Close(true);
    }
}
