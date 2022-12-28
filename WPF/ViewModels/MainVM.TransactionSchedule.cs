using Common;
using Common.ViewModels;
using MoneyPro.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MoneyPro.ViewModels
{
    partial class MainVM : BaseVM
    {
        public void InitTransactionSchedule()
        {
            ShowSchedulesCommand = new Command(ShowSchedulesAction);
            HideSchedulesCommand = new Command(HideSchedulesAction);
            EnterScheduleCommand = new Command(EnterScheduleAction);
            DeleteScheduleCommand = new Command(DeleteScheduleAction);
            ShowNewScheduleCommand = new Command(ShowNewScheduleAction);
            NewScheduleCancelCommand = new Command(NewScheduleCancelAction);
            NewScheduleOkCommand = new Command(NewScheduleOkAction, param => SelectedSched != null && SelectedSched.IsScheduleValid);
        }

        #region Commands

        public Command ShowSchedulesCommand { get; set; }
        private void ShowSchedulesAction(object obj) => ShowSchedules();

        public Command HideSchedulesCommand { get; set; }
        private void HideSchedulesAction(object obj) => HideSchedules();

        public Command EnterScheduleCommand { get; set; }
        private void EnterScheduleAction(object obj) => EnterSchedule();

        public Command DeleteScheduleCommand { get; set; }
        private void DeleteScheduleAction(object obj) => DeleteSchedule();

        public Command ShowNewScheduleCommand { get; set; }
        private void ShowNewScheduleAction(object obj) => ShowNewSchedule((BankTransactionVM)obj);

        public Command NewScheduleOkCommand { get; set; }
        private void NewScheduleOkAction(object obj) => MakeRecurring();

        public Command NewScheduleCancelCommand { get; set; }
        private void NewScheduleCancelAction(object obj) => HideNewSchedule();

        #endregion

        #region Properties

        private string schedDataGridVisibility = VISIBILITY_HIDE;
        public string SchedDataGridVisibility
        {
            get => schedDataGridVisibility;
            set
            {
                schedDataGridVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string subSchedVisibility = VISIBILITY_COLLAPSE;
        public string SubSchedVisibility
        {
            get => subSchedVisibility;
            set
            {
                subSchedVisibility = value;
                NotifyPropertyChanged();
            }
        }


        private string selectedSchedVisibility = VISIBILITY_HIDE;
        public string SelectedSchedVisibility
        {
            get => selectedSchedVisibility;
            set
            {
                selectedSchedVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<ScheduleRule> scheduleRules;
        public ObservableCollection<ScheduleRule> ScheduleRules
        {
            get => scheduleRules;
            set
            {
                scheduleRules = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<ScheduleTransactionVM> scheduleTransactions;
        public ObservableCollection<ScheduleTransactionVM> ScheduleTransactions
        {
            get => scheduleTransactions;
            set
            {
                scheduleTransactions = value;
                NotifyPropertyChanged();
            }
        }

        private ScheduleTransactionVM selectedSched;
        public ScheduleTransactionVM SelectedSched
        {
            get => selectedSched;
            set
            {
                selectedSched = value;
                NotifyPropertyChanged();
            }
        }

        private ScheduleSubtransactionVM selectedSubSched;
        public ScheduleSubtransactionVM SelectedSubSched
        {
            get => selectedSubSched;
            set
            {
                selectedSubSched = value;
                NotifyPropertyChanged();
            }
        }

        private SubtransactionVM scheduleSelectedSubtransaction;
        public SubtransactionVM ScheduleSelectedSubtransaction
        {
            get => scheduleSelectedSubtransaction;
            set
            {
                scheduleSelectedSubtransaction = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Methods

        private void ShowNewSchedule(BankTransactionVM trans)
        {
            if (trans != null)
            {
                var acct = AccountList.First(a => a.AccountId == trans.AccountId);
                SelectedSched = new ScheduleTransactionVM
                {
                    Account = acct.Name,
                    Payee = trans.Payee,
                    Memo = trans.Memo
                };

                foreach (var sub in trans.Subtransactions)
                {
                    SelectedSched.Subtransactions.Add(new ScheduleSubtransactionVM
                    {
                        Memo = sub.Memo,
                        Category = sub.Category,
                        Subcategory = sub.Subcategory,
                        Class = sub.Class,
                        Subclass = sub.Subclass,
                        Budget = sub.Budget,
                        Amount = sub.Amount
                    });
                }
            }

            SelectedSchedVisibility = VISIBILITY_SHOW;
        }

        private void HideNewSchedule() => SelectedSchedVisibility = VISIBILITY_HIDE;

        private void ShowSchedules()
        {
            var list = DatabaseManager.ScheduleTransactionsRead();
            ScheduleTransactions = new ObservableCollection<ScheduleTransactionVM>();
            foreach (var sched in list)
            {
                var item = ScheduleTransactionVM.ModelToVM(sched);
                item.PropertyChanged += new PropertyChangedEventHandler(Transaction_PropertyChanged);
                ScheduleTransactions.Add(item);
            }
            ScheduleTransactions.CollectionChanged += ScheduleTransactions_CollectionChanged;

            SchedDataGridVisibility = VISIBILITY_SHOW;
        }

        private void HideSchedules() => SchedDataGridVisibility = VISIBILITY_HIDE;

        private void MakeRecurring()
        {
            if (ScheduleTransactions == null)
            {
                ScheduleTransactions = new ObservableCollection<ScheduleTransactionVM>();
                ScheduleTransactions.CollectionChanged += ScheduleTransactions_CollectionChanged;
            }
            ScheduleTransactions.Add(SelectedSched);
            HideNewSchedule();
        }

        private void EnterSchedule()
        {
            // TODO: Enter the selected schedule
            // 
        }

        private void DeleteSchedule()
        {
            ScheduleTransactions.Remove(SelectedSched);
        }

        #endregion

        #region Event Handlers

        private void Transaction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        private void ScheduleTransactions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ScheduleTransactionVM item in e.OldItems)
                {
                    item.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ScheduleTransactionVM item in e.NewItems)
                {
                    item.Insert();
                    foreach (var sub in item.Subtransactions)
                    {
                        sub.ScheduleTransactionId = item.ScheduleTransactionId;
                        sub.Insert();
                    }

                    item.PropertyChanged += new PropertyChangedEventHandler(Transaction_PropertyChanged);
                }
            }
        }

        #endregion
    }
}
