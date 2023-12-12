using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SysAdminApp
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
        public HelperType HelperType { get; set; } = null;

        public HelperInfo(string helperInfoText, string images)
        {
            HelperInfoText = helperInfoText;
            Images = images;
        }
        public HelperInfo() { }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
