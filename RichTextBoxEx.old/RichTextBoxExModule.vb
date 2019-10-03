Module RichTextBoxExModule
		'         see if keystrokes are given for special characters
		Public Sub CheckForSpecialCharacters(tb As TextBox, e As KeyEventArgs)
		Dim InsertionText As String = ""
		If Not e.Control Then
			Exit Sub ' [Ctrl] not pressed
		End If
		'   else check key combination
		Select Case e.KeyCode
			Case Keys.OemMinus, Keys.Subtract
				'   hyphens/dashes
				If e.Alt Then
					'   em dash ("—")
					'      -- [Ctrl] + [Alt] + [-]
					InsertionText = "—"
				 Else
					'   optional hyphen (display only when breaking line)
					'      -- [Ctrl] + [-]
					InsertionText = ChrW(173)
				End If
			Case Keys.Oemtilde
				'   left quotes
				If e.Shift Then 
					'   left double-quote
					'      -- [Ctrl] + [Shift] + [~]
					InsertionText = ChrW(8220)
				 Else
					'   left single-quote
					'      -- [Ctrl] + [`]
					InsertionText = ChrW(8216)
				End If
			Case Keys.OemQuotes
				'   right quotes
				If e.Shift Then
					'    right double-quote
					'      -- [Ctrl] + [Shift] + ["]
					InsertionText = ChrW(8221)
				Else
					'   right single-quote
					'      -- [Ctrl] + [']
					InsertionText = ChrW(8217)
				End If
			Case Keys.C
				'   copyright ("©")
				'      -- [Ctrl] + [Alt] + [C]
				If e.Alt Then
					InsertionText = "©"
				End If
			Case Keys.R
				'   registered trademark ("®")
				'      -- [Ctrl] + [Alt] + [RC]
				If e.Alt Then
					InsertionText = "®"
				End If
			Case Keys.T
				'   trademark ("™")
				'      -- [Ctrl] + [Alt] + [T]
				If e.Alt Then
					InsertionText = "™"
				End If
		End Select
		'   insert special character if one was given
		If Not String.IsNullOrEmpty(InsertionText) Then
			tb.SelectedText = InsertionText : e.SuppressKeyPress = True
		End If
		End Sub
End Module
