using Library.Functions;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;

namespace IronXHelper
{
    public static class Downloader
    {
        private static string path = "IronXHelper";
        private static string fileVer = "HelperVer";
        private static string fileText = "HelperText.json";
        private static string tempHelperFolder = BaseFunctions.GetDocumentFolder() + "\\TempHelper\\";
        private static string helperFolder = BaseFunctions.GetDocumentFolder() + "\\Helper\\";
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true
        };

        private static string GetKey()
        {
            string key;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "IronXHelper.privateKey";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    key = reader.ReadToEnd();
                }
            }
            return key;
        }

        private static void DeleteFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (IOException)
                {
                    Thread.Sleep(0);
                    Directory.Delete(folder, true);
                }
            }
        }
        private static void CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
        private static async Task<bool> FillHelperDB()
        {
            List <HelperInfo> tempHelpers;
            using (FileStream fs = new FileStream(Path.Combine(tempHelperFolder, fileText), FileMode.Open))
            {
                try
                {
                    tempHelpers = await JsonSerializer.DeserializeAsync<List<HelperInfo>>(fs);
                    tempHelpers.ForEach(x => x.HelperInfoText = x.HelperInfoText.Replace("\\n", "\n"));
                }
                catch (Exception ex)
                {
                    tempHelpers = new List<HelperInfo>();
                }
            }

            IronContext context = new IronContext();
            context.HelperInfo.Load();

            {
                string json = JsonSerializer.Serialize(context.HelperInfo.Select(x => new
                {
                    x.Title,
                    x.HelperInfoText,
                    x.Images,
                    x.Type,
                    x.Keys
                }).ToList(),jsonOptions);
                string tempFolder = BaseFunctions.GetDocumentFolder() + $"\\Temp";
                string tempFile = $"\\HelperInfo{DateTime.Now.ToFileTime()}.json";

                CreateFolder(tempFolder);
                File.WriteAllText(tempFolder + tempFile, json);
            }


            context.HelperInfo.RemoveRange(context.HelperInfo.ToList());

            foreach (var content in tempHelpers)
            {
                context.HelperInfo.Add(content);
            }
            context.SaveChanges();

            DeleteFolder(helperFolder);
            //CreateFolder(helperFolder);
            Directory.Move(tempHelperFolder, helperFolder);
            return true;
        }

        private static async Task<bool> GetFiles(DiskHttpApi api, string helperVer)
        {
            File.Delete(Path.Combine(tempHelperFolder, fileVer));
            Resource resources = await api.MetaInfo.GetInfoAsync(new ResourceRequest
            {
                Path = path,
            }, CancellationToken.None);

            IEnumerable<Resource> allFiles =
                resources.Embedded.Items.Where(item => item.Type == ResourceType.File);

            IEnumerable<Task> downloadingTasks =
                allFiles.Select(file =>
                  api.Files.DownloadFileAsync(file.Path, localFile: Path.Combine(tempHelperFolder, file.Name)));

            await Task.WhenAll(downloadingTasks);

            BaseFunctions.SetHelperVer(helperVer);

            return await FillHelperDB();
        }


        public static async Task<bool> DownloadInfo()
        {
            string key = GetKey();

            var api = new DiskHttpApi(key);

            DeleteFolder(tempHelperFolder);
            CreateFolder(tempHelperFolder);
            
            try
            {
                await api.Files.DownloadFileAsync("disk:/" + path + "/" + fileVer, Path.Combine(tempHelperFolder, fileVer));

                string helperVer = File.ReadAllText(Path.Combine(tempHelperFolder, fileVer));

                if (helperVer == BaseFunctions.GetHelperVer())
                {
                    MessageBoxResult dialogResult = MessageBox.Show("Обновление не требуется.\n\nВыполнить принудительное обновление файлов?", "Обновление", MessageBoxButton.YesNo);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        return await GetFiles(api, helperVer);
                    }
                    DeleteFolder(tempHelperFolder);
                    return true;
                }
                return await GetFiles(api, helperVer);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Невозможно подключится к источнику.\n{ex.Message}\n{ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                DeleteFolder(tempHelperFolder);
                return false;
            }
        }
    }
}