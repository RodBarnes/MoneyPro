using System.Collections.Generic;

namespace MoneyPro.Models
{
    public class BankTransactionImport
    {
        public BankTransaction Model { get; set; } = new BankTransaction();
        public List<Subtransaction> Subtransactions { get; set; } = new List<Subtransaction>();
    }
}
