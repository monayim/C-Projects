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
    public partial class FormChangeAdminPass : Form
    {
        private FormLogin FLogin { get; set; }
        private FormEmployee FEmployee { get; set; }
        private int AdminID { get; set; }
        private DataAccess db = new DataAccess();

        public FormChangeAdminPass()
        {
            InitializeComponent();
        }

        public FormChangeAdminPass(FormLogin fLogin, FormEmployee fEmployee, int adminId)
        {
            InitializeComponent();
            this.FLogin = fLogin;
            this.AdminID = adminId;
            this.FEmployee = fEmployee;
            this.txtAdminId.Text = adminId.ToString();
        }

        private void FormChangeAdminPass_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormEmployee.SwitchForm(this, this.FEmployee);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.txtNewPass.Text) || string.IsNullOrEmpty(this.txtConfirmPass.Text))
                {
                    MessageBox.Show("Please all the field", "Empty Field", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (!this.txtNewPass.Text.Equals(this.txtConfirmPass.Text))
                {
                    MessageBox.Show("Password didn't matched", "Password Missmatch", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                int result = db.ExecuteDMLQuery($"UPDATE UserInformation SET Password = '{this.txtNewPass.Text}' WHERE UserID = {this.AdminID};");
                if(result == 1)
                {
                    MessageBox.Show("Password Updated Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Password Updated Failed", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception exp)
            {
                {
                    MessageBox.Show($"ERROR: {exp.Message}");
                }
            }
        }
    }
}