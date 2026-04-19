using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuantumBitsComputerShopManagementSystem
{
    public partial class FormSupplier : Form 
    {
        private FormLogin FLogin { get; set; }
        private FormEmployee FEmployee { get; set; }
        private FormProductDetails FProductDetails { get; set; }
        private FormSalesHistory FSalesHistory { get; set; }
        private DataAccess db = new DataAccess();
        private DataSet Ds { set; get; }
        private FormUpdateSupplierInfo FUpdateSupplierInfo { get; set; }
        private int validity;

        public FormSupplier()
        {
            InitializeComponent();
        }

        public FormSupplier(FormLogin fLogin, FormEmployee fEmployee, FormProductDetails fProductDetails, FormSalesHistory fSalesHistory)
        {
            InitializeComponent();
            this.FLogin = fLogin;
            this.FEmployee = fEmployee;
            this.FProductDetails = fProductDetails;
            this.FSalesHistory = fSalesHistory;
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            this.FEmployee = new FormEmployee(this.FLogin, this.FProductDetails, this.FSalesHistory, this);
            FormEmployee.SwitchForm(this, this.FEmployee);
        }

        private void btnProductDetails_Click(object sender, EventArgs e)
        {
            this.FProductDetails = new FormProductDetails(this.FLogin, this.FEmployee, this.FSalesHistory, this);
            FormEmployee.SwitchForm(this, this.FProductDetails);
        }

        private void btnSalesHistory_Click(object sender, EventArgs e)
        {
            this.FSalesHistory = new FormSalesHistory(this.FLogin, this.FEmployee, this.FProductDetails, this);
            FormEmployee.SwitchForm(this, this.FSalesHistory);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            FormEmployee.LogOutWarning(this, this.FLogin);
        }

        private void FormSupplier_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormEmployee.SwitchForm(this, this.FLogin);
        }

        // Above code contains Form Switching logic , which is not a good practice.
        // Below code does other operations like loading data from database and displaying it in DataGridView.

        public void LoadSupplierDataGridView()
        {
            try
            {
                this.Ds = db.ExecuteQuery("SELECT * FROM SupplierInformation;");
                this.dgvSupplierInfo.AutoGenerateColumns = false;
                this.dgvSupplierInfo.DataSource = Ds.Tables[0];
                this.dgvSupplierInfo.ClearSelection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error loading Supplier Information!");
            } 

        }

        private void SearchSupplier()
        {
            try
            {
                Ds = db.ExecuteQuery($"SELECT * FROM SupplierInformation WHERE CompanyName LIKE '%{this.txtSearchBox.Text}%' OR Phone LIKE '%{this.txtSearchBox.Text}%';");
                dgvSupplierInfo.AutoGenerateColumns = false;
                dgvSupplierInfo.DataSource = Ds.Tables[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error loading Supplier Information!");
            }
        }

        private void ClearAllFields()
        {
            this.txtSupplierId.Clear();
            this.txtCompanyName.Clear();
            this.txtPhone.Clear();
            this.txtEmail.Clear();
            this.txtAddress.Clear();
            this.txtSearchBox.Clear();
        }

        private bool IsValidToSave()
        {
            if (string.IsNullOrEmpty(this.txtSupplierId.Text) || string.IsNullOrEmpty(this.txtCompanyName.Text) || string.IsNullOrEmpty(this.txtPhone.Text) || string.IsNullOrEmpty(this.txtAddress.Text))
            {
                MessageBox.Show("Please fill all fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void FormSupplier_Load(object sender, EventArgs e)
        {
            LoadSupplierDataGridView();
        }

        private void btnShowDetails_Click(object sender, EventArgs e)
        {
            LoadSupplierDataGridView();
        }

        private void txtSearchBox_TextChanged(object sender, EventArgs e)
        {
            SearchSupplier();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSearchBox.Text))
            {
                MessageBox.Show("Please enter a value to search!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SearchSupplier();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            FormEmployee.NumericValueOnly(e, this.txtPhone.Text);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValidToSave())
            {
                return;
            }

            try
            {
                validity = db.ExecuteDMLQuery($"INSERT INTO SupplierInformation VALUES({Convert.ToInt32(this.txtSupplierId.Text)}, '{this.txtCompanyName.Text}', '{this.txtEmail.Text}', {Convert.ToInt64(this.txtPhone.Text)}, '{this.txtAddress.Text}');");
                if (validity == 1)
                {
                    MessageBox.Show("Supplier Information saved successfully!", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSupplierDataGridView();
                    ClearAllFields();
                }
                else
                {
                    MessageBox.Show("Failed to save Supplier Information!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show("Please input valid information!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgvSupplierInfo_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int supplierId = Convert.ToInt32(this.dgvSupplierInfo.SelectedRows[0].Cells[0].Value);
                string companyName = this.dgvSupplierInfo.SelectedRows[0].Cells[1].Value.ToString();
                string email = this.dgvSupplierInfo.SelectedRows[0].Cells[2].Value.ToString();
                long phone = Convert.ToInt64(this.dgvSupplierInfo.SelectedRows[0].Cells[3].Value);
                string address = this.dgvSupplierInfo.SelectedRows[0].Cells[4].Value.ToString();

                FUpdateSupplierInfo = new FormUpdateSupplierInfo(this, supplierId, companyName, email, phone, address); 
                FormEmployee.SwitchForm(this, this.FUpdateSupplierInfo);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show("Please Select a valid row", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteSupplier_Click(object sender, EventArgs e)
        {
            if (this.dgvSupplierInfo.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a row to delete!", "Select a Row", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete {this.dgvSupplierInfo.CurrentRow.Cells[1].Value.ToString()} Information?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.No)
            {
                return;
            }

            try
            {
                validity = db.ExecuteDMLQuery($"DELETE FROM SupplierInformation WHERE SupplierID = {Convert.ToInt32(this.dgvSupplierInfo.CurrentRow.Cells[0].Value)};");
                if (validity == 1)
                {
                    MessageBox.Show("Supplier Information deleted successfully!", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSupplierDataGridView();
                }
                else
                {
                    MessageBox.Show("Failed to delete Supplier Information!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show("Please select a valid row to delete!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } 
        }
    }
}
