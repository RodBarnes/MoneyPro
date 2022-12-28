using System;
using System.Collections.Generic;

namespace MoneyPro.Models
{
    public class Account : ICloneable
    {
        #region Properties

        public int AccountId { get; set; } = 0;
        public AccountType Type { get; set; } = null;
        public AccountStatus Status { get; set; } = AccountStatus.Open;
        public string Name { get; set; } = "";
        public string Number { get; set; } = "";
        public string Institution { get; set; } = "";
        public decimal StartingBalance { get; set; } = 0;

        #endregion

        #region Methods

        public void Insert() => DatabaseManager.AccountInsert(this);
        public void Update() => DatabaseManager.AccountUpdate(this);
        public void Delete() => DatabaseManager.AccountDelete(AccountId);
        public static List<BankTransaction> TransactionsRead(int accountId, TransactionStatus status) => DatabaseManager.TransactionsRead(accountId, status);

        object ICloneable.Clone() => Clone();

        public Account Clone()
        {
            var acct = new Account
            {
                AccountId = AccountId,
                Type = Type,
                Status = Status,
                Name = Name,
                Number = Number,
                Institution = Institution,
                StartingBalance = StartingBalance
            };

            return acct;
        }

        #endregion
    }

    public enum AccountStatus
    {
        Open,
        Closed
    }
}
