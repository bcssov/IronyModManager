// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="Keysyms.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using IronyModManager.Shared;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Enum X11Key
    /// </summary>
    [ExcludeFromCoverage("External component.")]
    internal enum X11Key
    {
        /// <summary>
        /// The void symbol
        /// </summary>
        VoidSymbol = 0xffffff /* Void symbol */,

        /// <summary>
        /// The back space
        /// </summary>
        BackSpace = 0xff08 /* Back space, back char */,

        /// <summary>
        /// The tab
        /// </summary>
        Tab = 0xff09,

        /// <summary>
        /// The linefeed
        /// </summary>
        Linefeed = 0xff0a /* Linefeed, LF */,

        /// <summary>
        /// The clear
        /// </summary>
        Clear = 0xff0b,

        /// <summary>
        /// The return
        /// </summary>
        Return = 0xff0d /* Return, enter */,

        /// <summary>
        /// The pause
        /// </summary>
        Pause = 0xff13 /* Pause, hold */,

        /// <summary>
        /// The scroll lock
        /// </summary>
        Scroll_Lock = 0xff14,

        /// <summary>
        /// The system req
        /// </summary>
        Sys_Req = 0xff15,

        /// <summary>
        /// The escape
        /// </summary>
        Escape = 0xff1b,

        /// <summary>
        /// The delete
        /// </summary>
        Delete = 0xffff /* Delete, rubout */,

        /// <summary>
        /// The multi key
        /// </summary>
        Multi_key = 0xff20 /* Multi-key character compose */,

        /// <summary>
        /// The codeinput
        /// </summary>
        Codeinput = 0xff37,

        /// <summary>
        /// The single candidate
        /// </summary>
        SingleCandidate = 0xff3c,

        /// <summary>
        /// The multiple candidate
        /// </summary>
        MultipleCandidate = 0xff3d,

        /// <summary>
        /// The previous candidate
        /// </summary>
        PreviousCandidate = 0xff3e,

        /// <summary>
        /// The kanji
        /// </summary>
        Kanji = 0xff21 /* Kanji, Kanji convert */,

        /// <summary>
        /// The muhenkan
        /// </summary>
        Muhenkan = 0xff22 /* Cancel Conversion */,

        /// <summary>
        /// The henkan mode
        /// </summary>
        Henkan_Mode = 0xff23 /* Start/Stop Conversion */,

        /// <summary>
        /// The henkan
        /// </summary>
        Henkan = 0xff23 /* Alias for Henkan_Mode */,

        /// <summary>
        /// The romaji
        /// </summary>
        Romaji = 0xff24 /* to Romaji */,

        /// <summary>
        /// The hiragana
        /// </summary>
        Hiragana = 0xff25 /* to Hiragana */,

        /// <summary>
        /// The katakana
        /// </summary>
        Katakana = 0xff26 /* to Katakana */,

        /// <summary>
        /// The hiragana katakana
        /// </summary>
        Hiragana_Katakana = 0xff27 /* Hiragana/Katakana toggle */,

        /// <summary>
        /// The zenkaku
        /// </summary>
        Zenkaku = 0xff28 /* to Zenkaku */,

        /// <summary>
        /// The hankaku
        /// </summary>
        Hankaku = 0xff29 /* to Hankaku */,

        /// <summary>
        /// The zenkaku hankaku
        /// </summary>
        Zenkaku_Hankaku = 0xff2a /* Zenkaku/Hankaku toggle */,

        /// <summary>
        /// The touroku
        /// </summary>
        Touroku = 0xff2b /* Add to Dictionary */,

        /// <summary>
        /// The massyo
        /// </summary>
        Massyo = 0xff2c /* Delete from Dictionary */,

        /// <summary>
        /// The kana lock
        /// </summary>
        Kana_Lock = 0xff2d /* Kana Lock */,

        /// <summary>
        /// The kana shift
        /// </summary>
        Kana_Shift = 0xff2e /* Kana Shift */,

        /// <summary>
        /// The eisu shift
        /// </summary>
        Eisu_Shift = 0xff2f /* Alphanumeric Shift */,

        /// <summary>
        /// The eisu toggle
        /// </summary>
        Eisu_toggle = 0xff30 /* Alphanumeric toggle */,

        /// <summary>
        /// The kanji bangou
        /// </summary>
        Kanji_Bangou = 0xff37 /* Codeinput */,

        /// <summary>
        /// The zen koho
        /// </summary>
        Zen_Koho = 0xff3d /* Multiple/All Candidate(s) */,

        /// <summary>
        /// The mae koho
        /// </summary>
        Mae_Koho = 0xff3e /* Previous Candidate */,

        /// <summary>
        /// The home
        /// </summary>
        Home = 0xff50,

        /// <summary>
        /// The left
        /// </summary>
        Left = 0xff51 /* Move left, left arrow */,

        /// <summary>
        /// Up
        /// </summary>
        Up = 0xff52 /* Move up, up arrow */,

        /// <summary>
        /// The right
        /// </summary>
        Right = 0xff53 /* Move right, right arrow */,

        /// <summary>
        /// Down
        /// </summary>
        Down = 0xff54 /* Move down, down arrow */,

        /// <summary>
        /// The prior
        /// </summary>
        Prior = 0xff55 /* Prior, previous */,

        /// <summary>
        /// The page up
        /// </summary>
        Page_Up = 0xff55,

        /// <summary>
        /// The next
        /// </summary>
        Next = 0xff56 /* Next */,

        /// <summary>
        /// The page down
        /// </summary>
        Page_Down = 0xff56,

        /// <summary>
        /// The end
        /// </summary>
        End = 0xff57 /* EOL */,

        /// <summary>
        /// The begin
        /// </summary>
        Begin = 0xff58 /* BOL */,

        /// <summary>
        /// The select
        /// </summary>
        Select = 0xff60 /* Select, mark */,

        /// <summary>
        /// The print
        /// </summary>
        Print = 0xff61,

        /// <summary>
        /// The execute
        /// </summary>
        Execute = 0xff62 /* Execute, run, do */,

        /// <summary>
        /// The insert
        /// </summary>
        Insert = 0xff63 /* Insert, insert here */,

        /// <summary>
        /// The undo
        /// </summary>
        Undo = 0xff65,

        /// <summary>
        /// The redo
        /// </summary>
        Redo = 0xff66 /* Redo, again */,

        /// <summary>
        /// The menu
        /// </summary>
        Menu = 0xff67,

        /// <summary>
        /// The find
        /// </summary>
        Find = 0xff68 /* Find, search */,

        /// <summary>
        /// The cancel
        /// </summary>
        Cancel = 0xff69 /* Cancel, stop, abort, exit */,

        /// <summary>
        /// The help
        /// </summary>
        Help = 0xff6a /* Help */,

        /// <summary>
        /// The break
        /// </summary>
        Break = 0xff6b,

        /// <summary>
        /// The mode switch
        /// </summary>
        Mode_switch = 0xff7e /* Character set switch */,

        /// <summary>
        /// The script switch
        /// </summary>
        script_switch = 0xff7e /* Alias for mode_switch */,

        /// <summary>
        /// The number lock
        /// </summary>
        Num_Lock = 0xff7f,

        /// <summary>
        /// The kp space
        /// </summary>
        KP_Space = 0xff80 /* Space */,

        /// <summary>
        /// The kp tab
        /// </summary>
        KP_Tab = 0xff89,

        /// <summary>
        /// The kp enter
        /// </summary>
        KP_Enter = 0xff8d /* Enter */,

        /// <summary>
        /// The kp f1
        /// </summary>
        KP_F1 = 0xff91 /* PF1, KP_A, ... */,

        /// <summary>
        /// The kp f2
        /// </summary>
        KP_F2 = 0xff92,

        /// <summary>
        /// The kp f3
        /// </summary>
        KP_F3 = 0xff93,

        /// <summary>
        /// The kp f4
        /// </summary>
        KP_F4 = 0xff94,

        /// <summary>
        /// The kp home
        /// </summary>
        KP_Home = 0xff95,

        /// <summary>
        /// The kp left
        /// </summary>
        KP_Left = 0xff96,

        /// <summary>
        /// The kp up
        /// </summary>
        KP_Up = 0xff97,

        /// <summary>
        /// The kp right
        /// </summary>
        KP_Right = 0xff98,

        /// <summary>
        /// The kp down
        /// </summary>
        KP_Down = 0xff99,

        /// <summary>
        /// The kp prior
        /// </summary>
        KP_Prior = 0xff9a,

        /// <summary>
        /// The kp page up
        /// </summary>
        KP_Page_Up = 0xff9a,

        /// <summary>
        /// The kp next
        /// </summary>
        KP_Next = 0xff9b,

        /// <summary>
        /// The kp page down
        /// </summary>
        KP_Page_Down = 0xff9b,

        /// <summary>
        /// The kp end
        /// </summary>
        KP_End = 0xff9c,

        /// <summary>
        /// The kp begin
        /// </summary>
        KP_Begin = 0xff9d,

        /// <summary>
        /// The kp insert
        /// </summary>
        KP_Insert = 0xff9e,

        /// <summary>
        /// The kp delete
        /// </summary>
        KP_Delete = 0xff9f,

        /// <summary>
        /// The kp equal
        /// </summary>
        KP_Equal = 0xffbd /* Equals */,

        /// <summary>
        /// The kp multiply
        /// </summary>
        KP_Multiply = 0xffaa,

        /// <summary>
        /// The kp add
        /// </summary>
        KP_Add = 0xffab,

        /// <summary>
        /// The kp separator
        /// </summary>
        KP_Separator = 0xffac /* Separator, often comma */,

        /// <summary>
        /// The kp subtract
        /// </summary>
        KP_Subtract = 0xffad,

        /// <summary>
        /// The kp decimal
        /// </summary>
        KP_Decimal = 0xffae,

        /// <summary>
        /// The kp divide
        /// </summary>
        KP_Divide = 0xffaf,

        /// <summary>
        /// The kp 0
        /// </summary>
        KP_0 = 0xffb0,

        /// <summary>
        /// The kp 1
        /// </summary>
        KP_1 = 0xffb1,

        /// <summary>
        /// The kp 2
        /// </summary>
        KP_2 = 0xffb2,

        /// <summary>
        /// The kp 3
        /// </summary>
        KP_3 = 0xffb3,

        /// <summary>
        /// The kp 4
        /// </summary>
        KP_4 = 0xffb4,

        /// <summary>
        /// The kp 5
        /// </summary>
        KP_5 = 0xffb5,

        /// <summary>
        /// The kp 6
        /// </summary>
        KP_6 = 0xffb6,

        /// <summary>
        /// The kp 7
        /// </summary>
        KP_7 = 0xffb7,

        /// <summary>
        /// The kp 8
        /// </summary>
        KP_8 = 0xffb8,

        /// <summary>
        /// The kp 9
        /// </summary>
        KP_9 = 0xffb9,

        /// <summary>
        /// The f1
        /// </summary>
        F1 = 0xffbe,

        /// <summary>
        /// The f2
        /// </summary>
        F2 = 0xffbf,

        /// <summary>
        /// The f3
        /// </summary>
        F3 = 0xffc0,

        /// <summary>
        /// The f4
        /// </summary>
        F4 = 0xffc1,

        /// <summary>
        /// The f5
        /// </summary>
        F5 = 0xffc2,

        /// <summary>
        /// The f6
        /// </summary>
        F6 = 0xffc3,

        /// <summary>
        /// The f7
        /// </summary>
        F7 = 0xffc4,

        /// <summary>
        /// The f8
        /// </summary>
        F8 = 0xffc5,

        /// <summary>
        /// The f9
        /// </summary>
        F9 = 0xffc6,

        /// <summary>
        /// The F10
        /// </summary>
        F10 = 0xffc7,

        /// <summary>
        /// The F11
        /// </summary>
        F11 = 0xffc8,

        /// <summary>
        /// The l1
        /// </summary>
        L1 = 0xffc8,

        /// <summary>
        /// The F12
        /// </summary>
        F12 = 0xffc9,

        /// <summary>
        /// The l2
        /// </summary>
        L2 = 0xffc9,

        /// <summary>
        /// The F13
        /// </summary>
        F13 = 0xffca,

        /// <summary>
        /// The l3
        /// </summary>
        L3 = 0xffca,

        /// <summary>
        /// The F14
        /// </summary>
        F14 = 0xffcb,

        /// <summary>
        /// The l4
        /// </summary>
        L4 = 0xffcb,

        /// <summary>
        /// The F15
        /// </summary>
        F15 = 0xffcc,

        /// <summary>
        /// The l5
        /// </summary>
        L5 = 0xffcc,

        /// <summary>
        /// The F16
        /// </summary>
        F16 = 0xffcd,

        /// <summary>
        /// The l6
        /// </summary>
        L6 = 0xffcd,

        /// <summary>
        /// The F17
        /// </summary>
        F17 = 0xffce,

        /// <summary>
        /// The l7
        /// </summary>
        L7 = 0xffce,

        /// <summary>
        /// The F18
        /// </summary>
        F18 = 0xffcf,

        /// <summary>
        /// The l8
        /// </summary>
        L8 = 0xffcf,

        /// <summary>
        /// The F19
        /// </summary>
        F19 = 0xffd0,

        /// <summary>
        /// The l9
        /// </summary>
        L9 = 0xffd0,

        /// <summary>
        /// The F20
        /// </summary>
        F20 = 0xffd1,

        /// <summary>
        /// The L10
        /// </summary>
        L10 = 0xffd1,

        /// <summary>
        /// The F21
        /// </summary>
        F21 = 0xffd2,

        /// <summary>
        /// The r1
        /// </summary>
        R1 = 0xffd2,

        /// <summary>
        /// The F22
        /// </summary>
        F22 = 0xffd3,

        /// <summary>
        /// The r2
        /// </summary>
        R2 = 0xffd3,

        /// <summary>
        /// The F23
        /// </summary>
        F23 = 0xffd4,

        /// <summary>
        /// The r3
        /// </summary>
        R3 = 0xffd4,

        /// <summary>
        /// The F24
        /// </summary>
        F24 = 0xffd5,

        /// <summary>
        /// The r4
        /// </summary>
        R4 = 0xffd5,

        /// <summary>
        /// The F25
        /// </summary>
        F25 = 0xffd6,

        /// <summary>
        /// The r5
        /// </summary>
        R5 = 0xffd6,

        /// <summary>
        /// The F26
        /// </summary>
        F26 = 0xffd7,

        /// <summary>
        /// The r6
        /// </summary>
        R6 = 0xffd7,

        /// <summary>
        /// The F27
        /// </summary>
        F27 = 0xffd8,

        /// <summary>
        /// The r7
        /// </summary>
        R7 = 0xffd8,

        /// <summary>
        /// The F28
        /// </summary>
        F28 = 0xffd9,

        /// <summary>
        /// The r8
        /// </summary>
        R8 = 0xffd9,

        /// <summary>
        /// The F29
        /// </summary>
        F29 = 0xffda,

        /// <summary>
        /// The r9
        /// </summary>
        R9 = 0xffda,

        /// <summary>
        /// The F30
        /// </summary>
        F30 = 0xffdb,

        /// <summary>
        /// The R10
        /// </summary>
        R10 = 0xffdb,

        /// <summary>
        /// The F31
        /// </summary>
        F31 = 0xffdc,

        /// <summary>
        /// The R11
        /// </summary>
        R11 = 0xffdc,

        /// <summary>
        /// The F32
        /// </summary>
        F32 = 0xffdd,

        /// <summary>
        /// The R12
        /// </summary>
        R12 = 0xffdd,

        /// <summary>
        /// The F33
        /// </summary>
        F33 = 0xffde,

        /// <summary>
        /// The R13
        /// </summary>
        R13 = 0xffde,

        /// <summary>
        /// The F34
        /// </summary>
        F34 = 0xffdf,

        /// <summary>
        /// The R14
        /// </summary>
        R14 = 0xffdf,

        /// <summary>
        /// The F35
        /// </summary>
        F35 = 0xffe0,

        /// <summary>
        /// The R15
        /// </summary>
        R15 = 0xffe0,

        /// <summary>
        /// The shift l
        /// </summary>
        Shift_L = 0xffe1 /* Left shift */,

        /// <summary>
        /// The shift r
        /// </summary>
        Shift_R = 0xffe2 /* Right shift */,

        /// <summary>
        /// The control l
        /// </summary>
        Control_L = 0xffe3 /* Left control */,

        /// <summary>
        /// The control r
        /// </summary>
        Control_R = 0xffe4 /* Right control */,

        /// <summary>
        /// The caps lock
        /// </summary>
        Caps_Lock = 0xffe5 /* Caps lock */,

        /// <summary>
        /// The shift lock
        /// </summary>
        Shift_Lock = 0xffe6 /* Shift lock */,

        /// <summary>
        /// The meta l
        /// </summary>
        Meta_L = 0xffe7 /* Left meta */,

        /// <summary>
        /// The meta r
        /// </summary>
        Meta_R = 0xffe8 /* Right meta */,

        /// <summary>
        /// The alt l
        /// </summary>
        Alt_L = 0xffe9 /* Left alt */,

        /// <summary>
        /// The alt r
        /// </summary>
        Alt_R = 0xffea /* Right alt */,

        /// <summary>
        /// The super l
        /// </summary>
        Super_L = 0xffeb /* Left super */,

        /// <summary>
        /// The super r
        /// </summary>
        Super_R = 0xffec /* Right super */,

        /// <summary>
        /// The hyper l
        /// </summary>
        Hyper_L = 0xffed /* Left hyper */,

        /// <summary>
        /// The hyper r
        /// </summary>
        Hyper_R = 0xffee /* Right hyper */,

        /// <summary>
        /// The iso lock
        /// </summary>
        ISO_Lock = 0xfe01,

        /// <summary>
        /// The iso level2 latch
        /// </summary>
        ISO_Level2_Latch = 0xfe02,

        /// <summary>
        /// The iso level3 shift
        /// </summary>
        ISO_Level3_Shift = 0xfe03,

        /// <summary>
        /// The iso level3 latch
        /// </summary>
        ISO_Level3_Latch = 0xfe04,

        /// <summary>
        /// The iso level3 lock
        /// </summary>
        ISO_Level3_Lock = 0xfe05,

        /// <summary>
        /// The iso level5 shift
        /// </summary>
        ISO_Level5_Shift = 0xfe11,

        /// <summary>
        /// The iso level5 latch
        /// </summary>
        ISO_Level5_Latch = 0xfe12,

        /// <summary>
        /// The iso level5 lock
        /// </summary>
        ISO_Level5_Lock = 0xfe13,

        /// <summary>
        /// The iso group shift
        /// </summary>
        ISO_Group_Shift = 0xff7e /* Alias for mode_switch */,

        /// <summary>
        /// The iso group latch
        /// </summary>
        ISO_Group_Latch = 0xfe06,

        /// <summary>
        /// The iso group lock
        /// </summary>
        ISO_Group_Lock = 0xfe07,

        /// <summary>
        /// The iso next group
        /// </summary>
        ISO_Next_Group = 0xfe08,

        /// <summary>
        /// The iso next group lock
        /// </summary>
        ISO_Next_Group_Lock = 0xfe09,

        /// <summary>
        /// The iso previous group
        /// </summary>
        ISO_Prev_Group = 0xfe0a,

        /// <summary>
        /// The iso previous group lock
        /// </summary>
        ISO_Prev_Group_Lock = 0xfe0b,

        /// <summary>
        /// The iso first group
        /// </summary>
        ISO_First_Group = 0xfe0c,

        /// <summary>
        /// The iso first group lock
        /// </summary>
        ISO_First_Group_Lock = 0xfe0d,

        /// <summary>
        /// The iso last group
        /// </summary>
        ISO_Last_Group = 0xfe0e,

        /// <summary>
        /// The iso last group lock
        /// </summary>
        ISO_Last_Group_Lock = 0xfe0f,

        /// <summary>
        /// The iso left tab
        /// </summary>
        ISO_Left_Tab = 0xfe20,

        /// <summary>
        /// The iso move line up
        /// </summary>
        ISO_Move_Line_Up = 0xfe21,

        /// <summary>
        /// The iso move line down
        /// </summary>
        ISO_Move_Line_Down = 0xfe22,

        /// <summary>
        /// The iso partial line up
        /// </summary>
        ISO_Partial_Line_Up = 0xfe23,

        /// <summary>
        /// The iso partial line down
        /// </summary>
        ISO_Partial_Line_Down = 0xfe24,

        /// <summary>
        /// The iso partial space left
        /// </summary>
        ISO_Partial_Space_Left = 0xfe25,

        /// <summary>
        /// The iso partial space right
        /// </summary>
        ISO_Partial_Space_Right = 0xfe26,

        /// <summary>
        /// The iso set margin left
        /// </summary>
        ISO_Set_Margin_Left = 0xfe27,

        /// <summary>
        /// The iso set margin right
        /// </summary>
        ISO_Set_Margin_Right = 0xfe28,

        /// <summary>
        /// The iso release margin left
        /// </summary>
        ISO_Release_Margin_Left = 0xfe29,

        /// <summary>
        /// The iso release margin right
        /// </summary>
        ISO_Release_Margin_Right = 0xfe2a,

        /// <summary>
        /// The iso release both margins
        /// </summary>
        ISO_Release_Both_Margins = 0xfe2b,

        /// <summary>
        /// The iso fast cursor left
        /// </summary>
        ISO_Fast_Cursor_Left = 0xfe2c,

        /// <summary>
        /// The iso fast cursor right
        /// </summary>
        ISO_Fast_Cursor_Right = 0xfe2d,

        /// <summary>
        /// The iso fast cursor up
        /// </summary>
        ISO_Fast_Cursor_Up = 0xfe2e,

        /// <summary>
        /// The iso fast cursor down
        /// </summary>
        ISO_Fast_Cursor_Down = 0xfe2f,

        /// <summary>
        /// The iso continuous underline
        /// </summary>
        ISO_Continuous_Underline = 0xfe30,

        /// <summary>
        /// The iso discontinuous underline
        /// </summary>
        ISO_Discontinuous_Underline = 0xfe31,

        /// <summary>
        /// The iso emphasize
        /// </summary>
        ISO_Emphasize = 0xfe32,

        /// <summary>
        /// The iso center object
        /// </summary>
        ISO_Center_Object = 0xfe33,

        /// <summary>
        /// The iso enter
        /// </summary>
        ISO_Enter = 0xfe34,

        /// <summary>
        /// The dead grave
        /// </summary>
        dead_grave = 0xfe50,

        /// <summary>
        /// The dead acute
        /// </summary>
        dead_acute = 0xfe51,

        /// <summary>
        /// The dead circumflex
        /// </summary>
        dead_circumflex = 0xfe52,

        /// <summary>
        /// The dead tilde
        /// </summary>
        dead_tilde = 0xfe53,

        /// <summary>
        /// The dead perispomeni
        /// </summary>
        dead_perispomeni = 0xfe53 /* alias for dead_tilde */,

        /// <summary>
        /// The dead macron
        /// </summary>
        dead_macron = 0xfe54,

        /// <summary>
        /// The dead breve
        /// </summary>
        dead_breve = 0xfe55,

        /// <summary>
        /// The dead abovedot
        /// </summary>
        dead_abovedot = 0xfe56,

        /// <summary>
        /// The dead diaeresis
        /// </summary>
        dead_diaeresis = 0xfe57,

        /// <summary>
        /// The dead abovering
        /// </summary>
        dead_abovering = 0xfe58,

        /// <summary>
        /// The dead doubleacute
        /// </summary>
        dead_doubleacute = 0xfe59,

        /// <summary>
        /// The dead caron
        /// </summary>
        dead_caron = 0xfe5a,

        /// <summary>
        /// The dead cedilla
        /// </summary>
        dead_cedilla = 0xfe5b,

        /// <summary>
        /// The dead ogonek
        /// </summary>
        dead_ogonek = 0xfe5c,

        /// <summary>
        /// The dead iota
        /// </summary>
        dead_iota = 0xfe5d,

        /// <summary>
        /// The dead voiced sound
        /// </summary>
        dead_voiced_sound = 0xfe5e,

        /// <summary>
        /// The dead semivoiced sound
        /// </summary>
        dead_semivoiced_sound = 0xfe5f,

        /// <summary>
        /// The dead belowdot
        /// </summary>
        dead_belowdot = 0xfe60,

        /// <summary>
        /// The dead hook
        /// </summary>
        dead_hook = 0xfe61,

        /// <summary>
        /// The dead horn
        /// </summary>
        dead_horn = 0xfe62,

        /// <summary>
        /// The dead stroke
        /// </summary>
        dead_stroke = 0xfe63,

        /// <summary>
        /// The dead abovecomma
        /// </summary>
        dead_abovecomma = 0xfe64,

        /// <summary>
        /// The dead psili
        /// </summary>
        dead_psili = 0xfe64 /* alias for dead_abovecomma */,

        /// <summary>
        /// The dead abovereversedcomma
        /// </summary>
        dead_abovereversedcomma = 0xfe65,

        /// <summary>
        /// The dead dasia
        /// </summary>
        dead_dasia = 0xfe65 /* alias for dead_abovereversedcomma */,

        /// <summary>
        /// The dead doublegrave
        /// </summary>
        dead_doublegrave = 0xfe66,

        /// <summary>
        /// The dead belowring
        /// </summary>
        dead_belowring = 0xfe67,

        /// <summary>
        /// The dead belowmacron
        /// </summary>
        dead_belowmacron = 0xfe68,

        /// <summary>
        /// The dead belowcircumflex
        /// </summary>
        dead_belowcircumflex = 0xfe69,

        /// <summary>
        /// The dead belowtilde
        /// </summary>
        dead_belowtilde = 0xfe6a,

        /// <summary>
        /// The dead belowbreve
        /// </summary>
        dead_belowbreve = 0xfe6b,

        /// <summary>
        /// The dead belowdiaeresis
        /// </summary>
        dead_belowdiaeresis = 0xfe6c,

        /// <summary>
        /// The dead invertedbreve
        /// </summary>
        dead_invertedbreve = 0xfe6d,

        /// <summary>
        /// The dead belowcomma
        /// </summary>
        dead_belowcomma = 0xfe6e,

        /// <summary>
        /// The dead currency
        /// </summary>
        dead_currency = 0xfe6f,

        /// <summary>
        /// The dead lowline
        /// </summary>
        dead_lowline = 0xfe90,

        /// <summary>
        /// The dead aboveverticalline
        /// </summary>
        dead_aboveverticalline = 0xfe91,

        /// <summary>
        /// The dead belowverticalline
        /// </summary>
        dead_belowverticalline = 0xfe92,

        /// <summary>
        /// The dead longsolidusoverlay
        /// </summary>
        dead_longsolidusoverlay = 0xfe93,

        /// <summary>
        /// The dead a
        /// </summary>
        dead_a = 0xfe80,

        /// <summary>
        /// The dead a
        /// </summary>
        dead_A = 0xfe81,

        /// <summary>
        /// The dead e
        /// </summary>
        dead_e = 0xfe82,

        /// <summary>
        /// The dead e
        /// </summary>
        dead_E = 0xfe83,

        /// <summary>
        /// The dead i
        /// </summary>
        dead_i = 0xfe84,

        /// <summary>
        /// The dead i
        /// </summary>
        dead_I = 0xfe85,

        /// <summary>
        /// The dead o
        /// </summary>
        dead_o = 0xfe86,

        /// <summary>
        /// The dead o
        /// </summary>
        dead_O = 0xfe87,

        /// <summary>
        /// The dead u
        /// </summary>
        dead_u = 0xfe88,

        /// <summary>
        /// The dead u
        /// </summary>
        dead_U = 0xfe89,

        /// <summary>
        /// The dead small schwa
        /// </summary>
        dead_small_schwa = 0xfe8a,

        /// <summary>
        /// The dead capital schwa
        /// </summary>
        dead_capital_schwa = 0xfe8b,

        /// <summary>
        /// The dead greek
        /// </summary>
        dead_greek = 0xfe8c,

        /// <summary>
        /// The first virtual screen
        /// </summary>
        First_Virtual_Screen = 0xfed0,

        /// <summary>
        /// The previous virtual screen
        /// </summary>
        Prev_Virtual_Screen = 0xfed1,

        /// <summary>
        /// The next virtual screen
        /// </summary>
        Next_Virtual_Screen = 0xfed2,

        /// <summary>
        /// The last virtual screen
        /// </summary>
        Last_Virtual_Screen = 0xfed4,

        /// <summary>
        /// The terminate server
        /// </summary>
        Terminate_Server = 0xfed5,

        /// <summary>
        /// The access x enable
        /// </summary>
        AccessX_Enable = 0xfe70,

        /// <summary>
        /// The access x feedback enable
        /// </summary>
        AccessX_Feedback_Enable = 0xfe71,

        /// <summary>
        /// The repeat keys enable
        /// </summary>
        RepeatKeys_Enable = 0xfe72,

        /// <summary>
        /// The slow keys enable
        /// </summary>
        SlowKeys_Enable = 0xfe73,

        /// <summary>
        /// The bounce keys enable
        /// </summary>
        BounceKeys_Enable = 0xfe74,

        /// <summary>
        /// The sticky keys enable
        /// </summary>
        StickyKeys_Enable = 0xfe75,

        /// <summary>
        /// The mouse keys enable
        /// </summary>
        MouseKeys_Enable = 0xfe76,

        /// <summary>
        /// The mouse keys accel enable
        /// </summary>
        MouseKeys_Accel_Enable = 0xfe77,

        /// <summary>
        /// The overlay1 enable
        /// </summary>
        Overlay1_Enable = 0xfe78,

        /// <summary>
        /// The overlay2 enable
        /// </summary>
        Overlay2_Enable = 0xfe79,

        /// <summary>
        /// The audible bell enable
        /// </summary>
        AudibleBell_Enable = 0xfe7a,

        /// <summary>
        /// The pointer left
        /// </summary>
        Pointer_Left = 0xfee0,

        /// <summary>
        /// The pointer right
        /// </summary>
        Pointer_Right = 0xfee1,

        /// <summary>
        /// The pointer up
        /// </summary>
        Pointer_Up = 0xfee2,

        /// <summary>
        /// The pointer down
        /// </summary>
        Pointer_Down = 0xfee3,

        /// <summary>
        /// The pointer up left
        /// </summary>
        Pointer_UpLeft = 0xfee4,

        /// <summary>
        /// The pointer up right
        /// </summary>
        Pointer_UpRight = 0xfee5,

        /// <summary>
        /// The pointer down left
        /// </summary>
        Pointer_DownLeft = 0xfee6,

        /// <summary>
        /// The pointer down right
        /// </summary>
        Pointer_DownRight = 0xfee7,

        /// <summary>
        /// The pointer button DFLT
        /// </summary>
        Pointer_Button_Dflt = 0xfee8,

        /// <summary>
        /// The pointer button1
        /// </summary>
        Pointer_Button1 = 0xfee9,

        /// <summary>
        /// The pointer button2
        /// </summary>
        Pointer_Button2 = 0xfeea,

        /// <summary>
        /// The pointer button3
        /// </summary>
        Pointer_Button3 = 0xfeeb,

        /// <summary>
        /// The pointer button4
        /// </summary>
        Pointer_Button4 = 0xfeec,

        /// <summary>
        /// The pointer button5
        /// </summary>
        Pointer_Button5 = 0xfeed,

        /// <summary>
        /// The pointer double click DFLT
        /// </summary>
        Pointer_DblClick_Dflt = 0xfeee,

        /// <summary>
        /// The pointer double click1
        /// </summary>
        Pointer_DblClick1 = 0xfeef,

        /// <summary>
        /// The pointer double click2
        /// </summary>
        Pointer_DblClick2 = 0xfef0,

        /// <summary>
        /// The pointer double click3
        /// </summary>
        Pointer_DblClick3 = 0xfef1,

        /// <summary>
        /// The pointer double click4
        /// </summary>
        Pointer_DblClick4 = 0xfef2,

        /// <summary>
        /// The pointer double click5
        /// </summary>
        Pointer_DblClick5 = 0xfef3,

        /// <summary>
        /// The pointer drag DFLT
        /// </summary>
        Pointer_Drag_Dflt = 0xfef4,

        /// <summary>
        /// The pointer drag1
        /// </summary>
        Pointer_Drag1 = 0xfef5,

        /// <summary>
        /// The pointer drag2
        /// </summary>
        Pointer_Drag2 = 0xfef6,

        /// <summary>
        /// The pointer drag3
        /// </summary>
        Pointer_Drag3 = 0xfef7,

        /// <summary>
        /// The pointer drag4
        /// </summary>
        Pointer_Drag4 = 0xfef8,

        /// <summary>
        /// The pointer drag5
        /// </summary>
        Pointer_Drag5 = 0xfefd,

        /// <summary>
        /// The pointer enable keys
        /// </summary>
        Pointer_EnableKeys = 0xfef9,

        /// <summary>
        /// The pointer accelerate
        /// </summary>
        Pointer_Accelerate = 0xfefa,

        /// <summary>
        /// The pointer DFLT BTN next
        /// </summary>
        Pointer_DfltBtnNext = 0xfefb,

        /// <summary>
        /// The pointer DFLT BTN previous
        /// </summary>
        Pointer_DfltBtnPrev = 0xfefc,

        /// <summary>
        /// The ch
        /// </summary>
        ch = 0xfea0,

        /// <summary>
        /// The ch
        /// </summary>
        Ch = 0xfea1,

        /// <summary>
        /// The ch
        /// </summary>
        CH = 0xfea2,

        /// <summary>
        /// The c h
        /// </summary>
        c_h = 0xfea3,

        /// <summary>
        /// The c h
        /// </summary>
        C_h = 0xfea4,

        /// <summary>
        /// The c h
        /// </summary>
        C_H = 0xfea5,

        /// <summary>
        /// The xk 3270 duplicate
        /// </summary>
        XK_3270_Duplicate = 0xfd01,

        /// <summary>
        /// The xk 3270 field mark
        /// </summary>
        XK_3270_FieldMark = 0xfd02,

        /// <summary>
        /// The xk 3270 right2
        /// </summary>
        XK_3270_Right2 = 0xfd03,

        /// <summary>
        /// The xk 3270 left2
        /// </summary>
        XK_3270_Left2 = 0xfd04,

        /// <summary>
        /// The xk 3270 back tab
        /// </summary>
        XK_3270_BackTab = 0xfd05,

        /// <summary>
        /// The xk 3270 erase EOF
        /// </summary>
        XK_3270_EraseEOF = 0xfd06,

        /// <summary>
        /// The xk 3270 erase input
        /// </summary>
        XK_3270_EraseInput = 0xfd07,

        /// <summary>
        /// The xk 3270 reset
        /// </summary>
        XK_3270_Reset = 0xfd08,

        /// <summary>
        /// The xk 3270 quit
        /// </summary>
        XK_3270_Quit = 0xfd09,

        /// <summary>
        /// The xk 3270 p a1
        /// </summary>
        XK_3270_PA1 = 0xfd0a,

        /// <summary>
        /// The xk 3270 p a2
        /// </summary>
        XK_3270_PA2 = 0xfd0b,

        /// <summary>
        /// The xk 3270 p a3
        /// </summary>
        XK_3270_PA3 = 0xfd0c,

        /// <summary>
        /// The xk 3270 test
        /// </summary>
        XK_3270_Test = 0xfd0d,

        /// <summary>
        /// The xk 3270 attn
        /// </summary>
        XK_3270_Attn = 0xfd0e,

        /// <summary>
        /// The xk 3270 cursor blink
        /// </summary>
        XK_3270_CursorBlink = 0xfd0f,

        /// <summary>
        /// The xk 3270 alt cursor
        /// </summary>
        XK_3270_AltCursor = 0xfd10,

        /// <summary>
        /// The xk 3270 key click
        /// </summary>
        XK_3270_KeyClick = 0xfd11,

        /// <summary>
        /// The xk 3270 jump
        /// </summary>
        XK_3270_Jump = 0xfd12,

        /// <summary>
        /// The xk 3270 ident
        /// </summary>
        XK_3270_Ident = 0xfd13,

        /// <summary>
        /// The xk 3270 rule
        /// </summary>
        XK_3270_Rule = 0xfd14,

        /// <summary>
        /// The xk 3270 copy
        /// </summary>
        XK_3270_Copy = 0xfd15,

        /// <summary>
        /// The xk 3270 play
        /// </summary>
        XK_3270_Play = 0xfd16,

        /// <summary>
        /// The xk 3270 setup
        /// </summary>
        XK_3270_Setup = 0xfd17,

        /// <summary>
        /// The xk 3270 record
        /// </summary>
        XK_3270_Record = 0xfd18,

        /// <summary>
        /// The xk 3270 change screen
        /// </summary>
        XK_3270_ChangeScreen = 0xfd19,

        /// <summary>
        /// The xk 3270 delete word
        /// </summary>
        XK_3270_DeleteWord = 0xfd1a,

        /// <summary>
        /// The xk 3270 ex select
        /// </summary>
        XK_3270_ExSelect = 0xfd1b,

        /// <summary>
        /// The xk 3270 cursor select
        /// </summary>
        XK_3270_CursorSelect = 0xfd1c,

        /// <summary>
        /// The xk 3270 print screen
        /// </summary>
        XK_3270_PrintScreen = 0xfd1d,

        /// <summary>
        /// The xk 3270 enter
        /// </summary>
        XK_3270_Enter = 0xfd1e,

        /// <summary>
        /// The space
        /// </summary>
        space = 0x0020 /* U+0020 SPACE */,

        /// <summary>
        /// The exclam
        /// </summary>
        exclam = 0x0021 /* U+0021 EXCLAMATION MARK */,

        /// <summary>
        /// The quotedbl
        /// </summary>
        quotedbl = 0x0022 /* U+0022 QUOTATION MARK */,

        /// <summary>
        /// The numbersign
        /// </summary>
        numbersign = 0x0023 /* U+0023 NUMBER SIGN */,

        /// <summary>
        /// The dollar
        /// </summary>
        dollar = 0x0024 /* U+0024 DOLLAR SIGN */,

        /// <summary>
        /// The percent
        /// </summary>
        percent = 0x0025 /* U+0025 PERCENT SIGN */,

        /// <summary>
        /// The ampersand
        /// </summary>
        ampersand = 0x0026 /* U+0026 AMPERSAND */,

        /// <summary>
        /// The apostrophe
        /// </summary>
        apostrophe = 0x0027 /* U+0027 APOSTROPHE */,

        /// <summary>
        /// The quoteright
        /// </summary>
        quoteright = 0x0027 /* deprecated */,

        /// <summary>
        /// The parenleft
        /// </summary>
        parenleft = 0x0028 /* U+0028 LEFT PARENTHESIS */,

        /// <summary>
        /// The parenright
        /// </summary>
        parenright = 0x0029 /* U+0029 RIGHT PARENTHESIS */,

        /// <summary>
        /// The asterisk
        /// </summary>
        asterisk = 0x002a /* U+002A ASTERISK */,

        /// <summary>
        /// The plus
        /// </summary>
        plus = 0x002b /* U+002B PLUS SIGN */,

        /// <summary>
        /// The comma
        /// </summary>
        comma = 0x002c /* U+002C COMMA */,

        /// <summary>
        /// The minus
        /// </summary>
        minus = 0x002d /* U+002D HYPHEN-MINUS */,

        /// <summary>
        /// The period
        /// </summary>
        period = 0x002e /* U+002E FULL STOP */,

        /// <summary>
        /// The slash
        /// </summary>
        slash = 0x002f /* U+002F SOLIDUS */,

        /// <summary>
        /// The xk 0
        /// </summary>
        XK_0 = 0x0030 /* U+0030 DIGIT ZERO */,

        /// <summary>
        /// The xk 1
        /// </summary>
        XK_1 = 0x0031 /* U+0031 DIGIT ONE */,

        /// <summary>
        /// The xk 2
        /// </summary>
        XK_2 = 0x0032 /* U+0032 DIGIT TWO */,

        /// <summary>
        /// The xk 3
        /// </summary>
        XK_3 = 0x0033 /* U+0033 DIGIT THREE */,

        /// <summary>
        /// The xk 4
        /// </summary>
        XK_4 = 0x0034 /* U+0034 DIGIT FOUR */,

        /// <summary>
        /// The xk 5
        /// </summary>
        XK_5 = 0x0035 /* U+0035 DIGIT FIVE */,

        /// <summary>
        /// The xk 6
        /// </summary>
        XK_6 = 0x0036 /* U+0036 DIGIT SIX */,

        /// <summary>
        /// The xk 7
        /// </summary>
        XK_7 = 0x0037 /* U+0037 DIGIT SEVEN */,

        /// <summary>
        /// The xk 8
        /// </summary>
        XK_8 = 0x0038 /* U+0038 DIGIT EIGHT */,

        /// <summary>
        /// The xk 9
        /// </summary>
        XK_9 = 0x0039 /* U+0039 DIGIT NINE */,

        /// <summary>
        /// The colon
        /// </summary>
        colon = 0x003a /* U+003A COLON */,

        /// <summary>
        /// The semicolon
        /// </summary>
        semicolon = 0x003b /* U+003B SEMICOLON */,

        /// <summary>
        /// The less
        /// </summary>
        less = 0x003c /* U+003C LESS-THAN SIGN */,

        /// <summary>
        /// The equal
        /// </summary>
        equal = 0x003d /* U+003D EQUALS SIGN */,

        /// <summary>
        /// The greater
        /// </summary>
        greater = 0x003e /* U+003E GREATER-THAN SIGN */,

        /// <summary>
        /// The question
        /// </summary>
        question = 0x003f /* U+003F QUESTION MARK */,

        /// <summary>
        /// At
        /// </summary>
        at = 0x0040 /* U+0040 COMMERCIAL AT */,

        /// <summary>
        /// a
        /// </summary>
        A = 0x0041 /* U+0041 LATIN CAPITAL LETTER A */,

        /// <summary>
        /// The b
        /// </summary>
        B = 0x0042 /* U+0042 LATIN CAPITAL LETTER B */,

        /// <summary>
        /// The c
        /// </summary>
        C = 0x0043 /* U+0043 LATIN CAPITAL LETTER C */,

        /// <summary>
        /// The d
        /// </summary>
        D = 0x0044 /* U+0044 LATIN CAPITAL LETTER D */,

        /// <summary>
        /// The e
        /// </summary>
        E = 0x0045 /* U+0045 LATIN CAPITAL LETTER E */,

        /// <summary>
        /// The f
        /// </summary>
        F = 0x0046 /* U+0046 LATIN CAPITAL LETTER F */,

        /// <summary>
        /// The g
        /// </summary>
        G = 0x0047 /* U+0047 LATIN CAPITAL LETTER G */,

        /// <summary>
        /// The h
        /// </summary>
        H = 0x0048 /* U+0048 LATIN CAPITAL LETTER H */,

        /// <summary>
        /// The i
        /// </summary>
        I = 0x0049 /* U+0049 LATIN CAPITAL LETTER I */,

        /// <summary>
        /// The j
        /// </summary>
        J = 0x004a /* U+004A LATIN CAPITAL LETTER J */,

        /// <summary>
        /// The k
        /// </summary>
        K = 0x004b /* U+004B LATIN CAPITAL LETTER K */,

        /// <summary>
        /// The l
        /// </summary>
        L = 0x004c /* U+004C LATIN CAPITAL LETTER L */,

        /// <summary>
        /// The m
        /// </summary>
        M = 0x004d /* U+004D LATIN CAPITAL LETTER M */,

        /// <summary>
        /// The n
        /// </summary>
        N = 0x004e /* U+004E LATIN CAPITAL LETTER N */,

        /// <summary>
        /// The o
        /// </summary>
        O = 0x004f /* U+004F LATIN CAPITAL LETTER O */,

        /// <summary>
        /// The p
        /// </summary>
        P = 0x0050 /* U+0050 LATIN CAPITAL LETTER P */,

        /// <summary>
        /// The q
        /// </summary>
        Q = 0x0051 /* U+0051 LATIN CAPITAL LETTER Q */,

        /// <summary>
        /// The r
        /// </summary>
        R = 0x0052 /* U+0052 LATIN CAPITAL LETTER R */,

        /// <summary>
        /// The s
        /// </summary>
        S = 0x0053 /* U+0053 LATIN CAPITAL LETTER S */,

        /// <summary>
        /// The t
        /// </summary>
        T = 0x0054 /* U+0054 LATIN CAPITAL LETTER T */,

        /// <summary>
        /// The u
        /// </summary>
        U = 0x0055 /* U+0055 LATIN CAPITAL LETTER U */,

        /// <summary>
        /// The v
        /// </summary>
        V = 0x0056 /* U+0056 LATIN CAPITAL LETTER V */,

        /// <summary>
        /// The w
        /// </summary>
        W = 0x0057 /* U+0057 LATIN CAPITAL LETTER W */,

        /// <summary>
        /// The x
        /// </summary>
        X = 0x0058 /* U+0058 LATIN CAPITAL LETTER X */,

        /// <summary>
        /// The y
        /// </summary>
        Y = 0x0059 /* U+0059 LATIN CAPITAL LETTER Y */,

        /// <summary>
        /// The z
        /// </summary>
        Z = 0x005a /* U+005A LATIN CAPITAL LETTER Z */,

        /// <summary>
        /// The bracketleft
        /// </summary>
        bracketleft = 0x005b /* U+005B LEFT SQUARE BRACKET */,

        /// <summary>
        /// The backslash
        /// </summary>
        backslash = 0x005c /* U+005C REVERSE SOLIDUS */,

        /// <summary>
        /// The bracketright
        /// </summary>
        bracketright = 0x005d /* U+005D RIGHT SQUARE BRACKET */,

        /// <summary>
        /// The asciicircum
        /// </summary>
        asciicircum = 0x005e /* U+005E CIRCUMFLEX ACCENT */,

        /// <summary>
        /// The underscore
        /// </summary>
        underscore = 0x005f /* U+005F LOW LINE */,

        /// <summary>
        /// The grave
        /// </summary>
        grave = 0x0060 /* U+0060 GRAVE ACCENT */,

        /// <summary>
        /// The quoteleft
        /// </summary>
        quoteleft = 0x0060 /* deprecated */,

        /// <summary>
        /// a
        /// </summary>
        a = 0x0061 /* U+0061 LATIN SMALL LETTER A */,

        /// <summary>
        /// The b
        /// </summary>
        b = 0x0062 /* U+0062 LATIN SMALL LETTER B */,

        /// <summary>
        /// The c
        /// </summary>
        c = 0x0063 /* U+0063 LATIN SMALL LETTER C */,

        /// <summary>
        /// The d
        /// </summary>
        d = 0x0064 /* U+0064 LATIN SMALL LETTER D */,

        /// <summary>
        /// The e
        /// </summary>
        e = 0x0065 /* U+0065 LATIN SMALL LETTER E */,

        /// <summary>
        /// The f
        /// </summary>
        f = 0x0066 /* U+0066 LATIN SMALL LETTER F */,

        /// <summary>
        /// The g
        /// </summary>
        g = 0x0067 /* U+0067 LATIN SMALL LETTER G */,

        /// <summary>
        /// The h
        /// </summary>
        h = 0x0068 /* U+0068 LATIN SMALL LETTER H */,

        /// <summary>
        /// The i
        /// </summary>
        i = 0x0069 /* U+0069 LATIN SMALL LETTER I */,

        /// <summary>
        /// The j
        /// </summary>
        j = 0x006a /* U+006A LATIN SMALL LETTER J */,

        /// <summary>
        /// The k
        /// </summary>
        k = 0x006b /* U+006B LATIN SMALL LETTER K */,

        /// <summary>
        /// The l
        /// </summary>
        l = 0x006c /* U+006C LATIN SMALL LETTER L */,

        /// <summary>
        /// The m
        /// </summary>
        m = 0x006d /* U+006D LATIN SMALL LETTER M */,

        /// <summary>
        /// The n
        /// </summary>
        n = 0x006e /* U+006E LATIN SMALL LETTER N */,

        /// <summary>
        /// The o
        /// </summary>
        o = 0x006f /* U+006F LATIN SMALL LETTER O */,

        /// <summary>
        /// The p
        /// </summary>
        p = 0x0070 /* U+0070 LATIN SMALL LETTER P */,

        /// <summary>
        /// The q
        /// </summary>
        q = 0x0071 /* U+0071 LATIN SMALL LETTER Q */,

        /// <summary>
        /// The r
        /// </summary>
        r = 0x0072 /* U+0072 LATIN SMALL LETTER R */,

        /// <summary>
        /// The s
        /// </summary>
        s = 0x0073 /* U+0073 LATIN SMALL LETTER S */,

        /// <summary>
        /// The t
        /// </summary>
        t = 0x0074 /* U+0074 LATIN SMALL LETTER T */,

        /// <summary>
        /// The u
        /// </summary>
        u = 0x0075 /* U+0075 LATIN SMALL LETTER U */,

        /// <summary>
        /// The v
        /// </summary>
        v = 0x0076 /* U+0076 LATIN SMALL LETTER V */,

        /// <summary>
        /// The w
        /// </summary>
        w = 0x0077 /* U+0077 LATIN SMALL LETTER W */,

        /// <summary>
        /// The x
        /// </summary>
        x = 0x0078 /* U+0078 LATIN SMALL LETTER X */,

        /// <summary>
        /// The y
        /// </summary>
        y = 0x0079 /* U+0079 LATIN SMALL LETTER Y */,

        /// <summary>
        /// The z
        /// </summary>
        z = 0x007a /* U+007A LATIN SMALL LETTER Z */,

        /// <summary>
        /// The braceleft
        /// </summary>
        braceleft = 0x007b /* U+007B LEFT CURLY BRACKET */,

        /// <summary>
        /// The bar
        /// </summary>
        bar = 0x007c /* U+007C VERTICAL LINE */,

        /// <summary>
        /// The braceright
        /// </summary>
        braceright = 0x007d /* U+007D RIGHT CURLY BRACKET */,

        /// <summary>
        /// The asciitilde
        /// </summary>
        asciitilde = 0x007e /* U+007E TILDE */,

        /// <summary>
        /// The nobreakspace
        /// </summary>
        nobreakspace = 0x00a0 /* U+00A0 NO-BREAK SPACE */,

        /// <summary>
        /// The exclamdown
        /// </summary>
        exclamdown = 0x00a1 /* U+00A1 INVERTED EXCLAMATION MARK */,

        /// <summary>
        /// The cent
        /// </summary>
        cent = 0x00a2 /* U+00A2 CENT SIGN */,

        /// <summary>
        /// The sterling
        /// </summary>
        sterling = 0x00a3 /* U+00A3 POUND SIGN */,

        /// <summary>
        /// The currency
        /// </summary>
        currency = 0x00a4 /* U+00A4 CURRENCY SIGN */,

        /// <summary>
        /// The yen
        /// </summary>
        yen = 0x00a5 /* U+00A5 YEN SIGN */,

        /// <summary>
        /// The brokenbar
        /// </summary>
        brokenbar = 0x00a6 /* U+00A6 BROKEN BAR */,

        /// <summary>
        /// The section
        /// </summary>
        section = 0x00a7 /* U+00A7 SECTION SIGN */,

        /// <summary>
        /// The diaeresis
        /// </summary>
        diaeresis = 0x00a8 /* U+00A8 DIAERESIS */,

        /// <summary>
        /// The copyright
        /// </summary>
        copyright = 0x00a9 /* U+00A9 COPYRIGHT SIGN */,

        /// <summary>
        /// The ordfeminine
        /// </summary>
        ordfeminine = 0x00aa /* U+00AA FEMININE ORDINAL INDICATOR */,

        /// <summary>
        /// The guillemotleft
        /// </summary>
        guillemotleft = 0x00ab /* U+00AB LEFT-POINTING DOUBLE ANGLE QUOTATION MARK */,

        /// <summary>
        /// The notsign
        /// </summary>
        notsign = 0x00ac /* U+00AC NOT SIGN */,

        /// <summary>
        /// The hyphen
        /// </summary>
        hyphen = 0x00ad /* U+00AD SOFT HYPHEN */,

        /// <summary>
        /// The registered
        /// </summary>
        registered = 0x00ae /* U+00AE REGISTERED SIGN */,

        /// <summary>
        /// The macron
        /// </summary>
        macron = 0x00af /* U+00AF MACRON */,

        /// <summary>
        /// The degree
        /// </summary>
        degree = 0x00b0 /* U+00B0 DEGREE SIGN */,

        /// <summary>
        /// The plusminus
        /// </summary>
        plusminus = 0x00b1 /* U+00B1 PLUS-MINUS SIGN */,

        /// <summary>
        /// The twosuperior
        /// </summary>
        twosuperior = 0x00b2 /* U+00B2 SUPERSCRIPT TWO */,

        /// <summary>
        /// The threesuperior
        /// </summary>
        threesuperior = 0x00b3 /* U+00B3 SUPERSCRIPT THREE */,

        /// <summary>
        /// The acute
        /// </summary>
        acute = 0x00b4 /* U+00B4 ACUTE ACCENT */,

        /// <summary>
        /// The mu
        /// </summary>
        mu = 0x00b5 /* U+00B5 MICRO SIGN */,

        /// <summary>
        /// The paragraph
        /// </summary>
        paragraph = 0x00b6 /* U+00B6 PILCROW SIGN */,

        /// <summary>
        /// The periodcentered
        /// </summary>
        periodcentered = 0x00b7 /* U+00B7 MIDDLE DOT */,

        /// <summary>
        /// The cedilla
        /// </summary>
        cedilla = 0x00b8 /* U+00B8 CEDILLA */,

        /// <summary>
        /// The onesuperior
        /// </summary>
        onesuperior = 0x00b9 /* U+00B9 SUPERSCRIPT ONE */,

        /// <summary>
        /// The masculine
        /// </summary>
        masculine = 0x00ba /* U+00BA MASCULINE ORDINAL INDICATOR */,

        /// <summary>
        /// The guillemotright
        /// </summary>
        guillemotright = 0x00bb /* U+00BB RIGHT-POINTING DOUBLE ANGLE QUOTATION MARK */,

        /// <summary>
        /// The onequarter
        /// </summary>
        onequarter = 0x00bc /* U+00BC VULGAR FRACTION ONE QUARTER */,

        /// <summary>
        /// The onehalf
        /// </summary>
        onehalf = 0x00bd /* U+00BD VULGAR FRACTION ONE HALF */,

        /// <summary>
        /// The threequarters
        /// </summary>
        threequarters = 0x00be /* U+00BE VULGAR FRACTION THREE QUARTERS */,

        /// <summary>
        /// The questiondown
        /// </summary>
        questiondown = 0x00bf /* U+00BF INVERTED QUESTION MARK */,

        /// <summary>
        /// The agrave
        /// </summary>
        Agrave = 0x00c0 /* U+00C0 LATIN CAPITAL LETTER A WITH GRAVE */,

        /// <summary>
        /// The aacute
        /// </summary>
        Aacute = 0x00c1 /* U+00C1 LATIN CAPITAL LETTER A WITH ACUTE */,

        /// <summary>
        /// The acircumflex
        /// </summary>
        Acircumflex = 0x00c2 /* U+00C2 LATIN CAPITAL LETTER A WITH CIRCUMFLEX */,

        /// <summary>
        /// The atilde
        /// </summary>
        Atilde = 0x00c3 /* U+00C3 LATIN CAPITAL LETTER A WITH TILDE */,

        /// <summary>
        /// The adiaeresis
        /// </summary>
        Adiaeresis = 0x00c4 /* U+00C4 LATIN CAPITAL LETTER A WITH DIAERESIS */,

        /// <summary>
        /// The aring
        /// </summary>
        Aring = 0x00c5 /* U+00C5 LATIN CAPITAL LETTER A WITH RING ABOVE */,

        /// <summary>
        /// The ae
        /// </summary>
        AE = 0x00c6 /* U+00C6 LATIN CAPITAL LETTER AE */,

        /// <summary>
        /// The ccedilla
        /// </summary>
        Ccedilla = 0x00c7 /* U+00C7 LATIN CAPITAL LETTER C WITH CEDILLA */,

        /// <summary>
        /// The egrave
        /// </summary>
        Egrave = 0x00c8 /* U+00C8 LATIN CAPITAL LETTER E WITH GRAVE */,

        /// <summary>
        /// The eacute
        /// </summary>
        Eacute = 0x00c9 /* U+00C9 LATIN CAPITAL LETTER E WITH ACUTE */,

        /// <summary>
        /// The ecircumflex
        /// </summary>
        Ecircumflex = 0x00ca /* U+00CA LATIN CAPITAL LETTER E WITH CIRCUMFLEX */,

        /// <summary>
        /// The ediaeresis
        /// </summary>
        Ediaeresis = 0x00cb /* U+00CB LATIN CAPITAL LETTER E WITH DIAERESIS */,

        /// <summary>
        /// The igrave
        /// </summary>
        Igrave = 0x00cc /* U+00CC LATIN CAPITAL LETTER I WITH GRAVE */,

        /// <summary>
        /// The iacute
        /// </summary>
        Iacute = 0x00cd /* U+00CD LATIN CAPITAL LETTER I WITH ACUTE */,

        /// <summary>
        /// The icircumflex
        /// </summary>
        Icircumflex = 0x00ce /* U+00CE LATIN CAPITAL LETTER I WITH CIRCUMFLEX */,

        /// <summary>
        /// The idiaeresis
        /// </summary>
        Idiaeresis = 0x00cf /* U+00CF LATIN CAPITAL LETTER I WITH DIAERESIS */,

        /// <summary>
        /// The eth
        /// </summary>
        ETH = 0x00d0 /* U+00D0 LATIN CAPITAL LETTER ETH */,

        /// <summary>
        /// The eth
        /// </summary>
        Eth = 0x00d0 /* deprecated */,

        /// <summary>
        /// The ntilde
        /// </summary>
        Ntilde = 0x00d1 /* U+00D1 LATIN CAPITAL LETTER N WITH TILDE */,

        /// <summary>
        /// The ograve
        /// </summary>
        Ograve = 0x00d2 /* U+00D2 LATIN CAPITAL LETTER O WITH GRAVE */,

        /// <summary>
        /// The oacute
        /// </summary>
        Oacute = 0x00d3 /* U+00D3 LATIN CAPITAL LETTER O WITH ACUTE */,

        /// <summary>
        /// The ocircumflex
        /// </summary>
        Ocircumflex = 0x00d4 /* U+00D4 LATIN CAPITAL LETTER O WITH CIRCUMFLEX */,

        /// <summary>
        /// The otilde
        /// </summary>
        Otilde = 0x00d5 /* U+00D5 LATIN CAPITAL LETTER O WITH TILDE */,

        /// <summary>
        /// The odiaeresis
        /// </summary>
        Odiaeresis = 0x00d6 /* U+00D6 LATIN CAPITAL LETTER O WITH DIAERESIS */,

        /// <summary>
        /// The multiply
        /// </summary>
        multiply = 0x00d7 /* U+00D7 MULTIPLICATION SIGN */,

        /// <summary>
        /// The oslash
        /// </summary>
        Oslash = 0x00d8 /* U+00D8 LATIN CAPITAL LETTER O WITH STROKE */,

        /// <summary>
        /// The ooblique
        /// </summary>
        Ooblique = 0x00d8 /* U+00D8 LATIN CAPITAL LETTER O WITH STROKE */,

        /// <summary>
        /// The ugrave
        /// </summary>
        Ugrave = 0x00d9 /* U+00D9 LATIN CAPITAL LETTER U WITH GRAVE */,

        /// <summary>
        /// The uacute
        /// </summary>
        Uacute = 0x00da /* U+00DA LATIN CAPITAL LETTER U WITH ACUTE */,

        /// <summary>
        /// The ucircumflex
        /// </summary>
        Ucircumflex = 0x00db /* U+00DB LATIN CAPITAL LETTER U WITH CIRCUMFLEX */,

        /// <summary>
        /// The udiaeresis
        /// </summary>
        Udiaeresis = 0x00dc /* U+00DC LATIN CAPITAL LETTER U WITH DIAERESIS */,

        /// <summary>
        /// The yacute
        /// </summary>
        Yacute = 0x00dd /* U+00DD LATIN CAPITAL LETTER Y WITH ACUTE */,

        /// <summary>
        /// The thorn
        /// </summary>
        THORN = 0x00de /* U+00DE LATIN CAPITAL LETTER THORN */,

        /// <summary>
        /// The thorn
        /// </summary>
        Thorn = 0x00de /* deprecated */,

        /// <summary>
        /// The ssharp
        /// </summary>
        ssharp = 0x00df /* U+00DF LATIN SMALL LETTER SHARP S */,

        /// <summary>
        /// The agrave
        /// </summary>
        agrave = 0x00e0 /* U+00E0 LATIN SMALL LETTER A WITH GRAVE */,

        /// <summary>
        /// The aacute
        /// </summary>
        aacute = 0x00e1 /* U+00E1 LATIN SMALL LETTER A WITH ACUTE */,

        /// <summary>
        /// The acircumflex
        /// </summary>
        acircumflex = 0x00e2 /* U+00E2 LATIN SMALL LETTER A WITH CIRCUMFLEX */,

        /// <summary>
        /// The atilde
        /// </summary>
        atilde = 0x00e3 /* U+00E3 LATIN SMALL LETTER A WITH TILDE */,

        /// <summary>
        /// The adiaeresis
        /// </summary>
        adiaeresis = 0x00e4 /* U+00E4 LATIN SMALL LETTER A WITH DIAERESIS */,

        /// <summary>
        /// The aring
        /// </summary>
        aring = 0x00e5 /* U+00E5 LATIN SMALL LETTER A WITH RING ABOVE */,

        /// <summary>
        /// The ae
        /// </summary>
        ae = 0x00e6 /* U+00E6 LATIN SMALL LETTER AE */,

        /// <summary>
        /// The ccedilla
        /// </summary>
        ccedilla = 0x00e7 /* U+00E7 LATIN SMALL LETTER C WITH CEDILLA */,

        /// <summary>
        /// The egrave
        /// </summary>
        egrave = 0x00e8 /* U+00E8 LATIN SMALL LETTER E WITH GRAVE */,

        /// <summary>
        /// The eacute
        /// </summary>
        eacute = 0x00e9 /* U+00E9 LATIN SMALL LETTER E WITH ACUTE */,

        /// <summary>
        /// The ecircumflex
        /// </summary>
        ecircumflex = 0x00ea /* U+00EA LATIN SMALL LETTER E WITH CIRCUMFLEX */,

        /// <summary>
        /// The ediaeresis
        /// </summary>
        ediaeresis = 0x00eb /* U+00EB LATIN SMALL LETTER E WITH DIAERESIS */,

        /// <summary>
        /// The igrave
        /// </summary>
        igrave = 0x00ec /* U+00EC LATIN SMALL LETTER I WITH GRAVE */,

        /// <summary>
        /// The iacute
        /// </summary>
        iacute = 0x00ed /* U+00ED LATIN SMALL LETTER I WITH ACUTE */,

        /// <summary>
        /// The icircumflex
        /// </summary>
        icircumflex = 0x00ee /* U+00EE LATIN SMALL LETTER I WITH CIRCUMFLEX */,

        /// <summary>
        /// The idiaeresis
        /// </summary>
        idiaeresis = 0x00ef /* U+00EF LATIN SMALL LETTER I WITH DIAERESIS */,

        /// <summary>
        /// The eth
        /// </summary>
        eth = 0x00f0 /* U+00F0 LATIN SMALL LETTER ETH */,

        /// <summary>
        /// The ntilde
        /// </summary>
        ntilde = 0x00f1 /* U+00F1 LATIN SMALL LETTER N WITH TILDE */,

        /// <summary>
        /// The ograve
        /// </summary>
        ograve = 0x00f2 /* U+00F2 LATIN SMALL LETTER O WITH GRAVE */,

        /// <summary>
        /// The oacute
        /// </summary>
        oacute = 0x00f3 /* U+00F3 LATIN SMALL LETTER O WITH ACUTE */,

        /// <summary>
        /// The ocircumflex
        /// </summary>
        ocircumflex = 0x00f4 /* U+00F4 LATIN SMALL LETTER O WITH CIRCUMFLEX */,

        /// <summary>
        /// The otilde
        /// </summary>
        otilde = 0x00f5 /* U+00F5 LATIN SMALL LETTER O WITH TILDE */,

        /// <summary>
        /// The odiaeresis
        /// </summary>
        odiaeresis = 0x00f6 /* U+00F6 LATIN SMALL LETTER O WITH DIAERESIS */,

        /// <summary>
        /// The division
        /// </summary>
        division = 0x00f7 /* U+00F7 DIVISION SIGN */,

        /// <summary>
        /// The oslash
        /// </summary>
        oslash = 0x00f8 /* U+00F8 LATIN SMALL LETTER O WITH STROKE */,

        /// <summary>
        /// The ooblique
        /// </summary>
        ooblique = 0x00f8 /* U+00F8 LATIN SMALL LETTER O WITH STROKE */,

        /// <summary>
        /// The ugrave
        /// </summary>
        ugrave = 0x00f9 /* U+00F9 LATIN SMALL LETTER U WITH GRAVE */,

        /// <summary>
        /// The uacute
        /// </summary>
        uacute = 0x00fa /* U+00FA LATIN SMALL LETTER U WITH ACUTE */,

        /// <summary>
        /// The ucircumflex
        /// </summary>
        ucircumflex = 0x00fb /* U+00FB LATIN SMALL LETTER U WITH CIRCUMFLEX */,

        /// <summary>
        /// The udiaeresis
        /// </summary>
        udiaeresis = 0x00fc /* U+00FC LATIN SMALL LETTER U WITH DIAERESIS */,

        /// <summary>
        /// The yacute
        /// </summary>
        yacute = 0x00fd /* U+00FD LATIN SMALL LETTER Y WITH ACUTE */,

        /// <summary>
        /// The thorn
        /// </summary>
        thorn = 0x00fe /* U+00FE LATIN SMALL LETTER THORN */,

        /// <summary>
        /// The ydiaeresis
        /// </summary>
        ydiaeresis = 0x00ff /* U+00FF LATIN SMALL LETTER Y WITH DIAERESIS */,

        /// <summary>
        /// The aogonek
        /// </summary>
        Aogonek = 0x01a1 /* U+0104 LATIN CAPITAL LETTER A WITH OGONEK */,

        /// <summary>
        /// The breve
        /// </summary>
        breve = 0x01a2 /* U+02D8 BREVE */,

        /// <summary>
        /// The lstroke
        /// </summary>
        Lstroke = 0x01a3 /* U+0141 LATIN CAPITAL LETTER L WITH STROKE */,

        /// <summary>
        /// The lcaron
        /// </summary>
        Lcaron = 0x01a5 /* U+013D LATIN CAPITAL LETTER L WITH CARON */,

        /// <summary>
        /// The sacute
        /// </summary>
        Sacute = 0x01a6 /* U+015A LATIN CAPITAL LETTER S WITH ACUTE */,

        /// <summary>
        /// The scaron
        /// </summary>
        Scaron = 0x01a9 /* U+0160 LATIN CAPITAL LETTER S WITH CARON */,

        /// <summary>
        /// The scedilla
        /// </summary>
        Scedilla = 0x01aa /* U+015E LATIN CAPITAL LETTER S WITH CEDILLA */,

        /// <summary>
        /// The tcaron
        /// </summary>
        Tcaron = 0x01ab /* U+0164 LATIN CAPITAL LETTER T WITH CARON */,

        /// <summary>
        /// The zacute
        /// </summary>
        Zacute = 0x01ac /* U+0179 LATIN CAPITAL LETTER Z WITH ACUTE */,

        /// <summary>
        /// The zcaron
        /// </summary>
        Zcaron = 0x01ae /* U+017D LATIN CAPITAL LETTER Z WITH CARON */,

        /// <summary>
        /// The zabovedot
        /// </summary>
        Zabovedot = 0x01af /* U+017B LATIN CAPITAL LETTER Z WITH DOT ABOVE */,

        /// <summary>
        /// The aogonek
        /// </summary>
        aogonek = 0x01b1 /* U+0105 LATIN SMALL LETTER A WITH OGONEK */,

        /// <summary>
        /// The ogonek
        /// </summary>
        ogonek = 0x01b2 /* U+02DB OGONEK */,

        /// <summary>
        /// The lstroke
        /// </summary>
        lstroke = 0x01b3 /* U+0142 LATIN SMALL LETTER L WITH STROKE */,

        /// <summary>
        /// The lcaron
        /// </summary>
        lcaron = 0x01b5 /* U+013E LATIN SMALL LETTER L WITH CARON */,

        /// <summary>
        /// The sacute
        /// </summary>
        sacute = 0x01b6 /* U+015B LATIN SMALL LETTER S WITH ACUTE */,

        /// <summary>
        /// The caron
        /// </summary>
        caron = 0x01b7 /* U+02C7 CARON */,

        /// <summary>
        /// The scaron
        /// </summary>
        scaron = 0x01b9 /* U+0161 LATIN SMALL LETTER S WITH CARON */,

        /// <summary>
        /// The scedilla
        /// </summary>
        scedilla = 0x01ba /* U+015F LATIN SMALL LETTER S WITH CEDILLA */,

        /// <summary>
        /// The tcaron
        /// </summary>
        tcaron = 0x01bb /* U+0165 LATIN SMALL LETTER T WITH CARON */,

        /// <summary>
        /// The zacute
        /// </summary>
        zacute = 0x01bc /* U+017A LATIN SMALL LETTER Z WITH ACUTE */,

        /// <summary>
        /// The doubleacute
        /// </summary>
        doubleacute = 0x01bd /* U+02DD DOUBLE ACUTE ACCENT */,

        /// <summary>
        /// The zcaron
        /// </summary>
        zcaron = 0x01be /* U+017E LATIN SMALL LETTER Z WITH CARON */,

        /// <summary>
        /// The zabovedot
        /// </summary>
        zabovedot = 0x01bf /* U+017C LATIN SMALL LETTER Z WITH DOT ABOVE */,

        /// <summary>
        /// The racute
        /// </summary>
        Racute = 0x01c0 /* U+0154 LATIN CAPITAL LETTER R WITH ACUTE */,

        /// <summary>
        /// The abreve
        /// </summary>
        Abreve = 0x01c3 /* U+0102 LATIN CAPITAL LETTER A WITH BREVE */,

        /// <summary>
        /// The lacute
        /// </summary>
        Lacute = 0x01c5 /* U+0139 LATIN CAPITAL LETTER L WITH ACUTE */,

        /// <summary>
        /// The cacute
        /// </summary>
        Cacute = 0x01c6 /* U+0106 LATIN CAPITAL LETTER C WITH ACUTE */,

        /// <summary>
        /// The ccaron
        /// </summary>
        Ccaron = 0x01c8 /* U+010C LATIN CAPITAL LETTER C WITH CARON */,

        /// <summary>
        /// The eogonek
        /// </summary>
        Eogonek = 0x01ca /* U+0118 LATIN CAPITAL LETTER E WITH OGONEK */,

        /// <summary>
        /// The ecaron
        /// </summary>
        Ecaron = 0x01cc /* U+011A LATIN CAPITAL LETTER E WITH CARON */,

        /// <summary>
        /// The dcaron
        /// </summary>
        Dcaron = 0x01cf /* U+010E LATIN CAPITAL LETTER D WITH CARON */,

        /// <summary>
        /// The dstroke
        /// </summary>
        Dstroke = 0x01d0 /* U+0110 LATIN CAPITAL LETTER D WITH STROKE */,

        /// <summary>
        /// The nacute
        /// </summary>
        Nacute = 0x01d1 /* U+0143 LATIN CAPITAL LETTER N WITH ACUTE */,

        /// <summary>
        /// The ncaron
        /// </summary>
        Ncaron = 0x01d2 /* U+0147 LATIN CAPITAL LETTER N WITH CARON */,

        /// <summary>
        /// The odoubleacute
        /// </summary>
        Odoubleacute = 0x01d5 /* U+0150 LATIN CAPITAL LETTER O WITH DOUBLE ACUTE */,

        /// <summary>
        /// The rcaron
        /// </summary>
        Rcaron = 0x01d8 /* U+0158 LATIN CAPITAL LETTER R WITH CARON */,

        /// <summary>
        /// The uring
        /// </summary>
        Uring = 0x01d9 /* U+016E LATIN CAPITAL LETTER U WITH RING ABOVE */,

        /// <summary>
        /// The udoubleacute
        /// </summary>
        Udoubleacute = 0x01db /* U+0170 LATIN CAPITAL LETTER U WITH DOUBLE ACUTE */,

        /// <summary>
        /// The tcedilla
        /// </summary>
        Tcedilla = 0x01de /* U+0162 LATIN CAPITAL LETTER T WITH CEDILLA */,

        /// <summary>
        /// The racute
        /// </summary>
        racute = 0x01e0 /* U+0155 LATIN SMALL LETTER R WITH ACUTE */,

        /// <summary>
        /// The abreve
        /// </summary>
        abreve = 0x01e3 /* U+0103 LATIN SMALL LETTER A WITH BREVE */,

        /// <summary>
        /// The lacute
        /// </summary>
        lacute = 0x01e5 /* U+013A LATIN SMALL LETTER L WITH ACUTE */,

        /// <summary>
        /// The cacute
        /// </summary>
        cacute = 0x01e6 /* U+0107 LATIN SMALL LETTER C WITH ACUTE */,

        /// <summary>
        /// The ccaron
        /// </summary>
        ccaron = 0x01e8 /* U+010D LATIN SMALL LETTER C WITH CARON */,

        /// <summary>
        /// The eogonek
        /// </summary>
        eogonek = 0x01ea /* U+0119 LATIN SMALL LETTER E WITH OGONEK */,

        /// <summary>
        /// The ecaron
        /// </summary>
        ecaron = 0x01ec /* U+011B LATIN SMALL LETTER E WITH CARON */,

        /// <summary>
        /// The dcaron
        /// </summary>
        dcaron = 0x01ef /* U+010F LATIN SMALL LETTER D WITH CARON */,

        /// <summary>
        /// The dstroke
        /// </summary>
        dstroke = 0x01f0 /* U+0111 LATIN SMALL LETTER D WITH STROKE */,

        /// <summary>
        /// The nacute
        /// </summary>
        nacute = 0x01f1 /* U+0144 LATIN SMALL LETTER N WITH ACUTE */,

        /// <summary>
        /// The ncaron
        /// </summary>
        ncaron = 0x01f2 /* U+0148 LATIN SMALL LETTER N WITH CARON */,

        /// <summary>
        /// The odoubleacute
        /// </summary>
        odoubleacute = 0x01f5 /* U+0151 LATIN SMALL LETTER O WITH DOUBLE ACUTE */,

        /// <summary>
        /// The rcaron
        /// </summary>
        rcaron = 0x01f8 /* U+0159 LATIN SMALL LETTER R WITH CARON */,

        /// <summary>
        /// The uring
        /// </summary>
        uring = 0x01f9 /* U+016F LATIN SMALL LETTER U WITH RING ABOVE */,

        /// <summary>
        /// The udoubleacute
        /// </summary>
        udoubleacute = 0x01fb /* U+0171 LATIN SMALL LETTER U WITH DOUBLE ACUTE */,

        /// <summary>
        /// The tcedilla
        /// </summary>
        tcedilla = 0x01fe /* U+0163 LATIN SMALL LETTER T WITH CEDILLA */,

        /// <summary>
        /// The abovedot
        /// </summary>
        abovedot = 0x01ff /* U+02D9 DOT ABOVE */,

        /// <summary>
        /// The hstroke
        /// </summary>
        Hstroke = 0x02a1 /* U+0126 LATIN CAPITAL LETTER H WITH STROKE */,

        /// <summary>
        /// The hcircumflex
        /// </summary>
        Hcircumflex = 0x02a6 /* U+0124 LATIN CAPITAL LETTER H WITH CIRCUMFLEX */,

        /// <summary>
        /// The iabovedot
        /// </summary>
        Iabovedot = 0x02a9 /* U+0130 LATIN CAPITAL LETTER I WITH DOT ABOVE */,

        /// <summary>
        /// The gbreve
        /// </summary>
        Gbreve = 0x02ab /* U+011E LATIN CAPITAL LETTER G WITH BREVE */,

        /// <summary>
        /// The jcircumflex
        /// </summary>
        Jcircumflex = 0x02ac /* U+0134 LATIN CAPITAL LETTER J WITH CIRCUMFLEX */,

        /// <summary>
        /// The hstroke
        /// </summary>
        hstroke = 0x02b1 /* U+0127 LATIN SMALL LETTER H WITH STROKE */,

        /// <summary>
        /// The hcircumflex
        /// </summary>
        hcircumflex = 0x02b6 /* U+0125 LATIN SMALL LETTER H WITH CIRCUMFLEX */,

        /// <summary>
        /// The idotless
        /// </summary>
        idotless = 0x02b9 /* U+0131 LATIN SMALL LETTER DOTLESS I */,

        /// <summary>
        /// The gbreve
        /// </summary>
        gbreve = 0x02bb /* U+011F LATIN SMALL LETTER G WITH BREVE */,

        /// <summary>
        /// The jcircumflex
        /// </summary>
        jcircumflex = 0x02bc /* U+0135 LATIN SMALL LETTER J WITH CIRCUMFLEX */,

        /// <summary>
        /// The cabovedot
        /// </summary>
        Cabovedot = 0x02c5 /* U+010A LATIN CAPITAL LETTER C WITH DOT ABOVE */,

        /// <summary>
        /// The ccircumflex
        /// </summary>
        Ccircumflex = 0x02c6 /* U+0108 LATIN CAPITAL LETTER C WITH CIRCUMFLEX */,

        /// <summary>
        /// The gabovedot
        /// </summary>
        Gabovedot = 0x02d5 /* U+0120 LATIN CAPITAL LETTER G WITH DOT ABOVE */,

        /// <summary>
        /// The gcircumflex
        /// </summary>
        Gcircumflex = 0x02d8 /* U+011C LATIN CAPITAL LETTER G WITH CIRCUMFLEX */,

        /// <summary>
        /// The ubreve
        /// </summary>
        Ubreve = 0x02dd /* U+016C LATIN CAPITAL LETTER U WITH BREVE */,

        /// <summary>
        /// The scircumflex
        /// </summary>
        Scircumflex = 0x02de /* U+015C LATIN CAPITAL LETTER S WITH CIRCUMFLEX */,

        /// <summary>
        /// The cabovedot
        /// </summary>
        cabovedot = 0x02e5 /* U+010B LATIN SMALL LETTER C WITH DOT ABOVE */,

        /// <summary>
        /// The ccircumflex
        /// </summary>
        ccircumflex = 0x02e6 /* U+0109 LATIN SMALL LETTER C WITH CIRCUMFLEX */,

        /// <summary>
        /// The gabovedot
        /// </summary>
        gabovedot = 0x02f5 /* U+0121 LATIN SMALL LETTER G WITH DOT ABOVE */,

        /// <summary>
        /// The gcircumflex
        /// </summary>
        gcircumflex = 0x02f8 /* U+011D LATIN SMALL LETTER G WITH CIRCUMFLEX */,

        /// <summary>
        /// The ubreve
        /// </summary>
        ubreve = 0x02fd /* U+016D LATIN SMALL LETTER U WITH BREVE */,

        /// <summary>
        /// The scircumflex
        /// </summary>
        scircumflex = 0x02fe /* U+015D LATIN SMALL LETTER S WITH CIRCUMFLEX */,

        /// <summary>
        /// The kra
        /// </summary>
        kra = 0x03a2 /* U+0138 LATIN SMALL LETTER KRA */,

        /// <summary>
        /// The kappa
        /// </summary>
        kappa = 0x03a2 /* deprecated */,

        /// <summary>
        /// The rcedilla
        /// </summary>
        Rcedilla = 0x03a3 /* U+0156 LATIN CAPITAL LETTER R WITH CEDILLA */,

        /// <summary>
        /// The itilde
        /// </summary>
        Itilde = 0x03a5 /* U+0128 LATIN CAPITAL LETTER I WITH TILDE */,

        /// <summary>
        /// The lcedilla
        /// </summary>
        Lcedilla = 0x03a6 /* U+013B LATIN CAPITAL LETTER L WITH CEDILLA */,

        /// <summary>
        /// The emacron
        /// </summary>
        Emacron = 0x03aa /* U+0112 LATIN CAPITAL LETTER E WITH MACRON */,

        /// <summary>
        /// The gcedilla
        /// </summary>
        Gcedilla = 0x03ab /* U+0122 LATIN CAPITAL LETTER G WITH CEDILLA */,

        /// <summary>
        /// The tslash
        /// </summary>
        Tslash = 0x03ac /* U+0166 LATIN CAPITAL LETTER T WITH STROKE */,

        /// <summary>
        /// The rcedilla
        /// </summary>
        rcedilla = 0x03b3 /* U+0157 LATIN SMALL LETTER R WITH CEDILLA */,

        /// <summary>
        /// The itilde
        /// </summary>
        itilde = 0x03b5 /* U+0129 LATIN SMALL LETTER I WITH TILDE */,

        /// <summary>
        /// The lcedilla
        /// </summary>
        lcedilla = 0x03b6 /* U+013C LATIN SMALL LETTER L WITH CEDILLA */,

        /// <summary>
        /// The emacron
        /// </summary>
        emacron = 0x03ba /* U+0113 LATIN SMALL LETTER E WITH MACRON */,

        /// <summary>
        /// The gcedilla
        /// </summary>
        gcedilla = 0x03bb /* U+0123 LATIN SMALL LETTER G WITH CEDILLA */,

        /// <summary>
        /// The tslash
        /// </summary>
        tslash = 0x03bc /* U+0167 LATIN SMALL LETTER T WITH STROKE */,

        /// <summary>
        /// The eng
        /// </summary>
        ENG = 0x03bd /* U+014A LATIN CAPITAL LETTER ENG */,

        /// <summary>
        /// The eng
        /// </summary>
        eng = 0x03bf /* U+014B LATIN SMALL LETTER ENG */,

        /// <summary>
        /// The amacron
        /// </summary>
        Amacron = 0x03c0 /* U+0100 LATIN CAPITAL LETTER A WITH MACRON */,

        /// <summary>
        /// The iogonek
        /// </summary>
        Iogonek = 0x03c7 /* U+012E LATIN CAPITAL LETTER I WITH OGONEK */,

        /// <summary>
        /// The eabovedot
        /// </summary>
        Eabovedot = 0x03cc /* U+0116 LATIN CAPITAL LETTER E WITH DOT ABOVE */,

        /// <summary>
        /// The imacron
        /// </summary>
        Imacron = 0x03cf /* U+012A LATIN CAPITAL LETTER I WITH MACRON */,

        /// <summary>
        /// The ncedilla
        /// </summary>
        Ncedilla = 0x03d1 /* U+0145 LATIN CAPITAL LETTER N WITH CEDILLA */,

        /// <summary>
        /// The omacron
        /// </summary>
        Omacron = 0x03d2 /* U+014C LATIN CAPITAL LETTER O WITH MACRON */,

        /// <summary>
        /// The kcedilla
        /// </summary>
        Kcedilla = 0x03d3 /* U+0136 LATIN CAPITAL LETTER K WITH CEDILLA */,

        /// <summary>
        /// The uogonek
        /// </summary>
        Uogonek = 0x03d9 /* U+0172 LATIN CAPITAL LETTER U WITH OGONEK */,

        /// <summary>
        /// The utilde
        /// </summary>
        Utilde = 0x03dd /* U+0168 LATIN CAPITAL LETTER U WITH TILDE */,

        /// <summary>
        /// The umacron
        /// </summary>
        Umacron = 0x03de /* U+016A LATIN CAPITAL LETTER U WITH MACRON */,

        /// <summary>
        /// The amacron
        /// </summary>
        amacron = 0x03e0 /* U+0101 LATIN SMALL LETTER A WITH MACRON */,

        /// <summary>
        /// The iogonek
        /// </summary>
        iogonek = 0x03e7 /* U+012F LATIN SMALL LETTER I WITH OGONEK */,

        /// <summary>
        /// The eabovedot
        /// </summary>
        eabovedot = 0x03ec /* U+0117 LATIN SMALL LETTER E WITH DOT ABOVE */,

        /// <summary>
        /// The imacron
        /// </summary>
        imacron = 0x03ef /* U+012B LATIN SMALL LETTER I WITH MACRON */,

        /// <summary>
        /// The ncedilla
        /// </summary>
        ncedilla = 0x03f1 /* U+0146 LATIN SMALL LETTER N WITH CEDILLA */,

        /// <summary>
        /// The omacron
        /// </summary>
        omacron = 0x03f2 /* U+014D LATIN SMALL LETTER O WITH MACRON */,

        /// <summary>
        /// The kcedilla
        /// </summary>
        kcedilla = 0x03f3 /* U+0137 LATIN SMALL LETTER K WITH CEDILLA */,

        /// <summary>
        /// The uogonek
        /// </summary>
        uogonek = 0x03f9 /* U+0173 LATIN SMALL LETTER U WITH OGONEK */,

        /// <summary>
        /// The utilde
        /// </summary>
        utilde = 0x03fd /* U+0169 LATIN SMALL LETTER U WITH TILDE */,

        /// <summary>
        /// The umacron
        /// </summary>
        umacron = 0x03fe /* U+016B LATIN SMALL LETTER U WITH MACRON */,

        /// <summary>
        /// The wcircumflex
        /// </summary>
        Wcircumflex = 0x1000174 /* U+0174 LATIN CAPITAL LETTER W WITH CIRCUMFLEX */,

        /// <summary>
        /// The wcircumflex
        /// </summary>
        wcircumflex = 0x1000175 /* U+0175 LATIN SMALL LETTER W WITH CIRCUMFLEX */,

        /// <summary>
        /// The ycircumflex
        /// </summary>
        Ycircumflex = 0x1000176 /* U+0176 LATIN CAPITAL LETTER Y WITH CIRCUMFLEX */,

        /// <summary>
        /// The ycircumflex
        /// </summary>
        ycircumflex = 0x1000177 /* U+0177 LATIN SMALL LETTER Y WITH CIRCUMFLEX */,

        /// <summary>
        /// The babovedot
        /// </summary>
        Babovedot = 0x1001e02 /* U+1E02 LATIN CAPITAL LETTER B WITH DOT ABOVE */,

        /// <summary>
        /// The babovedot
        /// </summary>
        babovedot = 0x1001e03 /* U+1E03 LATIN SMALL LETTER B WITH DOT ABOVE */,

        /// <summary>
        /// The dabovedot
        /// </summary>
        Dabovedot = 0x1001e0a /* U+1E0A LATIN CAPITAL LETTER D WITH DOT ABOVE */,

        /// <summary>
        /// The dabovedot
        /// </summary>
        dabovedot = 0x1001e0b /* U+1E0B LATIN SMALL LETTER D WITH DOT ABOVE */,

        /// <summary>
        /// The fabovedot
        /// </summary>
        Fabovedot = 0x1001e1e /* U+1E1E LATIN CAPITAL LETTER F WITH DOT ABOVE */,

        /// <summary>
        /// The fabovedot
        /// </summary>
        fabovedot = 0x1001e1f /* U+1E1F LATIN SMALL LETTER F WITH DOT ABOVE */,

        /// <summary>
        /// The mabovedot
        /// </summary>
        Mabovedot = 0x1001e40 /* U+1E40 LATIN CAPITAL LETTER M WITH DOT ABOVE */,

        /// <summary>
        /// The mabovedot
        /// </summary>
        mabovedot = 0x1001e41 /* U+1E41 LATIN SMALL LETTER M WITH DOT ABOVE */,

        /// <summary>
        /// The pabovedot
        /// </summary>
        Pabovedot = 0x1001e56 /* U+1E56 LATIN CAPITAL LETTER P WITH DOT ABOVE */,

        /// <summary>
        /// The pabovedot
        /// </summary>
        pabovedot = 0x1001e57 /* U+1E57 LATIN SMALL LETTER P WITH DOT ABOVE */,

        /// <summary>
        /// The sabovedot
        /// </summary>
        Sabovedot = 0x1001e60 /* U+1E60 LATIN CAPITAL LETTER S WITH DOT ABOVE */,

        /// <summary>
        /// The sabovedot
        /// </summary>
        sabovedot = 0x1001e61 /* U+1E61 LATIN SMALL LETTER S WITH DOT ABOVE */,

        /// <summary>
        /// The tabovedot
        /// </summary>
        Tabovedot = 0x1001e6a /* U+1E6A LATIN CAPITAL LETTER T WITH DOT ABOVE */,

        /// <summary>
        /// The tabovedot
        /// </summary>
        tabovedot = 0x1001e6b /* U+1E6B LATIN SMALL LETTER T WITH DOT ABOVE */,

        /// <summary>
        /// The wgrave
        /// </summary>
        Wgrave = 0x1001e80 /* U+1E80 LATIN CAPITAL LETTER W WITH GRAVE */,

        /// <summary>
        /// The wgrave
        /// </summary>
        wgrave = 0x1001e81 /* U+1E81 LATIN SMALL LETTER W WITH GRAVE */,

        /// <summary>
        /// The wacute
        /// </summary>
        Wacute = 0x1001e82 /* U+1E82 LATIN CAPITAL LETTER W WITH ACUTE */,

        /// <summary>
        /// The wacute
        /// </summary>
        wacute = 0x1001e83 /* U+1E83 LATIN SMALL LETTER W WITH ACUTE */,

        /// <summary>
        /// The wdiaeresis
        /// </summary>
        Wdiaeresis = 0x1001e84 /* U+1E84 LATIN CAPITAL LETTER W WITH DIAERESIS */,

        /// <summary>
        /// The wdiaeresis
        /// </summary>
        wdiaeresis = 0x1001e85 /* U+1E85 LATIN SMALL LETTER W WITH DIAERESIS */,

        /// <summary>
        /// The ygrave
        /// </summary>
        Ygrave = 0x1001ef2 /* U+1EF2 LATIN CAPITAL LETTER Y WITH GRAVE */,

        /// <summary>
        /// The ygrave
        /// </summary>
        ygrave = 0x1001ef3 /* U+1EF3 LATIN SMALL LETTER Y WITH GRAVE */,

        /// <summary>
        /// The oe
        /// </summary>
        OE = 0x13bc /* U+0152 LATIN CAPITAL LIGATURE OE */,

        /// <summary>
        /// The oe
        /// </summary>
        oe = 0x13bd /* U+0153 LATIN SMALL LIGATURE OE */,

        /// <summary>
        /// The ydiaeresis
        /// </summary>
        Ydiaeresis = 0x13be /* U+0178 LATIN CAPITAL LETTER Y WITH DIAERESIS */,

        /// <summary>
        /// The overline
        /// </summary>
        overline = 0x047e /* U+203E OVERLINE */,

        /// <summary>
        /// The kana fullstop
        /// </summary>
        kana_fullstop = 0x04a1 /* U+3002 IDEOGRAPHIC FULL STOP */,

        /// <summary>
        /// The kana openingbracket
        /// </summary>
        kana_openingbracket = 0x04a2 /* U+300C LEFT CORNER BRACKET */,

        /// <summary>
        /// The kana closingbracket
        /// </summary>
        kana_closingbracket = 0x04a3 /* U+300D RIGHT CORNER BRACKET */,

        /// <summary>
        /// The kana comma
        /// </summary>
        kana_comma = 0x04a4 /* U+3001 IDEOGRAPHIC COMMA */,

        /// <summary>
        /// The kana conjunctive
        /// </summary>
        kana_conjunctive = 0x04a5 /* U+30FB KATAKANA MIDDLE DOT */,

        /// <summary>
        /// The kana middledot
        /// </summary>
        kana_middledot = 0x04a5 /* deprecated */,

        /// <summary>
        /// The kana wo
        /// </summary>
        kana_WO = 0x04a6 /* U+30F2 KATAKANA LETTER WO */,

        /// <summary>
        /// The kana a
        /// </summary>
        kana_a = 0x04a7 /* U+30A1 KATAKANA LETTER SMALL A */,

        /// <summary>
        /// The kana i
        /// </summary>
        kana_i = 0x04a8 /* U+30A3 KATAKANA LETTER SMALL I */,

        /// <summary>
        /// The kana u
        /// </summary>
        kana_u = 0x04a9 /* U+30A5 KATAKANA LETTER SMALL U */,

        /// <summary>
        /// The kana e
        /// </summary>
        kana_e = 0x04aa /* U+30A7 KATAKANA LETTER SMALL E */,

        /// <summary>
        /// The kana o
        /// </summary>
        kana_o = 0x04ab /* U+30A9 KATAKANA LETTER SMALL O */,

        /// <summary>
        /// The kana ya
        /// </summary>
        kana_ya = 0x04ac /* U+30E3 KATAKANA LETTER SMALL YA */,

        /// <summary>
        /// The kana yu
        /// </summary>
        kana_yu = 0x04ad /* U+30E5 KATAKANA LETTER SMALL YU */,

        /// <summary>
        /// The kana yo
        /// </summary>
        kana_yo = 0x04ae /* U+30E7 KATAKANA LETTER SMALL YO */,

        /// <summary>
        /// The kana tsu
        /// </summary>
        kana_tsu = 0x04af /* U+30C3 KATAKANA LETTER SMALL TU */,

        /// <summary>
        /// The kana tu
        /// </summary>
        kana_tu = 0x04af /* deprecated */,

        /// <summary>
        /// The prolongedsound
        /// </summary>
        prolongedsound = 0x04b0 /* U+30FC KATAKANA-HIRAGANA PROLONGED SOUND MARK */,

        /// <summary>
        /// The kana a
        /// </summary>
        kana_A = 0x04b1 /* U+30A2 KATAKANA LETTER A */,

        /// <summary>
        /// The kana i
        /// </summary>
        kana_I = 0x04b2 /* U+30A4 KATAKANA LETTER I */,

        /// <summary>
        /// The kana u
        /// </summary>
        kana_U = 0x04b3 /* U+30A6 KATAKANA LETTER U */,

        /// <summary>
        /// The kana e
        /// </summary>
        kana_E = 0x04b4 /* U+30A8 KATAKANA LETTER E */,

        /// <summary>
        /// The kana o
        /// </summary>
        kana_O = 0x04b5 /* U+30AA KATAKANA LETTER O */,

        /// <summary>
        /// The kana ka
        /// </summary>
        kana_KA = 0x04b6 /* U+30AB KATAKANA LETTER KA */,

        /// <summary>
        /// The kana ki
        /// </summary>
        kana_KI = 0x04b7 /* U+30AD KATAKANA LETTER KI */,

        /// <summary>
        /// The kana ku
        /// </summary>
        kana_KU = 0x04b8 /* U+30AF KATAKANA LETTER KU */,

        /// <summary>
        /// The kana ke
        /// </summary>
        kana_KE = 0x04b9 /* U+30B1 KATAKANA LETTER KE */,

        /// <summary>
        /// The kana ko
        /// </summary>
        kana_KO = 0x04ba /* U+30B3 KATAKANA LETTER KO */,

        /// <summary>
        /// The kana sa
        /// </summary>
        kana_SA = 0x04bb /* U+30B5 KATAKANA LETTER SA */,

        /// <summary>
        /// The kana shi
        /// </summary>
        kana_SHI = 0x04bc /* U+30B7 KATAKANA LETTER SI */,

        /// <summary>
        /// The kana su
        /// </summary>
        kana_SU = 0x04bd /* U+30B9 KATAKANA LETTER SU */,

        /// <summary>
        /// The kana se
        /// </summary>
        kana_SE = 0x04be /* U+30BB KATAKANA LETTER SE */,

        /// <summary>
        /// The kana so
        /// </summary>
        kana_SO = 0x04bf /* U+30BD KATAKANA LETTER SO */,

        /// <summary>
        /// The kana ta
        /// </summary>
        kana_TA = 0x04c0 /* U+30BF KATAKANA LETTER TA */,

        /// <summary>
        /// The kana chi
        /// </summary>
        kana_CHI = 0x04c1 /* U+30C1 KATAKANA LETTER TI */,

        /// <summary>
        /// The kana ti
        /// </summary>
        kana_TI = 0x04c1 /* deprecated */,

        /// <summary>
        /// The kana tsu
        /// </summary>
        kana_TSU = 0x04c2 /* U+30C4 KATAKANA LETTER TU */,

        /// <summary>
        /// The kana tu
        /// </summary>
        kana_TU = 0x04c2 /* deprecated */,

        /// <summary>
        /// The kana te
        /// </summary>
        kana_TE = 0x04c3 /* U+30C6 KATAKANA LETTER TE */,

        /// <summary>
        /// The kana to
        /// </summary>
        kana_TO = 0x04c4 /* U+30C8 KATAKANA LETTER TO */,

        /// <summary>
        /// The kana na
        /// </summary>
        kana_NA = 0x04c5 /* U+30CA KATAKANA LETTER NA */,

        /// <summary>
        /// The kana ni
        /// </summary>
        kana_NI = 0x04c6 /* U+30CB KATAKANA LETTER NI */,

        /// <summary>
        /// The kana nu
        /// </summary>
        kana_NU = 0x04c7 /* U+30CC KATAKANA LETTER NU */,

        /// <summary>
        /// The kana ne
        /// </summary>
        kana_NE = 0x04c8 /* U+30CD KATAKANA LETTER NE */,

        /// <summary>
        /// The kana no
        /// </summary>
        kana_NO = 0x04c9 /* U+30CE KATAKANA LETTER NO */,

        /// <summary>
        /// The kana ha
        /// </summary>
        kana_HA = 0x04ca /* U+30CF KATAKANA LETTER HA */,

        /// <summary>
        /// The kana hi
        /// </summary>
        kana_HI = 0x04cb /* U+30D2 KATAKANA LETTER HI */,

        /// <summary>
        /// The kana fu
        /// </summary>
        kana_FU = 0x04cc /* U+30D5 KATAKANA LETTER HU */,

        /// <summary>
        /// The kana hu
        /// </summary>
        kana_HU = 0x04cc /* deprecated */,

        /// <summary>
        /// The kana he
        /// </summary>
        kana_HE = 0x04cd /* U+30D8 KATAKANA LETTER HE */,

        /// <summary>
        /// The kana ho
        /// </summary>
        kana_HO = 0x04ce /* U+30DB KATAKANA LETTER HO */,

        /// <summary>
        /// The kana ma
        /// </summary>
        kana_MA = 0x04cf /* U+30DE KATAKANA LETTER MA */,

        /// <summary>
        /// The kana mi
        /// </summary>
        kana_MI = 0x04d0 /* U+30DF KATAKANA LETTER MI */,

        /// <summary>
        /// The kana mu
        /// </summary>
        kana_MU = 0x04d1 /* U+30E0 KATAKANA LETTER MU */,

        /// <summary>
        /// The kana me
        /// </summary>
        kana_ME = 0x04d2 /* U+30E1 KATAKANA LETTER ME */,

        /// <summary>
        /// The kana mo
        /// </summary>
        kana_MO = 0x04d3 /* U+30E2 KATAKANA LETTER MO */,

        /// <summary>
        /// The kana ya
        /// </summary>
        kana_YA = 0x04d4 /* U+30E4 KATAKANA LETTER YA */,

        /// <summary>
        /// The kana yu
        /// </summary>
        kana_YU = 0x04d5 /* U+30E6 KATAKANA LETTER YU */,

        /// <summary>
        /// The kana yo
        /// </summary>
        kana_YO = 0x04d6 /* U+30E8 KATAKANA LETTER YO */,

        /// <summary>
        /// The kana ra
        /// </summary>
        kana_RA = 0x04d7 /* U+30E9 KATAKANA LETTER RA */,

        /// <summary>
        /// The kana ri
        /// </summary>
        kana_RI = 0x04d8 /* U+30EA KATAKANA LETTER RI */,

        /// <summary>
        /// The kana ru
        /// </summary>
        kana_RU = 0x04d9 /* U+30EB KATAKANA LETTER RU */,

        /// <summary>
        /// The kana re
        /// </summary>
        kana_RE = 0x04da /* U+30EC KATAKANA LETTER RE */,

        /// <summary>
        /// The kana ro
        /// </summary>
        kana_RO = 0x04db /* U+30ED KATAKANA LETTER RO */,

        /// <summary>
        /// The kana wa
        /// </summary>
        kana_WA = 0x04dc /* U+30EF KATAKANA LETTER WA */,

        /// <summary>
        /// The kana n
        /// </summary>
        kana_N = 0x04dd /* U+30F3 KATAKANA LETTER N */,

        /// <summary>
        /// The voicedsound
        /// </summary>
        voicedsound = 0x04de /* U+309B KATAKANA-HIRAGANA VOICED SOUND MARK */,

        /// <summary>
        /// The semivoicedsound
        /// </summary>
        semivoicedsound = 0x04df /* U+309C KATAKANA-HIRAGANA SEMI-VOICED SOUND MARK */,

        /// <summary>
        /// The kana switch
        /// </summary>
        kana_switch = 0xff7e /* Alias for mode_switch */,

        /// <summary>
        /// The farsi 0
        /// </summary>
        Farsi_0 = 0x10006f0 /* U+06F0 EXTENDED ARABIC-INDIC DIGIT ZERO */,

        /// <summary>
        /// The farsi 1
        /// </summary>
        Farsi_1 = 0x10006f1 /* U+06F1 EXTENDED ARABIC-INDIC DIGIT ONE */,

        /// <summary>
        /// The farsi 2
        /// </summary>
        Farsi_2 = 0x10006f2 /* U+06F2 EXTENDED ARABIC-INDIC DIGIT TWO */,

        /// <summary>
        /// The farsi 3
        /// </summary>
        Farsi_3 = 0x10006f3 /* U+06F3 EXTENDED ARABIC-INDIC DIGIT THREE */,

        /// <summary>
        /// The farsi 4
        /// </summary>
        Farsi_4 = 0x10006f4 /* U+06F4 EXTENDED ARABIC-INDIC DIGIT FOUR */,

        /// <summary>
        /// The farsi 5
        /// </summary>
        Farsi_5 = 0x10006f5 /* U+06F5 EXTENDED ARABIC-INDIC DIGIT FIVE */,

        /// <summary>
        /// The farsi 6
        /// </summary>
        Farsi_6 = 0x10006f6 /* U+06F6 EXTENDED ARABIC-INDIC DIGIT SIX */,

        /// <summary>
        /// The farsi 7
        /// </summary>
        Farsi_7 = 0x10006f7 /* U+06F7 EXTENDED ARABIC-INDIC DIGIT SEVEN */,

        /// <summary>
        /// The farsi 8
        /// </summary>
        Farsi_8 = 0x10006f8 /* U+06F8 EXTENDED ARABIC-INDIC DIGIT EIGHT */,

        /// <summary>
        /// The farsi 9
        /// </summary>
        Farsi_9 = 0x10006f9 /* U+06F9 EXTENDED ARABIC-INDIC DIGIT NINE */,

        /// <summary>
        /// The arabic percent
        /// </summary>
        Arabic_percent = 0x100066a /* U+066A ARABIC PERCENT SIGN */,

        /// <summary>
        /// The arabic superscript alef
        /// </summary>
        Arabic_superscript_alef = 0x1000670 /* U+0670 ARABIC LETTER SUPERSCRIPT ALEF */,

        /// <summary>
        /// The arabic tteh
        /// </summary>
        Arabic_tteh = 0x1000679 /* U+0679 ARABIC LETTER TTEH */,

        /// <summary>
        /// The arabic peh
        /// </summary>
        Arabic_peh = 0x100067e /* U+067E ARABIC LETTER PEH */,

        /// <summary>
        /// The arabic tcheh
        /// </summary>
        Arabic_tcheh = 0x1000686 /* U+0686 ARABIC LETTER TCHEH */,

        /// <summary>
        /// The arabic ddal
        /// </summary>
        Arabic_ddal = 0x1000688 /* U+0688 ARABIC LETTER DDAL */,

        /// <summary>
        /// The arabic rreh
        /// </summary>
        Arabic_rreh = 0x1000691 /* U+0691 ARABIC LETTER RREH */,

        /// <summary>
        /// The arabic comma
        /// </summary>
        Arabic_comma = 0x05ac /* U+060C ARABIC COMMA */,

        /// <summary>
        /// The arabic fullstop
        /// </summary>
        Arabic_fullstop = 0x10006d4 /* U+06D4 ARABIC FULL STOP */,

        /// <summary>
        /// The arabic 0
        /// </summary>
        Arabic_0 = 0x1000660 /* U+0660 ARABIC-INDIC DIGIT ZERO */,

        /// <summary>
        /// The arabic 1
        /// </summary>
        Arabic_1 = 0x1000661 /* U+0661 ARABIC-INDIC DIGIT ONE */,

        /// <summary>
        /// The arabic 2
        /// </summary>
        Arabic_2 = 0x1000662 /* U+0662 ARABIC-INDIC DIGIT TWO */,

        /// <summary>
        /// The arabic 3
        /// </summary>
        Arabic_3 = 0x1000663 /* U+0663 ARABIC-INDIC DIGIT THREE */,

        /// <summary>
        /// The arabic 4
        /// </summary>
        Arabic_4 = 0x1000664 /* U+0664 ARABIC-INDIC DIGIT FOUR */,

        /// <summary>
        /// The arabic 5
        /// </summary>
        Arabic_5 = 0x1000665 /* U+0665 ARABIC-INDIC DIGIT FIVE */,

        /// <summary>
        /// The arabic 6
        /// </summary>
        Arabic_6 = 0x1000666 /* U+0666 ARABIC-INDIC DIGIT SIX */,

        /// <summary>
        /// The arabic 7
        /// </summary>
        Arabic_7 = 0x1000667 /* U+0667 ARABIC-INDIC DIGIT SEVEN */,

        /// <summary>
        /// The arabic 8
        /// </summary>
        Arabic_8 = 0x1000668 /* U+0668 ARABIC-INDIC DIGIT EIGHT */,

        /// <summary>
        /// The arabic 9
        /// </summary>
        Arabic_9 = 0x1000669 /* U+0669 ARABIC-INDIC DIGIT NINE */,

        /// <summary>
        /// The arabic semicolon
        /// </summary>
        Arabic_semicolon = 0x05bb /* U+061B ARABIC SEMICOLON */,

        /// <summary>
        /// The arabic question mark
        /// </summary>
        Arabic_question_mark = 0x05bf /* U+061F ARABIC QUESTION MARK */,

        /// <summary>
        /// The arabic hamza
        /// </summary>
        Arabic_hamza = 0x05c1 /* U+0621 ARABIC LETTER HAMZA */,

        /// <summary>
        /// The arabic maddaonalef
        /// </summary>
        Arabic_maddaonalef = 0x05c2 /* U+0622 ARABIC LETTER ALEF WITH MADDA ABOVE */,

        /// <summary>
        /// The arabic hamzaonalef
        /// </summary>
        Arabic_hamzaonalef = 0x05c3 /* U+0623 ARABIC LETTER ALEF WITH HAMZA ABOVE */,

        /// <summary>
        /// The arabic hamzaonwaw
        /// </summary>
        Arabic_hamzaonwaw = 0x05c4 /* U+0624 ARABIC LETTER WAW WITH HAMZA ABOVE */,

        /// <summary>
        /// The arabic hamzaunderalef
        /// </summary>
        Arabic_hamzaunderalef = 0x05c5 /* U+0625 ARABIC LETTER ALEF WITH HAMZA BELOW */,

        /// <summary>
        /// The arabic hamzaonyeh
        /// </summary>
        Arabic_hamzaonyeh = 0x05c6 /* U+0626 ARABIC LETTER YEH WITH HAMZA ABOVE */,

        /// <summary>
        /// The arabic alef
        /// </summary>
        Arabic_alef = 0x05c7 /* U+0627 ARABIC LETTER ALEF */,

        /// <summary>
        /// The arabic beh
        /// </summary>
        Arabic_beh = 0x05c8 /* U+0628 ARABIC LETTER BEH */,

        /// <summary>
        /// The arabic tehmarbuta
        /// </summary>
        Arabic_tehmarbuta = 0x05c9 /* U+0629 ARABIC LETTER TEH MARBUTA */,

        /// <summary>
        /// The arabic teh
        /// </summary>
        Arabic_teh = 0x05ca /* U+062A ARABIC LETTER TEH */,

        /// <summary>
        /// The arabic theh
        /// </summary>
        Arabic_theh = 0x05cb /* U+062B ARABIC LETTER THEH */,

        /// <summary>
        /// The arabic jeem
        /// </summary>
        Arabic_jeem = 0x05cc /* U+062C ARABIC LETTER JEEM */,

        /// <summary>
        /// The arabic hah
        /// </summary>
        Arabic_hah = 0x05cd /* U+062D ARABIC LETTER HAH */,

        /// <summary>
        /// The arabic khah
        /// </summary>
        Arabic_khah = 0x05ce /* U+062E ARABIC LETTER KHAH */,

        /// <summary>
        /// The arabic dal
        /// </summary>
        Arabic_dal = 0x05cf /* U+062F ARABIC LETTER DAL */,

        /// <summary>
        /// The arabic thal
        /// </summary>
        Arabic_thal = 0x05d0 /* U+0630 ARABIC LETTER THAL */,

        /// <summary>
        /// The arabic ra
        /// </summary>
        Arabic_ra = 0x05d1 /* U+0631 ARABIC LETTER REH */,

        /// <summary>
        /// The arabic zain
        /// </summary>
        Arabic_zain = 0x05d2 /* U+0632 ARABIC LETTER ZAIN */,

        /// <summary>
        /// The arabic seen
        /// </summary>
        Arabic_seen = 0x05d3 /* U+0633 ARABIC LETTER SEEN */,

        /// <summary>
        /// The arabic sheen
        /// </summary>
        Arabic_sheen = 0x05d4 /* U+0634 ARABIC LETTER SHEEN */,

        /// <summary>
        /// The arabic sad
        /// </summary>
        Arabic_sad = 0x05d5 /* U+0635 ARABIC LETTER SAD */,

        /// <summary>
        /// The arabic dad
        /// </summary>
        Arabic_dad = 0x05d6 /* U+0636 ARABIC LETTER DAD */,

        /// <summary>
        /// The arabic tah
        /// </summary>
        Arabic_tah = 0x05d7 /* U+0637 ARABIC LETTER TAH */,

        /// <summary>
        /// The arabic zah
        /// </summary>
        Arabic_zah = 0x05d8 /* U+0638 ARABIC LETTER ZAH */,

        /// <summary>
        /// The arabic ain
        /// </summary>
        Arabic_ain = 0x05d9 /* U+0639 ARABIC LETTER AIN */,

        /// <summary>
        /// The arabic ghain
        /// </summary>
        Arabic_ghain = 0x05da /* U+063A ARABIC LETTER GHAIN */,

        /// <summary>
        /// The arabic tatweel
        /// </summary>
        Arabic_tatweel = 0x05e0 /* U+0640 ARABIC TATWEEL */,

        /// <summary>
        /// The arabic feh
        /// </summary>
        Arabic_feh = 0x05e1 /* U+0641 ARABIC LETTER FEH */,

        /// <summary>
        /// The arabic qaf
        /// </summary>
        Arabic_qaf = 0x05e2 /* U+0642 ARABIC LETTER QAF */,

        /// <summary>
        /// The arabic kaf
        /// </summary>
        Arabic_kaf = 0x05e3 /* U+0643 ARABIC LETTER KAF */,

        /// <summary>
        /// The arabic lam
        /// </summary>
        Arabic_lam = 0x05e4 /* U+0644 ARABIC LETTER LAM */,

        /// <summary>
        /// The arabic meem
        /// </summary>
        Arabic_meem = 0x05e5 /* U+0645 ARABIC LETTER MEEM */,

        /// <summary>
        /// The arabic noon
        /// </summary>
        Arabic_noon = 0x05e6 /* U+0646 ARABIC LETTER NOON */,

        /// <summary>
        /// The arabic ha
        /// </summary>
        Arabic_ha = 0x05e7 /* U+0647 ARABIC LETTER HEH */,

        /// <summary>
        /// The arabic heh
        /// </summary>
        Arabic_heh = 0x05e7 /* deprecated */,

        /// <summary>
        /// The arabic waw
        /// </summary>
        Arabic_waw = 0x05e8 /* U+0648 ARABIC LETTER WAW */,

        /// <summary>
        /// The arabic alefmaksura
        /// </summary>
        Arabic_alefmaksura = 0x05e9 /* U+0649 ARABIC LETTER ALEF MAKSURA */,

        /// <summary>
        /// The arabic yeh
        /// </summary>
        Arabic_yeh = 0x05ea /* U+064A ARABIC LETTER YEH */,

        /// <summary>
        /// The arabic fathatan
        /// </summary>
        Arabic_fathatan = 0x05eb /* U+064B ARABIC FATHATAN */,

        /// <summary>
        /// The arabic dammatan
        /// </summary>
        Arabic_dammatan = 0x05ec /* U+064C ARABIC DAMMATAN */,

        /// <summary>
        /// The arabic kasratan
        /// </summary>
        Arabic_kasratan = 0x05ed /* U+064D ARABIC KASRATAN */,

        /// <summary>
        /// The arabic fatha
        /// </summary>
        Arabic_fatha = 0x05ee /* U+064E ARABIC FATHA */,

        /// <summary>
        /// The arabic damma
        /// </summary>
        Arabic_damma = 0x05ef /* U+064F ARABIC DAMMA */,

        /// <summary>
        /// The arabic kasra
        /// </summary>
        Arabic_kasra = 0x05f0 /* U+0650 ARABIC KASRA */,

        /// <summary>
        /// The arabic shadda
        /// </summary>
        Arabic_shadda = 0x05f1 /* U+0651 ARABIC SHADDA */,

        /// <summary>
        /// The arabic sukun
        /// </summary>
        Arabic_sukun = 0x05f2 /* U+0652 ARABIC SUKUN */,

        /// <summary>
        /// The arabic madda above
        /// </summary>
        Arabic_madda_above = 0x1000653 /* U+0653 ARABIC MADDAH ABOVE */,

        /// <summary>
        /// The arabic hamza above
        /// </summary>
        Arabic_hamza_above = 0x1000654 /* U+0654 ARABIC HAMZA ABOVE */,

        /// <summary>
        /// The arabic hamza below
        /// </summary>
        Arabic_hamza_below = 0x1000655 /* U+0655 ARABIC HAMZA BELOW */,

        /// <summary>
        /// The arabic jeh
        /// </summary>
        Arabic_jeh = 0x1000698 /* U+0698 ARABIC LETTER JEH */,

        /// <summary>
        /// The arabic veh
        /// </summary>
        Arabic_veh = 0x10006a4 /* U+06A4 ARABIC LETTER VEH */,

        /// <summary>
        /// The arabic keheh
        /// </summary>
        Arabic_keheh = 0x10006a9 /* U+06A9 ARABIC LETTER KEHEH */,

        /// <summary>
        /// The arabic gaf
        /// </summary>
        Arabic_gaf = 0x10006af /* U+06AF ARABIC LETTER GAF */,

        /// <summary>
        /// The arabic noon ghunna
        /// </summary>
        Arabic_noon_ghunna = 0x10006ba /* U+06BA ARABIC LETTER NOON GHUNNA */,

        /// <summary>
        /// The arabic heh doachashmee
        /// </summary>
        Arabic_heh_doachashmee = 0x10006be /* U+06BE ARABIC LETTER HEH DOACHASHMEE */,

        /// <summary>
        /// The farsi yeh
        /// </summary>
        Farsi_yeh = 0x10006cc /* U+06CC ARABIC LETTER FARSI YEH */,

        /// <summary>
        /// The arabic farsi yeh
        /// </summary>
        Arabic_farsi_yeh = 0x10006cc /* U+06CC ARABIC LETTER FARSI YEH */,

        /// <summary>
        /// The arabic yeh baree
        /// </summary>
        Arabic_yeh_baree = 0x10006d2 /* U+06D2 ARABIC LETTER YEH BARREE */,

        /// <summary>
        /// The arabic heh goal
        /// </summary>
        Arabic_heh_goal = 0x10006c1 /* U+06C1 ARABIC LETTER HEH GOAL */,

        /// <summary>
        /// The arabic switch
        /// </summary>
        Arabic_switch = 0xff7e /* Alias for mode_switch */,

        /// <summary>
        /// The cyrillic ghe bar
        /// </summary>
        Cyrillic_GHE_bar = 0x1000492 /* U+0492 CYRILLIC CAPITAL LETTER GHE WITH STROKE */,

        /// <summary>
        /// The cyrillic ghe bar
        /// </summary>
        Cyrillic_ghe_bar = 0x1000493 /* U+0493 CYRILLIC SMALL LETTER GHE WITH STROKE */,

        /// <summary>
        /// The cyrillic zhe descender
        /// </summary>
        Cyrillic_ZHE_descender = 0x1000496 /* U+0496 CYRILLIC CAPITAL LETTER ZHE WITH DESCENDER */,

        /// <summary>
        /// The cyrillic zhe descender
        /// </summary>
        Cyrillic_zhe_descender = 0x1000497 /* U+0497 CYRILLIC SMALL LETTER ZHE WITH DESCENDER */,

        /// <summary>
        /// The cyrillic ka descender
        /// </summary>
        Cyrillic_KA_descender = 0x100049a /* U+049A CYRILLIC CAPITAL LETTER KA WITH DESCENDER */,

        /// <summary>
        /// The cyrillic ka descender
        /// </summary>
        Cyrillic_ka_descender = 0x100049b /* U+049B CYRILLIC SMALL LETTER KA WITH DESCENDER */,

        /// <summary>
        /// The cyrillic ka vertstroke
        /// </summary>
        Cyrillic_KA_vertstroke = 0x100049c /* U+049C CYRILLIC CAPITAL LETTER KA WITH VERTICAL STROKE */,

        /// <summary>
        /// The cyrillic ka vertstroke
        /// </summary>
        Cyrillic_ka_vertstroke = 0x100049d /* U+049D CYRILLIC SMALL LETTER KA WITH VERTICAL STROKE */,

        /// <summary>
        /// The cyrillic en descender
        /// </summary>
        Cyrillic_EN_descender = 0x10004a2 /* U+04A2 CYRILLIC CAPITAL LETTER EN WITH DESCENDER */,

        /// <summary>
        /// The cyrillic en descender
        /// </summary>
        Cyrillic_en_descender = 0x10004a3 /* U+04A3 CYRILLIC SMALL LETTER EN WITH DESCENDER */,

        /// <summary>
        /// The cyrillic u straight
        /// </summary>
        Cyrillic_U_straight = 0x10004ae /* U+04AE CYRILLIC CAPITAL LETTER STRAIGHT U */,

        /// <summary>
        /// The cyrillic u straight
        /// </summary>
        Cyrillic_u_straight = 0x10004af /* U+04AF CYRILLIC SMALL LETTER STRAIGHT U */,

        /// <summary>
        /// The cyrillic u straight bar
        /// </summary>
        Cyrillic_U_straight_bar = 0x10004b0 /* U+04B0 CYRILLIC CAPITAL LETTER STRAIGHT U WITH STROKE */,

        /// <summary>
        /// The cyrillic u straight bar
        /// </summary>
        Cyrillic_u_straight_bar = 0x10004b1 /* U+04B1 CYRILLIC SMALL LETTER STRAIGHT U WITH STROKE */,

        /// <summary>
        /// The cyrillic ha descender
        /// </summary>
        Cyrillic_HA_descender = 0x10004b2 /* U+04B2 CYRILLIC CAPITAL LETTER HA WITH DESCENDER */,

        /// <summary>
        /// The cyrillic ha descender
        /// </summary>
        Cyrillic_ha_descender = 0x10004b3 /* U+04B3 CYRILLIC SMALL LETTER HA WITH DESCENDER */,

        /// <summary>
        /// The cyrillic che descender
        /// </summary>
        Cyrillic_CHE_descender = 0x10004b6 /* U+04B6 CYRILLIC CAPITAL LETTER CHE WITH DESCENDER */,

        /// <summary>
        /// The cyrillic che descender
        /// </summary>
        Cyrillic_che_descender = 0x10004b7 /* U+04B7 CYRILLIC SMALL LETTER CHE WITH DESCENDER */,

        /// <summary>
        /// The cyrillic che vertstroke
        /// </summary>
        Cyrillic_CHE_vertstroke = 0x10004b8 /* U+04B8 CYRILLIC CAPITAL LETTER CHE WITH VERTICAL STROKE */,

        /// <summary>
        /// The cyrillic che vertstroke
        /// </summary>
        Cyrillic_che_vertstroke = 0x10004b9 /* U+04B9 CYRILLIC SMALL LETTER CHE WITH VERTICAL STROKE */,

        /// <summary>
        /// The cyrillic shha
        /// </summary>
        Cyrillic_SHHA = 0x10004ba /* U+04BA CYRILLIC CAPITAL LETTER SHHA */,

        /// <summary>
        /// The cyrillic shha
        /// </summary>
        Cyrillic_shha = 0x10004bb /* U+04BB CYRILLIC SMALL LETTER SHHA */,

        /// <summary>
        /// The cyrillic schwa
        /// </summary>
        Cyrillic_SCHWA = 0x10004d8 /* U+04D8 CYRILLIC CAPITAL LETTER SCHWA */,

        /// <summary>
        /// The cyrillic schwa
        /// </summary>
        Cyrillic_schwa = 0x10004d9 /* U+04D9 CYRILLIC SMALL LETTER SCHWA */,

        /// <summary>
        /// The cyrillic i macron
        /// </summary>
        Cyrillic_I_macron = 0x10004e2 /* U+04E2 CYRILLIC CAPITAL LETTER I WITH MACRON */,

        /// <summary>
        /// The cyrillic i macron
        /// </summary>
        Cyrillic_i_macron = 0x10004e3 /* U+04E3 CYRILLIC SMALL LETTER I WITH MACRON */,

        /// <summary>
        /// The cyrillic o bar
        /// </summary>
        Cyrillic_O_bar = 0x10004e8 /* U+04E8 CYRILLIC CAPITAL LETTER BARRED O */,

        /// <summary>
        /// The cyrillic o bar
        /// </summary>
        Cyrillic_o_bar = 0x10004e9 /* U+04E9 CYRILLIC SMALL LETTER BARRED O */,

        /// <summary>
        /// The cyrillic u macron
        /// </summary>
        Cyrillic_U_macron = 0x10004ee /* U+04EE CYRILLIC CAPITAL LETTER U WITH MACRON */,

        /// <summary>
        /// The cyrillic u macron
        /// </summary>
        Cyrillic_u_macron = 0x10004ef /* U+04EF CYRILLIC SMALL LETTER U WITH MACRON */,

        /// <summary>
        /// The serbian dje
        /// </summary>
        Serbian_dje = 0x06a1 /* U+0452 CYRILLIC SMALL LETTER DJE */,

        /// <summary>
        /// The macedonia gje
        /// </summary>
        Macedonia_gje = 0x06a2 /* U+0453 CYRILLIC SMALL LETTER GJE */,

        /// <summary>
        /// The cyrillic io
        /// </summary>
        Cyrillic_io = 0x06a3 /* U+0451 CYRILLIC SMALL LETTER IO */,

        /// <summary>
        /// The ukrainian ie
        /// </summary>
        Ukrainian_ie = 0x06a4 /* U+0454 CYRILLIC SMALL LETTER UKRAINIAN IE */,

        /// <summary>
        /// The ukranian je
        /// </summary>
        Ukranian_je = 0x06a4 /* deprecated */,

        /// <summary>
        /// The macedonia dse
        /// </summary>
        Macedonia_dse = 0x06a5 /* U+0455 CYRILLIC SMALL LETTER DZE */,

        /// <summary>
        /// The ukrainian i
        /// </summary>
        Ukrainian_i = 0x06a6 /* U+0456 CYRILLIC SMALL LETTER BYELORUSSIAN-UKRAINIAN I */,

        /// <summary>
        /// The ukranian i
        /// </summary>
        Ukranian_i = 0x06a6 /* deprecated */,

        /// <summary>
        /// The ukrainian yi
        /// </summary>
        Ukrainian_yi = 0x06a7 /* U+0457 CYRILLIC SMALL LETTER YI */,

        /// <summary>
        /// The ukranian yi
        /// </summary>
        Ukranian_yi = 0x06a7 /* deprecated */,

        /// <summary>
        /// The cyrillic je
        /// </summary>
        Cyrillic_je = 0x06a8 /* U+0458 CYRILLIC SMALL LETTER JE */,

        /// <summary>
        /// The serbian je
        /// </summary>
        Serbian_je = 0x06a8 /* deprecated */,

        /// <summary>
        /// The cyrillic lje
        /// </summary>
        Cyrillic_lje = 0x06a9 /* U+0459 CYRILLIC SMALL LETTER LJE */,

        /// <summary>
        /// The serbian lje
        /// </summary>
        Serbian_lje = 0x06a9 /* deprecated */,

        /// <summary>
        /// The cyrillic nje
        /// </summary>
        Cyrillic_nje = 0x06aa /* U+045A CYRILLIC SMALL LETTER NJE */,

        /// <summary>
        /// The serbian nje
        /// </summary>
        Serbian_nje = 0x06aa /* deprecated */,

        /// <summary>
        /// The serbian tshe
        /// </summary>
        Serbian_tshe = 0x06ab /* U+045B CYRILLIC SMALL LETTER TSHE */,

        /// <summary>
        /// The macedonia kje
        /// </summary>
        Macedonia_kje = 0x06ac /* U+045C CYRILLIC SMALL LETTER KJE */,

        /// <summary>
        /// The ukrainian ghe with upturn
        /// </summary>
        Ukrainian_ghe_with_upturn = 0x06ad /* U+0491 CYRILLIC SMALL LETTER GHE WITH UPTURN */,

        /// <summary>
        /// The byelorussian shortu
        /// </summary>
        Byelorussian_shortu = 0x06ae /* U+045E CYRILLIC SMALL LETTER SHORT U */,

        /// <summary>
        /// The cyrillic dzhe
        /// </summary>
        Cyrillic_dzhe = 0x06af /* U+045F CYRILLIC SMALL LETTER DZHE */,

        /// <summary>
        /// The serbian dze
        /// </summary>
        Serbian_dze = 0x06af /* deprecated */,

        /// <summary>
        /// The numerosign
        /// </summary>
        numerosign = 0x06b0 /* U+2116 NUMERO SIGN */,

        /// <summary>
        /// The serbian dje
        /// </summary>
        Serbian_DJE = 0x06b1 /* U+0402 CYRILLIC CAPITAL LETTER DJE */,

        /// <summary>
        /// The macedonia gje
        /// </summary>
        Macedonia_GJE = 0x06b2 /* U+0403 CYRILLIC CAPITAL LETTER GJE */,

        /// <summary>
        /// The cyrillic io
        /// </summary>
        Cyrillic_IO = 0x06b3 /* U+0401 CYRILLIC CAPITAL LETTER IO */,

        /// <summary>
        /// The ukrainian ie
        /// </summary>
        Ukrainian_IE = 0x06b4 /* U+0404 CYRILLIC CAPITAL LETTER UKRAINIAN IE */,

        /// <summary>
        /// The ukranian je
        /// </summary>
        Ukranian_JE = 0x06b4 /* deprecated */,

        /// <summary>
        /// The macedonia dse
        /// </summary>
        Macedonia_DSE = 0x06b5 /* U+0405 CYRILLIC CAPITAL LETTER DZE */,

        /// <summary>
        /// The ukrainian i
        /// </summary>
        Ukrainian_I = 0x06b6 /* U+0406 CYRILLIC CAPITAL LETTER BYELORUSSIAN-UKRAINIAN I */,

        /// <summary>
        /// The ukranian i
        /// </summary>
        Ukranian_I = 0x06b6 /* deprecated */,

        /// <summary>
        /// The ukrainian yi
        /// </summary>
        Ukrainian_YI = 0x06b7 /* U+0407 CYRILLIC CAPITAL LETTER YI */,

        /// <summary>
        /// The ukranian yi
        /// </summary>
        Ukranian_YI = 0x06b7 /* deprecated */,

        /// <summary>
        /// The cyrillic je
        /// </summary>
        Cyrillic_JE = 0x06b8 /* U+0408 CYRILLIC CAPITAL LETTER JE */,

        /// <summary>
        /// The serbian je
        /// </summary>
        Serbian_JE = 0x06b8 /* deprecated */,

        /// <summary>
        /// The cyrillic lje
        /// </summary>
        Cyrillic_LJE = 0x06b9 /* U+0409 CYRILLIC CAPITAL LETTER LJE */,

        /// <summary>
        /// The serbian lje
        /// </summary>
        Serbian_LJE = 0x06b9 /* deprecated */,

        /// <summary>
        /// The cyrillic nje
        /// </summary>
        Cyrillic_NJE = 0x06ba /* U+040A CYRILLIC CAPITAL LETTER NJE */,

        /// <summary>
        /// The serbian nje
        /// </summary>
        Serbian_NJE = 0x06ba /* deprecated */,

        /// <summary>
        /// The serbian tshe
        /// </summary>
        Serbian_TSHE = 0x06bb /* U+040B CYRILLIC CAPITAL LETTER TSHE */,

        /// <summary>
        /// The macedonia kje
        /// </summary>
        Macedonia_KJE = 0x06bc /* U+040C CYRILLIC CAPITAL LETTER KJE */,

        /// <summary>
        /// The ukrainian ghe with upturn
        /// </summary>
        Ukrainian_GHE_WITH_UPTURN = 0x06bd /* U+0490 CYRILLIC CAPITAL LETTER GHE WITH UPTURN */,

        /// <summary>
        /// The byelorussian shortu
        /// </summary>
        Byelorussian_SHORTU = 0x06be /* U+040E CYRILLIC CAPITAL LETTER SHORT U */,

        /// <summary>
        /// The cyrillic dzhe
        /// </summary>
        Cyrillic_DZHE = 0x06bf /* U+040F CYRILLIC CAPITAL LETTER DZHE */,

        /// <summary>
        /// The serbian dze
        /// </summary>
        Serbian_DZE = 0x06bf /* deprecated */,

        /// <summary>
        /// The cyrillic yu
        /// </summary>
        Cyrillic_yu = 0x06c0 /* U+044E CYRILLIC SMALL LETTER YU */,

        /// <summary>
        /// The cyrillic a
        /// </summary>
        Cyrillic_a = 0x06c1 /* U+0430 CYRILLIC SMALL LETTER A */,

        /// <summary>
        /// The cyrillic be
        /// </summary>
        Cyrillic_be = 0x06c2 /* U+0431 CYRILLIC SMALL LETTER BE */,

        /// <summary>
        /// The cyrillic tse
        /// </summary>
        Cyrillic_tse = 0x06c3 /* U+0446 CYRILLIC SMALL LETTER TSE */,

        /// <summary>
        /// The cyrillic de
        /// </summary>
        Cyrillic_de = 0x06c4 /* U+0434 CYRILLIC SMALL LETTER DE */,

        /// <summary>
        /// The cyrillic ie
        /// </summary>
        Cyrillic_ie = 0x06c5 /* U+0435 CYRILLIC SMALL LETTER IE */,

        /// <summary>
        /// The cyrillic ef
        /// </summary>
        Cyrillic_ef = 0x06c6 /* U+0444 CYRILLIC SMALL LETTER EF */,

        /// <summary>
        /// The cyrillic ghe
        /// </summary>
        Cyrillic_ghe = 0x06c7 /* U+0433 CYRILLIC SMALL LETTER GHE */,

        /// <summary>
        /// The cyrillic ha
        /// </summary>
        Cyrillic_ha = 0x06c8 /* U+0445 CYRILLIC SMALL LETTER HA */,

        /// <summary>
        /// The cyrillic i
        /// </summary>
        Cyrillic_i = 0x06c9 /* U+0438 CYRILLIC SMALL LETTER I */,

        /// <summary>
        /// The cyrillic shorti
        /// </summary>
        Cyrillic_shorti = 0x06ca /* U+0439 CYRILLIC SMALL LETTER SHORT I */,

        /// <summary>
        /// The cyrillic ka
        /// </summary>
        Cyrillic_ka = 0x06cb /* U+043A CYRILLIC SMALL LETTER KA */,

        /// <summary>
        /// The cyrillic el
        /// </summary>
        Cyrillic_el = 0x06cc /* U+043B CYRILLIC SMALL LETTER EL */,

        /// <summary>
        /// The cyrillic em
        /// </summary>
        Cyrillic_em = 0x06cd /* U+043C CYRILLIC SMALL LETTER EM */,

        /// <summary>
        /// The cyrillic en
        /// </summary>
        Cyrillic_en = 0x06ce /* U+043D CYRILLIC SMALL LETTER EN */,

        /// <summary>
        /// The cyrillic o
        /// </summary>
        Cyrillic_o = 0x06cf /* U+043E CYRILLIC SMALL LETTER O */,

        /// <summary>
        /// The cyrillic pe
        /// </summary>
        Cyrillic_pe = 0x06d0 /* U+043F CYRILLIC SMALL LETTER PE */,

        /// <summary>
        /// The cyrillic ya
        /// </summary>
        Cyrillic_ya = 0x06d1 /* U+044F CYRILLIC SMALL LETTER YA */,

        /// <summary>
        /// The cyrillic er
        /// </summary>
        Cyrillic_er = 0x06d2 /* U+0440 CYRILLIC SMALL LETTER ER */,

        /// <summary>
        /// The cyrillic es
        /// </summary>
        Cyrillic_es = 0x06d3 /* U+0441 CYRILLIC SMALL LETTER ES */,

        /// <summary>
        /// The cyrillic te
        /// </summary>
        Cyrillic_te = 0x06d4 /* U+0442 CYRILLIC SMALL LETTER TE */,

        /// <summary>
        /// The cyrillic u
        /// </summary>
        Cyrillic_u = 0x06d5 /* U+0443 CYRILLIC SMALL LETTER U */,

        /// <summary>
        /// The cyrillic zhe
        /// </summary>
        Cyrillic_zhe = 0x06d6 /* U+0436 CYRILLIC SMALL LETTER ZHE */,

        /// <summary>
        /// The cyrillic ve
        /// </summary>
        Cyrillic_ve = 0x06d7 /* U+0432 CYRILLIC SMALL LETTER VE */,

        /// <summary>
        /// The cyrillic softsign
        /// </summary>
        Cyrillic_softsign = 0x06d8 /* U+044C CYRILLIC SMALL LETTER SOFT SIGN */,

        /// <summary>
        /// The cyrillic yeru
        /// </summary>
        Cyrillic_yeru = 0x06d9 /* U+044B CYRILLIC SMALL LETTER YERU */,

        /// <summary>
        /// The cyrillic ze
        /// </summary>
        Cyrillic_ze = 0x06da /* U+0437 CYRILLIC SMALL LETTER ZE */,

        /// <summary>
        /// The cyrillic sha
        /// </summary>
        Cyrillic_sha = 0x06db /* U+0448 CYRILLIC SMALL LETTER SHA */,

        /// <summary>
        /// The cyrillic e
        /// </summary>
        Cyrillic_e = 0x06dc /* U+044D CYRILLIC SMALL LETTER E */,

        /// <summary>
        /// The cyrillic shcha
        /// </summary>
        Cyrillic_shcha = 0x06dd /* U+0449 CYRILLIC SMALL LETTER SHCHA */,

        /// <summary>
        /// The cyrillic che
        /// </summary>
        Cyrillic_che = 0x06de /* U+0447 CYRILLIC SMALL LETTER CHE */,

        /// <summary>
        /// The cyrillic hardsign
        /// </summary>
        Cyrillic_hardsign = 0x06df /* U+044A CYRILLIC SMALL LETTER HARD SIGN */,

        /// <summary>
        /// The cyrillic yu
        /// </summary>
        Cyrillic_YU = 0x06e0 /* U+042E CYRILLIC CAPITAL LETTER YU */,

        /// <summary>
        /// The cyrillic a
        /// </summary>
        Cyrillic_A = 0x06e1 /* U+0410 CYRILLIC CAPITAL LETTER A */,

        /// <summary>
        /// The cyrillic be
        /// </summary>
        Cyrillic_BE = 0x06e2 /* U+0411 CYRILLIC CAPITAL LETTER BE */,

        /// <summary>
        /// The cyrillic tse
        /// </summary>
        Cyrillic_TSE = 0x06e3 /* U+0426 CYRILLIC CAPITAL LETTER TSE */,

        /// <summary>
        /// The cyrillic de
        /// </summary>
        Cyrillic_DE = 0x06e4 /* U+0414 CYRILLIC CAPITAL LETTER DE */,

        /// <summary>
        /// The cyrillic ie
        /// </summary>
        Cyrillic_IE = 0x06e5 /* U+0415 CYRILLIC CAPITAL LETTER IE */,

        /// <summary>
        /// The cyrillic ef
        /// </summary>
        Cyrillic_EF = 0x06e6 /* U+0424 CYRILLIC CAPITAL LETTER EF */,

        /// <summary>
        /// The cyrillic ghe
        /// </summary>
        Cyrillic_GHE = 0x06e7 /* U+0413 CYRILLIC CAPITAL LETTER GHE */,

        /// <summary>
        /// The cyrillic ha
        /// </summary>
        Cyrillic_HA = 0x06e8 /* U+0425 CYRILLIC CAPITAL LETTER HA */,

        /// <summary>
        /// The cyrillic i
        /// </summary>
        Cyrillic_I = 0x06e9 /* U+0418 CYRILLIC CAPITAL LETTER I */,

        /// <summary>
        /// The cyrillic shorti
        /// </summary>
        Cyrillic_SHORTI = 0x06ea /* U+0419 CYRILLIC CAPITAL LETTER SHORT I */,

        /// <summary>
        /// The cyrillic ka
        /// </summary>
        Cyrillic_KA = 0x06eb /* U+041A CYRILLIC CAPITAL LETTER KA */,

        /// <summary>
        /// The cyrillic el
        /// </summary>
        Cyrillic_EL = 0x06ec /* U+041B CYRILLIC CAPITAL LETTER EL */,

        /// <summary>
        /// The cyrillic em
        /// </summary>
        Cyrillic_EM = 0x06ed /* U+041C CYRILLIC CAPITAL LETTER EM */,

        /// <summary>
        /// The cyrillic en
        /// </summary>
        Cyrillic_EN = 0x06ee /* U+041D CYRILLIC CAPITAL LETTER EN */,

        /// <summary>
        /// The cyrillic o
        /// </summary>
        Cyrillic_O = 0x06ef /* U+041E CYRILLIC CAPITAL LETTER O */,

        /// <summary>
        /// The cyrillic pe
        /// </summary>
        Cyrillic_PE = 0x06f0 /* U+041F CYRILLIC CAPITAL LETTER PE */,

        /// <summary>
        /// The cyrillic ya
        /// </summary>
        Cyrillic_YA = 0x06f1 /* U+042F CYRILLIC CAPITAL LETTER YA */,

        /// <summary>
        /// The cyrillic er
        /// </summary>
        Cyrillic_ER = 0x06f2 /* U+0420 CYRILLIC CAPITAL LETTER ER */,

        /// <summary>
        /// The cyrillic es
        /// </summary>
        Cyrillic_ES = 0x06f3 /* U+0421 CYRILLIC CAPITAL LETTER ES */,

        /// <summary>
        /// The cyrillic te
        /// </summary>
        Cyrillic_TE = 0x06f4 /* U+0422 CYRILLIC CAPITAL LETTER TE */,

        /// <summary>
        /// The cyrillic u
        /// </summary>
        Cyrillic_U = 0x06f5 /* U+0423 CYRILLIC CAPITAL LETTER U */,

        /// <summary>
        /// The cyrillic zhe
        /// </summary>
        Cyrillic_ZHE = 0x06f6 /* U+0416 CYRILLIC CAPITAL LETTER ZHE */,

        /// <summary>
        /// The cyrillic ve
        /// </summary>
        Cyrillic_VE = 0x06f7 /* U+0412 CYRILLIC CAPITAL LETTER VE */,

        /// <summary>
        /// The cyrillic softsign
        /// </summary>
        Cyrillic_SOFTSIGN = 0x06f8 /* U+042C CYRILLIC CAPITAL LETTER SOFT SIGN */,

        /// <summary>
        /// The cyrillic yeru
        /// </summary>
        Cyrillic_YERU = 0x06f9 /* U+042B CYRILLIC CAPITAL LETTER YERU */,

        /// <summary>
        /// The cyrillic ze
        /// </summary>
        Cyrillic_ZE = 0x06fa /* U+0417 CYRILLIC CAPITAL LETTER ZE */,

        /// <summary>
        /// The cyrillic sha
        /// </summary>
        Cyrillic_SHA = 0x06fb /* U+0428 CYRILLIC CAPITAL LETTER SHA */,

        /// <summary>
        /// The cyrillic e
        /// </summary>
        Cyrillic_E = 0x06fc /* U+042D CYRILLIC CAPITAL LETTER E */,

        /// <summary>
        /// The cyrillic shcha
        /// </summary>
        Cyrillic_SHCHA = 0x06fd /* U+0429 CYRILLIC CAPITAL LETTER SHCHA */,

        /// <summary>
        /// The cyrillic che
        /// </summary>
        Cyrillic_CHE = 0x06fe /* U+0427 CYRILLIC CAPITAL LETTER CHE */,

        /// <summary>
        /// The cyrillic hardsign
        /// </summary>
        Cyrillic_HARDSIGN = 0x06ff /* U+042A CYRILLIC CAPITAL LETTER HARD SIGN */,

        /// <summary>
        /// The greek alph aaccent
        /// </summary>
        Greek_ALPHAaccent = 0x07a1 /* U+0386 GREEK CAPITAL LETTER ALPHA WITH TONOS */,

        /// <summary>
        /// The greek epsilo naccent
        /// </summary>
        Greek_EPSILONaccent = 0x07a2 /* U+0388 GREEK CAPITAL LETTER EPSILON WITH TONOS */,

        /// <summary>
        /// The greek et aaccent
        /// </summary>
        Greek_ETAaccent = 0x07a3 /* U+0389 GREEK CAPITAL LETTER ETA WITH TONOS */,

        /// <summary>
        /// The greek iot aaccent
        /// </summary>
        Greek_IOTAaccent = 0x07a4 /* U+038A GREEK CAPITAL LETTER IOTA WITH TONOS */,

        /// <summary>
        /// The greek iot adieresis
        /// </summary>
        Greek_IOTAdieresis = 0x07a5 /* U+03AA GREEK CAPITAL LETTER IOTA WITH DIALYTIKA */,

        /// <summary>
        /// The greek iot adiaeresis
        /// </summary>
        Greek_IOTAdiaeresis = 0x07a5 /* old typo */,

        /// <summary>
        /// The greek omicro naccent
        /// </summary>
        Greek_OMICRONaccent = 0x07a7 /* U+038C GREEK CAPITAL LETTER OMICRON WITH TONOS */,

        /// <summary>
        /// The greek upsilo naccent
        /// </summary>
        Greek_UPSILONaccent = 0x07a8 /* U+038E GREEK CAPITAL LETTER UPSILON WITH TONOS */,

        /// <summary>
        /// The greek upsilo ndieresis
        /// </summary>
        Greek_UPSILONdieresis = 0x07a9 /* U+03AB GREEK CAPITAL LETTER UPSILON WITH DIALYTIKA */,

        /// <summary>
        /// The greek omeg aaccent
        /// </summary>
        Greek_OMEGAaccent = 0x07ab /* U+038F GREEK CAPITAL LETTER OMEGA WITH TONOS */,

        /// <summary>
        /// The greek accentdieresis
        /// </summary>
        Greek_accentdieresis = 0x07ae /* U+0385 GREEK DIALYTIKA TONOS */,

        /// <summary>
        /// The greek horizbar
        /// </summary>
        Greek_horizbar = 0x07af /* U+2015 HORIZONTAL BAR */,

        /// <summary>
        /// The greek alphaaccent
        /// </summary>
        Greek_alphaaccent = 0x07b1 /* U+03AC GREEK SMALL LETTER ALPHA WITH TONOS */,

        /// <summary>
        /// The greek epsilonaccent
        /// </summary>
        Greek_epsilonaccent = 0x07b2 /* U+03AD GREEK SMALL LETTER EPSILON WITH TONOS */,

        /// <summary>
        /// The greek etaaccent
        /// </summary>
        Greek_etaaccent = 0x07b3 /* U+03AE GREEK SMALL LETTER ETA WITH TONOS */,

        /// <summary>
        /// The greek iotaaccent
        /// </summary>
        Greek_iotaaccent = 0x07b4 /* U+03AF GREEK SMALL LETTER IOTA WITH TONOS */,

        /// <summary>
        /// The greek iotadieresis
        /// </summary>
        Greek_iotadieresis = 0x07b5 /* U+03CA GREEK SMALL LETTER IOTA WITH DIALYTIKA */,

        /// <summary>
        /// The greek iotaaccentdieresis
        /// </summary>
        Greek_iotaaccentdieresis = 0x07b6 /* U+0390 GREEK SMALL LETTER IOTA WITH DIALYTIKA AND TONOS */,

        /// <summary>
        /// The greek omicronaccent
        /// </summary>
        Greek_omicronaccent = 0x07b7 /* U+03CC GREEK SMALL LETTER OMICRON WITH TONOS */,

        /// <summary>
        /// The greek upsilonaccent
        /// </summary>
        Greek_upsilonaccent = 0x07b8 /* U+03CD GREEK SMALL LETTER UPSILON WITH TONOS */,

        /// <summary>
        /// The greek upsilondieresis
        /// </summary>
        Greek_upsilondieresis = 0x07b9 /* U+03CB GREEK SMALL LETTER UPSILON WITH DIALYTIKA */,

        /// <summary>
        /// The greek upsilonaccentdieresis
        /// </summary>
        Greek_upsilonaccentdieresis = 0x07ba /* U+03B0 GREEK SMALL LETTER UPSILON WITH DIALYTIKA AND TONOS */,

        /// <summary>
        /// The greek omegaaccent
        /// </summary>
        Greek_omegaaccent = 0x07bb /* U+03CE GREEK SMALL LETTER OMEGA WITH TONOS */,

        /// <summary>
        /// The greek alpha
        /// </summary>
        Greek_ALPHA = 0x07c1 /* U+0391 GREEK CAPITAL LETTER ALPHA */,

        /// <summary>
        /// The greek beta
        /// </summary>
        Greek_BETA = 0x07c2 /* U+0392 GREEK CAPITAL LETTER BETA */,

        /// <summary>
        /// The greek gamma
        /// </summary>
        Greek_GAMMA = 0x07c3 /* U+0393 GREEK CAPITAL LETTER GAMMA */,

        /// <summary>
        /// The greek delta
        /// </summary>
        Greek_DELTA = 0x07c4 /* U+0394 GREEK CAPITAL LETTER DELTA */,

        /// <summary>
        /// The greek epsilon
        /// </summary>
        Greek_EPSILON = 0x07c5 /* U+0395 GREEK CAPITAL LETTER EPSILON */,

        /// <summary>
        /// The greek zeta
        /// </summary>
        Greek_ZETA = 0x07c6 /* U+0396 GREEK CAPITAL LETTER ZETA */,

        /// <summary>
        /// The greek eta
        /// </summary>
        Greek_ETA = 0x07c7 /* U+0397 GREEK CAPITAL LETTER ETA */,

        /// <summary>
        /// The greek theta
        /// </summary>
        Greek_THETA = 0x07c8 /* U+0398 GREEK CAPITAL LETTER THETA */,

        /// <summary>
        /// The greek iota
        /// </summary>
        Greek_IOTA = 0x07c9 /* U+0399 GREEK CAPITAL LETTER IOTA */,

        /// <summary>
        /// The greek kappa
        /// </summary>
        Greek_KAPPA = 0x07ca /* U+039A GREEK CAPITAL LETTER KAPPA */,

        /// <summary>
        /// The greek lamda
        /// </summary>
        Greek_LAMDA = 0x07cb /* U+039B GREEK CAPITAL LETTER LAMDA */,

        /// <summary>
        /// The greek lambda
        /// </summary>
        Greek_LAMBDA = 0x07cb /* U+039B GREEK CAPITAL LETTER LAMDA */,

        /// <summary>
        /// The greek mu
        /// </summary>
        Greek_MU = 0x07cc /* U+039C GREEK CAPITAL LETTER MU */,

        /// <summary>
        /// The greek nu
        /// </summary>
        Greek_NU = 0x07cd /* U+039D GREEK CAPITAL LETTER NU */,

        /// <summary>
        /// The greek xi
        /// </summary>
        Greek_XI = 0x07ce /* U+039E GREEK CAPITAL LETTER XI */,

        /// <summary>
        /// The greek omicron
        /// </summary>
        Greek_OMICRON = 0x07cf /* U+039F GREEK CAPITAL LETTER OMICRON */,

        /// <summary>
        /// The greek pi
        /// </summary>
        Greek_PI = 0x07d0 /* U+03A0 GREEK CAPITAL LETTER PI */,

        /// <summary>
        /// The greek rho
        /// </summary>
        Greek_RHO = 0x07d1 /* U+03A1 GREEK CAPITAL LETTER RHO */,

        /// <summary>
        /// The greek sigma
        /// </summary>
        Greek_SIGMA = 0x07d2 /* U+03A3 GREEK CAPITAL LETTER SIGMA */,

        /// <summary>
        /// The greek tau
        /// </summary>
        Greek_TAU = 0x07d4 /* U+03A4 GREEK CAPITAL LETTER TAU */,

        /// <summary>
        /// The greek upsilon
        /// </summary>
        Greek_UPSILON = 0x07d5 /* U+03A5 GREEK CAPITAL LETTER UPSILON */,

        /// <summary>
        /// The greek phi
        /// </summary>
        Greek_PHI = 0x07d6 /* U+03A6 GREEK CAPITAL LETTER PHI */,

        /// <summary>
        /// The greek chi
        /// </summary>
        Greek_CHI = 0x07d7 /* U+03A7 GREEK CAPITAL LETTER CHI */,

        /// <summary>
        /// The greek psi
        /// </summary>
        Greek_PSI = 0x07d8 /* U+03A8 GREEK CAPITAL LETTER PSI */,

        /// <summary>
        /// The greek omega
        /// </summary>
        Greek_OMEGA = 0x07d9 /* U+03A9 GREEK CAPITAL LETTER OMEGA */,

        /// <summary>
        /// The greek alpha
        /// </summary>
        Greek_alpha = 0x07e1 /* U+03B1 GREEK SMALL LETTER ALPHA */,

        /// <summary>
        /// The greek beta
        /// </summary>
        Greek_beta = 0x07e2 /* U+03B2 GREEK SMALL LETTER BETA */,

        /// <summary>
        /// The greek gamma
        /// </summary>
        Greek_gamma = 0x07e3 /* U+03B3 GREEK SMALL LETTER GAMMA */,

        /// <summary>
        /// The greek delta
        /// </summary>
        Greek_delta = 0x07e4 /* U+03B4 GREEK SMALL LETTER DELTA */,

        /// <summary>
        /// The greek epsilon
        /// </summary>
        Greek_epsilon = 0x07e5 /* U+03B5 GREEK SMALL LETTER EPSILON */,

        /// <summary>
        /// The greek zeta
        /// </summary>
        Greek_zeta = 0x07e6 /* U+03B6 GREEK SMALL LETTER ZETA */,

        /// <summary>
        /// The greek eta
        /// </summary>
        Greek_eta = 0x07e7 /* U+03B7 GREEK SMALL LETTER ETA */,

        /// <summary>
        /// The greek theta
        /// </summary>
        Greek_theta = 0x07e8 /* U+03B8 GREEK SMALL LETTER THETA */,

        /// <summary>
        /// The greek iota
        /// </summary>
        Greek_iota = 0x07e9 /* U+03B9 GREEK SMALL LETTER IOTA */,

        /// <summary>
        /// The greek kappa
        /// </summary>
        Greek_kappa = 0x07ea /* U+03BA GREEK SMALL LETTER KAPPA */,

        /// <summary>
        /// The greek lamda
        /// </summary>
        Greek_lamda = 0x07eb /* U+03BB GREEK SMALL LETTER LAMDA */,

        /// <summary>
        /// The greek lambda
        /// </summary>
        Greek_lambda = 0x07eb /* U+03BB GREEK SMALL LETTER LAMDA */,

        /// <summary>
        /// The greek mu
        /// </summary>
        Greek_mu = 0x07ec /* U+03BC GREEK SMALL LETTER MU */,

        /// <summary>
        /// The greek nu
        /// </summary>
        Greek_nu = 0x07ed /* U+03BD GREEK SMALL LETTER NU */,

        /// <summary>
        /// The greek xi
        /// </summary>
        Greek_xi = 0x07ee /* U+03BE GREEK SMALL LETTER XI */,

        /// <summary>
        /// The greek omicron
        /// </summary>
        Greek_omicron = 0x07ef /* U+03BF GREEK SMALL LETTER OMICRON */,

        /// <summary>
        /// The greek pi
        /// </summary>
        Greek_pi = 0x07f0 /* U+03C0 GREEK SMALL LETTER PI */,

        /// <summary>
        /// The greek rho
        /// </summary>
        Greek_rho = 0x07f1 /* U+03C1 GREEK SMALL LETTER RHO */,

        /// <summary>
        /// The greek sigma
        /// </summary>
        Greek_sigma = 0x07f2 /* U+03C3 GREEK SMALL LETTER SIGMA */,

        /// <summary>
        /// The greek finalsmallsigma
        /// </summary>
        Greek_finalsmallsigma = 0x07f3 /* U+03C2 GREEK SMALL LETTER FINAL SIGMA */,

        /// <summary>
        /// The greek tau
        /// </summary>
        Greek_tau = 0x07f4 /* U+03C4 GREEK SMALL LETTER TAU */,

        /// <summary>
        /// The greek upsilon
        /// </summary>
        Greek_upsilon = 0x07f5 /* U+03C5 GREEK SMALL LETTER UPSILON */,

        /// <summary>
        /// The greek phi
        /// </summary>
        Greek_phi = 0x07f6 /* U+03C6 GREEK SMALL LETTER PHI */,

        /// <summary>
        /// The greek chi
        /// </summary>
        Greek_chi = 0x07f7 /* U+03C7 GREEK SMALL LETTER CHI */,

        /// <summary>
        /// The greek psi
        /// </summary>
        Greek_psi = 0x07f8 /* U+03C8 GREEK SMALL LETTER PSI */,

        /// <summary>
        /// The greek omega
        /// </summary>
        Greek_omega = 0x07f9 /* U+03C9 GREEK SMALL LETTER OMEGA */,

        /// <summary>
        /// The greek switch
        /// </summary>
        Greek_switch = 0xff7e /* Alias for mode_switch */,

        /// <summary>
        /// The leftradical
        /// </summary>
        leftradical = 0x08a1 /* U+23B7 RADICAL SYMBOL BOTTOM */,

        /// <summary>
        /// The topleftradical
        /// </summary>
        topleftradical = 0x08a2 /*(U+250C BOX DRAWINGS LIGHT DOWN AND RIGHT)*/,

        /// <summary>
        /// The horizconnector
        /// </summary>
        horizconnector = 0x08a3 /*(U+2500 BOX DRAWINGS LIGHT HORIZONTAL)*/,

        /// <summary>
        /// The topintegral
        /// </summary>
        topintegral = 0x08a4 /* U+2320 TOP HALF INTEGRAL */,

        /// <summary>
        /// The botintegral
        /// </summary>
        botintegral = 0x08a5 /* U+2321 BOTTOM HALF INTEGRAL */,

        /// <summary>
        /// The vertconnector
        /// </summary>
        vertconnector = 0x08a6 /*(U+2502 BOX DRAWINGS LIGHT VERTICAL)*/,

        /// <summary>
        /// The topleftsqbracket
        /// </summary>
        topleftsqbracket = 0x08a7 /* U+23A1 LEFT SQUARE BRACKET UPPER CORNER */,

        /// <summary>
        /// The botleftsqbracket
        /// </summary>
        botleftsqbracket = 0x08a8 /* U+23A3 LEFT SQUARE BRACKET LOWER CORNER */,

        /// <summary>
        /// The toprightsqbracket
        /// </summary>
        toprightsqbracket = 0x08a9 /* U+23A4 RIGHT SQUARE BRACKET UPPER CORNER */,

        /// <summary>
        /// The botrightsqbracket
        /// </summary>
        botrightsqbracket = 0x08aa /* U+23A6 RIGHT SQUARE BRACKET LOWER CORNER */,

        /// <summary>
        /// The topleftparens
        /// </summary>
        topleftparens = 0x08ab /* U+239B LEFT PARENTHESIS UPPER HOOK */,

        /// <summary>
        /// The botleftparens
        /// </summary>
        botleftparens = 0x08ac /* U+239D LEFT PARENTHESIS LOWER HOOK */,

        /// <summary>
        /// The toprightparens
        /// </summary>
        toprightparens = 0x08ad /* U+239E RIGHT PARENTHESIS UPPER HOOK */,

        /// <summary>
        /// The botrightparens
        /// </summary>
        botrightparens = 0x08ae /* U+23A0 RIGHT PARENTHESIS LOWER HOOK */,

        /// <summary>
        /// The leftmiddlecurlybrace
        /// </summary>
        leftmiddlecurlybrace = 0x08af /* U+23A8 LEFT CURLY BRACKET MIDDLE PIECE */,

        /// <summary>
        /// The rightmiddlecurlybrace
        /// </summary>
        rightmiddlecurlybrace = 0x08b0 /* U+23AC RIGHT CURLY BRACKET MIDDLE PIECE */,

        /// <summary>
        /// The topleftsummation
        /// </summary>
        topleftsummation = 0x08b1,

        /// <summary>
        /// The botleftsummation
        /// </summary>
        botleftsummation = 0x08b2,

        /// <summary>
        /// The topvertsummationconnector
        /// </summary>
        topvertsummationconnector = 0x08b3,

        /// <summary>
        /// The botvertsummationconnector
        /// </summary>
        botvertsummationconnector = 0x08b4,

        /// <summary>
        /// The toprightsummation
        /// </summary>
        toprightsummation = 0x08b5,

        /// <summary>
        /// The botrightsummation
        /// </summary>
        botrightsummation = 0x08b6,

        /// <summary>
        /// The rightmiddlesummation
        /// </summary>
        rightmiddlesummation = 0x08b7,

        /// <summary>
        /// The lessthanequal
        /// </summary>
        lessthanequal = 0x08bc /* U+2264 LESS-THAN OR EQUAL TO */,

        /// <summary>
        /// The notequal
        /// </summary>
        notequal = 0x08bd /* U+2260 NOT EQUAL TO */,

        /// <summary>
        /// The greaterthanequal
        /// </summary>
        greaterthanequal = 0x08be /* U+2265 GREATER-THAN OR EQUAL TO */,

        /// <summary>
        /// The integral
        /// </summary>
        integral = 0x08bf /* U+222B INTEGRAL */,

        /// <summary>
        /// The therefore
        /// </summary>
        therefore = 0x08c0 /* U+2234 THEREFORE */,

        /// <summary>
        /// The variation
        /// </summary>
        variation = 0x08c1 /* U+221D PROPORTIONAL TO */,

        /// <summary>
        /// The infinity
        /// </summary>
        infinity = 0x08c2 /* U+221E INFINITY */,

        /// <summary>
        /// The nabla
        /// </summary>
        nabla = 0x08c5 /* U+2207 NABLA */,

        /// <summary>
        /// The approximate
        /// </summary>
        approximate = 0x08c8 /* U+223C TILDE OPERATOR */,

        /// <summary>
        /// The similarequal
        /// </summary>
        similarequal = 0x08c9 /* U+2243 ASYMPTOTICALLY EQUAL TO */,

        /// <summary>
        /// The ifonlyif
        /// </summary>
        ifonlyif = 0x08cd /* U+21D4 LEFT RIGHT DOUBLE ARROW */,

        /// <summary>
        /// The implies
        /// </summary>
        implies = 0x08ce /* U+21D2 RIGHTWARDS DOUBLE ARROW */,

        /// <summary>
        /// The identical
        /// </summary>
        identical = 0x08cf /* U+2261 IDENTICAL TO */,

        /// <summary>
        /// The radical
        /// </summary>
        radical = 0x08d6 /* U+221A SQUARE ROOT */,

        /// <summary>
        /// The includedin
        /// </summary>
        includedin = 0x08da /* U+2282 SUBSET OF */,

        /// <summary>
        /// The includes
        /// </summary>
        includes = 0x08db /* U+2283 SUPERSET OF */,

        /// <summary>
        /// The intersection
        /// </summary>
        intersection = 0x08dc /* U+2229 INTERSECTION */,

        /// <summary>
        /// The union
        /// </summary>
        union = 0x08dd /* U+222A UNION */,

        /// <summary>
        /// The logicaland
        /// </summary>
        logicaland = 0x08de /* U+2227 LOGICAL AND */,

        /// <summary>
        /// The logicalor
        /// </summary>
        logicalor = 0x08df /* U+2228 LOGICAL OR */,

        /// <summary>
        /// The partialderivative
        /// </summary>
        partialderivative = 0x08ef /* U+2202 PARTIAL DIFFERENTIAL */,

        /// <summary>
        /// The function
        /// </summary>
        function = 0x08f6 /* U+0192 LATIN SMALL LETTER F WITH HOOK */,

        /// <summary>
        /// The leftarrow
        /// </summary>
        leftarrow = 0x08fb /* U+2190 LEFTWARDS ARROW */,

        /// <summary>
        /// The uparrow
        /// </summary>
        uparrow = 0x08fc /* U+2191 UPWARDS ARROW */,

        /// <summary>
        /// The rightarrow
        /// </summary>
        rightarrow = 0x08fd /* U+2192 RIGHTWARDS ARROW */,

        /// <summary>
        /// The downarrow
        /// </summary>
        downarrow = 0x08fe /* U+2193 DOWNWARDS ARROW */,

        /// <summary>
        /// The blank
        /// </summary>
        blank = 0x09df,

        /// <summary>
        /// The soliddiamond
        /// </summary>
        soliddiamond = 0x09e0 /* U+25C6 BLACK DIAMOND */,

        /// <summary>
        /// The checkerboard
        /// </summary>
        checkerboard = 0x09e1 /* U+2592 MEDIUM SHADE */,

        /// <summary>
        /// The ht
        /// </summary>
        ht = 0x09e2 /* U+2409 SYMBOL FOR HORIZONTAL TABULATION */,

        /// <summary>
        /// The ff
        /// </summary>
        ff = 0x09e3 /* U+240C SYMBOL FOR FORM FEED */,

        /// <summary>
        /// The cr
        /// </summary>
        cr = 0x09e4 /* U+240D SYMBOL FOR CARRIAGE RETURN */,

        /// <summary>
        /// The lf
        /// </summary>
        lf = 0x09e5 /* U+240A SYMBOL FOR LINE FEED */,

        /// <summary>
        /// The nl
        /// </summary>
        nl = 0x09e8 /* U+2424 SYMBOL FOR NEWLINE */,

        /// <summary>
        /// The vt
        /// </summary>
        vt = 0x09e9 /* U+240B SYMBOL FOR VERTICAL TABULATION */,

        /// <summary>
        /// The lowrightcorner
        /// </summary>
        lowrightcorner = 0x09ea /* U+2518 BOX DRAWINGS LIGHT UP AND LEFT */,

        /// <summary>
        /// The uprightcorner
        /// </summary>
        uprightcorner = 0x09eb /* U+2510 BOX DRAWINGS LIGHT DOWN AND LEFT */,

        /// <summary>
        /// The upleftcorner
        /// </summary>
        upleftcorner = 0x09ec /* U+250C BOX DRAWINGS LIGHT DOWN AND RIGHT */,

        /// <summary>
        /// The lowleftcorner
        /// </summary>
        lowleftcorner = 0x09ed /* U+2514 BOX DRAWINGS LIGHT UP AND RIGHT */,

        /// <summary>
        /// The crossinglines
        /// </summary>
        crossinglines = 0x09ee /* U+253C BOX DRAWINGS LIGHT VERTICAL AND HORIZONTAL */,

        /// <summary>
        /// The horizlinescan1
        /// </summary>
        horizlinescan1 = 0x09ef /* U+23BA HORIZONTAL SCAN LINE-1 */,

        /// <summary>
        /// The horizlinescan3
        /// </summary>
        horizlinescan3 = 0x09f0 /* U+23BB HORIZONTAL SCAN LINE-3 */,

        /// <summary>
        /// The horizlinescan5
        /// </summary>
        horizlinescan5 = 0x09f1 /* U+2500 BOX DRAWINGS LIGHT HORIZONTAL */,

        /// <summary>
        /// The horizlinescan7
        /// </summary>
        horizlinescan7 = 0x09f2 /* U+23BC HORIZONTAL SCAN LINE-7 */,

        /// <summary>
        /// The horizlinescan9
        /// </summary>
        horizlinescan9 = 0x09f3 /* U+23BD HORIZONTAL SCAN LINE-9 */,

        /// <summary>
        /// The leftt
        /// </summary>
        leftt = 0x09f4 /* U+251C BOX DRAWINGS LIGHT VERTICAL AND RIGHT */,

        /// <summary>
        /// The rightt
        /// </summary>
        rightt = 0x09f5 /* U+2524 BOX DRAWINGS LIGHT VERTICAL AND LEFT */,

        /// <summary>
        /// The bott
        /// </summary>
        bott = 0x09f6 /* U+2534 BOX DRAWINGS LIGHT UP AND HORIZONTAL */,

        /// <summary>
        /// The topt
        /// </summary>
        topt = 0x09f7 /* U+252C BOX DRAWINGS LIGHT DOWN AND HORIZONTAL */,

        /// <summary>
        /// The vertbar
        /// </summary>
        vertbar = 0x09f8 /* U+2502 BOX DRAWINGS LIGHT VERTICAL */,

        /// <summary>
        /// The emspace
        /// </summary>
        emspace = 0x0aa1 /* U+2003 EM SPACE */,

        /// <summary>
        /// The enspace
        /// </summary>
        enspace = 0x0aa2 /* U+2002 EN SPACE */,

        /// <summary>
        /// The em3space
        /// </summary>
        em3space = 0x0aa3 /* U+2004 THREE-PER-EM SPACE */,

        /// <summary>
        /// The em4space
        /// </summary>
        em4space = 0x0aa4 /* U+2005 FOUR-PER-EM SPACE */,

        /// <summary>
        /// The digitspace
        /// </summary>
        digitspace = 0x0aa5 /* U+2007 FIGURE SPACE */,

        /// <summary>
        /// The punctspace
        /// </summary>
        punctspace = 0x0aa6 /* U+2008 PUNCTUATION SPACE */,

        /// <summary>
        /// The thinspace
        /// </summary>
        thinspace = 0x0aa7 /* U+2009 THIN SPACE */,

        /// <summary>
        /// The hairspace
        /// </summary>
        hairspace = 0x0aa8 /* U+200A HAIR SPACE */,

        /// <summary>
        /// The emdash
        /// </summary>
        emdash = 0x0aa9 /* U+2014 EM DASH */,

        /// <summary>
        /// The endash
        /// </summary>
        endash = 0x0aaa /* U+2013 EN DASH */,

        /// <summary>
        /// The signifblank
        /// </summary>
        signifblank = 0x0aac /*(U+2423 OPEN BOX)*/,

        /// <summary>
        /// The ellipsis
        /// </summary>
        ellipsis = 0x0aae /* U+2026 HORIZONTAL ELLIPSIS */,

        /// <summary>
        /// The doubbaselinedot
        /// </summary>
        doubbaselinedot = 0x0aaf /* U+2025 TWO DOT LEADER */,

        /// <summary>
        /// The onethird
        /// </summary>
        onethird = 0x0ab0 /* U+2153 VULGAR FRACTION ONE THIRD */,

        /// <summary>
        /// The twothirds
        /// </summary>
        twothirds = 0x0ab1 /* U+2154 VULGAR FRACTION TWO THIRDS */,

        /// <summary>
        /// The onefifth
        /// </summary>
        onefifth = 0x0ab2 /* U+2155 VULGAR FRACTION ONE FIFTH */,

        /// <summary>
        /// The twofifths
        /// </summary>
        twofifths = 0x0ab3 /* U+2156 VULGAR FRACTION TWO FIFTHS */,

        /// <summary>
        /// The threefifths
        /// </summary>
        threefifths = 0x0ab4 /* U+2157 VULGAR FRACTION THREE FIFTHS */,

        /// <summary>
        /// The fourfifths
        /// </summary>
        fourfifths = 0x0ab5 /* U+2158 VULGAR FRACTION FOUR FIFTHS */,

        /// <summary>
        /// The onesixth
        /// </summary>
        onesixth = 0x0ab6 /* U+2159 VULGAR FRACTION ONE SIXTH */,

        /// <summary>
        /// The fivesixths
        /// </summary>
        fivesixths = 0x0ab7 /* U+215A VULGAR FRACTION FIVE SIXTHS */,

        /// <summary>
        /// The careof
        /// </summary>
        careof = 0x0ab8 /* U+2105 CARE OF */,

        /// <summary>
        /// The figdash
        /// </summary>
        figdash = 0x0abb /* U+2012 FIGURE DASH */,

        /// <summary>
        /// The leftanglebracket
        /// </summary>
        leftanglebracket = 0x0abc /*(U+27E8 MATHEMATICAL LEFT ANGLE BRACKET)*/,

        /// <summary>
        /// The decimalpoint
        /// </summary>
        decimalpoint = 0x0abd /*(U+002E FULL STOP)*/,

        /// <summary>
        /// The rightanglebracket
        /// </summary>
        rightanglebracket = 0x0abe /*(U+27E9 MATHEMATICAL RIGHT ANGLE BRACKET)*/,

        /// <summary>
        /// The marker
        /// </summary>
        marker = 0x0abf,

        /// <summary>
        /// The oneeighth
        /// </summary>
        oneeighth = 0x0ac3 /* U+215B VULGAR FRACTION ONE EIGHTH */,

        /// <summary>
        /// The threeeighths
        /// </summary>
        threeeighths = 0x0ac4 /* U+215C VULGAR FRACTION THREE EIGHTHS */,

        /// <summary>
        /// The fiveeighths
        /// </summary>
        fiveeighths = 0x0ac5 /* U+215D VULGAR FRACTION FIVE EIGHTHS */,

        /// <summary>
        /// The seveneighths
        /// </summary>
        seveneighths = 0x0ac6 /* U+215E VULGAR FRACTION SEVEN EIGHTHS */,

        /// <summary>
        /// The trademark
        /// </summary>
        trademark = 0x0ac9 /* U+2122 TRADE MARK SIGN */,

        /// <summary>
        /// The signaturemark
        /// </summary>
        signaturemark = 0x0aca /*(U+2613 SALTIRE)*/,

        /// <summary>
        /// The trademarkincircle
        /// </summary>
        trademarkincircle = 0x0acb,

        /// <summary>
        /// The leftopentriangle
        /// </summary>
        leftopentriangle = 0x0acc /*(U+25C1 WHITE LEFT-POINTING TRIANGLE)*/,

        /// <summary>
        /// The rightopentriangle
        /// </summary>
        rightopentriangle = 0x0acd /*(U+25B7 WHITE RIGHT-POINTING TRIANGLE)*/,

        /// <summary>
        /// The emopencircle
        /// </summary>
        emopencircle = 0x0ace /*(U+25CB WHITE CIRCLE)*/,

        /// <summary>
        /// The emopenrectangle
        /// </summary>
        emopenrectangle = 0x0acf /*(U+25AF WHITE VERTICAL RECTANGLE)*/,

        /// <summary>
        /// The leftsinglequotemark
        /// </summary>
        leftsinglequotemark = 0x0ad0 /* U+2018 LEFT SINGLE QUOTATION MARK */,

        /// <summary>
        /// The rightsinglequotemark
        /// </summary>
        rightsinglequotemark = 0x0ad1 /* U+2019 RIGHT SINGLE QUOTATION MARK */,

        /// <summary>
        /// The leftdoublequotemark
        /// </summary>
        leftdoublequotemark = 0x0ad2 /* U+201C LEFT DOUBLE QUOTATION MARK */,

        /// <summary>
        /// The rightdoublequotemark
        /// </summary>
        rightdoublequotemark = 0x0ad3 /* U+201D RIGHT DOUBLE QUOTATION MARK */,

        /// <summary>
        /// The prescription
        /// </summary>
        prescription = 0x0ad4 /* U+211E PRESCRIPTION TAKE */,

        /// <summary>
        /// The permille
        /// </summary>
        permille = 0x0ad5 /* U+2030 PER MILLE SIGN */,

        /// <summary>
        /// The minutes
        /// </summary>
        minutes = 0x0ad6 /* U+2032 PRIME */,

        /// <summary>
        /// The seconds
        /// </summary>
        seconds = 0x0ad7 /* U+2033 DOUBLE PRIME */,

        /// <summary>
        /// The latincross
        /// </summary>
        latincross = 0x0ad9 /* U+271D LATIN CROSS */,

        /// <summary>
        /// The hexagram
        /// </summary>
        hexagram = 0x0ada,

        /// <summary>
        /// The filledrectbullet
        /// </summary>
        filledrectbullet = 0x0adb /*(U+25AC BLACK RECTANGLE)*/,

        /// <summary>
        /// The filledlefttribullet
        /// </summary>
        filledlefttribullet = 0x0adc /*(U+25C0 BLACK LEFT-POINTING TRIANGLE)*/,

        /// <summary>
        /// The filledrighttribullet
        /// </summary>
        filledrighttribullet = 0x0add /*(U+25B6 BLACK RIGHT-POINTING TRIANGLE)*/,

        /// <summary>
        /// The emfilledcircle
        /// </summary>
        emfilledcircle = 0x0ade /*(U+25CF BLACK CIRCLE)*/,

        /// <summary>
        /// The emfilledrect
        /// </summary>
        emfilledrect = 0x0adf /*(U+25AE BLACK VERTICAL RECTANGLE)*/,

        /// <summary>
        /// The enopencircbullet
        /// </summary>
        enopencircbullet = 0x0ae0 /*(U+25E6 WHITE BULLET)*/,

        /// <summary>
        /// The enopensquarebullet
        /// </summary>
        enopensquarebullet = 0x0ae1 /*(U+25AB WHITE SMALL SQUARE)*/,

        /// <summary>
        /// The openrectbullet
        /// </summary>
        openrectbullet = 0x0ae2 /*(U+25AD WHITE RECTANGLE)*/,

        /// <summary>
        /// The opentribulletup
        /// </summary>
        opentribulletup = 0x0ae3 /*(U+25B3 WHITE UP-POINTING TRIANGLE)*/,

        /// <summary>
        /// The opentribulletdown
        /// </summary>
        opentribulletdown = 0x0ae4 /*(U+25BD WHITE DOWN-POINTING TRIANGLE)*/,

        /// <summary>
        /// The openstar
        /// </summary>
        openstar = 0x0ae5 /*(U+2606 WHITE STAR)*/,

        /// <summary>
        /// The enfilledcircbullet
        /// </summary>
        enfilledcircbullet = 0x0ae6 /*(U+2022 BULLET)*/,

        /// <summary>
        /// The enfilledsqbullet
        /// </summary>
        enfilledsqbullet = 0x0ae7 /*(U+25AA BLACK SMALL SQUARE)*/,

        /// <summary>
        /// The filledtribulletup
        /// </summary>
        filledtribulletup = 0x0ae8 /*(U+25B2 BLACK UP-POINTING TRIANGLE)*/,

        /// <summary>
        /// The filledtribulletdown
        /// </summary>
        filledtribulletdown = 0x0ae9 /*(U+25BC BLACK DOWN-POINTING TRIANGLE)*/,

        /// <summary>
        /// The leftpointer
        /// </summary>
        leftpointer = 0x0aea /*(U+261C WHITE LEFT POINTING INDEX)*/,

        /// <summary>
        /// The rightpointer
        /// </summary>
        rightpointer = 0x0aeb /*(U+261E WHITE RIGHT POINTING INDEX)*/,

        /// <summary>
        /// The club
        /// </summary>
        club = 0x0aec /* U+2663 BLACK CLUB SUIT */,

        /// <summary>
        /// The diamond
        /// </summary>
        diamond = 0x0aed /* U+2666 BLACK DIAMOND SUIT */,

        /// <summary>
        /// The heart
        /// </summary>
        heart = 0x0aee /* U+2665 BLACK HEART SUIT */,

        /// <summary>
        /// The maltesecross
        /// </summary>
        maltesecross = 0x0af0 /* U+2720 MALTESE CROSS */,

        /// <summary>
        /// The dagger
        /// </summary>
        dagger = 0x0af1 /* U+2020 DAGGER */,

        /// <summary>
        /// The doubledagger
        /// </summary>
        doubledagger = 0x0af2 /* U+2021 DOUBLE DAGGER */,

        /// <summary>
        /// The checkmark
        /// </summary>
        checkmark = 0x0af3 /* U+2713 CHECK MARK */,

        /// <summary>
        /// The ballotcross
        /// </summary>
        ballotcross = 0x0af4 /* U+2717 BALLOT X */,

        /// <summary>
        /// The musicalsharp
        /// </summary>
        musicalsharp = 0x0af5 /* U+266F MUSIC SHARP SIGN */,

        /// <summary>
        /// The musicalflat
        /// </summary>
        musicalflat = 0x0af6 /* U+266D MUSIC FLAT SIGN */,

        /// <summary>
        /// The malesymbol
        /// </summary>
        malesymbol = 0x0af7 /* U+2642 MALE SIGN */,

        /// <summary>
        /// The femalesymbol
        /// </summary>
        femalesymbol = 0x0af8 /* U+2640 FEMALE SIGN */,

        /// <summary>
        /// The telephone
        /// </summary>
        telephone = 0x0af9 /* U+260E BLACK TELEPHONE */,

        /// <summary>
        /// The telephonerecorder
        /// </summary>
        telephonerecorder = 0x0afa /* U+2315 TELEPHONE RECORDER */,

        /// <summary>
        /// The phonographcopyright
        /// </summary>
        phonographcopyright = 0x0afb /* U+2117 SOUND RECORDING COPYRIGHT */,

        /// <summary>
        /// The caret
        /// </summary>
        caret = 0x0afc /* U+2038 CARET */,

        /// <summary>
        /// The singlelowquotemark
        /// </summary>
        singlelowquotemark = 0x0afd /* U+201A SINGLE LOW-9 QUOTATION MARK */,

        /// <summary>
        /// The doublelowquotemark
        /// </summary>
        doublelowquotemark = 0x0afe /* U+201E DOUBLE LOW-9 QUOTATION MARK */,

        /// <summary>
        /// The cursor
        /// </summary>
        cursor = 0x0aff,

        /// <summary>
        /// The leftcaret
        /// </summary>
        leftcaret = 0x0ba3 /*(U+003C LESS-THAN SIGN)*/,

        /// <summary>
        /// The rightcaret
        /// </summary>
        rightcaret = 0x0ba6 /*(U+003E GREATER-THAN SIGN)*/,

        /// <summary>
        /// The downcaret
        /// </summary>
        downcaret = 0x0ba8 /*(U+2228 LOGICAL OR)*/,

        /// <summary>
        /// The upcaret
        /// </summary>
        upcaret = 0x0ba9 /*(U+2227 LOGICAL AND)*/,

        /// <summary>
        /// The overbar
        /// </summary>
        overbar = 0x0bc0 /*(U+00AF MACRON)*/,

        /// <summary>
        /// The downtack
        /// </summary>
        downtack = 0x0bc2 /* U+22A4 DOWN TACK */,

        /// <summary>
        /// The upshoe
        /// </summary>
        upshoe = 0x0bc3 /*(U+2229 INTERSECTION)*/,

        /// <summary>
        /// The downstile
        /// </summary>
        downstile = 0x0bc4 /* U+230A LEFT FLOOR */,

        /// <summary>
        /// The underbar
        /// </summary>
        underbar = 0x0bc6 /*(U+005F LOW LINE)*/,

        /// <summary>
        /// The jot
        /// </summary>
        jot = 0x0bca /* U+2218 RING OPERATOR */,

        /// <summary>
        /// The quad
        /// </summary>
        quad = 0x0bcc /* U+2395 APL FUNCTIONAL SYMBOL QUAD */,

        /// <summary>
        /// The uptack
        /// </summary>
        uptack = 0x0bce /* U+22A5 UP TACK */,

        /// <summary>
        /// The circle
        /// </summary>
        circle = 0x0bcf /* U+25CB WHITE CIRCLE */,

        /// <summary>
        /// The upstile
        /// </summary>
        upstile = 0x0bd3 /* U+2308 LEFT CEILING */,

        /// <summary>
        /// The downshoe
        /// </summary>
        downshoe = 0x0bd6 /*(U+222A UNION)*/,

        /// <summary>
        /// The rightshoe
        /// </summary>
        rightshoe = 0x0bd8 /*(U+2283 SUPERSET OF)*/,

        /// <summary>
        /// The leftshoe
        /// </summary>
        leftshoe = 0x0bda /*(U+2282 SUBSET OF)*/,

        /// <summary>
        /// The lefttack
        /// </summary>
        lefttack = 0x0bdc /* U+22A3 LEFT TACK */,

        /// <summary>
        /// The righttack
        /// </summary>
        righttack = 0x0bfc /* U+22A2 RIGHT TACK */,

        /// <summary>
        /// The hebrew doublelowline
        /// </summary>
        hebrew_doublelowline = 0x0cdf /* U+2017 DOUBLE LOW LINE */,

        /// <summary>
        /// The hebrew aleph
        /// </summary>
        hebrew_aleph = 0x0ce0 /* U+05D0 HEBREW LETTER ALEF */,

        /// <summary>
        /// The hebrew bet
        /// </summary>
        hebrew_bet = 0x0ce1 /* U+05D1 HEBREW LETTER BET */,

        /// <summary>
        /// The hebrew beth
        /// </summary>
        hebrew_beth = 0x0ce1 /* deprecated */,

        /// <summary>
        /// The hebrew gimel
        /// </summary>
        hebrew_gimel = 0x0ce2 /* U+05D2 HEBREW LETTER GIMEL */,

        /// <summary>
        /// The hebrew gimmel
        /// </summary>
        hebrew_gimmel = 0x0ce2 /* deprecated */,

        /// <summary>
        /// The hebrew dalet
        /// </summary>
        hebrew_dalet = 0x0ce3 /* U+05D3 HEBREW LETTER DALET */,

        /// <summary>
        /// The hebrew daleth
        /// </summary>
        hebrew_daleth = 0x0ce3 /* deprecated */,

        /// <summary>
        /// The hebrew he
        /// </summary>
        hebrew_he = 0x0ce4 /* U+05D4 HEBREW LETTER HE */,

        /// <summary>
        /// The hebrew waw
        /// </summary>
        hebrew_waw = 0x0ce5 /* U+05D5 HEBREW LETTER VAV */,

        /// <summary>
        /// The hebrew zain
        /// </summary>
        hebrew_zain = 0x0ce6 /* U+05D6 HEBREW LETTER ZAYIN */,

        /// <summary>
        /// The hebrew zayin
        /// </summary>
        hebrew_zayin = 0x0ce6 /* deprecated */,

        /// <summary>
        /// The hebrew chet
        /// </summary>
        hebrew_chet = 0x0ce7 /* U+05D7 HEBREW LETTER HET */,

        /// <summary>
        /// The hebrew het
        /// </summary>
        hebrew_het = 0x0ce7 /* deprecated */,

        /// <summary>
        /// The hebrew tet
        /// </summary>
        hebrew_tet = 0x0ce8 /* U+05D8 HEBREW LETTER TET */,

        /// <summary>
        /// The hebrew teth
        /// </summary>
        hebrew_teth = 0x0ce8 /* deprecated */,

        /// <summary>
        /// The hebrew yod
        /// </summary>
        hebrew_yod = 0x0ce9 /* U+05D9 HEBREW LETTER YOD */,

        /// <summary>
        /// The hebrew finalkaph
        /// </summary>
        hebrew_finalkaph = 0x0cea /* U+05DA HEBREW LETTER FINAL KAF */,

        /// <summary>
        /// The hebrew kaph
        /// </summary>
        hebrew_kaph = 0x0ceb /* U+05DB HEBREW LETTER KAF */,

        /// <summary>
        /// The hebrew lamed
        /// </summary>
        hebrew_lamed = 0x0cec /* U+05DC HEBREW LETTER LAMED */,

        /// <summary>
        /// The hebrew finalmem
        /// </summary>
        hebrew_finalmem = 0x0ced /* U+05DD HEBREW LETTER FINAL MEM */,

        /// <summary>
        /// The hebrew memory
        /// </summary>
        hebrew_mem = 0x0cee /* U+05DE HEBREW LETTER MEM */,

        /// <summary>
        /// The hebrew finalnun
        /// </summary>
        hebrew_finalnun = 0x0cef /* U+05DF HEBREW LETTER FINAL NUN */,

        /// <summary>
        /// The hebrew nun
        /// </summary>
        hebrew_nun = 0x0cf0 /* U+05E0 HEBREW LETTER NUN */,

        /// <summary>
        /// The hebrew samech
        /// </summary>
        hebrew_samech = 0x0cf1 /* U+05E1 HEBREW LETTER SAMEKH */,

        /// <summary>
        /// The hebrew samekh
        /// </summary>
        hebrew_samekh = 0x0cf1 /* deprecated */,

        /// <summary>
        /// The hebrew ayin
        /// </summary>
        hebrew_ayin = 0x0cf2 /* U+05E2 HEBREW LETTER AYIN */,

        /// <summary>
        /// The hebrew finalpe
        /// </summary>
        hebrew_finalpe = 0x0cf3 /* U+05E3 HEBREW LETTER FINAL PE */,

        /// <summary>
        /// The hebrew pe
        /// </summary>
        hebrew_pe = 0x0cf4 /* U+05E4 HEBREW LETTER PE */,

        /// <summary>
        /// The hebrew finalzade
        /// </summary>
        hebrew_finalzade = 0x0cf5 /* U+05E5 HEBREW LETTER FINAL TSADI */,

        /// <summary>
        /// The hebrew finalzadi
        /// </summary>
        hebrew_finalzadi = 0x0cf5 /* deprecated */,

        /// <summary>
        /// The hebrew zade
        /// </summary>
        hebrew_zade = 0x0cf6 /* U+05E6 HEBREW LETTER TSADI */,

        /// <summary>
        /// The hebrew zadi
        /// </summary>
        hebrew_zadi = 0x0cf6 /* deprecated */,

        /// <summary>
        /// The hebrew qoph
        /// </summary>
        hebrew_qoph = 0x0cf7 /* U+05E7 HEBREW LETTER QOF */,

        /// <summary>
        /// The hebrew kuf
        /// </summary>
        hebrew_kuf = 0x0cf7 /* deprecated */,

        /// <summary>
        /// The hebrew resh
        /// </summary>
        hebrew_resh = 0x0cf8 /* U+05E8 HEBREW LETTER RESH */,

        /// <summary>
        /// The hebrew shin
        /// </summary>
        hebrew_shin = 0x0cf9 /* U+05E9 HEBREW LETTER SHIN */,

        /// <summary>
        /// The hebrew taw
        /// </summary>
        hebrew_taw = 0x0cfa /* U+05EA HEBREW LETTER TAV */,

        /// <summary>
        /// The hebrew taf
        /// </summary>
        hebrew_taf = 0x0cfa /* deprecated */,

        /// <summary>
        /// The hebrew switch
        /// </summary>
        Hebrew_switch = 0xff7e /* Alias for mode_switch */,

        /// <summary>
        /// The thai kokai
        /// </summary>
        Thai_kokai = 0x0da1 /* U+0E01 THAI CHARACTER KO KAI */,

        /// <summary>
        /// The thai khokhai
        /// </summary>
        Thai_khokhai = 0x0da2 /* U+0E02 THAI CHARACTER KHO KHAI */,

        /// <summary>
        /// The thai khokhuat
        /// </summary>
        Thai_khokhuat = 0x0da3 /* U+0E03 THAI CHARACTER KHO KHUAT */,

        /// <summary>
        /// The thai khokhwai
        /// </summary>
        Thai_khokhwai = 0x0da4 /* U+0E04 THAI CHARACTER KHO KHWAI */,

        /// <summary>
        /// The thai khokhon
        /// </summary>
        Thai_khokhon = 0x0da5 /* U+0E05 THAI CHARACTER KHO KHON */,

        /// <summary>
        /// The thai khorakhang
        /// </summary>
        Thai_khorakhang = 0x0da6 /* U+0E06 THAI CHARACTER KHO RAKHANG */,

        /// <summary>
        /// The thai ngongu
        /// </summary>
        Thai_ngongu = 0x0da7 /* U+0E07 THAI CHARACTER NGO NGU */,

        /// <summary>
        /// The thai chochan
        /// </summary>
        Thai_chochan = 0x0da8 /* U+0E08 THAI CHARACTER CHO CHAN */,

        /// <summary>
        /// The thai choching
        /// </summary>
        Thai_choching = 0x0da9 /* U+0E09 THAI CHARACTER CHO CHING */,

        /// <summary>
        /// The thai chochang
        /// </summary>
        Thai_chochang = 0x0daa /* U+0E0A THAI CHARACTER CHO CHANG */,

        /// <summary>
        /// The thai soso
        /// </summary>
        Thai_soso = 0x0dab /* U+0E0B THAI CHARACTER SO SO */,

        /// <summary>
        /// The thai chochoe
        /// </summary>
        Thai_chochoe = 0x0dac /* U+0E0C THAI CHARACTER CHO CHOE */,

        /// <summary>
        /// The thai yoying
        /// </summary>
        Thai_yoying = 0x0dad /* U+0E0D THAI CHARACTER YO YING */,

        /// <summary>
        /// The thai dochada
        /// </summary>
        Thai_dochada = 0x0dae /* U+0E0E THAI CHARACTER DO CHADA */,

        /// <summary>
        /// The thai topatak
        /// </summary>
        Thai_topatak = 0x0daf /* U+0E0F THAI CHARACTER TO PATAK */,

        /// <summary>
        /// The thai thothan
        /// </summary>
        Thai_thothan = 0x0db0 /* U+0E10 THAI CHARACTER THO THAN */,

        /// <summary>
        /// The thai thonangmontho
        /// </summary>
        Thai_thonangmontho = 0x0db1 /* U+0E11 THAI CHARACTER THO NANGMONTHO */,

        /// <summary>
        /// The thai thophuthao
        /// </summary>
        Thai_thophuthao = 0x0db2 /* U+0E12 THAI CHARACTER THO PHUTHAO */,

        /// <summary>
        /// The thai nonen
        /// </summary>
        Thai_nonen = 0x0db3 /* U+0E13 THAI CHARACTER NO NEN */,

        /// <summary>
        /// The thai dodek
        /// </summary>
        Thai_dodek = 0x0db4 /* U+0E14 THAI CHARACTER DO DEK */,

        /// <summary>
        /// The thai totao
        /// </summary>
        Thai_totao = 0x0db5 /* U+0E15 THAI CHARACTER TO TAO */,

        /// <summary>
        /// The thai thothung
        /// </summary>
        Thai_thothung = 0x0db6 /* U+0E16 THAI CHARACTER THO THUNG */,

        /// <summary>
        /// The thai thothahan
        /// </summary>
        Thai_thothahan = 0x0db7 /* U+0E17 THAI CHARACTER THO THAHAN */,

        /// <summary>
        /// The thai thothong
        /// </summary>
        Thai_thothong = 0x0db8 /* U+0E18 THAI CHARACTER THO THONG */,

        /// <summary>
        /// The thai nonu
        /// </summary>
        Thai_nonu = 0x0db9 /* U+0E19 THAI CHARACTER NO NU */,

        /// <summary>
        /// The thai bobaimai
        /// </summary>
        Thai_bobaimai = 0x0dba /* U+0E1A THAI CHARACTER BO BAIMAI */,

        /// <summary>
        /// The thai popla
        /// </summary>
        Thai_popla = 0x0dbb /* U+0E1B THAI CHARACTER PO PLA */,

        /// <summary>
        /// The thai phophung
        /// </summary>
        Thai_phophung = 0x0dbc /* U+0E1C THAI CHARACTER PHO PHUNG */,

        /// <summary>
        /// The thai fofa
        /// </summary>
        Thai_fofa = 0x0dbd /* U+0E1D THAI CHARACTER FO FA */,

        /// <summary>
        /// The thai phophan
        /// </summary>
        Thai_phophan = 0x0dbe /* U+0E1E THAI CHARACTER PHO PHAN */,

        /// <summary>
        /// The thai fofan
        /// </summary>
        Thai_fofan = 0x0dbf /* U+0E1F THAI CHARACTER FO FAN */,

        /// <summary>
        /// The thai phosamphao
        /// </summary>
        Thai_phosamphao = 0x0dc0 /* U+0E20 THAI CHARACTER PHO SAMPHAO */,

        /// <summary>
        /// The thai moma
        /// </summary>
        Thai_moma = 0x0dc1 /* U+0E21 THAI CHARACTER MO MA */,

        /// <summary>
        /// The thai yoyak
        /// </summary>
        Thai_yoyak = 0x0dc2 /* U+0E22 THAI CHARACTER YO YAK */,

        /// <summary>
        /// The thai rorua
        /// </summary>
        Thai_rorua = 0x0dc3 /* U+0E23 THAI CHARACTER RO RUA */,

        /// <summary>
        /// The thai ru
        /// </summary>
        Thai_ru = 0x0dc4 /* U+0E24 THAI CHARACTER RU */,

        /// <summary>
        /// The thai loling
        /// </summary>
        Thai_loling = 0x0dc5 /* U+0E25 THAI CHARACTER LO LING */,

        /// <summary>
        /// The thai lu
        /// </summary>
        Thai_lu = 0x0dc6 /* U+0E26 THAI CHARACTER LU */,

        /// <summary>
        /// The thai wowaen
        /// </summary>
        Thai_wowaen = 0x0dc7 /* U+0E27 THAI CHARACTER WO WAEN */,

        /// <summary>
        /// The thai sosala
        /// </summary>
        Thai_sosala = 0x0dc8 /* U+0E28 THAI CHARACTER SO SALA */,

        /// <summary>
        /// The thai sorusi
        /// </summary>
        Thai_sorusi = 0x0dc9 /* U+0E29 THAI CHARACTER SO RUSI */,

        /// <summary>
        /// The thai sosua
        /// </summary>
        Thai_sosua = 0x0dca /* U+0E2A THAI CHARACTER SO SUA */,

        /// <summary>
        /// The thai hohip
        /// </summary>
        Thai_hohip = 0x0dcb /* U+0E2B THAI CHARACTER HO HIP */,

        /// <summary>
        /// The thai lochula
        /// </summary>
        Thai_lochula = 0x0dcc /* U+0E2C THAI CHARACTER LO CHULA */,

        /// <summary>
        /// The thai oang
        /// </summary>
        Thai_oang = 0x0dcd /* U+0E2D THAI CHARACTER O ANG */,

        /// <summary>
        /// The thai honokhuk
        /// </summary>
        Thai_honokhuk = 0x0dce /* U+0E2E THAI CHARACTER HO NOKHUK */,

        /// <summary>
        /// The thai paiyannoi
        /// </summary>
        Thai_paiyannoi = 0x0dcf /* U+0E2F THAI CHARACTER PAIYANNOI */,

        /// <summary>
        /// The thai saraa
        /// </summary>
        Thai_saraa = 0x0dd0 /* U+0E30 THAI CHARACTER SARA A */,

        /// <summary>
        /// The thai maihanakat
        /// </summary>
        Thai_maihanakat = 0x0dd1 /* U+0E31 THAI CHARACTER MAI HAN-AKAT */,

        /// <summary>
        /// The thai saraaa
        /// </summary>
        Thai_saraaa = 0x0dd2 /* U+0E32 THAI CHARACTER SARA AA */,

        /// <summary>
        /// The thai saraam
        /// </summary>
        Thai_saraam = 0x0dd3 /* U+0E33 THAI CHARACTER SARA AM */,

        /// <summary>
        /// The thai sarai
        /// </summary>
        Thai_sarai = 0x0dd4 /* U+0E34 THAI CHARACTER SARA I */,

        /// <summary>
        /// The thai saraii
        /// </summary>
        Thai_saraii = 0x0dd5 /* U+0E35 THAI CHARACTER SARA II */,

        /// <summary>
        /// The thai saraue
        /// </summary>
        Thai_saraue = 0x0dd6 /* U+0E36 THAI CHARACTER SARA UE */,

        /// <summary>
        /// The thai sarauee
        /// </summary>
        Thai_sarauee = 0x0dd7 /* U+0E37 THAI CHARACTER SARA UEE */,

        /// <summary>
        /// The thai sarau
        /// </summary>
        Thai_sarau = 0x0dd8 /* U+0E38 THAI CHARACTER SARA U */,

        /// <summary>
        /// The thai sarauu
        /// </summary>
        Thai_sarauu = 0x0dd9 /* U+0E39 THAI CHARACTER SARA UU */,

        /// <summary>
        /// The thai phinthu
        /// </summary>
        Thai_phinthu = 0x0dda /* U+0E3A THAI CHARACTER PHINTHU */,

        /// <summary>
        /// The thai maihanakat maitho
        /// </summary>
        Thai_maihanakat_maitho = 0x0dde,

        /// <summary>
        /// The thai baht
        /// </summary>
        Thai_baht = 0x0ddf /* U+0E3F THAI CURRENCY SYMBOL BAHT */,

        /// <summary>
        /// The thai sarae
        /// </summary>
        Thai_sarae = 0x0de0 /* U+0E40 THAI CHARACTER SARA E */,

        /// <summary>
        /// The thai saraae
        /// </summary>
        Thai_saraae = 0x0de1 /* U+0E41 THAI CHARACTER SARA AE */,

        /// <summary>
        /// The thai sarao
        /// </summary>
        Thai_sarao = 0x0de2 /* U+0E42 THAI CHARACTER SARA O */,

        /// <summary>
        /// The thai saraaimaimuan
        /// </summary>
        Thai_saraaimaimuan = 0x0de3 /* U+0E43 THAI CHARACTER SARA AI MAIMUAN */,

        /// <summary>
        /// The thai saraaimaimalai
        /// </summary>
        Thai_saraaimaimalai = 0x0de4 /* U+0E44 THAI CHARACTER SARA AI MAIMALAI */,

        /// <summary>
        /// The thai lakkhangyao
        /// </summary>
        Thai_lakkhangyao = 0x0de5 /* U+0E45 THAI CHARACTER LAKKHANGYAO */,

        /// <summary>
        /// The thai maiyamok
        /// </summary>
        Thai_maiyamok = 0x0de6 /* U+0E46 THAI CHARACTER MAIYAMOK */,

        /// <summary>
        /// The thai maitaikhu
        /// </summary>
        Thai_maitaikhu = 0x0de7 /* U+0E47 THAI CHARACTER MAITAIKHU */,

        /// <summary>
        /// The thai maiek
        /// </summary>
        Thai_maiek = 0x0de8 /* U+0E48 THAI CHARACTER MAI EK */,

        /// <summary>
        /// The thai maitho
        /// </summary>
        Thai_maitho = 0x0de9 /* U+0E49 THAI CHARACTER MAI THO */,

        /// <summary>
        /// The thai maitri
        /// </summary>
        Thai_maitri = 0x0dea /* U+0E4A THAI CHARACTER MAI TRI */,

        /// <summary>
        /// The thai maichattawa
        /// </summary>
        Thai_maichattawa = 0x0deb /* U+0E4B THAI CHARACTER MAI CHATTAWA */,

        /// <summary>
        /// The thai thanthakhat
        /// </summary>
        Thai_thanthakhat = 0x0dec /* U+0E4C THAI CHARACTER THANTHAKHAT */,

        /// <summary>
        /// The thai nikhahit
        /// </summary>
        Thai_nikhahit = 0x0ded /* U+0E4D THAI CHARACTER NIKHAHIT */,

        /// <summary>
        /// The thai leksun
        /// </summary>
        Thai_leksun = 0x0df0 /* U+0E50 THAI DIGIT ZERO */,

        /// <summary>
        /// The thai leknung
        /// </summary>
        Thai_leknung = 0x0df1 /* U+0E51 THAI DIGIT ONE */,

        /// <summary>
        /// The thai leksong
        /// </summary>
        Thai_leksong = 0x0df2 /* U+0E52 THAI DIGIT TWO */,

        /// <summary>
        /// The thai leksam
        /// </summary>
        Thai_leksam = 0x0df3 /* U+0E53 THAI DIGIT THREE */,

        /// <summary>
        /// The thai leksi
        /// </summary>
        Thai_leksi = 0x0df4 /* U+0E54 THAI DIGIT FOUR */,

        /// <summary>
        /// The thai lekha
        /// </summary>
        Thai_lekha = 0x0df5 /* U+0E55 THAI DIGIT FIVE */,

        /// <summary>
        /// The thai lekhok
        /// </summary>
        Thai_lekhok = 0x0df6 /* U+0E56 THAI DIGIT SIX */,

        /// <summary>
        /// The thai lekchet
        /// </summary>
        Thai_lekchet = 0x0df7 /* U+0E57 THAI DIGIT SEVEN */,

        /// <summary>
        /// The thai lekpaet
        /// </summary>
        Thai_lekpaet = 0x0df8 /* U+0E58 THAI DIGIT EIGHT */,

        /// <summary>
        /// The thai lekkao
        /// </summary>
        Thai_lekkao = 0x0df9 /* U+0E59 THAI DIGIT NINE */,

        /// <summary>
        /// The hangul
        /// </summary>
        Hangul = 0xff31 /* Hangul start/stop(toggle) */,

        /// <summary>
        /// The hangul start
        /// </summary>
        Hangul_Start = 0xff32 /* Hangul start */,

        /// <summary>
        /// The hangul end
        /// </summary>
        Hangul_End = 0xff33 /* Hangul end, English start */,

        /// <summary>
        /// The hangul hanja
        /// </summary>
        Hangul_Hanja = 0xff34 /* Start Hangul->Hanja Conversion */,

        /// <summary>
        /// The hangul jamo
        /// </summary>
        Hangul_Jamo = 0xff35 /* Hangul Jamo mode */,

        /// <summary>
        /// The hangul romaja
        /// </summary>
        Hangul_Romaja = 0xff36 /* Hangul Romaja mode */,

        /// <summary>
        /// The hangul codeinput
        /// </summary>
        Hangul_Codeinput = 0xff37 /* Hangul code input mode */,

        /// <summary>
        /// The hangul jeonja
        /// </summary>
        Hangul_Jeonja = 0xff38 /* Jeonja mode */,

        /// <summary>
        /// The hangul banja
        /// </summary>
        Hangul_Banja = 0xff39 /* Banja mode */,

        /// <summary>
        /// The hangul pre hanja
        /// </summary>
        Hangul_PreHanja = 0xff3a /* Pre Hanja conversion */,

        /// <summary>
        /// The hangul post hanja
        /// </summary>
        Hangul_PostHanja = 0xff3b /* Post Hanja conversion */,

        /// <summary>
        /// The hangul single candidate
        /// </summary>
        Hangul_SingleCandidate = 0xff3c /* Single candidate */,

        /// <summary>
        /// The hangul multiple candidate
        /// </summary>
        Hangul_MultipleCandidate = 0xff3d /* Multiple candidate */,

        /// <summary>
        /// The hangul previous candidate
        /// </summary>
        Hangul_PreviousCandidate = 0xff3e /* Previous candidate */,

        /// <summary>
        /// The hangul special
        /// </summary>
        Hangul_Special = 0xff3f /* Special symbols */,

        /// <summary>
        /// The hangul switch
        /// </summary>
        Hangul_switch = 0xff7e /* Alias for mode_switch */,

        /// <summary>
        /// The hangul kiyeog
        /// </summary>
        Hangul_Kiyeog = 0x0ea1,

        /// <summary>
        /// The hangul ssang kiyeog
        /// </summary>
        Hangul_SsangKiyeog = 0x0ea2,

        /// <summary>
        /// The hangul kiyeog sios
        /// </summary>
        Hangul_KiyeogSios = 0x0ea3,

        /// <summary>
        /// The hangul nieun
        /// </summary>
        Hangul_Nieun = 0x0ea4,

        /// <summary>
        /// The hangul nieun jieuj
        /// </summary>
        Hangul_NieunJieuj = 0x0ea5,

        /// <summary>
        /// The hangul nieun hieuh
        /// </summary>
        Hangul_NieunHieuh = 0x0ea6,

        /// <summary>
        /// The hangul dikeud
        /// </summary>
        Hangul_Dikeud = 0x0ea7,

        /// <summary>
        /// The hangul ssang dikeud
        /// </summary>
        Hangul_SsangDikeud = 0x0ea8,

        /// <summary>
        /// The hangul rieul
        /// </summary>
        Hangul_Rieul = 0x0ea9,

        /// <summary>
        /// The hangul rieul kiyeog
        /// </summary>
        Hangul_RieulKiyeog = 0x0eaa,

        /// <summary>
        /// The hangul rieul mieum
        /// </summary>
        Hangul_RieulMieum = 0x0eab,

        /// <summary>
        /// The hangul rieul pieub
        /// </summary>
        Hangul_RieulPieub = 0x0eac,

        /// <summary>
        /// The hangul rieul sios
        /// </summary>
        Hangul_RieulSios = 0x0ead,

        /// <summary>
        /// The hangul rieul tieut
        /// </summary>
        Hangul_RieulTieut = 0x0eae,

        /// <summary>
        /// The hangul rieul phieuf
        /// </summary>
        Hangul_RieulPhieuf = 0x0eaf,

        /// <summary>
        /// The hangul rieul hieuh
        /// </summary>
        Hangul_RieulHieuh = 0x0eb0,

        /// <summary>
        /// The hangul mieum
        /// </summary>
        Hangul_Mieum = 0x0eb1,

        /// <summary>
        /// The hangul pieub
        /// </summary>
        Hangul_Pieub = 0x0eb2,

        /// <summary>
        /// The hangul ssang pieub
        /// </summary>
        Hangul_SsangPieub = 0x0eb3,

        /// <summary>
        /// The hangul pieub sios
        /// </summary>
        Hangul_PieubSios = 0x0eb4,

        /// <summary>
        /// The hangul sios
        /// </summary>
        Hangul_Sios = 0x0eb5,

        /// <summary>
        /// The hangul ssang sios
        /// </summary>
        Hangul_SsangSios = 0x0eb6,

        /// <summary>
        /// The hangul ieung
        /// </summary>
        Hangul_Ieung = 0x0eb7,

        /// <summary>
        /// The hangul jieuj
        /// </summary>
        Hangul_Jieuj = 0x0eb8,

        /// <summary>
        /// The hangul ssang jieuj
        /// </summary>
        Hangul_SsangJieuj = 0x0eb9,

        /// <summary>
        /// The hangul cieuc
        /// </summary>
        Hangul_Cieuc = 0x0eba,

        /// <summary>
        /// The hangul khieuq
        /// </summary>
        Hangul_Khieuq = 0x0ebb,

        /// <summary>
        /// The hangul tieut
        /// </summary>
        Hangul_Tieut = 0x0ebc,

        /// <summary>
        /// The hangul phieuf
        /// </summary>
        Hangul_Phieuf = 0x0ebd,

        /// <summary>
        /// The hangul hieuh
        /// </summary>
        Hangul_Hieuh = 0x0ebe,

        /// <summary>
        /// The hangul a
        /// </summary>
        Hangul_A = 0x0ebf,

        /// <summary>
        /// The hangul ae
        /// </summary>
        Hangul_AE = 0x0ec0,

        /// <summary>
        /// The hangul ya
        /// </summary>
        Hangul_YA = 0x0ec1,

        /// <summary>
        /// The hangul yae
        /// </summary>
        Hangul_YAE = 0x0ec2,

        /// <summary>
        /// The hangul eo
        /// </summary>
        Hangul_EO = 0x0ec3,

        /// <summary>
        /// The hangul e
        /// </summary>
        Hangul_E = 0x0ec4,

        /// <summary>
        /// The hangul yeo
        /// </summary>
        Hangul_YEO = 0x0ec5,

        /// <summary>
        /// The hangul ye
        /// </summary>
        Hangul_YE = 0x0ec6,

        /// <summary>
        /// The hangul o
        /// </summary>
        Hangul_O = 0x0ec7,

        /// <summary>
        /// The hangul wa
        /// </summary>
        Hangul_WA = 0x0ec8,

        /// <summary>
        /// The hangul wae
        /// </summary>
        Hangul_WAE = 0x0ec9,

        /// <summary>
        /// The hangul oe
        /// </summary>
        Hangul_OE = 0x0eca,

        /// <summary>
        /// The hangul yo
        /// </summary>
        Hangul_YO = 0x0ecb,

        /// <summary>
        /// The hangul u
        /// </summary>
        Hangul_U = 0x0ecc,

        /// <summary>
        /// The hangul weo
        /// </summary>
        Hangul_WEO = 0x0ecd,

        /// <summary>
        /// The hangul we
        /// </summary>
        Hangul_WE = 0x0ece,

        /// <summary>
        /// The hangul wi
        /// </summary>
        Hangul_WI = 0x0ecf,

        /// <summary>
        /// The hangul yu
        /// </summary>
        Hangul_YU = 0x0ed0,

        /// <summary>
        /// The hangul eu
        /// </summary>
        Hangul_EU = 0x0ed1,

        /// <summary>
        /// The hangul yi
        /// </summary>
        Hangul_YI = 0x0ed2,

        /// <summary>
        /// The hangul i
        /// </summary>
        Hangul_I = 0x0ed3,

        /// <summary>
        /// The hangul j kiyeog
        /// </summary>
        Hangul_J_Kiyeog = 0x0ed4,

        /// <summary>
        /// The hangul j ssang kiyeog
        /// </summary>
        Hangul_J_SsangKiyeog = 0x0ed5,

        /// <summary>
        /// The hangul j kiyeog sios
        /// </summary>
        Hangul_J_KiyeogSios = 0x0ed6,

        /// <summary>
        /// The hangul j nieun
        /// </summary>
        Hangul_J_Nieun = 0x0ed7,

        /// <summary>
        /// The hangul j nieun jieuj
        /// </summary>
        Hangul_J_NieunJieuj = 0x0ed8,

        /// <summary>
        /// The hangul j nieun hieuh
        /// </summary>
        Hangul_J_NieunHieuh = 0x0ed9,

        /// <summary>
        /// The hangul j dikeud
        /// </summary>
        Hangul_J_Dikeud = 0x0eda,

        /// <summary>
        /// The hangul j rieul
        /// </summary>
        Hangul_J_Rieul = 0x0edb,

        /// <summary>
        /// The hangul j rieul kiyeog
        /// </summary>
        Hangul_J_RieulKiyeog = 0x0edc,

        /// <summary>
        /// The hangul j rieul mieum
        /// </summary>
        Hangul_J_RieulMieum = 0x0edd,

        /// <summary>
        /// The hangul j rieul pieub
        /// </summary>
        Hangul_J_RieulPieub = 0x0ede,

        /// <summary>
        /// The hangul j rieul sios
        /// </summary>
        Hangul_J_RieulSios = 0x0edf,

        /// <summary>
        /// The hangul j rieul tieut
        /// </summary>
        Hangul_J_RieulTieut = 0x0ee0,

        /// <summary>
        /// The hangul j rieul phieuf
        /// </summary>
        Hangul_J_RieulPhieuf = 0x0ee1,

        /// <summary>
        /// The hangul j rieul hieuh
        /// </summary>
        Hangul_J_RieulHieuh = 0x0ee2,

        /// <summary>
        /// The hangul j mieum
        /// </summary>
        Hangul_J_Mieum = 0x0ee3,

        /// <summary>
        /// The hangul j pieub
        /// </summary>
        Hangul_J_Pieub = 0x0ee4,

        /// <summary>
        /// The hangul j pieub sios
        /// </summary>
        Hangul_J_PieubSios = 0x0ee5,

        /// <summary>
        /// The hangul j sios
        /// </summary>
        Hangul_J_Sios = 0x0ee6,

        /// <summary>
        /// The hangul j ssang sios
        /// </summary>
        Hangul_J_SsangSios = 0x0ee7,

        /// <summary>
        /// The hangul j ieung
        /// </summary>
        Hangul_J_Ieung = 0x0ee8,

        /// <summary>
        /// The hangul j jieuj
        /// </summary>
        Hangul_J_Jieuj = 0x0ee9,

        /// <summary>
        /// The hangul j cieuc
        /// </summary>
        Hangul_J_Cieuc = 0x0eea,

        /// <summary>
        /// The hangul j khieuq
        /// </summary>
        Hangul_J_Khieuq = 0x0eeb,

        /// <summary>
        /// The hangul j tieut
        /// </summary>
        Hangul_J_Tieut = 0x0eec,

        /// <summary>
        /// The hangul j phieuf
        /// </summary>
        Hangul_J_Phieuf = 0x0eed,

        /// <summary>
        /// The hangul j hieuh
        /// </summary>
        Hangul_J_Hieuh = 0x0eee,

        /// <summary>
        /// The hangul rieul yeorin hieuh
        /// </summary>
        Hangul_RieulYeorinHieuh = 0x0eef,

        /// <summary>
        /// The hangul sunkyeongeum mieum
        /// </summary>
        Hangul_SunkyeongeumMieum = 0x0ef0,

        /// <summary>
        /// The hangul sunkyeongeum pieub
        /// </summary>
        Hangul_SunkyeongeumPieub = 0x0ef1,

        /// <summary>
        /// The hangul pan sios
        /// </summary>
        Hangul_PanSios = 0x0ef2,

        /// <summary>
        /// The hangul kkogji dalrin ieung
        /// </summary>
        Hangul_KkogjiDalrinIeung = 0x0ef3,

        /// <summary>
        /// The hangul sunkyeongeum phieuf
        /// </summary>
        Hangul_SunkyeongeumPhieuf = 0x0ef4,

        /// <summary>
        /// The hangul yeorin hieuh
        /// </summary>
        Hangul_YeorinHieuh = 0x0ef5,

        /// <summary>
        /// The hangul arae a
        /// </summary>
        Hangul_AraeA = 0x0ef6,

        /// <summary>
        /// The hangul arae ae
        /// </summary>
        Hangul_AraeAE = 0x0ef7,

        /// <summary>
        /// The hangul j pan sios
        /// </summary>
        Hangul_J_PanSios = 0x0ef8,

        /// <summary>
        /// The hangul j kkogji dalrin ieung
        /// </summary>
        Hangul_J_KkogjiDalrinIeung = 0x0ef9,

        /// <summary>
        /// The hangul j yeorin hieuh
        /// </summary>
        Hangul_J_YeorinHieuh = 0x0efa,

        /// <summary>
        /// The korean won
        /// </summary>
        Korean_Won = 0x0eff /*(U+20A9 WON SIGN)*/,

        /// <summary>
        /// The armenian ligature ew
        /// </summary>
        Armenian_ligature_ew = 0x1000587 /* U+0587 ARMENIAN SMALL LIGATURE ECH YIWN */,

        /// <summary>
        /// The armenian full stop
        /// </summary>
        Armenian_full_stop = 0x1000589 /* U+0589 ARMENIAN FULL STOP */,

        /// <summary>
        /// The armenian verjaket
        /// </summary>
        Armenian_verjaket = 0x1000589 /* U+0589 ARMENIAN FULL STOP */,

        /// <summary>
        /// The armenian separation mark
        /// </summary>
        Armenian_separation_mark = 0x100055d /* U+055D ARMENIAN COMMA */,

        /// <summary>
        /// The armenian but
        /// </summary>
        Armenian_but = 0x100055d /* U+055D ARMENIAN COMMA */,

        /// <summary>
        /// The armenian hyphen
        /// </summary>
        Armenian_hyphen = 0x100058a /* U+058A ARMENIAN HYPHEN */,

        /// <summary>
        /// The armenian yentamna
        /// </summary>
        Armenian_yentamna = 0x100058a /* U+058A ARMENIAN HYPHEN */,

        /// <summary>
        /// The armenian exclam
        /// </summary>
        Armenian_exclam = 0x100055c /* U+055C ARMENIAN EXCLAMATION MARK */,

        /// <summary>
        /// The armenian amanak
        /// </summary>
        Armenian_amanak = 0x100055c /* U+055C ARMENIAN EXCLAMATION MARK */,

        /// <summary>
        /// The armenian accent
        /// </summary>
        Armenian_accent = 0x100055b /* U+055B ARMENIAN EMPHASIS MARK */,

        /// <summary>
        /// The armenian shesht
        /// </summary>
        Armenian_shesht = 0x100055b /* U+055B ARMENIAN EMPHASIS MARK */,

        /// <summary>
        /// The armenian question
        /// </summary>
        Armenian_question = 0x100055e /* U+055E ARMENIAN QUESTION MARK */,

        /// <summary>
        /// The armenian paruyk
        /// </summary>
        Armenian_paruyk = 0x100055e /* U+055E ARMENIAN QUESTION MARK */,

        /// <summary>
        /// The armenian ayb
        /// </summary>
        Armenian_AYB = 0x1000531 /* U+0531 ARMENIAN CAPITAL LETTER AYB */,

        /// <summary>
        /// The armenian ayb
        /// </summary>
        Armenian_ayb = 0x1000561 /* U+0561 ARMENIAN SMALL LETTER AYB */,

        /// <summary>
        /// The armenian ben
        /// </summary>
        Armenian_BEN = 0x1000532 /* U+0532 ARMENIAN CAPITAL LETTER BEN */,

        /// <summary>
        /// The armenian ben
        /// </summary>
        Armenian_ben = 0x1000562 /* U+0562 ARMENIAN SMALL LETTER BEN */,

        /// <summary>
        /// The armenian gim
        /// </summary>
        Armenian_GIM = 0x1000533 /* U+0533 ARMENIAN CAPITAL LETTER GIM */,

        /// <summary>
        /// The armenian gim
        /// </summary>
        Armenian_gim = 0x1000563 /* U+0563 ARMENIAN SMALL LETTER GIM */,

        /// <summary>
        /// The armenian da
        /// </summary>
        Armenian_DA = 0x1000534 /* U+0534 ARMENIAN CAPITAL LETTER DA */,

        /// <summary>
        /// The armenian da
        /// </summary>
        Armenian_da = 0x1000564 /* U+0564 ARMENIAN SMALL LETTER DA */,

        /// <summary>
        /// The armenian yech
        /// </summary>
        Armenian_YECH = 0x1000535 /* U+0535 ARMENIAN CAPITAL LETTER ECH */,

        /// <summary>
        /// The armenian yech
        /// </summary>
        Armenian_yech = 0x1000565 /* U+0565 ARMENIAN SMALL LETTER ECH */,

        /// <summary>
        /// The armenian za
        /// </summary>
        Armenian_ZA = 0x1000536 /* U+0536 ARMENIAN CAPITAL LETTER ZA */,

        /// <summary>
        /// The armenian za
        /// </summary>
        Armenian_za = 0x1000566 /* U+0566 ARMENIAN SMALL LETTER ZA */,

        /// <summary>
        /// The armenian e
        /// </summary>
        Armenian_E = 0x1000537 /* U+0537 ARMENIAN CAPITAL LETTER EH */,

        /// <summary>
        /// The armenian e
        /// </summary>
        Armenian_e = 0x1000567 /* U+0567 ARMENIAN SMALL LETTER EH */,

        /// <summary>
        /// The armenian at
        /// </summary>
        Armenian_AT = 0x1000538 /* U+0538 ARMENIAN CAPITAL LETTER ET */,

        /// <summary>
        /// The armenian at
        /// </summary>
        Armenian_at = 0x1000568 /* U+0568 ARMENIAN SMALL LETTER ET */,

        /// <summary>
        /// The armenian to
        /// </summary>
        Armenian_TO = 0x1000539 /* U+0539 ARMENIAN CAPITAL LETTER TO */,

        /// <summary>
        /// The armenian to
        /// </summary>
        Armenian_to = 0x1000569 /* U+0569 ARMENIAN SMALL LETTER TO */,

        /// <summary>
        /// The armenian zhe
        /// </summary>
        Armenian_ZHE = 0x100053a /* U+053A ARMENIAN CAPITAL LETTER ZHE */,

        /// <summary>
        /// The armenian zhe
        /// </summary>
        Armenian_zhe = 0x100056a /* U+056A ARMENIAN SMALL LETTER ZHE */,

        /// <summary>
        /// The armenian ini
        /// </summary>
        Armenian_INI = 0x100053b /* U+053B ARMENIAN CAPITAL LETTER INI */,

        /// <summary>
        /// The armenian ini
        /// </summary>
        Armenian_ini = 0x100056b /* U+056B ARMENIAN SMALL LETTER INI */,

        /// <summary>
        /// The armenian lyun
        /// </summary>
        Armenian_LYUN = 0x100053c /* U+053C ARMENIAN CAPITAL LETTER LIWN */,

        /// <summary>
        /// The armenian lyun
        /// </summary>
        Armenian_lyun = 0x100056c /* U+056C ARMENIAN SMALL LETTER LIWN */,

        /// <summary>
        /// The armenian khe
        /// </summary>
        Armenian_KHE = 0x100053d /* U+053D ARMENIAN CAPITAL LETTER XEH */,

        /// <summary>
        /// The armenian khe
        /// </summary>
        Armenian_khe = 0x100056d /* U+056D ARMENIAN SMALL LETTER XEH */,

        /// <summary>
        /// The armenian tsa
        /// </summary>
        Armenian_TSA = 0x100053e /* U+053E ARMENIAN CAPITAL LETTER CA */,

        /// <summary>
        /// The armenian tsa
        /// </summary>
        Armenian_tsa = 0x100056e /* U+056E ARMENIAN SMALL LETTER CA */,

        /// <summary>
        /// The armenian ken
        /// </summary>
        Armenian_KEN = 0x100053f /* U+053F ARMENIAN CAPITAL LETTER KEN */,

        /// <summary>
        /// The armenian ken
        /// </summary>
        Armenian_ken = 0x100056f /* U+056F ARMENIAN SMALL LETTER KEN */,

        /// <summary>
        /// The armenian ho
        /// </summary>
        Armenian_HO = 0x1000540 /* U+0540 ARMENIAN CAPITAL LETTER HO */,

        /// <summary>
        /// The armenian ho
        /// </summary>
        Armenian_ho = 0x1000570 /* U+0570 ARMENIAN SMALL LETTER HO */,

        /// <summary>
        /// The armenian dza
        /// </summary>
        Armenian_DZA = 0x1000541 /* U+0541 ARMENIAN CAPITAL LETTER JA */,

        /// <summary>
        /// The armenian dza
        /// </summary>
        Armenian_dza = 0x1000571 /* U+0571 ARMENIAN SMALL LETTER JA */,

        /// <summary>
        /// The armenian ghat
        /// </summary>
        Armenian_GHAT = 0x1000542 /* U+0542 ARMENIAN CAPITAL LETTER GHAD */,

        /// <summary>
        /// The armenian ghat
        /// </summary>
        Armenian_ghat = 0x1000572 /* U+0572 ARMENIAN SMALL LETTER GHAD */,

        /// <summary>
        /// The armenian tche
        /// </summary>
        Armenian_TCHE = 0x1000543 /* U+0543 ARMENIAN CAPITAL LETTER CHEH */,

        /// <summary>
        /// The armenian tche
        /// </summary>
        Armenian_tche = 0x1000573 /* U+0573 ARMENIAN SMALL LETTER CHEH */,

        /// <summary>
        /// The armenian men
        /// </summary>
        Armenian_MEN = 0x1000544 /* U+0544 ARMENIAN CAPITAL LETTER MEN */,

        /// <summary>
        /// The armenian men
        /// </summary>
        Armenian_men = 0x1000574 /* U+0574 ARMENIAN SMALL LETTER MEN */,

        /// <summary>
        /// The armenian hi
        /// </summary>
        Armenian_HI = 0x1000545 /* U+0545 ARMENIAN CAPITAL LETTER YI */,

        /// <summary>
        /// The armenian hi
        /// </summary>
        Armenian_hi = 0x1000575 /* U+0575 ARMENIAN SMALL LETTER YI */,

        /// <summary>
        /// The armenian nu
        /// </summary>
        Armenian_NU = 0x1000546 /* U+0546 ARMENIAN CAPITAL LETTER NOW */,

        /// <summary>
        /// The armenian nu
        /// </summary>
        Armenian_nu = 0x1000576 /* U+0576 ARMENIAN SMALL LETTER NOW */,

        /// <summary>
        /// The armenian sha
        /// </summary>
        Armenian_SHA = 0x1000547 /* U+0547 ARMENIAN CAPITAL LETTER SHA */,

        /// <summary>
        /// The armenian sha
        /// </summary>
        Armenian_sha = 0x1000577 /* U+0577 ARMENIAN SMALL LETTER SHA */,

        /// <summary>
        /// The armenian vo
        /// </summary>
        Armenian_VO = 0x1000548 /* U+0548 ARMENIAN CAPITAL LETTER VO */,

        /// <summary>
        /// The armenian vo
        /// </summary>
        Armenian_vo = 0x1000578 /* U+0578 ARMENIAN SMALL LETTER VO */,

        /// <summary>
        /// The armenian cha
        /// </summary>
        Armenian_CHA = 0x1000549 /* U+0549 ARMENIAN CAPITAL LETTER CHA */,

        /// <summary>
        /// The armenian cha
        /// </summary>
        Armenian_cha = 0x1000579 /* U+0579 ARMENIAN SMALL LETTER CHA */,

        /// <summary>
        /// The armenian pe
        /// </summary>
        Armenian_PE = 0x100054a /* U+054A ARMENIAN CAPITAL LETTER PEH */,

        /// <summary>
        /// The armenian pe
        /// </summary>
        Armenian_pe = 0x100057a /* U+057A ARMENIAN SMALL LETTER PEH */,

        /// <summary>
        /// The armenian je
        /// </summary>
        Armenian_JE = 0x100054b /* U+054B ARMENIAN CAPITAL LETTER JHEH */,

        /// <summary>
        /// The armenian je
        /// </summary>
        Armenian_je = 0x100057b /* U+057B ARMENIAN SMALL LETTER JHEH */,

        /// <summary>
        /// The armenian ra
        /// </summary>
        Armenian_RA = 0x100054c /* U+054C ARMENIAN CAPITAL LETTER RA */,

        /// <summary>
        /// The armenian ra
        /// </summary>
        Armenian_ra = 0x100057c /* U+057C ARMENIAN SMALL LETTER RA */,

        /// <summary>
        /// The armenian se
        /// </summary>
        Armenian_SE = 0x100054d /* U+054D ARMENIAN CAPITAL LETTER SEH */,

        /// <summary>
        /// The armenian se
        /// </summary>
        Armenian_se = 0x100057d /* U+057D ARMENIAN SMALL LETTER SEH */,

        /// <summary>
        /// The armenian vev
        /// </summary>
        Armenian_VEV = 0x100054e /* U+054E ARMENIAN CAPITAL LETTER VEW */,

        /// <summary>
        /// The armenian vev
        /// </summary>
        Armenian_vev = 0x100057e /* U+057E ARMENIAN SMALL LETTER VEW */,

        /// <summary>
        /// The armenian tyun
        /// </summary>
        Armenian_TYUN = 0x100054f /* U+054F ARMENIAN CAPITAL LETTER TIWN */,

        /// <summary>
        /// The armenian tyun
        /// </summary>
        Armenian_tyun = 0x100057f /* U+057F ARMENIAN SMALL LETTER TIWN */,

        /// <summary>
        /// The armenian re
        /// </summary>
        Armenian_RE = 0x1000550 /* U+0550 ARMENIAN CAPITAL LETTER REH */,

        /// <summary>
        /// The armenian re
        /// </summary>
        Armenian_re = 0x1000580 /* U+0580 ARMENIAN SMALL LETTER REH */,

        /// <summary>
        /// The armenian tso
        /// </summary>
        Armenian_TSO = 0x1000551 /* U+0551 ARMENIAN CAPITAL LETTER CO */,

        /// <summary>
        /// The armenian tso
        /// </summary>
        Armenian_tso = 0x1000581 /* U+0581 ARMENIAN SMALL LETTER CO */,

        /// <summary>
        /// The armenian vyun
        /// </summary>
        Armenian_VYUN = 0x1000552 /* U+0552 ARMENIAN CAPITAL LETTER YIWN */,

        /// <summary>
        /// The armenian vyun
        /// </summary>
        Armenian_vyun = 0x1000582 /* U+0582 ARMENIAN SMALL LETTER YIWN */,

        /// <summary>
        /// The armenian pyur
        /// </summary>
        Armenian_PYUR = 0x1000553 /* U+0553 ARMENIAN CAPITAL LETTER PIWR */,

        /// <summary>
        /// The armenian pyur
        /// </summary>
        Armenian_pyur = 0x1000583 /* U+0583 ARMENIAN SMALL LETTER PIWR */,

        /// <summary>
        /// The armenian ke
        /// </summary>
        Armenian_KE = 0x1000554 /* U+0554 ARMENIAN CAPITAL LETTER KEH */,

        /// <summary>
        /// The armenian ke
        /// </summary>
        Armenian_ke = 0x1000584 /* U+0584 ARMENIAN SMALL LETTER KEH */,

        /// <summary>
        /// The armenian o
        /// </summary>
        Armenian_O = 0x1000555 /* U+0555 ARMENIAN CAPITAL LETTER OH */,

        /// <summary>
        /// The armenian o
        /// </summary>
        Armenian_o = 0x1000585 /* U+0585 ARMENIAN SMALL LETTER OH */,

        /// <summary>
        /// The armenian fe
        /// </summary>
        Armenian_FE = 0x1000556 /* U+0556 ARMENIAN CAPITAL LETTER FEH */,

        /// <summary>
        /// The armenian fe
        /// </summary>
        Armenian_fe = 0x1000586 /* U+0586 ARMENIAN SMALL LETTER FEH */,

        /// <summary>
        /// The armenian apostrophe
        /// </summary>
        Armenian_apostrophe = 0x100055a /* U+055A ARMENIAN APOSTROPHE */,

        /// <summary>
        /// The georgian an
        /// </summary>
        Georgian_an = 0x10010d0 /* U+10D0 GEORGIAN LETTER AN */,

        /// <summary>
        /// The georgian ban
        /// </summary>
        Georgian_ban = 0x10010d1 /* U+10D1 GEORGIAN LETTER BAN */,

        /// <summary>
        /// The georgian gan
        /// </summary>
        Georgian_gan = 0x10010d2 /* U+10D2 GEORGIAN LETTER GAN */,

        /// <summary>
        /// The georgian don
        /// </summary>
        Georgian_don = 0x10010d3 /* U+10D3 GEORGIAN LETTER DON */,

        /// <summary>
        /// The georgian en
        /// </summary>
        Georgian_en = 0x10010d4 /* U+10D4 GEORGIAN LETTER EN */,

        /// <summary>
        /// The georgian vin
        /// </summary>
        Georgian_vin = 0x10010d5 /* U+10D5 GEORGIAN LETTER VIN */,

        /// <summary>
        /// The georgian zen
        /// </summary>
        Georgian_zen = 0x10010d6 /* U+10D6 GEORGIAN LETTER ZEN */,

        /// <summary>
        /// The georgian tan
        /// </summary>
        Georgian_tan = 0x10010d7 /* U+10D7 GEORGIAN LETTER TAN */,

        /// <summary>
        /// The georgian in
        /// </summary>
        Georgian_in = 0x10010d8 /* U+10D8 GEORGIAN LETTER IN */,

        /// <summary>
        /// The georgian kan
        /// </summary>
        Georgian_kan = 0x10010d9 /* U+10D9 GEORGIAN LETTER KAN */,

        /// <summary>
        /// The georgian las
        /// </summary>
        Georgian_las = 0x10010da /* U+10DA GEORGIAN LETTER LAS */,

        /// <summary>
        /// The georgian man
        /// </summary>
        Georgian_man = 0x10010db /* U+10DB GEORGIAN LETTER MAN */,

        /// <summary>
        /// The georgian nar
        /// </summary>
        Georgian_nar = 0x10010dc /* U+10DC GEORGIAN LETTER NAR */,

        /// <summary>
        /// The georgian on
        /// </summary>
        Georgian_on = 0x10010dd /* U+10DD GEORGIAN LETTER ON */,

        /// <summary>
        /// The georgian par
        /// </summary>
        Georgian_par = 0x10010de /* U+10DE GEORGIAN LETTER PAR */,

        /// <summary>
        /// The georgian zhar
        /// </summary>
        Georgian_zhar = 0x10010df /* U+10DF GEORGIAN LETTER ZHAR */,

        /// <summary>
        /// The georgian rae
        /// </summary>
        Georgian_rae = 0x10010e0 /* U+10E0 GEORGIAN LETTER RAE */,

        /// <summary>
        /// The georgian san
        /// </summary>
        Georgian_san = 0x10010e1 /* U+10E1 GEORGIAN LETTER SAN */,

        /// <summary>
        /// The georgian tar
        /// </summary>
        Georgian_tar = 0x10010e2 /* U+10E2 GEORGIAN LETTER TAR */,

        /// <summary>
        /// The georgian un
        /// </summary>
        Georgian_un = 0x10010e3 /* U+10E3 GEORGIAN LETTER UN */,

        /// <summary>
        /// The georgian phar
        /// </summary>
        Georgian_phar = 0x10010e4 /* U+10E4 GEORGIAN LETTER PHAR */,

        /// <summary>
        /// The georgian khar
        /// </summary>
        Georgian_khar = 0x10010e5 /* U+10E5 GEORGIAN LETTER KHAR */,

        /// <summary>
        /// The georgian ghan
        /// </summary>
        Georgian_ghan = 0x10010e6 /* U+10E6 GEORGIAN LETTER GHAN */,

        /// <summary>
        /// The georgian qar
        /// </summary>
        Georgian_qar = 0x10010e7 /* U+10E7 GEORGIAN LETTER QAR */,

        /// <summary>
        /// The georgian shin
        /// </summary>
        Georgian_shin = 0x10010e8 /* U+10E8 GEORGIAN LETTER SHIN */,

        /// <summary>
        /// The georgian chin
        /// </summary>
        Georgian_chin = 0x10010e9 /* U+10E9 GEORGIAN LETTER CHIN */,

        /// <summary>
        /// The georgian can
        /// </summary>
        Georgian_can = 0x10010ea /* U+10EA GEORGIAN LETTER CAN */,

        /// <summary>
        /// The georgian jil
        /// </summary>
        Georgian_jil = 0x10010eb /* U+10EB GEORGIAN LETTER JIL */,

        /// <summary>
        /// The georgian cil
        /// </summary>
        Georgian_cil = 0x10010ec /* U+10EC GEORGIAN LETTER CIL */,

        /// <summary>
        /// The georgian character
        /// </summary>
        Georgian_char = 0x10010ed /* U+10ED GEORGIAN LETTER CHAR */,

        /// <summary>
        /// The georgian xan
        /// </summary>
        Georgian_xan = 0x10010ee /* U+10EE GEORGIAN LETTER XAN */,

        /// <summary>
        /// The georgian jhan
        /// </summary>
        Georgian_jhan = 0x10010ef /* U+10EF GEORGIAN LETTER JHAN */,

        /// <summary>
        /// The georgian hae
        /// </summary>
        Georgian_hae = 0x10010f0 /* U+10F0 GEORGIAN LETTER HAE */,

        /// <summary>
        /// The georgian he
        /// </summary>
        Georgian_he = 0x10010f1 /* U+10F1 GEORGIAN LETTER HE */,

        /// <summary>
        /// The georgian hie
        /// </summary>
        Georgian_hie = 0x10010f2 /* U+10F2 GEORGIAN LETTER HIE */,

        /// <summary>
        /// The georgian we
        /// </summary>
        Georgian_we = 0x10010f3 /* U+10F3 GEORGIAN LETTER WE */,

        /// <summary>
        /// The georgian har
        /// </summary>
        Georgian_har = 0x10010f4 /* U+10F4 GEORGIAN LETTER HAR */,

        /// <summary>
        /// The georgian hoe
        /// </summary>
        Georgian_hoe = 0x10010f5 /* U+10F5 GEORGIAN LETTER HOE */,

        /// <summary>
        /// The georgian fi
        /// </summary>
        Georgian_fi = 0x10010f6 /* U+10F6 GEORGIAN LETTER FI */,

        /// <summary>
        /// The xabovedot
        /// </summary>
        Xabovedot = 0x1001e8a /* U+1E8A LATIN CAPITAL LETTER X WITH DOT ABOVE */,

        /// <summary>
        /// The ibreve
        /// </summary>
        Ibreve = 0x100012c /* U+012C LATIN CAPITAL LETTER I WITH BREVE */,

        /// <summary>
        /// The zstroke
        /// </summary>
        Zstroke = 0x10001b5 /* U+01B5 LATIN CAPITAL LETTER Z WITH STROKE */,

        /// <summary>
        /// The gcaron
        /// </summary>
        Gcaron = 0x10001e6 /* U+01E6 LATIN CAPITAL LETTER G WITH CARON */,

        /// <summary>
        /// The ocaron
        /// </summary>
        Ocaron = 0x10001d1 /* U+01D2 LATIN CAPITAL LETTER O WITH CARON */,

        /// <summary>
        /// The obarred
        /// </summary>
        Obarred = 0x100019f /* U+019F LATIN CAPITAL LETTER O WITH MIDDLE TILDE */,

        /// <summary>
        /// The xabovedot
        /// </summary>
        xabovedot = 0x1001e8b /* U+1E8B LATIN SMALL LETTER X WITH DOT ABOVE */,

        /// <summary>
        /// The ibreve
        /// </summary>
        ibreve = 0x100012d /* U+012D LATIN SMALL LETTER I WITH BREVE */,

        /// <summary>
        /// The zstroke
        /// </summary>
        zstroke = 0x10001b6 /* U+01B6 LATIN SMALL LETTER Z WITH STROKE */,

        /// <summary>
        /// The gcaron
        /// </summary>
        gcaron = 0x10001e7 /* U+01E7 LATIN SMALL LETTER G WITH CARON */,

        /// <summary>
        /// The ocaron
        /// </summary>
        ocaron = 0x10001d2 /* U+01D2 LATIN SMALL LETTER O WITH CARON */,

        /// <summary>
        /// The obarred
        /// </summary>
        obarred = 0x1000275 /* U+0275 LATIN SMALL LETTER BARRED O */,

        /// <summary>
        /// The schwa
        /// </summary>
        SCHWA = 0x100018f /* U+018F LATIN CAPITAL LETTER SCHWA */,

        /// <summary>
        /// The schwa
        /// </summary>
        schwa = 0x1000259 /* U+0259 LATIN SMALL LETTER SCHWA */,

        /// <summary>
        /// The ezh
        /// </summary>
        EZH = 0x10001b7 /* U+01B7 LATIN CAPITAL LETTER EZH */,

        /// <summary>
        /// The ezh
        /// </summary>
        ezh = 0x1000292 /* U+0292 LATIN SMALL LETTER EZH */,

        /// <summary>
        /// The lbelowdot
        /// </summary>
        Lbelowdot = 0x1001e36 /* U+1E36 LATIN CAPITAL LETTER L WITH DOT BELOW */,

        /// <summary>
        /// The lbelowdot
        /// </summary>
        lbelowdot = 0x1001e37 /* U+1E37 LATIN SMALL LETTER L WITH DOT BELOW */,

        /// <summary>
        /// The abelowdot
        /// </summary>
        Abelowdot = 0x1001ea0 /* U+1EA0 LATIN CAPITAL LETTER A WITH DOT BELOW */,

        /// <summary>
        /// The abelowdot
        /// </summary>
        abelowdot = 0x1001ea1 /* U+1EA1 LATIN SMALL LETTER A WITH DOT BELOW */,

        /// <summary>
        /// The ahook
        /// </summary>
        Ahook = 0x1001ea2 /* U+1EA2 LATIN CAPITAL LETTER A WITH HOOK ABOVE */,

        /// <summary>
        /// The ahook
        /// </summary>
        ahook = 0x1001ea3 /* U+1EA3 LATIN SMALL LETTER A WITH HOOK ABOVE */,

        /// <summary>
        /// The acircumflexacute
        /// </summary>
        Acircumflexacute = 0x1001ea4 /* U+1EA4 LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND ACUTE */,

        /// <summary>
        /// The acircumflexacute
        /// </summary>
        acircumflexacute = 0x1001ea5 /* U+1EA5 LATIN SMALL LETTER A WITH CIRCUMFLEX AND ACUTE */,

        /// <summary>
        /// The acircumflexgrave
        /// </summary>
        Acircumflexgrave = 0x1001ea6 /* U+1EA6 LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND GRAVE */,

        /// <summary>
        /// The acircumflexgrave
        /// </summary>
        acircumflexgrave = 0x1001ea7 /* U+1EA7 LATIN SMALL LETTER A WITH CIRCUMFLEX AND GRAVE */,

        /// <summary>
        /// The acircumflexhook
        /// </summary>
        Acircumflexhook = 0x1001ea8 /* U+1EA8 LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND HOOK ABOVE */,

        /// <summary>
        /// The acircumflexhook
        /// </summary>
        acircumflexhook = 0x1001ea9 /* U+1EA9 LATIN SMALL LETTER A WITH CIRCUMFLEX AND HOOK ABOVE */,

        /// <summary>
        /// The acircumflextilde
        /// </summary>
        Acircumflextilde = 0x1001eaa /* U+1EAA LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND TILDE */,

        /// <summary>
        /// The acircumflextilde
        /// </summary>
        acircumflextilde = 0x1001eab /* U+1EAB LATIN SMALL LETTER A WITH CIRCUMFLEX AND TILDE */,

        /// <summary>
        /// The acircumflexbelowdot
        /// </summary>
        Acircumflexbelowdot = 0x1001eac /* U+1EAC LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND DOT BELOW */,

        /// <summary>
        /// The acircumflexbelowdot
        /// </summary>
        acircumflexbelowdot = 0x1001ead /* U+1EAD LATIN SMALL LETTER A WITH CIRCUMFLEX AND DOT BELOW */,

        /// <summary>
        /// The abreveacute
        /// </summary>
        Abreveacute = 0x1001eae /* U+1EAE LATIN CAPITAL LETTER A WITH BREVE AND ACUTE */,

        /// <summary>
        /// The abreveacute
        /// </summary>
        abreveacute = 0x1001eaf /* U+1EAF LATIN SMALL LETTER A WITH BREVE AND ACUTE */,

        /// <summary>
        /// The abrevegrave
        /// </summary>
        Abrevegrave = 0x1001eb0 /* U+1EB0 LATIN CAPITAL LETTER A WITH BREVE AND GRAVE */,

        /// <summary>
        /// The abrevegrave
        /// </summary>
        abrevegrave = 0x1001eb1 /* U+1EB1 LATIN SMALL LETTER A WITH BREVE AND GRAVE */,

        /// <summary>
        /// The abrevehook
        /// </summary>
        Abrevehook = 0x1001eb2 /* U+1EB2 LATIN CAPITAL LETTER A WITH BREVE AND HOOK ABOVE */,

        /// <summary>
        /// The abrevehook
        /// </summary>
        abrevehook = 0x1001eb3 /* U+1EB3 LATIN SMALL LETTER A WITH BREVE AND HOOK ABOVE */,

        /// <summary>
        /// The abrevetilde
        /// </summary>
        Abrevetilde = 0x1001eb4 /* U+1EB4 LATIN CAPITAL LETTER A WITH BREVE AND TILDE */,

        /// <summary>
        /// The abrevetilde
        /// </summary>
        abrevetilde = 0x1001eb5 /* U+1EB5 LATIN SMALL LETTER A WITH BREVE AND TILDE */,

        /// <summary>
        /// The abrevebelowdot
        /// </summary>
        Abrevebelowdot = 0x1001eb6 /* U+1EB6 LATIN CAPITAL LETTER A WITH BREVE AND DOT BELOW */,

        /// <summary>
        /// The abrevebelowdot
        /// </summary>
        abrevebelowdot = 0x1001eb7 /* U+1EB7 LATIN SMALL LETTER A WITH BREVE AND DOT BELOW */,

        /// <summary>
        /// The ebelowdot
        /// </summary>
        Ebelowdot = 0x1001eb8 /* U+1EB8 LATIN CAPITAL LETTER E WITH DOT BELOW */,

        /// <summary>
        /// The ebelowdot
        /// </summary>
        ebelowdot = 0x1001eb9 /* U+1EB9 LATIN SMALL LETTER E WITH DOT BELOW */,

        /// <summary>
        /// The ehook
        /// </summary>
        Ehook = 0x1001eba /* U+1EBA LATIN CAPITAL LETTER E WITH HOOK ABOVE */,

        /// <summary>
        /// The ehook
        /// </summary>
        ehook = 0x1001ebb /* U+1EBB LATIN SMALL LETTER E WITH HOOK ABOVE */,

        /// <summary>
        /// The etilde
        /// </summary>
        Etilde = 0x1001ebc /* U+1EBC LATIN CAPITAL LETTER E WITH TILDE */,

        /// <summary>
        /// The etilde
        /// </summary>
        etilde = 0x1001ebd /* U+1EBD LATIN SMALL LETTER E WITH TILDE */,

        /// <summary>
        /// The ecircumflexacute
        /// </summary>
        Ecircumflexacute = 0x1001ebe /* U+1EBE LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND ACUTE */,

        /// <summary>
        /// The ecircumflexacute
        /// </summary>
        ecircumflexacute = 0x1001ebf /* U+1EBF LATIN SMALL LETTER E WITH CIRCUMFLEX AND ACUTE */,

        /// <summary>
        /// The ecircumflexgrave
        /// </summary>
        Ecircumflexgrave = 0x1001ec0 /* U+1EC0 LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND GRAVE */,

        /// <summary>
        /// The ecircumflexgrave
        /// </summary>
        ecircumflexgrave = 0x1001ec1 /* U+1EC1 LATIN SMALL LETTER E WITH CIRCUMFLEX AND GRAVE */,

        /// <summary>
        /// The ecircumflexhook
        /// </summary>
        Ecircumflexhook = 0x1001ec2 /* U+1EC2 LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND HOOK ABOVE */,

        /// <summary>
        /// The ecircumflexhook
        /// </summary>
        ecircumflexhook = 0x1001ec3 /* U+1EC3 LATIN SMALL LETTER E WITH CIRCUMFLEX AND HOOK ABOVE */,

        /// <summary>
        /// The ecircumflextilde
        /// </summary>
        Ecircumflextilde = 0x1001ec4 /* U+1EC4 LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND TILDE */,

        /// <summary>
        /// The ecircumflextilde
        /// </summary>
        ecircumflextilde = 0x1001ec5 /* U+1EC5 LATIN SMALL LETTER E WITH CIRCUMFLEX AND TILDE */,

        /// <summary>
        /// The ecircumflexbelowdot
        /// </summary>
        Ecircumflexbelowdot = 0x1001ec6 /* U+1EC6 LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND DOT BELOW */,

        /// <summary>
        /// The ecircumflexbelowdot
        /// </summary>
        ecircumflexbelowdot = 0x1001ec7 /* U+1EC7 LATIN SMALL LETTER E WITH CIRCUMFLEX AND DOT BELOW */,

        /// <summary>
        /// The ihook
        /// </summary>
        Ihook = 0x1001ec8 /* U+1EC8 LATIN CAPITAL LETTER I WITH HOOK ABOVE */,

        /// <summary>
        /// The ihook
        /// </summary>
        ihook = 0x1001ec9 /* U+1EC9 LATIN SMALL LETTER I WITH HOOK ABOVE */,

        /// <summary>
        /// The ibelowdot
        /// </summary>
        Ibelowdot = 0x1001eca /* U+1ECA LATIN CAPITAL LETTER I WITH DOT BELOW */,

        /// <summary>
        /// The ibelowdot
        /// </summary>
        ibelowdot = 0x1001ecb /* U+1ECB LATIN SMALL LETTER I WITH DOT BELOW */,

        /// <summary>
        /// The obelowdot
        /// </summary>
        Obelowdot = 0x1001ecc /* U+1ECC LATIN CAPITAL LETTER O WITH DOT BELOW */,

        /// <summary>
        /// The obelowdot
        /// </summary>
        obelowdot = 0x1001ecd /* U+1ECD LATIN SMALL LETTER O WITH DOT BELOW */,

        /// <summary>
        /// The ohook
        /// </summary>
        Ohook = 0x1001ece /* U+1ECE LATIN CAPITAL LETTER O WITH HOOK ABOVE */,

        /// <summary>
        /// The ohook
        /// </summary>
        ohook = 0x1001ecf /* U+1ECF LATIN SMALL LETTER O WITH HOOK ABOVE */,

        /// <summary>
        /// The ocircumflexacute
        /// </summary>
        Ocircumflexacute = 0x1001ed0 /* U+1ED0 LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND ACUTE */,

        /// <summary>
        /// The ocircumflexacute
        /// </summary>
        ocircumflexacute = 0x1001ed1 /* U+1ED1 LATIN SMALL LETTER O WITH CIRCUMFLEX AND ACUTE */,

        /// <summary>
        /// The ocircumflexgrave
        /// </summary>
        Ocircumflexgrave = 0x1001ed2 /* U+1ED2 LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND GRAVE */,

        /// <summary>
        /// The ocircumflexgrave
        /// </summary>
        ocircumflexgrave = 0x1001ed3 /* U+1ED3 LATIN SMALL LETTER O WITH CIRCUMFLEX AND GRAVE */,

        /// <summary>
        /// The ocircumflexhook
        /// </summary>
        Ocircumflexhook = 0x1001ed4 /* U+1ED4 LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND HOOK ABOVE */,

        /// <summary>
        /// The ocircumflexhook
        /// </summary>
        ocircumflexhook = 0x1001ed5 /* U+1ED5 LATIN SMALL LETTER O WITH CIRCUMFLEX AND HOOK ABOVE */,

        /// <summary>
        /// The ocircumflextilde
        /// </summary>
        Ocircumflextilde = 0x1001ed6 /* U+1ED6 LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND TILDE */,

        /// <summary>
        /// The ocircumflextilde
        /// </summary>
        ocircumflextilde = 0x1001ed7 /* U+1ED7 LATIN SMALL LETTER O WITH CIRCUMFLEX AND TILDE */,

        /// <summary>
        /// The ocircumflexbelowdot
        /// </summary>
        Ocircumflexbelowdot = 0x1001ed8 /* U+1ED8 LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND DOT BELOW */,

        /// <summary>
        /// The ocircumflexbelowdot
        /// </summary>
        ocircumflexbelowdot = 0x1001ed9 /* U+1ED9 LATIN SMALL LETTER O WITH CIRCUMFLEX AND DOT BELOW */,

        /// <summary>
        /// The ohornacute
        /// </summary>
        Ohornacute = 0x1001eda /* U+1EDA LATIN CAPITAL LETTER O WITH HORN AND ACUTE */,

        /// <summary>
        /// The ohornacute
        /// </summary>
        ohornacute = 0x1001edb /* U+1EDB LATIN SMALL LETTER O WITH HORN AND ACUTE */,

        /// <summary>
        /// The ohorngrave
        /// </summary>
        Ohorngrave = 0x1001edc /* U+1EDC LATIN CAPITAL LETTER O WITH HORN AND GRAVE */,

        /// <summary>
        /// The ohorngrave
        /// </summary>
        ohorngrave = 0x1001edd /* U+1EDD LATIN SMALL LETTER O WITH HORN AND GRAVE */,

        /// <summary>
        /// The ohornhook
        /// </summary>
        Ohornhook = 0x1001ede /* U+1EDE LATIN CAPITAL LETTER O WITH HORN AND HOOK ABOVE */,

        /// <summary>
        /// The ohornhook
        /// </summary>
        ohornhook = 0x1001edf /* U+1EDF LATIN SMALL LETTER O WITH HORN AND HOOK ABOVE */,

        /// <summary>
        /// The ohorntilde
        /// </summary>
        Ohorntilde = 0x1001ee0 /* U+1EE0 LATIN CAPITAL LETTER O WITH HORN AND TILDE */,

        /// <summary>
        /// The ohorntilde
        /// </summary>
        ohorntilde = 0x1001ee1 /* U+1EE1 LATIN SMALL LETTER O WITH HORN AND TILDE */,

        /// <summary>
        /// The ohornbelowdot
        /// </summary>
        Ohornbelowdot = 0x1001ee2 /* U+1EE2 LATIN CAPITAL LETTER O WITH HORN AND DOT BELOW */,

        /// <summary>
        /// The ohornbelowdot
        /// </summary>
        ohornbelowdot = 0x1001ee3 /* U+1EE3 LATIN SMALL LETTER O WITH HORN AND DOT BELOW */,

        /// <summary>
        /// The ubelowdot
        /// </summary>
        Ubelowdot = 0x1001ee4 /* U+1EE4 LATIN CAPITAL LETTER U WITH DOT BELOW */,

        /// <summary>
        /// The ubelowdot
        /// </summary>
        ubelowdot = 0x1001ee5 /* U+1EE5 LATIN SMALL LETTER U WITH DOT BELOW */,

        /// <summary>
        /// The uhook
        /// </summary>
        Uhook = 0x1001ee6 /* U+1EE6 LATIN CAPITAL LETTER U WITH HOOK ABOVE */,

        /// <summary>
        /// The uhook
        /// </summary>
        uhook = 0x1001ee7 /* U+1EE7 LATIN SMALL LETTER U WITH HOOK ABOVE */,

        /// <summary>
        /// The uhornacute
        /// </summary>
        Uhornacute = 0x1001ee8 /* U+1EE8 LATIN CAPITAL LETTER U WITH HORN AND ACUTE */,

        /// <summary>
        /// The uhornacute
        /// </summary>
        uhornacute = 0x1001ee9 /* U+1EE9 LATIN SMALL LETTER U WITH HORN AND ACUTE */,

        /// <summary>
        /// The uhorngrave
        /// </summary>
        Uhorngrave = 0x1001eea /* U+1EEA LATIN CAPITAL LETTER U WITH HORN AND GRAVE */,

        /// <summary>
        /// The uhorngrave
        /// </summary>
        uhorngrave = 0x1001eeb /* U+1EEB LATIN SMALL LETTER U WITH HORN AND GRAVE */,

        /// <summary>
        /// The uhornhook
        /// </summary>
        Uhornhook = 0x1001eec /* U+1EEC LATIN CAPITAL LETTER U WITH HORN AND HOOK ABOVE */,

        /// <summary>
        /// The uhornhook
        /// </summary>
        uhornhook = 0x1001eed /* U+1EED LATIN SMALL LETTER U WITH HORN AND HOOK ABOVE */,

        /// <summary>
        /// The uhorntilde
        /// </summary>
        Uhorntilde = 0x1001eee /* U+1EEE LATIN CAPITAL LETTER U WITH HORN AND TILDE */,

        /// <summary>
        /// The uhorntilde
        /// </summary>
        uhorntilde = 0x1001eef /* U+1EEF LATIN SMALL LETTER U WITH HORN AND TILDE */,

        /// <summary>
        /// The uhornbelowdot
        /// </summary>
        Uhornbelowdot = 0x1001ef0 /* U+1EF0 LATIN CAPITAL LETTER U WITH HORN AND DOT BELOW */,

        /// <summary>
        /// The uhornbelowdot
        /// </summary>
        uhornbelowdot = 0x1001ef1 /* U+1EF1 LATIN SMALL LETTER U WITH HORN AND DOT BELOW */,

        /// <summary>
        /// The ybelowdot
        /// </summary>
        Ybelowdot = 0x1001ef4 /* U+1EF4 LATIN CAPITAL LETTER Y WITH DOT BELOW */,

        /// <summary>
        /// The ybelowdot
        /// </summary>
        ybelowdot = 0x1001ef5 /* U+1EF5 LATIN SMALL LETTER Y WITH DOT BELOW */,

        /// <summary>
        /// The yhook
        /// </summary>
        Yhook = 0x1001ef6 /* U+1EF6 LATIN CAPITAL LETTER Y WITH HOOK ABOVE */,

        /// <summary>
        /// The yhook
        /// </summary>
        yhook = 0x1001ef7 /* U+1EF7 LATIN SMALL LETTER Y WITH HOOK ABOVE */,

        /// <summary>
        /// The ytilde
        /// </summary>
        Ytilde = 0x1001ef8 /* U+1EF8 LATIN CAPITAL LETTER Y WITH TILDE */,

        /// <summary>
        /// The ytilde
        /// </summary>
        ytilde = 0x1001ef9 /* U+1EF9 LATIN SMALL LETTER Y WITH TILDE */,

        /// <summary>
        /// The ohorn
        /// </summary>
        Ohorn = 0x10001a0 /* U+01A0 LATIN CAPITAL LETTER O WITH HORN */,

        /// <summary>
        /// The ohorn
        /// </summary>
        ohorn = 0x10001a1 /* U+01A1 LATIN SMALL LETTER O WITH HORN */,

        /// <summary>
        /// The uhorn
        /// </summary>
        Uhorn = 0x10001af /* U+01AF LATIN CAPITAL LETTER U WITH HORN */,

        /// <summary>
        /// The uhorn
        /// </summary>
        uhorn = 0x10001b0 /* U+01B0 LATIN SMALL LETTER U WITH HORN */,

        /// <summary>
        /// The ecu sign
        /// </summary>
        EcuSign = 0x10020a0 /* U+20A0 EURO-CURRENCY SIGN */,

        /// <summary>
        /// The colon sign
        /// </summary>
        ColonSign = 0x10020a1 /* U+20A1 COLON SIGN */,

        /// <summary>
        /// The cruzeiro sign
        /// </summary>
        CruzeiroSign = 0x10020a2 /* U+20A2 CRUZEIRO SIGN */,

        /// <summary>
        /// The f franc sign
        /// </summary>
        FFrancSign = 0x10020a3 /* U+20A3 FRENCH FRANC SIGN */,

        /// <summary>
        /// The lira sign
        /// </summary>
        LiraSign = 0x10020a4 /* U+20A4 LIRA SIGN */,

        /// <summary>
        /// The mill sign
        /// </summary>
        MillSign = 0x10020a5 /* U+20A5 MILL SIGN */,

        /// <summary>
        /// The naira sign
        /// </summary>
        NairaSign = 0x10020a6 /* U+20A6 NAIRA SIGN */,

        /// <summary>
        /// The peseta sign
        /// </summary>
        PesetaSign = 0x10020a7 /* U+20A7 PESETA SIGN */,

        /// <summary>
        /// The rupee sign
        /// </summary>
        RupeeSign = 0x10020a8 /* U+20A8 RUPEE SIGN */,

        /// <summary>
        /// The won sign
        /// </summary>
        WonSign = 0x10020a9 /* U+20A9 WON SIGN */,

        /// <summary>
        /// Creates new sheqelsign.
        /// </summary>
        NewSheqelSign = 0x10020aa /* U+20AA NEW SHEQEL SIGN */,

        /// <summary>
        /// The dong sign
        /// </summary>
        DongSign = 0x10020ab /* U+20AB DONG SIGN */,

        /// <summary>
        /// The euro sign
        /// </summary>
        EuroSign = 0x20ac /* U+20AC EURO SIGN */,

        /// <summary>
        /// The zerosuperior
        /// </summary>
        zerosuperior = 0x1002070 /* U+2070 SUPERSCRIPT ZERO */,

        /// <summary>
        /// The foursuperior
        /// </summary>
        foursuperior = 0x1002074 /* U+2074 SUPERSCRIPT FOUR */,

        /// <summary>
        /// The fivesuperior
        /// </summary>
        fivesuperior = 0x1002075 /* U+2075 SUPERSCRIPT FIVE */,

        /// <summary>
        /// The sixsuperior
        /// </summary>
        sixsuperior = 0x1002076 /* U+2076 SUPERSCRIPT SIX */,

        /// <summary>
        /// The sevensuperior
        /// </summary>
        sevensuperior = 0x1002077 /* U+2077 SUPERSCRIPT SEVEN */,

        /// <summary>
        /// The eightsuperior
        /// </summary>
        eightsuperior = 0x1002078 /* U+2078 SUPERSCRIPT EIGHT */,

        /// <summary>
        /// The ninesuperior
        /// </summary>
        ninesuperior = 0x1002079 /* U+2079 SUPERSCRIPT NINE */,

        /// <summary>
        /// The zerosubscript
        /// </summary>
        zerosubscript = 0x1002080 /* U+2080 SUBSCRIPT ZERO */,

        /// <summary>
        /// The onesubscript
        /// </summary>
        onesubscript = 0x1002081 /* U+2081 SUBSCRIPT ONE */,

        /// <summary>
        /// The twosubscript
        /// </summary>
        twosubscript = 0x1002082 /* U+2082 SUBSCRIPT TWO */,

        /// <summary>
        /// The threesubscript
        /// </summary>
        threesubscript = 0x1002083 /* U+2083 SUBSCRIPT THREE */,

        /// <summary>
        /// The foursubscript
        /// </summary>
        foursubscript = 0x1002084 /* U+2084 SUBSCRIPT FOUR */,

        /// <summary>
        /// The fivesubscript
        /// </summary>
        fivesubscript = 0x1002085 /* U+2085 SUBSCRIPT FIVE */,

        /// <summary>
        /// The sixsubscript
        /// </summary>
        sixsubscript = 0x1002086 /* U+2086 SUBSCRIPT SIX */,

        /// <summary>
        /// The sevensubscript
        /// </summary>
        sevensubscript = 0x1002087 /* U+2087 SUBSCRIPT SEVEN */,

        /// <summary>
        /// The eightsubscript
        /// </summary>
        eightsubscript = 0x1002088 /* U+2088 SUBSCRIPT EIGHT */,

        /// <summary>
        /// The ninesubscript
        /// </summary>
        ninesubscript = 0x1002089 /* U+2089 SUBSCRIPT NINE */,

        /// <summary>
        /// The partdifferential
        /// </summary>
        partdifferential = 0x1002202 /* U+2202 PARTIAL DIFFERENTIAL */,

        /// <summary>
        /// The emptyset
        /// </summary>
        emptyset = 0x1002205 /* U+2205 NULL SET */,

        /// <summary>
        /// The elementof
        /// </summary>
        elementof = 0x1002208 /* U+2208 ELEMENT OF */,

        /// <summary>
        /// The notelementof
        /// </summary>
        notelementof = 0x1002209 /* U+2209 NOT AN ELEMENT OF */,

        /// <summary>
        /// The containsas
        /// </summary>
        containsas = 0x100220B /* U+220B CONTAINS AS MEMBER */,

        /// <summary>
        /// The squareroot
        /// </summary>
        squareroot = 0x100221A /* U+221A SQUARE ROOT */,

        /// <summary>
        /// The cuberoot
        /// </summary>
        cuberoot = 0x100221B /* U+221B CUBE ROOT */,

        /// <summary>
        /// The fourthroot
        /// </summary>
        fourthroot = 0x100221C /* U+221C FOURTH ROOT */,

        /// <summary>
        /// The dintegral
        /// </summary>
        dintegral = 0x100222C /* U+222C DOUBLE INTEGRAL */,

        /// <summary>
        /// The tintegral
        /// </summary>
        tintegral = 0x100222D /* U+222D TRIPLE INTEGRAL */,

        /// <summary>
        /// The because
        /// </summary>
        because = 0x1002235 /* U+2235 BECAUSE */,

        /// <summary>
        /// The approxeq
        /// </summary>
        approxeq = 0x1002248 /* U+2245 ALMOST EQUAL TO */,

        /// <summary>
        /// The notapproxeq
        /// </summary>
        notapproxeq = 0x1002247 /* U+2247 NOT ALMOST EQUAL TO */,

        /// <summary>
        /// The notidentical
        /// </summary>
        notidentical = 0x1002262 /* U+2262 NOT IDENTICAL TO */,

        /// <summary>
        /// The stricteq
        /// </summary>
        stricteq = 0x1002263 /* U+2263 STRICTLY EQUIVALENT TO */,

        /// <summary>
        /// The braille dot 1
        /// </summary>
        braille_dot_1 = 0xfff1,

        /// <summary>
        /// The braille dot 2
        /// </summary>
        braille_dot_2 = 0xfff2,

        /// <summary>
        /// The braille dot 3
        /// </summary>
        braille_dot_3 = 0xfff3,

        /// <summary>
        /// The braille dot 4
        /// </summary>
        braille_dot_4 = 0xfff4,

        /// <summary>
        /// The braille dot 5
        /// </summary>
        braille_dot_5 = 0xfff5,

        /// <summary>
        /// The braille dot 6
        /// </summary>
        braille_dot_6 = 0xfff6,

        /// <summary>
        /// The braille dot 7
        /// </summary>
        braille_dot_7 = 0xfff7,

        /// <summary>
        /// The braille dot 8
        /// </summary>
        braille_dot_8 = 0xfff8,

        /// <summary>
        /// The braille dot 9
        /// </summary>
        braille_dot_9 = 0xfff9,

        /// <summary>
        /// The braille dot 10
        /// </summary>
        braille_dot_10 = 0xfffa,

        /// <summary>
        /// The braille blank
        /// </summary>
        braille_blank = 0x1002800 /* U+2800 BRAILLE PATTERN BLANK */,

        /// <summary>
        /// The braille dots 1
        /// </summary>
        braille_dots_1 = 0x1002801 /* U+2801 BRAILLE PATTERN DOTS-1 */,

        /// <summary>
        /// The braille dots 2
        /// </summary>
        braille_dots_2 = 0x1002802 /* U+2802 BRAILLE PATTERN DOTS-2 */,

        /// <summary>
        /// The braille dots 12
        /// </summary>
        braille_dots_12 = 0x1002803 /* U+2803 BRAILLE PATTERN DOTS-12 */,

        /// <summary>
        /// The braille dots 3
        /// </summary>
        braille_dots_3 = 0x1002804 /* U+2804 BRAILLE PATTERN DOTS-3 */,

        /// <summary>
        /// The braille dots 13
        /// </summary>
        braille_dots_13 = 0x1002805 /* U+2805 BRAILLE PATTERN DOTS-13 */,

        /// <summary>
        /// The braille dots 23
        /// </summary>
        braille_dots_23 = 0x1002806 /* U+2806 BRAILLE PATTERN DOTS-23 */,

        /// <summary>
        /// The braille dots 123
        /// </summary>
        braille_dots_123 = 0x1002807 /* U+2807 BRAILLE PATTERN DOTS-123 */,

        /// <summary>
        /// The braille dots 4
        /// </summary>
        braille_dots_4 = 0x1002808 /* U+2808 BRAILLE PATTERN DOTS-4 */,

        /// <summary>
        /// The braille dots 14
        /// </summary>
        braille_dots_14 = 0x1002809 /* U+2809 BRAILLE PATTERN DOTS-14 */,

        /// <summary>
        /// The braille dots 24
        /// </summary>
        braille_dots_24 = 0x100280a /* U+280a BRAILLE PATTERN DOTS-24 */,

        /// <summary>
        /// The braille dots 124
        /// </summary>
        braille_dots_124 = 0x100280b /* U+280b BRAILLE PATTERN DOTS-124 */,

        /// <summary>
        /// The braille dots 34
        /// </summary>
        braille_dots_34 = 0x100280c /* U+280c BRAILLE PATTERN DOTS-34 */,

        /// <summary>
        /// The braille dots 134
        /// </summary>
        braille_dots_134 = 0x100280d /* U+280d BRAILLE PATTERN DOTS-134 */,

        /// <summary>
        /// The braille dots 234
        /// </summary>
        braille_dots_234 = 0x100280e /* U+280e BRAILLE PATTERN DOTS-234 */,

        /// <summary>
        /// The braille dots 1234
        /// </summary>
        braille_dots_1234 = 0x100280f /* U+280f BRAILLE PATTERN DOTS-1234 */,

        /// <summary>
        /// The braille dots 5
        /// </summary>
        braille_dots_5 = 0x1002810 /* U+2810 BRAILLE PATTERN DOTS-5 */,

        /// <summary>
        /// The braille dots 15
        /// </summary>
        braille_dots_15 = 0x1002811 /* U+2811 BRAILLE PATTERN DOTS-15 */,

        /// <summary>
        /// The braille dots 25
        /// </summary>
        braille_dots_25 = 0x1002812 /* U+2812 BRAILLE PATTERN DOTS-25 */,

        /// <summary>
        /// The braille dots 125
        /// </summary>
        braille_dots_125 = 0x1002813 /* U+2813 BRAILLE PATTERN DOTS-125 */,

        /// <summary>
        /// The braille dots 35
        /// </summary>
        braille_dots_35 = 0x1002814 /* U+2814 BRAILLE PATTERN DOTS-35 */,

        /// <summary>
        /// The braille dots 135
        /// </summary>
        braille_dots_135 = 0x1002815 /* U+2815 BRAILLE PATTERN DOTS-135 */,

        /// <summary>
        /// The braille dots 235
        /// </summary>
        braille_dots_235 = 0x1002816 /* U+2816 BRAILLE PATTERN DOTS-235 */,

        /// <summary>
        /// The braille dots 1235
        /// </summary>
        braille_dots_1235 = 0x1002817 /* U+2817 BRAILLE PATTERN DOTS-1235 */,

        /// <summary>
        /// The braille dots 45
        /// </summary>
        braille_dots_45 = 0x1002818 /* U+2818 BRAILLE PATTERN DOTS-45 */,

        /// <summary>
        /// The braille dots 145
        /// </summary>
        braille_dots_145 = 0x1002819 /* U+2819 BRAILLE PATTERN DOTS-145 */,

        /// <summary>
        /// The braille dots 245
        /// </summary>
        braille_dots_245 = 0x100281a /* U+281a BRAILLE PATTERN DOTS-245 */,

        /// <summary>
        /// The braille dots 1245
        /// </summary>
        braille_dots_1245 = 0x100281b /* U+281b BRAILLE PATTERN DOTS-1245 */,

        /// <summary>
        /// The braille dots 345
        /// </summary>
        braille_dots_345 = 0x100281c /* U+281c BRAILLE PATTERN DOTS-345 */,

        /// <summary>
        /// The braille dots 1345
        /// </summary>
        braille_dots_1345 = 0x100281d /* U+281d BRAILLE PATTERN DOTS-1345 */,

        /// <summary>
        /// The braille dots 2345
        /// </summary>
        braille_dots_2345 = 0x100281e /* U+281e BRAILLE PATTERN DOTS-2345 */,

        /// <summary>
        /// The braille dots 12345
        /// </summary>
        braille_dots_12345 = 0x100281f /* U+281f BRAILLE PATTERN DOTS-12345 */,

        /// <summary>
        /// The braille dots 6
        /// </summary>
        braille_dots_6 = 0x1002820 /* U+2820 BRAILLE PATTERN DOTS-6 */,

        /// <summary>
        /// The braille dots 16
        /// </summary>
        braille_dots_16 = 0x1002821 /* U+2821 BRAILLE PATTERN DOTS-16 */,

        /// <summary>
        /// The braille dots 26
        /// </summary>
        braille_dots_26 = 0x1002822 /* U+2822 BRAILLE PATTERN DOTS-26 */,

        /// <summary>
        /// The braille dots 126
        /// </summary>
        braille_dots_126 = 0x1002823 /* U+2823 BRAILLE PATTERN DOTS-126 */,

        /// <summary>
        /// The braille dots 36
        /// </summary>
        braille_dots_36 = 0x1002824 /* U+2824 BRAILLE PATTERN DOTS-36 */,

        /// <summary>
        /// The braille dots 136
        /// </summary>
        braille_dots_136 = 0x1002825 /* U+2825 BRAILLE PATTERN DOTS-136 */,

        /// <summary>
        /// The braille dots 236
        /// </summary>
        braille_dots_236 = 0x1002826 /* U+2826 BRAILLE PATTERN DOTS-236 */,

        /// <summary>
        /// The braille dots 1236
        /// </summary>
        braille_dots_1236 = 0x1002827 /* U+2827 BRAILLE PATTERN DOTS-1236 */,

        /// <summary>
        /// The braille dots 46
        /// </summary>
        braille_dots_46 = 0x1002828 /* U+2828 BRAILLE PATTERN DOTS-46 */,

        /// <summary>
        /// The braille dots 146
        /// </summary>
        braille_dots_146 = 0x1002829 /* U+2829 BRAILLE PATTERN DOTS-146 */,

        /// <summary>
        /// The braille dots 246
        /// </summary>
        braille_dots_246 = 0x100282a /* U+282a BRAILLE PATTERN DOTS-246 */,

        /// <summary>
        /// The braille dots 1246
        /// </summary>
        braille_dots_1246 = 0x100282b /* U+282b BRAILLE PATTERN DOTS-1246 */,

        /// <summary>
        /// The braille dots 346
        /// </summary>
        braille_dots_346 = 0x100282c /* U+282c BRAILLE PATTERN DOTS-346 */,

        /// <summary>
        /// The braille dots 1346
        /// </summary>
        braille_dots_1346 = 0x100282d /* U+282d BRAILLE PATTERN DOTS-1346 */,

        /// <summary>
        /// The braille dots 2346
        /// </summary>
        braille_dots_2346 = 0x100282e /* U+282e BRAILLE PATTERN DOTS-2346 */,

        /// <summary>
        /// The braille dots 12346
        /// </summary>
        braille_dots_12346 = 0x100282f /* U+282f BRAILLE PATTERN DOTS-12346 */,

        /// <summary>
        /// The braille dots 56
        /// </summary>
        braille_dots_56 = 0x1002830 /* U+2830 BRAILLE PATTERN DOTS-56 */,

        /// <summary>
        /// The braille dots 156
        /// </summary>
        braille_dots_156 = 0x1002831 /* U+2831 BRAILLE PATTERN DOTS-156 */,

        /// <summary>
        /// The braille dots 256
        /// </summary>
        braille_dots_256 = 0x1002832 /* U+2832 BRAILLE PATTERN DOTS-256 */,

        /// <summary>
        /// The braille dots 1256
        /// </summary>
        braille_dots_1256 = 0x1002833 /* U+2833 BRAILLE PATTERN DOTS-1256 */,

        /// <summary>
        /// The braille dots 356
        /// </summary>
        braille_dots_356 = 0x1002834 /* U+2834 BRAILLE PATTERN DOTS-356 */,

        /// <summary>
        /// The braille dots 1356
        /// </summary>
        braille_dots_1356 = 0x1002835 /* U+2835 BRAILLE PATTERN DOTS-1356 */,

        /// <summary>
        /// The braille dots 2356
        /// </summary>
        braille_dots_2356 = 0x1002836 /* U+2836 BRAILLE PATTERN DOTS-2356 */,

        /// <summary>
        /// The braille dots 12356
        /// </summary>
        braille_dots_12356 = 0x1002837 /* U+2837 BRAILLE PATTERN DOTS-12356 */,

        /// <summary>
        /// The braille dots 456
        /// </summary>
        braille_dots_456 = 0x1002838 /* U+2838 BRAILLE PATTERN DOTS-456 */,

        /// <summary>
        /// The braille dots 1456
        /// </summary>
        braille_dots_1456 = 0x1002839 /* U+2839 BRAILLE PATTERN DOTS-1456 */,

        /// <summary>
        /// The braille dots 2456
        /// </summary>
        braille_dots_2456 = 0x100283a /* U+283a BRAILLE PATTERN DOTS-2456 */,

        /// <summary>
        /// The braille dots 12456
        /// </summary>
        braille_dots_12456 = 0x100283b /* U+283b BRAILLE PATTERN DOTS-12456 */,

        /// <summary>
        /// The braille dots 3456
        /// </summary>
        braille_dots_3456 = 0x100283c /* U+283c BRAILLE PATTERN DOTS-3456 */,

        /// <summary>
        /// The braille dots 13456
        /// </summary>
        braille_dots_13456 = 0x100283d /* U+283d BRAILLE PATTERN DOTS-13456 */,

        /// <summary>
        /// The braille dots 23456
        /// </summary>
        braille_dots_23456 = 0x100283e /* U+283e BRAILLE PATTERN DOTS-23456 */,

        /// <summary>
        /// The braille dots 123456
        /// </summary>
        braille_dots_123456 = 0x100283f /* U+283f BRAILLE PATTERN DOTS-123456 */,

        /// <summary>
        /// The braille dots 7
        /// </summary>
        braille_dots_7 = 0x1002840 /* U+2840 BRAILLE PATTERN DOTS-7 */,

        /// <summary>
        /// The braille dots 17
        /// </summary>
        braille_dots_17 = 0x1002841 /* U+2841 BRAILLE PATTERN DOTS-17 */,

        /// <summary>
        /// The braille dots 27
        /// </summary>
        braille_dots_27 = 0x1002842 /* U+2842 BRAILLE PATTERN DOTS-27 */,

        /// <summary>
        /// The braille dots 127
        /// </summary>
        braille_dots_127 = 0x1002843 /* U+2843 BRAILLE PATTERN DOTS-127 */,

        /// <summary>
        /// The braille dots 37
        /// </summary>
        braille_dots_37 = 0x1002844 /* U+2844 BRAILLE PATTERN DOTS-37 */,

        /// <summary>
        /// The braille dots 137
        /// </summary>
        braille_dots_137 = 0x1002845 /* U+2845 BRAILLE PATTERN DOTS-137 */,

        /// <summary>
        /// The braille dots 237
        /// </summary>
        braille_dots_237 = 0x1002846 /* U+2846 BRAILLE PATTERN DOTS-237 */,

        /// <summary>
        /// The braille dots 1237
        /// </summary>
        braille_dots_1237 = 0x1002847 /* U+2847 BRAILLE PATTERN DOTS-1237 */,

        /// <summary>
        /// The braille dots 47
        /// </summary>
        braille_dots_47 = 0x1002848 /* U+2848 BRAILLE PATTERN DOTS-47 */,

        /// <summary>
        /// The braille dots 147
        /// </summary>
        braille_dots_147 = 0x1002849 /* U+2849 BRAILLE PATTERN DOTS-147 */,

        /// <summary>
        /// The braille dots 247
        /// </summary>
        braille_dots_247 = 0x100284a /* U+284a BRAILLE PATTERN DOTS-247 */,

        /// <summary>
        /// The braille dots 1247
        /// </summary>
        braille_dots_1247 = 0x100284b /* U+284b BRAILLE PATTERN DOTS-1247 */,

        /// <summary>
        /// The braille dots 347
        /// </summary>
        braille_dots_347 = 0x100284c /* U+284c BRAILLE PATTERN DOTS-347 */,

        /// <summary>
        /// The braille dots 1347
        /// </summary>
        braille_dots_1347 = 0x100284d /* U+284d BRAILLE PATTERN DOTS-1347 */,

        /// <summary>
        /// The braille dots 2347
        /// </summary>
        braille_dots_2347 = 0x100284e /* U+284e BRAILLE PATTERN DOTS-2347 */,

        /// <summary>
        /// The braille dots 12347
        /// </summary>
        braille_dots_12347 = 0x100284f /* U+284f BRAILLE PATTERN DOTS-12347 */,

        /// <summary>
        /// The braille dots 57
        /// </summary>
        braille_dots_57 = 0x1002850 /* U+2850 BRAILLE PATTERN DOTS-57 */,

        /// <summary>
        /// The braille dots 157
        /// </summary>
        braille_dots_157 = 0x1002851 /* U+2851 BRAILLE PATTERN DOTS-157 */,

        /// <summary>
        /// The braille dots 257
        /// </summary>
        braille_dots_257 = 0x1002852 /* U+2852 BRAILLE PATTERN DOTS-257 */,

        /// <summary>
        /// The braille dots 1257
        /// </summary>
        braille_dots_1257 = 0x1002853 /* U+2853 BRAILLE PATTERN DOTS-1257 */,

        /// <summary>
        /// The braille dots 357
        /// </summary>
        braille_dots_357 = 0x1002854 /* U+2854 BRAILLE PATTERN DOTS-357 */,

        /// <summary>
        /// The braille dots 1357
        /// </summary>
        braille_dots_1357 = 0x1002855 /* U+2855 BRAILLE PATTERN DOTS-1357 */,

        /// <summary>
        /// The braille dots 2357
        /// </summary>
        braille_dots_2357 = 0x1002856 /* U+2856 BRAILLE PATTERN DOTS-2357 */,

        /// <summary>
        /// The braille dots 12357
        /// </summary>
        braille_dots_12357 = 0x1002857 /* U+2857 BRAILLE PATTERN DOTS-12357 */,

        /// <summary>
        /// The braille dots 457
        /// </summary>
        braille_dots_457 = 0x1002858 /* U+2858 BRAILLE PATTERN DOTS-457 */,

        /// <summary>
        /// The braille dots 1457
        /// </summary>
        braille_dots_1457 = 0x1002859 /* U+2859 BRAILLE PATTERN DOTS-1457 */,

        /// <summary>
        /// The braille dots 2457
        /// </summary>
        braille_dots_2457 = 0x100285a /* U+285a BRAILLE PATTERN DOTS-2457 */,

        /// <summary>
        /// The braille dots 12457
        /// </summary>
        braille_dots_12457 = 0x100285b /* U+285b BRAILLE PATTERN DOTS-12457 */,

        /// <summary>
        /// The braille dots 3457
        /// </summary>
        braille_dots_3457 = 0x100285c /* U+285c BRAILLE PATTERN DOTS-3457 */,

        /// <summary>
        /// The braille dots 13457
        /// </summary>
        braille_dots_13457 = 0x100285d /* U+285d BRAILLE PATTERN DOTS-13457 */,

        /// <summary>
        /// The braille dots 23457
        /// </summary>
        braille_dots_23457 = 0x100285e /* U+285e BRAILLE PATTERN DOTS-23457 */,

        /// <summary>
        /// The braille dots 123457
        /// </summary>
        braille_dots_123457 = 0x100285f /* U+285f BRAILLE PATTERN DOTS-123457 */,

        /// <summary>
        /// The braille dots 67
        /// </summary>
        braille_dots_67 = 0x1002860 /* U+2860 BRAILLE PATTERN DOTS-67 */,

        /// <summary>
        /// The braille dots 167
        /// </summary>
        braille_dots_167 = 0x1002861 /* U+2861 BRAILLE PATTERN DOTS-167 */,

        /// <summary>
        /// The braille dots 267
        /// </summary>
        braille_dots_267 = 0x1002862 /* U+2862 BRAILLE PATTERN DOTS-267 */,

        /// <summary>
        /// The braille dots 1267
        /// </summary>
        braille_dots_1267 = 0x1002863 /* U+2863 BRAILLE PATTERN DOTS-1267 */,

        /// <summary>
        /// The braille dots 367
        /// </summary>
        braille_dots_367 = 0x1002864 /* U+2864 BRAILLE PATTERN DOTS-367 */,

        /// <summary>
        /// The braille dots 1367
        /// </summary>
        braille_dots_1367 = 0x1002865 /* U+2865 BRAILLE PATTERN DOTS-1367 */,

        /// <summary>
        /// The braille dots 2367
        /// </summary>
        braille_dots_2367 = 0x1002866 /* U+2866 BRAILLE PATTERN DOTS-2367 */,

        /// <summary>
        /// The braille dots 12367
        /// </summary>
        braille_dots_12367 = 0x1002867 /* U+2867 BRAILLE PATTERN DOTS-12367 */,

        /// <summary>
        /// The braille dots 467
        /// </summary>
        braille_dots_467 = 0x1002868 /* U+2868 BRAILLE PATTERN DOTS-467 */,

        /// <summary>
        /// The braille dots 1467
        /// </summary>
        braille_dots_1467 = 0x1002869 /* U+2869 BRAILLE PATTERN DOTS-1467 */,

        /// <summary>
        /// The braille dots 2467
        /// </summary>
        braille_dots_2467 = 0x100286a /* U+286a BRAILLE PATTERN DOTS-2467 */,

        /// <summary>
        /// The braille dots 12467
        /// </summary>
        braille_dots_12467 = 0x100286b /* U+286b BRAILLE PATTERN DOTS-12467 */,

        /// <summary>
        /// The braille dots 3467
        /// </summary>
        braille_dots_3467 = 0x100286c /* U+286c BRAILLE PATTERN DOTS-3467 */,

        /// <summary>
        /// The braille dots 13467
        /// </summary>
        braille_dots_13467 = 0x100286d /* U+286d BRAILLE PATTERN DOTS-13467 */,

        /// <summary>
        /// The braille dots 23467
        /// </summary>
        braille_dots_23467 = 0x100286e /* U+286e BRAILLE PATTERN DOTS-23467 */,

        /// <summary>
        /// The braille dots 123467
        /// </summary>
        braille_dots_123467 = 0x100286f /* U+286f BRAILLE PATTERN DOTS-123467 */,

        /// <summary>
        /// The braille dots 567
        /// </summary>
        braille_dots_567 = 0x1002870 /* U+2870 BRAILLE PATTERN DOTS-567 */,

        /// <summary>
        /// The braille dots 1567
        /// </summary>
        braille_dots_1567 = 0x1002871 /* U+2871 BRAILLE PATTERN DOTS-1567 */,

        /// <summary>
        /// The braille dots 2567
        /// </summary>
        braille_dots_2567 = 0x1002872 /* U+2872 BRAILLE PATTERN DOTS-2567 */,

        /// <summary>
        /// The braille dots 12567
        /// </summary>
        braille_dots_12567 = 0x1002873 /* U+2873 BRAILLE PATTERN DOTS-12567 */,

        /// <summary>
        /// The braille dots 3567
        /// </summary>
        braille_dots_3567 = 0x1002874 /* U+2874 BRAILLE PATTERN DOTS-3567 */,

        /// <summary>
        /// The braille dots 13567
        /// </summary>
        braille_dots_13567 = 0x1002875 /* U+2875 BRAILLE PATTERN DOTS-13567 */,

        /// <summary>
        /// The braille dots 23567
        /// </summary>
        braille_dots_23567 = 0x1002876 /* U+2876 BRAILLE PATTERN DOTS-23567 */,

        /// <summary>
        /// The braille dots 123567
        /// </summary>
        braille_dots_123567 = 0x1002877 /* U+2877 BRAILLE PATTERN DOTS-123567 */,

        /// <summary>
        /// The braille dots 4567
        /// </summary>
        braille_dots_4567 = 0x1002878 /* U+2878 BRAILLE PATTERN DOTS-4567 */,

        /// <summary>
        /// The braille dots 14567
        /// </summary>
        braille_dots_14567 = 0x1002879 /* U+2879 BRAILLE PATTERN DOTS-14567 */,

        /// <summary>
        /// The braille dots 24567
        /// </summary>
        braille_dots_24567 = 0x100287a /* U+287a BRAILLE PATTERN DOTS-24567 */,

        /// <summary>
        /// The braille dots 124567
        /// </summary>
        braille_dots_124567 = 0x100287b /* U+287b BRAILLE PATTERN DOTS-124567 */,

        /// <summary>
        /// The braille dots 34567
        /// </summary>
        braille_dots_34567 = 0x100287c /* U+287c BRAILLE PATTERN DOTS-34567 */,

        /// <summary>
        /// The braille dots 134567
        /// </summary>
        braille_dots_134567 = 0x100287d /* U+287d BRAILLE PATTERN DOTS-134567 */,

        /// <summary>
        /// The braille dots 234567
        /// </summary>
        braille_dots_234567 = 0x100287e /* U+287e BRAILLE PATTERN DOTS-234567 */,

        /// <summary>
        /// The braille dots 1234567
        /// </summary>
        braille_dots_1234567 = 0x100287f /* U+287f BRAILLE PATTERN DOTS-1234567 */,

        /// <summary>
        /// The braille dots 8
        /// </summary>
        braille_dots_8 = 0x1002880 /* U+2880 BRAILLE PATTERN DOTS-8 */,

        /// <summary>
        /// The braille dots 18
        /// </summary>
        braille_dots_18 = 0x1002881 /* U+2881 BRAILLE PATTERN DOTS-18 */,

        /// <summary>
        /// The braille dots 28
        /// </summary>
        braille_dots_28 = 0x1002882 /* U+2882 BRAILLE PATTERN DOTS-28 */,

        /// <summary>
        /// The braille dots 128
        /// </summary>
        braille_dots_128 = 0x1002883 /* U+2883 BRAILLE PATTERN DOTS-128 */,

        /// <summary>
        /// The braille dots 38
        /// </summary>
        braille_dots_38 = 0x1002884 /* U+2884 BRAILLE PATTERN DOTS-38 */,

        /// <summary>
        /// The braille dots 138
        /// </summary>
        braille_dots_138 = 0x1002885 /* U+2885 BRAILLE PATTERN DOTS-138 */,

        /// <summary>
        /// The braille dots 238
        /// </summary>
        braille_dots_238 = 0x1002886 /* U+2886 BRAILLE PATTERN DOTS-238 */,

        /// <summary>
        /// The braille dots 1238
        /// </summary>
        braille_dots_1238 = 0x1002887 /* U+2887 BRAILLE PATTERN DOTS-1238 */,

        /// <summary>
        /// The braille dots 48
        /// </summary>
        braille_dots_48 = 0x1002888 /* U+2888 BRAILLE PATTERN DOTS-48 */,

        /// <summary>
        /// The braille dots 148
        /// </summary>
        braille_dots_148 = 0x1002889 /* U+2889 BRAILLE PATTERN DOTS-148 */,

        /// <summary>
        /// The braille dots 248
        /// </summary>
        braille_dots_248 = 0x100288a /* U+288a BRAILLE PATTERN DOTS-248 */,

        /// <summary>
        /// The braille dots 1248
        /// </summary>
        braille_dots_1248 = 0x100288b /* U+288b BRAILLE PATTERN DOTS-1248 */,

        /// <summary>
        /// The braille dots 348
        /// </summary>
        braille_dots_348 = 0x100288c /* U+288c BRAILLE PATTERN DOTS-348 */,

        /// <summary>
        /// The braille dots 1348
        /// </summary>
        braille_dots_1348 = 0x100288d /* U+288d BRAILLE PATTERN DOTS-1348 */,

        /// <summary>
        /// The braille dots 2348
        /// </summary>
        braille_dots_2348 = 0x100288e /* U+288e BRAILLE PATTERN DOTS-2348 */,

        /// <summary>
        /// The braille dots 12348
        /// </summary>
        braille_dots_12348 = 0x100288f /* U+288f BRAILLE PATTERN DOTS-12348 */,

        /// <summary>
        /// The braille dots 58
        /// </summary>
        braille_dots_58 = 0x1002890 /* U+2890 BRAILLE PATTERN DOTS-58 */,

        /// <summary>
        /// The braille dots 158
        /// </summary>
        braille_dots_158 = 0x1002891 /* U+2891 BRAILLE PATTERN DOTS-158 */,

        /// <summary>
        /// The braille dots 258
        /// </summary>
        braille_dots_258 = 0x1002892 /* U+2892 BRAILLE PATTERN DOTS-258 */,

        /// <summary>
        /// The braille dots 1258
        /// </summary>
        braille_dots_1258 = 0x1002893 /* U+2893 BRAILLE PATTERN DOTS-1258 */,

        /// <summary>
        /// The braille dots 358
        /// </summary>
        braille_dots_358 = 0x1002894 /* U+2894 BRAILLE PATTERN DOTS-358 */,

        /// <summary>
        /// The braille dots 1358
        /// </summary>
        braille_dots_1358 = 0x1002895 /* U+2895 BRAILLE PATTERN DOTS-1358 */,

        /// <summary>
        /// The braille dots 2358
        /// </summary>
        braille_dots_2358 = 0x1002896 /* U+2896 BRAILLE PATTERN DOTS-2358 */,

        /// <summary>
        /// The braille dots 12358
        /// </summary>
        braille_dots_12358 = 0x1002897 /* U+2897 BRAILLE PATTERN DOTS-12358 */,

        /// <summary>
        /// The braille dots 458
        /// </summary>
        braille_dots_458 = 0x1002898 /* U+2898 BRAILLE PATTERN DOTS-458 */,

        /// <summary>
        /// The braille dots 1458
        /// </summary>
        braille_dots_1458 = 0x1002899 /* U+2899 BRAILLE PATTERN DOTS-1458 */,

        /// <summary>
        /// The braille dots 2458
        /// </summary>
        braille_dots_2458 = 0x100289a /* U+289a BRAILLE PATTERN DOTS-2458 */,

        /// <summary>
        /// The braille dots 12458
        /// </summary>
        braille_dots_12458 = 0x100289b /* U+289b BRAILLE PATTERN DOTS-12458 */,

        /// <summary>
        /// The braille dots 3458
        /// </summary>
        braille_dots_3458 = 0x100289c /* U+289c BRAILLE PATTERN DOTS-3458 */,

        /// <summary>
        /// The braille dots 13458
        /// </summary>
        braille_dots_13458 = 0x100289d /* U+289d BRAILLE PATTERN DOTS-13458 */,

        /// <summary>
        /// The braille dots 23458
        /// </summary>
        braille_dots_23458 = 0x100289e /* U+289e BRAILLE PATTERN DOTS-23458 */,

        /// <summary>
        /// The braille dots 123458
        /// </summary>
        braille_dots_123458 = 0x100289f /* U+289f BRAILLE PATTERN DOTS-123458 */,

        /// <summary>
        /// The braille dots 68
        /// </summary>
        braille_dots_68 = 0x10028a0 /* U+28a0 BRAILLE PATTERN DOTS-68 */,

        /// <summary>
        /// The braille dots 168
        /// </summary>
        braille_dots_168 = 0x10028a1 /* U+28a1 BRAILLE PATTERN DOTS-168 */,

        /// <summary>
        /// The braille dots 268
        /// </summary>
        braille_dots_268 = 0x10028a2 /* U+28a2 BRAILLE PATTERN DOTS-268 */,

        /// <summary>
        /// The braille dots 1268
        /// </summary>
        braille_dots_1268 = 0x10028a3 /* U+28a3 BRAILLE PATTERN DOTS-1268 */,

        /// <summary>
        /// The braille dots 368
        /// </summary>
        braille_dots_368 = 0x10028a4 /* U+28a4 BRAILLE PATTERN DOTS-368 */,

        /// <summary>
        /// The braille dots 1368
        /// </summary>
        braille_dots_1368 = 0x10028a5 /* U+28a5 BRAILLE PATTERN DOTS-1368 */,

        /// <summary>
        /// The braille dots 2368
        /// </summary>
        braille_dots_2368 = 0x10028a6 /* U+28a6 BRAILLE PATTERN DOTS-2368 */,

        /// <summary>
        /// The braille dots 12368
        /// </summary>
        braille_dots_12368 = 0x10028a7 /* U+28a7 BRAILLE PATTERN DOTS-12368 */,

        /// <summary>
        /// The braille dots 468
        /// </summary>
        braille_dots_468 = 0x10028a8 /* U+28a8 BRAILLE PATTERN DOTS-468 */,

        /// <summary>
        /// The braille dots 1468
        /// </summary>
        braille_dots_1468 = 0x10028a9 /* U+28a9 BRAILLE PATTERN DOTS-1468 */,

        /// <summary>
        /// The braille dots 2468
        /// </summary>
        braille_dots_2468 = 0x10028aa /* U+28aa BRAILLE PATTERN DOTS-2468 */,

        /// <summary>
        /// The braille dots 12468
        /// </summary>
        braille_dots_12468 = 0x10028ab /* U+28ab BRAILLE PATTERN DOTS-12468 */,

        /// <summary>
        /// The braille dots 3468
        /// </summary>
        braille_dots_3468 = 0x10028ac /* U+28ac BRAILLE PATTERN DOTS-3468 */,

        /// <summary>
        /// The braille dots 13468
        /// </summary>
        braille_dots_13468 = 0x10028ad /* U+28ad BRAILLE PATTERN DOTS-13468 */,

        /// <summary>
        /// The braille dots 23468
        /// </summary>
        braille_dots_23468 = 0x10028ae /* U+28ae BRAILLE PATTERN DOTS-23468 */,

        /// <summary>
        /// The braille dots 123468
        /// </summary>
        braille_dots_123468 = 0x10028af /* U+28af BRAILLE PATTERN DOTS-123468 */,

        /// <summary>
        /// The braille dots 568
        /// </summary>
        braille_dots_568 = 0x10028b0 /* U+28b0 BRAILLE PATTERN DOTS-568 */,

        /// <summary>
        /// The braille dots 1568
        /// </summary>
        braille_dots_1568 = 0x10028b1 /* U+28b1 BRAILLE PATTERN DOTS-1568 */,

        /// <summary>
        /// The braille dots 2568
        /// </summary>
        braille_dots_2568 = 0x10028b2 /* U+28b2 BRAILLE PATTERN DOTS-2568 */,

        /// <summary>
        /// The braille dots 12568
        /// </summary>
        braille_dots_12568 = 0x10028b3 /* U+28b3 BRAILLE PATTERN DOTS-12568 */,

        /// <summary>
        /// The braille dots 3568
        /// </summary>
        braille_dots_3568 = 0x10028b4 /* U+28b4 BRAILLE PATTERN DOTS-3568 */,

        /// <summary>
        /// The braille dots 13568
        /// </summary>
        braille_dots_13568 = 0x10028b5 /* U+28b5 BRAILLE PATTERN DOTS-13568 */,

        /// <summary>
        /// The braille dots 23568
        /// </summary>
        braille_dots_23568 = 0x10028b6 /* U+28b6 BRAILLE PATTERN DOTS-23568 */,

        /// <summary>
        /// The braille dots 123568
        /// </summary>
        braille_dots_123568 = 0x10028b7 /* U+28b7 BRAILLE PATTERN DOTS-123568 */,

        /// <summary>
        /// The braille dots 4568
        /// </summary>
        braille_dots_4568 = 0x10028b8 /* U+28b8 BRAILLE PATTERN DOTS-4568 */,

        /// <summary>
        /// The braille dots 14568
        /// </summary>
        braille_dots_14568 = 0x10028b9 /* U+28b9 BRAILLE PATTERN DOTS-14568 */,

        /// <summary>
        /// The braille dots 24568
        /// </summary>
        braille_dots_24568 = 0x10028ba /* U+28ba BRAILLE PATTERN DOTS-24568 */,

        /// <summary>
        /// The braille dots 124568
        /// </summary>
        braille_dots_124568 = 0x10028bb /* U+28bb BRAILLE PATTERN DOTS-124568 */,

        /// <summary>
        /// The braille dots 34568
        /// </summary>
        braille_dots_34568 = 0x10028bc /* U+28bc BRAILLE PATTERN DOTS-34568 */,

        /// <summary>
        /// The braille dots 134568
        /// </summary>
        braille_dots_134568 = 0x10028bd /* U+28bd BRAILLE PATTERN DOTS-134568 */,

        /// <summary>
        /// The braille dots 234568
        /// </summary>
        braille_dots_234568 = 0x10028be /* U+28be BRAILLE PATTERN DOTS-234568 */,

        /// <summary>
        /// The braille dots 1234568
        /// </summary>
        braille_dots_1234568 = 0x10028bf /* U+28bf BRAILLE PATTERN DOTS-1234568 */,

        /// <summary>
        /// The braille dots 78
        /// </summary>
        braille_dots_78 = 0x10028c0 /* U+28c0 BRAILLE PATTERN DOTS-78 */,

        /// <summary>
        /// The braille dots 178
        /// </summary>
        braille_dots_178 = 0x10028c1 /* U+28c1 BRAILLE PATTERN DOTS-178 */,

        /// <summary>
        /// The braille dots 278
        /// </summary>
        braille_dots_278 = 0x10028c2 /* U+28c2 BRAILLE PATTERN DOTS-278 */,

        /// <summary>
        /// The braille dots 1278
        /// </summary>
        braille_dots_1278 = 0x10028c3 /* U+28c3 BRAILLE PATTERN DOTS-1278 */,

        /// <summary>
        /// The braille dots 378
        /// </summary>
        braille_dots_378 = 0x10028c4 /* U+28c4 BRAILLE PATTERN DOTS-378 */,

        /// <summary>
        /// The braille dots 1378
        /// </summary>
        braille_dots_1378 = 0x10028c5 /* U+28c5 BRAILLE PATTERN DOTS-1378 */,

        /// <summary>
        /// The braille dots 2378
        /// </summary>
        braille_dots_2378 = 0x10028c6 /* U+28c6 BRAILLE PATTERN DOTS-2378 */,

        /// <summary>
        /// The braille dots 12378
        /// </summary>
        braille_dots_12378 = 0x10028c7 /* U+28c7 BRAILLE PATTERN DOTS-12378 */,

        /// <summary>
        /// The braille dots 478
        /// </summary>
        braille_dots_478 = 0x10028c8 /* U+28c8 BRAILLE PATTERN DOTS-478 */,

        /// <summary>
        /// The braille dots 1478
        /// </summary>
        braille_dots_1478 = 0x10028c9 /* U+28c9 BRAILLE PATTERN DOTS-1478 */,

        /// <summary>
        /// The braille dots 2478
        /// </summary>
        braille_dots_2478 = 0x10028ca /* U+28ca BRAILLE PATTERN DOTS-2478 */,

        /// <summary>
        /// The braille dots 12478
        /// </summary>
        braille_dots_12478 = 0x10028cb /* U+28cb BRAILLE PATTERN DOTS-12478 */,

        /// <summary>
        /// The braille dots 3478
        /// </summary>
        braille_dots_3478 = 0x10028cc /* U+28cc BRAILLE PATTERN DOTS-3478 */,

        /// <summary>
        /// The braille dots 13478
        /// </summary>
        braille_dots_13478 = 0x10028cd /* U+28cd BRAILLE PATTERN DOTS-13478 */,

        /// <summary>
        /// The braille dots 23478
        /// </summary>
        braille_dots_23478 = 0x10028ce /* U+28ce BRAILLE PATTERN DOTS-23478 */,

        /// <summary>
        /// The braille dots 123478
        /// </summary>
        braille_dots_123478 = 0x10028cf /* U+28cf BRAILLE PATTERN DOTS-123478 */,

        /// <summary>
        /// The braille dots 578
        /// </summary>
        braille_dots_578 = 0x10028d0 /* U+28d0 BRAILLE PATTERN DOTS-578 */,

        /// <summary>
        /// The braille dots 1578
        /// </summary>
        braille_dots_1578 = 0x10028d1 /* U+28d1 BRAILLE PATTERN DOTS-1578 */,

        /// <summary>
        /// The braille dots 2578
        /// </summary>
        braille_dots_2578 = 0x10028d2 /* U+28d2 BRAILLE PATTERN DOTS-2578 */,

        /// <summary>
        /// The braille dots 12578
        /// </summary>
        braille_dots_12578 = 0x10028d3 /* U+28d3 BRAILLE PATTERN DOTS-12578 */,

        /// <summary>
        /// The braille dots 3578
        /// </summary>
        braille_dots_3578 = 0x10028d4 /* U+28d4 BRAILLE PATTERN DOTS-3578 */,

        /// <summary>
        /// The braille dots 13578
        /// </summary>
        braille_dots_13578 = 0x10028d5 /* U+28d5 BRAILLE PATTERN DOTS-13578 */,

        /// <summary>
        /// The braille dots 23578
        /// </summary>
        braille_dots_23578 = 0x10028d6 /* U+28d6 BRAILLE PATTERN DOTS-23578 */,

        /// <summary>
        /// The braille dots 123578
        /// </summary>
        braille_dots_123578 = 0x10028d7 /* U+28d7 BRAILLE PATTERN DOTS-123578 */,

        /// <summary>
        /// The braille dots 4578
        /// </summary>
        braille_dots_4578 = 0x10028d8 /* U+28d8 BRAILLE PATTERN DOTS-4578 */,

        /// <summary>
        /// The braille dots 14578
        /// </summary>
        braille_dots_14578 = 0x10028d9 /* U+28d9 BRAILLE PATTERN DOTS-14578 */,

        /// <summary>
        /// The braille dots 24578
        /// </summary>
        braille_dots_24578 = 0x10028da /* U+28da BRAILLE PATTERN DOTS-24578 */,

        /// <summary>
        /// The braille dots 124578
        /// </summary>
        braille_dots_124578 = 0x10028db /* U+28db BRAILLE PATTERN DOTS-124578 */,

        /// <summary>
        /// The braille dots 34578
        /// </summary>
        braille_dots_34578 = 0x10028dc /* U+28dc BRAILLE PATTERN DOTS-34578 */,

        /// <summary>
        /// The braille dots 134578
        /// </summary>
        braille_dots_134578 = 0x10028dd /* U+28dd BRAILLE PATTERN DOTS-134578 */,

        /// <summary>
        /// The braille dots 234578
        /// </summary>
        braille_dots_234578 = 0x10028de /* U+28de BRAILLE PATTERN DOTS-234578 */,

        /// <summary>
        /// The braille dots 1234578
        /// </summary>
        braille_dots_1234578 = 0x10028df /* U+28df BRAILLE PATTERN DOTS-1234578 */,

        /// <summary>
        /// The braille dots 678
        /// </summary>
        braille_dots_678 = 0x10028e0 /* U+28e0 BRAILLE PATTERN DOTS-678 */,

        /// <summary>
        /// The braille dots 1678
        /// </summary>
        braille_dots_1678 = 0x10028e1 /* U+28e1 BRAILLE PATTERN DOTS-1678 */,

        /// <summary>
        /// The braille dots 2678
        /// </summary>
        braille_dots_2678 = 0x10028e2 /* U+28e2 BRAILLE PATTERN DOTS-2678 */,

        /// <summary>
        /// The braille dots 12678
        /// </summary>
        braille_dots_12678 = 0x10028e3 /* U+28e3 BRAILLE PATTERN DOTS-12678 */,

        /// <summary>
        /// The braille dots 3678
        /// </summary>
        braille_dots_3678 = 0x10028e4 /* U+28e4 BRAILLE PATTERN DOTS-3678 */,

        /// <summary>
        /// The braille dots 13678
        /// </summary>
        braille_dots_13678 = 0x10028e5 /* U+28e5 BRAILLE PATTERN DOTS-13678 */,

        /// <summary>
        /// The braille dots 23678
        /// </summary>
        braille_dots_23678 = 0x10028e6 /* U+28e6 BRAILLE PATTERN DOTS-23678 */,

        /// <summary>
        /// The braille dots 123678
        /// </summary>
        braille_dots_123678 = 0x10028e7 /* U+28e7 BRAILLE PATTERN DOTS-123678 */,

        /// <summary>
        /// The braille dots 4678
        /// </summary>
        braille_dots_4678 = 0x10028e8 /* U+28e8 BRAILLE PATTERN DOTS-4678 */,

        /// <summary>
        /// The braille dots 14678
        /// </summary>
        braille_dots_14678 = 0x10028e9 /* U+28e9 BRAILLE PATTERN DOTS-14678 */,

        /// <summary>
        /// The braille dots 24678
        /// </summary>
        braille_dots_24678 = 0x10028ea /* U+28ea BRAILLE PATTERN DOTS-24678 */,

        /// <summary>
        /// The braille dots 124678
        /// </summary>
        braille_dots_124678 = 0x10028eb /* U+28eb BRAILLE PATTERN DOTS-124678 */,

        /// <summary>
        /// The braille dots 34678
        /// </summary>
        braille_dots_34678 = 0x10028ec /* U+28ec BRAILLE PATTERN DOTS-34678 */,

        /// <summary>
        /// The braille dots 134678
        /// </summary>
        braille_dots_134678 = 0x10028ed /* U+28ed BRAILLE PATTERN DOTS-134678 */,

        /// <summary>
        /// The braille dots 234678
        /// </summary>
        braille_dots_234678 = 0x10028ee /* U+28ee BRAILLE PATTERN DOTS-234678 */,

        /// <summary>
        /// The braille dots 1234678
        /// </summary>
        braille_dots_1234678 = 0x10028ef /* U+28ef BRAILLE PATTERN DOTS-1234678 */,

        /// <summary>
        /// The braille dots 5678
        /// </summary>
        braille_dots_5678 = 0x10028f0 /* U+28f0 BRAILLE PATTERN DOTS-5678 */,

        /// <summary>
        /// The braille dots 15678
        /// </summary>
        braille_dots_15678 = 0x10028f1 /* U+28f1 BRAILLE PATTERN DOTS-15678 */,

        /// <summary>
        /// The braille dots 25678
        /// </summary>
        braille_dots_25678 = 0x10028f2 /* U+28f2 BRAILLE PATTERN DOTS-25678 */,

        /// <summary>
        /// The braille dots 125678
        /// </summary>
        braille_dots_125678 = 0x10028f3 /* U+28f3 BRAILLE PATTERN DOTS-125678 */,

        /// <summary>
        /// The braille dots 35678
        /// </summary>
        braille_dots_35678 = 0x10028f4 /* U+28f4 BRAILLE PATTERN DOTS-35678 */,

        /// <summary>
        /// The braille dots 135678
        /// </summary>
        braille_dots_135678 = 0x10028f5 /* U+28f5 BRAILLE PATTERN DOTS-135678 */,

        /// <summary>
        /// The braille dots 235678
        /// </summary>
        braille_dots_235678 = 0x10028f6 /* U+28f6 BRAILLE PATTERN DOTS-235678 */,

        /// <summary>
        /// The braille dots 1235678
        /// </summary>
        braille_dots_1235678 = 0x10028f7 /* U+28f7 BRAILLE PATTERN DOTS-1235678 */,

        /// <summary>
        /// The braille dots 45678
        /// </summary>
        braille_dots_45678 = 0x10028f8 /* U+28f8 BRAILLE PATTERN DOTS-45678 */,

        /// <summary>
        /// The braille dots 145678
        /// </summary>
        braille_dots_145678 = 0x10028f9 /* U+28f9 BRAILLE PATTERN DOTS-145678 */,

        /// <summary>
        /// The braille dots 245678
        /// </summary>
        braille_dots_245678 = 0x10028fa /* U+28fa BRAILLE PATTERN DOTS-245678 */,

        /// <summary>
        /// The braille dots 1245678
        /// </summary>
        braille_dots_1245678 = 0x10028fb /* U+28fb BRAILLE PATTERN DOTS-1245678 */,

        /// <summary>
        /// The braille dots 345678
        /// </summary>
        braille_dots_345678 = 0x10028fc /* U+28fc BRAILLE PATTERN DOTS-345678 */,

        /// <summary>
        /// The braille dots 1345678
        /// </summary>
        braille_dots_1345678 = 0x10028fd /* U+28fd BRAILLE PATTERN DOTS-1345678 */,

        /// <summary>
        /// The braille dots 2345678
        /// </summary>
        braille_dots_2345678 = 0x10028fe /* U+28fe BRAILLE PATTERN DOTS-2345678 */,

        /// <summary>
        /// The braille dots 12345678
        /// </summary>
        braille_dots_12345678 = 0x10028ff /* U+28ff BRAILLE PATTERN DOTS-12345678 */,

        /// <summary>
        /// The sinh ng
        /// </summary>
        Sinh_ng = 0x1000d82 /* U+0D82 SINHALA ANUSVARAYA */,

        /// <summary>
        /// The sinh h2
        /// </summary>
        Sinh_h2 = 0x1000d83 /* U+0D83 SINHALA VISARGAYA */,

        /// <summary>
        /// The sinh a
        /// </summary>
        Sinh_a = 0x1000d85 /* U+0D85 SINHALA AYANNA */,

        /// <summary>
        /// The sinh aa
        /// </summary>
        Sinh_aa = 0x1000d86 /* U+0D86 SINHALA AAYANNA */,

        /// <summary>
        /// The sinh ae
        /// </summary>
        Sinh_ae = 0x1000d87 /* U+0D87 SINHALA AEYANNA */,

        /// <summary>
        /// The sinh aee
        /// </summary>
        Sinh_aee = 0x1000d88 /* U+0D88 SINHALA AEEYANNA */,

        /// <summary>
        /// The sinh i
        /// </summary>
        Sinh_i = 0x1000d89 /* U+0D89 SINHALA IYANNA */,

        /// <summary>
        /// The sinh ii
        /// </summary>
        Sinh_ii = 0x1000d8a /* U+0D8A SINHALA IIYANNA */,

        /// <summary>
        /// The sinh u
        /// </summary>
        Sinh_u = 0x1000d8b /* U+0D8B SINHALA UYANNA */,

        /// <summary>
        /// The sinh uu
        /// </summary>
        Sinh_uu = 0x1000d8c /* U+0D8C SINHALA UUYANNA */,

        /// <summary>
        /// The sinh ri
        /// </summary>
        Sinh_ri = 0x1000d8d /* U+0D8D SINHALA IRUYANNA */,

        /// <summary>
        /// The sinh rii
        /// </summary>
        Sinh_rii = 0x1000d8e /* U+0D8E SINHALA IRUUYANNA */,

        /// <summary>
        /// The sinh lu
        /// </summary>
        Sinh_lu = 0x1000d8f /* U+0D8F SINHALA ILUYANNA */,

        /// <summary>
        /// The sinh luu
        /// </summary>
        Sinh_luu = 0x1000d90 /* U+0D90 SINHALA ILUUYANNA */,

        /// <summary>
        /// The sinh e
        /// </summary>
        Sinh_e = 0x1000d91 /* U+0D91 SINHALA EYANNA */,

        /// <summary>
        /// The sinh ee
        /// </summary>
        Sinh_ee = 0x1000d92 /* U+0D92 SINHALA EEYANNA */,

        /// <summary>
        /// The sinh ai
        /// </summary>
        Sinh_ai = 0x1000d93 /* U+0D93 SINHALA AIYANNA */,

        /// <summary>
        /// The sinh o
        /// </summary>
        Sinh_o = 0x1000d94 /* U+0D94 SINHALA OYANNA */,

        /// <summary>
        /// The sinh oo
        /// </summary>
        Sinh_oo = 0x1000d95 /* U+0D95 SINHALA OOYANNA */,

        /// <summary>
        /// The sinh au
        /// </summary>
        Sinh_au = 0x1000d96 /* U+0D96 SINHALA AUYANNA */,

        /// <summary>
        /// The sinh ka
        /// </summary>
        Sinh_ka = 0x1000d9a /* U+0D9A SINHALA KAYANNA */,

        /// <summary>
        /// The sinh kha
        /// </summary>
        Sinh_kha = 0x1000d9b /* U+0D9B SINHALA MAHA. KAYANNA */,

        /// <summary>
        /// The sinh ga
        /// </summary>
        Sinh_ga = 0x1000d9c /* U+0D9C SINHALA GAYANNA */,

        /// <summary>
        /// The sinh gha
        /// </summary>
        Sinh_gha = 0x1000d9d /* U+0D9D SINHALA MAHA. GAYANNA */,

        /// <summary>
        /// The sinh NG2
        /// </summary>
        Sinh_ng2 = 0x1000d9e /* U+0D9E SINHALA KANTAJA NAASIKYAYA */,

        /// <summary>
        /// The sinh nga
        /// </summary>
        Sinh_nga = 0x1000d9f /* U+0D9F SINHALA SANYAKA GAYANNA */,

        /// <summary>
        /// The sinh ca
        /// </summary>
        Sinh_ca = 0x1000da0 /* U+0DA0 SINHALA CAYANNA */,

        /// <summary>
        /// The sinh cha
        /// </summary>
        Sinh_cha = 0x1000da1 /* U+0DA1 SINHALA MAHA. CAYANNA */,

        /// <summary>
        /// The sinh ja
        /// </summary>
        Sinh_ja = 0x1000da2 /* U+0DA2 SINHALA JAYANNA */,

        /// <summary>
        /// The sinh jha
        /// </summary>
        Sinh_jha = 0x1000da3 /* U+0DA3 SINHALA MAHA. JAYANNA */,

        /// <summary>
        /// The sinh nya
        /// </summary>
        Sinh_nya = 0x1000da4 /* U+0DA4 SINHALA TAALUJA NAASIKYAYA */,

        /// <summary>
        /// The sinh jnya
        /// </summary>
        Sinh_jnya = 0x1000da5 /* U+0DA5 SINHALA TAALUJA SANYOOGA NAASIKYAYA */,

        /// <summary>
        /// The sinh nja
        /// </summary>
        Sinh_nja = 0x1000da6 /* U+0DA6 SINHALA SANYAKA JAYANNA */,

        /// <summary>
        /// The sinh tta
        /// </summary>
        Sinh_tta = 0x1000da7 /* U+0DA7 SINHALA TTAYANNA */,

        /// <summary>
        /// The sinh ttha
        /// </summary>
        Sinh_ttha = 0x1000da8 /* U+0DA8 SINHALA MAHA. TTAYANNA */,

        /// <summary>
        /// The sinh dda
        /// </summary>
        Sinh_dda = 0x1000da9 /* U+0DA9 SINHALA DDAYANNA */,

        /// <summary>
        /// The sinh ddha
        /// </summary>
        Sinh_ddha = 0x1000daa /* U+0DAA SINHALA MAHA. DDAYANNA */,

        /// <summary>
        /// The sinh nna
        /// </summary>
        Sinh_nna = 0x1000dab /* U+0DAB SINHALA MUURDHAJA NAYANNA */,

        /// <summary>
        /// The sinh ndda
        /// </summary>
        Sinh_ndda = 0x1000dac /* U+0DAC SINHALA SANYAKA DDAYANNA */,

        /// <summary>
        /// The sinh tha
        /// </summary>
        Sinh_tha = 0x1000dad /* U+0DAD SINHALA TAYANNA */,

        /// <summary>
        /// The sinh thha
        /// </summary>
        Sinh_thha = 0x1000dae /* U+0DAE SINHALA MAHA. TAYANNA */,

        /// <summary>
        /// The sinh dha
        /// </summary>
        Sinh_dha = 0x1000daf /* U+0DAF SINHALA DAYANNA */,

        /// <summary>
        /// The sinh dhha
        /// </summary>
        Sinh_dhha = 0x1000db0 /* U+0DB0 SINHALA MAHA. DAYANNA */,

        /// <summary>
        /// The sinh na
        /// </summary>
        Sinh_na = 0x1000db1 /* U+0DB1 SINHALA DANTAJA NAYANNA */,

        /// <summary>
        /// The sinh ndha
        /// </summary>
        Sinh_ndha = 0x1000db3 /* U+0DB3 SINHALA SANYAKA DAYANNA */,

        /// <summary>
        /// The sinh pa
        /// </summary>
        Sinh_pa = 0x1000db4 /* U+0DB4 SINHALA PAYANNA */,

        /// <summary>
        /// The sinh pha
        /// </summary>
        Sinh_pha = 0x1000db5 /* U+0DB5 SINHALA MAHA. PAYANNA */,

        /// <summary>
        /// The sinh ba
        /// </summary>
        Sinh_ba = 0x1000db6 /* U+0DB6 SINHALA BAYANNA */,

        /// <summary>
        /// The sinh bha
        /// </summary>
        Sinh_bha = 0x1000db7 /* U+0DB7 SINHALA MAHA. BAYANNA */,

        /// <summary>
        /// The sinh ma
        /// </summary>
        Sinh_ma = 0x1000db8 /* U+0DB8 SINHALA MAYANNA */,

        /// <summary>
        /// The sinh mba
        /// </summary>
        Sinh_mba = 0x1000db9 /* U+0DB9 SINHALA AMBA BAYANNA */,

        /// <summary>
        /// The sinh ya
        /// </summary>
        Sinh_ya = 0x1000dba /* U+0DBA SINHALA YAYANNA */,

        /// <summary>
        /// The sinh ra
        /// </summary>
        Sinh_ra = 0x1000dbb /* U+0DBB SINHALA RAYANNA */,

        /// <summary>
        /// The sinh la
        /// </summary>
        Sinh_la = 0x1000dbd /* U+0DBD SINHALA DANTAJA LAYANNA */,

        /// <summary>
        /// The sinh va
        /// </summary>
        Sinh_va = 0x1000dc0 /* U+0DC0 SINHALA VAYANNA */,

        /// <summary>
        /// The sinh sha
        /// </summary>
        Sinh_sha = 0x1000dc1 /* U+0DC1 SINHALA TAALUJA SAYANNA */,

        /// <summary>
        /// The sinh ssha
        /// </summary>
        Sinh_ssha = 0x1000dc2 /* U+0DC2 SINHALA MUURDHAJA SAYANNA */,

        /// <summary>
        /// The sinh sa
        /// </summary>
        Sinh_sa = 0x1000dc3 /* U+0DC3 SINHALA DANTAJA SAYANNA */,

        /// <summary>
        /// The sinh ha
        /// </summary>
        Sinh_ha = 0x1000dc4 /* U+0DC4 SINHALA HAYANNA */,

        /// <summary>
        /// The sinh lla
        /// </summary>
        Sinh_lla = 0x1000dc5 /* U+0DC5 SINHALA MUURDHAJA LAYANNA */,

        /// <summary>
        /// The sinh fa
        /// </summary>
        Sinh_fa = 0x1000dc6 /* U+0DC6 SINHALA FAYANNA */,

        /// <summary>
        /// The sinh al
        /// </summary>
        Sinh_al = 0x1000dca /* U+0DCA SINHALA AL-LAKUNA */,

        /// <summary>
        /// The sinh aa2
        /// </summary>
        Sinh_aa2 = 0x1000dcf /* U+0DCF SINHALA AELA-PILLA */,

        /// <summary>
        /// The sinh ae2
        /// </summary>
        Sinh_ae2 = 0x1000dd0 /* U+0DD0 SINHALA AEDA-PILLA */,

        /// <summary>
        /// The sinh aee2
        /// </summary>
        Sinh_aee2 = 0x1000dd1 /* U+0DD1 SINHALA DIGA AEDA-PILLA */,

        /// <summary>
        /// The sinh i2
        /// </summary>
        Sinh_i2 = 0x1000dd2 /* U+0DD2 SINHALA IS-PILLA */,

        /// <summary>
        /// The sinh ii2
        /// </summary>
        Sinh_ii2 = 0x1000dd3 /* U+0DD3 SINHALA DIGA IS-PILLA */,

        /// <summary>
        /// The sinh u2
        /// </summary>
        Sinh_u2 = 0x1000dd4 /* U+0DD4 SINHALA PAA-PILLA */,

        /// <summary>
        /// The sinh uu2
        /// </summary>
        Sinh_uu2 = 0x1000dd6 /* U+0DD6 SINHALA DIGA PAA-PILLA */,

        /// <summary>
        /// The sinh ru2
        /// </summary>
        Sinh_ru2 = 0x1000dd8 /* U+0DD8 SINHALA GAETTA-PILLA */,

        /// <summary>
        /// The sinh e2
        /// </summary>
        Sinh_e2 = 0x1000dd9 /* U+0DD9 SINHALA KOMBUVA */,

        /// <summary>
        /// The sinh ee2
        /// </summary>
        Sinh_ee2 = 0x1000dda /* U+0DDA SINHALA DIGA KOMBUVA */,

        /// <summary>
        /// The sinh ai2
        /// </summary>
        Sinh_ai2 = 0x1000ddb /* U+0DDB SINHALA KOMBU DEKA */,

        /// <summary>
        /// The sinh o2
        /// </summary>
        Sinh_o2 = 0x1000ddc /* U+0DDC SINHALA KOMBUVA HAA AELA-PILLA*/,

        /// <summary>
        /// The sinh oo2
        /// </summary>
        Sinh_oo2 = 0x1000ddd /* U+0DDD SINHALA KOMBUVA HAA DIGA AELA-PILLA*/,

        /// <summary>
        /// The sinh au2
        /// </summary>
        Sinh_au2 = 0x1000dde /* U+0DDE SINHALA KOMBUVA HAA GAYANUKITTA */,

        /// <summary>
        /// The sinh lu2
        /// </summary>
        Sinh_lu2 = 0x1000ddf /* U+0DDF SINHALA GAYANUKITTA */,

        /// <summary>
        /// The sinh ruu2
        /// </summary>
        Sinh_ruu2 = 0x1000df2 /* U+0DF2 SINHALA DIGA GAETTA-PILLA */,

        /// <summary>
        /// The sinh luu2
        /// </summary>
        Sinh_luu2 = 0x1000df3 /* U+0DF3 SINHALA DIGA GAYANUKITTA */,

        /// <summary>
        /// The sinh kunddaliya
        /// </summary>
        Sinh_kunddaliya = 0x1000df4 /* U+0DF4 SINHALA KUNDDALIYA */
    }
}
