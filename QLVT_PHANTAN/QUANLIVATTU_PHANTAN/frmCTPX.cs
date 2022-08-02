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
    public partial class frmCTPX : Form
    {
        SqlDataReader myReaderkt;

        private String sqlcmd_SP_CHECK_SoLuongPhieuXuat(string MaVT, int soluong)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CHECK_SoLuongPhieuXuat '" + MaVT + "'," + soluong +
                "SELECT 'Return Value' = @ret";
        }
        private String sqlcmd_SP_CHECKID_CT(string code, string code1, string type)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CHECKID_CT '" + code + "','" + code1 + "','" + type + "'" +
                "SELECT 'Return Value' = @ret";
        }
        public frmCTPX()
        {
            InitializeComponent();
        }

        private void cTPXBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsCTPX.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void bdsAddNew()
        {
            if (Program.ttCMSbtnThemFrmPX == 1)
            {
                bdsCTPX.AddNew();
                cmbTenVT.Text = "";
            }
        }

        private void frmCTPX_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.cTPXTableAdapter.Connection.ConnectionString = Program.connstr;
            this.dSMaPXTableAdapter.Connection.ConnectionString = Program.connstr;
            this.dSMaVTTableAdapter.Connection.ConnectionString = Program.connstr;

            this.cTPXTableAdapter.Fill(this.DS.CTPX);
            this.dSMaPXTableAdapter.Fill(this.DS.DSMaPX);
            this.dSMaVTTableAdapter.Fill(this.DS.DSMaVT);

            frmPhieuXuat.bds_CTPX = bdsCTPX;
            txtMaVT.Text = cmbTenVT.SelectedValue.ToString();
            if (Program.ttCMSbtnThemFrmPX == 1)
            {
                cmbTenVT.Text = "";
                gcCTPX.Enabled = false;
            }

        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

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

        private void btnGhi_Click(object sender, EventArgs e)
        {
            // check trống
            if (cmbMaPX.Text.Trim() == "")
            {
                MessageBox.Show("Mã PX không được thiếu!", "", MessageBoxButtons.OK);
                cmbMaPX.Focus();
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

            //check miền giá tr
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

            //check MACTPX trùng khi thêm mới
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECKID_CT(cmbMaPX.Text.ToString(), txtMaVT.Text.ToString(), "CTPX"));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckMaCTPX = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckMaCTPX == 1 && Program.ttCMSbtnThemFrmPN == 1)
            {
                MessageBox.Show("CTPX này đã tồn tại!", "", MessageBoxButtons.OK);
                cmbTenVT.Focus();
                return;
            }
            //check số lượng xuất với số lượng tồn khi thêm và sửa
            int soluong_tam = int.Parse(txtSoLuong.EditValue.ToString());
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECK_SoLuongPhieuXuat(txtMaVT.Text.ToString(), soluong_tam));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckSLTonCTPX = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckSLTonCTPX == 1)
            {
                MessageBox.Show("Số lượng xuất phải <= số lượng tồn", "", MessageBoxButtons.OK);
                txtSoLuong.Focus();
                return;
            }

            try
            {
                bdsCTPX.EndEdit();
                bdsCTPX.ResetCurrentItem();
                this.cTPXTableAdapter.Connection.ConnectionString = Program.connstr;
                this.cTPXTableAdapter.Update(this.DS.CTPX);

                bdsAddNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi CTPX.\n" + ex.Message, "", MessageBoxButtons.OK);
                this.cTPXTableAdapter.Fill(this.DS.CTPX);
                bdsAddNew();
                return;
            }
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
    }
}
