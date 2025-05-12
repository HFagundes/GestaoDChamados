using System;
using System.Drawing;
using System.Windows.Forms;
using GestaoDChamados.Funcionario;
using ChamadosApp; // para LoginForm

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

        // offsets de posicionamento
        private const int MetricsTopOffset = 80;
        private const int GridsVerticalSpacing = 40;
        private const int HorizontalExtraOffset = 50;

        public FuncionarioForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // form
            this.Text = "PAINEL DO FUNCIONARIO";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.BackColor = Color.DarkGray;

            // header
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

            // sidebar
            sidebar = CriarSidebar();
            this.Controls.Add(sidebar);

            // mainContent
            mainContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Gray,
                AutoScroll = true
            };
            this.Controls.Add(mainContent);

            // dashboard inicial
            AdicionarDashboard();

            // centraliza ao carregar e ao redimensionar
            this.Load += (s, e) => AjustarPosicoes();
            this.Resize += (s, e) => AjustarPosicoes();
        }

        private Panel CriarSidebar()
        {
            var panel = new Panel
            {
                Width = 240,
                BackColor = Color.Black,
                Dock = DockStyle.Left
            };

            string[] botoes = {
                "MENU",
                "CHAMADOS EM ABERTO",
                "CHAMADOS EM ANDAMENTO",
                "CHAMADOS ENCERRADOS"
            };

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
                btn.MouseEnter += (s, e) => { btn.BackColor = Color.White; btn.ForeColor = Color.Black; };
                btn.MouseLeave += (s, e) => { btn.BackColor = Color.Black; btn.ForeColor = Color.White; };

                // associa ação de clique conforme o nome
                if (nome == "MENU")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        AdicionarDashboard();
                        AjustarPosicoes();
                    };
                }
                else if (nome == "CHAMADOS EM ABERTO")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        var f = new ChamadosEmAberto(this)
                        {
                            TopLevel = false,
                            Dock = DockStyle.Fill
                        };
                        mainContent.Controls.Add(f);
                        f.Show();
                    };
                }
                else if (nome == "CHAMADOS EM ANDAMENTO")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        var f = new ChamadosEmAndamento(this)
                        {
                            TopLevel = false,
                            Dock = DockStyle.Fill
                        };
                        mainContent.Controls.Add(f);
                        f.Show();
                    };
                }
                else if (nome == "CHAMADOS ENCERRADOS")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        var f = new ChamadosEncerrados(this)
                        {
                            TopLevel = false,
                            Dock = DockStyle.Fill
                        };
                        mainContent.Controls.Add(f);
                        f.Show();
                    };
                }

                panel.Controls.Add(btn);
            }

            // espaço e logo
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

            // Botão SAIR fixo ao fundo
            var btnSair = new Button
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
            btnSair.FlatAppearance.BorderSize = 0;
            btnSair.MouseEnter += (s, e) => { btnSair.BackColor = Color.White; btnSair.ForeColor = Color.Black; };
            btnSair.MouseLeave += (s, e) => { btnSair.BackColor = Color.Black; btnSair.ForeColor = Color.White; };
            btnSair.Click += (s, e) =>
            {
                // Fecha o formulário atual
                this.Close();

                // Abre o formulário de login novamente
                var loginForm = new LoginForm();  // Supondo que seja esse o nome do formulário de login
                loginForm.Show();
            };
            panel.Controls.Add(btnSair);



            return panel;
        }

        private void AdicionarDashboard()
        {
            // painel de métricas
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

            metricsPanel.Controls.Add(CriarMetricCard("Chamados Abertos", "5", Color.FromArgb(0x21, 0x96, 0xF3)));
            metricsPanel.Controls.Add(CriarMetricCard("Em Andamento", "3", Color.FromArgb(0xFF, 0x98, 0x00)));
            metricsPanel.Controls.Add(CriarMetricCard("Vencidos", "1", Color.FromArgb(0xF4, 0x43, 0x36)));
            metricsPanel.Controls.Add(CriarMetricCard("Resolvidos Hoje", "7", Color.FromArgb(0x4C, 0xAF, 0x50)));

            // painel de grids
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

            // grupos de chamados
            var grpA = new GroupBox
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
            grpA.Controls.Add(dgvChamadosAbertos);
            InicializarGridChamadosAbertos();
            gridsPanel.Controls.Add(grpA);

            var grpF = new GroupBox
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
            grpF.Controls.Add(dgvChamadosFechados);
            InicializarGridChamadosFechados();
            gridsPanel.Controls.Add(grpF);
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

            var lblT = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblT);

            var lblV = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblV);

            return card;
        }

        private void AjustarPosicoes()
        {
            // cards
            int leftM = (mainContent.ClientSize.Width - metricsPanel.Width) / 2 + HorizontalExtraOffset;
            metricsPanel.Top = MetricsTopOffset;
            metricsPanel.Left = Math.Max(0, leftM);

            // grids
            int topG = metricsPanel.Bottom + GridsVerticalSpacing;
            int leftG = (mainContent.ClientSize.Width - gridsPanel.Width) / 2 + HorizontalExtraOffset;
            gridsPanel.Top = topG;
            gridsPanel.Left = Math.Max(0, leftG);
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
