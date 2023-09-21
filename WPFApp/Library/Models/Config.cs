using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class Config : INotifyPropertyChanged
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SuperUserPassword { get; set; } = "";
        public int NightWork { get; set; } = 21;
        public int DayWork { get; set; } = 9;
        public string HelperInfoVer { get; set; } = "";

        public Config(string password, int dayWork, int nightWork)
        {
            SuperUserPassword = password;
            NightWork = nightWork;
            DayWork = dayWork;
        }
        public Config() { }

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
