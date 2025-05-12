using System;
using System.Data;
using System.Windows.Forms;
using Npgsql; // Certifique-se de ter adicionado o pacote Npgsql ao seu projeto.

public class MeusChamados : Form
{
    private string usuarioAutenticado;
    private Label lblUsuario;
    private DataGridView dgvChamados;
    private Button btnAbertos;
    private Button btnEmAndamento;
    private Button btnResolvidos;
    private Button btnVencidos;

    // String de conexão com o banco de dados PostgreSQL
    private string connectionString = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

    public MeusChamados(string usuario)
    {
        this.usuarioAutenticado = usuario;
        this.Text = "Meus Chamados";
        this.FormBorderStyle = FormBorderStyle.None;
        this.Dock = DockStyle.Fill;

        InicializarTabela();
        InicializarBotoesFiltro();
        CarregarChamados(); // Carregar todos os chamados inicialmente
    }

    private void InicializarTabela()
    {
        dgvChamados = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false
        };

        dgvChamados.Columns.Add("Nome", "Nome");
        dgvChamados.Columns.Add("Email", "Email");
        dgvChamados.Columns.Add("Urgencia", "Urgência");
        dgvChamados.Columns.Add("Assunto", "Assunto");
        dgvChamados.Columns.Add("Descricao", "Descrição");
        dgvChamados.Columns.Add("DataCriacao", "Data de Criação");
        dgvChamados.Columns.Add("Situacao", "Situação");

        DataGridViewButtonColumn verChamadoBtn = new DataGridViewButtonColumn
        {
            HeaderText = "Ações",
            Text = "Ver chamado",
            UseColumnTextForButtonValue = true,
            Name = "Acoes"
        };
        dgvChamados.Columns.Add(verChamadoBtn);

        Controls.Add(dgvChamados);
    }

    private void InicializarBotoesFiltro()
    {
        // Botão para filtrar chamados Abertos
        btnAbertos = new Button
        {
            Text = "Abertos",
            Height = 40,
            Width = 120,
            Margin = new Padding(5),
            FlatStyle = FlatStyle.Flat,
            BackColor = System.Drawing.Color.LightSkyBlue,
            ForeColor = System.Drawing.Color.White,
            FlatAppearance = { BorderSize = 0 }
        };
        btnAbertos.Click += (sender, e) => CarregarChamados("Abertos");

        // Botão para filtrar chamados Em Andamento
        btnEmAndamento = new Button
        {
            Text = "Em Andamento",
            Height = 40,
            Width = 120,
            Margin = new Padding(5),
            FlatStyle = FlatStyle.Flat,
            BackColor = System.Drawing.Color.LightGreen,
            ForeColor = System.Drawing.Color.White,
            FlatAppearance = { BorderSize = 0 }
        };
        btnEmAndamento.Click += (sender, e) => CarregarChamados("Em Andamento");

        // Botão para filtrar chamados Resolvidos
        btnResolvidos = new Button
        {
            Text = "Resolvidos",
            Height = 40,
            Width = 120,
            Margin = new Padding(5),
            FlatStyle = FlatStyle.Flat,
            BackColor = System.Drawing.Color.LightCoral,
            ForeColor = System.Drawing.Color.White,
            FlatAppearance = { BorderSize = 0 }
        };
        btnResolvidos.Click += (sender, e) => CarregarChamados("Resolvidos");

        // Botão para filtrar chamados Vencidos
        btnVencidos = new Button
        {
            Text = "Vencidos",
            Height = 40,
            Width = 120,
            Margin = new Padding(5),
            FlatStyle = FlatStyle.Flat,
            BackColor = System.Drawing.Color.Orange,
            ForeColor = System.Drawing.Color.White,
            FlatAppearance = { BorderSize = 0 }
        };
        btnVencidos.Click += (sender, e) => CarregarChamados("Vencidos");

        // Painel para os botões
        FlowLayoutPanel pnlFiltros = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 60,
            FlowDirection = FlowDirection.LeftToRight, // Alinha os botões horizontalmente
            Padding = new Padding(10),
            AutoSize = true,
            WrapContents = false, // Para que os botões não se movam para a linha seguinte
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        // Centralizando os botões usando o AutoSize e o Dock
        pnlFiltros.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        pnlFiltros.Dock = DockStyle.Top;

        // Adicionando os botões ao painel
        pnlFiltros.Controls.Add(btnAbertos);
        pnlFiltros.Controls.Add(btnEmAndamento);
        pnlFiltros.Controls.Add(btnResolvidos);
        pnlFiltros.Controls.Add(btnVencidos);

        // Adicionando o painel de botões ao formulário
        Controls.Add(pnlFiltros);
    }


    private void CarregarChamados(string situacaoFiltro = null)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                // Consulta SQL com filtro baseado na situação
                string query = "SELECT nome, email, urgencia, assunto, descricao, datacriacao, situacao FROM chamados WHERE usuario = @usuario";

                // Se houver um filtro de situação, atualiza a consulta
                if (!string.IsNullOrEmpty(situacaoFiltro))
                {
                    query += " AND situacao = @situacao";
                }

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@usuario", usuarioAutenticado);

                    // Adiciona o parâmetro de filtro de situação, se necessário
                    if (!string.IsNullOrEmpty(situacaoFiltro))
                    {
                        cmd.Parameters.AddWithValue("@situacao", situacaoFiltro);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        dgvChamados.Rows.Clear();

                        while (reader.Read())
                        {
                            string nome = reader.GetString(0);
                            string email = reader.GetString(1);
                            string urgencia = reader.GetString(2);
                            string assunto = reader.GetString(3);
                            string descricao = reader.GetString(4);
                            string dataCriacao = reader.GetDateTime(5).ToString("dd-MM-yyyy");
                            string situacao = reader.GetString(6);

                            dgvChamados.Rows.Add(nome, email, urgencia, assunto, descricao, dataCriacao, situacao);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os chamados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
