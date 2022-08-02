using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace QUANLIVATTU_PHANTAN
{
    public partial class Xrpt_ChiTietSLTGHangNhapXuat : DevExpress.XtraReports.UI.XtraReport
    {
        public Xrpt_ChiTietSLTGHangNhapXuat()
        {
        }

        public Xrpt_ChiTietSLTGHangNhapXuat(string mode, string loai, string begin, string end)
        {
            InitializeComponent();
            this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            this.sqlDataSource1.Queries[0].Parameters[0].Value = mode;
            this.sqlDataSource1.Queries[0].Parameters[1].Value = loai;
            this.sqlDataSource1.Queries[0].Parameters[2].Value = begin;
            this.sqlDataSource1.Queries[0].Parameters[3].Value = end;
        }
    }
}
