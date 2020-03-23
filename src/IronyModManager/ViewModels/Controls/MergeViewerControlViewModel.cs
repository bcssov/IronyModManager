// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-20-2020
//
// Last Modified By : Mario
// Last Modified On : 03-23-2020
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
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using IronyModManager.Common.ViewModels;
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
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeViewerControlViewModel" /> class.
        /// </summary>
        public MergeViewerControlViewModel()
        {
        }

        #endregion Constructors

        #region Properties

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
        /// Sets the text.
        /// </summary>
        /// <param name="leftSide">The left side.</param>
        /// <param name="rightSide">The right side.</param>
        public virtual void SetText(string leftSide, string rightSide)
        {
            LeftSide = ReplaceTabs(leftSide);
            RightSide = ReplaceTabs(rightSide);
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
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(@"asl_close_options2 = { potential = {");
            sb.AppendLine(@"		always = yes2");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	allow2 = {");
            sb.AppendLine(@"		always2 = no");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	effect = {");
            sb.AppendLine(@"		hidden_effect = {");
            sb.AppendLine(@"			remove_country_flag = asl_options_opened1");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");

            var sb2 = new System.Text.StringBuilder();
            sb2.AppendLine(@"asl_close_options = { potential = {");
            sb2.AppendLine(@"		always = yes");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"	allow = {");
            sb2.AppendLine(@"		always = yes");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"	effect = {");
            sb2.AppendLine(@"		hidden_effect = {");
            sb2.AppendLine(@"			remove_country_flag = asl_options_opened");
            sb2.AppendLine(@"			remove_country_flag = asl_options_opened_2");
            sb2.AppendLine(@"		}");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");

            SetText(sb.ToString(), sb2.ToString());

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

        /// <summary>
        /// Replaces the tabs.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        protected virtual string ReplaceTabs(string text)
        {
            var tabSpace = new string(' ', 4);
            return text.Replace("\t", tabSpace);
        }

        #endregion Methods
    }
}
