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
    public partial class frmVatTu : Form
    {
        int vitri = 0;
        int trangthaibtnThem = 0;
        SqlDataReader myReaderkt;
        public frmVatTu()
        {
            InitializeComponent();
        }

        private void vattuBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsVT.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private String sqlcmd_SP_CHECKID(string code, string type)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CHECKID '" + code + "','"+type+"'"+
                "SELECT 'Return Value' = @ret";
        }

        private void frmVatTu_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.VatTuTableAdapter.Connection.ConnectionString = Program.connstr;
            this.VatTuTableAdapter.Fill(this.DS.Vattu);

            this.cTPNTableAdapter.Connection.ConnectionString = Program.connstr;
            this.cTPNTableAdapter.Fill(this.DS.CTPN);

            this.cTDDHTableAdapter.Connection.ConnectionString = Program.connstr;
            this.cTDDHTableAdapter.Fill(this.DS.CTDDH);

            this.cTPXTableAdapter.Connection.ConnectionString = Program.connstr;
            this.cTPXTableAdapter.Fill(this.DS.CTPX);

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
            vitri = bdsVT.Position;
            panelControl2.Enabled = true;
            bdsVT.AddNew();

            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcVatTu.Enabled = false;
            trangthaibtnThem = 1;
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcVatTu.Enabled = true;
            bdsVT.CancelEdit();
            if (btnThem.Enabled == false && trangthaibtnThem == 1) bdsVT.RemoveCurrent();
            if (btnThem.Enabled == false) bdsVT.Position = vitri;

            txtMAVT.Enabled = true;
            panelControl2.Enabled = false;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;
        }

        private void btnHieuChinh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsVT.Position;
            panelControl2.Enabled = true;
            btnThem.Enabled = btnXoa.Enabled = btnHieuChinh.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcVatTu.Enabled = true;
            txtMAVT.Enabled = false;
            trangthaibtnThem = 0;
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.VatTuTableAdapter.Fill(this.DS.Vattu);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload:" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String mavt = "";
            if (bdsCTPX.Count > 0)
            {
                MessageBox.Show("Không thể xóa vật tư vì đã CTPX", "", MessageBoxButtons.OK);
                return;
            }

            if (bdsCTPN.Count > 0)
            {
                MessageBox.Show("Không thể xóa vật tư vì đã CTPN", "", MessageBoxButtons.OK);
                return;
            }

            if (bdsCTDDH.Count > 0)
            {
                MessageBox.Show("Không thể xóa vật tư vì đã CTDDH", "", MessageBoxButtons.OK);
                return;
            }


            if (MessageBox.Show("Bạn thật sự muốn xóa vật tư này?", "Xác nhận.", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    mavt = ((DataRowView)bdsVT[bdsVT.Position])["MAVT"].ToString(); //
                    bdsVT.RemoveCurrent();
                    this.VatTuTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.VatTuTableAdapter.Update(this.DS.Vattu);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa nhân viên. Bạn hãy xóa lại\n" + ex.Message, "", MessageBoxButtons.OK);
                    this.VatTuTableAdapter.Fill(this.DS.Vattu);
                    bdsVT.Position = bdsVT.Find("MAVT", mavt);
                    return;
                }
            }
            if (bdsVT.Count == 0) btnXoa.Enabled = false;
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txtMAVT.Text.Trim() == "")
            {
                MessageBox.Show("Mã vật tư không được thiếu!", "", MessageBoxButtons.OK);
                txtMAVT.Focus();
                return;
            }
            if (txtDVT.Text.Trim() == "")
            {
                MessageBox.Show("Đơn vị tính vật tư không được thiếu!", "", MessageBoxButtons.OK);
                txtDVT.Focus();
                return;
            }
            if (txtSoLuongTon.Text.Trim() == "")
            {
                MessageBox.Show("Số lượng tồn của vật tư không được thiếu!", "", MessageBoxButtons.OK);
                txtSoLuongTon.Focus();
                return;
            }
            if (txtTenVT.Text.Trim() == "")
            {
                MessageBox.Show("Tên vật tư không được thiếu!", "", MessageBoxButtons.OK);
                txtTenVT.Focus();
                return;
            }

            //check MaVT trùng
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECKID(txtMAVT.Text.ToString(),"MAVT"));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckMaVT = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckMaVT == 1 && trangthaibtnThem == 1)
            {
                MessageBox.Show("Mã vật tư đã tồn tại!", "", MessageBoxButtons.OK);
                txtMAVT.Focus();
                return;
            }
            //check TenVT(unique)
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECKID(txtTenVT.Text.ToString(), "TENVT"));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckTENVT = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckTENVT == 1 && trangthaibtnThem == 1)
            {
                MessageBox.Show("Tên vật tư không được trùng!", "", MessageBoxButtons.OK);
                txtTenVT.Focus();
                return;
            }

            try
            {
                bdsVT.EndEdit();
                bdsVT.ResetCurrentItem();
                this.VatTuTableAdapter.Connection.ConnectionString = Program.connstr;
                this.VatTuTableAdapter.Update(this.DS.Vattu);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi Vật tư.\n" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
            gcVatTu.Enabled = true;
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
                this.VatTuTableAdapter.Connection.ConnectionString = Program.connstr;
                this.VatTuTableAdapter.Fill(this.DS.Vattu);

                this.cTPNTableAdapter.Connection.ConnectionString = Program.connstr;
                this.cTPNTableAdapter.Fill(this.DS.CTPN);

                this.cTDDHTableAdapter.Connection.ConnectionString = Program.connstr;
                this.cTDDHTableAdapter.Fill(this.DS.CTDDH);

                this.cTPXTableAdapter.Connection.ConnectionString = Program.connstr;
                this.cTPXTableAdapter.Fill(this.DS.CTPX);
            }
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }
    }
}
