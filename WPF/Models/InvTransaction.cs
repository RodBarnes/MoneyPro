using System;

namespace MoneyPro.Models
{
    public class InvTransaction
    {
        #region Properties

        public int AccountId { get; set; }
        public int TransactionId { get; set; }
        public string Memo { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public TransactionStatus Status { get; set; } = TransactionStatus.N;
        public string Action { get; set; } = "";
        public string Security { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Commission { get; set; }
        public decimal TransferAmount { get; set; }

        #endregion
    }
}
