using System;
using System.Drawing;
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
            // Remove completamente a barra de título
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;       // esconde quaisquer botões de controle
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Size = new Size(600, 600);

            CriarComponentes();
        }

        private void CriarComponentes()
        {
            // Título interno
            var lblTitulo = new Label
            {
                Text = "Criar Chamado",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // Nome
            var lblNome = new Label { Text = "Nome:", Location = new Point(20, 70) };
            txtNome = new TextBox { Location = new Point(20, 90), Width = 520 };

            // Email
            var lblEmail = new Label { Text = "Email:", Location = new Point(20, 130) };
            txtEmail = new TextBox { Location = new Point(20, 150), Width = 520 };

            // Urgência
            var lblUrgencia = new Label { Text = "Urgência:", Location = new Point(20, 190) };
            cbUrgencia = new ComboBox
            {
                Location = new Point(20, 210),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbUrgencia.Items.AddRange(new[] { "Simples", "Média", "Urgente" });
            cbUrgencia.SelectedIndex = 1;

            // Assunto
            var lblAssunto = new Label { Text = "Assunto (máx 150 caracteres):", Location = new Point(20, 250) };
            txtAssunto = new TextBox { Location = new Point(20, 270), Width = 520, MaxLength = 150 };

            // Descrição
            var lblDescricao = new Label { Text = "Descrição:", Location = new Point(20, 310) };
            txtDescricao = new TextBox
            {
                Location = new Point(20, 330),
                Size = new Size(520, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Botão Anexo
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

            // Botão Enviar
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

            // Adiciona todos os controles ao Form
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
            using var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                arquivoSelecionado = ofd.FileName;
                lblArquivoSelecionado.Text = Path.GetFileName(arquivoSelecionado);
            }
        }

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            // aqui você colocaria validações e envio real
            MessageBox.Show("Chamado enviado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
