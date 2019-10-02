Imports System.Runtime.InteropServices
Imports System.Drawing.Printing
Imports System.Runtime.CompilerServices
Imports System.Text

Public Module PrintRichTextBox
	'   private components

	'      constants
	Private Const WM_USER As Int32 = &H400&
	Private Const EM_FORMATRANGE As Int32 = WM_USER + 57, _
		EM_GETSCROLLPOS As Integer = WM_USER + 221, EM_SETSCROLLPOS As Integer = WM_USER + 222
	Private Const EM_GETCHARFORMAT As Integer = WM_USER + 58, _
		EM_SETCHARFORMAT As Integer = WM_USER + 68
	Private Const WM_SETREDRAW As Integer = &HB

	'      variables
	Private _RichTextBox As System.Windows.Forms.RichTextBox
	Private _TextLength As Integer
	Private _PrintDocument As PrintDocument
	Private _CurrentPage, _FromPage, _ToPage As Integer
	Private _PageIndexes() As Integer, _PageCount As Integer
	Private _CharFont As Font = Nothing

	'      structures

	<StructLayout(LayoutKind.Sequential)> _
	Private Structure Rect
		Public left As Int32
		Public top As Int32
		Public right As Int32
		Public bottom As Int32
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Private Structure CharRange
		Public cpMin As Int32
		Public cpMax As Int32
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Private Structure FormatRangeStructure
		Public hdc As IntPtr
		Public hdcTarget As IntPtr
		Public rc As Rect
		Public rcPage As Rect
		Public chrg As CharRange
	End Structure

	<StructLayout(LayoutKind.Sequential)> _
	Private Structure CHARFORMAT2_STRUCT
		Public cbSize As UInt32
		Public dwMask As UInt32
		Public dwEffects As UInt32
		Public yHeight As Int32
		Public yOffset As Int32
		Public crTextColor As Int32
		Public bCharSet As Byte
		Public bPitchAndFamily As Byte
		<MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)> _
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



	'      DLL calls

	'         for printing
	<DllImport("user32.dll")> _
	Private Function SendMessage(ByVal hWnd As IntPtr, ByVal msg As Int32, _
		ByVal wParam As Int32, ByVal lParam As IntPtr) As Int32
	End Function

	'         for scrolling and auto-redraw setting

	<DllImport("user32.dll")> _
	Private Function SendMessage(ByVal hWnd As IntPtr, ByVal msg As Int32, _
		ByVal wParam As Int32, ByRef lParam As Point) As Int32
	End Function

	<DllImport("user32.dll", EntryPoint:="GetScrollInfo")> _
	Private Function GetScrollInfo(ByVal hwnd As IntPtr, _
		ByVal nBar As Integer, ByRef lpsi As ScrollInfo) As <MarshalAs(UnmanagedType.Bool)> Boolean
	End Function


	'      private functions

	'         printout functions

	Private Function FormatRange(ByVal Graphics As Graphics, ByVal PageSettings As PageSettings, _
		ByVal charFrom As Integer, ByVal charTo As Integer, ByVal RenderText As Boolean) As Integer
		With PageSettings
			'   define character range   
			Dim cr As CharRange
			cr.cpMin = charFrom : cr.cpMax = charTo
			'   define margins
			Dim rc As Rect
			rc.top = HundredthInchToTwips(.Bounds.Top + .Margins.Top)
			rc.bottom = HundredthInchToTwips(.Bounds.Bottom - .Margins.Bottom)
			rc.left = HundredthInchToTwips(.Bounds.Left + .Margins.Left)
			rc.right = HundredthInchToTwips(.Bounds.Right - .Margins.Right)
			'   define page size
			Dim rcPage As Rect
			rcPage.top = HundredthInchToTwips(.Bounds.Top)
			rcPage.bottom = HundredthInchToTwips(.Bounds.Bottom)
			rcPage.left = HundredthInchToTwips(.Bounds.Left)
			rcPage.right = HundredthInchToTwips(.Bounds.Right)
			'   handle device context
			Dim hdc As IntPtr = Graphics.GetHdc
			'   handle full format info
			Dim fr As FormatRangeStructure
			fr.chrg = cr : fr.hdc = hdc
			fr.hdcTarget = hdc : fr.rc = rc
			fr.rcPage = rcPage
			'   prepare to render/measure page   
			Dim wParam As Int32
			If RenderText Then
				wParam = 1 'render text
			Else
				wParam = 0 'measure only
			End If
			Dim lParam As IntPtr
			lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr))
			Marshal.StructureToPtr(fr, lParam, False)
			'   render/measure page and return first char of next page
			Dim res As Integer = SendMessage(_RichTextBox.Handle, EM_FORMATRANGE, wParam, lParam)
			Marshal.FreeCoTaskMem(lParam)
			Graphics.ReleaseHdc(hdc)
			Return res
		End With
	End Function

	Private Function HundredthInchToTwips(ByVal n As Integer) As Int32
		'   convert units
		Return Convert.ToInt32((n * 144) \ 10)
	End Function

	Private Sub FormatRangeDone()
		'   flag printing done
		Dim lParam As New IntPtr(0)
		SendMessage(_RichTextBox.Handle, EM_FORMATRANGE, 0, lParam)
	End Sub

	Private Sub GetPageIndexes()
		'   determine indexes for page beginnings, based on printer info
		_PageCount = 0
		Dim FirstCharOnPage As Integer = 0
		Do
			'   store index for start of current page
			ReDim Preserve _PageIndexes(_PageCount)
			_PageIndexes(_PageCount) = FirstCharOnPage
			'   measure current page
			FirstCharOnPage = _
				FormatRange(_PrintDocument.PrinterSettings.CreateMeasurementGraphics, _
					_PrintDocument.DefaultPageSettings, _
					FirstCharOnPage, _TextLength, False)
			'   prepare for next page
			_PageCount += 1
		Loop While FirstCharOnPage < _TextLength
	End Sub

	Private Function GetPageNumber(ByVal Position As Integer) As Integer
		'   search for page containing a given caret Position
		Dim PageNumber As Integer = Array.BinarySearch(_PageIndexes, Position)
		If PageNumber < 0 Then
			PageNumber = (PageNumber Xor -1) 'caret is inside of page
		Else
			PageNumber += 1                  'caret is at beginning of page
		End If
		Return PageNumber
	End Function

	Private Sub SetUp(ByVal RichTextBox As RichTextBox, _
		ByVal PrintDocument As PrintDocument, ByVal PageIndexesOnly As Boolean)
		'   prepare for print job
		_RichTextBox = RichTextBox : _TextLength = RichTextBox.TextLength
		_PrintDocument = PrintDocument : GetPageIndexes()
		If PageIndexesOnly Then
			Exit Sub 'leave with page indexes
		End If
		'   else prepare to preview/print
		With _PrintDocument
			'   wire up events
			'      (RemoveHandler is used before AddHandler to guard against double-firing
			'         of events in the event this routine is called multiple times)
			RemoveHandler .BeginPrint, AddressOf BeginPrint 'remove any pre-existing handler
			AddHandler .BeginPrint, AddressOf BeginPrint    'add a new handler
			RemoveHandler .PrintPage, AddressOf PrintPage
			AddHandler .PrintPage, AddressOf PrintPage
			RemoveHandler .EndPrint, AddressOf EndPrint
			AddHandler .EndPrint, AddressOf EndPrint
			'   determine which pages to print/preview
			With .PrinterSettings
				Select Case .PrintRange
					Case PrintRange.AllPages
						'   all pages
						_FromPage = 1 : _ToPage = _PageCount
					Case PrintRange.SomePages
						'   range of pages
						_FromPage = .FromPage - .MinimumPage + 1
						_ToPage = .ToPage - .MinimumPage + 1
					Case PrintRange.Selection
						'   pages of selected text
						_FromPage = GetPageNumber(_RichTextBox.SelectionStart)
						If _RichTextBox.SelectionLength = 0 Then
							_ToPage = _FromPage 'no selection
						Else
							_ToPage = _
								GetPageNumber(_RichTextBox.SelectionStart _
									+ _RichTextBox.SelectionLength - 1)
						End If
					Case Else
						'   page at caret position
						_FromPage = GetPageNumber(_RichTextBox.SelectionStart)
						_ToPage = _FromPage
				End Select
				'   validate page range
				If _FromPage < 1 Then
					_FromPage = 1 'page #'s are 1-based
				ElseIf _FromPage > _PageCount Then
					_FromPage = _PageCount
				End If
				If _ToPage < _FromPage Then
					_ToPage = _FromPage 'at least 1 page
				ElseIf _ToPage > _PageCount Then
					_ToPage = _PageCount
				End If
			End With
		End With
	End Sub

	'         scrolling/text-width functions

	Private Function GetFontAtCharacterPosition( _
		RichTextBox As RichTextBox, CharPos As Integer) As Font
		'   get font at specified character position
		'   NOTE: Selection isn't changed if it's already where it needs to be,
		'      lest recursion result if GetRightMostCharacterPosition is called within
		'      the control's SelectionChanged event and recursion results!
		With RichTextBox
			Dim CharFont As Font = Nothing
			If .SelectionStart = CharPos AndAlso .SelectionLength = 0 Then
				'   temporaily change selection
				Dim CurrentSS As Integer = .SelectionStart, CurrentSL As Integer = .SelectionLength
				.SuspendLayout
				.Select(CharPos, 0) : CharFont = .SelectionFont : .Select(CurrentSS, CurrentSL)
				.ResumeLayout
			Else
				CharFont = .SelectionFont
			End If
			Return _
				CharFont
		End With
	End Function

	Private Function GetRightMostCharacterPosition(RichTextBox As RichTextBox) As Integer
		'   get width of widest line in text
		Dim RightMostPos As Integer = 0
		Dim LineLength, LineWidth, LineIndex, CharIndex, CharWidth As Integer
		Dim Line, Character As String
		Dim CharFont As Font = Nothing
		With RichTextBox
			'   go through each line
			For LineNumber As Integer = 0 To .Lines.GetUpperBound(0)
				'   how long is this line?
				Line = .Lines(LineNumber) : LineLength = Line.Length
				If LineLength > 0 Then
					'   get rightmost character in line
					LineIndex = _
						.GetFirstCharIndexFromLine(LineNumber)
					CharIndex = LineIndex + LineLength - 1
					Character = Line.Substring(LineLength - 1, 1)
					'   get width up to rightmost char
					LineWidth = _
						.GetPositionFromCharIndex(CharIndex).X - .GetPositionFromCharIndex(LineIndex).X
					'   get size of char
					CharFont = GetFontAtCharacterPosition(RichTextBox, CharIndex + 1)
					Using g As Graphics = .CreateGraphics()
						CharWidth = _
							CInt(.CreateGraphics.MeasureString(Character, CharFont).Width _
								* RichTextBox.ZoomFactor)
					End Using
					LineWidth += CharWidth
					'   see if new line is longer than previous ones
					If LineWidth > RightMostPos Then
						RightMostPos = LineWidth
					End If
				End If
			Next LineNumber
		End With
		Return RightMostPos
	End Function



	'      PrintDocument event procedures

	Private Sub BeginPrint(ByVal sender As Object, _
		ByVal e As System.Drawing.Printing.PrintEventArgs)
		'   prepare to start printing   
		_CurrentPage = _FromPage
	End Sub

	Private Sub PrintPage(ByVal sender As Object, _
		ByVal e As System.Drawing.Printing.PrintPageEventArgs)
		'   print current page
		Dim FirstCharOnNextPage As Integer = _
			FormatRange(e.Graphics, e.PageSettings, _PageIndexes(_CurrentPage - 1), _TextLength, True)
		'   prepare for next page; is it already the last page?
		_CurrentPage += 1
		e.HasMorePages = _
			_CurrentPage <= _ToPage AndAlso FirstCharOnNextPage < _TextLength
	End Sub

	Private Sub EndPrint(ByVal sender As Object, _
		ByVal e As System.Drawing.Printing.PrintEventArgs)
		'   finish printing
		FormatRangeDone()
	End Sub



	'   public components (extension methods for WinForms RichTextBox)

	'      constant for selection margin

	''' <summary>
	''' Width of left-side "selection margin" for highlighting whole lines
	''' when a RichTextBox's ShowSelection property is True
	''' </summary>
	Public Const SelectionMargin As Single = 8F

	'      enums for GetScrollInfo call (used by GetScrollBarInfo)

	<Flags()> _
	Public Enum ScrollBarMask
		Range = 1 : PageSize = 2 : Position = 4 : TrackPosition = 16
		Everything = _
			Range Or PageSize Or Position Or TrackPosition
	End Enum

	<Flags> _
	Public Enum ScrollBarType
		Horizontal = 0 : Vertical = 1
		Control = 2 'ScrollInfo's nBar parameter when hwnd is a handle to a ScrollBar 
	End Enum

	'      structure for GetScrollInfo call (used by GetScrollBarInfo)

	<StructLayout(LayoutKind.Sequential)> _
	Public Structure ScrollInfo
		'   INPUT: What information to get
		Public SizeOfStructure As Integer 'infrastructure
		Public ScrollBarMask As Integer 'tells which fields below to retrieve information for
		'   OUTPUT: Scrollbar information
		Public MinimumScrollPosition As Integer
		Public MaximumScrollPosition As Integer
		Public PageSize As Integer
		Public Position As Integer
		Public TrackPosition As Integer
	End Structure

	'      enum for list types
	Public Enum RTBListStyle
		NoList = -1
		Bullets
		Numbers
		LowercaseLetters
		UppercaseLetters
		LowercaseRomanNumerals
		UppercaseRomanNumerals
	End Enum



	'      printout methods

	''' <summary>
	''' Print RichTextBox contents or a range of pages thereof
	''' </summary>
	''' <param name="PrintDocument">Instance of PrintDocument</param>
	''' <remarks></remarks>
	<Extension()> _
	Public Sub Print(ByVal RichTextBox As RichTextBox, _
		ByVal PrintDocument As PrintDocument)
		'   print document
		SetUp(RichTextBox, PrintDocument, False) : PrintDocument.Print()
	End Sub

	''' <summary>
	''' Preview RichTextBox contents or a range of pages thereof to be printed
	''' </summary>
	''' <param name="PrintPreviewDialog">Instance of PrintPreviewDialog</param>
	''' <returns>Result of Print Preview dialog</returns>
	''' <remarks></remarks>
	<Extension()> _
	Public Function PrintPreview(ByVal RichTextBox As RichTextBox, _
		ByVal PrintPreviewDialog As PrintPreviewDialog) As DialogResult
		'   preview document
		SetUp(RichTextBox, PrintPreviewDialog.Document, False)
		Return _
			PrintPreviewDialog.ShowDialog()
	End Function

	''' <summary>
	''' Get array of indexes for beginnings of pages
	''' </summary>
	''' <param name="PrintDocument">Instance of PrintDocument</param>
	''' <returns></returns>
	''' <remarks>Pages are measured according to PrintDocument.DefaultPageSettings;
	''' no print job is performed. There is always at least one index (array element)
	''' returned, and the first index is always 0, representing the beginning of all text.</remarks>
	<Extension()> _
	Public Function PageIndexes(RichTextBox As RichTextBox, _
		PrintDocument As PrintDocument) As Integer()
		'   acquire page indexes
		SetUp(RichTextBox, PrintDocument, True)
		Return _PageIndexes
	End Function

	''' <summary>
	''' Set RightMargin property of RichTextBox to width of printer page (within horizontal margins)
	''' so that text wraps at the same position in the text box as on the printer
	''' </summary>
	''' <param name="PageSettings">Instance of PageSettings</param>
	''' <remarks></remarks>
	<Extension()> _
	Public Sub SetRightMarginToPrinterWidth(RichTextBox As RichTextBox, _
		PageSettings As PageSettings)
		Dim PageWidth As Integer
		'   get page width in 1/100's of inches 
		With PageSettings
			PageWidth = .Bounds.Width - .Margins.Left - .Margins.Right
		End With
		'   set rich-textboxes .RightMargin property
		With RichTextBox
			.RightMargin = CType(PageWidth * .CreateGraphics.DpiX / 100R, Integer)
		End With
	End Sub

	''' <summary>
	''' Set PageSettings right margin to RichTextBox's RightMargin value
	''' so that text wraps at the same position on the printer as in the text box
	''' </summary>
	''' <param name="PageSettings">Instance of PageSettings</param>
	''' <remarks></remarks>
	<Extension()> _
	Public Sub SetPrinterWidthToRightMargin(RichTextBox As RichTextBox, _
		PageSettings As PageSettings)
		Dim PageWidth As Integer
		'   get text box's.RightMargin property in 1/100's of inches 
		With RichTextBox
			PageWidth = CType(.RightMargin * 100R / .CreateGraphics.DpiX, Integer)
		End With
		'   set left- and right-hand margin of printer page
		With PageSettings
			Dim Difference As Integer =  .Bounds.Width - PageWidth
			.Margins.Left = Difference \ 2 : .Margins.Right = Difference - .Margins.Left
		End With
	End Sub

	'      scrolling/text-width methods

	''' <summary>
	''' Get scroll position of RichTextBox
	''' </summary>
	''' <returns>Point structure containing current horizontal (.x)
	''' and vertical (.y) scroll positions in pixels</returns>
	''' <remarks></remarks>
	<Extension()> _
	Public Function GetScrollPosition(RichTextBox As RichTextBox) As Point
		Dim RTBScrollPoint As Point = Nothing
		SendMessage(RichTextBox.Handle, EM_GETSCROLLPOS, 0, RTBScrollPoint)
		Return RTBScrollPoint
	End Function

	''' <summary>
	''' Set scroll position of RichTextBox
	''' </summary>
	''' <param name="RichTextBox"></param>
	''' <param name="RTBScrollPoint">Point structure containing new horizontal (.x)
	''' and vertical (.y) scroll positions in pixels</param>
	''' <remarks></remarks>
	<Extension()> _
	Public Sub SetScrollPosition(RichTextBox As RichTextBox, _
		ByVal RTBScrollPoint As Point)
		SendMessage(RichTextBox.Handle, EM_SETSCROLLPOS, 0, RTBScrollPoint)
	End Sub

	''' <summary>
	''' Get information about a RichTextBox scroll bar
	''' </summary>
	''' <param name="ScrollBarType">ScrollBarType value (.Horizontal or .Vertical)</param>
	''' <param name="ScrollBarMask">ScrollBarMask flags indicating what to get
	''' (range, page size, position, track position; defaults to everything)</param>
	''' <returns>ScrollInfo structure with requested info</returns>
	''' <remarks></remarks>
	<Extension()> _
	Public Function GetScrollBarInfo(RichTextBox As RichTextBox, _
		ScrollBarType As ScrollBarType, _
		Optional ScrollBarMask As ScrollBarMask = ScrollBarMask.Everything) As ScrollInfo
		Dim si As New ScrollInfo
		si.SizeOfStructure = Marshal.SizeOf(si) 'must alway be set to the size of a ScrollInfo structure
		si.ScrollBarMask = ScrollBarMask 'tells the GetScrollInfo what to get
		GetScrollInfo(RichTextBox.Handle, ScrollBarType, si) 'horizontal or vertical?
		Return si
	End Function

	''' <summary>
	''' Get effective maximum text width of RichTextBox in pixels
	''' </summary>
	''' <returns>Maximum available physical width for any text.
	''' (-1 if we're in a recursive loop--see remarks)</returns>
	''' <remarks>This value is calculated as follows:
	''' 1. If control's RightMargin propert is non-zero, then that us used
	''' 2. Otherwise, if WordWrap is True, then the control's client-area width
	'''    minus any left-edge "selection" margin is used
	''' 3. Otherwise, if horizontal scrollbars are enabled, then the "maximum horizontal
	'''    scroll position" plus the client width, or the width of the longest physical line,
	'''    whichever is longer, is used
	''' 4. Otherwise, the width of the longest physical line is uesd</remarks>
	<Extension()> _
	Public Function GetMaximumWidth(RichTextBox As RichTextBox) As Integer
		With RichTextBox
			'   see if text width is fixed
			If .RightMargin > 0 Then
				'   yes, so return with fixed width
				Return _
					.RightMargin
			End If
			'   else start with width of text box
			Dim SelectionOffset As Integer = + .Padding.Left + 1, _
				MaxTextWidth As Integer = .ClientRectangle.Width
			If .ShowSelectionMargin Then
				'   account for any selection margin
				SelectionOffset += CInt(.ZoomFactor * SelectionMargin)
			End If
			MaxTextWidth -= CInt(.ZoomFactor * SelectionMargin)
			'   determine maximum linewidth
			If Not .WordWrap Then
				If (.ScrollBars And RichTextBoxScrollBars.Horizontal) _
							= RichTextBoxScrollBars.Horizontal Then
					'   determine rightmost character position from scrollbar info
					MaxTextWidth += _
						.GetScrollBarInfo(ScrollBarType.Horizontal).MaximumScrollPosition
				End If
				'   make sure value is at least as large as widest line
				MaxTextWidth = _
					Math.Max(MaxTextWidth, GetRightMostCharacterPosition(RichTextBox))
			End If
			'   return value
			Return MaxTextWidth
		End With
	End Function

	'      RTF methods

	''' <summary>
	''' Mark selected text in RichTextBox as a list, using ListStyle
	''' </summary>
	''' <param name="ListStyle">Style of list (no list, bullets, numbers, lowercase letters,
	''' uppercase letters, lowercase Roman numerals, or uppercase Roman numerals)</param>
	''' <remarks>RichTextBox.SelectionBullet returns False for any list style other than
	''' bullets when read; there is no easy way to GET the specific list-style of selected
	''' text even when parsing RTF code (RichTextBox.SelRTF), since, for instance, "I" can
	''' just as easily be Roman numeral for 1 OR letter "I", given the way the control
	''' defines lists</remarks>
	<Extension()> _
	Public Sub SetListStyle(RichTextBox As RichTextBox, _
		ByVal ListStyle As RTBListStyle)
		With RichTextBox
			'   get active control
			Dim AC As Control = .GetContainerControl.ActiveControl
			'   set keyboard focus to rich-text box
			.SuspendLayout : .Select
			'   set list style
			Select Case ListStyle
				Case RTBListStyle.NoList
					.SelectionBullet = False
				Case RTBListStyle.Bullets
					.SelectionBullet = True
				Case Else
					'   cycle through styles until desired one is selected
					.SelectionBullet = True
					SendKeys.SendWait("^+({L " & CType(ListStyle, Integer).ToString & "})")
			End Select
			'   restore original focus
			.ResumeLayout
			If AC IsNot Nothing Then
				AC.Select
			End If
		End With
	End Sub

	''' <summary>
	''' Convert plain-text string into RTF string
	''' </summary>
	''' <param name="PlainText">Plain-text string</param>
	''' <returns>String with special characters ("{", "\", "}", and non-ASCII) escaped</returns>
	<Extension()> _
	Public Function EscapedRTFText(RichTextBox As RichTextBox, _
		ByVal PlainText As String) As String
		'   escape any special RTF characters in string
		Dim sb As StringBuilder = New StringBuilder("")
		For CharIndex As Integer = 0 To PlainText.Length - 1
			'   parse plain-text string
			Dim Character As String = PlainText.Substring(CharIndex, 1)
			Select Case Character
				Case " ", "{", "\", "}"
					'   escape character
					sb.Append("\" & Character)
				Case Is < " ", Is > "~"
					'   non-ASCII character
					sb.Append("\u" & AscW(Character).ToString & "?")
				Case Else
					'   normal character
					sb.Append(Character)
			End Select
		Next CharIndex
		'   return escaped result
		Return _
			sb.ToString()
	End Function

	''' <summary>
	''' Insert RTF text into rich-text box at a given position
	''' </summary>
	''' <param name="RtfTextToInsert">RTF-format text to insert</param>
	''' <param name="position">position in rich-text box to make insertion
	''' (defaults to current selection position if omitted)</param>
	''' <remarks>This is the "safe" way to insert, as it accounts for "template RTF"
	''' that's expected in any inserted RTF text</remarks>
	<Extension()> _
	Public Sub InsertRtf(RichTextBox As RichTextBox, _
		ByVal RtfTextToInsert As String, _
		Optional ByVal position As Integer = -1)
		With RichTextBox
			'   position defaults to current selection point
			If position < 0 Or position > .TextLength Then
				position = .SelectionStart
			 Else
				.SelectionLength = 0 'arbitrary insertion
			End If
			'   get RTF template from an empty selection and insert new RTF text
			Dim Length As Integer = .SelectionLength
			.Select(position, 0)
			Dim RtfText As String = RichTextBox.SelectedRtf
			.Select(position, Length)
			Dim EndBrace As Integer = _
				RtfText.LastIndexOf("}"c) 'new text comes before RTF closing brace
			RtfText = RtfText.Insert(EndBrace, RtfTextToInsert)
			RichTextBox.SelectedRtf = RtfText
		End With
	End Sub

    '      drawing methods

    ''' <summary>
    ''' Turns on or off redrawing of rich-text box
    ''' This can be used to make multiple changes to the text box while preventing
    ''' the intermediate rendering that would cause flicker
    ''' </summary>
    ''' <param name="OnOrOff">True to activate auto-redraw, False to deactivate it</param>
    ''' <remarks>Specifying True forces the cumulative results
    ''' of previous operations to be rendered</remarks>
    <Extension()>
    Public Sub SetRedrawMode(Control As Windows.Forms.Control,
        ByVal OnOrOff As Boolean)
        If Control Is Nothing Then Exit Sub
        With Control
            '   set redraw mode
            SendMessage(.Handle, WM_SETREDRAW, CInt(OnOrOff) And 1, 1)
            If OnOrOff Then
                .Invalidate() 'force redraw
            End If
        End With
    End Sub
End Module