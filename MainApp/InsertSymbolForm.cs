using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainApp
{
    public partial class InsertSymbolForm : Form
    {
        internal Font SymbolFont { get; set; }        // INPUT/OUTPUT: Font of symbol
        internal string SymbolCharacter { get; set; } // OUTPUT: Character for symbol desired

        public InsertSymbolForm(Font SymbolFont)
        {
            this.SymbolFont = SymbolFont; 
            this.SymbolCharacter = " ";
            InitializeComponent();
        }

        private const int CharactersAtATime = 1024;
        private const int TotalCharacters = 65536;
        private const int CharactersPerRow = 32;
        private bool pvtPopulating = false; // status flags
        private bool pvtValidChar = false;
        private char[] pvtSymbolList; // printable characters

        private void InsertSymbolForm_Load(object sender, EventArgs e)
        {
            pvtSymbolList = GetPrintableCharacters();

            cbxSymbolRange.Items.Clear();

            for (int StartCode = 0; StartCode <= pvtSymbolList.Length - 1; StartCode+= CharactersAtATime)
            {
                int EndCode = Math.Min(pvtSymbolList.Length, StartCode + CharactersAtATime) - 1;
                cbxSymbolRange.Items.Add(((int)pvtSymbolList[StartCode]).ToString()
                          + "-" + ((int)pvtSymbolList[EndCode]).ToString());
            }
            cbxSymbolRange.SelectedIndex = 0;
            NewFont(this.SymbolFont);
        }

        private char[] GetPrintableCharacters()
        {
            // go through Unicode character list
            string SymbolList = "";
            for (int SymbolIndex = 0, loopTo = TotalCharacters - 1; SymbolIndex <= loopTo; SymbolIndex++)
            {
                // check this character
                char Character = (char)SymbolIndex;
                switch (char.GetUnicodeCategory(Character))
                {
                    case UnicodeCategory.PrivateUse:
                    case UnicodeCategory.Format:
                    case UnicodeCategory.OtherNotAssigned:
                        {
                            // character unavailable
                            continue;
                        }
                }
                switch (true)
                {
                    //case object _ when Character == ' ':
                    //    {
                    //        // space--printable
                    //        SymbolList += Character.ToString();
                    //        break;
                    //    }

                    case object _ when char.IsControl(Character):
                    case object _ when char.IsSeparator(Character):
                    case object _ when char.IsWhiteSpace(Character):
                    case object _ when char.IsSurrogate(Character):
                        {
                            // character not renderable
                            continue;
                        }

                    default:
                        {
                            // printable character
                            SymbolList += Character.ToString();
                            break;
                        }
                }
            }
            // return with list of characters
            return SymbolList.ToCharArray();
        }
        private void NewFont(Font NewFont)
        {
            // change font
            {                
                dgvSymbols.SuspendLayout();
                // set new font
                dgvSymbols.Font = new Font(NewFont.Name, dgvSymbols.DefaultCellStyle.Font.SizeInPoints, NewFont.Style);
                dgvSymbols.ResumeLayout();
                // display font info
                lblSymbolFont.Text =$"Font: {NewFont.Name};{NewFont.SizeInPoints} points; {NewFont.Style}";
            }
        }
        private void dgvSymbols_CurrentCellChanged(object sender, EventArgs e)
        {
            // cell clicked

            if (!(pvtPopulating || dgvSymbols.CurrentCell.Value == null))
            {
                string character = dgvSymbols.CurrentCell.Value.ToString();
                pvtValidChar = !string.IsNullOrEmpty(character);
                if (pvtValidChar)
                {
                    // character chosen
                    this.SymbolCharacter = character; 
                    IndicateSymbol(this.SymbolCharacter);
                }
            }
        }
        private void IndicateSymbol(string NewSymbol)
        {
            lblSymbol.Text = "Symbol: \"" + NewSymbol + "\"   Shortcut: Alt+" + ((int)NewSymbol[0]).ToString();
        }
        private void dgvSymbols_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // cell double-clicked
            if (pvtValidChar)
                this.DialogResult = DialogResult.OK;// accept character
        }
        private void btnInsert_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK; // pick selected symbol
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; // ignore selected symbol
        }
        private void btnFont_Click(object sender, EventArgs e)
        {
            // change font           
               
                fntdlgInsert.Font = this.SymbolFont;
                if (fntdlgInsert.ShowDialog() == DialogResult.OK)
                {
                    // redraw grid
                    this.SymbolFont = fntdlgInsert.Font; 
                    NewFont(this.SymbolFont);
                    dgvSymbols.Select();
                }            
        }
        private void cbxSymbolRange_SelectedItemChanged(object sender, EventArgs e)
        {
            if (cbxSymbolRange.SelectedIndex > -1)
                NewGridRange(cbxSymbolRange.SelectedIndex);
        }
        private void NewGridRange(int Page)
        {
            dgvSymbols.SuspendLayout();
            pvtPopulating = true;
            int StartPos = Page * CharactersAtATime;
            int EndPos = Math.Min(pvtSymbolList.Length, StartPos + CharactersAtATime);
            int SymbolIndex = StartPos;
            int NumberOfRows = (int) Math.Ceiling((double)(EndPos - StartPos) / CharactersPerRow);

            var RowInfo = new string[CharactersPerRow - 1 + 1];
            dgvSymbols.ColumnCount = CharactersPerRow;

            dgvSymbols.Rows.Clear();
            for (int row = 0, loopTo = NumberOfRows - 1; row <= loopTo; row++)
            {
                for (int column = 0, loopTo1 = CharactersPerRow - 1; column <= loopTo1; column++)
                {
                    if (SymbolIndex >= EndPos)
                        RowInfo[column] = "";
                    else
                    {
                        RowInfo[column] = pvtSymbolList[SymbolIndex].ToString();
                        SymbolIndex += 1;
                    }
                }
                dgvSymbols.Rows.Add(RowInfo);
            }

            pvtPopulating = false;
            dgvSymbols.Invalidate();
            dgvSymbols.ResumeLayout();
            pvtValidChar = true;
            this.SymbolCharacter = pvtSymbolList[StartPos].ToString();
            IndicateSymbol(this.SymbolCharacter);
        }
    }
}
