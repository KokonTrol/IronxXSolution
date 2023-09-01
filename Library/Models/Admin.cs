using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class Admin : INotifyPropertyChanged
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = "Новый_админ";
        public double Salary { get; set; } = 0;
        public string Password { get; set; } = "";
        public bool IsActive { get; set; } = true;

        public virtual List<Transaction> Transactions { get; set; } = null;

        public override string ToString()
        {
            return Name;
        }

        public string ToCommandParameters()
        {
            return $"-AN {Name} -AP {Password} ";
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
