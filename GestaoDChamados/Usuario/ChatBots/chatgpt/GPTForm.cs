using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AtendeAI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GestaoDChamados.Usuario.ChatBots.chatgpt
{
    public class ChatForm : Form
    {
        private TextBox inputBox;
        private Button sendButton;
        private FlowLayoutPanel chatPanel;
        private static readonly HttpClient client = new HttpClient();

        public ChatForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            BackColor = Color.Gray;

            // Cabeçalho com nome e ícone
            var headerPanel = new Panel
            {
                Height = 30,
                BackColor = Color.FromArgb(37, 211, 102),
                Padding = new Padding(0, 10, 0, 0), // Ajuste para evitar sobreposição
                Margin = new Padding(0, 10, 0, 0) // Aplica margem superior de 20px
            };

            // Definir a localização manualmente
            headerPanel.Location = new Point(0, 20); // A partir do topo com a margem desejada

            Controls.Add(headerPanel);

            var pictureBox = new PictureBox
            {
                Width = 40,
                Height = 40,
                Left = 10,
                Top = 10,
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            var nameLabel = new Label
            {
                Text = "AjudaAI",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(60, 15),
                AutoSize = true
            };

            headerPanel.Controls.Add(pictureBox);
            headerPanel.Controls.Add(nameLabel);
            Controls.Add(headerPanel);

            // Painel de chat
            chatPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10),
                Margin = new Padding(0, headerPanel.Height, 50, 20) // Evita que a 1ª msg fique atrás do header
            };

            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10),
                BackColor = Color.WhiteSmoke
            };

            inputBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            sendButton = new Button
            {
                Text = "Enviar",
                Width = 80,
                Dock = DockStyle.Right,
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.Click += async (s, e) => await EnviarMensagem();

            bottomPanel.Controls.Add(inputBox);
            bottomPanel.Controls.Add(sendButton);

            Controls.Add(chatPanel);
            Controls.Add(bottomPanel);
        }

        private async Task EnviarMensagem()
        {
            string userMsg = inputBox.Text.Trim();
            if (string.IsNullOrEmpty(userMsg)) return;

            AdicionarMensagem(userMsg, true);
            inputBox.Clear();

            string resposta = await EnviarMensagemParaOllama(userMsg);
            AdicionarMensagem(resposta, false);
            PerguntarSeAjudou();
        }

        private async Task<string> EnviarMensagemParaOllama(string mensagem)
        {
            try
            {
                var body = new
                {
                    model = "mistral",
                    prompt = "Você é uma IA de chatBot , envie exatamente essa mensagem quando o chat for aberto (Olá como posso te ajudar?) ",
                    stream = false,
                    messages = new[] {
                        new { role = "system", content = "Você é um assistente de suporte técnico que ajuda usuários com dúvidas e problemas. Sempre pergunte se a resposta ajudou." },
                        new { role = "user", content = mensagem }
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:11434/api/chat", content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return $"Erro na API: {response.StatusCode}\n{responseContent}";

                JObject jsonResponse = JObject.Parse(responseContent);
                return jsonResponse["message"]?["content"]?.ToString() ?? "Texto não encontrado.";
            }
            catch (Exception ex)
            {
                return $"Erro ao enviar mensagem: {ex.Message}";
            }
        }
        private void AdicionarMensagem(string msg, bool ehUsuario)
        {
            var bubble = new Label
            {
                Text = msg,
                AutoSize = true,
                MaximumSize = new Size(220, 0),
                Font = new Font("Segoe UI", 10),
                BackColor = ehUsuario ? Color.FromArgb(220, 248, 198) : Color.FromArgb(240, 240, 240),
                ForeColor = Color.Black,
                Padding = new Padding(8),  // Aumentando o padding para não ficar tão grudado.
                Margin = new Padding(3),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Criação do container da bolha
            var bubbleContainer = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(2),
                BackColor = Color.Transparent
            };

            bubbleContainer.Controls.Add(bubble);

            // Usar um painel para controlar o alinhamento (não é necessário FlowLayoutPanel aqui)
            var wrapperPanel = new Panel
            {
                Width = chatPanel.ClientSize.Width,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Alinhamento das mensagens
            if (ehUsuario)
            {
                // Mensagens do usuário à direita
                wrapperPanel.Dock = DockStyle.Right;
            }
            else
            {
                // Mensagens do bot à esquerda
                wrapperPanel.Dock = DockStyle.Left;
            }

            // Adicionando o container da bolha ao painel de fluxo
            wrapperPanel.Controls.Add(bubbleContainer);
            chatPanel.Controls.Add(wrapperPanel);

            // Fazendo a rolagem automática para a última mensagem
            chatPanel.ScrollControlIntoView(wrapperPanel);
        }


        private void PerguntarSeAjudou()
        {
            var pergunta = new Label
            {
                Text = "Essa resposta te ajudou?",
                AutoSize = true,
                Padding = new Padding(5),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                Margin = new Padding(5, 10, 5, 2)
            };

            var btnSim = new Button
            {
                Text = "Sim",
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5)
            };
            btnSim.Click += (s, e) =>
            {
                AdicionarMensagem("Fico feliz! Estarei aqui sempre que precisar 😊", false);
                RemoverPergunta();
            };

            var btnNao = new Button
            {
                Text = "Não",
                BackColor = Color.IndianRed,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5)
            };
            btnNao.Click += (s, e) =>
            {
                RemoverPergunta();
                MostrarOpcoesAposNao();
            };

            chatPanel.Controls.Add(pergunta);
            chatPanel.Controls.Add(btnSim);
            chatPanel.Controls.Add(btnNao);
        }

        private void MostrarOpcoesAposNao()
        {
            var pergunta = new Label
            {
                Text = "Deseja abrir um chamado com um funcionário ou quer que eu tente novamente?",
                AutoSize = true,
                Padding = new Padding(5),
                Margin = new Padding(5)
            };

            var btnTentar = new Button
            {
                Text = "Tentar novamente",
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5)
            };
            btnTentar.Click += (s, e) =>
            {
                AdicionarMensagem("Claro! Me diga de outra forma o que você precisa 😊", false);
                RemoverPergunta();
            };

            var btnAbrirChamado = new Button
            {
                Text = "Abrir chamado",
                BackColor = Color.Orange,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5)
            };
            btnAbrirChamado.Click += (s, e) =>
            {
                var chamadoForm = new CriarChamadoForm();
                chamadoForm.TopLevel = false;
                chamadoForm.Dock = DockStyle.Fill;
                Controls.Clear();
                Controls.Add(chamadoForm);
                chamadoForm.Show();
            };

            chatPanel.Controls.Add(pergunta);
            chatPanel.Controls.Add(btnTentar);
            chatPanel.Controls.Add(btnAbrirChamado);
        }

        private void RemoverPergunta()
        {
            while (chatPanel.Controls.Count > 0 &&
                  (chatPanel.Controls[chatPanel.Controls.Count - 1] is Button || chatPanel.Controls[chatPanel.Controls.Count - 1] is Label))
            {
                chatPanel.Controls.RemoveAt(chatPanel.Controls.Count - 1);
            }
        }
    }
}
