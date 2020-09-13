// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-25-2020
//
// Last Modified By : Mario
// Last Modified On : 09-13-2020
// ***********************************************************************
// <copyright file="MergeViewerBinaryControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using IronyModManager.Common.ViewModels;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;
using SmartFormat;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class MergeViewerBinaryControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class MergeViewerBinaryControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The block selected
        /// </summary>
        private const string BlockSelected = "BlockSelected";

        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        /// <summary>
        /// The loading images
        /// </summary>
        private bool loadingImages = false;

        /// <summary>
        /// The previous left definition
        /// </summary>
        private IDefinition prevLeftDefinition = null;

        /// <summary>
        /// The previous right definition
        /// </summary>
        private IDefinition prevRightDefinition = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeViewerBinaryControlViewModel" /> class.
        /// </summary>
        /// <param name="modService">The mod service.</param>
        public MergeViewerBinaryControlViewModel(IModService modService, ILocalizationManager localizationManager)
        {
            this.modService = modService;
            this.localizationManager = localizationManager;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the binary file.
        /// </summary>
        /// <value>The binary file.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.BinaryFile)]
        public virtual string BinaryFile { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable selection].
        /// </summary>
        /// <value><c>true</c> if [enable selection]; otherwise, <c>false</c>.</value>
        public virtual bool EnableSelection { get; set; }

        /// <summary>
        /// Gets or sets the left definition.
        /// </summary>
        /// <value>The left definition.</value>
        public virtual IDefinition LeftDefinition { get; protected set; }

        /// <summary>
        /// Gets or sets the left image.
        /// </summary>
        /// <value>The left image.</value>
        public virtual IBitmap LeftImage { get; protected set; }

        public virtual string LeftImageInfo { get; protected set; }

        /// <summary>
        /// Gets or sets the right definition.
        /// </summary>
        /// <value>The right definition.</value>
        public virtual IDefinition RightDefinition { get; protected set; }

        /// <summary>
        /// Gets or sets the right image.
        /// </summary>
        /// <value>The right image.</value>
        public virtual IBitmap RightImage { get; protected set; }

        public virtual string RightImageInfo { get; protected set; }

        /// <summary>
        /// Gets or sets the take left.
        /// </summary>
        /// <value>The take left.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.TakeLeft)]
        public virtual string TakeLeft { get; protected set; }

        /// <summary>
        /// Gets or sets the take left class.
        /// </summary>
        /// <value>The take left class.</value>
        public virtual string TakeLeftClass { get; protected set; }

        /// <summary>
        /// Gets or sets the take left command.
        /// </summary>
        /// <value>The take left command.</value>
        public virtual ReactiveCommand<Unit, Unit> TakeLeftCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the take right.
        /// </summary>
        /// <value>The take right.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.TakeRight)]
        public virtual string TakeRight { get; protected set; }

        /// <summary>
        /// Gets or sets the take right class.
        /// </summary>
        /// <value>The take right class.</value>
        public virtual string TakeRightClass { get; protected set; }

        /// <summary>
        /// Gets or sets the take right command.
        /// </summary>
        /// <value>The take right command.</value>
        public virtual ReactiveCommand<Unit, Unit> TakeRightCommand { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            TakeLeftClass = string.Empty;
            TakeRightClass = string.Empty;
            var left = LeftImage;
            LeftImage = null;
            left?.Dispose();
            var right = RightImage;
            RightImage = null;
            right?.Dispose();
            LeftImageInfo = string.Empty;
            RightImageInfo = string.Empty;
        }

        /// <summary>
        /// Sets the left.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public void SetLeft(IDefinition definition)
        {
            prevLeftDefinition = LeftDefinition;
            LeftDefinition = definition;
            LoadImagesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the right.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public void SetRight(IDefinition definition)
        {
            prevRightDefinition = RightDefinition;
            RightDefinition = definition;
            LoadImagesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// load images as an asynchronous operation.
        /// </summary>
        protected virtual async Task LoadImagesAsync()
        {
            while (loadingImages)
            {
                await Task.Delay(25);
            }
            loadingImages = true;
            if (prevLeftDefinition != LeftDefinition)
            {
                LeftImageInfo = string.Empty;
                var left = LeftImage;
                LeftImage = null;
                left?.Dispose();
                using var ms = await modService.GetImageStreamAsync(LeftDefinition?.ModName, LeftDefinition?.File);
                if (ms != null)
                {
                    LeftImage = new Bitmap(ms);
                    var info = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ImageInfo);
                    LeftImageInfo = Smart.Format(info, new { LeftImage.PixelSize.Width, LeftImage.PixelSize.Height });
                }
            }
            if (prevRightDefinition != RightDefinition)
            {
                RightImageInfo = string.Empty;
                var right = RightImage;
                RightImage = null;
                right?.Dispose();
                using var ms = await modService.GetImageStreamAsync(RightDefinition?.ModName, RightDefinition?.File);
                if (ms != null)
                {
                    RightImage = new Bitmap(ms);
                    var info = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ImageInfo);
                    RightImageInfo = Smart.Format(info, new { RightImage.PixelSize.Width, RightImage.PixelSize.Height });
                }
            }
            loadingImages = false;
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            TakeLeftCommand = ReactiveCommand.Create(() =>
            {
                TakeRightClass = string.Empty;
                TakeLeftClass = BlockSelected;
            }).DisposeWith(disposables);

            TakeRightCommand = ReactiveCommand.Create(() =>
            {
                TakeLeftClass = string.Empty;
                TakeRightClass = BlockSelected;
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
