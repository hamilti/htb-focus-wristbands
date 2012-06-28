using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Forms2012
{
    public partial class Form3 : Form
    {
        Form1 mainForm;

        public Form3(Form1 mainForm)   
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            string strPrinter = mainForm.strSelectedPrinter;
            int i = 0;
            int sel = -1;
            foreach (String printer in PrinterSettings.InstalledPrinters)
            {   
                listBox1.Items.Add(printer.ToString());
                if (printer.ToString() == strPrinter)
                {
                    sel = i;
                }
                i += 1;
            }
            listBox1.SelectedIndex = sel;
            listBox1.Sorted = true;
            pbSave.Enabled = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = "Setup Printers";
        }

        private void pbSave_Click(object sender, EventArgs e)
        {
            string strSelected = listBox1.SelectedItem.ToString();
            mainForm.strSelectedPrinter = strSelected;
            Application.UserAppDataRegistry.SetValue("Printer", strSelected);
            mainForm.setupPrinter(strSelected);
            this.Close();
        }

        private void pbCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pbSave.Enabled = true;
        }


    }
}
