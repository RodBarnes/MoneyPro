namespace MoneyPro.Models
{
    public class ScheduleSubtransaction
    {

        #region Properties

        public int ScheduleSubtransactionId { get; set; } = 0;
        public int ScheduleTransactionId { get; set; } = 0;
        public string Memo { get; set; } = "";
        public string XferAccount { get; set; } = "";
        public string Category { get; set; } = "";
        public string Class { get; set; } = "";
        public string Subcategory { get; set; } = "";
        public string Subclass { get; set; } = "";
        public string Budget { get; set; } = "";
        public decimal Amount { get; set; } = 0;

        #endregion

        #region Methods

        public void Insert() => DatabaseManager.ScheduleSubtransactionInsert(this);
        public void Update() => DatabaseManager.ScheduleSubtransactionUpdate(this);
        public void Delete() => DatabaseManager.ScheduleSubtransactionDelete(this);

        #endregion
    }
}
