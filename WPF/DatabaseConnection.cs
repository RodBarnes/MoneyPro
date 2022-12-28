using Common;
using MoneyPro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace MoneyPro
{
    public class DatabaseConnection : IDisposable
    {

        #if DEBUG
        private const string DB_LOCATION = "ProjectModels";
        #else
        private const string DB_LOCATION = "MSSQLLocalDB";
        #endif

        private readonly string CONNECTION_STRING = $@"Data Source=(LocalDB)\{DB_LOCATION}; Initial Catalog=Database; " +
                "Integrated Security=True; MultipleActiveResultSets=True";
        private readonly string MASTER_STRING = $@"Data Source=(LocalDB)\{DB_LOCATION}; Initial Catalog=master; " +
                "Integrated Security=True; MultipleActiveResultSets=True";

        private static string DB_FILENAME;
        private static string DB_FILEPATH;

        private readonly SqlConnection sqlConn;

        public DatabaseConnection()
        {
            sqlConn = new SqlConnection(CONNECTION_STRING);
            sqlConn.Open();
        }

        void IDisposable.Dispose() => sqlConn.Close();

        #region Properties

        public Investments InvestmentList { get; set; }
        public List<Payee> NonPayeeList { get; set; }

        #endregion

        #region Special Purpose

        public void ResolveTransfers()
        {
            try
            {
                var cmd = new SqlCommand("spResolveTransfers", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ResolveTransfers)} failed", ex);
            }
        }

        public List<SearchItem> SearchTransactions(SearchQuery query)
        {
            var list = new List<SearchItem>();
            var sql = new StringBuilder();
            var ifAnd = false;

            try
            {
                sql.Append("SELECT * FROM vwSearch WHERE ");
                var cmd = new SqlCommand(sql.ToString(), sqlConn);

                if (!string.IsNullOrEmpty(query.Account))
                {
                    cmd.Parameters.AddWithValue("@account", query.Account);
                    sql.Append($"{(ifAnd ? "AND " : "")}Account=@account ");
                    ifAnd = true;
                }
                if (query.FromDate != null || query.ToDate != null)
                {
                    if (query.FromDate == null)
                    {
                        query.FromDate = new DateTime(1900, 1, 1);
                    }
                    if (query.ToDate == null)
                    {
                        query.ToDate = new DateTime(2500, 1, 1);
                    }
                    cmd.Parameters.AddWithValue("@fromDate", query.FromDate);
                    cmd.Parameters.AddWithValue("@toDate", query.ToDate);
                    sql.Append($"{(ifAnd ? "AND " : "")}(Date>=@fromDate AND Date<=@toDate) ");
                    ifAnd = true;
                }
                if (!string.IsNullOrEmpty(query.Payee))
                {
                    cmd.Parameters.AddWithValue("@payee", query.Payee);
                    sql.Append($"{(ifAnd ? "AND " : "")}Payee=@payee ");
                    ifAnd = true;
                }
                if (!string.IsNullOrEmpty(query.Reference))
                {
                    cmd.Parameters.AddWithValue("@reference", query.Reference);
                    sql.Append($"{(ifAnd ? "AND " : "")}Reference=@reference ");
                    ifAnd = true;
                }
                if (!string.IsNullOrEmpty(query.Memo))
                {
                    cmd.Parameters.AddWithValue("@memo", query.Memo);
                    sql.Append($"{(ifAnd ? "AND " : "")}(TransMemo=@memo OR SubMemo=@memo) ");
                    ifAnd = true;
                }
                if (query.FromAmount != null || query.ToAmount != null)
                {
                    if (query.FromAmount == null)
                    {
                        query.FromAmount = 0;
                    }
                    if (query.ToAmount == null)
                    {
                        query.ToAmount = decimal.MaxValue;
                    }
                    cmd.Parameters.AddWithValue("@fromAmount", query.FromAmount);
                    cmd.Parameters.AddWithValue("@toAmount", query.ToAmount);
                    sql.Append($"{(ifAnd ? "AND " : "")}(Amount>=@fromAmount AND Amount<=@toAmount) ");
                    ifAnd = true;
                }
                if (!string.IsNullOrEmpty(query.Category))
                {
                    cmd.Parameters.AddWithValue("@category", query.Category);
                    sql.Append($"{(ifAnd ? "AND " : "")}Category=@category ");
                    ifAnd = true;
                }
                if (!string.IsNullOrEmpty(query.Subcategory))
                {
                    cmd.Parameters.AddWithValue("@subcategory", query.Subcategory);
                    sql.Append($"{(ifAnd ? "AND " : "")}Subcategory=@subcategory ");
                    ifAnd = true;
                }
                if (!string.IsNullOrEmpty(query.Class))
                {
                    cmd.Parameters.AddWithValue("@class", query.Class);
                    sql.Append($"{(ifAnd ? "AND " : "")}Class=@class ");
                    ifAnd = true;
                }
                if (!string.IsNullOrEmpty(query.Subclass))
                {
                    cmd.Parameters.AddWithValue("@subclass", query.Subclass);
                    sql.Append($"{(ifAnd ? "AND " : "")}Subclass=@subclass ");
                    ifAnd = true;
                }
                cmd.CommandText = sql.ToString();
                var reader = cmd.ExecuteReader();
                if (reader == null)
                {
                    throw new Exception($"{nameof(SearchTransactions)} failed: reader={reader}\nsql={cmd.CommandText}.");
                }
                else
                {
                    while (reader.Read())
                    {
                        var item = new SearchItem
                        {
                            Reference = reader["Reference"].FromSqlString(),
                            Date = DateTime.Parse(reader["Date"].ToString()),
                            Payee = reader["Payee"].FromSqlString(),
                            Amount = decimal.Parse(reader["Amount"].ToString()),
                            TransMemo = reader["TransMemo"].FromSqlString(),
                            SubMemo = reader["SubMemo"].FromSqlString(),
                            Category = reader["Category"].FromSqlString(),
                            Subcategory = reader["Subcategory"].FromSqlString(),
                            Class = reader["Class"].FromSqlString(),
                            Subclass = reader["Subclass"].FromSqlString(),
                            SubtransactionId = (int)reader["SubtransactionId"],
                            TransactionId = (int)reader["TransactionId"],
                            Account = reader["Account"].FromSqlString()
                        };
                        list.Add(item);
                    }
                }
                reader.Close();

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SearchTransactions)} failed.", ex);
            }
        }

        #endregion

        #region Account

        public void AccountsRead(IList<Account> items, AccountStatus statusFilter)
        {
            try
            {
                var cmd = new SqlCommand("spAccountsRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@status", statusFilter);
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Account
                        {
                            AccountId = (int)reader["AccountId"],
                            Name = reader["Name"].FromSqlString(),
                            Type = AccountTypeRead((int)reader["AccountTypeId"]),
                            Institution = reader["Institution"].FromSqlString(),
                            Number = reader["Number"].FromSqlString(),
                            StartingBalance = (decimal)reader["StartingBalance"],
                            Status = (AccountStatus)Enum.Parse(typeof(AccountStatus), reader["Status"].ToString()),
                        };
                        items.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(AccountsRead)} failed: Unable to read accounts; ExecuteReader() returned null.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(AccountsRead)} failed.", ex);
            }
        }

        public void AccountInsert(Account acct)
        {
            try
            {
                var cmd = new SqlCommand("spAccountInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@accountTypeId", acct.Type.Id);
                cmd.Parameters.AddWithValue("@name", acct.Name.ToSqlString());
                cmd.Parameters.AddWithValue("@institution", acct.Institution);
                cmd.Parameters.AddWithValue("@number", acct.Number.ToSqlString());
                cmd.Parameters.AddWithValue("@balance", acct.StartingBalance);
                cmd.Parameters.AddWithValue("@status", acct.Status);
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    acct.AccountId = (int)obj;
                }
                else
                {
                    throw new Exception($"{nameof(AccountInsert)} failed: Account='{acct.Name}'.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(AccountInsert)} failed: Account='{acct.Name}'.", ex);
            }
        }

        public void AccountUpdate(Account acct)
        {
            try
            {
                var cmd = new SqlCommand("spAccountUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@accountId", acct.AccountId);
                cmd.Parameters.AddWithValue("@name", acct.Name.ToSqlString());
                cmd.Parameters.AddWithValue("@institution", acct.Institution);
                cmd.Parameters.AddWithValue("@number", acct.Number.ToSqlString());
                cmd.Parameters.AddWithValue("@balance", acct.StartingBalance);
                cmd.Parameters.AddWithValue("@status", acct.Status);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(AccountUpdate)} failed: AccountId={acct.AccountId}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(AccountUpdate)} failed: AccountId={acct.AccountId}.", ex);
            }
        }

        public void AccountDelete(int accountId)
        {
            try
            {
                var cmd = new SqlCommand("spAccountDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@accountId", accountId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(AccountDelete)} failed: AccountId={accountId}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(AccountDelete)} failed: AccountId={accountId}.", ex);
            }
        }

        #endregion

        #region BankTransaction

        public void BankTransactionsRead(int accountId, TransactionStatus status, List<BankTransaction> list)
        {
            try
            {
                var cmd = new SqlCommand("spBankTransactionsRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@accountId", accountId);
                cmd.Parameters.AddWithValue("@status", status.ToString());
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var trans = new BankTransaction
                        {
                            AccountId = accountId,
                            TransactionId = (int)reader["TransactionId"],
                            Date = DateTime.Parse(reader["Date"].ToString()),
                            Amount = (decimal)reader["Amount"],
                            Memo = reader["Memo"].FromSqlString(),
                            Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), reader["Status"].ToString()),
                            Payee = reader["Payee"].FromSqlString(),
                            Reference = reader["Reference"].FromSqlString(),
                            Void = (bool)reader["Void"]
                        };
                        list.Add(trans);
                    }
                }
                else
                    throw new Exception($"{nameof(BankTransactionsRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(BankTransactionsRead)} failed: AccountId={accountId}.", ex);
            }
        }

        public void BankTransactionInsert(BankTransaction trans)
        {
            try
            {
                var cmd = new SqlCommand("spBankTransactionInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@accountId", trans.AccountId);
                cmd.Parameters.AddWithValue("@date", trans.Date.ToShortDateString());
                cmd.Parameters.AddWithValue("@memo", trans.Memo.ToSqlString());
                cmd.Parameters.AddWithValue("@status", trans.Status.ToString());
                cmd.Parameters.AddWithValue("@payee", trans.Payee.ToSqlString());
                cmd.Parameters.AddWithValue("@reference", trans.Reference.ToSqlString());
                cmd.Parameters.AddWithValue("@void", trans.Void);
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    trans.TransactionId = (int)obj;
                }
                else
                    throw new Exception($"{nameof(BankTransactionInsert)} failed: obj={obj}, Date={trans.Date.ToShortDateString()}, AccountId={trans.AccountId}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(BankTransactionInsert)} failed: Payee={trans.Payee}, Date={trans.Date.ToShortDateString()}, AccountId={trans.AccountId}.", ex);
            }
        }

        public void BankTransactionUpdate(BankTransaction trans)
        {
            try
            {
                var cmd = new SqlCommand("spBankTransactionUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@transactionId", trans.TransactionId);
                cmd.Parameters.AddWithValue("@accountId", trans.AccountId);
                cmd.Parameters.AddWithValue("@date", trans.Date.ToShortDateString());
                cmd.Parameters.AddWithValue("@memo", trans.Memo.ToSqlString());
                cmd.Parameters.AddWithValue("@status", trans.Status.ToString());
                cmd.Parameters.AddWithValue("@payee", trans.Payee.ToSqlString());
                cmd.Parameters.AddWithValue("@reference", trans.Reference.ToSqlString());
                cmd.Parameters.AddWithValue("@void", trans.Void);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(BankTransactionUpdate)} failed: TransactionId={trans.TransactionId}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(BankTransactionUpdate)} failed: TransactionId={trans.TransactionId}.", ex);
            }
        }

        public void BankTransactionDelete(int transactionId)
        {
            try
            {
                var cmd = new SqlCommand("spBankTransactionDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@transactionId", transactionId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(BankTransactionDelete)} failed: TransactionId={transactionId}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(BankTransactionDelete)} failed: TransactionId={transactionId}.", ex);
            }
        }

        public void BankTransactionsDelete(int accountId)
        {
            try
            {
                var cmd = new SqlCommand("spBankTransactionsDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@accountId", accountId);
                var rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(BankTransactionsDelete)} failed: AccountId={accountId}.", ex);
            }
        }

        #endregion

        #region Subtransaction

        public void SubtransactionsRead(int transactionId, List<Subtransaction> list)
        {
            try
            {
                var cmd = new SqlCommand("spSubtransactionsRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@transactionId", transactionId);
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Subtransaction
                        {
                            SubtransactionId = (int)reader["SubtransactionId"],
                            TransactionId = transactionId,
                            Amount = decimal.Parse(reader["Amount"].ToString()),
                            Memo = reader["Memo"].FromSqlString(),
                            Category = reader["Category"].FromSqlString(),
                            Subcategory = reader["Subcategory"].FromSqlString(),
                            Class = reader["Class"].FromSqlString(),
                            Subclass = reader["Subclass"].FromSqlString(),
                            Budget = reader["Budget"].FromSqlString(),
                            XferAccount = reader["XferAccount"].FromSqlString(),
                            XferSubtransactionId = (int)reader["XferSubtransactionId"],
                            XferTransactionId = (int)reader["XferTransactionId"]
                        };
                        list.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(SubtransactionsRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubtransactionsRead)} failed: TransactionId={transactionId}.", ex);
            }
        }
        public void SubtransactionInsert(Subtransaction sub)
        {
            try
            {
                var cmd = new SqlCommand("spSubtransactionInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@transactionId", sub.TransactionId);
                cmd.Parameters.AddWithValue("@amount", sub.Amount);
                cmd.Parameters.AddWithValue("@memo", sub.Memo.ToSqlString());
                cmd.Parameters.AddWithValue("@category", sub.Category);
                cmd.Parameters.AddWithValue("@subcategory", sub.Subcategory);
                cmd.Parameters.AddWithValue("@class", sub.Class);
                cmd.Parameters.AddWithValue("@subclass", sub.Subclass);
                cmd.Parameters.AddWithValue("@xferSubtransactionId", sub.XferSubtransactionId);
                cmd.Parameters.AddWithValue("@xferTransactionId", sub.XferTransactionId);
                cmd.Parameters.AddWithValue("@xferAccount", sub.XferAccount);
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    sub.SubtransactionId = (int)obj;
                }
                else
                    throw new Exception($"{nameof(SubtransactionInsert)} failed: SubtransactionId={sub.SubtransactionId}, TransactionId={sub.TransactionId}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubtransactionInsert)} failed: SubtransactionId={sub.SubtransactionId}, TransactionId={sub.TransactionId}.", ex);
            }
        }

        public void SubtransactionUpdate(Subtransaction sub)
        {
            try
            {
                var cmd = new SqlCommand("spSubtransactionUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@subtransactionId", sub.SubtransactionId);
                cmd.Parameters.AddWithValue("@transactionId", sub.TransactionId);
                cmd.Parameters.AddWithValue("@amount", sub.Amount);
                cmd.Parameters.AddWithValue("@memo", sub.Memo.ToSqlString());
                cmd.Parameters.AddWithValue("@category", sub.Category);
                cmd.Parameters.AddWithValue("@subcategory", sub.Subcategory);
                cmd.Parameters.AddWithValue("@class", sub.Class);
                cmd.Parameters.AddWithValue("@subclass", sub.Subclass);
                cmd.Parameters.AddWithValue("@xferSubtransactionId", sub.XferSubtransactionId);
                cmd.Parameters.AddWithValue("@xferTransactionId", sub.XferTransactionId);
                cmd.Parameters.AddWithValue("@xferAccount", sub.XferAccount);
                var rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    ;   // Success
                }
                else
                    throw new Exception($"{nameof(SubtransactionUpdate)} failed: SubtransactionId={sub.SubtransactionId}, TransactionId={sub.TransactionId}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubtransactionUpdate)} failed: SubtransactionId={sub.SubtransactionId}, TransactionId={sub.TransactionId}.", ex);
            }
        }

        public void SubtransactionDelete(Subtransaction sub)
        {
            try
            {
                var cmd = new SqlCommand("spSubtransactionDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@subtransactionId", sub.SubtransactionId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(SubtransactionDelete)} failed: SubtransactionId={sub.SubtransactionId}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubtransactionDelete)} failed: SubtransactionId={sub.SubtransactionId}.", ex);
            }
        }

        public void SubtransactionsDeleteForTransaction(int transactionId)
        {
            try
            {
                var cmd = new SqlCommand("spSubtransactionsDeleteForTransaction", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@transactionId", transactionId);
                var rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubtransactionsDeleteForAccount)} failed: TransactionId={transactionId}.", ex);
            }
        }

        public void SubtransactionsDeleteForAccount(int accountId)
        {
            try
            {
                var cmd = new SqlCommand("spSubtransactionsDeleteForAccount", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@accountId", accountId);
                var rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubtransactionsDeleteForAccount)} failed: AccountId={accountId}.", ex);
            }
        }

        #endregion

        #region InvTransaction

        public void InvTransactionsRead(int accountId, List<InvTransaction> list)
        {
            var cmd = new SqlCommand("spInvTransactionsRead", sqlConn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@accountId", accountId);
            var reader = cmd.ExecuteReader();
            if (reader != null)
            {
                while (reader.Read())
                {
                    var item = new InvTransaction
                    {
                        AccountId = accountId,
                        TransactionId = (int)reader["TransactionId"],
                        Date = DateTime.Parse(reader["Date"].ToString()),
                        Memo = reader["Memo"].FromSqlString(),
                        Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), reader["Status"].ToString()),
                        Action = reader["Action"].ToString(),
                        Security = reader["Security"].ToString(),
                        Description = reader["Description"].FromSqlString(),
                        Price = decimal.Parse(reader["Price"].ToString()),
                        Quantity = (int)reader["Quantity"],
                        Commission = decimal.Parse(reader["Commission"].ToString()),
                        TransferAmount = decimal.Parse(reader["TransferAmount"].ToString())
                    };
                    list.Add(item);
                }
            }
            else
                throw new Exception($"{nameof(InvTransactionsRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

            reader.Close();
        }

        public void InvTransactionInsert(InvTransaction trans)
        {
            try
            {
                var cmd = new SqlCommand("spInvTransactionInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@accountId", trans.AccountId);
                cmd.Parameters.AddWithValue("@date", trans.Date.ToShortDateString());
                cmd.Parameters.AddWithValue("@memo", trans.Memo);
                cmd.Parameters.AddWithValue("@status", trans.Status.ToString());
                cmd.Parameters.AddWithValue("@action", trans.Action);
                cmd.Parameters.AddWithValue("@ticker", trans.Security);
                cmd.Parameters.AddWithValue("@desc", trans.Description.ToSqlString());
                cmd.Parameters.AddWithValue("@price", trans.Price);
                cmd.Parameters.AddWithValue("@qty", trans.Quantity);
                cmd.Parameters.AddWithValue("@commission", trans.Commission);
                cmd.Parameters.AddWithValue("@xferAmount", trans.TransferAmount);
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    trans.TransactionId = (int)obj;
                    InvestmentManage(trans.Security);
                }
                else
                    throw new Exception($"{nameof(InvTransactionInsert)} failed: Date={trans.Date.ToShortDateString()}, Security={trans.Security}, AccountId={trans.AccountId}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(InvTransactionInsert)} failed: Date={trans.Date.ToShortDateString()}, Security={trans.Security}, AccountId={trans.AccountId}.", ex);
            }
        }

        public void InvTransactionUpdate(InvTransaction trans)
        {
            try
            {
                var cmd = new SqlCommand("spInvTransactionUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@date", trans.Date.ToShortDateString());
                cmd.Parameters.AddWithValue("@memo", trans.Memo);
                cmd.Parameters.AddWithValue("@status", trans.Status.ToString());
                cmd.Parameters.AddWithValue("@action", trans.Action);
                cmd.Parameters.AddWithValue("@security", trans.Security);
                cmd.Parameters.AddWithValue("@desc", trans.Description.ToSqlString());
                cmd.Parameters.AddWithValue("@price", trans.Price);
                cmd.Parameters.AddWithValue("@qty", trans.Quantity);
                cmd.Parameters.AddWithValue("@commission", trans.Commission);
                cmd.Parameters.AddWithValue("@xferAmount", trans.TransferAmount);
                var rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                    InvestmentManage(trans.Security);
                else
                    throw new Exception($"{nameof(InvTransactionUpdate)} failed: TransactionId={trans.TransactionId}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(InvTransactionUpdate)} failed: TransactionId={trans.TransactionId}.", ex);
            }
        }

        public void InvTransactionDelete(InvTransaction trans)
        {
            try
            {
                var cmd = new SqlCommand("spInvTransactionDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@transactionId", trans.TransactionId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(InvTransactionDelete)} failed: TransactionId={trans.TransactionId}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(InvTransactionDelete)} failed: TransactionId={trans.TransactionId}.", ex);
            }
        }

        #endregion

        #region ScheduleTransaction

        public void ScheduleTransactionsRead(List<ScheduleTransaction> list)
        {
            try
            {
                var cmd = new SqlCommand("spScheduleTransactionsRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var trans = new ScheduleTransaction
                        {
                            ScheduleTransactionId = (int)reader["ScheduleTransactionId"],
                            Rule = reader["Rule"].FromSqlString(),
                            Account = reader["Account"].FromSqlString(),
                            Payee = reader["Payee"].FromSqlString(),
                            Amount = (decimal)reader["Amount"],
                            Memo = reader["Memo"].FromSqlString(),
                            NextDate = DateTime.Parse(reader["NextDate"].ToString())
                        };
                        if (reader["CountEnd"] != DBNull.Value)
                        {
                            trans.CountEnd = (int)reader["CountEnd"];
                        }
                        if (reader["DateEnd"] != DBNull.Value)
                        {
                            trans.DateEnd = DateTime.Parse(reader["DateEnd"].ToString());
                        }
                        if (reader["EnterDaysBefore"] != DBNull.Value)
                        {
                            trans.EnterDaysBefore = (int)reader["EnterDaysBefore"];
                        }
                        list.Add(trans);
                    }
                }
                else
                    throw new Exception($"{nameof(ScheduleTransactionsRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ScheduleTransactionsRead)} failed.", ex);
            }
        }

        public void ScheduleTransactionInsert(ScheduleTransaction sched)
        {
            try
            {
                var cmd = new SqlCommand("spScheduleTransactionInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@rule", sched.Rule);
                cmd.Parameters.AddWithValue("@account", sched.Account);
                cmd.Parameters.AddWithValue("@payee", sched.Payee);
                cmd.Parameters.AddWithValue("@memo", sched.Memo);
                cmd.Parameters.AddWithValue("@nextDate", sched.NextDate);
                if (sched.CountEnd != null)
                {
                    cmd.Parameters.AddWithValue("@countEnd", sched.CountEnd);
                }
                if (sched.DateEnd != null)
                {
                    cmd.Parameters.AddWithValue("@dateEnd", sched.DateEnd);
                }
                if (sched.EnterDaysBefore != null)
                {
                    cmd.Parameters.AddWithValue("@enterDaysBefore", sched.EnterDaysBefore);
                }
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    sched.ScheduleTransactionId = (int)obj;
                }
                else
                    throw new Exception($"{nameof(ScheduleTransactionInsert)} failed: obj={obj}, NextDate={sched.NextDate.ToShortDateString()}, Account={sched.Account}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ScheduleTransactionInsert)} failed: Payee={sched.Payee}, NextDate={sched.NextDate.ToShortDateString()}, Account={sched.Account}.", ex);
            }
        }

        public void ScheduleTransactionUpdate(ScheduleTransaction sched)
        {
            try
            {
                var cmd = new SqlCommand("spScheduleTransactionUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@scheduleTransactionId", sched.ScheduleTransactionId);
                cmd.Parameters.AddWithValue("@rule", sched.Rule);
                cmd.Parameters.AddWithValue("@account", sched.Account);
                cmd.Parameters.AddWithValue("@payee", sched.Payee);
                cmd.Parameters.AddWithValue("@memo", sched.Memo);
                cmd.Parameters.AddWithValue("@nextDate", sched.NextDate);
                cmd.Parameters.AddWithValue("@countEnd", sched.CountEnd);
                cmd.Parameters.AddWithValue("@dateEnd", sched.DateEnd);
                cmd.Parameters.AddWithValue("@enterDaysBefore", sched.EnterDaysBefore);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(BankTransactionUpdate)} failed: TransactionId={sched.ScheduleTransactionId}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(BankTransactionUpdate)} failed: TransactionId={sched.ScheduleTransactionId}.", ex);
            }
        }

        public void ScheduleTransactionDelete(int scheduleTransactionId)
        {
            try
            {
                var cmd = new SqlCommand("spScheduleTransactionDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@scheduleTransactionId", scheduleTransactionId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(ScheduleTransactionDelete)} failed: ScheduleTransactionId={scheduleTransactionId}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ScheduleTransactionDelete)} failed: ScheduleTransactionId={scheduleTransactionId}.", ex);
            }
        }

        #endregion

        #region ScheduleSubtransaction

        public void ScheduleSubtransactionsRead(int scheduleTransactionId, List<ScheduleSubtransaction> list)
        {
            try
            {
                var cmd = new SqlCommand("spScheduleSubtransactionsRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@scheduleTransactionId", scheduleTransactionId);
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new ScheduleSubtransaction
                        {
                            ScheduleSubtransactionId = (int)reader["ScheduleSubtransactionId"],
                            ScheduleTransactionId = scheduleTransactionId,
                            Amount = decimal.Parse(reader["Amount"].ToString()),
                            Memo = reader["Memo"].FromSqlString(),
                            Category = reader["Category"].FromSqlString(),
                            Subcategory = reader["Subcategory"].FromSqlString(),
                            Class = reader["Class"].FromSqlString(),
                            Subclass = reader["Subclass"].FromSqlString(),
                            Budget = reader["Budget"].FromSqlString()
                        };
                        list.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(ScheduleSubtransactionsRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ScheduleSubtransactionsRead)} failed: ScheduleTransactionId={scheduleTransactionId}.", ex);
            }
        }
        public void ScheduleSubtransactionInsert(ScheduleSubtransaction sub)
        {
            try
            {
                var cmd = new SqlCommand("spScheduleSubtransactionInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@scheduleTransactionId", sub.ScheduleTransactionId);
                cmd.Parameters.AddWithValue("@memo", sub.Memo.ToSqlString());
                cmd.Parameters.AddWithValue("@category", sub.Category);
                cmd.Parameters.AddWithValue("@subcategory", sub.Subcategory);
                cmd.Parameters.AddWithValue("@class", sub.Class);
                cmd.Parameters.AddWithValue("@subclass", sub.Subclass);
                cmd.Parameters.AddWithValue("@budget", sub.Budget);
                cmd.Parameters.AddWithValue("@amount", sub.Amount);
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    sub.ScheduleSubtransactionId = (int)obj;
                }
                else
                    throw new Exception($"{nameof(ScheduleSubtransactionInsert)} failed: ScheduleSubtransactionId={sub.ScheduleSubtransactionId}, ScheduleTransactionId={sub.ScheduleTransactionId}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ScheduleSubtransactionInsert)} failed: ScheduleSubtransactionId={sub.ScheduleSubtransactionId}, ScheduleTransactionId={sub.ScheduleTransactionId}.", ex);
            }
        }

        public void ScheduleSubtransactionUpdate(ScheduleSubtransaction sub)
        {
            try
            {
                var cmd = new SqlCommand("spScheduleSubtransactionUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@scheduleSubtransactionId", sub.ScheduleSubtransactionId);
                cmd.Parameters.AddWithValue("@scheduleTransactionId", sub.ScheduleTransactionId);
                cmd.Parameters.AddWithValue("@memo", sub.Memo.ToSqlString());
                cmd.Parameters.AddWithValue("@category", sub.Category);
                cmd.Parameters.AddWithValue("@subcategory", sub.Subcategory);
                cmd.Parameters.AddWithValue("@class", sub.Class);
                cmd.Parameters.AddWithValue("@subclass", sub.Subclass);
                cmd.Parameters.AddWithValue("@budget", sub.Budget);
                cmd.Parameters.AddWithValue("@amount", sub.Amount);
                var rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    ;   // Success
                }
                else
                    throw new Exception($"{nameof(ScheduleSubtransactionUpdate)} failed: ScheduleSubtransactionId={sub.ScheduleSubtransactionId}, ScheduleTransactionId={sub.ScheduleTransactionId}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ScheduleSubtransactionUpdate)} failed: ScheduleSubtransactionId={sub.ScheduleSubtransactionId}, ScheduleTransactionId={sub.ScheduleTransactionId}.", ex);
            }
        }

        public void ScheduleSubtransactionDelete(ScheduleSubtransaction sub)
        {
            try
            {
                var cmd = new SqlCommand("spScheduleSubtransactionDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@scheduleSubtransactionId", sub.ScheduleSubtransactionId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(ScheduleSubtransactionDelete)} failed: ScheduleSubtransactionId={sub.ScheduleSubtransactionId}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ScheduleSubtransactionDelete)} failed: ScheduleSubtransactionId={sub.ScheduleSubtransactionId}.", ex);
            }
        }

        public void ScheduleSubtransactionsDeleteForScheduleTransaction(int scheduleTransactionId)
        {
            try
            {
                var cmd = new SqlCommand("spScheduleSubtransactionsDeleteForScheduleTransaction", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@scheduleTransactionId", scheduleTransactionId);
                var rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubtransactionsDeleteForAccount)} failed: ScheduleTransactionId={scheduleTransactionId}.", ex);
            }
        }


        #endregion

        #region AccountTypes

        public void AccountTypesRead(ObservableCollection<AccountType> items)
        {
            try
            {
                var cmd = new SqlCommand("spAccountTypesRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new AccountType
                        {
                            Id = (int)reader["AccountTypeId"],
                            ImportName = reader["ImportName"].FromSqlString(),
                            DisplayName = reader["DisplayName"].FromSqlString()
                        };
                        items.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(AccountTypesRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(AccountTypesRead)} failed.", ex);
            }
        }

        public AccountType AccountTypeRead(int accountTypeId)
        {
            AccountType acctType = new AccountType();

            try
            {
                var cmd = new SqlCommand("spAccountTypeRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@accountTypeId", accountTypeId);
                var reader = cmd.ExecuteReader();
                if (reader == null)
                {
                    throw new Exception($"{nameof(AccountTypeRead)} failed: reader={reader}\nsql={cmd.CommandText}.");
                }
                else
                {
                    while (reader.Read())
                    {
                        acctType.Id = accountTypeId;
                        acctType.ImportName = reader["ImportName"].FromSqlString();
                        acctType.DisplayName = reader["DisplayName"].FromSqlString();
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(AccountTypeRead)} failed.", ex);
            }

            return acctType;
        }

        #endregion

        #region Institution

        public void InstitutionsRead(List<Institution> items)
        {
            try
            {
                var cmd = new SqlCommand("spInstitutionsRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Institution
                        {
                            InstitutionId = (int)reader["InstitutionId"],
                            Name = reader["Name"].FromSqlString(),
                            URL = reader["URL"].FromSqlString(),
                            Email = reader["Email"].FromSqlString(),
                            Phone = reader["Phone"].FromSqlString(),
                            Street = reader["Street"].FromSqlString(),
                            City = reader["City"].FromSqlString(),
                            State = reader["State"].FromSqlString(),
                            Zip = reader["Zip"].FromSqlString()
                        };
                        items.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(InstitutionsRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(InstitutionsRead)} failed.", ex);
            }
        }

        public void InstitutionInsert(Institution item)
        {
            try
            {
                var cmd = new SqlCommand("spInstitutionInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@name", item.Name.ToSqlString());
                cmd.Parameters.AddWithValue("@url", item.URL.ToSqlString());
                cmd.Parameters.AddWithValue("@email", item.Email.ToSqlString());
                cmd.Parameters.AddWithValue("@phone", item.Phone.ToSqlString());
                cmd.Parameters.AddWithValue("@street", item.Street.ToSqlString());
                cmd.Parameters.AddWithValue("@city", item.City.ToSqlString());
                cmd.Parameters.AddWithValue("@state", item.State.ToSqlString());
                cmd.Parameters.AddWithValue("@zip", item.Zip.ToSqlString());
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    item.InstitutionId = (int)obj;
                }
                else
                {
                    throw new Exception($"{nameof(InstitutionInsert)} failed: Institution='{item.Name}'.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(InstitutionInsert)} failed: Institution='{item.Name}'.", ex);
            }
        }

        public void InstitutionUpdate(Institution item)
        {
            int rows;

            try
            {
                var cmd = new SqlCommand("spInstitutionUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@institutionId", item.InstitutionId);
                cmd.Parameters.AddWithValue("@name", item.Name.ToSqlString());
                cmd.Parameters.AddWithValue("@url", item.URL.ToSqlString());
                cmd.Parameters.AddWithValue("@email", item.Email.ToSqlString());
                cmd.Parameters.AddWithValue("@phone", item.Phone.ToSqlString());
                cmd.Parameters.AddWithValue("@street", item.Street.ToSqlString());
                cmd.Parameters.AddWithValue("@city", item.City.ToSqlString());
                cmd.Parameters.AddWithValue("@state", item.State.ToSqlString());
                cmd.Parameters.AddWithValue("@zip", item.Zip.ToSqlString());
                rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(InstitutionUpdate)} failed: Name={item.Name}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(InstitutionUpdate)} failed: Name={item.Name}.", ex);
            }
        }

        public void InstitutionDelete(Institution item)
        {
            try
            {
                var cmd = new SqlCommand("spInstitutionDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@institutionId", item.InstitutionId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(InstitutionDelete)} failed: Name={item.Name}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(InstitutionDelete)} failed: Name={item.Name}.", ex);
            }
        }

        #endregion

        #region Reference

        public void ReferencesRead(ObservableCollection<Reference> items)
        {
            try
            {
                var cmd = new SqlCommand("spReferencesRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Reference
                        {
                            Text = reader["Reference"].FromSqlString()
                        };
                        items.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(ReferencesRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ReferencesRead)} failed.", ex);
            }
        }

        #endregion

        #region Payee

        public void PayeesRead(ObservableCollection<Payee> items)
        {
            try
            {
                var cmd = new SqlCommand("spPayeesRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        DateTime? date;
                        var dateVal = reader["DateLastUsed"];
                        if (dateVal == DBNull.Value || dateVal == null)
                        {
                            date = null;
                        }
                        else
                        {
                            date = DateTime.Parse(dateVal.ToString());
                        }    
                        var item = new Payee
                        {
                            PayeeId = (int)reader["PayeeId"],
                            Name = reader["Name"].FromSqlString(),
                            DateLastUsed = date
                        };
                        items.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(PayeesRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(PayeesRead)} failed.", ex);
            }
        }

        public void PayeeInsert(Payee item)
        {
            try
            {
                var cmd = new SqlCommand("spPayeeInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@name", item.Name.ToSqlString());
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    item.PayeeId = (int)obj;
                }
                else
                    throw new Exception($"{nameof(PayeeInsert)} failed: Name={item.Name}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(PayeeInsert)} failed: Name={item.Name}.", ex);
            }
        }

        public void PayeeUpdate(Payee item)
        {
            try
            {
                var cmd = new SqlCommand("spPayeeUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@payeeId", item.PayeeId);
                cmd.Parameters.AddWithValue("@name", item.Name.ToSqlString());
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(CategoryUpdate)} failed: Name={item.Name}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(CategoryUpdate)} failed: Name={item.Name}.", ex);
            }
        }

        public void PayeeDelete(Payee item)
        {
            try
            {
                var cmd = new SqlCommand("spPayeeDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@payeeId", item.PayeeId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(PayeeDelete)} failed: Name={item.Name}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(PayeeDelete)} failed: Name={item.Name}.", ex);
            }
        }

        #endregion

        #region Category

        public void CategoriesRead(ObservableCollection<Category> items)
        {
            try
            {
                var cmd = new SqlCommand("spCategoriesRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Category
                        {
                            CategoryId = (int)reader["CategoryId"],
                            Text = reader["Text"].FromSqlString(),
                            Tax = (bool)reader["Tax"]
                        };
                        items.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(CategoriesRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(CategoriesRead)} failed.", ex);
            }
        }

        public void CategoryInsert(Category item)
        {
            try
            {
                var cmd = new SqlCommand("spCategoryInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@text", item.Text.ToSqlString());
                cmd.Parameters.AddWithValue("@tax", item.Tax);
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    item.CategoryId = (int)obj;
                }
                else
                {
                    throw new Exception($"{nameof(CategoryInsert)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(CategoryInsert)} failed: Text={item.Text}.", ex);
            }
        }

        public void CategoryUpdate(Category item)
        {
            int rows;

            try
            {
                var cmd = new SqlCommand("spCategoryUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@categoryId", item.CategoryId);
                cmd.Parameters.AddWithValue("@text", item.Text.ToSqlString());
                cmd.Parameters.AddWithValue("@tax", item.Tax);
                rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(CategoryUpdate)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(CategoryUpdate)} failed: Text={item.Text}.", ex);
            }
        }

        public void CategoryDelete(Category item)
        {
            try
            {
                var cmd = new SqlCommand("spCategoryDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@categoryId", item.CategoryId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(CategoryDelete)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(CategoryDelete)} failed: Text={item.Text}.", ex);
            }
        }

        #endregion

        #region Subcategory

        public void SubcategoriesRead(ObservableCollection<Subcategory> items)
        {
            try
            {
                var cmd = new SqlCommand("spSubcategoriesRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Subcategory
                        {
                            SubcategoryId = (int)reader["SubcategoryId"],
                            Text = reader["Text"].FromSqlString(),
                            Tax = (bool)reader["Tax"]
                        };
                        items.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(SubcategoriesRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubcategoriesRead)} failed.", ex);
            }
        }

        public void SubcategoryInsert(Subcategory item)
        {
            try
            {
                var cmd = new SqlCommand("spSubcategoryInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@text", item.Text.ToSqlString());
                cmd.Parameters.AddWithValue("@tax", item.Tax);
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    item.SubcategoryId = (int)obj;
                }
                else
                {
                    throw new Exception($"{nameof(SubcategoryInsert)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubcategoryInsert)} failed: Text={item.Text}.", ex);
            }
        }

        public void SubcategoryUpdate(Subcategory item)
        {
            int rows;

            try
            {
                var cmd = new SqlCommand("spSubcategoryUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@subcategoryId", item.SubcategoryId);
                cmd.Parameters.AddWithValue("@text", item.Text.ToSqlString());
                cmd.Parameters.AddWithValue("@tax", item.Tax);
                rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(SubcategoryUpdate)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubcategoryUpdate)} failed: Text={item.Text}.", ex);
            }
        }

        public void SubcategoryDelete(Subcategory item)
        {
            try
            {
                var cmd = new SqlCommand("spSubcategoryDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@subcategoryId", item.SubcategoryId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(SubcategoryDelete)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubcategoryDelete)} failed: Text={item.Text}.", ex);
            }
        }
 
        #endregion

        #region Class

        public void ClassesRead(ObservableCollection<Class> items)
        {
            try
            {
                var cmd = new SqlCommand("spClassesRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Class
                        {
                            ClassId = (int)reader["ClassId"],
                            Text = reader["Text"].FromSqlString()
                        };
                        items.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(ClassesRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ClassesRead)} failed.", ex);
            }
        }

        public void ClassInsert(Class item)
        {
            try
            {
                var cmd = new SqlCommand("spClassInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@text", item.Text.ToSqlString());
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    item.ClassId = (int)obj;
                }
                else
                {
                    throw new Exception($"{nameof(ClassInsert)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ClassInsert)} failed: Text={item.Text}.", ex);
            }
        }

        public void ClassUpdate(Class item)
        {
            int rows;

            try
            {
                var cmd = new SqlCommand("spClassUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@classId", item.ClassId);
                cmd.Parameters.AddWithValue("@text", item.Text.ToSqlString());
                rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(ClassUpdate)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ClassUpdate)} failed: Text={item.Text}.", ex);
            }
        }

        public void ClassDelete(Class item)
        {
            try
            {
                var cmd = new SqlCommand("spClassDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@classId", item.ClassId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(ClassDelete)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ClassDelete)} failed: Text={item.Text}.", ex);
            }
        }

        #endregion

        #region Subclass

        public void SubclassesRead(ObservableCollection<Subclass> items)
        {
            try
            {
                var cmd = new SqlCommand("spSubclassesRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Subclass
                        {
                            SubclassId = (int)reader["SubclassId"],
                            Text = reader["Text"].FromSqlString()
                        };
                        items.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(SubclassesRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubclassesRead)} failed.", ex);
            }
        }

        public void SubclassInsert(Subclass item)
        {
            try
            {
                var cmd = new SqlCommand("spSubclassInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@text", item.Text.ToSqlString());
                var obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    item.SubclassId = (int)obj;
                }
                else
                {
                    throw new Exception($"{nameof(SubclassInsert)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubclassInsert)} failed: Text={item.Text}.", ex);
            }
        }

        public void SubclassUpdate(Subclass item)
        {
            try
            {
                var cmd = new SqlCommand("spSubclassUpdate", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@subclassId", item.SubclassId);
                cmd.Parameters.AddWithValue("@text", item.Text.ToSqlString());
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(SubclassUpdate)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubclassUpdate)} failed: Text={item.Text}.", ex);
            }
        }

        public void SubclassDelete(Subclass item)
        {
            try
            {
                var cmd = new SqlCommand("spSubclassDelete", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@subclassId", item.SubclassId);
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(SubclassDelete)} failed: Text={item.Text}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(SubclassDelete)} failed: Text={item.Text}.", ex);
            }
        }

        #endregion

        #region ScheduleRule

        public void ScheduleRulesRead(ObservableCollection<ScheduleRule> items)
        {
            try
            {
                var cmd = new SqlCommand("spScheduleRulesRead", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new ScheduleRule
                        {
                            ScheduleRuleId = (int)reader["ScheduleRuleId"],
                            Name = reader["Name"].FromSqlString(),
                            Number = (double)reader["Number"],
                            Interval = reader["Interval"].FromSqlString()
                        };
                        items.Add(item);
                    }
                }
                else
                    throw new Exception($"{nameof(ScheduleRulesRead)} failed: reader={reader}\nsql={cmd.CommandText}.");

                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(ScheduleRulesRead)} failed.", ex);
            }
        }

        #endregion

        #region Investment

        public void InvestmentInsert(Investment inv)
        {
            try
            {
                var cmd = new SqlCommand("spInvestmentInsert", sqlConn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ticker", inv.Ticker);
                cmd.Parameters.AddWithValue("@company", inv.Company.ToSqlString());
                cmd.Parameters.AddWithValue("@source", inv.Source.ToSqlString());
                var rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(InvestmentInsert)} failed: returned row count of zero for entry '{inv.Ticker}'.");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    // Some stocks dual list (more than one exchange) so update Source by appending the new source to the existing
                    object obj;
                    var sql = "SELECT Source FROM Investment WHERE Ticker = @ticker";
                    var cmd = new SqlCommand(sql, sqlConn);
                    cmd.Parameters.AddWithValue("@ticker", inv.Ticker);
                    obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        try
                        {
                            var source = $"{obj.ToString()},{inv.Source}";
                            int rows;
                            sql = $"UPDATE Investment SET Source = '{source}' WHERE Ticker = '{inv.Ticker}'";
                            rows = cmd.ExecuteNonQuery();
                            if (rows <= 0)
                                throw new Exception($"{nameof(InvestmentInsert)} failed: returned no rows for update of entry '{inv.Ticker}'.");
                        }
                        catch (SqlException ex2)
                        {
                            throw new Exception($"{nameof(InvestmentInsert)} failed: Ticker='{inv.Ticker}'.", ex2);
                        }
                    }
                    else
                    {
                        // Do we care about this?
                        ;
                    }
                }
                else
                    throw new Exception($"{nameof(InvestmentInsert)} failed: Ticker='{inv.Ticker}'.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(InvestmentInsert)} failed: Ticker='{inv.Ticker}'.", ex);
            }
        }

        private void InvestmentFill()
        {
            InvestmentList = new Investments();

            try
            {
                var sql = "SELECT Ticker, Company, Source FROM Investment";
                var cmd = new SqlCommand(sql, sqlConn);
                var reader = cmd.ExecuteReader();
                if (reader == null)
                {
                    throw new Exception($"{nameof(InvestmentFill)} query failed: reader={reader}\nsql={cmd.CommandText}.");
                }
                else
                {
                    while (reader.Read())
                    {
                        var item = new Investment
                        {
                            Ticker = reader["Ticker"].ToString(),
                            Company = reader["Company"].FromSqlString(),
                            Source = reader["Source"].ToString()
                        };
                        InvestmentList.Add(item);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(InvestmentFill)} failed: reading non payees.", ex);
            }
        }

        private void InvestmentManage(string text)
        {
            // Read an existing security by its name or add it if it doesn't exist
            if (InvestmentList == null)
                InvestmentFill();

            var ticker = InvestmentList.GetTickerForCompany(text);
            if (string.IsNullOrEmpty(ticker))
            {
                if (InvestmentList.Exists(text))
                {
                    // The value is the ticker so just return
                    return;
                }
                else
                    throw new Exception($"{nameof(InvestmentManage)}: Unrecognized Security '{text}'.");
            }

            try
            {
                object obj;
                var sql = "SELECT [Ticker] FROM Investment WHERE [Ticker]=@ticker";
                var cmd = new SqlCommand(sql, sqlConn);
                cmd.Parameters.AddWithValue("@ticker", ticker);
                obj = cmd.ExecuteScalar();
                if (obj == null)
                {
                    // Doesn't exist; insert the new record
                    cmd.CommandText = "INSERT INTO Investment (Ticker, Company) VALUES (@ticker, @company)";
                }
                else
                {
                    // Exists; update date DateLastUsed
                    cmd.CommandText = "UPDATE Investment SET Company = @company WHERE Ticker=@ticker";
                }

                int rows;
                cmd.Parameters.AddWithValue("@company", text.ToSqlString());
                rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                    throw new Exception($"{nameof(InvestmentManage)} failed: ticker={ticker}\nsql={cmd.CommandText}.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(InvestmentManage)} failed: ticker={ticker}.", ex);
            }
        }

        #endregion

        #region Database Administration

        public void InitDataPaths(string dbFilepath, string dbFilename)
        {
            DB_FILENAME = dbFilename;
            DB_FILEPATH = dbFilepath;
        }

        public void BackupDatabase(string backupPath)
        {
            if (string.IsNullOrEmpty(DB_FILENAME))
            {
                throw new ArgumentNullException(nameof(DB_FILENAME), "Unititialized variable. Call to InitDataPaths() required.");
            }
            if (string.IsNullOrEmpty(DB_FILEPATH))
            {
                throw new ArgumentNullException(nameof(DB_FILEPATH), "Unititialized variable. Call to InitDataPaths() required.");
            }

            DetachDatabase();
            ArchiveDatabaseFiles(backupPath);
            AttachDatabase();
        }

        public void OpenDatabase(string filepath)
        {
            if (string.IsNullOrEmpty(DB_FILENAME))
            {
                throw new ArgumentNullException(nameof(DB_FILENAME), "Unititialized variable. Call to InitDataPaths() required.");
            }
            if (string.IsNullOrEmpty(DB_FILEPATH))
            {
                throw new ArgumentNullException(nameof(DB_FILEPATH), "Unititialized variable. Call to InitDataPaths() required.");
            }

            DetachDatabase();
            ArchiveDatabaseFiles(filepath);
            ExtractDatabaseFiles(filepath);
            AttachDatabase();
        }

        private void ArchiveDatabaseFiles(string archivePath)
        {
            if (string.IsNullOrEmpty(DB_FILENAME))
            {
                throw new ArgumentNullException(nameof(DB_FILENAME), "Unititialized variable. Call to InitDataPaths() required.");
            }
            if (string.IsNullOrEmpty(DB_FILEPATH))
            {
                throw new ArgumentNullException(nameof(DB_FILEPATH), "Unititialized variable. Call to InitDataPaths() required.");
            }

            var path = Path.GetDirectoryName(archivePath);
            Archive.Compress(DB_FILEPATH, $"{DB_FILENAME}.?df", $@"{path}\MoneyPro_{DateTime.Now:yyyymmdd}.7z");
        }

        private void ExtractDatabaseFiles(string archivePath)
        {
            if (string.IsNullOrEmpty(DB_FILEPATH))
            {
                throw new ArgumentNullException(nameof(DB_FILEPATH), "Unititialized variable. Call to InitDataPaths() required.");
            }
            
            Archive.Extract(archivePath, DB_FILEPATH);
        }

        private void BackupDatabaseFiles(string backupPath)
        {
            if (string.IsNullOrEmpty(DB_FILENAME))
            {
                throw new ArgumentNullException(nameof(DB_FILENAME), "Unititialized variable. Call to InitDataPaths() required.");
            }
            if (string.IsNullOrEmpty(DB_FILEPATH))
            {
                throw new ArgumentNullException(nameof(DB_FILEPATH), "Unititialized variable. Call to InitDataPaths() required.");
            }

            var path = Path.GetDirectoryName(backupPath);
            var datetime = DateTime.Now;
            File.Copy($@"{DB_FILEPATH}\{DB_FILENAME}.mdf", $@"{path}\{DB_FILENAME}_{datetime:yyyyMMddhhmmss}.mdf");
            File.Copy($@"{DB_FILEPATH}\{DB_FILENAME}.ldf", $@"{path}\{DB_FILENAME}_{datetime:yyyyMMddhhmmss}.ldf");
        }

        private void RestoreDatabaseFiles(string filepath)
        {
            if (string.IsNullOrEmpty(DB_FILENAME))
            {
                throw new ArgumentNullException(nameof(DB_FILENAME), "Unititialized variable. Call to InitDataPaths() required.");
            }
            if (string.IsNullOrEmpty(DB_FILEPATH))
            {
                throw new ArgumentNullException(nameof(DB_FILEPATH), "Unititialized variable. Call to InitDataPaths() required.");
            }

            var backupPath = Path.GetDirectoryName(filepath);
            var filename = Path.GetFileNameWithoutExtension(filepath);

            File.Copy($@"{backupPath}\{filename}.mdf", $@"{DB_FILEPATH}\{DB_FILENAME}.mdf", true);
            File.Copy($@"{backupPath}\{filename}.ldf", $@"{DB_FILEPATH}\{DB_FILENAME}.ldf", true);
        }

        private void DetachDatabase()
        {
            sqlConn.Close();

            using (var mstConn = new SqlConnection(MASTER_STRING))
            {
                try
                {
                    mstConn.Open();

                    object obj;
                    var sql = "ALTER DATABASE [Database] SET OFFLINE WITH ROLLBACK IMMEDIATE; ALTER DATABASE [Database] SET SINGLE_USER; EXEC sp_detach_db 'Database'";
                    var cmd = new SqlCommand(sql, mstConn);
                    obj = cmd.ExecuteNonQuery();
                    if (obj == null)
                        throw new Exception($"{nameof(DetachDatabase)} failed: obj={obj}.\nsql={cmd.CommandText}");
                }
                catch (SqlException ex)
                {
                    throw new Exception($"{nameof(DetachDatabase)} failed.", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"{nameof(DetachDatabase)} failed.", ex);
                }
            }
        }

        private void AttachDatabase()
        {
            using (var mstConn = new SqlConnection(MASTER_STRING))
            {
                try
                {
                    mstConn.Open();

                    object obj;
                    var sql = $@"EXEC sp_attach_db @dbname=N'Database', @filename1 = N'{DB_FILEPATH}\{DB_FILENAME}.mdf', @filename2 =N'{DB_FILEPATH}\{DB_FILENAME}.ldf'";
                    var cmd = new SqlCommand(sql, mstConn);
                    obj = cmd.ExecuteNonQuery();
                    if (obj == null)
                        throw new Exception($"{nameof(DetachDatabase)} failed: obj={obj}.\nsql={cmd.CommandText}");
                }
                catch (SqlException ex)
                {
                    throw new Exception($"{nameof(DetachDatabase)} failed.", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"{nameof(DetachDatabase)} failed.", ex);
                }
            }

            sqlConn.Open();
        }

        #endregion

    }
}
