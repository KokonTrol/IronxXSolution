using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using System.Windows;
using System.Threading;

namespace IronXGameUpdate.ViewModel
{
    #region главное управление
    public class ViewModelMain : INotifyPropertyChanged
    {
        public IronContext db;
        public Admin _admin;
        bool _isAdmin;

        #region таблица обновлений
        public IEnumerable<IEnumerable<ComputerGame>> Updates
        {
            get
            {
                db = new IronContext();
                db.Updates.Load();
                var groups = db.Updates.AsEnumerable().OrderBy(u => u.PcID).GroupBy(u => u.PcID);

                IEnumerable<IEnumerable<ComputerGame>> localUpdates = new List<IEnumerable<ComputerGame>>();

                foreach (var group in groups)
                    localUpdates = localUpdates.Append(group.Join(
                        db.Games, u => u.GameID, g => g.Id,
                        (u, g) => new
                        {
                            GameName = g.Name,
                            GameID = g.Id,
                            PcID = u.PcID,
                            Date = u.Date
                        }).Join(
                        db.Computers, u => u.PcID, c => c.Id,
                        (u, c) => new
                        {
                            GameName = u.GameName,
                            GameID = u.GameID,
                            PcID = u.PcID,
                            Date = u.Date,
                            Color = c.Color
                        })
                        .OrderBy(g => g.GameName).Select(u => new ComputerGame
                        {
                            GameID = u.GameID,
                            PcID = u.PcID,
                            Date = u.Date
                        }).ToList());

                return localUpdates;
            }
            set { OnPropertyChanged("Updates"); }
        }
        #endregion

        #region Игры в алфавитном порядке
        public IEnumerable<Game> Games
        {
            get
            {
                db = new IronContext();
                db.Games.Load();
                return db.Games.ToList().OrderBy(g => g.Name).ToList();
            }
            set { OnPropertyChanged("Games"); }
        }
        #endregion

        #region список лаунчеров
        public IEnumerable<LaunchersInfo> Launchers
        {
            get
            {
                //db = new IronContext();
                db.LaunchersInfos.Load();
                return db.LaunchersInfos.Local.OrderBy(c => c.Name).ToList();
            }
            set { OnPropertyChanged("Launchers"); }
        }
        #endregion

        #region список обновлений
        public IEnumerable<CheckUpdate> CheckUpdatesList
        {
            get
            {
                var db1 = new IronContext();
                db1.CheckUpdates.Load();
                return db1.CheckUpdates.Local.Where(u => u.FoundedDate > DateTime.Now.AddDays(-2)).OrderByDescending(c => c.FoundedDate).ToList();
            }
            set { OnPropertyChanged("CheckUpdatesList"); }
        }
        #endregion

        public ViewModelMain(Admin admin, bool isAdmin)
        {
            db = new IronContext();
            _admin = admin;
            _isAdmin = isAdmin;

            ChangeUpdateCommand = new RelayCommand<object>(obj => UpdateGame(obj));
            ShowLogsCommand = new RelayCommand(ShowLogs);
            ChangeAdminNameCommand = new RelayCommand(ChangeAdminName);

            MessageBox.Show("Пожалуйста, введите свой ник в следующем диалоговом окне");

            ChangeAdminName();

            IsNewGameUpdates();
        }

        #region смена имени админа
        public ICommand ChangeAdminNameCommand { get; set; }
        private void ChangeAdminName()
        {
            var askName = new AskNewElement();
            if (askName.ShowDialog() == true)
            {
                _admin.Name = askName.newName;
                //NewLogLine($"Админ {_admin.Name} вошел.");
            }
        }
        #endregion

        #region показать логи
        public ICommand ShowLogsCommand { get; set; }
        private void ShowLogs()
        {
            ViewLogs view = new ViewLogs();
            view.ShowDialog();
        }
        #endregion


        #region команда обновления таблицы обновления
        public ICommand ChangeUpdateCommand { get; set; }
        private async void UpdateGame(object newUpdateParameters)
        {
            await Task.Run(() =>
            {
                try
                {
                    //db = new IronContext();
                    IList<object> parameters = newUpdateParameters as IList<object>; //игра - пк
                    var line = db.Updates.Where(u => u.PcID == (int)parameters[1] &&
                            u.GameID == (int)parameters[0]).First();

                    string log = "";
                    string baseLog = $"ПК: {db.Computers.Where(c => c.Id == line.PcID).First().Name}, " +
                            $"Игра: {db.Games.Where(c => c.Id == line.GameID).First().Name}.\n" +
                            $"Админ: {_admin.Name}";

                    if (line.Date.Date != DateTime.Today)
                    {
                        line.LastDate = line.Date;
                        line.Date = DateTime.Now; //.AddHours(-db.GetConfig().DayWork)
                        db.Updates.Update(line);

                        log = $"Смена даты обновления ({line.LastDate} -> {line.Date}).\n" + baseLog;
                    }
                    else
                    {
                        var date = line.Date;
                        line.Date = line.LastDate;
                        line.LastDate = date;

                        db.Updates.Update(line);

                        log = $"Убрано обозначение обновления.\n" + baseLog;
                    }
                    db.SaveChangesAsync();
                    //if (log != "")
                    //    NewLogLine(log);
                    OnPropertyChanged("Updates");
                }
                catch { }
            });

        }
        #endregion


        #region проверка обновлений

        private async void IsNewGameUpdates()
        {
            Task[] checking = new Task[2];
            checking[0] = Task.Run(() => CGULibrary.Checking.Start(new string[] { "-TO", "30" }, false));
            checking[1] = Task.Run(() =>
            {
                while (true)
                {
                    OnPropertyChanged("CheckUpdatesList");
                    Thread.Sleep(10000);
                }
            });

            await Task.WhenAll(checking);
        }
        #endregion


        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
    #endregion
}
