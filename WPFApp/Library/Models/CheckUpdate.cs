using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class CheckUpdate : INotifyPropertyChanged
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string GameName { get; set; } = "";
        public DateTime Date { get; set; } = new DateTime(0);
        public DateTime FoundedDate { get; set; } = new DateTime(0);

        public CheckUpdate(string gameName, DateTime date, DateTime foundedDate)
        {
            GameName = gameName;
            Date = date;
            FoundedDate = foundedDate;
        }

        public override string ToString()
        {
            return $"[{FoundedDate.ToString("dd.MM HH:mm")}] {GameName}";
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
