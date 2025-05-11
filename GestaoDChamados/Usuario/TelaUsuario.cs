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

        public UsuarioForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "atende.AI";
            Size = new Size(1920, 1080);
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

            var logo = new Label
            {
                Text = "ATENDE.AI",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top
            };
            panel.Controls.Add(logo);

            string[] botoes = { "MEUS CHAMADOS", "AJUDA.AI", "CRIAR CHAMADO", "MENU" };
            foreach (string nome in botoes)
            {
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

                panel.Controls.Add(btn);
                panel.Controls.SetChildIndex(btn, panel.Controls.Count - 2);

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
            }

            var linhaBranca = new Panel
            {
                Width = 2,
                Dock = DockStyle.Right,
                BackColor = Color.White
            };
            panel.Controls.Add(linhaBranca);

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

            var cardsPanel = new Panel
            {
                Size = new Size(900, 100),
                Location = new Point((ClientSize.Width - 900) / 2, 0),
                Anchor = AnchorStyles.Top
            };

            lblAbertos = CriarCard("Chamados Abertos", "5", 0);
            lblAndamento = CriarCard("Em Andamento", "3", 200);
            lblVencidos = CriarCard("Vencidos", "1", 400);
            lblResolvidos = CriarCard("Resolvidos Hoje", "7", 600);

            cardsPanel.Controls.Add((lblAbertos.Tag as Panel));
            cardsPanel.Controls.Add((lblAndamento.Tag as Panel));
            cardsPanel.Controls.Add((lblVencidos.Tag as Panel));
            cardsPanel.Controls.Add((lblResolvidos.Tag as Panel));

            var gbAbertos = new GroupBox
            {
                Text = "Chamados Abertos",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 70, 120),
                Size = new Size(550, 220),
                Location = new Point((ClientSize.Width - 550) / 2, 150)
            };
            dgvChamadosAbertos = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            gbAbertos.Controls.Add(dgvChamadosAbertos);

            var gbFechados = new GroupBox
            {
                Text = "Chamados Fechados",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 70, 120),
                Size = new Size(550, 220),
                Location = new Point((ClientSize.Width - 550) / 2, 390)
            };
            dgvVencidos = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            gbFechados.Controls.Add(dgvVencidos);

            mainContent.Controls.Add(cardsPanel);
            mainContent.Controls.Add(gbAbertos);
            mainContent.Controls.Add(gbFechados);

            CarregarDados();
        }

        private Label CriarCard(string titulo, string valor, int posX)
        {
            Color bgColor;
            Color textColor;
            switch (titulo)
            {
                case "Chamados Abertos":
                    bgColor = ColorTranslator.FromHtml("#E3F2FD");
                    textColor = ColorTranslator.FromHtml("#1976D2");
                    break;
                case "Em Andamento":
                    bgColor = ColorTranslator.FromHtml("#FFF8E1");
                    textColor = ColorTranslator.FromHtml("#FFA000");
                    break;
                case "Vencidos":
                    bgColor = ColorTranslator.FromHtml("#FFEBEE");
                    textColor = ColorTranslator.FromHtml("#D32F2F");
                    break;
                case "Resolvidos Hoje":
                    bgColor = ColorTranslator.FromHtml("#E8F5E9");
                    textColor = ColorTranslator.FromHtml("#388E3C");
                    break;
                default:
                    bgColor = Color.White;
                    textColor = Color.Black;
                    break;
            }

            var card = new Panel
            {
                Size = new Size(200, 80),
                Location = new Point(posX, 10),
                BackColor = bgColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblTitle = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 70),
                Location = new Point(10, 10),
                AutoSize = true
            };
            card.Controls.Add(lblTitle);

            var lblValue = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = textColor,
                Location = new Point(10, 35),
                AutoSize = true,
                Tag = card
            };
            card.Controls.Add(lblValue);

            return lblValue;
        }

        private void CarregarDados()
        {
            var tabela1 = new DataTable();
            tabela1.Columns.Add("ID");
            tabela1.Columns.Add("Assunto");
            tabela1.Columns.Add("Data Abertura");
            tabela1.Columns.Add("Prioridade");
            tabela1.Rows.Add("#152", "Problema de rede", "02/05/2025", "Alta");
            tabela1.Rows.Add("#151", "Erro ao abrir aplicação", "02/05/2025", "Média");
            dgvChamadosAbertos.DataSource = tabela1;

            var tabela2 = new DataTable();
            tabela2.Columns.Add("ID");
            tabela2.Columns.Add("Assunto");
            tabela2.Columns.Add("Data Abertura");
            tabela2.Columns.Add("Prioridade");
            tabela2.Rows.Add("#142", "Erro de login", "30/04/2025", "Baixa");
            tabela2.Rows.Add("#138", "Solicitação concluída", "29/04/2025", "Média");
            dgvVencidos.DataSource = tabela2;
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
