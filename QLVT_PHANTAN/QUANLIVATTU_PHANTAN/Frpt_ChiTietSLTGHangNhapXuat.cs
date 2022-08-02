using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QUANLIVATTU_PHANTAN
{
    public partial class Frpt_ChiTietSLTGHangNhapXuat : Form
    {
        public Frpt_ChiTietSLTGHangNhapXuat()
        {
            InitializeComponent();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            //CHECK EMPTY
            if (cmbLoai.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng chọn loại!", "", MessageBoxButtons.OK);
                cmbLoai.Focus();
                return;
            }
            if (deBegin.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu!", "", MessageBoxButtons.OK);
                deBegin.Focus();
                return;
            }
            if (deEnd.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng chọn ngày kết thúc!", "", MessageBoxButtons.OK);
                deEnd.Focus();
                return;
            }

            if(Program.mGroup == "CONGTY")
            {
                Xrpt_ChiTietSLTGHangNhapXuat rpt = new Xrpt_ChiTietSLTGHangNhapXuat("F", cmbLoai.Text.ToString().Substring(0,1), deBegin.Text.ToString(), deEnd.Text.ToString());
                rpt.lblCTSLTGHangNX.Text = "BẢNG KÊ CHI TIẾT SỐ LƯỢNG - TRỊ GIÁ HÀNG " + cmbLoai.Text.ToString() + " CỦA CÔNG TY";
                ReportPrintTool print = new ReportPrintTool(rpt);
                print.ShowPreviewDialog();
            }
            else
            {
                string tencn = "";
                if (Program.servername == "LETANDAT\\SERVER1") tencn = "1";
                else tencn = "2";
                Xrpt_ChiTietSLTGHangNhapXuat rpt = new Xrpt_ChiTietSLTGHangNhapXuat("C", cmbLoai.Text.ToString().Substring(0,1), deBegin.Text.ToString(), deEnd.Text.ToString());
                rpt.lblCTSLTGHangNX.Text = "BẢNG KÊ CHI TIẾT SỐ LƯỢNG -TRỊ GIÁ HÀNG " + cmbLoai.Text.ToString() + " CỦA CHI NHÁNH "+tencn;
                ReportPrintTool print = new ReportPrintTool(rpt);
                print.ShowPreviewDialog();
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
