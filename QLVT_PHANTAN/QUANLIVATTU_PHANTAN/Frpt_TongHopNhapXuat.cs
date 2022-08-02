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
    public partial class Frpt_TongHopNhapXuat : Form
    {
        Xrpt_TongHopNhapXuat rpt;
        public Frpt_TongHopNhapXuat()
        {
            InitializeComponent();
        }

        private void Frpt_TongHopNhapXuat_Load(object sender, EventArgs e)
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
                cmbChiNhanh.Enabled = btnPreviewCTy.Enabled = false;
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

        private void btnPreviewCN_Click(object sender, EventArgs e)
        {
            //check empty
            if(deBegin.Text.Trim() == "")
            {
                MessageBox.Show("Phải chọn ngày bắt đầu!", "", MessageBoxButtons.OK);
                deBegin.Focus();
                return;
            }
            if (deEnd.Text.Trim() == "")
            {
                MessageBox.Show("Phải chọn ngày kết thúc!", "", MessageBoxButtons.OK);
                deEnd.Focus();
                return;
            }

            rpt = new Xrpt_TongHopNhapXuat(deBegin.DateTime, deEnd.DateTime, false);
            rpt.lblTieuDe.Text = "TỔNG HỢP NHẬP XUẤT "+cmbChiNhanh.Text.ToString()+" TỪ: "+deBegin.Text.ToString()+" ĐẾN: "+deEnd.Text.ToString();
            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();

        }

        private void btnPreviewCTy_Click(object sender, EventArgs e)
        {
            //check empty
            if (deBegin.Text.Trim() == "")
            {
                MessageBox.Show("Phải chọn ngày bắt đầu!", "", MessageBoxButtons.OK);
                deBegin.Focus();
                return;
            }
            if (deEnd.Text.Trim() == "")
            {
                MessageBox.Show("Phải chọn ngày kết thúc!", "", MessageBoxButtons.OK);
                deEnd.Focus();
                return;
            }

            rpt = new Xrpt_TongHopNhapXuat(deBegin.DateTime, deEnd.DateTime, true);
            rpt.lblTieuDe.Text = "TỔNG HỢP NHẬP XUẤT CÔNG TY TỪ: " + deBegin.Text.ToString() + " ĐẾN: " + deEnd.Text.ToString();
            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
