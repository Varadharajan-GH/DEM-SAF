Friend Class frmSearch

	'   public components

	'      properties

	Friend Property AllowingReplacing As Boolean  'INPUT: Whether to allow replacing of text
	Friend Property SearchInfo As SearchCriteria  'INPUT: Search criteria
	Friend Property SearchOption As SearchOptions 'OUTPUT: Action to perform

	'      constructor

	Friend Sub New(ByVal SearchInfo As SearchCriteria, ByVal AllowingReplacing As Boolean)
	'   set paracters
	Me.SearchInfo = SearchInfo : Me.AllowingReplacing = AllowingReplacing
	'   this call is required by the designer.
	InitializeComponent()
	End Sub



	'   private components

	'      private variables

	Private pvt_Initializing As Boolean = True

	'      event procedures

	Private Sub rdoSearchUp_CheckedChanged(sender As System.Object,  e As System.EventArgs) _
		
	With SearchInfo
		If rdoSearchUp.Checked Then
			'   search up (backward)
			.SearchFinds = (.SearchFinds Or RichTextBoxFinds.Reverse)
		 Else
			'   search down (forward)
			.SearchFinds = (.SearchFinds And Not RichTextBoxFinds.Reverse)	
		End If
	End With
	End Sub

	Private Sub rdoSearchUp_Click(sender As Object, e As System.EventArgs) 
	rdoSearchUp_CheckedChanged(sender, e)
	End Sub

	Private Sub rdoSearchDown_CheckedChanged(sender As System.Object,  e As System.EventArgs) _
		Handles rdoSearchUp.CheckedChanged
	rdoSearchUp_CheckedChanged(sender, e)
	End Sub

	Private Sub rdoSearchDown_Click(sender As Object, e As System.EventArgs) _
		Handles rdoSearchUp.Click
	rdoSearchUp_CheckedChanged(sender, e)
	End Sub

	Private Sub chkMatchCase_CheckedChanged(sender As Object, e As System.EventArgs) _
		
	With SearchInfo
		If chkMatchCase.Checked Then
			'   case sensitive search
			.SearchFinds = (.SearchFinds Or RichTextBoxFinds.MatchCase)
		 Else
			'   case insensitive search
			.SearchFinds = (.SearchFinds And Not RichTextBoxFinds.MatchCase)
		End If
	End With
	End Sub
	
	Private Sub chkMatchWholeWord_CheckedChanged(sender As Object, e As System.EventArgs) _
		
	With SearchInfo
		If chkMatchWholeWord.Checked Then
			'   find whole word
			.SearchFinds = (.SearchFinds Or RichTextBoxFinds.WholeWord)
		 Else
			'   find any match
			.SearchFinds = (.SearchFinds And Not RichTextBoxFinds.WholeWord)
		End If
	End With
	End Sub

	Private Sub txtSearchText_KeyDown(sender As Object, e As KeyEventArgs) _
		Handles txtSearchText.KeyDown
	CheckForSpecialCharacters(txtSearchText, e)
	End Sub

	Private Sub txtReplacementText_KeyDown(sender As Object, e As KeyEventArgs) _
		Handles txtReplacementText.KeyDown
	CheckForSpecialCharacters(txtReplacementText, e)
	End Sub

	Private Sub txtSearchText_TextChanged(sender As Object, e As EventArgs) _
		Handles txtSearchText.TextChanged
	ShowOrHideControls() : CheckIfInitializing()
	End Sub

	Private Sub txtReplacementText_TextChanged(sender As Object, e As EventArgs) _
		Handles txtReplacementText.TextChanged
	If AllowingReplacing Then
		CheckIfInitializing()
	End If
	End Sub

	Private Sub btnCancel_Click(sender As Object, e As System.EventArgs) _
		Handles btnCancel.Click
	'   abort search dialog
	SearchInfo.IsFirstFind = True
	SearchOption = SearchOptions.None : DialogResult = Windows.Forms.DialogResult.Cancel
	End Sub

	Private Sub btnFind_Click(sender As Object, e As System.EventArgs) _
		Handles btnFind.Click
	'   find next occurrence
	SetText()
	SearchOption = SearchOptions.FindNext : DialogResult = Windows.Forms.DialogResult.Ok
	End Sub

	Private Sub btnReplace_Click(sender As Object, e As System.EventArgs) _
		Handles btnReplace.Click
	'   replace current occurrence
	SetText()
	SearchOption = SearchOptions.Replace : DialogResult = Windows.Forms.DialogResult.Ok
	End Sub

	Private Sub btnReplaceAll_Click(sender As Object, e As System.EventArgs) _
		Handles btnReplaceAll.Click
	'   replace all occurrences from here on
	SetText()
	SearchOption = SearchOptions.ReplaceAll :	DialogResult = Windows.Forms.DialogResult.Ok
	End Sub

	Private Sub frmSearch_Load(sender As Object, e As System.EventArgs) Handles Me.Load
	'   set up controls for search and replacement text
	pvt_Initializing = True
	With SearchInfo
		txtSearchText.Text = .SearchText : txtReplacementText.Text = .ReplacementText
		ShowOrHideControls()
		'   set check boxes
		chkMatchCase.Checked = CBool(.SearchFinds And RichTextBoxFinds.MatchCase)
		chkMatchWholeWord.Checked = CBool(.SearchFinds And RichTextBoxFinds.WholeWord)
		'   set search direction
		If (.SearchFinds And RichTextBoxFinds.Reverse) = 0 Then
			rdoSearchDown.Checked = True 'forward
		 Else
			rdoSearchUp.Checked = True   'backward
		End If
	End With
	pvt_Initializing = False
	End Sub

	'      NON-EVENT PROCEDURES

	'         hide, enabled, or disable certain controls
	Private Sub ShowOrHideControls
	'   is there anything to find?
	Dim AreEnabling As Boolean = Not String.IsNullOrEmpty(txtSearchText.Text)
	btnFind.Enabled = AreEnabling
	btnReplace.Enabled = AreEnabling :btnReplaceAll.Enabled = AreEnabling
	lblReplacementText.Enabled = AreEnabling : txtReplacementText.Enabled = AreEnabling
	'   can we replace?
	If AllowingReplacing Then
		'   find/replace   
		Me.Text = "Search/Replace Text"
		btnReplace.Visible = True : btnReplaceAll.Visible = True
		lblReplacementText.Visible = True : txtReplacementText.Visible = True
	 Else
		'   find only
		Dim Reduction As Integer = gbxSearchOptions.Top - lblReplacementText.Top
		gbxSearchOptions.Top -= Reduction : btnCancel.Top -= Reduction
		Me.Height -= Reduction
	End If
	End Sub

	'         see if we are initializing
	Public Sub CheckIfInitializing()
	If Not pvt_Initializing Then
		'   update info on RichTextBoxEx control
		SearchInfo.IsFirstFind = True
	End If
	End Sub

	'         set search and replacement text for host control
	Public Sub SetText()
	With SearchInfo
		.SearchText = txtSearchText.Text : .ReplacementText = txtReplacementText.Text
	End With
	End Sub
End Class



'   class for search criteria
Friend Class SearchCriteria

	'   properties

	Property SearchText As String            'Text to search for
	Property ReplacementText As String       'Text to replace with
	Property SearchFinds As RichTextBoxFinds 'How to search for text
	Property IsFirstFind As Boolean          'Whether or not we should be search
	                                         'for very first occurrence

	'   constructor

	Friend Sub New(Byval SearchText As String, ByVal ReplacementText As String, _
		ByVal SearchFinds As RichTextBoxFinds, ByVal IsFirstFind As Boolean)
	Me.SearchText = SearchText : Me.ReplacementText = ReplacementText
	Me.SearchFinds = SearchFinds : Me.IsFirstFind = IsFirstFind
	End Sub
End Class


	
'   performed actions upon exit
Friend Enum SearchOptions
	None
	FindNext
	Replace
	ReplaceAll
End Enum



'   module for handling special characters
Module SpecialCharacters
End Module