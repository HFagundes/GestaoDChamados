using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GestaoDChamados.Usuario;

namespace GestaoDChamados.Usuario.ChatBots.chatgpt
{
    public class ChatForm : Form
    {
        private readonly UsuarioForm _usuarioForm;
        private FlowLayoutPanel mensagensPanel;
        private TextBox inputBox;
        private Button btnEnviar;
        private static readonly HttpClient client = new HttpClient();

        public ChatForm(UsuarioForm usuarioForm)
        {
            _usuarioForm = usuarioForm;
            InitializeComponent();

            // Mensagem inicial
            this.Load += (s, e) =>
            {
                AdicionarBolha(
                    "Olá! Eu sou o Bitinho da Ajuda.AI e estou aqui para te ajudar. Por favor, me conte qual problema está enfrentando para que eu possa te auxiliar da melhor maneira possível.",
                    isUser: false
                );
            };
        }

        private void InitializeComponent()
        {
            FormBorderStyle = FormBorderStyle.None;
            Width = 500;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            // Cabeçalho
            var header = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.Blue
            };

            var avatar = new PictureBox
            {
                Width = 90,
                Height = 90,
                Location = new Point(10, -7),
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            try
            {
                avatar.Image = Image.FromFile("resources/bitinho.png"); // Caminho da imagem
            }
            catch
            {
                avatar.BackColor = Color.Gray; // Fallback visual
            }

            var title = new Label
            {
                Text = "Bitinho - AJUDA.AI",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(avatar.Right + 10, (header.Height - 30) / 2)
            };

            header.Controls.Add(avatar);
            header.Controls.Add(title);
            Controls.Add(header);

            // Painel de mensagens
            mensagensPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.DarkGray,
                Padding = new Padding(10),
                Margin = new Padding(0, header.Height + 20, 0, 0)
            };
            Controls.Add(mensagensPanel);
            mensagensPanel.BringToFront();

            // Input com botão de enviar
            var bottom = new Panel
            {
                Height = 70,
                Dock = DockStyle.Bottom,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var inputContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            inputBox = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 60, 0),
                Multiline = true,
                Height = 50
            };

