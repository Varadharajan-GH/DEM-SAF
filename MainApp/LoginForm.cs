﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainApp
{
    public partial class LoginForm : Form
    {
        private string UserNamefield;

        public LoginForm()
        {
            InitializeComponent();
        }

        public string UserName { get => UserNamefield; set => UserNamefield = value; }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            UserNamefield = TextUserName.Text;
            //this.Hide();
            //using(MainForm mainForm = new MainForm())
            //{
            //    mainForm.UserName = UserName;
            //    mainForm.Show();
            //}               
        }        
        

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
