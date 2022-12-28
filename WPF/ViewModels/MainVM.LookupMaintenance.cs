using Common;
using Common.UserControls;
using Common.ViewModels;
using MoneyPro.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MoneyPro.ViewModels
{
	public partial class MainVM : BaseVM
	{
		private void InitLookupMaintenance() 
		{
			ShowMaintenanceCommand = new Command(ShowMaintenanceAction);
			HideMaintenanceCommand = new Command(HideMaintenanceAction);
			DeleteCategoryCommand = new Command(DeleteCategoryAction);
			DeleteSubcategoryCommand = new Command(DeleteSubcategoryAction);
			DeleteClassCommand = new Command(DeleteClassAction);
			DeleteSubclassCommand = new Command(DeleteSubclassAction);
			DeletePayeeCommand = new Command(DeletePayeeAction);
			DeleteInstitutionCommand = new Command(DeleteInstitutionAction);

			PopulateSelector();
		}

		#region Commands

		public Command ShowMaintenanceCommand { get; set; }
		private void ShowMaintenanceAction(object obj) => ShowLookupMaintenance();

		public Command HideMaintenanceCommand { get; set; }
		private void HideMaintenanceAction(object obj) => HideLookupMaintenance();

		public Command DeleteCategoryCommand { get; set; }
		private void DeleteCategoryAction(object obj) => ManageCategory();

		public Command DeleteSubcategoryCommand { get; set; }
		private void DeleteSubcategoryAction(object obj) => ManageSubcategory();

		public Command DeleteClassCommand { get; set; }
		private void DeleteClassAction(object obj) => ManageClass();

		public Command DeleteSubclassCommand { get; set; }
		private void DeleteSubclassAction(object obj) => ManageSubclass();

		public Command DeletePayeeCommand { get; set; }
		private void DeletePayeeAction(object obj) => DeleteLookup();

		public Command DeleteInstitutionCommand { get; set; }
		private void DeleteInstitutionAction(object obj) => DeleteLookup();

		#endregion

		#region Properties

		private string maintenancePanelVisibility = VISIBILITY_HIDE;
		public string LookupMaintenanceVisibility
		{
			get => maintenancePanelVisibility;
			set
			{
				maintenancePanelVisibility = value;
				NotifyPropertyChanged();
			}
		}

		private ObservableCollection<SelectionItem> maintSelections = new ObservableCollection<SelectionItem>();
		public ObservableCollection<SelectionItem> MaintSelections
		{
			get => maintSelections;
			set
			{
				maintSelections = value;
				NotifyPropertyChanged();
			}
		}

		private SelectionItem selectedMaintItem = null;
		public SelectionItem SelectedMaintItem
		{
			get => selectedMaintItem;
			set
			{
				selectedMaintItem = value;
				if (selectedMaintItem != null)
				{
					DisplayData(selectedMaintItem);
				}
				NotifyPropertyChanged();
			}
		}

		private string maintCategoryVisibility = VISIBILITY_COLLAPSE;
		public string MaintCategoryVisibility
		{
			get => maintCategoryVisibility;
			set
			{
				maintCategoryVisibility = value;
				NotifyPropertyChanged();
			}
		}

		private Category maintCategory;
		public Category MaintCategory
		{
			get => maintCategory;
			set
			{
				maintCategory = value;
				NotifyPropertyChanged();
			}
		}

		private string maintSubcategoryVisibility = VISIBILITY_COLLAPSE;
		public string MaintSubcategoryVisibility
		{
			get => maintSubcategoryVisibility;
			set
			{
				maintSubcategoryVisibility = value;
				NotifyPropertyChanged();
			}
		}

		private Subcategory maintSubcategory;
		public Subcategory MaintSubcategory
		{
			get => maintSubcategory;
			set
			{
				maintSubcategory = value;
				NotifyPropertyChanged();
			}
		}

		private string maintClassVisibility = VISIBILITY_COLLAPSE;
		public string MaintClassVisibility
		{
			get => maintClassVisibility;
			set
			{
				maintClassVisibility = value;
				NotifyPropertyChanged();
			}
		}

		private Class maintClass;
		public Class MaintClass
		{
			get => maintClass;
			set
			{
				maintClass = value;
				NotifyPropertyChanged();
			}
		}

		private string maintSubclassVisibility = VISIBILITY_COLLAPSE;
		public string MaintSubclassVisibility
		{
			get => maintSubclassVisibility;
			set
			{
				maintSubclassVisibility = value;
				NotifyPropertyChanged();
			}
		}

		private Subclass maintSubclass;
		public Subclass MaintSubclass
		{
			get => maintSubclass;
			set
			{
				maintSubclass = value;
				NotifyPropertyChanged();
			}
		}

		private string maintPayeeVisibility = VISIBILITY_COLLAPSE;
		public string MaintPayeeVisibility
		{
			get => maintPayeeVisibility;
			set
			{
				maintPayeeVisibility = value;
				NotifyPropertyChanged();
			}
		}

		private PayeeVM maintPayee;
		public PayeeVM MaintPayee
		{
			get => maintPayee;
			set
			{
				maintPayee = value;
				NotifyPropertyChanged();
			}
		}

		private string maintInstitutionVisibility = VISIBILITY_COLLAPSE;
		public string MaintInstitutionVisibility
		{
			get => maintInstitutionVisibility;
			set
			{
				maintInstitutionVisibility = value;
				NotifyPropertyChanged();
			}
		}

		private InstitutionVM maintInstitution;
		public InstitutionVM MaintInstitution
		{
			get => maintInstitution;
			set
			{
				maintInstitution = value;
				NotifyPropertyChanged();
			}
		}
		#endregion

		#region Window Methods

		private void ShowLookupMaintenance()
		{
			LookupMaintenanceVisibility = VISIBILITY_SHOW;
		}

		private void HideLookupMaintenance()
		{
			LookupMaintenanceVisibility = VISIBILITY_COLLAPSE;
			SelectedMaintItem = null;
			MaintCategoryVisibility = MaintSubcategoryVisibility = MaintClassVisibility = MaintSubclassVisibility = MaintPayeeVisibility = MaintInstitutionVisibility = VISIBILITY_COLLAPSE;
		}

		private void PopulateSelector()
		{
			// Populate the lookup selector
			MaintSelections.Add(new SelectionItem("Category"));
			MaintSelections.Add(new SelectionItem("Subcategory"));
			MaintSelections.Add(new SelectionItem("Class"));
			MaintSelections.Add(new SelectionItem("Subclass"));
			MaintSelections.Add(new SelectionItem("Payee"));
			MaintSelections.Add(new SelectionItem("Institution"));
		}

		private void DisplayData(SelectionItem item)
		{
			MaintCategoryVisibility = MaintSubcategoryVisibility = MaintClassVisibility = MaintSubclassVisibility = MaintPayeeVisibility = MaintInstitutionVisibility = VISIBILITY_COLLAPSE;
			switch (item.Name)
			{
				case nameof(Category):
					MaintCategoryVisibility = VISIBILITY_SHOW;
					break;
				case nameof(Subcategory):
					MaintSubcategoryVisibility = VISIBILITY_SHOW;
					break;
				case nameof(Class):
					MaintClassVisibility = VISIBILITY_SHOW;
					break;
				case nameof(Subclass):
					MaintSubclassVisibility = VISIBILITY_SHOW;
					break;
				case nameof(Payee):
					MaintPayeeVisibility = VISIBILITY_SHOW;
					break;
				case nameof(Institution):
					MaintInstitutionVisibility = VISIBILITY_SHOW;
					break;
				default:
					break;
			}
		}

        #endregion

        #region Operation Methods

        private void ManageCategory()
		{
			try
			{
				if (IsCategoryUsed(MaintCategory))
				{
					ReplacementCategories = SubtransactionVM.Categories.Clone();
					ReplacementCategories.Remove(MaintCategory);
					ReplacementCategory = null;

					ShowLookupReplacement();
				}
				else
				{
					DeleteLookup();
				}
			}
			catch (Exception ex)
			{
				window.MessagePanel.Show("Exception", Utility.ParseException(ex));
			}
		}

		private void ManageSubcategory()
		{
			try
			{
				if (IsSubcategoryUsed(MaintSubcategory))
				{
					ReplacementSubcategories = SubtransactionVM.Subcategories.Clone();
					ReplacementSubcategories.Remove(MaintSubcategory);
					ReplacementSubcategory = null;

					ShowLookupReplacement();
				}
				else
				{
					DeleteLookup();
				}
			}
			catch (Exception ex)
			{
				window.MessagePanel.Show("Exception", Utility.ParseException(ex));
			}
		}

		private void ManageClass()
		{
			try
			{
				if (IsClassUsed(MaintClass))
				{
					ReplacementClasses = SubtransactionVM.Classes.Clone();
					ReplacementClasses.Remove(MaintClass);
					ReplacementClass = null;

					ShowLookupReplacement();
				}
				else
				{
					DeleteLookup();
				}
			}
			catch (Exception ex)
			{
				window.MessagePanel.Show("Exception", Utility.ParseException(ex));
			}
		}

		private void ManageSubclass()
		{
			try
			{
				if (IsSubclassUsed(MaintSubclass))
				{
					ReplacementSubclasses = SubtransactionVM.Subclasses.Clone();
					ReplacementSubclasses.Remove(MaintSubclass);
					ReplacementSubclass = null;

					ShowLookupReplacement();
				}
				else
				{
					DeleteLookup();
				}
			}
			catch (Exception ex)
			{
				window.MessagePanel.Show("Exception", Utility.ParseException(ex));
			}
		}

        private void DeleteLookup()
		{
			try
			{
				if (MaintCategoryVisibility == VISIBILITY_SHOW)
				{
					
					CategoryReplace(MaintCategory, ReplacementCategory);
					ReplacementCategory = null;
					HideLookupReplacement();
				}
				else if (MaintSubcategoryVisibility == VISIBILITY_SHOW)
				{
					SubcategoryReplace(MaintSubcategory, ReplacementSubcategory);
					ReplacementSubcategory = null;
					HideLookupReplacement();
				}
				else if (MaintClassVisibility == VISIBILITY_SHOW)
				{
					ClassReplace(MaintClass, ReplacementClass);
					ReplacementClass = null;
					HideLookupReplacement();
				}
				else if (MaintSubclassVisibility == VISIBILITY_SHOW)
				{
					SubclassReplace(MaintSubclass, ReplacementSubclass);
					ReplacementSubclass = null;
					HideLookupReplacement();
				}
				else if (MaintPayeeVisibility == VISIBILITY_SHOW)
				{
					currentMessageAction = MessageAction.DeletePayee;
					window.MessagePanel.Show(
						"Delete Payee",
						$"The Payee '{MaintPayee.Name}' will be deleted from the selection list.  " +
						$"This payee will be removed from all transactions and will be unavailable for future selection.",
						 MessagePanel.MessageType.OkCancel);
				}
				else if (MaintInstitutionVisibility == VISIBILITY_SHOW)
				{
					currentMessageAction = MessageAction.DeleteInstitution;
					window.MessagePanel.Show(
						"Delete Institution",
						$"The Institution '{MaintInstitution.Name}' will be deleted from the selection list.  " +
						$"This institution will be removed from all accounts and will be unavailable for future selection.",
						 MessagePanel.MessageType.OkCancel);
				}
			}
			catch (Exception ex)
			{
				window.MessagePanel.Show("Exception", Utility.ParseException(ex));
			}
		}

		private bool IsCategoryUsed(Category item)
		{
			foreach (var account in AccountList)
				foreach (var trans in account.Transactions)
					foreach (var sub in trans.Subtransactions)
						if (sub.Category == item.Text)
						{
							return true;
						}
			return false;
		}

		private bool IsSubcategoryUsed(Subcategory item)
		{
			foreach (var account in AccountList)
				foreach (var trans in account.Transactions)
					foreach (var sub in trans.Subtransactions)
						if (sub.Subcategory == item.Text)
						{
							return true;
						}
			return false;
		}

		private bool IsClassUsed(Class item)
		{
			foreach (var account in AccountList)
				foreach (var trans in account.Transactions)
					foreach (var sub in trans.Subtransactions)
						if (sub.Class == item.Text)
						{
							return true;
						}
			return false;
		}

		private bool IsSubclassUsed(Subclass item)
		{
			foreach (var account in AccountList)
				foreach (var trans in account.Transactions)
					foreach (var sub in trans.Subtransactions)
						if (sub.Subclass == item.Text)
						{
							return true;
						}
			return false;
		}

		private void CategoryReplace(Category delItem, Category repItem)
		{
			foreach (var account in AccountList)
				foreach (var trans in account.Transactions)
					foreach (var sub in trans.Subtransactions)
						if (sub.Category == delItem.Text)
						{
							sub.Category = repItem.Text;
							sub.Update();
						}
			SubtransactionVM.Categories.Remove(delItem);
		}

		private void SubcategoryReplace(Subcategory delItem, Subcategory repItem)
		{
			foreach (var account in AccountList)
				foreach (var trans in account.Transactions)
					foreach (var sub in trans.Subtransactions)
						if (sub.Subcategory == delItem.Text)
						{
							sub.Subcategory = repItem.Text;
							sub.Update();
						}
			SubtransactionVM.Subcategories.Remove(delItem);
		}

		private void ClassReplace(Class delItem, Class repItem)
		{
			foreach (var account in AccountList)
				foreach (var trans in account.Transactions)
					foreach (var sub in trans.Subtransactions)
						if (sub.Class == delItem.Text)
						{
							sub.Class = repItem.Text;
							sub.Update();
						}
			SubtransactionVM.Classes.Remove(delItem);
		}

		private void SubclassReplace(Subclass delItem, Subclass repItem)
		{
			foreach (var account in AccountList)
				foreach (var trans in account.Transactions)
					foreach (var sub in trans.Subtransactions)
						if (sub.Subclass == delItem.Text)
						{
							sub.Subclass = repItem.Text;
							sub.Update();
						}
			SubtransactionVM.Subclasses.Remove(delItem);
		}

		#endregion
	}

	public class SelectionItem
	{
		public SelectionItem(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}
