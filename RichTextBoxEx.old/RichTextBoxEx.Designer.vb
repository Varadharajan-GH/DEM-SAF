<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RichTextBoxEx
    Inherits System.Windows.Forms.UserControl

    'UserControl1 overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RichTextBoxEx))
        Me.rtb = New System.Windows.Forms.RichTextBox()
        Me.RTBContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PasteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UndoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RedoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FormatToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BoldToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ItalicToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UnderlineToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StrikethroughToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GeneralFontSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AlignmentToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LeftToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CenterToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RightToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VerticalPositionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RaisedSuperscriptToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoweredSubscriptToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.SearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FindToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FindNextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReplaceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SpellCheckToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.ListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NoneContextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BulletContextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NumContextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LCAlphaContextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UCAlphaContextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LCRomanContextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UCRomanContextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InsertStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InsertPictureToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InsertSymbolToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HyphenateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HyphenateTextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RemoveAllHyphensToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.RemoveHiddenHyphensOnlyToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.HyperlinksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InsertEditRemoveHyperlinkToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RemoveAllHyperlinksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        Me.KeepHypertextWhenRemovingLinksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SpellChecker = New NetSpell.SpellChecker.Spelling(Me.components)
        Me.FontToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.FontColorToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.BoldToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ItalicToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.UnderlineToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.tssTextFormat = New System.Windows.Forms.ToolStripSeparator()
        Me.LeftToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.CenterToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.RightToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.tssSearch = New System.Windows.Forms.ToolStripSeparator()
        Me.SpellcheckToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.tssAlignment = New System.Windows.Forms.ToolStripSeparator()
        Me.FindNextToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.BackColorToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.tssFontColor = New System.Windows.Forms.ToolStripSeparator()
        Me.FontNameToolStripComboBox = New System.Windows.Forms.ToolStripComboBox()
        Me.FontSizeToolStripComboBox = New System.Windows.Forms.ToolStripComboBox()
        Me.FindToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.InsertPictureToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.InsertSymbolToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.tssSymbol = New System.Windows.Forms.ToolStripSeparator()
        Me.HyphenateToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.tssSpellCheck = New System.Windows.Forms.ToolStripSeparator()
        Me.ListToolStripDropDownButton = New System.Windows.Forms.ToolStripDropDownButton()
        Me.NoneToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BulletToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NumToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LCAlphaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UCAlphaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LCRomanToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UCRomanToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tssList = New System.Windows.Forms.ToolStripSeparator()
        Me.HyperlinkToolstripButton = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.FontDlg = New System.Windows.Forms.FontDialog()
        Me.ColorDlg = New System.Windows.Forms.ColorDialog()
        Me.RulerContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.MeasureInInchesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MeasureInCentimetersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RTBContextMenuStrip.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.RulerContextMenuStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'rtb
        '
        Me.rtb.AcceptsTab = True
        Me.rtb.ContextMenuStrip = Me.RTBContextMenuStrip
        Me.rtb.EnableAutoDragDrop = True
        Me.rtb.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtb.HideSelection = False
        Me.rtb.Location = New System.Drawing.Point(3, 28)
        Me.rtb.Name = "rtb"
        Me.rtb.ShowSelectionMargin = True
        Me.rtb.Size = New System.Drawing.Size(642, 146)
        Me.rtb.TabIndex = 0
        Me.rtb.Text = "MyText"
        Me.rtb.WordWrap = False
        '
        'RTBContextMenuStrip
        '
        Me.RTBContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EditToolStripMenuItem, Me.FormatToolStripMenuItem, Me.AlignmentToolStripMenuItem, Me.VerticalPositionToolStripMenuItem, Me.ToolStripSeparator8, Me.SearchToolStripMenuItem, Me.SpellCheckToolStripMenuItem, Me.ToolStripSeparator6, Me.ListToolStripMenuItem, Me.InsertStripMenuItem, Me.HyphenateToolStripMenuItem, Me.HyperlinksToolStripMenuItem})
        Me.RTBContextMenuStrip.Name = "ContextMenuStrip1"
        Me.RTBContextMenuStrip.Size = New System.Drawing.Size(224, 236)
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SelectAllToolStripMenuItem, Me.CopyToolStripMenuItem, Me.CutToolStripMenuItem, Me.PasteToolStripMenuItem, Me.UndoToolStripMenuItem, Me.RedoToolStripMenuItem})
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.EditToolStripMenuItem.Text = "&Edit"
        '
        'SelectAllToolStripMenuItem
        '
        Me.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
        Me.SelectAllToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
        Me.SelectAllToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.SelectAllToolStripMenuItem.Text = "Select &All"
        '
        'CopyToolStripMenuItem
        '
        Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        Me.CopyToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.CopyToolStripMenuItem.Text = "&Copy"
        '
        'CutToolStripMenuItem
        '
        Me.CutToolStripMenuItem.Name = "CutToolStripMenuItem"
        Me.CutToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys)
        Me.CutToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.CutToolStripMenuItem.Text = "C&ut"
        '
        'PasteToolStripMenuItem
        '
        Me.PasteToolStripMenuItem.Name = "PasteToolStripMenuItem"
        Me.PasteToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys)
        Me.PasteToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.PasteToolStripMenuItem.Text = "&Paste"
        '
        'UndoToolStripMenuItem
        '
        Me.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem"
        Me.UndoToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.UndoToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.UndoToolStripMenuItem.Text = "&Undo"
        '
        'RedoToolStripMenuItem
        '
        Me.RedoToolStripMenuItem.Name = "RedoToolStripMenuItem"
        Me.RedoToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Y), System.Windows.Forms.Keys)
        Me.RedoToolStripMenuItem.Size = New System.Drawing.Size(164, 22)
        Me.RedoToolStripMenuItem.Text = "&Redo"
        '
        'FormatToolStripMenuItem
        '
        Me.FormatToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BoldToolStripMenuItem, Me.ItalicToolStripMenuItem, Me.UnderlineToolStripMenuItem, Me.StrikethroughToolStripMenuItem, Me.GeneralFontSettingsToolStripMenuItem})
        Me.FormatToolStripMenuItem.Name = "FormatToolStripMenuItem"
        Me.FormatToolStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.FormatToolStripMenuItem.Text = "&Font"
        '
        'BoldToolStripMenuItem
        '
        Me.BoldToolStripMenuItem.Name = "BoldToolStripMenuItem"
        Me.BoldToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.B), System.Windows.Forms.Keys)
        Me.BoldToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
        Me.BoldToolStripMenuItem.Text = "&Bold"
        '
        'ItalicToolStripMenuItem
        '
        Me.ItalicToolStripMenuItem.Name = "ItalicToolStripMenuItem"
        Me.ItalicToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.I), System.Windows.Forms.Keys)
        Me.ItalicToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
        Me.ItalicToolStripMenuItem.Text = "&Italic"
        '
        'UnderlineToolStripMenuItem
        '
        Me.UnderlineToolStripMenuItem.Name = "UnderlineToolStripMenuItem"
        Me.UnderlineToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.U), System.Windows.Forms.Keys)
        Me.UnderlineToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
        Me.UnderlineToolStripMenuItem.Text = "&Underline"
        '
        'StrikethroughToolStripMenuItem
        '
        Me.StrikethroughToolStripMenuItem.Name = "StrikethroughToolStripMenuItem"
        Me.StrikethroughToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
        Me.StrikethroughToolStripMenuItem.Text = "&Strikethrough"
        '
        'GeneralFontSettingsToolStripMenuItem
        '
        Me.GeneralFontSettingsToolStripMenuItem.Name = "GeneralFontSettingsToolStripMenuItem"
        Me.GeneralFontSettingsToolStripMenuItem.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.F), System.Windows.Forms.Keys)
        Me.GeneralFontSettingsToolStripMenuItem.Size = New System.Drawing.Size(224, 22)
        Me.GeneralFontSettingsToolStripMenuItem.Text = "&Font Settings..."
        '
        'AlignmentToolStripMenuItem
        '
        Me.AlignmentToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LeftToolStripMenuItem, Me.CenterToolStripMenuItem, Me.RightToolStripMenuItem})
        Me.AlignmentToolStripMenuItem.Name = "AlignmentToolStripMenuItem"
        Me.AlignmentToolStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.AlignmentToolStripMenuItem.Text = "&Alignment"
        '
        'LeftToolStripMenuItem
        '
        Me.LeftToolStripMenuItem.Name = "LeftToolStripMenuItem"
        Me.LeftToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.L), System.Windows.Forms.Keys)
        Me.LeftToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.LeftToolStripMenuItem.Text = "&Left"
        '
        'CenterToolStripMenuItem
        '
        Me.CenterToolStripMenuItem.Name = "CenterToolStripMenuItem"
        Me.CenterToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.E), System.Windows.Forms.Keys)
        Me.CenterToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.CenterToolStripMenuItem.Text = "&Center"
        '
        'RightToolStripMenuItem
        '
        Me.RightToolStripMenuItem.Name = "RightToolStripMenuItem"
        Me.RightToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
        Me.RightToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.RightToolStripMenuItem.Text = "&Right"
        '
        'VerticalPositionToolStripMenuItem
        '
        Me.VerticalPositionToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RaisedSuperscriptToolStripMenuItem, Me.LoweredSubscriptToolStripMenuItem})
        Me.VerticalPositionToolStripMenuItem.Name = "VerticalPositionToolStripMenuItem"
        Me.VerticalPositionToolStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.VerticalPositionToolStripMenuItem.Text = "&Vertical Position"
        '
        'RaisedSuperscriptToolStripMenuItem
        '
        Me.RaisedSuperscriptToolStripMenuItem.Name = "RaisedSuperscriptToolStripMenuItem"
        Me.RaisedSuperscriptToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift++"
        Me.RaisedSuperscriptToolStripMenuItem.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.Oemplus), System.Windows.Forms.Keys)
        Me.RaisedSuperscriptToolStripMenuItem.Size = New System.Drawing.Size(252, 22)
        Me.RaisedSuperscriptToolStripMenuItem.Text = "&Raised (Superscript)"
        '
        'LoweredSubscriptToolStripMenuItem
        '
        Me.LoweredSubscriptToolStripMenuItem.Name = "LoweredSubscriptToolStripMenuItem"
        Me.LoweredSubscriptToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+="
        Me.LoweredSubscriptToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Oemplus), System.Windows.Forms.Keys)
        Me.LoweredSubscriptToolStripMenuItem.Size = New System.Drawing.Size(252, 22)
        Me.LoweredSubscriptToolStripMenuItem.Text = "&Lowered (Subscript)"
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(220, 6)
        '
        'SearchToolStripMenuItem
        '
        Me.SearchToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FindToolStripMenuItem, Me.FindNextToolStripMenuItem, Me.ReplaceToolStripMenuItem})
        Me.SearchToolStripMenuItem.Enabled = False
        Me.SearchToolStripMenuItem.Name = "SearchToolStripMenuItem"
        Me.SearchToolStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.SearchToolStripMenuItem.Text = "&Search"
        '
        'FindToolStripMenuItem
        '
        Me.FindToolStripMenuItem.Enabled = False
        Me.FindToolStripMenuItem.Name = "FindToolStripMenuItem"
        Me.FindToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F), System.Windows.Forms.Keys)
        Me.FindToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.FindToolStripMenuItem.Text = "&Find..."
        '
        'FindNextToolStripMenuItem
        '
        Me.FindNextToolStripMenuItem.Enabled = False
        Me.FindNextToolStripMenuItem.Name = "FindNextToolStripMenuItem"
        Me.FindNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3
        Me.FindNextToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.FindNextToolStripMenuItem.Text = "Find &Next"
        '
        'ReplaceToolStripMenuItem
        '
        Me.ReplaceToolStripMenuItem.Name = "ReplaceToolStripMenuItem"
        Me.ReplaceToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.H), System.Windows.Forms.Keys)
        Me.ReplaceToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.ReplaceToolStripMenuItem.Text = "&Replace..."
        '
        'SpellCheckToolStripMenuItem
        '
        Me.SpellCheckToolStripMenuItem.Enabled = False
        Me.SpellCheckToolStripMenuItem.Name = "SpellCheckToolStripMenuItem"
        Me.SpellCheckToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7
        Me.SpellCheckToolStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.SpellCheckToolStripMenuItem.Text = "Spell &Check..."
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(220, 6)
        '
        'ListToolStripMenuItem
        '
        Me.ListToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NoneContextToolStripMenuItem, Me.BulletContextToolStripMenuItem, Me.NumContextToolStripMenuItem, Me.LCAlphaContextToolStripMenuItem, Me.UCAlphaContextToolStripMenuItem, Me.LCRomanContextToolStripMenuItem, Me.UCRomanContextToolStripMenuItem})
        Me.ListToolStripMenuItem.Name = "ListToolStripMenuItem"
        Me.ListToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl + Shift + L"
        Me.ListToolStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.ListToolStripMenuItem.Text = "Set &List Style"
        '
        'NoneContextToolStripMenuItem
        '
        Me.NoneContextToolStripMenuItem.Name = "NoneContextToolStripMenuItem"
        Me.NoneContextToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.NoneContextToolStripMenuItem.Tag = "-1"
        Me.NoneContextToolStripMenuItem.Text = "(None)"
        '
        'BulletContextToolStripMenuItem
        '
        Me.BulletContextToolStripMenuItem.Name = "BulletContextToolStripMenuItem"
        Me.BulletContextToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.BulletContextToolStripMenuItem.Tag = "0"
        Me.BulletContextToolStripMenuItem.Text = "● (Bullets)"
        '
        'NumContextToolStripMenuItem
        '
        Me.NumContextToolStripMenuItem.Name = "NumContextToolStripMenuItem"
        Me.NumContextToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.NumContextToolStripMenuItem.Tag = "1"
        Me.NumContextToolStripMenuItem.Text = "1, 2, 3, ..."
        '
        'LCAlphaContextToolStripMenuItem
        '
        Me.LCAlphaContextToolStripMenuItem.Name = "LCAlphaContextToolStripMenuItem"
        Me.LCAlphaContextToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.LCAlphaContextToolStripMenuItem.Tag = "2"
        Me.LCAlphaContextToolStripMenuItem.Text = "a, b, c, ..."
        '
        'UCAlphaContextToolStripMenuItem
        '
        Me.UCAlphaContextToolStripMenuItem.Name = "UCAlphaContextToolStripMenuItem"
        Me.UCAlphaContextToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.UCAlphaContextToolStripMenuItem.Tag = "3"
        Me.UCAlphaContextToolStripMenuItem.Text = "A, B, C, ..."
        '
        'LCRomanContextToolStripMenuItem
        '
        Me.LCRomanContextToolStripMenuItem.Name = "LCRomanContextToolStripMenuItem"
        Me.LCRomanContextToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.LCRomanContextToolStripMenuItem.Tag = "4"
        Me.LCRomanContextToolStripMenuItem.Text = "i, ii, iii, ..."
        '
        'UCRomanContextToolStripMenuItem
        '
        Me.UCRomanContextToolStripMenuItem.Name = "UCRomanContextToolStripMenuItem"
        Me.UCRomanContextToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.UCRomanContextToolStripMenuItem.Tag = "5"
        Me.UCRomanContextToolStripMenuItem.Text = "I, II, III, ..."
        '
        'InsertStripMenuItem
        '
        Me.InsertStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InsertPictureToolStripMenuItem, Me.InsertSymbolToolStripMenuItem})
        Me.InsertStripMenuItem.Name = "InsertStripMenuItem"
        Me.InsertStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.InsertStripMenuItem.Text = "&Insert"
        '
        'InsertPictureToolStripMenuItem
        '
        Me.InsertPictureToolStripMenuItem.Name = "InsertPictureToolStripMenuItem"
        Me.InsertPictureToolStripMenuItem.Size = New System.Drawing.Size(123, 22)
        Me.InsertPictureToolStripMenuItem.Text = "&Picture..."
        '
        'InsertSymbolToolStripMenuItem
        '
        Me.InsertSymbolToolStripMenuItem.Name = "InsertSymbolToolStripMenuItem"
        Me.InsertSymbolToolStripMenuItem.Size = New System.Drawing.Size(123, 22)
        Me.InsertSymbolToolStripMenuItem.Text = "&Symbol..."
        '
        'HyphenateToolStripMenuItem
        '
        Me.HyphenateToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HyphenateTextToolStripMenuItem, Me.RemoveAllHyphensToolStripMenuItem1, Me.RemoveHiddenHyphensOnlyToolStripMenuItem1})
        Me.HyphenateToolStripMenuItem.Name = "HyphenateToolStripMenuItem"
        Me.HyphenateToolStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.HyphenateToolStripMenuItem.Text = "&Hyphenation"
        '
        'HyphenateTextToolStripMenuItem
        '
        Me.HyphenateTextToolStripMenuItem.Name = "HyphenateTextToolStripMenuItem"
        Me.HyphenateTextToolStripMenuItem.Size = New System.Drawing.Size(237, 22)
        Me.HyphenateTextToolStripMenuItem.Text = "&Hyphenate..."
        '
        'RemoveAllHyphensToolStripMenuItem1
        '
        Me.RemoveAllHyphensToolStripMenuItem1.Name = "RemoveAllHyphensToolStripMenuItem1"
        Me.RemoveAllHyphensToolStripMenuItem1.Size = New System.Drawing.Size(237, 22)
        Me.RemoveAllHyphensToolStripMenuItem1.Text = "&Remove All Hyphens"
        '
        'RemoveHiddenHyphensOnlyToolStripMenuItem1
        '
        Me.RemoveHiddenHyphensOnlyToolStripMenuItem1.Name = "RemoveHiddenHyphensOnlyToolStripMenuItem1"
        Me.RemoveHiddenHyphensOnlyToolStripMenuItem1.Size = New System.Drawing.Size(237, 22)
        Me.RemoveHiddenHyphensOnlyToolStripMenuItem1.Text = "Remove Hidden Hyphens &Only"
        '
        'HyperlinksToolStripMenuItem
        '
        Me.HyperlinksToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InsertEditRemoveHyperlinkToolStripMenuItem, Me.RemoveAllHyperlinksToolStripMenuItem, Me.ToolStripSeparator9, Me.KeepHypertextWhenRemovingLinksToolStripMenuItem})
        Me.HyperlinksToolStripMenuItem.Enabled = False
        Me.HyperlinksToolStripMenuItem.Name = "HyperlinksToolStripMenuItem"
        Me.HyperlinksToolStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.HyperlinksToolStripMenuItem.Text = "Hyperlinks"
        Me.HyperlinksToolStripMenuItem.Visible = False
        '
        'InsertEditRemoveHyperlinkToolStripMenuItem
        '
        Me.InsertEditRemoveHyperlinkToolStripMenuItem.Enabled = False
        Me.InsertEditRemoveHyperlinkToolStripMenuItem.Name = "InsertEditRemoveHyperlinkToolStripMenuItem"
        Me.InsertEditRemoveHyperlinkToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.K), System.Windows.Forms.Keys)
        Me.InsertEditRemoveHyperlinkToolStripMenuItem.Size = New System.Drawing.Size(266, 22)
        Me.InsertEditRemoveHyperlinkToolStripMenuItem.Text = "&Insert/Edit/Remove ..."
        Me.InsertEditRemoveHyperlinkToolStripMenuItem.Visible = False
        '
        'RemoveAllHyperlinksToolStripMenuItem
        '
        Me.RemoveAllHyperlinksToolStripMenuItem.Enabled = False
        Me.RemoveAllHyperlinksToolStripMenuItem.Name = "RemoveAllHyperlinksToolStripMenuItem"
        Me.RemoveAllHyperlinksToolStripMenuItem.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.F9), System.Windows.Forms.Keys)
        Me.RemoveAllHyperlinksToolStripMenuItem.Size = New System.Drawing.Size(266, 22)
        Me.RemoveAllHyperlinksToolStripMenuItem.Text = "&Remove All..."
        Me.RemoveAllHyperlinksToolStripMenuItem.Visible = False
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        Me.ToolStripSeparator9.Size = New System.Drawing.Size(263, 6)
        Me.ToolStripSeparator9.Visible = False
        '
        'KeepHypertextWhenRemovingLinksToolStripMenuItem
        '
        Me.KeepHypertextWhenRemovingLinksToolStripMenuItem.Enabled = False
        Me.KeepHypertextWhenRemovingLinksToolStripMenuItem.Name = "KeepHypertextWhenRemovingLinksToolStripMenuItem"
        Me.KeepHypertextWhenRemovingLinksToolStripMenuItem.Size = New System.Drawing.Size(266, 22)
        Me.KeepHypertextWhenRemovingLinksToolStripMenuItem.Text = "&Keep hypertext when removing links"
        Me.KeepHypertextWhenRemovingLinksToolStripMenuItem.Visible = False
        '
        'SpellChecker
        '
        Me.SpellChecker.Dictionary = Nothing
        '
        'FontToolStripButton
        '
        Me.FontToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.FontToolStripButton.Image = CType(resources.GetObject("FontToolStripButton.Image"), System.Drawing.Image)
        Me.FontToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.FontToolStripButton.Name = "FontToolStripButton"
        Me.FontToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.FontToolStripButton.Text = "Font"
        '
        'FontColorToolStripButton
        '
        Me.FontColorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.FontColorToolStripButton.Image = CType(resources.GetObject("FontColorToolStripButton.Image"), System.Drawing.Image)
        Me.FontColorToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.FontColorToolStripButton.Name = "FontColorToolStripButton"
        Me.FontColorToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.FontColorToolStripButton.Text = "Text Color"
        Me.FontColorToolStripButton.ToolTipText = "Text Color"
        '
        'BoldToolStripButton
        '
        Me.BoldToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BoldToolStripButton.Image = CType(resources.GetObject("BoldToolStripButton.Image"), System.Drawing.Image)
        Me.BoldToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.BoldToolStripButton.Name = "BoldToolStripButton"
        Me.BoldToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.BoldToolStripButton.Text = "Bold"
        '
        'ItalicToolStripButton
        '
        Me.ItalicToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ItalicToolStripButton.Image = CType(resources.GetObject("ItalicToolStripButton.Image"), System.Drawing.Image)
        Me.ItalicToolStripButton.ImageTransparentColor = System.Drawing.Color.White
        Me.ItalicToolStripButton.Name = "ItalicToolStripButton"
        Me.ItalicToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.ItalicToolStripButton.Text = "Italic"
        '
        'UnderlineToolStripButton
        '
        Me.UnderlineToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.UnderlineToolStripButton.Image = CType(resources.GetObject("UnderlineToolStripButton.Image"), System.Drawing.Image)
        Me.UnderlineToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.UnderlineToolStripButton.Name = "UnderlineToolStripButton"
        Me.UnderlineToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.UnderlineToolStripButton.Text = "Underline"
        '
        'tssTextFormat
        '
        Me.tssTextFormat.Name = "tssTextFormat"
        Me.tssTextFormat.Size = New System.Drawing.Size(6, 25)
        '
        'LeftToolStripButton
        '
        Me.LeftToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.LeftToolStripButton.Image = CType(resources.GetObject("LeftToolStripButton.Image"), System.Drawing.Image)
        Me.LeftToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.LeftToolStripButton.Name = "LeftToolStripButton"
        Me.LeftToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.LeftToolStripButton.Text = "Align Text Left"
        Me.LeftToolStripButton.ToolTipText = "Align Text Left"
        '
        'CenterToolStripButton
        '
        Me.CenterToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.CenterToolStripButton.Image = CType(resources.GetObject("CenterToolStripButton.Image"), System.Drawing.Image)
        Me.CenterToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.CenterToolStripButton.Name = "CenterToolStripButton"
        Me.CenterToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.CenterToolStripButton.Text = "Center Text"
        Me.CenterToolStripButton.ToolTipText = "Center Text"
        '
        'RightToolStripButton
        '
        Me.RightToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.RightToolStripButton.Image = CType(resources.GetObject("RightToolStripButton.Image"), System.Drawing.Image)
        Me.RightToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.RightToolStripButton.Name = "RightToolStripButton"
        Me.RightToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.RightToolStripButton.Text = "Align Text Right"
        Me.RightToolStripButton.ToolTipText = "Align Text Right"
        '
        'tssSearch
        '
        Me.tssSearch.Name = "tssSearch"
        Me.tssSearch.Size = New System.Drawing.Size(6, 25)
        '
        'SpellcheckToolStripButton
        '
        Me.SpellcheckToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.SpellcheckToolStripButton.Enabled = False
        Me.SpellcheckToolStripButton.Image = CType(resources.GetObject("SpellcheckToolStripButton.Image"), System.Drawing.Image)
        Me.SpellcheckToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.SpellcheckToolStripButton.Name = "SpellcheckToolStripButton"
        Me.SpellcheckToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.SpellcheckToolStripButton.Text = "Spell Check"
        '
        'tssAlignment
        '
        Me.tssAlignment.Name = "tssAlignment"
        Me.tssAlignment.Size = New System.Drawing.Size(6, 25)
        '
        'FindNextToolStripButton
        '
        Me.FindNextToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.FindNextToolStripButton.Enabled = False
        Me.FindNextToolStripButton.Image = CType(resources.GetObject("FindNextToolStripButton.Image"), System.Drawing.Image)
        Me.FindNextToolStripButton.ImageTransparentColor = System.Drawing.Color.Black
        Me.FindNextToolStripButton.Name = "FindNextToolStripButton"
        Me.FindNextToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.FindNextToolStripButton.Text = "Find Next"
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FontColorToolStripButton, Me.BackColorToolStripButton, Me.tssFontColor, Me.FontToolStripButton, Me.FontNameToolStripComboBox, Me.FontSizeToolStripComboBox, Me.BoldToolStripButton, Me.ItalicToolStripButton, Me.UnderlineToolStripButton, Me.tssTextFormat, Me.LeftToolStripButton, Me.CenterToolStripButton, Me.RightToolStripButton, Me.tssAlignment, Me.FindToolStripButton, Me.FindNextToolStripButton, Me.tssSearch, Me.InsertPictureToolStripButton, Me.InsertSymbolToolStripButton, Me.tssSymbol, Me.HyphenateToolStripButton, Me.SpellcheckToolStripButton, Me.tssSpellCheck, Me.ListToolStripDropDownButton, Me.tssList, Me.HyperlinkToolstripButton, Me.ToolStripSeparator2})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(650, 25)
        Me.ToolStrip1.TabIndex = 1
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'BackColorToolStripButton
        '
        Me.BackColorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BackColorToolStripButton.Image = CType(resources.GetObject("BackColorToolStripButton.Image"), System.Drawing.Image)
        Me.BackColorToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.BackColorToolStripButton.Name = "BackColorToolStripButton"
        Me.BackColorToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.BackColorToolStripButton.Text = "Background Color"
        '
        'tssFontColor
        '
        Me.tssFontColor.Name = "tssFontColor"
        Me.tssFontColor.Size = New System.Drawing.Size(6, 25)
        '
        'FontNameToolStripComboBox
        '
        Me.FontNameToolStripComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.FontNameToolStripComboBox.DropDownHeight = 182
        Me.FontNameToolStripComboBox.DropDownWidth = 155
        Me.FontNameToolStripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard
        Me.FontNameToolStripComboBox.IntegralHeight = False
        Me.FontNameToolStripComboBox.MaxDropDownItems = 9
        Me.FontNameToolStripComboBox.Name = "FontNameToolStripComboBox"
        Me.FontNameToolStripComboBox.Size = New System.Drawing.Size(121, 25)
        Me.FontNameToolStripComboBox.Sorted = True
        Me.FontNameToolStripComboBox.ToolTipText = "Font Name"
        '
        'FontSizeToolStripComboBox
        '
        Me.FontSizeToolStripComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.FontSizeToolStripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard
        Me.FontSizeToolStripComboBox.Items.AddRange(New Object() {"8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "38", "48", "72"})
        Me.FontSizeToolStripComboBox.Name = "FontSizeToolStripComboBox"
        Me.FontSizeToolStripComboBox.Size = New System.Drawing.Size(75, 25)
        Me.FontSizeToolStripComboBox.ToolTipText = "Font Size"
        '
        'FindToolStripButton
        '
        Me.FindToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.FindToolStripButton.Enabled = False
        Me.FindToolStripButton.Image = CType(resources.GetObject("FindToolStripButton.Image"), System.Drawing.Image)
        Me.FindToolStripButton.ImageTransparentColor = System.Drawing.Color.Black
        Me.FindToolStripButton.Name = "FindToolStripButton"
        Me.FindToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.FindToolStripButton.Text = "Find/Replace"
        Me.FindToolStripButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'InsertPictureToolStripButton
        '
        Me.InsertPictureToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.InsertPictureToolStripButton.Image = CType(resources.GetObject("InsertPictureToolStripButton.Image"), System.Drawing.Image)
        Me.InsertPictureToolStripButton.ImageTransparentColor = System.Drawing.Color.Black
        Me.InsertPictureToolStripButton.Name = "InsertPictureToolStripButton"
        Me.InsertPictureToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.InsertPictureToolStripButton.Text = "Insert Picture"
        Me.InsertPictureToolStripButton.ToolTipText = "Insert Picture"
        '
        'InsertSymbolToolStripButton
        '
        Me.InsertSymbolToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.InsertSymbolToolStripButton.Image = CType(resources.GetObject("InsertSymbolToolStripButton.Image"), System.Drawing.Image)
        Me.InsertSymbolToolStripButton.ImageTransparentColor = System.Drawing.Color.Black
        Me.InsertSymbolToolStripButton.Name = "InsertSymbolToolStripButton"
        Me.InsertSymbolToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.InsertSymbolToolStripButton.Text = "Insert Symbol"
        '
        'tssSymbol
        '
        Me.tssSymbol.Name = "tssSymbol"
        Me.tssSymbol.Size = New System.Drawing.Size(6, 25)
        '
        'HyphenateToolStripButton
        '
        Me.HyphenateToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.HyphenateToolStripButton.Image = CType(resources.GetObject("HyphenateToolStripButton.Image"), System.Drawing.Image)
        Me.HyphenateToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.HyphenateToolStripButton.Name = "HyphenateToolStripButton"
        Me.HyphenateToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.HyphenateToolStripButton.Text = "Hyphenate"
        '
        'tssSpellCheck
        '
        Me.tssSpellCheck.Name = "tssSpellCheck"
        Me.tssSpellCheck.Size = New System.Drawing.Size(6, 25)
        '
        'ListToolStripDropDownButton
        '
        Me.ListToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ListToolStripDropDownButton.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NoneToolStripMenuItem, Me.BulletToolStripMenuItem, Me.NumToolStripMenuItem, Me.LCAlphaToolStripMenuItem, Me.UCAlphaToolStripMenuItem, Me.LCRomanToolStripMenuItem, Me.UCRomanToolStripMenuItem})
        Me.ListToolStripDropDownButton.Image = CType(resources.GetObject("ListToolStripDropDownButton.Image"), System.Drawing.Image)
        Me.ListToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Black
        Me.ListToolStripDropDownButton.Name = "ListToolStripDropDownButton"
        Me.ListToolStripDropDownButton.Size = New System.Drawing.Size(29, 22)
        Me.ListToolStripDropDownButton.Text = "Set List Style"
        Me.ListToolStripDropDownButton.ToolTipText = "Set List Style"
        '
        'NoneToolStripMenuItem
        '
        Me.NoneToolStripMenuItem.Name = "NoneToolStripMenuItem"
        Me.NoneToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.NoneToolStripMenuItem.Tag = "-1"
        Me.NoneToolStripMenuItem.Text = "(None)"
        '
        'BulletToolStripMenuItem
        '
        Me.BulletToolStripMenuItem.Name = "BulletToolStripMenuItem"
        Me.BulletToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.BulletToolStripMenuItem.Tag = "0"
        Me.BulletToolStripMenuItem.Text = "● (Bullets)"
        '
        'NumToolStripMenuItem
        '
        Me.NumToolStripMenuItem.Name = "NumToolStripMenuItem"
        Me.NumToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.NumToolStripMenuItem.Tag = "1"
        Me.NumToolStripMenuItem.Text = "1, 2, 3, ..."
        '
        'LCAlphaToolStripMenuItem
        '
        Me.LCAlphaToolStripMenuItem.Name = "LCAlphaToolStripMenuItem"
        Me.LCAlphaToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.LCAlphaToolStripMenuItem.Tag = "2"
        Me.LCAlphaToolStripMenuItem.Text = "a, b, c, ..."
        '
        'UCAlphaToolStripMenuItem
        '
        Me.UCAlphaToolStripMenuItem.Name = "UCAlphaToolStripMenuItem"
        Me.UCAlphaToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.UCAlphaToolStripMenuItem.Tag = "3"
        Me.UCAlphaToolStripMenuItem.Text = "A, B, C, ..."
        '
        'LCRomanToolStripMenuItem
        '
        Me.LCRomanToolStripMenuItem.Name = "LCRomanToolStripMenuItem"
        Me.LCRomanToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.LCRomanToolStripMenuItem.Tag = "4"
        Me.LCRomanToolStripMenuItem.Text = "i, ii, iii, ..."
        '
        'UCRomanToolStripMenuItem
        '
        Me.UCRomanToolStripMenuItem.Name = "UCRomanToolStripMenuItem"
        Me.UCRomanToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.UCRomanToolStripMenuItem.Tag = "5"
        Me.UCRomanToolStripMenuItem.Text = "I, II, III, ..."
        '
        'tssList
        '
        Me.tssList.Name = "tssList"
        Me.tssList.Size = New System.Drawing.Size(6, 25)
        '
        'HyperlinkToolstripButton
        '
        Me.HyperlinkToolstripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.HyperlinkToolstripButton.Enabled = False
        Me.HyperlinkToolstripButton.Image = CType(resources.GetObject("HyperlinkToolstripButton.Image"), System.Drawing.Image)
        Me.HyperlinkToolstripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.HyperlinkToolstripButton.Name = "HyperlinkToolstripButton"
        Me.HyperlinkToolstripButton.Size = New System.Drawing.Size(23, 22)
        Me.HyperlinkToolstripButton.Text = "Insert/Edit/Remove Hyperlink"
        Me.HyperlinkToolstripButton.Visible = False
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'FontDlg
        '
        Me.FontDlg.ShowColor = True
        '
        'RulerContextMenuStrip
        '
        Me.RulerContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MeasureInInchesToolStripMenuItem, Me.MeasureInCentimetersToolStripMenuItem})
        Me.RulerContextMenuStrip.Name = "RulerContextMenuStrip"
        Me.RulerContextMenuStrip.Size = New System.Drawing.Size(200, 48)
        '
        'MeasureInInchesToolStripMenuItem
        '
        Me.MeasureInInchesToolStripMenuItem.Checked = True
        Me.MeasureInInchesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.MeasureInInchesToolStripMenuItem.Name = "MeasureInInchesToolStripMenuItem"
        Me.MeasureInInchesToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.MeasureInInchesToolStripMenuItem.Text = "Measure In &Inches"
        '
        'MeasureInCentimetersToolStripMenuItem
        '
        Me.MeasureInCentimetersToolStripMenuItem.Name = "MeasureInCentimetersToolStripMenuItem"
        Me.MeasureInCentimetersToolStripMenuItem.Size = New System.Drawing.Size(199, 22)
        Me.MeasureInCentimetersToolStripMenuItem.Text = "Measure In &Centimeters"
        '
        'RichTextBoxEx
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ContextMenuStrip = Me.RulerContextMenuStrip
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.rtb)
        Me.Name = "RichTextBoxEx"
        Me.Size = New System.Drawing.Size(650, 186)
        Me.RTBContextMenuStrip.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.RulerContextMenuStrip.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents SpellChecker As NetSpell.SpellChecker.Spelling
    Public WithEvents rtb As System.Windows.Forms.RichTextBox
    Friend WithEvents FontToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents FontColorToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents BoldToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ItalicToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents UnderlineToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents tssTextFormat As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents LeftToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents CenterToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents RightToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents tssSearch As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SpellcheckToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents tssAlignment As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents FindNextToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents FontNameToolStripComboBox As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents FindToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents RTBContextMenuStrip As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents EditToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SelectAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PasteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UndoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RedoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FormatToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BoldToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ItalicToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UnderlineToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SearchToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FindToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FindNextToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SpellCheckToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AlignmentToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LeftToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CenterToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RightToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ListToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GeneralFontSettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FontSizeToolStripComboBox As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents InsertPictureToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents tssSpellCheck As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents InsertStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BackColorToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents tssFontColor As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents HyphenateToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HyphenateToolStripButton As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripSeparator8 As ToolStripSeparator
	Friend WithEvents ToolStripSeparator6 As ToolStripSeparator
	Friend WithEvents HyphenateTextToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents RemoveAllHyphensToolStripMenuItem1 As ToolStripMenuItem
	Friend WithEvents RemoveHiddenHyphensOnlyToolStripMenuItem1 As ToolStripMenuItem
	Friend WithEvents ReplaceToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents TextRuler1 As TextRuler
	Friend WithEvents FontDlg As FontDialog
	Friend WithEvents ColorDlg As ColorDialog
	Friend WithEvents RulerContextMenuStrip As ContextMenuStrip
	Friend WithEvents MeasureInInchesToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents MeasureInCentimetersToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents NoneContextToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents BulletContextToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents NumContextToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents LCAlphaContextToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents UCAlphaContextToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents LCRomanContextToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents UCRomanContextToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ListToolStripDropDownButton As ToolStripDropDownButton
	Friend WithEvents NoneToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents BulletToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents NumToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents LCAlphaToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents UCAlphaToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents LCRomanToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents UCRomanToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents InsertSymbolToolStripButton As ToolStripButton
	Friend WithEvents InsertPictureToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents InsertSymbolToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents VerticalPositionToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents RaisedSuperscriptToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents LoweredSubscriptToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents StrikethroughToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents HyperlinksToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents InsertEditRemoveHyperlinkToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents RemoveAllHyperlinksToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ToolStripSeparator9 As ToolStripSeparator
	Friend WithEvents KeepHypertextWhenRemovingLinksToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents HyperlinkToolstripButton As ToolStripButton
	Friend WithEvents tssSymbol As ToolStripSeparator
	Friend WithEvents tssList As ToolStripSeparator
	Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
End Class
