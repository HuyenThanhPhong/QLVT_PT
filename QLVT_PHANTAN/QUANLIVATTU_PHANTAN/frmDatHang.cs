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
    public partial class frmDatHang : Form
    {
        int vitri = 0;
        int trangthaibtnThem = 0;
        SqlDataReader myReaderkt;
        public static frmCTDDH fCTDDH;

        public static BindingSource bds_CTDDH;

        public frmDatHang()
        {
            InitializeComponent();
        }

        private void datHangBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsDDH.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }
        private String sqlcmd_SP_CHECKID(string code, string type)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CHECKID '" + code + "','" + type + "'" +
                "SELECT 'Return Value' = @ret";
        }

        private void frmDatHang_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.datHangTableAdapter.Connection.ConnectionString = Program.connstr;
            this.datHangTableAdapter.Fill(this.DS.DatHang);

            this.cTDDHTableAdapter.Connection.ConnectionString = Program.connstr;
            this.cTDDHTableAdapter.Fill(this.DS.CTDDH);

            this.phieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
            this.phieuNhapTableAdapter.Fill(this.DS.PhieuNhap);

            this.hoTenNVTableAdapter.Connection.ConnectionString = Program.connstr;
            this.hoTenNVTableAdapter.Fill(this.DS.HoTenNV);

            this.khoTableAdapter.Connection.ConnectionString = Program.connstr;
            this.khoTableAdapter.Fill(this.DS.Kho);

            cmbChiNhanh.DataSource = Program.bds_dspm;
            cmbChiNhanh.DisplayMember = "TENCN";
            cmbChiNhanh.ValueMember = "TENSERVER";
            cmbChiNhanh.SelectedIndex = Program.mChinhanh;

            if (Program.mGroup == "CONGTY") //bật tắt theo phân quyền
            {
                cmbChiNhanh.Enabled = true;
                btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnGhi.Enabled = btnPhucHoi.Enabled = false;
            }
            else
            {
                cmbChiNhanh.Enabled = false;
                btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = true;
            }

            txtMaNV.Text = cmbHoTenNV.SelectedValue.ToString();
            txtMaKho.Text = cmbTenKho.SelectedValue.ToString();
        }

        private void cmbHoTenNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            try 
            {
                if (cmbHoTenNV.SelectedValue.ToString() == "System.Data.DataRowView")
                    return;
                txtMaNV.Text = cmbHoTenNV.SelectedValue.ToString();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void cmbTenKho_SelectedIndexChanged(object sender, EventArgs e)
        {
            try 
            {
                if (cmbTenKho.SelectedValue.ToString() == "System.Data.DataRowView")
                    return;
                txtMaKho.Text = cmbTenKho.SelectedValue.ToString();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsDDH.Position;
            panelControl2.Enabled = true;
            bdsDDH.AddNew();
            deNgay.EditValue = "";
            txtMaNV.Text = cmbHoTenNV.SelectedValue.ToString();
            txtMaKho.Text = cmbTenKho.SelectedValue.ToString();

            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcDDH.Enabled = false;
            trangthaibtnThem = 1;
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcDDH.Enabled = true;
            bdsDDH.CancelEdit();
            if (btnThem.Enabled == false && trangthaibtnThem == 1) bdsDDH.RemoveCurrent();
            if (btnThem.Enabled == false) bdsDDH.Position = vitri;

            txtMaSoDDH.Enabled = true;
            panelControl2.Enabled = false;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;

            this.datHangTableAdapter.Fill(this.DS.DatHang);
        }

        private void btnHieuChinh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsDDH.Position;
            panelControl2.Enabled = true;
            btnThem.Enabled = btnXoa.Enabled = btnHieuChinh.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcDDH.Enabled = true;
            txtMaSoDDH.Enabled = false;
            trangthaibtnThem = 0;
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.datHangTableAdapter.Fill(this.DS.DatHang);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload:" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String masoddh = "";
            if (bdsCTDDH.Count > 0)
            {
                MessageBox.Show("Không thể xóa DDH vì đã có CTDDH", "", MessageBoxButtons.OK);
                return;
            }
            if (bdsPN.Count > 0)
            {
                MessageBox.Show("Không thể xóa DDH vì đã có PN", "", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show("Bạn thật sự muốn xóa phiếu nhập này?", "Xác nhận.", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    masoddh = ((DataRowView)bdsDDH[bdsDDH.Position])["MasoDDH"].ToString(); //
                    bdsDDH.RemoveCurrent();
                    this.datHangTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.datHangTableAdapter.Update(this.DS.DatHang);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa DDH. Bạn hãy xóa lại\n" + ex.Message, "", MessageBoxButtons.OK);
                    this.datHangTableAdapter.Fill(this.DS.DatHang);
                    bdsDDH.Position = bdsPN.Find("MasoDDH", masoddh);
                    return;
                }
            }
            if (bdsDDH.Count == 0) btnXoa.Enabled = false;
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txtMaSoDDH.Text.Trim() == "")
            {
                MessageBox.Show("Mã số DDH không được thiếu!", "", MessageBoxButtons.OK);
                txtMaSoDDH.Focus();
                return;
            }
            if (deNgay.Text.Trim() == "")
            {
                MessageBox.Show("Ngày không được thiếu!", "", MessageBoxButtons.OK);
                deNgay.Focus();
                return;
            }
            if (txtNhaCC.Text.Trim() == "")
            {
                MessageBox.Show("Nhà CC không được thiếu!", "", MessageBoxButtons.OK);
                txtNhaCC.Focus();
                return;
            }

            //check MasoDDH trùng
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECKID(txtMaSoDDH.Text.ToString(), "MasoDDH"));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckMasoDDH = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckMasoDDH == 1 && trangthaibtnThem == 1)
            {
                MessageBox.Show("Mã DDH đã tồn tại!", "", MessageBoxButtons.OK);
                txtMaSoDDH.Focus();
                return;
            }


            try
            {
                bdsDDH.EndEdit();
                bdsDDH.ResetCurrentItem();
                this.datHangTableAdapter.Connection.ConnectionString = Program.connstr;
                this.datHangTableAdapter.Update(this.DS.DatHang);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi DDH.\n" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
            gcDDH.Enabled = true;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;

            panelControl2.Enabled = false;
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                this.datHangTableAdapter.Connection.ConnectionString = Program.connstr;
                this.datHangTableAdapter.Fill(this.DS.DatHang);

                this.cTDDHTableAdapter.Connection.ConnectionString = Program.connstr;
                this.cTDDHTableAdapter.Fill(this.DS.CTDDH);

                this.phieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
                this.phieuNhapTableAdapter.Fill(this.DS.PhieuNhap);

                this.hoTenNVTableAdapter.Connection.ConnectionString = Program.connstr;
                this.hoTenNVTableAdapter.Fill(this.DS.HoTenNV);

                this.khoTableAdapter.Connection.ConnectionString = Program.connstr;
                this.khoTableAdapter.Fill(this.DS.Kho);
            }
        }

        private void CMSbtnThem_Click(object sender, EventArgs e)
        {
            Program.ttCMSbtnThemFrmDDH = 1;
            if (fCTDDH == null || fCTDDH.IsDisposed)
            {
                fCTDDH = new frmCTDDH();
                fCTDDH.Show();
            }
            else
            {
                fCTDDH.Activate();
            }
            bds_CTDDH.AddNew();
        }

        private void CMSbtnSua_Click(object sender, EventArgs e)
        {
            Program.ttCMSbtnThemFrmDDH = 0;
            if (fCTDDH == null || fCTDDH.IsDisposed)
            {
                fCTDDH = new frmCTDDH();
                fCTDDH.Show();
            }
            else
            {
                fCTDDH.Activate();
            }
        }

        private void CMSbtnXoa_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Bạn thật sự muốn xóa CTDDH này?", "Xác nhận.", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    bdsCTDDH.RemoveCurrent();
                    this.cTDDHTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.cTDDHTableAdapter.Update(this.DS.CTDDH);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa CTDDH. Bạn hãy xóa lại\n" + ex.Message, "", MessageBoxButtons.OK);
                    this.cTDDHTableAdapter.Fill(this.DS.CTDDH);
                    return;
                }
            }
            if (bdsCTDDH.Count == 0) CMSbtnXoa.Enabled = false;
        }

        private void CMSbtnReload_Click(object sender, EventArgs e)
        {
            try
            {
                this.cTDDHTableAdapter.Fill(this.DS.CTDDH);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload CTPN:" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
            if (bdsCTDDH.Count > 0) CMSbtnXoa.Enabled = true;
        }

        private void cmsCTDDH_Opening(object sender, CancelEventArgs e)
        {
            if (bdsCTDDH.Count == 0) CMSbtnXoa.Enabled = false;
            else CMSbtnXoa.Enabled = true;
        }
    }
}
