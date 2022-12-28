namespace MoneyPro.Models
{
    public class Category
    {

        #region Properties

        public int CategoryId { get; set; }
        public string Text { get; set; }
        public bool Tax { get; set; }

        #endregion

        #region Methods

        public void Insert() => DatabaseManager.CategoryInsert(this);
        public void Update() => DatabaseManager.CategoryUpdate(this);
        public void Delete() => DatabaseManager.CategoryDelete(this);

        #endregion

    }
}
