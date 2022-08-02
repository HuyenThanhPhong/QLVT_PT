using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace QUANLIVATTU_PHANTAN
{
    public partial class Xrpt_InDanhMucVatTu : DevExpress.XtraReports.UI.XtraReport
    {
        public Xrpt_InDanhMucVatTu()
        {
            InitializeComponent();
            this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            this.sqlDataSource1.Fill();
        }

    }
}
