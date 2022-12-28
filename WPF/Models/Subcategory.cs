namespace MoneyPro.Models
{
    public class Subcategory
    {
        #region Properties

        public int SubcategoryId { get; set; }
        public string Text { get; set; }
        public bool Tax { get; set; }

        #endregion

        #region Methods

        public void Insert() => DatabaseManager.SubcategoryInsert(this);
        public void Update() => DatabaseManager.SubcategoryUpdate(this);
        public void Delete() => DatabaseManager.SubcategoryDelete(this);

        #endregion

    }
}
