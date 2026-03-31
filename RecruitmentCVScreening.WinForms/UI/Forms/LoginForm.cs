using Microsoft.Data.SqlClient;
using RecruitmentCVScreening.WinForms.Data.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecruitmentCVScreening.WinForms.UI.Forms
{
    public partial class LoginForm : Form

    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Đoạn code bo tròn góc Form
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, 15, 15, 180, 90); // Top-left
            path.AddArc(this.Width - 15, 0, 15, 15, 270, 90); // Top-right
            path.AddArc(this.Width - 15, this.Height - 15, 15, 15, 0, 90); // Bottom-right
            path.AddArc(0, this.Height - 15, 15, 15, 90, 90); // Bottom-left
            this.Region = new Region(path);
        }

        public LoginForm()
        {
            InitializeComponent();
            // Bắt sự kiện ấn phím cho 2 ô nhập liệu
            txtUsername.KeyDown += txtUsername_KeyDown;
            txtPassword.KeyDown += txtPassword_KeyDown;

            // Bo tròn góc cho cái khung nền trắng. 
            // Dựa vào code cũ của bạn, tôi đoán cái khung trắng đó tên là "groupBox1".
            // Nếu nó tên là "panel1" thì bạn đổi chữ "groupBox1" thành "panel1" nhé!
            if (this.Controls["groupBox1"] != null)
            {
                this.Controls["groupBox1"].Paint += KhungTrang_Paint;
            }
            // BẮT BUỘC PHẢI CÓ DÒNG LỆNH GỌI HÀM NÀY SAU KHI KHỞI TẠO FORM
            CreateForgotPasswordLink();
        }
        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            // Bo 4 góc
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Điều kiện đăng nhập
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                // 1. Kiểm tra đầu vào trống
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ Username và Password!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Kiểm tra trong Database
                try
                {
                    using (SqlConnection conn = AppDbContext.GetConnection())
                    {
                        conn.Open();
                        // Truy vấn kiểm tra tài khoản (Lưu ý: Trong thực tế nên dùng Hash mật khẩu)
                        string sql = "SELECT COUNT(*) FROM Users WHERE Username = @user AND Password = @pass";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@pass", password);

                        int result = (int)cmd.ExecuteScalar();

                        if (result > 0)
                        {
                            MessageBox.Show("Đăng nhập thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Mở MainForm và ẩn LoginForm hiện tại
                            MainForm mainForm = new MainForm();
                            mainForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

            }
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        { }
        private void btnLogin_MouseEnter(object sender, EventArgs e)
        {
            // Khi di chuột vào: Nút đổi thành màu xanh đậm, chữ trắng
            btnLogin.BackColor = Color.FromArgb(0, 120, 215); // Màu xanh đẹp
            btnLogin.ForeColor = Color.White;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        { }
        private void btnLogin_MouseLeave(object sender, EventArgs e)
        {
            // Khi di chuột ra: Trở lại màu mặc định
            btnLogin.BackColor = SystemColors.Control;
            btnLogin.ForeColor = SystemColors.ControlText;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Bo góc loGin
        private void btnLogin_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = GetRoundedPath(btn.ClientRectangle, 15))
            {
                btn.Region = new Region(path);
            }
        }
        // Bo góc Exit
        private void button2_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = GetRoundedPath(btn.ClientRectangle, 15))
            {
                btn.Region = new Region(path);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        // Khi ấn Enter ở ô Username -> Nhảy xuống Password
        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Tắt âm thanh "ting" khó chịu của Windows
                txtPassword.Focus();       // Chuyển con trỏ chuột xuống ô Password
            }
        }

        // Khi ấn Enter ở ô Password -> Gọi nút Đăng nhập
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Tắt âm thanh "ting"
                button1_Click(sender, e);  // Tự động kích hoạt hành động Click của nút Login
            }
        }

        // Hàm xử lý bo góc cho cái khung (Panel/GroupBox) màu trắng
        private void KhungTrang_Paint(object sender, PaintEventArgs e)
        {
            Control ctrl = (Control)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Số 20 ở đây là độ cong của góc. Bạn có thể tăng lên 30 nếu muốn góc tròn hơn.
            using (GraphicsPath path = GetRoundedPath(ctrl.ClientRectangle, 30))
            {
                ctrl.Region = new Region(path);
            }
        }
        // ==========================================================
        // === PHẦN CODE THÊM MỚI: QUÊN MẬT KHẨU (LOGIC XỬ LÝ) ===
        // ==========================================================

        //Tạo một Link "Quên mật khẩu?" bằng code C# để không làm hỏng giao diện Design của bạn
        
        private void CreateForgotPasswordLink()
        {
            LinkLabel linkForgot = new LinkLabel();
            linkForgot.Text = "Quên mật khẩu?";
            linkForgot.AutoSize = true;
            linkForgot.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            linkForgot.LinkColor = Color.FromArgb(0, 120, 215); // Màu xanh biển giống nút Login
            linkForgot.ActiveLinkColor = Color.Red;

            // Đặt link này nằm ở phía dưới nút Đăng nhập/Exit. 
            // Nếu vị trí bị lệch, có thể chỉnh 2 con số X (150) và Y (280):
            linkForgot.Location = new Point(120, 320);

            // Gắn sự kiện khi click vào chữ
            linkForgot.LinkClicked += LinkForgot_LinkClicked;

            // Thêm vào khung trắng
            if (this.Controls["groupBox1"] != null)
            {
                this.Controls["groupBox1"].Controls.Add(linkForgot);
                linkForgot.BringToFront(); // Đưa lên trên cùng cho khỏi bị che
            }
        }

        //Sự kiện mở Cửa sổ khôi phục mật khẩu khi bấm vào link
        
        private void LinkForgot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Code tạo một Form nhỏ (Dialog) tự động
            Form resetForm = new Form
            {
                Text = "Khôi phục mật khẩu",
                Size = new Size(350, 260),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            Label lblUser = new Label { Text = "Nhập Tên đăng nhập của bạn:", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            TextBox txtUserReset = new TextBox { Location = new Point(20, 45), Width = 290, Font = new Font("Segoe UI", 10F) };

            Label lblNewPass = new Label { Text = "Nhập Mật khẩu mới:", Location = new Point(20, 85), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            TextBox txtNewPassReset = new TextBox { Location = new Point(20, 110), Width = 290, Font = new Font("Segoe UI", 10F), UseSystemPasswordChar = true };

            Button btnConfirm = new Button
            {
                Text = "Xác nhận đổi mật khẩu",
                Location = new Point(20, 160),
                Width = 290,
                Height = 40,
                BackColor = Color.FromArgb(39, 174, 96), // Nút màu xanh lá cây
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnConfirm.FlatAppearance.BorderSize = 0;

            // Xử lý khi bấm nút "Xác nhận đổi" trong form nhỏ
            btnConfirm.Click += (s, args) =>
            {
                if (string.IsNullOrWhiteSpace(txtUserReset.Text) || string.IsNullOrWhiteSpace(txtNewPassReset.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu mới!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (SqlConnection conn = AppDbContext.GetConnection())
                    {
                        conn.Open();
                        // 1. Kiểm tra tài khoản có thực sự tồn tại trong DB không
                        string checkSql = "SELECT COUNT(*) FROM Users WHERE Username = @user";
                        SqlCommand checkCmd = new SqlCommand(checkSql, conn);
                        checkCmd.Parameters.AddWithValue("@user", txtUserReset.Text.Trim());
                        int exists = (int)checkCmd.ExecuteScalar();

                        if (exists > 0)
                        {
                            // 2. Tài khoản hợp lệ -> Tiến hành Cập nhật Password
                            string updateSql = "UPDATE Users SET Password = @pass WHERE Username = @user";
                            SqlCommand updateCmd = new SqlCommand(updateSql, conn);
                            updateCmd.Parameters.AddWithValue("@pass", txtNewPassReset.Text.Trim());
                            updateCmd.Parameters.AddWithValue("@user", txtUserReset.Text.Trim());
                            updateCmd.ExecuteNonQuery();

                            MessageBox.Show("Đổi mật khẩu thành công! Bạn có thể dùng mật khẩu mới để đăng nhập.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            resetForm.Close(); // Tự động đóng form khôi phục
                        }
                        else
                        {
                            MessageBox.Show("Tên đăng nhập không tồn tại trong hệ thống!", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            resetForm.Controls.Add(lblUser);
            resetForm.Controls.Add(txtUserReset);
            resetForm.Controls.Add(lblNewPass);
            resetForm.Controls.Add(txtNewPassReset);
            resetForm.Controls.Add(btnConfirm);

            // Bật form nhỏ lên màn hình
            resetForm.ShowDialog();
        }
    }
}
