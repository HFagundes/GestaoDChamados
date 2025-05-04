using System;
using System.Windows.Forms;
using System.Drawing;

namespace ChamadosApp
{
    public class FuncionarioForm : Form
    {
        public FuncionarioForm()
        {
            // Configurações do Formulário
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Painel do Funcionário"; // Título da janela
            this.Size = new Size(800, 600); // Tamanho do formulário
            this.StartPosition = FormStartPosition.CenterScreen; // Posição inicial no centro da tela
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Não permite maximizar o formulário
            this.MaximizeBox = false; // Desabilita a maximização

            // Outras configurações e elementos do formulário podem ser adicionados aqui conforme necessário
        }
    }
}