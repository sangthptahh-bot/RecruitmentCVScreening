using Microsoft.Data.SqlClient;
using RecruitmentCVScreening.WinForms.Business.Services;
using RecruitmentCVScreening.WinForms.Core.Models;
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
    public partial class MainForm : Form
    {
        private Panel panelSidebar, panelHeader, panelDesktop; private Label lblTitle; private Button btnDashboard, btnManageJobs, btnManageCandidates, btnManageApplications, btnExit;

        private Form currentChildForm;
        public MainForm()
        {
            // 1. Gọi hàm này để giữ cho Visual Studio Designer không bị lỗi (nó nằm trong file MainForm.Designer.cs)
            InitializeComponent();
            BuildCustomUI();
        }

        // Đổi tên từ InitializeComponent thành BuildCustomUI để tránh xung đột (Lỗi CS0111)
        private void BuildCustomUI()
        {
            this.panelSidebar = new Panel();
            this.panelHeader = new Panel();
            this.panelDesktop = new Panel();
            this.lblTitle = new Label();
            this.btnDashboard = new Button();
            this.btnManageJobs = new Button();
            this.btnManageCandidates = new Button();
            this.btnManageApplications = new Button();
            this.btnExit = new Button();

            // Cấu hình Form chính
            this.Text = "Hệ Thống Quản Lý Tuyển Dụng  - Nhóm 4";
            this.Size = new Size(1700, 820);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // panelSidebar
            this.panelSidebar.BackColor = Color.FromArgb(44, 62, 80);
            this.panelSidebar.Dock = DockStyle.Left;
            this.panelSidebar.Width = 250;

            this.panelSidebar.Controls.Add(this.btnExit);
            this.panelSidebar.Controls.Add(this.btnManageApplications);
            this.panelSidebar.Controls.Add(this.btnManageCandidates);
            this.panelSidebar.Controls.Add(this.btnManageJobs);
            this.panelSidebar.Controls.Add(this.btnDashboard);

            // Định dạng Nút bấm
            Button[] menuButtons = { btnDashboard, btnManageJobs, btnManageCandidates, btnManageApplications, btnExit };
            foreach (Button btn in menuButtons)
            {
                btn.Dock = DockStyle.Top;
                btn.Height = 120;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.ForeColor = Color.White;
                btn.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(20, 0, 0, 0);
                btn.Cursor = Cursors.Hand;
            }

            this.btnDashboard.Text = "📊 Giao Diện Chính";
            this.btnDashboard.Click += (s, e) => OpenChildForm(new DashboardForm());

            this.btnManageJobs.Text = "💼 Quản Lý Vị Trí";
            this.btnManageJobs.Click += (s, e) => OpenChildForm(new JobForm());

            this.btnManageCandidates.Text = "🧑‍💻 Quản Lý Ứng Viên";
            this.btnManageCandidates.Click += (s, e) => OpenChildForm(new CandidateListForm());

            this.btnManageApplications.Text = "📝 Đơn Ứng Tuyển";
            this.btnManageApplications.Click += (s, e) => OpenChildForm(new UploadCVForm());

            this.btnExit.Text = "🚪 Thoát Hệ Thống";
            this.btnExit.Dock = DockStyle.Bottom;
            this.btnExit.BackColor = Color.FromArgb(192, 57, 43);

            // Khắc phục lỗi CS0104: Gọi tường minh System.Windows.Forms.Application
            this.btnExit.Click += (s, e) => System.Windows.Forms.Application.Exit();

            // panelHeader
            this.panelHeader.BackColor = Color.FromArgb(41, 128, 185);
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 80;
            this.panelHeader.Controls.Add(this.lblTitle);

            // lblTitle
            this.lblTitle.Text = "HỆ THỐNG QUẢN LÝ TUYỂN DỤNG ";
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitle.AutoSize = false;
            this.lblTitle.Dock = DockStyle.Fill;
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // panelDesktop
            this.panelDesktop.Dock = DockStyle.Fill;
            this.panelDesktop.BackColor = Color.White;

            this.Controls.Add(this.panelDesktop);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelSidebar);
        }

        private void OpenChildForm(Form childForm)
        {
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }

            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

            lblTitle.Text = childForm.Text.ToUpper();
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }
    }
    /* =========================================================================
     * CÁC CLASS GIẢ LẬP (MOCK FORMS) ĐỂ CHẠY THỬ
     * Nhóm trưởng phân công cho các thành viên tạo các Form thật, sau đó xóa
* các class này đi. Giao diện chính sẽ tự động liên kết với Form thật.
     * ========================================================================= */
    /// Class Dashboard hiển thị các thông số thống kê tổng quan (Job, Ứng viên, Đơn)
    /// </summary>
    public class DashboardForm : Form
    {
        private Label lblCountJobs;
        private Label lblCountCandidates;
        private Label lblCountPassedApps; // Thay đổi biến thống kê qua vòng lọc

        public DashboardForm()
        {
            BuildUI();
            LoadStatistics();
        }
        /// <summary>
        /// Hàm xây dựng giao diện các thẻ (Cards) thống kê trên Dashboard
        /// </summary>
        private void BuildUI()
        {
            this.Text = "Giao diện chính - Thống kê";
            this.BackColor = Color.FromArgb(245, 247, 250); // Nền xám nhạt hiện đại

            // Tiêu đề chào mừng
            Label lblWelcome = new Label
            {
                Text = "TỔNG QUAN HỆ THỐNG",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(50, 40)
            };
            this.Controls.Add(lblWelcome);

            // Tạo Panel chứa các thẻ để dễ canh giữa
            FlowLayoutPanel flowPanel = new FlowLayoutPanel
            {
                Location = new Point(50, 120),
                Size = new Size(1400, 250),
                BackColor = Color.Transparent,
                WrapContents = false // Không tự động xuống dòng
            };
            this.Controls.Add(flowPanel);

            // Thẻ 1: Tổng Vị trí tuyển dụng (Màu xanh dương)
            Panel cardJobs = CreateStatCard("VỊ TRÍ TUYỂN DỤNG", Color.FromArgb(52, 152, 219), out lblCountJobs);
            // Thẻ 2: Tổng Ứng viên (Màu xanh lá)
            Panel cardCandidates = CreateStatCard("TỔNG SỐ ỨNG VIÊN", Color.FromArgb(39, 174, 96), out lblCountCandidates);
            // Thẻ 3: Số ứng viên qua vòng lọc CV (Màu tím)
            Panel cardPassedApps = CreateStatCard("ỨNG VIÊN QUA VÒNG LỌC", Color.FromArgb(142, 68, 173), out lblCountPassedApps);

            flowPanel.Controls.Add(cardJobs);
            flowPanel.Controls.Add(cardCandidates);
            flowPanel.Controls.Add(cardPassedApps);

            // THÊM MỚI: Thêm ảnh minh họa từ máy tính (nằm dưới các thẻ thống kê)
            PictureBox picIllustration = new PictureBox
            {
                Location = new Point(1220, 40),
                // Kích thước vuông vắn hơn, bao trọn khu vực bên phải
                Size = new Size(210, 680),
                SizeMode = PictureBoxSizeMode.Zoom, // Thu phóng giữ đúng tỷ lệ ảnh
                BackColor = Color.Transparent,
                // Neo ảnh bám vào cạnh phải và cạnh dưới để luôn ổn định khi mở lớn cửa sổ
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            try
            {
                string imagePath = @"C:\Users\Admin\OneDrive\Hình ảnh\Camera Roll\anhdaidien.jpg";

                picIllustration.Image = Image.FromFile(imagePath);
            }
            catch (Exception ex)
            {
                // Bỏ qua lỗi nếu bạn chưa sửa đường dẫn hoặc file không tồn tại
                Console.WriteLine("Không tải được ảnh: " + ex.Message);
            }
            // Thêm hình ảnh vào giao diện chính
            this.Controls.Add(picIllustration);
        }

        /// Hàm tối ưu hiệu suất truy vấn CSDL: Sử dụng ExecuteScalar để đếm số lượng nhanh nhất
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                using (SqlConnection conn = AppDbContext.GetConnection())
                {
                    conn.Open();

                    // 1. Đếm số lượng Vị trí công việc
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Jobs", conn))
                    {
                        int jobCount = (int)cmd.ExecuteScalar();
                        lblCountJobs.Text = jobCount.ToString("N0");
                    }

                    // 2. Đếm tổng số Ứng viên
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Candidates", conn))
                    {
                        int candidateCount = (int)cmd.ExecuteScalar();
                        lblCountCandidates.Text = candidateCount.ToString("N0");
                    }

                    // 3. Đếm Số ứng viên qua vòng lọc CV 
                    // Chú ý: Thay thế chữ 'Passed' bằng trạng thái thực tế trong DB của bạn nếu khác (ví dụ: 'Interviewed' hoặc 'Đạt')
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Applications WHERE Status = 'Accepted'", conn))
                    {
                        int passedCount = (int)cmd.ExecuteScalar();
                        lblCountPassedApps.Text = passedCount.ToString("N0");
                    }
                }
            }
            catch (Exception ex)
            {
                // Nếu DB chưa có bảng hoặc lỗi kết nối, hiển thị số 0 để không bị văng app
                lblCountJobs.Text = "0";
                lblCountCandidates.Text = "0";
                lblCountPassedApps.Text = "0";
                Console.WriteLine("Lỗi thống kê: " + ex.Message);
            }
        }

        /// <summary>
        /// Hàm phụ trợ: Vẽ ra một thẻ (Card) thống kê bo góc đẹp mắt
        /// </summary>
        private Panel CreateStatCard(string title, Color bgColor, out Label valueLabel)
        {
            Panel card = new Panel
            {
                Size = new Size(350, 180),
                BackColor = bgColor,
                Margin = new Padding(0, 0, 50, 0) // Tạo khoảng cách giữa các thẻ
            };

            // Sự kiện vẽ bo góc cho thẻ
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    int radius = 20;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(card.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(card.Width - radius, card.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, card.Height - radius, radius, radius, 90, 90);
                    path.CloseFigure();
                    card.Region = new Region(path);
                }
            };

            // Nhãn Tiêu đề (Ví dụ: VỊ TRÍ TUYỂN DỤNG)
            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // Nhãn Giá trị số (Ví dụ: 15)
            valueLabel = new Label
            {
                Text = "...",
                Font = new Font("Segoe UI", 48F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(15, 60)
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(valueLabel);

            return card;
        }
    }

    // Các class giả lập (Mock) cho các Form chưa ghép nối vào đây
    // Nếu nhóm đã tạo các file riêng biệt (JobForm.cs, CandidateListForm.cs...) thì hãy XÓA các class dưới này đi để tránh lỗi trùng lặp CS0111
    public class JobManagementForm : Form
    {
        public JobManagementForm() { }
    }
}