using MoneyPro.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MoneyPro
{
    /// <summary>
    /// These methods support database access from the view model and hiding the specifics of interacting with the database.
    /// </summary>
    public static class DatabaseManager
    {
        #region Administration

        public static void InitDataPaths(string dbFilepath, string dbFilename)
        {
            using (var db = new DatabaseConnection())
            {
                db.InitDataPaths(dbFilepath, dbFilename);
            }
        }
        public static void BackupDatabase(string filepath)
        {
            using (var db = new DatabaseConnection())
            {
                db.BackupDatabase(filepath);
            }
        }
        public static void OpenDatabase(string filepath)
        {
            using (var db = new DatabaseConnection())
            {
                db.OpenDatabase(filepath);
            }
        }

        #endregion

        #region Main

        public static List<Account> AccountsRead(AccountStatus status)
        {
            var list = new List<Account>();
            using (var db = new DatabaseConnection())
            {
                db.AccountsRead(list, status);
            }
            return list;
        }

        public static void ResolveTransfers()
        {
            using (var db = new DatabaseConnection())
            {
                db.ResolveTransfers();
            }
        }

        public static IList<SearchItem> Search(SearchQuery item)
        {
            IList<SearchItem> list;

            using (var db = new DatabaseConnection())
            {
                list = db.SearchTransactions(item);
            }

            return list;
        }

        public static void InvestmentsInsert(List<Investment> list)
        {
            using (var db = new DatabaseConnection())
            {
                foreach (var inv in list)
                    db.InvestmentInsert(inv);
            }
        }

        #endregion

        #region Account

        public static void AccountInsert(Account item)
        {
            using (var db = new DatabaseConnection())
            {
                db.AccountInsert(item);
            }
        }
        public static void AccountDelete(int accountId)
        {
            using (var db = new DatabaseConnection())
            {
                db.AccountDelete(accountId);
                db.BankTransactionsDelete(accountId);
                db.SubtransactionsDeleteForAccount(accountId);
            }
        }
        public static void AccountUpdate(Account item)
        {
            using (var db = new DatabaseConnection())
            {
                db.AccountUpdate(item);
            }
        }

        #endregion

        #region BankTransaction

        public static List<BankTransaction> TransactionsRead(int accountId, TransactionStatus status)
        {
            var list = new List<BankTransaction>();
            using (var db = new DatabaseConnection())
            {
                //if (item.Type.ImportName == "Invst")
                //{
                //    db.InvTransactionsRead(item);
                //}
                //else
                //{
                db.BankTransactionsRead(accountId, status, list);
                //}
            }
            return list;
        }
        public static void BankTransactionInsert(BankTransaction item)
        {
            using (var db = new DatabaseConnection())
            {
                db.BankTransactionInsert(item);
            }
        }
        public static void BankTransactionUpdate(BankTransaction item)
        {
            using (var db = new DatabaseConnection())
            {
                db.BankTransactionUpdate(item);
            }
        }
        public static void BankTransactionDelete(int transactionId)
        {
            using (var db = new DatabaseConnection())
            {
                db.SubtransactionsDeleteForTransaction(transactionId);
                db.BankTransactionDelete(transactionId);
            }
        }

        #endregion

        #region Subtransaction

        public static List<Subtransaction> SubtransactionsRead(int transactionId)
        {
            var list = new List<Subtransaction>();
            using (var db = new DatabaseConnection())
            {
                db.SubtransactionsRead(transactionId, list);
            }
            return list;
        }
        public static void SubtransactionInsert(Subtransaction item)
        {
            using (var db = new DatabaseConnection())
            {
                db.SubtransactionInsert(item);
            }
        }
        public static void SubtransactionDelete(Subtransaction item)
        {
            using (var db = new DatabaseConnection())
            {
                db.SubtransactionDelete(item);
            }
        }
        public static void SubtransactionUpdate(Subtransaction item)
        {
            using (var db = new DatabaseConnection())
            {
                db.SubtransactionUpdate(item);
            }
        }

        #endregion

        #region ScheduleTransaction

        public static List<ScheduleTransaction> ScheduleTransactionsRead()
        {
            var list = new List<ScheduleTransaction>();
            using (var db = new DatabaseConnection())
            {
                db.ScheduleTransactionsRead(list);
            }
            return list;
        }
        public static void ScheduleTransactionInsert(ScheduleTransaction item)
        {
            using (var db = new DatabaseConnection())
            {
                db.ScheduleTransactionInsert(item);
            }
        }
        public static void ScheduleTransactionUpdate(ScheduleTransaction item)
        {
            using (var db = new DatabaseConnection())
            {
                db.ScheduleTransactionUpdate(item);
            }
        }
        public static void ScheduleTransactionDelete(int scheduleTransactionId)
        {
            using (var db = new DatabaseConnection())
            {
                db.ScheduleSubtransactionsDeleteForScheduleTransaction(scheduleTransactionId);
                db.ScheduleTransactionDelete(scheduleTransactionId);
            }
        }

        #endregion

        #region ScheduleSubtransaction

        public static List<ScheduleSubtransaction> ScheduleSubtransactionsRead(int scheduleTransactionId)
        {
            var list = new List<ScheduleSubtransaction>();
            using (var db = new DatabaseConnection())
            {
                db.ScheduleSubtransactionsRead(scheduleTransactionId, list);
            }
            return list;
        }
        public static void ScheduleSubtransactionInsert(ScheduleSubtransaction item)
        {
            using (var db = new DatabaseConnection())
            {
                db.ScheduleSubtransactionInsert(item);
            }
        }
        public static void ScheduleSubtransactionDelete(ScheduleSubtransaction item)
        {
            using (var db = new DatabaseConnection())
            {
                db.ScheduleSubtransactionDelete(item);
            }
        }
        public static void ScheduleSubtransactionUpdate(ScheduleSubtransaction item)
        {
            using (var db = new DatabaseConnection())
            {
                db.ScheduleSubtransactionUpdate(item);
            }
        }

        #endregion

        #region References

        public static ObservableCollection<Reference> ReferencesRead()
        {
            var items = new ObservableCollection<Reference>();

            using (var db = new DatabaseConnection())
            {
                 db.ReferencesRead(items);
            }

            return items;
        }

        #endregion

        #region Account Type

        public static ObservableCollection<AccountType> AccountTypesRead()
        {
            var items = new ObservableCollection<AccountType>();

            using (var db = new DatabaseConnection())
            {
                db.AccountTypesRead(items);
            }

            return items;
        }

        #endregion

        #region Institution

        public static List<Institution> InstitutionsRead()
        {
            var items = new List<Institution>();

            using (var db = new DatabaseConnection())
            {
                db.InstitutionsRead(items);
            }

            return items;
        }
        public static void InstitutionInsert(Institution item)
        {
            using (var db = new DatabaseConnection())
            {
                db.InstitutionInsert(item);
            }
        }
        public static void InstitutionUpdate(Institution item)
        {
            using (var db = new DatabaseConnection())
            {
                db.InstitutionUpdate(item);
            }
        }
        public static void InstitutionDelete(Institution item)
        {
            using (var db = new DatabaseConnection())
            {
                db.InstitutionDelete(item);
            }
        }

        #endregion

        #region Category

        public static ObservableCollection<Category> CategoriesRead()
        {
            var items = new ObservableCollection<Category>();

            using (var db = new DatabaseConnection())
            {
               db.CategoriesRead(items);
            }

            return items;
        }
        public static void CategoryInsert(Category item)
        {
            using (var db = new DatabaseConnection())
            {
                db.CategoryInsert(item);
            }
        }
        public static void CategoryUpdate(Category item)
        {
            using (var db = new DatabaseConnection())
            {
                db.CategoryUpdate(item);
            }
        }
        public static void CategoryDelete(Category item)
        {
            using (var db = new DatabaseConnection())
            {
                db.CategoryDelete(item);
            }
        }

        #endregion

        #region Subcategory

        public static ObservableCollection<Subcategory> SubcategoriesRead()
        {
            var items = new ObservableCollection<Subcategory>();

            using (var db = new DatabaseConnection())
            {
                db.SubcategoriesRead(items);
            }

            return items;
        }
        public static void SubcategoryInsert(Subcategory item)
        {
            using (var db = new DatabaseConnection())
            {
                db.SubcategoryInsert(item);
            }
        }
        public static void SubcategoryUpdate(Subcategory item)
        {
            using (var db = new DatabaseConnection())
            {
                db.SubcategoryUpdate(item);
            }
        }
        public static void SubcategoryDelete(Subcategory item)
        {
            using (var db = new DatabaseConnection())
            {
                db.SubcategoryDelete(item);
            }
        }

        #endregion

        #region Class

        public static ObservableCollection<Class> ClassesRead()
        {
            var items = new ObservableCollection<Class>();

            using (var db = new DatabaseConnection())
            {
                db.ClassesRead(items);
            }

            return items;
        }
        public static void ClassInsert(Class item)
        {
            using (var db = new DatabaseConnection())
            {
                db.ClassInsert(item);
            }
        }
        public static void ClassUpdate(Class item)
        {
            using (var db = new DatabaseConnection())
            {
                db.ClassUpdate(item);
            }
        }
        public static void ClassDelete(Class item)
        {
            using (var db = new DatabaseConnection())
            {
                db.ClassDelete(item);
            }
        }

        #endregion

        #region Subclass

        public static ObservableCollection<Subclass> SubclassesRead()
        {
            var items = new ObservableCollection<Subclass>();

            using (var db = new DatabaseConnection())
            {
                 db.SubclassesRead(items);
            }

            return items;
        }
        public static void SubclassInsert(Subclass item)
        {
            using (var db = new DatabaseConnection())
            {
                db.SubclassInsert(item);
            }
        }
        public static void SubclassUpdate(Subclass item)
        {
            using (var db = new DatabaseConnection())
            {
                db.SubclassUpdate(item);
            }
        }
        public static void SubclassDelete(Subclass item)
        {
            using (var db = new DatabaseConnection())
            {
                db.SubclassDelete(item);
            }
        }

        #endregion

        #region Payee

        public static ObservableCollection<Payee> PayeesRead()
        {
            var items = new ObservableCollection<Payee>();

            using (var db = new DatabaseConnection())
            {
                db.PayeesRead(items);
            }

            return items;
        }
        public static void PayeeInsert(Payee item)
        {
            using (var db = new DatabaseConnection())
            {
                db.PayeeInsert(item);
            }
        }
        public static void PayeeUpdate(Payee item)
        {
            using (var db = new DatabaseConnection())
            {
                db.PayeeUpdate(item);
            }
        }
        public static void PayeeDelete(Payee item)
        {
            using (var db = new DatabaseConnection())
            {
                db.PayeeDelete(item);
            }
        }

        #endregion

        #region Schedule Rule

        public static ObservableCollection<ScheduleRule> ScheduleRulesRead()
        {
            var items = new ObservableCollection<ScheduleRule>();

            using (var db = new DatabaseConnection())
            {
                db.ScheduleRulesRead(items);
            }

            return items;
        }

        #endregion
    }
}
