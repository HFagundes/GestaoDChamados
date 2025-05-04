using System;
using System.Drawing;
using System.Windows.Forms;
using AtendeAI;
using Npgsql;

namespace ChamadosApp
{
    public class LoginForm : Form
    {
        private Panel panelLeft;
        private Label lblWelcome;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkRemember;
        private LinkLabel linkForgotPassword;
        private Button btnSubmit;
        private Panel panelRight;
        private PictureBox picIllustration;

        private string connectionString = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";


        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            panelLeft = new Panel();
            lblWelcome = new Label();
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            chkRemember = new CheckBox();
            linkForgotPassword = new LinkLabel();
            btnSubmit = new Button();
            panelRight = new Panel();
            picIllustration = new PictureBox();

            panelLeft.SuspendLayout();
            panelRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picIllustration).BeginInit();
            SuspendLayout();

            // panelLeft
            panelLeft.BackColor = Color.FromArgb(0, 120, 215);
            panelLeft.Controls.Add(lblWelcome);
            panelLeft.Controls.Add(txtUsername);
            panelLeft.Controls.Add(txtPassword);
            panelLeft.Controls.Add(chkRemember);
            panelLeft.Controls.Add(linkForgotPassword);
            panelLeft.Controls.Add(btnSubmit);
            panelLeft.Dock = DockStyle.Left;
            panelLeft.Location = new Point(0, 0);
            panelLeft.Name = "panelLeft";
            panelLeft.Size = new Size(300, 400);
            panelLeft.TabIndex = 1;

            // lblWelcome
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Location = new Point(75, 50);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(150, 30);
            lblWelcome.TabIndex = 0;
            lblWelcome.Text = "BEM-VINDO";

            // txtUsername
            txtUsername.Font = new Font("Segoe UI", 10F);
            txtUsername.Location = new Point(50, 100);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Usuário";
            txtUsername.Size = new Size(200, 25);
            txtUsername.TabIndex = 1;

            // txtPassword
            txtPassword.Font = new Font("Segoe UI", 10F);
            txtPassword.Location = new Point(50, 140);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Senha";
            txtPassword.Size = new Size(200, 25);
            txtPassword.TabIndex = 2;
            txtPassword.UseSystemPasswordChar = true;

            // chkRemember
            chkRemember.AutoSize = true;
            chkRemember.Font = new Font("Segoe UI", 8F);
            chkRemember.ForeColor = Color.White;
            chkRemember.Location = new Point(50, 180);
            chkRemember.Name = "chkRemember";
            chkRemember.Size = new Size(80, 17);
            chkRemember.TabIndex = 3;
            chkRemember.Text = "Lembrar-se";

            // linkForgotPassword
            linkForgotPassword.AutoSize = true;
            linkForgotPassword.Font = new Font("Segoe UI", 8F);
            linkForgotPassword.LinkColor = Color.White;
            linkForgotPassword.Location = new Point(140, 180);
            linkForgotPassword.Name = "linkForgotPassword";
            linkForgotPassword.Size = new Size(99, 13);
            linkForgotPassword.TabIndex = 4;
            linkForgotPassword.TabStop = true;
            linkForgotPassword.Text = "Esqueceu Senha";

            // btnSubmit
            btnSubmit.BackColor = Color.FromArgb(76, 175, 80);
            btnSubmit.Cursor = Cursors.Hand;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSubmit.ForeColor = Color.White;
            btnSubmit.Location = new Point(50, 220);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(200, 35);
            btnSubmit.TabIndex = 5;
            btnSubmit.Text = "ENTRAR";
            btnSubmit.UseVisualStyleBackColor = false;
            btnSubmit.Click += BtnSubmit_Click;

            // panelRight
            panelRight.BackColor = Color.White;
            panelRight.Controls.Add(picIllustration);
            panelRight.Dock = DockStyle.Fill;
            panelRight.Location = new Point(300, 0);
            panelRight.Name = "panelRight";
            panelRight.Size = new Size(300, 400);
            panelRight.TabIndex = 0;

            // picIllustration
            picIllustration.Dock = DockStyle.Fill;
            picIllustration.Image = (Image)resources.GetObject(@"C:\Users\vaspt\OneDrive\Área de Trabalho\PIM\GestaoDChamados\GestaoDChamados\resouces\ilustration.png");
            picIllustration.Location = new Point(0, 0);
            picIllustration.Name = "picIllustration";
            picIllustration.Size = new Size(300, 400);
            picIllustration.SizeMode = PictureBoxSizeMode.Zoom;
            picIllustration.TabIndex = 0;
            picIllustration.TabStop = false;

            // LoginForm
            ClientSize = new Size(600, 400);
            Controls.Add(panelRight);
            Controls.Add(panelLeft);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            panelLeft.ResumeLayout(false);
            panelLeft.PerformLayout();
            panelRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picIllustration).EndInit();
            ResumeLayout(false);
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;

            string tipoUsuario = AutenticarUsuario(user, pass);

            if (tipoUsuario == "admin")
            {
                MessageBox.Show("Login efetuado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AdminForm adminForm = new AdminForm();
                adminForm.Show();
                this.Hide();
            }
            else if (tipoUsuario == "funcionario")
            {
                MessageBox.Show("Login funcionário feito com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FuncionarioForm funcionarioForm = new FuncionarioForm();
                funcionarioForm.Show();
                this.Hide();
            }
            else if (tipoUsuario == "usuario")
            {
                MessageBox.Show("Login funcionário feito com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UsuarioForm usuarioForm = new UsuarioForm();
                usuarioForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Usuário ou senha inválidos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private string AutenticarUsuario(string username, string password)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT tipo FROM usuarios WHERE usuario = @usuario AND senha = @senha";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("usuario", username);
                        cmd.Parameters.AddWithValue("senha", password);

                        var result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar ao banco: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

    }
}
