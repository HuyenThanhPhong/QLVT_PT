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
    public partial class frmNhanVien : Form
    {
        string macn = "";
        int vitri = 0;
        int trangthaibtnThem = 0;
        int resultCheckMaNV = 0;
        SqlDataReader myReaderkt;

        public frmNhanVien()
        {
            InitializeComponent();
        }

        private void nhanVienBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsNV.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        //private void KiemtraManv(int manvkt)
        //{
        //    string strLenh = "DECLARE	@return_value int"+

        //                        "EXEC @return_value = dbo.SP_TimMaNV " +manvkt+

        //                                "SELECT  'Return Value' = @return_value";
        //    SqlDataReader myReaderkt;
        //    myReaderkt = Program.ExecSqlDataReader(strLenh);
        //    if (myReaderkt == null) return;
        //    myReaderkt.Read();
        //    if (int.Parse(myReaderkt.GetValue(0).ToString()) == 1) ManvKt = 1;
        //    else ManvKt = 0;
        //}

        private void frmNhanVien_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
            this.NhanVienTableAdapter.Fill(this.DS.NhanVien);

            this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
            this.phieuXuatTableAdapter.Fill(this.DS.PhieuXuat);

            this.datHangTableAdapter.Connection.ConnectionString = Program.connstr;
            this.datHangTableAdapter.Fill(this.DS.DatHang);

            this.phieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
            this.phieuNhapTableAdapter.Fill(this.DS.PhieuNhap);

            macn = ((DataRowView)bdsNV[0])["MACN"].ToString(); //Giữ lại macn nhân viên đầu tiên đã đăng nhập từ phía ngoài nhưng tiềm ẩn lỗi, chờ sữa?
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

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsNV.Position;
            panelControl2.Enabled = true;
            bdsNV.AddNew();
            txtMaCN.Text = macn;
            deNgaySinh.EditValue = "";

            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcNhanVien.Enabled = false;
            trangthaibtnThem = 1;
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcNhanVien.Enabled = true;
            bdsNV.CancelEdit();
            if (btnThem.Enabled == false && trangthaibtnThem == 1) bdsNV.RemoveCurrent();
            if (btnThem.Enabled == false) bdsNV.Position = vitri;

            txtMaNV.Enabled = true;
            panelControl2.Enabled = false;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;
        }

        private void btnHieuChinh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {   //bổ sung đồng bộ thanh trạng thái khi sữa người đang đăng nhập
            vitri = bdsNV.Position;
            panelControl2.Enabled = true;
            btnThem.Enabled = btnXoa.Enabled = btnHieuChinh.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcNhanVien.Enabled = true;
            txtMaNV.Enabled = false;
            trangthaibtnThem = 0;
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //DS.EnforceConstraints = false;
                //this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                this.NhanVienTableAdapter.Fill(this.DS.NhanVien);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload:" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            Int32 manv = 0;
            if (bdsDH.Count > 0)
            {
                MessageBox.Show("Không thể xóa nhân viên vì đã lập đơn đặt hàng", "", MessageBoxButtons.OK);
                return;
            }

            if (bdsPX.Count > 0)
            {
                MessageBox.Show("Không thể xóa nhân viên vì đã lập phiếu xuất", "", MessageBoxButtons.OK);
                return;
            }

            if (bdsPN.Count > 0)
            {
                MessageBox.Show("Không thể xóa nhân viên vì đã lập phiếu nhập", "", MessageBoxButtons.OK);
                return;
            }

            if (((DataRowView)bdsNV[bdsNV.Position])["MANV"].ToString() == Program.username) //bổ sung điều kiện không thể xóa người đang đăng nhập
            {
                MessageBox.Show("Không thể xóa nhân viên vì người này đang đăng nhập", "", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show("Bạn thật sự muốn xóa nhân viên này?", "Xác nhận.", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    manv = int.Parse(((DataRowView)bdsNV[bdsNV.Position])["MANV"].ToString()); //
                    bdsNV.RemoveCurrent();
                    this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.NhanVienTableAdapter.Update(this.DS.NhanVien);

                }
                catch(Exception ex)
                {
                    MessageBox.Show("Lỗi xóa nhân viên. Bạn hãy xóa lại\n" + ex.Message, "", MessageBoxButtons.OK);
                    this.NhanVienTableAdapter.Fill(this.DS.NhanVien);
                    bdsNV.Position = bdsNV.Find("MANV", manv);
                    return;
                }
            }
            if (bdsNV.Count == 0) btnXoa.Enabled = false;
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txtMaNV.Text.Trim() == "")
            {
                MessageBox.Show("Mã nhân viên không được thiếu!", "", MessageBoxButtons.OK);
                txtMaNV.Focus();
                return;
            }
            if (txtHo.Text.Trim() == "")
            {
                MessageBox.Show("Họ nhân viên không được thiếu!", "", MessageBoxButtons.OK);
                txtHo.Focus();
                return;
            }
            if (txtTen.Text.Trim() == "")
            {
                MessageBox.Show("Tên nhân viên không được thiếu!", "", MessageBoxButtons.OK);
                txtTen.Focus();
                return;
            }
            if (deNgaySinh.Text.Trim()=="")
            {
                MessageBox.Show("Ngày sinh nhân viên không được thiếu!", "", MessageBoxButtons.OK);
                deNgaySinh.Focus();
                return;
            }
            if (txtLuong.Text.Trim() == "")
            {
                MessageBox.Show("Lương nhân viên không được thiếu!", "", MessageBoxButtons.OK);
                txtLuong.Focus();
                return;
            }
            if ( int.Parse(txtLuong.Text.ToString()) < 4000000)
            {
                MessageBox.Show("Lương phải >= 4,000,000 !", "", MessageBoxButtons.OK);
                txtLuong.Focus();
                return;
            }
            //KiemtraManv(int.Parse(txtMaNV.Text.ToString()));
            int manv = int.Parse(txtMaNV.Text.ToString());
            String sqlcmd = "DECLARE @ret int " +
                "EXEC @ret = SP_TimMaNV " + manv +
                "SELECT 'Return Value' = @ret";
            //try
            //{
            //    myReaderkt = Program.ExecSqlDataReader(sqlcmd);
            //    if (myReaderkt == null) return;
            //}catch(Exception ex)
            //{
            //    MessageBox.Show("Lỗi chạy SP_TimMaNV.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            myReaderkt = Program.ExecSqlDataReader(sqlcmd);
            if (myReaderkt == null) return;

            myReaderkt.Read();
            resultCheckMaNV = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckMaNV == 1 && trangthaibtnThem==1)
            {
                MessageBox.Show("Mã NV đã tồn tại!", "", MessageBoxButtons.OK);
                txtMaNV.Focus();
                return;
            }

            try
            {
                bdsNV.EndEdit();
                bdsNV.ResetCurrentItem();
                this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                this.NhanVienTableAdapter.Update(this.DS.NhanVien);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi nhân viên.\n" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
            gcNhanVien.Enabled = true;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;

            panelControl2.Enabled = false;
        }

        private void cmbChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbChiNhanh.SelectedValue.ToString() == "System.Data.DataRowView")
                return;
            Program.servername = cmbChiNhanh.SelectedValue.ToString();

            if(cmbChiNhanh.SelectedIndex != Program.mChinhanh)
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
                this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                this.NhanVienTableAdapter.Fill(this.DS.NhanVien);

                this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                this.phieuXuatTableAdapter.Fill(this.DS.PhieuXuat);

                this.datHangTableAdapter.Connection.ConnectionString = Program.connstr;
                this.datHangTableAdapter.Fill(this.DS.DatHang);

                this.phieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
                this.phieuNhapTableAdapter.Fill(this.DS.PhieuNhap);
                //macn = ((DataRowView)bdsNV[0])["MACN"].ToString(); //áp dung cho trường hợp vừa chuyển chi nhánh vừa sửa dữ liệu
            }
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }
    }
}
