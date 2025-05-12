using System;
using System.Data;
using System.Windows.Forms;
using Npgsql; // Certifique-se de ter adicionado o pacote Npgsql ao seu projeto.

public class MeusChamados : Form
{
    private string usuarioAutenticado;
    private Label lblUsuario;
    private DataGridView dgvChamados;

    // String de conexão com o banco de dados PostgreSQL
    private string connectionString = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

    public MeusChamados(string usuario)
    {
        this.usuarioAutenticado = usuario;
        this.Text = "Meus Chamados";
        this.FormBorderStyle = FormBorderStyle.None;
        this.Dock = DockStyle.Fill;

        InicializarTabela();
        CarregarChamados();
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

        // Coluna de ação com botão "Ver chamado"
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

    private void CarregarChamados()
    {
        // Estabelecer a conexão com o banco de dados
        using (var connection = new NpgsqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                // Query SQL para buscar os chamados criados pelo usuário autenticado
                string query = "SELECT nome, email, urgencia, assunto, descricao, datacriacao FROM chamados WHERE usuario = @usuario";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    // Passar o nome do usuário autenticado como parâmetro
                    cmd.Parameters.AddWithValue("@usuario", usuarioAutenticado);

                    using (var reader = cmd.ExecuteReader())
                    {
                        // Limpar os dados antigos
                        dgvChamados.Rows.Clear();

                        // Preencher a tabela com os dados retornados do banco
                        while (reader.Read())
                        {
                            string nome = reader.GetString(0);
                            string email = reader.GetString(1);
                            string urgencia = reader.GetString(2);
                            string assunto = reader.GetString(3);
                            string descricao = reader.GetString(4);
                            string dataCriacao = reader.GetDateTime(5).ToString("dd-MM-yyyy");

                            // Adicionar as informações à DataGridView
                            dgvChamados.Rows.Add(nome, email, urgencia, assunto, descricao, dataCriacao);
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
