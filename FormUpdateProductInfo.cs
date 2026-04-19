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
    public partial class FormUpdateProductInfo : Form
    {
        private FormProductDetails FProductDetails { get; set; }
        private int PId { get; set; }
        private string PName { get; set; }
        private string Manufacturer { get; set; }
        private string Supplier { get; set; }
        private string Category { get; set; }
        private int BuyingPrice { get; set; }
        private int RetailPrice { get; set; }
        private int Quantity { get; set; }
        private int Vat { get; set; }
        private DataAccess db = new DataAccess();
        private int Result { get; set; }

        public FormUpdateProductInfo()
        {
            InitializeComponent();
        }

        public FormUpdateProductInfo(FormProductDetails formUpdateProductInfo, int pId, string pName, string manufacturer, string supplier, string category, int buyingPrice, int rPrice, int quantity, int vat)
        {
            InitializeComponent();
            this.FProductDetails = formUpdateProductInfo;
            this.PId = pId;
            this.PName = pName;
            this.Manufacturer = manufacturer;
            this.Supplier = supplier;
            this.Category = category;
            this.BuyingPrice = buyingPrice;
            this.RetailPrice = rPrice;
            this.Quantity = quantity;
            this.Vat = vat;
        }

        private void SetAllField()
        {
            this.txtProductId.Text = this.PId.ToString();
            this.txtProductName.Text = this.PName;
            this.cmbManufacturer.Text = this.Manufacturer;
            this.cmbSupplier.Text = this.Supplier;
            this.cmbCategory.Text = this.Category;
            this.txtBuyingPrice.Text = this.BuyingPrice.ToString();
            this.txtRetailPrice.Text = this.RetailPrice.ToString();
            this.nudQuantity.Text = this.Quantity.ToString();
            this.txtVatPercent.Text = this.Vat.ToString();
        }

        private bool IsValidToUpdate()
        {
            if (string.IsNullOrEmpty(this.txtProductName.Text) || string.IsNullOrEmpty(this.cmbManufacturer.Text) || string.IsNullOrEmpty(this.cmbCategory.Text) || string.IsNullOrEmpty(this.txtBuyingPrice.Text) || string.IsNullOrEmpty(this.txtRetailPrice.Text) || string.IsNullOrEmpty(this.txtVatPercent.Text))
            {
                return false;
            }
            return true;
        }

        private void FormUpdateProductInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormEmployee.SwitchForm(this, this.FProductDetails);
        }

        private void FormUpdateProductInfo_Load(object sender, EventArgs e)
        {
            SetAllField();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            SetAllField();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsValidToUpdate())
                {
                    MessageBox.Show("Please fill all the fields properly", "Empty Inputs", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }

                Result = db.ExecuteDMLQuery(
                    $"UPDATE ProductInformation SET ProductName='{this.txtProductName.Text}', Manufacturer='{this.cmbManufacturer.Text}', Supplier='{this.cmbSupplier.Text}', Category='{this.cmbCategory.Text}', BuyingPrice= {this.txtBuyingPrice.Text}, RetailPrice= {this.txtRetailPrice.Text},Quantity = {this.nudQuantity.Value}, Vat = {this.txtVatPercent.Text} WHERE ProductID = {this.txtProductId.Text};");
                if (Result == 1)
                {
                    MessageBox.Show("Product Information updated successfully!", "Successfull", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    this.FProductDetails.PopulateDataGridView();
                }
                else
                {
                    MessageBox.Show("Failed to update Product Information!", "Information", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show("Pleases insert valid information!", "Information", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
