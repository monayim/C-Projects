using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace QuantumBitsComputerShopManagementSystem
{
    public partial class FormLogin : Form
    {
        private FormEmployee FEmployee { get; set; }
        private FormSales FSales { get; set; }

        public FormLogin()
        {
            InitializeComponent();
        }
        // When the form is created, it will also create the FormEmployee
        public FormLogin(FormEmployee fEmployee)
        {
            InitializeComponent();
            this.FEmployee = fEmployee;
        }

        // Check if all fields are filled
        private bool IsValidToLogin()
        {
            if (string.IsNullOrEmpty(this.txtUserId.Text) || String.IsNullOrEmpty(this.txtPassword.Text))
            {
                return false;
            }
            return true;
        }

        private void ckbShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkbShowPassword.Checked)
            {
                this.txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                this.txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void ClearFields()
        {
            this.txtUserId.Clear();
            this.txtPassword.Clear();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsValidToLogin())
                {
                    MessageBox.Show("Please fill all fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataAccess db = new DataAccess();
                DataSet ds =
                    db.ExecuteQuery(
                        $"SELECT * FROM UserInformation WHERE UserID = {this.txtUserId.Text} AND Password = '{txtPassword.Text}';");

                if (ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("Invalid Username or Password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (ds.Tables[0].Rows[0]["Role"].ToString() == "admin")
                {
                    MessageBox.Show("Welcome Admin!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.FEmployee = new FormEmployee(this, Convert.ToInt32(ds.Tables[0].Rows[0][0]));
                    FormEmployee.SwitchForm(this, this.FEmployee);
                    ClearFields();
                }
                else if (ds.Tables[0].Rows[0]["Role"].ToString() == "employee")
                {
                    MessageBox.Show($"Welcome, {ds.Tables[0].Rows[0][1].ToString()}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.FSales = new FormSales(this, ds.Tables[0].Rows[0][1].ToString(), Convert.ToInt32(ds.Tables[0].Rows[0][0]));
                    FormEmployee.SwitchForm(this, this.FSales);
                    ClearFields();
                }

                //this.FEmployee = new FormEmployee(this);
                //FormEmployee.SwitchForm(this, this.FEmployee);


                //this.FSales = new FormSales(this, 101);
                //FormEmployee.SwitchForm(this, this.FSales);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void txtUserId_KeyPress(object sender, KeyPressEventArgs e)
        {
            FormEmployee.NumericValueOnly(e, this.txtUserId.Text);
        }
    }
}
