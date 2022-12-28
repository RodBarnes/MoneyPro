using Common;
using Common.ViewModels;
using MoneyPro.Models;
using System.Collections.ObjectModel;

namespace MoneyPro.ViewModels
{
    public partial class MainVM : BaseVM
    {
        public void InitLookupReplacement()
        {
            ReplacementResponseCommand = new Command(ReplacementResponseAction);
            ReplacementCancelCommand = new Command(ReplacementCancelAction);
        }

        #region Commands

        public Command ReplacementResponseCommand { get; set; }
        private void ReplacementResponseAction(object obj) => DeleteLookup();

        public Command ReplacementCancelCommand { get; set; }
        private void ReplacementCancelAction(object obj) => HideLookupReplacement();

        #endregion

        #region Properties

        private string replacementPanelVisibility = VISIBILITY_COLLAPSE;
        public string LookupReplacementVisibility
        {
            get => replacementPanelVisibility;
            set
            {
                replacementPanelVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string replacementText = "";
        public string ReplacementText
        {
            get => replacementText;
            set
            {
                replacementText = value;
                NotifyPropertyChanged();
            }
        }

        private string isReplacementSelected = LITERAL_FALSE;
        public string IsReplacementSelected
        {
            get => isReplacementSelected;
            set
            {
                isReplacementSelected = value;
                NotifyPropertyChanged();
            }
        }

        private string replacementCategoryVisibility = VISIBILITY_COLLAPSE;
        public string ReplacementCategoryVisibility
        {
            get => replacementCategoryVisibility;
            set
            {
                replacementCategoryVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Category replacementCategory;
        public Category ReplacementCategory
        {
            get => replacementCategory;
            set
            {
                replacementCategory = value;
                NotifyPropertyChanged();
                IsReplacementSelected = (replacementCategory == null) ? LITERAL_FALSE : LITERAL_TRUE;
            }
        }

        private ObservableCollection<Category> replacementCategories;
        public ObservableCollection<Category> ReplacementCategories
        {
            get => replacementCategories;
            set
            {
                replacementCategories = value;
                NotifyPropertyChanged();
            }
        }

        private string replacementSubcategoryVisibility = VISIBILITY_COLLAPSE;
        public string ReplacementSubcategoryVisibility
        {
            get => replacementSubcategoryVisibility;
            set
            {
                replacementSubcategoryVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Subcategory replacementSubcategory;
        public Subcategory ReplacementSubcategory
        {
            get => replacementSubcategory;
            set
            {
                replacementSubcategory = value;
                NotifyPropertyChanged();
                IsReplacementSelected = (replacementCategory == null) ? LITERAL_FALSE : LITERAL_TRUE;
            }
        }

        private ObservableCollection<Subcategory> replacementSubcategories;
        public ObservableCollection<Subcategory> ReplacementSubcategories
        {
            get => replacementSubcategories;
            set
            {
                replacementSubcategories = value;
                NotifyPropertyChanged();
            }
        }

        private string replacementClassVisibility = VISIBILITY_COLLAPSE;
        public string ReplacementClassVisibility
        {
            get => replacementClassVisibility;
            set
            {
                replacementClassVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Class replacementClass;
        public Class ReplacementClass
        {
            get => replacementClass;
            set
            {
                replacementClass = value;
                NotifyPropertyChanged();
                IsReplacementSelected = (replacementCategory == null) ? LITERAL_FALSE : LITERAL_TRUE;
            }
        }

        private ObservableCollection<Class> replacementClasses;
        public ObservableCollection<Class> ReplacementClasses
        {
            get => replacementClasses;
            set
            {
                replacementClasses = value;
                NotifyPropertyChanged();
            }
        }

        private string replacementSubclassVisibility = VISIBILITY_COLLAPSE;
        public string ReplacementSubclassVisibility
        {
            get => replacementSubclassVisibility;
            set
            {
                replacementSubclassVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private Subclass replacementSubclass;
        public Subclass ReplacementSubclass
        {
            get => replacementSubclass;
            set
            {
                replacementSubclass = value;
                NotifyPropertyChanged();
                IsReplacementSelected = (replacementCategory == null) ? LITERAL_FALSE : LITERAL_TRUE;
            }
        }

        private ObservableCollection<Subclass> replacementSubclasses;
        public ObservableCollection<Subclass> ReplacementSubclasses
        {
            get => replacementSubclasses;
            set
            {
                replacementSubclasses = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public void ShowLookupReplacement()
        {
            string itemToBeDeletedText = "";
            string itemToBeDeletedTypeName = "";

            if (MaintCategoryVisibility == VISIBILITY_SHOW)
            {
                itemToBeDeletedText = MaintCategory.Text;
                itemToBeDeletedTypeName = nameof(Category);
                ReplacementCategoryVisibility = VISIBILITY_SHOW;
            }
            else if (MaintSubcategoryVisibility == VISIBILITY_SHOW)
            {
                itemToBeDeletedText = MaintSubcategory.Text;
                itemToBeDeletedTypeName = nameof(Subcategory);
                ReplacementSubcategoryVisibility = VISIBILITY_SHOW;
            }
            else if (MaintClassVisibility == VISIBILITY_SHOW)
            {
                itemToBeDeletedText = MaintClass.Text;
                itemToBeDeletedTypeName = nameof(Class);
                ReplacementClassVisibility = VISIBILITY_SHOW;
            }
            else if (MaintSubclassVisibility == VISIBILITY_SHOW)
            {
                itemToBeDeletedText = MaintSubclass.Text;
                itemToBeDeletedTypeName = nameof(Subclass);
                ReplacementSubclassVisibility = VISIBILITY_SHOW;
            }

            ReplacementText = $"The {itemToBeDeletedTypeName} '{itemToBeDeletedText}' is in use in one or more transactions.  " +
                $"These transactions will be assigned the {itemToBeDeletedTypeName} you select.";
            LookupReplacementVisibility = VISIBILITY_SHOW;
        }

        public void HideLookupReplacement()
        {
            if (ReplacementCategoryVisibility == VISIBILITY_SHOW)
            {
                ReplacementCategories = null;
                ReplacementCategory = null;
            }
            else if (ReplacementSubcategoryVisibility == VISIBILITY_SHOW)
            {
                ReplacementSubcategories = null;
                ReplacementSubcategory = null;
            }
            else if (ReplacementClassVisibility == VISIBILITY_SHOW)
            {
                ReplacementClasses = null;
                ReplacementClass = null;
            }
            else if (ReplacementSubclassVisibility == VISIBILITY_SHOW)
            {
                ReplacementSubclasses = null;
                ReplacementSubclass = null;
            }

            LookupReplacementVisibility = VISIBILITY_COLLAPSE;
            ReplacementCategoryVisibility = ReplacementSubcategoryVisibility = ReplacementClassVisibility = ReplacementSubclassVisibility = VISIBILITY_COLLAPSE;
        }

        #endregion
    }
}
