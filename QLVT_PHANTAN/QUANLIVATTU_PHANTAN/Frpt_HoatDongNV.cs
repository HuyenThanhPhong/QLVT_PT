using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QUANLIVATTU_PHANTAN
{
    public partial class Frpt_HoatDongNV : Form
    {
        //SqlDataReader sdr;
        public Frpt_HoatDongNV()
        {
            InitializeComponent();
        }

        private void hoTenNVBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsHoTenNV.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void Frpt_HoatDongNV_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.hoTenNVTableAdapter.Connection.ConnectionString = Program.connstr;
            this.hoTenNVTableAdapter.Fill(this.DS.HoTenNV);

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
                cmbChiNhanh.Enabled = false;
            }

            txtMaNV.Text = cmbHoTenNV.SelectedValue.ToString();

        }

        private void cmbHoTenNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtMaNV.Text = cmbHoTenNV.SelectedValue.ToString();
            }
            catch (Exception)
            {
                return;
            }
            
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            //check empty
            if (cmbLoai.Text.Trim() == "")
            {
                MessageBox.Show("Phải chọn loại !", "", MessageBoxButtons.OK);
                cmbLoai.Focus();
            }
            if (deBegin.Text.Trim() == "")
            {
                MessageBox.Show("Phải chọn ngày bắt đầu !", "", MessageBoxButtons.OK);
                deBegin.Focus();
            }
            if (deEnd.Text.Trim() == "")
            {
                MessageBox.Show("Phải chọn ngày kết thúc !", "", MessageBoxButtons.OK);
                deEnd.Focus();
            }
            //sdr = Program.ExecSqlDataReader("SELECT MANV, (HO+' '+TEN) HOTEN, DIACHI, NGAYSINH, LUONG, MACN, TrangThaiXoa FROM LINK2.QLVT.dbo.NhanVien WHERE MANV = "+int.Parse(cmbHoTenNV.Text.ToString()));
            //if (sdr == null) return;
            //sdr.Read();
            DataTable dt = Program.ExecSqlDataTable("SELECT MANV, (HO+' '+TEN) HOTEN, DIACHI, FORMAT(NGAYSINH, 'dd/MM/yyyy') NGAYSINH, LUONG, MACN, TrangThaiXoa FROM LINK2.QLVT.dbo.NhanVien WHERE MANV = " + int.Parse(txtMaNV.Text.ToString()));

            Xrpt_HoatDongNV rpt = new Xrpt_HoatDongNV(int.Parse(txtMaNV.Text.ToString()), cmbLoai.Text.ToString().Substring(0, 1), deBegin.Text.ToString(), deEnd.Text.ToString());
            rpt.lblTieuDe.Text = "HOẠT ĐỘNG "+cmbLoai.Text.ToString()+" CỦA MỘT NHÂN VIÊN";

            rpt.lblMaNV.Text = dt.Rows[0]["MANV"].ToString();
            rpt.lblHoTen.Text = dt.Rows[0]["HOTEN"].ToString();
            rpt.lblCN.Text = dt.Rows[0]["MACN"].ToString();
            rpt.lblNgaySinh.Text = dt.Rows[0]["NGAYSINH"].ToString();
            rpt.lblLuong.Text = dt.Rows[0]["LUONG"].ToString();
            rpt.lblDiaChi.Text = dt.Rows[0]["DIACHI"].ToString();
            rpt.lblGhiChu.Text = "";

            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
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
            else
            {
                this.hoTenNVTableAdapter.Connection.ConnectionString = Program.connstr;
                this.hoTenNVTableAdapter.Fill(this.DS.HoTenNV);
            }
        }
    }
}
