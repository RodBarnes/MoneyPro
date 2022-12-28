using Common;
using Common.UserControls;
using Common.ViewModels;
using Microsoft.Win32;
using MoneyPro.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace MoneyPro.ViewModels
{
    public enum AccountAction
    {
        New,
        Importing,
        Details
    }

    partial class MainVM : BaseVM
    {
        private AccountVM mergeAccount;
        private AccountVM importAccount;
        private List<BankTransactionImport> importList;

        private AccountAction currentAccountAction;

        private void InitAccountDetails()
        {
            // Account Panel commands
            NewAccountCommand = new Command(NewAccountAction);
            ImportAccountCommand = new Command(ImportAccountAction);
            AccountDetailsCommand = new Command(AccountDetailsAction, param => SelectedAccount != null);
            AccountButtonCommand = new Command(AccountButtonAction);
            HideAccountDetailsCommand = new Command(HideAccountDetailsAction);
            SelectImportFileCommand = new Command(SelectImportFileAction);
        }

        #region Commands

        public Command NewAccountCommand { get; set; }
        private void NewAccountAction(object obj) => ShowAccountDetails(AccountAction.New);

        public Command ImportAccountCommand { get; set; }
        private void ImportAccountAction(object obj) => ShowAccountDetails(AccountAction.Importing);

        public Command AccountDetailsCommand { get; set; }
        private void AccountDetailsAction(object obj) => ShowAccountDetails(AccountAction.Details);

        public Command AccountButtonCommand { get; set; }
        private void AccountButtonAction(object obj) => ProcessRequest(currentAccountAction);

        public Command HideAccountDetailsCommand { get; set; }
        private void HideAccountDetailsAction(object obj) => HideAccountDetails();

        public Command SelectImportFileCommand { get; set; }
        private void SelectImportFileAction(object sender) => SelectImportFile();

        #endregion

        #region Properties

        public string ImportFilepath { get; set; }

        private AccountVM accountDetails;
        public AccountVM AccountDetails
        {
            get => accountDetails;
            set
            {
                accountDetails = value;
                NotifyPropertyChanged();
            }
        }

        private string accountDetailsVisibility = VISIBILITY_HIDE;
        public string AccountDetailsVisibility
        {
            get => accountDetailsVisibility; 
            set
            {
                accountDetailsVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string accountButtonContent = "Add";
        public string AccountButtonContent
        {
            get => accountButtonContent;
            set
            {
                accountButtonContent = value;
                NotifyPropertyChanged();
            }
        }

        private string accountButtonEnabled = LITERAL_FALSE;
        public string AccountButtonEnabled
        {
            get => accountButtonEnabled;
            set
            {
                accountButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        private string accountTypeSelectable = LITERAL_FALSE;
        public string AccountTypeSelectable
        {
            get => accountTypeSelectable;
            set
            {
                accountTypeSelectable = value;
                NotifyPropertyChanged();
            }
        }

        private string accountNameReadonly = LITERAL_FALSE;
        public string AccountNameReadonly
        {
            get => accountNameReadonly;
            set
            {
                accountNameReadonly = value;
                NotifyPropertyChanged();
            }
        }

        private string accountStatusVisibility = VISIBILITY_COLLAPSE;
        public string AccountStatusVisibility
        {
            get => accountStatusVisibility;
            set
            {
                accountStatusVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string importVisibility = VISIBILITY_COLLAPSE;
        public string ImportButtonVisibility
        {
            get => importVisibility;
            set
            {
                importVisibility = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Methods

        private void ShowAccountDetails(AccountAction state)
        {
            switch (state)
            {
                case AccountAction.New:
                    ImportButtonVisibility = VISIBILITY_COLLAPSE;
                    AccountStatusVisibility = VISIBILITY_COLLAPSE;
                    AccountTypeSelectable = LITERAL_TRUE;
                    AccountNameReadonly = LITERAL_FALSE;
                    AccountButtonContent = "Add";
                    AccountDetails = new AccountVM();
                    break;
                case AccountAction.Importing:
                    ImportFilepath = "";
                    ImportButtonVisibility = VISIBILITY_SHOW;
                    AccountStatusVisibility = VISIBILITY_COLLAPSE;
                    AccountTypeSelectable = LITERAL_FALSE;
                    AccountNameReadonly = LITERAL_TRUE;
                    AccountButtonContent = "Import";
                    AccountDetails = new AccountVM();
                    break;
                case AccountAction.Details:
                    ImportButtonVisibility = VISIBILITY_COLLAPSE;
                    AccountStatusVisibility = VISIBILITY_SHOW;
                    AccountTypeSelectable = LITERAL_FALSE;
                    AccountNameReadonly = LITERAL_FALSE;
                    AccountButtonContent = "Update";
                    AccountDetails = SelectedAccount.Clone();
                    break;
                default:
                    break;
            }

            currentAccountAction = state;
            AccountDetailsVisibility = VISIBILITY_SHOW;
        }

        private void HideAccountDetails() => AccountDetailsVisibility = VISIBILITY_HIDE;

        private void SelectImportFile()
        {
            var dlg = new OpenFileDialog
            {
                InitialDirectory = ContentPath,
                DefaultExt = "qif",
                Filter = "Quicken Export|*.qif"
            };

            bool? result = dlg.ShowDialog(window);
            if (result == true)
            {
                var filename = dlg.FileName;
                ImportFilepath = filename;
                var pathname = Path.GetDirectoryName(filename);
                if (pathname != ContentPath)
                {
                    ContentPath = pathname;
                }
                PopulateAccountDetails(filename);
            }
        }

        public void PopulateAccountDetails(string filepath)
        {
            ImportManager.ImportAccountProperties(filepath, out string acctType, out string name, out decimal balance);
            AccountDetails.Type = AccountVM.AccountTypeByName(acctType);
            AccountDetails.Name = name;
            AccountDetails.StartingBalance = balance;
            AccountDetails.Status = AccountStatus.Open;
        }

        private void UpdateAccountDetail(AccountVM acct)
        {
            acct.Name = AccountDetails.Name;
            acct.Institution = AccountDetails.Institution;
            acct.Number = AccountDetails.Number;
            acct.StartingBalance = AccountDetails.StartingBalance;
            acct.Status = AccountDetails.Status;
            acct.Update();
        }

        private void ProcessRequest(AccountAction state)
        {
            switch (state)
            {
                case AccountAction.Importing:
                    //System.Diagnostics.Debug.WriteLine($"{nameof(ProcessRequest)} thread=:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

                    // Need to create import account in main thread so it can be updated in main thread after being populated.
                    importAccount = AccountDetails;

                    worker = new BackgroundWorker
                    {
                        WorkerReportsProgress = true,
                        WorkerSupportsCancellation = true
                    };
                    worker.DoWork += Worker_ImportAccountDoWork;
                    worker.ProgressChanged += Worker_ImportAccountProgressChanged;
                    worker.RunWorkerCompleted += Worker_ImportAccountRunWorkerCompleted;
                    if (!worker.IsBusy)
                    {
                        worker.RunWorkerAsync();
                    }
                    break;
                case AccountAction.New:
                    // User is adding a new account
                    AccountList.Add(AccountDetails);
                    AccountsRead();
                    // Select the added account
                    SelectedAccount = AccountList.First(a => a.Name == AccountDetails.Name);
                    break;
                case AccountAction.Details:
                    UpdateAccountDetail(SelectedAccount);
                    break;
                default:
                    break;
            }

            HideAccountDetails();
        }

        #endregion

        #region Background Workers

        private void Worker_ImportAccountDoWork(object sender, DoWorkEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"{nameof(Worker_ImportAccountDoWork)} thread=:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

            ShowBusyPanel("Importing data...");
            importList = ImportManager.ImportTransactions(ImportFilepath);

            // Check for duplicate account
            mergeAccount = AccountList.FirstOrDefault(a => a.Name == importAccount.Name);
            if (mergeAccount != null)
            {
                // Replace the list of transactions with only those that are not already present
                var list = new List<BankTransactionImport>();
                foreach (var import in importList)
                {
                    //System.Diagnostics.Debug.WriteLine($"\nimport Reference:{import.Model.Reference}, Date:{import.Model.Date}, Payee:{import.Model.Payee}, Status:{import.Model.Status}, Memo:{import.Model.Memo}");
                    //var reference = mergeAccount.Transactions.FirstOrDefault(t => t.Reference == import.Model.Reference);
                    //var date = mergeAccount.Transactions.FirstOrDefault(t => t.Date == import.Model.Date);
                    //var payee = mergeAccount.Transactions.FirstOrDefault(t => t.Payee == import.Model.Payee);
                    //var status = mergeAccount.Transactions.FirstOrDefault(t => t.Status == import.Model.Status);
                    //var memo = mergeAccount.Transactions.FirstOrDefault(t => t.Memo == import.Model.Memo);
                    //var amount = mergeAccount.Transactions.FirstOrDefault(t => t.Amount == import.Subtransactions.Sum(s => s.Amount));
                    //System.Diagnostics.Debug.WriteLine($"{nameof(reference)}:{reference!=null}, {nameof(date)}:{date!=null}, {nameof(payee)}:{payee!=null}, {nameof(status)}:{status!=null}, {nameof(memo)}:{memo!=null}, {nameof(amount)}:{amount!=null}");

                    var match = mergeAccount.Transactions.FirstOrDefault(t =>
                        t.Reference == import.Model.Reference &&
                        t.Date == import.Model.Date &&
                        t.Payee == import.Model.Payee &&
                        t.Memo == import.Model.Memo &&
                        t.Amount == import.Subtransactions.Sum(s => s.Amount));
                    if (match == null)
                    {
                        list.Add(import);
                    }
                }

                importList = list;
            }
        }

        private void Worker_ImportAccountProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"{nameof(Worker_ImportAccountProgressChanged)} thread=:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

            BusyProgressText = $"{e.ProgressPercentage}%";
        }

        private void Worker_ImportAccountRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"{nameof(Worker_ImportAccountRunWorkerCompleted)} thread=:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

            if ((e.Cancelled == true))
            {
                HideBusyPanel();
            }
            else if (!(e.Error == null))
            {
                HideBusyPanel();
                currentMessageAction = MessageAction.Acknowledge;
                window.MessagePanel.Show("Error!!", $"{e.Error.Message}");
            }
            else
            {
                if (mergeAccount == null)
                {
                    // Add the acount, its transactions, and their subtransactions
                    AccountList.Add(importAccount);
                    foreach (var import in importList)
                    {
                        importAccount.TransactionImportSubtransactionsAdd(import);
                    }

                    // Resolve transfers directly in database and then refresh accounts to pick up changes
                    DatabaseManager.ResolveTransfers();     
                    AccountsRead();

                    // Refresh lookups because there may be new ones imported
                    SubtransactionVM.XferAccountsRefresh(AccountList);
                    SubtransactionVM.CategoriesRefresh();
                    SubtransactionVM.SubcategoriesRefresh();
                    SubtransactionVM.ClassesRefresh();
                    SubtransactionVM.SubclassesRefresh();
                    BankTransactionVM.PayeesRefresh();

                    // Select the imported account
                    SelectedAccount = AccountList.First(a => a.Name == importAccount.Name);
                    importAccount = null;
                }
                else
                {
                    SelectedAccount = mergeAccount;
                    mergeAccount = null;

                    var total = importList.Sum(t => t.Subtransactions.Sum(s => s.Amount));
                    var count = importList.Count;
                    if (total == 0 && count == 0)
                    {
                        currentMessageAction = MessageAction.Acknowledge;
                        window.MessagePanel.Show("No transactions found",
                            $"The imported account name matched the existing account '{SelectedAccount.Name}'.  There " +
                            $"are no new transactions that can be merged as all of the transactions already exist.");
                    }
                    else
                    {
                        currentMessageAction = MessageAction.MergeTransactions;
                        window.MessagePanel.Show("New transactions found",
                            $"The imported account name matched the existing account '{SelectedAccount.Name}'.  There " +
                            $"are {count} transactions totalling {total:$#,0.00} that could be merged." +
                            $"\nDo you wish to merge these into the existing account?", MessagePanel.MessageType.YesNo);
                    }
                }

                HideBusyPanel();
            }
        }

        #endregion

    }
}
