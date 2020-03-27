// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-20-2020
//
// Last Modified By : Mario
// Last Modified On : 03-27-2020
// ***********************************************************************
// <copyright file="MergeViewerControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization.Attributes;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class MergeViewerControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    public class MergeViewerControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The URL action
        /// </summary>
        private readonly IUrlAction urlAction;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeViewerControlViewModel"/> class.
        /// </summary>
        /// <param name="urlAction">The URL action.</param>
        public MergeViewerControlViewModel(IUrlAction urlAction)
        {
            this.urlAction = urlAction;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the cancel.
        /// </summary>
        /// <value>The cancel.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Cancel)]
        public virtual string Cancel { get; protected set; }

        /// <summary>
        /// Gets or sets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public virtual ReactiveCommand<Unit, Unit> CancelCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the copy text.
        /// </summary>
        /// <value>The copy text.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.CopyText)]
        public virtual string CopyText { get; protected set; }

        /// <summary>
        /// Gets or sets the copy text command.
        /// </summary>
        /// <value>The copy text command.</value>
        public virtual ReactiveCommand<bool, Unit> CopyTextCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the copy this.
        /// </summary>
        /// <value>The copy this.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.CopyThis)]
        public virtual string CopyThis { get; protected set; }

        /// <summary>
        /// Gets or sets the copy this after line.
        /// </summary>
        /// <value>The copy this after line.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.CopyThisAfterLine)]
        public virtual string CopyThisAfterLine { get; protected set; }

        /// <summary>
        /// Gets or sets the take other then this command.
        /// </summary>
        /// <value>The take other then this command.</value>
        public virtual ReactiveCommand<bool, Unit> CopyThisAfterLineCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the copy this before line.
        /// </summary>
        /// <value>The copy this before line.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.CopyThisBeforeLine)]
        public virtual string CopyThisBeforeLine { get; protected set; }

        /// <summary>
        /// Gets or sets the copy this before line command.
        /// </summary>
        /// <value>The copy this before line command.</value>
        public virtual ReactiveCommand<bool, Unit> CopyThisBeforeLineCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the copy this command.
        /// </summary>
        /// <value>The copy this command.</value>
        public virtual ReactiveCommand<bool, Unit> CopyThisCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the difference.
        /// </summary>
        /// <value>The difference.</value>
        public virtual SideBySideDiffModel Diff { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [editing left].
        /// </summary>
        /// <value><c>true</c> if [editing left]; otherwise, <c>false</c>.</value>
        public virtual bool EditingLeft { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [editing right].
        /// </summary>
        /// <value><c>true</c> if [editing right]; otherwise, <c>false</c>.</value>
        public virtual bool EditingRight { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [editing text].
        /// </summary>
        /// <value><c>true</c> if [editing text]; otherwise, <c>false</c>.</value>
        public virtual bool EditingText { get; protected set; }

        /// <summary>
        /// Gets or sets the edit this.
        /// </summary>
        /// <value>The edit this.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.Edit)]
        public virtual string EditThis { get; protected set; }

        /// <summary>
        /// Gets or sets the edit this command.
        /// </summary>
        /// <value>The edit this command.</value>
        public virtual ReactiveCommand<bool, Unit> EditThisCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the left side.
        /// </summary>
        /// <value>The left side.</value>
        public virtual string LeftSide { get; protected set; }

        /// <summary>
        /// Gets or sets the left side selected.
        /// </summary>
        /// <value>The left side selected.</value>
        public virtual IEnumerable<DiffPiece> LeftSideSelected { get; set; }

        /// <summary>
        /// Gets or sets the move down.
        /// </summary>
        /// <value>The move down.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.MoveDown)]
        public virtual string MoveDown { get; protected set; }

        /// <summary>
        /// Gets or sets the move down command.
        /// </summary>
        /// <value>The move down command.</value>
        public virtual ReactiveCommand<bool, Unit> MoveDownCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the move up.
        /// </summary>
        /// <value>The move up.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.MoveUp)]
        public virtual string MoveUp { get; protected set; }

        /// <summary>
        /// Gets or sets the move up command.
        /// </summary>
        /// <value>The move up command.</value>
        public virtual ReactiveCommand<bool, Unit> MoveUpCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the ok.
        /// </summary>
        /// <value>The ok.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.OK)]
        public virtual string OK { get; protected set; }

        /// <summary>
        /// Gets or sets the ok command.
        /// </summary>
        /// <value>The ok command.</value>
        public virtual ReactiveCommand<Unit, Unit> OKCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the right side.
        /// </summary>
        /// <value>The right side.</value>
        public virtual string RightSide { get; protected set; }

        /// <summary>
        /// Gets or sets the right side selected.
        /// </summary>
        /// <value>The right side selected.</value>
        public virtual IEnumerable<DiffPiece> RightSideSelected { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Exits the edit mode.
        /// </summary>
        public virtual void ExitEditMode()
        {
            EditingLeft = false;
            EditingRight = false;
            EditingText = false;
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="leftSide">The left side.</param>
        /// <param name="rightSide">The right side.</param>
        public virtual void SetText(string leftSide, string rightSide)
        {
            LeftSide = leftSide ?? string.Empty;
            RightSide = rightSide ?? string.Empty;
            Compare();
        }

        /// <summary>
        /// Compares this instance.
        /// </summary>
        protected virtual void Compare()
        {
            var builder = new SideBySideDiffBuilder(new Differ());
            Diff = builder.BuildDiffModel(LeftSide, RightSide, true);
        }

        /// <summary>
        /// Copies the specified selected.
        /// </summary>
        /// <param name="selected">The selected.</param>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void Copy(IEnumerable<DiffPiece> selected, IList<DiffPiece> source, IList<DiffPiece> destination, bool leftSide)
        {
            if (selected?.Count() > 0)
            {
                var ordered = OrderSelected(selected, source);
                foreach (var item in ordered)
                {
                    destination.RemoveAt(item.Key);
                    destination.Insert(item.Key, item.Value);
                }
                if (leftSide)
                {
                    SetText(LeftSide, string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)));
                }
                else
                {
                    SetText(string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)), RightSide);
                }
            }
        }

        /// <summary>
        /// Copies the after lines.
        /// </summary>
        /// <param name="selected">The selected.</param>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void CopyAfterLines(IEnumerable<DiffPiece> selected, IList<DiffPiece> source, IList<DiffPiece> destination, bool leftSide)
        {
            if (selected?.Count() > 0)
            {
                var previousIdx = -1;
                var offset = 0;
                var ordered = OrderSelected(selected, source);
                foreach (var item in ordered.Reverse())
                {
                    var index = item.Key + 1;
                    var diff = Math.Abs(index - previousIdx);
                    if (diff <= 1 && previousIdx > -1)
                    {
                        index = previousIdx;
                    }
                    var count = destination.Count - 1;
                    if (index < 1)
                    {
                        index = 1;
                    }
                    else if (index > count)
                    {
                        index = count;
                    }
                    previousIdx = index;
                    destination.Insert(index, item.Value);
                    offset++;
                }
                if (leftSide)
                {
                    SetText(LeftSide, string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)));
                }
                else
                {
                    SetText(string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)), RightSide);
                }
            }
        }

        /// <summary>
        /// Copies the before lines.
        /// </summary>
        /// <param name="selected">The selected.</param>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void CopyBeforeLines(IEnumerable<DiffPiece> selected, IList<DiffPiece> source, IList<DiffPiece> destination, bool leftSide)
        {
            if (selected?.Count() > 0)
            {
                var previousIdx = -1;
                var offset = 0;
                var ordered = OrderSelected(selected, source);
                foreach (var item in ordered)
                {
                    var index = item.Key;
                    var diff = Math.Abs(index - previousIdx);
                    if (diff > 1 && previousIdx > -1)
                    {
                        index += offset;
                    }
                    if (index == previousIdx && previousIdx > -1)
                    {
                        index = previousIdx + 1;
                    }
                    var count = destination.Count - 1;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    else if (index > count)
                    {
                        index = count;
                    }
                    previousIdx = index;
                    destination.Insert(index, item.Value);
                    offset++;
                }
                if (leftSide)
                {
                    SetText(LeftSide, string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)));
                }
                else
                {
                    SetText(string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)), RightSide);
                }
            }
        }

        /// <summary>
        /// copy text as an asynchronous operation.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected async Task CopyTextAsync(bool leftSide)
        {
            await urlAction.CopyAsync(leftSide ? LeftSide : RightSide);
        }

        /// <summary>
        /// Moves the specified move up.
        /// </summary>
        /// <param name="moveUp">if set to <c>true</c> [move up].</param>
        /// <param name="selected">The selected.</param>
        /// <param name="source">The source.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void Move(bool moveUp, IEnumerable<DiffPiece> selected, IList<DiffPiece> source, bool leftSide)
        {
            if (selected?.Count() > 0)
            {
                var ordered = OrderSelected(selected, source);
                foreach (var item in ordered)
                {
                    var count = source.Count - 1;
                    source.RemoveAt(item.Key);
                    var index = moveUp ? item.Key - 1 : item.Key + 1;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    else if (index > count)
                    {
                        index = count;
                    }
                    source.Insert(index, item.Value);
                }
                if (leftSide)
                {
                    SetText(string.Join(Environment.NewLine, source.Where(p => p.Text != null).Select(p => p.Text)), RightSide);
                }
                else
                {
                    SetText(LeftSide, string.Join(Environment.NewLine, source.Where(p => p.Text != null).Select(p => p.Text)));
                }
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            LeftSideSelected = new ObservableCollection<DiffPiece>();
            RightSideSelected = new ObservableCollection<DiffPiece>();

            CopyThisCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                if (leftSide)
                {
                    Copy(LeftSideSelected, Diff.OldText.Lines, Diff.NewText.Lines, leftSide);
                }
                else
                {
                    Copy(RightSideSelected, Diff.NewText.Lines, Diff.OldText.Lines, leftSide);
                }
            }).DisposeWith(disposables);

            CopyThisBeforeLineCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                if (leftSide)
                {
                    CopyBeforeLines(LeftSideSelected, Diff.OldText.Lines, Diff.NewText.Lines, leftSide);
                }
                else
                {
                    CopyBeforeLines(RightSideSelected, Diff.NewText.Lines, Diff.OldText.Lines, leftSide);
                }
            }).DisposeWith(disposables);

            CopyThisAfterLineCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                if (leftSide)
                {
                    CopyAfterLines(LeftSideSelected, Diff.OldText.Lines, Diff.NewText.Lines, leftSide);
                }
                else
                {
                    CopyAfterLines(RightSideSelected, Diff.NewText.Lines, Diff.OldText.Lines, leftSide);
                }
            }).DisposeWith(disposables);

            MoveUpCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                if (leftSide)
                {
                    Move(true, LeftSideSelected, Diff.OldText.Lines, leftSide);
                }
                else
                {
                    Move(true, LeftSideSelected, Diff.NewText.Lines, leftSide);
                }
            }).DisposeWith(disposables);

            MoveDownCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                if (leftSide)
                {
                    Move(false, LeftSideSelected, Diff.OldText.Lines, leftSide);
                }
                else
                {
                    Move(false, LeftSideSelected, Diff.NewText.Lines, leftSide);
                }
            }).DisposeWith(disposables);

            OKCommand = ReactiveCommand.Create(() =>
            {
                SetText(LeftSide, RightSide);
                ExitEditMode();
            }).DisposeWith(disposables);

            CancelCommand = ReactiveCommand.Create(() =>
            {
                ExitEditMode();
            }).DisposeWith(disposables);

            EditThisCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                EditingLeft = leftSide;
                EditingRight = !leftSide;
                EditingText = true;
            }).DisposeWith(disposables);

            CopyTextCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                CopyTextAsync(leftSide).ConfigureAwait(false);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Orders the selected.
        /// </summary>
        /// <param name="selected">The selected.</param>
        /// <param name="source">The source.</param>
        /// <returns>Dictionary&lt;System.Int32, DiffPiece&gt;.</returns>
        protected virtual Dictionary<int, DiffPiece> OrderSelected(IEnumerable<DiffPiece> selected, IList<DiffPiece> source)
        {
            var orderedSelected = new Dictionary<int, DiffPiece>();
            foreach (var item in selected)
            {
                var idx = source.IndexOf(item);
                orderedSelected.Add(idx, item);
            }
            return orderedSelected.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
        }

        #endregion Methods
    }
}
