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
    public partial class frmPhieuXuat : Form
    {
        int vitri = 0;
        int trangthaibtnThem = 0;
        SqlDataReader myReaderkt;
        public static frmCTPX fCTPX;
        public static BindingSource bds_CTPX;

        private String sqlcmd_SP_CHECKID(string code, string type)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CHECKID '" + code + "','" + type + "'" +
                "SELECT 'Return Value' = @ret";
        }

        public frmPhieuXuat()
        {
            InitializeComponent();
        }

        private void phieuXuatBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsPX.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void frmPhieuXuat_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
            this.phieuXuatTableAdapter.Fill(this.DS.PhieuXuat);

            this.cTPXTableAdapter.Connection.ConnectionString = Program.connstr;
            this.cTPXTableAdapter.Fill(this.DS.CTPX);

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


        }

        private void cmbHotenNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            try 
            {
                if (cmbHotenNV.SelectedValue.ToString() == "System.Data.DataRowView")
                    return;
                txtMaNV.Text = cmbHotenNV.SelectedValue.ToString();
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
            vitri = bdsPX.Position;
            panelControl3.Enabled = true;
            bdsPX.AddNew();
            deNgay.EditValue = "";
            txtMaNV.Text = cmbHotenNV.SelectedValue.ToString();
            txtMaKho.Text = cmbTenKho.SelectedValue.ToString();

            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcPX.Enabled = false;
            trangthaibtnThem = 1;
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcPX.Enabled = true;
            bdsPX.CancelEdit();
            if (btnThem.Enabled == false && trangthaibtnThem == 1) bdsPX.RemoveCurrent();
            if (btnThem.Enabled == false) bdsPX.Position = vitri;

            txtMaPX.Enabled = true;
            panelControl3.Enabled = false;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;

            this.phieuXuatTableAdapter.Fill(this.DS.PhieuXuat);
        }

        private void btnHieuChinh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsPX.Position;
            panelControl3.Enabled = true;
            btnThem.Enabled = btnXoa.Enabled = btnHieuChinh.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcPX.Enabled = true;
            txtMaPX.Enabled = false;
            trangthaibtnThem = 0;
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                
                this.phieuXuatTableAdapter.Fill(this.DS.PhieuXuat);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload:" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String mapx = "";
            if (bdsCTPX.Count > 0)
            {
                MessageBox.Show("Không thể xóa phiếu xuất vì đã có CTPX", "", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show("Bạn thật sự muốn xóa phiếu xuất này?", "Xác nhận.", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    mapx = ((DataRowView)bdsPX[bdsPX.Position])["MAPX"].ToString(); //
                    bdsPX.RemoveCurrent();
                    this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.phieuXuatTableAdapter.Update(this.DS.PhieuXuat);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa phiếu nhập. Bạn hãy xóa lại\n" + ex.Message, "", MessageBoxButtons.OK);
                    this.phieuXuatTableAdapter.Fill(this.DS.PhieuXuat);
                    bdsPX.Position = bdsPX.Find("MAPN", mapx);
                    return;
                }
            }
            if (bdsPX.Count == 0) btnXoa.Enabled = false;
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txtMaPX.Text.Trim() == "")
            {
                MessageBox.Show("Mã PX không được thiếu!", "", MessageBoxButtons.OK);
                txtMaPX.Focus();
                return;
            }
            if (deNgay.Text.Trim() == "")
            {
                MessageBox.Show("Ngày không được thiếu!", "", MessageBoxButtons.OK);
                deNgay.Focus();
                return;
            }
            if (txtHoTenKH.Text.Trim() == "")
            {
                MessageBox.Show("Họ tên KH không được thiếu!", "", MessageBoxButtons.OK);
                txtHoTenKH.Focus();
                return;
            }

            //check MaPN trùng
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECKID(txtMaPX.Text.ToString(), "MAPX"));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckMaPX = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckMaPX == 1 && trangthaibtnThem == 1)
            {
                MessageBox.Show("Mã PX đã tồn tại!", "", MessageBoxButtons.OK);
                txtMaPX.Focus();
                return;
            }


            try
            {
                bdsPX.EndEdit();
                bdsPX.ResetCurrentItem();
                this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                this.phieuXuatTableAdapter.Update(this.DS.PhieuXuat);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi PX.\n" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
            gcPX.Enabled = true;
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
                this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                this.phieuXuatTableAdapter.Fill(this.DS.PhieuXuat);

                this.cTPXTableAdapter.Connection.ConnectionString = Program.connstr;
                this.cTPXTableAdapter.Fill(this.DS.CTPX);

                this.hoTenNVTableAdapter.Connection.ConnectionString = Program.connstr;
                this.hoTenNVTableAdapter.Fill(this.DS.HoTenNV);

                this.khoTableAdapter.Connection.ConnectionString = Program.connstr;
                this.khoTableAdapter.Fill(this.DS.Kho);
            }
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        private void CMSbtnThem_Click(object sender, EventArgs e)
        {
            Program.ttCMSbtnThemFrmPX = 1;

            if (fCTPX == null || fCTPX.IsDisposed)
            {
                fCTPX = new frmCTPX();
                fCTPX.Show();
            }
            else
            {
                fCTPX.Activate();
            }
            bds_CTPX.AddNew();
        }

        private void CMSbtnSua_Click(object sender, EventArgs e)
        {
            Program.ttCMSbtnThemFrmPX = 0;
            if (fCTPX == null || fCTPX.IsDisposed)
            {
                fCTPX = new frmCTPX();
                fCTPX.Show();
            }
            else
            {
                fCTPX.Activate();
            }
        }

        private void CMSbtnXoa_Click(object sender, EventArgs e)
        {
            //String mapn = "";

            if (MessageBox.Show("Bạn thật sự muốn xóa CTPX này?", "Xác nhận.", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    //mapn = ((DataRowView)bdsCTPN[bdsCTPN.Position])["MAPN"].ToString(); //
                    bdsCTPX.RemoveCurrent();
                    this.cTPXTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.cTPXTableAdapter.Update(this.DS.CTPX);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa CTPX. Bạn hãy xóa lại\n" + ex.Message, "", MessageBoxButtons.OK);
                    this.cTPXTableAdapter.Fill(this.DS.CTPX);
                    //bdsPN.Position = bdsPN.Find("MAPN", mapn);
                    return;
                }
            }
            if (bdsCTPX.Count == 0) CMSbtnXoa.Enabled = false;
        }

        private void CMSbtnReload_Click(object sender, EventArgs e)
        {
            try
            {
                this.cTPXTableAdapter.Fill(this.DS.CTPX);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload CTPX:" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
            if (bdsCTPX.Count > 0) CMSbtnXoa.Enabled = true;
        }

        private void cmsPX_Opening(object sender, CancelEventArgs e)
        {
            if (bdsCTPX.Count == 0) CMSbtnXoa.Enabled = false;
            else CMSbtnXoa.Enabled = true;
        }
    }
}
