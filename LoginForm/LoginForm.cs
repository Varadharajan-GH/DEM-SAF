﻿using MainApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginForm
{
    public partial class LoginForm : Form
    {
       

        public LoginForm()
        {
            InitializeComponent();
        }        

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.UserName = TextUserName.Text;
            mainForm.Show();
            this.Hide();           
        }
    }
}
