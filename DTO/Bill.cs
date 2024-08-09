using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class Bill
    {
        public Bill(int id, DateTime? datecheckIn, DateTime? dateCheckOut, int status, int discount = 0) {
            this.iD = id;
            this.dateCheckIn = datecheckIn;
            this.datecheckOut = dateCheckOut;
            this.status = status;
            this.Discount = discount;
        }   

        public Bill(DataRow row) {
            this.iD = (int)row["id"];
            this.dateCheckIn = (DateTime?)row["datecheckIn"];
            var dateCheckOutTemp = row["dateCheckOut"];
            if(dateCheckOutTemp.ToString() != "")
                this.datecheckOut = (DateTime?)dateCheckOutTemp;
            this.status = (int)row["status"];
            if(row["status"].ToString() != "")
                this.Discount = (int)row["discount"];
        }
        private int discount;
        private int status;
        private DateTime? datecheckOut;
        private DateTime? dateCheckIn;
        private int iD;

        public int ID { get => iD; set => iD = value; }
        public DateTime? DateCheckIn { get => dateCheckIn; set => dateCheckIn = value; }
        public DateTime? DatecheckOut { get => datecheckOut; set => datecheckOut = value; }
        public int Status { get => status; set => status = value; }
        public int Discount { get => discount; set => discount = value; }
    }
}
