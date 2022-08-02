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
    public partial class frmPhieuNhap : Form
    {
        public static BindingSource bds_CTPN;
        int vitri = 0;
        int trangthaibtnThem = 0;
        SqlDataReader myReaderkt;
        public static frmCTPN fCTPN;
            
        public frmPhieuNhap()
        {
            InitializeComponent();
        }

        private void phieuNhapBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsPN.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private String sqlcmd_SP_CHECKID(string code, string type)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CHECKID '" + code + "','" + type + "'" +
                "SELECT 'Return Value' = @ret";
        }
        private String sqlcmd_SP_CHECKID_CT(string code, string code1, string type)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CHECKID_CT '" + code + "','" +code1+ "','" + type + "'" +
                "SELECT 'Return Value' = @ret";
        }

        private void frmPhieuNhap_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.PhieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
            this.PhieuNhapTableAdapter.Fill(this.DS.PhieuNhap);

            this.CTPNTableAdapter.Connection.ConnectionString = Program.connstr;
            this.CTPNTableAdapter.Fill(this.DS.CTPN);

            this.HoTenNVTableAdapter.Connection.ConnectionString = Program.connstr;
            this.HoTenNVTableAdapter.Fill(this.DS.HoTenNV);

            this.KhoTableAdapter.Connection.ConnectionString = Program.connstr;
            this.KhoTableAdapter.Fill(this.DS.Kho);

            //macn = ((DataRowView)bdsPN[0])["MACN"].ToString(); //Giữ lại macn nhân viên đầu tiên đã đăng nhập từ phía ngoài nhưng tiềm ẩn lỗi, chờ sữa?
            cmbChiNhanh.DataSource = Program.bds_dspm;
            cmbChiNhanh.DisplayMember = "TENCN";
            cmbChiNhanh.ValueMember = "TENSERVER";
            cmbChiNhanh.SelectedIndex = Program.mChinhanh;

            cmbMaSoDDH.DataSource = Program.ExecSqlDataTable("SELECT MaSoDDH FROM DatHang");
            cmbMaSoDDH.DisplayMember = "MaSoDDH";
            cmbMaSoDDH.ValueMember = "MaSoDDH";
            txtMaNV.Text = cmbHoTenNV.SelectedValue.ToString();
            txtMaKho.Text = cmbTenKho.SelectedValue.ToString();
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
        }

        private void cmbHoTenNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            try // bỏ trycatch xuất hiện lỗi chưa xử lí
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
            try // bỏ trycatch xuất hiện lỗi chưa xử lí
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
            vitri = bdsPN.Position;
            panelControl3.Enabled = true;
            bdsPN.AddNew();
            deNgay.EditValue = "";
            txtMaNV.Text = cmbHoTenNV.SelectedValue.ToString();
            txtMaKho.Text = cmbTenKho.SelectedValue.ToString();

            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcPN.Enabled = false;
            trangthaibtnThem = 1;
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcPN.Enabled = true;
            bdsPN.CancelEdit();
            if (btnThem.Enabled == false && trangthaibtnThem == 1) bdsPN.RemoveCurrent();
            if (btnThem.Enabled == false) bdsPN.Position = vitri;

            txtMaPN.Enabled = true;
            panelControl3.Enabled = false;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;

            this.PhieuNhapTableAdapter.Fill(this.DS.PhieuNhap); // thêm dòng này vì bấm thêm, hiệu chỉnh, phục hồi nhiều lần xuất hiện lỗi
        }

        private void btnHieuChinh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsPN.Position;
            panelControl3.Enabled = true;
            btnThem.Enabled = btnXoa.Enabled = btnHieuChinh.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcPN.Enabled = true;
            txtMaPN.Enabled = false;
            trangthaibtnThem = 0;
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //DS.EnforceConstraints = false;
                //this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                this.PhieuNhapTableAdapter.Fill(this.DS.PhieuNhap);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload:" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String mapn = "";
            if (bdsCTPN.Count > 0)
            {
                MessageBox.Show("Không thể xóa phiếu nhập vì đã có CTPN", "", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show("Bạn thật sự muốn xóa phiếu nhập này?", "Xác nhận.", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    mapn = ((DataRowView)bdsPN[bdsPN.Position])["MAPN"].ToString(); //
                    bdsPN.RemoveCurrent();
                    this.PhieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.PhieuNhapTableAdapter.Update(this.DS.PhieuNhap);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa phiếu nhập. Bạn hãy xóa lại\n" + ex.Message, "", MessageBoxButtons.OK);
                    this.PhieuNhapTableAdapter.Fill(this.DS.PhieuNhap);
                    bdsPN.Position = bdsPN.Find("MAPN", mapn);
                    return;
                }
            }
            if (bdsPN.Count == 0) btnXoa.Enabled = false;
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txtMaPN.Text.Trim() == "")
            {
                MessageBox.Show("Mã PN không được thiếu!", "", MessageBoxButtons.OK);
                txtMaPN.Focus();
                return;
            }
            if (deNgay.Text.Trim() == "")
            {
                MessageBox.Show("Ngày không được thiếu!", "", MessageBoxButtons.OK);
                deNgay.Focus();
                return;
            }
            if (cmbMaSoDDH.Text.Trim() == "")
            {
                MessageBox.Show("MaSoDDH không được thiếu!", "", MessageBoxButtons.OK);
                cmbMaSoDDH.Focus();
                return;
            }

            //check MaPN trùng
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECKID(txtMaPN.Text.ToString(), "MAPN"));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckMaPN = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckMaPN == 1 && trangthaibtnThem == 1)
            {
                MessageBox.Show("Mã PN đã tồn tại!", "", MessageBoxButtons.OK);
                txtMaPN.Focus();
                return;
            }

            //check MaSoDDH trùng
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECKID_CT("", cmbMaSoDDH.Text.ToString(), "PN"));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckMasoDDH = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckMasoDDH == 1 && trangthaibtnThem == 1)
            {
                MessageBox.Show("Mã số DDH này đã lập PN!", "", MessageBoxButtons.OK);
                cmbMaSoDDH.Focus();
                return;
            }

            try
            {
                bdsPN.EndEdit();
                bdsPN.ResetCurrentItem();
                this.PhieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
                this.PhieuNhapTableAdapter.Update(this.DS.PhieuNhap);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi PN.\n" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
            gcPN.Enabled = true;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;

            panelControl3.Enabled = false;
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
                this.PhieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
                this.PhieuNhapTableAdapter.Fill(this.DS.PhieuNhap);

                this.CTPNTableAdapter.Connection.ConnectionString = Program.connstr;
                this.CTPNTableAdapter.Fill(this.DS.CTPN);

                this.HoTenNVTableAdapter.Connection.ConnectionString = Program.connstr;
                this.HoTenNVTableAdapter.Fill(this.DS.HoTenNV);

                this.KhoTableAdapter.Connection.ConnectionString = Program.connstr;
                this.KhoTableAdapter.Fill(this.DS.Kho);
            }
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        private void CMSbtnThem_Click(object sender, EventArgs e)
        {
            Program.ttCMSbtnThemFrmPN = 1;

            if (fCTPN == null || fCTPN.IsDisposed)
            {
                fCTPN = new frmCTPN();
                fCTPN.Show();
            }
            else
            {
                fCTPN.Activate();
            }
            bds_CTPN.AddNew();
            
        }

        private void CMSbtnXoa_Click(object sender, EventArgs e)
        {
            //String mapn = "";

            if (MessageBox.Show("Bạn thật sự muốn xóa CTPN này?", "Xác nhận.", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    //mapn = ((DataRowView)bdsCTPN[bdsCTPN.Position])["MAPN"].ToString(); //
                    bdsCTPN.RemoveCurrent();
                    this.CTPNTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.CTPNTableAdapter.Update(this.DS.CTPN);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa CTPN. Bạn hãy xóa lại\n" + ex.Message, "", MessageBoxButtons.OK);
                    this.CTPNTableAdapter.Fill(this.DS.CTPN);
                    //bdsPN.Position = bdsPN.Find("MAPN", mapn);
                    return;
                }
            }
            if (bdsCTPN.Count == 0) CMSbtnXoa.Enabled = false;
        }

        private void CMSbtnReload_Click(object sender, EventArgs e)
        {
            try
            {
                this.CTPNTableAdapter.Fill(this.DS.CTPN);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload CTPN:" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }

        private void CMSbtnSua_Click(object sender, EventArgs e)
        {
            Program.ttCMSbtnThemFrmPN = 0;

            if (fCTPN == null || fCTPN.IsDisposed)
            {
                fCTPN = new frmCTPN();
                fCTPN.Show();
            }
            else
            {
                fCTPN.Activate();
            }
            
        }

        private void cmsPN_Opening(object sender, CancelEventArgs e)
        {
            if (bdsCTPN.Count == 0) CMSbtnXoa.Enabled = false;
            else CMSbtnXoa.Enabled = true;
        }
    }
}
