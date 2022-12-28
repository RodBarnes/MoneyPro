using Common.ViewModels;
using MoneyPro.Models;

namespace MoneyPro.ViewModels
{
    public class ScheduleSubtransactionVM : BaseVM
    {
        private readonly ScheduleSubtransaction model;

        #region Constructors

        public static ScheduleSubtransactionVM ModelToVM(ScheduleSubtransaction model) => new ScheduleSubtransactionVM(model);
        private ScheduleSubtransactionVM(ScheduleSubtransaction model) => this.model = model;
        public ScheduleSubtransactionVM() => model = new ScheduleSubtransaction();
        public ScheduleSubtransactionVM(int transactionId) : this() => ScheduleTransactionId = transactionId;

        #endregion

        #region Properties

        public int ScheduleSubtransactionId { get => model.ScheduleSubtransactionId; set => model.ScheduleSubtransactionId = value; }
        public int ScheduleTransactionId { get => model.ScheduleTransactionId; set => model.ScheduleTransactionId = value; }
        public string Memo
        {
            get => model.Memo;
            set
            {
                model.Memo = value;
                NotifyPropertyChanged();
            }
        }
        public string XferAccount
        {
            get => model.XferAccount;
            set
            {
                model.XferAccount = value;
                NotifyPropertyChanged();
            }
        }
        public string Category
        {
            get => model.Category;
            set
            {
                model.Category = value;
                NotifyPropertyChanged();
            }
        }
        public string Subcategory 
        {
            get => model.Subcategory;
            set
            {
                model.Subcategory = value;
                NotifyPropertyChanged();
            }
        }
        public string Class
        {
            get => model.Class;
            set
            {
                model.Class = value;
                NotifyPropertyChanged();
            }
        }
        public string Subclass
        {
            get => model.Subclass;
            set
            {
                model.Subclass = value;
                NotifyPropertyChanged();
            }
        }
        public string Budget 
        {
            get => model.Budget;
            set
            {
                model.Budget = value;
                NotifyPropertyChanged();
            }
        }
        public decimal Amount
        {
            get => model.Amount;
            set
            {
                model.Amount = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public ScheduleSubtransaction ToModel() => model;
        public void Insert() => model.Insert();
        public void Update() => model.Update();
        public void Delete() => model.Delete();

        #endregion
    }
}
