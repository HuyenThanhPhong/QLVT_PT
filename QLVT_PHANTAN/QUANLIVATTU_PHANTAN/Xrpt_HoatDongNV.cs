using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace QUANLIVATTU_PHANTAN
{
    public partial class Xrpt_HoatDongNV : DevExpress.XtraReports.UI.XtraReport
    {
        public Xrpt_HoatDongNV()
        {
            
        }

        public Xrpt_HoatDongNV(int manv, string loai, string begin, string end)
        {
            InitializeComponent();
            this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            this.sqlDataSource1.Queries[0].Parameters[0].Value = manv;
            this.sqlDataSource1.Queries[0].Parameters[1].Value = loai;
            this.sqlDataSource1.Queries[0].Parameters[2].Value = begin;
            this.sqlDataSource1.Queries[0].Parameters[3].Value = end;
            this.sqlDataSource1.Fill();
        }
    }
}
