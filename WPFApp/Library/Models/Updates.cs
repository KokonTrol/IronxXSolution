using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    [Table("Updates")]
    public class ComputerGame : INotifyPropertyChanged
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PcID { get; set; }      // внешний ключ
        [ForeignKey("PcID")]
        public Computer Computer { get; set; }

        [Required]
        public int GameID { get; set; }      // внешний ключ
        [ForeignKey("GameID")]
        public Game Game { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.FromBinary(1);
        public DateTime LastDate { get; set; } = DateTime.FromBinary(1);


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
