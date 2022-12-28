using System.Collections.Generic;

namespace MoneyPro.Models
{
    public class InvTransactionImport
    {
        public InvTransaction Model { get; private set; } = new InvTransaction();
        public List<Subtransaction> Subtransactions { get; set; } = new List<Subtransaction>();
    }
}
