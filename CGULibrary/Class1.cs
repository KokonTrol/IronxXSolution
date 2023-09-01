using System.Text.Json;
using System.Reflection;
using System.Media;
using Library.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace CGULibrary
{
    class GameDllInfo
    {
        public int ID { get; set; }
        public string GameName { get; set; }
        public string DllName { get; set; } = "";
        public DateTime LastDateUpdate { get; set; } = new DateTime();
        public DateTime FoundedUpdate { get; set; } = DateTime.Now;
        public GameDllInfo(int id, string gameName, string dllName, DateTime lastDateUpdate, DateTime foundedUpdate)
        {
            ID = id;
            GameName = gameName;
            DllName = dllName;
            LastDateUpdate = lastDateUpdate;
            FoundedUpdate = foundedUpdate;
        }
    }
    public static class Checking
    {
        static string _folderPath = "";
        static List<GameDllInfo> _gameDllInfos;
        static SoundPlayer _sp;
        static int _timeout = 0;
        static bool _canPlaySound = true;
        static string _longLine = "------------------------------";
        static bool _isConsole;
        static IronContext db;

        static private async Task<List<GameDllInfo>> GetGamesIDsFromJson(string _folderPath)
        {
            using (FileStream fs = new FileStream(_folderPath + "Data\\GamesList.json", FileMode.OpenOrCreate))
            {
                try
                {
                    return await JsonSerializer.DeserializeAsync<List<GameDllInfo>>(fs);
                }
                catch (Exception ex)
                {
                    return new List<GameDllInfo>();
                }
            }
        }

        public static async Task Start(string[] args, bool isConsole = false)
        {
            string temp = "";
            _isConsole = isConsole;
            db = new IronContext();
            if (!Directory.Exists(_folderPath + "Data"))
            {
                Directory.CreateDirectory(_folderPath + "Data");
            }


            if (_isConsole)
            {
                Console.Clear();
            }
            if (args.Length >= 2)
            {
                for (int i = 0; i < args.Length; i += 2)
                {
                    try
                    {
                        if (args[i] == "-FP")
                        {
                            _folderPath = args[i + 1] + "\\";
                        }
                        else if (args[i] == "-TO")
                        {
                            temp = args[i + 1];
                            Int32.TryParse(temp, out _timeout);
                            if (_timeout > 180 || _timeout <= 0)
                            {
                                _timeout = 60;
                            }
                        }
                        else if (args[i] == "-PS")
                        {
                            _canPlaySound = args[i + 1] == "yes";
                        }
                    }
                    catch { }
                }
            }


            if (_canPlaySound && File.Exists("update_sound.wav"))
            {
                Assembly assembly;
                assembly = Assembly.GetExecutingAssembly();
                _sp = new SoundPlayer();
                _sp.SoundLocation = "update_sound.wav";
            }
            else
            {
                _canPlaySound = false;
            }

            //set timout for task
            if (_isConsole)
            {
                if (_timeout == 0)
                {
                    while (true)
                    {
                        Console.Write("С каким периодом (в минутах) осуществлять проверку обновлений? Максимально допустимое значение: 180\nМинут: ");
                        temp = Console.ReadLine();
                        if (!Int32.TryParse(temp, out _timeout) || _timeout > 180 || _timeout <= 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Введено не целое число.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
                

            //write preferences 
            if (_isConsole)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(_longLine);
                Console.WriteLine($"Итоговые установки:\n" +
                    $"Период проверки обновлений: {_timeout} мин;\n" +
                    $"Звуковое оповещение: {(_canPlaySound ? "да" : "нет")}.");
                Console.WriteLine(_longLine);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }
            _timeout *= 60 * 1000;

            try
            {
                await CheckingTask();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Произошел сбой в программе. Информация о возникшем исключении записана в файл \"log.txt\"");
                File.AppendAllText("log.txt", $"[{DateTime.Now}] {ex.Message}\n{ex}\n\n");
            }
        }

        static void SetNewDate( GameDllInfo game, DateTime date, DateTime dateNowCicle)
        {
            if (_isConsole)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{dateNowCicle:T} ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write($"НАЙДЕНО ОБНОВЛЕНИЕ ДЛЯ ИГРЫ ");
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{game.GameName}");
                Console.BackgroundColor = ConsoleColor.Black;

                File.AppendAllText("logUpdates.txt", $"[{DateTime.Now}] {game.GameName} от {game.LastDateUpdate}\n");
            }

            db.CheckUpdates.Add(new CheckUpdate(gameName: game.GameName, date: game.LastDateUpdate, foundedDate: game.FoundedUpdate));
            db.SaveChanges();

            if (_canPlaySound)
            {
                _sp.Play();
            }
        }

        static async Task CheckingTask()
        {
            _gameDllInfos = (await GetGamesIDsFromJson(_folderPath));


            #region Finding new games
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\Data");

            FileInfo[] Files = dir.GetFiles("*CheckUpdate.dll");

            foreach (FileInfo file in Files)
            {
                string fiWOExt = Path.GetFileNameWithoutExtension(file.Name);
                if (_gameDllInfos.Where(d => d.DllName == fiWOExt).Count() == 0)
                {
                    _gameDllInfos.Add(new GameDllInfo(
                        _gameDllInfos.Count() + 1,
                        "",
                        fiWOExt,
                        new DateTime(0),
                        new DateTime(0)));
                    try
                    {
                        Assembly a = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "\\Data\\" + file.Name);

                        Type classType = a.GetType($"{fiWOExt}.CheckGameUpdate");
                        MethodInfo mi = classType.GetMethod("GameName");
                        _gameDllInfos.First(d => d.DllName == fiWOExt).GameName = (string)mi.Invoke(null, null);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
            #endregion

            if (_isConsole)
            {
                Console.WriteLine("Доступен поиск для таких игр, как:");
                _gameDllInfos = _gameDllInfos.OrderBy(g => g.GameName).ToList();
                foreach (GameDllInfo game in _gameDllInfos)
                {
                    Console.WriteLine($"{game.GameName}");
                }
                Console.WriteLine(_longLine);
                Console.WriteLine();
            }

            DateTime dateNowCicle = DateTime.Now;
            while (true)
            {
                if (_isConsole)
                {
                    dateNowCicle = DateTime.Now;
                    Console.WriteLine($"{dateNowCicle:T} Начало цикла проверки");
                }
                foreach (GameDllInfo game in _gameDllInfos)
                {
                    if (game.DllName != "" && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Data\\" + game.DllName + ".dll"))
                    {
                        try
                        {
                            Assembly a = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "\\Data\\" + game.DllName + ".dll");
                            Type classType = a.GetType($"{game.DllName}.CheckGameUpdate");
                            MethodInfo mi = classType.GetMethod("Check");
                            DateTime date = (DateTime)mi.Invoke(null, null);

                            var upd = db.CheckUpdates.Where(u => u.GameName == game.GameName).ToList();
                            if (upd.Count() == 0)
                            {
                                game.LastDateUpdate = date;
                                game.FoundedUpdate = DateTime.Now;
                                SetNewDate(game, date, dateNowCicle);
                            }
                            else
                            if (date > upd.Last().Date)
                            {
                                game.LastDateUpdate = date;
                                game.FoundedUpdate = DateTime.Now;
                                SetNewDate( game, date, dateNowCicle);
                            }
                            else
                            {
                                if (_isConsole)
                                {
                                    Console.WriteLine($"\t{game.GameName} без обновлений");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }

                string json = JsonSerializer.Serialize(_gameDllInfos);

                File.WriteAllText(_folderPath + "Data\\GamesList.json", json);
                //Console.WriteLine("Запись выполнена");
                //Console.ReadKey();
                if (_isConsole)
                {
                    dateNowCicle = DateTime.Now;
                    Console.Write($"{dateNowCicle:T} Конец цикла проверки\n");
                    Console.WriteLine(_longLine);
                }
                Thread.Sleep(_timeout);
            }
        }
    }
}
