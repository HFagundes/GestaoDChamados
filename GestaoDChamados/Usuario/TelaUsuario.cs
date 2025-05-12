using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using Npgsql;  // Para realizar a conexão com o banco de dados PostgreSQL
using AtendeAI;
using GestaoDChamados.Usuario.ChatBots.chatgpt;
using Microsoft.VisualBasic.ApplicationServices;
using ChamadosApp;

namespace GestaoDChamados.Usuario
{
    public class UsuarioForm : Form
    {
        private string usuarioAutenticado;
        private Panel sidebar, header, mainContent;
        private Label lblTitulo;
        private string connectionString = "Host=localhost;Port=5432;Database=GestaoChamados;Username=postgres;Password=123;";

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
            BackColor = Color.DarkGray;

            sidebar = CriarSidebar();
            header = CriarHeader();
            mainContent = new Panel { Dock = DockStyle.Fill, BackColor = Color.DarkGray };

            Controls.Add(mainContent);
            Controls.Add(header);
            Controls.Add(sidebar);

            CriarMainContent();
        }

        private Panel CriarSidebar()
        {
            var panel = new Panel
            {
                Width = 240,
                BackColor = Color.Black,
                Dock = DockStyle.Left
            };

            string[] botoes = {
        "MEUS CHAMADOS",
        "CRIAR CHAMADO",
        "AJUDA.AI",
        "MENU"
    };

            // Adiciona os botões de cima para baixo
            foreach (string nome in botoes)
            {
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

                // Ações
                if (nome == "MENU")
                {
                    btn.Click += (s, e) => { mainContent.Controls.Clear(); CriarMainContent(); };
                }
                else if (nome == "AJUDA.AI")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        var chatForm = new ChatForm(this) { TopLevel = false, Dock = DockStyle.Fill };
                        mainContent.Controls.Add(chatForm);
                        chatForm.Show();
                    };
                }
                else if (nome == "CRIAR CHAMADO")
                {
                    btn.Click += (s, e) => NavigateToCriarChamado();
                }
                else if (nome == "MEUS CHAMADOS")
                {
                    btn.Click += (s, e) =>
                    {
                        mainContent.Controls.Clear();
                        var chatForm = new MeusChamados(this.usuarioAutenticado) { TopLevel = false, Dock = DockStyle.Fill };
                        mainContent.Controls.Add(chatForm);
                        chatForm.Show();
                    };
                }

