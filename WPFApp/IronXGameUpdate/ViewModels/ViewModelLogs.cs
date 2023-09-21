using Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System;

namespace IronXGameUpdate.ViewModel
{
    #region управление окном логов
    public class ViewModelLogs : INotifyPropertyChanged
    {
        public IronContext db;
        private DateTime? minSelectedDate = DateTime.Today;
        public DateTime? MinSelectedDate
        {
            get
            {
                return minSelectedDate;
            }
            set
            {
                minSelectedDate = value;
                OnPropertyChanged("MinSelectedDate");
            }
        }
        private DateTime? maxSelectedDate = DateTime.Today;
        public DateTime? MaxSelectedDate
        {
            get
            {
                return maxSelectedDate;
            }
            set
            {
                maxSelectedDate = value;
                OnPropertyChanged("MaxSelectedDate");
                EndDateCalendarChanged();
            }
        }

        public ViewModelLogs() { 
            db = new IronContext();
            FindLogsByDateCommand = new RelayCommand(FindLogsByDate);
        }

        public IEnumerable<Log> Logs
        {
            get
            {
                db.Logs.Load();
                return db.Logs.Local.Where(l=>l.Date.Date <= MaxSelectedDate && l.Date.Date >= MinSelectedDate)
                    .OrderByDescending(c => c.Date).ToList();
            }
            set { OnPropertyChanged("Logs"); }
        }

        private void EndDateCalendarChanged()
        {
            if (MaxSelectedDate != null)
            {
                if (MinSelectedDate > MaxSelectedDate)
                {
                    MinSelectedDate = MaxSelectedDate;
                }
            }
        }

        public ICommand FindLogsByDateCommand { get; set; }
        private void FindLogsByDate()
        {
            OnPropertyChanged("Logs");
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
