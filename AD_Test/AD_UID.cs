using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Security.Principal;

namespace nm_AD_UID
{

    /// <summary>
    /// Form class
    /// </summary>
    public partial class frmAD_UID : Form
    {
        /// <summary>
        /// List that holds AD objects (AD_Object)
        /// </summary>
        private List<AD_Object> UsersUIDs;

        /// <summary>
        /// Number of pages (also called max number of pages ) for AD objects with UID
        /// </summary>
        private int intPagesUIDs;

        /// <summary>
        /// Number of pages (also called max number of pages ) for AD objects without UID
        /// </summary>
        private int intPagesNoUIDs;

        /// <summary>
        /// Number of pages shown on one pager's page
        /// </summary>
        private int intPageSize;

        /// <summary>
        /// Current page viewed on the NoUID tab
        /// </summary>
        private int intCurrentPageNoUIDs;

        /// <summary>
        /// Current page viewed on the UID tab
        /// </summary>
        private int intCurrentPageUIDs;

        /// <summary>
        /// Initialisation method
        /// </summary>
        public frmAD_UID()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Innitial form load event
        /// </summary>
        /// <param name="sender">Form object</param>
        /// <param name="e">Event arguments</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //UsersUIDs = GetTestData();
            UsersUIDs = GetUsersUIDs();
            intPageSize = 50;
            intCurrentPageNoUIDs = 0;
            intCurrentPageUIDs = 0;

            intPagesNoUIDs = (int)(UsersUIDs.Where(a => String.IsNullOrEmpty(a.UID)).Count() / intPageSize);
            if (intPagesNoUIDs < (UsersUIDs.Where(a => String.IsNullOrEmpty(a.UID)).Count() / intPageSize)) { intPagesNoUIDs++; }

            intPagesUIDs = (int)(UsersUIDs.Where(a => !String.IsNullOrEmpty(a.UID)).Count() / intPageSize);
            if (intPagesUIDs < (UsersUIDs.Where(a => !String.IsNullOrEmpty(a.UID)).Count() / intPageSize)) { intPagesUIDs++; }

            PopulateNoUIDs();
            PopulateUIDs();
        }

        /// <summary>
        /// Method that populates list of AD objects of the objects that do not have UIDs
        /// </summary>
        private void PopulateNoUIDs()
        {
            pnlDataNoUI.Controls.Clear();
            pnlDataNoUI.Controls.Add(PopulateTable(UsersUIDs.Where(a => !a.Locked).ToList(), false));
        }

        /// <summary>
        /// Method that populates list of AD objects of the objects that have UIDs
        /// </summary>
        private void PopulateUIDs()
        {
            pnlDataUI.Controls.Clear();
            pnlDataUI.Controls.Add(PopulateTable(UsersUIDs.Where(a => a.Locked).ToList(), true));
        }

        /// <summary>
        /// Event handler that commences writting to the AD
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">Event arguments</param>
        private void SaveToAD(object sender, EventArgs e)
        {
            SetUserIDs(UsersUIDs);
        }

        /// <summary>
        /// Event handler for click event on check boxes associated to the AD object on the interface
        /// </summary>
        /// <param name="sender">CheckBox that is clicked</param>
        /// <param name="e">Event arguments</param>
        private void ChBoxClicked(object sender, EventArgs e)
        {
            CheckBox ch = (CheckBox)sender; String tag;
            if(ch.Tag!=null && !String.IsNullOrEmpty(ch.Tag.ToString()))
            {
                tag = ch.Tag.ToString();
                switch (tag)
                {
                    case "SelectAll":
                        SelectAll(ch.Checked); break;
                    default:
                        SelectIndividual(ch.Checked, ch.Tag.ToString()); 
                        break;
                }
            }
        }

