namespace MoneyPro.Models
{
    public class Subclass
    {
        #region Properties

        public int SubclassId { get; set; }
        public string Text { get; set; }

        #endregion

        #region Methods

        public void Insert() => DatabaseManager.SubclassInsert(this);
        public void Update() => DatabaseManager.SubclassUpdate(this);
        public void Delete() => DatabaseManager.SubclassDelete(this);

        #endregion

    }
}
