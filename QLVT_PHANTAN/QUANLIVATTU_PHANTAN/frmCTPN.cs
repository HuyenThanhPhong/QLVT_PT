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
    public partial class frmCTPN : Form
    {
        SqlDataReader myReaderkt;

        private String sqlcmd_SP_CHECK_SoLuongPhieuNhap(string MaPN, string MaVT, int soluong)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CHECK_SoLuongPhieuNhap '" + MaPN + "','" + MaVT + "'," + soluong +
                "SELECT 'Return Value' = @ret";
        }
        private String sqlcmd_SP_CHECKID_CT(string code, string code1, string type)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CHECKID_CT '" + code + "','" + code1 + "','" + type + "'" +
                "SELECT 'Return Value' = @ret";
        }
        public frmCTPN()
        {
            InitializeComponent();
        }

        private void cTPNBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsCTPN.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void bdsAddNew()
        {
            if (Program.ttCMSbtnThemFrmPN == 1)
            {
                bdsCTPN.AddNew();
                cmbTenVT.Text = "";
            }
        }

        private void frmCTPN_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.cTPNTableAdapter.Connection.ConnectionString = Program.connstr;
            this.dSMaPNTableAdapter.Connection.ConnectionString = Program.connstr;
            this.dSMaVTTableAdapter.Connection.ConnectionString = Program.connstr;

            this.cTPNTableAdapter.Fill(this.DS.CTPN);
            this.dSMaPNTableAdapter.Fill(this.DS.DSMaPN);
            this.dSMaVTTableAdapter.Fill(this.DS.DSMaVT);

            frmPhieuNhap.bds_CTPN = bdsCTPN;
            txtMaVT.Text = cmbTenVT.SelectedValue.ToString();
            if (Program.ttCMSbtnThemFrmPN == 1)
            {
                cmbTenVT.Text = "";
                gcCTPN.Enabled = false;
            }


        }

        private void txtMaVT_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtMaVT.Text.Trim() != "")
                    cmbTenVT.SelectedValue = txtMaVT.Text.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message, "", MessageBoxButtons.OK);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmbTenVT_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtMaVT.Text = cmbTenVT.SelectedValue.ToString();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            //check trống
            if (cmbMaPN.Text.Trim() == "")
            {
                MessageBox.Show("Mã PN không được thiếu!", "", MessageBoxButtons.OK);
                cmbMaPN.Focus();
                return;
            }
            if (txtMaVT.Text.Trim() == "")
            {
                MessageBox.Show("Mã VT không được thiếu!", "", MessageBoxButtons.OK);
                txtMaVT.Focus();
                return;
            }
            if (txtSoLuong.Text.Trim() == "")
            {
                MessageBox.Show("Số lượng không được thiếu!", "", MessageBoxButtons.OK);
                txtSoLuong.Focus();
                return;
            }
            if (txtDonGia.Text.Trim() == "")
            {
                MessageBox.Show("Đơn giá không được thiếu!", "", MessageBoxButtons.OK);
                txtDonGia.Focus();
                return;
            }

            //check miền giá trị
            if (int.Parse(txtSoLuong.EditValue.ToString()) <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!", "", MessageBoxButtons.OK);
                txtSoLuong.Focus();
                return;
            }
            if (int.Parse(txtDonGia.EditValue.ToString()) <= 0)
            {
                MessageBox.Show("Đơn giá phải lớn hơn hoặc bằng 0!", "", MessageBoxButtons.OK);
                txtDonGia.Focus();
                return;
            }

            //check MACTPN trùng khi thêm mới
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECKID_CT(cmbMaPN.Text.ToString(), txtMaVT.Text.ToString(), "CTPN"));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckMaCTPN = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckMaCTPN == 1 && Program.ttCMSbtnThemFrmPN == 1)
            {
                MessageBox.Show("CTPN này đã tồn tại!", "", MessageBoxButtons.OK);
                cmbTenVT.Focus();
                return;
            }

            //check số lượng nhập với số lượng đã đặt 
            int soluong_tam = int.Parse(txtSoLuong.EditValue.ToString());
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECK_SoLuongPhieuNhap(cmbMaPN.Text.ToString(), txtMaVT.Text.ToString(), soluong_tam));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckSoLuongCTPN = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckSoLuongCTPN == 1)
            {
                MessageBox.Show("Số lượng nhập phải <= số lượng đã đặt", "", MessageBoxButtons.OK);
                txtSoLuong.Focus();
                return;
            }

            if (resultCheckSoLuongCTPN == 2)
            {
                MessageBox.Show("Mã số PN này không có phiếu CTDDH để lập phiếu CTPN", "", MessageBoxButtons.OK);
                cmbMaPN.Focus();
                return;
            }

            try
            {
                bdsCTPN.EndEdit();
                bdsCTPN.ResetCurrentItem();
                this.cTPNTableAdapter.Connection.ConnectionString = Program.connstr;
                this.cTPNTableAdapter.Update(this.DS.CTPN);

                bdsAddNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi CTPN.\n" + ex.Message, "", MessageBoxButtons.OK);
                this.cTPNTableAdapter.Fill(this.DS.CTPN);
                bdsAddNew();
                return;
            }
        }
    }
}
