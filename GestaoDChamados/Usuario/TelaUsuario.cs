using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using GestaoDChamados.Usuario.ChatBots.chatgpt;
using AtendeAI;
using ChamadosApp; // para LoginForm

namespace GestaoDChamados.Usuario
{
    public class UsuarioForm : Form
    {
        private Panel sidebar, header, mainContent;
        private Label lblTitulo;

        public UsuarioForm()
        {
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

                        var painelChamados = new Panel
                        {
                            Dock = DockStyle.Fill,
                            BackColor = Color.White,
                            Padding = new Padding(20)
                        };

                        var lblTitulo = new Label
                        {
                            Text = "Meus Chamados",
                            Font = new Font("Segoe UI", 16, FontStyle.Bold),
                            ForeColor = Color.Black,
                            AutoSize = true,
                            Location = new Point(10, 10)
                        };
                        painelChamados.Controls.Add(lblTitulo);

                        var grid = new DataGridView
                        {
                            Location = new Point(10, 50),
                            Width = mainContent.Width - 40,
                            Height = mainContent.Height - 100,
                            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                            ReadOnly = true,
                            AllowUserToAddRows = false,
                            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                            BackgroundColor = Color.White
                        };

                        grid.Columns.Add("ID", "ID");
                        grid.Columns.Add("Titulo", "Título");
                        grid.Columns.Add("Status", "Status");
                        grid.Columns.Add("Data", "Data de Abertura");

                        grid.Rows.Add("001", "Problema no login", "Aberto", "10/05/2025");
                        grid.Rows.Add("002", "Erro ao imprimir relatório", "Em andamento", "11/05/2025");
                        grid.Rows.Add("003", "Sistema lento", "Resolvido", "09/05/2025");

                        painelChamados.Controls.Add(grid);
                        mainContent.Controls.Add(painelChamados);
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

            // Botão de logout
            var btnLogout = new Button
            {
                Text = "SAIR",
                Height = 45,
                Dock = DockStyle.Bottom,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.MouseEnter += (s, e) =>
            {
                btnLogout.BackColor = Color.White;
                btnLogout.ForeColor = Color.Black;
            };
            btnLogout.MouseLeave += (s, e) =>
            {
                btnLogout.BackColor = Color.Black;
                btnLogout.ForeColor = Color.White;
            };
            btnLogout.Click += (s, e) =>
            {
                var loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            };
            panel.Controls.Add(btnLogout);

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
                BackColor = Color.White,
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

            string nomeUsuario = "João da Silva";
            string cargoUsuario = "Analista de Suporte";

            var lblBemVindo = new Label
            {
                Text = $"Bem-vindo {nomeUsuario} ({cargoUsuario})",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.Black,
                Location = new Point(20, 140),
                AutoSize = true
            };
            painel.Controls.Add(lblBemVindo);

            var lblAviso = new Label
            {
                Text = "Avisos: Nenhum aviso no momento.",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.DimGray,
                Location = new Point(20, 180),
                AutoSize = true
            };
            painel.Controls.Add(lblAviso);

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

                var lblCardTitulo = new Label
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

                card.Controls.Add(lblCardTitulo);
                card.Controls.Add(lblValor);
                painel.Controls.Add(card);
            }

            mainContent.Controls.Add(painel);
        }

        public void NavigateToCriarChamado()
        {
            mainContent.Controls.Clear();
            var formChamado = new CriarChamadoForm
            {
                TopLevel = false,
                Dock = DockStyle.Fill
            };
            mainContent.Controls.Add(formChamado);
            formChamado.Show();
        }
    }
}