using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Forms2012
{
    public partial class Splash1 : Form
    {
        public Splash1()
        {
            InitializeComponent();
           
        }

        private void Splash1_Load(object sender, EventArgs e)
        {

            //Get the working area of the screen and assign it to a rectangle object   
            Rectangle rect = Screen.PrimaryScreen.WorkingArea;
            //Divide the screen in half, and find the center of the form to center it
            this.Top = (rect.Height / 2) - (this.Height / 2);
            this.Left = (rect.Width / 2) - (this.Width / 2);

        }
    }
}
