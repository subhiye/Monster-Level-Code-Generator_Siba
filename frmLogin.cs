using Code_Generator_Siba_business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Generator_Code_Siba
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            clsCheckingConnection Data = clsCheckingConnection.ValidateDatabaseConnection(txtServerID.Text, txtDataBaseName.Text, txtUserName.Text, txtPassword.Text);

            clsRegistry_Settings.StoreIntoRegistry(txtKeyPath.Text, lblDataBase.Text, txtDataBaseName.Text, txtDataBaseName.Text);
            clsRegistry_Settings.StoreIntoRegistry(txtKeyPath.Text, lblPassword.Text, txtPassword.Text, txtDataBaseName.Text);
            clsRegistry_Settings.StoreIntoRegistry(txtKeyPath.Text, lblServerName.Text, txtServerID.Text, txtDataBaseName.Text);
            clsRegistry_Settings.StoreIntoRegistry(txtKeyPath.Text, lblUserID.Text, lblUserID.Text, txtDataBaseName.Text);

            if (Data != null)
            {
                frmMain frm = new frmMain(txtDataBaseName.Text, txtServerID.Text, txtUserName.Text, txtPassword.Text, txtKeyPath.Text);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Unable to connect to the database. Please check your credentials and try again.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clsUtil.OpeningSqlApp();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtServerID.Text = ".";
            txtUserName.Text = "sa";
            txtPassword.Text = "sa123456";
            txtKeyPath.Text = "Current";
            txtDataBaseName.Focus();
        }
    }
}
