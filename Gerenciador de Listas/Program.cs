using System;
using System.Windows.Forms;

namespace Gerenciador_de_Listas
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Garante que o Windows Forms use a cultura correta para pontos e vírgulas
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            
            Application.Run(new MainForm());
        }
    }
}