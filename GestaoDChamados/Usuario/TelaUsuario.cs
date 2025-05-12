using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using Npgsql;  // Para realizar a conexão com o banco de dados PostgreSQL
using AtendeAI;
using GestaoDChamados.Usuario.ChatBots.chatgpt;

namespace GestaoDChamados.Usuario
{
    public class UsuarioForm : Form
    {
        private string usuarioAutenticado;
        private Panel sidebar, header, mainContent;
        private Label lblTitulo;
        private string connectionString = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

        public UsuarioForm(string usuario)
        {
            this.usuarioAutenticado = usuario;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "atende.AI";
            Size = new Size(1920, 1080);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            BackColor = Color.DarkGray;

            sidebar = CriarSidebar();
            header = CriarHeader();

            mainContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.DarkGray,
            };

            Controls.Add(mainContent);
            Controls.Add(header);
            Controls.Add(sidebar);

            CriarMainContent();
        }

        private Panel CriarSidebar()
        {
            var panel = new Panel
            {
                Width = 200,
                BackColor = Color.Black,
                Dock = DockStyle.Left,
            };

            string[] botoes = { "MENU", "AJUDA.AI", "CRIAR CHAMADO", "MEUS CHAMADOS" };

            for (int i = botoes.Length - 1; i >= 0; i--)
            {
                string nome = botoes[i];
                var btn = new Button
                {
                    Text = nome,
                    Height = 45,
                    Dock = DockStyle.Top,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                btn.FlatAppearance.BorderSize = 0;

                btn.MouseEnter += (s, e) =>
                {
                    btn.BackColor = Color.White;
                    btn.ForeColor = Color.Black;
                };
                btn.MouseLeave += (s, e) =>
                {
                    btn.BackColor = Color.Black;
                    btn.ForeColor = Color.White;
                };

                if (nome == "CRIAR CHAMADO")
                {
                    btn.Click += (s, e) => NavigateToCriarChamado();
                }
                else if (nome == "AJUDA.AI")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        var chatForm = new ChatForm(this)
                        {
                            TopLevel = false,
                            Dock = DockStyle.Fill
                        };
                        mainContent.Controls.Add(chatForm);
                        chatForm.Show();
                    };
                }
                else if (nome == "MENU")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        CriarMainContent();
                    };
                }
                else if (nome == "MEUS CHAMADOS")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();

                        // Passando o usuarioAutenticado diretamente para o MeusChamados
                        var chatForm = new MeusChamados(this.usuarioAutenticado)
                        {
                            TopLevel = false,
                            Dock = DockStyle.Fill
                        };

                        mainContent.Controls.Add(chatForm);
                        chatForm.Show();
                    };
                }
                panel.Controls.Add(btn);
            }

            var spacer = new Panel { Height = 50, Dock = DockStyle.Top };
            panel.Controls.Add(spacer);

            var logo = new PictureBox
            {
                Image = Image.FromFile("resources/atendeai.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Height = 90,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(logo);

            return panel;
        }

        private Panel CriarHeader()
        {
            var panel = new Panel
            {
                Height = 60,
                BackColor = Color.DarkGray,
                Dock = DockStyle.Top
            };

            lblTitulo = new Label
            {
                Text = "Painel do Usuário",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 60),
                AutoSize = true,
                Location = new Point(10, 15)
            };

            panel.Controls.Add(lblTitulo);
            return panel;
        }

        private void CriarMainContent()
        {
            mainContent.Controls.Clear();

            var painel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.DarkGray,
                Padding = new Padding(30)
            };

            // Logo e nome da empresa
            var logo = new PictureBox
            {
                Image = Image.FromFile("resources/metacorp.png"),
                Size = new Size(100, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(20, 20)
            };
            painel.Controls.Add(logo);

            var lblEmpresa = new Label
            {
                Text = "META CORP",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 198, 96, 21),
                Location = new Point(140, 45),
                AutoSize = true
            };
            painel.Controls.Add(lblEmpresa);

            // Pegar o nome e cargo do usuário do banco de dados
            var (nomeUsuario, cargoUsuario) = GetUsuarioInfo(usuarioAutenticado);

            // Boas-vindas
            var lblBemVindo = new Label
            {
                Text = $"Bem-vindo {nomeUsuario} ({cargoUsuario})",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.Black,
                Location = new Point(20, 140),
                AutoSize = true
            };
            painel.Controls.Add(lblBemVindo);

            // Avisos
            var lblAviso = new Label
            {
                Text = "Avisos: Nenhum aviso no momento.",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.DimGray,
                Location = new Point(20, 180),
                AutoSize = true
            };
            painel.Controls.Add(lblAviso);

            // Cards de resumo
            string[] titulos = { "Abertos", "Em Andamento", "Resolvidos", "Vencidos" };
            int[] valores = { 4, 2, 10, 1 };
            Color[] cores = { Color.Blue, Color.Gold, Color.Green, Color.Red };

            for (int i = 0; i < titulos.Length; i++)
            {
                var card = new Panel
                {
                    Size = new Size(160, 80),
                    BackColor = cores[i],
                    Location = new Point(20 + (i * 170), 230),
                    BorderStyle = BorderStyle.FixedSingle
                };

                var lblTitulo = new Label
                {
                    Text = titulos[i],
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.Black,
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                var lblValor = new Label
                {
                    Text = valores[i].ToString(),
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.Black,
                    Location = new Point(10, 35),
                    AutoSize = true
                };

                card.Controls.Add(lblTitulo);
                card.Controls.Add(lblValor);
                painel.Controls.Add(card);
            }

            mainContent.Controls.Add(painel);
        }

        // Função para buscar o nome e cargo do usuário no banco
        private (string, string) GetUsuarioInfo(string usuarioAutenticado)
        {
            string nome = string.Empty;
            string cargo = string.Empty;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT nome, cargo FROM usuarios WHERE usuario = @usuario", conn))
                    {
                        cmd.Parameters.AddWithValue("usuario", usuarioAutenticado);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nome = reader.GetString(0); // Nome do usuário
                                cargo = reader.GetString(1); // Cargo do usuário
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar usuário no banco: {ex.Message}");
            }

            return (nome, cargo);
        }

        public void NavigateToCriarChamado()
        {
            mainContent.Controls.Clear();
            var formChamado = new CriarChamadoForm(this.usuarioAutenticado)
            {
                TopLevel = false,
                Dock = DockStyle.Fill
            };
            mainContent.Controls.Add(formChamado);
            formChamado.Show();
        }
    }
}
