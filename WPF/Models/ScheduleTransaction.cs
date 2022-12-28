using System;
using System.Collections.Generic;

namespace MoneyPro.Models
{
    public class ScheduleTransaction
    {
        #region Properties

        public int ScheduleTransactionId { get; set; } = 0;
        public string Rule { get; set; } = "";
        public string Account { get; set; } = "";
        public string Payee { get; set; } = "";
        public decimal Amount { get; set; } = 0;
        public string Memo { get; set; } = "";
        public DateTime NextDate { get; set; } = DateTime.Today.AddDays(1);
        public int? CountEnd { get; set; } = null;
        public DateTime? DateEnd { get; set; } = null;
        public int? EnterDaysBefore { get; set; } = null;

        #endregion

        #region Methods

        public void Insert() => DatabaseManager.ScheduleTransactionInsert(this);
        public void Update() => DatabaseManager.ScheduleTransactionUpdate(this);
        public void Delete() => DatabaseManager.ScheduleTransactionDelete(ScheduleTransactionId);
        public static List<ScheduleSubtransaction> ScheduleSubtransactionsRead(int transactionId) => DatabaseManager.ScheduleSubtransactionsRead(transactionId);

        #endregion

    }
}
