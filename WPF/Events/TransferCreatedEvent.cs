using MoneyPro.ViewModels;
using System;

namespace MoneyPro.Events
{
    public delegate void TransferCreatedEventHandler(object source, TransferCreatedEventArgs e);
    public class TransferCreatedEventArgs : EventArgs
    {
        public new static readonly TransferCreatedEventArgs Empty;

        public string XferAccountName { get; set; }
        public BankTransactionVM Transaction { get; set; }
        public SubtransactionVM Subtransaction { get; set; }
    }
}
