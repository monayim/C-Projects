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
    public partial class FormUpdateSupplierInfo : Form
    {
        private FormSupplier FSupplier { get; set; }
        private int SupplierId { get; set; }
        private string CompanyName { get; set; }
        private string Email { get; set; }
        private Int64 Phone { get; set; }
        private string Address { get; set; }
        private DataAccess db = new DataAccess();
        private int validity;

        public FormUpdateSupplierInfo()
        {
            InitializeComponent();
        }

        public FormUpdateSupplierInfo(FormSupplier fSupplier, int supplierId, string companyName, string email, Int64 phone, string address)
        {
            InitializeComponent();
            this.FSupplier = fSupplier;
            this.SupplierId = supplierId;
            this.CompanyName = companyName;
            this.Email = email;
            this.Phone = phone;
            this.Address = address;
        }

        private void SetAllFields()
        {
            this.txtSupplierId.Text = this.SupplierId.ToString();
            this.txtCompanyName.Text = this.CompanyName;
            this.txtEmail.Text = this.Email;
            this.txtPhone.Text = this.Phone.ToString();
            this.txtAddress.Text = this.Address;
        }

        private bool IsValidToUpdate()
        {
            if (string.IsNullOrEmpty(this.txtCompanyName.Text) || string.IsNullOrEmpty(this.txtPhone.Text) ||
                string.IsNullOrEmpty(this.txtAddress.Text))
            {
                MessageBox.Show("Please fill all the fields", "Fill Empty Fields", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private void FormUpdateSupplierInfo_Load(object sender, EventArgs e)
        {
            SetAllFields();
        }

        private void FormUpdateSupplierInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormEmployee.SwitchForm(this, this.FSupplier);
            this.FSupplier.LoadSupplierDataGridView();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            SetAllFields();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsValidToUpdate())
                {
                    return;
                }

                this.validity = db.ExecuteDMLQuery(
                    $"UPDATE SupplierInformation SET CompanyName = '{this.txtCompanyName.Text}', Email = '{this.txtEmail.Text}', Phone = {Convert.ToInt64(this.txtPhone.Text)}, Address = '{this.txtAddress.Text}' WHERE SupplierID = {Convert.ToInt32(this.txtSupplierId.Text)};");
                if (this.validity == 1)
                {
                    MessageBox.Show("Supplier Information updated successfully!", "Successfull", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update Supplier Information!", "Information", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            FormEmployee.NumericValueOnly(e, this.txtPhone.Text);
        }
    }
}