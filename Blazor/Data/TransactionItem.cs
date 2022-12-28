using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class TransactionItem
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public Budget Budget { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }

        public TransactionItem(decimal amount, string note)
        {
            Amount = amount;
            Note = note;
        }
    }
}
