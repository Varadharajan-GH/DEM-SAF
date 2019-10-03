Imports System.Text.RegularExpressions

Public Class frmInsertLink

	'   public components

	'      properties
	Friend CustomLinkInfo As CustomLinkInfo   'INPUT/OUTPUT: Visible and hyperlink text
	Friend Property LinkAction As LinkActions 'OUTPUT: Action to perform
	Friend Property KeepHypertext As Boolean  'OUTPUT: Whether to preserve hypertext on remove

	'      constructor

	Public Sub New(Byval CustomLinkInfo As CustomLinkInfo, _
		Byval Exists As Boolean, ByVal KeepHypertext As Boolean)
	'   This call is required by the designer.
	InitializeComponent()
	'   Initialize values
	Me.CustomLinkInfo = CustomLinkInfo
	With CustomLinkInfo
		txtText.Text = .Text : txtHyperlink.Text = .Hyperlink
	End With
	pvt_Exists = Exists
	ckbKeepHypertext.Visible = Exists : ckbKeepHypertext.Enabled = Exists
	If Exists Then
		'   existing link
		btnInsertOrUpdate.Text = "Update"
		btnRemove.Visible = True : btnRemove.Enabled = True
		ckbKeepHypertext.Checked = KeepHypertext
	End If
	End Sub



	'   private components

	'      variables

	Private pvt_Exists As Boolean = False

	'      event procedures


	Private Sub btnInsertOrUpdate_Click(sender As Object, e As EventArgs) _
		Handles btnInsertOrUpdate.Click
	'   prepare to insert/update link
	If GetTextAndHyperlink() Then
		'   return with text, hyperlink, and action
		If pvt_Exists Then
			'   pre-existing link
			Me.LinkAction = LinkActions.Update
		 Else
			'   new link
			Me.KeepHypertext = ckbKeepHypertext.Checked
			Me.LinkAction = LinkActions.Insert
		End If
		Me.DialogResult = DialogResult.OK
	 Else
		'   error
		Me.DialogResult = DialogResult.None
	End If
	End Sub

	Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
	'   don't change anything
	Me.LinkAction = LinkActions.None : Me.DialogResult = DialogResult.Cancel
	End Sub

	Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
	'   prepare to remove link
	Me.KeepHypertext = ckbKeepHypertext.Checked
	Me.LinkAction = LinkActions.Remove : Me.DialogResult = DialogResult.Ok
	End Sub

	Private Sub cmsHyperlink_ItemClicked(sender As Object, _
		e As ToolStripItemClickedEventArgs) Handles cmsHyperlinkScheme.ItemClicked
	'   insert scheme
	txtHyperlink.Text = e.ClickedItem.Text & txtHyperlink.Text
	End Sub

	Private Sub txtHyperlink_TextChanged(sender As Object, e As EventArgs) _
		Handles txtHyperlink.TextChanged
	'   make context menu available if hyperlink is missing a scheme 
	If IsSchemePresent(txtHyperlink.Text) Then
		'   scheme present--no menu
		txtHyperlink.ContextMenuStrip = New ContextMenuStrip()
	 Else
		'   no scheme--make menu available
		txtHyperlink.ContextMenuStrip = cmsHyperlinkScheme
	End If	
	End Sub

	Private Sub txtText_KeyDown(sender As Object, e As KeyEventArgs) _
		Handles txtText.KeyDown
	CheckForSpecialCharacters(txtText, e)	
	End Sub

	Private Sub txtHyperlink_KeyDown(sender As Object, e As KeyEventArgs) _
		Handles txtHyperlink.KeyDown
	CheckForSpecialCharacters(txtHyperlink, e)	
	End Sub



	'      non-event procedures

	Private Function IsValid(ByVal TextToValidate As String) As Boolean
	'   make sure text/hyperlink is valid
	Return _
		Not String.IsNullOrEmpty(TextToValidate.Trim()) _
			AndAlso Not RegEx.IsMatch(TextToValidate, "[\{\|\}]")
	End Function

	Private Function GetTextAndHyperlink() As Boolean
	'   get text and hyperlink if they are valid; return whether or not they are
	Dim Text As String = txtText.Text.Trim(), _
		Hyperlink As String = txtHyperlink.Text.Trim()
	If IsValid(Text) AndAlso IsValid(Hyperlink) Then
		'   valid--get values
		With Me.CustomLinkInfo
			.Text = Text : .Hyperlink = Hyperlink
		End With
		Return True
	 Else
		'   invalid
		Beep()
		MessageBox.Show("Invalid visible and/or hyperlink text!", _
			"Invalid Text!", MessageBoxButtons.OK, MessageBoxIcon.Error)
		Return False
	End If
	End Function

	Private Function IsSchemePresent(ByVal Text As String) As Boolean
	'   check to see if a pre-defined scheme is already present
	For Each CMSItem As ToolStripItem In cmsHyperlinkScheme.Items
		If TypeOf CMSItem IsNot ToolStripSeparator _
				AndAlso Text.StartsWith(CMSItem.Text, _
					StringComparison.CurrentCultureIgnoreCase) Then
			'   known scheme found
			Return True
		End If
	Next CMSItem
	'   unknown scheme found?
	Return _
		Text.Contains("://")
	End Function
End Class



'   performed actions upon exit

Friend Enum LinkActions
	None
	Insert
	Update
	Remove
End Enum