//using QUANLIVATTU_PHANTAN.ChildForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QUANLIVATTU_PHANTAN
{
    public partial class frmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private Form CheckExists(Type fType)
        {
            foreach (Form f in this.MdiChildren)
                if (f.GetType() == fType)
                    return f;
            return null;
        }

        private void btnDangNhap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(frmDangNhap));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new frmDangNhap();
                f.MdiParent = this;
                f.Show();


            }
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        public void HienThiMenu()
        {
            MANV.Text = "MÃ NV: " + Program.username;
            HOTEN.Text = "|| HỌ VÀ TÊN: " + Program.mHoten;
            NHOM.Text = "|| NHÓM: " + Program.mGroup;

            //Phân quyền
            DanhMuc.Visible = NghiepVu.Visible = BaoCao.Visible = true;
        }

        private void btnDangXuat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Program.conn.Close();

            //đóng tất cả form con
            Form[] childArray = this.MdiChildren;
            foreach (Form childForm in childArray)
            {
                childForm.Close();
                Program.conn.Close();
            }
            //if (frmDatHang.fCTDDH != null)
            //{
            //    frmDatHang.fCTDDH.Close();
            //}
            //if (frmPhieuNhap.fCTPN != null)
            //{
            //    frmPhieuNhap.fCTPN.Close();
            //}
            //if (frmPhieuXuat.fCTPX != null)
            //{
            //    frmPhieuXuat.fCTPX.Close();
            //}

            Program.servername = "";
            Program.mlogin = "";
            Program.password = "";
            Program.mloginDN = "";
            Program.passwordDN = "";

            Program.username = "";
            Program.mGroup = "";
            Program.mHoten = "";
            HienThiMenu();
            btnTaoTaiKhoan.Enabled = btnDangXuat.Enabled = DanhMuc.Visible = NghiepVu.Visible = BaoCao.Visible = false;

            
        }

        private void btnNhanVien_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(frmNhanVien));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new frmNhanVien();
                f.MdiParent = this;
                f.Show();


            }
        }

        private void btnVatTu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(frmVatTu));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new frmVatTu();
                f.MdiParent = this;
                f.Show();


            }
        }

        private void btnKho_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(frmKho));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new frmKho();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void btnPN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(frmPhieuNhap));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new frmPhieuNhap();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void btnPX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(frmPhieuXuat));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new frmPhieuXuat();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void btnDDH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(frmDatHang));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new frmDatHang();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void btnTaoTaiKhoan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(frmTaoTaiKhoan));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new frmTaoTaiKhoan();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void btnDSNV_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_InDSNhanVien));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new Frpt_InDSNhanVien();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (Program.mGroup == "USER") btnTaoTaiKhoan.Enabled = false;
        }

        private void btnDMVatTu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_InDanhMucVatTu));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new Frpt_InDanhMucVatTu();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void btnCTSLTGHangNhapXuat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_ChiTietSLTGHangNhapXuat));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new Frpt_ChiTietSLTGHangNhapXuat();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void btnDDHChuaCoPN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_DSDDHChuaCoPhieuNhap));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new Frpt_DSDDHChuaCoPhieuNhap();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void btnTTHDMotNV_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_HoatDongNV));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new Frpt_HoatDongNV();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void btnTongHopNX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(Frpt_TongHopNhapXuat));
            if (frm != null) frm.Activate();
            else
            {
                Form f = new Frpt_TongHopNhapXuat();
                f.MdiParent = this;
                f.Show();
            }
        }
    }
}
