using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class ScheduledTransaction
    {
        public int Id { get; set; }
        public ScheduledType Type { get; set; }
        public Account Account { get; set; }
        public Payee Payee { get; set; }
        public string PayeeName { get; set; }
        public string Reference { get; set; }
        public IList<TransactionItem> Items { get; set; }
    }
}
