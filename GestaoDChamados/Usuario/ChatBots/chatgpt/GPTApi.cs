using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtendeAI
{
    public static class ChatBotOllama
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string ollamaEndpoint = "http://localhost:11434/api/chat";

        public static async Task<string> EnviarMensagemOllama(string mensagem)
        {
            try
            {
                var body = new
                {
                    model = "mistral",
                    messages = new[]
                    {
                        new { role = "system", content = "Você é um assistente de suporte técnico que ajuda usuários com dúvidas e problemas. Sempre pergunte se a resposta ajudou." },
                        new { role = "user", content = mensagem }
                    },
                    stream = false
                };

                var jsonBody = JsonConvert.SerializeObject(body);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(ollamaEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    return $"Erro na API do Ollama: {response.StatusCode}\n{error}";
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(json);

                return result["message"]?["content"]?.ToString()?.Trim() ?? "Sem resposta da API.";
            }
            catch (Exception ex)
            {
                return $"Erro ao enviar mensagem para o Ollama: {ex.Message}";
            }
        }
    }
}
