using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace QUANLIVATTU_PHANTAN
{
    public partial class Xrpt_DSDDHChuaCoPhieuNhap : DevExpress.XtraReports.UI.XtraReport
    {
        public Xrpt_DSDDHChuaCoPhieuNhap()
        {
            InitializeComponent();
        }

        public Xrpt_DSDDHChuaCoPhieuNhap(bool i)
        {
            InitializeComponent();
            if (i == true)
            {
                this.sqlDataSource1.Connection.ConnectionString = Program.connstr_Publisher;
            }
            else
            {
                this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            }
            this.sqlDataSource1.Fill();
        }

    }
}
