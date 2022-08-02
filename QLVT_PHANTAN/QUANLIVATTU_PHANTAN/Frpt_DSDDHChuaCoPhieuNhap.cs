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
    public partial class Frpt_DSDDHChuaCoPhieuNhap : Form
    {
        Xrpt_DSDDHChuaCoPhieuNhap rpt;
        public Frpt_DSDDHChuaCoPhieuNhap()
        {
            InitializeComponent();
        }

        private void Frpt_DSDDHChuaCoPhieuNhap_Load(object sender, EventArgs e)
        {
            cmbChiNhanh.DataSource = Program.bds_dspm;
            cmbChiNhanh.DisplayMember = "TENCN";
            cmbChiNhanh.ValueMember = "TENSERVER";
            cmbChiNhanh.SelectedIndex = Program.mChinhanh;
            if (Program.mGroup == "CONGTY") //bật tắt theo phân quyền
            {
                cmbChiNhanh.Enabled = true;
            }
            else
            {
                cmbChiNhanh.Enabled = cbCCT.Enabled = false;
            }
            
        }

        private void cmbChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbChiNhanh.SelectedValue.ToString() == "System.Data.DataRowView")
                return;
            Program.servername = cmbChiNhanh.SelectedValue.ToString();

            if (cmbChiNhanh.SelectedIndex != Program.mChinhanh)
            {
                Program.mlogin = Program.remotelogin;
                Program.password = Program.remotepassword;
            }
            else
            {
                Program.mlogin = Program.mloginDN;
                Program.password = Program.passwordDN;
            }

            if (Program.KetNoi() == 0)
                MessageBox.Show("Lỗi kết nối về chi nhánh mới", "", MessageBoxButtons.OK);
        }

        private void cbCCT_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCCT.Checked == false)
            {
                cmbChiNhanh.Enabled = true;
            }
            else
            {
                cmbChiNhanh.Enabled = false;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (cbCCT.Checked == true)
            {
                rpt = new Xrpt_DSDDHChuaCoPhieuNhap(true);
                rpt.lblTieuDe.Text = "DANH SÁCH ĐƠN ĐẶT HÀNG CHƯA CÓ PHIẾU NHẬP CẢ CÔNG TY";

            }

            else
            {
                rpt = new Xrpt_DSDDHChuaCoPhieuNhap(false);
                rpt.lblTieuDe.Text = "DANH SÁCH ĐƠN ĐẶT HÀNG CHƯA CÓ PHIẾU NHẬP CỦA " + cmbChiNhanh.Text.ToString();
            }
            
            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
