
Friend Class frmHyphenate

	'   public components

	'      properties

	Friend Word As String             'INPUT: Text to hyphenate
	Friend MaxPosition As Integer     'INPUT: Maximum hyphenation position
	Friend DesiredPosition As Integer 'OUTPUT: Desired hyphenation position

	'      constructor

	Friend Sub New(ByVal Word As String, ByVal MaxPosition As Integer)
	'   set parameters
	Me.Word = Word : Me.MaxPosition = MaxPosition : Me.DesiredPosition = 0
	pvtPendingDesiredPosition = MaxPosition
	'   this call is required by the designer.
	InitializeComponent()
	End Sub



	'   private components

	'      private variables 

	Private pvtPendingDesiredPosition As Integer = 0

	'      event procedures

	Private Sub frmHyphenate_Load(sender As System.Object,  _
		e As System.EventArgs) Handles MyBase.Load
	txtWord.Text = Me.Word : txtWord.Select(pvtPendingDesiredPosition, 0)
	UpdateHyphenationStatus()
	End Sub

	Private Sub txtWord_KeyUp(sender As Object, _
		e As System.Windows.Forms.KeyEventArgs) Handles txtWord.KeyUp
	Select Case e.KeyCode
	  Case Keys.OemMinus, Keys.Subtract
		  '   "-"" was pressed--hyphenate here
		  btnHyphenate_Click(sender, e)
	  Case Else
		  '   update display status
		  UpdateHyphenationStatus()
	End Select
	End Sub
	
	Private Sub txtWord_MouseDown(sender As Object, _
		e As System.Windows.Forms.MouseEventArgs) Handles txtWord.MouseDown
	UpdateHyphenationStatus()
	End Sub

	Private Sub btnCancel_Click(sender As Object, e As System.EventArgs) Handles btnCancel.Click
	'   stop looking for words to hpyhenate	
	DialogResult = Windows.Forms.DialogResult.Cancel
	End Sub

	Private Sub btnHyphenate_Click(sender As Object, _
		e As System.EventArgs) Handles btnHyphenate.Click
	'   hyphenate this word at given position
	Me.DesiredPosition = pvtPendingDesiredPosition
	DialogResult = Windows.Forms.DialogResult.OK
	End Sub

	Private Sub btnSkip_Click(sender As Object, e As System.EventArgs) Handles btnSkip.Click
	'   leave this word un-hyphenated
	Me.DesiredPosition = 0
	DialogResult = Windows.Forms.DialogResult.OK
	End Sub

	'      NON-EVENT PROCEDURES

	Private Sub UpdateHyphenationStatus()
	'   get and normalize hyphenation point
	txtWord.SuspendLayout
	pvtPendingDesiredPosition = txtWord.SelectionStart
	If pvtPendingDesiredPosition < 2 Then
		'   must have at least 1 char on top line
		pvtPendingDesiredPosition = 2
	 ElseIf pvtPendingDesiredPosition > Me.MaxPosition Then
		'   must not go past end of top line
		pvtPendingDesiredPosition = Me.MaxPosition
	End If
	txtWord.Select(pvtPendingDesiredPosition, 0) : txtWord.Refresh
	'   display broken text
	Dim HyphenatedText As String = "Word as HYPHENATED:" _
		& ControlChars.CrLf & Me.Word.Insert(pvtPendingDesiredPosition, "-" & ControlChars.CrLf)
	Using g As Graphics = txtWord.CreateGraphics()
		With lblHyphenatedWord
			Dim s As SizeF = g.MeasureString(HyphenatedText, .Font)
			.Width = CInt(s.Width) : .Height = CInt(.Height)
			.Text = HyphenatedText
		End With
	End Using
	End Sub
End Class