using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AtendeAI
{
    public class CriarChamadoForm : Form
    {
        private TextBox txtNome, txtEmail, txtAssunto, txtDescricao;
        private ComboBox cbUrgencia;
        private Button btnAnexo, btnLimparAnexo, btnEnviar;
        private Label lblArquivoSelecionado;
        private string arquivoSelecionado = string.Empty;

        public CriarChamadoForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.DarkGray;
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
            cbUrgencia.Items.AddRange(new[] { "Simples", "Média", "Urgente" });
            cbUrgencia.SelectedIndex = 1;

            var lblAssunto = new Label { Text = "Assunto (máx 150 caracteres):", AutoSize = true, Location = new Point(20, 240) };
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
                Location = new Point(btnLimparAnexo.Location.X + btnLimparAnexo.Width + 10, btnAnexo.Location.Y + 5),
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

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            // Validações antes do envio
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Por favor, preencha o Nome.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return;
            }

            // Regex de email corrigido como string verbatim para não gerar erros de escape
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !Regex.IsMatch(txtEmail.Text, emailPattern))
            {
                MessageBox.Show("Por favor, insira um Email válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAssunto.Text))
            {
                MessageBox.Show("Por favor, preencha o Assunto.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAssunto.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MessageBox.Show("Por favor, preencha a Descrição.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescricao.Focus();
                return;
            }

            // TODO: implementar lógica de envio do chamado
            MessageBox.Show("Chamado enviado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Limpa os campos após envio
            txtNome.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtAssunto.Text = string.Empty;
            txtDescricao.Text = string.Empty;
            cbUrgencia.SelectedIndex = 1;
            arquivoSelecionado = string.Empty;
            lblArquivoSelecionado.Text = "Nenhum arquivo selecionado";
        }
    }
}