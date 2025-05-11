using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GestaoDChamados.Usuario.ChatBots.chatgpt
{
    public class ChatForm : Form
    {
        private FlowLayoutPanel mensagensPanel;
        private TextBox inputBox;
        private static readonly HttpClient client = new HttpClient();

        public ChatForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            Width = 500;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            // Cabeçalho fixo
            var header = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(37, 211, 102)
            };

            var title = new Label
            {
                Text = "ATENDE.AI",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point((Width - 100) / 2, 18)
            };

            var avatar = new PictureBox
            {
                Width = 40,
                Height = 40,
                Location = new Point(title.Left - 50, 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = new Bitmap(40, 40) // Substitua por um avatar real
            };

            header.Controls.Add(title);
            header.Controls.Add(avatar);
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

            // Painel inferior com input
            var bottom = new Panel
            {
                Height = 53,
                Dock = DockStyle.Bottom,
                BackColor = Color.White,
                Padding = new Padding(12)
            };

            inputBox = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Dock = DockStyle.Fill
            };
            bottom.Controls.Add(inputBox);
            Controls.Add(bottom);

            // Mensagem inicial automática da IA
            this.Load += (s, e) =>
            {
                AdicionarBolha(
                    "Olá! Eu sou a Ajuda.AI e estou aqui para te ajudar. Por favor, me conte qual problema está enfrentando para que eu possa te auxiliar da melhor maneira possível.",
                    isUser: false
                );
            };

            // Evento para envio e placeholder de "Digitando..." em caixinha
            inputBox.KeyPress += async (sender, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrWhiteSpace(inputBox.Text))
                {
                    var texto = inputBox.Text.Trim();
                    AdicionarBolha(texto, isUser: true);
                    inputBox.Clear();

                    // Caixinha de placeholder enquanto a IA pensa
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

                    // Chamada à IA
                    string resposta = await EnviarParaOllama(texto);

                    // Remove placeholder e adiciona resposta
                    mensagensPanel.Controls.Remove(placeholderPanel);
                    placeholderPanel.Dispose();
                    AdicionarBolha(resposta, isUser: false);

                    // Impede o som do Windows ao pressionar Enter
                    e.Handled = true;
                }
            };
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
                        new { role = "system", content = "Você é um assistente de suporte técnico. Sempre pergunte se ajudou. Você somente responderá perguntas sobre Hardware e Software, qualquer outro tipo de pergunta, você dirá que não pode ajudar." },
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

        private void AdicionarBolha(string texto, bool isUser)
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
                ForeColor = Color.Black,
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
        }
    }
}
