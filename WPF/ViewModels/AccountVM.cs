using Common.ViewModels;
using MoneyPro.Events;
using MoneyPro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace MoneyPro.ViewModels
{
    public class AccountVM : BaseVM, IDataErrorInfo
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        public event TransferCreatedEventHandler TransferCreated;
        public event TransferDeletedEventHandler TransferDeleted;

        private readonly Account model;

        #region Constructors

        public static AccountVM ModelToVM(Account model) => new AccountVM(model);
        private AccountVM(Account model) => this.model = model;
        public AccountVM()
        {
            model = new Account();
            Transactions.CollectionChanged += BankTransactions_CollectionChanged;
        }

        #endregion

        #region Properties

        public int AccountId { get => model.AccountId; set => model.AccountId = value; }
        public bool IsAccountAddable => Type != null && !string.IsNullOrEmpty(Name);

        private ObservableCollection<BankTransactionVM> transactions = new ObservableCollection<BankTransactionVM>();
        public ObservableCollection<BankTransactionVM> Transactions
        {
            get => transactions;
            private set
            {
                transactions = value;
                NotifyPropertyChanged();
            }
        }

        private BankTransactionVM selectedTransaction;
        public BankTransactionVM SelectedTransaction
        {
            get => selectedTransaction;
            set
            {
                selectedTransaction = value;
                NotifyPropertyChanged();
                if (selectedTransaction != null)
                {
                    selectedTransaction.SubtransactionsRead();
                }
            }
        }

        public SolidColorBrush ItemForeColor => new SolidColorBrush(Status == AccountStatus.Closed ? Colors.Gray : Colors.Black);
        public SolidColorBrush ItemBackColor => new SolidColorBrush(Colors.Transparent);
        public AccountType Type
        {
            get => model.Type;
            set
            {
                model.Type = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsAccountAddable));
            }
        }
        public string Name
        {
            get => model.Name;
            set
            {
                model.Name = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsAccountAddable));
            }
        }
        public string Number
        {
            get => model.Number;
            set
            {
                model.Number = value;
                NotifyPropertyChanged();
            }
        }
        public string Institution
        {
            get => model.Institution;
            set
            {
                if (!string.IsNullOrEmpty(value) && Institutions.FirstOrDefault(i => i.Name == value) == null)
                {
                    Institutions.Add(new InstitutionVM(value));
                }
                model.Institution = value;
                NotifyPropertyChanged();
            }
        }
        public decimal StartingBalance
        {
            get => model.StartingBalance;
            set
            {
                model.StartingBalance = value;
                NotifyPropertyChanged();
            }
        }
        public AccountStatus Status
        {
            get => model.Status;
            set
            {
                model.Status = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ItemForeColor));
                NotifyPropertyChanged(nameof(ItemBackColor));
            }
        }
        public bool HasTransactions => Transactions != null && Transactions.Count > 0;

        public decimal TransactionsTotal
        {
            get
            {
                decimal total = 0;
                if (Transactions != null && Transactions.Count > 0)
                {
                    total = Transactions.Sum(t => t.Amount) + StartingBalance;
                }
                return total;
            }
        }

        public int TransactionsCount => (Transactions != null) ? Transactions.Count : 0;

        #endregion

        #region Methods

        public Account ToModel() => model;

        public void Insert() => model.Insert();
        public void Update() => model.Update();
        public void Delete() => model.Delete();

        public AccountVM Clone()
        {
            var acct = new AccountVM(model.Clone());
            foreach (var trans in Transactions)
            {
                acct.Transactions.Add(trans.Clone());
            }
            acct.Transactions.CollectionChanged += BankTransactions_CollectionChanged;

            return acct;
        }

        public void ToCSV(string pathname)
        {
            using (var writer = new StreamWriter(pathname))
            {
                writer.WriteLine($"Account,Type,{BankTransactionVM.WriteCsvHeader()},{SubtransactionVM.WriteCsvHeader()}");
                foreach (BankTransactionVM trans in Transactions)
                {
                    foreach (SubtransactionVM sub in trans.Subtransactions)
                    {
                        writer.WriteLine($"{Name},{Type.DisplayName},{trans.ToCSV()},{sub.ToCSV()}");
                    }
                }
            }
        }

        public void TransactionsMerge(List<BankTransactionImport> importList)
        {
            decimal adjustment = 0;
            foreach (var import in importList)
            {
                adjustment += import.Subtransactions.Sum(s => s.Amount);
                TransactionImportSubtransactionsAdd(import);
            }
            StartingBalance -= adjustment;

            Update();
        }

        public void TransactionsRead(TransactionStatus status = TransactionStatus.C)
        {
            var list = Account.TransactionsRead(AccountId, status);
            Transactions = new ObservableCollection<BankTransactionVM>();
            foreach (var trans in list)
            {
                var item = BankTransactionVM.ModelToVM(trans);
                item.PropertyChanged += new PropertyChangedEventHandler(Transaction_PropertyChanged);
                item.TransferCreated += new TransferCreatedEventHandler(Transfer_Created);
                item.TransferDeleted += new TransferDeletedEventHandler(Transfer_Deleted);
                Transactions.Add(item);
            }
            Transactions.CollectionChanged += BankTransactions_CollectionChanged;
        }

        public void TransactionAddSubtransactionsInsert(BankTransactionVM trans)
        {
            Transactions.Add(trans);
            foreach (var sub in trans.Subtransactions)
            {
                sub.TransactionId = trans.TransactionId;
                sub.Insert();
            }
        }

        public void TransactionsAddSubtransactionsInsert(int accountId, List<BankTransactionVM> list)
        {
            foreach (var trans in list)
            {
                trans.AccountId = accountId;
                TransactionAddSubtransactionsInsert(trans);
            }
        }

        public void TransactionImportSubtransactionsAdd(BankTransactionImport import)
        {
            var trans = BankTransactionVM.ImportToVM(import);
            Transactions.Add(trans);
            foreach (var sub in import.Subtransactions)
            {
                trans.Subtransactions.Add(SubtransactionVM.ModelToVM(sub));
            }
        }

        public void TransactionsDelete(List<BankTransactionVM> list)
        {
            foreach (var trans in list)
            {
                Transactions.Remove(trans);
            }
        }

        #endregion

        #region Event Handlers

        private void Transfer_Created(object sender, TransferCreatedEventArgs e)
        {
            e.Transaction = (BankTransactionVM)sender;
            TransferCreated?.Invoke(this, e);
        }

        private void Transfer_Deleted(object sender, TransferDeletedEventArgs e)
        {
            TransferDeleted?.Invoke(this, e);
        }

        private void Transaction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        private void BankTransactions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (BankTransactionVM item in e.OldItems)
                {
                    // Notify parent of properties of change for properties dependent upon collection items
                    Transaction_PropertyChanged(item, new PropertyChangedEventArgs(nameof(TransactionsCount)));
                    Transaction_PropertyChanged(item, new PropertyChangedEventArgs(nameof(TransactionsTotal)));

                    // Remove the item
                    item.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (BankTransactionVM item in e.NewItems)
                {
                    // Add the item
                    item.AccountId = AccountId;
                    item.Insert();

                    // Add the event handler
                    item.PropertyChanged += new PropertyChangedEventHandler(Transaction_PropertyChanged);
                    item.TransferCreated += new TransferCreatedEventHandler(Transfer_Created);
                    item.TransferDeleted += new TransferDeletedEventHandler(Transfer_Deleted);

                    // Notifiy parent of properties of change for properties dependent upon collection items
                    Transaction_PropertyChanged(item, new PropertyChangedEventArgs(nameof(TransactionsCount)));
                    Transaction_PropertyChanged(item, new PropertyChangedEventArgs(nameof(TransactionsTotal)));
                }
            }
        }

        #endregion

        #region Institutions

        private static ObservableCollection<InstitutionVM> institutions;
        public static ObservableCollection<InstitutionVM> Institutions
        {
            get
            {
                if (institutions == null)
                {
                    InstitutionsRefresh();
                }
                return institutions;
            }
            set
            {
                institutions = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Institutions)));
            }
        }

        private static void InstitutionsRefresh()
        {
            var col = new ObservableCollection<InstitutionVM>();
            var list = DatabaseManager.InstitutionsRead();
            foreach (var item in list)
            {
                col.Add(new InstitutionVM(item));
            }
            Institutions = col;
            Institutions.CollectionChanged += Institutions_CollectionChanged;
        }

        private static void Institutions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (InstitutionVM item in e.OldItems)
                {
                    item.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (InstitutionVM item in e.NewItems)
                {
                    item.Insert();
                }
            }
        }

        #endregion

        #region AccountTypes

        private static ObservableCollection<AccountType> accountTypes;
        public static ObservableCollection<AccountType> AccountTypes
        {
            get
            {
                if (accountTypes == null)
                {
                    AccountTypesRefresh();
                }
                return accountTypes;
            }
            set
            {
                accountTypes = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AccountTypes)));
            }
        }

        public static void AccountTypesRefresh()
        {
            AccountTypes = DatabaseManager.AccountTypesRead();
        }

        public static AccountType AccountTypeByName(string text) => AccountTypes.FirstOrDefault(t => t.ImportName == text || t.DisplayName == text);

        //public AccountType this[string name]
        //{
        //    get => accountTypes.FirstOrDefault(a => a.DisplayName == name);
        //    set => accountTypes.Insert(accountTypes.IndexOf(accountTypes.FirstOrDefault(a => a.DisplayName == name)), value);
        //}

        #endregion

        #region Validation

        public string Error => "";

        public string this[string columnName]
        {
            get
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(AccountVM)} >> DataErrorInfo:{columnName}");

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
