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
        public int? TypeId { get; set; }
        [ForeignKey("TypeId")]
        public HelperType? HelperType { get; set; } = null;
        [NotMapped]
        public List<string> ImagesList { get { return Images.Split(';').ToList(); }}
        [NotMapped]
        public List<string> KeysList { get { return Keys.Split(';').ToList(); } }

        public HelperInfo(string helperInfoText, string images)
        {
            HelperInfoText = helperInfoText;
            Images = images;
        }
        public HelperInfo(string helperInfoText, string title, string images, string keys, int? typeId)
        {
            HelperInfoText = helperInfoText;
            Images = images;
            Keys = keys;
            TypeId = typeId;
            Title = title;
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
