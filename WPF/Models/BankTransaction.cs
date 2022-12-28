using System;
using System.Collections.Generic;

namespace MoneyPro.Models
{
    public class BankTransaction : ICloneable
    {
        #region Properties

        public int TransactionId { get; set; } = 0;
        public int AccountId { get; set; } = 0;
        public string Reference { get; set; } = "";
        public string Payee { get; set; } = "";
        public decimal Amount { get; set; } = 0;
        public string Memo { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now;
        public TransactionStatus Status { get; set; } = TransactionStatus.N;
        public bool Void { get; set; } = false;

        #endregion

        #region Methods

        public void Insert() => DatabaseManager.BankTransactionInsert(this);
        public void Update() => DatabaseManager.BankTransactionUpdate(this);
        public void Delete() => DatabaseManager.BankTransactionDelete(TransactionId);
        public static List<Subtransaction> SubtransactionsRead(int transactionId) => DatabaseManager.SubtransactionsRead(transactionId);
        object ICloneable.Clone() => Clone();

        public BankTransaction Clone()
        {
            var trans = new BankTransaction
            {
                AccountId = AccountId,
                TransactionId = TransactionId,
                Reference = Reference,
                Date = Date,
                Amount = Amount,
                Payee = Payee,
                Status = Status,
                Memo = Memo,
                Void = Void
            };

            return trans;
        }

        #endregion
    }

    public enum TransactionStatus
    {
        N,  // New
        C,  // Cleared
        R  // Reconciled
    }
}
