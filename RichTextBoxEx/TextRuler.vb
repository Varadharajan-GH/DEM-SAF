Imports System.ComponentModel
Imports System.Drawing.Drawing2D

'   main class

Partial Public Class TextRuler
	Inherits UserControl

	'   private components

	'      private constants
	Private MinimumDistance As Single = 11F 'tabs and left-vs.-right indents must be this far apart

	'      private variables

	Private Myself As RectangleF = New RectangleF()
	Private drawZone As RectangleF = New RectangleF()
	Private workArea As RectangleF = New RectangleF()
	Private markers As List(Of RectangleF) = New List(Of RectangleF)()
	Private tabs As List(Of RectangleF) = New List(Of RectangleF)()
	Private p As Pen = New Pen(Color.Transparent)
	Private lMargin As Single = 1F, rMargin As Single = 1F, _
		llIndent As Single = 1F, luIndent As Single = 1F, rIndent As Single = 1F
	Private _strokeColor As Color = Color.Black
	Private _baseColor As Color = Color.White
	Private pos As Single = -1F
	Private mCaptured As Boolean = False
	Private _noMargins As Boolean = True
	Private _CurrentMarker As MarkerType = MarkerType.No_Marker, _
		_CurrentTab As Integer = MarkerType.No_Marker
	Private _tabsEnabled As Boolean = True
	Private _Units As UnitType = UnitType.Centimeters
	Private _ToolTipString As String = ""
	Private RulerToolTip As New ToolTip()
	Private _UsingSmartToolTips As Boolean = True
	Private ReadOnly pixelsPerCM, pixelsPerIn As Single
	Private components As IContainer
	Private _ScrollOffset As Single = 0F, _MaxWidth As Single = 0F
	Private _ZoomFactor As Single = 1F
	Private _MaxTabs As Integer = 32

	'      private procedures

	Private Function ZoomValue(value As Single) As Single
	'   zoom position
	Return value * _ZoomFactor
	End Function

	Private Function UnZoomValue(value As Single) As Single
	'   un-zoom position
	Return value / _ZoomFactor
	End Function

	Private Function RightMostLeftIndent() As Single
	'   get upper or lower left indent, whichever is larger
	Return _
		Math.Max(luIndent, llIndent)
	End Function

	Private Function LeftMostLeftIndent() As Single
	'   get upper or lower left indent, whichever is smaller
	Return _
		Math.Max(luIndent, llIndent)
	End Function

	Private Function LeftMostTab() As Single
	'   get leftmost tab position
	If tabs.Count = 0 Then
		Return rMargin
	 Else
		Return tabs(0).X + _ScrollOffset
	End If
	End Function

	Private Function RightMostTab() As Single
	'   get rightmost tab position
	If tabs.Count = 0 Then
		Return lMargin
	 Else
		Return tabs(tabs.Count- 1).X + _ScrollOffset
	End If
	End Function

	Private Function IsTabInOrder(ByVal TabNumber As Integer, _
		ByVal TabPosition As Single) As Boolean
	'   make sure tab isn't moved past another tab
	If tabs.Count < 2 Then
		Return True 'single tab is always valid
	End If
	'   compare this tab to previous and next tabs
	Select Case TabNumber
	  Case 0
		  '   first tab must must be left of second
		  Return TabPosition <= tabs(1).X - MinimumDistance
	  Case tabs.Count - 1
		  '   last tab must be right of second-to-last
		  Return TabPosition >= tabs(tabs.Count - 2).X + MinimumDistance
	  Case Else
		  '   current tab must be between previous and next
		  Return TabPosition >= tabs(TabNumber - 1).X + MinimumDistance _
			  AndAlso TabPosition <= tabs(TabNumber + 1).X - MinimumDistance
	End Select
	End Function

	Private Function IsTabNew(ByVal TabPosition As Single) As Boolean
	'   make sure this proposed tab isn't already present
	For i As Integer = 0 To tabs.Count - 1
		If Math.Abs(tabs(i).X - TabPosition) <= MinimumDistance Then
			Return False 'tab already here
		End If
	Next i
	'   report tab is new
	Return True
	End Function

	Private Sub AdjustIndents(ByVal NewLeftMargin As Single, NewRightMargin As Single)
	'   adjust indents for new margins
	Dim LeftChange As Single = NewLeftMargin - lMargin, _
		RightChange As Single = NewRightMargin - rMargin
	luIndent += LeftChange : llIndent += LeftChange : rIndent += RightChange
	End Sub

	Private Function IsValidPosition(ByVal value As Single, _
		MarkerType As MarkerType) As Boolean
	'   make sure marker value is in range
	Select Case MarkerType
		Case MarkerType.Left_Margin
			'   left margin
			Return _
				value >= 1F _
					AndAlso RightMostLeftIndent() + (value - lMargin) + MinimumDistance _
						<= _MaxWidth - rIndent _
					AndAlso value <= LeftMostTab()
		Case MarkerType.Right_Margin
			'   right margin
			Return _
				RightMostLeftIndent() + MinimumDistance <= value - rIndent _
					AndAlso value <= _MaxWidth - 1F _
					AndAlso value >= RightMostTab()
		Case MarkerType.First_Line_Indent, MarkerType.Hanging_Indent
			'   upper or lower left indent
			Return _
				value >= lMargin AndAlso _
					value <= _MaxWidth - rIndent - MinimumDistance
		Case MarkerType.Left_Indents
			'   both indents
			Dim hi As Single = llIndent - luIndent
			If hi < 0 Then
				'    lower indent is leftmost
				Return _
					value >= lMargin AndAlso _
						value - hi <= _MaxWidth - (rIndent + MinimumDistance)
			 Else
				'   upper indent is leftmost
				Return _
					value - hi >= lMargin AndAlso _
						value <= _MaxWidth - (rIndent + MinimumDistance)
			End If
		Case MarkerType.Right_Indent
			'   right indent
			Return _
				 value >= RightMostLeftIndent() + MinimumDistance _
					 AndAlso value <= _MaxWidth - rMargin
		Case MarkerType.Tab
			'   tab
			Return _
				value >= lMargin AndAlso value <= _MaxWidth - rMargin
	End Select
	Return True
	End Function

	Private Function CreateTabArgs(ByVal newPos As Single) As TabEventArgs
	'   set up tab info for event
	Dim tae As TabEventArgs = _
		New TabEventArgs( _
			CInt(UnZoomValue(newPos - 1F) + _ScrollOffset), _
			CInt(UnZoomValue(pos - 1F) + _ScrollOffset))
	Return tae
	End Function

	Private Sub CheckForMarkerOrTab(e As MouseEventArgs)
	'   check to see if mouse is hovering over an indent/marging marker or a tab
	_CurrentMarker = MarkerType.No_Marker : _CurrentTab = MarkerType.No_Marker
	'   check for indent or margin marker 
	For i As MarkerType = MarkerType.Left_Margin To MarkerType.Left_Indents
		'   check this marker
		If markers(i).Contains(e.Location) Then
			If NoMargins AndAlso _
					(i = MarkerType.Left_Margin OrElse i = MarkerType.Right_Margin) Then
				Exit For 'margins not allowed
			End If
			'   mose is over one--see if left mouse button is down
			_CurrentMarker = i : Exit Sub
		End If
	Next i
	'   see if mouse is over a tab
	If _tabsEnabled AndAlso tabs.Count > 0 Then
		For i As Integer = 0 To tabs.Count - 1
			'   check this tab
			If tabs(i).Contains(e.Location) Then
				'   mouse is over one--see if left mouse button is down
				_CurrentMarker = MarkerType.Tab : _CurrentTab = i
				Exit Sub
			End If
		Next i
	End If
	End Sub

	Private Sub InitializeComponent()
	'   initialization
	Me.SuspendLayout
	'
	'   TextRuler
	'
	Me.Name = "TextRuler" : _ScrollOffset = 0 : _MaxWidth = 100F
	Me.Size = New System.Drawing.Size(CInt(_MaxWidth), 20)
	Me.ResumeLayout(False)
	End Sub

	Private Sub SetToolTip(i As MarkerType)
	'   configure tooltip text to mouse situation
	With RulerToolTip
		Dim ts As String = _ToolTipString
		If _UsingSmartToolTips AndAlso _
				i <> MarkerType.No_Marker AndAlso i <> MarkerType.Left_Indent_Bottom Then
			'   display tooltip for marker name
			Try
				ts = My.Resources.Resource1.ResourceManager.GetString(i.ToString)
			 Catch ex As Exception
				'   invalid tooltip value (shouldn't happen)--ignore
			End Try
		End If
		'   change tooltip text if needed
		If ts <> .GetToolTip(Me) Then
			.SetToolTip(Me, ts)
		End If
	End With
	End Sub

	Private Sub DrawBackGround(ByVal g As Graphics)
	'   draw background of ruler
	p.Color = Color.Transparent : g.FillRectangle(p.Brush, Myself)
	p.Color = _baseColor : g.FillRectangle(p.Brush, drawZone)
	End Sub

	Private Sub DrawMargins(ByVal g As Graphics)
	'   draw ruler's left and right margins
	If Not _noMargins Then
		markers(MarkerType.Left_Margin) = _
			New RectangleF(-_ScrollOffset, 3.0F, lMargin, 14.0F)
		markers(MarkerType.Right_Margin) = _
			New RectangleF(_MaxWidth - _ScrollOffset - rMargin + 1.0F, 3.0F, _
				CSng(rMargin), 14.0F)
		p.Color = Color.DarkGray
		g.FillRectangle(p.Brush, markers(MarkerType.Left_Margin))
		g.FillRectangle(p.Brush, markers(MarkerType.Right_Margin))
	End If
	'   draw outer rectangle
	g.PixelOffsetMode = PixelOffsetMode.None
	p.Color = _strokeColor
	g.DrawRectangle(p, -_ScrollOffset, 3, _MaxWidth - 1, 14)
	'   if there's any space after outer rectangle, mark it off
	Dim AfterMargin As Single= _MaxWidth - _ScrollOffset
	If AfterMargin < Myself.Width Then
		'   this space is inaccessible to user, so mark it off
		p.Color = Me.ForeColor
		g.FillRectangle(p.Brush, AfterMargin, 3, Myself.Width - AfterMargin, 14)
	End If
	End Sub

	Private Sub DrawTextAndMarks(ByVal g As Graphics)
	'   draw measurement info
	p.Color = Me.ForeColor
	Dim sz As SizeF
	Dim points, position, maximum, subdiv As Integer
	Dim range As Single = 0.0F, yLine() As Single = Nothing
	'   get subdivisions
	Select Case _Units
		Case UnitType.Centimeters
			'   mark off centimeters (divided in 1/2ths)
			points = CInt(UnZoomValue(_MaxWidth) / pixelsPerCM)
			subdiv = 2 : range = pixelsPerCM / CSng(subdiv)
			maximum = points * subdiv : ReDim yLine(maximum)
			'   determine subdivision marker lengths
			For i As Integer = 0 To maximum
				position = i + 1
				If (position Mod subdiv) = 1 Then
					yLine(i) = 8.0F  ' 1/2 cm (5 mm)
				 Else
					yLine(i) = -1.0F ' whole cm (no marker)
				End If
			Next i
		Case UnitType.Inches
			'   mark off inches (divided in 1/8ths)
			points = CInt(UnZoomValue(_MaxWidth) / pixelsPerIn)
			subdiv = 8 : range = pixelsPerIn / CSng(subdiv)
			maximum = points * subdiv : ReDim yLine(maximum)
			'   determine subdivision marker lengths
			For i As Integer = 0 To maximum
				position = i + 1
				Select Case position Mod subdiv
					Case 1, 3, 5, 7
						yLine(i) = 4.0F  ' 1/8, 3/8, 5/8, 7/8 inch
					Case 2, 6
						yLine(i) = 6.0F  ' 1/4, 3/4 inch
					Case 4
						yLine(i) = 8.0F  ' 1/2 inch
					Case Else
						yLine(i) = -1.0F ' whole inch (no marker)
				End Select
			Next i
	End Select
	'   draw #'s and markers
	Dim numeral As String = "", _
		XPos As Single = 0F
	For i As Integer = 0 To maximum
		position = i + 1 : XPos = ZoomValue(position * range) - _ScrollOffset
		If (position Mod subdiv) > 0 AndAlso yLine(i) > 0.0F Then
			'   draw subdivision marker
			g.DrawLine(p, XPos, 9.0F - yLine(i) / 2.0F, XPos, 9.0F + yLine(i) / 2.0F)
		 Else
			'   draw #
			numeral = Convert.ToInt32(position / subdiv).ToString
			sz = g.MeasureString(numeral, Me.Font)
			g.DrawString(numeral, Me.Font, p.Brush, _
				New PointF(XPos - sz.Width / 2, _
					(Myself.Height - sz.Height) / 2))
		End If
	Next i
	'   set pixel offset
	g.PixelOffsetMode = PixelOffsetMode.Half
	End Sub

	Private Sub DrawIndents(ByVal g As Graphics)
	'   draw indent markers
	markers(MarkerType.First_Line_Indent) = _
		New RectangleF(luIndent - _ScrollOffset - 4.5F, 0F, 9.0F, 8.0F)
	markers(MarkerType.Left_Indent_Bottom) = _
		New RectangleF(llIndent - _ScrollOffset - 4.5F, 8.2F, 9.0F, 11.8F)
	markers(MarkerType.Right_Indent) = _
		New RectangleF(_MaxWidth - _ScrollOffset - rIndent - 4.5F, 11.0F, _
			9.0F, 8.0F)
	markers(MarkerType.Hanging_Indent) = _
		New RectangleF(llIndent - _ScrollOffset - 4.5F, 8.2F, 9.0F, 5.9F)
	markers(MarkerType.Left_Indents) = _
		New RectangleF(llIndent - _ScrollOffset - 4.5F, 14.1F, 9.0F, 5.9F)
	g.DrawImage(My.Resources.l_indet_pos_upper, markers(MarkerType.First_Line_Indent))
	g.DrawImage(My.Resources.l_indent_pos_lower, markers(MarkerType.Left_Indent_Bottom))
	g.DrawImage(My.Resources.r_indent_pos, markers(MarkerType.Right_Indent))
	End Sub

	Private Sub DrawTabs(ByVal g As Graphics)
	'   draw tab markers
	If Not _tabsEnabled OrElse tabs.Count = 0 Then
		Return
	End If
	For i As Integer = 0 To tabs.Count - 1
		g.DrawImage(My.Resources.tab_pos, tabs(i))
	Next i
	End Sub

	Private Sub AddTab(ByVal pos As Single)
	'   create new tab and raise event
	Dim rect As RectangleF = New RectangleF(pos, 10.0F, 8.0F, 8.0F)
	'   determine where to insert
	Dim Index As Integer = -1
	Do
		Index += 1
	Loop While Index < tabs.Count AndAlso pos > tabs(Index).X
	tabs.Insert(Index, rect)
	_CurrentMarker = MarkerType.Tab : _CurrentTab = Index
	'   fire event
	RaiseEvent TabAdded(Me, CreateTabArgs(pos))
	End Sub

	'    public components

	'      enums

	''' <summary>
	''' Enumerations for ruler's units (inches or centimeters)
	''' </summary>
	Public Enum UnitType
		Centimeters
		Inches
	End Enum

	''' <summary>
	''' Enumerations for margin, indent, and tab markers
	''' </summary>
	Public Enum MarkerType
		No_Marker = -1 : Left_Indent_Bottom = 0
		Left_Margin = 1 : Right_Margin = 2 : First_Line_Indent = 3
		Right_Indent = 4 : Hanging_Indent = 5 : Left_Indents = 6
		Tab = 7
	End Enum

	'      events

	''' <summary>
	''' Event for tracking a change in indent(s) by user
	''' </summary>
	''' <param name="sender">TextRuler instance</param>
	''' <param name="e">INPUT: e.MarkerType = type of indent(s) changed</param>
	Public Event IndentsChanged(sender As Object, e As MarginOrIndentEventArgs)

	''' <summary>
	''' Event for tracking a change in margin by user
	''' </summary>
	''' <param name="sender">TextRuler instance</param>
	''' <param name="e">INPUT: e.MarkerType = type of margin changed</param>
	Public Event MarginsChanged(sender As Object, e As MarginOrIndentEventArgs)

	''' <summary>
	''' Event for tracking a new tab being added by user
	''' </summary>
	''' <param name="sender">TextRuler instance</param>
	''' <param name="e">INPUT: e.NewPosition = position of new tab</param>
	Public Event TabAdded(sender As Object, e As TabEventArgs)

	''' <summary>
	''' Event for tracking an existing tab being removed by user
	''' </summary>
	''' <param name="sender">TextRuler instance</param>
	''' <param name="e">INPUT: e.OldPosition = position of deleted tab</param>
	Public Event TabRemoved(sender As Object, e As TabEventArgs)

	''' <summary>
	''' Event for tracking an existing tab being moved by user
	''' </summary>
	''' <param name="sender">TextRuler instance</param>
	''' <param name="e">INPUT:
	''' e.NewPosition = mew position of tab, and e.OldPosition = previous position of tab</param>
	Public Event TabChanged(sender As Object, e As TabEventArgs)

	'      constructor

	''' <summary>
	''' Initialize control
	''' </summary>
	Public Sub New()
	InitializeComponent()
	'   set up marker rectangles and styles
	With markers
		For i As MarkerType = MarkerType.Left_Indent_Bottom To MarkerType.Left_Indents
			.Add(New RectangleF())
		Next i
	End With
	Me.SetStyle(ControlStyles.UserPaint, True)
	Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
	Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
	Me.SetStyle(ControlStyles.ResizeRedraw, True)
	Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
	'   set up graphical settings
	Me.BackColor = Color.Transparent : Me.Font = New Font("Arial", 9.25F)
	tabs.Clear()
	'   get unit-to-pixel conversion values
	Using g As Graphics = Graphics.FromHwnd(Me.Handle)
		pixelsPerIn = g.DpiX : pixelsPerCM = g.DpiX / 2.54F
	End Using
	'   set up tooltip
	With RulerToolTip
		.ShowAlways = True : .SetToolTip(Me, _ToolTipString)
	End With
	End Sub

	'      properties

	''' <summary>
	''' Which margin or indent(s) is/are the mouse hovering over or dragging?
	''' </summary>
	''' <returns>Type of marker (MarkerType.No_Marker if none)</returns>
	<Browsable(False)> _
	Public ReadOnly Property MarkerUnderMouse() As MarkerType
	Get
		Return _CurrentMarker
	End Get
	End Property

	''' <summary>
	''' Which tab is the mouse hovering over or dragging?
	''' </summary>
	''' <returns>Tab position number (MarkerType.No_Marker if none)</returns>
	<Browsable(False)> _
	Public ReadOnly Property TabUnderMouse() As Integer
	Get
		If _CurrentMarker = MarkerType.No_Marker _
				OrElse _CurrentTab = MarkerType.No_Marker Then
			'   not over a tab
			Return MarkerType.No_Marker
		 Else
			'   else get position
			Return CInt(UnZoomValue(tabs(_CurrentTab).X - 1F) + _ScrollOffset)
		End If
	End Get
	End Property

	''' <summary>
	''' Gets or sets whether ruler can be used to handle tabs
	''' </summary>
	''' <value>True to enable, False to disable</value>
	''' <returns>True (default) if enabled, False if disabled</returns>
	<Category("Tabulation")> _
	<Description("Gets or sets whether tabs are to be displayed.")>
	<DefaultValue(False)> _
	Public Property TabsEnabled As Boolean
	Get
		Return _tabsEnabled
	End Get
	Set(ByVal value As Boolean)
		_tabsEnabled = value : Me.Refresh()
	End Set
	End Property

	''' <summary>
	''' Gets or sets maximum number of tabs allowed when tabs are enabled
	''' </summary>
	''' <vale>Number of tabs allowed (must be non-negative)</vale>
	''' <returns>Number of tabs allowed (defaults to 32)</returns>
	''' <remarks>If number of existing tabs is greater than value given,
	''' then this property is not changed</remarks>
	<Category("Tabulation")> _
	<Description("Gets or sets how many tabs are allowed.")>
	<DefaultValue(GetType(Integer), "32")> _
	Public Property MaximumTabs As Integer
	Get
		Return _MaxTabs
	End Get
	Set(value As Integer)
		If value < 0 Then
			value = 0 'make negative value 0
		End If
		If value <= tabs.Count Then
			_MaxTabs = value 'can set new ceiling
		End If
	End Set
	End Property

	''' <summary>
	''' Gets or sets positions of tabs in inches/centimeters,
	''' depending on value of Units property
	''' </summary>
	''' <value>Array of new tab positions</value>
	''' <returns>Array of existing tab positions</returns>
	<Browsable(False)> _
	Public Property TabPositionsInUnits() As Single()
	Get
		'   get positions using a certain measurement
		Dim positions() As Integer = Me.TabPositions()
		Dim Units() As Single = Nothing
		If positions IsNot Nothing Then
			ReDim Units(positions.Length - 1)
			For i As Integer = 0 To positions.Length - 1
				Units(i) = Me.PixelsToUnits(positions(i))
			Next i
		End If
		Return Units
	End Get
	Set(Units() As Single)
		'   set positions using a certain measurement
		Dim positions() As Integer = Nothing
		If Units IsNot Nothing Then
			ReDim positions(Units.Length - 1)
			For i As Integer = 0 To Math.Min(Positions.Length, _MaxTabs) - 1
				positions(i) = Me.UnitsToPixels(Units(i))
			Next i
		End If
		Me.TabPositions() = positions
	End Set
	End Property

	''' <summary>
	''' Gets or sets positions of tabs in pixels
	''' </summary>
	''' <value>Array of new tab positions</value>
	''' <returns>Array of existing tab positions</returns>
	<Browsable(False)> _
	Public Property TabPositions() As Integer()
	Get
		'   get positions
		Dim positions(tabs.Count - 1) As Integer
		For i As Integer = 0 To tabs.Count - 1
			positions(i) = CInt(UnZoomValue(tabs(i).X - 1F) + _ScrollOffset)
		Next i
		Array.Sort(positions) : Return positions
	End Get
	Set(positions() As Integer)
		'   set positions
		Array.Sort(positions) : tabs.Clear()
		If positions IsNot Nothing Then
			For i As Integer = 0 To Math.Min(positions.Length, _MaxTabs) - 1
				Dim TabValue As Single = ZoomValue(positions(i)) + 1F
				If IsValidPosition(TabValue, MarkerType.Tab) Then
					'   tab stop can't be outside margins or redundant
					If i = 0 OrElse positions(i) > positions(i - 1) Then
						Dim rect As RectangleF = _
							New RectangleF(TabValue - _ScrollOffset, 10.0F, 8.0F, 8.0F)
						tabs.Add(rect)
					End If
				End If
			Next i
		End If
		Me.Refresh()
	End Set
	End Property

	''' <summary>
	''' Gets number of pixels per centimeter
	''' </summary>
	''' <returns>Number of pixels/cm</returns>
	<Category("Measurements")> _
	<Description("Gets number of pixels per centimeter.")> _
	Public ReadOnly Property PixelsPerCentimeter() As Single
	Get
		Return pixelsPerCM
	End Get
	End Property

	''' <summary>
	''' Gets number of pixels per inch
	''' </summary>
	''' <returns>Number of pixels/inch</returns>
	<Category("Measurements")> _
	<Description("Gets number of pixels per inch.")> _
	Public ReadOnly Property PixelsPerInch() As Single
	Get
		Return pixelsPerIn
	End Get
	End Property

	''' <summary>
	''' Gets or sets whether units are inches or centimeters
	''' </summary>
	''' <value>New type of unit</value>
	''' <returns>Existing type of unit (default is inches)</returns>
	<Category("Measurements")> _
	<DefaultValue(GetType(UnitType), "Centimeters")> _
	<Description("Gets or sets whether units are measured in millimeters, 1/16th inch, or pixels.")> _
	Public Property Units() As UnitType
	Get
		Return _Units
	End Get
	Set(value As UnitType)
		_Units = value : Me.Refresh()
	End Set
	End Property

	''' <summary>
	''' Gets or sets normal tooltip for ruler
	''' </summary>
	''' <value>New tooltip text</value>
	''' <returns>Existing tooltip text (defaults to null)</returns>
	<Category("ToolTips")> _
	<Description("Gets or sets TooTip text for ruler.")> _
	Public Property ToolTipText() As String
	Get
		Return _ToolTipString
	End Get
	Set(value As String)
		_ToolTipString = value : SetToolTip(MarkerType.No_Marker)
	End Set
	End Property

	''' <summary>
	''' Gets or sets whether tooltip becomes name of marker when mouse is over one
	''' </summary>
	''' <value>True for yes (use marker names when over markers),
	''' False for no (always stick with standard ToolTipText)</value>
	''' <returns>True (default) or yes, False for no</returns>
	<Category("ToolTips")> _
	<Description("Gets or sets whther TooltTips change when over markers and tabs.")> _
	Public Property UsingSmartToolTips() As Boolean
	Get
		Return _UsingSmartToolTips
	End Get
	Set(value As Boolean)
		_UsingSmartToolTips = value
	End Set
	End Property

	''' <summary>
	''' Gets or sets color of ruler's border
	''' </summary>
	''' <value>New border color</value>
	''' <returns>Existing border color (defaults to black)</returns>
	<Category("Appearance")> _
	<DefaultValue(GetType(Color), "Black")> _
	<Description("Color of the border drawn on the bounds of control.")> _
	Public Property BorderColor As Color
	Get
		Return _strokeColor
	End Get
	Set(ByVal value As Color)
		_strokeColor = value : Me.Refresh()
	End Set
	End Property

	''' <summary>
	''' Gets or sets zoom factor for ruler
	''' </summary>
	''' <value>Factor to multiply "true" margin/indent/tab positions by (must be positive!)</value>
	''' <returns>Factor by which "true" positions are multiplied</returns>
	''' <remarks>Changing the zoom factor only changes the visual scaling of the ruler and of
	''' the positions of markers; if DOES NOT effect the values gotten/set for the margin,
	''' indent, and tab properties (that is, their pixel/unit values are always treated as if
	''' ZoomFactor were 1.0).</remarks>
	<Category("Appearance")> _
	<DefaultValue(GetType(Single), "1.0")> _
	<Description("Zoom factor for control.")> _
	Public Property ZoomFactor As Single
	Get
		Return _ZoomFactor
	End Get
	Set(value As Single)
		If value <= 0F Then
			Throw New ArgumentException("Invalid zoom factor!")
		 ElseIf value <> _ZoomFactor Then
			'   re-position information
			Dim Adjustment As Single = value / _ZoomFactor
			'   margins
			lMargin = Adjustment * (lMargin - 1F) + 1F
			rMargin = Adjustment * (rMargin - 1F) + 1F
			'   indents
			llIndent = Adjustment * (llIndent - 1F) + 1F
			luIndent = Adjustment * (luIndent - 1F) + 1F
			rIndent = Adjustment * (rIndent - 1F) + 1F
			'   tabs
			For i = 0 To tabs.Count - 1
				tabs(i) = _
					New RectangleF( _
						Adjustment * (tabs(i).X + _ScrollOffset - 1F) + 1F - _ScrollOffset, _
						tabs(i).Y, tabs(i).Width, tabs(i).Height)
			Next i
			'   re-draw
			_ZoomFactor = value : Me.Refresh()
		End If
	End Set
	End Property

	''' <summary>
	''' Gets or sets base color for ruler
	''' </summary>
	''' <value>New base color</value>
	''' <returns>Existing base color (defaults to white)</returns>
	<Category("Appearance")> _
	<DefaultValue(GetType(Color), "White")> _
	<Description("Base color for the control.")> _
	Public Property BaseColor As Color
	Get
		Return _baseColor
	End Get
	Set(ByVal value As Color)
		_baseColor = value
	End Set
	End Property

	''' <summary>
	''' Get or sets the maximum width of ruler, including margins, in pixels
	''' </summary>
	''' <value>Maximum width for ruler area in pixels, including margins</value>
	''' <returns>Maximum width of ruler area in pixels, including margins</returns>
	''' <remarks>NOTES:
	''' 1. Value cannot be too narrow to allow space between the rightmost left-indent
	'''    and the right indent
	''' 2. This is PrintableWidth plus any margin widths</remarks>
	<Category("Ruler Dimensions")> _
	Public Property RulerWidth() As Integer
	Get
		Return Cint(_MaxWidth - 2F)
	End Get
	Set(value As Integer)
		Dim ActualValue As Single = CSng(value) + 2F
		If ActualValue > RightMostLeftIndent() + rIndent Then
			_MaxWidth = CInt(ActualValue) : Me.Refresh()
		End If
	End Set
	End Property

	''' <summary>
	''' Gets or sets width of area within margins of ruler, in pixels
	''' </summary>
	''' <value>The maximum printable area within margins in pixels</value>
	''' <returns>The maximum printable area with margins in pixels</returns>
	''' <remarks>NOTES:
	''' 1. Value cannot be too narrow to allow space between the rightmost left-indent
	'''    and the right indent
	''' 2. This is MaximumWidth minus any margin widths</remarks>
	<Category("Ruler Dimensions")> _
	Public Property PrintableWidth() As Integer
	Get
		Return _
			Me.RulerWidth - CInt(lMargin + rMargin)
	End Get
	Set(value As Integer)
		Me.RulerWidth = value + CInt(lMargin + rMargin)
	End Set
	End Property

	''' <summary>
	''' Get or sets the scrolling offset of ruler
	''' </summary>
	''' <value>Scrolling offset for ruler area in pixels</value>
	''' <returns>Scrolling offset of ruler area in pixles</returns>
	''' <remarks>Value cannot be larger than maximum ruler width</remarks>
	<Category("Ruler Dimensions")> _
	Public Property ScrollingOffset() As Integer
	Get
		Return CInt(_ScrollOffset)
	End Get
	Set(value As Integer)
		If _ScrollOffset <= _MaxWidth Then
			'   move tabs to synch with new value
			For i As Integer = 0 To tabs.Count - 1
				tabs(i) = New RectangleF( _
					tabs(i).X - (CSng(value) - _ScrollOffset), tabs(i).Y, _
					tabs(i).Width, tabs(i).Height)
			Next i
			_ScrollOffset = CSng(value) : Me.Refresh()
		End If
	End Set
	End Property

	''' <summary>
	''' Gets or sets whether margins are disabled
	''' </summary>
	''' <value>True to disable (no margins), False to enable (margins allowed)</value>
	''' <returns>True if disabled (default), False if enabled</returns>
	''' <remarks>NOTE: If margins pre-exist, then the left and right indent properties
	''' are adjusted to point to the same locations as before</remarks>
	<Category("Margins")> _
	<Description("Gets or sets whether margins are disabled.")> _
	<DefaultValue(False)> _
	Public Property NoMargins() As Boolean
	Get
		Return _noMargins
	End Get
	Set(ByVal value As Boolean)
		_noMargins = value
		If value Then
			'   erase margins
			AdjustIndents(1F, 1F) : lMargin = 1F : rMargin = 1F : Me.Refresh()
		End If
	End Set
	End Property

	''' <summary>
	''' Gets or sets left margin, in pixels whenever ZoomFactor = 1
	''' </summary>
	''' <value>New margin (NoMargins property must be False)</value>
	''' <returns>Existing margin (defaults to 0)</returns>
	''' <remarks>NOTES:
	''' 1. If NoMargins property is True, margin will not be set
	''' 2. Left margin cannot be set to move rightmost left indent
	'''    too close to right indent</remarks>
	<Category("Margins")> _
	<Description("Gets or sets left margin.")> _
	<DefaultValue(0)> _
	Public Property LeftMargin As Integer
	Get
		Return _
			CInt(UnZoomValue(lMargin - 1F))
	End Get
	Set(ByVal value As Integer)
		If Not NoMargins Then
			'   set left margin
			Dim ActualValue As Single = ZoomValue(value) + 1F
			If IsValidPosition(ActualValue, MarkerType.Left_Margin) Then
				AdjustIndents(ActualValue, rMargin)
				lMargin = ActualValue : Me.Refresh
			End If
		End If
	End Set
	End Property

	''' <summary>
	''' Gets or sets right margin, in pixels whenever ZoomFactor = 1
	''' </summary>
	''' <value>New margin</value>
	''' <returns>Existing margin (defaults to 0)</returns>
	''' <remarks>NOTES:
	''' 1. If NoMargins property is True, margin will not be set
	''' 2. Right margin cannot be set to move right indent
	'''    too close to rightmost left indent</remarks>
	<Category("Margins")> _
	<Description("Gets or sets right margin.")> _
	<DefaultValue(0)> _
	Public Property RightMargin As Integer
	Get
		Return _
			CInt(UnZoomValue(rMargin - 1F))
	End Get
	Set(ByVal value As Integer)
		If Not NoMargins Then
			'   set right margin
			Dim ActualValue As Single = ZoomValue(value) + 1F
			If IsValidPosition(_MaxWidth - ActualValue, MarkerType.Right_Margin) Then
				AdjustIndents(lMargin, ActualValue)
				rMargin = ActualValue : Me.Refresh()
			End If
		End If
	End Set
	End Property

	''' <summary>
	''' 'Gets or sets left hanging indent--offset, in pixels whenever ZoomFactor = 1,
	''' from first-line left indent for all lines in a paragraph after the first
	''' (negative for "first-line" indent, 0 for no indent offset, positive for true "hanging" indent)
	''' </summary>
	''' <value>New offset from left indent</value>
	''' <returns>Existing offset from left indent (defaults to 0)</returns>
	''' <remarks>NOTE: Hanging indent may not be set to get left of left margin</remarks>
	<Category("Indents")> _
	<Description("Gets or sets left hanging indent.")> _
	<DefaultValue(0)> _
	Public Property HangingIndent As Integer
	Get
		Return _
			CInt(UnZoomValue(llIndent - luIndent))
	End Get
	Set(ByVal value As Integer)
		Dim ActualValue = ZoomValue(value) + luIndent
		If IsValidPosition(ActualValue, MarkerType.Hanging_Indent) Then
			'   set left indent for all lines but first
			llIndent = ActualValue : Me.Refresh
		End If
	End Set
	End Property

	''' <summary>
	''' Gets or sets first-line indent--offset, in pixels whenever ZoomFactor = 1,
	''' from any left margin of the first line in a paragraph without changing indent
	''' of subsequent lines
	''' </summary>
	''' <value>New first-line left-indent value</value>
	''' <returns>Existing first-line left-indent value
	''' (the same as LeftIndent property when reading; defaults to 0)</returns>
	''' <remarks>NOTES:
	''' 1. First-line indent cannot be set to move left of left margin or right of right indent
	''' 2. This moves only the indent of the first-line in each paragraph; the indent of subsequent
	'''    lines remains unchanged, causing HangingIndent property to be counter-offset</remarks>
	<Category("Indents")> _
	<Description("Gets or sets left indent.")> _
	<DefaultValue(0)> _
	Public Property FirstLineIndent As Integer
	Get
		Return _
			CInt(UnZoomValue(luIndent - lMargin))
	End Get
	Set(ByVal value As Integer)
		Dim ActualValue As Single = ZoomValue(value) + lMargin
		If IsValidPosition(ActualValue, MarkerType.First_Line_Indent) Then
			'   set first-line left indent
			luIndent = ActualValue : Me.Refresh()
		End If
	End Set
	End Property

	''' <summary>
	''' Gets or sets left indent--offset, in pixels whenever ZoomFactor = 1, from any left margin
	''' of first line in paragraph, moving indendation of subsequent lines along with it,
	''' </summary>
	''' <value>New first-line left-indent value
	''' (cannot be make other lines go left of left MARGIN!)</value>
	''' <returns>Existing first-line left-indent value
	''' (the same as FirstLineIndent property when reading; defaults to 0)</returns>
	''' <remarks>NOTES:
	''' 1. Left indent cannot be set to move left of left margin or right of right indent
	''' 2. All lines in paragraph are moved the same distance left or right,
	'''    causing HangingIndent property to remain unchanged</remarks>
	<Category("Indents")> _
	<Description("Gets or sets left indent.")> _
	<DefaultValue(0)> _
	Public Property LeftIndent As Integer
	Get
		Return _
			CInt(UnZoomValue(luIndent - lMargin))
	End Get
	Set(ByVal value As Integer)
		Dim hi As Single = ZoomValue(llIndent - luIndent)
		Dim ActualValue As Single = ZoomValue(value) + lMargin
		If IsValidPosition(ActualValue + hi, MarkerType.Left_Indents) Then
			'   set both left indents
			luIndent = ActualValue : llIndent = luIndent + hi : Me.Refresh()
		End If
	End Set
	End Property

	''' <summary>
	''' Gets or sets right indent--offset, in pixels whenever ZoomFactor = 1,
	''' from any right margin
	''' </summary>
	''' <value>New right-indent value</value>
	''' <returns>Existing right-indent value (defaults to 0)</returns>
	''' <remarks>NOTE: Right indent cannot be set to move
	''' right of right margin or left of leftmost left indent</remarks>
	<Category("Indents")> _
	<Description("Gets or sets right indent.")> _
	<DefaultValue(0)> _
	Public Property RightIndent() As Integer
	Get
		Return _
			CInt(UnZoomValue(rIndent - rMargin))
	End Get
	Set(ByVal value As Integer)
		Dim ActualValue As Single = ZoomValue(value)
		If IsValidPosition(_MaxWidth - ActualValue - rMargin, _
				MarkerType.Right_Indent) Then
			'   set right indent
			rIndent = ActualValue + rMargin : Me.Refresh()
		End If
	End Set
	End Property

	'      methods

	''' <summary>
	''' Convert value from current unit type (inches or centimeters) to pixels
	''' </summary>
	''' <param name="value">Value in units specified by Units property</param>
	''' <returns>Equivalent value in pixels</returns>
	Public Function UnitsToPixels(ByVal value As Single) As Integer
	If _Units = UnitType.Centimeters Then
		'   unit is in centimeters
		Return _
			CInt(value * pixelsPerCM)
	 Else
		'   unit is in inches
		Return _
			CInt(value * pixelsPerIn)
	End If
	End Function

	''' <summary>
	''' Convert value from pixels to current unit type (inches or centimeters)
	''' </summary>
	''' <param name="value">Value in pixels</param>
	''' <returns>Equivalent value in units specified by Units property</returns>
	<Browsable(False)> _
	Public Function PixelsToUnits(ByVal value As Integer) As Single
	If _Units = UnitType.Centimeters Then
		'   unit will be in centimeters
		Return _
			CSng(value) / pixelsPerCM
	 Else
		'   unit will be in inches
		Return _
			CSng(value) / pixelsPerIn
	End If
	End Function

	'   protected methods

	'      drawing/sizing of control

	Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
	'   handle redraw
	MyBase.OnPaint(e)
	With e.Graphics
		.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality
		.InterpolationMode = InterpolationMode.HighQualityBicubic
		.SmoothingMode = SmoothingMode.AntiAlias
		.PixelOffsetMode = PixelOffsetMode.Half
		.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit
	End With
	Myself = New RectangleF(0F, 0F, CSng(Me.Width), CSng(Me.Height))
	drawZone = New RectangleF(1.0F, 3.0F, CSng(Me.Width - 2), 14.0F)
	workArea = _
		New RectangleF(Math.Max(lMargin - _ScrollOffset, 0), 3.0F, _
			drawZone.Width - rMargin - drawZone.X * 2, 14.0F)
	DrawBackGround(e.Graphics)
	DrawMargins(e.Graphics) : DrawTextAndMarks(e.Graphics)
	DrawIndents(e.Graphics) : DrawTabs(e.Graphics)
	End Sub

	Protected Overrides Sub OnResize(ByVal e As EventArgs)
	MyBase.OnResize(e)
	'   fix height at 20
	If Me.Height <> 20 Then
		Me.Height = 20
	End If
	End Sub

	Protected Overrides Sub OnFontChanged(e As EventArgs)
	'   keep font size at 9.25
	MyBase.OnFontChanged(e)
	If Me.Font.SizeInPoints <> 9.25F Then
		Me.Font = New Font(Me.Font.FontFamily, _
			9.25F, Me.Font.Style, GraphicsUnit.Point)
	End If
	End Sub

	'      mouse actions

	Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
	'   see if mouse is over an indent/margin marker or a tab
	MyBase.OnMouseDown(e)
	mCaptured = False : CheckForMarkerOrTab(e)
	If (e.Button And MouseButtons.Left) = MouseButtons.Left Then
		'   button pressed--set capture info
		Select Case _CurrentMarker
			Case MarkerType.No_Marker
				'   not over anything
				mCaptured = False
			Case MarkerType.Tab
				'   over a tab
				mCaptured = True : pos = CInt(tabs(_CurrentTab).X)
			Case Else
				'   over an indent/margin marker
				mCaptured = True
		End Select
	End If
	'   set up tooltip
	SetToolTip(_CurrentMarker)
	End Sub

	Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
	'   see if we are to add, remove, or move a tab
	MyBase.OnMouseUp(e)
  	'   mouse is down--what was tracked?
	If Not workArea.Contains(e.Location) Then
		'   removing a tab?
		If mCaptured AndAlso _tabsEnabled AndAlso _CurrentTab <> MarkerType.No_Marker Then
			'   get tab position and remove it from list
			Try
				Dim pos As Single = tabs(_CurrentTab).X
				tabs.RemoveAt(_CurrentTab) : Me.Refresh()
				RaiseEvent TabRemoved(Me, CreateTabArgs(pos))
			 Catch __unusedException1__ As Exception
				'   ignore any error
			End Try
			_CurrentMarker = MarkerType.No_Marker : _CurrentTab = MarkerType.No_Marker
		End If
	 ElseIf IsValidPosition(e.Location.X + _ScrollOffset, MarkerType.Tab) Then
		'   adding or moving a tab?		
		If Not mCaptured AndAlso _tabsEnabled Then
			'   new tab
			_CurrentMarker = MarkerType.Tab : Me.Refresh()
			If tabs.Count < _MaxTabs AndAlso IsTabNew(e.Location.X) Then
				AddTab(CSng(e.Location.X))
			End If
		 ElseIf mCaptured AndAlso _CurrentTab <> MarkerType.No_Marker _
				AndAlso IsTabInOrder(_CurrentTab, e.Location.X) Then
			'   existing tab
			_CurrentMarker = MarkerType.Tab : Me.Refresh()
			RaiseEvent TabChanged(Me, CreateTabArgs(e.Location.X))
		End If
	End If
	'capObject = MarkerType.No_Marker
	'   set up tooltip
	SetToolTip(_CurrentMarker) : Me.Refresh()
	'   no marker being tracked
	mCaptured = False
	_CurrentMarker = MarkerType.No_Marker : _CurrentTab = MarkerType.No_Marker
	End Sub

	Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
	'   see if we are to drag an indent/margin marker or a tab
	MyBase.OnMouseMove(e)
	Dim XPos As Single = e.Location.X + _ScrollOffset 'account for unseen region
	If (e.Button And MouseButtons.Left) = 0 Then
		'   mouse not down--check where it is for tooltip
		CheckForMarkerOrTab(e) : SetToolTip(_CurrentMarker)
		Exit Sub
	End If
	'   button down--update marker or tab
	If mCaptured AndAlso IsValidPosition(XPos, _CurrentMarker) Then
		'   marker was captured--which one?
		Select Case _CurrentMarker
			Case MarkerType.Left_Margin
				'   moving left margin?
				If Not NoMargins Then
					'   update left margin
					AdjustIndents(XPos, rMargin) : lMargin = XPos
					Me.Refresh()
					RaiseEvent MarginsChanged(Me, _
						New MarginOrIndentEventArgs(_CurrentMarker))
				End If
			Case MarkerType.Right_Margin
				'   moving right margin?
				If Not NoMargins Then
					'   update right margin
					rMargin = _MaxWidth - XPos : AdjustIndents(lMargin, rMargin)
					Me.Refresh()
					RaiseEvent MarginsChanged(Me, _
						New MarginOrIndentEventArgs(_CurrentMarker))
				End If
			Case MarkerType.First_Line_Indent
				'   moving main left indent?
				luIndent = XPos : Me.Refresh()
				RaiseEvent IndentsChanged(Me, _
					New MarginOrIndentEventArgs(_CurrentMarker))
			Case MarkerType.Right_Indent
				'   moving right indent?
				rIndent = _MaxWidth - XPos : Me.Refresh()
				RaiseEvent IndentsChanged(Me, _
					New MarginOrIndentEventArgs(_CurrentMarker))
			Case MarkerType.Hanging_Indent
				'   moving left hanging indent?
				llIndent = XPos : Me.Refresh()
				RaiseEvent IndentsChanged(Me, _
					New MarginOrIndentEventArgs(_CurrentMarker))
			Case MarkerType.Left_Indents
				'    moving BOTH left indents?
				luIndent = XPos - (llIndent - luIndent) : llIndent = XPos
				Me.Refresh()
				RaiseEvent IndentsChanged(Me, _
					New MarginOrIndentEventArgs(_CurrentMarker))
			Case MarkerType.Tab
				'   moving tab?
				If _CurrentTab <> MarkerType.No_Marker _
						AndAlso workArea.Contains(e.Location) _
						AndAlso IsTabInOrder(_CurrentTab, e.Location.X) Then
					tabs(_CurrentTab) = _
						New RectangleF(CSng(e.Location.X), tabs(_CurrentTab).Y, _
							tabs(_CurrentTab).Width, tabs(_CurrentTab).Height)
					Me.Refresh()
				End If
		End Select
	End If
	'   set cursor
	If NoMargins AndAlso _
			(markers(MarkerType.Left_Margin).Contains(e.Location) _
				OrElse markers(MarkerType.Right_Margin).Contains(e.Location)) Then
		'   inside margin space--use west-east cursor
		Me.Cursor = Cursors.SizeWE
	 Else
		'   not inside margin--normal cursor
		Me.Cursor = Cursors.[Default]
	End If
	'   do tooltip
	SetToolTip(_CurrentMarker)
	End Sub
