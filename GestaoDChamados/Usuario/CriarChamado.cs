using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Mail;              // 👈 ADICIONA ISSO
using System.Windows.Forms;
using Npgsql;

namespace AtendeAI
{
    public class CriarChamadoForm : Form
    {
        private readonly string usuarioAutenticado;

        private TextBox txtNome, txtEmail, txtAssunto, txtDescricao;
        private ComboBox cbUrgencia;
        private Button btnAnexo, btnLimparAnexo, btnEnviar;
        private Label lblArquivoSelecionado;
        private string arquivoSelecionado = string.Empty;

        private readonly string _connectionString =
            "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

        public CriarChamadoForm(string usuario)
        {
            usuarioAutenticado = usuario;
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Size = new Size(600, 600);

            CriarComponentes();
        }

        private Panel CriarTextBoxArredondada(out TextBox txt, Point location, Size size)
        {
            txt = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Location = new Point(10, 7),
                Width = size.Width - 20
            };

            var panel = new Panel
            {
                Location = location,
                Size = size,
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, 10, 10, 180, 90);
                    path.AddArc(panel.Width - 11, 0, 10, 10, 270, 90);
                    path.AddArc(panel.Width - 11, panel.Height - 11, 10, 10, 0, 90);
                    path.AddArc(0, panel.Height - 11, 10, 10, 90, 90);
                    path.CloseAllFigures();
                    using (var pen = new Pen(Color.Gray, 1))
                        g.DrawPath(pen, path);
                }
            };

            panel.Controls.Add(txt);
            return panel;
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
            var pnlNome = CriarTextBoxArredondada(out txtNome, new Point(20, 90), new Size(520, 30));

            var lblEmail = new Label { Text = "Email:", Location = new Point(20, 120) };
            var pnlEmail = CriarTextBoxArredondada(out txtEmail, new Point(20, 150), new Size(520, 30));

            var lblUrgencia = new Label { Text = "Urgência:", Location = new Point(20, 180) };
            cbUrgencia = new ComboBox
            {
                Location = new Point(20, 210),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbUrgencia.Items.AddRange(new[] { "Baixa", "Média", "Alta" });
            cbUrgencia.SelectedIndex = 1;

            var lblAssunto = new Label
            {
                Text = "Assunto (máx 150 caracteres):",
                AutoSize = true,
                Location = new Point(20, 240)
            };
            var pnlAssunto = CriarTextBoxArredondada(out txtAssunto, new Point(20, 270), new Size(520, 30));
            txtAssunto.MaxLength = 150;

            var lblDescricao = new Label { Text = "Descrição:", Location = new Point(20, 300) };
            txtDescricao = new TextBox
            {
                Location = new Point(20, 330),
                Size = new Size(520, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            btnAnexo = new Button
            {
                Text = "Anexar Arquivo",
                Location = new Point(20, 440),
                Width = 150
            };
            btnAnexo.Click += BtnAnexo_Click;

            btnLimparAnexo = new Button
            {
                Text = "X",
                Location = new Point(btnAnexo.Location.X + btnAnexo.Width + 5, btnAnexo.Location.Y),
                Size = new Size(25, btnAnexo.Height),
                BackColor = Color.Red,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLimparAnexo.FlatAppearance.BorderSize = 0;
            btnLimparAnexo.Click += BtnLimparAnexo_Click;

            lblArquivoSelecionado = new Label
            {
                Text = "Nenhum arquivo selecionado",
                Location = new Point(btnLimparAnexo.Location.X + btnLimparAnexo.Width + 10,
                                     btnAnexo.Location.Y + 5),
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
                lblNome, pnlNome,
                lblEmail, pnlEmail,
                lblUrgencia, cbUrgencia,
                lblAssunto, pnlAssunto,
                lblDescricao, txtDescricao,
                btnAnexo, btnLimparAnexo, lblArquivoSelecionado,
                btnEnviar
            });
        }

        private void BtnAnexo_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                arquivoSelecionado = ofd.FileName;
                lblArquivoSelecionado.Text = Path.GetFileName(arquivoSelecionado);
            }
        }

        private void BtnLimparAnexo_Click(object sender, EventArgs e)
        {
            arquivoSelecionado = string.Empty;
            lblArquivoSelecionado.Text = "Nenhum arquivo selecionado";
        }

        // 🔍 Validação de e-mail
        private bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new MailAddress(email);
                // opcional: garantir que não tenha espaços extras
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            // 👉 Verificação de e-mail ANTES de enviar pro banco
            if (!EmailValido(txtEmail.Text))
            {
                MessageBox.Show("Informe um e-mail válido.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                using var cmd = new NpgsqlCommand(
                @"INSERT INTO chamados 
                    (nome, usuario, email, urgencia, assunto, descricao, anexo_caminho, datacriacao, id_usuario, situacao) 
                  VALUES 
                    (@nome, @usuario, @email, @urgencia, @assunto, @descricao, @anexo_caminho, @datacriacao, @id_usuario, @situacao);",
                conn);

                cmd.Parameters.AddWithValue("@nome", txtNome.Text);
                cmd.Parameters.AddWithValue("@usuario", usuarioAutenticado);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@urgencia", cbUrgencia.SelectedItem?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@assunto", txtAssunto.Text);
                cmd.Parameters.AddWithValue("@descricao", txtDescricao.Text);
                cmd.Parameters.AddWithValue("@id_usuario", usuarioAutenticado);
                cmd.Parameters.AddWithValue("@situacao", "Aberto");

                // ====== ANEXO: copia para /uploads e grava o caminho no banco ======
                if (!string.IsNullOrEmpty(arquivoSelecionado) && File.Exists(arquivoSelecionado))
                {
                    var pastaUploadsFisica = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads");
                    Directory.CreateDirectory(pastaUploadsFisica); // cria se não existir

                    var nomeArquivo = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}{Path.GetExtension(arquivoSelecionado)}";
                    var caminhoFisicoDestino = Path.Combine(pastaUploadsFisica, nomeArquivo);

                    File.Copy(arquivoSelecionado, caminhoFisicoDestino, overwrite: true);

                    var caminhoBanco = $"/uploads/{nomeArquivo}";
                    cmd.Parameters.AddWithValue("@anexo_caminho", caminhoBanco);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@anexo_caminho", DBNull.Value);
                }
                // ===================================================================

                cmd.Parameters.AddWithValue("@datacriacao", DateTime.Now);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Chamado enviado com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtNome.Text = "";
                txtEmail.Text = "";
                cbUrgencia.SelectedIndex = 1;
                txtAssunto.Text = "";
                txtDescricao.Text = "";
                lblArquivoSelecionado.Text = "Nenhum arquivo selecionado";
                arquivoSelecionado = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao enviar chamado: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
