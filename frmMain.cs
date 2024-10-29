using Code_Generator_Siba_business;
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

namespace Generator_Code_Siba
{
    public partial class frmMain : Form
    {
        string _DataBaseName = "", _ServerID = "", _UserName = "", _Password = "", _KeyPath;

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (txtBusinessString.Text.Length > 0)
            {
                Clipboard.SetText(txtBusinessString.Text);
                MessageBox.Show("Your Function Is Copied Successfully.", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Your Function Is Empty. Generate A Function To Copy.", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            txtBusinessString.Clear();
            txtDataString.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTableName.Text))
            {
                errorProvider1.SetError(txtTableName, "Fill This Field Please To Generate Your Code");
                txtTableName.Focus();
                return;
            }

            List<string> ListRegistryDetails = new List<string>
            {
                _ServerID,
                _DataBaseName,
                _UserName,
                _Password
            };

            if (!clsColumn.IsTableFound(txtTableName.Text, _KeyPath, ListRegistryDetails))
            {
                MessageBox.Show("There Is No Table Called " + txtTableName.Text, "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<clsColumn> columns = clsColumn.GetColumnsFromDataBase(_DataBaseName, txtTableName.Text, _KeyPath, ListRegistryDetails);

            if (columns != null)
            {
                dataGridView1.DataSource = columns;
                if (dataGridView1.ColumnCount > 0)
                {
                    dataGridView1.Columns[0].HeaderText = "Column Name";
                    dataGridView1.Columns[0].Width = 150;
                    dataGridView1.Columns[1].HeaderText = "Data Type";
                    dataGridView1.Columns[1].Width = 120;
                    dataGridView1.Columns[2].HeaderText = "Is Null";
                    dataGridView1.Columns[2].Width = 80;
                }
                MessageBox.Show(@"Make Sure That Your Application And Your Data Base Are In The Same Name Or The Code Generator Will Not Works Correctly", "Hello",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtBusinessString.Text = "";
                txtDataString.Text = "";
                button4.Enabled = false;
            }
            else
            {
                MessageBox.Show("No columns found or unable to fetch columns.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetBusinessLayerCode(List<clsColumn> Columns, string ClassName)
        {
            string ConnectionString = "/// Add Your Connection String Here ////";
            return clsGenerate_Business_Layer_Class.GenerateAllBusinessLayerMethods(txtTableName.Text, ConnectionString, Columns);
        }

        private string GetDataLayerCode(List<clsColumn> Columns, string ClassName)
        {
            string ConnectionString = "/// Add Your Connection String Here ////";
            return clsGenerate_Data_Layer_Class.GetDataLayerAllFuncitonsCode(txtTableName.Text, ConnectionString, Columns);
        }

        private void StoreFileToBusinessLayer(string BusinessLayerClassCode, string className, string filePath)
        {
            clsGenerate_Business_Layer_Class.CreateClassFile(BusinessLayerClassCode, className, filePath);
        }

        private void OpenFileByItsPath(string FilePath)
        {
            System.Diagnostics.Process.Start("code", FilePath);
        }

        private bool IsFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTableName.Text))
            {
                errorProvider1.SetError(txtTableName, "Fill This Field Please To Generate Your Code");
                txtTableName.Focus();
                return;
            }

            MessageBox.Show("Make Sure That You Created A Project For The Business , Data Layer Before Generating Any Code", "Be Careful", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            MessageBox.Show("Are You Ready?", "Amazing", MessageBoxButtons.OK, MessageBoxIcon.Information);

            List<string> listRegistryDetails = new List<string>
            {
                _ServerID,
                _DataBaseName,
                _UserName,
                _Password
            };

            // Determine file paths
            string FilePathForBusiness = clsUtil.GetFilePath() + "\\" + _DataBaseName + "_Business" + "\\" + txtTableName.Text + ".cs";
            string FilePathForData = clsUtil.GetFilePath() + "\\" + _DataBaseName + "_Data" + "\\" + txtTableName.Text + ".cs";

            // Check if files already exist
            if (IsFileExist(FilePathForBusiness) || IsFileExist(FilePathForData))
            {
                MessageBox.Show("File already exists. Please delete the existing file before generating a new one.", "File Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                button4.Enabled = true;
                return;
            }

            List<clsColumn> columns = clsColumn.GetColumnsFromDataBase(_DataBaseName, txtTableName.Text, _KeyPath, listRegistryDetails);

            if (columns != null)
            {
                string ApplicationName = _DataBaseName; // Assume you have a textbox for the class name
                string BusinessLayerClassCode = GetBusinessLayerCode(columns, ApplicationName);
                string DataLayerClassCode = GetDataLayerCode(columns, ApplicationName);

                txtBusinessString.Text = BusinessLayerClassCode;
                txtDataString.Text = DataLayerClassCode;

                string ClassNameBusiness = "cls" + txtTableName.Text;
                string ClassNameData =  txtTableName.Text + "_Data";

                StoreFileToBusinessLayer(BusinessLayerClassCode, ClassNameBusiness, clsUtil.GetFilePath() + "\\" + _DataBaseName + "_Business");
                StoreFileToDataLayer(DataLayerClassCode, ClassNameData, clsUtil.GetFilePath() + "\\" + _DataBaseName + "_Data");

                MessageBox.Show("Class file generated successfully As A File And You Can Go To See It.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button4.Enabled = true;
                txtTableName.Text = "";

                // Clear the DataGridView as well
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
            }
            else
            {
                MessageBox.Show("No columns found or unable to fetch columns.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtTableName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTableName.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTableName, "Fill This Field Please.");
            }
            else
            {
                errorProvider1.SetError(txtTableName, "");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StoreFileToDataLayer(string dataLayerClassCode, string className, string filePath)
        {
            clsGenerate_Business_Layer_Class.CreateClassFile(dataLayerClassCode, className, filePath);
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            clsUtil.OpeningVisualStudio();
        }

        private void btnAdding_Click(object sender, EventArgs e)
        {
            clsUtil.OpeningSqlApp();
        }

        public frmMain(string DataBaseName, string ServerID, string UserName, string Password, string KeyPath)
        {
            InitializeComponent();
                _DataBaseName = DataBaseName;
                _ServerID = ServerID;
                _UserName = UserName;
                _Password = Password;
                _KeyPath = KeyPath;
        }
    }
}