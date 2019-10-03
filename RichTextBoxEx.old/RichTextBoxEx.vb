Imports System.ComponentModel
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions

Public Class RichTextBoxEx
    Inherits System.Windows.Forms.UserControl

    '   PRIVATE (USERCONTROL) COMPONENTS


#Region "private_Constants"
#Region "Editing_Constants"
    Private Const MaxFontSize As Single = 1638.35
    Private Const NoBeginning As Integer = -1, NoEnd As Integer = -1,
        NotFound As Integer = -1, NotReplacing As Integer = -1
    Private Const NoSpecificFileFormat As RichTextBoxStreamType = -1
    Private Const ShowErrors As Boolean = True, DontShowErrors As Boolean = False
    Private Const SelectionMargin As Single = 8.0F
    Private Const LeftButtonPressed As Integer = 1, RightButtonPressed As Integer = 2,
        MiddleButtonPressed As Integer = 16
    Private Const AnyButtonsPressed As Integer =
        LeftButtonPressed Or RightButtonPressed Or MiddleButtonPressed
    Private Const CustomLinkSyntax As String =
        "(?n)\{(?<Text>[^\{\|\}\r\n\s]+(\ +[^\{\|\}\r\n\s]+)*)" _
            & "\|(?<Hyperlink>[^\{\|\}\r\n\s]+(\ +[^\{\|\}\r\n\s]+)*)\}"
#End Region
#Region "Win32_Constants"
    Private Const WM_USER As Int32 = &H400&
    Private Const EM_GETCHARFORMAT As Integer = WM_USER + 58,
        EM_SETCHARFORMAT As Integer = WM_USER + 68
    Private Const SCF_SELECTION As Integer = &H1
    Private Const CFE_LINK As UInt32 = &H20, CFM_LINK As UInt32 = &H20
#End Region
#End Region

#Region "Private_Variables"
#Region "standard_Editing_Variables"
    'which mouse buttons are pressed?
    Private MouseButtonsPressed As MouseButtons = MouseButtons.None
    'syllable hyphen
    Private HyphenChar As String = ChrW(173)
    'ruler being read? then don't update it
    Private UpdatingIndentsOrTabs As Boolean = False
    'avoids event cascades during font-setting operations
    Private SettingFont As Boolean
    'avoids event cascades during changes of link mode
    Private SettingLinkMode As Boolean
    'handling whole text or just selection?
    Private DoingAll As Boolean
    'what kind of search/replace are we to do?
    Private SearchInfo As SearchCriteria
    'range for edit
    Private StartPos As Integer = NoBeginning, EndPos As Integer = NoEnd
    'text-line width span
    Private MaxTextWidth As Integer = 0, SelectionOffset As Integer = 0
    'restoring selection after edit?
    Private MaintainingSelection As Boolean = False
    'can we redraw?
    Private AreRedrawing As Boolean = True
    'protecting links?
    Private AreProtectingLinks As Boolean = True
#End Region
#Region "Custom-link_Variables"
    'any custom link mouse is over
    Private CurrentCustomLink As CustomLinkInfo = Nothing
    'most recent hyperlink
    Private PreviousHyperlink As String = ""
    'avoids event cascades during custom-link operations
    Private UpdatingCustomLinks As Boolean = False
    Private IsAutoDragDropInProgress As Boolean = False
    'list of links in document
    Private CustomLinks As SortedList(Of Integer, CustomLinkInfo) = New SortedList(Of Integer, CustomLinkInfo)()
    'custom-link tooltip (displays hyperlink)
    Private LinkTooltip As ToolTip = New ToolTip()
    '      change-tracking variable:
    '         The following variable allows one to suppress firing of ChangesMade event
    '         when temporary, reversing changes are made, or to prevent multiple firings
    '         of it when a series of edits are basically one compound change; always
    '         set it to False when changes are meant to be ignored, then set it to True
    '         when changes are meant to be counted again.
    'are current changes being tracked?
    Private TrackingChanges As Boolean = True
    '      protection-watching variable:
    '         The following variable is used to see if an edit operation succeeded,
    '         or failed due to text-protection; always set it to True right before
    '         performing an individual edit inside this code, and check it immediately
    '         after before doing any success-dependent action.
    'trying to change possibly protected text
    Private IsNotProtected As Boolean = True
#End Region
#End Region

#Region "PROPERTY_VARIABLES"
    Private SettingColorWithFont As Boolean = True
    Private ShowingToolStrip As Boolean = True
    'Private ShowingRuler As Boolean = True
    Private AllowingTabs As Boolean = True
    Private AllowingSpellCheck As Boolean = True
    Private AllowingLists As Boolean = True
    Private AllowingPictures As Boolean = False
    Private AllowingSymbols As Boolean = True
    Private AllowingHyphenation As Boolean = True
    Private AllowingDefaultSmartText As Boolean = True
    Private AllowingDefaultInsertText As Boolean = True
    Private DoingCustomLinks As Boolean = False
    Private KeepingHypertextOnRemove As Boolean = False
    'have recent (tracked) changes been made?
    Private HaveChangedText As Boolean = False
    Private PathForAnyFiles As String = ""

#Region "User Customization"
    Private AllowingFontColor As Boolean = True
    Private AllowingBackColor As Boolean = True
    Private AllowingFont As Boolean = True
    Private AllowingSearch As Boolean = True
    Private AllowingFontSize As Boolean = True
    Private AllowingAlignment As Boolean = True
#End Region

#End Region

