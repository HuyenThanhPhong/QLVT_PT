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
    public partial class frmCTDDH : Form
    {
        SqlDataReader myReaderkt;
        public frmCTDDH()
        {
            InitializeComponent();
        }
        private void bdsAddNew()
        {
            if(Program.ttCMSbtnThemFrmDDH == 1)
            {
                bdsCTDDH.AddNew();
                cmbTenVT.Text = "";
            }
        }

        private String sqlcmd_SP_CHECKID_CT(string code, string code1, string type)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CHECKID_CT '" + code + "','" + code1 + "','" + type + "'" +
                "SELECT 'Return Value' = @ret";
        }
        private void cTDDHBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsCTDDH.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void frmCTDDH_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.CTDDHTableAdapter.Connection.ConnectionString = Program.connstr;
            this.dSMasoDDHTableAdapter.Connection.ConnectionString = Program.connstr;
            this.dSMaVTTableAdapter.Connection.ConnectionString = Program.connstr;

            this.CTDDHTableAdapter.Fill(this.DS.CTDDH);
            this.dSMaVTTableAdapter.Fill(this.DS.DSMaVT);
            this.dSMasoDDHTableAdapter.Fill(this.DS.DSMasoDDH);
            frmDatHang.bds_CTDDH = bdsCTDDH;
            txtMaVT.Text = cmbTenVT.SelectedValue.ToString();
            if (Program.ttCMSbtnThemFrmDDH == 1)
            {
                cmbTenVT.Text = "";
                gcCTDDH.Enabled = false;
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

        private void btnThoat_Click(object sender, EventArgs e)
        {
            bdsCTDDH.EndEdit();
            Close();
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            if (cmbMasoDDH.Text.Trim() == "")
            {
                MessageBox.Show("Mã DDH không được thiếu!", "", MessageBoxButtons.OK);
                cmbMasoDDH.Focus();
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

            //check lớn hơn 0
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

            //check MASODDH trùng khi thêm
            myReaderkt = Program.ExecSqlDataReader(sqlcmd_SP_CHECKID_CT(cmbMasoDDH.Text.ToString(), txtMaVT.Text.ToString(), "CTDDH"));
            if (myReaderkt == null) return;

            myReaderkt.Read();
            int resultCheckMaCTmasoDDH = int.Parse(myReaderkt.GetValue(0).ToString());
            myReaderkt.Close();
            if (resultCheckMaCTmasoDDH == 1 && Program.ttCMSbtnThemFrmDDH == 1)
            {
                MessageBox.Show("Mã DDH đã có vật tư này rồi!", "", MessageBoxButtons.OK);
                cmbTenVT.Focus();
                return;
            }

            try
            {
                bdsCTDDH.EndEdit();
                bdsCTDDH.ResetCurrentItem();
                this.CTDDHTableAdapter.Connection.ConnectionString = Program.connstr;
                this.CTDDHTableAdapter.Update(this.DS.CTDDH);

                bdsAddNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi CTDDH.\n" + ex.Message, "", MessageBoxButtons.OK);
                this.CTDDHTableAdapter.Fill(this.DS.CTDDH);
                bdsAddNew();
                return;
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
    }
}
