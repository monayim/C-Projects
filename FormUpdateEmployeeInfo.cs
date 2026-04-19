using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace QuantumBitsComputerShopManagementSystem
{
    public partial class FormUpdateEmployeeInfo : Form
    {
        private FormEmployee FEmployee { get; set; }
        private string EmployeeId { get; set; }
        private string FullName { get; set; }
        private string Email { get; set; }
        private Int64 EmployeePhone { get; set; }
        private string Gender { get; set; }
        private string JoinDate { get; set; }
        private string Address { get; set; }
        private string Password { get; set; }

        public FormUpdateEmployeeInfo()
        {
            InitializeComponent();
        }

        public FormUpdateEmployeeInfo(FormEmployee formEmployee, String employeeId, string fullName, string email, Int64 employeePhone, string gender, string joinDate, string address,  string password)
        {
            InitializeComponent();
            this.FEmployee = formEmployee;
            this.EmployeeId = employeeId;
            this.FullName = fullName;
            this.Email = email;
            this.EmployeePhone = employeePhone;
            this.Gender = gender;
            this.JoinDate = joinDate;
            this.Address = address;
            this.Password = password;

            ResetAllFields();
        }

        private void ResetAllFields()
        {
            this.txtEmployeeId.Text = this.EmployeeId;
            this.txtFullName.Text = this.FullName;
            this.txtEmail.Text = this.Email;
            this.txtPhone.Text = this.EmployeePhone.ToString();
            this.cmbGender.Text = this.Gender;
            this.dtpJoinDate.Text = this.JoinDate;
            this.txtAddress.Text = this.Address;
            this.txtPassword.Text = this.Password;
        }

        private bool IsValidToUpdate()
        {
            if (string.IsNullOrEmpty(this.txtFullName.Text) || string.IsNullOrEmpty(this.txtEmail.Text) || string.IsNullOrEmpty(this.txtPhone.Text) || string.IsNullOrEmpty(this.cmbGender.Text) || string.IsNullOrEmpty(this.txtAddress.Text) || string.IsNullOrEmpty(this.txtPassword.Text))
            {
                MessageBox.Show("Please fill all fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }


        private void FormUpdateEmployeeInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormEmployee.SwitchForm(this, this.FEmployee);
            this.FEmployee.PopulateDataGridView();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetAllFields();
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

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            FormEmployee.NumericValueOnly(e, this.txtPhone.Text);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsValidToUpdate())
                {
                    MessageBox.Show("Please fill all fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataAccess db = new DataAccess();

                int result1 = db.ExecuteDMLQuery($"UPDATE EmployeeDetails SET FullName = '{this.txtFullName.Text}', Email = '{this.txtEmail.Text}', Phone = {Convert.ToInt64(this.txtPhone.Text)}, Gender = '{this.cmbGender.Text}', JoinDate = '{this.dtpJoinDate.Text}', Address='{this.txtAddress.Text}', Password = '{this.txtPassword.Text}' WHERE EmployeeID = '{this.txtEmployeeId.Text}';");
                int result2 = db.ExecuteDMLQuery($"UPDATE UserInformation SET FullName = '{this.txtFullName.Text}', Password = '{this.txtPassword.Text}' WHERE UserID = {Convert.ToInt32(this.txtEmployeeId.Text)};");
                if (result1+result2 == 2)
                {
                    MessageBox.Show("Employee Information Updated Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //this.FEmployee = new FormEmployee();
                    //this.FEmployee.PopulateDataGridView();
                    //FormEmployee.SwitchForm(this, this.FEmployee); 
                }
                else
                {
                    MessageBox.Show("Failed to Update Employee Information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"ERROR: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
