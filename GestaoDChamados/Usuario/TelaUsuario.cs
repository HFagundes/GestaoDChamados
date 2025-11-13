using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using Npgsql;
using AtendeAI;
using GestaoDChamados.Usuario.ChatBots.chatgpt;
using Microsoft.VisualBasic.ApplicationServices;

namespace GestaoDChamados.Usuario
{
    public class UsuarioForm : Form
    {
        private string usuarioAutenticado;
        private Panel sidebar, header, mainContent;
        private Label lblTitulo;
        private string connectionString = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

        // vamos guardar a logo pra conseguir centralizar no Resize
        private PictureBox logoMeta;

        public UsuarioForm(string usuario)
        {
            this.usuarioAutenticado = usuario;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "atende.AI";
            Size = new Size(1920, 1080);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            BackColor = Color.White; // fundo geral branco

            sidebar = CriarSidebar();

            // 👉 mainContent agora fundo branco, sem nada além da logo
            mainContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            Controls.Add(mainContent);

            // se você realmente tiver um header em outro lugar, mantém.
            // se não tiver, pode comentar a linha abaixo:
            // Controls.Add(header);

            Controls.Add(sidebar);

            CriarMainContent();

            // centralizar logo quando redimensionar
            mainContent.Resize += (s, e) => CentralizarLogo();
        }

        private Panel CriarSidebar()
        {
            var panel = new Panel { Width = 200, BackColor = Color.Black, Dock = DockStyle.Left };
            string[] botoes = { "MENU", "AJUDA.AI", "CRIAR CHAMADO", "MEUS CHAMADOS" };

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
                    TextAlign = ContentAlignment.MiddleCenter
                };
                btn.FlatAppearance.BorderSize = 0;

                btn.MouseEnter += (s, e) => { btn.BackColor = Color.White; btn.ForeColor = Color.Black; };
                btn.MouseLeave += (s, e) => { btn.BackColor = Color.Black; btn.ForeColor = Color.White; };

                if (nome == "CRIAR CHAMADO")
                    btn.Click += (s, e) => NavigateToCriarChamado();
                else if (nome == "AJUDA.AI")
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        var chatForm = new ChatForm(this) { TopLevel = false, Dock = DockStyle.Fill };
                        mainContent.Controls.Add(chatForm);
                        chatForm.Show();
                    };
                else if (nome == "MENU")
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        CriarMainContent();
                    };
                else if (nome == "MEUS CHAMADOS")
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        var meusChamados = new MeusChamados(this.usuarioAutenticado)
                        {
                            TopLevel = false,
                            Dock = DockStyle.Fill
                        };
                        mainContent.Controls.Add(meusChamados);
                        meusChamados.Show();
                    };

                panel.Controls.Add(btn);
            }

            var spacer = new Panel { Height = 50, Dock = DockStyle.Top };
            panel.Controls.Add(spacer);

            var logo = new PictureBox
            {
                Image = Image.FromFile("resources/atendeai.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Height = 90,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(logo);

            return panel;
        }

        // 🔥 NOVA VERSÃO: só fundo branco + logo da Metacorp centralizada
        private void CriarMainContent()
        {
            mainContent.Controls.Clear();
            mainContent.BackColor = Color.White;

            logoMeta = new PictureBox
            {
                Image = Image.FromFile("resources/metacorp.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(400, 400), // logo grande
                BackColor = Color.Transparent
            };

            mainContent.Controls.Add(logoMeta);
            CentralizarLogo();
        }

        private void CentralizarLogo()
        {
            if (logoMeta == null || mainContent == null) return;

            int x = (mainContent.ClientSize.Width - logoMeta.Width) / 2;
            int y = (mainContent.ClientSize.Height - logoMeta.Height) / 2;

            if (x < 0) x = 0;
            if (y < 0) y = 0;

            logoMeta.Location = new Point(x, y);
        }

        public void NavigateToCriarChamado()
        {
            mainContent.Controls.Clear();
            var formChamado = new CriarChamadoForm(this.usuarioAutenticado)
            {
                TopLevel = false,
                Dock = DockStyle.Fill
            };
            mainContent.Controls.Add(formChamado);
            formChamado.Show();
        }

        // As funções ObterAvisos, ObterResumoChamados, GetUsuarioInfo ainda podem ficar aí
        // se você for usar em outro lugar. Se não usar mais, pode apagá-las sem problema.
    }
}
