using System;

namespace MoneyPro.Events
{
    public delegate void TransferDeletedEventHandler(object source, TransferDeletedEventArgs e);
    public class TransferDeletedEventArgs : EventArgs
    {
        public new static readonly TransferDeletedEventArgs Empty;

        public string AccountName { get; set; }
        public int TransactionId { get; set; }
        public int SubtransactionId { get; set; }

    }
}
