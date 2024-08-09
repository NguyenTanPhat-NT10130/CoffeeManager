using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class BillDAO
    {
        private static BillDAO instance;
        public static BillDAO Instance 
        {
            get { if (instance == null) instance = new BillDAO(); return BillDAO.instance; }
            private set { BillDAO.instance = value; }

        }

        private BillDAO() { }

        // Thành công: BillID
        // thất bại: -1
        public int GetUnCheckBillIDByTableID(int id)
        {
           DataTable data = DataProvider.Instance.ExecuteQuery("Select * from Bill where idTable = " + id + " and status = 0");
           if(data.Rows.Count>0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }
            return -1;
        }

        public void CheckOut(int id, int discount, float totalPrice)
        {
            string query = "Update Bill set status = 1 , DateCheckOut = GETDATE(), " + "discount = " + discount +", totalPrice = " + totalPrice + " where id = " + id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }
        public void InsertBill(int id)
        {
            DataProvider.Instance.ExecuteNonQuery("exec USP_InsertBill @idTable", new object[] { id });
        }

        public DataTable GetListBillByDate (DateTime checkIn, DateTime checkOut)
        {
            return DataProvider.Instance.ExecuteQuery("exec USP_GetListBillByDate @checkIn , @checkOut", new object[] {checkIn, checkOut});
        }
        public int GetMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScala("Select MAX(id) from Bill");
            }
            catch {
                return 1;
            }
        }

    }
}
