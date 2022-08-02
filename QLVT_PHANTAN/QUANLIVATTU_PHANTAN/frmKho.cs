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
    public partial class frmKho : Form
    {
        int vitri = 0;
        string macn = "";
        int trangthaibtnThem = 0;
        SqlDataReader myReaderkt;
        public frmKho()
        {
            InitializeComponent();
        }

        private void khoBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsKho.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private String sqlcmd_SP_CHECKID(string code, string type)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CHECKID '" + code + "','" + type + "'" +
                "SELECT 'Return Value' = @ret";
        }

        private void frmKho_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.KhoTableAdapter.Connection.ConnectionString = Program.connstr;
            this.KhoTableAdapter.Fill(this.DS.Kho);

            this.phieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
            this.phieuNhapTableAdapter.Fill(this.DS.PhieuNhap);

            this.datHangTableAdapter.Connection.ConnectionString = Program.connstr;
            this.datHangTableAdapter.Fill(this.DS.DatHang);

            this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
            this.phieuXuatTableAdapter.Fill(this.DS.PhieuXuat);

            macn = ((DataRowView)bdsKho[0])["MACN"].ToString();
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

        private void khoBindingNavigatorSaveItem_Click_1(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsKho.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsKho.Position;
            panelControl2.Enabled = true;
            bdsKho.AddNew();
            txtMaCN.Text = macn;

            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcKho.Enabled = false;
            trangthaibtnThem = 1;
        }

        private void btnHieuChinh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsKho.Position;
            panelControl2.Enabled = true;
            btnThem.Enabled = btnXoa.Enabled = btnHieuChinh.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcKho.Enabled = true;
            txtMaKho.Enabled = false;
            trangthaibtnThem = 0;
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcKho.Enabled = true;
            bdsKho.CancelEdit();
            if (btnThem.Enabled == false && trangthaibtnThem == 1) bdsKho.RemoveCurrent();
            if (btnThem.Enabled == false) bdsKho.Position = vitri;

            txtMaKho.Enabled = true;
            panelControl2.Enabled = false;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.KhoTableAdapter.Fill(this.DS.Kho);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload:" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String makho = "";
            if (bdsDDH.Count > 0)
            {
                MessageBox.Show("Không thể xóa kho vì đã có DDH", "", MessageBoxButtons.OK);
                return;
            }

            if (bdsPN.Count > 0)
            {
                MessageBox.Show("Không thể xóa kho vì đã có PN", "", MessageBoxButtons.OK);
                return;
            }

            if (bdsPX.Count > 0)
            {
                MessageBox.Show("Không thể xóa kho vì đã PX", "", MessageBoxButtons.OK);
                return;
            }


            if (MessageBox.Show("Bạn thật sự muốn xóa vật tư này?", "Xác nhận.", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    makho = ((DataRowView)bdsKho[bdsKho.Position])["MAKHO"].ToString(); //
                    bdsKho.RemoveCurrent();
                    this.KhoTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.KhoTableAdapter.Update(this.DS.Kho);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa kho. Bạn hãy xóa lại\n" + ex.Message, "", MessageBoxButtons.OK);
                    this.KhoTableAdapter.Fill(this.DS.Kho);
                    bdsKho.Position = bdsKho.Find("MAKHO", makho);
                    return;
                }
            }
            if (bdsKho.Count == 0) btnXoa.Enabled = false;
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txtMaKho.Text.Trim() == "")
            {
                MessageBox.Show("Mã kho không được thiếu!", "", MessageBoxButtons.OK);
                txtMaKho.Focus();
                return;
            }
            if (txtTenKho.Text.Trim() == "")
            {
                MessageBox.Show("Tên kho không được thiếu!", "", MessageBoxButtons.OK);
                txtTenKho.Focus();
                return;
            }
            if (txtDiaChi.Text.Trim() == "")
            {
                MessageBox.Show("Địa chỉ kho không được thiếu!", "", MessageBoxButtons.OK);
                txtDiaChi.Focus();
                return;
            }

            //check MaKho trùng
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECKID(txtMaKho.Text.ToString(), "MAKHO"));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckMaVT = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckMaVT == 1 && trangthaibtnThem == 1)
            {
                MessageBox.Show("Mã kho đã tồn tại!", "", MessageBoxButtons.OK);
                txtMaKho.Focus();
                return;
            }
            //check TenKho(unique)
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECKID(txtTenKho.Text.ToString(), "TENKHO"));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckTENVT = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckTENVT == 1 && trangthaibtnThem == 1)
            {
                MessageBox.Show("Tên kho không được trùng!", "", MessageBoxButtons.OK);
                txtTenKho.Focus();
                return;
            }

            try
            {
                bdsKho.EndEdit();
                bdsKho.ResetCurrentItem();
                this.KhoTableAdapter.Connection.ConnectionString = Program.connstr;
                this.KhoTableAdapter.Update(this.DS.Kho);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi Kho.\n" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
            gcKho.Enabled = true;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;

            panelControl2.Enabled = false;
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
                this.KhoTableAdapter.Connection.ConnectionString = Program.connstr;
                this.KhoTableAdapter.Fill(this.DS.Kho);

                this.phieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
                this.phieuNhapTableAdapter.Fill(this.DS.PhieuNhap);

                this.datHangTableAdapter.Connection.ConnectionString = Program.connstr;
                this.datHangTableAdapter.Fill(this.DS.DatHang);

                this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                this.phieuXuatTableAdapter.Fill(this.DS.PhieuXuat);
            }
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }
    }
}
