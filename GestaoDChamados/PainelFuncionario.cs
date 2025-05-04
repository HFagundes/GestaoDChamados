using System.Windows.Forms;
using System.Drawing;

namespace ChamadosApp
{
    public class FuncionarioForm : Form
    {
        public FuncionarioForm()
        {
            Text = "Painel do Funcionario";
            this.Size = new Size(1200, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable; // Alterado para permitir maximizar
            this.MaximizeBox = true; // Alterado para permitir maximizar
            this.BackColor = Color.White;
        }
    }
}