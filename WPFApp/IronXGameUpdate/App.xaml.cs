using Library.Functions;
using Library.Models;
using System.Text.Json;
using System.Windows;

namespace IronXGameUpdate
{
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            Admin? admin = BaseFunctions.FindAdminByArgs(e);
            if (admin != null)
            {
                bool isAdmin = BaseFunctions.IsSuperUser(e);
                MainWindow mainWindow = new MainWindow(admin, isAdmin);
                mainWindow.Show();
            }
            else
            {
                MainWindow mainWindow = new MainWindow(new Admin(), true);
                mainWindow.Show();
                //var data = JsonDocument.Parse(BaseFunctions.GetStringFileFromResources("DBConfig.json"));
                //MessageBox.Show(data.RootElement.GetProperty("EnterAppError").ToString(), "Ошибка");
                //this.Shutdown();
            }
        }
    }
}
