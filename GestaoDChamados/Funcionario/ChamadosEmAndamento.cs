using System;
using System.Drawing;
using System.Windows.Forms;
using ChamadosApp;

namespace GestaoDChamados.Funcionario
{
    public partial class ChamadosEmAndamento : Form
    {
        private Panel container;

        public ChamadosEmAndamento()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        /// <summary>
        /// Construtor simplificado; a adição ao painel é feita no FuncionarioForm.
        /// </summary>
        public ChamadosEmAndamento(FuncionarioForm funcionarioForm)
            : this()
        {
            // Abertura no painel ocorre em FuncionarioForm
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(235, 234, 230);
            this.FormBorderStyle = FormBorderStyle.None;

            // Container fixo e centralizado
            container = new Panel
            {
                Size = new Size(1000, 500),
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(20),
                Anchor = AnchorStyles.None
            };
            CenterContainer();
            this.Controls.Add(container);

            // Header
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(200, 200, 200)
            };
            var lblHeader = new Label
            {
                Text = "CHAMADOS EM ANDAMENTO",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            header.Controls.Add(lblHeader);

            // Grid
            var dgvChamados = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            // Colunas
            dgvChamados.Columns.Add("ID", "ID");
            dgvChamados.Columns.Add("Assunto", "Assunto");
            dgvChamados.Columns.Add("DataAbertura", "Data Abertura");
            dgvChamados.Columns.Add("Prioridade", "Prioridade");
            var btnVer = new DataGridViewButtonColumn
            {
                Name = "VerChamado",
                HeaderText = "Ações",
                Text = "Ver",
                UseColumnTextForButtonValue = true
            };
            dgvChamados.Columns.Add(btnVer);
            // Pesos
            dgvChamados.Columns["ID"].FillWeight = 15;
            dgvChamados.Columns["Assunto"].FillWeight = 40;
            dgvChamados.Columns["DataAbertura"].FillWeight = 25;
            dgvChamados.Columns["Prioridade"].FillWeight = 15;
            dgvChamados.Columns["VerChamado"].FillWeight = 5;

            // Exemplo de linhas
            dgvChamados.Rows.Add("#153", "Erro no módulo X", "03/05/2025", "Alta");
            dgvChamados.Rows.Add("#154", "Lentidão no sistema", "03/05/2025", "Média");
            dgvChamados.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            // Adiciona ao container (header antes do grid)
            container.Controls.Add(dgvChamados);
            container.Controls.Add(header);

            // Reposiciona ao redimensionar
            this.Resize += (s, e) => CenterContainer();
        }

        private void CenterContainer()
        {
            if (container == null) return;
            container.Location = new Point(
                (this.ClientSize.Width - container.Width) / 2,
                (this.ClientSize.Height - container.Height) / 2);
        }
    }
}
