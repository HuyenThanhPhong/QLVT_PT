using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace QUANLIVATTU_PHANTAN
{
    public partial class Xrpt_InDSNhanVien : DevExpress.XtraReports.UI.XtraReport
    {
        public Xrpt_InDSNhanVien()
        {
            InitializeComponent();
            this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            this.sqlDataSource1.Fill();
        }

    }
}
