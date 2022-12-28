using Common;
using Common.ViewModels;
using MoneyPro.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MoneyPro.ViewModels
{
    partial class MainVM : BaseVM
    {
        public void InitTransactionSearch()
        {
            // Search Panel commands
            ShowTransactionSearchCommand = new Command(ShowTransactionSearchAction, param => AccountList.Count > 0);
            PerformSearchCommand = new Command(PerformSearchAction);
            ClearSearchCommand = new Command(ClearSearchAction);
            HideTransactionSearchCommand = new Command(HideTransactionSearchAction);
            SearchGoToAccountCommand = new Command(SearchGoToAccountAction);

            try
            {
                References = DatabaseManager.ReferencesRead();
            }
            catch (Exception ex)
            {
                window.MessagePanel.Show("Exception", Utility.ParseException(ex));
            }
        }

        #region Commands

        public Command ShowTransactionSearchCommand { get; set; }
        private void ShowTransactionSearchAction(object obj) => ShowTransactionSearch();

        public Command PerformSearchCommand { get; set; }
        private void PerformSearchAction(object obj) => PerformSearch();

        public Command ClearSearchCommand { get; set; }
        private void ClearSearchAction(object obj)
        {
            ResetSearchItems();
            SetSearchButtonState(false);
        }

        public Command HideTransactionSearchCommand { get; set; }
        private void HideTransactionSearchAction(object obj) => HideTransactionSearch();

        public Command SearchGoToAccountCommand { get; set; }
        private void SearchGoToAccountAction(object obj)
        {
            HideTransactionSearch();
            JumpToTransaction(SelectedSearchItem.Account, SelectedSearchItem.TransactionId);
        }

        #endregion

        #region Properties

        public SearchItem SelectedSearchItem { get; set; }
        public decimal? SearchTotal => SearchItems?.Sum(i => i.Amount);

        private string searchPanelVisibility = VISIBILITY_HIDE;
        public string SearchTransactionVisibility
        {
            get => searchPanelVisibility;
            set
            {
                searchPanelVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string isSearchCriteria = LITERAL_FALSE;
        public string IsSearchCriteria
        {
            get => isSearchCriteria;
            set
            {
                isSearchCriteria = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<Reference> references;
        public ObservableCollection<Reference> References
        {
            get => references;
            set
            {
                references = value;
                NotifyPropertyChanged();
            }
        }


        private ObservableCollection<SearchItem> searchItems;
        public ObservableCollection<SearchItem> SearchItems
        {
            get => searchItems;
            set
            {
                searchItems = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(SearchTotal));
                SetSearchButtonState(searchAccount != null);
            }
        }

        private Account searchAccount = null;
        public Account SearchAccount
        {
            get => searchAccount;
            set
            {
                searchAccount = value;
                NotifyPropertyChanged();
                SetSearchButtonState(searchAccount != null);
            }
        }

        private PayeeVM searchPayee;
        public PayeeVM SearchPayee
        {
            get => searchPayee;
            set
            {
                searchPayee = value;
                NotifyPropertyChanged();
                SetSearchButtonState(searchPayee != null);
            }
        }

        private Reference searchReference;
        public Reference SearchReference
        {
            get => searchReference;
            set
            {
                searchReference = value;
                NotifyPropertyChanged();
                SetSearchButtonState(searchReference != null);
            }
        }

        private DateTime? searchFromDate = null;
        public DateTime? SearchFromDate
        {
            get => searchFromDate;
            set
            {
                searchFromDate = value;
                NotifyPropertyChanged();
                SetSearchButtonState(searchFromDate != null);
            }
        }

        private DateTime? searchToDate = null;
        public DateTime? SearchToDate
        {
            get => searchToDate;
            set
            {
                searchToDate = value;
                NotifyPropertyChanged();
                SetSearchButtonState(searchToDate != null);
            }
        }

        private decimal? searchFromAmount = null;
        public decimal? SearchFromAmount
        {
            get => searchFromAmount;
            set
            {
                searchFromAmount = value;
                NotifyPropertyChanged();
                SetSearchButtonState(searchFromAmount.GetValueOrDefault() != 0);
            }
        }

        private decimal? searchToAmount = null;
        public decimal? SearchToAmount
        {
            get => searchToAmount;
            set
            {
                searchToAmount = value;
                NotifyPropertyChanged();
                SetSearchButtonState(searchToAmount.GetValueOrDefault() != 0);
            }
        }

        private string searchMemo = null;
        public string SearchMemo
        {
            get => searchMemo;
            set
            {
                searchMemo = value;
                NotifyPropertyChanged();
                SetSearchButtonState(!string.IsNullOrEmpty(searchMemo));
            }
        }

        private Category searchCategory;
        public Category SearchCategory
        {
            get => searchCategory;
            set
            {
                searchCategory = value;
                NotifyPropertyChanged();
                SetSearchButtonState(searchCategory != null);
            }
        }

        private Subcategory searchSubcategory;
        public Subcategory SearchSubcategory
        {
            get => searchSubcategory;
            set
            {
                searchSubcategory = value;
                NotifyPropertyChanged();
                SetSearchButtonState(searchSubcategory != null);
            }
        }

        private Class searchClass = null;
        public Class SearchClass
        {
            get => searchClass;
            set
            {
                searchClass = value;
                NotifyPropertyChanged();
                SetSearchButtonState(searchClass != null);
            }
        }

        private Subclass searchSubclass = null;
        public Subclass SearchSubclass
        {
            get => searchSubclass;
            set
            {
                searchSubclass = value;
                NotifyPropertyChanged();
                SetSearchButtonState(searchSubclass != null);
            }
        }

        #endregion

        #region Methods

        public void ShowTransactionSearch() => SearchTransactionVisibility = VISIBILITY_SHOW;

        public void HideTransactionSearch() => SearchTransactionVisibility = VISIBILITY_HIDE;

        public void ResetSearchItems()
        {
            SearchAccount = null;
            SearchPayee = null;
            SearchReference = null;
            SearchFromDate = null;
            SearchToDate = null;
            SearchFromAmount = null;
            SearchToAmount = null;
            SearchMemo = null;
            SearchCategory = null;
            SearchSubcategory = null;
            SearchClass = null;
            SearchSubclass = null;
            SearchItems = new ObservableCollection<SearchItem>();
        }

        private void PerformSearch()
        {
            using (new WaitCursor())
            {
                var query = new SearchQuery
                {
                    Account = SearchAccount?.Name,
                    FromDate = SearchFromDate,
                    ToDate = SearchToDate,
                    Payee = SearchPayee?.Name,
                    Reference = SearchReference?.Text,
                    Memo = SearchMemo,
                    FromAmount = SearchFromAmount,
                    ToAmount = SearchToAmount,
                    Category = SearchCategory?.Text,
                    Subcategory = SearchSubcategory?.Text,
                    Class = SearchClass?.Text,
                    Subclass = SearchSubclass?.Text
                };
                SearchItems = new ObservableCollection<SearchItem>(query.Search());
            }

            HideBusyPanel();
        }

        private void SetSearchButtonState(bool enableButton)
        {
            if (enableButton)
            {
                IsSearchCriteria = LITERAL_TRUE;
            }
            else if (IsSearchCriteria == LITERAL_TRUE)
            {
                // Need to determine if any other field has been selected
                if (SearchAccount == null &&
                    SearchPayee == null &&
                    SearchReference == null &&
                    SearchFromDate == null &&
                    SearchToDate == null &&
                    SearchFromAmount.GetValueOrDefault() == 0 &&
                    SearchToAmount.GetValueOrDefault() == 0 &&
                    string.IsNullOrEmpty(searchMemo) &&
                    SearchCategory == null &&
                    SearchSubcategory ==null &&
                    SearchClass == null &&
                    SearchSubclass == null &&
                    SearchItems?.Count == 0)
                {
                    IsSearchCriteria = LITERAL_FALSE;
                }
            }
        }

        #endregion

    }
}
