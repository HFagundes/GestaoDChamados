using System;
using System.Drawing;
using System.Windows.Forms;
using ChamadosApp;

namespace GestaoDChamados.Funcionario
{
    public partial class ChamadosEmAberto : Form
    {
        public ChamadosEmAberto(FuncionarioForm funcionarioForm)
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // O Form ocupa todo o espaço disponível
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(235, 234, 230);

            // Container principal com padding para evitar corte
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(30)  // margem interna
            };
            this.Controls.Add(container);

            // Cabeçalho estilizado
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(200, 200, 200)
            };
            container.Controls.Add(header);

            var lblHeader = new Label
            {
                Text = "CHAMADOS EM ABERTO",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            header.Controls.Add(lblHeader);

            // DataGridView ocupa o restante, com margin para evitar corte
            var dgvChamados = new DataGridView
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray
            };

            // Definição das colunas
            dgvChamados.Columns.Add("ID", "ID");
            dgvChamados.Columns.Add("Assunto", "Assunto");
            dgvChamados.Columns.Add("DataAbertura", "Data Abertura");
            dgvChamados.Columns.Add("Prioridade", "Prioridade");

            var btnAbrir = new DataGridViewButtonColumn
            {
                Name = "AbrirChamado",
                HeaderText = "Ações",
                Text = "Abrir",
                UseColumnTextForButtonValue = true,
                Width = 80
            };
            dgvChamados.Columns.Add(btnAbrir);

            // Exemplo de linhas
            dgvChamados.Rows.Add("#151", "Erro ao abrir app", "02/05/2025", "Média");
            dgvChamados.Rows.Add("#152", "Problema de rede", "02/05/2025", "Alta");

            // Linhas alternadas
            dgvChamados.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            container.Controls.Add(dgvChamados);
        }
    }
}
