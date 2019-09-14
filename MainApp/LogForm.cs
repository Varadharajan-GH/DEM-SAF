using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainApp
{
    public partial class LogForm : Form
    {
        private readonly Paths paths = new Paths();
        public LogForm()
        {
            InitializeComponent();
        }

        public void SetLog(string log)
        {
            txtLog.Text = log;
        }

        private void BtnSaveLog_Click(object sender, EventArgs e)
        {
            using (MainForm mainForm = new MainForm())
            {
                mainForm.AddLog("Saving log");
                File.WriteAllText(paths.Folders.Log_Dir,txtLog.Text);
            }          
        }
    }
}
