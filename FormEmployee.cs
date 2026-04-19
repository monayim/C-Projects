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
    public partial class FormEmployee : Form
    {
        private FormLogin FLogin { get; set; }
        private FormProductDetails FProductDetails { get; set; }
        private FormSalesHistory FSalesHistory { get; set; }
        private FormSupplier FSupplier { get; set; }
        private FormUpdateEmployeeInfo FUpdateEmployeeInfo { get; set; }
        private FormChangeAdminPass FChangeAdminPass { get; set; }
        private int AdminId { get; set; }
        private DataAccess db = new DataAccess();

        public FormEmployee()
        {
            InitializeComponent();
        }

        public FormEmployee(FormLogin formLogin, int adminId)
        {
            InitializeComponent();
            this.FLogin = formLogin;
            this.AdminId = adminId;
            this.lblAdminId.Text = adminId.ToString();
        }

        public FormEmployee(FormLogin fLogin, FormProductDetails fProductDetails, FormSalesHistory fSalesHistory, FormSupplier fSupplier)
        {
            InitializeComponent();
            this.FLogin = fLogin;
            this.FProductDetails = fProductDetails;
            this.FSalesHistory = fSalesHistory;
            this.FSupplier = fSupplier;
        }

        public static void LogOutWarning(Form currentForm, Form loginForm)
        {
            DialogResult dialogResult = MessageBox.Show($"Are you sure you want to Logout?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
            {
                return; 
            }
            FormEmployee.SwitchForm(currentForm, loginForm);
        }

        public static void SwitchForm(Form currentForm, Form nextForm)
        {
            currentForm.Visible = false;
            nextForm.Visible = true;
        }

        private void btnProductDetails_Click(object sender, EventArgs e)
        {
            this.FProductDetails = new FormProductDetails(this.FLogin, this, this.FSalesHistory, this.FSupplier);
            FormEmployee.SwitchForm(this, this.FProductDetails);
        }

        private void btnSalesHistory_Click(object sender, EventArgs e)
        {
            this.FSalesHistory = new FormSalesHistory(this.FLogin, this, this.FProductDetails, this.FSupplier);
            FormEmployee.SwitchForm(this, this.FSalesHistory);
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            this.FSupplier = new FormSupplier(this.FLogin, this, this.FProductDetails, this.FSalesHistory);
            FormEmployee.SwitchForm(this, this.FSupplier);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            FormEmployee.LogOutWarning(this, this.FLogin);
        }

        private void FormEmployee_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormEmployee.SwitchForm(this, this.FLogin);
        }


        // Above code contains Form Switching logic , which is not a good practice.
        // Below code does other operations like loading data from database and displaying it in DataGridView.
        public void PopulateDataGridView()
        {
            DataSet dS = db.ExecuteQuery("SELECT * FROM EmployeeDetails;");
            this.dgvEmployeeDetails.AutoGenerateColumns = false;
            this.dgvEmployeeDetails.DataSource = dS.Tables[0];
            this.dgvEmployeeDetails.ClearSelection();
        }

        private void SearchEmployee()
        {
            DataSet dS = db.ExecuteQuery($"SELECT * FROM EmployeeDetails WHERE EmployeeID LIKE '%{this.txtSearchBox.Text}%' OR FullName LIKE '%{this.txtSearchBox.Text}%' OR Phone LIKE '%{this.txtSearchBox.Text}%';");
            this.dgvEmployeeDetails.AutoGenerateColumns = false;
            this.dgvEmployeeDetails.DataSource = dS.Tables[0];
        }

        private void ClearAllFields()
        {
            this.txtEmployeeId.Clear();
            this.txtFullName.Clear();
            this.txtEmail.Clear();
            this.txtPhone.Clear();
            this.cmbGender.SelectedIndex = -1;
            this.txtAddress.Clear();
            this.txtPassword.Clear();
            this.txtSearchBox.Clear();
        }

        private bool IsValidToSave()
        {
            if (string.IsNullOrEmpty(this.txtEmployeeId.Text) || string.IsNullOrEmpty(this.txtFullName.Text) || string.IsNullOrEmpty(this.txtEmail.Text) || string.IsNullOrEmpty(this.txtPhone.Text) || string.IsNullOrEmpty(this.cmbGender.Text) || string.IsNullOrEmpty(this.txtAddress.Text) || string.IsNullOrEmpty(this.txtPassword.Text))
            {
                MessageBox.Show("Please fill all fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        public static void NumericValueOnly(KeyPressEventArgs e, String text)
        {
            char ch = e.KeyChar;
            if (char.IsDigit(ch) || ch == 8)
            {
                if (text.Length >= 11 && ch != 8)
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void FormEmployee_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }

        private void btnShowDetails_Click(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }

        private void txtSearchBox_TextChanged(object sender, EventArgs e)
        {
            SearchEmployee();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchEmployee();
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            NumericValueOnly(e, this.txtPhone.Text);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void ckbShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbShowPassword.Checked)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsValidToSave())
                {
                    return;
                }

                DataAccess db = new DataAccess();
                int result1 = db.ExecuteDMLQuery($"INSERT INTO EmployeeDetails VALUES('{this.txtEmployeeId.Text}', '{this.txtFullName.Text}', '{this.txtEmail.Text}', {Convert.ToInt64(this.txtPhone.Text)}, '{this.cmbGender.Text}', '{this.dtpJoinDate.Text}', '{this.txtAddress.Text}', '{this.txtPassword.Text}');");
                int result2 = db.ExecuteDMLQuery($"INSERT INTO UserInformation VALUES({Convert.ToInt32(this.txtEmployeeId.Text)}, '{this.txtFullName.Text}', 'employee', '{this.txtPassword.Text}');");
                if (result1+result2 == 2)
                {
                    MessageBox.Show("Employee Details Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateDataGridView();
                    ClearAllFields();
                }
                else
                {
                    MessageBox.Show("Failed to Save Employee Details!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"ERROR: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvEmployeeDetails_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvEmployeeDetails.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a row to update!", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string employeeId = this.dgvEmployeeDetails.CurrentRow.Cells[0].Value.ToString();
                string fullName = this.dgvEmployeeDetails.CurrentRow.Cells[1].Value.ToString();
                string email = this.dgvEmployeeDetails.CurrentRow.Cells[2].Value.ToString();
                Int64 employeePhone = Convert.ToInt64(this.dgvEmployeeDetails.CurrentRow.Cells[3].Value);
                string gender = this.dgvEmployeeDetails.CurrentRow.Cells[4].Value.ToString();
                string joinDate = this.dgvEmployeeDetails.CurrentRow.Cells[5].Value.ToString();
                string address = this.dgvEmployeeDetails.CurrentRow.Cells[6].Value.ToString();
                string password = this.dgvEmployeeDetails.CurrentRow.Cells[7].Value.ToString();

                this.FUpdateEmployeeInfo = new FormUpdateEmployeeInfo(this, employeeId, fullName, email, employeePhone,
                    gender, joinDate, address, password);
                SwitchForm(this, this.FUpdateEmployeeInfo);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"ERROR: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteEmployee_Click(object sender, EventArgs e)
        {
            if (this.dgvEmployeeDetails.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a row to delete!", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete this {this.dgvEmployeeDetails.CurrentRow.Cells[1].Value.ToString()}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                int result1 = db.ExecuteDMLQuery($"DELETE FROM EmployeeDetails WHERE EmployeeID = {Convert.ToInt32(this.dgvEmployeeDetails.CurrentRow.Cells[0].Value)};");
                int result2 = db.ExecuteDMLQuery($"DELETE FROM UserInformation WHERE UserID = {Convert.ToInt32(this.dgvEmployeeDetails.CurrentRow.Cells[0].Value)};");
                if (result1 + result2 == 2)
                {
                    MessageBox.Show($"{this.dgvEmployeeDetails.CurrentRow.Cells[1].Value.ToString()} Information deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateDataGridView();
                }
                else
                {
                    MessageBox.Show($"Failed to delete {this.dgvEmployeeDetails.CurrentRow.Cells[1].Value.ToString()} Information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            this.FChangeAdminPass = new FormChangeAdminPass(this.FLogin, this, Convert.ToInt32(this.lblAdminId.Text));
            FormEmployee.SwitchForm(this, this.FChangeAdminPass);
        }
    }
}