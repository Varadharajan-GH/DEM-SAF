<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSearch
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSearch))
		Me.lblSearchText = New System.Windows.Forms.Label()
		Me.txtSearchText = New System.Windows.Forms.TextBox()
		Me.btnFind = New System.Windows.Forms.Button()
		Me.btnCancel = New System.Windows.Forms.Button()
		Me.lblReplacementText = New System.Windows.Forms.Label()
		Me.txtReplacementText = New System.Windows.Forms.TextBox()
		Me.btnReplaceAll = New System.Windows.Forms.Button()
		Me.btnReplace = New System.Windows.Forms.Button()
		Me.gbxSearchOptions = New System.Windows.Forms.GroupBox()
		Me.rdoSearchDown = New System.Windows.Forms.RadioButton()
		Me.rdoSearchUp = New System.Windows.Forms.RadioButton()
		Me.chkMatchCase = New System.Windows.Forms.CheckBox()
		Me.chkMatchWholeWord = New System.Windows.Forms.CheckBox()
		Me.gbxSearchOptions.SuspendLayout
		Me.SuspendLayout
		'
		'lblSearchText
		'
		resources.ApplyResources(Me.lblSearchText, "lblSearchText")
		Me.lblSearchText.Name = "lblSearchText"
		'
		'txtSearchText
		'
		resources.ApplyResources(Me.txtSearchText, "txtSearchText")
		Me.txtSearchText.Name = "txtSearchText"
		'
		'btnFind
		'
		resources.ApplyResources(Me.btnFind, "btnFind")
		Me.btnFind.Name = "btnFind"
		Me.btnFind.UseVisualStyleBackColor = true
		'
		'btnCancel
		'
		Me.btnCancel.CausesValidation = false
		Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		resources.ApplyResources(Me.btnCancel, "btnCancel")
		Me.btnCancel.Name = "btnCancel"
		Me.btnCancel.UseVisualStyleBackColor = true
		'
		'lblReplacementText
		'
		resources.ApplyResources(Me.lblReplacementText, "lblReplacementText")
		Me.lblReplacementText.Name = "lblReplacementText"
		'
		'txtReplacementText
		'
		resources.ApplyResources(Me.txtReplacementText, "txtReplacementText")
		Me.txtReplacementText.Name = "txtReplacementText"
		'
		'btnReplaceAll
		'
		resources.ApplyResources(Me.btnReplaceAll, "btnReplaceAll")
		Me.btnReplaceAll.Name = "btnReplaceAll"
		Me.btnReplaceAll.UseVisualStyleBackColor = true
		'
		'btnReplace
		'
		resources.ApplyResources(Me.btnReplace, "btnReplace")
		Me.btnReplace.Name = "btnReplace"
		Me.btnReplace.UseVisualStyleBackColor = true
		'
		'gbxSearchOptions
		'
		Me.gbxSearchOptions.Controls.Add(Me.rdoSearchDown)
		Me.gbxSearchOptions.Controls.Add(Me.rdoSearchUp)
		Me.gbxSearchOptions.Controls.Add(Me.chkMatchCase)
		Me.gbxSearchOptions.Controls.Add(Me.chkMatchWholeWord)
		resources.ApplyResources(Me.gbxSearchOptions, "gbxSearchOptions")
		Me.gbxSearchOptions.Name = "gbxSearchOptions"
		Me.gbxSearchOptions.TabStop = false
		'
		'rdoSearchDown
		'
		resources.ApplyResources(Me.rdoSearchDown, "rdoSearchDown")
		Me.rdoSearchDown.CausesValidation = false
		Me.rdoSearchDown.Name = "rdoSearchDown"
		Me.rdoSearchDown.TabStop = true
		Me.rdoSearchDown.UseVisualStyleBackColor = true
		'
		'rdoSearchUp
		'
		resources.ApplyResources(Me.rdoSearchUp, "rdoSearchUp")
		Me.rdoSearchUp.CausesValidation = false
		Me.rdoSearchUp.Name = "rdoSearchUp"
		Me.rdoSearchUp.TabStop = true
		Me.rdoSearchUp.UseVisualStyleBackColor = true
		'
		'chkMatchCase
		'
		resources.ApplyResources(Me.chkMatchCase, "chkMatchCase")
		Me.chkMatchCase.CausesValidation = false
		Me.chkMatchCase.Name = "chkMatchCase"
		Me.chkMatchCase.UseVisualStyleBackColor = true
		'
		'chkMatchWholeWord
		'
		resources.ApplyResources(Me.chkMatchWholeWord, "chkMatchWholeWord")
		Me.chkMatchWholeWord.CausesValidation = false
		Me.chkMatchWholeWord.Name = "chkMatchWholeWord"
		Me.chkMatchWholeWord.UseVisualStyleBackColor = true
		'
		'frmSearch
		'
		Me.AcceptButton = Me.btnFind
		resources.ApplyResources(Me, "$this")
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.btnCancel
		Me.Controls.Add(Me.gbxSearchOptions)
		Me.Controls.Add(Me.btnReplace)
		Me.Controls.Add(Me.btnReplaceAll)
		Me.Controls.Add(Me.txtReplacementText)
		Me.Controls.Add(Me.lblReplacementText)
		Me.Controls.Add(Me.btnCancel)
		Me.Controls.Add(Me.btnFind)
		Me.Controls.Add(Me.txtSearchText)
		Me.Controls.Add(Me.lblSearchText)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
		Me.MaximizeBox = false
		Me.MinimizeBox = false
		Me.Name = "frmSearch"
		Me.gbxSearchOptions.ResumeLayout(false)
		Me.gbxSearchOptions.PerformLayout
		Me.ResumeLayout(false)
		Me.PerformLayout

End Sub
    Friend WithEvents lblSearchText As System.Windows.Forms.Label
    Friend WithEvents txtSearchText As System.Windows.Forms.TextBox
    Friend WithEvents btnFind As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
	Friend WithEvents lblReplacementText As Label
	Friend WithEvents txtReplacementText As TextBox
	Friend WithEvents btnReplaceAll As Button
	Friend WithEvents btnReplace As Button
	Friend WithEvents gbxSearchOptions As GroupBox
	Friend WithEvents rdoSearchDown As RadioButton
	Friend WithEvents rdoSearchUp As RadioButton
	Friend WithEvents chkMatchCase As CheckBox
	Friend WithEvents chkMatchWholeWord As CheckBox
End Class
