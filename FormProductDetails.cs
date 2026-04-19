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
    public partial class FormProductDetails : Form
    {
        private FormLogin FLogin { get; set; }
        private FormEmployee FEmployee { get; set; }
        private FormSalesHistory FSalesHistory { get; set; }
        private FormSupplier FSupplier { get; set; }
        private FormUpdateProductInfo FUpdateProductInfo { get; set; }
        private DataAccess db = new DataAccess();
        private DataSet Ds { set; get; }
        private int Result { set; get; }

        public FormProductDetails()
        {
            InitializeComponent();
        }
        public FormProductDetails(FormLogin fLogin, FormEmployee fEmployee, FormSalesHistory fSalesHistory, FormSupplier fSupplier)
        {
            InitializeComponent();
            this.FLogin = fLogin;
            this.FEmployee = fEmployee;
            this.FSalesHistory = fSalesHistory;
            this.FSupplier = fSupplier;
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            this.FEmployee = new FormEmployee(this.FLogin, this, this.FSalesHistory, this.FSupplier);
            FormEmployee.SwitchForm(this, this.FEmployee);
        }

        private void btnSalesHistory_Click(object sender, EventArgs e)
        {
            this.FSalesHistory = new FormSalesHistory(this.FLogin, this.FEmployee, this, this.FSupplier);
            FormEmployee.SwitchForm(this, this.FSalesHistory);
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            this.FSalesHistory = new FormSalesHistory(this.FLogin, this.FEmployee, this, this.FSupplier);
            FormEmployee.SwitchForm(this, this.FSalesHistory);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            FormEmployee.LogOutWarning(this, this.FLogin);
        }

        private void FormProductDetails_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormEmployee.SwitchForm(this, this.FLogin);
        }


        // Above code contains Form Switching logic , which is not a good practice.
        // Below code does other operations like loading data from database and displaying it in DataGridView.

        public void PopulateDataGridView()
        {
            try
            {
                this.Ds = db.ExecuteQuery("SELECT * FROM ProductInformation;SELECT * FROM SupplierInformation;");
                this.dgvProductDetails.AutoGenerateColumns = false;
                this.dgvProductDetails.DataSource = Ds.Tables[0];
                this.dgvProductDetails.ClearSelection();

                this.cmbSupplier.DataSource = Ds.Tables[1];
                this.cmbSupplier.ValueMember = "CompanyName";
                this.cmbSupplier.SelectedIndex = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Failed to load data", "Data Grid View ERROR", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SearchProduct()
        {
            try
            {
                Ds = db.ExecuteQuery($"SELECT * FROM ProductInformation WHERE ProductID LIKE '%{this.txtSearchBox.Text}%' OR ProductName LIKE '%{this.txtSearchBox.Text}%' OR Manufacturer LIKE '%{this.txtSearchBox.Text}%' OR Supplier LIKE '%{this.txtSearchBox.Text}%' OR Category LIKE '%{this.txtSearchBox.Text}%';");
                this.dgvProductDetails.AutoGenerateColumns = false;
                this.dgvProductDetails.DataSource = Ds.Tables[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Failed to Search product", "Search Failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool IsValidToSave()
        {
            if (string.IsNullOrEmpty(this.txtProductId.Text) || string.IsNullOrEmpty(this.txtProductName.Text) || string.IsNullOrEmpty(this.cmbManufacturer.Text) || string.IsNullOrEmpty(this.cmbCategory.Text) || string.IsNullOrEmpty(this.txtBuyingPrice.Text) || string.IsNullOrEmpty(this.txtRetailPrice.Text) || string.IsNullOrEmpty(this.txtVatPercent.Text))
            {
                return false;
            }
            return true;
        }

        private void ClearAllFields()
        {
            this.txtProductId.Clear();
            this.txtProductName.Clear();
            this.cmbManufacturer.SelectedIndex = -1;
            this.cmbSupplier.SelectedIndex = -1;
            this.cmbCategory.SelectedIndex = -1;
            this.txtBuyingPrice.Clear();
            this.txtRetailPrice.Clear();
            this.txtVatPercent.Clear();
            this.nudQuantity.Value = 0;
            this.txtSearchBox.Clear();
        }

        private void btnShowDetails_Click(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }

        private void FormProductDetails_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }

        private void txtSearchBox_TextChanged(object sender, EventArgs e)
        {
            SearchProduct();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSearchBox.Text))
            {
                MessageBox.Show("Please enter a search term", "No Search Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void txtBuyingPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            FormEmployee.NumericValueOnly(e, this.txtBuyingPrice.Text);
        }

        private void txtRetailPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            FormEmployee.NumericValueOnly(e, this.txtRetailPrice.Text);
        }

        private void txtVatPercent_KeyPress(object sender, KeyPressEventArgs e)
        {
            FormEmployee.NumericValueOnly(e, this.txtRetailPrice.Text);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsValidToSave())
                {
                    MessageBox.Show("Fill all information", "Empty Input", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }

                Result = db.ExecuteDMLQuery($"INSERT INTO ProductInformation VALUES({Convert.ToInt32(this.txtProductId.Text)}, '{this.txtProductName.Text}', '{this.cmbManufacturer.Text}', '{this.cmbSupplier.Text}', '{this.cmbCategory.Text}', {this.txtBuyingPrice.Text}, {this.txtRetailPrice.Text}, {this.nudQuantity.Value}, {this.txtVatPercent.Text});");
                if (Result == 1)
                {
                    MessageBox.Show("Product Details Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateDataGridView();
                    ClearAllFields();
                }
                else
                {
                    MessageBox.Show("Failed to Save Product Details!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show("Check your Product ID!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dgvProductDetails.SelectedRows.Count != 1)
                {
                    MessageBox.Show("Pleas select a row to delete", "No row selected", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete {this.dgvProductDetails.SelectedRows[0].Cells[1].Value.ToString()}?", "Delete Product", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                {
                    Result = db.ExecuteDMLQuery($"DELETE FROM ProductInformation WHERE ProductID = {Convert.ToInt32(this.dgvProductDetails.SelectedRows[0].Cells[0].Value)};");
                    if (Result == 1)
                    {
                        MessageBox.Show($"{this.dgvProductDetails.SelectedRows[0].Cells[1].Value.ToString()} Deleted Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PopulateDataGridView();
                    }
                    else
                    {
                        MessageBox.Show($"Failed to delete {this.dgvProductDetails.SelectedRows[0].Cells[1].Value.ToString()}!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void dgvProductDetails_DoubleClick(object sender, EventArgs e)
        {
            if (this.dgvProductDetails.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a row to update!", "Select a Row", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Int32 productId = Convert.ToInt32(this.dgvProductDetails.CurrentRow.Cells[0].Value);
            string productName = this.dgvProductDetails.CurrentRow.Cells[1].Value.ToString();
            string manufacturer = this.dgvProductDetails.CurrentRow.Cells[2].Value.ToString();
            string supplier = this.dgvProductDetails.CurrentRow.Cells[3].Value.ToString();
            string category = this.dgvProductDetails.CurrentRow.Cells[4].Value.ToString();
            Int32 buyingPrice = Convert.ToInt32(this.dgvProductDetails.CurrentRow.Cells[5].Value);
            Int32 retailPrice = Convert.ToInt32(this.dgvProductDetails.CurrentRow.Cells[6].Value);
            Int32 quantity = Convert.ToInt32(this.dgvProductDetails.CurrentRow.Cells[7].Value);
            Int32 vatPercent = Convert.ToInt32(this.dgvProductDetails.CurrentRow.Cells[8].Value);

            this.FUpdateProductInfo = new FormUpdateProductInfo(this, productId, productName, manufacturer, supplier, category, buyingPrice, retailPrice, quantity, vatPercent);
            FormEmployee.SwitchForm(this, this.FUpdateProductInfo);
        }
    }
}
