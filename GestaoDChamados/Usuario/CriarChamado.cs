using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Npgsql;  // Referência do Npgsql

namespace AtendeAI
{
    public class CriarChamadoForm : Form
    {
        private TextBox txtNome, txtEmail, txtAssunto, txtDescricao;
        private ComboBox cbUrgencia;
        private Button btnAnexo, btnEnviar;
        private Label lblArquivoSelecionado;
        private string arquivoSelecionado = "";

        public CriarChamadoForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Size = new Size(600, 600);

            CriarComponentes();
        }

        private void CriarComponentes()
        {
            var lblTitulo = new Label
            {
                Text = "Criar Chamado",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var lblNome = new Label { Text = "Nome:", Location = new Point(20, 60) };
            txtNome = new TextBox { Location = new Point(20, 90), Width = 520 };

            var lblEmail = new Label { Text = "Email:", Location = new Point(20, 120) };
            txtEmail = new TextBox { Location = new Point(20, 150), Width = 520 };

            var lblUrgencia = new Label { Text = "Urgência:", Location = new Point(20, 180) };
            cbUrgencia = new ComboBox
            {
                Location = new Point(20, 210),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbUrgencia.Items.AddRange(new[] { "Simples", "Média", "Urgente" });
            cbUrgencia.SelectedIndex = 1;

            var lblAssunto = new Label { Text = "Assunto (máx 150 caracteres):", AutoSize = true, Location = new Point(20, 240) };
            txtAssunto = new TextBox { Location = new Point(20, 270), Width = 520, MaxLength = 150 };

            var lblDescricao = new Label { Text = "Descrição:", Location = new Point(20, 320) };
            txtDescricao = new TextBox
            {
                Location = new Point(20, 350),
                Size = new Size(520, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            btnAnexo = new Button
            {
                Text = "Anexar Arquivo",
                Location = new Point(20, 470),
                Width = 150
            };
            btnAnexo.Click += BtnAnexo_Click;

            lblArquivoSelecionado = new Label
            {
                Text = "Nenhum arquivo selecionado",
                Location = new Point(180, 475),
                Width = 360,
                AutoEllipsis = true
            };

            btnEnviar = new Button
            {
                Text = "Enviar",
                Location = new Point(450, 500),
                Width = 90,
                BackColor = Color.FromArgb(30, 60, 110),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEnviar.FlatAppearance.BorderSize = 0;
            btnEnviar.Click += BtnEnviar_Click;

            Controls.AddRange(new Control[]
            {
                lblTitulo,
                lblNome, txtNome,
                lblEmail, txtEmail,
                lblUrgencia, cbUrgencia,
                lblAssunto, txtAssunto,
                lblDescricao, txtDescricao,
                btnAnexo, lblArquivoSelecionado,
                btnEnviar
            });
        }

        private void BtnAnexo_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Imagem JPG|*.jpg;*.jpeg|Imagem PNG|*.png|Imagem GIF|*.gif";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    arquivoSelecionado = ofd.FileName;
                    lblArquivoSelecionado.Text = Path.GetFileName(arquivoSelecionado);
                }
            }
        }

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();
            string urgencia = cbUrgencia.SelectedItem?.ToString();
            string assunto = txtAssunto.Text.Trim();
            string descricao = txtDescricao.Text.Trim();

            string imagemNome = "";
            byte[] imagemDados = null;

            if (!string.IsNullOrEmpty(arquivoSelecionado))
            {
                imagemNome = Path.GetFileName(arquivoSelecionado);
                imagemDados = File.ReadAllBytes(arquivoSelecionado);  // Lê a imagem como dados binários
            }

            // String de conexão para PostgreSQL
            string connectionString = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                string query = @"INSERT INTO Chamados 
                        (Nome, Email, Urgencia, Assunto, Descricao, ImagemNome, ImagemDados)
                         VALUES (@nome, @email, @urgencia, @assunto, @descricao, @imagemNome, @imagemDados)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Urgencia", urgencia);
                    cmd.Parameters.AddWithValue("@Assunto", assunto);
                    cmd.Parameters.AddWithValue("@Descricao", descricao);
                    cmd.Parameters.AddWithValue("@ImagemNome", (object)imagemNome ?? DBNull.Value);
                    cmd.Parameters.Add("@ImagemDados", NpgsqlTypes.NpgsqlDbType.Bytea);
                    cmd.Parameters["@ImagemDados"].Value = (imagemDados != null && imagemDados.Length > 0) ? imagemDados : (object)DBNull.Value;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Chamado enviado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao enviar o chamado: " + ex.Message);
                    }
                }
            }
        }
    }
}

