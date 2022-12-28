using Common.ViewModels;
using System;

namespace MoneyPro.Models
{
    public class InvTransactionVM : BaseVM
    {
        private readonly InvTransaction model;

        #region Constructors

        public static InvTransactionVM ModelToVM(InvTransaction model) => new InvTransactionVM(model);
        public static InvTransactionVM ImportToVM(InvTransactionImport import) => new InvTransactionVM(import);
        private InvTransactionVM(InvTransaction model) => this.model = model;

        private InvTransactionVM(InvTransactionImport import)
        {
            model = import.Model;
        }
        public InvTransactionVM()
        {
            model = new InvTransaction();
        }

        #endregion

        #region Properties

        public int AccountId { get; set; }
        public int TransactionId { get; set; }
        public string Memo
        {
            get => model.Memo;
            set
            {
                model.Memo = value;
                NotifyPropertyChanged();
            }
        }
        public DateTime Date
        {
            get => model.Date;
            set
            {
                model.Date = value;
                NotifyPropertyChanged();
            }
        }
        public TransactionStatus Status
        {
            get => model.Status;
            set
            {
                model.Status = value;
                NotifyPropertyChanged();
            }
        }
        public string Action 
        {
            get => model.Action;
            set
            {
                model.Action = value;
                NotifyPropertyChanged();
            }
        }
        public string Security
        {
            get => model.Security;
            set
            {
                model.Security = value;
                NotifyPropertyChanged();
            }
        }
        public string Description
        {
            get => model.Description;
            set
            {
                model.Description = value;
                NotifyPropertyChanged();
            }
        }
        public decimal Price 
        {
            get => model.Price;
            set
            {
                model.Price = value;
                NotifyPropertyChanged();
            }
        }
        public decimal Quantity
        {
            get => model.Quantity;
            set
            {
                model.Quantity = value;
                NotifyPropertyChanged();
            }
        }
        public decimal Commission
        {
            get => model.Commission;
            set
            {
                model.Commission = value;
                NotifyPropertyChanged();
            }
        }
        public decimal TransferAmount 
        {
            get => model.TransferAmount;
            set
            {
                model.TransferAmount = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public string ToCSV() => $"{Date.ToShortDateString()},\"{Memo}\",\"{Status}\",\"{Action}\",\"{Security}\",\"{Description}\",{Price},{Quantity},{Commission},{TransferAmount}";

        public string WriteCsvHeader() => $"Date,Memo,Status,Action,Security,Description,Price,Qty,Commission,XferAmount";

        #endregion
    }
}
