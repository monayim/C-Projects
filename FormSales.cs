using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuantumBitsComputerShopManagementSystem
{
    public partial class FormSales : Form
    {
        private FormLogin FLogin { get; set; }
        private string EmployeeFullName { get; set; }
        private int EmployeeId { get; set; }
        private DataAccess db = new DataAccess();
        private DataSet Ds { get; set; }
        private int SubTotal { get; set; }
        private int Revenue { get; set; }
        private int InvoiceNo { get; set; }

        public FormSales()
        {
            InitializeComponent();
        }

        public FormSales(FormLogin loginForm, string employeeFullName, int id)
        {
            InitializeComponent();
            this.FLogin = loginForm;
            this.EmployeeFullName = employeeFullName;
            this.EmployeeId = id;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            FormEmployee.LogOutWarning(this, this.FLogin);
        }

        private void FormSales_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormEmployee.SwitchForm(this, this.FLogin);
        }

        private void FormSales_Load(object sender, EventArgs e)
        {
            this.PopulateProductDataGridView();
            this.lblDate.Text = $"Date: {DateTime.Now.ToString("yyyy-MM-dd")}";
            this.lblEmpFullName.Text = this.EmployeeFullName.ToString();
            this.lblEmployeeId.Text = this.EmployeeId.ToString();
        }

        private void PopulateProductDataGridView()
        {
            try
            {
                this.Ds = db.ExecuteQuery("SELECT * FROM ProductInformation;SELECT TOP 1 InvoiceNo FROM Sales ORDER BY InvoiceNo DESC;");
                this.dgvProductDetails.AutoGenerateColumns = false;
                this.dgvProductDetails.DataSource = Ds.Tables[0];
                this.dgvProductDetails.ClearSelection();
                this.txtSearchBox.Clear();
                this.lblInvoiceNo.Text = (Convert.ToInt32(this.Ds.Tables[1].Rows[0][0]) + 1).ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error: " + e.Message, "Failed to show Products", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchProduct()
        {
            try
            {
                Ds = db.ExecuteQuery($"SELECT * FROM ProductInformation WHERE ProductName LIKE '%{this.txtSearchBox.Text}%' OR Manufacturer LIKE '%{this.txtSearchBox.Text}%' OR Supplier LIKE '%{this.txtSearchBox.Text}%' OR Category LIKE '%{this.txtSearchBox.Text}%';");
                this.dgvProductDetails.AutoGenerateColumns = false;
                this.dgvProductDetails.DataSource = Ds.Tables[0];
                this.dgvProductDetails.ClearSelection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error: " + e.Message, "Failed to show Products", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidToAdd()
        {
            if (this.dgvProductDetails.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a row!", "No Product Selected", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return false;
            }

            if (Convert.ToInt32(this.dgvProductDetails.CurrentRow.Cells[7].Value) - this.nudQuantity.Value < 0)
            {
                MessageBox.Show($"You don't have {this.nudQuantity.Value} [{this.dgvProductDetails.CurrentRow.Cells[1].Value.ToString()}] In Stock", "Insufficient Stock", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return false;
            }

            foreach (ListViewItem item in lvCart.Items)
            {
                if ((Convert.ToInt32(item.SubItems[0].Text) == Convert.ToInt32(this.dgvProductDetails.CurrentRow.Cells[0].Value)))
                {
                    MessageBox.Show("Product already in cart!", "Duplicate Product", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            return true;
        }

        private void AddItemToListViw()
        {
            try
            { 
                ListViewItem item = new ListViewItem(this.dgvProductDetails.CurrentRow.Cells["ProductID"].Value.ToString());
                item.SubItems.Add(this.dgvProductDetails.CurrentRow.Cells["ProductName"].Value.ToString());
                item.SubItems.Add(this.dgvProductDetails.CurrentRow.Cells["Manufacturer"].Value.ToString());
                item.SubItems.Add(this.dgvProductDetails.CurrentRow.Cells["Supplier"].Value.ToString());
                item.SubItems.Add(this.dgvProductDetails.CurrentRow.Cells["Category"].Value.ToString());
                item.SubItems.Add(this.dgvProductDetails.CurrentRow.Cells["RetailPrice"].Value.ToString());
                item.SubItems.Add(this.nudQuantity.Value.ToString());
                item.SubItems.Add(this.dgvProductDetails.CurrentRow.Cells["Vat"].Value.ToString());
                
                int retailPrice = Convert.ToInt32(this.dgvProductDetails.CurrentRow.Cells["RetailPrice"].Value);
                int quantity = Convert.ToInt32(this.nudQuantity.Value);
                int vat = Convert.ToInt32(this.dgvProductDetails.CurrentRow.Cells["Vat"].Value);
                if (vat > 0)
                {
                    item.SubItems.Add((retailPrice+ ((retailPrice * quantity * vat) / 100)).ToString()); // VAT
                }
                else
                {
                    item.SubItems.Add((0).ToString()); // VAT
                }

                item.SubItems.Add(this.dgvProductDetails.CurrentRow.Cells["BuyingPrice"].Value.ToString());
                item.SubItems.Add(((Convert.ToInt32(this.dgvProductDetails.CurrentRow.Cells["RetailPrice"].Value) - Convert.ToInt32(this.dgvProductDetails.CurrentRow.Cells["BuyingPrice"].Value)) * Convert.ToInt32(this.nudQuantity.Value)).ToString());
                this.lvCart.Items.Add(item);
                this.nudQuantity.Value = 1;
                PopulateProductDataGridView();
                CalculateCosts();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error: " + e.Message, "Failed to add Product", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateCosts()
        {
            try
            {
                this.SubTotal = 0;
                this.Revenue = 0;
                int rowCount = lvCart.Items.Count;

                foreach (ListViewItem item in lvCart.Items)
                {
                    this.SubTotal += Convert.ToInt32(item.SubItems[8].Text);
                    this.Revenue += Convert.ToInt32(item.SubItems[10].Text);
                }

                this.lblSubTotal.Text = $"{this.SubTotal}";
                this.lblRevenue.Text = $"{this.Revenue}";
                if (string.IsNullOrEmpty(this.txtDiscount.Text))
                {
                    return;
                }
                this.lblTotal.Text = (this.SubTotal - Convert.ToInt32(this.txtDiscount.Text)).ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message, "Failed to calculate costs", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidToConfirm()
        {
            if (this.lvCart.Items.Count == 0)
            {
                MessageBox.Show("Please add items to cart!", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            if (string.IsNullOrEmpty(this.txtDiscount.Text))
            {
                MessageBox.Show("Please enter a discount!", "Missing Discount", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if ((this.rbtnCash.Checked == false) && (this.rbtnCard.Checked == false) && (this.rbtnMfs.Checked == false))
            {
                MessageBox.Show("Select a payment method", "Payment method not selected", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void ClearCart()
        {
            this.lvCart.Items.Clear();
            this.txtDiscount.Clear();
            this.lblSubTotal.Text = "0";
            this.lblRevenue.Text = "0";
            this.lblTotal.Text = "0";
            this.rbtnCash.Checked = false;
            this.rbtnCard.Checked = false;
            this.rbtnMfs.Checked = false;
            this.txtSearchBox.Clear();
        }

        private void txtSearchBox_TextChanged(object sender, EventArgs e)
        {
            SearchProduct();
        }

        private void btnLoadProducts_Click(object sender, EventArgs e)
        {
            PopulateProductDataGridView();
            this.txtSearchBox.Clear();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSearchBox.Text))
            {
                MessageBox.Show("Please enter a product name to search!", "Missing Search item", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SearchProduct();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!IsValidToAdd())
            {
                return;
            }
            AddItemToListViw();
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            if (lvCart.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select an item!", "No Item Selected", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            lvCart.Items.Remove(lvCart.SelectedItems[0]);
            CalculateCosts();
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            FormEmployee.NumericValueOnly(e, this.txtDiscount.Text);
            CalculateCosts();
        }

        private void dgvProductDetails_DoubleClick(object sender, EventArgs e)
        {
            if (!IsValidToAdd())
            {
                return;
            }
            AddItemToListViw();
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            CalculateCosts();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!IsValidToConfirm())
            {
                return;
            }
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to confirm this sale?", "Confirm Sale", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int result1 = 0;
                    int result2= 0;
                    string paymentType = "";
                    if (rbtnCard.Checked)
                    {
                        paymentType = "Card";
                    }
                    else if (rbtnCash.Checked)
                    {
                        paymentType = "Cash";
                    }
                    else if (rbtnMfs.Checked)
                    {
                        paymentType = "MFS";
                    }

                    foreach (ListViewItem item in lvCart.Items)
                    {
                        //MessageBox.Show($"{Convert.ToDouble(this.txtDiscount.Text) / lvCart.Items.Count}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        result1 += this.db.ExecuteDMLQuery($"INSERT INTO Sales(InvoiceNo, ProductID, ProductName, Manufacturer, Supplier, Category, RetailPrice, Quantity, Vat, TotalPrice, BuyingPrice, Revenue, Discount, PaymentType, Date, EmployeeName) VALUES({Convert.ToInt32(this.lblInvoiceNo.Text)}, {item.SubItems[0].Text} , '{item.SubItems[1].Text}', '{item.SubItems[2].Text}', '{item.SubItems[3].Text}', '{item.SubItems[4].Text}', {item.SubItems[5].Text}, {item.SubItems[6].Text}, {item.SubItems[7].Text}, {item.SubItems[8].Text}, {item.SubItems[9].Text}, {item.SubItems[10].Text}, {Convert.ToDouble(this.txtDiscount.Text) / lvCart.Items.Count}, '{paymentType}', '{DateTime.Now.ToString("yyyy-MM-dd")}', '{this.lblEmpFullName.Text}');");
                        result2 += this.db.ExecuteDMLQuery($"UPDATE ProductInformation SET Quantity = Quantity - {item.SubItems[6].Text} WHERE ProductID = {item.SubItems[0].Text};");
                        //if (result1 == 1)
                        //{
                            //int result2 = this.db.ExecuteDMLQuery($"UPDATE ProductInformation SET Quantity = Quantity - {item.SubItems[6].Text} WHERE ProductID = {item.SubItems[0].Text};");
                            //if (result2 == 1)
                            //{
                            //    MessageBox.Show("Sale Confirmed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //    this.lvCart.Items.Clear();
                            //    this.txtDiscount.Clear();
                            //    this.lblSubTotal.Text = "0";
                            //    this.lblRevenue.Text = "0";
                            //    this.lblTotal.Text = "0";
                            //    this.rbtnCash.Checked = false;
                            //    this.rbtnCard.Checked = false;
                            //    this.rbtnMfs.Checked = false;
                            //    this.txtSearchBox.Clear();
                            //    this.PopulateProductDataGridView();
                            //}
                        //}
                    }

                    if (result1 + result2 == (lvCart.Items.Count) * 2)
                    {
                        MessageBox.Show("Sale Confirmed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PopulateProductDataGridView();
                        ClearCart();
                    }
                    else
                    {
                        MessageBox.Show("Failed to confirm sale", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Error: " + exception.Message, "Failed to confirm sale", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearCart();
        }
    }
}
