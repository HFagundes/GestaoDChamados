using Npgsql;
using System.Data;

public class MostrarChamadosForm : Form
{
    private DataGridView dgvChamados;
    private string nomeUsuario;

    public MostrarChamadosForm(string user)
    {
        nomeUsuario = user;

        Text = "Meus Chamados";
        Size = new Size(800, 500);
        StartPosition = FormStartPosition.CenterScreen;

        dgvChamados = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false
        };

        Controls.Add(dgvChamados);
        CarregarChamadosDoUsuario();
    }

    private void CarregarChamadosDoUsuario()
    {
        string connectionString = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

        using (var conn = new NpgsqlConnection(connectionString))
        {
            string query = @"
                SELECT
                    Usuario,
                    Email,
                    LEFT(Assunto, 30) AS AssuntoResumido,
                    LEFT(Descricao, 40) AS DescricaoResumida,
                    COALESCE(Status, 'Aberto') AS Status,
                    DataCriacao
                FROM Chamados
                WHERE Usuario = @Usuario
                ORDER BY DataCriacao DESC";

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Usuario", nomeUsuario);

                try
                {
                    conn.Open();
                    var dt = new DataTable();
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                    dgvChamados.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar chamados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
