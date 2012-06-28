using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Data.Odbc;
using System.Data.SqlClient;

namespace Forms2012
{
    public partial class Form1 : Form
    {

        // Declare the PrintDocument objects.
        private PrintDocument printDocument1 = new PrintDocument();

        private BindingSource masterBindingSource = new BindingSource();
        private BindingSource detailsBindingSource = new BindingSource();
        private BindingSource resultsBindingSource = new BindingSource();
        private BindingSource masteroffsiteBindingSource = new BindingSource();
        private BindingSource detailoffsiteBindingSource = new BindingSource();

        private string strHeader;
        private string strName;
        private string strBookingRef;
        private string strChurch;
        private string strYears;
        private string strPhone;
        private string strStreamID;
        private string strDays;

        //printing vars
        private Font fontSubText = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
        private Font fontHeader = new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Point);
        private Font fontName = new Font("Arial", 21, FontStyle.Bold, GraphicsUnit.Point);
        private Font fontNameSmall = new Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Point);
        private Font fontCode = new Font("Arial", 22, FontStyle.Bold, GraphicsUnit.Point);
        private SolidBrush brush = new SolidBrush(Color.Black);
        private StringFormat normalFormat =  new StringFormat();
        private StringFormat verticalFormat = new StringFormat();

        private DateTime currentDate = DateTime.Now.Date;

        public string strDSN;
        public string strUsername;
        public string strPassword;
        public string strDatabase;
        public string strSelectedPrinter;
        public string strRows;
        public string strColour;
        public string strConnect;
        public Int32 nPrintRows;

        public Splash1 splash = new Splash1();

        public Form1()
        {
                
            splash.Show();
            splash.Update();

            InitializeComponent();
            printDocument1.PrintPage += new PrintPageEventHandler(PrintPageMethod);

            PrintController pController = new StandardPrintController();
            printDocument1.PrintController = pController;

            this.Load += new System.EventHandler(Form1_Load);
            this.Text = "HTB Focus Wristband Printer [2012]";
            strHeader = "FOCUS 2012";
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            getSettings();
            setupPrinter(strSelectedPrinter);
            label1.Text = "To Print:";
            
            nPrintRows = 100;

            //run the update scripts
            runScripts(); 
            
            //bind the datagrids
            if (strDSN != null)
            {
                // Bind the DataGridView controls to the BindingSource
                // components and load the data from the database.
                masterDataGridView.DataSource = masterBindingSource;
                detailsDataGridView.DataSource = detailsBindingSource;

                try
                {
                    GetData();

                    // Resize the DataGridView columns to fit the newly loaded data.
                    masterDataGridView.AutoResizeColumns();
                    detailsDataGridView.AutoResizeColumns();

                    //setup the look and feel
                    masterDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
                    masterDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    detailsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                    //hide the colour column
                    masterDataGridView.Columns["streamID"].Visible = false;
                    detailsDataGridView.Columns["band_streamID"].Visible = false;
                    masterDataGridView.Columns["StreamDesc"].HeaderText = "Stream Name";
                    masterDataGridView.Columns["StreamColour"].HeaderText = "Wristband Colour";

                    // Configure the details DataGridView so that its columns automatically
                    // adjust their widths when the data changes.
                    masterDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    detailsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                    //set the colour flags
                    setupColours(masterDataGridView.Rows[0].Cells[2].Value.ToString());
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                //setup the images

                //Set the print formats
                normalFormat.Alignment = StringAlignment.Near;
                normalFormat.LineAlignment = StringAlignment.Near;
                verticalFormat.Alignment = StringAlignment.Near;
                verticalFormat.LineAlignment = StringAlignment.Near;
                verticalFormat.FormatFlags = StringFormatFlags.DirectionVertical;

                //get the search forms data
                DataSet ds = GetBindData("select * from htb_focus.focus_band_streams order by streamID");
                if (ds != null)
                {
                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        cmbStream.Items.Add(dataRow["StreamDesc"].ToString());
                    }

                    ds = GetBindData("select * from htb_focus.focus_status order by statusID");

                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        cmbStatus.Items.Add(dataRow["statusDesc"].ToString());
                    }
                }
            }
            System.Threading.Thread.Sleep(2000);
            splash.Hide();
        }

        private void runScripts()
        {
            if (strDSN != null)
            {
                strConnect = genConnection(strDSN, strDatabase, strUsername, strPassword);
                try
                {
                    //setup the update connection
                    OdbcConnection connUpdate = new OdbcConnection(strConnect);
                    OdbcCommand cmd = new OdbcCommand();
                    cmd.CommandType = CommandType.Text;

                     //update the database
                     //default = set the band ID to be the stream ID
                     cmd.CommandText = "update focus_guests" +
                                        " set band_streamID = streamID " +
                                        " where band_streamID is null ";
                     cmd.Connection = connUpdate;
                     connUpdate.Open();
                     cmd.ExecuteNonQuery();
                     connUpdate.Close();
                 
                     //off site single days
                     cmd.CommandText = "update focus_guests set band_streamID = 13 " +
                            " where guestID in (SELECT bg.guestID FROM focus_booking_groups bg " +
                            " inner join focus_bookings b on (bg.bookingID = b.bookingID) " +
                            " inner join focus_status st on (b.statusID = st.statusID) " +
                            " left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID) " +
                            " where ba.accommodationID in (13,14,15,16,16,18))  ";
                     cmd.Connection = connUpdate;
                     connUpdate.Open();
                     cmd.ExecuteNonQuery();
                     connUpdate.Close();

                     //offsite full week pass
                     cmd.CommandText = "update focus_guests set band_streamID = 14 " +
                            " where guestID in (SELECT bg.guestID FROM focus_booking_groups bg " +
                            " inner join focus_bookings b on (bg.bookingID = b.bookingID) " +
                            " inner join focus_status st on (b.statusID = st.statusID) " +
                            " left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID) " +
                            " where ba.accommodationID in (19)) ";
                     cmd.Connection = connUpdate;
                     connUpdate.Open();
                     cmd.ExecuteNonQuery();
                     connUpdate.Close();

                     //staff
                     cmd.CommandText = "update focus_guests upd set band_streamID = ( " +
                                        " SELECT CASE min(c.couponID) " +
                                        "           WHEN 1 THEN 12 " +
                                        "           WHEN 2 THEN 12 " +
                                        "           ELSE 11 " +
                                        "        END AS STAFF " +
                                        " FROM focus_guest_coupons gc " +
                                        "      inner join focus_coupons c on (gc.couponID = c.couponID) " +
                                        "      where gc.guestID = upd.guestID) " +
                                        " where band_streamID = 11 ";
                     cmd.Connection = connUpdate;
                     connUpdate.Open();
                     cmd.ExecuteNonQuery();
                     connUpdate.Close();
                     
                     //reset child streams that are offsite - use the correct streamID
                     cmd.CommandText = "update focus_guests set band_streamID = streamID " +
                                        " where band_streamID in (13,14) " +
                                        " and age <= 17 ";
                     cmd.Connection = connUpdate;
                     connUpdate.Open();
                     cmd.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void GetData()
        {

            if (strDSN != null)
            {
                strConnect = genConnection(strDSN, strDatabase, strUsername, strPassword);
                //connect the database
                try
                {
                    //connect the database
                    OdbcConnection connection = new OdbcConnection(strConnect);

                    // Create a DataSet.
                    DataSet data = new DataSet();
                    data.Locale = System.Globalization.CultureInfo.InvariantCulture;

                    // Add data from the Streams table to the DataSet.
                    OdbcDataAdapter masterDataAdapter = new
                        OdbcDataAdapter("SELECT * FROM htb_focus.focus_band_streams order by streamID ", connection);
                    masterDataAdapter.Fill(data, "Streams");

                    // Add data from the Guests table to the DataSet.
                    OdbcDataAdapter detailsDataAdapter = new
                        OdbcDataAdapter("SELECT * FROM htb_focus.view_wristbands where band_print_date = 0 and statusID not in (0, 4, 7, 8, 11, 6) ", connection);
                    detailsDataAdapter.Fill(data, "Guests");

                    // Establish a relationship between the two tables.
                    DataRelation relation = new DataRelation("StreamGuests",
                        data.Tables["Streams"].Columns["streamID"],
                        data.Tables["Guests"].Columns["band_streamID"]);
                    data.Relations.Add(relation);

                    // Bind the master data connector to the Customers table.
                    masterBindingSource.DataSource = data;
                    masterBindingSource.DataMember = "Streams";

                    // Bind the details data connector to the master data connector,
                    // using the DataRelation name to filter the information in the 
                    // details table based on the current row in the master table. 
                    detailsBindingSource.DataSource = masterBindingSource;
                    detailsBindingSource.DataMember = "StreamGuests";

                    lblDatabase.Text = strDSN;
                    setupColours(masterDataGridView.Rows[0].Cells[2].Value.ToString());
                }
                catch (System.Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                lblDatabase.Text = "No database!";
                pbPrint.Enabled = false;
            }

        }
 
        public void GetSearchData(string strSQLIn)
        {

            if (strDSN != null)
            {
                strConnect = genConnection(strDSN, strDatabase, strUsername, strPassword);
                //connect the database
                try
                {
                    //connect the database
                    OdbcConnection connection = new OdbcConnection(strConnect);

                    // Create a DataSet.
                    DataSet resdata = new DataSet();
                    resdata.Locale = System.Globalization.CultureInfo.InvariantCulture;

                    // Add data from the Guests table to the DataSet.
                    OdbcDataAdapter DataAdapter = new OdbcDataAdapter(strSQLIn, connection);
                    DataAdapter.Fill(resdata, "Results");

                    // Bind the master data connector to the Customers table.
                    resultsBindingSource.DataSource = resdata;
                    resultsBindingSource.DataMember = "Results";

                    if (strDSN == null)
                    {
                        lblDatabase.Text = "No database!";
                        pbPrint.Enabled = false;
                    }
                    else
                    {
                        lblDatabase.Text = strDSN;
                    }

                }
                catch (System.Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public DataSet GetBindData(string strSQLIn)
        {
            if (strDSN != null)
            {
                strConnect = genConnection(strDSN, strDatabase, strUsername, strPassword);
                //connect the database

                OdbcConnection connection = new OdbcConnection(strConnect);

                // Create a DataSet.
                DataSet dataset = new DataSet();

                // Add data from the Guests table to the DataSet.
                OdbcDataAdapter resultsDataAdapter = new OdbcDataAdapter(strSQLIn, connection);
                resultsDataAdapter.Fill(dataset);

                return dataset;
            }
            else
            {
                return null;
            }

        }

        private void getSettings()
        {
            if (Application.UserAppDataRegistry.GetValue("DSN") != null)
            {
                strDSN = Application.UserAppDataRegistry.GetValue("DSN").ToString();
            }
            if (Application.UserAppDataRegistry.GetValue("Password") != null)
            {
                strPassword = Application.UserAppDataRegistry.GetValue("Password").ToString();
            }
            if (Application.UserAppDataRegistry.GetValue("UserID") != null)
            {
                strUsername = Application.UserAppDataRegistry.GetValue("UserID").ToString();
            }
            if (Application.UserAppDataRegistry.GetValue("Database") != null)
            {
                strDatabase = Application.UserAppDataRegistry.GetValue("Database").ToString();
            }
            if (Application.UserAppDataRegistry.GetValue("Printer") != null)
            {
                strSelectedPrinter = Application.UserAppDataRegistry.GetValue("Printer").ToString();
            }

        }

        public string genConnection(string strDSN, string strDB, string strID, string strPass) 
        {
            string strConn = "";
            strConn += "DSN=" + strDSN + ";";
            strConn += "UID=" + strID + ";";
            strConn += "PWD=" + strPass + ";";
            strConn += "DATABASE=" + strDB;
            return strConn;
        }

        private void PrintPageMethod(object sender, PrintPageEventArgs ppev)
        {
            //page rendering

            Graphics g = ppev.Graphics;

            //check the offsite flag - override the child details with the ticket type in this case
            switch (strStreamID) {

                case "13":
                    //Print the days for offsite only
                    g.DrawString(strDays, fontSubText, brush, 104, 80, normalFormat);
                    break;

                case "14":
                    //Print the days for offsite only
                    g.DrawString(strDays, fontSubText, brush, 104, 80, normalFormat);
                    //add star
                    g.DrawImage(Properties.Resources.star, 370, 30);
                    break;

                default:
                    //Print the age
                    g.DrawString(strYears, fontSubText, brush, 104, 80, normalFormat);
                    //print the phone number
                    g.DrawString(strPhone, fontSubText, brush, 270, 80, normalFormat);
                    break;
            }

            //Print Awakening Logo
            g.DrawString(strHeader, fontHeader, brush, 104, 0, normalFormat);

            // Draw the Name String.  Check the length - longer names need a smaller font
            if (strName.Length > 22)
            {
                g.DrawString(strName, fontNameSmall, brush, 104, 23, normalFormat);
            }
            else
            {
                g.DrawString(strName, fontName, brush, 102, 23, normalFormat);
            }

            //Print Church Text
            g.DrawString(strChurch, fontSubText, brush, 104, 58, normalFormat);

            //Print Booking Ref ID
            g.DrawString(strBookingRef, fontCode, brush, 340, -5, normalFormat);
        
        }

        public void setupPrinter(string strPrn)
        {
            //set the printer to use
            printDocument1.PrinterSettings.PrinterName = strPrn;
            //set Landscape
            printDocument1.DefaultPageSettings.Landscape = true;
            if (strPrn == null)
            {
                lblPrinter.Text = "No printer!";
                pbPrint.Enabled = false;
            }
            else
            {
                lblPrinter.Text = strPrn;
                pbPrint.Enabled = true;
            }
            if (lblMessage.Text=="0")
            {
                pbPrint.Enabled = false;
            }
        }

        private void pbPrint_Click_1(object sender, EventArgs e)
        {
            //set the cursor
            Cursor.Current = Cursors.WaitCursor;

            goPrint(detailsDataGridView, false);
            GetData();
        }

        private void goPrint(DataGridView dgvrIn, bool bSelectedOnly)
        {
            Int32 nRows = Convert.ToInt32(strRows);
            if (nRows > nPrintRows)
            {
                strRows = nPrintRows.ToString() ;
                nRows = nPrintRows;
            }

            Form4 printDialog = new Form4(this, strRows, strColour);

            if (printDialog.ShowDialog(this) == DialogResult.OK)
            {
                setupPrinter(strSelectedPrinter);

                //setup the update connection
                OdbcConnection connUpdate = new OdbcConnection(strConnect);
                OdbcCommand cmd = new OdbcCommand();
                cmd.CommandType = CommandType.Text;

                string strCheck = "";
                Int32 nCount = 0;

                if (bSelectedOnly)
                {
                    foreach (DataGridViewRow dgRow in dgvrIn.SelectedRows)
                    {
                        strName = "";
                        strBookingRef = "";
                        strChurch = "";
                        strYears = "";
                        strPhone = "";

                        //Print Name
                        if (dgRow.Cells["firstName"].Value != null)
                        {
                            strName = dgRow.Cells["firstName"].Value.ToString() + " " + dgRow.Cells["lastName"].Value.ToString();
                        }
                        //Booking ID
                        if (dgRow.Cells["bookingID"].Value != null)
                        {
                            strBookingRef = dgRow.Cells["bookingID"].Value.ToString();
                        }
                        //Church Name
                        if (dgRow.Cells["churchDesc"].Value != null)
                        {
                            strChurch = dgRow.Cells["churchDesc"].Value.ToString();
                        }

                        //Days
                        if (dgRow.Cells["Days"].Value != null)
                        {
                            strDays = dgRow.Cells["Days"].Value.ToString();
                        }

                        //StreamID
                        if (dgRow.Cells["band_streamID"].Value != null)
                        {
                            strStreamID = dgRow.Cells["band_streamID"].Value.ToString();
                        }

                        //Age Years
                        if (Convert.ToInt32(dgRow.Cells["band_streamID"].Value) <= 7 & dgRow.Cells["Age"].Value != null)
                        {
                            //this is a child under 10 years of age. Calc the age in years
                            strYears = "Age: " + dgRow.Cells["Age"].Value.ToString() + " years";
                            if (dgRow.Cells["family_phone"].Value != null)
                            {
                                strPhone = dgRow.Cells["family_phone"].Value.ToString();
                            }
                            else
                            {
                                if (dgRow.Cells["booking_phone"].Value != null)
                                {
                                    strPhone = dgRow.Cells["booking_phone"].Value.ToString();
                                }
                            }
                        }
                        else
                        {
                            strYears = "";
                        }


                        if (strCheck != strName)
                        {
                            printDocument1.Print();

                            //update the database
                            //cmd.CommandText = "update htb_focus.focus_guests set band_print_date = unix_timestamp() where guestID = " + dgRow.Cells["guestID"].Value;
                            cmd.CommandText = "insert into htb_bands.band_prints (guestID, band_print_date) values (" + dgRow.Cells["guestID"].Value + ", unix_timestamp()) on duplicate key update band_print_date = unix_timestamp() ";

                            cmd.Connection = connUpdate;
                            connUpdate.Open();
                            cmd.ExecuteNonQuery();
                            connUpdate.Close();
                        }
                        strCheck = strName;
                        nCount += 1;
                        if (nCount >= nRows)
                        {
                            break;
                        }

                    }

                }
                else
                {
                    nCount = 0;

                    foreach (DataGridViewRow dgRow in dgvrIn.Rows)
                    {
                        strName = "";
                        strBookingRef = "";
                        strChurch = "";
                        strYears = "";
                        strPhone = "";

                        //Print Name
                        if (dgRow.Cells["firstName"].Value != null)
                        {
                            strName = dgRow.Cells["firstName"].Value.ToString() + " " + dgRow.Cells["lastName"].Value.ToString();
                        }
                        //Booking ID
                        if (dgRow.Cells["bookingID"].Value != null)
                        {
                            strBookingRef = dgRow.Cells["bookingID"].Value.ToString();
                        }
                        //Church Name
                        if (dgRow.Cells["churchDesc"].Value != null)
                        {
                            strChurch = dgRow.Cells["churchDesc"].Value.ToString();
                        }

                        //Days
                        if (dgRow.Cells["Days"].Value != null)
                        {
                            strDays = dgRow.Cells["Days"].Value.ToString();
                        }

                        //StreamID
                        if (dgRow.Cells["band_streamID"].Value != null)
                        {
                            strStreamID = dgRow.Cells["band_streamID"].Value.ToString();
                        }

                        //Age Years
                        if (Convert.ToInt32(dgRow.Cells["band_streamID"].Value) <= 7 & dgRow.Cells["Age"].Value != null)
                        {
                            //this is a child under 10 years of age. Calc the age in years
                            //diff = currentDate - Convert.ToDateTime(dgRow.Cells["dateOfBirth"].Value.ToString());
                            //span = Math.Round(Convert.ToDecimal(diff.Days) / 365, 1);
                            strYears = "Age: " + dgRow.Cells["Age"].Value.ToString() + " years";
                            if (dgRow.Cells["family_phone"].Value != null)
                            {
                                strPhone = dgRow.Cells["family_phone"].Value.ToString();
                            }
                            else
                            {
                                if (dgRow.Cells["booking_phone"].Value != null)
                                {
                                    strPhone = dgRow.Cells["booking_phone"].Value.ToString();
                                }
                            }
                        }
                        else
                        {
                            strYears = "";
                        }

                        if (strCheck != strName)
                        {
                            printDocument1.Print();

                            //update the database
                            //cmd.CommandText = "update htb_focus.focus_guests set band_print_date = unix_timestamp() where guestID = " + dgRow.Cells["guestID"].Value;
                            cmd.CommandText = "insert into htb_bands.band_prints (guestID, band_print_date) values (" + dgRow.Cells["guestID"].Value + ", unix_timestamp()) on duplicate key update band_print_date = unix_timestamp() ";

                            cmd.Connection = connUpdate;
                            connUpdate.Open();
                            cmd.ExecuteNonQuery();
                            connUpdate.Close();
                        }
                        strCheck = strName;
                        nCount += 1;
                        if (nCount >= nRows)
                        {
                            break;
                        }
                    } 
                }

                MessageBox.Show("Printing Completed Successfully");
            }
            else
            {
                MessageBox.Show("Print Cancelled");
            }
            printDialog.Dispose();

        }

        private void setupColours(string sIn)
        {
            lblColour.Text = sIn;
            strColour = sIn;

            switch (sIn)
            {
                case "Red":
                    picWristband.Image = (Image)Properties.Resources.red_band;
                    break;

                case "Green":
                    picWristband.Image = (Image)Properties.Resources.green_band;
                    break;

                case "Yellow":
                    picWristband.Image = (Image)Properties.Resources.yellow_band;
                    break;

                case "Blue":
                    picWristband.Image = (Image)Properties.Resources.blue_band;
                    break;

                case "Orange":
                    picWristband.Image = (Image)Properties.Resources.orange_band;
                    break;

                case "Grey":
                    picWristband.Image = (Image)Properties.Resources.grey_band;
                    break;

                case "Pink":
                    picWristband.Image = (Image)Properties.Resources.pink_band;
                    break;

                default:
                    //no image loaded
                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Save the setting for the form that may have changed
            


        }

        private void databaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 subForm = new Form2(this);
            subForm.ShowDialog();

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 subForm = new AboutBox1();
            subForm.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void printerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 subform = new Form3(this);
            subform.ShowDialog();
        }

        private void focusWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.htb.org.uk/focus");
        }

        private void detailsDataGridView_DataBindingComplete_1(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            lblMessage.Text = detailsDataGridView.RowCount.ToString();
            strRows = lblMessage.Text;
        }

        private void masterDataGridView_CellMouseUp_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            setupColours(masterDataGridView.SelectedRows[0].Cells[2].Value.ToString());
        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbSearch_Click(object sender, EventArgs e)
        {
            //check the fields and build the search string
            string sSql = "select streamDesc, statusDesc, title,firstName,lastName,streamColour,offsite,guestID,bookingID,gender,dateOfBirth,phone,email,churchID,age,firstFocus,schoolYear,band_streamID,band_print_date,churchDesc,days,family_phone,booking_phone";
            sSql += "  FROM htb_focus.view_wristbands  where ";
                        
            string sAnd = "";

            if (dfsBookingID.Text!="")
            {
                sSql += " bookingID in ( " + dfsBookingID.Text + ")";
                sAnd = " AND ";
            }

            if (dfsName.Text != "")
            {
                sSql += sAnd + " concat(firstname, ' ', lastname) like '%" + dfsName.Text + "%'";
                sAnd = " AND ";
            }

            if (cmbStatus.SelectedIndex >= 0)
            {
                sSql += sAnd + " statusDesc = '" + cmbStatus.SelectedItem.ToString() + "'";
                sAnd = " AND ";
            }

            if (cmbStream.SelectedIndex >= 0)
            {
                sSql += sAnd + " band_streamID = " + cmbStream.SelectedIndex;
                sAnd = " AND ";
            }

            tblResults.DataSource = resultsBindingSource;
            GetSearchData(sSql);

            pbPrintAll.Enabled = true;
            pbPrintSelected.Enabled = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dfsBookingID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                pbSearch_Click(sender, e);
            }
        }

        private void dfsName_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                pbSearch_Click(sender, e);
            }
        }

        private void pbClear_Click(object sender, EventArgs e)
        {
            dfsBookingID.Text = "";
            dfsName.Text = "";
            cmbStream.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
            if (tblResults.DataSource != null)
            {
                tblResults.DataSource = null;
            }
            pbSearch.Enabled = false;
        }

        private void pbPrintAll_Click(object sender, EventArgs e)
        {
            //fetch the rows/colours
            int nRows = 0;
            string strTest = "";
            foreach (DataGridViewRow dgRow in tblResults.Rows)
            {
                if (nRows > 0)
                {
                    if (strTest != dgRow.Cells["streamColour"].Value.ToString())
                    {
                        MessageBox.Show("Cannot print - more than one stream/wristband colour is selected", "Error");
                        return;
                    }
                }
                strTest = dgRow.Cells["streamColour"].Value.ToString();
                nRows += 1;
            }

            //set the cursor
            Cursor.Current = Cursors.WaitCursor;
            strRows = nRows.ToString();
            strColour = strTest;
            goPrint(tblResults, false);
        }

        private void pbPrintSelected_Click(object sender, EventArgs e)
        {
            
            //fetch the rows/colours
            int nRows = 0;
            string strTest = "";
            foreach (DataGridViewRow dgRow in tblResults.SelectedRows)
            {  
                if (nRows > 0)
                {
                    if (strTest != dgRow.Cells["streamColour"].Value.ToString())
                    {
                        MessageBox.Show("Cannot print - more than one stream/wristband colour is selected", "Error");
                        return;
                    }
                }
                strTest = dgRow.Cells["streamColour"].Value.ToString();
                nRows += 1;
            }
            
            //set the cursor
            Cursor.Current = Cursors.WaitCursor;
            strRows = nRows.ToString();
            strColour = strTest;
            goPrint(tblResults, true);
        }

        private void dfsBookingID_TextChanged(object sender, EventArgs e)
        {
            pbClear.Enabled = true;
            pbSearch.Enabled = true;
        }

        private void dfsName_TextChanged(object sender, EventArgs e)
        {
            pbClear.Enabled = true;
            pbSearch.Enabled = true;
        }

        private void tblResults_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridViewCellStyle red = tblResults.DefaultCellStyle.Clone();
            red.ForeColor=Color.Red;
 
            foreach (DataGridViewRow r in tblResults.Rows)
            {
                if (r.Cells["band_print_date"].Value.ToString()!="0")
                {
                    r.DefaultCellStyle = red;
                }
            }
            Cursor.Current = Cursors.Default;

            if (tblResults.Rows.Count > 0)
            {
                lblSearchRows.Text = "Rows: " + tblResults.Rows.Count.ToString();
            }
            
        }

        private void cmbStream_SelectedIndexChanged(object sender, EventArgs e)
        {
            pbClear.Enabled = true;
            pbSearch.Enabled = true;
        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            pbClear.Enabled = true;
            pbSearch.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbPrintDirect_Click(object sender, EventArgs e)
        {
            strName = "";
            strBookingRef = "";
            strChurch = "";
            strYears = "";
            strPhone = "";
            strStreamID = "";
            strDays = "";

            //check the required fields
            if (dfsDirectName.Text == "")
            {
                dfsDirectName.BackColor = Color.PeachPuff;
                MessageBox.Show("Name is required", "Error");
                return;
            }

            if (dfsDirectChurchName.Text == "")
            {
                dfsDirectChurchName.BackColor = Color.PeachPuff;
                MessageBox.Show("Church name is required", "Error");
                return;
            }

            if (cbDirectOffsite.Checked&&dfsDirectDayInfo.Text=="")
            {
                MessageBox.Show("Offsite pass details missing", "Error");
                return;
            }

            if (cbDirectChild.Checked)
            {
                if (dfsDirectAge.Text==""|dfsDirectParentPhone.Text=="")
                {
                    MessageBox.Show("Child Age or parent phone info missing", "Error");
                    return;
                }
            }

            strName = dfsDirectName.Text;
            strChurch = dfsDirectChurchName.Text;
            strBookingRef = dfsDirectBookingRef.Text;

            if (cbDirectOffsite.Checked) {
                if (cbDirectFullWeek.Checked)
                {
                   strStreamID = "14";
                } else {
                   strStreamID = "13";
                }
                strDays = dfsDirectDayInfo.Text;
            }

            if (cbDirectChild.Checked)
            {
                strYears = dfsDirectAge.Text + " years";
                strPhone = dfsDirectParentPhone.Text;
            }

            printDocument1.Print();

        }

        private void pbCloseDirect_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbDirectChild_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDirectChild.Checked)
            {
                //dfsDirectAge.Enabled = true;
                //dfsDirectParentPhone.Enabled = true;
            }
            else
            {
                //dfsDirectAge.Enabled = false;
                //dfsDirectParentPhone.Enabled = false;
                dfsDirectAge.Text = "";
                dfsDirectParentPhone.Text = "";
            }
        }

        private void cbDirectOffsite_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (cbDirectOffsite.Checked)
            {
                cbDirectFullWeek.Enabled = true;
                dfsDirectDayInfo.Enabled = true;
                gbChild.Enabled = false;
            }
            else
            {
                cbDirectFullWeek.Enabled = false;
                dfsDirectDayInfo.Enabled = false;
                gbChild.Enabled = true;
            }
             */
        }

        private void cbDirectFullWeek_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDirectFullWeek.Checked){
                dfsDirectDayInfo.Text = "FULL WEEK PASS";
            } else {
                dfsDirectDayInfo.Text = "";
            }
        }

        private void dfsDirectName_TextChanged(object sender, EventArgs e)
        {
            dfsDirectName.BackColor = Color.White;
        }

        private void dfsDirectChurchName_TextChanged(object sender, EventArgs e)
        {
            dfsDirectChurchName.BackColor = Color.White;
        }

        private void pbResetDirectForm_Click(object sender, EventArgs e)
        {
            dfsDirectAge.Text = "";
            dfsDirectBookingRef.Text = "";
            dfsDirectChurchName.Text = "";
            dfsDirectDayInfo.Text = "";
            dfsDirectName.Text = "";
            dfsDirectParentPhone.Text = "";

            cbDirectChild.Checked = false;
            cbDirectFullWeek.Checked = false;
            cbDirectOffsite.Checked = false;

        }


    }
}
