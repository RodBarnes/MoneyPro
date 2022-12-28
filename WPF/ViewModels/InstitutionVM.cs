using Common.ViewModels;
using MoneyPro.Models;

namespace MoneyPro.ViewModels
{
    public class InstitutionVM : BaseVM
    {
        private readonly Institution model;

        public InstitutionVM(string name)
        {
            model = new Institution();
            Name = name;
        }

        public InstitutionVM(Institution inst)
        {
            model = inst;
        }

        #region Properties

        public string Name
        {
            get => model.Name;
            set
            {
                model.Name = value;
                NotifyPropertyChanged();
            }
        }
        public string URL
        {
            get => model.URL;
            set
            {
                model.URL = value;
                NotifyPropertyChanged();
            }
        }
        public string Email
        {
            get => model.Email;
            set
            {
                model.Email = value;
                NotifyPropertyChanged();
            }
        }
        public string Phone
        {
            get => model.Phone;
            set
            {
                model.Phone = value;
                NotifyPropertyChanged();
            }
        }
        public string City
        {
            get => model.City;
            set
            {
                model.City = value;
                NotifyPropertyChanged();
            }
        }
        public string Street
        {
            get => model.Street;
            set
            {
                model.Street = value;
                NotifyPropertyChanged();
            }
        }
        public string State
        {
            get => model.State;
            set
            {
                model.State = value;
                NotifyPropertyChanged();
            }
        }
        public string Zip
        {
            get => model.Zip;
            set
            {
                model.Zip = value;
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
