// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-15-2021
//
// Last Modified By : Mario
// Last Modified On : 04-15-2021
// ***********************************************************************
// <copyright file="TextBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Styling;
using IronyModManager.DI;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Platform.Clipboard;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class TextBox.
    /// Implements the <see cref="Avalonia.Controls.TextBox" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.TextBox" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    public class TextBox : Avalonia.Controls.TextBox, IStyleable
    {
        #region Fields

        /// <summary>
        /// The allow global hot keys property
        /// </summary>
        public static readonly DirectProperty<TextBox, bool> AllowGlobalHotKeysProperty = AvaloniaProperty.RegisterDirect<TextBox, bool>(nameof(AllowGlobalHotKeys), o => o.AllowGlobalHotKeys, (o, v) => o.AllowGlobalHotKeys = v);

        /// <summary>
        /// The show context menu property
        /// </summary>
        public static readonly DirectProperty<TextBox, bool> ShowContextMenuProperty = AvaloniaProperty.RegisterDirect<TextBox, bool>(nameof(ShowContextMenu), o => o.ShowContextMenu, (o, v) => o.ShowContextMenu = v);

        /// <summary>
        /// The allow global hotkeys
        /// </summary>
        private bool allowGlobalHotkeys = false;

        /// <summary>
        /// The focused
        /// </summary>
        private bool focused = false;

        /// <summary>
        /// The message bus
        /// </summary>
        private IMessageBus messageBus;

        /// <summary>
        /// The registerd event
        /// </summary>
        private bool registeredEvent = false;

        /// <summary>
        /// The show context menu
        /// </summary>
        private bool showContextMenu = true;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets a value indicating whether [allow global hot keys].
        /// </summary>
        /// <value><c>true</c> if [allow global hot keys]; otherwise, <c>false</c>.</value>
        public bool AllowGlobalHotKeys
        {
            get { return allowGlobalHotkeys; }
            set { SetAndRaise(AllowGlobalHotKeysProperty, ref allowGlobalHotkeys, value); }
        }

        /// <summary>
        /// Gets a value indicating whether [show context menu].
        /// </summary>
        /// <value><c>true</c> if [show context menu]; otherwise, <c>false</c>.</value>
        public bool ShowContextMenu
        {
            get { return showContextMenu; }
            set { SetAndRaise(ShowContextMenuProperty, ref showContextMenu, value); }
        }

        /// <summary>
        /// Gets the type by which the control is styled.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(Avalonia.Controls.TextBox);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Pastes this instance.
        /// </summary>
        public new void Paste()
        {
            if (!IsReadOnly)
            {
                // Fix avalonia bug 5600 where textbox accepts CRLF even if single line only
                var clipboard = AvaloniaLocator.Current.GetService(typeof(IClipboard)) as IIronyClipboard;
                bool originalClipboardState = clipboard.PreventMultiLineText;
                if (!AcceptsReturn)
                {
                    clipboard.PreventMultiLineText = true;
                }
                base.Paste();
                clipboard.PreventMultiLineText = originalClipboardState;
            }
            else
            {
                base.Paste();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:ApplyTemplate" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs" /> instance containing the event data.</param>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
#pragma warning disable CS0618 // Type or member is obsolete
            if (ContextMenu != null)
            {
                if (registeredEvent)
                {
                    ContextMenu.ContextMenuOpening -= ContextMenuOpening;
                }
                ContextMenu.ContextMenuOpening += ContextMenuOpening;
                registeredEvent = true;
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Handles the <see cref="E:GotFocus" /> event.
        /// </summary>
        /// <param name="e">The <see cref="GotFocusEventArgs" /> instance containing the event data.</param>
        protected override async void OnGotFocus(GotFocusEventArgs e)
        {
            await SetGlobalKeyStateAsync(true);
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Handles the <see cref="E:KeyDown" /> event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!IsReadOnly)
            {
                // Fix avalonia bug 5600 where textbox accepts CRLF even if single line only
                var clipboard = AvaloniaLocator.Current.GetService(typeof(IClipboard)) as IIronyClipboard;
                var keymap = AvaloniaLocator.Current.GetService<PlatformHotkeyConfiguration>();
                bool Match(List<KeyGesture> gestures) => gestures.Any(g => g.Matches(e));
                bool originalClipboardState = clipboard.PreventMultiLineText;
                if (Match(keymap.Paste) && !AcceptsReturn)
                {
                    clipboard.PreventMultiLineText = true;
                }
                base.OnKeyDown(e);
                clipboard.PreventMultiLineText = originalClipboardState;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:LostFocus" /> event.
        /// </summary>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        protected override async void OnLostFocus(RoutedEventArgs e)
        {
            await SetGlobalKeyStateAsync(false);
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Called before the <see cref="E:Avalonia.Input.InputElement.PointerLeave" /> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnPointerLeave(PointerEventArgs e)
        {
            focused = false;
            base.OnPointerLeave(e);
        }

        /// <summary>
        /// Handles the <see cref="E:PointerMoved" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs" /> instance containing the event data.</param>
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            focused = true;
            base.OnPointerMoved(e);
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="change">The change.</param>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);
#pragma warning disable CS0618 // Type or member is obsolete
            if (change.Property == ContextMenuProperty)
            {
                if (ContextMenu != null)
                {
                    if (registeredEvent)
                    {
                        ContextMenu.ContextMenuOpening -= ContextMenuOpening;
                    }
                    ContextMenu.ContextMenuOpening += ContextMenuOpening;
                    registeredEvent = true;
                }
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Contexts the menu opening.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void ContextMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (focused)
            {
                e.Cancel = !ShowContextMenu;
            }
        }

        /// <summary>
        /// set global key state as an asynchronous operation.
        /// </summary>
        /// <param name="suspendKeys">if set to <c>true</c> [suspend keys].</param>
        private async Task SetGlobalKeyStateAsync(bool suspendKeys)
        {
            if (IsReadOnly || AllowGlobalHotKeys)
            {
                return;
            }
            if (messageBus == null)
            {
                messageBus = DIResolver.Get<IMessageBus>();
            }
            await messageBus.PublishAsync(new SuspendHotkeysEvent(suspendKeys));
        }

        #endregion Methods
    }
}
