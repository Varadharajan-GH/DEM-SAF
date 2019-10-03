<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmInsertLink
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
		Me.components = New System.ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmInsertLink))
		Me.lblText = New System.Windows.Forms.Label()
		Me.lblHyperlink = New System.Windows.Forms.Label()
		Me.txtText = New System.Windows.Forms.TextBox()
		Me.txtHyperlink = New System.Windows.Forms.TextBox()
		Me.cmsHyperlinkScheme = New System.Windows.Forms.ContextMenuStrip(Me.components)
		Me.tsmiFile = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsmiFtp = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsmiGopher = New System.Windows.Forms.ToolStripMenuItem()
		Me.tss1 = New System.Windows.Forms.ToolStripSeparator()
		Me.tsmiHttp = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsmiHttpWWW = New System.Windows.Forms.ToolStripMenuItem()
		Me.tss2 = New System.Windows.Forms.ToolStripSeparator()
		Me.tsmiHttps = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsmiHttpsWWW = New System.Windows.Forms.ToolStripMenuItem()
		Me.tss3 = New System.Windows.Forms.ToolStripSeparator()
		Me.tsmiMailto = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsmiNews = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsmiNntp = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsmiNetPipe = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsmiNetTcp = New System.Windows.Forms.ToolStripMenuItem()
		Me.tss4 = New System.Windows.Forms.ToolStripSeparator()
		Me.tsmiWWW = New System.Windows.Forms.ToolStripMenuItem()
		Me.btnInsertOrUpdate = New System.Windows.Forms.Button()
		Me.btnCancel = New System.Windows.Forms.Button()
		Me.btnRemove = New System.Windows.Forms.Button()
		Me.ckbKeepHypertext = New System.Windows.Forms.CheckBox()
		Me.cmsHyperlinkScheme.SuspendLayout
		Me.SuspendLayout
		'
		'lblText
		'
		Me.lblText.AutoSize = true
		Me.lblText.Location = New System.Drawing.Point(21, 9)
		Me.lblText.Name = "lblText"
		Me.lblText.Size = New System.Drawing.Size(64, 13)
		Me.lblText.TabIndex = 0
		Me.lblText.Text = "Visible &Text:"
		'
		'lblHyperlink
		'
		Me.lblHyperlink.AutoSize = true
		Me.lblHyperlink.Location = New System.Drawing.Point(21, 39)
		Me.lblHyperlink.Name = "lblHyperlink"
		Me.lblHyperlink.Size = New System.Drawing.Size(51, 13)
		Me.lblHyperlink.TabIndex = 2
		Me.lblHyperlink.Text = "&Hyperlink"
		'
		'txtText
		'
		Me.txtText.Location = New System.Drawing.Point(91, 6)
		Me.txtText.Name = "txtText"
		Me.txtText.Size = New System.Drawing.Size(208, 20)
		Me.txtText.TabIndex = 1
		'
		'txtHyperlink
		'
		Me.txtHyperlink.ContextMenuStrip = Me.cmsHyperlinkScheme
		Me.txtHyperlink.Location = New System.Drawing.Point(91, 36)
		Me.txtHyperlink.Name = "txtHyperlink"
		Me.txtHyperlink.Size = New System.Drawing.Size(208, 20)
		Me.txtHyperlink.TabIndex = 3
		'
		'cmsHyperlinkScheme
		'
		Me.cmsHyperlinkScheme.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiFile, Me.tsmiFtp, Me.tsmiGopher, Me.tss1, Me.tsmiHttp, Me.tsmiHttpWWW, Me.tss2, Me.tsmiHttps, Me.tsmiHttpsWWW, Me.tss3, Me.tsmiMailto, Me.tsmiNetPipe, Me.tsmiNetTcp, Me.tsmiNews, Me.tsmiNntp, Me.tss4, Me.tsmiWWW})
		Me.cmsHyperlinkScheme.Name = "cmsHyperlink"
		Me.cmsHyperlinkScheme.Size = New System.Drawing.Size(153, 336)
		'
		'tsmiFile
		'
		Me.tsmiFile.Name = "tsmiFile"
		Me.tsmiFile.Size = New System.Drawing.Size(152, 22)
		Me.tsmiFile.Text = "file://"
		'
		'tsmiFtp
		'
		Me.tsmiFtp.Name = "tsmiFtp"
		Me.tsmiFtp.Size = New System.Drawing.Size(152, 22)
		Me.tsmiFtp.Text = "ftp://"
		'
		'tsmiGopher
		'
		Me.tsmiGopher.Name = "tsmiGopher"
		Me.tsmiGopher.Size = New System.Drawing.Size(152, 22)
		Me.tsmiGopher.Text = "gopher://"
		'
		'tss1
		'
		Me.tss1.Name = "tss1"
		Me.tss1.Size = New System.Drawing.Size(149, 6)
		'
		'tsmiHttp
		'
		Me.tsmiHttp.Name = "tsmiHttp"
		Me.tsmiHttp.Size = New System.Drawing.Size(152, 22)
		Me.tsmiHttp.Text = "http://"
		'
		'tsmiHttpWWW
		'
		Me.tsmiHttpWWW.Name = "tsmiHttpWWW"
		Me.tsmiHttpWWW.Size = New System.Drawing.Size(152, 22)
		Me.tsmiHttpWWW.Text = "http://www."
		'
		'tss2
		'
		Me.tss2.Name = "tss2"
		Me.tss2.Size = New System.Drawing.Size(149, 6)
		'
		'tsmiHttps
		'
		Me.tsmiHttps.Name = "tsmiHttps"
		Me.tsmiHttps.Size = New System.Drawing.Size(152, 22)
		Me.tsmiHttps.Text = "https://"
		'
		'tsmiHttpsWWW
		'
		Me.tsmiHttpsWWW.Name = "tsmiHttpsWWW"
		Me.tsmiHttpsWWW.Size = New System.Drawing.Size(152, 22)
		Me.tsmiHttpsWWW.Text = "https://www."
		'
		'tss3
		'
		Me.tss3.Name = "tss3"
		Me.tss3.Size = New System.Drawing.Size(149, 6)
		'
		'tsmiMailto
		'
		Me.tsmiMailto.Name = "tsmiMailto"
		Me.tsmiMailto.Size = New System.Drawing.Size(152, 22)
		Me.tsmiMailto.Text = "mailto:"
		'
		'tsmiNews
		'
		Me.tsmiNews.Name = "tsmiNews"
		Me.tsmiNews.Size = New System.Drawing.Size(152, 22)
		Me.tsmiNews.Text = "news:"
		'
		'tsmiNntp
		'
		Me.tsmiNntp.Name = "tsmiNntp"
		Me.tsmiNntp.Size = New System.Drawing.Size(152, 22)
		Me.tsmiNntp.Text = "nntp://"
		'
		'tsmiNetPipe
		'
		Me.tsmiNetPipe.Name = "tsmiNetPipe"
		Me.tsmiNetPipe.Size = New System.Drawing.Size(152, 22)
		Me.tsmiNetPipe.Text = "net.pipe://"
		'
		'tsmiNetTcp
		'
		Me.tsmiNetTcp.Name = "tsmiNetTcp"
		Me.tsmiNetTcp.Size = New System.Drawing.Size(152, 22)
		Me.tsmiNetTcp.Text = "net.tcp://"
		'
		'tss4
		'
		Me.tss4.Name = "tss4"
		Me.tss4.Size = New System.Drawing.Size(149, 6)
		'
		'tsmiWWW
		'
		Me.tsmiWWW.Name = "tsmiWWW"
		Me.tsmiWWW.Size = New System.Drawing.Size(152, 22)
		Me.tsmiWWW.Text = "www."
		'
		'btnInsertOrUpdate
		'
		Me.btnInsertOrUpdate.Location = New System.Drawing.Point(24, 87)
		Me.btnInsertOrUpdate.Name = "btnInsertOrUpdate"
		Me.btnInsertOrUpdate.Size = New System.Drawing.Size(75, 23)
		Me.btnInsertOrUpdate.TabIndex = 5
		Me.btnInsertOrUpdate.Text = "&Insert"
		Me.btnInsertOrUpdate.UseVisualStyleBackColor = true
		'
		'btnCancel
		'
		Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.btnCancel.Location = New System.Drawing.Point(224, 87)
		Me.btnCancel.Name = "btnCancel"
		Me.btnCancel.Size = New System.Drawing.Size(75, 23)
		Me.btnCancel.TabIndex = 7
		Me.btnCancel.Text = "&Cancel"
		Me.btnCancel.UseVisualStyleBackColor = true
		'
		'btnRemove
		'
		Me.btnRemove.Enabled = false
		Me.btnRemove.Location = New System.Drawing.Point(124, 87)
		Me.btnRemove.Name = "btnRemove"
		Me.btnRemove.Size = New System.Drawing.Size(75, 23)
		Me.btnRemove.TabIndex = 6
		Me.btnRemove.Text = "&Remove"
		Me.btnRemove.UseVisualStyleBackColor = true
		Me.btnRemove.Visible = false
		'
		'ckbKeepHypertext
		'
		Me.ckbKeepHypertext.AutoSize = true
		Me.ckbKeepHypertext.Location = New System.Drawing.Point(57, 62)
		Me.ckbKeepHypertext.Name = "ckbKeepHypertext"
		Me.ckbKeepHypertext.Size = New System.Drawing.Size(181, 17)
		Me.ckbKeepHypertext.TabIndex = 4
		Me.ckbKeepHypertext.Text = "&Keep hypertext in text on remove"
		Me.ckbKeepHypertext.UseVisualStyleBackColor = true
		'
		'frmInsertLink
		'
		Me.AcceptButton = Me.btnInsertOrUpdate
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange
		Me.CancelButton = Me.btnCancel
		Me.ClientSize = New System.Drawing.Size(320, 116)
		Me.Controls.Add(Me.ckbKeepHypertext)
		Me.Controls.Add(Me.btnRemove)
		Me.Controls.Add(Me.btnCancel)
		Me.Controls.Add(Me.btnInsertOrUpdate)
		Me.Controls.Add(Me.txtHyperlink)
		Me.Controls.Add(Me.txtText)
		Me.Controls.Add(Me.lblHyperlink)
		Me.Controls.Add(Me.lblText)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
		Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
		Me.Name = "frmInsertLink"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Insert Link"
		Me.cmsHyperlinkScheme.ResumeLayout(false)
		Me.ResumeLayout(false)
		Me.PerformLayout

