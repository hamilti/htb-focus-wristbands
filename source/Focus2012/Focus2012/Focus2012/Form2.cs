using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;

namespace Forms2012
{
    public partial class Form2 : Form
    {
        Form1 mainForm;

        public Form2(Form1 mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            pbSave.Enabled = false;

            //if (Application.UserAppDataRegistry.GetValue(

            txtPassword.Text = mainForm.strPassword;
            txtDSN.Text = mainForm.strDSN;
            txtDatabase.Text = mainForm.strDatabase;
            txtUser.Text = mainForm.strUsername;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = "Setup Database Connection";

        }

        private void pbTest_Click(object sender, EventArgs e)
        {
            string strConnect = mainForm.genConnection(txtDSN.Text, txtDatabase.Text, txtUser.Text, txtPassword.Text);

            //connect the database
            OdbcConnection connection = new OdbcConnection(strConnect);
            try
            {
                connection.Open();
                MessageBox.Show("Success!");
                pbSave.Enabled = true;
            }
            catch (System.Exception  ex)
            {
                MessageBox.Show(ex.Message);
                pbSave.Enabled = false;
            }
        }

        private void pbCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbSave_Click(object sender, EventArgs e)
        {
            //save the new options
            mainForm.strPassword = txtPassword.Text;
            mainForm.strDSN = txtDSN.Text;
            mainForm.strDatabase = txtDatabase.Text;
            mainForm.strUsername = txtUser.Text;

            //persist to the registry
            Application.UserAppDataRegistry.SetValue("DSN", txtDSN.Text);
            Application.UserAppDataRegistry.SetValue("Password", txtPassword.Text);
            Application.UserAppDataRegistry.SetValue("Database", txtDatabase.Text);
            Application.UserAppDataRegistry.SetValue("UserID", txtUser.Text);

            //reset the main form
            mainForm.GetData();


            //close the dialog
            this.Close();
        }


    }
}