        /// <summary>
        /// Select or deselect all AD objects
        /// </summary>
        /// <param name="sel">Boolean, select or deselect</param>
        private void SelectAll(Boolean sel)
        {
            var tbl = pnlDataNoUI.Controls.Find("tblNoUID", false);
            if (tbl != null && tbl.Length>0)
            {
               //select all in the array
               UsersUIDs.ForEach(a => { if (!a.Locked) {a.Selected = sel; if (!sel) { a.UID = ""; }; if (sel && String.IsNullOrEmpty(a.UID)) { a.UID = GetNextUID(); };  } });
               //select visible
                foreach (Control c in tbl[0].Controls)
                {
                    if (c is CheckBox chBox)
                    {
                        chBox.Checked = sel;
                        SelectIndividual(sel, chBox.Tag.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Select or deselect AD object in the list
        /// </summary>
        /// <param name="sel">Boolean, slect or deselect</param>
        /// <param name="Tag">Unique samAccountName, stored as Tag</param>
        private void SelectIndividual(Boolean sel, String Tag)
        {
            var tbl = pnlDataNoUI.Controls.Find("tblNoUID", false);
            AD_Object AD = UsersUIDs.Where(a => a.samAccountName == Tag).FirstOrDefault();
            if (AD !=default && tbl != null && tbl.Length > 0)
            {
                AD.UID = !sel ? "" : String.IsNullOrEmpty(AD.UID)? GetNextUID(): AD.UID; AD.Selected = sel;
                foreach (Control c in tbl[0].Controls)
                {
                    if(c is CheckBox chkBox && chkBox.Tag.ToString() == "SelectAll")
                    {
                        if (!sel)
                        {
                            chkBox.CheckedChanged -= ChBoxClicked; chkBox.Checked = false; chkBox.CheckedChanged += ChBoxClicked;
                        }
                        else 
                        { 
                            if(UsersUIDs.Count(a=>a.Locked) + UsersUIDs.Count(a=>!a.Locked && !String.IsNullOrEmpty(a.UID))== UsersUIDs.Count())
                            {
                                chkBox.CheckedChanged -= ChBoxClicked; chkBox.Checked = true; chkBox.CheckedChanged += ChBoxClicked;
                            }
                        }; 
                    };
                    if (c is TextBox txtBox && txtBox.Tag.ToString() == Tag)
                    {
                        txtBox.Text = AD.UID;
                    }
                }
            }
        }
        
        /// <summary>
        /// This function populates tables that are shown on the tabs
        /// </summary>
        /// <param name="UsersAndUIDs">List of AD_Objects to make table from</param>
        /// <param name="en">True for AD objects that have UIDs and false otherwise</param>
        /// <returns>TableLayoutPanel - table that is used for arranging the lsit of AD objects</returns>
        private TableLayoutPanel PopulateTable(List<AD_Object> UsersAndUIDs, Boolean en)
        {
            TableLayoutPanel tbl = new TableLayoutPanel(); int h = 20; int increment = 20; int rowNo=1;
            if (!en)
            {
                AddLabelToTable(tbl, "Select all", "Select all", h, 120, 100, true, 2, rowNo);
                CheckBox chkSelAll = new CheckBox(); chkSelAll.Tag = "SelectAll"; chkSelAll.Top = h; chkSelAll.Left = 240; chkSelAll.Enabled = false;
                tbl.Controls.Add(chkSelAll); tbl.SetColumn(chkSelAll, 3); tbl.SetRow(chkSelAll, rowNo);
                chkSelAll.CheckedChanged += ChBoxClicked; chkSelAll.Name = "chkSelAllNoUID"; 
                int intTmpChecked = UsersAndUIDs.Count(a=>!a.Locked && !String.IsNullOrEmpty(a.UID));
                int intTmpNotLocked = UsersAndUIDs.Count(a => !a.Locked);
                if (intTmpChecked < intTmpNotLocked) { chkSelAll.Checked = false; } else { chkSelAll.Checked = true; }
                rowNo++;
            }
            //Make table header
            AddLabelToTable(tbl, "samAccountName", "samAccountName", h, 10, 100, true, 1, rowNo);
            AddLabelToTable(tbl, "uid", "uid", h, 120, 100, true, 2, rowNo);
            AddLabelToTable(tbl, "uidNumber", "uidNumber", h, 240, 100, true, 3, rowNo);
            AddLabelToTable(tbl, "gid", "gid", h, 360, 100, true, 4, rowNo);
            AddLabelToTable(tbl, "gidNumber", "gidNumber", h, 480, 100, true, 5, rowNo);
            AddLabelToTable(tbl, "UNIXHomeDirectory", "UNIXHomeDirectory", h, 600, 100, true, 6, rowNo);
            AddLabelToTable(tbl, "loginShell", "loginShell", h, 720, 100, true, 7, rowNo);
            rowNo++; h += increment;
            int intSkip = en? intCurrentPageUIDs * intPageSize: intCurrentPageNoUIDs * intPageSize;
            foreach (AD_Object uid in UsersAndUIDs.Skip(intSkip).Take(intPageSize))
            {
                AddLabelToTable(tbl, uid.samAccountName, uid.samAccountName, h, 10, 100, true, 1, rowNo);
                AddTxtBoxToTable(tbl, uid.samAccountName, uid.UID, h, 120, 100, !en, 2, rowNo);
                AddTxtBoxToTable(tbl, uid.samAccountName, uid.UIDNumber, h, 240, 100, !en, 3, rowNo);
                AddTxtBoxToTable(tbl, uid.samAccountName, uid.GID, h, 360, 100, !en, 4, rowNo);
                AddTxtBoxToTable(tbl, uid.samAccountName, uid.GIDNumber, h, 480, 100, !en, 5, rowNo);
                AddTxtBoxToTable(tbl, uid.samAccountName, uid.UNIXHomeDirectory, h, 600, 100, !en, 6, rowNo);
                AddTxtBoxToTable(tbl, uid.samAccountName, uid.LoginShell, h, 720, 100, !en, 7, rowNo);

                if (!en)
                {
                    CheckBox chk = new CheckBox(); chk.Checked = uid.Selected; chk.Tag = uid.samAccountName; chk.Enabled = false;
                    chk.Top = h; chk.Left = 240; tbNoUID.Controls.Add((CheckBox)chk);
                    tbl.Controls.Add(chk); tbl.SetColumn(chk, 8); tbl.SetRow(chk, rowNo);
                    chk.CheckedChanged += ChBoxClicked;
                }
                h += increment; rowNo++;
                Console.WriteLine(rowNo);
            }
            ComboBox cbPager;
            if (!en)
            {
                Button btn = new Button(); btn.Text = "SaveUIDs"; btn.Width = 100; btn.Left = 120;
                btn.Click += SaveToAD; btn.Enabled = false;
                tbl.Controls.Add(btn); tbl.SetColumn(btn, 2); tbl.SetRow(btn, rowNo);
                cbPager = AddPagingControls(intPagesNoUIDs, "chkNoUID", intCurrentPageNoUIDs); cbPager.Visible = intPagesNoUIDs > 0;
                cbPager.SelectedIndexChanged += ChangePageNoUID;
            }
            else
            {
                cbPager = AddPagingControls(intPagesUIDs, "chkUID", intCurrentPageUIDs); cbPager.Visible = intPagesUIDs > 0;
                cbPager.SelectedIndexChanged += ChangePageUID;
           }
            rowNo++;
            tbl.Controls.Add(cbPager); tbl.SetColumn(cbPager, 2); tbl.SetRow(cbPager, rowNo);
            tbl.AutoSize = true;tbl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tbl.Name = en ? "tblUID" : "tblNoUID";
            return tbl;
        }

        private void AddTxtBoxToTable(TableLayoutPanel tbl, String tag, String txt, 
            int top, int L, int W, Boolean Enabled, int colNo, int rowNo)
        {
            TextBox txtUID = new TextBox(); txtUID.Tag = tag; txtUID.Text = txt;
            txtUID.Top = top; txtUID.Left = L; txtUID.Width = W; txtUID.Enabled = Enabled;
            tbl.Controls.Add(txtUID); tbl.SetColumn(txtUID, colNo); tbl.SetRow(txtUID, rowNo);
        }

        private void AddLabelToTable(TableLayoutPanel tbl, String tag, String txt,
            int top, int L, int W, Boolean Enabled, int colNo, int rowNo)
        {
            Label lblName = new Label(); lblName.Text = txt; lblName.Tag = txt;
            lblName.Top = top; lblName.Left = L; lblName.Width = W;
            tbl.Controls.Add(lblName); tbl.SetColumn(lblName, colNo); tbl.SetRow(lblName, rowNo);
        }



        /// <summary>
        /// Event triggered by selection change on the ComboBox on the second (UID) Tab
        /// </summary>
        /// <param name="o">ComboBox on the NoUID Tab</param>
        /// <param name="e">Event arguments</param>
        private void ChangePageNoUID(object o, EventArgs e)
        {
            ComboBox cmb = o as ComboBox;
            intCurrentPageNoUIDs = ((CB_Item)cmb.SelectedItem).Value;
            PopulateNoUIDs();
        }

        /// <summary>
        /// Event triggered by selection change on the ComboBox on the second (UID) Tab
        /// </summary>
        /// <param name="o">ComboBox on the UID Tab</param>
        /// <param name="e">Event arguments</param>
        private void ChangePageUID(object o, EventArgs e)
        {
            ComboBox cmb = o as ComboBox;
            intCurrentPageUIDs = ((CB_Item)cmb.SelectedItem).Value;
            PopulateUIDs();
        }

        /// <summary>
        /// Adds ComboBox used as a pager at the bottom of the tab
        /// </summary>
        /// <param name="intPages">Number of pages (also called maximum number of pages)</param>
        /// <param name="strName">Control name</param>
        /// <param name="intSelectedIndex">Index of the current page (also called selected page)</param>
        /// <returns>Pager ComboBox object that is placed below the AD objects list</returns>
        private ComboBox AddPagingControls(int intPages, String strName, int intSelectedIndex)
        {
            ComboBox cb = new ComboBox(); cb.Name = strName; cb.DisplayMember = "Text"; cb.ValueMember = "Value"; 
            cb.DropDownStyle=System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 1; i <= intPages; i++)
                {
                cb.Items.Add(new CB_Item { Text = "Page " + i + " of " + intPages, Value = i - 1 });
                }
            if (intPages > 0) { cb.SelectedIndex = intSelectedIndex; };
            return cb;
        }

        /// <summary>
        /// Overload for writting AD object's information to AD
        /// </summary>
        /// <param name="AD">AD_Object that gets converted to Dictionary<samAccountName, UID> for processing</param>
        private void SetUserIDs(List<AD_Object> AD) 
        { 
            SetUserIDs(AD.Where(a=>!String.IsNullOrEmpty(a.UID)).ToDictionary(a => a.samAccountName, a => a.UID)); 
        }

        /// <summary>
        /// Overload for writting AD object's information to AD
        /// </summary>
        /// <param name="dicUID">Dictionary containing list of samAccountName and UID key value pairs</param>
        private void SetUserIDs(Dictionary<String, String> dicUID)
        {
            using (var context = new PrincipalContext(ContextType.Domain, "scion.local"))
            {
                using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    foreach (KeyValuePair<String, String> kv in dicUID)
                    {
                        SetUserID((PrincipalContext)context, searcher, kv.Key, kv.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Function that writes AD object's UID into the Active directory
        /// </summary>
        /// <param name="context">System.DirectoryServices.AccountManagement.PrincipalContext</param>
        /// <param name="searcher">System.DirectoryServices.AccountManagement.PrincipalSearcher</param>
        /// <param name="samAccountName">AD object's samAccountName used to search for the AD object</param>
        /// <param name="UID">AD object's UID to write to the Active Directory</param>
        private void SetUserID(PrincipalContext context, PrincipalSearcher searcher, String samAccountName, String UID)
        {
            if (String.IsNullOrEmpty(samAccountName)) { return; }
            if (String.IsNullOrEmpty(UID)) { return; }
            using (Principal res = searcher.QueryFilter =
                            Principal.FindByIdentity(context, "samAccountName=" + samAccountName))
            {
                using (DirectoryEntry de = res.GetUnderlyingObject() as DirectoryEntry)
                {
                    de.AuthenticationType = AuthenticationTypes.Secure;
                    de.Properties["uid"].Value = UID;
                    de.CommitChanges();
                }
            }
        }

        /// <summary>
        /// Returns a list of AD_Object-s that represent user information found in Active Directory
        /// </summary>
        /// <returns>List of AD objects</returns>
        private List<AD_Object> GetUsersUIDs()
        {
            UsersUIDs = new List<AD_Object>();

            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, "scion.local"))
                {
                    using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                    {
                        Object samAccountName; String UID; String UIDNumber; String GIDNumber;
                        String LoginShell; String UNIXHomeDirectory; String GID; Boolean Locked;
                        foreach (var result in searcher.FindAll())
                        {
                            DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                            de.AuthenticationType = AuthenticationTypes.Secure;
                            samAccountName = de.Properties["samAccountName"].Value;
                            if (samAccountName != null && !String.IsNullOrEmpty(samAccountName.ToString()))
                            {
                                Locked = false;
                                UID = ""; UIDNumber = ""; GIDNumber = ""; 
                                LoginShell = ""; UNIXHomeDirectory = ""; GID = "";
                                if (de.Properties["uid"].Value != null)
                                {
                                    Locked = true;
                                    UID = de.Properties["uid"].Value.ToString();
                                    UIDNumber = de.Properties["uidNumber"].Value != null ? de.Properties["uidNumber"].Value.ToString() : "Not defined";
                                    GID = de.Properties["gid"].Value != null ? de.Properties["gid"].Value.ToString() : "Not defined";
                                    GIDNumber = de.Properties["gidNumber"].Value != null ? de.Properties["gidNumber"].Value.ToString() : "Not defined";
                                    LoginShell = de.Properties["loginShell"].Value != null ? de.Properties["loginShell"].Value.ToString() : "Not defined";
                                    UNIXHomeDirectory = de.Properties["UNIXHomeDirectory"].Value != null ? de.Properties["UNIXHomeDirectory"].Value.ToString() : "Not defined";
                                }
                                UsersUIDs.Add(new AD_Object() { samAccountName = samAccountName.ToString(),
                                    UID = UID, UIDNumber = UIDNumber, GIDNumber = GIDNumber, LoginShell = LoginShell, 
                                    UNIXHomeDirectory = UNIXHomeDirectory, GID = GID, Locked = Locked }) ;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                UsersUIDs = GetTestData();
            }
            return UsersUIDs;
        }

        /// <summary>
        /// Next String UID is returned for AD objects that need UID. 
        /// Generated using Random function, thus slow.
        /// </summary>
        /// <returns>New UID string (integer converted to a String)</returns>
        private String GetNextUID()
        {
            String L; Random rnd = new Random(DateTime.Now.Second);
            List<String> lstUIDs = UsersUIDs.Where(u => !String.IsNullOrEmpty(u.UID)).Select(a => a.UID).ToList();
            do
            {
                L = rnd.Next().ToString(); 
            } 
            while (lstUIDs.Contains(L));
            return L;
        }

        /// <summary>
        /// This function returns test data
        /// </summary>
        /// <returns>List of Active Directory objcts</returns>
        private List<AD_Object> GetTestData()
        {
            var res = new List<AD_Object>(); var tmpList = new List<int>(); int tmp;
            Random rnd = new Random(DateTime.Now.Second);
            for (int i = 1; i < 1200; i++)
            {
                do {tmp = rnd.Next();} while (tmpList.Contains(tmp)); tmpList.Add(tmp);
                if (Math.Ceiling((double)tmp / 2) == tmp / 2)
                {
                    res.Add(new AD_Object()
                    {
                        Locked = true,
                        samAccountName = "Name_" + tmp,
                        UIDNumber = "UID_N_" + tmp,
                        GID = "GID_" + tmp,
                        GIDNumber = "GID_N_" + tmp,
                        LoginShell = "Shell_" + tmp,
                        UNIXHomeDirectory = "UNIX_" + tmp,
                        Selected = false,
                        UID = tmp.ToString()
                    });
                }
                else
                {
                    res.Add(new AD_Object() { Locked = false, samAccountName = "Name_" + tmp, Selected = false });
                }
            }
            return res;
        }
    }

    /// <summary>
    /// Class for use for objects collecting information on Active Directory entries
    /// Active directory entries are users, groups and others. 
    /// </summary>
    public class AD_Object
    {
        public String samAccountName { get; set; }
        public String UID { get; set; }
        public String UIDNumber { get; set; }
        public String GID { get; set; }
        public String GIDNumber { get; set; }
        public String LoginShell { get; set; }
        public String UNIXHomeDirectory { get; set; }
        public Boolean Selected { get; set; }
        public Boolean Locked { get; set; }
    }

    /// <summary>
    /// Class for use as ComboBoxItem
    /// </summary>
    public class CB_Item
    {
        public String Text { get; set; }
        public int Value { get; set; }
    }

}

