using Common.ViewModels;
using MoneyPro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MoneyPro.ViewModels
{
    public class ScheduleTransactionVM : BaseVM
    {
        private readonly ScheduleTransaction model;

        #region Constructors

        public static ScheduleTransactionVM ModelToVM(ScheduleTransaction model) => new ScheduleTransactionVM(model);
        private ScheduleTransactionVM(ScheduleTransaction model) => this.model = model;
        public ScheduleTransactionVM()
        {
            model = new ScheduleTransaction();
            Subtransactions.CollectionChanged += Subtransactions_CollectionChanged;
        }

        #endregion

        #region Properties

        private ObservableCollection<ScheduleSubtransactionVM> subtransactions = new ObservableCollection<ScheduleSubtransactionVM>();

        public ObservableCollection<ScheduleSubtransactionVM> Subtransactions
        {
            get => subtransactions;
            set
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

        public bool IsScheduleValid => !string.IsNullOrEmpty(Rule) && NextDate > DateTime.Today && Subtransactions.Count > 0;

        public int ScheduleTransactionId { get => model.ScheduleTransactionId; set => model.ScheduleTransactionId = value; }
        public string Rule
        {
            get => model.Rule;
            set
            {
                model.Rule = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsScheduleValid));
            }
        }
        public string Account
        {
            get => model.Account;
            set
            {
                model.Account = value;
                NotifyPropertyChanged();
            }
        }
        public string Payee
        {
            get => model.Payee;
            set
            {
                model.Payee = value;
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
        public string Memo
        {
            get => model.Memo;
            set
            {
                model.Memo = value;
                NotifyPropertyChanged();
            }
        }
        public DateTime NextDate
        {
            get => model.NextDate;
            set
            {
                model.NextDate = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsScheduleValid));
            }
        }
        public int? CountEnd
        {
            get => model.CountEnd;
            set
            {
                model.CountEnd = value;
                NotifyPropertyChanged();
            }
        }
        public DateTime? DateEnd 
        {
            get => model.DateEnd;
            set
            {
                model.DateEnd = value;
                NotifyPropertyChanged();
            }
        }
        private bool autoEnter;
        public bool AutoEnter
        {
            get => autoEnter;
            set
            {
                autoEnter = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(EnterDaysBeforeEnabled));
            }
        }
        public int? EnterDaysBefore
        {
            get => model.EnterDaysBefore;
            set
            {
                model.EnterDaysBefore = value;
                NotifyPropertyChanged();
            }
        }

        public string EnterDaysBeforeEnabled => AutoEnter ? "True" : "False";

        #endregion

        #region Methods

        public ScheduleTransaction ToModel() => model;
        public void Insert() => model.Insert();
        public void Update() => model.Update();
        public void Delete() => model.Delete();

        public void SubtransactionsRead()
        {
            var list = ScheduleTransaction.ScheduleSubtransactionsRead(ScheduleTransactionId);
            Subtransactions = new ObservableCollection<ScheduleSubtransactionVM>();
            foreach (var sub in list)
            {
                var item = ScheduleSubtransactionVM.ModelToVM(sub);
                item.PropertyChanged += new PropertyChangedEventHandler(Subtransaction_PropertyChanged);
                Subtransactions.Add(item);
            }
            Subtransactions.CollectionChanged += Subtransactions_CollectionChanged;
            Amount = Subtransactions.Sum(s => s.Amount);
        }

        public void SubtransactionsDelete(List<ScheduleSubtransactionVM> list)
        {
            foreach (var sub in list)
            {
                Subtransactions.Remove(sub);
            }
        }


        #endregion

        #region Event Handlers

        private void Subtransaction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        private void Subtransactions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ScheduleSubtransactionVM item in e.OldItems)
                {
                    // Notifiy parent of change for properties dependent upon collection items
                    Amount -= item.Amount;
                    Subtransaction_PropertyChanged(item, new PropertyChangedEventArgs(nameof(Amount)));
                    Subtransaction_PropertyChanged(item, new PropertyChangedEventArgs(nameof(IsScheduleValid)));

                    // Remove the item
                    item.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ScheduleSubtransactionVM item in e.NewItems)
                {
                    if (ScheduleTransactionId > 0)
                    {
                        // Add the item
                        item.ScheduleTransactionId = ScheduleTransactionId;
                        item.Insert();
                    }

                    // Add the event handlers
                    item.PropertyChanged += new PropertyChangedEventHandler(Subtransaction_PropertyChanged);

                    // Notifiy parent of change for properties dependent upon collection items
                    Amount += item.Amount;
                    Subtransaction_PropertyChanged(item, new PropertyChangedEventArgs(nameof(Amount)));
                    Subtransaction_PropertyChanged(item, new PropertyChangedEventArgs(nameof(IsScheduleValid)));
                }
            }
        }

        #endregion

        #region Schedule Rules

        private static ObservableCollection<ScheduleRule> scheduleRules;
        public static ObservableCollection<ScheduleRule> ScheduleRules
        {
            get
            {
                if (scheduleRules == null)
                {
                    ScheduleRulesRefresh();
                }
                return scheduleRules;
            }
            set
            {
                scheduleRules = value;
                //StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(scheduleRules)));
            }
        }

        public static void ScheduleRulesRefresh()
        {
            ScheduleRules = new ObservableCollection<ScheduleRule>();
            var rules = DatabaseManager.ScheduleRulesRead();
            foreach (var rule in rules)
            {
                ScheduleRules.Add(rule);
            }
        }

        #endregion

        #region Accounts

        private static ObservableCollection<AccountVM> accounts;
        public static ObservableCollection<AccountVM> Accounts
        {
            get
            {
                if (accounts == null)
                {
                    AccountsRefresh();
                }
                return accounts;
            }
            set
            {
                accounts = value;
                //StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(scheduleRules)));
            }
        }

        public static void AccountsRefresh()
        {
            Accounts = new ObservableCollection<AccountVM>();
            var accts = DatabaseManager.AccountsRead(AccountStatus.Open);
            foreach (var acct in accts)
            {
                Accounts.Add(AccountVM.ModelToVM(acct));
            }
        }

        #endregion
    }
}
