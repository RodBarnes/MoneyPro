using System;

namespace MoneyPro.Models
{
    public class SearchItem
    {
        public string Reference { get; set; }
        public DateTime? Date { get; set; } = null;
        public string Payee { get; set; } = null;
        public decimal? Amount { get; set; } = null;
        public string TransMemo { get; set; } = null;
        public string SubMemo { get; set; } = null;
        public string Category { get; set; } = null;
        public string Subcategory { get; set; } = null;
        public string Class { get; set; } = null;
        public string Subclass { get; set; } = null;
        public string Account { get; set; } = null;
        public int TransactionId { get; set; }
        public int SubtransactionId { get; set; }
    }
}
