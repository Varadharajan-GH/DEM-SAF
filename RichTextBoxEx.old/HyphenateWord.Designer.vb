<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmHyphenate
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmHyphenate))
		Me.lblHyphenatedWord = New System.Windows.Forms.Label()
		Me.lblWord = New System.Windows.Forms.Label()
		Me.btnHyphenate = New System.Windows.Forms.Button()
		Me.btnSkip = New System.Windows.Forms.Button()
		Me.btnCancel = New System.Windows.Forms.Button()
		Me.txtWord = New System.Windows.Forms.TextBox()
		Me.SuspendLayout
		'
		'lblHyphenatedWord
		'
		resources.ApplyResources(Me.lblHyphenatedWord, "lblHyphenatedWord")
		Me.lblHyphenatedWord.Name = "lblHyphenatedWord"
		'
		'lblWord
		'
		resources.ApplyResources(Me.lblWord, "lblWord")
		Me.lblWord.Name = "lblWord"
		'
		'btnHyphenate
		'
		resources.ApplyResources(Me.btnHyphenate, "btnHyphenate")
		Me.btnHyphenate.Name = "btnHyphenate"
		Me.btnHyphenate.UseVisualStyleBackColor = true
		'
		'btnSkip
		'
		resources.ApplyResources(Me.btnSkip, "btnSkip")
		Me.btnSkip.Name = "btnSkip"
		Me.btnSkip.UseVisualStyleBackColor = true
		'
		'btnCancel
		'
		Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		resources.ApplyResources(Me.btnCancel, "btnCancel")
		Me.btnCancel.Name = "btnCancel"
		Me.btnCancel.UseVisualStyleBackColor = true
		'
		'txtWord
		'
		resources.ApplyResources(Me.txtWord, "txtWord")
		Me.txtWord.Name = "txtWord"
		Me.txtWord.ReadOnly = true
		'
		'frmHyphenate
		'
		resources.ApplyResources(Me, "$this")
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.btnCancel
		Me.Controls.Add(Me.txtWord)
		Me.Controls.Add(Me.btnCancel)
		Me.Controls.Add(Me.btnSkip)
		Me.Controls.Add(Me.btnHyphenate)
		Me.Controls.Add(Me.lblWord)
		Me.Controls.Add(Me.lblHyphenatedWord)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
		Me.MaximizeBox = false
		Me.MinimizeBox = false
		Me.Name = "frmHyphenate"
		Me.ResumeLayout(false)
		Me.PerformLayout

End Sub
    Friend WithEvents lblHyphenatedWord As System.Windows.Forms.Label
    Friend WithEvents lblWord As System.Windows.Forms.Label
    Friend WithEvents btnHyphenate As System.Windows.Forms.Button
    Friend WithEvents btnSkip As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents txtWord As System.Windows.Forms.TextBox
End Class
