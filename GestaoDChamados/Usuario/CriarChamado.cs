using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

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
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(0, 0, 10, 10, 180, 90);
                    path.AddArc(panel.Width - 11, 0, 10, 10, 270, 90);
                    path.AddArc(panel.Width - 11, panel.Height - 11, 10, 10, 0, 90);
                    path.AddArc(0, panel.Height - 11, 10, 10, 90, 90);
                    path.CloseAllFigures();
                    using (Pen pen = new Pen(Color.Gray, 1))
                    {
                        g.DrawPath(pen, path);
                    }
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
            txtDescricao = new TextBox{
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

            lblArquivoSelecionado = new Label
            {
                Text = "Nenhum arquivo selecionado",
                Location = new Point(180, 445),
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
                btnAnexo, lblArquivoSelecionado,
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

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chamado enviado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
