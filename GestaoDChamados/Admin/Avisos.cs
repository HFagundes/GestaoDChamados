using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;  // Biblioteca para conexão com PostgreSQL

namespace GestaoDChamados.Admin
{
    public partial class Avisos : Form
    {
        private string connectionString = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

        // Componentes criados manualmente
        private DataGridView dataGridViewAvisos;
        private Button btnExcluirAviso;
        private Button btnAdicionarAviso;
        private TextBox txtAviso;
        private DateTimePicker dateTimePickerAviso;
        private Button btnLimparAviso;
        private TextBox txtAvisoDetalhado; // Para mostrar o aviso completo

        public Avisos()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ControlBox = false;
            // Inicializa os componentes manualmente
            InitializeComponents();
            CarregarAvisos();
        }

        private void InitializeComponents()
        {
            // DataGridView
            this.dataGridViewAvisos = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Size = new System.Drawing.Size(600, 300),
                Location = new System.Drawing.Point(20, 20)
            };
            this.dataGridViewAvisos.CellClick += new DataGridViewCellEventHandler(this.dataGridViewAvisos_CellClick);
            this.Controls.Add(dataGridViewAvisos);

            // Button Excluir Aviso
            this.btnExcluirAviso = new Button
            {
                Text = "Excluir Aviso",
                Size = new System.Drawing.Size(120, 40),
                Location = new System.Drawing.Point(20, 340)
            };
            this.btnExcluirAviso.Click += new EventHandler(this.btnExcluirAviso_Click);
            this.Controls.Add(btnExcluirAviso);

            // Button Adicionar Aviso
            this.btnAdicionarAviso = new Button
            {
                Text = "Adicionar Aviso",
                Size = new System.Drawing.Size(120, 40),
                Location = new System.Drawing.Point(520, 340)
            };
            this.btnAdicionarAviso.Click += new EventHandler(this.btnAdicionarAviso_Click);
            this.Controls.Add(btnAdicionarAviso);

            // TextBox Aviso
            this.txtAviso = new TextBox
            {
                Multiline = true,
                Size = new System.Drawing.Size(600, 60),
                Location = new System.Drawing.Point(20, 400)
            };
            this.Controls.Add(txtAviso);

            // DateTimePicker Aviso
            this.dateTimePickerAviso = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Size = new System.Drawing.Size(200, 20),
                Location = new System.Drawing.Point(20, 470)
            };
            this.Controls.Add(dateTimePickerAviso);

            // Button Limpar Aviso
            this.btnLimparAviso = new Button
            {
                Text = "Limpar Aviso",
                Size = new System.Drawing.Size(120, 40),
                Location = new System.Drawing.Point(200, 340)
            };
            this.btnLimparAviso.Click += new EventHandler(this.btnLimparAviso_Click);
            this.Controls.Add(btnLimparAviso);

            // TextBox para exibir o aviso completo
            this.txtAvisoDetalhado = new TextBox
            {
                Multiline = true,
                Size = new System.Drawing.Size(600, 100),
                Location = new System.Drawing.Point(20, 180),
                ReadOnly = true
            };
            this.Controls.Add(txtAvisoDetalhado);

            // Configuração do Form
            this.ClientSize = new System.Drawing.Size(640, 530);
            this.Text = "Gerenciar Avisos";
        }

        // Carregar os avisos do banco de dados
        private void CarregarAvisos()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id, LEFT(aviso, 100) AS aviso_resumo, data FROM avisos ORDER BY data DESC";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Ajustando o nome da coluna para o resumo do aviso
                dataGridViewAvisos.DataSource = dt;
                dataGridViewAvisos.Columns["aviso_resumo"].HeaderText = "Aviso (Resumo)";
            }
        }

        // Evento para clicar sobre a linha do aviso
        private void dataGridViewAvisos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica se a célula clicada não é de cabeçalho e está na coluna correta
            if (e.RowIndex >= 0)
            {
                // Pega o ID do aviso da célula clicada
                int idAviso = Convert.ToInt32(dataGridViewAvisos.Rows[e.RowIndex].Cells["id"].Value);
                AbrirAvisoDetalhado(idAviso);
            }
        }

        // Abrir o aviso completo em um campo de texto
        private void AbrirAvisoDetalhado(int idAviso)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT aviso FROM avisos WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idAviso);
                    var avisoDetalhado = cmd.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(avisoDetalhado))
                    {
                        txtAvisoDetalhado.Text = avisoDetalhado;
                    }
                    else
                    {
                        txtAvisoDetalhado.Text = "Aviso não encontrado.";
                    }
                }
            }
        }

        // Excluir aviso selecionado
        private void btnExcluirAviso_Click(object sender, EventArgs e)
        {
            if (dataGridViewAvisos.SelectedRows.Count > 0)
            {
                int idAviso = Convert.ToInt32(dataGridViewAvisos.SelectedRows[0].Cells["id"].Value);

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM avisos WHERE id = @id";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idAviso);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Aviso excluído com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CarregarAvisos(); // Atualiza a lista de avisos
                txtAvisoDetalhado.Clear(); // Limpa o campo de detalhes
            }
            else
            {
                MessageBox.Show("Selecione um aviso para excluir.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Adicionar um novo aviso
        private void btnAdicionarAviso_Click(object sender, EventArgs e)
        {
            string aviso = txtAviso.Text;
            DateTime data = dateTimePickerAviso.Value;

            if (string.IsNullOrEmpty(aviso))
            {
                MessageBox.Show("Informe o aviso.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO avisos (aviso, data) VALUES (@aviso, @data)";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@aviso", aviso);
                    cmd.Parameters.AddWithValue("@data", data);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Aviso criado com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CarregarAvisos(); // Atualiza a lista de avisos
            txtAviso.Clear(); // Limpa o campo de texto
        }

        // Limpar o campo de texto
        private void btnLimparAviso_Click(object sender, EventArgs e)
        {
            txtAviso.Clear();
        }
    }
}
