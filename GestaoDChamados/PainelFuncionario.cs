using System;
using System.Windows.Forms;
using System.Drawing;

namespace ChamadosApp
{
    public class FuncionarioForm : Form
    {
        private Panel sidebar;
        private Panel header;
        private Panel mainContent;

        public FuncionarioForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            this.Text = "Painel do Funcionário";
            this.Size = new Size(1920, 1080);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.BackColor = Color.White;

            // Header
            header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.LightGray
            };
            this.Controls.Add(header);

            var lblTitulo = new Label
            {
                Text = "Painel do Funcionário",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Black,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            header.Controls.Add(lblTitulo);

            // Sidebar via CriarSidebar()
            sidebar = CriarSidebar();
            this.Controls.Add(sidebar);

            // Main content area
            mainContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke
            };
            this.Controls.Add(mainContent);
        }

        private Panel CriarSidebar()
        {
            var panel = new Panel
            {
                Width = 200,
                BackColor = Color.Black,
                Dock = DockStyle.Left,
            };

            // Lista de botões (em ordem normal)
            string[] botoes = { "MENU", "CHAMADOS EM ABERTO", "CHAMADOS EM ANDAMENTO", "CHAMADOS ENCERRADOS" };

            // Adiciona os botões de baixo para cima
            for (int i = botoes.Length - 1; i >= 0; i--)
            {
                string nome = botoes[i];
                var btn = new Button
                {
                    Text = nome,
                    Height = 45,
                    Dock = DockStyle.Top,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Padding = new Padding(0)
                };
                btn.FlatAppearance.BorderSize = 0;

                btn.MouseEnter += (s, e) =>
                {
                    btn.BackColor = Color.White;
                    btn.ForeColor = Color.Black;
                };
                btn.MouseLeave += (s, e) =>
                {
                    btn.BackColor = Color.Black;
                    btn.ForeColor = Color.White;
                };

                panel.Controls.Add(btn);
            }

            // Espaço entre logo e botões
            var spacer = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top
            };
            panel.Controls.Add(spacer);

            // Logo no topo
            var logo = new PictureBox
            {
                // Ajuste o caminho da imagem conforme necessário
                Image = Image.FromFile("resources/atendeai.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Height = 90,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(logo);

            return panel;
        }
    }
}