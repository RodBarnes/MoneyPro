using MoneyPro.ViewModels;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MoneyPro.Validation
{
    public class ScheduleSubtransactionValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = ValidationResult.ValidResult;

            var bg = (BindingGroup)value;
            var row = (DataGridRow)bg.Owner;
            if (row.Item is ScheduleSubtransactionVM item)
            {
                //System.Diagnostics.Debug.WriteLine($"{nameof(ScheduleSubtransactionValidation)} >> Step:{ValidationStep}, BindingGroup.IsDirty:{row.BindingGroup.IsDirty}");
                switch (ValidationStep)
                {
                    case ValidationStep.RawProposedValue:
                        break;
                    case ValidationStep.ConvertedProposedValue:
                        break;
                    case ValidationStep.UpdatedValue:
                        break;
                    case ValidationStep.CommittedValue:
                        if (item.ScheduleSubtransactionId > 0)
                        {
                            item.Update();
                        }
                        break;
                }
            }

            return result;
        }
    }
}
