using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class HelperInfo : INotifyPropertyChanged
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string HelperInfoText { get; set; } = "";
        public string Images { get; set; } = "";
        public string Keys { get; set; } = "";
        public string Type { get; set; } = "";


        public HelperInfo(string helperInfoText, string images)
        {
            HelperInfoText = helperInfoText;
            Images = images;
        }
        public HelperInfo() { }

        public List<string> GetImagesNames()
        {
            return Images.Split(';').ToList();
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
