using Common.ViewModels;
using MoneyPro.Models;
using System;

namespace MoneyPro.ViewModels
{
    public class PayeeVM : BaseVM
    {
        private readonly Payee model;

        #region Constructors

        public static PayeeVM ModelToVM(Payee model) => new PayeeVM(model);
        private PayeeVM(Payee model) => this.model = model;
        public PayeeVM() => model = new Payee();

        #endregion

        #region Properties

        public int PayeeId { get => model.PayeeId; set => model.PayeeId = value; }
        public string Name
        {
            get => model.Name;
            set
            {
                model.Name = value;
                if (model.Name != null)
                {
                    DateLastUsed = DateTime.Now;
                }
                NotifyPropertyChanged();
            }
        }
        public DateTime? DateLastUsed
        {
            get => model.DateLastUsed;
            set
            {
                model.DateLastUsed = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Methods
        public void Insert() => model.Insert();
        public void Update() => model.Update();
        public void Delete() => model.Delete();

        #endregion
    }
}
