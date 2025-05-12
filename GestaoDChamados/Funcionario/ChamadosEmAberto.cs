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
            this.BackColor = Color.FromArgb(235, 234, 230);

            // Container Centralizado
            var container = new Panel
            {
                Size = new Size(800, 400),
                BackColor = Color.WhiteSmoke,
                Anchor = AnchorStyles.None,
            };
            container.Location = new Point(
                (this.ClientSize.Width - container.Width) / 2,
                (this.ClientSize.Height - container.Height) / 2);

            // Header estilizado
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(200, 200, 200),
            };

            var lblHeader = new Label
            {
                Text = "CHAMADOS EM ABERTO",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            header.Controls.Add(lblHeader);

            // DataGridView estilizado
            var dgvChamados = new DataGridView
            {
                Dock = DockStyle.Fill,
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

            // Coluna de Ações (botão)
            var btnAbrir = new DataGridViewButtonColumn
            {
                Name = "AbrirChamado",
                HeaderText = "Ações",
                Text = "Abrir",
                UseColumnTextForButtonValue = true,
                Width = 80
            };
            dgvChamados.Columns.Add(btnAbrir);

            // Linhas de exemplo
            dgvChamados.Rows.Add("#151", "Erro ao abrir app", "02/05/2025", "Média");
            dgvChamados.Rows.Add("#152", "Problema de rede", "02/05/2025", "Alta");

            // Linhas alternadas
            dgvChamados.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            // Aplica bordas arredondadas (simulado via Padding)
            container.Padding = new Padding(10);
            container.Controls.Add(dgvChamados);
            container.Controls.Add(header);

            this.Controls.Add(container);

            // Centraliza container ao redimensionar
            this.Resize += (s, e) =>
            {
                container.Location = new Point(
                    (this.ClientSize.Width - container.Width) / 2,
                    (this.ClientSize.Height - container.Height) / 2);
            };
        }
    }
}
