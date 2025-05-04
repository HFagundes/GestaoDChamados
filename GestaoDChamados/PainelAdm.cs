using System.Windows.Forms;
using System.Drawing;

namespace ChamadosApp
{
    public class AdminForm : Form
    {
        public AdminForm()
        {
            Text = "Painel do Administrador";
            this.Size = new Size(1200, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable; // Alterado para permitir maximizar
            this.MaximizeBox = true; // Alterado para permitir maximizar
            this.BackColor = Color.White;
        }
    }
}