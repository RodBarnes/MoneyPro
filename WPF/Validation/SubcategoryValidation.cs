﻿using MoneyPro.Models;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MoneyPro.Validation
{
    public class SubcategoryValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var bg = (BindingGroup)value;
            var row = (DataGridRow)bg.Owner;
            if (row.Item is Subcategory item)
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
                        if (bg.IsDirty)
                        {
                            if (item.SubcategoryId > 0)
                            {
                                item.Update();
                            }
                        }
                        break;
                }
            }

            return ValidationResult.ValidResult;
        }
    }
}
