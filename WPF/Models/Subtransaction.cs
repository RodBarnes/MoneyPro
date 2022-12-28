using System;

namespace MoneyPro.Models
{
    public class Subtransaction : ICloneable
    {
        #region Properties

        public int SubtransactionId { get; set; } = 0;
        public int TransactionId { get; set; } = 0;
        public int XferSubtransactionId { get; set; } = 0;
        public int XferTransactionId { get; set; } = 0;
        public string XferAccount { get; set; } = "";
        public string Memo { get; set; } = "";
        public string Category { get; set; } = "";
        public string Class { get; set; } = "";
        public string Subcategory { get; set; } = "";
        public string Subclass { get; set; } = "";
        public string Budget { get; set; } = "";
        public decimal Amount { get; set; } = 0;

        #endregion

        #region Methods

        public void Insert() => DatabaseManager.SubtransactionInsert(this);
        public void Update() => DatabaseManager.SubtransactionUpdate(this);
        public void Delete() => DatabaseManager.SubtransactionDelete(this);
        object ICloneable.Clone() => Clone();

        public Subtransaction Clone()
        {
            var sub = new Subtransaction
            {
                SubtransactionId = SubtransactionId,
                TransactionId = TransactionId,
                Amount = Amount,
                Memo = Memo,
                Category = Category,
                Subcategory = Subcategory,
                Class = Class,
                Subclass = Subclass,
                Budget = Budget,
                XferSubtransactionId = XferSubtransactionId,
                XferTransactionId = XferTransactionId,
                XferAccount = XferAccount
            };

            return sub;
        }

        #endregion
    }
}
