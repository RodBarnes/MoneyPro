using Common.ViewModels;
using MoneyPro.Events;
using MoneyPro.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MoneyPro.ViewModels
{
    public class SubtransactionVM : BaseVM, IDataErrorInfo
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public event TransferCreatedEventHandler TransferCreated;
        public event TransferDeletedEventHandler TransferDeleted;

        private static string prevXferAccount = "";
        private readonly Subtransaction model;

        #region Constructors

        public static SubtransactionVM ModelToVM(Subtransaction model) => new SubtransactionVM(model);
        private SubtransactionVM(Subtransaction model) => this.model = model;
        public SubtransactionVM() => model = new Subtransaction();
        public SubtransactionVM(int transactionId) : this() => TransactionId = transactionId;

        #endregion

        #region Properties

        public bool IsTransfer => !string.IsNullOrEmpty(XferAccount);
        public int SubtransactionId { get => model.SubtransactionId; set { model.SubtransactionId = value; } }
        public int TransactionId { get => model.TransactionId; set { model.TransactionId = value; } }
        public int XferSubtransactionId { get => model.XferSubtransactionId; set { model.XferSubtransactionId = value; } }
        public int XferTransactionId { get => model.XferTransactionId; set { model.XferTransactionId = value; } }

        private string validationResult;
        public string ValidationResult
        {
            get => validationResult;
            set
            {
                validationResult = value;
                NotifyPropertyChanged();
            }
        }

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
                prevXferAccount = model.XferAccount;
                model.XferAccount = value;
                NotifyPropertyChanged();
                if (IsTransfer)
                {
                    Category = "Transfer";
                    NotifyPropertyChanged(nameof(Category));
                }
            }
        }
        public string Category
        {
            get => model.Category;
            set
            {
                if (!string.IsNullOrEmpty(value) && Categories.FirstOrDefault(c => c.Text == value) == null)
                {
                    Categories.Add(new Category
                    {
                        Text = value
                    });
                }
                model.Category = value;
                NotifyPropertyChanged();
                if (model.Category != "Transfer")
                {
                    XferAccount = "";
                    NotifyPropertyChanged(nameof(XferAccount));
                }
            }
        }
        public string Subcategory
        {
            get => model.Subcategory;
            set
            {
                if (!string.IsNullOrEmpty(value) && Subcategories.FirstOrDefault(i => i.Text == value) == null)
                {
                    Subcategories.Add(new Subcategory
                    {
                        Text = value
                    });
                }
                model.Subcategory = value;
                NotifyPropertyChanged();
            }
        }
        public string Class
        {
            get => model.Class;
            set
            {
                if (!string.IsNullOrEmpty(value) && Classes.FirstOrDefault(i => i.Text == value) == null)
                {
                    Classes.Add(new Class
                    {
                        Text = value
                    });
                }
                model.Class = value;
                NotifyPropertyChanged();
            }
        }
        public string Subclass 
        {
            get => model.Subclass;
            set
            {
                if (!string.IsNullOrEmpty(value) && Subclasses.FirstOrDefault(i => i.Text == value) == null)
                {
                    Subclasses.Add(new Subclass
                    {
                        Text = value
                    });
                }
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

        public Subtransaction ToModel() => model;

        public void Insert() => model.Insert();

        public void Update()
        {
            model.Update();
            if (IsTransfer && XferSubtransactionId == 0)
            {
                // Changed to or added transfer subtransaction
                // Need to add the other side
                CreateTranfer();
            }
            else if (!IsTransfer && XferSubtransactionId != 0)
            {
                // Changed from transfer to normal subtransaction,
                // Need to delete the other side
                DeleteTransfer(prevXferAccount);
            }
        }

        public void Delete()
        {
            model.Delete();
            if (IsTransfer && XferTransactionId != 0)
            {
                // Need to delete the other side
                DeleteTransfer(XferAccount);
            }
        }

        public SubtransactionVM Clone() => new SubtransactionVM(model.Clone());

        public string ToCSV() => $"{Amount:N2},\"{Memo}\",\"{model.Category}\",\"{model.Subcategory}\",\"{model.Class}\",\"{model.Subclass}\"";

        public static string WriteCsvHeader() => "Amount,Memo,Category,Subategory,Class,Subclass";

        private void CreateTranfer()
        {
            var ev = new TransferCreatedEventArgs
            {
                XferAccountName = XferAccount
            };
            TransferCreated?.Invoke(this, ev);
        }

        private void DeleteTransfer(string acctName)
        {
            // Only allow the first side of a transfer to initiate the delete of all
            var ev = new TransferDeletedEventArgs
            {
                AccountName = acctName,
                TransactionId = XferTransactionId,
                SubtransactionId = XferSubtransactionId
            };
            TransferDeleted?.Invoke(this, ev);

            // Now that the values have been passed in the event, if it as an update
            // that caused deletion of the tranfer, need to update the xfer values to
            // show it is no longer a transfer.
            if (!IsTransfer)
            {
                XferSubtransactionId = 0;
                XferTransactionId = 0;
            }
        }

        #endregion

        #region Categories

        private static ObservableCollection<Category> categories;
        public static ObservableCollection<Category> Categories
        {
            get
            {
                if (categories == null)
                {
                    CategoriesRefresh();
                }
                return categories;
            }
            set
            {
                categories = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Categories)));
            }
        }

        public static void CategoriesRefresh()
        {
            Categories = DatabaseManager.CategoriesRead();
            Categories.CollectionChanged += Categories_CollectionChanged;
        }

        private static void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Category item in e.OldItems)
                {
                    item.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Category item in e.NewItems)
                {
                    item.Insert();
                }
            }
        }

        #endregion

        #region Subcategories

        private static ObservableCollection<Subcategory> subcategories;
        public static ObservableCollection<Subcategory> Subcategories
        {
            get
            {
                if (subcategories == null)
                {
                    SubcategoriesRefresh();
                }
                return subcategories;
            }
            set
            {
                subcategories = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Subcategories)));
            }
        }

        public static void SubcategoriesRefresh()
        {
            Subcategories = DatabaseManager.SubcategoriesRead();
            Subcategories.CollectionChanged += Subcategories_CollectionChanged;
        }

        private static void Subcategories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Subcategory item in e.OldItems)
                {
                    item.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Subcategory item in e.NewItems)
                {
                    item.Insert();
                }
            }
        }

        #endregion

        #region Classes

        private static ObservableCollection<Class> classes;
        public static ObservableCollection<Class> Classes
        {
            get
            {
                if (classes == null)
                {
                    ClassesRefresh();
                }
                return classes;
            }
            set
            {
                classes = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Classes)));
            }
        }

        public static void ClassesRefresh()
        {
            Classes = DatabaseManager.ClassesRead();
            Classes.CollectionChanged += Classes_CollectionChanged;
        }

        private static void Classes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Class item in e.OldItems)
                {
                    item.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Class item in e.NewItems)
                {
                    item.Insert();
                }
            }
        }

        #endregion

        #region Subclasses

        private static ObservableCollection<Subclass> subclasses;
        public static ObservableCollection<Subclass> Subclasses
        {
            get
            {
                if (subclasses == null)
                {
                    SubclassesRefresh();
                }
                return subclasses;
            }
            set
            {
                subclasses = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Subclasses)));
            }
        }

        public static void SubclassesRefresh()
        {
            Subclasses = DatabaseManager.SubclassesRead();
            Subclasses.CollectionChanged += Subclasses_CollectionChanged;
        }

        private static void Subclasses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Subclass item in e.OldItems)
                {
                    item.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Subclass item in e.NewItems)
                {
                    item.Insert();
                }
            }
        }

        #endregion

        #region XferAccounts

        private static ObservableCollection<AccountVM> xferAccounts;
        public static ObservableCollection<AccountVM> XferAccounts
        {
            get
            {
                if (xferAccounts == null)
                {
                    XferAccountsRefresh();
                }
                return xferAccounts;
            }
            set
            {
                xferAccounts = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(XferAccounts)));
            }
        }

        public static void XferAccountsRefresh(ObservableCollection<AccountVM> accounts = null)
        {
            if (accounts == null)
            {
                accounts = new ObservableCollection<AccountVM>();
                var list = DatabaseManager.AccountsRead(AccountStatus.Open);
                foreach (var item in list)
                {
                    accounts.Add(AccountVM.ModelToVM(item));
                }
            }
            XferAccounts = new ObservableCollection<AccountVM>(accounts);
            XferAccounts.CollectionChanged += XferAccounts_CollectionChanged;
        }

        private static void XferAccounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AccountVM item in e.OldItems)
                {
                    xferAccounts.Remove(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AccountVM item in e.NewItems)
                {
                    xferAccounts.Add(item);
                }
            }
        }

        public static void XferAccountRemove(string name, ObservableCollection<AccountVM> accounts)
        {
            XferAccountsRefresh(accounts);
            xferAccounts.Remove(xferAccounts.FirstOrDefault(a => a.Name == name));
        }

        #endregion

        #region Validation

        public string Error => "";

        public string this[string columnName]
        {
            get
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(SubtransactionVM)} >> DataErrorInfo:{columnName}");
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
