using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChamadosApp
{
    public class FuncionarioForm : Form
    {
        private Panel sidebar;
        private Panel header;
        private Panel mainContent;

        private FlowLayoutPanel metricsPanel;
        private FlowLayoutPanel gridsPanel;
        private DataGridView dgvChamadosAbertos;
        private DataGridView dgvChamadosFechados;

        // Ajustes de posicionamento
        private const int MetricsTopOffset = 80;
        private const int GridsVerticalSpacing = 40;
        private const int HorizontalExtraOffset = 50; // deslocamento extra à direita

        public FuncionarioForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            this.Text = "PAINEL DO FUNCIONARIO";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.BackColor = Color.DarkGray;

            // Header
            header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.DarkGray
            };
            this.Controls.Add(header);

            var lblTitulo = new Label
            {
                Text = "PAINEL DO FUNCIONARIO",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Black,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            header.Controls.Add(lblTitulo);

            // Sidebar
            sidebar = CriarSidebar();
            this.Controls.Add(sidebar);

            // MainContent
            mainContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Gray,
                AutoScroll = true
            };
            this.Controls.Add(mainContent);

            // Monta o dashboard
            AdicionarDashboard();

            // Reposiciona no load e no resize
            this.Load += (s, e) => AjustarPosicoes();
            this.Resize += (s, e) => AjustarPosicoes();
        }

        private Panel CriarSidebar()
        {
            var panel = new Panel
            {
                Width = 240,
                BackColor = Color.Black,
                Dock = DockStyle.Left,
            };

            string[] botoes = { "MENU", "CHAMADOS EM ABERTO", "CHAMADOS EM ANDAMENTO", "CHAMADOS ENCERRADOS" };
            for (int i = botoes.Length - 1; i >= 0; i--)
            {
                var btn = new Button
                {
                    Text = botoes[i],
                    Height = 45,
                    Dock = DockStyle.Top,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.MouseEnter += (s, e) => { btn.BackColor = Color.White; btn.ForeColor = Color.Black; };
                btn.MouseLeave += (s, e) => { btn.BackColor = Color.Black; btn.ForeColor = Color.White; };
                panel.Controls.Add(btn);
            }

            panel.Controls.Add(new Panel { Height = 50, Dock = DockStyle.Top });
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

        private void AdicionarDashboard()
        {
            // --- Painel de Métricas ---
            metricsPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(5),
                BackColor = Color.Transparent
            };
            mainContent.Controls.Add(metricsPanel);

            metricsPanel.Controls.Add(CriarMetricCard("Chamados Abertos",  "5", Color.FromArgb(0x21,0x96,0xF3)));
            metricsPanel.Controls.Add(CriarMetricCard("Em Andamento",      "3", Color.FromArgb(0xFF,0x98,0x00)));
            metricsPanel.Controls.Add(CriarMetricCard("Vencidos",          "1", Color.FromArgb(0xF4,0x43,0x36)));
            metricsPanel.Controls.Add(CriarMetricCard("Resolvidos Hoje",   "7", Color.FromArgb(0x4C,0xAF,0x50)));

            // --- Painel de Grids ---
            gridsPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(5),
                BackColor = Color.Transparent
            };
            mainContent.Controls.Add(gridsPanel);

            // Chamados Abertos
            var grpAbertos = new GroupBox
            {
                Text = "Chamados Abertos",
                Font = new Font("Segoe UI", 10),
                Width = 800,
                Height = 240
            };
            dgvChamadosAbertos = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grpAbertos.Controls.Add(dgvChamadosAbertos);
            InicializarGridChamadosAbertos();
            gridsPanel.Controls.Add(grpAbertos);

            // Chamados Fechados
            var grpFechados = new GroupBox
            {
                Text = "Chamados Fechados",
                Font = new Font("Segoe UI", 10),
                Width = 800,
                Height = 240
            };
            dgvChamadosFechados = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grpFechados.Controls.Add(dgvChamadosFechados);
            InicializarGridChamadosFechados();
            gridsPanel.Controls.Add(grpFechados);
        }

        private Panel CriarMetricCard(string titulo, string valor, Color bgColor)
        {
            var card = new Panel
            {
                Size = new Size(200, 100),
                BackColor = bgColor,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(10)
            };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblTitulo);

            var lblValor = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblValor);

            return card;
        }

        private void AjustarPosicoes()
        {
            // Calcula posição dos cards
            int leftMetrics = (mainContent.ClientSize.Width - metricsPanel.Width) / 2 + HorizontalExtraOffset;
            metricsPanel.Top = MetricsTopOffset;
            metricsPanel.Left = Math.Max(0, leftMetrics);

            // Calcula posição dos grids
            int topGrids = metricsPanel.Bottom + GridsVerticalSpacing;
            int leftGrids = (mainContent.ClientSize.Width - gridsPanel.Width) / 2 + HorizontalExtraOffset;
            gridsPanel.Top = topGrids;
            gridsPanel.Left = Math.Max(0, leftGrids);
        }

        private void InicializarGridChamadosAbertos()
        {
            dgvChamadosAbertos.Columns.Add("ID", "ID");
            dgvChamadosAbertos.Columns.Add("Assunto", "Assunto");
            dgvChamadosAbertos.Columns.Add("DataAbertura", "Data Abertura");
            dgvChamadosAbertos.Columns.Add("Prioridade", "Prioridade");

            dgvChamadosAbertos.Rows.Add("#152", "Problema de rede", "02/05/2025", "Alta");
            dgvChamadosAbertos.Rows.Add("#151", "Erro ao abrir app", "02/05/2025", "Média");
        }

        private void InicializarGridChamadosFechados()
        {
            dgvChamadosFechados.Columns.Add("ID", "ID");
            dgvChamadosFechados.Columns.Add("Assunto", "Assunto");
            dgvChamadosFechados.Columns.Add("DataAbertura", "Data Abertura");
            dgvChamadosFechados.Columns.Add("Prioridade", "Prioridade");

            dgvChamadosFechados.Rows.Add("#142", "Erro de login", "30/04/2025", "Baixa");
            dgvChamadosFechados.Rows.Add("#138", "Solicitação de acesso", "29/04/2025", "Média");
        }
    }
}
