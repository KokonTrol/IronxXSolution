using GalaSoft.MvvmLight.Command;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace IronXGameUpdate.ViewModel
{
    #region управление окном редактирования данных
    public class ViewModelEdit : INotifyPropertyChanged
    {
        public IronContext db;

        //родительское управление
        ViewModelMain Parent;
        public ViewModelEdit(ViewModelMain parent)
        {
            AddItemCommand = new RelayCommand<object>(obj => AddItem(obj));
            DeleteItemCommand = new RelayCommand<object>(obj => DeleteItem(obj));
            db = new IronContext();

            Parent = parent;
        }

        #region список пк
        public IEnumerable<Computer> PC
        {
            get
            {
                //db = new IronContext();
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
                //db = new IronContext();
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
                //db = new IronContext();
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
                    var askName = new AskNewElement();
                    if (askName.ShowDialog() == true)
                    {
                        if (tab.Tag.ToString() == "pc" && db.Computers.Where(c => c.Name == askName.newName).Count() == 0)
                        {
                            var newPC = new Computer { Name = askName.newName };
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
                            log = $"Добавление нового ПК в базу. ПК: {newPC.Name}.\nАдмин: {Parent._admin.Name}";

                        }
                        else if (tab.Tag.ToString() == "games" && db.Games.Where(c => c.Name == askName.newName).Count() == 0)
                        {
                            var newGame = new Game { Name = askName.newName };
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

                            log = $"Добавление новой игры в базу. Игра: {newGame.Name}.\nАдмин: {Parent._admin.Name}";
                        }
                    }
                    Parent.NewLogLine(log);
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

                    log = $"Удаление ПК из базы.\nID: {pc.Id}; ПК: {pc.Name}.\nАдмин: {Parent._admin.Name}";

                }
                else if (((TabItem)parameters[0]).Tag.ToString() == "games" && parameters[2] != null)
                {
                    Game game = parameters[2] as Game;
                    db.Games.Remove(game);
                    db.SaveChanges();

                    log = $"Удаление игры из базы.\nID: {game.Id}; ПК: {game.Name}.\nАдмин: {Parent._admin.Name}";
                }
                else if (((TabItem)parameters[0]).Tag.ToString() == "launchers" && parameters[3] != null)
                {
                    LaunchersInfo launcher = parameters[3] as LaunchersInfo;
                    db.LaunchersInfos.Remove(launcher);
                    db.SaveChanges();

                    log = $"Удаление информации о лаунчере из базы.\nID: {launcher.Id}; Лаунчер: {launcher.Name}.";
                }
                Parent.NewLogLine(log);
                OnPropertyChanged("Games");
                OnPropertyChanged("PC");
                OnPropertyChanged("Launchers");

            }
        }
        #endregion

        public void OnWindowClosing(object sender, CancelEventArgs e)
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
    #endregion
}
