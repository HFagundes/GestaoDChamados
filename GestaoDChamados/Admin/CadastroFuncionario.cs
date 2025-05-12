using System;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace GestaoDChamados.Admin
{
    public partial class CadastroFuncionario : Form
    {
        private TextBox txtNome, txtUsuario, txtSenha, txtCargo;
        private Button btnSalvar;
        private Panel container;

        public CadastroFuncionario()
        {
            InitializeComponent();

            // Remove borda e barra de título internas
            this.FormBorderStyle = FormBorderStyle.None;
            this.ControlBox = false;
            this.Text = string.Empty;

            // Mantém maximizado para preencher todo o container/MDI
            this.WindowState = FormWindowState.Maximized;

            CriarInterfaceUsuario();
        }

        private void CriarInterfaceUsuario()
        {
            this.BackColor = Color.DarkGray;

            container = new Panel
            {
                Size = new Size(500, 350),
                BackColor = Color.FromArgb(240, 240, 240),
                Location = new Point((ClientSize.Width - 500) / 2, 100),
                Anchor = AnchorStyles.Top
            };

            Label CriarLabel(string texto, int y) => new Label
            {
                Text = texto,
                Location = new Point(30, y),
                Font = new Font("Segoe UI", 10),
                AutoSize = true
            };

            TextBox CriarTextBox(int y, bool isPassword = false)
            {
                var tb = new TextBox
                {
                    Location = new Point(30, y),
                    Width = 420,
                    Font = new Font("Segoe UI", 10)
                };
                if (isPassword) tb.PasswordChar = '*';
                return tb;
            }

            container.Controls.Add(CriarLabel("Nome:", 20));
            txtNome = CriarTextBox(45);
            container.Controls.Add(txtNome);

            container.Controls.Add(CriarLabel("Usuário:", 90));
            txtUsuario = CriarTextBox(115);
            container.Controls.Add(txtUsuario);

            container.Controls.Add(CriarLabel("Senha:", 160));
            txtSenha = CriarTextBox(185, isPassword: true);
            container.Controls.Add(txtSenha);

            container.Controls.Add(CriarLabel("Cargo:", 230));
            txtCargo = CriarTextBox(255);
            container.Controls.Add(txtCargo);

            btnSalvar = new Button
            {
                Text = "Salvar",
                Location = new Point(30, 300),
                Width = 420,
                Height = 35,
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            btnSalvar.MouseEnter += (s, e) => btnSalvar.BackColor = Color.DodgerBlue;
            btnSalvar.MouseLeave += (s, e) => btnSalvar.BackColor = Color.Black;
            btnSalvar.Click += BtnSalvar_Click;

            container.Controls.Add(btnSalvar);
            this.Controls.Add(container);
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            string nome = txtNome.Text;
            string usuario = txtUsuario.Text;
            string senha = txtSenha.Text;
            string cargo = txtCargo.Text;
            string tipo = "funcionario";

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(cargo))
            {
                MessageBox.Show("Preencha todos os campos.");
                return;
            }

            try
            {
                string connStr = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

                using (var conn = new NpgsqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "INSERT INTO usuarios (nome, usuario, senha, tipo, cargo) VALUES (@nome, @usuario, @senha, @tipo, @cargo)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("nome", nome);
                        cmd.Parameters.AddWithValue("usuario", usuario);
                        cmd.Parameters.AddWithValue("senha", senha);
                        cmd.Parameters.AddWithValue("tipo", tipo);
                        cmd.Parameters.AddWithValue("cargo", cargo);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Funcionário cadastrado com sucesso!");
                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar no banco: " + ex.Message);
            }
        }

        private void LimparCampos()
        {
            txtNome.Text = "";
            txtUsuario.Text = "";
            txtSenha.Text = "";
            txtCargo.Text = "";
        }
    }
}
