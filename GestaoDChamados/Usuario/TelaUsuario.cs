using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using AtendeAI;
using GestaoDChamados.Usuario.ChatBots;
using GestaoDChamados.Usuario.ChatBots.chatgpt;

namespace GestaoDChamados.Usuario
{
    public class UsuarioForm : Form
    {
        private Panel sidebar, header, mainContent;
        private Label lblTitulo, lblAbertos, lblAndamento, lblVencidos, lblResolvidos;
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
            BackColor = Color.White;

            sidebar = CriarSidebar();
            header = CriarHeader();

            mainContent = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
            };

            Controls.Add(mainContent);
            Controls.Add(header);
            Controls.Add(sidebar);

            CriarMainContent(); // Chamada inicial do conteúdo principal
        }

        private Panel CriarSidebar()
        {
            var panel = new Panel()
            {
                Width = 200,
                BackColor = Color.FromArgb(30, 60, 110),
                Dock = DockStyle.Left
            };

            Label logo = new Label()
            {
                Text = "atende.AI",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top
            };
            panel.Controls.Add(logo);

            string[] botoes = { "Meus Chamados", "Ajuda.AI", "Criar Chamado", "Menu" };

            foreach (string nome in botoes)
            {
                Button btn = new Button()
                {
                    Text = nome,
                    Height = 45,
                    Dock = DockStyle.Top,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(30, 60, 110),
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(20, 0, 0, 0)
                };
                btn.FlatAppearance.BorderSize = 0;
                panel.Controls.Add(btn);
                panel.Controls.SetChildIndex(btn, panel.Controls.Count - 2);

                // Botões com ação
                if (nome == "Criar Chamado")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        CriarChamadoForm formChamado = new CriarChamadoForm();
                        formChamado.TopLevel = false;
                        formChamado.Dock = DockStyle.Fill;
                        mainContent.Controls.Add(formChamado);
                        formChamado.Show();
                    };
                }

                if (nome == "Ajuda.AI")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        var chatForm = new ChatForm();
                        chatForm.TopLevel = false;
                        chatForm.Dock = DockStyle.Fill;
                        mainContent.Controls.Add(chatForm);
                        chatForm.Show();
                    };
                }

                if (nome == "Menu")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        CriarMainContent(); // Recria o painel principal
                    };
                }
            }

            return panel;
        }

        private Panel CriarHeader()
        {
            var panel = new Panel()
            {
                Height = 60,
                BackColor = Color.WhiteSmoke,
                Dock = DockStyle.Top
            };

            lblTitulo = new Label()
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

            Panel cardsPanel = new Panel()
            {
                Size = new Size(900, 100),
                Location = new Point((ClientSize.Width - 900) / 2, 0),
                Anchor = AnchorStyles.Top
            };

            lblAbertos = CriarCard("Chamados Abertos", "5", 0);
            lblAndamento = CriarCard("Em Andamento", "3", 200);
            lblVencidos = CriarCard("Vencidos", "1", 400);
            lblResolvidos = CriarCard("Resolvidos Hoje", "7", 600);

            cardsPanel.Controls.AddRange(new Control[] {
                lblAbertos.Tag as Panel,
                lblAndamento.Tag as Panel,
                lblVencidos.Tag as Panel,
                lblResolvidos.Tag as Panel
            });

            GroupBox gbAbertos = new GroupBox()
            {
                Text = "Chamados Abertos",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 70, 120),
                Size = new Size(550, 220),
                Location = new Point((ClientSize.Width - 550) / 2, 150),
            };

            dgvChamadosAbertos = new DataGridView()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
            };
            gbAbertos.Controls.Add(dgvChamadosAbertos);

            GroupBox gbFechados = new GroupBox()
            {
                Text = "Chamados Fechados",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 70, 120),
                Size = new Size(550, 220),
                Location = new Point((ClientSize.Width - 550) / 2, 390),
            };

            dgvVencidos = new DataGridView()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
            };
            gbFechados.Controls.Add(dgvVencidos);

            mainContent.Controls.Add(cardsPanel);
            mainContent.Controls.Add(gbAbertos);
            mainContent.Controls.Add(gbFechados);

            CarregarDados();
        }

        private Label CriarCard(string titulo, string valor, int posX)
        {
            Panel card = new Panel()
            {
                Size = new Size(200, 80),
                Location = new Point(posX, 10),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTitulo = new Label()
            {
                Text = titulo,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 70),
                Location = new Point(10, 10),
                AutoSize = true
            };

            Label lblValor = new Label()
            {
                Text = valor,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 90, 160),
                Location = new Point(10, 30),
                AutoSize = true,
                Tag = card
            };

            card.Controls.Add(lblTitulo);
            card.Controls.Add(lblValor);

            return lblValor;
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
    }
}
