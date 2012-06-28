using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Drawing.Printing;


namespace Forms2012
{   
    public partial class Form4 : Form
    {
        Form1 mainForm;
        public string str;
        public int pStatus;
        public string numBands;
        public string strColour;
        public string[] extendedPrinterStatus = { "", "Paused", "Online", "Idle", "Printing", "Warming Up", "Stopped Printing", "Offline", "Paused", "Error", "Busy", "Not Available", "Waiting", "Processing", "Initializing", "Power Save", "Pending Deletion", "I/O Active", "Manual Feed" };

        public Form4(Form1 mainForm, string numBands, string strColour)
        {
            this.mainForm = mainForm;
            this.numBands = numBands;
            this.strColour = strColour;
            InitializeComponent();
        }

        public string isPrinterReady(string strIn)
        {

            ManagementScope scope = new ManagementScope(@"\root\cimv2");
            scope.Connect();
            ObjectQuery query = new ObjectQuery("Select * from Win32_Printer WHERE Name = \'" + strIn + "\'");
            ManagementObjectSearcher objsearch = new ManagementObjectSearcher(scope, query);

            foreach(ManagementObject printer in objsearch.Get()) 
            {

                str = printer["ExtendedPrinterStatus"].ToString();
                this.Text = "Are you sure?";
                pStatus = Convert.ToInt32(str);
                str =  extendedPrinterStatus[pStatus];
            }

            return str;
        }

        private void Form4_Load(object sender, EventArgs e)
        {

            label1.Text = "You are about to print " + numBands + " wristbands.  Make sure that you have enough wristbands of this colour available:";

            switch (strColour)
            {
                case "Red":
                    pictureBox1.Image = (Image)Properties.Resources.red_band;
                    break;

                case "Green":
                    pictureBox1.Image = (Image)Properties.Resources.green_band;

                    break;

                case "Yellow":
                    pictureBox1.Image = (Image)Properties.Resources.yellow_band;
                    break;

                case "Blue":
                    pictureBox1.Image = (Image)Properties.Resources.blue_band;
                    break;

                case "Orange":
                    pictureBox1.Image = (Image)Properties.Resources.orange_band;
                    break;

                case "Grey":
                    pictureBox1.Image = (Image)Properties.Resources.grey_band;
                    break;

                case "Pink":
                    pictureBox1.Image = (Image)Properties.Resources.pink_band;
                    break;

                default:
                    //no image loaded
                    break;

            }

            string response = isPrinterReady(mainForm.strSelectedPrinter);
            if (response.Equals("Online"))
            {
                label2.Text = "The printer '" + mainForm.strSelectedPrinter + "' is ready to print.  Click the continue button when ready.";
                pbGo.Enabled = true;
            }
            else
            {
                label2.Text = "The printer '" + mainForm.strSelectedPrinter + "' is "+response+".  Check the cables, settings or load printing stock.";
                pbGo.Enabled = false;
             }

            pbCancel.DialogResult = DialogResult.Cancel;
            pbGo.DialogResult = DialogResult.OK;

            Cursor.Current = Cursors.Default;

        }

        private void pbCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbGo_Click(object sender, EventArgs e)
        {
            this.Close();
        }



    }
}
