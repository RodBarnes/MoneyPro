using Common.ViewModels;
using MoneyPro.Events;
using MoneyPro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MoneyPro.ViewModels
{
    public class BankTransactionVM : BaseVM, IDataErrorInfo
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public event TransferCreatedEventHandler TransferCreated;
        public event TransferDeletedEventHandler TransferDeleted;

        private readonly BankTransaction model;

        #region Constructors

        public static BankTransactionVM ModelToVM(BankTransaction model) => new BankTransactionVM(model);
        public static BankTransactionVM ImportToVM(BankTransactionImport import) => new BankTransactionVM(import);
        private BankTransactionVM(BankTransaction model) => this.model = model;

        private BankTransactionVM(BankTransactionImport import)
        {
            model = import.Model;
            Subtransactions.CollectionChanged += Subtransactions_CollectionChanged;
        }
        public BankTransactionVM()
        {
            model = new BankTransaction();
            Subtransactions.CollectionChanged += Subtransactions_CollectionChanged;
        }

        #endregion

        #region Properties

        public bool IsDirty { get; private set; } = false;
        public int AccountId { get => model.AccountId; set => model.AccountId = value; }
        public int TransactionId { get => model.TransactionId; set => model.TransactionId = value; }

        private ObservableCollection<SubtransactionVM> subtransactions = new ObservableCollection<SubtransactionVM>();
        public ObservableCollection<SubtransactionVM> Subtransactions
        {
            get => subtransactions;
            private set
            {
                subtransactions = value;
                NotifyPropertyChanged();
            }
        }

        private SubtransactionVM selectedSubtransaction;
        public SubtransactionVM SelectedSubtransaction
        {
            get => selectedSubtransaction;
            set
            {
                selectedSubtransaction = value;
                NotifyPropertyChanged();
            }
        }

        public string Reference
        {
            get => model.Reference;
            set
            {
                model.Reference = value;
                NotifyPropertyChanged();
                IsDirty = true;
            }
        }
        public DateTime Date
        {
            get => model.Date;
            set
            {
                model.Date = value;
                NotifyPropertyChanged();
                IsDirty = true;
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
        public string Payee
        {
            get => model.Payee;
            set
            {
                if (!string.IsNullOrEmpty(value) && Payees.FirstOrDefault(i => i.Name == value) == null)
                {
                    Payees.Add(new PayeeVM
                    {
                        Name = value
                    });
                }
                model.Payee = value;
                NotifyPropertyChanged();
                IsDirty = true;
            }
        }
        public TransactionStatus Status
        {
            get => model.Status;
            set
            {
                model.Status = value;
                NotifyPropertyChanged();
                IsDirty = true;
            }
        }
        public string Memo
        {
            get => model.Memo;
            set
            {
                model.Memo = value;
                NotifyPropertyChanged();
                IsDirty = true;
            }
        }

        public bool Void
        {
            get => model.Void;
            set
            {
                model.Void = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public BankTransaction ToModel() => model;

        public void Insert() => model.Insert();

        public void Update()
        {
            model.Update();
            IsDirty = false;
        }

        public void Delete()
        {
            model.Delete();
            foreach (var sub in Subtransactions)
            {
                if (sub.IsTransfer)
                {
                    // Need to delete the transfer
                    var ev = new TransferDeletedEventArgs
                    {
                        AccountName = sub.XferAccount,
                        TransactionId = sub.XferTransactionId,
                        SubtransactionId = sub.XferSubtransactionId
                    };
                    TransferDeleted(this, ev);
                }
            }
        }

        public BankTransactionVM Clone()
        {
            var trans = new BankTransactionVM(model.Clone())
            {
                IsDirty = IsDirty
            };
            foreach (var sub in Subtransactions)
            {
                trans.Subtransactions.Add(sub.Clone());
            }
            trans.Subtransactions.CollectionChanged += Subtransactions_CollectionChanged;

            return trans;
        }

        public string ToCSV() => $"\"{Reference}\",{Date.ToShortDateString()},{Amount},\"{Payee}\",\"{Memo}\",\"{Status}\"";

        public static string WriteCsvHeader() => $"Date,Total,Memo,Status,Reference,Payee";

        public void SubtransactionsRead()
        {
            var list = BankTransaction.SubtransactionsRead(TransactionId);
            Subtransactions = new ObservableCollection<SubtransactionVM>();
            foreach (var sub in list)
            {
                var item = SubtransactionVM.ModelToVM(sub);
                item.PropertyChanged += new PropertyChangedEventHandler(Subtransaction_PropertyChanged);
                item.TransferCreated += new TransferCreatedEventHandler(Transfer_Created);
                item.TransferDeleted += new TransferDeletedEventHandler(Transfer_Deleted);
                Subtransactions.Add(item);
            }
            Subtransactions.CollectionChanged += Subtransactions_CollectionChanged;
            Amount = Subtransactions.Sum(s => s.Amount);
        }

        public void SubtransactionsDelete(List<SubtransactionVM> list)
        {
            foreach (var sub in list)
            {
                Subtransactions.Remove(sub);
            }
        }

        #endregion

        #region Event Handlers

        private void Transfer_Created(object sender, TransferCreatedEventArgs e)
        {
            e.Subtransaction = (SubtransactionVM)sender;
            TransferCreated?.Invoke(this, e);
        }

        private void Transfer_Deleted(object sender, TransferDeletedEventArgs e)
        {
            TransferDeleted?.Invoke(this, e);
        }

        private void Subtransaction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        private void Subtransactions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (SubtransactionVM item in e.OldItems)
                {
                    // Notifiy parent of change for properties dependent upon collection items
                    Amount -= item.Amount;
                    Subtransaction_PropertyChanged(item, new PropertyChangedEventArgs(nameof(Amount)));

                    // Remove the item
                    item.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (SubtransactionVM item in e.NewItems)
                {
                    // Add the item
                    item.TransactionId = TransactionId;
                    item.Insert();

                    // Add the event handlers
                    item.PropertyChanged += new PropertyChangedEventHandler(Subtransaction_PropertyChanged);
                    item.TransferCreated += new TransferCreatedEventHandler(Transfer_Created);
                    item.TransferDeleted += new TransferDeletedEventHandler(Transfer_Deleted);

                    // Notify parent of change for properties dependent upon collection items
                    Amount += item.Amount;
                    Subtransaction_PropertyChanged(item, new PropertyChangedEventArgs(nameof(Amount)));
                }
            }
        }

        #endregion

        #region Payees

        private static ObservableCollection<PayeeVM> payees;
        public static ObservableCollection<PayeeVM> Payees
        {
            get
            {
                if (payees == null)
                {
                    PayeesRefresh();
                }
                return payees;
            }
            set
            {
                payees = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Payees)));
            }
        }

        public static void PayeesRefresh()
        {
            Payees = new ObservableCollection<PayeeVM>();
            var payees = DatabaseManager.PayeesRead();
            foreach (var payee in payees)
            {
                Payees.Add(PayeeVM.ModelToVM(payee));
            }
            Payees.CollectionChanged += Payees_CollectionChanged;
        }

        private static void Payees_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (PayeeVM item in e.OldItems)
                {
                    item.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PayeeVM item in e.NewItems)
                {
                    item.Insert();
                }
            }
        }

        #endregion

        #region Validation

        public string Error => "";

        public string this[string columnName]
        {
            get
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(BankTransactionVM)} >> DataErrorInfo:{columnName}");

                string result = null;

                switch (columnName)
                {
                    default:
                        break;
                }

                return result;
            }
        }

        #endregion

    }
}