            btnEnviar = new Button
            {
                Text = "➤",
                Dock = DockStyle.Right,
                Width = 50,
                Height = 50,
                BackColor = Color.FromArgb(37, 211, 102),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnEnviar.FlatAppearance.BorderSize = 0;

            // Efeito hover no botão de enviar
            btnEnviar.MouseEnter += (s, e) => btnEnviar.BackColor = Color.FromArgb(30, 180, 90);
            btnEnviar.MouseLeave += (s, e) => btnEnviar.BackColor = Color.FromArgb(37, 211, 102);

            // Evento de clique do botão de enviar
            btnEnviar.Click += async (sender, e) => await EnviarMensagem();

            inputContainer.Controls.Add(inputBox);
            inputContainer.Controls.Add(btnEnviar);
            bottom.Controls.Add(inputContainer);
            Controls.Add(bottom);

            // Evento KeyPress do inputBox
            inputBox.KeyPress += async (sender, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrWhiteSpace(inputBox.Text))
                {
                    await EnviarMensagem();
                    e.Handled = true;
                }
            };
        }

        private async Task EnviarMensagem()
        {
            if (!string.IsNullOrWhiteSpace(inputBox.Text))
            {
                var texto = inputBox.Text.Trim();
                AdicionarBolha(texto, isUser: true);
                inputBox.Clear();

                // placeholder
                var placeholderPanel = new Panel
                {
                    AutoSize = true,
                    Padding = new Padding(10),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(3)
                };
                var placeholderLabel = new Label
                {
                    Text = "Digitando...",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Italic),
                    ForeColor = Color.Gray
                };
                placeholderPanel.Controls.Add(placeholderLabel);
                mensagensPanel.Controls.Add(placeholderPanel);
                mensagensPanel.ScrollControlIntoView(placeholderPanel);

                // chamada IA
                string resposta = await EnviarParaOllama(texto);

                // replace placeholder
                mensagensPanel.Controls.Remove(placeholderPanel);
                placeholderPanel.Dispose();
                var responsePanel = AdicionarBolha(resposta, isUser: false);

                // adiciona feedback
                AdicionarFeedback(responsePanel);
            }
        }

        private async Task<string> EnviarParaOllama(string mensagem)
        {
            try
            {
                var body = new
                {
                    model = "mistral",
                    stream = false,
                    messages = new[] {
                        new { role = "system", content = "Você é um assistente de suporte técnico. Somente responde sobre Hardware e Software, qualquer tipo de outra pergunta que a pessoa fizer sobre vida pessoal , você responde que não foi programado para responder perguntas pessoais." },
                        new { role = "user", content = mensagem }
                    }
                };
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var resp = await client.PostAsync("http://localhost:11434/api/chat", content);
                var text = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode) return $"Erro: {resp.StatusCode}";
                var json = JObject.Parse(text);
                return json["message"]?["content"]?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $"Erro: {ex.Message}";
            }
        }

        private Panel AdicionarBolha(string texto, bool isUser)
        {
            int maxWidth = (ClientSize.Width / 2) - 40;
            var bubble = new Label
            {
                Text = texto,
                AutoSize = true,
                MaximumSize = new Size(maxWidth, 0),
                Font = new Font("Segoe UI", 10),
                Padding = new Padding(10),
                Margin = new Padding(3),
                BackColor = isUser ? Color.FromArgb(220, 248, 198) : Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var timeLabel = new Label
            {
                Text = DateTime.Now.ToString("HH:mm"),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                AutoSize = true,
                Margin = new Padding(3, 5, 3, 0)
            };
            var bubblePanel = new Panel
            {
                AutoSize = true,
                Padding = new Padding(3),
                Margin = new Padding(3),
                Width = mensagensPanel.ClientSize.Width - 25,
                BackColor = Color.Transparent
            };
            bubblePanel.Controls.Add(bubble);
            bubblePanel.Controls.Add(timeLabel);

            // posicionamento
            if (isUser)
            {
                bubble.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                bubble.Location = new Point(bubblePanel.Width - bubble.Width - 20, 0);
                timeLabel.Location = new Point(bubblePanel.Width - timeLabel.Width - 20, bubble.Bottom + 5);
            }
            else
            {
                bubble.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                bubble.Location = new Point(10, 0);
                timeLabel.Location = new Point(10, bubble.Bottom + 5);
            }

            bubblePanel.Resize += (s, e) =>
            {
                if (isUser)
                    bubble.Location = new Point(bubblePanel.Width - bubble.Width - 20, 0);
                else
                    bubble.Location = new Point(10, 0);
                if (isUser)
                    timeLabel.Location = new Point(bubblePanel.Width - timeLabel.Width - 20, bubble.Bottom + 5);
                else
                    timeLabel.Location = new Point(10, bubble.Bottom + 5);
            };

            mensagensPanel.Controls.Add(bubblePanel);
            mensagensPanel.ScrollControlIntoView(bubblePanel);
            return bubblePanel;
        }

        private void AdicionarFeedback(Panel afterPanel)
        {
            var feedbackPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(5),
                Margin = new Padding(3),
                BackColor = Color.LightGray,
                Width = mensagensPanel.ClientSize.Width - 25
            };
            var pergunta = new Label
            {
                Text = "Essa resposta ajudou?",
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(3)
            };

            var btnSim = new Button
            {
                Text = "👍 Sim",
                AutoSize = true,
                Margin = new Padding(3),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Padding = new Padding(5, 2, 5, 2)
            };
            btnSim.FlatAppearance.BorderSize = 0;
            btnSim.MouseEnter += (s, e) => btnSim.BackColor = Color.FromArgb(60, 150, 70);
            btnSim.MouseLeave += (s, e) => btnSim.BackColor = Color.FromArgb(76, 175, 80);

            var btnNao = new Button
            {
                Text = "👎 Não",
                AutoSize = true,
                Margin = new Padding(3),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Padding = new Padding(5, 2, 5, 2)
            };
            btnNao.FlatAppearance.BorderSize = 0;
            btnNao.MouseEnter += (s, e) => btnNao.BackColor = Color.FromArgb(200, 50, 40);
            btnNao.MouseLeave += (s, e) => btnNao.BackColor = Color.FromArgb(244, 67, 54);

            feedbackPanel.Controls.Add(pergunta);
            feedbackPanel.Controls.Add(btnSim);
            feedbackPanel.Controls.Add(btnNao);
            mensagensPanel.Controls.Add(feedbackPanel);
            mensagensPanel.ScrollControlIntoView(feedbackPanel);

            btnSim.Click += (s, e) =>
            {
                feedbackPanel.Controls.Clear();
                feedbackPanel.Controls.Add(new Label
                {
                    Text = "Obrigado pelo feedback! 👍",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10),
                    Margin = new Padding(3)
                });
            };

            btnNao.Click += (s, e) =>
            {
                feedbackPanel.Controls.Clear();
                feedbackPanel.Controls.Add(new Label
                {
                    Text = "Gostaria de abrir um chamado?",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10),
                    Margin = new Padding(3)
                });

                var btnAbrirSim = new Button
                {
                    Text = "Sim",
                    AutoSize = true,
                    Margin = new Padding(3),
                    BackColor = Color.FromArgb(76, 175, 80),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnAbrirSim.FlatAppearance.BorderSize = 0;

                var btnAbrirNao = new Button
                {
                    Text = "Não",
                    AutoSize = true,
                    Margin = new Padding(3),
                    BackColor = Color.FromArgb(244, 67, 54),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnAbrirNao.FlatAppearance.BorderSize = 0;

                feedbackPanel.Controls.Add(btnAbrirSim);
                feedbackPanel.Controls.Add(btnAbrirNao);

                btnAbrirSim.Click += (_, __) =>
                {
                    _usuarioForm.NavigateToCriarChamado();
                };

                btnAbrirNao.Click += (_, __) =>
                {
                    feedbackPanel.Controls.Clear();
                    feedbackPanel.Controls.Add(new Label
                    {
                        Text = "Tudo bem. Caso precise, estou à disposição.",
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10),
                        Margin = new Padding(3)
                    });
                };
            };
        }
    }
}