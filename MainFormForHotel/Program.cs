using Microsoft.Playwright;

namespace MainFormForHotel
{
    class Program
    {
 
        [STAThread]
        public static void Main()  
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

    }
}