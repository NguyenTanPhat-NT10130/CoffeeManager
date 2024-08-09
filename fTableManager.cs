using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Menu = QuanLyQuanCafe.DTO.Menu;

namespace QuanLyQuanCafe
{
    
    public partial class fTableManager : Form
    {
        private Account loginAccount;

        public Account LoginAccount { 
            
            get  {return loginAccount; }


            set { loginAccount = value; changeAccount(loginAccount.Type); }

}

        public fTableManager(Account acc)
        {
            InitializeComponent();
            this.LoginAccount = acc; // lấy được tài khoản đăng nhập
            loadTable();
            loadCategory();
            loadComboboxTable(cboSwicthTable);
        }



        #region Method
        void changeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            thôngTinTàiKhoảnToolStripMenuItem.Text += " (" + LoginAccount.DisplayName + ")";
            
        }
        void loadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetCategoryList();
            cboCategory.DataSource = listCategory;
            cboCategory.DisplayMember = "Name";
        }

        void loadFoodBycategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cboFood.DataSource = listFood;
            cboFood.DisplayMember = "Name";
        }
        void loadTable()
        {
            flbTable.Controls.Clear();
            List<Table> tableList = TableDAO.Instance.LoadTableList();
            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height =TableDAO.TableHeight }; 
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click; 
                btn.Tag = item;
                switch(item.Status)
                {
                    case "Trống": 
                        btn.BackColor = Color.Aqua; 
                        break;
                    default: 
                        btn.BackColor = Color.LightPink; 
                        break;
                }    
                flbTable.Controls.Add(btn);
            }
        }

        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            //List<BillInfo> listBillInfo = BillInfoDAO.Instance.GetListBillInfo(BillDAO.Instance.GetUnCheckBillIDByTableID(id));
            List<Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float totalPrice = 0;
            foreach (Menu item in listBillInfo) { 
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;
                lsvBill.Items.Add(lsvItem); 
            }
            CultureInfo culture = new CultureInfo("vi-VN"); // en-US: tiền đô

            //Thread.CurrentThread.CurrentCulture = culture;

            txbToalPrice.Text = totalPrice.ToString("c", culture);

            
        }

        void loadComboboxTable (ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";
        }
        #endregion

        #region Event

        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }
        private void fTableManager_Load(object sender, EventArgs e)
        {

        }
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAcountProfile f = new fAcountProfile(loginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }

        void f_UpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.InsertFood += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.UpdateFood += f_UpdateFood;
            f.ShowDialog();
        }

        private void f_DeleteFood(object sender, EventArgs e)
        {
            loadFoodBycategoryID((cboCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
            loadTable();
        }
        private void f_InsertFood(object sender, EventArgs e)
        {
            loadFoodBycategoryID((cboCategory.SelectedItem as Category).ID);
            if(lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }
        private void f_UpdateFood(object sender, EventArgs e)
        {
            loadFoodBycategoryID((cboCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }



        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;
            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null)
                return;
            Category selected = cb.SelectedItem as Category;
            id = selected.ID;
            loadFoodBycategoryID(id);

        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table; // lấy được Table hiện tại
            if(table == null)
            {
                MessageBox.Show("Hãy chọn bàn");
                return;
            } 
            
            int idBill = BillDAO.Instance.GetUnCheckBillIDByTableID(table.ID);
            int foodID = (cboFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;
            if(idBill == -1 )
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }
            ShowBill(table.ID);
            loadTable();
        }
        private void btnCheckout_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table; // lấy được table hiện tại

            int idBill = BillDAO.Instance.GetUnCheckBillIDByTableID(table.ID);
            int discount = (int)nmDiscount.Value;
            //double totalPrice = Convert.ToDouble(txbToalPrice.Text.Split(',')[0].Replace(".", ""));
            double totalPrice = double.Parse(txbToalPrice.Text, NumberStyles.Currency, new CultureInfo("vi-VN"));
            double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;

            if(idBill != -1 )
            {
                if(MessageBox.Show(string.Format("Bạn có chăc thanh toán hóa đơn cho bàn {0}\n Tổng tiền - (Tổng tiền / 100) x Giảm giá\n=> {1} - ({1} / 100) x {2} = {3} ", table.Name, totalPrice, discount, finalTotalPrice), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                    ShowBill(table.ID);
                    loadTable();
                }    
            }

        }

        private void btnSwitchTable_Click(object sender, EventArgs e)
        {
            int id1 = (lsvBill.Tag as Table).ID;
            int id2 = (cboSwicthTable.SelectedItem as Table).ID;
            if(MessageBox.Show(string.Format("Do you really want to change tables? {0} qua bàn {1}?", (lsvBill.Tag as Table).Name, (cboSwicthTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);
                loadTable();
            }    
            
        }

        #endregion
    }
   
}
