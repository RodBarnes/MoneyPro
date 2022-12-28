namespace MoneyPro.Models
{
    public class Institution
    {
        #region Properties

        public int InstitutionId { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        #endregion

        #region Methods

        public void Insert() => DatabaseManager.InstitutionInsert(this);
        public void Update() => DatabaseManager.InstitutionUpdate(this);
        public void Delete() => DatabaseManager.InstitutionDelete(this);

        #endregion
    }
}
