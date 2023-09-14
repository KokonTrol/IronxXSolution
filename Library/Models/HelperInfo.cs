using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class HelperInfo : INotifyPropertyChanged
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string HelperInfoText { get; set; } = "";
        [NotMapped]
        public byte[][] Images { get; set; } = null;
        public HelperInfo(string helperInfoText, byte[][] images)
        {
            HelperInfoText = helperInfoText;
            Images = images;
        }
        public HelperInfo() { }

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
