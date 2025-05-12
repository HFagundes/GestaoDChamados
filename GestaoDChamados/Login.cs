using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using ChamadosApp;
using GestaoDChamados.Usuario;
using Npgsql;

namespace ChamadosApp
{
    public class LoginForm : Form
    {
        private string usernameAutenticado;
        private Panel panelMain;
        private Label lblWelcome;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkRemember;
        private LinkLabel linkForgotPassword;
        private Button btnSubmit;
        private PictureBox picIllustration;
        private Label lblAppName;

        private string connectionString = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";
        private readonly string rememberFile = Path.Combine(Application.StartupPath, "remember.txt");

        public LoginForm()
        {
            InitializeComponent();
            this.AcceptButton = btnSubmit;
            this.Load += LoginForm_Load;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Carrega usuário salvo, se existir
            if (File.Exists(rememberFile))
            {
                txtUsername.Text = File.ReadAllText(rememberFile);
                chkRemember.Checked = true;
            }
        }

        private void InitializeComponent()
        {
            panelMain = new Panel();
            lblAppName = new Label();
            lblWelcome = new Label();
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            chkRemember = new CheckBox();
            linkForgotPassword = new LinkLabel();
            btnSubmit = new Button();
            picIllustration = new PictureBox();

            panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picIllustration).BeginInit();
            SuspendLayout();

            // panelMain
            panelMain.BackColor = Color.Black;
            panelMain.Controls.Add(lblAppName);
            panelMain.Controls.Add(lblWelcome);
            panelMain.Controls.Add(txtUsername);
            panelMain.Controls.Add(txtPassword);
            panelMain.Controls.Add(chkRemember);
            panelMain.Controls.Add(linkForgotPassword);
            panelMain.Controls.Add(btnSubmit);
            panelMain.Dock = DockStyle.Left;
            panelMain.Size = new Size(400, 400);

            // lblAppName (opcional, caso queira exibir nome do app)
            lblAppName.AutoSize = true;
            lblAppName.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblAppName.ForeColor = Color.White;
            lblAppName.Location = new Point(114, 20);
            lblAppName.Text = "ATENDE.AI";

            // lblWelcome
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Location = new Point(120, 80);
            lblWelcome.Text = "BEM-VINDO";

            // txtUsername
            txtUsername.Font = new Font("Segoe UI", 10F);
            txtUsername.Location = new Point(35, 120);
            txtUsername.PlaceholderText = "Usuário";
            txtUsername.Size = new Size(300, 25);
            txtUsername.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtUsername.Width, txtUsername.Height, 15, 15));

            // txtPassword
            txtPassword.Font = new Font("Segoe UI", 10F);
            txtPassword.Location = new Point(35, 160);
            txtPassword.PlaceholderText = "Senha";
            txtPassword.Size = new Size(300, 25);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtPassword.Width, txtPassword.Height, 15, 15));

            // chkRemember
            chkRemember.AutoSize = true;
            chkRemember.Font = new Font("Segoe UI", 8F);
            chkRemember.ForeColor = Color.White;
            chkRemember.Location = new Point(35, 200);
            chkRemember.Text = "Lembrar-me";

            // linkForgotPassword
            linkForgotPassword.AutoSize = true;
            linkForgotPassword.Font = new Font("Segoe UI", 8F);
            linkForgotPassword.LinkColor = Color.White;
            linkForgotPassword.Location = new Point(245, 200);
            linkForgotPassword.Text = "Esqueceu Senha";

            // btnSubmit
            btnSubmit.BackColor = Color.White;
            btnSubmit.Cursor = Cursors.Hand;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSubmit.ForeColor = Color.Black;
            btnSubmit.Location = new Point(35, 230);
            btnSubmit.Size = new Size(300, 35);
            btnSubmit.Text = "ENTRAR";
            btnSubmit.Click += BtnSubmit_Click;

            // picIllustration
            picIllustration.Image = AplicarDegradeCircular(Image.FromFile("resources\\montanha.png"));
            picIllustration.Location = new Point(400, 0);
            picIllustration.Size = new Size(400, 400);
            picIllustration.SizeMode = PictureBoxSizeMode.StretchImage;

            // LoginForm
            ClientSize = new Size(800, 400);
            Controls.Add(panelMain);
            Controls.Add(picIllustration);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picIllustration).EndInit();
            ResumeLayout(false);
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;

            var (tipoUsuario, idUsuario) = AutenticarUsuario(user, pass);

            if (tipoUsuario != null)
            {
                // Salva ou remove o usuário no arquivo “remember.txt”
                if (chkRemember.Checked)
                    File.WriteAllText(rememberFile, user);
                else if (File.Exists(rememberFile))
                    File.Delete(rememberFile);

                usernameAutenticado = user;
                MessageBox.Show("Login efetuado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Redireciona conforme tipo
                if (tipoUsuario == "admin")
                {
                    var adminForm = new AdminForm();
                    adminForm.Show();
                }
                else if (tipoUsuario == "funcionario")
                {
                    var funcionarioForm = new FuncionarioForm();
                    funcionarioForm.Show();
                }
                else // usuario
                {
                    var usuarioForm = new UsuarioForm(usernameAutenticado);
                    usuarioForm.Show();
                }

                this.Hide();
            }
            else
            {
                MessageBox.Show("Usuário ou senha inválidos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private (string tipo, int? id) AutenticarUsuario(string username, string password)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, tipo FROM usuarios WHERE usuario = @usuario AND senha = @senha";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("usuario", username);
                        cmd.Parameters.AddWithValue("senha", password);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string tipo = reader.GetString(1);
                                return (tipo, id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar ao banco: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return (null, null);
        }

        private Image AplicarDegradeCircular(Image img)
        {
            Bitmap bitmap = new Bitmap(img);
            int width = bitmap.Width;
            int height = bitmap.Height;
            Bitmap gradiente = new Bitmap(width, height);

            using (var g = Graphics.FromImage(gradiente))
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        double distX = x - width / 1.4;
                        double distY = y - height / 3;
                        double dist = Math.Sqrt(distX * distX + distY * distY);
                        double maxDist = Math.Sqrt((width / 2.0) * (width / 2.0) + (height / 2.0) * (height / 2.0));
                        int alpha = (int)(255 * (dist / maxDist));
                        alpha = Math.Max(0, Math.Min(255, alpha));
                        gradiente.SetPixel(x, y, Color.FromArgb(alpha, Color.Black));
                    }
                }
            }

            Bitmap resultado = new Bitmap(width, height);
            using (var g = Graphics.FromImage(resultado))
            {
                g.DrawImage(bitmap, 0, 0);
                g.DrawImage(gradiente, 0, 0);
            }

            return resultado;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );
    }
}
