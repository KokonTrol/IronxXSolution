using Library.Models;
using System.IO;
using System.Linq;
using System.Windows;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Reflection;

namespace Library.Functions
{
    public static class BaseFunctions
    {
        static void ShowMessage(string message, MessageBoxImage icon)
        {
            MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, icon);
        }
        public static Admin? FindAdminByArgs(StartupEventArgs e)
        {
            IronContext context = new IronContext();
            string _errorEnter = "При вызове произошла ошибка передачи данных о пользователе. Обычно, такой ошибки быть не должно, если запускать программу через Окно входа. Свяжитесь с разработчиком.";
            string _adminName = "";
            string _adminPassword = "";

            try
            {
                for (int i = 0; i != e.Args.Length; i += 2)
                {
                    if (e.Args[i] == "-AN")
                    {
                        _adminName = e.Args[i + 1];
                    }
                    if (e.Args[i] == "-AP")
                    {
                        _adminPassword = e.Args[i + 1];
                    }
                }
                if (_adminName == "" || _adminPassword == "")
                {
                    //ShowMessage(_errorEnter, MessageBoxImage.Error);
                    return null;
                }
                else
                {
                    Admin admin = context.Admin.FirstOrDefault(p => p.Name == _adminName && p.Password == _adminPassword);
                    if (admin != null)
                    {
                        return admin;
                    }
                    else
                    {
                        ShowMessage("Пользователя с данным именем и паролем не найдено. Попробуйте снова.", MessageBoxImage.Warning);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(_errorEnter, MessageBoxImage.Error);
                return null;
            }
        }

        public static bool IsSuperUser(StartupEventArgs e)
        {
            bool isSuperUser = false;
            for (int i = 0; i != e.Args.Length; i += 2)
            {
                if (e.Args[i] == "-SU")
                {
                    isSuperUser = Convert.ToBoolean(e.Args[i + 1]);
                }
            }
            return isSuperUser;
        }

        public static string GetStringFileFromResources(string filename)
        {
            //Uri uri = new Uri(@"pack://application:,,,/Library;component/Resources/" + filename, UriKind.Absolute); //@"pack://application:,,,
            string result;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Library.Resources." + filename;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            //using (Stream stream = Application.GetResourceStream(uri).Stream)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }

        public static Config SetDefaultConfig()
        {
            string result = GetStringFileFromResources("ConfigDefault.json");
            Config newConfig = JsonSerializer.Deserialize<Config>(result)!;

            return newConfig;
        }
        public static string GetDocumentFolder()
        {
            var data = JsonDocument.Parse(BaseFunctions.GetStringFileFromResources("DBConfig.json"));
            string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" +
                            data.RootElement.GetProperty("DocumentsFolder").ToString();

            return documentPath;
        }
        public static void CreateDocumentsFolder()
        {
            string documentPath = GetDocumentFolder();

            if (!Directory.Exists(documentPath))
                Directory.CreateDirectory(documentPath);
        }

        public static async Task CreateDBBackup()
        {

        }
    }
}
