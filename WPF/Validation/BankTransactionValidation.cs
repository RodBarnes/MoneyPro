using MoneyPro.ViewModels;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MoneyPro.Validation
{
    public class BankTransactionValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = ValidationResult.ValidResult;

            var bg = (BindingGroup)value;
            var row = (DataGridRow)bg.Owner;
            if (row.Item is BankTransactionVM item)
            {
                switch (ValidationStep)
                {
                    case ValidationStep.RawProposedValue:
                        break;
                    case ValidationStep.ConvertedProposedValue:
                        break;
                    case ValidationStep.UpdatedValue:
                        break;
                    case ValidationStep.CommittedValue:
                        if (item.TransactionId > 0)
                        {
                            item.Update();
                            if (item.Subtransactions.Count == 0)
                            {
                                item.Subtransactions.Add(new SubtransactionVM(item.TransactionId));
                            }
                        }
                        break;
                }
            }

            return result;
        }
    }
}
