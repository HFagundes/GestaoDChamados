using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using GestaoDChamados.Usuario.ChatBots.chatgpt;
using AtendeAI;

namespace GestaoDChamados.Usuario
{
    public class UsuarioForm : Form
    {
        private Panel sidebar, header, mainContent;
        private Label lblTitulo;
        private Label lblAbertos, lblAndamento, lblVencidos, lblResolvidos;
        private DataGridView dgvChamadosAbertos, dgvVencidos;

        public UsuarioForm(int id)
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

            // Lista de botões (em ordem normal)
            string[] botoes = {"MENU", "AJUDA.AI", "CRIAR CHAMADO", "MEUS CHAMADOS" };

            // Adiciona os botões de baixo para cima
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
                    TextAlign = ContentAlignment.MiddleCenter,
                    Padding = new Padding(0)
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

                panel.Controls.Add(btn); // Adiciona do último ao primeiro (vai aparecendo de baixo pra cima)
            }

            // Espaço entre logo e botões
            var spacer = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top
            };
            panel.Controls.Add(spacer);

            // Logo no topo
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

    // Painel centralizado
    var painelCentral = new Panel
    {
        Size = new Size(600, 300),
        Location = new Point((ClientSize.Width - 600) / 2, 50),
        Anchor = AnchorStyles.Top
    };

    // Logo da empresa
    var logo = new PictureBox
    {
        Image = Image.FromFile("resources/metacorp.png"), // ajuste o caminho se necessário
        SizeMode = PictureBoxSizeMode.Zoom,
        Size = new Size(100, 100),
        Location = new Point(0, 0)
    };
    painelCentral.Controls.Add(logo);

    // Nome da empresa ao lado da logo
    var lblEmpresa = new Label
    {
        Text = "META CORP",
        Font = new Font("Segoe UI", 24, FontStyle.Bold),
        ForeColor = Color.FromArgb(40, 70, 120),
        AutoSize = true,
        Location = new Point(logo.Right + 20, 30)
    };
    painelCentral.Controls.Add(lblEmpresa);

    // Simulação de nome e cargo do usuário
    string nomeUsuario = "João da Silva";
    string cargoUsuario = "Analista de Suporte";

    var lblBemVindo = new Label
    {
        Text = $"Bem-vindo {nomeUsuario} ({cargoUsuario})",
        Font = new Font("Segoe UI", 12, FontStyle.Regular),
        ForeColor = Color.Black,
        AutoSize = true,
        Location = new Point(0, logo.Bottom + 20)
    };
    painelCentral.Controls.Add(lblBemVindo);

    // Avisos
    string aviso = ""; // aqui você pode buscar os avisos dinamicamente
    var lblAviso = new Label
    {
        Text = "Avisos: " + (string.IsNullOrWhiteSpace(aviso) ? "Sem avisos" : aviso),
        Font = new Font("Segoe UI", 10, FontStyle.Italic),
        ForeColor = Color.DarkGray,
        AutoSize = true,
        Location = new Point(0, lblBemVindo.Bottom + 15)
    };
    painelCentral.Controls.Add(lblAviso);

    mainContent.Controls.Add(painelCentral);
}


        /// <summary>
        /// Navega programaticamente para a aba "Criar Chamado" dentro do painel principal.
        /// </summary>
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
