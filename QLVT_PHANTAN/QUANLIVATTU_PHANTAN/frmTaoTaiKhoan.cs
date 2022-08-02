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
    public partial class frmTaoTaiKhoan : Form
    {
        SqlDataReader myReaderTTK;
        public frmTaoTaiKhoan()
        {
            InitializeComponent();
        }

        private void hoTenNVBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsHoTenNV.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private String sqlcmd_SP_CreateLogin(string loginname, string pass, string username, string role)
        {
            return "DECLARE @ret int " +
                "EXEC @ret = SP_CreateLogin '" + loginname + "','" + pass + "','" + username + "','"+role+ "'" +
               "SELECT 'Return Value' = @ret";
        }

        private void frmTaoTaiKhoan_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.hoTenNVTableAdapter.Connection.ConnectionString = Program.connstr;
            this.hoTenNVTableAdapter.Fill(this.DS.HoTenNV);

            cmbChiNhanh.DataSource = Program.bds_dspm;
            cmbChiNhanh.DisplayMember = "TENCN";
            cmbChiNhanh.ValueMember = "TENSERVER";
            cmbChiNhanh.SelectedIndex = Program.mChinhanh;
            txtMaNV.Text = cmbHoTenNV.SelectedValue.ToString();

            if (Program.mGroup == "CONGTY") //bật tắt theo phân quyền
            {
                cmbChiNhanh.Enabled = true;
                cmbNhom.Visible = false;
                txtNhom.Text = "CONGTY";
            }
            else
            {
                cmbChiNhanh.Enabled = false;
                txtNhom.Text = cmbNhom.Text.ToString();
            }
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

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            //check empty
            if (txtLogin.Text.Trim() == "")
            {
                MessageBox.Show("Tên đăng nhập không được thiếu!", "", MessageBoxButtons.OK);
                txtLogin.Focus();
                return;
            }
            if (txtPass.Text.Trim() == "")
            {
                MessageBox.Show("Mật khẩu không được thiếu!", "", MessageBoxButtons.OK);
                txtPass.Focus();
                return;
            }
            if (txtPass2.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập lại mật khẩu!", "", MessageBoxButtons.OK);
                txtPass2.Focus();
                return;
            }
            if (txtNhom.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhóm quyền!", "", MessageBoxButtons.OK);
                cmbNhom.Focus();
                return;
            }

            if (txtPass.Text.ToString() != txtPass2.Text.ToString())
            {
                MessageBox.Show("Nhập lại mật khẩu không đúng!", "", MessageBoxButtons.OK);
                txtPass2.Focus();
                return;
            }

            myReaderTTK = Program.ExecSqlDataReader(sqlcmd_SP_CreateLogin(txtLogin.Text.ToString(), txtPass.Text.ToString(), txtMaNV.Text.ToString(), txtNhom.Text.ToString()));
            if (myReaderTTK == null) return;

            myReaderTTK.Read();
            int resultTTK = int.Parse(myReaderTTK.GetValue(0).ToString());
            myReaderTTK.Close();
            if (resultTTK==1)
            {
                MessageBox.Show("Tên tài khoản bị trùng!", "", MessageBoxButtons.OK);
                txtLogin.Focus();
                return;
            }
            if (resultTTK == 2)
            {
                MessageBox.Show("Thất bại! Nhân viên "+cmbHoTenNV.Text.ToString()+" đã có tài khoản!", "", MessageBoxButtons.OK);
                cmbHoTenNV.Focus();
                return;
            }
            if (resultTTK == 0)
            {
                MessageBox.Show("Tạo tài khoản thành công!", "", MessageBoxButtons.OK);
                return;
            }
        }

        private void cmbNhom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.mGroup != "CONGTY") 
            {
                txtNhom.Text = cmbNhom.Text.ToString();
            }
        }
    }
}
