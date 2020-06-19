// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 05-16-2020
// ***********************************************************************
// <copyright file="ManagedDialogViewModel.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>
// Based on Avalonia ManagedFileChooserViewModel. Why of why would
// the Avalonia guys expose some of this stuff?
// </summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Dialogs;
using Avalonia.Threading;
using IronyModManager.Shared;

namespace IronyModManager.Controls.Dialogs
{
    /// <summary>
    /// Class ManagedDialogViewModel.
    /// Implements the <see cref="IronyModManager.Controls.Dialogs.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Controls.Dialogs.BaseViewModel" />
    [ExcludeFromCoverage("External logic.")]
    public class ManagedDialogViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The default extension
        /// </summary>
        private readonly string _defaultExtension;

        /// <summary>
        /// The disposables
        /// </summary>
        private readonly CompositeDisposable _disposables;

        /// <summary>
        /// The options
        /// </summary>
        private readonly ManagedFileDialogOptions _options;

        /// <summary>
        /// The saving file
        /// </summary>
        private readonly bool _savingFile;

        /// <summary>
        /// The already cancelled
        /// </summary>
        private bool _alreadyCancelled = false;

        /// <summary>
        /// The file name
        /// </summary>
        private string _fileName;

        /// <summary>
        /// The location
        /// </summary>
        private string _location;

        /// <summary>
        /// The scheduled selection validation
        /// </summary>
        private bool _scheduledSelectionValidation;

        /// <summary>
        /// The selected filter
        /// </summary>
        private ManagedDialogFilterViewModel _selectedFilter;

