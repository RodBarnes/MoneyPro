using MoneyPro.ViewModels;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MoneyPro.Validation
{
    public class ScheduleTransactionValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = ValidationResult.ValidResult;

            var bg = (BindingGroup)value;
            var row = (DataGridRow)bg.Owner;
            if (row.Item is ScheduleTransactionVM item)
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
                        if (item.ScheduleTransactionId > 0)
                        {
                            item.Update();
                            if (item.Subtransactions.Count == 0)
                            {
                                item.Subtransactions.Add(new ScheduleSubtransactionVM(item.ScheduleTransactionId));
                            }
                        }
                        break;
                }
            }

            return result;
        }
    }
}