                panel.Controls.Add(btn);
            }

            // Espaço após a logo
            panel.Controls.Add(new Panel { Height = 20, Dock = DockStyle.Top });

            // Adiciona a logo por último (para aparecer no topo)
            var logo = new PictureBox
            {
                Image = Image.FromFile("resources/atendeai.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Height = 90,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(logo);

            // Botão SAIR na parte inferior
            var btnSair = new Button
            {
                Text = "SAIR",
                Height = 45,
                Dock = DockStyle.Bottom,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnSair.FlatAppearance.BorderSize = 0;
            btnSair.MouseEnter += (s, e) => { btnSair.BackColor = Color.White; btnSair.ForeColor = Color.Black; };
            btnSair.MouseLeave += (s, e) => { btnSair.BackColor = Color.Black; btnSair.ForeColor = Color.White; };
            btnSair.Click += (s, e) =>
            {
                var loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            };
            panel.Controls.Add(btnSair);

            return panel;
        }

        private Panel CriarHeader()
        {
            var panel = new Panel { Height = 60, BackColor = Color.DarkGray, Dock = DockStyle.Top };
            lblTitulo = new Label
            {
                Text = "Painel do Usuário",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 60),
                AutoSize = true,
                Location = new Point(10, 15)
            };
            panel.Controls.Add(lblTitulo);
            return panel;
        }

        private void CriarMainContent()
        {
            mainContent.Controls.Clear();

            var painel = new Panel { Dock = DockStyle.Fill, BackColor = Color.DarkGray, Padding = new Padding(30), AutoScroll = true };

            var logo = new PictureBox
            {
                Image = Image.FromFile("resources/metacorp.png"),
                Size = new Size(100, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(20, 20)
            };
            painel.Controls.Add(logo);

            var lblEmpresa = new Label
            {
                Text = "META CORP",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 198, 96, 21),
                Location = new Point(140, 45),
                AutoSize = true
            };
            painel.Controls.Add(lblEmpresa);

            var (nomeUsuario, cargoUsuario) = GetUsuarioInfo(usuarioAutenticado);

            var lblBemVindo = new Label
            {
                Text = $"Bem-vindo {nomeUsuario} ({cargoUsuario})",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.Black,
                Location = new Point(20, 140),
                AutoSize = true
            };
            painel.Controls.Add(lblBemVindo);

            var avisos = ObterAvisos();
            int yAviso = 180;

            if (avisos.Count == 0)
            {
                var lblSemAvisos = new Label
                {
                    Text = "Nenhum aviso no momento.",
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    ForeColor = Color.Black,
                    Location = new Point(20, yAviso),
                    AutoSize = true
                };
                painel.Controls.Add(lblSemAvisos);
                yAviso += 40;
            }
            else
            {
                foreach (var (data, aviso) in avisos)
                {
                    var box = new Panel
                    {
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle,
                        Location = new Point(20, yAviso),
                        Size = new Size(600, 80)
                    };

                    var lblData = new Label
                    {
                        Text = data.ToString("dd/MM/yyyy"),
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        ForeColor = Color.Black,
                        Location = new Point(10, 10),
                        AutoSize = true
                    };

                    var lblTexto = new Label
                    {
                        Text = aviso,
                        Font = new Font("Segoe UI", 10),
                        ForeColor = Color.Black,
                        Location = new Point(10, 30),
                        AutoSize = true,
                        MaximumSize = new Size(580, 0)
                    };

                    box.Controls.Add(lblData);
                    box.Controls.Add(lblTexto);
                    painel.Controls.Add(box);
                    yAviso += box.Height + 10;
                }
            }

            string[] titulos = { "Abertos", "Em Andamento", "Resolvidos", "Vencidos" };
            Color[] cores = { Color.Blue, Color.Gold, Color.Green, Color.Red };
            var valores = ObterResumoChamados();

            int yCards = yAviso + 20;

            for (int i = 0; i < titulos.Length; i++)
            {
                var card = new Panel
                {
                    Size = new Size(160, 80),
                    BackColor = cores[i],
                    Location = new Point(20 + (i * 170), yCards),
                    BorderStyle = BorderStyle.FixedSingle
                };

                var lblTitulo = new Label
                {
                    Text = titulos[i],
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.Black,
                    Location = new Point(10, 10),
                    AutoSize = true
                };

                var lblValor = new Label
                {
                    Text = valores.ContainsKey(titulos[i]) ? valores[titulos[i]].ToString() : "0",
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.Black,
                    Location = new Point(10, 35),
                    AutoSize = true
                };

                card.Controls.Add(lblTitulo);
                card.Controls.Add(lblValor);
                painel.Controls.Add(card);
            }

            mainContent.Controls.Add(painel);
        }

        private List<(DateTime, string)> ObterAvisos()
        {
            var avisos = new List<(DateTime, string)>();
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT data, aviso FROM avisos WHERE data <= @hoje ORDER BY data DESC LIMIT 5", conn))
                    {
                        cmd.Parameters.AddWithValue("hoje", DateTime.Now);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime data = reader.GetDateTime(0);
                                string aviso = reader.GetString(1);
                                avisos.Add((data, aviso));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao buscar avisos: " + ex.Message);
            }
            return avisos;
        }

        private Dictionary<string, int> ObterResumoChamados()
        {
            var resultado = new Dictionary<string, int>
            {
                { "Abertos", 0 },
                { "Em Andamento", 0 },
                { "Resolvidos", 0 },
                { "Vencidos", 0 }
            };

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT situacao, COUNT(*) FROM chamados WHERE usuario = @usuario GROUP BY situacao", conn))
                    {
                        cmd.Parameters.AddWithValue("usuario", usuarioAutenticado);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string situacao = reader.GetString(0);
                                int quantidade = reader.GetInt32(1);

                                if (situacao == "Abertos") resultado["Abertos"] += quantidade;
                                else if (situacao == "Em Andamento") resultado["Em Andamento"] += quantidade;
                                else if (situacao == "Resolvidos") resultado["Resolvidos"] += quantidade;
                                else if (situacao == "Vencidos") resultado["Vencidos"] += quantidade;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao buscar resumo de chamados: " + ex.Message);
            }

            return resultado;
        }

        private (string, string) GetUsuarioInfo(string usuarioAutenticado)
        {
            string nome = string.Empty, cargo = string.Empty;
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT nome, cargo FROM usuarios WHERE usuario = @usuario", conn))
                    {
                        cmd.Parameters.AddWithValue("usuario", usuarioAutenticado);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nome = reader.GetString(0);
                                cargo = reader.GetString(1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar usuário no banco: {ex.Message}");
            }
            return (nome, cargo);
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
    }
}