        /// <summary>
        /// The show hidden files
        /// </summary>
        private bool _showHiddenFiles;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedDialogViewModel" /> class.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentException">dialog</exception>
        public ManagedDialogViewModel(FileSystemDialog dialog, ManagedFileDialogOptions options)
        {
            _options = options;
            _disposables = new CompositeDisposable();

            var quickSources = AvaloniaLocator.Current
                                              .GetService<ManagedDialogSources>()
                                              ?? new ManagedDialogSources();

            var sub1 = AvaloniaLocator.Current
                                      .GetService<IMountedVolumeInfoProvider>()
                                      .Listen(ManagedDialogSources.MountedVolumes);

            var sub2 = Observable.FromEventPattern(ManagedDialogSources.MountedVolumes,
                                            nameof(ManagedDialogSources.MountedVolumes.CollectionChanged))
                                 .ObserveOn(AvaloniaScheduler.Instance)
                                 .Subscribe(x => RefreshQuickLinks(quickSources));

            _disposables.Add(sub1);
            _disposables.Add(sub2);

            CompleteRequested += delegate { _disposables?.Dispose(); };
            CancelRequested += delegate { _disposables?.Dispose(); };

            RefreshQuickLinks(quickSources);

            Title = dialog.Title ?? (
                        dialog is OpenFileDialog ? "Open file"
                        : dialog is SaveFileDialog ? "Save file"
                        : dialog is OpenFolderDialog ? "Select directory"
                        : throw new ArgumentException(nameof(dialog)));

            var directory = dialog.Directory;

            if (directory == null || !Directory.Exists(directory))
            {
                directory = Directory.GetCurrentDirectory();
            }

            if (dialog is FileDialog fd)
            {
                if (fd.Filters?.Count > 0)
                {
                    Filters.AddRange(fd.Filters.Select(f => new ManagedDialogFilterViewModel(f)));
                    _selectedFilter = Filters[0];
                    ShowFilters = true;
                }

                if (dialog is OpenFileDialog ofd)
                {
                    if (ofd.AllowMultiple)
                    {
                        SelectionMode = SelectionMode.Multiple;
                    }
                }
            }

            SelectingFolder = dialog is OpenFolderDialog;

            if (dialog is SaveFileDialog sfd)
            {
                _savingFile = true;
                _defaultExtension = sfd.DefaultExtension;
                FileName = sfd.InitialFileName;
            }

            Navigate(directory, (dialog as FileDialog)?.InitialFileName);
            SelectedItems.CollectionChanged += OnSelectionChangedAsync;
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when [cancel requested].
        /// </summary>
        public event Action CancelRequested;

        /// <summary>
        /// Occurs when [complete requested].
        /// </summary>
        public event Action<string[]> CompleteRequested;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get => _fileName;
            set => RaiseAndSetIfChanged(ref _fileName, value);
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public AvaloniaList<ManagedDialogFilterViewModel> Filters { get; } =
            new AvaloniaList<ManagedDialogFilterViewModel>();

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public AvaloniaList<ManagedDialogItemViewModel> Items { get; } =
            new AvaloniaList<ManagedDialogItemViewModel>();

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public string Location
        {
            get => _location;
            private set => RaiseAndSetIfChanged(ref _location, value);
        }

        /// <summary>
        /// Gets the quick links.
        /// </summary>
        /// <value>The quick links.</value>
        public AvaloniaList<ManagedDialogItemViewModel> QuickLinks { get; } =
            new AvaloniaList<ManagedDialogItemViewModel>();

        /// <summary>
        /// Gets or sets the index of the quick links selected.
        /// </summary>
        /// <value>The index of the quick links selected.</value>
        public int QuickLinksSelectedIndex
        {
            get
            {
                for (var index = 0; index < QuickLinks.Count; index++)
                {
                    var i = QuickLinks[index];

                    if (i.Path == Location)
                    {
                        return index;
                    }
                }

                return -1;
            }
            set => this.RaisePropertyChanged(nameof(QuickLinksSelectedIndex));
        }

        /// <summary>
        /// Gets or sets the selected filter.
        /// </summary>
        /// <value>The selected filter.</value>
        public ManagedDialogFilterViewModel SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFilter, value);
                Refresh();
            }
        }

        /// <summary>
        /// Gets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        public AvaloniaList<ManagedDialogItemViewModel> SelectedItems { get; } =
            new AvaloniaList<ManagedDialogItemViewModel>();

        /// <summary>
        /// Gets a value indicating whether [selecting folder].
        /// </summary>
        /// <value><c>true</c> if [selecting folder]; otherwise, <c>false</c>.</value>
        public bool SelectingFolder { get; }

        /// <summary>
        /// Gets the selection mode.
        /// </summary>
        /// <value>The selection mode.</value>
        public SelectionMode SelectionMode { get; }

        /// <summary>
        /// Gets a value indicating whether [show filters].
        /// </summary>
        /// <value><c>true</c> if [show filters]; otherwise, <c>false</c>.</value>
        public bool ShowFilters { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [show hidden files].
        /// </summary>
        /// <value><c>true</c> if [show hidden files]; otherwise, <c>false</c>.</value>
        public bool ShowHiddenFiles
        {
            get => _showHiddenFiles;
            set
            {
                this.RaiseAndSetIfChanged(ref _showHiddenFiles, value);
                Refresh();
            }
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            if (!_alreadyCancelled)
            {
                // INFO: Don't misplace this check or it might cause
                //       StackOverflowException because of recursive
                //       event invokes.
                _alreadyCancelled = true;
                CancelRequested?.Invoke();
            }
        }

        /// <summary>
        /// Enters the pressed.
        /// </summary>
        public void EnterPressed()
        {
            if (Directory.Exists(Location))
            {
                Navigate(Location);
            }
            else if (File.Exists(Location))
            {
                CompleteRequested?.Invoke(new[] { Location });
            }
        }

        /// <summary>
        /// Goes up.
        /// </summary>
        public void GoUp()
        {
            var parent = Path.GetDirectoryName(Location);

            if (string.IsNullOrWhiteSpace(parent))
            {
                return;
            }

            Navigate(parent);
        }

        /// <summary>
        /// Navigates the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="initialSelectionName">Initial name of the selection.</param>
        public void Navigate(string path, string initialSelectionName = null)
        {
            if (!Directory.Exists(path))
            {
                NavigateRoot(initialSelectionName);
            }
            else
            {
                Location = path;
                Items.Clear();
                SelectedItems.Clear();

                try
                {
                    var infos = new DirectoryInfo(path).EnumerateFileSystemInfos();

                    if (!ShowHiddenFiles)
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            infos = infos.Where(i => (i.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0);
                        }
                        else
                        {
                            infos = infos.Where(i => !i.Name.StartsWith("."));
                        }
                    }

                    if (SelectedFilter != null)
                    {
                        infos = infos.Where(i => i is DirectoryInfo || SelectedFilter.Match(i.Name));
                    }

                    Items.AddRange(infos.Where(x =>
                    {
                        if (SelectingFolder)
                        {
                            if (!(x is DirectoryInfo))
                            {
                                return false;
                            }
                        }

                        return true;
                    })
                    .Where(x => x.Exists)
                    .Select(info => new ManagedDialogItemViewModel
                    {
                        DisplayName = info.Name,
                        Path = info.FullName,
                        Type = info is FileInfo ? info.Extension : "File Folder",
                        ItemType = info is FileInfo ? ManagedFileChooserItemType.File
                                                     : ManagedFileChooserItemType.Folder,
                        Size = info is FileInfo f ? f.Length : 0,
                        Modified = info.LastWriteTime
                    })
                    .OrderByDescending(x => x.ItemType == ManagedFileChooserItemType.Folder)
                    .ThenBy(x => x.DisplayName, StringComparer.InvariantCultureIgnoreCase));

                    if (initialSelectionName != null)
                    {
                        var sel = Items.FirstOrDefault(i => i.ItemType == ManagedFileChooserItemType.File && i.DisplayName == initialSelectionName);

                        if (sel != null)
                        {
                            SelectedItems.Add(sel);
                        }
                    }

                    RaisePropertyChanged(nameof(QuickLinksSelectedIndex));
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        /// <summary>
        /// Oks this instance.
        /// </summary>
        public void Ok()
        {
            if (SelectingFolder)
            {
                CompleteRequested?.Invoke(new[] { Location });
            }
            else if (_savingFile)
            {
                if (!string.IsNullOrWhiteSpace(FileName))
                {
                    var ext = !string.IsNullOrWhiteSpace(_defaultExtension) ?
                        (!_defaultExtension.StartsWith(".") ? $".{_defaultExtension}" : _defaultExtension) :
                        (!(SelectedFilter?.Extensions.FirstOrDefault()).StartsWith(".") ? $".{SelectedFilter?.Extensions.FirstOrDefault()}" : SelectedFilter?.Extensions.FirstOrDefault());
                    if (!string.IsNullOrWhiteSpace(ext) && !FileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                    {
                        FileName = FileName + ext;
                    }
                    CompleteRequested?.Invoke(new[] { Path.Combine(Location, FileName) });
                }
            }
            else
            {
                CompleteRequested?.Invoke(SelectedItems.Select(i => i.Path).ToArray());
            }
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh() => Navigate(Location);

        /// <summary>
        /// Selects the single file.
        /// </summary>
        /// <param name="item">The item.</param>
        public void SelectSingleFile(ManagedDialogItemViewModel item)
        {
            CompleteRequested?.Invoke(new[] { item.Path });
        }

        /// <summary>
        /// Navigates the root.
        /// </summary>
        /// <param name="initialSelectionName">Initial name of the selection.</param>
        private void NavigateRoot(string initialSelectionName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Navigate(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), initialSelectionName);
            }
            else
            {
                Navigate("/", initialSelectionName);
            }
        }

        /// <summary>
        /// on selection changed as an asynchronous operation.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private async void OnSelectionChangedAsync(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_scheduledSelectionValidation)
            {
                return;
            }

            _scheduledSelectionValidation = true;
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    if (SelectingFolder)
                    {
                        SelectedItems.Clear();
                    }
                    else
                    {
                        if (!_options.AllowDirectorySelection)
                        {
                            var invalidItems = SelectedItems.Where(i => i.ItemType == ManagedFileChooserItemType.Folder)
                                .ToList();
                            foreach (var item in invalidItems)
                                SelectedItems.Remove(item);
                        }

                        if (!SelectingFolder)
                        {
                            var selectedItem = SelectedItems.FirstOrDefault();

                            if (selectedItem != null)
                            {
                                FileName = selectedItem.DisplayName;
                            }
                        }
                    }
                }
                finally
                {
                    _scheduledSelectionValidation = false;
                }
            });
        }

        /// <summary>
        /// Refreshes the quick links.
        /// </summary>
        /// <param name="quickSources">The quick sources.</param>
        private void RefreshQuickLinks(ManagedDialogSources quickSources)
        {
            QuickLinks.Clear();
            QuickLinks.AddRange(quickSources.GetAllItems().Select(i => new ManagedDialogItemViewModel(i)));
        }

        #endregion Methods
    }
}
