using Common;
using Common.UserControls;
using Common.ViewModels;
using Microsoft.Win32;
using MoneyPro.Events;
using MoneyPro.Models;
using MoneyPro.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MoneyPro.ViewModels
{
    partial class MainVM : BaseVM
    {
        private const string LITERAL_FALSE = "False";
        private const string LITERAL_TRUE = "True";
        private const string VISIBILITY_SHOW = "Visible";
        private const string VISIBILITY_SELECTABLE = "VisibleWhenSelected";
        private const string VISIBILITY_COLLAPSE = "Collapsed";
        private const string VISIBILITY_HIDE = "Hidden";

        private static AboutProperties aboutProperties;
        private static MainWindow window;
        private static List<BankTransactionVM> delBankTransactions = new List<BankTransactionVM>();
        private static List<SubtransactionVM> delSubtransactions = new List<SubtransactionVM>();
        private static BackgroundWorker worker;
        private static readonly AutoResetEvent resetEvent = new AutoResetEvent(false);
        private static bool deleteTransferInProgress = false;
        private static int targetTransactionId;
        private static bool fromMenuExit = false;

        public MainVM(MainWindow wdw)
        {
            window = wdw;

            LoadSettings();

            InitMainPanel();
            InitTransactionSchedule();
            InitTransactionSearch();
            InitLookupMaintenance();
            InitLookupReplacement();
            InitAccountDetails();
            InitBusyPanel();
        }

        #region Commands

        public Command ExitApplicationCommand { get; set; }
        private void ExitApplicationAction(object obj) => ExitApplication();

        public Command OpenDatabaseCommand { get; set; }
        private void OpenDatabaseAction(object obj) => OpenDatabase();

        public Command BackupDatabaseCommand { get; set; }
        private void BackupDatabaseAction(object obj) => BackupDatabase();

        public Command ShowAboutCommand { get; set; }
        private void ShowAboutAction(object obj) => ShowAbout();

        public Command DeleteAccountCommand { get; set; }
        private void DeleteAccountAction(object obj) => ConfirmDelete();

        public Command WriteToCsvCommand { get; set; }
        private void WriteToCsvAction(object obj) => WriteToCsv(SelectedAccount);

        public Command ImportInvestmentInfoCommand { get; set; }
        private void ImportInvestmentInfoAction(object obj) => ImportInvestmentInfo(ContentPath);

        public Command XferGoToAccountCommand { get; set; }
        private void XferGoToAccountAction(object obj) => XferGoToAccount();

        #endregion

        #region MessagePanel

        public enum MessageAction
        {
            Acknowledge,
            DeleteAccount,
            DeleteTransaction,
            DeleteSubtransaction,
            DeletePayee,
            DeleteInstitution,
            MergeTransactions
        }

        private MessageAction currentMessageAction;

        private string mainMessageResponse;
        public string MainMessageResponse
        {
            get => mainMessageResponse;
            set
            {
                mainMessageResponse = value;
                NotifyPropertyChanged();
                if (mainMessageResponse == "Proceed")
                {
                    switch (currentMessageAction)
                    {
                        case MessageAction.Acknowledge:
                            break;
                        case MessageAction.DeleteAccount:
                            DeleteAccount(SelectedAccount);
                            break;
                        case MessageAction.DeleteTransaction:
                            SelectedAccount.TransactionsDelete(delBankTransactions);
                            delBankTransactions.Clear();
                            break;
                        case MessageAction.DeleteSubtransaction:
                            SelectedAccount.SelectedTransaction.Status = TransactionStatus.N;
                            SelectedAccount.SelectedTransaction.SubtransactionsDelete(delSubtransactions);
                            delSubtransactions.Clear();
                            break;
                        case MessageAction.MergeTransactions:
                            SelectedAccount.TransactionsMerge(importList);
                            break;
                        case MessageAction.DeletePayee:
                            BankTransactionVM.Payees.Remove(MaintPayee);
                            break;
                        case MessageAction.DeleteInstitution:
                            AccountVM.Institutions.Remove(MaintInstitution);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        #region Properties

        private bool SelectedAccountHasTransactions => SelectedAccount != null && SelectedAccount.HasTransactions;

        private bool showClosedAccounts;
        public bool ShowClosedAccounts
        {
            get => showClosedAccounts;
            set
            {
                showClosedAccounts = value;
                AccountsRead(showClosedAccounts ? AccountStatus.Closed : AccountStatus.Open);
                SelectedAccount = null;
                NotifyPropertyChanged();
            }
        }

        private bool showReconciledTransactions;
        public bool ShowReconciledTransactions
        {
            get => showReconciledTransactions;
            set
            {
                showReconciledTransactions = value;
                SelectedAccount.TransactionsRead(showReconciledTransactions ? TransactionStatus.R : TransactionStatus.C);
                NotifyPropertyChanged();
            }
        }

        private string windowTitle;
        public string WindowTitle
        {
            get => windowTitle ?? AppName;
            set
            {
                windowTitle = value;
                NotifyPropertyChanged();
            }
        }

        private string hasAccounts = LITERAL_FALSE;
        public string HasAccounts
        {
            get => hasAccounts;
            set
            {
                hasAccounts = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<AccountVM> accountList;
        public ObservableCollection<AccountVM> AccountList
        {
            get => accountList;
            set
            {
                accountList = value;
                NotifyPropertyChanged();
            }
        }

        private AccountVM selectedAccount;
        public AccountVM SelectedAccount
        {
            get => selectedAccount;
            set
            {
                selectedAccount = value;
                NotifyPropertyChanged();
                if (selectedAccount != null)
                {
                    WindowTitle = $"{selectedAccount.Name} - {AppName}";
                    selectedAccount.TransactionsRead();
                    if (targetTransactionId == 0)
                    {
                        selectedAccount.SelectedTransaction = selectedAccount.Transactions.FirstOrDefault();
                    }
                    else
                    {
                        selectedAccount.SelectedTransaction = SelectedAccount.Transactions.FirstOrDefault(t => t.TransactionId == targetTransactionId);
                    }
                    BankDataGridVisibility = VISIBILITY_SHOW;
                    BackgroundImageVisibility = VISIBILITY_HIDE;
                    SubtransactionVM.XferAccountRemove(selectedAccount.Name, AccountList);
                }
                else
                {
                    HideTransactions();
                }
                HasAccounts = (AccountList != null && AccountList.Count > 0) ? LITERAL_TRUE : LITERAL_FALSE;
            }
        }

        private string backgroundImageVisibility = VISIBILITY_SHOW;
        public string BackgroundImageVisibility
        {
            get => backgroundImageVisibility;
            set
            {
                backgroundImageVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string bankDataGridVisibility = VISIBILITY_HIDE;
        public string BankDataGridVisibility
        {
            get => bankDataGridVisibility;
            set
            {
                bankDataGridVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string subTransVisibility = VISIBILITY_COLLAPSE;
        public string SubTransVisibility
        {
            get => subTransVisibility;
            set
            {
                subTransVisibility = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Methods

        private void InitMainPanel()
        {
            ExitApplicationCommand = new Command(ExitApplicationAction);
            OpenDatabaseCommand = new Command(OpenDatabaseAction);
            BackupDatabaseCommand = new Command(BackupDatabaseAction);
            ShowAboutCommand = new Command(ShowAboutAction);
            DeleteAccountCommand = new Command(DeleteAccountAction, param => SelectedAccount != null);
            WriteToCsvCommand = new Command(WriteToCsvAction, param => SelectedAccountHasTransactions);
            ImportInvestmentInfoCommand = new Command(ImportInvestmentInfoAction);
            XferGoToAccountCommand = new Command(XferGoToAccountAction);

            var assyInfo = new AssemblyInfo(Assembly.GetExecutingAssembly());
            aboutProperties = new AboutProperties
            {
                ApplicationName = assyInfo.Product,
                ApplicationVersion = assyInfo.AssemblyVersionString,
                Background = nameof(Colors.Wheat),
                ImagePath = "/MoneyPro;component/Images/financial-icon-94-96.png",
                Copyright = $"{assyInfo.Copyright} {assyInfo.Company}",
                Description = assyInfo.Description,
            };

            DatabaseManager.InitDataPaths(DatabasePath, DatabaseName);
            ScheduleRules = DatabaseManager.ScheduleRulesRead();

            InitializeData();
        }

        private void ExitApplication()
        {
            fromMenuExit = true;
            window.Close();
        }

        private void OpenDatabase()
        {
            var dlg = new OpenFileDialog
            {
                InitialDirectory = BackupPath,
                DefaultExt = "7z",
                Filter = "7-Zip Archive (*.7z, *.zip)|*.7z;*.zip"
            };

            bool? result = dlg.ShowDialog(window);
            if (result == true)
            {
                worker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                worker.DoWork += Worker_RestoreDatabaseDoWork;
                worker.ProgressChanged += Worker_RestoreDatabaseProgressChanged;
                worker.RunWorkerCompleted += Worker_RestoreDatabaseRunWorkerCompleted;
                if (!worker.IsBusy)
                {
                    worker.RunWorkerAsync(dlg.FileName);
                }
            }
        }

        private void BackupDatabase()
        {
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            worker.DoWork += Worker_BackupDatabaseDoWork;
            worker.ProgressChanged += Worker_BackupDatabaseProgressChanged;
            worker.RunWorkerCompleted += Worker_BackupDatabaseRunWorkerCompleted;
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync(BackupPath);
                resetEvent.WaitOne();
            }
        }

        private void ShowAbout()
        {
            var dlg = new AboutWindow(aboutProperties);
            dlg.ShowDialog();
        }

        private void DeleteAccount(AccountVM acct)
        {
            try
            {
                AccountList.Remove(acct);
                SelectedAccount = null;
            }
            catch (Exception ex)
            {
                window.MessagePanel.Show("Exception", Utility.ParseException(ex));
            }
        }

        private void WriteToCsv(AccountVM acct)
        {
            try
            {
                var filepath = ContentPath + "\\" + Path.GetFileNameWithoutExtension(acct.Name) + ".csv";
                acct.ToCSV(filepath);

                currentMessageAction = MessageAction.Acknowledge;
                window.MessagePanel.Show("Success", $"Created {filepath}");
            }
            catch (Exception ex)
            {
                currentMessageAction = MessageAction.Acknowledge;
                window.MessagePanel.Show("Exception", Utility.ParseException(ex));
            }
        }

        private void ImportInvestmentInfo(string filepath)
        {
            var investments = new Investments(filepath);

            var adp = investments.GetInvestmentWithTicker("ADP");

            var list = investments.GetInvestmentsCompanyContainsText("Clean");

            investments.ToDB();

            currentMessageAction = MessageAction.Acknowledge;
            window.MessagePanel.Show("Success", "Investment info refreshed.");
        }

        private void XferGoToAccount()
        {
            var sub = SelectedAccount.SelectedTransaction.SelectedSubtransaction;
            JumpToTransaction(sub.XferAccount, sub.XferTransactionId);
        }

        public void JumpToTransaction(string account, int transactionId)
        {
            targetTransactionId = transactionId;
            SelectedAccount = AccountList.FirstOrDefault(a => a.Name == account);
        }

        private void InitializeData()
        {
            AccountsRead();
            SelectedAccount = null;
        }

        private void AddEvents(AccountVM item)
        {
            item.TransferCreated += new TransferCreatedEventHandler(Account_TransferCreated);
            item.TransferDeleted += new TransferDeletedEventHandler(Account_TransferDeleted);
        }

        private void ManageBackups()
        {
            var files = Directory.GetFiles(BackupPath, "Database_Primary_*.mdf");
            Array.Sort(files);
            Array.Reverse(files);

            var cnt = int.Parse(BackupCount);
            for (int i = cnt; i < files.Length; i++)
            {
                var filename = Path.GetFileNameWithoutExtension(files[i]);
                var path = Path.GetDirectoryName(files[i]);
                File.Delete($@"{path}\{filename}.mdf");
                File.Delete($@"{path}\{filename}.ldf");
            }
        }

        private void HideTransactions()
        {
            BankDataGridVisibility = VISIBILITY_HIDE;
            BackgroundImageVisibility = VISIBILITY_SHOW;
        }

        private void AccountsRead(AccountStatus status = AccountStatus.Open)
        {
            try
            {
                AccountList = new ObservableCollection<AccountVM>();
                var accounts = Main.AccountsRead(status);
                foreach (var account in accounts)
                {
                    var vm = AccountVM.ModelToVM(account);
                    AddEvents(vm);
                    AccountList.Add(vm);
                }
                AccountList.CollectionChanged += AccountList_CollectionChanged;
            }
            catch (Exception ex)
            {
                window.MessagePanel.Show("Exception", Utility.ParseException(ex));
            }
        }

        public void ConfirmTransactionsDelete(List<BankTransactionVM> list)
        {
            var ClearedResolved = false;
            foreach (var trans in list)
            {
                if (trans.Status == TransactionStatus.C || trans.Status == TransactionStatus.R)
                {
                    ClearedResolved = true;
                    break;
                }
            }

            if (ClearedResolved)
            {
                delBankTransactions = list;

                string wording1;
                string wording2;
                if (list.Count > 1)
                {
                    wording1 = "One or more transactions are";
                    wording2 = "them";
                }
                else
                {
                    wording1 = "The transaction is";
                    wording2 = "this";
                }
                currentMessageAction = MessageAction.DeleteTransaction;
                window.MessagePanel.Show("Verifying", $"{wording1} marked Cleared or Resolved. Are you sure you want to delete {wording2}?", MessagePanel.MessageType.YesNo);
            }
            else
            {
                SelectedAccount.TransactionsDelete(list);
            }
        }

        public void ConfirmSubtransactionsDelete(BankTransactionVM trans, List<SubtransactionVM> list)
        {
            if (trans.Status == TransactionStatus.C || trans.Status == TransactionStatus.R)
            {
                delSubtransactions = list;

                string wording;
                if (list.Count > 1)
                {
                    wording = "these";
                }
                else
                {
                    wording = "this";
                }
                currentMessageAction = MessageAction.DeleteSubtransaction;
                window.MessagePanel.Show("Verifying", $"The transaction is marked Cleared or Resolved. Are you sure you want to delete {wording}?" +
                    "\nProceeding will mark the transaction as 'New'.");
            }
            else
            {
                SelectedAccount.SelectedTransaction.SubtransactionsDelete(list);
            }
        }

        private void ConfirmDelete()
        {
            currentMessageAction = MessageAction.DeleteAccount;
            window.MessagePanel.Show(
                "DELETE!!!",
                $"This will permanenly delete the '{SelectedAccount.Name}' account and all of its transactions!",
                "Check this to REALLY proceed!", MessagePanel.MessageType.YesNo);
        }

        #endregion

        #region Event Handlers

        private void Account_TransferCreated(object sender, TransferCreatedEventArgs e)
        {
            var hostAcct = (AccountVM)sender;
            var hostTrans = e.Transaction;
            var hostSub = e.Subtransaction;

            var xferAcct = AccountList.First(a => a.Name == e.XferAccountName);

            var xferTrans = new BankTransactionVM
            {
                TransactionId = 0,
                AccountId = xferAcct.AccountId,
                Reference = hostTrans.Reference,
                Date = hostTrans.Date,
                Payee = hostTrans.Payee,
                Status = hostTrans.Status,
                Memo = hostTrans.Memo
            };
            xferAcct.Transactions.Add(xferTrans);

            var xferSub = new SubtransactionVM
            {
                SubtransactionId = 0,
                TransactionId = xferTrans.TransactionId,
                Amount = -hostSub.Amount,
                Budget = hostSub.Budget,
                Memo = hostSub.Memo,
                Category = hostSub.Category,
                Subcategory = hostSub.Subcategory,
                Class = hostSub.Class,
                Subclass = hostSub.Subclass,
                XferAccount = hostAcct.Name,
                XferTransactionId = hostTrans.TransactionId,
                XferSubtransactionId = hostSub.SubtransactionId
            };
            xferTrans.Subtransactions.Add(xferSub);

            hostSub.XferTransactionId = xferTrans.TransactionId;
            hostSub.XferSubtransactionId = xferSub.SubtransactionId;
            hostSub.XferAccount = xferAcct.Name;
            hostSub.Update();
        }

        private void Account_TransferDeleted(object sender, TransferDeletedEventArgs e)
        {
            if (!deleteTransferInProgress)
            {
                deleteTransferInProgress = true;

                var xferAcct = AccountList.First(a => a.Name == e.AccountName);
                var xferTrans = xferAcct.Transactions.First(t => t.TransactionId == e.TransactionId);
                var xferSub = xferTrans.Subtransactions.First(s => s.SubtransactionId == e.SubtransactionId);

                xferTrans.Subtransactions.Remove(xferSub);
                if (xferTrans.Subtransactions.Count == 0)
                {
                    xferAcct.Transactions.Remove(xferTrans);
                }

                deleteTransferInProgress = false;
            }
        }

        private void AccountList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AccountVM item in e.OldItems)
                {
                    item.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AccountVM item in e.NewItems)
                {
                    item.Insert();
                    AddEvents(item);
                }
            }
        }

        #endregion

        #region Closing Handler

        private Command closingCommand;
        public Command ClosingCommand
        {
            get
            {
                if (closingCommand == null)
                    closingCommand = new Command(ExecuteClosing, CanExecuteClosing);

                return closingCommand;
            }
        }

        private void ExecuteClosing(object obj)
        {
            //if (!fromMenuExit)
            //{
            //    // Delay needed here because the [X] is too quick?  Still doesn't show consistently.
            //    // But it really needs to be event driven; i.e., when it knows it is displayed, then proceed
            //    Thread.Sleep(2000);
            //}
            //ShowBusyPanel("Shutting down...");
            //AllowUIToUpdate();

            //BackupDatabase();
            //ManageBackups();

            SaveSettings();
        }

        private bool CanExecuteClosing(object obj)
        {
            return true;
        }

        private void AllowUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            //EDIT:
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        #endregion

        #region Background Workers

        private void Worker_BackupDatabaseDoWork(object sender, DoWorkEventArgs e)
        {
            ShowBusyPanel("Backing up database...");
            var path = e.Argument.ToString();
            DatabaseManager.BackupDatabase(path);

            resetEvent.Set();
        }

        private void Worker_BackupDatabaseProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update progress here
        }

        private void Worker_BackupDatabaseRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                HideBusyPanel();
            }
            else if (!(e.Error == null))
            {
                HideBusyPanel();
                currentMessageAction = MessageAction.Acknowledge;
                window.MessagePanel.Show("Error!!", $"{e.Error.Message}", MessagePanel.MessageType.YesNo);
            }
            else
            {
                // Update info
                ManageBackups();
                HideBusyPanel();
            }
        }

        private void Worker_RestoreDatabaseDoWork(object sender, DoWorkEventArgs e)
        {
            ShowBusyPanel("Opening database...");
            var filename = e.Argument.ToString();
            DatabaseManager.OpenDatabase(filename);
        }

        private void Worker_RestoreDatabaseProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update progress here
        }

        private void Worker_RestoreDatabaseRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                HideBusyPanel();
            }
            else if (!(e.Error == null))
            {
                HideBusyPanel();
                currentMessageAction = MessageAction.Acknowledge;
                window.MessagePanel.Show("Error!!", $"{e.Error.Message}", MessagePanel.MessageType.YesNo);
            }
            else
            {
                // Update info
                InitializeData();
                HideBusyPanel();
            }
        }

        #endregion

        #region Settings Management

        private string appName;
        private string AppName
        {
            get => appName ?? "MoneyPro";
            set => appName = value;
        }
        private string contentPath;
        private string ContentPath
        {
            get => contentPath ?? AppDomain.CurrentDomain.BaseDirectory;
            set => contentPath = value;
        }
        private string backupPath;
        private string BackupPath
        {
            get => backupPath ?? @"K:\Temp";
            set => backupPath = value;
        }
        private string backupCount;
        private string BackupCount
        {
            get => backupCount ?? "3";
            set => backupCount = value;
        }
        private string databasePath;
        public string DatabasePath
        {
            #if DEBUG
            get => databasePath ?? @"C:\Users\rodba\AppData\Local\Microsoft\VisualStudio\SSDT";
            #else
            get => databasePath ?? @"C:\Users\rodba\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB";
            #endif
            set => databasePath = value;
        }
        private string databaseName;
        public string DatabaseName
        {
            get => databaseName ?? "MoneyPro_Primary";
            set => databaseName = value;
        }
        private string windowHeight;
        public string WindowHeight
        {
            get => windowHeight ?? WindowHeightMin.ToString();
            set => windowHeight = value;
        }
        private string windowWidth;
        public string WindowWidth
        {
            get => windowWidth ?? WindowWidthMin.ToString();
            set => windowWidth = value;
        }
        public double WindowWidthMin => window.MinWidth;
        public double WindowHeightMin => window.MinHeight;

        private void LoadSettings()
        {
            var list = new Settings
            {
                new Setting(nameof(ContentPath), ContentPath),
                new Setting(nameof(BackupPath), BackupPath),
                new Setting(nameof(BackupCount), BackupCount),
                new Setting(nameof(DatabasePath), DatabasePath),
                new Setting(nameof(DatabaseName), DatabaseName),
                new Setting(nameof(WindowHeight), WindowHeight),
                new Setting(nameof(WindowWidth), WindowWidth)
            };
            Utility.LoadSettings(list);
            ContentPath = list[nameof(ContentPath)].Value;
            BackupPath = list[nameof(BackupPath)].Value;
            BackupCount = list[nameof(BackupCount)].Value;
            DatabasePath = list[nameof(DatabasePath)].Value;
            DatabaseName = list[nameof(DatabaseName)].Value;
            WindowHeight = list[nameof(WindowHeight)].Value;
            WindowWidth = list[nameof(WindowWidth)].Value;
        }

        private void SaveSettings()
        {
            var list = new Settings
            {
                new Setting(nameof(ContentPath), ContentPath),
                new Setting(nameof(BackupPath), BackupPath),
                new Setting(nameof(BackupCount), BackupCount),
                new Setting(nameof(DatabasePath), DatabasePath),
                new Setting(nameof(DatabaseName), DatabaseName),
                new Setting(nameof(WindowHeight), WindowHeight),
                new Setting(nameof(WindowWidth), WindowWidth)
            };
            Utility.SaveSettings(list);
        }

        #endregion

    }
}
