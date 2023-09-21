using System.Text.Json;
using System.Windows;
using Library.Functions;
using Library.Models;

namespace IronxXCashier
{
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            Admin? admin = BaseFunctions.FindAdminByArgs(e);

            if (admin != null)
            { 
                bool isAdmin = BaseFunctions.IsSuperUser(e);
                Working working = new Working(admin, isAdmin);
                working.Show();
            }
            else
            {
                var data = JsonDocument.Parse(BaseFunctions.GetStringFileFromResources("DBConfig.json"));
                MessageBox.Show( data.RootElement.GetProperty("EnterAppError").ToString(), "Ошибка");
                this.Shutdown();
            }
        }
    }
}
