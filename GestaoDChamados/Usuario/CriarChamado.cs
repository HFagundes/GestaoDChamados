using System;
using System.Drawing;
using System.Windows.Forms;

namespace AtendeAI
{
    public class CriarChamadoForm : Form
    {
        private TextBox txtNome, txtEmail, txtAssunto;
        private ComboBox cbUrgencia;
        private TextBox txtDescricao;
        private Button btnAnexo, btnEnviar;
        private Label lblArquivoSelecionado;
        private string arquivoSelecionado = "";

        public CriarChamadoForm()
        {
            this.Text = "Criar Chamado";
            this.Size = new Size(600, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            CriarComponentes();
        }

        private void CriarComponentes()
        {
            Label lblTitulo = new Label()
            {
                Text = "Criar Chamado",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            Label lblNome = new Label() { Text = "Nome:", Location = new Point(20, 70) };
            txtNome = new TextBox() { Location = new Point(20, 90), Width = 520 };

            Label lblEmail = new Label() { Text = "Email:", Location = new Point(20, 130) };
            txtEmail = new TextBox() { Location = new Point(20, 150), Width = 520 };

            Label lblUrgencia = new Label() { Text = "Urgência:", Location = new Point(20, 190) };
            cbUrgencia = new ComboBox()
            {
                Location = new Point(20, 210),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbUrgencia.Items.AddRange(new string[] { "Simples", "Média", "Urgente" });

            Label lblAssunto = new Label() { Text = "Assunto (máx 150 caracteres):", Location = new Point(20, 250) };
            txtAssunto = new TextBox() { Location = new Point(20, 270), Width = 520, MaxLength = 150 };

            Label lblDescricao = new Label() { Text = "Descrição:", Location = new Point(20, 310) };
            txtDescricao = new TextBox()
            {
                Location = new Point(20, 330),
                Width = 520,
                Height = 100,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            btnAnexo = new Button()
            {
                Text = "Anexar Arquivo",
                Location = new Point(20, 440),
                Width = 150
            };
            btnAnexo.Click += BtnAnexo_Click;

            lblArquivoSelecionado = new Label()
            {
                Text = "Nenhum arquivo selecionado",
                Location = new Point(180, 445),
                Width = 360,
                AutoEllipsis = true
            };

            btnEnviar = new Button()
            {
                Text = "Enviar",
                Location = new Point(450, 500),
                Width = 90,
                BackColor = Color.FromArgb(30, 60, 110),
                ForeColor = Color.White
            };
            btnEnviar.Click += BtnEnviar_Click;

            Controls.AddRange(new Control[] {
                lblTitulo, lblNome, txtNome, lblEmail, txtEmail, lblUrgencia, cbUrgencia,
                lblAssunto, txtAssunto, lblDescricao, txtDescricao,
                btnAnexo, lblArquivoSelecionado, btnEnviar
            });
        }

        private void BtnAnexo_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                arquivoSelecionado = ofd.FileName;
                lblArquivoSelecionado.Text = System.IO.Path.GetFileName(arquivoSelecionado);
            }
        }

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            // Aqui você pode adicionar validações ou salvar os dados
            MessageBox.Show("Chamado enviado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close(); // Fecha a janela após enviar
        }

        private void CriarChamado_Load(object sender, EventArgs e)
        {

        }
    }
}
