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
    public partial class FormSalesHistory : Form
    {
        private FormLogin FLogin { get; set; }
        private FormEmployee FEmployee { get; set; }
        private FormProductDetails FProductDetails { get; set; }
        private FormSupplier FSupplier { get; set; }
        private DataAccess db = new DataAccess();
        private DataSet Ds { set; get; }

        public FormSalesHistory()
        {
            InitializeComponent();
        }
        public FormSalesHistory(FormLogin fLogin, FormEmployee fEmployee, FormProductDetails fProductDetails, FormSupplier fSupplier)
        {
            InitializeComponent();
            this.FLogin = fLogin;
            this.FEmployee = fEmployee;
            this.FProductDetails = fProductDetails;
            this.FSupplier = fSupplier;
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            this.FEmployee = new FormEmployee(this.FLogin, this.FProductDetails, this, this.FSupplier);
            FormEmployee.SwitchForm(this, this.FEmployee);
        }

        private void btnProductDetails_Click(object sender, EventArgs e)
        {
            this.FProductDetails = new FormProductDetails(this.FLogin, this.FEmployee, this, this.FSupplier);
            FormEmployee.SwitchForm(this, this.FProductDetails);
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            this.FSupplier = new FormSupplier(this.FLogin, this.FEmployee, this.FProductDetails, this);
            FormEmployee.SwitchForm(this, this.FSupplier);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            FormEmployee.LogOutWarning(this, this.FLogin);
        }

        private void FormSalesHistory_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormEmployee.SwitchForm(this, this.FLogin);
        }

        // Above code contains Form Switching logic , which is not a good practice.
        // Below code does other operations like loading data from database and displaying it in DataGridView.

        private void LoadSalesDataGridView(string query)
        {
            try
            {
                this.Ds = db.ExecuteQuery(query);
                this.dgvSalesHistory.AutoGenerateColumns = false;
                this.dgvSalesHistory.DataSource = this.Ds.Tables[0];
                this.dgvSalesHistory.ClearSelection();

                this.lblTotalSale.Text = $"${this.Ds.Tables[1].Rows[0][0].ToString()}";
                this.lblTotalVat.Text = $"${this.Ds.Tables[1].Rows[0][1].ToString()}";
                this.lblTotalDiscount.Text = $"${this.Ds.Tables[1].Rows[0][2].ToString()}";
                this.lblTotdalRevenue.Text = $"${this.Ds.Tables[1].Rows[0][3].ToString()}";
            }
            catch (Exception e)
            {
                MessageBox.Show($"ERROR: {e.Message}", "Failed To Load Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormSalesHistory_Load(object sender, EventArgs e)
        {
            LoadSalesDataGridView("SELECT * FROM Sales; SELECT SUM(TotalPrice), SUM((RetailPrice*Quantity*Vat)/100), SUM(Discount), SUM(Revenue) FROM Sales;");
        }

        private void txtSearchBox_TextChanged(object sender, EventArgs e)
        {
            LoadSalesDataGridView($"SELECT * FROM Sales WHERE InvoiceNo LIKE '%{this.txtSearchBox.Text}%' OR ProductID LIKE '%{this.txtSearchBox.Text}%' OR ProductName LIKE '%{this.txtSearchBox.Text}%' OR Manufacturer LIKE '%{this.txtSearchBox.Text}%' OR Supplier LIKE '%{this.txtSearchBox.Text}%' OR Category LIKE '%{this.txtSearchBox.Text}%' OR Date LIKE '%{this.txtSearchBox.Text}%' OR EmployeeName LIKE '%{this.txtSearchBox.Text}%';SELECT SUM(TotalPrice), SUM((RetailPrice*Quantity*Vat)/100), SUM(Discount), SUM(Revenue) FROM Sales WHERE InvoiceNo LIKE '%{this.txtSearchBox.Text}%' OR ProductID LIKE '%{this.txtSearchBox.Text}%' OR ProductName LIKE '%{this.txtSearchBox.Text}%' OR Manufacturer LIKE '%{this.txtSearchBox.Text}%' OR Supplier LIKE '%{this.txtSearchBox.Text}%' OR Category LIKE '%{this.txtSearchBox.Text}%' OR Date LIKE '%{this.txtSearchBox.Text}%' OR EmployeeName LIKE '%{this.txtSearchBox.Text}%';");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSearchBox.Text))
            {
                MessageBox.Show("Please enter value to search", "Nothing To Search", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            LoadSalesDataGridView($"SELECT * FROM Sales WHERE InvoiceNo LIKE '%{this.txtSearchBox.Text}%' OR ProductID LIKE '%{this.txtSearchBox.Text}%' OR ProductName LIKE '%{this.txtSearchBox.Text}%' OR Manufacturer LIKE '%{this.txtSearchBox.Text}%' OR Supplier LIKE '%{this.txtSearchBox.Text}%' OR Category LIKE '%{this.txtSearchBox.Text}%' OR Date LIKE '%{this.txtSearchBox.Text}%';SELECT SUM(TotalPrice), SUM((RetailPrice*Quantity*Vat)/100), SUM(Discount), SUM(Revenue) FROM Sales WHERE InvoiceNo LIKE '%{this.txtSearchBox.Text}%' OR ProductID LIKE '%{this.txtSearchBox.Text}%' OR ProductName LIKE '%{this.txtSearchBox.Text}%' OR Manufacturer LIKE '%{this.txtSearchBox.Text}%' OR Supplier LIKE '%{this.txtSearchBox.Text}%' OR Category LIKE '%{this.txtSearchBox.Text}%' OR Date LIKE '%{this.txtSearchBox.Text}%';");
        }

        private void btnShowDetails_Click(object sender, EventArgs e)
        {
            LoadSalesDataGridView("SELECT * FROM Sales; SELECT SUM(TotalPrice), SUM((RetailPrice*Quantity*Vat)/100), SUM(Discount), SUM(Revenue) FROM Sales;");
            this.txtSearchBox.Clear();
        }
        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            this.dtpTo.MaxDate = DateTime.Now;
            LoadSalesDataGridView($"SELECT * FROM Sales WHERE Date BETWEEN '{this.dtpFrom.Text}' AND '{this.dtpTo.Text}';SELECT SUM(TotalPrice), SUM((RetailPrice*Quantity*Vat)/100), SUM(Discount), SUM(Revenue) FROM Sales WHERE Date BETWEEN '{this.dtpFrom.Text}' AND '{this.dtpTo.Text}';");
        }

        private void dtpFrom_ValueChanged_2(object sender, EventArgs e)
        {
            this.dtpFrom.MaxDate = DateTime.Now;
            LoadSalesDataGridView($"SELECT * FROM Sales WHERE Date BETWEEN '{this.dtpFrom.Text}' AND '{this.dtpTo.Text}';SELECT SUM(TotalPrice), SUM((RetailPrice*Quantity*Vat)/100), SUM(Discount), SUM(Revenue) FROM Sales WHERE Date BETWEEN '{this.dtpFrom.Text}' AND '{this.dtpTo.Text}';");
        }
    }
}
