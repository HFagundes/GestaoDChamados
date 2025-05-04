using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

namespace AtendeAI
{
    public class UsuarioForm : Form
    {
        private Panel sidebar, header, mainContent;
        private Label lblTitulo, lblAbertos, lblAndamento, lblVencidos, lblResolvidos;
        private DataGridView dgvChamadosAbertos, dgvVencidos;

        public UsuarioForm()
        {
            // Inicializa as variáveis
            sidebar = new Panel();
            header = new Panel();
            mainContent = new Panel();
            lblTitulo = new Label();
            lblAbertos = new Label();
            lblAndamento = new Label();
            lblResolvidos = new Label();
            lblVencidos = new Label();
            dgvChamadosAbertos = new DataGridView();
            dgvVencidos = new DataGridView();

            // Configurações do Formulário
            InitializeComponent();
        }

        private void InitializeComponent()
        {

            this.Text = "atende.AI"; // Título da janela
            this.Size = new Size(1200, 700); // Tamanho do formulário
            this.StartPosition = FormStartPosition.CenterScreen; // Posição inicial no centro da tela
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Não permite maximizar o formulário
            this.MaximizeBox = false; // Desabilita a maximização

            CriarSidebar();
            CriarHeader();
            CriarMainContent();  // Chama antes de CarregarDados
            CarregarDados();     // Agora, os dados serão carregados corretamente
        }

        private void CriarSidebar()
        {
            sidebar = new Panel()
            {
                Size = new Size(200, this.Height),
                BackColor = Color.FromArgb(20, 40, 80),
                Dock = DockStyle.Left
            };

            Label logo = new Label()
            {
                Text = "atende.AI",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };
            sidebar.Controls.Add(logo);

            string[] botoes = {
                "Meus Chamados", "Ajuda.AI", "Criar Chamado"
            };

            foreach (string nome in botoes)
            {
                Button btn = new Button()
                {
                    Text = nome,
                    Height = 40,
                    Dock = DockStyle.Top,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(20, 40, 80),
                    Font = new Font("Segoe UI", 10, FontStyle.Regular)
                };
                btn.FlatAppearance.BorderSize = 0;
                sidebar.Controls.Add(btn);
                sidebar.Controls.SetChildIndex(btn, sidebar.Controls.Count - 2);
            }

            this.Controls.Add(sidebar);
        }

        private void CriarHeader()
        {
            header = new Panel()
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.White
            };

            lblTitulo = new Label()
            {
                Text = "atende.AI",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 100),
                AutoSize = true,
                Location = new Point(220, 15)
            };

            header.Controls.Add(lblTitulo);
            this.Controls.Add(header);
        }

        private void CriarMainContent()
        {
            mainContent = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Cards superiores
            lblAbertos = CriarCard("Chamados Abertos", "5", 220);
            lblAndamento = CriarCard("Em Andamento", "3", 430);
            lblVencidos = CriarCard("Vencidos", "1", 640);
            lblResolvidos = CriarCard("Resolvidos Hoje", "7", 850);

            // Inicialização das Tabelas (DataGridView)
            dgvChamadosAbertos = new DataGridView()
            {
                Location = new Point(220, 120),
                Size = new Size(450, 200),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvVencidos = new DataGridView()
            {
                Location = new Point(690, 120),
                Size = new Size(450, 200),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Agora podemos adicionar as tabelas ao painel principal
            mainContent.Controls.AddRange(new Control[] { dgvChamadosAbertos, dgvVencidos });

            // Após adicionar as tabelas ao painel, adiciona o painel ao formulário
            this.Controls.Add(mainContent);

            // Carregar dados nas tabelas
            CarregarDados();
        }

        private Label CriarCard(string titulo, string valor, int posX)
        {
            Panel card = new Panel()
            {
                Size = new Size(200, 70),
                Location = new Point(posX, 40),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTitulo = new Label()
            {
                Text = titulo,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            Label lblValor = new Label()
            {
                Text = valor,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(10, 30),
                AutoSize = true
            };

            card.Controls.Add(lblTitulo);
            card.Controls.Add(lblValor);
            mainContent.Controls.Add(card);

            return lblValor; // Retorna o Label lblValor para ser usado como referencia para lblAbertos, etc.
        }

        private void CarregarDados()
        {
            // Dados para a tabela de chamados abertos
            var tabela1 = new DataTable();
            tabela1.Columns.Add("ID");
            tabela1.Columns.Add("Assunto");
            tabela1.Columns.Add("Data Abertura");
            tabela1.Columns.Add("Prioridade");
            tabela1.Rows.Add("#152", "Problema de rede", "02/05/2025", "Alta");
            tabela1.Rows.Add("#151", "Erro ao abrir aplicação", "02/05/2025", "Média");

            dgvChamadosAbertos.DataSource = tabela1;

            // Dados para a tabela de chamados vencidos
            var tabela2 = new DataTable();
            tabela2.Columns.Add("ID");
            tabela2.Columns.Add("Assunto");
            tabela2.Columns.Add("Dias em Atraso");
            tabela2.Columns.Add("Prioridade");
            tabela2.Rows.Add("#117", "Acesso negado", "2", "Alta");

            // Atribuindo a tabela ao DataGridView
            dgvVencidos.DataSource = tabela2;
        }
    }
}
