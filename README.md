# Gestão de Chamados

Este é o repositório do sistema de **Gestão de Chamados**, desenvolvido em C#, integrando recursos de Inteligência Artificial e banco de dados PostgreSQL.

## 🚀 Funcionalidades Principais

* **Cadastro e Gerenciamento de Chamados:** Criação, edição, exclusão e visualização de chamados.
* **Painel de Controle por Perfil:** Telas customizadas para Usuário, Funcionário e Administrador.
* **Autenticação e Autorização:** Login com verificação de credenciais armazenadas no PostgreSQL.
* **Integração com IA via Ollama:** Assistente virtual para suporte inicial e sugestão de soluções antes de abrir um novo chamado.
* **Banco de Dados PostgreSQL:** Armazenamento de todas as informações de usuários, chamados e históricos.

## 🛠️ Tecnologias Utilizadas

* Linguagem: C# (.NET Framework)
* Interface Gráfica: Windows Forms
* Banco de Dados: PostgreSQL
* IA: Ollama (modelo local de linguagem)
* ORMs / Drivers: Npgsql

## 📦 Instalação

1. **Clone o repositório:**

   ```bash
   git clone https://github.com/SEU_USUARIO/gestaodechamados.git
   cd gestaodechamados
   ```

2. **Configuração do Banco de Dados:**

   * Instale e configure o PostgreSQL.
   * Crie um banco de dados chamado `GestaoChamados`.
   * Execute o script de criação de tabelas em `database/schema.sql`.

3. **Configuração do Ollama:**

   * Instale o Ollama seguindo a [documentação oficial](https://ollama.com/docs).
   * Baixe e configure o modelo de linguagem desejado (ex: `ollama pull llama2`).
   * Ajuste o endpoint de conexão no arquivo `appsettings.json`.

4. **Build do Projeto:**

   * Abra a solução `GestaoDeChamados.sln` no Visual Studio.
   * Restaure os pacotes NuGet.
   * Compile e execute.

## 💡 Como Usar

1. **Login:** Insira seu usuário e senha.
2. **Tela Inicial:** Escolha entre visualizar chamados abertos, em andamento, vencidos ou resolvidos.
3. **Criar Chamado:** Clique em `Novo Chamado` e preencha os detalhes.
4. **Assistente IA:** Utilize a aba de chat com o modelo Ollama para diagnóstico inicial.
5. **Gerenciamento:** Edite ou feche chamados conforme necessário.

