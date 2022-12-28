using MoneyPro.ViewModels;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MoneyPro.Validation
{
    public class SubtransactionValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = ValidationResult.ValidResult;

            var bg = (BindingGroup)value;
            var row = (DataGridRow)bg.Owner;
            if (row.Item is SubtransactionVM item)
            {
                switch (ValidationStep)
                {
                    case ValidationStep.RawProposedValue:
                        break;
                    case ValidationStep.ConvertedProposedValue:
                        break;
                    case ValidationStep.UpdatedValue:
                        // Enforce transfer rules
                        if (item.Category == "Transfer" && string.IsNullOrEmpty(item.XferAccount))
                        {
                            var msg = "Transfers requires selecting an XferAccount.";
                            item.ValidationResult = msg;
                            return new ValidationResult(false, msg);
                        }
                        item.ValidationResult = "";
                        break;
                    case ValidationStep.CommittedValue:
                        if (bg.IsDirty)
                        {
                            if (item.SubtransactionId > 0)
                            {
                                item.Update();
                            }
                        }
                        break;
                }
            }

            return result;
        }
    }
}
