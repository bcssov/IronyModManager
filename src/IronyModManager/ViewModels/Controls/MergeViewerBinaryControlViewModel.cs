// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-25-2020
//
// Last Modified By : Mario
// Last Modified On : 09-06-2021
// ***********************************************************************
// <copyright file="MergeViewerBinaryControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using IronyModManager.Common.ViewModels;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using Nito.AsyncEx;
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

        /// <summary>
        /// The left image lock
        /// </summary>
        private readonly AsyncLock leftImageLock = new();

        /// <summary>
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        /// <summary>
        /// The right image lock
        /// </summary>
        private readonly AsyncLock rightImageLock = new();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeViewerBinaryControlViewModel" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="modService">The mod service.</param>
        /// <param name="localizationManager">The localization manager.</param>
        public MergeViewerBinaryControlViewModel(ILogger logger, IModService modService, ILocalizationManager localizationManager)
        {
            this.modService = modService;
            this.localizationManager = localizationManager;
            this.logger = logger;
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
        /// Gets or sets the height of the left.
        /// </summary>
        /// <value>The height of the left.</value>
        public virtual double LeftHeight { get; protected set; }

        /// <summary>
        /// Gets or sets the left image.
        /// </summary>
        /// <value>The left image.</value>
        public virtual IBitmap LeftImage { get; protected set; }

        /// <summary>
        /// Gets or sets the left image information.
        /// </summary>
        /// <value>The left image information.</value>
        public virtual string LeftImageInfo { get; protected set; }

        /// <summary>
        /// Gets or sets the width of the left.
        /// </summary>
        /// <value>The width of the left.</value>
        public virtual double LeftWidth { get; protected set; }

        /// <summary>
        /// Gets or sets the height of the right.
        /// </summary>
        /// <value>The height of the right.</value>
        public virtual double RightHeight { get; protected set; }

        /// <summary>
        /// Gets or sets the right image.
        /// </summary>
        /// <value>The right image.</value>
        public virtual IBitmap RightImage { get; protected set; }

        /// <summary>
        /// Gets or sets the right image information.
        /// </summary>
        /// <value>The right image information.</value>
        public virtual string RightImageInfo { get; protected set; }

        /// <summary>
        /// Gets or sets the width of the right.
        /// </summary>
        /// <value>The width of the right.</value>
        public virtual double RightWidth { get; protected set; }

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
        /// <param name="fullReset">if set to <c>true</c> [full reset].</param>
        public void Reset(bool fullReset = true)
        {
            async Task resetLeft(bool fullReset)
            {
                using var mutex = await leftImageLock.LockAsync();
                TakeLeftClass = string.Empty;
                if (fullReset)
                {
                    var left = LeftImage;
                    LeftImage = null;
                    left?.Dispose();
                    LeftImageInfo = string.Empty;
                    LeftHeight = LeftWidth = 0;
                }
                await Task.Delay(10);
                mutex.Dispose();
            }
            async Task resetRight(bool fullReset)
            {
                using var mutex = await rightImageLock.LockAsync();
                TakeRightClass = string.Empty;
                if (fullReset)
                {
                    var right = RightImage;
                    RightImage = null;
                    right?.Dispose();
                    RightImageInfo = string.Empty;
                    RightHeight = RightWidth = 0;
                }
                await Task.Delay(10);
                mutex.Dispose();
            }

            resetLeft(fullReset).ConfigureAwait(false);
            resetRight(fullReset).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the left.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public void SetLeft(IDefinition definition)
        {
            var localDefinition = definition;
            async Task parseImage()
            {
                using var mutex = await leftImageLock.LockAsync();
                var left = LeftImage;
                if (definition != null)
                {
                    LeftImageInfo = string.Empty;
                    LeftImage = null;
                    left?.Dispose();
                    LeftHeight = LeftWidth = 0;
                    using var ms = await modService.GetImageStreamAsync(localDefinition.ModName, localDefinition.File, localDefinition.IsFromGame);
                    if (ms != null)
                    {
                        try
                        {
                            LeftImage = new Bitmap(ms);
                            var imageHeight = (LeftImage?.PixelSize.Width).GetValueOrDefault();
                            var imageWidth = (LeftImage?.PixelSize.Height).GetValueOrDefault();
                            var info = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ImageInfo);
                            LeftImageInfo = Smart.Format(info, new { Width = imageWidth, Height = imageHeight });
                            LeftHeight = imageHeight;
                            LeftWidth = imageWidth;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            LeftImageInfo = string.Empty;
                            LeftImage = null;
                            left?.Dispose();
                            LeftHeight = LeftWidth = 0;
                        }
                    }
                }
                else
                {
                    LeftImageInfo = string.Empty;
                    LeftImage = null;
                    left?.Dispose();
                    LeftHeight = LeftWidth = 0;
                }
                await Task.Delay(10);
                mutex.Dispose();
            }

            Task.Run(() => parseImage().ConfigureAwait(false)).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the right.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public void SetRight(IDefinition definition)
        {
            var localDefinition = definition;
            async Task parseImage()
            {
                using var mutex = await rightImageLock.LockAsync();
                var right = RightImage;
                if (definition != null)
                {
                    RightImageInfo = string.Empty;
                    RightImage = null;
                    right?.Dispose();
                    RightHeight = RightWidth = 0;
                    using var ms = await modService.GetImageStreamAsync(localDefinition.ModName, localDefinition.File, localDefinition.IsFromGame);
                    if (ms != null)
                    {
                        try
                        {
                            RightImage = new Bitmap(ms);
                            var imageHeight = (RightImage?.PixelSize.Width).GetValueOrDefault();
                            var imageWidth = (RightImage?.PixelSize.Height).GetValueOrDefault();
                            var info = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ImageInfo);
                            RightImageInfo = Smart.Format(info, new { Width = imageWidth, Height = imageHeight });
                            RightHeight = imageHeight;
                            RightWidth = imageWidth;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            RightImageInfo = string.Empty;
                            RightImage = null;
                            right?.Dispose();
                            RightHeight = RightWidth = 0;
                        }
                    }
                }
                else
                {
                    RightImageInfo = string.Empty;
                    RightImage = null;
                    right?.Dispose();
                    RightHeight = RightWidth = 0;
                }
                await Task.Delay(10);
                mutex.Dispose();
            }
            Task.Run(() => parseImage().ConfigureAwait(false)).ConfigureAwait(false);
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