End Class



'   event classses

''' <summary>
''' Event class for dealing with when a tab is added, moved, or removed
''' </summary>
Public Class TabEventArgs
	Inherits EventArgs

	'   private variables

	Private _posN, _posO As Integer

	'   public components

	'      constructor

	''' <summary>
	''' Set up new and old positions
	''' </summary>
	''' <param name="NewPosition">New location of tab</param>
	''' <param name="OldPositon">Original location of tab</param>
	Public Sub New(ByVal NewPosition As Integer, ByVal OldPositon As Integer)
	_posN = NewPosition : _posO = OldPosition
	End Sub

	'      properties

	''' <summary>
	''' Get new position of tab
	''' </summary>
	''' <returns>New position of tab</returns>
	Public ReadOnly Property NewPosition As Integer
	Get
		Return _posN
	End Get
	End Property

	''' <summary>
	''' Get original position of tab
	''' </summary>
	''' <returns>Original positon of tab</returns>
	Public ReadOnly Property OldPosition As Integer
	Get
		Return _posO
	End Get
	End Property
End Class

''' <summary>
''' Event class for dealing with when 1 or more 2 margins or indents are changed by the user
''' </summary>
Public Class MarginOrIndentEventArgs
	Inherits EventArgs

	'   private variables

	Private _MarkerType As TextRuler.MarkerType

	'   public components

	'      constructor

	''' <summary>
	''' Set up margin/ident type
	''' </summary>
	''' <param name="MarkerType">Type of margin or indent</param>
	Public Sub New(ByVal MarkerType As TextRuler.MarkerType)
	_MarkerType = MarkerType
	End Sub

	'      properties

	''' <summary>
	''' Get type of margin or indent(s)
	''' </summary>
	''' <returns>Margin/indent(s) type</returns>
	Public ReadOnly Property MarkerType As TextRuler.MarkerType
	Get
		Return _MarkerType
	End Get
	End Property
End Class
