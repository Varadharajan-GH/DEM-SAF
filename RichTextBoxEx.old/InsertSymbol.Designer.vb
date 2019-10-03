<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmInsertSymbol
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
		Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmInsertSymbol))
		Me.dgvSymbols = New System.Windows.Forms.DataGridView()
		Me.btnInsert = New System.Windows.Forms.Button()
		Me.btnFont = New System.Windows.Forms.Button()
		Me.btnCnacel = New System.Windows.Forms.Button()
		Me.lblSymbol = New System.Windows.Forms.Label()
		Me.lblSymbolFont = New System.Windows.Forms.Label()
		Me.fntdlgInsert = New System.Windows.Forms.FontDialog()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.cbxSymbolRange = New System.Windows.Forms.ComboBox()
		CType(Me.dgvSymbols,System.ComponentModel.ISupportInitialize).BeginInit
		Me.SuspendLayout
		'
		'dgvSymbols
		'
		Me.dgvSymbols.AllowUserToAddRows = false
		Me.dgvSymbols.AllowUserToDeleteRows = false
		Me.dgvSymbols.AllowUserToResizeColumns = false
		Me.dgvSymbols.AllowUserToResizeRows = false
		Me.dgvSymbols.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
		Me.dgvSymbols.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
		Me.dgvSymbols.BackgroundColor = System.Drawing.Color.White
		Me.dgvSymbols.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvSymbols.ColumnHeadersVisible = false
		DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
		DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window
		DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
		DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText
		DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
		DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
		DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
		Me.dgvSymbols.DefaultCellStyle = DataGridViewCellStyle1
		Me.dgvSymbols.Dock = System.Windows.Forms.DockStyle.Top
		Me.dgvSymbols.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
		Me.dgvSymbols.Location = New System.Drawing.Point(0, 0)
		Me.dgvSymbols.MultiSelect = false
		Me.dgvSymbols.Name = "dgvSymbols"
		Me.dgvSymbols.RowHeadersVisible = false
		Me.dgvSymbols.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
		Me.dgvSymbols.Size = New System.Drawing.Size(721, 150)
		Me.dgvSymbols.TabIndex = 0
		'
		'btnInsert
		'
		Me.btnInsert.Location = New System.Drawing.Point(201, 245)
		Me.btnInsert.Name = "btnInsert"
		Me.btnInsert.Size = New System.Drawing.Size(75, 23)
		Me.btnInsert.TabIndex = 6
		Me.btnInsert.Text = "Insert"
		Me.btnInsert.UseVisualStyleBackColor = true
		'
		'btnFont
		'
		Me.btnFont.Location = New System.Drawing.Point(449, 164)
		Me.btnFont.Name = "btnFont"
		Me.btnFont.Size = New System.Drawing.Size(75, 23)
		Me.btnFont.TabIndex = 3
		Me.btnFont.Text = "Font..."
		Me.btnFont.UseVisualStyleBackColor = true
		'
		'btnCnacel
		'
		Me.btnCnacel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.btnCnacel.Location = New System.Drawing.Point(445, 245)
		Me.btnCnacel.Name = "btnCnacel"
		Me.btnCnacel.Size = New System.Drawing.Size(75, 23)
		Me.btnCnacel.TabIndex = 7
		Me.btnCnacel.Text = "Cencel"
		Me.btnCnacel.UseVisualStyleBackColor = true
		'
		'lblSymbol
		'
		Me.lblSymbol.AutoSize = true
		Me.lblSymbol.Location = New System.Drawing.Point(12, 201)
		Me.lblSymbol.Name = "lblSymbol"
		Me.lblSymbol.Size = New System.Drawing.Size(44, 13)
		Me.lblSymbol.TabIndex = 4
		Me.lblSymbol.Text = "Symbol:"
		'
		'lblSymbolFont
		'
		Me.lblSymbolFont.AutoSize = true
		Me.lblSymbolFont.Location = New System.Drawing.Point(12, 216)
		Me.lblSymbolFont.Name = "lblSymbolFont"
		Me.lblSymbolFont.Size = New System.Drawing.Size(31, 13)
		Me.lblSymbolFont.TabIndex = 5
		Me.lblSymbolFont.Text = "Font:"
		'
		'Label1
		'
		Me.Label1.AutoSize = true
		Me.Label1.Location = New System.Drawing.Point(197, 169)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(119, 13)
		Me.Label1.TabIndex = 1
		Me.Label1.Text = "Character Code Range:"
		'
		'cbxSymbolRange
		'
		Me.cbxSymbolRange.FormattingEnabled = true
		Me.cbxSymbolRange.Location = New System.Drawing.Point(322, 166)
		Me.cbxSymbolRange.Name = "cbxSymbolRange"
		Me.cbxSymbolRange.Size = New System.Drawing.Size(106, 21)
		Me.cbxSymbolRange.TabIndex = 2
		'
		'frmInsertSymbol
		'
		Me.AcceptButton = Me.btnInsert
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.btnCnacel
		Me.ClientSize = New System.Drawing.Size(721, 278)
		Me.Controls.Add(Me.cbxSymbolRange)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.lblSymbolFont)
		Me.Controls.Add(Me.lblSymbol)
		Me.Controls.Add(Me.btnCnacel)
		Me.Controls.Add(Me.btnFont)
		Me.Controls.Add(Me.btnInsert)
		Me.Controls.Add(Me.dgvSymbols)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
		Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
		Me.Name = "frmInsertSymbol"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Insert Symbol"
		CType(Me.dgvSymbols,System.ComponentModel.ISupportInitialize).EndInit
		Me.ResumeLayout(false)
		Me.PerformLayout

End Sub

	Friend WithEvents dgvSymbols As DataGridView
	Friend WithEvents btnInsert As Button
	Friend WithEvents btnFont As Button
	Friend WithEvents btnCnacel As Button
	Friend WithEvents lblSymbol As Label
	Friend WithEvents lblSymbolFont As Label
	Friend WithEvents fntdlgInsert As FontDialog
	Friend WithEvents Label1 As Label
	Friend WithEvents cbxSymbolRange As ComboBox
End Class
