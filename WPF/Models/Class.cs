namespace MoneyPro.Models
{
    public class Class
    {
        #region Properties

        public int ClassId { get; set; }
        public string Text { get; set; }

        #endregion

        #region Methods

        public void Insert() => DatabaseManager.ClassInsert(this);
        public void Update() => DatabaseManager.ClassUpdate(this);
        public void Delete() => DatabaseManager.ClassDelete(this);

        #endregion

    }
}
