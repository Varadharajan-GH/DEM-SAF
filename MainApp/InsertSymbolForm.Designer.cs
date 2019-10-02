using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MainApp
{

    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    partial class InsertSymbolForm : System.Windows.Forms.Form
    {

        // Form overrides dispose to clean up the component list.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                    components.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this._dgvSymbols = new System.Windows.Forms.DataGridView();
            this._btnInsert = new System.Windows.Forms.Button();
            this._btnFont = new System.Windows.Forms.Button();
            this._btnCnacel = new System.Windows.Forms.Button();
            this._lblSymbol = new System.Windows.Forms.Label();
            this._lblSymbolFont = new System.Windows.Forms.Label();
            this._fntdlgInsert = new System.Windows.Forms.FontDialog();
            this._Label1 = new System.Windows.Forms.Label();
            this._cbxSymbolRange = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this._dgvSymbols)).BeginInit();
            this.SuspendLayout();
            // 
            // _dgvSymbols
            // 
            this._dgvSymbols.AllowUserToAddRows = false;
            this._dgvSymbols.AllowUserToDeleteRows = false;
            this._dgvSymbols.AllowUserToResizeColumns = false;
            this._dgvSymbols.AllowUserToResizeRows = false;
            this._dgvSymbols.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this._dgvSymbols.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this._dgvSymbols.BackgroundColor = System.Drawing.Color.White;
            this._dgvSymbols.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvSymbols.ColumnHeadersVisible = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dgvSymbols.DefaultCellStyle = dataGridViewCellStyle1;
            this._dgvSymbols.Dock = System.Windows.Forms.DockStyle.Top;
            this._dgvSymbols.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this._dgvSymbols.Location = new System.Drawing.Point(0, 0);
            this._dgvSymbols.MultiSelect = false;
            this._dgvSymbols.Name = "_dgvSymbols";
            this._dgvSymbols.RowHeadersVisible = false;
            this._dgvSymbols.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this._dgvSymbols.Size = new System.Drawing.Size(721, 150);
            this._dgvSymbols.TabIndex = 0;
            this._dgvSymbols.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSymbols_CellDoubleClick);
            this._dgvSymbols.CurrentCellChanged += new System.EventHandler(this.dgvSymbols_CurrentCellChanged);
            // 
            // _btnInsert
            // 
            this._btnInsert.Location = new System.Drawing.Point(201, 245);
            this._btnInsert.Name = "_btnInsert";
            this._btnInsert.Size = new System.Drawing.Size(75, 23);
            this._btnInsert.TabIndex = 6;
            this._btnInsert.Text = "Insert";
            this._btnInsert.UseVisualStyleBackColor = true;
            this._btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // _btnFont
            // 
            this._btnFont.Location = new System.Drawing.Point(449, 164);
            this._btnFont.Name = "_btnFont";
            this._btnFont.Size = new System.Drawing.Size(75, 23);
            this._btnFont.TabIndex = 3;
            this._btnFont.Text = "Font...";
            this._btnFont.UseVisualStyleBackColor = true;
            this._btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // _btnCnacel
            // 
            this._btnCnacel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCnacel.Location = new System.Drawing.Point(445, 245);
            this._btnCnacel.Name = "_btnCnacel";
            this._btnCnacel.Size = new System.Drawing.Size(75, 23);
            this._btnCnacel.TabIndex = 7;
            this._btnCnacel.Text = "Cencel";
            this._btnCnacel.UseVisualStyleBackColor = true;
            this._btnCnacel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // _lblSymbol
            // 
            this._lblSymbol.AutoSize = true;
            this._lblSymbol.Location = new System.Drawing.Point(12, 201);
            this._lblSymbol.Name = "_lblSymbol";
            this._lblSymbol.Size = new System.Drawing.Size(44, 13);
            this._lblSymbol.TabIndex = 4;
            this._lblSymbol.Text = "Symbol:";
            // 
            // _lblSymbolFont
            // 
            this._lblSymbolFont.AutoSize = true;
            this._lblSymbolFont.Location = new System.Drawing.Point(12, 216);
            this._lblSymbolFont.Name = "_lblSymbolFont";
            this._lblSymbolFont.Size = new System.Drawing.Size(31, 13);
            this._lblSymbolFont.TabIndex = 5;
            this._lblSymbolFont.Text = "Font:";
            // 
            // _Label1
            // 
            this._Label1.AutoSize = true;
            this._Label1.Location = new System.Drawing.Point(197, 169);
            this._Label1.Name = "_Label1";
            this._Label1.Size = new System.Drawing.Size(119, 13);
            this._Label1.TabIndex = 1;
            this._Label1.Text = "Character Code Range:";
            // 
            // _cbxSymbolRange
            // 
            this._cbxSymbolRange.FormattingEnabled = true;
            this._cbxSymbolRange.Location = new System.Drawing.Point(322, 166);
            this._cbxSymbolRange.Name = "_cbxSymbolRange";
            this._cbxSymbolRange.Size = new System.Drawing.Size(106, 21);
            this._cbxSymbolRange.TabIndex = 2;
            this._cbxSymbolRange.SelectedValueChanged += new System.EventHandler(this.cbxSymbolRange_SelectedItemChanged);
            // 
            // InsertSymbolForm
            // 
            this.AcceptButton = this._btnInsert;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCnacel;
            this.ClientSize = new System.Drawing.Size(721, 278);
            this.Controls.Add(this._cbxSymbolRange);
            this.Controls.Add(this._Label1);
            this.Controls.Add(this._lblSymbolFont);
            this.Controls.Add(this._lblSymbol);
            this.Controls.Add(this._btnCnacel);
            this.Controls.Add(this._btnFont);
            this.Controls.Add(this._btnInsert);
            this.Controls.Add(this._dgvSymbols);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "InsertSymbolForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Insert Symbol";
            this.Load += new System.EventHandler(this.InsertSymbolForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._dgvSymbols)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private DataGridView _dgvSymbols;

        internal DataGridView dgvSymbols
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _dgvSymbols;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_dgvSymbols != null)
                {
                }

                _dgvSymbols = value;
                if (_dgvSymbols != null)
                {
                }
            }
        }

        private Button _btnInsert;

        internal Button btnInsert
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _btnInsert;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_btnInsert != null)
                {
                }

                _btnInsert = value;
                if (_btnInsert != null)
                {
                }
            }
        }

        private Button _btnFont;

        internal Button btnFont
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _btnFont;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_btnFont != null)
                {
                }

                _btnFont = value;
                if (_btnFont != null)
                {
                }
            }
        }

        private Button _btnCnacel;

        internal Button btnCnacel
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _btnCnacel;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_btnCnacel != null)
                {
                }

                _btnCnacel = value;
                if (_btnCnacel != null)
                {
                }
            }
        }

        private Label _lblSymbol;

        internal Label lblSymbol
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _lblSymbol;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_lblSymbol != null)
                {
                }

                _lblSymbol = value;
                if (_lblSymbol != null)
                {
                }
            }
        }

        private Label _lblSymbolFont;

        internal Label lblSymbolFont
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _lblSymbolFont;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_lblSymbolFont != null)
                {
                }

                _lblSymbolFont = value;
                if (_lblSymbolFont != null)
                {
                }
            }
        }

        private FontDialog _fntdlgInsert;

        internal FontDialog fntdlgInsert
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _fntdlgInsert;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_fntdlgInsert != null)
                {
                }

                _fntdlgInsert = value;
                if (_fntdlgInsert != null)
                {
                }
            }
        }

        private Label _Label1;

        internal Label Label1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Label1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Label1 != null)
                {
                }

                _Label1 = value;
                if (_Label1 != null)
                {
                }
            }
        }

        private ComboBox _cbxSymbolRange;

        internal ComboBox cbxSymbolRange
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cbxSymbolRange;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cbxSymbolRange != null)
                {
                }

                _cbxSymbolRange = value;
                if (_cbxSymbolRange != null)
                {
                }
            }
        }
    }

}