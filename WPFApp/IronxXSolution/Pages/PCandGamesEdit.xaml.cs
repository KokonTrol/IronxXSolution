using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using Library.Functions;
using GalaSoft.MvvmLight.Command;

namespace IronxXSolution.Pages
{
    public partial class PCandGamesEdit : Page, INotifyPropertyChanged
    {
        public IronContext db;

        public PCandGamesEdit()
        {
            db = new IronContext();
            InitializeComponent();
            DataContext = this;
            AddItemCommand = new RelayCommand<object>(obj => AddItem(obj));
            DeleteItemCommand = new RelayCommand<object>(obj => DeleteItem(obj));
            SaveChangesCommand = new RelayCommand(SaveChanges);

            DataContext = this;
        }

        #region список пк
        public IEnumerable<Computer> PC
        {
            get
            {
                db.Computers.Load();
                return db.Computers.Local.OrderBy(c => c.Name).ToList();
            }
            set
            {
                db.SaveChanges();
                OnPropertyChanged("PC");
            }
        }
        #endregion

        #region список игр
        public IEnumerable<Game> Games
        {
            get
            {
                db.Games.Load();
                return db.Games.Local.OrderBy(c => c.Name).ToList();
            }
            set
            {
                db.SaveChanges();
                OnPropertyChanged("Games");
            }
        }
        #endregion

        #region список лаунчеров
        public IEnumerable<LaunchersInfo> Launchers
        {
            get
            {
                db.LaunchersInfos.Load();
                return db.LaunchersInfos.Local.OrderBy(c => c.Name).ToList();
            }
            set
            {
                db.SaveChanges();
                OnPropertyChanged("Launchers");
            }
        }
        #endregion


        #region добавление нового элемента
        public ICommand AddItemCommand { get; set; }
        private void AddItem(object obj)
        {
            var tab = obj as TabItem;
            if (tab != null)
            {
                string log = "Добавление повторного элемента";

                if (tab.Tag.ToString() == "launchers")
                {
                    db.LaunchersInfos.Add(new LaunchersInfo());
                    db.SaveChanges();

                    OnPropertyChanged("launchers");
                }
                else
                {
                    if (tab.Tag.ToString() == "pc")
                    {
                        var newPC = new Computer();
                        newPC.Name = newPC.Name + db.Computers.Count().ToString();
                        db.Computers.Add(newPC);
                        db.SaveChanges();

                        foreach (var game in Games)
                        {
                            db.Updates.Add(new ComputerGame
                            {
                                PcID = (db.Computers.Where(n => n.Name == newPC.Name).First().Id),
                                Computer = null,
                                GameID = game.Id,
                                Game = null,
                                Date = DateTime.FromBinary(1),
                                LastDate = DateTime.FromBinary(1)
                            });
                            db.SaveChanges();
                        }
                        log = $"Добавление нового ПК в базу. ПК: {newPC.Name}.";

                    }
                    else if (tab.Tag.ToString() == "games")
                    {
                        var newGame = new Game();
                        newGame.Name = newGame.Name + db.Games.Count().ToString();
                        db.Games.Add(newGame);
                        db.SaveChanges();

                        foreach (var pc in PC)
                        {
                            db = new IronContext();
                            db.Updates.Add(new ComputerGame
                            {
                                PcID = pc.Id,
                                Computer = null,
                                GameID = (db.Games.Where(n => n.Name == newGame.Name).First().Id),
                                Game = null,
                                Date = DateTime.FromBinary(1),
                                LastDate = DateTime.FromBinary(1)
                            });
                            db.SaveChanges();
                        }

                        log = $"Добавление новой игры в базу. Игра: {newGame.Name}.";
                    }
                    BaseFunctions.NewLogLine(log, null);
                    OnPropertyChanged("Games");
                    OnPropertyChanged("PC");
                }
            }
        }
        #endregion


        #region удаление элемента
        public ICommand DeleteItemCommand { get; set; }
        private void DeleteItem(object deletingParameter)
        {
            IList<object> parameters = deletingParameter as IList<object>;

            if (parameters != null)
            {
                string log = "";
                if (((TabItem)parameters[0]).Tag.ToString() == "pc" && parameters[1] != null)
                {
                    Computer pc = parameters[1] as Computer;

                    db.Computers.Remove(pc);
                    db.SaveChanges();

                    log = $"Удаление ПК из базы.\nID: {pc.Id}; ПК: {pc.Name}.";

                }
                else if (((TabItem)parameters[0]).Tag.ToString() == "games" && parameters[2] != null)
                {
                    Game game = parameters[2] as Game;
                    db.Games.Remove(game);
                    db.SaveChanges();

                    log = $"Удаление игры из базы.\nID: {game.Id}; ПК: {game.Name}.";
                }
                else if (((TabItem)parameters[0]).Tag.ToString() == "launchers" && parameters[3] != null)
                {
                    LaunchersInfo launcher = parameters[3] as LaunchersInfo;
                    db.LaunchersInfos.Remove(launcher);
                    db.SaveChanges();

                    log = $"Удаление информации о лаунчере из базы.\nID: {launcher.Id}; Лаунчер: {launcher.Name}.";
                }
                BaseFunctions.NewLogLine(log, null);
                OnPropertyChanged("Games");
                OnPropertyChanged("PC");
                OnPropertyChanged("Launchers");

            }
        }
        #endregion

        public ICommand SaveChangesCommand { get; set; }
        private void SaveChanges()
        {
            db.SaveChanges();
        }


        #region propertyChangrd
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