#Region "WIN32 STUFF FOR CUSTOM LINKS"
    <StructLayout(LayoutKind.Sequential)>
    Private Structure CHARFORMAT2_STRUCT
        Public cbSize As UInt32
        Public dwMask As UInt32
        Public dwEffects As UInt32
        Public yHeight As Int32
        Public yOffset As Int32
        Public crTextColor As Int32
        Public bCharSet As Byte
        Public bPitchAndFamily As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)>
        Public szFaceName() As Char
        Public wWeight As UInt16
        Public sSpacing As UInt16
        Public crBackColor As Integer ' Color.ToArgb() -> int
        Public lcid As Integer
        Public dwReserved As Integer
        Public sStyle As Int16
        Public wKerning As Int16
        Public bUnderlineType As Byte
        Public bAnimation As Byte
        Public bRevAuthor As Byte
        Public bReserved1 As Byte
    End Structure

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function SendMessage(ByVal hWnd As IntPtr,
        ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

#End Region

#Region "PRIVATE USERCONTROL EVENT PROCEDURES"

    '      Load control
    Private Sub RichTextBoxEx_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
      Handles Me.Load
        '   set focus
        RichTextBoxEx_Enter(sender, e)
    End Sub

    '      Handle showing control
    Private Sub RichTextBoxEx_VisibleChanged(sender As Object, e As EventArgs) _
        Handles Me.VisibleChanged
        If Me.Visible Then
            RichTextBoxEx_Enter(sender, e)
        End If
    End Sub

    '      Handles resizing
    Private Sub RichTextBoxEx_Resize(sender As Object, e As System.EventArgs) _
        Handles Me.Resize
        '   size ToolStrip, TextRuler, and RichTextBox to fill UserControl's space
        Dim YPos As Integer = 0, RTBHeight As Integer = Me.Height
        ''GetHorizontalInfo()
        If ShowingToolStrip Then
            '   toolstrip
            With ToolStrip1
                .SetBounds(0, 0, Me.Width, .Height) : YPos += .Height : RTBHeight -= .Height
            End With
        End If
        'If ShowingRuler Then
        '    '   ruler
        '    With TextRuler1
        '        .SetBounds(SelectionOffset, YPos, Me.Width - SelectionOffset, .Height)
        '        'GetHorizontalInfo()
        '        YPos += .Height : RTBHeight -= .Height
        '    End With
        'End If
        '   rich-text box
        rtb.SetBounds(0, YPos, Me.Width, RTBHeight)
    End Sub

    '      Handles focus
    Private Sub RichTextBoxEx_Enter(sender As Object, e As System.EventArgs) _
        Handles Me.Enter
        '   initialize toolbar and give text box the focus
        rtb.Focus()
        'Me.AutoValidate = Windows.Forms.AutoValidate.EnablePreventFocusChange
        ''SetProtectionForCustomLinks(True)
        'UpdateToolbarNoChangesTracked()
    End Sub

#End Region

#Region "TOOLSTRIP EVENT PROCEDURES"

    '   Handle a change to visibility
    Private Sub ToolStrip1_VisibleChanged(sender As Object,
        e As System.EventArgs) Handles ToolStrip1.VisibleChanged
        '   adjust for whether ToolStrip is visible now
        RichTextBoxEx_Resize(sender, e)
    End Sub

#End Region

#Region "RICHTEXTBOX EVENT PROCEDURES"

    '      Handle special characters
    'Private Sub rtb_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) _
    '    Handles rtb.KeyDown
    '    '   make sure any custom-links are protected when a non-modifier key is pressed
    '    Select Case e.KeyCode
    '        Case Keys.None, Keys.ControlKey, Keys.LControlKey, Keys.RControlKey,
    '                  Keys.ShiftKey, Keys.LShiftKey, Keys.RShiftKey
    '            e.SuppressKeyPress = True : Exit Sub 'no key or modifier key only
    '    End Select
    '    If MouseButtonsPressed <> MouseButtons.None Then
    '        e.SuppressKeyPress = True : Exit Sub 'no key if mouse button pressed
    '    End If
    '    'SetProtectionForCustomLinks(rtb.SelectionLength = 0)
    '    '   raise event to see if user wants to make a custom insertion
    '    Dim CustomInsert As InsertRtfTextEventArgs = New InsertRtfTextEventArgs(e)
    '    Me.OnInsertRtfText(CustomInsert)
    '    e.SuppressKeyPress = CustomInsert.KeyEventArgs.SuppressKeyPress
    '    Dim InsertionText As String = CustomInsert.RtfText
    '    If String.IsNullOrEmpty(InsertionText) _
    '            AndAlso AllowingDefaultInsertText _
    '            AndAlso Not e.SuppressKeyPress Then
    '        If e.Control Then
    '            '   check for default keyboard shortcuts
    '            Select Case e.KeyCode
    '                Case Keys.OemMinus, Keys.Subtract
    '                    '   hyphens/dashes
    '                    If e.Alt Then
    '                        '   em dash ("—")
    '                        '      -- [Ctrl] + [Alt] + [-]
    '                        InsertionText = "—"
    '                    Else
    '                        '   optional hyphen (display only when breaking line)
    '                        '      -- [Ctrl] + [-]
    '                        InsertionText = "\-"
    '                    End If
    '                Case Keys.Oemtilde
    '                    '   left quotes
    '                    If e.Shift Then
    '                        '   left double-quote
    '                        '      -- [Ctrl] + [Shift] + [~]
    '                        InsertionText = "\ldblquote"
    '                    Else
    '                        '   left single-quote
    '                        '      -- [Ctrl] + [`]
    '                        InsertionText = "\lquote"
    '                    End If
    '                Case Keys.OemQuotes
    '                    '   right quotes
    '                    If e.Shift Then
    '                        '    right double-quote
    '                        '      -- [Ctrl] + [Shift] + ["]
    '                        InsertionText = "\rdblquote"
    '                    Else
    '                        '   right single-quote
    '                        '      -- [Ctrl] + [']
    '                        InsertionText = "\rquote"
    '                    End If
    '                Case Keys.C
    '                    '   copyright ("©")
    '                    '      -- [Ctrl] + [Alt] + [C]
    '                    If e.Alt Then
    '                        InsertionText = "©"
    '                    End If
    '                Case Keys.R
    '                    '   registered trademark ("®")
    '                    '      -- [Ctrl] + [Alt] + [RC]
    '                    If e.Alt Then
    '                        InsertionText = "®"
    '                    End If
    '                Case Keys.T
    '                    '   trademark ("™")
    '                    '      -- [Ctrl] + [Alt] + [T]
    '                    If e.Alt Then
    '                        InsertionText = "™"
    '                    End If
    '                Case Keys.L
    '                    '   needed to make sure shortcut [Ctrl] + [Shift] + [L]
    '                    '   only makes lists when listing is actually enabled
    '                    If e.Shift Then
    '                        '   make sure bullets are enabled
    '                        If Not AllowingLists Then
    '                            e.SuppressKeyPress = True : Exit Sub
    '                        End If
    '                    End If
    '            End Select
    '        Else
    '            '   see if user wants to remove a link
    '            Select Case e.KeyCode
    '                Case Keys.Back, Keys.Delete
    '                    If DoingCustomLinks Then
    '                        If rtb.SelectionLength > 0 Then
    '                            '   deleting a selection
    '                            UnprotectCustomLinks() : rtb.SelectedText = ""
    '                            'ReprotectCustomLinks
    '                            e.SuppressKeyPress = True : Exit Sub
    '                        End If
    '                        '   see if we're deleting a link
    '                        Dim CurrentPos As Integer
    '                        Dim CustomLinkInfo As CustomLinkInfo = Nothing
    '                        If e.KeyCode = Keys.Back Then
    '                            '   are we backing over a link?
    '                            CurrentPos = rtb.SelectionStart + rtb.SelectionLength
    '                            If CurrentPos > 0 Then
    '                                CustomLinkInfo = GetCustomLink(-CurrentPos, True)
    '                                If CustomLinkInfo Is Nothing _
    '                                    AndAlso AreWeBeforeACustomLink(CurrentPos - 1) Then
    '                                    CustomLinkInfo = CustomLinks(CurrentPos)
    '                                End If
    '                            End If
    '                        Else
    '                            '   are we deleting over a link?
    '                            CurrentPos = rtb.SelectionStart
    '                            If CurrentPos < rtb.TextLength Then
    '                                CustomLinkInfo = GetCustomLink(CurrentPos, True)
    '                                If CustomLinkInfo Is Nothing _
    '                                        AndAlso AreWeBeforeACustomLink(CurrentPos) Then
    '                                    CustomLinkInfo = CustomLinks(CurrentPos + 1)
    '                                End If
    '                            End If
    '                        End If
    '                        '   see if we can remove any link here
    '                        If CustomLinkInfo IsNot Nothing Then
    '                            '   does user to remove it?
    '                            Beep()
    '                            Dim DefaultButton As MessageBoxDefaultButton
    '                            If KeepingHypertextOnRemove Then
    '                                DefaultButton = MessageBoxDefaultButton.Button1
    '                            Else
    '                                DefaultButton = MessageBoxDefaultButton.Button2
    '                            End If
    '                            Dim Result As DialogResult = MessageBox.Show(
    '                                "About to delete a link to " & ControlChars.CrLf _
    '                                    & """" & CustomLinkInfo.Hyperlink & """" _
    '                                    & ControlChars.CrLf & ControlChars.CrLf _
    '                                    & "Keep aforementioned hyperlink in text?",
    '                                "Link Present", MessageBoxButtons.YesNoCancel,
    '                                MessageBoxIcon.Warning, DefaultButton)
    '                            If Result <> DialogResult.Cancel Then
    '                                '   yes--remove it
    '                                KeepingHypertextOnRemove =
    '                                    Result = DialogResult.Yes
    '                                DeleteCustomLink(CustomLinkInfo.Position)
    '                                e.SuppressKeyPress = True
    '                            End If
    '                        End If
    '                    End If
    '            End Select
    '        End If
    '    End If
    '    '   insert custom text if necessary
    '    If Not String.IsNullOrEmpty(InsertionText) AndAlso Not e.SuppressKeyPress Then
    '        'SuppressRedraw() : ReplaceRTFText(InsertionText) 'make insertion
    '        'ResumeRedraw() : e.SuppressKeyPress = True 'absorb keystroke
    '    End If
    'End Sub

    '      Handle smart character conversions
    'Private Sub rtb_KeyPress(sender As Object, e As KeyPressEventArgs) _
    '    Handles rtb.KeyPress
    '    '   make sure any custom-links are protected when a non-modifier key is pressed
    '    Select Case AscW(e.KeyChar)
    '        Case Keys.ControlKey, Keys.LControlKey, Keys.RControlKey,
    '                Keys.ShiftKey, Keys.LShiftKey, Keys.RShiftKey
    '            e.Handled = True : Exit Sub 'modifier key only
    '    End Select
    '    If MouseButtonsPressed <> MouseButtons.None Then
    '        e.Handled = True : Exit Sub 'no key if mouse button pressed
    '    End If
    '    'SetProtectionForCustomLinks(rtb.SelectionLength = 0)
    '    '   raise event to see if user wants to specify any "smart text" conversions
    '    Dim Start As Integer = rtb.SelectionStart, Length As Integer = rtb.SelectionLength
    '    Dim CustomInsert As SmartRtfTextEventArgs = New SmartRtfTextEventArgs(e)
    '    Me.OnSmartRtfText(CustomInsert)
    '    Dim SmartText As String = CustomInsert.RtfText
    '    Dim PrecedingCharLength As Integer = CustomInsert.PrecedingCharacterCount
    '    If String.IsNullOrEmpty(SmartText) AndAlso AllowingDefaultSmartText Then
    '        '   get current and preceeding char
    '        Dim CurrentChar As String = e.KeyChar.ToString, PreviousChar As String = ""
    '        Dim PositionInLine As Integer = GetPositionInLine(Start)
    '        If PositionInLine > 0 Then
    '            '   not at beginning of line--get previous char
    '            PreviousChar = rtb.Text.Substring(Start - 1, 1)(0)
    '        End If
    '        '   check incoming char		
    '        Select Case CurrentChar
    '            Case "-"
    '                '   dash--doubled for em dash?
    '                If PreviousChar = "-" Then
    '                    SmartText = "—" : PrecedingCharLength = 1 'em dash
    '                End If
    '            Case """"
    '                '   double-quote char
    '                Select Case PreviousChar
    '                    Case "", " ", "—", "-", ControlChars.Tab, ChrW(8216)
    '                        SmartText = "\ldblquote" 'use left double-quote
    '                    Case Else
    '                        SmartText = "\rdblquote" 'use right double-quote
    '                End Select
    '            Case "'"
    '                '   single-quote char
    '                Select Case PreviousChar
    '                    Case "", " ", "—", "-", ControlChars.Tab, ChrW(8220)
    '                        SmartText = "\lquote" 'use left single-quote
    '                    Case Else
    '                        SmartText = "\rquote" 'use right single-quote
    '                End Select
    '        End Select
    '    End If
    '    '   replace with smart text if necessary
    '    If Not String.IsNullOrEmpty(SmartText) Then
    '        'SuppressRedraw()
    '        rtb.Select(Start - PrecedingCharLength,
    '            Length + PrecedingCharLength) 'remove preceding chars?
    '        ReplaceRTFText(SmartText) 'make replacement
    '        'ResumeRedraw() : e.Handled = True 'suppress keystroke
    '    End If
    'End Sub

    'Private Sub rtb_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) _
    '    Handles rtb.KeyUp
    '    '   make sure are custom links are protected when key is released
    '    'SetProtectionForCustomLinks(MouseButtonsPressed = MouseButtons.None)
    'End Sub

    '   Handle attempts to modify portected text and/or custom links
    'Private Sub rtb_Protected(sender As Object, e As EventArgs) Handles rtb.[Protected]
    '    '   can't protect what isn't there
    '    If rtb.TextLength = 0 Then
    '        rtb.SelectionProtected = False : Exit Sub
    '    End If
    '    '   flag that edit failed
    '    IsNotProtected = False
    '    '   give developer-user ability for custom action
    '    Dim Canceling As CancelEventArgs =
    '        New CancelEventArgs() With {.Cancel = False}
    '    Me.OnTextProtected(Canceling)
    '    If Canceling.Cancel Then
    '        '   developer wants to preempt automatic warning
    '        Exit Sub
    '    End If
    '    '   warn end-user--are custom links potentially involved?
    '    Dim s As String = ""
    '    If DoingCustomLinks Then
    '        s = " and/or active link(s)"
    '    End If
    '    Beep()
    '    MessageBox.Show(
    '        "Region of text contains protected text" & s & "!" _
    '            & ControlChars.CrLf & ControlChars.CrLf _
    '            & "To perform this edit, text protection(s)" & s & ControlChars.CrLf _
    '            & " must be removed from affected text first.",
    '        "Invalid Edit", MessageBoxButtons.OK, MessageBoxIcon.Error)
    'End Sub

    '   Handle mouse and drag-and-drop situations
    'Private Sub rtb_MouseUp(sender As Object, e As MouseEventArgs) Handles rtb.MouseUp
    '    '   enable custom-link protection when mouse button is up
    '    MouseButtonsPressed = e.Button '': 'ReprotectCustomLinks
    'End Sub

    'Private Sub rtb_MouseDown(sender As Object, e As MouseEventArgs) Handles rtb.MouseDown
    '    '   disable custom-link protection when mouse button is down
    '    MouseButtonsPressed = e.Button ': UnprotectCustomLinks()
    'End Sub

    'Private Sub rtb_MouseMove(sender As Object, e As MouseEventArgs) Handles rtb.MouseMove
    '    Dim Hyperlink As String = ""
    '    MouseButtonsPressed = e.Button
    '    If DoingCustomLinks Then
    '        '   handle custom-link protection in case of drag-and-drop
    '        If AreProtectingLinks Xor MouseButtonsPressed = MouseButtons.None Then
    '            '   toggle custom-link protection
    '            ''SetProtectionForCustomLinks(Not AreProtectingLinks)
    '        End If
    '        '   tooltip logic
    '        Dim CurrentPlace As Point = New Point(e.X, e.Y)
    '        Dim CurrentCharacter As Integer = rtb.GetCharIndexFromPosition(CurrentPlace)
    '        If CurrentCharacter >= 0 _
    '                AndAlso CurrentCharacter < rtb.TextLength Then
    '            '   get current link, if any
    '            CurrentCustomLink = GetCustomLink(CurrentCharacter)
    '            If CurrentCustomLink IsNot Nothing Then
    '                '   make hyperlink text box's tooltip
    '                Hyperlink = CurrentCustomLink.Hyperlink
    '            End If
    '        Else
    '            '   not over anything
    '            CurrentCustomLink = Nothing
    '        End If
    '    End If
    '    '   display any hyperlink
    '    If Hyperlink <> PreviousHyperlink Then
    '        LinkTooltip.SetToolTip(rtb, Hyperlink) : PreviousHyperlink = Hyperlink
    '    End If
    'End Sub

    '   Drag-and-drop
    'Private Sub rtb_DragDrop(sender As Object, e As DragEventArgs) _
    '    Handles rtb.DragDrop
    '    '   raise event for whole class
    '    Try
    '        IsAutoDragDropInProgress = True 'set auto-drop flag
    '        Me.OnDragDrop(e) 'protect any custom links and raise class-level event
    '    Catch ex As Exception
    '        '   cancel drop on error
    '        e.Effect = DragDropEffects.None : Throw
    '    Finally
    '        '   make sure auto-drop flag is reset even if error occurs
    '        IsAutoDragDropInProgress = False
    '    End Try
    '    '   see if we can do default drop
    '    If e.Effect = DragDropEffects.None Then
    '        Exit Sub 'pre-empt default drop
    '    End If
    '    'If AreCustomLinksInSelection() Then
    '    '    '   don't do drop if custom links are affteded
    '    '    e.Effect = DragDropEffects.None
    '    '    Beep()
    '    '    MessageBox.Show(
    '    '        "Region of text contains link(s)!" _
    '    '            & ControlChars.CrLf & ControlChars.CrLf _
    '    '            & "To drop, links(s) must be removed from affected text first.",
    '    '        "Invalid Edit", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    'End If
    'End Sub

    'Private Sub rtb_DragOver(sender As Object, e As DragEventArgs) _
    '    Handles rtb.DragOver
    '    '   make sure custom-link protection is disabled when mouse buttons are pressed
    '    '   and preparations are made in case drop occurs over text containing links
    '    If (e.KeyState And AnyButtonsPressed) = 0 Xor AreProtectingLinks Then
    '        ''SetProtectionForCustomLinks(Not AreProtectingLinks)
    '    End If
    '    Me.OnDragOver(e) 'raise event for whole class
    'End Sub

    'Private Sub rtb_DragEnter(sender As Object, e As DragEventArgs) _
    '    Handles rtb.DragEnter
    '    '   make sure custom-link protection is disabled when mouse buttons are pressed
    '    '   and preparations are made in case drop occurs over text containing links
    '    If (e.KeyState And AnyButtonsPressed) = 0 Xor AreProtectingLinks Then
    '        'SetProtectionForCustomLinks(Not AreProtectingLinks)
    '    End If
    '    Me.OnDragEnter(e) 'raise event for whole class
    'End Sub

    'Private Sub rtb_DragLeave(sender As Object, e As DragEventArgs) _
    '    Handles rtb.DragLeave
    '    '   make sure custom-link protection is disabled when mouse buttons are pressed
    '    '   and preparations are made in case drop occurs over text containing links
    '    If (e.KeyState And AnyButtonsPressed) = 0 Xor AreProtectingLinks Then
    '        'SetProtectionForCustomLinks(Not AreProtectingLinks)
    '    End If
    '    Me.OnDragLeave(e) 'raise event for whole class
    'End Sub

    '   Handle the clicking of links
    'Private Sub rtb_LinkClicked(sender As Object, e As LinkClickedEventArgs) _
    '    Handles rtb.LinkClicked
    '    '   handle clicked links
    '    Dim CustomLinkInfoEA As CustomLinkInfoEventArgs = New CustomLinkInfoEventArgs()
    '    With CustomLinkInfoEA
    '        '   get link information
    '        If DoingCustomLinks Then
    '            '   custom link--get one under mouse (there could be multiple adjacent ones)
    '            .CustomLinkInfo = CurrentCustomLink
    '        Else
    '            '   standard (non-custom) link--rely on link text
    '            .CustomLinkInfo =
    '                New CustomLinkInfo With {.Position = -1, .Text = "", .Hyperlink = ""}
    '        End If
    '        '   raise event
    '        If .CustomLinkInfo IsNot Nothing Then
    '            Me.OnHyperlinkClicked(CustomLinkInfoEA)
    '        End If
    '    End With
    'End Sub

    '   Update buttons when text is selected
    'Private Sub rtb_SelectionChanged(ByVal sender As Object,
    '    ByVal e As System.EventArgs) Handles rtb.SelectionChanged
    '    '   are turning custom links on or off?
    '    If SettingLinkMode Then
    '        Exit Sub
    '    End If
    '    '   see if we need to normalize selection
    '    If DoingCustomLinks AndAlso Not UpdatingCustomLinks Then
    '        rtb.DetectUrls = False : NormalizeSelectionForCustomLinks()
    '    End If
    '    '   handle ruler
    '    'GetHorizontalInfo()
    '    '   see which toolbar items should be updated
    '    Dim CurrentFont As Font = rtb.SelectionFont
    '    '   update font-attribute items
    '    If CurrentFont IsNot Nothing Then
    '        With CurrentFont
    '            '   update font name and size
    '            If Not SettingFont Then
    '                SettingFont = True 'prevent font from being changed as it's reported
    '                If FontNameToolStripComboBox.Text <> .Name Then
    '                    FontNameToolStripComboBox.Text = .Name
    '                End If
    '                If FontSizeToolStripComboBox.Text <> CStr(.SizeInPoints) Then
    '                    FontSizeToolStripComboBox.Text = CStr(.SizeInPoints)
    '                End If
    '                SettingFont = False
    '            End If
    '            '   update font styles
    '            BoldToolStripButton.Checked = .Bold
    '            BoldToolStripMenuItem.Checked = .Bold
    '            ItalicToolStripButton.Checked = .Italic
    '            ItalicToolStripMenuItem.Checked = .Italic
    '            UnderlineToolStripButton.Checked = .Underline
    '            UnderlineToolStripMenuItem.Checked = .Underline
    '            StrikethroughToolStripMenuItem.Checked = .Strikeout
    '        End With
    '    End If
    '    '   update superscript/subscript status items
    '    HandleSuperOrSubScript(rtb.SelectionCharOffset)
    '    '   update alignment-attribute items
    '    LeftToolStripButton.Checked =
    '        (rtb.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Left)
    '    CenterToolStripButton.Checked =
    '        (rtb.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Center)
    '    RightToolStripButton.Checked =
    '        (rtb.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Right)
    '    LeftToolStripMenuItem.Checked = LeftToolStripButton.Checked
    '    CenterToolStripMenuItem.Checked = CenterToolStripButton.Checked
    '    RightToolStripMenuItem.Checked = RightToolStripButton.Checked
    '    HyperlinkToolstripButton.Checked =
    '        DoingCustomLinks AndAlso AreCustomLinksInSelection()
    '    '   allow insertion between adjacent links
    '    If DoingCustomLinks AndAlso rtb.SelectionLength = 0 _
    '            AndAlso AreWeBeforeACustomLink(rtb.SelectionStart) Then
    '        rtb.SelectionProtected = False
    '    End If
    'End Sub

    '   Enable/Disable options based on text changes
    'Private Sub rtb_TextChanged(sender As Object, e As System.EventArgs) _
    '    Handles rtb.TextChanged
    '    '   see if we need to alter custom-link info
    '    If DoingCustomLinks AndAlso Not UpdatingCustomLinks Then
    '        rtb.DetectUrls = False : GetAllCustomLinks()
    '    End If
    '    '   Spell-Check, Find, and Hyphenate require text to be present
    '    '   set context-menu/toolbar options
    '    Dim IsTextPresent As Boolean = (rtb.TextLength > 0)
    '    Dim CanSpellCheck As Boolean = (IsTextPresent AndAlso AllowingSpellCheck)
    '    SpellcheckToolStripButton.Enabled = CanSpellCheck
    '    SpellCheckToolStripMenuItem.Enabled = CanSpellCheck
    '    SearchToolStripMenuItem.Enabled = IsTextPresent
    '    FindToolStripButton.Enabled = IsTextPresent
    '    FindToolStripMenuItem.Enabled = IsTextPresent
    '    ReplaceToolStripMenuItem.Enabled = IsTextPresent
    '    HyphenateToolStripButton.Enabled = IsTextPresent
    '    HyphenateToolStripMenuItem.Enabled = IsTextPresent
    '    RemoveAllHyphensToolStripMenuItem1.Enabled = IsTextPresent
    '    RemoveHiddenHyphensOnlyToolStripMenuItem1.Enabled = IsTextPresent
    '    '   Find Next also requires a search string to have been given
    '    Dim IsSearchTextPresent As Boolean =
    '        (IsTextPresent AndAlso SearchInfo.SearchText.Length > 0)
    '    FindNextToolStripButton.Enabled = IsSearchTextPresent
    '    FindNextToolStripMenuItem.Enabled = IsSearchTextPresent
    '    '   refresh toolbar display
    '    rtb_SelectionChanged(sender, e)
    '    '   raise ChangesMade event if a tracked change has occurred
    '    HandleTrackedChange()
    'End Sub

    '   Account for horizontal scrolling
    'Private Sub rtb_HScroll(sender As Object, e As EventArgs) _
    '    Handles rtb.HScroll
    '    If Not UpdatingIndentsOrTabs Then
    '        '   check against text being over-scrolled
    '        Dim ScrollPosition As Point = rtb.GetScrollPosition()
    '        With ScrollPosition
    '            If .X > MaxTextWidth - rtb.ClientRectangle.Width + SelectionOffset Then
    '                .X = MaxTextWidth - rtb.ClientRectangle.Width + SelectionOffset
    '                rtb.SetScrollPosition(ScrollPosition)
    '            End If
    '            TextRuler1.ScrollingOffset = .X
    '        End With
    '    End If
    'End Sub

#End Region

#Region "RULER EVENT PROCEDURES"

    '   Handle a change to visiblibity
    'Private Sub TextRuler1_VisibleChanged(sender As Object,
    '    e As System.EventArgs)
    '    '   adjust for whether TextRuler is visible now
    '    RichTextBoxEx_Resize(sender, e)
    'End Sub

    '   Handle new tabs
    'Private Sub TextRuler1_TabAdded(sender As Object, e As TabEventArgs)

    '    UpdatingIndentsOrTabs = True 'suspend updating rule
    '    UnprotectCustomLinks()
    '    rtb.SelectionTabs = TextRuler1.TabPositions 'set new tabs
    '    'ReprotectCustomLinks
    '    UpdatingIndentsOrTabs = False 'resume updating ruler
    '    'GetHorizontalInfo() : rtb.Focus()
    'End Sub

    '   Handle new indents
    'Private Sub TextRuler1_IndentsChanged(sender As Object, e As MarginOrIndentEventArgs)

    '    With TextRuler1
    '        '   defer updating ruler
    '        UpdatingIndentsOrTabs = True
    '        'TurnRedrawModeOnOrOff(False) : UnprotectCustomLinks()
    '        '   set new indents
    '        Select Case e.MarkerType
    '            Case TextRuler.MarkerType.First_Line_Indent
    '                '   moving only first-line indent
    '                rtb.SelectionIndent = .FirstLineIndent
    '                rtb.SelectionHangingIndent = .HangingIndent
    '            Case TextRuler.MarkerType.Hanging_Indent
    '                '   moving only hanging indent
    '                rtb.SelectionHangingIndent = .HangingIndent
    '            Case TextRuler.MarkerType.Left_Indents
    '                '   moving both left indents
    '                rtb.SelectionIndent = .LeftIndent
    '            Case TextRuler.MarkerType.Right_Indent
    '                '   moving right indent
    '                rtb.SelectionRightIndent = .RightIndent
    '        End Select
    '        '   update ruler now
    '        'TurnRedrawModeOnOrOff(AreRedrawing) ': 'ReprotectCustomLinks
    '        UpdatingIndentsOrTabs = False  'GetHorizontalInfo() : rtb.Focus()
    '    End With
    'End Sub

#End Region

#Region "SPELLCHECKER EVENT PROCEDURES"

    '      Spell checking thanks to: http://www.codeproject.com/KB/string/netspell.aspx

    '      Handles when user chooses to delete in spell cehck
    Private Sub SpellChecker_DeletedWord(ByVal sender As Object,
        ByVal e As NetSpell.SpellChecker.SpellingEventArgs) _
            Handles SpellChecker.DeletedWord
        'SuppressRedraw()
        '   select word for this event
        rtb.Select(StartPos + e.TextIndex, e.Word.Length)
        'If DoingCustomLinks AndAlso AreCustomLinksInSelection() Then
        '    '   make sure we avoid custom links
        '    Beep()
        '    MessageBox.Show("Text in active hyperlink(s) cannot be deleted.",
        '        "Link Present", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        'Else
        '   delete word
        rtb.SelectedText = ""
        If IsNotProtected Then
            '   succeeded
            If EndPos > StartPos Then
                EndPos -= e.Word.Length
            End If
            If EndPos > rtb.TextLength Then
                EndPos = rtb.TextLength
            End If
        End If
        'End If
        IsNotProtected = True ': 'ResumeRedraw()
    End Sub

    '      Handles reaching the end of text
    Private Sub SpellChecker_EndOfText(sender As Object, e As System.EventArgs) _
        Handles SpellChecker.EndOfText
        DoneWithRange()
    End Sub

    '      Handles replacing a word from spell checking
    Private Sub SpellChecker_ReplacedWord(ByVal sender As Object,
        ByVal e As NetSpell.SpellChecker.ReplaceWordEventArgs) _
            Handles SpellChecker.ReplacedWord
        'SuppressRedraw()
        '   select word for this event
        rtb.Select(StartPos + e.TextIndex, e.Word.Length)
        'If DoingCustomLinks AndAlso AreCustomLinksInSelection() Then
        '    '   make sure we avoid custom links
        '    Beep()
        '    MessageBox.Show("Text in active hyperlink(s) cannot be replaced.",
        '        "Link Present", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        'Else
        '   replace word
        rtb.SelectedText = e.ReplacementWord
        If IsNotProtected Then
            '   succeeded
            If EndPos > StartPos Then
                EndPos += e.ReplacementWord.Length - e.Word.Length
            End If
            If EndPos > rtb.TextLength Then
                EndPos = rtb.TextLength
            End If
        End If
        'End If
        IsNotProtected = True ': 'ResumeRedraw()
    End Sub

#End Region

#Region "EDIT SHORTCUT MENU EVENT PROCEDURES"
    '      Copy
    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As System.EventArgs) _
        Handles CopyToolStripMenuItem.Click
        rtb.Copy()
    End Sub

    '      Select All
    Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As System.EventArgs) _
        Handles SelectAllToolStripMenuItem.Click
        rtb.SelectAll()
    End Sub

    '      Cut
    Private Sub CutToolStripMenuItem_Click(sender As Object, e As System.EventArgs) _
        Handles CutToolStripMenuItem.Click
        'UnprotectCustomLinks()
        rtb.Cut()
        'IsNotProtected = True ': 'ReprotectCustomLinks
    End Sub

    '      Paste
    Private Sub PasteToolStripMenuItem_Click(sender As Object, e As System.EventArgs) _
        Handles PasteToolStripMenuItem.Click
        'SetProtectionForCustomLinks(rtb.SelectionLength = 0)
        rtb.Paste() : IsNotProtected = True ': 'ReprotectCustomLinks
    End Sub

    '      Undo
    Private Sub UndoToolStripMenuItem_Click(sender As Object, e As System.EventArgs) _
        Handles UndoToolStripMenuItem.Click
        'UnprotectCustomLinks()
        rtb.Undo()
        'IsNotProtected = True ': 'ReprotectCustomLinks
    End Sub

    '      Redo
    Private Sub RedoToolStripMenuItem_Click(sender As Object, e As System.EventArgs) _
        Handles RedoToolStripMenuItem.Click
        'UnprotectCustomLinks()
        rtb.Redo()
        'IsNotProtected = True ': 'ReprotectCustomLinks
    End Sub

#End Region

#Region "FONT-SELECTION EVENT PROCEDURES"

    '      Show Font dialog
    Private Sub FontToolStripButton_Click(ByVal sender As System.Object,
        ByVal e As System.EventArgs) Handles FontToolStripButton.Click,
            GeneralFontSettingsToolStripMenuItem.Click
        FontDlg.Font = GetCurrentFont()
        FontDlg.ShowColor = SettingColorWithFont
        If SettingColorWithFont Then
            '   setting color with font
            FontDlg.Color = GetCurrentColor()
        End If
        If FontDlg.ShowDialog() <> Windows.Forms.DialogResult.Cancel Then
            '   new font (and maybe color)
            'UnprotectCustomLinks()
            Try
                rtb.SelectionFont = FontDlg.Font
                If SettingColorWithFont Then
                    '   setting color with font
                    rtb.SelectionColor = FontDlg.Color
                End If
            Catch ex As Exception
                Beep()
            End Try
            ''ReprotectCustomLinks
            'UpdateToolbarNoChangesTracked()
        End If
    End Sub

    '      Show Foreground Color dialog
    Private Sub FontColorToolStripButton_Click(ByVal sender As System.Object,
        ByVal e As System.EventArgs) Handles FontColorToolStripButton.Click
        '   new color
        ColorDlg.Color = GetCurrentColor()
        If ColorDlg.ShowDialog() <> Windows.Forms.DialogResult.Cancel Then
            'UnprotectCustomLinks() : 
            rtb.SelectionColor = ColorDlg.Color
            ''ReprotectCustomLinks : 
            'UpdateToolbarNoChangesTracked()
        End If
    End Sub

    '      Show Background Color dialog
    Private Sub BackColorToolStripButton_Click(ByVal sender As System.Object,
        ByVal e As System.EventArgs) Handles BackColorToolStripButton.Click
        '   new color
        ColorDlg.Color = GetCurrentBackgroundColor()
        If ColorDlg.ShowDialog() <> Windows.Forms.DialogResult.Cancel Then
            'UnprotectCustomLinks() : 
            rtb.SelectionBackColor = ColorDlg.Color
            ''ReprotectCustomLinks : 
            'UpdateToolbarNoChangesTracked()
        End If
    End Sub

    '      Populate list of font names
    Private Sub FontNameToolStripComboBox_DropDown(sender As Object,
        e As System.EventArgs) Handles FontNameToolStripComboBox.DropDown
        With FontNameToolStripComboBox.Items
            '    set list of names
            .Clear()
            For Each FontFamily As FontFamily In FontFamily.Families
                .Add(FontFamily.Name)
            Next FontFamily
        End With
    End Sub

    '      Handles edit-box caret when an item is selected from the drop-down box
    Private Sub FontNameToolStripComboBox_SelectedIndexChanged(sender As Object,
        e As System.EventArgs) Handles FontNameToolStripComboBox.SelectedIndexChanged
        If Not SettingFont Then
            SettingFont = True 'prevent recursive event cascade
            Dim Result As Boolean = AreSettingValidFontName(DontShowErrors)
            SettingFont = False
        End If
        FontNameToolStripComboBox.Select(FontNameToolStripComboBox.Text.Length, 0)
    End Sub

    '      New font name is being entered--show it in text box
    Private Sub FontNameToolStripComboBox_TextChanged(sender As Object,
        e As System.EventArgs) Handles FontNameToolStripComboBox.TextChanged
        If Not SettingFont Then
            With FontNameToolStripComboBox
                '   validate font name
                SettingFont = True 'prevent recursive event cascade
                Dim Result As Boolean = AreSettingValidFontName(DontShowErrors)
                .Select(.Text.Length, 0) : SettingFont = False
            End With
        End If
    End Sub

    '      Validate new font name
    Private Sub FontNameToolStripComboBox_Validating(sender As Object,
        e As System.ComponentModel.CancelEventArgs) _
            Handles FontNameToolStripComboBox.Validating
        '   set font name
        If AreSettingValidFontName(ShowErrors) Then
            '   send focus to text box if OK
            ResetFocus()
        Else
            '   restore focus back if error
            e.Cancel = True
        End If
    End Sub

    '      Set new font size
    Private Sub FontSizeToolStripComboBox_Leave(sender As Object,
        e As System.EventArgs) Handles FontSizeToolStripComboBox.Leave
        '   set font size
        If AreSettingValidFontSize(ShowErrors) Then
            '   send focus to text box if OK
            ResetFocus()
        Else
            '   restore focus back if error
            FontSizeToolStripComboBox.Focus()
        End If
    End Sub

    '      Handles edit-box caret when an item is selected from the drop-down box
    Private Sub FontSizeToolStripComboBox_SelectedIndexChanged(sender As Object,
        e As System.EventArgs) Handles FontSizeToolStripComboBox.SelectedIndexChanged
        If Not SettingFont Then
            SettingFont = True 'prevent recursive event cascade
            Dim Result As Boolean = AreSettingValidFontSize(DontShowErrors)
            SettingFont = False
        End If
        FontSizeToolStripComboBox.Select(FontSizeToolStripComboBox.Text.Length, 0)
    End Sub

    '      New font size is being entered--show it in text box
    Private Sub FontSizeToolStripComboBox_TextChanged(sender As Object,
        e As System.EventArgs) Handles FontSizeToolStripComboBox.TextChanged
        If Not SettingFont Then
            With FontSizeToolStripComboBox
                SettingFont = True 'prevent recursive event cascade
                '   validate font size
                Dim Result As Boolean = AreSettingValidFontSize(DontShowErrors)
                .Select(.Text.Length, 0) : SettingFont = False
            End With
        End If
    End Sub

    '      Validate new font size
    Private Sub FontSizeToolStripComboBox_Validating(sender As Object,
        e As System.ComponentModel.CancelEventArgs) Handles FontSizeToolStripComboBox.Validating
        '   set font size
        If AreSettingValidFontSize(ShowErrors) Then
            '   send focus to text box if OK
            rtb.Focus()
        Else
            '   restore focus back if error
            e.Cancel = True
        End If
    End Sub

    '      Switch Bold
    Private Sub BoldToolStripButton_Click(ByVal sender As System.Object,
        ByVal e As System.EventArgs) Handles BoldToolStripButton.Click,
            BoldToolStripMenuItem.Click
        ToggleFontStyle(Drawing.FontStyle.Bold) : ResetFocus()
    End Sub

    '      Switch Italic
    Private Sub ItalicToolStripButton_Click(ByVal sender As System.Object,
        ByVal e As System.EventArgs) Handles ItalicToolStripButton.Click,
            ItalicToolStripMenuItem.Click
        ToggleFontStyle(Drawing.FontStyle.Italic) : ResetFocus()
    End Sub

    '      Switch Underline
    Private Sub UnderlineToolStripButton_Click(ByVal sender As System.Object,
        ByVal e As System.EventArgs) Handles UnderlineToolStripButton.Click,
            UnderlineToolStripMenuItem.Click
        ToggleFontStyle(Drawing.FontStyle.Underline) : ResetFocus()
    End Sub

    '   Switch Strikethrough
    Private Sub StrikethroughToolStripMenuItem_Click(sender As Object, e As System.EventArgs) _
        Handles StrikethroughToolStripMenuItem.Click
        ToggleFontStyle(Drawing.FontStyle.Strikeout) : ResetFocus()
    End Sub

    '   TEXT-ALIGNMENT EVENT PROCEDURES:

    '      Align text left
    Private Sub LeftToolStripButton_Click(ByVal sender As System.Object,
        ByVal e As System.EventArgs) Handles LeftToolStripButton.Click,
            LeftToolStripMenuItem.Click
        'UnprotectCustomLinks() : 
        rtb.SelectionAlignment = HorizontalAlignment.Left
        ''ReprotectCustomLinks : 
        ResetFocus()
    End Sub

    '      Align text center
    Private Sub CenterToolStripButton_Click(ByVal sender As System.Object,
        ByVal e As System.EventArgs) Handles CenterToolStripButton.Click,
            CenterToolStripMenuItem.Click
        'UnprotectCustomLinks() : 
        rtb.SelectionAlignment = HorizontalAlignment.Center
        ''ReprotectCustomLinks : 
        ResetFocus()
    End Sub

    '      Align text right
    Private Sub RightToolStripButton_Click(ByVal sender As System.Object,
        ByVal e As System.EventArgs) Handles RightToolStripButton.Click,
            RightToolStripMenuItem.Click
        'UnprotectCustomLinks()
        rtb.SelectionAlignment = HorizontalAlignment.Right
        ''ReprotectCustomLinks
        ResetFocus()
    End Sub

    '   LIST, SPELL-CHECK, PICTURE, AND SYMBOL EVENT PROCEDURES:

    '   Handles lists
    Private Sub NoneToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles NoneToolStripMenuItem.Click, BulletToolStripMenuItem.Click,
            NumToolStripMenuItem.Click, LCAlphaToolStripMenuItem.Click,
            UCAlphaToolStripMenuItem.Click, LCRomanToolStripMenuItem.Click,
            UCRomanToolStripMenuItem.Click
        '   set list style for selected text
        Dim ctl As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim ListStyle As RTBListStyle = CType(ctl.Tag, RTBListStyle)
        'UnprotectCustomLinks()
        'SuppressRedraw() : rtb.SetListStyle(ListStyle) ': 'ResumeRedraw()
        ''ReprotectCustomLinks
        '   update toolbar
        'UpdateToolbarNoChangesTracked()
    End Sub

    Private Sub NoneContextToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles NoneContextToolStripMenuItem.Click, BulletContextToolStripMenuItem.Click,
            NumContextToolStripMenuItem.Click, LCAlphaContextToolStripMenuItem.Click,
            UCAlphaContextToolStripMenuItem.Click, LCRomanContextToolStripMenuItem.Click,
            UCRomanContextToolStripMenuItem.Click
        NoneToolStripMenuItem_Click(sender, e)
    End Sub

    '      Handles spell-check
    Private Sub SpellcheckToolStripButton_Click(ByVal sender As System.Object,
        ByVal e As System.EventArgs) Handles SpellcheckToolStripButton.Click,
            SpellCheckToolStripMenuItem.Click
        '   do spell check on selection
        'ReprotectCustomLinks : GetRange()
        '   are there hyhens to account for?
        Dim DoSpellCheck As Boolean = True
        If rtb.Find(HyphenChar, StartPos, EndPos, RichTextBoxFinds.NoHighlight) > -1 Then
            Beep()
            DoSpellCheck =
                (MessageBox.Show(
                    "Text to be spell-checked contains syllable hyphen(s)." & ControlChars.CrLf _
                        & "This might affect the accuracy of the spell-check.",
                    "Hyphens Present", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) = DialogResult.OK)
        End If
        '   do spell-check if given the OK
        If DoSpellCheck Then
            With SpellChecker
                If rtb.SelectionLength = 0 Then
                    .Text = rtb.Text         'all text
                Else
                    .Text = rtb.SelectedText 'selected text
                End If
                .SpellCheck()
            End With
        End If
    End Sub

    '      Handles pictures
    Private Sub InsertPictureToolStripButton_Click(sender As Object, e As System.EventArgs) _
        Handles InsertPictureToolStripButton.Click, InsertPictureToolStripMenuItem.Click
        '   paste picture into rich text-box
        'SetProtectionForCustomLinks(rtb.SelectionLength = 0)
        Try
            Me.InsertPicture()
        Catch ex As Exception
            Beep() : MessageBox.Show("Invalid picture format!")
        End Try
        'ReprotectCustomLinks
    End Sub

    '      Handles symbols
    Private Sub InsertSymbolToolStripButton_Click(sender As Object,
        e As EventArgs) Handles InsertSymbolToolStripButton.Click,
            InsertSymbolToolStripMenuItem.Click
        '   paste symbol
        Dim InsertSymbolForm As frmInsertSymbol = New frmInsertSymbol(GetCurrentFont)
        With InsertSymbolForm
            If .ShowDialog() = DialogResult.OK Then
                '   symbol was selected
                'SuppressRedraw()  'SetProtectionForCustomLinks(rtb.SelectionLength = 0)
                rtb.SelectionFont = .SymbolFont
                If IsNotProtected Then
                    rtb.SelectedText = .SymbolCharacter
                End If
                'ReprotectCustomLinks
                IsNotProtected = True ': 'ResumeRedraw()
            End If
        End With
    End Sub

#End Region

#Region "TEXT-SEARCHING AND HYPHENATION EVENT PROCEDURES"

    '      Handles searching for and/or replacing sub-text
    Private Sub FindToolStripButton_Click(sender As Object,
        e As System.EventArgs) Handles FindToolStripButton.Click
        '    find/replace
        FindOrReplaceText(True)
    End Sub

    Private Sub FindToolStripMenuItem_Click(sender As Object, e As System.EventArgs) _
        Handles FindToolStripMenuItem.Click
        '   find only
        FindOrReplaceText(False)
    End Sub

    Private Sub ReplaceToolStripMenuItem_Click(sender As Object, e As System.EventArgs) _
        Handles ReplaceToolStripMenuItem.Click
        '   replace/find
        FindOrReplaceText(True)
    End Sub

    '      Handles searching for subsequent appearances of sub-text 
    Private Sub FindNextToolStripButton_Click(sender As Object,
        e As System.EventArgs) Handles FindNextToolStripButton.Click,
            FindNextToolStripMenuItem.Click
        'ReprotectCustomLinks
        With SearchInfo
            If String.IsNullOrEmpty(.SearchText) Then
                '   no text given? (shouldn't happen)
                Beep() : MessageBox.Show("No search string was specified!")
            Else
                '   find it
                Dim CurrentIndex As Integer = rtb.SelectionStart
                Dim SearchIndex As Integer = FindOccurrence()
                If SearchIndex = CurrentIndex Then
                    '   were we already on it? then search for another
                    SearchIndex = FindOccurrence()
                End If
                '   have we exhausted our search?
                If SearchIndex = NotFound Then
                    ReportDoneSearching()
                End If
            End If
            'UpdateToolbarNoChangesTracked()
        End With
    End Sub

    '   Handle hyphenation
    Private Sub HyphenateToolStripButton_Click(sender As Object,
        e As System.EventArgs) Handles HyphenateToolStripButton.Click,
            HyphenateTextToolStripMenuItem.Click
        '   hyphenate
        HyphenateText()
    End Sub

    Private Sub RemoveAllHyphensToolStripMenuItem1_Click(sender As Object, e As EventArgs) _
        Handles RemoveAllHyphensToolStripMenuItem1.Click
        '   de-hyphenate -- remove all hyphens
        RemoveHyphens(True)
    End Sub

    Private Sub RemoveHiddenHyphensOnlyToolStripMenuItem1_Click(sender As Object,
        e As EventArgs) Handles RemoveHiddenHyphensOnlyToolStripMenuItem1.Click
        '   de-hyphenate -- remove hyphens not at line-ends
        RemoveHyphens(False)
    End Sub

#End Region

#Region "RULER MEASUREMENT MENU EVENT PROCEDURES"
    'Private Sub MeasureInCentimetersToolStripMenuItem_Click(sender As Object, e As EventArgs) _
    '    Handles MeasureInCentimetersToolStripMenuItem.Click,
    '        MeasureInInchesToolStripMenuItem.Click
    '    With TextRuler1
    '        '   set checks
    '        MeasureInInchesToolStripMenuItem.Checked =
    '            sender Is MeasureInInchesToolStripMenuItem
    '        MeasureInCentimetersToolStripMenuItem.Checked =
    '            sender Is MeasureInCentimetersToolStripMenuItem
    '        '   which one is it?
    '        If sender Is MeasureInInchesToolStripMenuItem Then
    '            '   measure in inches now
    '            .Units = TextRuler.UnitType.Inches
    '        Else
    '            '   measure in centimeters now
    '            .Units = TextRuler.UnitType.Centimeters
    '        End If
    '    End With
    'End Sub
#End Region

#Region "VERTICAL ALIGNMENT AND HYPERLINK EVENT PROCEDURES"

    '   handle superscript
    Private Sub RaisedSuperscriptToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles RaisedSuperscriptToolStripMenuItem.Click
        If rtb.SelectionCharOffset > 0 Then
            HandleSuperOrSubScript(0, True)
        Else
            HandleSuperOrSubScript(GetCurrentFont().Size \ 2, True)
        End If
    End Sub

    '   handle subscript
    Private Sub LoweredSubscriptToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles LoweredSubscriptToolStripMenuItem.Click
        If rtb.SelectionCharOffset < 0 Then
            HandleSuperOrSubScript(0, True)
        Else
            HandleSuperOrSubScript(-GetCurrentFont().Size \ 2, True)
        End If
    End Sub

    '   handle inserting/updating/removing a hyperlink
    'Private Sub HyperlinkToolstripButton_Click(sender As Object, e As EventArgs) _
    '    Handles HyperlinkToolstripButton.Click,
    '        InsertEditRemoveHyperlinkToolStripMenuItem.Click
    '    '   check for link
    '    'ReprotectCustomLinks
    '    Dim Start As Integer = rtb.SelectionStart,
    '        Length As Integer = rtb.SelectionLength
    '    Dim ExistingLink As CustomLinkInfo = GetCustomLink(Start, (Length > 0))
    '    Dim LinkExists As Boolean =
    '        ExistingLink IsNot Nothing
    '    If Not LinkExists Then
    '        '   link not pre-existing
    '        If rtb.SelectionProtected Then
    '            '   wholly protected but not a link
    '            Beep()
    '            MessageBox.Show("Protected non-link text is not available for a link.",
    '                "Invalid Edit")
    '            Exit Sub
    '        ElseIf Length > 0 Then
    '            '   validate selected text
    '            ExistingLink = GetValidCustomLinkInfo(Start, rtb.SelectedText)
    '            If ExistingLink Is Nothing Then
    '                Beep()
    '                MessageBox.Show("Invalid text for link.")
    '                Exit Sub
    '            End If
    '        Else
    '            '   no selection--starting from scratch
    '            ExistingLink =
    '                New CustomLinkInfo _
    '                    With {.Position = Start, .Text = "", .Hyperlink = ""}
    '        End If
    '    End If
    '    '   let user set link info
    '    Dim HyperlinkForm As frmInsertLink =
    '        New frmInsertLink(CopyCustomLinkInfo(ExistingLink),
    '            LinkExists, KeepingHypertextOnRemove)
    '    With HyperlinkForm
    '        .ShowDialog() : KeepingHypertextOnRemove = .KeepHypertext
    '        Select Case .LinkAction
    '            Case LinkActions.Insert
    '                '   insert new link
    '                rtb.SelectedText = "" : InsertCustomLink(.CustomLinkInfo)
    '            Case LinkActions.Remove
    '                '   remove existing link
    '                DeleteCustomLink(ExistingLink.Position)
    '            Case LinkActions.Update
    '                '   update--replace existing link with new info
    '                DeleteCustomLink(ExistingLink.Position)
    '                With .CustomLinkInfo
    '                    .Position = ExistingLink.Position - 1
    '                    rtb.Select(.Position, rtb.SelectionStart - .Position)
    '                End With
    '                InsertCustomLink(.CustomLinkInfo)
    '        End Select
    '        '   clean up and exit
    '        IsNotProtected = True
    '    End With
    'End Sub

    '   handle removing all custom links from a slection
    'Private Sub RemoveAllHyperlinksToolStripMenuItem_Click(sender As Object, e As EventArgs) _
    '    Handles RemoveAllHyperlinksToolStripMenuItem.Click
    '    'ReprotectCustomLinks : RemoveCustomLinksInSelection(False)
    'End Sub

    '   handle turning on/off hyperlink-text retention when removing custom links
    'Private Sub KeepHypertextWhenRemovingLinksToolStripMenuItem_Click(
    '    sender As Object, e As EventArgs) _
    '        Handles KeepHypertextWhenRemovingLinksToolStripMenuItem.Click
    '    KeepingHypertextOnRemove =
    '        Not KeepingHypertextOnRemove
    '    KeepHypertextWhenRemovingLinksToolStripMenuItem.Checked = KeepingHypertextOnRemove
    'End Sub

#End Region

#Region "NON-EVENT_PROCEDURES"

    '   set file-format filters and default extension
    Private Sub SetFileFormatFilters(fd As FileDialog, ByVal Format As RichTextBoxStreamType)
        Select Case Format
            Case NoSpecificFileFormat
                '   rich or plain text
                fd.DefaultExt = "rtf"
                fd.Filter =
                    "Rich text files (*.rtf)|*.rtf|Plain text files (*.txt)|*.txt|" _
                    & "All files (*.*)|*.*"
            Case RichTextBoxStreamType.RichText, RichTextBoxStreamType.RichNoOleObjs
                '   rich text
                fd.DefaultExt = "rtf"
                fd.Filter = "Rich text files (*.rtf)|*.rtf|All files (*.*)|*.*"
            Case Else
                '   plain text
                fd.DefaultExt = "txt"
                fd.Filter = "Plain text files (*.txt)|*.txt|All files (*.*)|*.*"
        End Select
    End Sub

    '   determine file format
    Private Function GetFileFormat(ByVal FileName As String,
        ByVal Format As RichTextBoxStreamType) As RichTextBoxStreamType
        If Format = NoSpecificFileFormat Then
            '   if any format is allowed, check filename extension
            If FileIO.FileSystem.GetName(FileName).ToLower.EndsWith(".rtf") Then
                Return RichTextBoxStreamType.RichText
            Else
                Return RichTextBoxStreamType.PlainText
            End If
        Else
            '   return specified format
            Return Format
        End If
    End Function

    '   get info on width of text
    'Private Sub 'GetHorizontalInfo()
    '    With rtb
    '        '   synch ruler's magnification to text-box's
    '        If .ZoomFactor <> TextRuler1.ZoomFactor Then
    '            TextRuler1.ZoomFactor = .ZoomFactor
    '            rtb.Focus()
    '        End If
    '        '   account for any selection margin
    '        SelectionOffset = .Padding.Left + 1
    '        If .ShowSelectionMargin Then
    '            SelectionOffset += CInt(SelectionMargin * rtb.ZoomFactor)
    '        End If
    '        '   determine maximum linewidth
    '        MaxTextWidth = .GetMaximumWidth()
    '        If MaxTextWidth > .ClientRectangle.Width - SelectionOffset Then
    '            '   if larger than text-box client area, enable horizontal scroll bars
    '            .WordWrap = False
    '            .ScrollBars =
    '                .ScrollBars Or RichTextBoxScrollBars.Horizontal
    '        End If
    '        If rtb.RightMargin <> MaxTextWidth Then
    '            rtb.RightMargin = MaxTextWidth
    '        End If
    '    End With
    '    '   set up ruler if no
    '    With TextRuler1
    '        If Not UpdatingIndentsOrTabs Then
    '            .SetBounds(SelectionOffset, .Top, MaxTextWidth, .Height)
    '            .PrintableWidth = MaxTextWidth : .ScrollingOffset = rtb.GetScrollPosition.X
    '            .FirstLineIndent = rtb.SelectionIndent
    '            .HangingIndent = rtb.SelectionHangingIndent
    '            .RightIndent = rtb.SelectionRightIndent
    '            If AllowingTabs Then
    '                .TabPositions = rtb.SelectionTabs
    '            End If
    '        End If
    '    End With
    'End Sub

    '   update toolbar
    Private Sub UpdateToolbarNoChangesTracked()
        '   update toolbar without tracking changes
        TrackingChanges = False  'UpdateToolbar() : TrackingChanges = True
    End Sub

    'Private Sub 'UpdateToolbar()
    '    '   update toolbar and raise ChangesMade event if text is changed
    '    rtb_TextChanged(rtb, New EventArgs()) : ResetFocus()
    'End Sub

    '   suppress or resume redrawing
    'Private Sub 'TurnRedrawModeOnOrOff(ByVal AreRedrawing As Boolean)
    '    rtb.SetRedrawMode(AreRedrawing) : ToolStrip1.SetRedrawMode(AreRedrawing)
    '    TextRuler1.SetRedrawMode(AreRedrawing)
    '    If AreRedrawing Then
    '        rtb.ResumeLayout()
    '    Else
    '        rtb.SuspendLayout()
    '    End If
    'End Sub

    'Private Sub 'SuppressRedraw()
    '    AreRedrawing = False : 'TurnRedrawModeOnOrOff(AreRedrawing)
    'End Sub

    'Private Sub 'ResumeRedraw()
    '    AreRedrawing = True : 'TurnRedrawModeOnOrOff(AreRedrawing)
    'End Sub

    '   determine whether or not our text range is universal
    Private Function IsRangeASelection() As Boolean
        Return _
            StartPos > NoBeginning AndAlso Not DoingAll
    End Function

    '   save any range to text
    Private Sub GetRange()
        '   range default to all text
        DoingAll =
           rtb.SelectionLength = 0
        If DoingAll Then
            '   complete text
            StartPos = 0 : EndPos = rtb.TextLength
        Else
            '   get range of text
            StartPos = rtb.SelectionStart : EndPos = StartPos + rtb.SelectionLength
        End If
    End Sub

    '   restore any range
    Private Sub RestoreRange()
        '   restore any range
        If IsRangeASelection() Then
            rtb.Select(StartPos, EndPos - StartPos)
        End If
        StartPos = NoBeginning : EndPos = NoEnd
    End Sub

    '   dispense with edit range
    Private Sub DoneWithRange(
        Optional ByVal Direction As RichTextBoxFinds = RichTextBoxFinds.None,
        Optional ByVal AreMovingCaret As Boolean = True)
        '   finished with range--set caret
        If MaintainingSelection AndAlso IsRangeASelection() Then
            '   restore selection
            RestoreRange()
        Else
            '   set for no selection
            If AreMovingCaret Then
                '   set caret
                If (Direction And RichTextBoxFinds.Reverse) = 0 Then
                    '   go to end of range
                    rtb.Select(EndPos, 0)
                Else
                    '   go to beginning of range
                    rtb.Select(StartPos, 0)
                End If
            End If
            '   flag that we are no longer using a range
            StartPos = NoBeginning : EndPos = NoEnd
        End If
        'UpdateToolbarNoChangesTracked()
    End Sub

    '   get position in line of character index
    Private Function GetPositionInLine(ByVal Index As Integer) As Integer
        Return _
            Index - rtb.GetFirstCharIndexFromLine(rtb.GetLineFromCharIndex(Index))
    End Function

    '   replace RTF text
    Private Sub ReplaceRTFText(ByVal ReplacementText As String)
        '   remove existing text and insert RTF info
        rtb.InsertRtf(ReplacementText) : IsNotProtected = True
    End Sub

    '   document whether change is made and raise ChangesMade event if True
    Private Sub HandleTrackedChange()
        '   has text been modified?
        If TrackingChanges AndAlso rtb.Modified Then
            '   a permanent (tracked) change has been made
            HaveChangedText = True : Me.OnChangesMade(New EventArgs())
        End If
        '   don't fire ChangesMade again until a new change is actually made
        rtb.Modified = False
    End Sub

    '   look for words to hyphenate
    Private Sub HyphenateText()
        'ReprotectCustomLinks
        '   get selection range
        GetRange() : TrackingChanges = False 'ignore temporary changes
        '   temporarily add line break to end of text
        rtb.Select(rtb.TextLength, 0) : rtb.SelectionProtected = False
        rtb.SelectedText = ControlChars.Lf
        '   define variables
        Dim Word As String = ""
        Dim CurrentLine As Integer = rtb.GetLineFromCharIndex(StartPos) - 1
        Dim StartOfLine, EndOfLine, WordEnd, LineAtBreak, MaxPosition As Integer
        '   parse text line by line
        Do
            '   get start of current line
            CurrentLine += 1
            StartOfLine = rtb.GetFirstCharIndexFromLine(CurrentLine)
            '   see if first word is wrapped from last line
            If StartOfLine = 0 Then
                '   beginning of text--don't break word
                Continue Do
            Else
                '   beginning of paragraph or page?
                Select Case rtb.Text.Substring(StartOfLine - 1, 1)
                    Case ControlChars.Lf, ControlChars.FormFeed
                        '   yes--don't break word
                        Continue Do
                End Select
            End If
            '   get end of line
            If CurrentLine = rtb.Lines.GetUpperBound(0) Then
                EndOfLine = rtb.TextLength 'already at last line
            Else
                EndOfLine = rtb.GetFirstCharIndexFromLine(CurrentLine + 1)
            End If
            '    get end of first word in line
            WordEnd = StartOfLine
            Do While WordEnd < EndOfLine _
                    AndAlso Char.IsLetterOrDigit(rtb.Text.Chars(WordEnd))
                WordEnd += 1
            Loop
            '   get word (must allow for 2+ chars at end of last line
            '   and 3+ at beginning of current line)
            Word = rtb.Text.Substring(StartOfLine, WordEnd - StartOfLine)
            MaxPosition = Word.Length - 2
            If MaxPosition > 1 Then
                '   look for greatest position at which word may be broken
                'SuppressRedraw()
                Do
                    MaxPosition -= 1
                    '   temporarily insert hyphen here and check line #
                    rtb.Select(StartOfLine + MaxPosition, 0)
                    If rtb.SelectionProtected Then
                        Continue Do 'don't hyphenate link
                    End If
                    rtb.SelectedText = HyphenChar
                    LineAtBreak = rtb.GetLineFromCharIndex(StartOfLine + MaxPosition)
                    rtb.Undo() 'reverse insertion
                Loop Until MaxPosition = 1 OrElse LineAtBreak < CurrentLine
                'ResumeRedraw()
                '   if word can be hyphenated, ask user if and where
                If LineAtBreak < CurrentLine AndAlso MaxPosition > 1 Then
                    '   get user input
                    Dim HyphenationForm As frmHyphenate = New frmHyphenate(Word, MaxPosition)
                    With HyphenationForm
                        Dim Result As DialogResult = .ShowDialog()
                        '   what are we to do?
                        If Result = DialogResult.Cancel Then
                            '   we are done
                            Exit Do
                        ElseIf .DesiredPosition > 0 Then
                            '   insert hyphen here
                            rtb.Select(StartOfLine + .DesiredPosition, 0)
                            TrackingChanges = True 'make sure this change is tracked
                            rtb.SelectedText = HyphenChar
                            If IsNotProtected Then
                                EndPos += 1
                            End If
                            IsNotProtected = True : TrackingChanges = False
                        End If
                    End With
                End If
            End If
        Loop Until CurrentLine = rtb.GetLineFromCharIndex(EndPos)
        '   remove temporary line break at end
        rtb.Select(rtb.TextLength - 1, 1)
        If rtb.SelectedText = ControlChars.Lf Then
            rtb.SelectedText = ""
        End If
        TrackingChanges = True 'notice all changes again
        '   finished--restore selection
        DoneWithRange()
    End Sub

    '   remove hyphens -- return ending position
    Private Sub RemoveHyphens(ByVal RemoveAll As Boolean)
        'ReprotectCustomLinks
        '   get selection range
        GetRange()  'SuppressRedraw()
        Dim HyphenPos As Integer =
            rtb.Find(HyphenChar, StartPos, EndPos, RichTextBoxFinds.None) 'find first hyphen
        Do While HyphenPos > NotFound AndAlso HyphenPos < EndPos
            '   do we remove this hyphen?
            If RemoveAll OrElse GetPositionInLine(HyphenPos + 1) > 0 Then
                '   removing all hyphens or this hyphen is not at the end of a line
                If Not rtb.SelectionProtected Then
                    rtb.SelectedText = "" : EndPos -= 1
                End If
            Else
                '   hyphen is at end of line--skip
                HyphenPos += 1
            End If
            HyphenPos =
                rtb.Find(HyphenChar, HyphenPos, EndPos, RichTextBoxFinds.None) 'find next hyphen
        Loop
        '   finished--set caret to end of selection
        DoneWithRange() ': 'ResumeRedraw()
    End Sub

    '   search/replace text
    Private Sub FindOrReplaceText(ByVal AllowingReplacing As Boolean)
        '   initialize info
        Dim SearchOption As SearchOptions = SearchOptions.None,
            SearchForm As frmSearch = Nothing,
            IsFirstTime As Boolean = True
        'ReprotectCustomLinks : GetRange()
        Do
            '   initialize search form
            SearchForm = New frmSearch(SearchInfo, AllowingReplacing)
            Dim Result As DialogResult,
                IsDone As Boolean = False,
                ReplacementCount As Integer = NotReplacing
            With SearchForm
                '   display form
                Result = .ShowDialog()
                If Result = DialogResult.Cancel Then
                    '   finished!
                    Exit Do
                End If
                '   get info returned
                SearchOption = .SearchOption : SearchInfo = .SearchInfo
            End With
            '   what are we doing
            If SearchOption = SearchOptions.None Then
                '   finished! (should be pre-empted by above logic)
                Exit Do
            ElseIf IsFirstTime AndAlso IsRangeASelection() _
                   AndAlso (SearchInfo.SearchFinds And RichTextBoxFinds.Reverse) _
                       = RichTextBoxFinds.Reverse Then
                '   we are searching backwards through a selection
                rtb.Select(EndPos, 0)
            End If
            IsFirstTime = False 'no longer an issue
            '   what should we do?
            Select Case SearchOption
                Case SearchOptions.FindNext
                    '   find next occurrence
                    IsDone = FindNextText() : ReplacementCount = NotReplacing
                Case SearchOptions.Replace
                    '   replace 1 occurence
                    IsDone = ReplaceText() : ReplacementCount = NotReplacing
                Case SearchOptions.ReplaceAll
                    '   replace all occurrences
                    ReplacementCount = ReplaceAllText() : IsDone = True
            End Select
            '   do we need to change the final selection?
            If IsDone Then
                '   we've reached edge of available text
                ReportDoneSearching(ReplacementCount)
            End If
        Loop
        '   searching finished--clean up
        If SearchOption = SearchOptions.FindNext Then
            '   last did a Find--narrow selection to it
            StartPos = rtb.SelectionStart : EndPos = StartPos + rtb.SelectionLength
        End If
        DoneWithRange(SearchInfo.SearchFinds, False)
    End Sub

    '   report if end of text is reached
    Private Sub ReportDoneSearching(
        Optional ByVal ReplacementCount As Integer = NotReplacing)
        '   redraw to show changes
        'ResumeRedraw()
        '   update info on RichTextBoxEx control
        Dim ReportText As String = ""
        With SearchInfo
            '   are we search everything or just a region
            If IsRangeASelection() Then
                ReportText = "selected text"
            Else
                ReportText = "text"
            End If
            '   finished searching
            If (.SearchFinds And RichTextBoxFinds.Reverse) = 0 Then
                ReportText = "Reached the end of the " & ReportText & "!"
                '   go to end of range
                If IsRangeASelection() Then
                    rtb.Select(EndPos, 0)         'end of selection 
                Else
                    rtb.Select(rtb.TextLength, 0) 'end of entire text
                End If
            Else
                ReportText = "Reached the beginning of the " & ReportText & "!"
                '   go to beginning of range
                If IsRangeASelection() Then
                    rtb.Select(StartPos, 0) 'beginning of selection
                Else
                    rtb.Select(0, 0)        'beginning of entire text
                End If
            End If
            If ReplacementCount > NotReplacing Then
                '   replacements were made
                ReportText &=
                    ControlChars.CrLf & ControlChars.CrLf _
                    & "Number of replacements: " & ReplacementCount.ToString
            End If
            '   report
            Beep() : MessageBox.Show(ReportText)
        End With
    End Sub

    '   find search text -- return whether at end
    Private Function FindNextText() As Boolean
        '   find text
        With SearchInfo
            '   get info
            'SuppressRedraw()
            Dim SearchIndex As Integer = FindOccurrence()
            'ResumeRedraw()
            Return _
                SearchIndex = NotFound
        End With
    End Function

    '   replace one instance of search text (if on it) -- return whether at end
    Private Function ReplaceText() As Boolean
        '   replace current occurrence
        With SearchInfo
            '   get info
            'SuppressRedraw() : .IsFirstFind = True
            Dim CurrentPos As Integer = rtb.SelectionStart,
                SearchIndex As Integer = FindOccurrence()
            If SearchIndex = CurrentPos Then
                '   already at an occurrence--replace it and move to next one
                SearchIndex = MakeReplacement(1)
            End If
            'ResumeRedraw()
            Return _
                SearchIndex = NotFound
        End With
    End Function

    '   replace all instances of search text -- return # of replacements
    Private Function ReplaceAllText() As Integer
        '   replace all occurrences from here on
        With SearchInfo
            '   get info
            'SuppressRedraw() : .IsFirstFind = True
            Dim SearchIndex As Integer = FindOccurrence(),
                ReplacementCount As Integer = 0
            '   make replacements
            Do While SearchIndex > NotFound
                SearchIndex = MakeReplacement(ReplacementCount)
            Loop
            'ResumeRedraw()
            '   return # of replacements
            Return ReplacementCount
        End With
    End Function

    '   find an occurrence of search text
    Private Function FindOccurrence() As Integer
        '   StringLength = .ReplacementText.Length if a replacement was previously made,
        '      otherwise StringLength = .SearchText.Length
        With SearchInfo
            '   look for item
            Dim SearchPos As Integer = rtb.SelectionStart,
                SearchIndex As Integer = NotFound
            If (.SearchFinds And RichTextBoxFinds.Reverse) = 0 Then
                '   searching down
                If Not .IsFirstFind Then
                    '   find subsequent occurrence
                    SearchPos += 1
                    'If DoingCustomLinks AndAlso SearchPos <= rtb.TextLength Then
                    '    '   move past any custom link
                    '    Dim CustomLinkInfo As CustomLinkInfo = GetCustomLink(SearchPos)
                    '    If CustomLinkInfo IsNot Nothing Then
                    '        SearchPos = GetEndOfCustomLink(CustomLinkInfo)
                    '    End If
                    'End If
                End If
                If SearchPos <= rtb.TextLength Then
                    SearchIndex = rtb.Find(.SearchText, SearchPos, NoEnd, .SearchFinds)
                End If
            Else
                '   searching up
                If Not .IsFirstFind Then
                    '   find previous occurrence
                    SearchPos -= 1
                    'If DoingCustomLinks AndAlso SearchPos >= 0 Then
                    '    '   move past any custom link
                    '    Dim CustomLinkInfo As CustomLinkInfo = GetCustomLink(SearchPos)
                    '    If CustomLinkInfo IsNot Nothing Then
                    '        SearchPos = CustomLinkInfo.Position - 1
                    '    End If
                    'End If
                End If
                If SearchPos >= 0 Then
                    SearchIndex =
                        rtb.Find(.SearchText, 0, SearchPos + .SearchText.Length, .SearchFinds)
                End If
            End If
            If IsRangeASelection() _
                    AndAlso (SearchIndex < StartPos _
                        OrElse SearchIndex > EndPos - .SearchText.Length) Then
                SearchIndex = NotFound 'out of range
            End If
            '   return value
            .IsFirstFind = (SearchIndex = NotFound)
            Return SearchIndex
        End With
    End Function

    '   replace an occurrence of search text -- return next occurrence position
    Private Function MakeReplacement(ByRef ReplacementCount As Integer) As Integer
        '   replacement count is adjusted here based on success or failure
        With SearchInfo
            '   initialize
            Dim NewPos As Integer = rtb.SelectionStart,
                IsTooFar As Boolean = False
            '   replace current occurrence and adjust range
            'If DoingCustomLinks AndAlso AreCustomLinksInSelection() Then
            '    '   can't replace a custom link
            '    Beep() : IsNotProtected = False
            '    MessageBox.Show("Text in active hyperlink(s) cannot be replaced.",
            '        "Link Present", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            'Else
            '   no custom links in selection--make change
            rtb.SelectedText = .ReplacementText
            If IsNotProtected Then
                '   replacement succeeded
                EndPos += .ReplacementText.Length - .SearchText.Length
                ReplacementCount += 1
            End If
            'End If
            '   get new search start
            If (.SearchFinds And RichTextBoxFinds.Reverse) = 0 Then
                '   searching down
                If IsNotProtected Then
                    '   replacement succeeded
                    NewPos += .ReplacementText.Length
                    rtb.Select(rtb.SelectionStart - .ReplacementText.Length,
                        .ReplacementText.Length)
                    rtb.SelectionProtected = False
                Else
                    '   replacement failed
                    NewPos += .SearchText.Length
                End If
                IsTooFar = (NewPos >= rtb.TextLength)
                If Not .IsFirstFind Then
                    NewPos -= 1 'adjust
                End If
                'If DoingCustomLinks Then
                '    '   move past any custom link
                '    Dim CustomLinkInfo As CustomLinkInfo = GetCustomLink(NewPos)
                '    If CustomLinkInfo IsNot Nothing Then
                '        NewPos = GetEndOfCustomLink(CustomLinkInfo)
                '        IsTooFar = (NewPos >= rtb.TextLength)
                '    End If
                'End If
            Else
                '   searching up
                NewPos -= .SearchText.Length
                IsTooFar = (NewPos < 0)
                If Not .IsFirstFind Then
                    NewPos += 1 'adjust
                End If
                'If DoingCustomLinks Then
                '    '   move past any custom link
                '    Dim CustomLinkInfo As CustomLinkInfo = GetCustomLink(NewPos)
                '    If CustomLinkInfo IsNot Nothing Then
                '        NewPos = CustomLinkInfo.Position - 1 : IsTooFar = (NewPos < 0)
                '    End If
                'End If
            End If
            IsNotProtected = True
            '   get next occurrence
            If IsTooFar Then
                '   new position is out of bounds
                Return NotFound
            Else
                '   look for another occurrence
                rtb.Select(NewPos, 0)
                Return _
                    FindOccurrence()
            End If
        End With
    End Function

    '   validate and set new info
    Private Function AreSettingValidFontName(ByVal AreShowingErrors As Boolean) As Boolean
        '   get the new font name and use it if it's valid
        Dim NewFontName As String = FontNameToolStripComboBox.Text
        If Not String.IsNullOrEmpty(NewFontName) Then
            For Each FontFamily As FontFamily In FontFamily.Families
                If FontFamily.Name = NewFontName Then
                    '	new font exists--set it and leave
                    Dim currentFont As System.Drawing.Font = GetCurrentFont()
                    If currentFont IsNot Nothing Then
                        'UnprotectCustomLinks()
                        Try
                            '   set font face
                            rtb.SelectionFont =
                                New Drawing.Font(NewFontName,
                                    currentFont.Size, currentFont.Style)
                            Return True
                        Catch ex As Exception
                            '   invalid font name
                            If AreShowingErrors Then
                                Beep() : MessageBox.Show("Invalid font name!", "Font Name")
                            End If
                            Return False
                        Finally
                            ''ReprotectCustomLinks
                        End Try
                    End If
                End If
            Next FontFamily
        End If
        '   flag failure
        Return False
    End Function

    Private Function AreSettingValidFontSize(ByVal AreShowingErrors As Boolean) As Boolean
        '    get the new font size and use if it's valid
        Dim NewFontSize As Single
        If Single.TryParse(FontSizeToolStripComboBox.Text, NewFontSize) Then
            If NewFontSize > 0 And NewFontSize <= MaxFontSize Then
                '   font size is within range--set it
                Dim currentFont As System.Drawing.Font = GetCurrentFont()
                If currentFont IsNot Nothing Then
                    'UnprotectCustomLinks()
                    Try
                        '   set font size
                        rtb.SelectionFont =
                            New Drawing.Font(currentFont.FontFamily,
                                NewFontSize, currentFont.Style)
                        Return True
                    Catch ex As Exception
                        '   invalid font size
                        Beep()
                        If AreShowingErrors Then
                            MessageBox.Show("Invalid font size!", "Font Size")
                        End If
                    Finally
                        ''ReprotectCustomLinks
                    End Try
                End If
            End If
        End If
        '   flag failure
        Return False
    End Function

    '   get current font
    Private Function GetCurrentFont() As Font
        '   get font in current selection
        Dim currentFont As Font = rtb.SelectionFont
        If currentFont Is Nothing Then
            '   if multiple fonts are in selection, then get font at beginning of selection
            'TurnRedrawModeOnOrOff(False)
            Dim length As Integer = rtb.SelectionLength
            rtb.SelectionLength = 0 : currentFont = rtb.SelectionFont
            rtb.SelectionLength = length  'TurnRedrawModeOnOrOff(AreRedrawing)
        End If
        '   return current font
        Return currentFont
    End Function

    '    get current foreground color
    Private Function GetCurrentColor() As Color
        '   get foreground color in current selection
        Dim currentColor As Color = rtb.SelectionColor
        If currentColor = Color.Empty Then
            '   if multiple colors are in selection, then get color at beginning of selection
            'TurnRedrawModeOnOrOff(False)
            Dim length As Integer = rtb.SelectionLength
            rtb.SelectionLength = 0 : currentColor = rtb.SelectionColor
            rtb.SelectionLength = length  'TurnRedrawModeOnOrOff(AreRedrawing)
        End If
        '   return current Color
        Return currentColor
    End Function

    '     get current backgound color
    Private Function GetCurrentBackgroundColor() As Color
        '   get background color in current selection
        Dim currentColor As Color = rtb.SelectionBackColor
        If currentColor = Color.Empty Then
            '   if multiple colors are in selection, then get color at beginning of selection
            'TurnRedrawModeOnOrOff(False)
            Dim length As Integer = rtb.SelectionLength
            rtb.SelectionLength = 0 : currentColor = rtb.SelectionBackColor
            rtb.SelectionLength = length  'TurnRedrawModeOnOrOff(AreRedrawing)
        End If
        '   return current Color
        Return currentColor
    End Function

    '   reset focus to text box after action
    Private Sub ResetFocus()
        If Me.ActiveControl IsNot rtb Then
            rtb.Focus()
        End If
    End Sub

    '   toggle font style
    Private Sub ToggleFontStyle(ByVal FontStyle As Drawing.FontStyle)
        Dim currentFont As Font = GetCurrentFont()
        If currentFont IsNot Nothing Then
            '   get current font style
            Dim newFontStyle As System.Drawing.FontStyle
            '   toggle bit for Underline
            newFontStyle = (currentFont.Style Xor FontStyle)
            'UnprotectCustomLinks()
            Try
                rtb.SelectionFont =
                    New Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle)
            Catch ex As Exception
                Beep()
            End Try
            ''ReprotectCustomLinks
        End If
    End Sub

    '   handle superscript/subscript situations
    Private Sub HandleSuperOrSubScript(ByVal CharOffset As Integer,
        Optional ByVal Setting As Boolean = False)
        '   set checked status of context-menu options
        RaisedSuperscriptToolStripMenuItem.Checked =
            CharOffset > 0
        LoweredSubscriptToolStripMenuItem.Checked =
            CharOffset < 0
        If Setting Then
            '   set character offset
            'UnprotectCustomLinks()
            rtb.SelectionCharOffset = CharOffset
            ''ReprotectCustomLinks
        End If
    End Sub

    '   handle custom links

    Private Function CopyCustomLinkInfo(
        ByVal CustomLinkInfo As CustomLinkInfo) As CustomLinkInfo
        '   copy link information instance
        Return _
           New CustomLinkInfo With {.Position = CustomLinkInfo.Position,
                .Text = CustomLinkInfo.Text, .Hyperlink = CustomLinkInfo.Hyperlink}
    End Function

    'Private Function AreCustomLinksInSelection() As Boolean
    '    '    does the selected text include custom links?
    '    Dim StartPos As Integer = rtb.SelectionStart,
    '        EndPos As Integer = StartPos + rtb.SelectionLength
    '    Return _
    '       GetCustomLinksAtPositions(StartPos, EndPos).Length > 0
    'End Function

    'Private Function GetEndOfCustomLink(ByVal CustomLinkInfo As CustomLinkInfo) As Integer
    '    '   get tail end of link
    '    Return _
    '        CustomLinkInfo.Position + GetLengthOfCustomLink(CustomLinkInfo)
    'End Function

    'Private Function GetLengthOfCustomLink(ByVal CustomLinkInfo As CustomLinkInfo) As Integer
    '    '   get length of link--including special characters
    '    With CustomLinkInfo
    '        Return _
    '            .Text.Length + .Hyperlink.Length + 3
    '    End With
    'End Function

    'Private Function SelectOrDeselectCustomLink(ByVal CustomLinkInfo As CustomLinkInfo,
    '    ByVal AreSelecting As Boolean) As Boolean
    '    '   select or deselect custom link
    '    If AreSelecting Then
    '        '   select
    '        Dim length As Integer = GetLengthOfCustomLink(CustomLinkInfo)
    '        rtb.Select(CustomLinkInfo.Position, length)
    '        Return _
    '            rtb.SelectionLength = length
    '    Else
    '        '   deselect
    '        rtb.Select(GetEndOfCustomLink(CustomLinkInfo), 0) : rtb.SelectionProtected = False
    '        Return True
    '    End If
    'End Function

    'Private Function IsHyphenHere(ByVal position As Integer) As Boolean
    '    '   is there a hyphen at the specified position?
    '    Return _
    '        position >= 0 AndAlso position < rtb.TextLength _
    '            AndAlso rtb.Text.Substring(position, 1) = HyphenChar
    'End Function

    'Private Function AreWeBeforeACustomLink(ByVal position As Integer) As Boolean
    '    '   are we at the hyphen before a link?
    '    Return _
    '        IsHyphenHere(position) AndAlso CustomLinks.ContainsKey(position + 1)
    'End Function

    'Private Function ExpandSelectionForHyphen() As Boolean
    '    '   expand selection to cover any immediately preceding hyphen;
    '    '   return whether one was found
    '    Dim position As Integer = rtb.SelectionStart,
    '        length As Integer = rtb.SelectionLength
    '    If IsHyphenHere(position - 1) Then
    '        '   hyphen precedes selection--incorporate it
    '        rtb.Select(position - 1, length + 1) : Return True
    '    Else
    '        '   no preceding hyphen
    '        Return False
    '    End If
    'End Function

    'Private Function GetCustomLink(ByVal position As Integer,
    '    Optional ByVal IncludeHyphen As Boolean = False) As CustomLinkInfo
    '    '   see if a custom link is at a given position;
    '    '   position is +/0 to look forward, - to look backward
    '    '   IncludeHyphen is True to include a link's preceding hyphen in check, False to not
    '    '   returns CustomLinkInfo instance (Nothing if no link present at location)
    '    If CustomLinks.Count > 0 Then
    '        Dim Link As CustomLinkInfo = Nothing
    '        If position < 0 Then
    '            '   link may end at -position
    '            Link = GetCustomLinkAtPosition((-position) - 1) 'get last before
    '            If Link IsNot Nothing Then
    '                '   candidate link found--are we over it?
    '                If -position > Link.Position _
    '                        AndAlso -position <= GetEndOfCustomLink(Link) Then
    '                    Return Link 'yes
    '                ElseIf IncludeHyphen AndAlso CustomLinks.ContainsKey(-position) _
    '                       AndAlso IsHyphenHere((-position) - 1) Then
    '                    Return Link 'over hyphen
    '                End If
    '            End If
    '        Else
    '            '   link may begin at position
    '            Link = GetCustomLinkAtPosition(position) 'get last at or before
    '            If Link IsNot Nothing Then
    '                '   candidate link found--are we over it?
    '                If position >= Link.Position _
    '                        AndAlso position < GetEndOfCustomLink(Link) Then
    '                    Return Link 'yes
    '                ElseIf IncludeHyphen AndAlso CustomLinks.ContainsKey(position + 1) _
    '                       AndAlso IsHyphenHere(position) Then
    '                    Return CustomLinks(position + 1) 'over hyphen
    '                End If
    '            End If
    '        End If
    '    End If
    '    '   not over link
    '    Return Nothing
    'End Function

    'Private Function GetCustomLinkAtPosition(ByVal Position As Integer) As CustomLinkInfo
    '    '   find last link at or before specifed Position
    '    Return _
    '        (From CustomLink As CustomLinkInfo In CustomLinks.Values
    '         Take While CustomLink.Position <= Position).LastOrDefault()
    'End Function

    'Private Function GetCustomLinksAtPositions _
    '    (ByVal StartPos As Integer, ByVal EndPos As Integer) As CustomLinkInfo()
    '    '   returns array of all links at or after StartPos AND before EndPos
    '    Return _
    '        (From CustomLink As CustomLinkInfo In CustomLinks.Values
    '         Where GetEndOfCustomLink(CustomLink) > StartPos _
    '             AndAlso CustomLink.Position <= EndPos).ToArray
    'End Function

    'Private Sub GetAllCustomLinks()
    '    '   update info on any custom links after text changes
    '    UpdatingCustomLinks = True 'prevents event cascade
    '    TrackingChanges = False 'don't count changes until afterwards
    '    Dim RecentlyChanged As Boolean = rtb.Modified
    '    '   save selection and scroll position
    '    Dim Start As Integer = rtb.SelectionStart,
    '        Length As Integer = rtb.SelectionLength
    '    Dim CurrentScrollPosition As Point = rtb.GetScrollPosition()
    '    'TurnRedrawModeOnOrOff(False) 'suppress redraw
    '    '   redo all links
    '    CustomLinks.Clear()
    '    Dim IsTextLengthened As Boolean = False
    '    '   get first potential link
    '    Dim r As Regex = New Regex(CustomLinkSyntax),
    '        m As Match = r.Match(rtb.Text, 0)
    '    Do While m.Success AndAlso m.Index < rtb.TextLength
    '        '   get information on current potential link
    '        Dim CustomLinkInfo As CustomLinkInfo = New CustomLinkInfo _
    '            With {.Position = m.Index, .Text = m.Groups("Text").Value,
    '                .Hyperlink = m.Groups("Hyperlink").Value}
    '        If SelectOrDeselectCustomLink(CustomLinkInfo, True) Then
    '            If IsSelectionStyleInEffect(CFM_LINK, CFE_LINK) _
    '                    OrElse Regex.IsMatch(rtb.SelectedRtf, "\\v[^0]") Then
    '                '   valid potential new link--add it
    '                rtb.SelectionProtected = False : SetSelectionStyle(CFM_LINK, CFE_LINK)
    '                If Not ExpandSelectionForHyphen() Then
    '                    '   add preceding hyphen if it's not present
    '                    IsTextLengthened = True
    '                    rtb.SelectionLength = 0 : rtb.InsertRtf("\v0\-")
    '                    CustomLinkInfo.Position = m.Index + 1
    '                    '   adjust selection information as needed
    '                    If Start >= m.Index Then
    '                        Start += 1  'start is after insertion
    '                    ElseIf Start + Length >= m.Index Then
    '                        Length += 1 'end is after insertion
    '                    End If
    '                End If
    '                If AreProtectingLinks Then
    '                    '   protect link
    '                    rtb.Select(CustomLinkInfo.Position - 1,
    '                        GetLengthOfCustomLink(CustomLinkInfo) + 1)
    '                    rtb.SelectionProtected = True
    '                End If
    '                '   add link to list
    '                CustomLinks.Add(CustomLinkInfo.Position, CustomLinkInfo)
    '                SelectOrDeselectCustomLink(CustomLinkInfo, False)
    '                rtb.SelectionProtected = False
    '            End If
    '        End If
    '        '   look for next potential link
    '        m = r.Match(rtb.Text, GetEndOfCustomLink(CustomLinkInfo))
    '    Loop
    '    '   restore selection and scroll position
    '    rtb.Select(Start, Length) : rtb.SetScrollPosition(CurrentScrollPosition)
    '    If IsTextLengthened Then
    '        rtb.ScrollToCaret()
    '    End If
    '    'TurnRedrawModeOnOrOff(AreRedrawing)
    '    TrackingChanges = True : UpdatingCustomLinks = False
    '    rtb.Modified = (RecentlyChanged OrElse IsTextLengthened)
    'End Sub

    'Private Function InsertCustomLink(CustomLinkInfo As CustomLinkInfo) As Boolean
    '    '   insert custom link; return whether or not IsSuccessful
    '    With CustomLinkInfo
    '        '   get RTF for text and hyperlink; get selection info
    '        'TurnRedrawModeOnOrOff(False) 'suppress redraw
    '        Dim IsSuccessful As Boolean = False
    '        If CustomLinkInfo IsNot Nothing _
    '                AndAlso Not CustomLinks.ContainsKey(.Position) _
    '                AndAlso Not rtb.SelectionProtected Then
    '            UpdatingCustomLinks = True 'prevent event cascade
    '            '   link not found--insert it
    '            Dim escText As String = rtb.EscapedRTFText(.Text),
    '                escHyperlink As String = rtb.EscapedRTFText(.Hyperlink)
    '            Dim PreviousLength As Integer = rtb.TextLength
    '            '   insert RTF for text and hyperlink
    '            TrackingChanges = False : .Position += 1
    '            rtb.InsertRtf("\v0\-{\v \{}" & escText & "{\v |" & escHyperlink & "\}}")
    '            If IsNotProtected Then
    '                '   edit succeeded--mark link, account for text-length changes,
    '                '   add reference, and protect link
    '                If SelectOrDeselectCustomLink(CustomLinkInfo, True) Then
    '                    '   link text selected--complete insertion
    '                    SetSelectionStyle(CFM_LINK, CFE_LINK) 'turn on link
    '                    SelectOrDeselectCustomLink(CustomLinkInfo, False)
    '                    IsSuccessful = True
    '                Else
    '                    '   selection failed
    '                    rtb.Undo()
    '                End If
    '            Else
    '                '   edit failed
    '                IsNotProtected = True
    '            End If
    '            UpdatingCustomLinks = False : TrackingChanges = True
    '        End If
    '        '   return with success or failure
    '        'TurnRedrawModeOnOrOff(AreRedrawing)
    '        rtb.Modified = IsSuccessful : rtb_TextChanged(Me, New EventArgs())
    '        Return IsSuccessful
    '    End With
    'End Function

    'Private Function DeleteCustomLink(ByVal position As Integer) As Boolean
    '    '   remove custom link; return whether or not IsSuccessful
    '    Dim IsSuccessful As Boolean = False
    '    If CustomLinks.ContainsKey(position) Then
    '        '   link found--remove it, replace text,
    '        '   and account for text-length changes
    '        UpdatingCustomLinks = True 'prevent event cascade
    '        'TurnRedrawModeOnOrOff(False) 'suppress redraw
    '        Dim CustomLinkInfo As CustomLinkInfo = CustomLinks(position)
    '        With CustomLinkInfo
    '            Dim PreviousLength As Integer = rtb.TextLength
    '            If SelectOrDeselectCustomLink(CustomLinkInfo, True) Then
    '                '   link selected--proceeed with deletion
    '                ExpandSelectionForHyphen() : TrackingChanges = False
    '                rtb.SelectionProtected = False 'unprotect link
    '                SetSelectionStyle(CFM_LINK, 0) 'turn off link
    '                Dim ResidualText As String
    '                If KeepHypertextOnRemove Then
    '                    '   keep whole information
    '                    ResidualText = "{" & .Text & "|" & .Hyperlink & "}"
    '                Else
    '                    '   just visible text
    '                    ResidualText = .Text
    '                End If
    '                rtb.InsertRtf("{\v0" & rtb.EscapedRTFText(ResidualText) & "}")
    '                IsSuccessful = True
    '            Else
    '                '   selection failed
    '                SelectOrDeselectCustomLink(CustomLinkInfo, False)
    '            End If
    '        End With
    '        UpdatingCustomLinks = False : TrackingChanges = True
    '    End If
    '    '   return with success or failure
    '    'TurnRedrawModeOnOrOff(AreRedrawing)
    '    rtb.Modified = IsSuccessful : rtb_TextChanged(Me, New EventArgs())
    '    Return IsSuccessful
    'End Function

    'Private Function GetValidCustomLinkInfo(ByVal position As Integer,
    '    ByVal Text As String) As CustomLinkInfo
    '    '   make text into a valid link string if possible;
    '    '   returns CustomLinkInfo instance (Nothing if invalid text)
    '    'Const CustomLinkExactSyntax As String = "^" & CustomLinkSyntax & "\z"    //changedto//
    '    Dim CustomLinkExactSyntax As String = "^" & CustomLinkSyntax & "\z"
    '    '   see if it's already valid
    '    Dim m As Match = Regex.Match(Text, CustomLinkExactSyntax)
    '    If Not m.Success Then
    '        '   string not already valid--let's try to make it so
    '        Text = Text.Trim()
    '        If Text.StartsWith("{") AndAlso Text.EndsWith("}") Then
    '            '   strip off start and end braces
    '            Text = Text.Substring(1, Text.Length - 2)
    '        End If
    '        If Not Text.Contains("|") Then
    '            '   default hyperlink to main text
    '            Text = Text & "|" & Text
    '        End If
    '        '   rebuild and try again
    '        Text = "{" & Text & "}"
    '        m = Regex.Match(Text, CustomLinkExactSyntax)
    '    End If
    '    If m.Success Then
    '        '   if syntax matches, then get information
    '        Return _
    '            New CustomLinkInfo With {.Position = position,
    '                .Text = m.Groups("Text").Value, .Hyperlink = m.Groups("Hyperlink").Value}
    '    Else
    '        '   invalid text
    '        Return Nothing
    '    End If
    'End Function

    'Private Sub RemoveCustomLinksInSelection(
    '    Optional ByVal DoingWholeDocument As Boolean = False)
    '    '   remove all custom links from selected text
    '    If CustomLinks.Count = 0 Then
    '        Exit Sub
    '    End If
    '    GetRange() : UpdatingCustomLinks = True 'prevents event cascade
    '    Dim CurrentScrollPosition As Point = rtb.GetScrollPosition()
    '    'SuppressRedraw()
    '    If DoingWholeDocument Then
    '        '   whole document
    '        DoingAll = True : StartPos = 0 : EndPos = rtb.TextLength
    '    End If
    '    '   remove links in range
    '    Dim CurrentLength As Integer = rtb.TextLength
    '    '   go through region
    '    Dim Links() As CustomLinkInfo = GetCustomLinksAtPositions(StartPos, EndPos)
    '    Dim AreChanging As Boolean =
    '        Links.Length > 0
    '    For LinkIndex As Integer = Links.Length - 1 To 0 Step -1
    '        DeleteCustomLink(Links(LinkIndex).Position)
    '        '   check for change in text length
    '        If rtb.TextLength <> CurrentLength Then
    '            EndPos += (rtb.TextLength - CurrentLength)
    '            CurrentLength = rtb.TextLength
    '        End If
    '    Next LinkIndex
    '    '   restore selection and scroll position
    '    RestoreRange() : rtb.SetScrollPosition(CurrentScrollPosition)
    '    If KeepingHypertextOnRemove Then
    '        rtb.ScrollToCaret()
    '    End If
    '    'ResumeRedraw() : UpdatingCustomLinks = False
    '    rtb.Modified = AreChanging : rtb_TextChanged(rtb, New EventArgs())
    'End Sub

    'Private Sub UnprotectCustomLinks()
    '    '   temporarily remove protection from custom links
    '    AreProtectingLinks = False
    '    If DoingCustomLinks Then
    '        '    remove protection
    '        UpdatingCustomLinks = True : TrackingChanges = False
    '        If CustomLinks.Count > 0 Then
    '            'TurnRedrawModeOnOrOff(False) 'suppress redraw
    '            Dim CurrentScrollPosition As Point = rtb.GetScrollPosition()
    '            Dim Start As Integer = rtb.SelectionStart,
    '                length As Integer = rtb.SelectionLength
    '            '   unprotect links
    '            Dim Links() As CustomLinkInfo = GetCustomLinksAtPositions(0, rtb.TextLength)
    '            For LinkIndex As Integer = 0 To Links.Length - 1
    '                If SelectOrDeselectCustomLink(Links(LinkIndex), True) Then
    '                    ExpandSelectionForHyphen() : rtb.SelectionProtected = False
    '                End If
    '            Next LinkIndex
    '            '   restore scroll position and selection
    '            rtb.Select(Start, length) : rtb.SetScrollPosition(CurrentScrollPosition)
    '            'TurnRedrawModeOnOrOff(AreRedrawing)
    '        End If
    '    End If
    'End Sub

    'Private Sub 'ReprotectCustomLinks
    '    '   restore protection to custom links
    '    AreProtectingLinks = True
    '    If DoingCustomLinks Then
    '        If IsNotProtected Then
    '            rtb.Modified = True   'change succeeded--flag that
    '        Else
    '            IsNotProtected = True 'change failed--prepare for future changes
    '        End If
    '        TrackingChanges = True : UpdatingCustomLinks = False
    '        rtb_TextChanged(rtb, New EventArgs()) 're-protect links and report any change
    '    End If
    'End Sub

    'Private Sub 'SetProtectionForCustomLinks(ByVal AreTurningProtectionOn As Boolean)
    '    '   turn protection for custom-links on or off
    '    If DoingCustomLinks Then
    '        If AreTurningProtectionOn AndAlso Not AreProtectingLinks Then
    '            'ReprotectCustomLinks
    '        ElseIf Not AreTurningProtectionOn AndAlso AreProtectingLinks Then
    '            UnprotectCustomLinks()
    '        End If
    '    End If
    'End Sub

    'Private Sub NormalizeSelectionForCustomLinks()
    '    '   make sure selection bounds don't straddle custom links
    '    UpdatingCustomLinks = True 'prevents event cascade
    '    Dim StartPos As Integer = rtb.SelectionStart,
    '        EndPos As Integer = StartPos + rtb.SelectionLength
    '    Dim CustomLinkInfo As CustomLinkInfo
    '    If StartPos = EndPos Then
    '        '   no selection--are we over invisible text?
    '        CustomLinkInfo = GetCustomLink(-StartPos)
    '        If CustomLinkInfo IsNot Nothing _
    '                AndAlso StartPos > CustomLinkInfo.Position + CustomLinkInfo.Text.Length Then
    '            '   skip over it
    '            StartPos = GetEndOfCustomLink(CustomLinkInfo) : EndPos = StartPos
    '        End If
    '    Else
    '        '   check to see if bounds are on links
    '        '   does the selection start on a link?
    '        CustomLinkInfo = GetCustomLink(StartPos) 'look forward
    '        If CustomLinkInfo IsNot Nothing _
    '                AndAlso CustomLinkInfo.Position <= StartPos Then
    '            '   yes--move selection start to beginning of link
    '            StartPos = CustomLinkInfo.Position
    '            If IsHyphenHere(StartPos - 1) Then
    '                StartPos -= 1 'include preceding hyphen
    '            End If
    '        End If
    '        '   does the selection end on a link?
    '        CustomLinkInfo = GetCustomLink(-EndPos) 'look backward
    '        If CustomLinkInfo IsNot Nothing _
    '                AndAlso GetEndOfCustomLink(CustomLinkInfo) > EndPos Then
    '            '   yes--move selection end to end of link
    '            EndPos = GetEndOfCustomLink(CustomLinkInfo)
    '        ElseIf AreWeBeforeACustomLink(EndPos - 1) Then
    '            '   we're ending on the hyphen preceding a link--include link
    '            EndPos = GetEndOfCustomLink(CustomLinks(EndPos))
    '        End If
    '    End If
    '    '   update selection
    '    If StartPos <> rtb.SelectionStart _
    '            OrElse EndPos <> rtb.SelectionStart + rtb.SelectionLength Then
    '        rtb.Select(StartPos, EndPos - StartPos)
    '    End If
    '    UpdatingCustomLinks = False
    'End Sub

    '   Win32 link procedures
    Private Sub SetSelectionStyle(ByVal mask As UInt32, ByVal effect As UInt32)
        '   modify current selection style
        Dim cf As New CHARFORMAT2_STRUCT()
        cf.cbSize = CType(Marshal.SizeOf(cf), System.UInt32)
        cf.dwMask = mask : cf.dwEffects = effect
        Dim wpar As New IntPtr(SCF_SELECTION)
        Dim lpar As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf))
        Marshal.StructureToPtr(cf, lpar, False)
        Dim res As IntPtr = SendMessage(rtb.Handle, EM_SETCHARFORMAT, wpar, lpar)
        Marshal.FreeCoTaskMem(lpar)
    End Sub

    Private Function IsSelectionStyleInEffect(
        ByVal mask As UInt32, ByVal effect As UInt32) As Boolean
        '   test for given selection style
        Dim cf As New CHARFORMAT2_STRUCT()
        cf.cbSize = CType(Marshal.SizeOf(cf), System.UInt32) : cf.szFaceName = New Char(31) {}
        Dim wpar As New IntPtr(SCF_SELECTION)
        Dim lpar As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf))
        Marshal.StructureToPtr(cf, lpar, False)
        Dim res As IntPtr = SendMessage(rtb.Handle, EM_GETCHARFORMAT, wpar, lpar)
        cf = DirectCast(Marshal.PtrToStructure(lpar,
            GetType(CHARFORMAT2_STRUCT)), CHARFORMAT2_STRUCT)
        Dim state As Boolean = False
        '   dwMask holds the information for which
        '   properties are consistent throughout the selection:
        If (cf.dwMask And mask) = mask Then
            state =
                (cf.dwEffects And effect) = effect
        Else
            state = False
        End If
        Marshal.FreeCoTaskMem(lpar)
        Return state
    End Function

#End Region

#Region "PROTECTED_METHODS"

    '      Overrides for class-level events
    Protected Overrides Sub OnDragDrop(drgevent As DragEventArgs)
        ''ReprotectCustomLinks 'make sure any custom links are protected
        MyBase.OnDragDrop(drgevent) 'do drop
    End Sub

    '      Native event-handling methods
    Protected Overridable Sub OnInsertRtfText(e As InsertRtfTextEventArgs)
        RaiseEvent InsertRtfText(Me, e)
    End Sub

    Protected Overridable Sub OnSmartRtfText(e As SmartRtfTextEventArgs)
        RaiseEvent SmartRtfText(Me, e)
    End Sub

    Protected Overridable Sub OnChangesMade(e As EventArgs)
        RaiseEvent ChangesMade(Me, e)
    End Sub

    Protected Overridable Sub OnTextProtected(e As CancelEventArgs)
        RaiseEvent TextProtected(Me, e)
    End Sub

    Protected Overridable Sub OnHyperlinkClicked(e As CustomLinkInfoEventArgs)
        RaiseEvent HyperlinkClicked(Me, e)
    End Sub

    Protected Overridable Sub OnEditingWithLinksUnprotected(e As ParameterEventArgs)
        RaiseEvent EditingWithLinksUnprotected(Me, e)
    End Sub

#End Region

#Region "PUBLIC COMPONENTS SPECIFIC TO THIS USERCONTROL"

#Region "Events"

    ''' <summary>
    ''' InsertRtfText event --
    '''    allows user to specify special text (in RTF format)--for specific key sequences--
    '''    to insert at caret in place of any selected text
    ''' </summary>
    ''' <param name="sender">RichTextBoxEx instance</param>
    ''' <param name="e">InsertRtfTextEventArgs instance
    '''    (INPUT: e.KeyEventArgs = keyboard information
    '''        from underlying (internal) KeyDown event;
    '''     OUTPUT: e.RtfText = RTF String to insert (null for no change)</param>
    ''' <remarks>e.RtfText is expected to be a String in RICH-TEXT-FORMAT,
    ''' not "plain text"!</remarks>
    Public Event InsertRtfText(sender As Object, e As InsertRtfTextEventArgs)

    ''' <summary>
    ''' SmartRtfText event --
    '''    allows user to specify text (in RTF format) to replace the incoming character
    '''    being typed in (and optionally preceding characters) at caret inplace
    '''    of any selected type 
    ''' </summary>
    ''' <param name="sender">RichTextBoxEx instance</param>
    ''' <param name="e">SmartRtfTextEventArgs instance
    '''    (INPUT: e.KeyPressEventArgs = incoming keyboard character from underlying (internal)
    '''        KeyPress event;
    '''     OUTPUT: e.RtfText = RTF String to replace text with (null for no change),
    '''        e.PrecedingCharacterCount = number of preceding characters
    '''           to remove before inserting e.RtfText</param>
    Public Event SmartRtfText(sender As Object, e As SmartRtfTextEventArgs)

    ''' <summary>
    ''' ChangesMade event --
    '''    tracks when a change has been made by the user or programatically
    '''    (unlike underlying text box's TextChanged event, ignores "temporary" edits
    '''     [that reverse themselves] during a bigger edit operation of this control)
    ''' </summary>
    ''' <param name="sender">RichTextBoxEx instance</param>
    ''' <param name="e">EventArgs instance (no parameters)</param>
    ''' <remarks>If this event fires, then the IsTextChanged property is True</remarks>
    Public Event ChangesMade(sender As Object, e As EventArgs)

    ''' <summary>
    ''' TextProtected event --
    '''    tracks when an attempt is made to edit wholly or partially protected text
    '''    (unlike underlying text box's Protected events, handles attempts to remove
    '''     local links, and otherwise gives programmer ability to supress usual error
    '''     warning after handling event)
    ''' </summary>
    ''' <param name="sender">RichTextBoxEx instance</param>
    ''' <param name="e">CancelEventArgs instance
    '''    (OUTPUT: e.Cancel = True to pre-empt default warning after event,
    '''        False (default) to not)</param>
    Public Event TextProtected(sender As Object, e As CancelEventArgs)

    ''' <summary>
    ''' HyperlinkClicked event --
    '''    tracks when user clicks on a link or custom link; designed to handle custom links
    '''    (unlike underlying text box's LinkClicked event, handles text/hyperlink of custom
    '''     links, and distinguishes between multiple adjacent custom links)
    ''' </summary>
    ''' <param name="sender">RichTextBoxEx instance</param>
    ''' <param name="e">CustomLinkInfoEventArgs istance
    '''     (OUTPUT: e.CustomLinkInfo instance, with
    '''         e.CustomLinkInfo.Text = (visible) text,
    '''         e.CustomLinkInfo.Hyperlink = (invisible) hyperlink)</param>
    ''' <remarks>If the link is a standard (non-custom) RTB link, then
    ''' e.CustomLinkInfo.Hyperlink = e.CustomLinkInfo.Text</remarks>
    Public Event HyperlinkClicked(sender As Object, e As CustomLinkInfoEventArgs)

    ''' <summary>
    ''' EditingWithLinksUnprotected event --
    '''    allows programmer to change regions of text containing custom links
    '''    (i.e., formatting changes); temporarily unprotects links and disables
    '''    auto-protection of links, then re-protects links and re-enables auto-protection
    '''    upon completion
    ''' </summary>
    ''' <param name="sender">RichTextBox instances</param>
    ''' <param name="e">ParameterEventArgs instance
    '''     (INPUT/OUTPUT: e.Parameters = information to pass to and receive from
    '''          this event)</param>
    Public Event EditingWithLinksUnprotected(sender As Object, e As ParameterEventArgs)

#End Region

#Region "Constructor"
    ''' <summary>
    ''' initialize components and search information
    ''' </summary>
    Public Sub New()
        InitializeComponent()
        '   flag that there are no changes yet
        HaveChangedText = False : rtb.Modified = False
        MaxTextWidth = rtb.GetMaximumWidth()
        '   initialize search criteria
        SearchInfo = New SearchCriteria("", "", RichTextBoxFinds.None, True)
        LinkTooltip = New ToolTip()
        LinkTooltip.ShowAlways = True : LinkTooltip.SetToolTip(rtb, "")
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
    End Sub

#End Region

#Region "Properties"

    ''' <summary>
    ''' Get whether an automatic drag-and-drop is in progress
    ''' </summary>
    ''' <returns>True if internal text box is receiving a DragDrop event
    ''' (only possible if drop is auto-drop or if underlying text box's
    ''' AllowDrop property is set to True), otherwise False</returns>
    ''' <remarks>This property can be used by the outer class's DragDrop
    ''' event procedure to determine if the rich-text box should handle the
    ''' drop internally rather than imposing custom logic. To prevent the underlying
    ''' text box's default drop logic from occurring, the outer class's DragDrop
    ''' procedure should set e.Effects to DragDropEffects.None. (If custom logic
    ''' is not bypassed AND e.Effects is NOT set to DragDropEffects.None, then BOTH
    ''' the custom and default drop logic occur!)</remarks>
    <Browsable(False)>
    Public ReadOnly Property AutoDragDropInProgress() As Boolean
        Get
            Return IsAutoDragDropInProgress
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets whether to text has been modified
    ''' </summary>
    ''' <value>True to flag text as modified (ChangesMade event is fired),
    ''' False (default) to flag it as unmodified</value>
    ''' <returns>True if modified (or flagged as modified), False if not</returns>
    <Browsable(False)>
    Public Property IsTextChanged() As Boolean
        Get
            Return HaveChangedText
        End Get
        Set(value As Boolean)
            '   set modification status and raise ChangesMade event if necessary
            HaveChangedText = value : rtb.Modified = value  'UpdateToolbar()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether to allow user to specify color with font
    ''' when invoking the Font Dialog
    ''' </summary>
    ''' <value>True (default) for setting color and font together, False for just font</value>
    ''' <returns>True for setting color and font together, False for just font</returns>
    ''' <remarks>Setting this property to True still allows one to invoke
    ''' the Color Dialog to set only color</remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Allow Or disallow color to be specified in Font Dialog")>
    Public Property SetColorWithFont() As Boolean
        Get
            Return SettingColorWithFont
        End Get
        Set(value As Boolean)
            SettingColorWithFont = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether to show ToolStrip above RichTextBox
    ''' </summary>
    ''' <value>True (default) to show ToolStrip, False to hide it</value>
    ''' <returns>True if showing ToolStrip, False if not</returns>
    ''' <remarks>All the shortcut keys and special-character options
    ''' still work if ToolStrip is hidden</remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Show Or hide toolstrip bar")>
    Public Property ShowToolStrip() As Boolean
        Get
            Return ShowingToolStrip
        End Get
        Set(value As Boolean)
            '   invoke VisibleChanged event for ToolStrip
            ShowingToolStrip = value : ToolStrip1.Visible = ShowingToolStrip
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether to show TextRuler above RichTextBox
    ''' </summary>
    ''' <value>True (default) to show TextRuler, False to hide it</value>
    ''' <returns>True if showing TextRuler, False if not</returns>

    '<Browsable(True), Category("Behavior")>
    '<Description("Show Or hide ruler bar")>
    'Public Property ShowRuler() As Boolean
    '    Get
    '        Return ShowingRuler
    '    End Get
    '    Set(value As Boolean)
    '        '   invoke VisibleChanged event for TextRuler
    '        ShowingRuler = value : TextRuler1.Visible = ShowingRuler
    '    End Set
    'End Property

    ''' <summary>
    ''' Gets or sets whether to allow arbitrary text to be used as links
    ''' </summary>
    ''' <value>True to allow custom links, False (default) to disallow</value>
    ''' <returns>True if allowing custom links, False if not</returns>
    ''' <remarks>
    ''' 1. Setting this property to True sets the DetectURLs property of
    '''    the underlying text box to False, and DetectURLs should remain false
    '''    while this property is True, in order to preserve link formatting
    '''    when adjacent text is modified
    ''' 2. While this property is True, the LinkClicked event of the underlying
    '''    text box returns both the main text and the hypertext in the form {text|hyperlink},
    '''    and the HyperlinkClicked event in THIS control returns
    '''    the text and hyperlink of the link in a CustomLinkInfo instance
    '''    within CustomLinkInfoEventArgs</remarks>
    '<Browsable(True), Category("Behavior")>
    '<Description("Enable Or disable making links with arbitrary custom text")>
    'Public Property DoCustomLinks() As Boolean
    '    Get
    '        Return DoingCustomLinks
    '    End Get
    '    Set(value As Boolean)
    '        '   turn on or off custom hyperlinking
    '        If value = DoingCustomLinks Then
    '            Exit Property 'value not changed
    '        End If
    '        '   changing value
    '        DoingCustomLinks = value : CurrentCustomLink = Nothing
    '        HyperlinksToolStripMenuItem.Enabled = value
    '        HyperlinksToolStripMenuItem.Visible = value
    '        InsertEditRemoveHyperlinkToolStripMenuItem.Enabled = value
    '        InsertEditRemoveHyperlinkToolStripMenuItem.Visible = value
    '        RemoveAllHyperlinksToolStripMenuItem.Enabled = value
    '        RemoveAllHyperlinksToolStripMenuItem.Visible = value
    '        ToolStripSeparator9.Visible = value
    '        KeepHypertextWhenRemovingLinksToolStripMenuItem.Enabled = value
    '        KeepHypertextWhenRemovingLinksToolStripMenuItem.Visible = value
    '        tssList.Visible = value
    '        HyperlinkToolstripButton.Visible = value
    '        HyperlinkToolstripButton.Enabled = value
    '        SettingLinkMode = True 'prevent event cascade
    '        If value Then
    '            '   turn on custom-link mode and find any custom-link-qualifying protected text
    '            rtb.DetectUrls = False : GetAllCustomLinks()
    '        Else
    '            '   remove custom links and turn off custom-link mode
    '            RemoveCustomLinksInSelection(True) : rtb.DetectUrls = True
    '            Me.KeepHypertextOnRemove = False
    '        End If
    '        SettingLinkMode = False : rtb_SelectionChanged(rtb, New EventArgs())
    '    End Set
    'End Property

    ''' <summary>
    ''' Gets or sets whether to retain hyperlink text in document when
    ''' removing custom links, or just return main text
    ''' </summary>
    ''' <value>True to allow keep hyperlinks, False (default) to remove</value>
    ''' <returns>True if keeping links, False if not</returns>
    ''' <remarks>If the DoCustomLinks property is False, then 
    ''' this property is always False and attempts to set it to True are ignored</remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Get Or set whether to preserve hypertext when removing custom link")>
    Public Property KeepHypertextOnRemove() As Boolean
        Get
            Return KeepingHypertextOnRemove
        End Get
        Set(value As Boolean)
            If DoingCustomLinks Then
                KeepingHypertextOnRemove = value
                KeepHypertextWhenRemovingLinksToolStripMenuItem.Checked = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether tabs are supported
    ''' </summary>
    ''' <value>True (default) to allow tabs, False to disallow then</value>
    ''' <returns>True if tab marking is enabled, else False</returns>
    ''' <remarks>ShowRuler property must be True, as tabs must be set using ruler</remarks>
    '<Browsable(True), Category("Behavior")>
    '<Description("Enable Or disable spell-checking")>
    'Public Property AllowTabs() As Boolean
    '    Get
    '        Return AllowingTabs
    '    End Get
    '    Set(value As Boolean)
    '        '   show/hide spell-check ToolStrip and MenuStrip items
    '        AllowingTabs = value : TextRuler1.TabsEnabled = value
    '    End Set
    'End Property

    ''' <summary>
    ''' Gets or sets whether spell checking is available
    ''' </summary>
    ''' <value>True (default) to enable spell checking, False to disable it</value>
    ''' <returns>True if spell checking is enabled, else False</returns>
    ''' <remarks></remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable spell-checking")>
    Public Property AllowSpellCheck() As Boolean
        Get
            Return AllowingSpellCheck
        End Get
        Set(value As Boolean)
            '   show/hide spell-check ToolStrip and MenuStrip items
            AllowingSpellCheck = value
            SpellcheckToolStripButton.Visible = AllowingSpellCheck
            SpellCheckToolStripMenuItem.Visible = AllowingSpellCheck
            SpellCheckToolStripMenuItem.Enabled = AllowingSpellCheck
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether default custom-text insertions are allowed
    ''' </summary>
    ''' <value>True (default) to enable default custom text, False to disable it</value>
    ''' <returns>True if enabling custom-insert text, else False</returns>
    ''' <remarks></remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable default custom text")>
    Public Property AllowDefaultInsertText() As Boolean
        Get
            Return AllowingDefaultInsertText
        End Get
        Set(value As Boolean)
            AllowingDefaultInsertText = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether default smart-text replacements are allowed
    ''' </summary>
    ''' <value>True (default) to enable default smart text, False to disable it</value>
    ''' <returns>True if enabling smart-text replacement, else False</returns>
    ''' <remarks></remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable default smart text")>
    Public Property AllowDefaultSmartText() As Boolean
        Get
            Return AllowingDefaultSmartText
        End Get
        Set(value As Boolean)
            AllowingDefaultSmartText = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether listing is available
    ''' </summary>
    ''' <value>True (default) to enable lists, False to disable it</value>
    ''' <returns>True if listting is enabled, else False</returns>
    ''' <remarks></remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable listing")>
    Public Property AllowLists() As Boolean
        Get
            Return AllowingLists
        End Get
        Set(value As Boolean)
            '   show/hide spell-check ToolStrip and MenuStrip items
            AllowingLists = value
            ListToolStripDropDownButton.Visible = AllowingLists
            ListToolStripDropDownButton.Enabled = AllowingLists
            ListToolStripMenuItem.Visible = AllowingLists
            ListToolStripMenuItem.Enabled = AllowingLists
            tssList.Visible = AllowingLists
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether picture-inserting is available
    ''' </summary>
    ''' <value>True (default) to enable picture insertion, False to disable it</value>
    ''' <returns>True if picture insertion is enabled, else False</returns>
    ''' <remarks></remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable inserting of pictures")>
    Public Property AllowPictures() As Boolean
        Get
            Return AllowingPictures
        End Get
        Set(value As Boolean)
            '   show/hide InsertPicture ToolStrip and MenuStrip items
            AllowingPictures = value
            InsertPictureToolStripButton.Visible = AllowingPictures
            InsertStripMenuItem.Visible = AllowingPictures
            InsertStripMenuItem.Enabled = AllowingPictures
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether smybol-inserting is available
    ''' </summary>
    ''' <value>True (default) to enable smybol insertion, False to disable it</value>
    ''' <returns>True if symbol insertion is enabled, else False</returns>
    ''' <remarks></remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable inserting of symbols")>
    Public Property AllowSymbols() As Boolean
        Get
            Return AllowingSymbols
        End Get
        Set(value As Boolean)
            '   show/hide spell-check ToolStrip and MenuStrip items
            AllowingSymbols = value
            InsertSymbolToolStripButton.Visible = AllowingSymbols
            InsertSymbolToolStripMenuItem.Visible = AllowingSymbols
            InsertSymbolToolStripMenuItem.Enabled = AllowingSymbols
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether hyphenation-searching is available
    ''' </summary>
    ''' <value>True (default) to enable hyphenation-searching, False to disable it</value>
    ''' <returns>True if hyphenation-searching is enabled, else False</returns>
    ''' <remarks></remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable searching for words to hyphenate")>
    Public Property AllowHyphenation() As Boolean
        Get
            Return AllowingHyphenation
        End Get
        Set(value As Boolean)
            '   show/hide spell-check ToolStrip and MenuStrip items
            AllowingHyphenation = value
            HyphenateToolStripButton.Visible = AllowingHyphenation
            HyphenateToolStripMenuItem.Visible = AllowingHyphenation
            HyphenateToolStripMenuItem.Enabled = AllowingHyphenation
            RemoveAllHyphensToolStripMenuItem1.Enabled = AllowingHyphenation
            RemoveHiddenHyphensOnlyToolStripMenuItem1.Enabled = AllowingHyphenation
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether selection of selected text is
    ''' restored after Find, Replace, Hyphenation, Spell-Check, or
    ''' removal of all custom links dialogs in a region
    ''' </summary>
    ''' <value>True (default) to enable re-selection, False to disable it</value>
    ''' <returns>True if re-selection is enabled, else False</returns>
    ''' <remarks></remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable restoring text-selection after dialogs")>
    Public Property MaintainSelection() As Boolean
        Get
            Return MaintainingSelection
        End Get
        Set(value As Boolean)
            MaintainingSelection = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets units used for ruler
    ''' </summary>
    ''' <value>TextRuler.UnitType.Inches or TextRuler.UnitType.Centimeters</value>
    ''' <returns>TextRuler.UnitType.Inches or TextRuler.UnitType.Centimeters</returns>
    '<Browsable(True), Category("Appearance")>
    '<Description("Get Or set ruler unit type")>
    'Public Property UnitsForRuler() As TextRuler.UnitType
    '    Get
    '        Return TextRuler1.Units
    '    End Get
    '    Set(value As TextRuler.UnitType)
    '        TextRuler1.Units = value  'GetHorizontalInfo()
    '    End Set
    'End Property

    ''' <summary>
    ''' Gets or set maximum width of all lines of text in pixels
    ''' </summary>
    ''' <value>Text width in pixels</value>
    ''' <returns>Text width in pixels</returns>
    ''' <remarks>This is the same as the underlying rtb.RightMargin value,
    ''' except that setting it IMMEDIATELY affects the rich-text box and ruler</remarks>
    <Browsable(True), Category("Behavior")>
    <Description("Get Or set the width of the printable area in pixels")>
    Public Property RightMargin() As Integer
        Get
            Return rtb.RightMargin
        End Get
        Set(value As Integer)
            rtb.RightMargin = value  'GetHorizontalInfo()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets plain text in text box
    ''' </summary>
    ''' <value>New plain Text (ChangesMade event is fired if a change
    ''' in existing text or formatting occurs)</value>
    ''' <returns>Existing plain Text</returns>
    ''' <remarks>This is the same as the underlying rtb.Text value,
    ''' except that setting always marks the text as modified</remarks>
    <Browsable(True), Category("Data")>
    <Description("Get Or set plain text of rich-text box")>
    Public Overrides Property Text() As String
        Get
            Return rtb.Text
        End Get
        Set(NewText As String)
            rtb.Text = NewText : rtb.Modified = True
            'UpdateToolbar()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets RTF text in text box
    ''' </summary>
    ''' <value>New RTF Text (ChangesMade event is fired if different from existing)</value>
    ''' <returns>Existing RTF Text</returns>
    ''' <remarks>This is the same as the underlying rtb.Rtf value</remarks>
    <Browsable(True), Category("Data")>
    <Description("Get Or set RTF text of rich-text box")>
    Public Property Rtf() As String
        Get
            Return rtb.Rtf
        End Get
        Set(NewRtfText As String)
            If NewRtfText <> rtb.Rtf Then
                rtb.Rtf = NewRtfText  'UpdateToolbar()
            End If
        End Set
    End Property

    '''<summary>
    ''' Gets or sets directory for any text and pictures saved for loaded
    ''' </summary>
    ''' <value>New file path</value>
    ''' <returns>Existing file path</returns>
    <Browsable(True), Category("Data")>
    <Description("Get Or set path for files")>
    Public Property FilePath() As String
        Get
            Return PathForAnyFiles
        End Get
        Set(NewPath As String)
            PathForAnyFiles = NewPath
        End Set
    End Property

#Region "User Properties"
    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable searching")>
    Public Property AllowSearch() As Boolean
        Get
            Return AllowingSearch
        End Get
        Set(value As Boolean)
            '   show/hide Searc ToolStrip and MenuStrip items
            AllowingSearch = value
            FindToolStripButton.Visible = AllowingSearch
            FindNextToolStripButton.Visible = AllowingSearch
            tssSearch.Visible = AllowingSearch
        End Set
    End Property

    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable choosing Font color")>
    Public Property AllowFontColor() As Boolean
        Get
            Return AllowingFontColor
        End Get
        Set(value As Boolean)
            '   show/hide FontColor ToolStrip and MenuStrip items
            AllowingFontColor = value
            FontColorToolStripButton.Visible = AllowingFontColor
            tssFontColor.Visible = AllowingFontColor
        End Set
    End Property

    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable choosing Back color")>
    Public Property AllowBackColor() As Boolean
        Get
            Return AllowingBackColor
        End Get
        Set(value As Boolean)
            '   show/hide BackColor ToolStrip and MenuStrip items
            AllowingBackColor = value
            BackColorToolStripButton.Visible = AllowingBackColor
            tssFontColor.Visible = AllowingBackColor
        End Set
    End Property

    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable choosing Font")>
    Public Property AllowFont() As Boolean
        Get
            Return AllowingFont
        End Get
        Set(value As Boolean)
            '   show/hide Font ToolStrip and MenuStrip items
            AllowingFont = value
            FontToolStripButton.Visible = AllowingFont
            FontNameToolStripComboBox.Visible = AllowingFont
        End Set
    End Property

    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable choosing FontSize")>
    Public Property AllowFontSize() As Boolean
        Get
            Return AllowingFontSize
        End Get
        Set(value As Boolean)
            '   show/hide FontSize ToolStrip and MenuStrip items
            AllowingFontSize = value
            FontSizeToolStripComboBox.Visible = AllowingFontSize
        End Set
    End Property

    <Browsable(True), Category("Behavior")>
    <Description("Enable Or disable choosing FontSize")>
    Public Property AllowAlignment() As Boolean
        Get
            Return AllowingAlignment
        End Get
        Set(value As Boolean)
            '   show/hide Alignment ToolStrip and MenuStrip items
            AllowingAlignment = value
            LeftToolStripButton.Visible = AllowingAlignment
            CenterToolStripButton.Visible = AllowingAlignment
            RightToolStripButton.Visible = AllowingAlignment
            tssAlignment.Visible = AllowingAlignment
        End Set
    End Property

#End Region

#End Region

#Region "Methods"

    ''' <summary>
    ''' Save contents of the rich-text box
    ''' </summary>
    ''' <param name="FileName">Name of RTF/plain-text file
    ''' (if null or omitted, then a file dialog is invoked)</param>
    ''' <param name="Format">Format of file (user-specified if -1 or omitted)</param>
    ''' <returns>False if dialog was canceled, True if text was saved</returns>
    ''' <remarks>When saving as plain text, any custom
    ''' links take the form "{text|hyperlink}"</remarks>
    Public Overridable Function SaveFile(
        Optional ByVal FileName As String = "",
        Optional ByVal Format As RichTextBoxStreamType = NoSpecificFileFormat) As Boolean
        If String.IsNullOrEmpty(FileName) Then
            '   no file specified? then invoke file dialog
            Dim sfd As SaveFileDialog = New SaveFileDialog()
            With sfd
                '   initialize file dialog
                .Title = "Save Text File"
                .CheckPathExists = True : .OverwritePrompt = True
                SetFileFormatFilters(sfd, Format)
                .FilterIndex = 1 : .InitialDirectory = PathForAnyFiles
                .RestoreDirectory = True : .ShowHelp = False
                '   show dialog and get file
                If .ShowDialog() = DialogResult.Cancel Then
                    Return False
                Else
                    FileName = .FileName
                End If
            End With
        End If
        '   load RTF text into rich text-box
        Format = GetFileFormat(FileName, Format)
        rtb.SaveFile(FileName, Format) : Me.IsTextChanged = False
        Return True
    End Function

    ''' <summary>
    ''' Load a file into the rich-text box
    ''' </summary>
    ''' <param name="FileName">Name of RTF/plain-text file
    ''' (if null or omitted, then a file dialog is invoked)</param>
    ''' <param name="Format">Format of file (user-specified if -1 or omitted)</param>
    ''' <returns>False if dialog was canceled, True if text was loaded</returns>
    Public Overridable Function LoadFile(
        Optional ByVal FileName As String = "",
        Optional ByVal Format As RichTextBoxStreamType = NoSpecificFileFormat) As Boolean
        If String.IsNullOrEmpty(FileName) Then
            '   no file specified? then invoke file dialog
            Dim ofd As OpenFileDialog = New OpenFileDialog()
            With ofd
                '   initialize file dialog
                .Title = "Open Text File"
                .CheckPathExists = True : .CheckFileExists = True
                SetFileFormatFilters(ofd, Format)
                .FilterIndex = 1 : .InitialDirectory = PathForAnyFiles
                .RestoreDirectory = True : .ShowHelp = False
                '   show dialog and get file
                If .ShowDialog() = DialogResult.Cancel Then
                    Return False
                Else
                    FileName = .FileName
                End If
            End With
        End If
        '   load RTF text into rich text-box
        Format = GetFileFormat(FileName, Format) : rtb.LoadFile(FileName, Format)
        'UpdateToolbar() : Me.IsTextChanged = False
        Return True
    End Function

    ''' <summary>
    ''' pastes a picture into the rich-text box at where text is selected
    ''' </summary>
    ''' <param name="FileName">Name of picture file
    ''' (if null or omitted, then a file dialog is invoked)</param>
    ''' <returns>False if dialog was canceled,
    ''' True (and ChangesMade event is fired) if picture was inserted</returns>
    Public Overridable Function InsertPicture(Optional ByVal FileName As String = "") As Boolean
        If String.IsNullOrEmpty(FileName) Then
            '   no file specified? then invoke file dialog
            Dim ofd As OpenFileDialog = New OpenFileDialog()
            With ofd
                '   initialize file dialog
                .DefaultExt = "bmp" : .Title = "Insert Picture"
                .CheckPathExists = True : .CheckFileExists = True
                .Filter =
                    "Picture files (*.bmp; *.gif; *.jpeg; *.jpg; *.png; *.tif; *.tiff)" _
                    & "|*.bmp;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff|" _
                    & "All files (*.*)|*.*"
                .FilterIndex = 1 : .InitialDirectory = PathForAnyFiles
                .RestoreDirectory = True : .ShowHelp = False
                '   show dialog and get file
                If .ShowDialog() = DialogResult.Cancel Then
                    Return False
                Else
                    FileName = .FileName
                End If
            End With
        End If
        '   paste picture into rich text-box
        Using Bmp As New Bitmap(FileName)
            Clipboard.SetImage(Bmp) : rtb.Paste()
        End Using
        Me.IsTextChanged = IsNotProtected : IsNotProtected = True
        Return True
    End Function


    ''' <summary>
    ''' find out if a link is at the given position
    ''' </summary>
    ''' <param name="position">position to check for link
    ''' (current caret position if ommitted or negative)</param>
    ''' <param name="AreLookingBackwards">True to look backwards from position,
    ''' False (default if omitted) to look forwards from position</param>
    ''' <returns>CustomLinkInfo instance with starting position, text, and hyperlink</returns>
    ''' <remarms>NOTES:
    ''' 1. An exception is thrown if the custom links are disallowed (if the
    '''    DoCustomLinks property is False)
    ''' 2. If no link is present at the given position, then Nothing is returned</remarms>
    'Public Function CheckForCustomLink(Optional ByVal position As Integer = -1,
    '    Optional ByVal AreLookingBackwards As Boolean = False) As CustomLinkInfo
    '    '   make sure we can do this
    '    If Not DoingCustomLinks Then
    '        Throw New InvalidOperationException("Custom links are not enabled!")
    '    End If
    '    '   look for link
    '    If position < 0 OrElse position >= rtb.TextLength Then
    '        position = rtb.SelectionStart 'default to caret
    '    End If
    '    If AreLookingBackwards Then
    '        position = -position 'going backwards
    '    End If
    '    Return _
    '        GetCustomLink(position, True)
    'End Function

    ''' <summary>
    ''' add link with given information
    ''' </summary>
    ''' <param name="CustomLinkInfo">Information about insertion position,
    ''' text, and hyperlink</param>
    ''' <returns>True if added, False if not</returns>
    ''' <remarks>>NOTES:
    ''' 1. An exception is thrown if the custom links are disallowed
    '''    (if the DoCustomLinks property is False),
    '''    or if CustomLinkInfo contains invalid data
    ''' 2. If a link is successfully added, then 
    '''    a. the caret moves to the end of the link, and
    '''    b. a "hidden" syllable hyphen is prepended the link text/hyperlink info,
    '''       CustomLinkInfo.Position is incremented by 1</remarks>
    'Public Function AddCustomLink(ByRef CustomLinkInfo As CustomLinkInfo) As Boolean
    '    With CustomLinkInfo
    '        '   make sure we can do this
    '        If Not DoingCustomLinks Then
    '            Throw New InvalidOperationException("Custom links are not enabled!")
    '        End If
    '        If CustomLinkInfo Is Nothing _
    '                OrElse .Position < 0 _
    '                OrElse .Position > rtb.TextLength _
    '                OrElse String.IsNullOrEmpty(.Text) _
    '                OrElse Regex.IsMatch(.Text, "[\{\|\}]") _
    '                OrElse String.IsNullOrEmpty(.Hyperlink) _
    '                OrElse Regex.IsMatch(.Hyperlink, "[\{\|\}]") Then
    '            Throw New ArgumentException("Invalid link information!")
    '        End If
    '        '   insert link
    '        GetRange()
    '        Dim IsSuccessful As Boolean = InsertCustomLink(CustomLinkInfo)
    '        If IsSuccessful Then
    '            '   if we did it, move caret to end of new link
    '            StartPos = rtb.SelectionStart : EndPos = StartPos
    '        End If
    '        '   report result
    '        RestoreRange()
    '        Return IsSuccessful
    '    End With
    'End Function

    ''' <summary>
    ''' remove link at a given position
    ''' </summary>
    ''' <param name="position">Starting position of link</param>
    ''' <returns>True if link removed, False if not</returns>
    ''' <remarms>>NOTES:
    ''' 1. An exception is thrown if the custom links are disallowed
    '''   (if the DoCustomLinks property is False), or if position is invalid
    ''' 2. If link is successfully removed, then caret move to end of de-linked text
    ''' 3. If the KeepHyperlinksOnRemove property is True, then the de-linked text
    '''    is of the form "{text|hyperlink}", including both main text and hyperlink text;
    '''    otherwise, it is "text", including only the main text</remarms>
    'Public Function RemoveCustomLink(ByVal position As Integer) As Boolean
    '    '   make sure we can do this
    '    If Not DoingCustomLinks Then
    '        Throw New InvalidOperationException("Custom links are not enabled!")
    '    End If
    '    If position < 0 AndAlso position >= rtb.TextLength Then
    '        Throw New ArgumentOutOfRangeException("Invalid link position!")
    '    End If
    '    '   remove link
    '    GetRange()
    '    Dim IsSuccessful As Boolean = DeleteCustomLink(position)
    '    If IsSuccessful Then
    '        '   if we did it, move caret to end of de-linked text
    '        StartPos = rtb.SelectionStart : EndPos = StartPos
    '    End If
    '    '   report result
    '    RestoreRange()
    '    Return IsSuccessful
    'End Function

#Disable Warning BC42301 ' Only one XML comment block is allowed per language element
    ''' <summary>
    ''' Get a list of all custom links currently existing in selection or document
    ''' </summary>
    ''' <param name="InSelectionOnly">True to get custom links in selected text only,
    ''' False (default) to get links in entire document</param>
    ''' <returns>Array of CustomLinkInfo instances with each
    ''' link's position, visible text, and hyperlink text</returns>
    ''' <remarks>An exeption is thrown if custom links are disallowed
    ''' (if DoCustomLinks is False)</remarks>
    'Public Function GetCustomLinks(Optional ByVal InSelectionOnly As Boolean = False) _
    '    As CustomLinkInfo()
    '    '   make sure we can do this
    '    If Not DoingCustomLinks Then
    '        Throw New InvalidOperationException("Custom links are not enabled!")
    '    End If
    '    If InSelectionOnly Then
    '        '   links in selection
    '        Return _
    '            GetCustomLinksAtPositions(rtb.SelectionStart,
    '                rtb.SelectionStart + rtb.SelectionLength)
    '    Else
    '        '   all links
    '        Return _
    '            CustomLinks.Values.ToArray
    '    End If
    'End Function

    ''' <summary>
    ''' Temporarily unprotect custom link and disable auto-protection of links,
    ''' then fire EditingWithLinksUnprotected event to allow programmatic edits to
    ''' regions containing custom links (i.e., formatting changes)
    ''' </summary>
    ''' <param name="Parameters">Information to be give to and receive from the
    ''' EditingWithLinksUnprotected event's procedure (can be omitted for no info)</param>
    ''' <remarks>NOTES:
    ''' 1. An exeption is thrown if custom links are disallowed (if DoCustomLinks is False)
    ''' 2. Links are re-protected and auto-protection is re-enabled after the event finishes,
    '''    even if the event's procedure throws an unhandled exception
    ''' 3. This method is ignored if the EditingWithLinksUnprotected event is ALREADY
    '''    being processed</remarks>
    Public Sub EditWithLinksUnprotected(Optional ByRef Parameters As Object = Nothing)
#Enable Warning BC42301 ' Only one XML comment block is allowed per language element
        '   make sure we can do this
        If Not DoingCustomLinks Then
            Throw New InvalidOperationException("Custom links are not enabled!")
        End If
        If AreProtectingLinks Then
            '   not already doing it, so do it
            Dim PEA As ParameterEventArgs = New ParameterEventArgs(Parameters)
            Try
                '   remove protection and fire event
                'SetProtectionForCustomLinks(False) : Me.OnEditingWithLinksUnprotected(PEA)
            Finally
                '   restore protection
                Parameters = PEA.Parameters  'SetProtectionForCustomLinks(True)
            End Try
        End If
    End Sub

    'Private Sub rtb_MouseClick(sender As Object, e As MouseEventArgs) Handles rtb.MouseClick

    'End Sub
End Class

#End Region

#Region "Classes"
'   CLASSES FOR INFORMATION
''' <summary>
''' Information about a link--position, visible text, and invisible hyperlink
''' </summary>
Public Class CustomLinkInfo
    Public Position As Integer 'position
    Public Text As String      'visible text
    Public Hyperlink As String 'invisible hyperlink text
End Class

'    CLASSES FOR EVENTS
''' <summary>
''' Event Arguments for InsertRtfText event--which allows one
''' to specify custom text for specific key sequences
''' </summary>
''' <remarks>If KeyEventArgs.SuppressKeyPress is True, then no change is made to text</remarks>
Public Class InsertRtfTextEventArgs
    Inherits EventArgs

    '   Constructor

    ''' <summary>
    ''' Supplies initial keyboard information
    ''' </summary>
    ''' <param name="KeyEventArgs">KeyEventArgs instance for underlying KeyDown event</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal KeyEventArgs As System.Windows.Forms.KeyEventArgs)
        Me.KeyEventArgs = KeyEventArgs
    End Sub

    '   Properties

    ''' <summary>
    ''' RTF text to be inserted
    ''' </summary>
    ''' <value>Text to be inserted in RTF format</value>
    ''' <returns>Text to be inserted in RTF format</returns>
    ''' <remarks>Defaults to null for making no change, except any default text allowed</remarks>
    Public Property RtfText As String = ""

    ''' <summary>
    ''' Keyboard information
    ''' </summary>
    ''' <value>KeyEventArgs instance from underlying KeyDown event</value>
    ''' <returns>KeyEventArgs instance from underlying KeyDown event</returns>
    ''' <remarks>Set KeyEventArgs.SuppressKeyPress to True ensure that NO text is
    ''' inserted, even any default text</remarks>
    Public Property KeyEventArgs As System.Windows.Forms.KeyEventArgs 'keyboard info
End Class

''' <summary>
''' Event Arguments for SmartRtfText event--
''' which allows one to replace a recent set of characters
''' (including an incoming one) with custom text
''' </summary>
''' <remarks>If no RTF text or length of text to replace is specified on output,
''' no change is made to text</remarks>
Public Class SmartRtfTextEventArgs
    Inherits EventArgs

    '   Constructor

    ''' <summary>
    ''' Supplies initial keyboard information
    ''' </summary>
    ''' <param name="KeyPressEventArgs">KeyPressEventArgs instance
    ''' for underlying KeyPress event</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal KeyPressEventArgs As System.Windows.Forms.KeyPressEventArgs)
        Me.KeyPressEventArgs = KeyPressEventArgs
    End Sub

    '   Properties

    ''' <summary>
    ''' Smart text to be inserted
    ''' </summary>
    ''' <value>Smart text to be inserted in RTF format</value>
    ''' <returns>Smart text to be inserted in RTF format</returns>
    ''' <remarks>Defaults to null for making no change</remarks>
    Public Property RtfText As String = ""

    ''' <summary>
    ''' Number of characters to replace prior to incoming character
    ''' </summary>
    ''' <value># of characters to replace</value>
    ''' <returns># of characters to replace</returns>
    ''' <remarks>Characters preceding the incoming character are removed
    ''' before smart text is inserted</remarks>
    Public Property PrecedingCharacterCount As Integer = 0

    ''' <summary>
    ''' Keyboard information
    ''' </summary>
    ''' <value>KeyEventArgs instance from underlying KeyPress event</value>
    ''' <returns>KeyEventArgs instance from underlying KeyPress event</returns>
    ''' <remarks>e.KeyChar is the incoming character</remarks>
    Public Property KeyPressEventArgs As System.Windows.Forms.KeyPressEventArgs 'keyboard info
End Class

''' <summary>
''' Event Arguments for HyperlinkClicked event--
''' returns CustomLinkInfo instance with position, text, and hyperlink
''' </summary>
''' <remarks>If custom links are not enabled, then
''' position is -1 and text and hyperlink are the same</remarks>
Public Class CustomLinkInfoEventArgs
    Inherits EventArgs
    '   Properties

    ''' <summary>
    ''' Information about text and hyperlink link
    ''' </summary>
    ''' <returns>.Text = (visible) text, and .Hyperlink = (invisible) hyperlink</returns>
    Public Property CustomLinkInfo As CustomLinkInfo
End Class

''' <summary>
''' Event Arguments for EditingWithLinksUnprotected event--which allows one
''' to specify information for event
''' </summary>
Public Class ParameterEventArgs
    Inherits EventArgs

    '   Constructor

    ''' <summary>
    ''' Supplies initial event information
    ''' </summary>
    ''' <param name="Parameters">Information for event</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Parameters As Object)
        Me.Parameters = Parameters
    End Sub

    '   Properties

    ''' <summary>
    ''' Information for event procedure
    ''' </summary>
    ''' <value>Information supplied to event</value>
    ''' <returns>Information returned from event</returns>
    Public Property Parameters As Object
End Class

#End Region

#End Region