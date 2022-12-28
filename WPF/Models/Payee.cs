using System;

namespace MoneyPro.Models
{
    public class Payee
    {
        #region Properties

        public int PayeeId { get; set; }
        public string Name { get; set; }
        public DateTime? DateLastUsed { get; set; } = null;

        #endregion

        #region Methods

        public void Insert() => DatabaseManager.PayeeInsert(this);
        public void Update() => DatabaseManager.PayeeUpdate(this);
        public void Delete() => DatabaseManager.PayeeDelete(this);

        #endregion
    }
}
