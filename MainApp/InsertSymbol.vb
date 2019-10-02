Friend Class frmInsertSymbol

	'   public components

	'      properties
	Friend Property SymbolFont As Font        'INPUT/OUTPUT: Font of symbol
	Friend Property SymbolCharacter As String 'OUTPUT: Character for symbol desired

	'      constructor

	Friend Sub New(Byval SymbolFont As Font)
	'   set parameter
	Me.SymbolFont = SymbolFont : Me.SymbolCharacter = " "
	'   this call is required by the designer.
	InitializeComponent()
	End Sub



	'   private components

	'      private constants

	Private Const CharactersAtATime As Integer = 1024, _
		TotalCharacters As Integer = 65536, CharactersPerRow As Integer = 32

	
	'      private variables
	
	Private pvtPopulating As Boolean = False, pvtValidChar As Boolean = False 'status flags
	Private pvtSymbolList() As Char 'printable characters

	'      event procedures

	Private Sub InsertSymbol_Load(sender As Object, e As EventArgs) Handles Me.Load
	'    get list of printable characters
	pvtSymbolList = GetPrintableCharacters()
	With cbxSymbolRange.Items
		.Clear
		For StartCode As Integer = 0 To pvtSymbolList.Length - 1 _
				Step CharactersAtATime
			'    get character code range for this page
			Dim EndCode As Integer = _
				Math.Min(pvtSymbolList.Length, StartCode + CharactersAtATime) - 1
			.Add(AscW(pvtSymbolList(StartCode)).ToString _
				  & "-" & AscW(pvtSymbolList(EndCode)).ToString)
		Next StartCode
	End With
	'   fill grid with characters and set font
	cbxSymbolRange.SelectedIndex = 0
	NewFont(Me.SymbolFont)
	End Sub

    Private Sub dgvSymbols_CurrentCellchanged(sender As Object, e As EventArgs) _
        Handles dgvSymbols.CurrentCellChanged
        '   cell clicked

        If Not (pvtPopulating OrElse dgvSymbols.CurrentCell.Value Is Nothing) Then
            Dim character As String = dgvSymbols.CurrentCell.Value.ToString
            pvtValidChar = Not String.IsNullOrEmpty(character)
            If pvtValidChar Then
                '   character chosen
                Me.SymbolCharacter = character : IndicateSymbol(Me.SymbolCharacter)
            End If
        End If

    End Sub

    Private Sub dgvSymbols_CellDoubleClick(sender As Object, _
		e As DataGridViewCellEventArgs) Handles dgvSymbols.CellDoubleClick
	'   cell double-clicked
	If pvtValidChar Then
		Me.DialogResult = DialogResult.OK 'accept character
	End If
	End Sub

	Private Sub btnInsert_Click(sender As Object, e As EventArgs) Handles btnInsert.Click
	Me.DialogResult = DialogResult.OK 'pick selected symbol
	End Sub

	Private Sub btnCnacel_Click(sender As Object, e As EventArgs) Handles btnCnacel.Click
	Me.DialogResult = DialogResult.Cancel 'ignore selected symbol
	End Sub

	Private Sub btnFont_Click(sender As Object, e As EventArgs) Handles btnFont.Click
	'   change font
	With fntdlgInsert
		.Font = Me.SymbolFont
		If .ShowDialog = DialogResult.OK Then
			'   redraw grid
			Me.SymbolFont = .Font : NewFont(Me.SymbolFont)
			dgvSymbols.Select()
		End If
	End With
	End Sub
	
	Private Sub cbxSymbolRange_SelectedItemChanged(sender As Object, e As EventArgs) _
		Handles cbxSymbolRange.SelectedIndexChanged
	If cbxSymbolRange.SelectedIndex > -1 Then
		NewGridRange(cbxSymbolRange.SelectedIndex)	
	End If
	End Sub

	'      NON-EVENT procedures

	'         change grid font and character range
	Private Sub NewGridRange(Byval Page As Integer)

        dgvSymbols.SuspendLayout
        pvtPopulating = True

        Dim StartPos As Integer = Page * CharactersAtATime,
            EndPos As Integer = Math.Min(pvtSymbolList.Length, StartPos + CharactersAtATime),
            SymbolIndex As Integer = StartPos,
            NumberOfRows As Integer = Math.Ceiling((EndPos - StartPos) / CharactersPerRow)

        Dim RowInfo(CharactersPerRow - 1) As String
            dgvSymbols.ColumnCount = CharactersPerRow

        dgvSymbols.Rows.Clear
                For row As Integer = 0 To NumberOfRows - 1
            For column As Integer = 0 To CharactersPerRow - 1
                If SymbolIndex >= EndPos Then
                    RowInfo(column) = ""
                Else
                    RowInfo(column) = pvtSymbolList(SymbolIndex).ToString
                    SymbolIndex += 1
                End If
            Next column
            dgvSymbols.Rows.Add(RowInfo)
        Next row

        pvtPopulating = False
        dgvSymbols.Invalidate()
        dgvSymbols.ResumeLayout
        pvtValidChar = True
        Me.SymbolCharacter = pvtSymbolList(StartPos)
        IndicateSymbol(Me.SymbolCharacter)
    End Sub

	'         configure grid for new font
	Private Sub NewFont(NewFont As Font)
	'   change font
	With dgvSymbols
		.SuspendLayout
		'   set new font
		.Font = New Font(NewFont.Name, .DefaultCellStyle.Font.SizeInPoints, NewFont.Style)
		.ResumeLayout
		'   display font info
		lblSymbolFont.Text = "Font: " & NewFont.Name & "; " _
			& NewFont.SizeInPoints & " points; " & CType(NewFont.Style, FontStyle).ToString
	End With
	End Sub


	'         display current symbol info
	Private Sub IndicateSymbol(Byval NewSymbol As String)
	lblSymbol.Text = "Symbol: """ & NewSymbol.ToString _
		& """   Shortcut: Alt+" & AscW(NewSymbol(0)).ToString
	End Sub

	'         get string of printable characters
	Private Function GetPrintableCharacters() As Char()
	'   go through Unicode character list
	Dim SymbolList As String = ""
	For SymbolIndex As Integer = 0 To TotalCharacters - 1
		'   check this character
		Dim Character As Char = ChrW(SymbolIndex)
		Select Case Char.GetUnicodeCategory(Character)
		  Case Globalization.UnicodeCategory.PrivateUse, _
				  Globalization.UnicodeCategory.Format, _
				  Globalization.UnicodeCategory.OtherNotAssigned
			  '   character unavailable
			  Continue For
		End Select
		Select Case True
		  Case Character = " "c
			  '   space--printable
			  SymbolList &= Character
		  Case Char.IsControl(Character), Char.IsSeparator(Character), _
			 	  Char.IsWhiteSpace(Character), Char.IsSurrogate(Character)
			  '   character not renderable
			  Continue For
		  Case Else
			  '   printable character
			  SymbolList &= Character
		End Select
	Next SymbolIndex
	'   return with list of characters
	Return _
		SymbolList.ToCharArray
	End Function

End Class