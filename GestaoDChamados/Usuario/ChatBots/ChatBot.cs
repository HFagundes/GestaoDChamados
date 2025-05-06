using System;
using System.Drawing;
using System.Windows.Forms;

namespace GestaoDChamados.Usuario.ChatBots
{
    public class ChatBotForm : Form
    {
        private RichTextBox chatBox;
        private TextBox inputBox;
        private Button sendButton;

        public ChatBotForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            chatBox = new RichTextBox
            {
                Location = new Point(10, 10),
                Size = new Size(560, 400),
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            inputBox = new TextBox
            {
                Location = new Point(10, 420),
                Size = new Size(460, 30),
                Font = new Font("Segoe UI", 10)
            };

            sendButton = new Button
            {
                Text = "Enviar",
                Location = new Point(480, 420),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(30, 60, 110),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.Click += SendButton_Click;

            Controls.Add(chatBox);
            Controls.Add(inputBox);
            Controls.Add(sendButton);
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string userInput = inputBox.Text.Trim();
            if (!string.IsNullOrEmpty(userInput))
            {
                chatBox.AppendText($"Você: {userInput}\n");

                // Resposta automática simples
                string resposta = ObterRespostaSimples(userInput);
                chatBox.AppendText($"AI: {resposta}\n\n");

                inputBox.Clear();
                inputBox.Focus();
            }
        }

        private string ObterRespostaSimples(string pergunta)
        {
            pergunta = pergunta.ToLower();
            if (pergunta.Contains("problema") || pergunta.Contains("erro"))
                return "Você pode criar um chamado clicando em 'Criar Chamado'.";
            if (pergunta.Contains("urgente"))
                return "Se for algo urgente, recomendamos selecionar 'Urgente' ao criar o chamado.";
            if (pergunta.Contains("obrigado"))
                return "Estou aqui para ajudar!";

            return "Desculpe, não entendi. Você pode tentar reformular sua pergunta.";
        }
    }
}
