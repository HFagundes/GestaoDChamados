using System.Windows.Forms;
using System.Drawing;

namespace ChamadosApp
{
    public class FuncionarioForm : Form
    {
        public FuncionarioForm()
        {
            Text = "Painel do Administrador";
            Size = new Size(800, 600);
            StartPosition = FormStartPosition.CenterScreen;
        }
    }
}