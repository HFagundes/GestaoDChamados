using System;
using System.Drawing;
using System.Windows.Forms;
using ChamadosApp;

namespace GestaoDChamados.Funcionario
{
    public partial class ChamadosEncerrados : Form
    {
        private Panel container;

        public ChamadosEncerrados()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        /// <summary>
        /// Construtor usado pelo FuncionarioForm; inserção no painel ocorre fora deste form.
        /// </summary>
        public ChamadosEncerrados(FuncionarioForm funcionarioForm)
            : this()
        {
            // Será adicionado ao painel de conteúdo em FuncionarioForm
        }

        private void InitializeCustomComponents()
        {
            // Form sem bordas e cor de fundo suave
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(235, 234, 230);

            // Container centralizado e de tamanho fixo
            container = new Panel
            {
                Size = new Size(1000, 500),
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(20),
                Anchor = AnchorStyles.None
            };
            CenterContainer();
            this.Controls.Add(container);

            // Reposiciona ao redimensionar
            this.Resize += (s, e) => CenterContainer();

            // Cabeçalho cinza
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(200, 200, 200)
            };
            var lblHeader = new Label
            {
                Text = "CHAMADOS ENCERRADOS",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            header.Controls.Add(lblHeader);

            // DataGridView estilizado
            var dgv = new DataGridView
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

            // Definição de colunas
            dgv.Columns.Add("ID", "ID");
            dgv.Columns.Add("Assunto", "Assunto");
            dgv.Columns.Add("DataAbertura", "Data Abertura");
            dgv.Columns.Add("DataEncerramento", "Data Encerramento");
            dgv.Columns.Add("Prioridade", "Prioridade");
            var btnDetalhes = new DataGridViewButtonColumn
            {
                Name = "DetalhesChamado",
                HeaderText = "Ações",
                Text = "Detalhes",
                UseColumnTextForButtonValue = true
            };
            dgv.Columns.Add(btnDetalhes);

            // Ajuste de pesos para não cortar colunas
            dgv.Columns["ID"].FillWeight = 10;
            dgv.Columns["Assunto"].FillWeight = 35;
            dgv.Columns["DataAbertura"].FillWeight = 20;
            dgv.Columns["DataEncerramento"].FillWeight = 20;
            dgv.Columns["Prioridade"].FillWeight = 10;
            dgv.Columns["DetalhesChamado"].FillWeight = 5;

            // Exemplo de linhas encerradas
            dgv.Rows.Add("#142", "Erro de login", "30/04/2025", "01/05/2025", "Baixa");
            dgv.Rows.Add("#138", "Solicitação de acesso", "29/04/2025", "30/04/2025", "Média");
            dgv.Rows.Add("#134", "Falha de impressão", "28/04/2025", "28/04/2025", "Baixa");

            // Linhas alternadas
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            // Monta hierarquia dos controles
            container.Controls.Add(dgv);
            container.Controls.Add(header);
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