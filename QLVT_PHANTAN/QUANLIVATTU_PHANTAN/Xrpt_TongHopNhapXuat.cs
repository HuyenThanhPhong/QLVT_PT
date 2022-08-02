using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace QUANLIVATTU_PHANTAN
{
    public partial class Xrpt_TongHopNhapXuat : DevExpress.XtraReports.UI.XtraReport
    {
        public Xrpt_TongHopNhapXuat()
        {
            InitializeComponent();
        }

        public Xrpt_TongHopNhapXuat(DateTime begin, DateTime end, bool b)
        {
            InitializeComponent();
            if (b == true)
            {
                this.sqlDataSource1.Connection.ConnectionString = Program.connstr_Publisher;
            }
            else
            {
                this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            }
            this.sqlDataSource1.Queries[0].Parameters[0].Value = begin;
            this.sqlDataSource1.Queries[0].Parameters[1].Value = end;
            this.sqlDataSource1.Fill();
        }

    }
}