End Sub

	Friend WithEvents lblText As Label
	Friend WithEvents lblHyperlink As Label
	Friend WithEvents txtText As TextBox
	Friend WithEvents txtHyperlink As TextBox
	Friend WithEvents btnInsertOrUpdate As Button
	Friend WithEvents btnCancel As Button
	Friend WithEvents btnRemove As Button
	Friend WithEvents ckbKeepHypertext As CheckBox
	Friend WithEvents cmsHyperlinkScheme As ContextMenuStrip
	Friend WithEvents tsmiFile As ToolStripMenuItem
	Friend WithEvents tsmiFtp As ToolStripMenuItem
	Friend WithEvents tsmiGopher As ToolStripMenuItem
	Friend WithEvents tsmiHttp As ToolStripMenuItem
	Friend WithEvents tsmiHttpWWW As ToolStripMenuItem
	Friend WithEvents tsmiHttps As ToolStripMenuItem
	Friend WithEvents tsmiHttpsWWW As ToolStripMenuItem
	Friend WithEvents tsmiMailto As ToolStripMenuItem
	Friend WithEvents tsmiNews As ToolStripMenuItem
	Friend WithEvents tsmiNntp As ToolStripMenuItem
	Friend WithEvents tsmiNetPipe As ToolStripMenuItem
	Friend WithEvents tsmiNetTcp As ToolStripMenuItem
	Friend WithEvents tsmiWWW As ToolStripMenuItem
	Friend WithEvents tss1 As ToolStripSeparator
	Friend WithEvents tss2 As ToolStripSeparator
	Friend WithEvents tss3 As ToolStripSeparator
	Friend WithEvents tss4 As ToolStripSeparator
End Class
