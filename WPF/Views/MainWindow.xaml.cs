using Common;
using MoneyPro.Models;
using MoneyPro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace MoneyPro.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainVM vm;
        private static AccountVM oldAccount;
        private static BankTransactionVM oldBankTransaction;
        private static readonly List<BankTransactionVM> newBankTransactions = new List<BankTransactionVM>();
        private static readonly List<SubtransactionVM> newSubtransactions = new List<SubtransactionVM>();
        private static bool isCutting;
        private static bool isEditingTransaction = false;
        private static bool isEditingSubtransaction = false;
        private static bool isEditingSchedule = false;
        private static bool isEditionSubschedule = false;
        private static bool isBankGridContextMenuInitialized = false;
        private static MenuItem menuReconciled;
        private static MenuItem menuCleared;
        private static MenuItem menuUnreconciled;
        private static MenuItem menuVoid;
        private static MenuItem menuRecur;
        private static Separator sepRecur;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm = new MainVM(this);
            BankDataGrid.ItemContainerGenerator.StatusChanged += BankDataGrid_ItemContainerGenerator_StatusChanged;

            MarkTransactionsCommand = new Command(MarkTransactionsAction);
            VoidTransactionsCommand = new Command(VoidTransactionsAction);
        }

        #region Commands

        public Command MarkTransactionsCommand { get; set; }
        private void MarkTransactionsAction(object obj) => MarkTransactions((string)obj);

        public Command VoidTransactionsCommand { get; set; }
        private void VoidTransactionsAction(object obj) => VoidTransactions();

        #endregion

        #region Methods

        private void VoidTransactions()
        {
            foreach (BankTransactionVM trans in BankDataGrid.SelectedItems)
            {
                trans.Void = !trans.Void;
                menuVoid.IsChecked = trans.Void;
            }
        }

        private void MarkTransactions(string status)
        {
            var valid = Enum.TryParse(status, out TransactionStatus statusVal);
            if (valid)
            {
                foreach (BankTransactionVM trans in BankDataGrid.SelectedItems)
                {
                    trans.Status = statusVal;
                    switch (trans.Status)
                    {
                        case TransactionStatus.N:
                            menuUnreconciled.IsChecked = true;
                            menuCleared.IsChecked = false;
                            menuReconciled.IsChecked = false;
                            break;
                        case TransactionStatus.C:
                            menuUnreconciled.IsChecked = false;
                            menuCleared.IsChecked = true;
                            menuReconciled.IsChecked = false;
                            break;
                        case TransactionStatus.R:
                            menuUnreconciled.IsChecked = false;
                            menuCleared.IsChecked = false;
                            menuReconciled.IsChecked = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static ScrollBar GetScrollbar(DependencyObject dep, Orientation orientation)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++)
            {
                var child = VisualTreeHelper.GetChild(dep, i);
                if (child is ScrollBar bar && bar.Orientation == orientation)
                    return bar;
                else
                {
                    ScrollBar scrollBar = GetScrollbar(child, orientation);
                    if (scrollBar != null)
                        return scrollBar;
                }
            }
            return null;
        }

        #endregion

        #region Main Event Handlers

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // This handler exists to implement use of the WindowClosingBehavior class.
        }

        #endregion

        #region BankDataGrid Context Menu Event Handlers

        /* All standard commands are listed here:
        * https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.applicationcommands?view=netframework-4.8
        * Each of these command will do whatever the target action does by default unless an event
        * handler is implemented to do otherwise.  The custom event handler should take the action and
        * then finish by setting e.Handled = true to inform the target that it doesn't need to do anything.
        */

        private void BankDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var dg = (DataGrid)sender;
            if (!isBankGridContextMenuInitialized)
            {
                sepRecur = new Separator();
                menuRecur = new MenuItem
                {
                    Header = "Make recurring...",
                    Command = vm.ShowNewScheduleCommand
                };

                var menuMark = new MenuItem
                {
                    Header = "Mark as",
                };
                menuUnreconciled = new MenuItem
                {
                    Header = "Unreconciled",
                    Command = MarkTransactionsCommand,
                    CommandParameter = "N",
                    IsCheckable = true
                };
                menuCleared = new MenuItem
                {
                    Header = "Cleared",
                    Command = MarkTransactionsCommand,
                    CommandParameter = "C",
                    IsCheckable = true
                };
                menuReconciled = new MenuItem
                {
                    Header = "Reconciled",
                    Command = MarkTransactionsCommand,
                    CommandParameter = "R",
                    IsCheckable = true
                };
                menuVoid = new MenuItem
                {
                    Header = "Void",
                    Command = VoidTransactionsCommand,
                    CommandParameter = "V",
                    IsCheckable = true
                };

                menuMark.Items.Add(menuUnreconciled);
                menuMark.Items.Add(menuCleared);
                menuMark.Items.Add(menuReconciled);
                menuMark.Items.Add(new Separator());
                menuMark.Items.Add(menuVoid);
                dg.ContextMenu.Items.Add(menuMark);

                isBankGridContextMenuInitialized = true;
            }

            if (dg.SelectedItems.Count == 1)
            {
                if (!dg.ContextMenu.Items.Contains(menuRecur))
                {
                    dg.ContextMenu.Items.Add(sepRecur);
                    dg.ContextMenu.Items.Add(menuRecur);
                }
                menuRecur.CommandParameter = BankDataGrid.SelectedItem;
            }
            else
            {
                if (dg.ContextMenu.Items.Contains(menuRecur))
                {
                    dg.ContextMenu.Items.Remove(sepRecur);
                    dg.ContextMenu.Items.Remove(menuRecur);
                }
            }

            var trans = (BankTransactionVM)dg.SelectedItem;
            menuVoid.IsChecked = trans.Void;
            switch (trans.Status)
            {
                case TransactionStatus.N:
                    menuUnreconciled.IsChecked = true;
                    menuCleared.IsChecked = false;
                    menuReconciled.IsChecked = false;
                    break;
                case TransactionStatus.C:
                    menuUnreconciled.IsChecked = false;
                    menuCleared.IsChecked = true;
                    menuReconciled.IsChecked = false;
                    break;
                case TransactionStatus.R:
                    menuUnreconciled.IsChecked = false;
                    menuCleared.IsChecked = false;
                    menuReconciled.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private void BankDataGrid_CanCut(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = (BankDataGrid.CurrentItem is BankTransactionVM);
                e.Handled = true;
            }

        private void BankDataGrid_Cut(object sender, ExecutedRoutedEventArgs e)
        {
            oldAccount = vm.SelectedAccount;
            var dg = (DataGrid)sender;
            foreach (BankTransactionVM trans in dg.SelectedItems)
            {
                if (trans.Subtransactions.Count == 0)
                {
                    trans.SubtransactionsRead();
                }
                newBankTransactions.Add(trans);
            }
            isCutting = true;
        }

        private void BankDataGrid_CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (BankDataGrid.CurrentItem is BankTransactionVM);
            e.Handled = true;
        }

        private void BankDataGrid_Copy(object sender, ExecutedRoutedEventArgs e)
        {
            var dg = (DataGrid)sender;
            foreach (BankTransactionVM trans in dg.SelectedItems)
            {
                if (trans.Subtransactions.Count == 0)
                {
                    trans.SubtransactionsRead();
                }
                newBankTransactions.Add(trans.Clone());
            }
            isCutting = false;
        }

        private void BankDataGrid_CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            // Only allow paste if to a new line (PlaceHolder not existing transaction)
            e.CanExecute = !(BankDataGrid.CurrentItem is BankTransactionVM);
            e.Handled = true;
        }

        private void BankDataGrid_Paste(object sender, ExecutedRoutedEventArgs e)
        {
            if (!isCutting)
            {
                vm.SelectedAccount.TransactionsAddSubtransactionsInsert(vm.SelectedAccount.AccountId, newBankTransactions);
            }
            else if (oldAccount.AccountId == vm.SelectedAccount.AccountId)
            {
                MessagePanel.Show("Pointless Action", "Cut and paste to the same account isn't supported.");
            }
            else
            {
                foreach (var trans in newBankTransactions)
                {
                    oldAccount.Transactions.Remove(trans);
                }
                vm.SelectedAccount.TransactionsAddSubtransactionsInsert(vm.SelectedAccount.AccountId, newBankTransactions);
            }

            newBankTransactions.Clear();
            oldAccount = null;
        }

        private void BankDataGrid_CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            // Only allow delete if it is an actual transaction and not just a placeholder
            e.CanExecute = (BankDataGrid.CurrentItem is BankTransactionVM);
            e.Handled = true;
        }

        private void BankDataGrid_Delete(object sender, ExecutedRoutedEventArgs e)
        {
            var dg = (DataGrid)sender;
            var list = new List<BankTransactionVM>();
            foreach (BankTransactionVM trans in dg.SelectedItems)
            {
                list.Add(trans);
            }
            vm.ConfirmTransactionsDelete(list);
            e.Handled = true;
        }

        #endregion

        #region BankDatagrid Event Handlers

        /* DataGrid docs
         * https://docs.microsoft.com/en-us/dotnet/framework/wpf/controls/datagrid
         */

        private void BankDataGrid_ToggleSubs(object sender, RoutedEventArgs e)
        {
            if (BankDataGrid.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed)
            {
                foreach (var item in BankDataGrid.SelectedItems)
                {
                    if (item is BankTransactionVM trans)
                    {
                        BankDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                        if (trans.Subtransactions.Count == 0)
                        {
                            trans.SubtransactionsRead();
                        }
                    }
                }
                BankDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            }
            else
            {
                BankDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }

        private void BankDataGrid_ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (!isEditingTransaction && !isEditingSubtransaction)
            {
                // If the selected row is a valid transaction item, move focus there
                var gen = (ItemContainerGenerator)sender;
                if (BankDataGrid.SelectedItem is BankTransactionVM item)
                {
                    // Determine if it is already in view
                    var verticalScrollBar = GetScrollbar(BankDataGrid, Orientation.Vertical);
                    var cnt = BankDataGrid.Items.Count;
                    var rowFirst = verticalScrollBar.Value;
                    var rowLast = rowFirst + cnt - verticalScrollBar.Maximum;
                    var idx = BankDataGrid.SelectedIndex;
                    if (rowFirst > idx || idx > rowLast)
                    {
                        // Determine if the row is visible
                        var row = gen.ContainerFromIndex(idx);
                        if (row == null)
                        {

                            // Scroll it into view and close the detail
                            BankDataGrid.ScrollIntoView(BankDataGrid.SelectedItem);
                            BankDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                        }
                    }

                    // Move focus to the row
                    if (gen.ContainerFromItem(item) is DataGridRow dgRow)
                    {
                        if (gen.Status == GeneratorStatus.ContainersGenerated)
                        {
                            dgRow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        }
                    }
                }
            }
        }

        private void BankDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.IsNewItem && e.Row.Item is BankTransactionVM trans)
            {
                trans.AccountId = vm.SelectedAccount.AccountId;
            }
        }

        private void BankDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            isEditingTransaction = true;
        }

        private void BankDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            // This is necessary to put the row back into edit mode after
            // tabbing across a read-only cell
            if (isEditingTransaction)
            {
                var dg = (DataGrid)sender;
                var mainItem = dg.CurrentItem;
                var col = dg.CurrentColumn;
                if (!col.IsReadOnly)
                {
                    // Enable editing for the new cell
                    dg.CurrentCell = new DataGridCellInfo(mainItem, col);
                    dg.BeginEdit();
                }
            }
        }

        private void BankDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            isEditingTransaction = false;
        }

        #endregion

        #region SubDataGrid Context Menu Event Handlers

        private void SubDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var dg = (DataGrid)sender;
            if (dg.SelectedItem is SubtransactionVM sub)
            {
                if (sub != null && sub.IsTransfer)
                {
                    // Add Go To Account option
                    var account = vm.AccountList.First(a => a.Name == sub.XferAccount);
                    var menuItem = new MenuItem
                    {
                        Header = $"Go to Account: {account.Name}",
                        Command = vm.XferGoToAccountCommand
                    };
                    dg.ContextMenu.Items.Add(new Separator());
                    dg.ContextMenu.Items.Add(menuItem);
                }
                else if (dg.ContextMenu.Items.Count > 4)
                {
                    // Remove the separator and Go To Account option
                    dg.ContextMenu.Items.RemoveAt(5);
                    dg.ContextMenu.Items.RemoveAt(4);
                }
            }
        }

        private void SubDataGrid_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            var dg = (DataGrid)sender;
            if (dg.ContextMenu.Items.Count > 4)
            {
                // Remove the separator and Go To Account option
                dg.ContextMenu.Items.RemoveAt(5);
                dg.ContextMenu.Items.RemoveAt(4);
            }
        }

        private void SubDataGrid_CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            var trans = (BankTransactionVM)BankDataGrid.CurrentItem;
            var dg = (DataGrid)sender;
            e.CanExecute = (dg.CurrentItem is SubtransactionVM && trans.Subtransactions.Count > 1);
            e.Handled = true;
        }

        private void SubDataGrid_Cut(object sender, ExecutedRoutedEventArgs e)
        {
            var dg = (DataGrid)sender;
            oldBankTransaction = (BankTransactionVM)BankDataGrid.CurrentItem;
            foreach (SubtransactionVM sub in dg.SelectedItems)
            {
                newSubtransactions.Add(sub);
            }
            isCutting = true;
        }

        private void SubDataGrid_CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            var dg = (DataGrid)sender;
            e.CanExecute = (dg.CurrentItem is SubtransactionVM);
            e.Handled = true;
        }

        private void SubDataGrid_Copy(object sender, ExecutedRoutedEventArgs e)
        {
			var dg = (DataGrid)sender;
            foreach (SubtransactionVM sub in dg.SelectedItems)
            {
                newSubtransactions.Add(sub.Clone());
            }
            isCutting = false;
        }

        private void SubDataGrid_CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            var dg = (DataGrid)sender;
            // Only allow paste to a new line (PlaceHolder not existing subtransaction)
            e.CanExecute = !(dg.CurrentItem is Subtransaction);
            e.Handled = true;
        }

        private void SubDataGrid_Paste(object sender, ExecutedRoutedEventArgs e)
        {
            var trans = (BankTransactionVM)BankDataGrid.CurrentItem;
            if (!isCutting)
            {
                foreach (var sub in newSubtransactions)
                {
                    sub.TransactionId = trans.TransactionId;
                    trans.Subtransactions.Add(sub);
                }
            }
            else if (oldBankTransaction.TransactionId == trans.TransactionId)
            {
                MessagePanel.Show("Pointless Action", "Cut and paste to the same transaction isn't supported.");
            }
            else
            {
                foreach (var sub in newSubtransactions)
                {
                    oldBankTransaction.Subtransactions.Remove(sub);
                    trans.Subtransactions.Add(sub);
                }
            }

            newSubtransactions.Clear();
            oldBankTransaction = null;
        }

        private void SubDataGrid_CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            // TODO: What about deleting from a Cleared or Resolved BankTransaction?

            var trans = (BankTransactionVM)BankDataGrid.CurrentItem;
            var dg = (DataGrid)sender;
            e.CanExecute = (dg.CurrentItem is SubtransactionVM && trans.Subtransactions.Count > 1);
            e.Handled = true;
        }

        private void SubDataGrid_Delete(object sender, ExecutedRoutedEventArgs e)
        {
            var trans = (BankTransactionVM)BankDataGrid.CurrentItem;
            var dg = (DataGrid)sender;
            var list = new List<SubtransactionVM>();
            foreach (SubtransactionVM sub in dg.SelectedItems)
            {
                list.Add(sub);
            }
            vm.ConfirmSubtransactionsDelete(trans, list);
            e.Handled = true;
        }

        #endregion

        #region SubDataGrid Event Handlers

        private void SubDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            isEditingSubtransaction = true;
        }

        private void SubDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (isEditingSubtransaction)
            {
                var dg = (DataGrid)sender;
                var mainItem = dg.CurrentItem;
                var col = dg.CurrentColumn;
                if (!col.IsReadOnly)
                {
                    // Enable editing for the new cell
                    dg.CurrentCell = new DataGridCellInfo(mainItem, col);
                    dg.BeginEdit();
                }
            }
        }

        private void SubDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            isEditingSubtransaction = false;
        }

        #endregion

        #region SearchDataGrid Context Menu Event Handlers

        private void SearchDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var dg = (DataGrid)sender;
            var searchItem = (SearchItem)dg.SelectedItem;
            if (searchItem != null)
            {
                // Check for and add context menu as needed
                if (dg.ContextMenu == null)
                    SearchContextMenuCreate(dg, searchItem.Account);

                // Update the menu entry with the account name
                ((MenuItem)dg.ContextMenu.Items[0]).Header = $"Go to Account: {searchItem.Account}";
            }
        }

        private static void SearchContextMenuCreate(DataGrid dg, string text = null)
        {
            var title = "";
            if (!string.IsNullOrEmpty(text))
            {
                title = $"Go to Account: {text}";
            }
            // Add the context menu with the single Go To Account option
            var menuItem = new MenuItem()
            {
                Command = vm.SearchGoToAccountCommand,
                Header = title,
            };
            var context = new ContextMenu();
            context.Items.Add(menuItem);
            dg.ContextMenu = context;
        }

        #endregion

        #region SearchDataGrid Event Handlers

        private void SearchDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var dg = (DataGrid)sender;

            // Add empty context menu entry for rows that are on screen
            // Without this step, the initial right-click doesn't do anything
            if (dg.ContextMenu == null)
                SearchContextMenuCreate(dg);
        }

        private void SearchDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Execute VM command JumpToAccount
            var dg = SearchDataGrid;
            if (dg.CurrentItem is SearchItem item)
            {
                vm.SearchTransactionVisibility = "Hidden";
                vm.JumpToTransaction(item.Account, item.TransactionId);
            }
        }

        private void SearchClear_Click(object sender, RoutedEventArgs e)
        {
            // Command="{Binding ClearSearchCommand}"
            // Need to clear the context menu, then execute the VM command
            SearchDataGrid.ContextMenu = null;
            vm.ResetSearchItems();
        }

        #endregion

        #region SchedDataGrid Event Handlers

        private void SchedDataGrid_ToggleSubs(object sender, RoutedEventArgs e)
        {
            if (SchedDataGrid.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed)
            {
                foreach (var item in SchedDataGrid.SelectedItems)
                {
                    if (item is ScheduleTransactionVM trans)
                    {
                        SchedDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                        if (trans.Subtransactions.Count == 0)
                        {
                            trans.SubtransactionsRead();
                        }
                    }
                }
                SchedDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            }
            else
            {
                SchedDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }

        private void SchedDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            isEditingSchedule = true;
        }

        private void SchedDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            // This is necessary to put the row back into edit mode after
            // tabbing across a read-only cell
            if (isEditingSchedule)
            {
                var dg = (DataGrid)sender;
                var mainItem = dg.CurrentItem;
                var col = dg.CurrentColumn;
                if (!col.IsReadOnly)
                {
                    // Enable editing for the new cell
                    dg.CurrentCell = new DataGridCellInfo(mainItem, col);
                    dg.BeginEdit();
                }
            }
        }

        private void SchedDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            isEditingSchedule = false;
        }

        #endregion

        #region SchedSubDataGrid Events

        private void SubSchedDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            isEditionSubschedule = true;
        }

        private void SubSchedDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (isEditionSubschedule)
            {
                var dg = (DataGrid)sender;
                var mainItem = dg.CurrentItem;
                var col = dg.CurrentColumn;
                if (!col.IsReadOnly)
                {
                    // Enable editing for the new cell
                    dg.CurrentCell = new DataGridCellInfo(mainItem, col);
                    dg.BeginEdit();
                }
            }
        }

        private void SubSchedDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            isEditionSubschedule = false;
        }

        #endregion
    }
}
