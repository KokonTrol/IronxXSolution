using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class Transaction : INotifyPropertyChanged
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AdminId { get; set; }
        [ForeignKey("AdminId")]
        public virtual Admin Admin { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int ProductTypeId { get; set; }
        [ForeignKey("ProductTypeId")]
        public virtual ProductType ProductType { get; set; }
        public double Paid { get; set; }
        public bool IsCash { get; set; }
        public Transaction(int adminId, int productTypeId, double paid, bool isCash)
        {
            AdminId = adminId;
            ProductTypeId = productTypeId;
            Paid = paid;
            IsCash = isCash;
        }

        public override string ToString()
        {
            return $"{ProductTypeId}, {Paid}Р, {Date}";
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
